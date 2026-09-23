using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherNativeSignInController
{
    private readonly ILauncherAgentClient _agent;
    private readonly ILauncherAccountAuthClient _auth;

    public LauncherNativeSignInController(
        ILauncherAgentClient agent,
        ILauncherAccountAuthClient auth)
    {
        _agent = agent;
        _auth = auth;
    }

    public async Task<LauncherNativeSignInResult> SignInAsync(
        string email,
        string password,
        string? accountId,
        CancellationToken cancellationToken)
    {
        var context = await _agent.GetNativeAccountSessionContextAsync(
            new AccountSessionNativeContextRequest(NewCorrelationId()),
            cancellationToken);

        if (context.Status != "READY" ||
            string.IsNullOrWhiteSpace(context.DeviceId) ||
            string.IsNullOrWhiteSpace(context.DeviceName) ||
            string.IsNullOrWhiteSpace(context.Platform) ||
            string.IsNullOrWhiteSpace(context.Architecture))
        {
            return Failure(
                context.Error?.Code ?? "AGENT_CONTEXT_UNAVAILABLE",
                context.Error?.Message ?? "BKE Licensing Agent native sign-in context is unavailable.");
        }

        var authentication = await _auth.AuthenticateAsync(
            new NativeAccountLoginRequest(
                email,
                password,
                string.IsNullOrWhiteSpace(accountId) ? null : accountId,
                context.DeviceId,
                context.DeviceName,
                context.Platform,
                context.Architecture),
            cancellationToken);

        if (authentication.Status == "account_selection_required")
        {
            var accounts = (authentication.Accounts ?? Array.Empty<NativeAccountSummary>())
                .Select(account => new LauncherNativeAccountChoice(
                    account.AccountId,
                    account.AccountType,
                    account.DisplayName))
                .ToArray();
            if (accounts.Length == 0)
            {
                return Failure(
                    "ACCOUNT_SELECTION_INVALID",
                    "Digital Solutions requested account selection without any available accounts.");
            }

            return new LauncherNativeSignInResult(
                "ACCOUNT_SELECTION_REQUIRED",
                null,
                accounts,
                null,
                "Choose the account to use on this machine.");
        }

        if (authentication.Status != "handoff_issued" ||
            string.IsNullOrWhiteSpace(authentication.HandoffCode))
        {
            return Failure(
                authentication.Error ?? "AUTHENTICATION_FAILED",
                AuthenticationMessage(authentication.Error));
        }

        var completed = await _agent.CompleteNativeAccountSessionAsync(
            new AccountSessionNativeCompleteRequest(
                NewCorrelationId(),
                authentication.HandoffCode),
            cancellationToken);

        if (completed.Status == "AUTHENTICATED" && completed.Account is not null)
        {
            return new LauncherNativeSignInResult(
                "AUTHENTICATED",
                completed.Account,
                Array.Empty<LauncherNativeAccountChoice>(),
                null,
                "Signed in with BKE on this machine.");
        }

        return Failure(
            completed.Error?.Code ?? completed.Status,
            completed.Error?.Message ?? "BKE Licensing Agent could not complete sign-in.");
    }

    private static LauncherNativeSignInResult Failure(string code, string message) =>
        new(
            "FAILED",
            null,
            Array.Empty<LauncherNativeAccountChoice>(),
            code,
            message);

    private static string AuthenticationMessage(string? code) => code switch
    {
        "INVALID_CREDENTIALS" => "Email or password is incorrect.",
        "EMAIL_NOT_VERIFIED" => "Verify your email before signing in with BKE.",
        "ACCOUNT_NOT_ACTIVE" => "This BKE account is not active.",
        "RATE_LIMITED" => "Too many sign-in attempts. Try again later.",
        "FORBIDDEN" => "This account cannot use native BKE sign-in.",
        "AUTHENTICATION_UNAVAILABLE" => "BKE account authentication is temporarily unavailable.",
        _ => "BKE account sign-in failed.",
    };

    private static string NewCorrelationId() => Guid.NewGuid().ToString("N");
}
