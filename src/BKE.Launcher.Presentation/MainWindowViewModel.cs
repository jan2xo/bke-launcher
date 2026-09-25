using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Presentation;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly LauncherAccountSessionController _accountSession;
    private readonly LauncherNativeSignInController _nativeSignIn;
    private readonly LauncherCatalogService _catalog;
    private readonly LauncherStoreService _store;
    private readonly LauncherStoreCheckoutReviewService _storeCheckoutReview;
    private readonly LauncherStoreCheckoutStartService _storeCheckoutStart;
    private readonly LauncherStoreCheckoutStatusService _storeCheckoutStatus;
    private readonly LauncherStoreGiftClaimRevealService _storeGiftClaimReveal;
    private readonly ILauncherCheckoutRecoveryStore _checkoutRecoveryStore;
    private readonly ILauncherExternalNavigator _externalNavigator;
    private readonly LauncherSoftwareInstallController _softwareInstall;
    private readonly LauncherSoftwareUpdateController _softwareUpdate;
    private readonly LauncherSoftwareRepairController _softwareRepair;
    private readonly LauncherSoftwareOpenController _softwareOpen;
    private readonly LauncherSoftwareRemoveController _softwareRemove;
    private readonly LauncherClaimCodeRedemptionController _claimCodeRedemption;
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
    private string _claimCode = string.Empty;
    private string _claimStatus = "AUTH_REQUIRED";
    private string _claimMessage = "Sign in to redeem a Claim Code.";
    private string _storeStatus = "AUTH_REQUIRED";
    private string _storeMessage = "Sign in to browse the BKE Store.";
    private bool _giftCheckoutEnabled;
    private string _purchaseReviewStatus = "IDLE";
    private string _purchaseReviewMessage = "Select a plan to review the current purchase terms.";
    private string _purchaseReviewProductLabel = string.Empty;
    private string _purchaseReviewEditionLabel = string.Empty;
    private string _purchaseReviewPriceLabel = string.Empty;
    private string _purchaseReviewModesLabel = string.Empty;
    private string _purchaseReviewLegalLabel = string.Empty;
    private bool _showPurchaseReview;
    private string _purchaseCheckoutStatus = "IDLE";
    private string _purchaseCheckoutMessage = "Review a plan before starting checkout.";
    private string _giftClaimCode = string.Empty;
    private string? _reviewedPurchasePlanId;
    private IReadOnlySet<string> _reviewedPurchaseModes =
        new HashSet<string>(StringComparer.Ordinal);
    private bool _purchaseAttemptLocked;
    private string? _checkoutRecoveryCorrelationId;
    private string? _recoverableCheckoutUrl;
    private bool _checkoutRecoveryStateBlocked;

    public MainWindowViewModel(
        LauncherAccountSessionController accountSession,
        LauncherNativeSignInController nativeSignIn,
        LauncherCatalogService catalog,
        LauncherStoreService store,
        LauncherStoreCheckoutReviewService storeCheckoutReview,
        LauncherStoreCheckoutStartService storeCheckoutStart,
        LauncherStoreCheckoutStatusService storeCheckoutStatus,
        LauncherStoreGiftClaimRevealService storeGiftClaimReveal,
        ILauncherCheckoutRecoveryStore checkoutRecoveryStore,
        ILauncherExternalNavigator externalNavigator,
        LauncherSoftwareInstallController softwareInstall,
        LauncherSoftwareUpdateController softwareUpdate,
        LauncherSoftwareRepairController softwareRepair,
        LauncherSoftwareOpenController softwareOpen,
        LauncherSoftwareRemoveController softwareRemove,
        LauncherClaimCodeRedemptionController claimCodeRedemption)
    {
        _accountSession = accountSession;
        _nativeSignIn = nativeSignIn;
        _catalog = catalog;
        _store = store;
        _storeCheckoutReview = storeCheckoutReview;
        _storeCheckoutStart = storeCheckoutStart;
        _storeCheckoutStatus = storeCheckoutStatus;
        _storeGiftClaimReveal = storeGiftClaimReveal;
        _checkoutRecoveryStore = checkoutRecoveryStore;
        _externalNavigator = externalNavigator;
        _softwareInstall = softwareInstall;
        _softwareUpdate = softwareUpdate;
        _softwareRepair = softwareRepair;
        _softwareOpen = softwareOpen;
        _softwareRemove = softwareRemove;
        _claimCodeRedemption = claimCodeRedemption;
        RestoreCheckoutRecoveryState();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<SoftwareProductViewModel> Products { get; } = [];
    public ObservableCollection<StoreProductViewModel> StoreProducts { get; } = [];
    public ObservableCollection<PurchaseLegalDocumentViewModel> PurchaseLegalDocuments { get; } = [];
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

    public string ClaimCode
    {
        get => _claimCode;
        set => SetField(ref _claimCode, value);
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
        private set
        {
            SetField(ref _sessionStatus, value);
            Raise(nameof(CanRedeemClaimCode));
            Raise(nameof(CanCheckCheckoutStatus));
        }
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

    public string ClaimStatus
    {
        get => _claimStatus;
        private set => SetField(ref _claimStatus, value);
    }

    public string ClaimMessage
    {
        get => _claimMessage;
        private set => SetField(ref _claimMessage, value);
    }

    public string StoreStatus
    {
        get => _storeStatus;
        private set => SetField(ref _storeStatus, value);
    }

    public string StoreMessage
    {
        get => _storeMessage;
        private set => SetField(ref _storeMessage, value);
    }

    public bool GiftCheckoutEnabled
    {
        get => _giftCheckoutEnabled;
        private set
        {
            SetField(ref _giftCheckoutEnabled, value);
            Raise(nameof(GiftCheckoutLabel));
            Raise(nameof(CanBuyGift));
        }
    }

    public string GiftCheckoutLabel =>
        GiftCheckoutEnabled
            ? "Gift purchase option: unbound one-time Claim Code delivery available"
            : "Gift purchase option: not available in this environment";

    public string PurchaseReviewStatus
    {
        get => _purchaseReviewStatus;
        private set
        {
            SetField(ref _purchaseReviewStatus, value);
            Raise(nameof(ShowPurchaseActions));
            Raise(nameof(CanBuySelf));
            Raise(nameof(CanBuyGift));
        }
    }

    public string PurchaseReviewMessage
    {
        get => _purchaseReviewMessage;
        private set => SetField(ref _purchaseReviewMessage, value);
    }

    public string PurchaseReviewProductLabel
    {
        get => _purchaseReviewProductLabel;
        private set => SetField(ref _purchaseReviewProductLabel, value);
    }

    public string PurchaseReviewEditionLabel
    {
        get => _purchaseReviewEditionLabel;
        private set => SetField(ref _purchaseReviewEditionLabel, value);
    }

    public string PurchaseReviewPriceLabel
    {
        get => _purchaseReviewPriceLabel;
        private set => SetField(ref _purchaseReviewPriceLabel, value);
    }

    public string PurchaseReviewModesLabel
    {
        get => _purchaseReviewModesLabel;
        private set => SetField(ref _purchaseReviewModesLabel, value);
    }

    public string PurchaseReviewLegalLabel
    {
        get => _purchaseReviewLegalLabel;
        private set => SetField(ref _purchaseReviewLegalLabel, value);
    }

    public bool ShowPurchaseReview
    {
        get => _showPurchaseReview;
        private set => SetField(ref _showPurchaseReview, value);
    }

    public string PurchaseCheckoutStatus
    {
        get => _purchaseCheckoutStatus;
        private set => SetField(ref _purchaseCheckoutStatus, value);
    }

    public string PurchaseCheckoutMessage
    {
        get => _purchaseCheckoutMessage;
        private set => SetField(ref _purchaseCheckoutMessage, value);
    }

    public string GiftClaimCode
    {
        get => _giftClaimCode;
        private set
        {
            SetField(ref _giftClaimCode, value);
            Raise(nameof(HasGiftClaimCode));
            Raise(nameof(CanCompleteGiftDelivery));
        }
    }

    public bool HasGiftClaimCode =>
        !string.IsNullOrWhiteSpace(GiftClaimCode);

    public bool CanCompleteGiftDelivery =>
        HasGiftClaimCode &&
        _purchaseAttemptLocked &&
        !_checkoutRecoveryStateBlocked &&
        !string.IsNullOrWhiteSpace(_checkoutRecoveryCorrelationId);

    public bool ShowPurchaseActions =>
        PurchaseReviewStatus == "READY" &&
        _reviewedPurchasePlanId is not null &&
        PurchaseLegalDocuments.Count is >= 2 and <= 3;

    public bool CanBuySelf =>
        ShowPurchaseActions &&
        !_purchaseAttemptLocked &&
        _reviewedPurchaseModes.Contains("SELF");

    public bool CanBuyGift =>
        GiftCheckoutEnabled &&
        ShowPurchaseActions &&
        !_purchaseAttemptLocked &&
        _reviewedPurchaseModes.Contains("GIFT");

    public bool CanCheckCheckoutStatus =>
        string.Equals(SessionStatus, "AUTHENTICATED", StringComparison.Ordinal) &&
        _purchaseAttemptLocked &&
        !_checkoutRecoveryStateBlocked &&
        !string.IsNullOrWhiteSpace(_checkoutRecoveryCorrelationId);

    public bool CanOpenExistingCheckout =>
        !_checkoutRecoveryStateBlocked &&
        !string.IsNullOrWhiteSpace(_recoverableCheckoutUrl);

    public bool ShowPurchaseCheckoutState =>
        ShowPurchaseActions ||
        _checkoutRecoveryStateBlocked ||
        !string.IsNullOrWhiteSpace(_checkoutRecoveryCorrelationId);

    public bool CanRedeemClaimCode =>
        string.Equals(SessionStatus, "AUTHENTICATED", StringComparison.Ordinal);

    public bool ShowEmptyProducts => Products.Count == 0;
    public bool ShowEmptyStore => StoreProducts.Count == 0;

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
            ClaimCode = string.Empty;
            ClaimStatus = "AUTH_REQUIRED";
            ClaimMessage = "Sign in to redeem a Claim Code.";
            AvailableAccounts.Clear();
            SelectedAccount = null;
            Raise(nameof(HasAccountChoices));

            SessionStatus = result.Status;
            if (result.Status == "AUTHENTICATED" && result.Account is not null)
            {
                AccountDisplay = $"{result.Account.DisplayName} · {result.Account.Email}";
                Message = "Signed in. Durable account-session secrets are stored by the BKE Licensing Agent.";
                await RefreshCatalogAsync(cancellationToken);
                await RefreshStoreAsync(cancellationToken);
                return;
            }

            AccountDisplay = "Not signed in";
            Message = result.ErrorMessage ?? "BKE account sign-in failed.";
            ClearCatalog("AUTH_REQUIRED", "Sign in to load your BKE software.");
            ClearStore("AUTH_REQUIRED", "Sign in to browse the BKE Store.");
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
                await RefreshStoreAsync(cancellationToken);
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
                await RefreshStoreAsync(cancellationToken);
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
            ClearStore(
                "AGENT_UNAVAILABLE",
                "BKE Store is unavailable while the Agent cannot be reached.");
        }
    }

    public async Task RedeemClaimCodeAsync(CancellationToken cancellationToken)
    {
        if (!CanRedeemClaimCode)
        {
            ClaimStatus = "AUTH_REQUIRED";
            ClaimMessage = "Sign in with BKE before redeeming a Claim Code.";
            return;
        }

        var code = ClaimCode.Trim();
        if (string.IsNullOrWhiteSpace(code))
        {
            ClaimStatus = "INVALID_REQUEST";
            ClaimMessage = "Enter a Claim Code.";
            return;
        }

        ClaimStatus = "REDEEMING";
        ClaimMessage = $"Redeeming this Claim Code to {AccountDisplay}…";

        try
        {
            var response = await _claimCodeRedemption.RedeemAsync(
                code,
                cancellationToken);

            switch (response.Status)
            {
                case "CLAIMED":
                    ClaimCode = string.Empty;
                    ClaimStatus = "CLAIMED";
                    ClaimMessage =
                        "Claim Code redeemed. Your BKE software is being refreshed.";
                    await RefreshCatalogAsync(cancellationToken);
                await RefreshStoreAsync(cancellationToken);
                    return;

                case "AUTH_REQUIRED":
                    ClaimStatus = "AUTH_REQUIRED";
                    ClaimMessage =
                        response.Error?.Message ??
                        "Sign in with BKE before redeeming a Claim Code.";
                    return;

                case "NOT_FOUND":
                case "ALREADY_USED":
                case "REVOKED":
                case "EXPIRED":
                case "ACCOUNT_FORBIDDEN":
                case "ACCOUNT_UNAVAILABLE":
                    ClaimStatus = response.Status;
                    ClaimMessage =
                        response.Error?.Message ??
                        "The Claim Code could not be redeemed.";
                    return;

                default:
                    ClaimStatus = "FAILED";
                    ClaimMessage =
                        response.Error?.Message ??
                        "Claim Code redemption failed.";
                    return;
            }
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            ClaimStatus = "AGENT_UNAVAILABLE";
            ClaimMessage =
                "The BKE Licensing Agent Claim Code capability is unavailable or invalid.";
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

    public async Task RefreshStoreAsync(CancellationToken cancellationToken)
    {
        try
        {
            var snapshot = await _store.GetProductsAsync(cancellationToken);
            StoreStatus = snapshot.Status;
            GiftCheckoutEnabled = snapshot.GiftCheckoutEnabled;
            StoreMessage = snapshot.Message ?? snapshot.Status switch
            {
                "READY" => snapshot.Products.Count == 0
                    ? "No purchasable software is currently published."
                    : "Products, editions, plans, and prices are supplied through the BKE Licensing Agent.",
                "AUTH_REQUIRED" => "Sign in to browse the BKE Store.",
                _ => "The BKE Store is currently unavailable.",
            };

            StoreProducts.Clear();
            foreach (var product in snapshot.Products)
            {
                StoreProducts.Add(StoreProductViewModel.From(product));
            }

            Raise(nameof(ShowEmptyStore));
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            ClearStore(
                "AGENT_UNAVAILABLE",
                "BKE Licensing Agent Store catalog is unavailable or invalid.");
        }
    }

    public async Task ReviewPurchaseAsync(
        string purchasePlanId,
        CancellationToken cancellationToken)
    {
        if (_checkoutRecoveryStateBlocked ||
            (_purchaseAttemptLocked &&
             !string.IsNullOrWhiteSpace(_checkoutRecoveryCorrelationId)))
        {
            PurchaseCheckoutStatus = _checkoutRecoveryStateBlocked
                ? "RECOVERY_STATE_INVALID"
                : "RECOVERY_REQUIRED";
            PurchaseCheckoutMessage = _checkoutRecoveryStateBlocked
                ? "BKE cannot safely read the saved checkout recovery state, so a new checkout is blocked."
                : "Resolve the existing checkout attempt before reviewing or starting another purchase.";
            RaiseCheckoutRecoveryState();
            return;
        }

        ShowPurchaseReview = true;
        PurchaseReviewProductLabel = string.Empty;
        PurchaseReviewEditionLabel = string.Empty;
        PurchaseReviewPriceLabel = string.Empty;
        PurchaseReviewModesLabel = string.Empty;
        PurchaseReviewLegalLabel = string.Empty;
        _reviewedPurchasePlanId = null;
        _reviewedPurchaseModes = new HashSet<string>(StringComparer.Ordinal);
        _purchaseAttemptLocked = false;
        _checkoutRecoveryCorrelationId = null;
        _recoverableCheckoutUrl = null;
        PurchaseLegalDocuments.Clear();
        PurchaseCheckoutStatus = "IDLE";
        PurchaseCheckoutMessage =
            "Review the required Legal documents before starting checkout.";
        RaisePurchaseActionState();
        RaiseCheckoutRecoveryState();

        if (!string.Equals(
                SessionStatus,
                "AUTHENTICATED",
                StringComparison.Ordinal))
        {
            PurchaseReviewStatus = "AUTH_REQUIRED";
            PurchaseReviewMessage =
                "Sign in with BKE before reviewing a purchase.";
            return;
        }

        try
        {
            PurchaseReviewStatus = "REVIEWING";
            PurchaseReviewMessage =
                "Refreshing canonical price, purchase modes, and Legal requirements…";

            var review = await _storeCheckoutReview.ReviewAsync(
                purchasePlanId,
                cancellationToken);

            PurchaseReviewStatus = review.Status;
            PurchaseReviewMessage = review.Message ?? review.Status switch
            {
                "READY" => "Purchase review is current. No payment has been started.",
                "LEGAL_REACCEPTANCE_REQUIRED" =>
                    "Current BKE Legal documents must be accepted before this purchase can continue.",
                "LEGAL_ACCEPTANCE_REQUIRED" =>
                    "This purchase requires Legal acceptance before checkout can continue.",
                "PLAN_NOT_AVAILABLE" =>
                    "The selected plan is no longer available. Refresh the Store.",
                "ACCOUNT_FORBIDDEN" =>
                    "This account cannot make this purchase.",
                "ACCOUNT_UNAVAILABLE" =>
                    "This account is not currently available for purchases.",
                "AUTH_REQUIRED" =>
                    "Sign in with BKE before reviewing a purchase.",
                _ => "Purchase review is currently unavailable.",
            };

            if (review.Product is not null)
            {
                PurchaseReviewProductLabel =
                    $"Product: {review.Product.DisplayName}";
            }

            if (review.Edition is not null)
            {
                PurchaseReviewEditionLabel =
                    $"Edition: {review.Edition.Name} · up to {review.Edition.MaxUsers} user(s) · {review.Edition.MaxDevicesPerUser} device(s) per user";
            }

            if (review.Plan is not null)
            {
                PurchaseReviewPriceLabel =
                    $"Canonical price: {StorePlanViewModel.From(review.Plan).PriceLabel}";
            }

            if (review.PurchaseModes.Count > 0)
            {
                PurchaseReviewModesLabel =
                    "Purchase modes: " +
                    string.Join(" · ", review.PurchaseModes);
            }

            if (review.Status == "READY")
            {
                _reviewedPurchasePlanId = purchasePlanId;
                _reviewedPurchaseModes =
                    review.PurchaseModes.ToHashSet(StringComparer.Ordinal);
                foreach (var document in review.LegalDocuments)
                {
                    PurchaseLegalDocuments.Add(
                        PurchaseLegalDocumentViewModel.From(document));
                }
                PurchaseCheckoutStatus = "READY";
                PurchaseCheckoutMessage =
                    PurchaseLegalDocuments.Count is >= 2 and <= 3
                        ? "Review each required Legal document, accept it, then choose how to buy."
                        : "Checkout is blocked because the required Legal document set is incomplete.";
                RaisePurchaseActionState();
            }

            if (review.PendingLegal.Count > 0)
            {
                PurchaseReviewLegalLabel =
                    "Legal reacceptance required: " +
                    string.Join(
                        " · ",
                        review.PendingLegal.Select(document =>
                            $"{document.Title} v{document.Version}"));
            }
            else if (review.LegalDocuments.Count > 0)
            {
                PurchaseReviewLegalLabel =
                    "Legal requirements: " +
                    string.Join(
                        " · ",
                        review.LegalDocuments.Select(document =>
                            document.RequiresReacceptance
                                ? $"{document.Title} v{document.Version} (reacceptance required)"
                                : $"{document.Title} v{document.Version}"));
            }
            else if (review.Status == "READY")
            {
                PurchaseReviewLegalLabel =
                    "Legal requirements: none returned for this plan.";
            }
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            PurchaseReviewStatus = "AGENT_UNAVAILABLE";
            PurchaseReviewMessage =
                "BKE Licensing Agent purchase review is unavailable or invalid.";
            PurchaseReviewProductLabel = string.Empty;
            PurchaseReviewEditionLabel = string.Empty;
            PurchaseReviewPriceLabel = string.Empty;
            PurchaseReviewModesLabel = string.Empty;
            PurchaseReviewLegalLabel = string.Empty;
        }
    }

    public async Task StartPurchaseAsync(
        string purchaseMode,
        CancellationToken cancellationToken)
    {
        if (!ShowPurchaseActions ||
            _reviewedPurchasePlanId is null ||
            !_reviewedPurchaseModes.Contains(purchaseMode))
        {
            PurchaseCheckoutStatus = "REVIEW_REQUIRED";
            PurchaseCheckoutMessage =
                "Review the current plan and purchase options again before checkout.";
            return;
        }

        if (PurchaseLegalDocuments.Any(document => !document.IsAccepted))
        {
            PurchaseCheckoutStatus = "LEGAL_ACCEPTANCE_REQUIRED";
            PurchaseCheckoutMessage =
                "Open, review, and accept every required Legal document before checkout.";
            return;
        }

        GiftClaimCode = string.Empty;

        var correlationId = Guid.NewGuid().ToString("N");
        try
        {
            _checkoutRecoveryStore.WriteCorrelationId(correlationId);
        }
        catch (Exception storageError) when (
            storageError is IOException or
            UnauthorizedAccessException or
            InvalidDataException or
            InvalidOperationException)
        {
            PurchaseCheckoutStatus = "RECOVERY_STORAGE_FAILED";
            PurchaseCheckoutMessage =
                "Checkout was not started because BKE could not persist its recovery correlation safely.";
            return;
        }

        _purchaseAttemptLocked = true;
        _checkoutRecoveryCorrelationId = correlationId;
        _recoverableCheckoutUrl = null;
        _checkoutRecoveryStateBlocked = false;
        RaisePurchaseActionState();
        RaiseCheckoutRecoveryState();
        PurchaseCheckoutStatus = "STARTING";
        PurchaseCheckoutMessage =
            "Creating a secure checkout through the BKE Licensing Agent…";

        try
        {
            var result = await _storeCheckoutStart.StartAsync(
                _checkoutRecoveryCorrelationId,
                _reviewedPurchasePlanId,
                purchaseMode,
                PurchaseLegalDocuments
                    .Select(document => document.DocumentVersionId)
                    .ToArray(),
                cancellationToken);

            var recoveryAvailable =
                result.Status is "READY" or "RESULT_UNKNOWN" or "CHECKOUT_IN_PROGRESS";
            if (recoveryAvailable)
            {
                _checkoutRecoveryCorrelationId = result.CorrelationId;
                _recoverableCheckoutUrl =
                    result.Status == "READY" ? result.CheckoutUrl : null;
            }
            else if (!TryClearCheckoutRecoveryState())
            {
                PurchaseCheckoutStatus = "RECOVERY_STATE_LOCKED";
                PurchaseCheckoutMessage =
                    "Checkout was not created, but BKE could not clear its recovery lock safely. Check the saved attempt before retrying.";
                RaiseCheckoutRecoveryState();
                return;
            }
            RaiseCheckoutRecoveryState();

            PurchaseCheckoutStatus = result.Status;
            PurchaseCheckoutMessage = result.Message ?? result.Status switch
            {
                "READY" when result.Complimentary == true =>
                    "Purchase completed without payment. Refreshing your BKE software.",
                "READY" =>
                    purchaseMode == "GIFT"
                        ? "Secure payment opened. After confirmed payment, the purchaser account receives an unbound Claim Code."
                        : "Secure payment opened. Your entitlement is issued only after confirmed payment.",
                "LEGAL_REACCEPTANCE_REQUIRED" =>
                    "Current BKE Legal documents changed. Review the purchase again.",
                "LEGAL_ACCEPTANCE_REQUIRED" =>
                    "The required Legal acceptance was not accepted by Digital Solutions. Review the purchase again.",
                "GIFT_CHECKOUT_DISABLED" =>
                    "Gift Claim Code purchase is no longer available in this environment.",
                "PLAN_NOT_AVAILABLE" =>
                    "The selected plan changed or is no longer available. Refresh the Store.",
                "ACCOUNT_FORBIDDEN" =>
                    "This BKE account cannot make this purchase.",
                "ACCOUNT_UNAVAILABLE" =>
                    "This BKE account is not currently available for purchases.",
                "CHECKOUT_IN_PROGRESS" =>
                    "A checkout creation attempt already exists. Check this attempt instead of creating another checkout.",
                "RESULT_UNKNOWN" =>
                    "Checkout status is uncertain. Check this attempt before trying anything else; do not retry checkout creation.",
                _ =>
                    "Checkout could not be started. Review the purchase again before another attempt.",
            };

            if (result.Status == "READY" &&
                result.Complimentary == true &&
                purchaseMode == "GIFT")
            {
                _recoverableCheckoutUrl = null;
                await RevealGiftClaimCodeAsync(
                    result.CorrelationId,
                    cancellationToken);
                return;
            }

            if (result.Status == "READY" &&
                !string.IsNullOrWhiteSpace(result.CheckoutUrl))
            {
                try
                {
                    _externalNavigator.OpenCheckout(result.CheckoutUrl);
                }
                catch (Exception navigationError) when (
                    navigationError is InvalidDataException or
                    System.ComponentModel.Win32Exception or
                    InvalidOperationException)
                {
                    PurchaseCheckoutStatus = "NAVIGATION_FAILED";
                    PurchaseCheckoutMessage =
                        "Checkout already exists, but the secure payment page could not be opened. Reopen this existing checkout or check its status; do not create another checkout.";
                    return;
                }

                if (result.Complimentary == true)
                {
                    TryClearCheckoutRecoveryState();
                    await RefreshCatalogAsync(cancellationToken);
                    await RefreshStoreAsync(cancellationToken);
                }
            }
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException)
        {
            PurchaseCheckoutStatus = "RESULT_UNKNOWN";
            PurchaseCheckoutMessage =
                "The Agent checkout result could not be confirmed. Check this exact attempt. Do not retry automatically or start another checkout.";
            RaiseCheckoutRecoveryState();
        }
        catch (Exception error) when (
            error is InvalidDataException or
            ArgumentException)
        {
            PurchaseCheckoutStatus = "RESULT_UNKNOWN";
            PurchaseCheckoutMessage =
                "The Agent checkout response was invalid after checkout-start. BKE preserved this exact recovery correlation. Check this attempt. Do not retry automatically or start another checkout.";
            RaiseCheckoutRecoveryState();
        }
    }

    public async Task CheckPurchaseCheckoutStatusAsync(
        CancellationToken cancellationToken)
    {
        var correlationId = _checkoutRecoveryCorrelationId;
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            PurchaseCheckoutStatus = "RECOVERY_UNAVAILABLE";
            PurchaseCheckoutMessage =
                "There is no recoverable checkout attempt in this purchase review.";
            return;
        }

        PurchaseCheckoutStatus = "CHECKING";
        PurchaseCheckoutMessage =
            "Checking the existing checkout attempt through the BKE Licensing Agent…";

        try
        {
            var result = await _storeCheckoutStatus.CheckAsync(
                correlationId,
                cancellationToken);

            PurchaseCheckoutStatus = result.Status;

            if (result.Status == "FOUND")
            {
                _checkoutRecoveryCorrelationId = result.CorrelationId;
                _recoverableCheckoutUrl =
                    result.PaymentStatus is "NOT_STARTED" or "CREATING" or "PENDING"
                        ? result.CheckoutUrl
                        : null;

                var orderLabel = string.IsNullOrWhiteSpace(result.OrderNumber)
                    ? "The existing order"
                    : $"Order {result.OrderNumber}";

                PurchaseCheckoutMessage = result.PaymentStatus switch
                {
                    "SETTLED" =>
                        $"{orderLabel} is settled. Refreshing your BKE software and Store.",
                    "NOT_REQUIRED" =>
                        $"{orderLabel} completed without payment. Refreshing your BKE software and Store.",
                    "PENDING" when !string.IsNullOrWhiteSpace(result.CheckoutUrl) =>
                        $"{orderLabel} is awaiting payment. Open the existing secure checkout; this does not create another order.",
                    "PENDING" =>
                        $"{orderLabel} is awaiting payment. Check again later; do not create another checkout.",
                    "CREATING" =>
                        $"{orderLabel} is still preparing its secure checkout. Check again; do not create another checkout.",
                    "NOT_STARTED" when !string.IsNullOrWhiteSpace(result.CheckoutUrl) =>
                        $"{orderLabel} exists and is ready to resume through the existing secure checkout.",
                    "NOT_STARTED" =>
                        $"{orderLabel} exists, but its secure checkout is not available yet. Check again.",
                    "FAILED" =>
                        $"{orderLabel} has a failed payment state. Review the current plan again before starting a new checkout.",
                    "CANCELLED" =>
                        $"{orderLabel} is cancelled. Review the current plan again before starting a new checkout.",
                    _ =>
                        $"{orderLabel} was recovered with status {result.OrderStatus ?? result.PaymentStatus ?? "UNKNOWN"}.",
                };

                if (result.FulfillmentMode == "CLAIM_CODE" &&
                    (result.PaymentStatus is "SETTLED" or "NOT_REQUIRED"))
                {
                    await RevealGiftClaimCodeAsync(
                        result.CorrelationId,
                        cancellationToken);
                }
                else if (result.PaymentStatus is "SETTLED" or "NOT_REQUIRED")
                {
                    TryClearCheckoutRecoveryState();
                    await RefreshCatalogAsync(cancellationToken);
                    await RefreshStoreAsync(cancellationToken);
                }
                else if (result.PaymentStatus is "FAILED" or "CANCELLED")
                {
                    TryClearCheckoutRecoveryState();
                }
            }
            else if (result.Status == "NOT_FOUND")
            {
                _recoverableCheckoutUrl = null;
                PurchaseCheckoutMessage =
                    "No durable checkout is visible for this correlation. NOT_FOUND is not treated as proof that no mutation occurred, so this recovery remains locked. Repeat the read-only check later; do not start a second checkout.";
            }
            else
            {
                PurchaseCheckoutMessage = result.Message ?? result.Status switch
                {
                    "AUTH_REQUIRED" =>
                        "Sign in again before checking this checkout attempt.",
                    "ACCOUNT_FORBIDDEN" =>
                        "This account cannot read the existing checkout state.",
                    "UNAVAILABLE" =>
                        "Checkout status is temporarily unavailable. This read-only check may be repeated; do not start another checkout.",
                    _ =>
                        "The existing checkout state could not be verified. Do not start another checkout.",
                };
            }

            RaiseCheckoutRecoveryState();
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException)
        {
            PurchaseCheckoutStatus = "UNAVAILABLE";
            PurchaseCheckoutMessage =
                "The read-only checkout-status check is temporarily unavailable. Check again; do not start another checkout.";
            RaiseCheckoutRecoveryState();
        }
        catch (Exception error) when (
            error is InvalidDataException or
            ArgumentException)
        {
            PurchaseCheckoutStatus = "FAILED";
            PurchaseCheckoutMessage =
                "The Agent checkout-status response was invalid. Do not start another checkout.";
            RaiseCheckoutRecoveryState();
        }
    }

    private async Task RevealGiftClaimCodeAsync(
        string correlationId,
        CancellationToken cancellationToken)
    {
        PurchaseCheckoutStatus = "REVEALING_GIFT_CLAIM_CODE";
        PurchaseCheckoutMessage =
            "Recovering the settled gift Claim Code through the BKE Licensing Agent…";

        try
        {
            var result = await _storeGiftClaimReveal.RevealAsync(
                correlationId,
                cancellationToken);

            switch (result.Status)
            {
                case "AVAILABLE":
                    GiftClaimCode = result.ClaimCode ?? string.Empty;
                    _recoverableCheckoutUrl = null;
                    PurchaseCheckoutStatus = "GIFT_CLAIM_CODE_READY";
                    PurchaseCheckoutMessage =
                        "Your unbound one-time Claim Code is ready. Copy or save it before acknowledging delivery. BKE Launcher does not persist this plaintext code.";
                    break;

                case "PENDING":
                    PurchaseCheckoutStatus = "GIFT_FULFILLMENT_PENDING";
                    PurchaseCheckoutMessage =
                        result.Message ??
                        "Payment is complete, but the gift Claim Code is still being finalized. Check this existing purchase again; do not create another checkout.";
                    break;

                case "AUTH_REQUIRED":
                    PurchaseCheckoutStatus = "AUTH_REQUIRED";
                    PurchaseCheckoutMessage =
                        result.Message ??
                        "The same Agent account session is required to recover this gift Claim Code.";
                    break;

                case "NOT_FOUND":
                    PurchaseCheckoutStatus = "GIFT_FULFILLMENT_NOT_FOUND";
                    PurchaseCheckoutMessage =
                        "No gift fulfillment is visible for this retained correlation. NOT_FOUND does not unlock another checkout; repeat the read-only recovery later.";
                    break;

                case "ACCOUNT_FORBIDDEN":
                case "ACCOUNT_UNAVAILABLE":
                case "NOT_GIFT_ORDER":
                case "FAILED":
                    PurchaseCheckoutStatus = result.Status;
                    PurchaseCheckoutMessage =
                        result.Message ??
                        "Gift Claim Code recovery failed closed. The retained correlation remains locked.";
                    break;

                case "UNAVAILABLE":
                    PurchaseCheckoutStatus = "UNAVAILABLE";
                    PurchaseCheckoutMessage =
                        result.Message ??
                        "Gift Claim Code recovery is temporarily unavailable. Retry this recovery; do not create another checkout.";
                    break;

                case "CANCELLED":
                case "ALREADY_USED":
                case "REVOKED":
                case "EXPIRED":
                    GiftClaimCode = string.Empty;
                    PurchaseCheckoutStatus = result.Status;
                    PurchaseCheckoutMessage =
                        result.Message ??
                        "This gift fulfillment is terminal and cannot deliver a usable Claim Code.";
                    if (TryClearCheckoutRecoveryState())
                    {
                        PurchaseReviewMessage =
                            "This gift fulfillment is terminal. Review the current plan and Legal terms again before another purchase.";
                        RaisePurchaseActionState();
                    }
                    break;

                default:
                    PurchaseCheckoutStatus = "FAILED";
                    PurchaseCheckoutMessage =
                        "Gift Claim Code recovery returned an unknown state. The retained correlation remains locked.";
                    break;
            }

            RaiseCheckoutRecoveryState();
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException)
        {
            PurchaseCheckoutStatus = "UNAVAILABLE";
            PurchaseCheckoutMessage =
                "The Agent gift Claim Code capability is temporarily unavailable. Retry this recovery; do not create another checkout.";
            RaiseCheckoutRecoveryState();
        }
        catch (Exception error) when (
            error is InvalidDataException or
            ArgumentException)
        {
            PurchaseCheckoutStatus = "FAILED";
            PurchaseCheckoutMessage =
                "The Agent gift Claim Code response was invalid. The retained correlation remains locked.";
            RaiseCheckoutRecoveryState();
        }
    }

    public void CompleteGiftClaimDelivery()
    {
        if (!CanCompleteGiftDelivery)
        {
            PurchaseCheckoutMessage =
                "A revealed gift Claim Code must remain visible until you save it and acknowledge delivery.";
            return;
        }

        if (!TryClearCheckoutRecoveryState())
        {
            PurchaseCheckoutStatus = "RECOVERY_STATE_LOCKED";
            PurchaseCheckoutMessage =
                "The Claim Code remains visible because BKE could not safely clear its recovery lock.";
            return;
        }

        GiftClaimCode = string.Empty;
        _purchaseAttemptLocked = false;
        ClearPurchaseReview();
        StoreMessage =
            "Gift Claim Code delivery acknowledged. Start another purchase only after reviewing the current plan and Legal terms again.";
        RaisePurchaseActionState();
        RaiseCheckoutRecoveryState();
    }

    public void OpenExistingCheckout()
    {
        if (string.IsNullOrWhiteSpace(_recoverableCheckoutUrl))
        {
            PurchaseCheckoutMessage =
                "Check the existing checkout status before trying to reopen payment.";
            return;
        }

        try
        {
            _externalNavigator.OpenCheckout(_recoverableCheckoutUrl);
            PurchaseCheckoutMessage =
                "Opened the existing secure checkout. No new order or payment attempt was created.";
        }
        catch (Exception error) when (
            error is InvalidDataException or
            System.ComponentModel.Win32Exception or
            InvalidOperationException)
        {
            PurchaseCheckoutStatus = "NAVIGATION_FAILED";
            PurchaseCheckoutMessage =
                "The existing secure checkout could not be opened. Its correlation remains recoverable.";
        }
    }

    public void OpenLegalDocument(PurchaseLegalDocumentViewModel document)
    {
        try
        {
            _externalNavigator.OpenLegalDocument(document.Slug);
        }
        catch (Exception error) when (
            error is InvalidDataException or
            ArgumentException or
            System.ComponentModel.Win32Exception)
        {
            PurchaseCheckoutStatus = "NAVIGATION_FAILED";
            PurchaseCheckoutMessage =
                "The authoritative Legal document could not be opened.";
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

    public async Task UpdateProductAsync(
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

        if (index < 0 || !Products[index].CanUpdate)
        {
            return;
        }

        var previous = Products[index];
        Products[index] = previous with
        {
            StateLabel = "Updating",
            CanInstall = false,
            CanUpdate = false,
            CanRepair = false,
            CanOpen = false,
            CanRemove = false,
        };
        CatalogStatus = "UPDATING";
        CatalogMessage =
            $"Starting verified update for {previous.DisplayName}…";

        try
        {
            var response = await _softwareUpdate.UpdateAsync(
                productId,
                cancellationToken);

            switch (response.Status)
            {
                case "STARTED":
                case "IN_PROGRESS":
                    CatalogStatus = "UPDATING";
                    CatalogMessage = response.Status == "STARTED"
                        ? $"Verified update started for {previous.DisplayName}. Refresh software after the elevation step completes."
                        : $"An update is already in progress for {previous.DisplayName}.";
                    return;

                case "UP_TO_DATE":
                    await RefreshCatalogAsync(cancellationToken);
                await RefreshStoreAsync(cancellationToken);
                    CatalogStatus = "READY";
                    CatalogMessage =
                        $"{previous.DisplayName} is already up to date.";
                    return;

                case "NOT_INSTALLED":
                    await RefreshCatalogAsync(cancellationToken);
                await RefreshStoreAsync(cancellationToken);
                    CatalogMessage =
                        $"{previous.DisplayName} is no longer installed on this machine.";
                    return;

                case "AUTH_REQUIRED":
                    Products[index] = previous with
                    {
                        CanUpdate = false,
                    };
                    CatalogStatus = "AUTH_REQUIRED";
                    CatalogMessage =
                        response.Error?.Message ??
                        "Sign in with BKE before updating software.";
                    return;

                default:
                    Products[index] = previous;
                    CatalogStatus = "FAILED";
                    CatalogMessage =
                        response.Error?.Message ??
                        $"Update failed: {response.State}.";
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
                "The BKE Licensing Agent update capability is unavailable or invalid.";
        }
    }

    public async Task RepairProductAsync(
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

        if (index < 0 || !Products[index].CanRepair)
        {
            return;
        }

        var previous = Products[index];
        Products[index] = previous with
        {
            StateLabel = "Repairing",
            CanInstall = false,
            CanUpdate = false,
            CanRepair = false,
            CanOpen = false,
            CanRemove = false,
        };
        CatalogStatus = "REPAIRING";
        CatalogMessage =
            $"Starting verified Repair for {previous.DisplayName}…";

        try
        {
            var response = await _softwareRepair.RepairAsync(
                productId,
                cancellationToken);

            switch (response.Status)
            {
                case "STARTED":
                case "IN_PROGRESS":
                    CatalogStatus = "REPAIRING";
                    CatalogMessage = response.Status == "STARTED"
                        ? $"Verified Repair started for {previous.DisplayName}. Refresh software after the elevation step completes."
                        : $"Repair is already in progress for {previous.DisplayName}.";
                    return;

                case "NOT_INSTALLED":
                    await RefreshCatalogAsync(cancellationToken);
                await RefreshStoreAsync(cancellationToken);
                    CatalogMessage =
                        $"{previous.DisplayName} is no longer installed on this machine.";
                    return;

                case "AUTH_REQUIRED":
                    Products[index] = previous with
                    {
                        CanRepair = false,
                    };
                    CatalogStatus = "AUTH_REQUIRED";
                    CatalogMessage =
                        response.Error?.Message ??
                        "Sign in with BKE before repairing software.";
                    return;

                default:
                    Products[index] = previous;
                    CatalogStatus = "FAILED";
                    CatalogMessage =
                        response.Error?.Message ??
                        $"Repair failed: {response.State}.";
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
                "The BKE Licensing Agent Repair capability is unavailable or invalid.";
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
            CanUpdate = false,
            CanRepair = false,
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
                await RefreshStoreAsync(cancellationToken);
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
        if (_purchaseAttemptLocked &&
            !string.IsNullOrWhiteSpace(_checkoutRecoveryCorrelationId))
        {
            PurchaseCheckoutStatus = "RECOVERY_REQUIRED";
            PurchaseCheckoutMessage =
                "Resolve the existing checkout attempt before signing out. Checkout recovery is bound to this same Agent account session.";
            Message =
                "Sign-out is blocked while checkout recovery depends on the current Agent account session.";
            RaiseCheckoutRecoveryState();
            return;
        }

        try
        {
            var response = await _accountSession.LogoutAsync(cancellationToken);
            SessionStatus = response.Status;
            AccountDisplay = "Not signed in";
            UserCode = string.Empty;
            VerificationUri = string.Empty;
            Password = string.Empty;
            ClaimCode = string.Empty;
            ClaimStatus = "AUTH_REQUIRED";
            ClaimMessage = "Sign in to redeem a Claim Code.";
            AvailableAccounts.Clear();
            SelectedAccount = null;
            Raise(nameof(HasAccountChoices));
            Message = response.Error?.Message ?? "Signed out on this machine.";
            ClearCatalog(
                "AUTH_REQUIRED",
                "Sign in to load your BKE software.");
            ClearStore(
                "AUTH_REQUIRED",
                "Sign in to browse the BKE Store.");
            ClearStore(
                "AUTH_REQUIRED",
                "Sign in to browse the BKE Store.");
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

    private void ClearStore(string status, string message)
    {
        StoreStatus = status;
        StoreMessage = message;
        GiftCheckoutEnabled = false;
        StoreProducts.Clear();
        Raise(nameof(ShowEmptyStore));
        ClearPurchaseReview();
    }

    private void ClearPurchaseReview()
    {
        var preserveRecovery =
            _checkoutRecoveryStateBlocked ||
            !string.IsNullOrWhiteSpace(_checkoutRecoveryCorrelationId);

        PurchaseReviewStatus = preserveRecovery
            ? (_checkoutRecoveryStateBlocked
                ? "RECOVERY_STATE_INVALID"
                : "RECOVERY_REQUIRED")
            : "IDLE";
        PurchaseReviewMessage = preserveRecovery
            ? (_checkoutRecoveryStateBlocked
                ? "BKE cannot safely read the saved checkout recovery state. New checkout creation remains blocked."
                : "A previous checkout attempt must be recovered before another purchase can be reviewed.")
            : "Select a plan to review the current purchase terms.";
        PurchaseReviewProductLabel = string.Empty;
        PurchaseReviewEditionLabel = string.Empty;
        PurchaseReviewPriceLabel = string.Empty;
        PurchaseReviewModesLabel = string.Empty;
        PurchaseReviewLegalLabel = string.Empty;
        PurchaseLegalDocuments.Clear();
        _reviewedPurchasePlanId = null;
        _reviewedPurchaseModes = new HashSet<string>(StringComparer.Ordinal);

        if (!preserveRecovery)
        {
            GiftClaimCode = string.Empty;
            _purchaseAttemptLocked = false;
            PurchaseCheckoutStatus = "IDLE";
            PurchaseCheckoutMessage = "Review a plan before starting checkout.";
        }
        else
        {
            _purchaseAttemptLocked = true;
            _recoverableCheckoutUrl = null;
            PurchaseCheckoutStatus = _checkoutRecoveryStateBlocked
                ? "RECOVERY_STATE_INVALID"
                : "RECOVERY_REQUIRED";
            PurchaseCheckoutMessage = _checkoutRecoveryStateBlocked
                ? "Saved checkout recovery state is invalid. BKE will not create another checkout."
                : "Keep the same Agent account session, then check the saved checkout attempt. Signing out or replacing the Agent session can make this correlation unrecoverable.";
        }

        ShowPurchaseReview = preserveRecovery;
        RaisePurchaseActionState();
        RaiseCheckoutRecoveryState();
    }

    private void RestoreCheckoutRecoveryState()
    {
        try
        {
            var correlationId = _checkoutRecoveryStore.ReadCorrelationId();
            if (string.IsNullOrWhiteSpace(correlationId))
            {
                return;
            }

            _purchaseAttemptLocked = true;
            _checkoutRecoveryCorrelationId = correlationId;
            _recoverableCheckoutUrl = null;
            _checkoutRecoveryStateBlocked = false;
            ShowPurchaseReview = true;
            PurchaseReviewStatus = "RECOVERY_REQUIRED";
            PurchaseReviewMessage =
                "A previous checkout attempt must be recovered before another purchase can be reviewed.";
            PurchaseCheckoutStatus = "RECOVERY_REQUIRED";
            PurchaseCheckoutMessage =
                "Keep the same Agent account session, then check this saved checkout attempt. Signing out or replacing the Agent session can make this correlation unrecoverable.";
        }
        catch (Exception storageError) when (
            storageError is IOException or
            UnauthorizedAccessException or
            InvalidDataException or
            InvalidOperationException)
        {
            _purchaseAttemptLocked = true;
            _checkoutRecoveryCorrelationId = null;
            _recoverableCheckoutUrl = null;
            _checkoutRecoveryStateBlocked = true;
            ShowPurchaseReview = true;
            PurchaseReviewStatus = "RECOVERY_STATE_INVALID";
            PurchaseReviewMessage =
                "BKE cannot safely read the saved checkout recovery state. New checkout creation remains blocked.";
            PurchaseCheckoutStatus = "RECOVERY_STATE_INVALID";
            PurchaseCheckoutMessage =
                "Saved checkout recovery state is invalid. BKE will not create another checkout.";
        }
    }

    private bool TryClearCheckoutRecoveryState()
    {
        try
        {
            _checkoutRecoveryStore.Clear();
            _checkoutRecoveryCorrelationId = null;
            _recoverableCheckoutUrl = null;
            _checkoutRecoveryStateBlocked = false;
            RaiseCheckoutRecoveryState();
            return true;
        }
        catch (Exception storageError) when (
            storageError is IOException or
            UnauthorizedAccessException or
            InvalidOperationException)
        {
            return false;
        }
    }

    private void RaisePurchaseActionState()
    {
        Raise(nameof(ShowPurchaseActions));
        Raise(nameof(CanBuySelf));
        Raise(nameof(CanBuyGift));
        Raise(nameof(ShowPurchaseCheckoutState));
    }

    private void RaiseCheckoutRecoveryState()
    {
        Raise(nameof(CanCheckCheckoutStatus));
        Raise(nameof(CanOpenExistingCheckout));
        Raise(nameof(CanCompleteGiftDelivery));
        Raise(nameof(ShowPurchaseCheckoutState));
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

public sealed class PurchaseLegalDocumentViewModel : INotifyPropertyChanged
{
    private bool _isAccepted;

    private PurchaseLegalDocumentViewModel(
        string title,
        string version,
        string slug,
        string documentVersionId)
    {
        Title = title;
        Version = version;
        Slug = slug;
        DocumentVersionId = documentVersionId;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Title { get; }
    public string Version { get; }
    public string Slug { get; }
    public string DocumentVersionId { get; }
    public string Label => $"{Title} v{Version}";

    public bool IsAccepted
    {
        get => _isAccepted;
        set
        {
            if (_isAccepted == value)
            {
                return;
            }

            _isAccepted = value;
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nameof(IsAccepted)));
        }
    }

    public static PurchaseLegalDocumentViewModel From(
        StoreCheckoutReviewLegalDocument document) =>
        new(
            document.Title,
            document.Version,
            document.Slug,
            document.DocumentVersionId);
}

public sealed record StoreProductViewModel(
    string ProductId,
    string DisplayName,
    string Summary,
    string Description,
    IReadOnlyList<StoreEditionViewModel> Editions)
{
    public static StoreProductViewModel From(StoreCatalogProduct product) =>
        new(
            product.ProductId,
            product.DisplayName,
            product.Summary,
            product.Description,
            product.Editions.Select(StoreEditionViewModel.From).ToArray());
}

public sealed record StoreEditionViewModel(
    string Name,
    string Description,
    string UsageLabel,
    string FeatureSummary,
    IReadOnlyList<StorePlanViewModel> Plans)
{
    public static StoreEditionViewModel From(StoreCatalogEdition edition) =>
        new(
            edition.Name,
            edition.Description ?? string.Empty,
            $"Up to {edition.MaxUsers} user(s) · {edition.MaxDevicesPerUser} device(s) per user · updates {edition.UpdatePolicy.Replace("_", " ").ToLowerInvariant()}",
            edition.Features.Count == 0
                ? "No capability summary supplied."
                : string.Join(" · ", edition.Features),
            edition.Plans.Select(StorePlanViewModel.From).ToArray());
}

public sealed record StorePlanViewModel(
    string PurchasePlanId,
    string TypeLabel,
    string PriceLabel,
    string DetailLabel)
{
    public static StorePlanViewModel From(StoreCatalogPlan plan)
    {
        var culture = CultureInfo.GetCultureInfo("en-PH");
        var amount = string.Format(
            culture,
            "{0:C}",
            plan.AmountMinor / 100m);

        var type = plan.Type switch
        {
            "PERPETUAL" => "Perpetual",
            "MONTHLY" => "Monthly",
            "ANNUAL" => "Annual",
            _ => plan.Type,
        };

        var suffix = plan.IntervalUnit switch
        {
            "MONTH" => " / month",
            "YEAR" => " / year",
            _ => string.Empty,
        };

        var renewal = plan.RenewalBehavior == "NONE"
            ? "No renewal"
            : "Customer-authorized renewal";

        var savings = plan.SavingsMinor > 0
            ? $" · saves {string.Format(culture, "{0:C}", plan.SavingsMinor / 100m)}"
            : string.Empty;

        return new StorePlanViewModel(
            plan.PurchasePlanId,
            type,
            amount + suffix,
            renewal + savings);
    }
}

public sealed record SoftwareProductViewModel(
    string ProductId,
    string DisplayName,
    string Summary,
    string ExecutionLabel,
    string StateLabel,
    string VersionLabel,
    bool CanInstall,
    bool CanUpdate,
    bool CanRepair,
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
            product.State == LauncherProductState.UpdateAvailable,
            product.ExecutionType == ProductExecutionType.Standalone &&
            product.State is LauncherProductState.Installed
                or LauncherProductState.UpdateAvailable
                or LauncherProductState.RepairRequired,
            product.ExecutionType == ProductExecutionType.Standalone &&
            product.State is LauncherProductState.Installed or LauncherProductState.UpdateAvailable,
            product.ExecutionType == ProductExecutionType.Standalone &&
            product.State is LauncherProductState.Installed
                or LauncherProductState.UpdateAvailable
                or LauncherProductState.InstalledNotEntitled
                or LauncherProductState.RepairRequired);
    }
}
