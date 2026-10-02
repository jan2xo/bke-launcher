using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using BKE.Launcher.AgentClient;
using BKE.Launcher.Application;
using BKE.Launcher.Infrastructure;
using BKE.Launcher.Presentation;
using BKE.Launcher.PluginHost;
using BKE.Demo.LauncherPlugin;

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
            var passwordResetCompletion =
                new LauncherPasswordResetCompletionController(
                    platformAuthority,
                    identityClient);
            var catalogSource = new AgentSoftwareCatalogSource(agentClient);
            var catalog = new LauncherCatalogService(catalogSource);
            var store = new LauncherStoreService(agentClient);
            var storeCheckoutReview = new LauncherStoreCheckoutReviewService(agentClient);
            var storeCheckoutStart = new LauncherStoreCheckoutStartService(agentClient);
            var storeTrialStart = new LauncherStoreTrialStartService(agentClient);
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
            var pluginAuthorization =
                new LauncherPluginAuthorizationController(agentClient);
            var pluginRuntime = new LauncherPluginRuntime(
                new IBkeLauncherPlugin[]
                {
                    new BkeDemoLauncherPlugin(),
                },
                pluginAuthorization);
            var claimCodeRedemption = new LauncherClaimCodeRedemptionController(agentClient);
            var accountPasswordChange = new LauncherAccountPasswordChangeController(agentClient);
            var accountMfa = new LauncherAccountMfaController(agentClient);
            var accountPrivacy = new LauncherAccountPrivacyController(agentClient);
            var accountPurchases = new LauncherAccountPurchasesController(agentClient);
            var persistentGiftClaims =
                new LauncherPersistentGiftClaimsController(agentClient);
            var accountBilling = new LauncherAccountBillingController(agentClient);
            var accountPendingOrders = new LauncherAccountPendingOrdersController(agentClient);
            var accountLicenseSeats = new LauncherAccountLicenseSeatsController(agentClient);
            var accountLicenseDevices = new LauncherAccountLicenseDevicesController(agentClient);
            var accountOrganization = new LauncherAccountOrganizationController(agentClient);
            var viewModel = new MainWindowViewModel(
                accountSession,
                nativeSignIn,
                nativeRegistration,
                passwordResetRequest,
                passwordResetCompletion,
                catalog,
                store,
                storeCheckoutReview,
                storeCheckoutStart,
                storeTrialStart,
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
                persistentGiftClaims,
                accountBilling,
                accountPendingOrders,
                accountLicenseSeats,
                accountLicenseDevices,
                accountOrganization,
                pluginRuntime);

            desktop.MainWindow = new MainWindow
            {
                DataContext = viewModel,
            };

            if (Program.IsUiSmoke)
            {
                desktop.MainWindow.Opened += (_, _) =>
                {
                    var timer = new DispatcherTimer
                    {
                        Interval = TimeSpan.FromSeconds(1),
                    };
                    timer.Tick += (_, _) =>
                    {
                        timer.Stop();
                        desktop.Shutdown(0);
                    };
                    timer.Start();
                };
            }

            desktop.Exit += (_, _) =>
            {
                using var shutdownTimeout =
                    new CancellationTokenSource(TimeSpan.FromSeconds(5));
                try
                {
                    pluginRuntime
                        .ShutdownAsync(shutdownTimeout.Token)
                        .GetAwaiter()
                        .GetResult();
                }
                catch
                {
                    // Application shutdown must continue even when a bundled
                    // plugin cannot finish cleanup within the bounded window.
                }

                identityClient.Dispose();
                agentClient.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
