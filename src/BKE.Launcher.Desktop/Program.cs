using Avalonia;

namespace BKE.Launcher.Desktop;

internal static class Program
{
    internal static bool IsUiSmoke { get; private set; }
    internal static string? ScreenshotDir { get; private set; }

    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length == 1 &&
            string.Equals(args[0], "--smoke", StringComparison.Ordinal))
        {
            return;
        }

        IsUiSmoke =
            args.Length == 1 &&
            string.Equals(args[0], "--ui-smoke", StringComparison.Ordinal);

        for (int i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], "--screenshot", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                ScreenshotDir = args[i + 1];
            }
        }

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(
            IsUiSmoke || ScreenshotDir != null ? [] : args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
