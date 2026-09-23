using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BKE.Launcher.AgentClient;
using BKE.Launcher.Application;
using BKE.Launcher.Infrastructure;
using BKE.Launcher.Presentation;

namespace BKE.Launcher.Desktop;

public sealed partial class App : Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var agentClient = new AgentLoopbackClient();
            var accountSession = new LauncherAccountSessionController(agentClient);
            var accountAuthClient = new NativeBkeAccountAuthClient();
            var nativeSignIn = new LauncherNativeSignInController(
                agentClient,
                accountAuthClient);
            var catalogSource = new AgentSoftwareCatalogSource(agentClient);
            var catalog = new LauncherCatalogService(catalogSource);
            var softwareInstall = new LauncherSoftwareInstallController(agentClient);
            var softwareOpen = new LauncherSoftwareOpenController(agentClient);
            var softwareRemove = new LauncherSoftwareRemoveController(agentClient);
            var viewModel = new MainWindowViewModel(
                accountSession,
                nativeSignIn,
                catalog,
                softwareInstall,
                softwareOpen,
                softwareRemove);

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };

            desktop.Exit += (_, _) =>
            {
                accountAuthClient.Dispose();
                agentClient.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
