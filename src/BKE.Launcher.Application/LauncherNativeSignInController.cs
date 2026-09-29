using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherNativeSignInController
{
    private readonly ILauncherAgentClient _agent;
    private readonly LauncherPlatformAuthorityResolver _platformAuthority;
    private readonly ILauncherIdentityClient _identity;

    public LauncherNativeSignInController(
        ILauncherAgentClient agent,
        LauncherPlatformAuthorityResolver platformAuthority,
        ILauncherIdentityClient identity)
    {
        _agent = agent;
        _platformAuthority = platformAuthority;
        _identity = identity;
    }

    public async Task<LauncherNativeSignInResult> SignInAsync(
        string email,
        string password,
        string? customerAccountId,
        CancellationToken cancellationToken)
    {
        var platformBaseAddress =
            await _platformAuthority.ResolveAsync(cancellationToken);
        var context = await GetDeviceContextAsync(cancellationToken);
        if (context is null)
        {
            return Failed(
                "AGENT_DEVICE_CONTEXT_UNAVAILABLE",
                "BKE Licensing Agent device context is unavailable.");
        }

        var login = await _identity.LoginAsync(
            platformBaseAddress,
            new NativeBkeLoginRequest(
                email.Trim(),
                password,
                NormalizeAccountId(customerAccountId),
                context.DeviceId!,
                context.DeviceName!,
                context.Platform!,
                context.Architecture!),
            cancellationToken);

        if (login.Status == "mfa_challenge_required")
        {
            if (string.IsNullOrWhiteSpace(login.ChallengeToken) ||
                string.IsNullOrWhiteSpace(login.MfaReference))
            {
                throw new InvalidDataException(
                    "Digital Solutions returned an invalid native MFA challenge.");
            }

            return new LauncherNativeSignInResult(
                Status: "MFA_CHALLENGE_REQUIRED",
                Accounts: Array.Empty<NativeBkeAccountChoice>(),
                Account: null,
                ErrorCode: null,
                ErrorMessage: null,
                ChallengeToken: login.ChallengeToken,
                ExpiresAt: login.ExpiresAt,
                EmailSent: login.EmailSent ?? false,
                MfaReference: login.MfaReference);
        }

        if (login.Status == "account_selection_required")
        {
            return AccountSelection(login.Accounts);
        }

        if (login.Status != "handoff_issued" ||
            string.IsNullOrWhiteSpace(login.HandoffCode))
        {
            return Failed(
                login.Error ?? "NATIVE_LOGIN_FAILED",
                NativeLoginMessage(login.Error));
        }

        return await CompleteHandoffAsync(login.HandoffCode, cancellationToken);
    }

    public async Task<LauncherNativeSignInResult> VerifyMfaAsync(
        string challengeToken,
        string code,
        string? customerAccountId,
        CancellationToken cancellationToken)
    {
        var platformBaseAddress =
            await _platformAuthority.ResolveAsync(cancellationToken);
        var context = await GetDeviceContextAsync(cancellationToken);
        if (context is null)
        {
            return Failed(
                "AGENT_DEVICE_CONTEXT_UNAVAILABLE",
                "BKE Licensing Agent device context is unavailable.");
        }

        var verified = await _identity.VerifyMfaAsync(
            platformBaseAddress,
            new NativeBkeMfaVerifyRequest(
                challengeToken,
                code,
                NormalizeAccountId(customerAccountId),
                context.DeviceId!,
                context.DeviceName!,
                context.Platform!,
                context.Architecture!),
            cancellationToken);

        if (verified.Status == "account_selection_required")
        {
            return AccountSelection(verified.Accounts);
        }

        if (verified.Status != "handoff_issued" ||
            string.IsNullOrWhiteSpace(verified.HandoffCode))
        {
            return Failed(
                verified.Error ?? "MFA_UNAVAILABLE",
                NativeLoginMessage(verified.Error));
        }

        return await CompleteHandoffAsync(
            verified.HandoffCode,
            cancellationToken);
    }

    private async Task<AccountSessionDeviceContextResponse?> GetDeviceContextAsync(
        CancellationToken cancellationToken)
    {
        var context = await _agent.GetAccountSessionDeviceContextAsync(
            new AccountSessionDeviceContextRequest(NewCorrelationId()),
            cancellationToken);

        return context.Status == "READY" &&
               !string.IsNullOrWhiteSpace(context.DeviceId) &&
               !string.IsNullOrWhiteSpace(context.DeviceName) &&
               !string.IsNullOrWhiteSpace(context.Platform) &&
               !string.IsNullOrWhiteSpace(context.Architecture)
            ? context
            : null;
    }

    private async Task<LauncherNativeSignInResult> CompleteHandoffAsync(
        string handoffCode,
        CancellationToken cancellationToken)
    {
        var completed = await _agent.CompleteAccountSessionAsync(
            new AccountSessionCompleteRequest(
                NewCorrelationId(),
                handoffCode),
            cancellationToken);

        return new LauncherNativeSignInResult(
            Status: completed.Status,
            Accounts: Array.Empty<NativeBkeAccountChoice>(),
            Account: completed.Account,
            ErrorCode: completed.Error?.Code,
            ErrorMessage: completed.Error?.Message);
    }

    private static LauncherNativeSignInResult AccountSelection(
        IReadOnlyList<NativeBkeAccountChoice>? accounts) =>
        new(
            Status: "ACCOUNT_SELECTION_REQUIRED",
            Accounts: accounts ?? Array.Empty<NativeBkeAccountChoice>(),
            Account: null,
            ErrorCode: null,
            ErrorMessage: null);

    private static LauncherNativeSignInResult Failed(
        string code,
        string message) =>
        new(
            Status: "FAILED",
            Accounts: Array.Empty<NativeBkeAccountChoice>(),
            Account: null,
            ErrorCode: code,
            ErrorMessage: message);

    private static string NativeLoginMessage(string? code) => code switch
    {
        "INVALID_CREDENTIALS" => "Email or password is incorrect.",
        "INVALID_MFA_CHALLENGE" => "This verification challenge is no longer valid. Sign in again.",
        "INVALID_MFA_CODE" => "The verification code is incorrect.",
        "MFA_UNAVAILABLE" => "BKE multi-factor verification is temporarily unavailable.",
        "EMAIL_NOT_VERIFIED" => "Verify your email before signing in to BKE.",
        "ACCOUNT_NOT_ACTIVE" => "This BKE account is not active.",
        "ACCOUNT_ROLE_FORBIDDEN" => "This account cannot be used for BKE software.",
        "RATE_LIMITED" => "Too many sign-in attempts. Try again later.",
        "AUTHENTICATION_UNAVAILABLE" => "BKE account authentication is temporarily unavailable.",
        "FORBIDDEN" => "This identity cannot sign in to the BKE customer Launcher.",
        _ => "BKE account sign-in failed.",
    };

    private static string? NormalizeAccountId(string? accountId) =>
        string.IsNullOrWhiteSpace(accountId) ? null : accountId.Trim();

    private static string NewCorrelationId() => Guid.NewGuid().ToString("N");
}
