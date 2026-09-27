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
