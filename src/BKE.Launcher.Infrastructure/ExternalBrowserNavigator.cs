using System.Diagnostics;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Infrastructure;

public sealed class ExternalBrowserNavigator : ILauncherExternalNavigator
{
    private readonly Uri _platformBaseUri;

    public ExternalBrowserNavigator(Uri? platformBaseUri = null)
    {
        var configured = Environment.GetEnvironmentVariable("BKE_PLATFORM_BASE_URL")?.Trim();
        _platformBaseUri = platformBaseUri
            ?? new Uri(
                string.IsNullOrWhiteSpace(configured)
                    ? BkePlatformContract.DefaultBaseAddress
                    : configured,
                UriKind.Absolute);

        ValidateHttpsUri(_platformBaseUri, requireOrigin: true);
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

    public void OpenLegalDocument(string slug)
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

        Open(new Uri(_platformBaseUri, $"/legal/{Uri.EscapeDataString(slug)}"));
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
