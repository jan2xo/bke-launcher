using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Infrastructure;

public sealed class PlatformIdentityClient :
    ILauncherIdentityClient,
    ILauncherRegistrationClient,
    IDisposable
{
    internal static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(20);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
    };

    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;

    public PlatformIdentityClient(HttpClient? httpClient = null)
    {
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

    public async Task<NativeBkeLoginResponse> LoginAsync(
        Uri platformBaseAddress,
        NativeBkeLoginRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePlatformBaseAddress(platformBaseAddress);
        using var response = await SendAsync(
            new Uri(platformBaseAddress, BkePlatformContract.NativeLoginPath),
            request,
            cancellationToken);

        if (IsRedirect(response.StatusCode))
        {
            return FailedLogin("PLATFORM_REDIRECT_REJECTED");
        }
        if (!response.IsSuccessStatusCode)
        {
            return FailedLogin(await ReadErrorAsync(response, cancellationToken));
        }

        EnsureNativeProtocol(response);
        var result = await response.Content.ReadFromJsonAsync<NativeBkeLoginResponse>(
            JsonOptions,
            cancellationToken);
        if (result is null ||
            result.Status is not ("account_selection_required" or "handoff_issued" or "mfa_challenge_required"))
        {
            throw new InvalidDataException(
                "Digital Solutions returned an invalid native sign-in response.");
        }

        if (result.Status == "handoff_issued")
        {
            ValidateHandoff(result.HandoffCode);
        }
        if (result.Status == "mfa_challenge_required" &&
            (string.IsNullOrWhiteSpace(result.ChallengeToken) ||
             result.ChallengeToken.Length is < 16 or > 512 ||
             string.IsNullOrWhiteSpace(result.ExpiresAt) ||
             string.IsNullOrWhiteSpace(result.MfaReference)))
        {
            throw new InvalidDataException(
                "Digital Solutions returned an invalid native MFA challenge.");
        }

        return result;
    }

    public async Task<NativeBkeMfaVerifyResponse> VerifyMfaAsync(
        Uri platformBaseAddress,
        NativeBkeMfaVerifyRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePlatformBaseAddress(platformBaseAddress);
        using var response = await SendAsync(
            new Uri(platformBaseAddress, BkePlatformContract.NativeMfaVerifyPath),
            request,
            cancellationToken);

        if (IsRedirect(response.StatusCode))
        {
            return FailedMfa("PLATFORM_REDIRECT_REJECTED");
        }
        if (!response.IsSuccessStatusCode)
        {
            return FailedMfa(await ReadErrorAsync(response, cancellationToken));
        }

        EnsureNativeProtocol(response);
        var result = await response.Content.ReadFromJsonAsync<NativeBkeMfaVerifyResponse>(
            JsonOptions,
            cancellationToken);
        if (result is null ||
            result.Status is not ("account_selection_required" or "handoff_issued"))
        {
            throw new InvalidDataException(
                "Digital Solutions returned an invalid native MFA verification response.");
        }
        if (result.Status == "handoff_issued")
        {
            ValidateHandoff(result.HandoffCode);
        }
        return result;
    }

    public async Task<NativeBkeRegistrationPreflightResponse> GetRegistrationPreflightAsync(
        Uri platformBaseAddress,
        CancellationToken cancellationToken)
    {
        ValidatePlatformBaseAddress(platformBaseAddress);
        using var response = await SendGetAsync(
            new Uri(platformBaseAddress, BkePlatformContract.NativeRegistrationPreflightPath),
            cancellationToken);

        if (IsRedirect(response.StatusCode))
        {
            return FailedRegistrationPreflight("PLATFORM_REDIRECT_REJECTED");
        }
        if (!response.IsSuccessStatusCode)
        {
            return FailedRegistrationPreflight(
                await ReadErrorAsync(response, cancellationToken));
        }

        EnsureNativeProtocol(response);
        var result = await response.Content.ReadFromJsonAsync<NativeBkeRegistrationPreflightResponse>(
            JsonOptions,
            cancellationToken);
        if (result is null ||
            result.Status != "ready" ||
            result.Error is not null ||
            result.LegalDocuments.Count != 2 ||
            result.LegalDocuments.Select(document => document.DocumentType)
                .ToHashSet(StringComparer.Ordinal)
                .SetEquals(["TERMS_OF_SERVICE", "PRIVACY_POLICY"]) is false ||
            result.LegalDocuments.Any(document =>
                string.IsNullOrWhiteSpace(document.Title) ||
                string.IsNullOrWhiteSpace(document.Slug) ||
                string.IsNullOrWhiteSpace(document.VersionId) ||
                document.VersionNumber < 1 ||
                string.IsNullOrWhiteSpace(document.ContentMarkdown)))
        {
            throw new InvalidDataException(
                "Digital Solutions returned an invalid native registration preflight response.");
        }

        return result;
    }

    public async Task<NativeBkeRegistrationResponse> RegisterAsync(
        Uri platformBaseAddress,
        NativeBkeRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePlatformBaseAddress(platformBaseAddress);
        using var response = await SendAsync(
            new Uri(platformBaseAddress, BkePlatformContract.NativeRegistrationPath),
            request,
            cancellationToken);

        if (IsRedirect(response.StatusCode))
        {
            return FailedRegistration("PLATFORM_REDIRECT_REJECTED");
        }
        if (!response.IsSuccessStatusCode)
        {
            return FailedRegistration(await ReadErrorAsync(response, cancellationToken));
        }

        EnsureNativeProtocol(response);
        var result = await response.Content.ReadFromJsonAsync<NativeBkeRegistrationResponse>(
            JsonOptions,
            cancellationToken);
        if (result is null ||
            result.Status != "verification_required" ||
            result.Error is not null)
        {
            throw new InvalidDataException(
                "Digital Solutions returned an invalid native registration response.");
        }

        return result;
    }

    public async Task<NativeBkeEmailVerificationResponse> VerifyEmailAsync(
        Uri platformBaseAddress,
        NativeBkeEmailVerificationRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePlatformBaseAddress(platformBaseAddress);
        using var response = await SendAsync(
            new Uri(platformBaseAddress, BkePlatformContract.NativeEmailVerifyPath),
            request,
            cancellationToken);

        if (IsRedirect(response.StatusCode))
        {
            return FailedEmailVerification("PLATFORM_REDIRECT_REJECTED");
        }
        if (!response.IsSuccessStatusCode)
        {
            return FailedEmailVerification(
                await ReadErrorAsync(response, cancellationToken));
        }

        EnsureNativeProtocol(response);
        var result = await response.Content.ReadFromJsonAsync<NativeBkeEmailVerificationResponse>(
            JsonOptions,
            cancellationToken);
        if (result is null ||
            result.Status != "verified" ||
            result.Error is not null)
        {
            throw new InvalidDataException(
                "Digital Solutions returned an invalid native email-verification response.");
        }

        return result;
    }

    public async Task<NativeBkeVerificationResendResponse> ResendVerificationAsync(
        Uri platformBaseAddress,
        NativeBkeVerificationResendRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePlatformBaseAddress(platformBaseAddress);
        using var response = await SendAsync(
            new Uri(platformBaseAddress, BkePlatformContract.NativeVerificationResendPath),
            request,
            cancellationToken);

        if (IsRedirect(response.StatusCode))
        {
            return FailedVerificationResend("PLATFORM_REDIRECT_REJECTED");
        }
        if (!response.IsSuccessStatusCode)
        {
            return FailedVerificationResend(
                await ReadErrorAsync(response, cancellationToken));
        }

        EnsureNativeProtocol(response);
        var result = await response.Content.ReadFromJsonAsync<NativeBkeVerificationResendResponse>(
            JsonOptions,
            cancellationToken);
        if (result is null ||
            result.Status != "accepted" ||
            result.Error is not null)
        {
            throw new InvalidDataException(
                "Digital Solutions returned an invalid native verification-resend response.");
        }

        return result;
    }

    public async Task<NativeBkePasswordResetResponse> RequestPasswordResetAsync(
        Uri platformBaseAddress,
        NativeBkePasswordResetRequest request,
        CancellationToken cancellationToken)
    {
        ValidatePlatformBaseAddress(platformBaseAddress);
        using var response = await SendAsync(
            new Uri(platformBaseAddress, BkePlatformContract.NativePasswordResetRequestPath),
            request,
            cancellationToken);

        if (IsRedirect(response.StatusCode))
        {
            return FailedReset("PLATFORM_REDIRECT_REJECTED");
        }
        if (!response.IsSuccessStatusCode)
        {
            return FailedReset(await ReadErrorAsync(response, cancellationToken));
        }

        EnsureNativeProtocol(response);
        var result =
            await response.Content.ReadFromJsonAsync<NativeBkePasswordResetResponse>(
                JsonOptions,
                cancellationToken);
        if (result is null ||
            result.Status != "accepted" ||
            result.Error is not null)
        {
            throw new InvalidDataException(
                "Digital Solutions returned an invalid native password-reset response.");
        }

        return result;
    }

    private async Task<HttpResponseMessage> SendGetAsync(
        Uri endpoint,
        CancellationToken cancellationToken)
    {
        using var timeoutSource =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(DefaultRequestTimeout);

        using var message = new HttpRequestMessage(HttpMethod.Get, endpoint);
        AddNativeHeaders(message);

        return await _http.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            timeoutSource.Token);
    }

    private async Task<HttpResponseMessage> SendAsync<TRequest>(
        Uri endpoint,
        TRequest request,
        CancellationToken cancellationToken)
    {
        using var timeoutSource =
            CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutSource.CancelAfter(DefaultRequestTimeout);

        using var message = new HttpRequestMessage(HttpMethod.Post, endpoint);
        AddNativeHeaders(message);
        message.Content = JsonContent.Create(request, options: JsonOptions);

        return await _http.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            timeoutSource.Token);
    }

    private static void AddNativeHeaders(HttpRequestMessage message)
    {
        message.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        message.Headers.UserAgent.ParseAdd("bke-launcher");
        message.Headers.TryAddWithoutValidation(
            "x-bke-account-session-version",
            BkePlatformContract.AccountSessionProtocolVersion);
        message.Headers.TryAddWithoutValidation(
            "x-request-id",
            Guid.NewGuid().ToString("N"));
    }

    private static void ValidateHandoff(string? handoffCode)
    {
        if (string.IsNullOrWhiteSpace(handoffCode) ||
            handoffCode.Length is < 32 or > 256)
        {
            throw new InvalidDataException(
                "Digital Solutions returned an invalid native handoff.");
        }
    }

    private static void EnsureNativeProtocol(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues(
                "x-bke-account-session-version",
                out var values))
        {
            throw new InvalidDataException(
                "Digital Solutions native response is missing its protocol version.");
        }

        var versions = values.ToArray();
        if (versions.Length != 1 ||
            versions[0] != BkePlatformContract.AccountSessionProtocolVersion)
        {
            throw new InvalidDataException(
                "Digital Solutions native protocol version drifted.");
        }
    }

    private static NativeBkeLoginResponse FailedLogin(string error) =>
        new("failed", Error: error);

    private static NativeBkeMfaVerifyResponse FailedMfa(string error) =>
        new("failed", Error: error);

    private static NativeBkePasswordResetResponse FailedReset(string error) =>
        new("failed", error);


    private static NativeBkeRegistrationPreflightResponse FailedRegistrationPreflight(
        string error) =>
        new("failed", Array.Empty<NativeBkeRegistrationLegalDocument>(), error);

    private static NativeBkeRegistrationResponse FailedRegistration(string error) =>
        new("failed", error);

    private static NativeBkeEmailVerificationResponse FailedEmailVerification(
        string error) =>
        new("failed", error);

    private static NativeBkeVerificationResendResponse FailedVerificationResend(
        string error) =>
        new("failed", error);

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

    private static void ValidatePlatformBaseAddress(Uri value)
    {
        if (!value.IsAbsoluteUri ||
            value.Scheme != Uri.UriSchemeHttps ||
            string.IsNullOrWhiteSpace(value.Host) ||
            !string.IsNullOrEmpty(value.UserInfo) ||
            value.AbsolutePath != "/" ||
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
