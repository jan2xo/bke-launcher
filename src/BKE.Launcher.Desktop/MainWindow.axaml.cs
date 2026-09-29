using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using BKE.Launcher.Presentation;

namespace BKE.Launcher.Desktop;

public sealed partial class MainWindow : Window
{
    private bool _startupInitialized;

    public MainWindow()
    {
        InitializeComponent();
        Opened += WindowOpened;
    }

    private MainWindowViewModel ViewModel =>
        DataContext as MainWindowViewModel
        ?? throw new InvalidOperationException("BKE Launcher view model is unavailable.");

    private async void WindowOpened(object? sender, EventArgs args)
    {
        if (_startupInitialized)
        {
            return;
        }

        _startupInitialized = true;
        await ViewModel.InitializeAsync(CancellationToken.None);
    }

    private async void OpenAccount(object? sender, RoutedEventArgs args)
    {
        ViewModel.OpenAccountSurface();
        if (ViewModel.ShowOrganizationSection)
        {
            await ViewModel.RefreshAccountOrganizationAsync(
                CancellationToken.None);
        }
    }

    private async void ModuleChanged(object? sender, SelectionChangedEventArgs args)
    {
        if (sender is TabControl { SelectedIndex: >= 0 } tabs)
        {
            await ViewModel.OpenModuleAsync(
                tabs.SelectedIndex,
                CancellationToken.None);
        }
    }

    private async void OpenRegistration(object? sender, RoutedEventArgs args)
    {
        await ViewModel.OpenRegistrationAsync(CancellationToken.None);
    }

    private void CancelRegistration(object? sender, RoutedEventArgs args)
    {
        ViewModel.CancelRegistration();
    }

    private async void CreateNativeAccount(object? sender, RoutedEventArgs args)
    {
        await ViewModel.CreateNativeAccountAsync(CancellationToken.None);
    }

    private async void VerifyRegistrationEmail(
        object? sender,
        RoutedEventArgs args)
    {
        await ViewModel.VerifyRegistrationEmailAsync(CancellationToken.None);
    }

    private async void ResendRegistrationVerification(
        object? sender,
        RoutedEventArgs args)
    {
        await ViewModel.ResendRegistrationVerificationAsync(
            CancellationToken.None);
    }

    private async void NativeSignIn(object? sender, RoutedEventArgs args)
    {
        await ViewModel.NativeSignInAsync(CancellationToken.None);
    }

    private async void VerifyNativeMfa(object? sender, RoutedEventArgs args)
    {
        await ViewModel.VerifyNativeMfaAsync(CancellationToken.None);
    }

    private void DismissMfaRecoveryCodes(object? sender, RoutedEventArgs args)
    {
        ViewModel.DismissMfaRecoveryCodes();
    }

    private async void RequestPasswordReset(
        object? sender,
        RoutedEventArgs args)
    {
        await ViewModel.RequestPasswordResetAsync(CancellationToken.None);
    }

    private async void RefreshStatus(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RefreshStatusAsync(CancellationToken.None);
    }

    private async void ChangePassword(object? sender, RoutedEventArgs args)
    {
        await ViewModel.ChangePasswordAsync(CancellationToken.None);
    }

    private async void RefreshAccountMfa(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RefreshAccountMfaAsync(CancellationToken.None);
    }

    private async void StartMfaEnrollment(object? sender, RoutedEventArgs args)
    {
        await ViewModel.StartMfaEnrollmentAsync(CancellationToken.None);
    }

    private async void CompleteMfaEnrollment(object? sender, RoutedEventArgs args)
    {
        await ViewModel.CompleteMfaEnrollmentAsync(CancellationToken.None);
    }

    private async void StartMfaProof(object? sender, RoutedEventArgs args)
    {
        await ViewModel.StartMfaProofAsync(CancellationToken.None);
    }

    private async void DisableMfa(object? sender, RoutedEventArgs args)
    {
        await ViewModel.DisableMfaAsync(CancellationToken.None);
    }

    private async void RegenerateMfaRecovery(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RegenerateMfaRecoveryAsync(CancellationToken.None);
    }

    private async void CreateAccountOrganization(
        object? sender,
        RoutedEventArgs args)
    {
        await ViewModel.CreateAccountOrganizationAsync(
            CancellationToken.None);
    }

    private async void RefreshAccountOrganization(
        object? sender,
        RoutedEventArgs args)
    {
        await ViewModel.RefreshAccountOrganizationAsync(
            CancellationToken.None);
    }

    private async void UpdateAccountOrganizationProfile(
        object? sender,
        RoutedEventArgs args)
    {
        await ViewModel.UpdateAccountOrganizationProfileAsync(
            CancellationToken.None);
    }

    private async void RefreshAccountPrivacy(
        object? sender,
        RoutedEventArgs args)
    {
        await ViewModel.RefreshAccountPrivacyAsync(
            CancellationToken.None);
    }

    private async void CreateAccountPrivacyRequest(
        object? sender,
        RoutedEventArgs args)
    {
        await ViewModel.CreateAccountPrivacyRequestAsync(
            CancellationToken.None);
    }

    private async void RedeemClaimCode(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RedeemClaimCodeAsync(CancellationToken.None);
    }

    private async void RefreshCatalog(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RefreshCatalogAsync(CancellationToken.None);
    }

    private async void RefreshStore(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RefreshStoreAsync(CancellationToken.None);
    }

    private async void RefreshNotifications(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RefreshNotificationsAsync(CancellationToken.None);
    }

    private async void MarkNotificationRead(object? sender, RoutedEventArgs args)
    {
        if (sender is Button
            {
                DataContext: NotificationViewModel notification,
            })
        {
            await ViewModel.MarkNotificationReadAsync(
                notification,
                CancellationToken.None);
        }
    }

    private async void DismissNotification(object? sender, RoutedEventArgs args)
    {
        if (sender is Button
            {
                DataContext: NotificationViewModel notification,
            })
        {
            await ViewModel.DismissNotificationAsync(
                notification,
                CancellationToken.None);
        }
    }

    private async void ReviewPurchase(object? sender, RoutedEventArgs args)
    {
        if (sender is Button
            {
                DataContext: StorePlanViewModel plan,
            })
        {
            await ViewModel.ReviewPurchaseAsync(
                plan.PurchasePlanId,
                CancellationToken.None);
        }
    }

    private async void BuyForSelf(object? sender, RoutedEventArgs args)
    {
        await ViewModel.StartPurchaseAsync(
            "SELF",
            CancellationToken.None);
    }

    private async void BuyAsGift(object? sender, RoutedEventArgs args)
    {
        await ViewModel.StartPurchaseAsync(
            "GIFT",
            CancellationToken.None);
    }

    private async void CheckPurchaseCheckoutStatus(object? sender, RoutedEventArgs args)
    {
        await ViewModel.CheckPurchaseCheckoutStatusAsync(
            CancellationToken.None);
    }

    private async void RetryOriginalCheckout(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RetryOriginalCheckoutAsync(
            CancellationToken.None);
    }

    private void CompleteGiftClaimDelivery(object? sender, RoutedEventArgs args)
    {
        ViewModel.CompleteGiftClaimDelivery();
    }

    private void OpenExistingCheckout(object? sender, RoutedEventArgs args)
    {
        ViewModel.OpenExistingCheckout();
    }

    private async void OpenPurchaseLegalDocument(
        object? sender,
        RoutedEventArgs args)
    {
        if (sender is Button
            {
                DataContext: PurchaseLegalDocumentViewModel document,
            })
        {
            await ViewModel.OpenLegalDocumentAsync(
                document,
                CancellationToken.None);
        }
    }

    private async void InstallProduct(object? sender, RoutedEventArgs args)
    {
        if (sender is Button
            {
                DataContext: SoftwareProductViewModel product,
            } &&
            product.CanInstall)
        {
            await ViewModel.InstallProductAsync(
                product.ProductId,
                CancellationToken.None);
        }
    }

    private async void UpdateProduct(object? sender, RoutedEventArgs args)
    {
        if (sender is Button
            {
                DataContext: SoftwareProductViewModel product,
            } &&
            product.CanUpdate)
        {
            await ViewModel.UpdateProductAsync(
                product.ProductId,
                CancellationToken.None);
        }
    }

    private async void RepairProduct(object? sender, RoutedEventArgs args)
    {
        if (sender is Button
            {
                DataContext: SoftwareProductViewModel product,
            } &&
            product.CanRepair)
        {
            await ViewModel.RepairProductAsync(
                product.ProductId,
                CancellationToken.None);
        }
    }

    private async void OpenProduct(object? sender, RoutedEventArgs args)
    {
        if (sender is Button
            {
                DataContext: SoftwareProductViewModel product,
            } &&
            product.CanOpen)
        {
            await ViewModel.OpenProductAsync(
                product.ProductId,
                CancellationToken.None);
        }
    }

    private async void RemoveProduct(object? sender, RoutedEventArgs args)
    {
        if (sender is not Button
            {
                DataContext: SoftwareProductViewModel product,
            } ||
            !product.CanRemove)
        {
            return;
        }

        if (!await ConfirmRemovalAsync(product))
        {
            return;
        }

        await ViewModel.RemoveProductAsync(
            product.ProductId,
            CancellationToken.None);
    }

    private async Task<bool> ConfirmRemovalAsync(
        SoftwareProductViewModel product)
    {
        var dialog = new Window
        {
            Title = "Remove software",
            Width = 440,
            SizeToContent = SizeToContent.Height,
            CanResize = false,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        var remove = new Button
        {
            Content = "Remove",
        };
        var cancel = new Button
        {
            Content = "Cancel",
        };

        remove.Click += (_, _) => dialog.Close(true);
        cancel.Click += (_, _) => dialog.Close(false);

        dialog.Content = new StackPanel
        {
            Margin = new Thickness(24),
            Spacing = 16,
            Children =
            {
                new TextBlock
                {
                    Text = $"Remove {product.DisplayName}?",
                    FontSize = 20,
                    FontWeight = Avalonia.Media.FontWeight.SemiBold,
                },
                new TextBlock
                {
                    Text = "BKE will ask the Licensing Agent to run the product's trusted uninstall strategy. User-created projects and exports outside the managed install root are not part of this removal.",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 10,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Children =
                    {
                        cancel,
                        remove,
                    },
                },
            },
        };

        return await dialog.ShowDialog<bool>(this);
    }

    private async void SwitchAccount(object? sender, RoutedEventArgs args)
    {
        await ViewModel.SwitchAccountAsync(CancellationToken.None);
    }

    private async void Logout(object? sender, RoutedEventArgs args)
    {
        await ViewModel.LogoutAsync(CancellationToken.None);
    }

}
