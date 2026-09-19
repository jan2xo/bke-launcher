using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Presentation;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly LauncherAccountSessionController _accountSession;
    private readonly LauncherCatalogService _catalog;
    private readonly LauncherSoftwareInstallController _softwareInstall;
    private string _sessionStatus = "SIGNED_OUT";
    private string _accountDisplay = "Not signed in";
    private string _userCode = string.Empty;
    private string _verificationUri = string.Empty;
    private string _message = "Connect this Launcher to the BKE Licensing Agent.";
    private string _catalogStatus = "AUTH_REQUIRED";
    private string _catalogMessage = "Sign in to load your BKE software.";

    public MainWindowViewModel(
        LauncherAccountSessionController accountSession,
        LauncherCatalogService catalog,
        LauncherSoftwareInstallController softwareInstall)
    {
        _accountSession = accountSession;
        _catalog = catalog;
        _softwareInstall = softwareInstall;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<SoftwareProductViewModel> Products { get; } = [];

    public string SessionStatus
    {
        get => _sessionStatus;
        private set => SetField(ref _sessionStatus, value);
    }

    public string AccountDisplay
    {
        get => _accountDisplay;
        private set => SetField(ref _accountDisplay, value);
    }

    public string UserCode
    {
        get => _userCode;
        private set => SetField(ref _userCode, value);
    }

    public string VerificationUri
    {
        get => _verificationUri;
        private set => SetField(ref _verificationUri, value);
    }

    public string Message
    {
        get => _message;
        private set => SetField(ref _message, value);
    }

    public string CatalogStatus
    {
        get => _catalogStatus;
        private set => SetField(ref _catalogStatus, value);
    }

    public string CatalogMessage
    {
        get => _catalogMessage;
        private set => SetField(ref _catalogMessage, value);
    }

    public bool ShowEmptyProducts => Products.Count == 0;

    public async Task StartSignInAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountSession.StartAsync(cancellationToken);
            SessionStatus = response.Status;
            UserCode = response.UserCode ?? string.Empty;
            VerificationUri = response.VerificationUri ?? string.Empty;
            Message = response.Status switch
            {
                "AUTHENTICATED" => "This machine already has an authenticated BKE account session.",
                "PENDING" => "Complete sign-in in your browser, then refresh account status.",
                _ => response.Error?.Message ?? "BKE account sign-in could not be started.",
            };

            if (response.Status == "AUTHENTICATED")
            {
                await RefreshCatalogAsync(cancellationToken);
            }
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or InvalidDataException)
        {
            SessionStatus = "AGENT_UNAVAILABLE";
            Message = "BKE Licensing Agent is unavailable or returned an invalid response.";
        }
    }

    public async Task RefreshStatusAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountSession.StatusAsync(cancellationToken);
            ApplyStatus(response);

            if (response.Status == "AUTHENTICATED")
            {
                await RefreshCatalogAsync(cancellationToken);
            }
            else
            {
                ClearCatalog(
                    "AUTH_REQUIRED",
                    "Sign in to load your BKE software.");
            }
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or InvalidDataException)
        {
            SessionStatus = "AGENT_UNAVAILABLE";
            AccountDisplay = "Not available";
            Message = "BKE Licensing Agent is unavailable or returned an invalid response.";
            ClearCatalog(
                "AGENT_UNAVAILABLE",
                "Software catalog is unavailable while the Agent cannot be reached.");
        }
    }

    public async Task RefreshCatalogAsync(CancellationToken cancellationToken)
    {
        try
        {
            var snapshot = await _catalog.GetProductsAsync(cancellationToken);
            CatalogStatus = snapshot.Status;
            CatalogMessage = snapshot.Message ?? snapshot.Status switch
            {
                "READY" => snapshot.Products.Count == 0
                    ? "No software is currently available for this account."
                    : "Catalog and installed state are supplied by the BKE Licensing Agent.",
                "AUTH_REQUIRED" => "Sign in to load your BKE software.",
                _ => "The software catalog is currently unavailable.",
            };

            Products.Clear();
            foreach (var product in snapshot.Products)
            {
                Products.Add(SoftwareProductViewModel.From(product));
            }

            Raise(nameof(ShowEmptyProducts));
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or InvalidDataException)
        {
            ClearCatalog(
                "AGENT_UNAVAILABLE",
                "BKE Licensing Agent software catalog is unavailable or invalid.");
        }
    }

    public async Task InstallProductAsync(
        string productId,
        CancellationToken cancellationToken)
    {
        var index = Products
            .Select((product, index) => (product, index))
            .Where(item =>
                string.Equals(
                    item.product.ProductId,
                    productId,
                    StringComparison.Ordinal))
            .Select(item => item.index)
            .DefaultIfEmpty(-1)
            .First();

        if (index < 0 || !Products[index].CanInstall)
        {
            return;
        }

        var previous = Products[index];
        Products[index] = previous with
        {
            StateLabel = "Installing",
            CanInstall = false,
        };
        CatalogStatus = "INSTALLING";
        CatalogMessage =
            $"Starting verified installation for {previous.DisplayName}…";

        try
        {
            var response = await _softwareInstall.InstallAsync(
                productId,
                cancellationToken);

            switch (response.Status)
            {
                case "STARTED":
                case "IN_PROGRESS":
                    CatalogStatus = "INSTALLING";
                    CatalogMessage = response.Status == "STARTED"
                        ? $"Verified installation started for {previous.DisplayName}. Refresh software after the elevation step completes."
                        : $"Installation is already in progress for {previous.DisplayName}.";
                    break;

                case "ALREADY_INSTALLED":
                    Products[index] = previous with
                    {
                        StateLabel = "Installed",
                        CanInstall = false,
                    };
                    CatalogStatus = "READY";
                    CatalogMessage =
                        $"{previous.DisplayName} is already installed on this machine.";
                    break;

                case "AUTH_REQUIRED":
                    Products[index] = previous with
                    {
                        StateLabel = "Sign in required",
                        CanInstall = false,
                    };
                    CatalogStatus = "AUTH_REQUIRED";
                    CatalogMessage =
                        response.Error?.Message ??
                        "Sign in with BKE before installing software.";
                    break;

                default:
                    Products[index] = previous with
                    {
                        StateLabel = "Install failed",
                        CanInstall = response.Error?.Retryable == true,
                    };
                    CatalogStatus = "FAILED";
                    CatalogMessage =
                        response.Error?.Message ??
                        $"Installation failed: {response.State}.";
                    break;
            }
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            Products[index] = previous;
            CatalogStatus = "AGENT_UNAVAILABLE";
            CatalogMessage =
                "The BKE Licensing Agent install capability is unavailable or invalid.";
        }
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _accountSession.LogoutAsync(cancellationToken);
            SessionStatus = response.Status;
            AccountDisplay = "Not signed in";
            UserCode = string.Empty;
            VerificationUri = string.Empty;
            Message = response.Error?.Message ?? "Signed out on this machine.";
            ClearCatalog(
                "AUTH_REQUIRED",
                "Sign in to load your BKE software.");
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or InvalidDataException)
        {
            SessionStatus = "AGENT_UNAVAILABLE";
            Message = "The local session could not be changed because the BKE Licensing Agent is unavailable.";
        }
    }

    private void ApplyStatus(AccountSessionStatusResponse response)
    {
        SessionStatus = response.Status;

        if (response.Account is not null)
        {
            AccountDisplay = $"{response.Account.DisplayName} · {response.Account.Email}";
        }
        else if (response.Status is "SIGNED_OUT" or "DENIED" or "EXPIRED" or "FAILED")
        {
            AccountDisplay = "Not signed in";
        }

        Message = response.Error?.Message ?? response.Status switch
        {
            "AUTHENTICATED" => "Launcher and License Center share this Agent-owned account session.",
            "PENDING" => "Waiting for browser approval.",
            "SIGNED_OUT" => "No BKE account is authenticated on this machine.",
            "DENIED" => "BKE account authorization was denied.",
            "EXPIRED" => "BKE account authorization expired.",
            _ => "BKE account-session status updated.",
        };
    }

    private void ClearCatalog(string status, string message)
    {
        CatalogStatus = status;
        CatalogMessage = message;
        Products.Clear();
        Raise(nameof(ShowEmptyProducts));
    }

    private void SetField(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (string.Equals(field, value, StringComparison.Ordinal))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void Raise(string? propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public sealed record SoftwareProductViewModel(
    string ProductId,
    string DisplayName,
    string Summary,
    string ExecutionLabel,
    string StateLabel,
    string VersionLabel,
    bool CanInstall)
{
    public static SoftwareProductViewModel From(LauncherProduct product)
    {
        var execution = product.ExecutionType switch
        {
            ProductExecutionType.LauncherPlugin => "Launcher Plugin",
            ProductExecutionType.Standalone => "Standalone",
            null => "Unassigned",
            _ => "Unknown",
        };

        var state = product.State switch
        {
            LauncherProductState.NotEntitled => "Not entitled",
            LauncherProductState.Installable => "Installable",
            LauncherProductState.Installed => "Installed",
            LauncherProductState.UpdateAvailable => "Update available",
            LauncherProductState.InstalledNotEntitled => "Installed · entitlement unavailable",
            LauncherProductState.PolicyUnassigned => "Owner policy unassigned",
            LauncherProductState.ReleaseUnavailable => "Release unavailable",
            LauncherProductState.Unavailable => "Unavailable",
            LauncherProductState.Installing => "Installing",
            LauncherProductState.RepairRequired => "Repair required",
            _ => "Unknown",
        };

        var version = product.InstalledVersion is not null
            ? product.AvailableVersion is not null &&
              !string.Equals(product.InstalledVersion, product.AvailableVersion, StringComparison.Ordinal)
                ? $"{product.InstalledVersion} → {product.AvailableVersion}"
                : product.InstalledVersion
            : product.AvailableVersion ?? "No release";

        return new SoftwareProductViewModel(
            product.ProductId,
            product.DisplayName,
            product.Summary,
            execution,
            state,
            version,
            product.ExecutionType == ProductExecutionType.Standalone &&
            product.State == LauncherProductState.Installable);
    }
}
