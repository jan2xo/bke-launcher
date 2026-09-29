using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed record LauncherRegistrationPreflightResult(
    string Status,
    IReadOnlyList<NativeBkeRegistrationLegalDocument> LegalDocuments,
    string? ErrorCode,
    string? ErrorMessage);

public sealed record LauncherRegistrationActionResult(
    string Status,
    string? ErrorCode,
    string? ErrorMessage);

public sealed class LauncherNativeRegistrationController
{
    private readonly LauncherPlatformAuthorityResolver _platformAuthority;
    private readonly ILauncherRegistrationClient _registration;

    public LauncherNativeRegistrationController(
        LauncherPlatformAuthorityResolver platformAuthority,
        ILauncherRegistrationClient registration)
    {
        _platformAuthority = platformAuthority;
        _registration = registration;
    }

    public async Task<LauncherRegistrationPreflightResult> LoadAsync(
        CancellationToken cancellationToken)
    {
        var authority = await _platformAuthority.ResolveAsync(cancellationToken);
        var result = await _registration.GetRegistrationPreflightAsync(
            authority,
            cancellationToken);

        return result.Status == "ready"
            ? new(
                "READY",
                result.LegalDocuments,
                null,
                null)
            : new(
                "FAILED",
                Array.Empty<NativeBkeRegistrationLegalDocument>(),
                result.Error ?? "REGISTRATION_UNAVAILABLE",
                RegistrationMessage(result.Error));
    }

    public async Task<LauncherRegistrationActionResult> RegisterAsync(
        string email,
        string name,
        string password,
        IReadOnlyList<string> legalVersionIds,
        CancellationToken cancellationToken)
    {
        if (legalVersionIds.Count != 2 ||
            legalVersionIds.Any(string.IsNullOrWhiteSpace) ||
            legalVersionIds.Distinct(StringComparer.Ordinal).Count() != 2)
        {
            return new(
                "INVALID_INPUT",
                "LEGAL_ACCEPTANCE_REQUIRED",
                "Accept the current Terms of Service and Privacy Policy before creating your BKE account.");
        }

        var authority = await _platformAuthority.ResolveAsync(cancellationToken);
        var result = await _registration.RegisterAsync(
            authority,
            new NativeBkeRegistrationRequest(
                email.Trim(),
                name.Trim(),
                password,
                legalVersionIds),
            cancellationToken);

        return result.Status == "verification_required"
            ? new(
                "VERIFICATION_REQUIRED",
                null,
                "Account created. Enter the verification code sent to your email.")
            : new(
                "FAILED",
                result.Error ?? "REGISTRATION_FAILED",
                RegistrationMessage(result.Error));
    }

    public async Task<LauncherRegistrationActionResult> VerifyEmailAsync(
        string email,
        string code,
        CancellationToken cancellationToken)
    {
        var authority = await _platformAuthority.ResolveAsync(cancellationToken);
        var result = await _registration.VerifyEmailAsync(
            authority,
            new NativeBkeEmailVerificationRequest(
                email.Trim(),
                code.Trim().ToUpperInvariant()),
            cancellationToken);

        return result.Status == "verified"
            ? new(
                "VERIFIED",
                null,
                "Email verified. Sign in to finish connecting BKE on this machine.")
            : new(
                "FAILED",
                result.Error ?? "INVALID_VERIFICATION_CODE",
                RegistrationMessage(result.Error));
    }

    public async Task<LauncherRegistrationActionResult> ResendAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var authority = await _platformAuthority.ResolveAsync(cancellationToken);
        var result = await _registration.ResendVerificationAsync(
            authority,
            new NativeBkeVerificationResendRequest(email.Trim()),
            cancellationToken);

        return result.Status == "accepted"
            ? new(
                "ACCEPTED",
                null,
                "If this account still needs verification, a new code will be sent.")
            : new(
                "FAILED",
                result.Error ?? "VERIFICATION_UNAVAILABLE",
                RegistrationMessage(result.Error));
    }

    private static string RegistrationMessage(string? code) => code switch
    {
        "ACCOUNT_EXISTS" => "A BKE account already exists for this email. Sign in instead.",
        "LEGAL_ACCEPTANCE_REQUIRED" => "The current Terms of Service and Privacy Policy must be accepted.",
        "REGISTRATION_UNAVAILABLE" => "BKE account registration is temporarily unavailable.",
        "INVALID_VERIFICATION_CODE" => "The verification code is invalid or expired.",
        "RATE_LIMITED" => "Too many requests. Try again later.",
        "PLATFORM_REDIRECT_REJECTED" => "BKE account registration returned an unexpected redirect.",
        _ => "BKE account registration is temporarily unavailable.",
    };
}
