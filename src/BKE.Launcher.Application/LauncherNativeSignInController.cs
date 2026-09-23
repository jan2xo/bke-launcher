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
