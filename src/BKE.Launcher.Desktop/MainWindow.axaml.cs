using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

    private async void StartSignIn(object? sender, RoutedEventArgs args)
    {
        await ViewModel.StartSignInAsync(CancellationToken.None);

        if (Uri.TryCreate(ViewModel.VerificationUri, UriKind.Absolute, out var uri) &&
            uri.Scheme == Uri.UriSchemeHttps)
        {
            TryOpenBrowser(uri);
        }
    }

    private async void RefreshStatus(object? sender, RoutedEventArgs args)
    {
        await ViewModel.RefreshStatusAsync(CancellationToken.None);
    }

    private async void Logout(object? sender, RoutedEventArgs args)
    {
        await ViewModel.LogoutAsync(CancellationToken.None);
    }

    private static void TryOpenBrowser(Uri uri)
    {
        try
        {
            Process.Start(new ProcessStartInfo(uri.ToString())
            {
                UseShellExecute = true,
            });
        }
        catch
        {
            // The verification URI remains visible so the user can open it manually.
        }
    }
}
