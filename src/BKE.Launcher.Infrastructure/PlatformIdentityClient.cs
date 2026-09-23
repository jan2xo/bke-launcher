using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Infrastructure;

public sealed class PlatformIdentityClient : ILauncherIdentityClient, IDisposable
{
    internal static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(20);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
    };

    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;

    public PlatformIdentityClient(HttpClient? httpClient = null, Uri? baseAddress = null)
    {
        var configured = Environment.GetEnvironmentVariable("BKE_PLATFORM_BASE_URL")?.Trim();
        var resolved = baseAddress
            ?? new Uri(
                string.IsNullOrWhiteSpace(configured)
                    ? BkePlatformContract.DefaultBaseAddress
                    : configured,
                UriKind.Absolute);
        ValidatePlatformBaseAddress(resolved);

        if (httpClient is null)
        {
            _http = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
            })
            {
                BaseAddress = resolved,
                Timeout = Timeout.InfiniteTimeSpan,
            };
            _ownsHttpClient = true;
        }
        else
        {
            _http = httpClient;
            _http.BaseAddress = resolved;
            _http.Timeout = Timeout.InfiniteTimeSpan;
            _ownsHttpClient = false;
        }
    }

    public async Task<NativeBkeLoginResponse> LoginAsync(
        NativeBkeLoginRequest request,
        CancellationToken cancellationToken)
    {
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(DefaultRequestTimeout);

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            BkePlatformContract.NativeLoginPath);
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        message.Headers.UserAgent.ParseAdd("bke-launcher");
        message.Headers.TryAddWithoutValidation(
            "x-bke-account-session-version",
            BkePlatformContract.AccountSessionProtocolVersion);
        message.Headers.TryAddWithoutValidation("x-request-id", Guid.NewGuid().ToString("N"));
        message.Content = JsonContent.Create(request, options: JsonOptions);

        using var response = await _http.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            timeoutSource.Token);

        if (IsRedirect(response.StatusCode))
        {
            return Failed("PLATFORM_REDIRECT_REJECTED");
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await ReadErrorAsync(response, timeoutSource.Token);
            return Failed(error);
        }

        var result = await response.Content.ReadFromJsonAsync<NativeBkeLoginResponse>(
            JsonOptions,
            timeoutSource.Token);
        if (result is null ||
            result.Status is not ("account_selection_required" or "handoff_issued"))
        {
            throw new InvalidDataException("Digital Solutions returned an invalid native sign-in response.");
        }

        if (result.Status == "handoff_issued" &&
            (string.IsNullOrWhiteSpace(result.HandoffCode) ||
             result.HandoffCode.Length is < 32 or > 256))
        {
            throw new InvalidDataException("Digital Solutions returned an invalid native handoff.");
        }

        return result;
    }

    private static async Task<string> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            using var document = await JsonDocument.ParseAsync(
                await response.Content.ReadAsStreamAsync(cancellationToken),
                cancellationToken: cancellationToken);
            if (document.RootElement.TryGetProperty("error", out var property) &&
                property.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(property.GetString()))
            {
                return property.GetString()!;
            }
        }
        catch (JsonException)
        {
        }

        return response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "INVALID_CREDENTIALS",
            HttpStatusCode.Forbidden => "FORBIDDEN",
            (HttpStatusCode)429 => "RATE_LIMITED",
            HttpStatusCode.ServiceUnavailable => "AUTHENTICATION_UNAVAILABLE",
            _ => "NATIVE_LOGIN_FAILED",
        };
    }

    private static NativeBkeLoginResponse Failed(string error) =>
        new("failed", null, null, null, null, error);

    private static void ValidatePlatformBaseAddress(Uri value)
    {
        if (!value.IsAbsoluteUri ||
            value.Scheme != Uri.UriSchemeHttps ||
            string.IsNullOrWhiteSpace(value.Host) ||
            !string.IsNullOrEmpty(value.Query) ||
            !string.IsNullOrEmpty(value.Fragment))
        {
            throw new ArgumentException(
                "BKE platform authority must be an absolute HTTPS origin.",
                nameof(value));
        }
    }

    private static bool IsRedirect(HttpStatusCode statusCode) =>
        (int)statusCode is >= 300 and <= 399;

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _http.Dispose();
        }
    }
}
