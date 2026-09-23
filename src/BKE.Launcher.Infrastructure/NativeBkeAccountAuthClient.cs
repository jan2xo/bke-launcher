using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Infrastructure;

public sealed class NativeBkeAccountAuthClient : ILauncherAccountAuthClient, IDisposable
{
    internal static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(20);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
    };

    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;
    private readonly string _platformBaseUrl;

    public NativeBkeAccountAuthClient(
        HttpClient? httpClient = null,
        Uri? baseAddress = null)
    {
        var configured = baseAddress?.ToString()
            ?? Environment.GetEnvironmentVariable("BKE_PLATFORM_BASE_URL")
            ?? "https://jl-bke.com";
        _platformBaseUrl = configured.TrimEnd('/');
        ValidatePlatformBaseUrl(_platformBaseUrl);

        if (httpClient is null)
        {
            _http = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
            })
            {
                Timeout = Timeout.InfiniteTimeSpan,
            };
            _ownsHttpClient = true;
        }
        else
        {
            _http = httpClient;
            _http.Timeout = Timeout.InfiniteTimeSpan;
            _ownsHttpClient = false;
        }
    }

    public async Task<NativeAccountLoginResponse> AuthenticateAsync(
        NativeAccountLoginRequest request,
        CancellationToken cancellationToken)
    {
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);
        timeoutSource.CancelAfter(DefaultRequestTimeout);

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            $"{_platformBaseUrl}{BkeAccountAuthContract.NativeLoginPath}");
        message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        message.Headers.UserAgent.ParseAdd("bke-launcher");
        message.Headers.TryAddWithoutValidation(
            "x-bke-account-session-version",
            BkeAccountAuthContract.ProtocolVersion);
        message.Headers.TryAddWithoutValidation(
            "x-request-id",
            Guid.NewGuid().ToString("N"));
        message.Content = JsonContent.Create(
            request,
            options: JsonOptions);

        using var response = await _http.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            timeoutSource.Token);

        if (IsRedirect(response.StatusCode))
        {
            throw new HttpRequestException(
                "Digital Solutions native BKE authentication endpoint redirected.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await ReadErrorAsync(response, timeoutSource.Token);
            return new NativeAccountLoginResponse(
                "failed",
                null,
                null,
                null,
                null,
                error);
        }

        RequireProtocolHeader(response);
        var result = await response.Content.ReadFromJsonAsync<NativeAccountLoginResponse>(
            JsonOptions,
            timeoutSource.Token)
            ?? throw new InvalidDataException(
                "Digital Solutions returned an empty native BKE authentication response.");

        ValidateSuccess(result);
        return result;
    }

    private static void ValidateSuccess(NativeAccountLoginResponse response)
    {
        if (response.Status == "account_selection_required")
        {
            if (response.Accounts is not { Count: > 0 } ||
                response.Accounts.Any(account =>
                    string.IsNullOrWhiteSpace(account.AccountId) ||
                    string.IsNullOrWhiteSpace(account.DisplayName) ||
                    account.AccountType is not ("INDIVIDUAL" or "ORGANIZATION")))
            {
                throw new InvalidDataException(
                    "Digital Solutions returned invalid native BKE account choices.");
            }
            return;
        }

        if (response.Status == "handoff_issued")
        {
            if (string.IsNullOrWhiteSpace(response.HandoffCode) ||
                response.HandoffCode.Length is < 32 or > 256 ||
                response.HandoffCode.Any(character =>
                    character is not (
                        >= 'A' and <= 'Z' or
                        >= 'a' and <= 'z' or
                        >= '0' and <= '9' or
                        '-' or '_')) ||
                response.ExpiresIn is not (> 0 and <= 300) ||
                response.Account is null ||
                string.IsNullOrWhiteSpace(response.Account.AccountId) ||
                string.IsNullOrWhiteSpace(response.Account.DisplayName) ||
                response.Account.AccountType is not ("INDIVIDUAL" or "ORGANIZATION"))
            {
                throw new InvalidDataException(
                    "Digital Solutions returned an invalid native BKE handoff.");
            }
            return;
        }

        throw new InvalidDataException(
            "Digital Solutions returned an unknown native BKE authentication status.");
    }

    private static async Task<string> ReadErrorAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var document = await JsonDocument.ParseAsync(
                stream,
                cancellationToken: cancellationToken);
            if (document.RootElement.TryGetProperty("error", out var error) &&
                error.ValueKind == JsonValueKind.String &&
                !string.IsNullOrWhiteSpace(error.GetString()))
            {
                return error.GetString()!;
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
            _ => "AUTHENTICATION_FAILED",
        };
    }

    private static void RequireProtocolHeader(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues(
                "x-bke-account-session-version",
                out var values) ||
            values.SingleOrDefault() != BkeAccountAuthContract.ProtocolVersion)
        {
            throw new InvalidDataException(
                "Digital Solutions native BKE authentication protocol drifted.");
        }
    }

    private static void ValidatePlatformBaseUrl(string value)
    {
        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) ||
            !string.IsNullOrEmpty(uri.Query) ||
            !string.IsNullOrEmpty(uri.Fragment))
        {
            throw new ArgumentException(
                "BKE_PLATFORM_BASE_URL must be an absolute HTTP(S) URL without query or fragment.");
        }

        if (uri.Scheme == Uri.UriSchemeHttps)
        {
            return;
        }

        var allowLocal =
            Environment.GetEnvironmentVariable("BKE_LAUNCHER_ALLOW_INSECURE_LOCAL") == "1";
        if (!allowLocal || !uri.IsLoopback)
        {
            throw new ArgumentException(
                "HTTPS is required for BKE account authentication outside explicit loopback certification.");
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
