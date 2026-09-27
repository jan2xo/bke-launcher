using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherNativeSignInController
{
    private readonly ILauncherAgentClient _agent;
    private readonly ILauncherIdentityClient _identity;

    public LauncherNativeSignInController(
        ILauncherAgentClient agent,
        ILauncherIdentityClient identity)
    {
        _agent = agent;
        _identity = identity;
    }

    public async Task<LauncherNativeSignInResult> SignInAsync(
        string email,
        string password,
        string? customerAccountId,
        CancellationToken cancellationToken)
    {
        var authority = await _agent.GetPlatformAuthorityAsync(
            new PlatformAuthorityRequest(NewCorrelationId()),
            cancellationToken);

        if (authority.Status != "READY" ||
            string.IsNullOrWhiteSpace(authority.Environment) ||
            string.IsNullOrWhiteSpace(authority.PlatformBaseUrl) ||
            !TryParsePlatformAuthority(
                authority.PlatformBaseUrl,
                out var platformBaseAddress))
        {
            return new LauncherNativeSignInResult(
                "FAILED",
                Array.Empty<NativeBkeAccountChoice>(),
                null,
                authority.Error?.Code ?? "AGENT_PLATFORM_AUTHORITY_UNAVAILABLE",
                authority.Error?.Message ?? "BKE Licensing Agent platform authority is unavailable.");
        }

        var context = await _agent.GetAccountSessionDeviceContextAsync(
            new AccountSessionDeviceContextRequest(NewCorrelationId()),
            cancellationToken);

        if (context.Status != "READY" ||
            string.IsNullOrWhiteSpace(context.DeviceId) ||
            string.IsNullOrWhiteSpace(context.DeviceName) ||
            string.IsNullOrWhiteSpace(context.Platform) ||
            string.IsNullOrWhiteSpace(context.Architecture))
        {
            return new LauncherNativeSignInResult(
                "FAILED",
                Array.Empty<NativeBkeAccountChoice>(),
                null,
                context.Error?.Code ?? "AGENT_DEVICE_CONTEXT_UNAVAILABLE",
                context.Error?.Message ?? "BKE Licensing Agent device context is unavailable.");
        }

        var login = await _identity.LoginAsync(
            platformBaseAddress,
            new NativeBkeLoginRequest(
                email.Trim(),
                password,
                string.IsNullOrWhiteSpace(customerAccountId) ? null : customerAccountId.Trim(),
                context.DeviceId,
                context.DeviceName,
                context.Platform,
                context.Architecture),
            cancellationToken);

        if (login.Status == "account_selection_required")
        {
            return new LauncherNativeSignInResult(
                "ACCOUNT_SELECTION_REQUIRED",
                login.Accounts ?? Array.Empty<NativeBkeAccountChoice>(),
                null,
                null,
                null);
        }

        if (login.Status != "handoff_issued" ||
            string.IsNullOrWhiteSpace(login.HandoffCode))
        {
            return new LauncherNativeSignInResult(
                "FAILED",
                Array.Empty<NativeBkeAccountChoice>(),
                null,
                login.Error ?? "NATIVE_LOGIN_FAILED",
                NativeLoginMessage(login.Error));
        }

        var completed = await _agent.CompleteAccountSessionAsync(
            new AccountSessionCompleteRequest(
                NewCorrelationId(),
                login.HandoffCode),
            cancellationToken);

        return new LauncherNativeSignInResult(
            completed.Status,
            Array.Empty<NativeBkeAccountChoice>(),
            completed.Account,
            completed.Error?.Code,
            completed.Error?.Message);
    }

    private static bool TryParsePlatformAuthority(
        string value,
        out Uri platformBaseAddress)
    {
        if (Uri.TryCreate(value, UriKind.Absolute, out var resolved) &&
            resolved.Scheme == Uri.UriSchemeHttps &&
            !string.IsNullOrWhiteSpace(resolved.Host) &&
            string.IsNullOrEmpty(resolved.UserInfo) &&
            resolved.AbsolutePath == "/" &&
            string.IsNullOrEmpty(resolved.Query) &&
            string.IsNullOrEmpty(resolved.Fragment))
        {
            platformBaseAddress = resolved;
            return true;
        }

        platformBaseAddress = null!;
        return false;
    }

    private static string NativeLoginMessage(string? code) => code switch
    {
        "INVALID_CREDENTIALS" => "Email or password is incorrect.",
        "EMAIL_NOT_VERIFIED" => "Verify your email before signing in to BKE.",
        "ACCOUNT_NOT_ACTIVE" => "This BKE account is not active.",
        "ACCOUNT_ROLE_FORBIDDEN" => "This account cannot be used for BKE software.",
        "RATE_LIMITED" => "Too many sign-in attempts. Try again later.",
        "AUTHENTICATION_UNAVAILABLE" => "BKE account authentication is temporarily unavailable.",
        "FORBIDDEN" => "This identity cannot sign in to the BKE customer Launcher.",
        _ => "BKE account sign-in failed.",
    };

    private static string NewCorrelationId() => Guid.NewGuid().ToString("N");
}
