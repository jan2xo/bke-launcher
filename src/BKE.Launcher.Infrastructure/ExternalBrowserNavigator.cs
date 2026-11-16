using System.Diagnostics;
using BKE.Launcher.Application;

namespace BKE.Launcher.Infrastructure;

public sealed class ExternalBrowserNavigator : ILauncherExternalNavigator
{
    private readonly LauncherPlatformAuthorityResolver _platformAuthority;

    public ExternalBrowserNavigator(
        LauncherPlatformAuthorityResolver platformAuthority)
    {
        _platformAuthority = platformAuthority;
    }

    public void OpenCheckout(string absoluteUrl)
    {
        if (!Uri.TryCreate(absoluteUrl, UriKind.Absolute, out var uri))
        {
            throw new InvalidDataException(
                "Secure checkout navigation target is invalid.");
        }

        ValidateHttpsUri(uri, requireOrigin: false);
        Open(uri);
    }

    public async Task OpenVerificationUriAsync(
        string absoluteUrl,
        CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(absoluteUrl, UriKind.Absolute, out var uri))
        {
            throw new InvalidDataException(
                "Device verification navigation target is invalid.");
        }

        var platformBaseUri =
            await _platformAuthority.ResolveAsync(cancellationToken);

        ValidateVerificationTarget(uri, platformBaseUri);
        Open(uri);
    }

    public void OpenVerificationUri(string absoluteUrl)
    {
        OpenVerificationUriAsync(absoluteUrl, CancellationToken.None).GetAwaiter().GetResult();
    }

    public static void ValidateVerificationTarget(
        Uri uri,
        Uri platformAuthority,
        string? environment = null)
    {
        ValidateHttpsUri(uri, requireOrigin: false, environment);

        if (!string.Equals(uri.Scheme, platformAuthority.Scheme, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(uri.Host, platformAuthority.Host, StringComparison.OrdinalIgnoreCase) ||
            uri.Port != platformAuthority.Port)
        {
            throw new InvalidDataException(
                "Device verification navigation target does not match the trusted Digital Solutions authority.");
        }

        if (!uri.AbsolutePath.Equals("/device", StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                "Device verification path must be /device.");
        }
    }

    public async Task OpenLegalDocumentAsync(
        string slug,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(slug) ||
            slug.Length > 256 ||
            slug.Any(character =>
                character is not (
                    >= 'a' and <= 'z' or
                    >= '0' and <= '9' or
                    '-')))
        {
            throw new ArgumentException(
                "Legal document slug is invalid.",
                nameof(slug));
        }

        var platformBaseUri =
            await _platformAuthority.ResolveAsync(cancellationToken);
        Open(new Uri(
            platformBaseUri,
            $"/legal/{Uri.EscapeDataString(slug)}"));
    }

    private static void ValidateHttpsUri(
        Uri uri,
        bool requireOrigin,
        string? environment = null)
    {
        var allowInsecureLoopback = string.Equals(
            Environment.GetEnvironmentVariable("BKE_AGENT_ALLOW_INSECURE_LOOPBACK"),
            "1",
            StringComparison.Ordinal);

        var configuredEnv = !string.IsNullOrWhiteSpace(environment)
            ? environment
            : Environment.GetEnvironmentVariable("BKE_ENVIRONMENT");

        var isApprovedNonprod =
            LauncherPlatformAuthorityResolver.IsApprovedNonproductionEnvironment(configuredEnv);

        var isLoopbackHttp = allowInsecureLoopback &&
            isApprovedNonprod &&
            uri.Scheme == Uri.UriSchemeHttp &&
            uri.IsLoopback;

        if (!uri.IsAbsoluteUri ||
            (!isLoopbackHttp && uri.Scheme != Uri.UriSchemeHttps) ||
            string.IsNullOrWhiteSpace(uri.Host) ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            !string.IsNullOrEmpty(uri.Fragment) ||
            (requireOrigin &&
             (!string.IsNullOrEmpty(uri.Query) ||
              uri.AbsolutePath != "/")))
        {
            throw new InvalidDataException(
                "BKE external navigation requires a trusted HTTPS target.");
        }
    }

    private static void Open(Uri uri)
    {
        Process.Start(new ProcessStartInfo(uri.AbsoluteUri)
        {
            UseShellExecute = true,
        });
    }
}
