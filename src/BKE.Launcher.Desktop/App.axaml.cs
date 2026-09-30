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
            var platformAuthority = new LauncherPlatformAuthorityResolver(agentClient);
            var identityClient = new PlatformIdentityClient();
            var nativeSignIn = new LauncherNativeSignInController(
                agentClient,
                platformAuthority,
                identityClient);
            var nativeRegistration = new LauncherNativeRegistrationController(
                platformAuthority,
                identityClient);
            var passwordResetRequest =
                new LauncherPasswordResetRequestController(
                    platformAuthority,
                    identityClient);
            var catalogSource = new AgentSoftwareCatalogSource(agentClient);
            var catalog = new LauncherCatalogService(catalogSource);
            var store = new LauncherStoreService(agentClient);
            var storeCheckoutReview = new LauncherStoreCheckoutReviewService(agentClient);
            var storeCheckoutStart = new LauncherStoreCheckoutStartService(agentClient);
            var storeCheckoutStatus = new LauncherStoreCheckoutStatusService(agentClient);
            var storeGiftClaimReveal = new LauncherStoreGiftClaimRevealService(agentClient);
            var notifications = new LauncherNotificationInboxService(agentClient);
            var checkoutRecoveryStore = new FileLauncherCheckoutRecoveryStore();
            var externalNavigator =
                new ExternalBrowserNavigator(platformAuthority);
            var softwareInstall = new LauncherSoftwareInstallController(agentClient);
            var softwareUpdate = new LauncherSoftwareUpdateController(agentClient);
            var softwareRepair = new LauncherSoftwareRepairController(agentClient);
            var softwareOpen = new LauncherSoftwareOpenController(agentClient);
            var softwareRemove = new LauncherSoftwareRemoveController(agentClient);
            var claimCodeRedemption = new LauncherClaimCodeRedemptionController(agentClient);
            var accountPasswordChange = new LauncherAccountPasswordChangeController(agentClient);
            var accountMfa = new LauncherAccountMfaController(agentClient);
            var accountPrivacy = new LauncherAccountPrivacyController(agentClient);
            var accountPurchases = new LauncherAccountPurchasesController(agentClient);
            var accountOrganization = new LauncherAccountOrganizationController(agentClient);
            var viewModel = new MainWindowViewModel(
                accountSession,
                nativeSignIn,
                nativeRegistration,
                passwordResetRequest,
                catalog,
                store,
                storeCheckoutReview,
                storeCheckoutStart,
                storeCheckoutStatus,
                storeGiftClaimReveal,
                notifications,
                checkoutRecoveryStore,
                externalNavigator,
                softwareInstall,
                softwareUpdate,
                softwareRepair,
                softwareOpen,
                softwareRemove,
                claimCodeRedemption,
                accountPasswordChange,
                accountMfa,
                accountPrivacy,
                accountPurchases,
                accountOrganization);

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
