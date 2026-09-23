using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Presentation;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly LauncherAccountSessionController _accountSession;
    private readonly LauncherNativeSignInController _nativeSignIn;
    private readonly LauncherCatalogService _catalog;
    private readonly LauncherSoftwareInstallController _softwareInstall;
    private readonly LauncherSoftwareOpenController _softwareOpen;
    private readonly LauncherSoftwareRemoveController _softwareRemove;
    private string _sessionStatus = "SIGNED_OUT";
    private string _email = string.Empty;
    private string _password = string.Empty;
    private NativeBkeAccountChoice? _selectedAccount;
    private string _accountDisplay = "Not signed in";
    private string _userCode = string.Empty;
    private string _verificationUri = string.Empty;
    private string _message = "Connect this Launcher to the BKE Licensing Agent.";
    private string _catalogStatus = "AUTH_REQUIRED";
    private string _catalogMessage = "Sign in to load your BKE software.";

    public MainWindowViewModel(
        LauncherAccountSessionController accountSession,
        LauncherNativeSignInController nativeSignIn,
        LauncherCatalogService catalog,
        LauncherSoftwareInstallController softwareInstall,
        LauncherSoftwareOpenController softwareOpen,
        LauncherSoftwareRemoveController softwareRemove)
    {
        _accountSession = accountSession;
        _nativeSignIn = nativeSignIn;
        _catalog = catalog;
        _softwareInstall = softwareInstall;
        _softwareOpen = softwareOpen;
        _softwareRemove = softwareRemove;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<SoftwareProductViewModel> Products { get; } = [];
    public ObservableCollection<NativeBkeAccountChoice> AvailableAccounts { get; } = [];

    public string Email
    {
        get => _email;
        set => SetField(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => SetField(ref _password, value);
    }

    public NativeBkeAccountChoice? SelectedAccount
    {
        get => _selectedAccount;
        set => SetField(ref _selectedAccount, value);
    }

    public bool HasAccountChoices => AvailableAccounts.Count > 0;

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

    public async Task NativeSignInAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrEmpty(Password))
        {
            SessionStatus = "SIGNED_OUT";
            Message = "Enter your BKE email and password.";
            return;
        }

        try
        {
            SessionStatus = "SIGNING_IN";
            Message = "Authenticating directly with BKE Digital Solutions…";

            var result = await _nativeSignIn.SignInAsync(
                Email,
                Password,
                SelectedAccount?.AccountId,
                cancellationToken);

            if (result.Status == "ACCOUNT_SELECTION_REQUIRED")
            {
                AvailableAccounts.Clear();
                foreach (var account in result.Accounts)
                {
                    AvailableAccounts.Add(account);
                }
                SelectedAccount = AvailableAccounts.FirstOrDefault();
                Raise(nameof(HasAccountChoices));
                SessionStatus = "ACCOUNT_SELECTION_REQUIRED";
                Message = AvailableAccounts.Count == 0
                    ? "No active BKE account is available for this identity."
                    : "Choose the account to connect to this machine, then sign in again.";
                return;
            }

            Password = string.Empty;
            AvailableAccounts.Clear();
            SelectedAccount = null;
            Raise(nameof(HasAccountChoices));

            SessionStatus = result.Status;
            if (result.Status == "AUTHENTICATED" && result.Account is not null)
            {
                AccountDisplay = $"{result.Account.DisplayName} · {result.Account.Email}";
                Message = "Signed in. Durable account-session secrets are stored by the BKE Licensing Agent.";
                await RefreshCatalogAsync(cancellationToken);
                return;
            }

            AccountDisplay = "Not signed in";
            Message = result.ErrorMessage ?? "BKE account sign-in failed.";
            ClearCatalog("AUTH_REQUIRED", "Sign in to load your BKE software.");
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            Password = string.Empty;
            SessionStatus = "SIGN_IN_UNAVAILABLE";
            AccountDisplay = "Not signed in";
            Message = "BKE native sign-in is unavailable or returned an invalid response.";
            ClearCatalog("AUTH_REQUIRED", "Sign in to load your BKE software.");
        }
    }

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
                        CanOpen = true,
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

    public async Task OpenProductAsync(
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

        if (index < 0 || !Products[index].CanOpen)
        {
            return;
        }

        var product = Products[index];
        CatalogStatus = "OPENING";
        CatalogMessage =
            $"Opening {product.DisplayName} through the BKE Licensing Agent…";

        try
        {
            var response = await _softwareOpen.OpenAsync(
                productId,
                cancellationToken);

            if (response.Status == "STARTED")
            {
                CatalogStatus = "READY";
                CatalogMessage =
                    $"{product.DisplayName} was started by the BKE Licensing Agent.";
                return;
            }

            if (response.Status == "AUTH_REQUIRED")
            {
                Products[index] = product with
                {
                    CanOpen = false,
                };
                CatalogStatus = "AUTH_REQUIRED";
                CatalogMessage =
                    response.Error?.Message ??
                    "Sign in with BKE before opening software.";
                return;
            }

            CatalogStatus = "FAILED";
            CatalogMessage =
                response.Error?.Message ??
                $"Open failed: {response.State}.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            CatalogStatus = "AGENT_UNAVAILABLE";
            CatalogMessage =
                "The BKE Licensing Agent open capability is unavailable or invalid.";
        }
    }

    public async Task RemoveProductAsync(
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

        if (index < 0 || !Products[index].CanRemove)
        {
            return;
        }

        var previous = Products[index];
        Products[index] = previous with
        {
            StateLabel = "Removing",
            CanInstall = false,
            CanOpen = false,
            CanRemove = false,
        };
        CatalogStatus = "REMOVING";
        CatalogMessage =
            $"Removing {previous.DisplayName} through the BKE Licensing Agent…";

        try
        {
            var response = await _softwareRemove.RemoveAsync(
                productId,
                cancellationToken);

            switch (response.Status)
            {
                case "REMOVED":
                case "NOT_INSTALLED":
                    CatalogStatus = "READY";
                    CatalogMessage = response.Status == "REMOVED"
                        ? $"{previous.DisplayName} was removed from this machine."
                        : $"{previous.DisplayName} is no longer installed on this machine.";
                    await RefreshCatalogAsync(cancellationToken);
                    return;

                case "AUTH_REQUIRED":
                    Products[index] = previous with
                    {
                        CanRemove = false,
                    };
                    CatalogStatus = "AUTH_REQUIRED";
                    CatalogMessage =
                        response.Error?.Message ??
                        "Sign in with BKE before removing software.";
                    return;

                default:
                    Products[index] = previous;
                    CatalogStatus = "FAILED";
                    CatalogMessage =
                        response.Error?.Message ??
                        $"Remove failed: {response.State}.";
                    return;
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
                "The BKE Licensing Agent remove capability is unavailable or invalid.";
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
            Password = string.Empty;
            AvailableAccounts.Clear();
            SelectedAccount = null;
            Raise(nameof(HasAccountChoices));
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

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
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
    bool CanInstall,
    bool CanOpen,
    bool CanRemove)
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
            product.State == LauncherProductState.Installable,
            product.ExecutionType == ProductExecutionType.Standalone &&
            product.State is LauncherProductState.Installed or LauncherProductState.UpdateAvailable,
            product.ExecutionType == ProductExecutionType.Standalone &&
            product.State is LauncherProductState.Installed
                or LauncherProductState.UpdateAvailable
                or LauncherProductState.InstalledNotEntitled
                or LauncherProductState.RepairRequired);
    }
}
