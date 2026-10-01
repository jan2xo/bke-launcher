using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherPasswordResetCompletionController
{
    private const string ResetPagePath = "/reset-password";

    private readonly LauncherPlatformAuthorityResolver _platformAuthority;
    private readonly ILauncherIdentityClient _identity;

    public LauncherPasswordResetCompletionController(
        LauncherPlatformAuthorityResolver platformAuthority,
        ILauncherIdentityClient identity)
    {
        _platformAuthority = platformAuthority;
        _identity = identity;
    }

    public async Task<NativeBkePasswordResetCompletionResponse> CompleteAsync(
        string resetProof,
        string newPassword,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(resetProof) ||
            resetProof != resetProof.Trim() ||
            string.IsNullOrEmpty(newPassword) ||
            newPassword.Length > 128)
        {
            return InvalidInput();
        }

        var platformBaseAddress =
            await _platformAuthority.ResolveAsync(cancellationToken);

        if (!TryExtractToken(
                platformBaseAddress,
                resetProof,
                out var token))
        {
            return new NativeBkePasswordResetCompletionResponse(
                "failed",
                "INVALID_TOKEN");
        }

        return await _identity.CompletePasswordResetAsync(
            platformBaseAddress,
            new NativeBkePasswordResetCompletionRequest(
                token,
                newPassword),
            cancellationToken);
    }

    internal static bool TryExtractToken(
        Uri platformBaseAddress,
        string resetProof,
        out string token)
    {
        token = string.Empty;

        if (string.IsNullOrEmpty(resetProof) ||
            resetProof != resetProof.Trim())
        {
            return false;
        }

        if (!Uri.TryCreate(
                resetProof,
                UriKind.Absolute,
                out var resetUri))
        {
            return ValidToken(resetProof, out token);
        }

        if (resetUri.Scheme != Uri.UriSchemeHttps ||
            !string.Equals(
                resetUri.Host,
                platformBaseAddress.Host,
                StringComparison.OrdinalIgnoreCase) ||
            resetUri.Port != platformBaseAddress.Port ||
            !string.IsNullOrEmpty(resetUri.UserInfo) ||
            !string.Equals(
                resetUri.AbsolutePath,
                ResetPagePath,
                StringComparison.Ordinal) ||
            !string.IsNullOrEmpty(resetUri.Fragment) ||
            !resetUri.Query.StartsWith(
                "?token=",
                StringComparison.Ordinal) ||
            resetUri.Query.Length <= "?token=".Length ||
            resetUri.Query.Contains(
                '&',
                StringComparison.Ordinal))
        {
            return false;
        }

        try
        {
            var decoded = Uri.UnescapeDataString(
                resetUri.Query["?token=".Length..]);
            return ValidToken(decoded, out token);
        }
        catch (UriFormatException)
        {
            return false;
        }
    }

    private static bool ValidToken(
        string value,
        out string token)
    {
        token = string.Empty;

        if (value.Length is < 20 or > 2048 ||
            value.Any(character =>
                char.IsWhiteSpace(character) ||
                char.IsControl(character)))
        {
            return false;
        }

        token = value;
        return true;
    }

    private static NativeBkePasswordResetCompletionResponse InvalidInput() =>
        new(
            "failed",
            "INVALID_INPUT");
}
