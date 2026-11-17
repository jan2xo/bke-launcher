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

    public async Task OpenAccountManagementAsync(CancellationToken cancellationToken)
    {
        var platformBaseUri =
            await _platformAuthority.ResolveAsync(cancellationToken);
        Open(new Uri(platformBaseUri, "/account"));
    }

    public void OpenAccountManagement()
    {
        OpenAccountManagementAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public static void ValidateVerificationTarget(
        Uri uri,
        Uri platformAuthority)
    {
        ValidateVerificationUri(uri, platformAuthority);

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

    private static void ValidateVerificationUri(Uri uri, Uri platformAuthority)
    {
        if (!uri.IsAbsoluteUri ||
            string.IsNullOrWhiteSpace(uri.Host) ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            !string.IsNullOrEmpty(uri.Fragment))
        {
            throw new InvalidDataException(
                "BKE external navigation requires a valid absolute target.");
        }

        var isLoopbackHttp =
            platformAuthority.Scheme == Uri.UriSchemeHttp &&
            platformAuthority.IsLoopback &&
            uri.Scheme == Uri.UriSchemeHttp &&
            uri.IsLoopback;

        if (!isLoopbackHttp && uri.Scheme != Uri.UriSchemeHttps)
        {
            throw new InvalidDataException(
                "BKE external navigation requires a trusted HTTPS target.");
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

    private static void ValidateHttpsUri(Uri uri, bool requireOrigin)
    {
        if (!uri.IsAbsoluteUri ||
            uri.Scheme != Uri.UriSchemeHttps ||
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
