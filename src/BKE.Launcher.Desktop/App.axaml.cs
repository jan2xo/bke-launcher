using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using BKE.Launcher.AgentClient;
using BKE.Launcher.Application;
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
            var viewModel = new MainWindowViewModel(accountSession);

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };

            desktop.Exit += (_, _) => agentClient.Dispose();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
