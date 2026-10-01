using Avalonia;

namespace BKE.Launcher.Desktop;

internal static class Program
{
    internal static bool IsUiSmoke { get; private set; }

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

        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace();
}
