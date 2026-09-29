using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherPasswordResetRequestController
{
    private readonly LauncherPlatformAuthorityResolver _platformAuthority;
    private readonly ILauncherIdentityClient _identity;

    public LauncherPasswordResetRequestController(
        LauncherPlatformAuthorityResolver platformAuthority,
        ILauncherIdentityClient identity)
    {
        _platformAuthority = platformAuthority;
        _identity = identity;
    }

    public async Task<NativeBkePasswordResetResponse> RequestAsync(
        string email,
        CancellationToken cancellationToken)
    {
        var normalizedEmail = email.Trim();
        if (string.IsNullOrWhiteSpace(normalizedEmail) ||
            normalizedEmail.Length > 254)
        {
            return new NativeBkePasswordResetResponse(
                "failed",
                "INVALID_INPUT");
        }

        var platformBaseAddress =
            await _platformAuthority.ResolveAsync(cancellationToken);

        return await _identity.RequestPasswordResetAsync(
            platformBaseAddress,
            new NativeBkePasswordResetRequest(normalizedEmail),
            cancellationToken);
    }
}
