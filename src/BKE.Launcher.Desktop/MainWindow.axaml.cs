using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using BKE.Launcher.Presentation;

namespace BKE.Launcher.Desktop;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private MainWindowViewModel ViewModel =>
        DataContext as MainWindowViewModel
        ?? throw new InvalidOperationException("BKE Launcher view model is unavailable.");

    private async void NativeSignIn(object? sender, RoutedEventArgs args)
    {
        await ViewModel.NativeSignInAsync(CancellationToken.None);
    }

    private async void RefreshStatus(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RefreshStatusAsync(CancellationToken.None);
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

    private async void Logout(object? sender, RoutedEventArgs args)
    {
        await ViewModel.LogoutAsync(CancellationToken.None);
    }

}
