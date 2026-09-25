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
            var identityClient = new PlatformIdentityClient();
            var nativeSignIn = new LauncherNativeSignInController(agentClient, identityClient);
            var catalogSource = new AgentSoftwareCatalogSource(agentClient);
            var catalog = new LauncherCatalogService(catalogSource);
            var store = new LauncherStoreService(agentClient);
            var storeCheckoutReview = new LauncherStoreCheckoutReviewService(agentClient);
            var storeCheckoutStart = new LauncherStoreCheckoutStartService(agentClient);
            var storeCheckoutStatus = new LauncherStoreCheckoutStatusService(agentClient);
            var storeGiftClaimReveal = new LauncherStoreGiftClaimRevealService(agentClient);
            var checkoutRecoveryStore = new FileLauncherCheckoutRecoveryStore();
            var externalNavigator = new ExternalBrowserNavigator();
            var softwareInstall = new LauncherSoftwareInstallController(agentClient);
            var softwareUpdate = new LauncherSoftwareUpdateController(agentClient);
            var softwareRepair = new LauncherSoftwareRepairController(agentClient);
            var softwareOpen = new LauncherSoftwareOpenController(agentClient);
            var softwareRemove = new LauncherSoftwareRemoveController(agentClient);
            var claimCodeRedemption = new LauncherClaimCodeRedemptionController(agentClient);
            var viewModel = new MainWindowViewModel(
                accountSession,
                nativeSignIn,
                catalog,
                store,
                storeCheckoutReview,
                storeCheckoutStart,
                storeCheckoutStatus,
                storeGiftClaimReveal,
                checkoutRecoveryStore,
                externalNavigator,
                softwareInstall,
                softwareUpdate,
                softwareRepair,
                softwareOpen,
                softwareRemove,
                claimCodeRedemption);

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };

            desktop.Exit += (_, _) =>
            {
                identityClient.Dispose();
                agentClient.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
