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
    private readonly LauncherNativeRegistrationController _nativeRegistration;
    private readonly LauncherPasswordResetRequestController _passwordResetRequest;
    private readonly LauncherCatalogService _catalog;
    private readonly LauncherStoreService _store;
    private readonly LauncherStoreCheckoutReviewService _storeCheckoutReview;
    private readonly LauncherStoreCheckoutStartService _storeCheckoutStart;
    private readonly LauncherStoreCheckoutStatusService _storeCheckoutStatus;
    private readonly LauncherStoreGiftClaimRevealService _storeGiftClaimReveal;
    private readonly LauncherNotificationInboxService _notifications;
    private readonly ILauncherCheckoutRecoveryStore _checkoutRecoveryStore;
    private readonly ILauncherExternalNavigator _externalNavigator;
    private readonly LauncherSoftwareInstallController _softwareInstall;
    private readonly LauncherSoftwareUpdateController _softwareUpdate;
    private readonly LauncherSoftwareRepairController _softwareRepair;
    private readonly LauncherSoftwareOpenController _softwareOpen;
    private readonly LauncherSoftwareRemoveController _softwareRemove;
    private readonly LauncherClaimCodeRedemptionController _claimCodeRedemption;
    private readonly LauncherAccountPasswordChangeController _accountPasswordChange;
    private readonly LauncherAccountMfaController _accountMfa;
    private readonly LauncherAccountPrivacyController _accountPrivacy;
    private readonly LauncherAccountOrganizationController _accountOrganization;
    private string _sessionStatus = "SIGNED_OUT";
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _passwordResetStatus = "IDLE";
    private string _passwordResetMessage =
        "Forgot your password? Request a one-time reset link by email.";
    private bool _showRegistration;
    private string _registrationName = string.Empty;
    private string _registrationEmail = string.Empty;
    private string _registrationPassword = string.Empty;
    private string _registrationCode = string.Empty;
    private string _registrationStatus = "IDLE";
    private string _registrationMessage =
        "Create your BKE account without leaving the Launcher.";
    private NativeBkeAccountChoice? _selectedAccount;
    private string _accountDisplay = "Not signed in";
    private string _authenticatedAccountEmail = string.Empty;
    private string _authenticatedAccountType = string.Empty;
    private string _userCode = string.Empty;
    private string _verificationUri = string.Empty;
    private string _message = "Connect this Launcher to the BKE Licensing Agent.";
    private string _catalogStatus = "AUTH_REQUIRED";
    private string _catalogMessage = "Sign in to load your BKE software.";
    private string _claimCode = string.Empty;
    private string _claimStatus = "AUTH_REQUIRED";
    private string _claimMessage = "Sign in to redeem a Claim Code.";
    private string _currentPassword = string.Empty;
    private string _newPassword = string.Empty;
    private string _confirmNewPassword = string.Empty;
    private string _passwordChangeStatus = "IDLE";
    private string _passwordChangeMessage = "Use your current password to set a new BKE password.";
    private string _nativeMfaCode = string.Empty;
    private string? _nativeMfaChallengeToken;
    private string _nativeMfaReference = string.Empty;
    private string _nativeMfaMessage = string.Empty;
    private string _accountMfaStatus = "UNKNOWN";
    private string _accountMfaMessage = "Refresh MFA status to manage account security.";
    private bool _accountMfaEnabled;
    private bool _accountMfaEnrollmentPending;
    private int _accountMfaRecoveryCodesRemaining;
    private string _accountMfaCurrentPassword = string.Empty;
    private string _accountMfaCode = string.Empty;
    private string? _accountMfaChallengeToken;
    private string _accountMfaReference = string.Empty;
    private string _accountMfaChallengePurpose = string.Empty;
    private string _accountMfaRecoveryCodes = string.Empty;
    private string _accountPrivacyStatus = "UNKNOWN";
    private string _accountPrivacyMessage =
        "Refresh privacy requests to load the authoritative request types and history.";
    private string _accountPrivacySummary = string.Empty;
    private string? _selectedAccountPrivacyRequestType;
    private string _accountOrganizationStatus = "UNKNOWN";
    private string _accountOrganizationMessage =
        "Refresh organization details to load the Agent-authoritative selected-account overview.";
    private string _organizationCreateStatus = "IDLE";
    private string _organizationCreateMessage =
        "Create a BKE Organization, then switch accounts to select it.";
    private string _organizationCreateDisplayName = string.Empty;
    private string _organizationCreateLegalName = string.Empty;
    private string _organizationCreateBillingEmail = string.Empty;
    private string _organizationCreateRegistrationNumber = string.Empty;
    private string _organizationCreateTaxId = string.Empty;
    private bool _organizationCreateRetryBlocked;
    private string _organizationProfileUpdateStatus = "IDLE";
    private string _organizationProfileUpdateMessage =
        "Refresh organization details before editing the selected Organization.";
    private string _organizationEditDisplayName = string.Empty;
    private string _organizationEditLegalName = string.Empty;
    private string _organizationEditRegistrationNumber = string.Empty;
    private string _organizationEditBillingEmail = string.Empty;
    private string _organizationEditTaxId = string.Empty;
    private string _organizationInvitationAcceptanceStatus = "IDLE";
    private string _organizationInvitationAcceptanceMessage =
        "Enter an Organization invitation code to join it with the signed-in BKE identity.";
    private string _organizationInvitationAcceptanceCode = string.Empty;
    private string _organizationInvitationAcceptedRole = string.Empty;
    private string _organizationInvitationStatus = "IDLE";
    private string _organizationInvitationMessage =
        "Invite members after loading the Agent-authoritative Organization overview.";
    private string _organizationInvitationEmail = string.Empty;
    private string _organizationInvitationRole = "MEMBER";
    private string _organizationInvitationCode = string.Empty;
    private string _organizationMemberManagementStatus = "IDLE";
    private string _organizationMemberManagementMessage =
        "Manage members after loading the Agent-authoritative Organization overview.";
    private string _organizationOwnershipTransferStatus = "IDLE";
    private string _organizationOwnershipTransferMessage =
        "Transfer ownership only when Digital Solutions authorizes the selected Organization.";
    private string _organizationLeaveStatus = "IDLE";
    private string _organizationLeaveMessage =
        "Leave is available only when Digital Solutions authorizes the selected Organization membership.";
    private AccountOrganizationAccount? _organizationAccount;
    private AccountOrganizationPermissions? _organizationPermissions;
    private AccountOrganizationProfile? _organizationProfile;
    private AccountOrganizationCounts? _organizationCounts;
    private string? _organizationBillingEmail;
    private string? _organizationTaxId;
    private string _storeStatus = "AUTH_REQUIRED";
    private string _storeMessage = "Sign in to browse the BKE Store.";
    private string _notificationStatus = "AUTH_REQUIRED";
    private string _notificationMessage = "Sign in to view BKE notifications.";
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
    private LauncherCheckoutRecoveryState? _checkoutRecoveryState;
    private string? _checkoutRecoveryCorrelationId;
    private string? _recoverableCheckoutUrl;
    private bool _checkoutRecoveryStateBlocked;
    private int _selectedModuleIndex = -1;
    private bool _showAccountSurface;

    public MainWindowViewModel(
        LauncherAccountSessionController accountSession,
        LauncherNativeSignInController nativeSignIn,
        LauncherNativeRegistrationController nativeRegistration,
        LauncherPasswordResetRequestController passwordResetRequest,
        LauncherCatalogService catalog,
        LauncherStoreService store,
        LauncherStoreCheckoutReviewService storeCheckoutReview,
        LauncherStoreCheckoutStartService storeCheckoutStart,
        LauncherStoreCheckoutStatusService storeCheckoutStatus,
        LauncherStoreGiftClaimRevealService storeGiftClaimReveal,
        LauncherNotificationInboxService notifications,
        ILauncherCheckoutRecoveryStore checkoutRecoveryStore,
        ILauncherExternalNavigator externalNavigator,
        LauncherSoftwareInstallController softwareInstall,
        LauncherSoftwareUpdateController softwareUpdate,
        LauncherSoftwareRepairController softwareRepair,
        LauncherSoftwareOpenController softwareOpen,
        LauncherSoftwareRemoveController softwareRemove,
        LauncherClaimCodeRedemptionController claimCodeRedemption,
        LauncherAccountPasswordChangeController accountPasswordChange,
        LauncherAccountMfaController accountMfa,
        LauncherAccountPrivacyController accountPrivacy,
        LauncherAccountOrganizationController accountOrganization)
    {
        _accountSession = accountSession;
        _nativeSignIn = nativeSignIn;
        _nativeRegistration = nativeRegistration;
        _passwordResetRequest = passwordResetRequest;
        _catalog = catalog;
        _store = store;
        _storeCheckoutReview = storeCheckoutReview;
        _storeCheckoutStart = storeCheckoutStart;
        _storeCheckoutStatus = storeCheckoutStatus;
        _storeGiftClaimReveal = storeGiftClaimReveal;
        _notifications = notifications;
        _checkoutRecoveryStore = checkoutRecoveryStore;
        _externalNavigator = externalNavigator;
        _softwareInstall = softwareInstall;
        _softwareUpdate = softwareUpdate;
        _softwareRepair = softwareRepair;
        _softwareOpen = softwareOpen;
        _softwareRemove = softwareRemove;
        _claimCodeRedemption = claimCodeRedemption;
        _accountPasswordChange = accountPasswordChange;
        _accountMfa = accountMfa;
        _accountPrivacy = accountPrivacy;
        _accountOrganization = accountOrganization;
        RestoreCheckoutRecoveryState();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<SoftwareProductViewModel> Products { get; } = [];
    public ObservableCollection<StoreProductViewModel> StoreProducts { get; } = [];
    public ObservableCollection<NotificationViewModel> Notifications { get; } = [];
    public ObservableCollection<string> AccountPrivacyRequestTypes { get; } = [];
    public ObservableCollection<AccountPrivacyRequestViewModel> AccountPrivacyRequests { get; } = [];
    public ObservableCollection<AccountOrganizationMember> OrganizationMembers { get; } = [];
    public ObservableCollection<AccountOrganizationInvitation> OrganizationInvitations { get; } = [];
    public IReadOnlyList<string> OrganizationInvitationRoles { get; } =
        ["MEMBER", "LICENSE_MANAGER", "BILLING", "OWNER"];
    public ObservableCollection<PurchaseLegalDocumentViewModel> PurchaseLegalDocuments { get; } = [];
    public ObservableCollection<RegistrationLegalDocumentViewModel> RegistrationLegalDocuments { get; } = [];
    public ObservableCollection<NativeBkeAccountChoice> AvailableAccounts { get; } = [];

    public string RegistrationName
    {
        get => _registrationName;
        set
        {
            SetField(ref _registrationName, value);
            RaiseRegistrationCapabilities();
        }
    }

    public string RegistrationEmail
    {
        get => _registrationEmail;
        set
        {
            SetField(ref _registrationEmail, value);
            RaiseRegistrationCapabilities();
        }
    }

    public string RegistrationPassword
    {
        get => _registrationPassword;
        set
        {
            SetField(ref _registrationPassword, value);
            RaiseRegistrationCapabilities();
        }
    }

    public string RegistrationCode
    {
        get => _registrationCode;
        set
        {
            SetField(ref _registrationCode, value);
            RaiseRegistrationCapabilities();
        }
    }

    public string RegistrationStatus
    {
        get => _registrationStatus;
        private set
        {
            SetField(ref _registrationStatus, value);
            RaiseRegistrationCapabilities();
        }
    }

    public string RegistrationMessage
    {
        get => _registrationMessage;
        private set => SetField(ref _registrationMessage, value);
    }

    public bool ShowSignInForm => ShowLoginPage && !_showRegistration;
    public bool ShowRegistration => ShowLoginPage && _showRegistration;
    public bool ShowRegistrationEntry =>
        ShowRegistration && RegistrationStatus != "VERIFICATION_REQUIRED";
    public bool ShowRegistrationVerification =>
        ShowRegistration && RegistrationStatus == "VERIFICATION_REQUIRED";

    public bool CanCreateAccount =>
        ShowRegistrationEntry &&
        RegistrationStatus is not ("LOADING" or "CREATING") &&
        !string.IsNullOrWhiteSpace(RegistrationName) &&
        !string.IsNullOrWhiteSpace(RegistrationEmail) &&
        !string.IsNullOrEmpty(RegistrationPassword) &&
        RegistrationLegalDocuments.Count == 2 &&
        RegistrationLegalDocuments.All(document => document.IsAccepted);

    public bool CanVerifyRegistrationEmail =>
        ShowRegistrationVerification &&
        RegistrationStatus != "VERIFYING" &&
        RegistrationCode.Length == 8;

    public bool CanResendRegistrationEmail =>
        ShowRegistrationVerification &&
        RegistrationStatus != "RESENDING" &&
        !string.IsNullOrWhiteSpace(RegistrationEmail);

    public string Email
    {
        get => _email;
        set
        {
            SetField(ref _email, value);
            Raise(nameof(CanRequestPasswordReset));
        }
    }

    public string PasswordResetStatus
    {
        get => _passwordResetStatus;
        private set
        {
            SetField(ref _passwordResetStatus, value);
            Raise(nameof(CanRequestPasswordReset));
        }
    }

    public string PasswordResetMessage
    {
        get => _passwordResetMessage;
        private set => SetField(ref _passwordResetMessage, value);
    }

    public string Password
    {
        get => _password;
        set => SetField(ref _password, value);
    }

    public string ClaimCode
    {
        get => _claimCode;
        set
        {
            SetField(ref _claimCode, value);
            Raise(nameof(CanRedeemClaimCode));
        }
    }

    public string CurrentPassword
    {
        get => _currentPassword;
        set
        {
            SetField(ref _currentPassword, value);
            Raise(nameof(CanChangePassword));
        }
    }

    public string NewPassword
    {
        get => _newPassword;
        set
        {
            SetField(ref _newPassword, value);
            Raise(nameof(CanChangePassword));
        }
    }

    public string ConfirmNewPassword
    {
        get => _confirmNewPassword;
        set
        {
            SetField(ref _confirmNewPassword, value);
            Raise(nameof(CanChangePassword));
        }
    }

    public string PasswordChangeStatus
    {
        get => _passwordChangeStatus;
        private set
        {
            SetField(ref _passwordChangeStatus, value);
            Raise(nameof(CanChangePassword));
        }
    }

    public string PasswordChangeMessage
    {
        get => _passwordChangeMessage;
        private set => SetField(ref _passwordChangeMessage, value);
    }


    public string NativeMfaCode
    {
        get => _nativeMfaCode;
        set
        {
            SetField(ref _nativeMfaCode, value);
            Raise(nameof(CanVerifyNativeMfa));
        }
    }

    public string NativeMfaReference
    {
        get => _nativeMfaReference;
        private set => SetField(ref _nativeMfaReference, value);
    }

    public string NativeMfaMessage
    {
        get => _nativeMfaMessage;
        private set => SetField(ref _nativeMfaMessage, value);
    }

    public bool ShowNativeMfaChallenge =>
        ShowLoginPage && !string.IsNullOrWhiteSpace(_nativeMfaChallengeToken);

    public bool CanVerifyNativeMfa =>
        ShowNativeMfaChallenge &&
        SessionStatus != "VERIFYING_MFA" &&
        NativeMfaCode.Length is >= 6 and <= 32;

    public string AccountMfaStatus
    {
        get => _accountMfaStatus;
        private set
        {
            SetField(ref _accountMfaStatus, value);
            RaiseAccountMfaCapabilities();
        }
    }

    public string AccountMfaMessage
    {
        get => _accountMfaMessage;
        private set => SetField(ref _accountMfaMessage, value);
    }

    public string AccountMfaSummary =>
        AccountMfaStatus == "READY"
            ? AccountMfaEnabled
                ? $"Enabled · {AccountMfaRecoveryCodesRemaining} recovery code(s) remaining"
                : AccountMfaEnrollmentPending
                    ? "Enrollment pending"
                    : "Not enabled"
            : AccountMfaStatus;

    public bool AccountMfaEnabled
    {
        get => _accountMfaEnabled;
        private set
        {
            SetField(ref _accountMfaEnabled, value);
            Raise(nameof(AccountMfaSummary));
            RaiseAccountMfaCapabilities();
        }
    }

    public bool AccountMfaEnrollmentPending
    {
        get => _accountMfaEnrollmentPending;
        private set
        {
            SetField(ref _accountMfaEnrollmentPending, value);
            Raise(nameof(AccountMfaSummary));
        }
    }

    public int AccountMfaRecoveryCodesRemaining
    {
        get => _accountMfaRecoveryCodesRemaining;
        private set
        {
            SetField(ref _accountMfaRecoveryCodesRemaining, value);
            Raise(nameof(AccountMfaSummary));
        }
    }

    public string AccountMfaCurrentPassword
    {
        get => _accountMfaCurrentPassword;
        set
        {
            SetField(ref _accountMfaCurrentPassword, value);
            RaiseAccountMfaCapabilities();
        }
    }

    public string AccountMfaCode
    {
        get => _accountMfaCode;
        set
        {
            SetField(ref _accountMfaCode, value);
            RaiseAccountMfaCapabilities();
        }
    }

    public string AccountMfaReference
    {
        get => _accountMfaReference;
        private set => SetField(ref _accountMfaReference, value);
    }

    public string AccountMfaRecoveryCodes
    {
        get => _accountMfaRecoveryCodes;
        private set
        {
            SetField(ref _accountMfaRecoveryCodes, value);
            Raise(nameof(HasMfaRecoveryCodes));
        }
    }

    public bool HasMfaRecoveryCodes =>
        !string.IsNullOrWhiteSpace(AccountMfaRecoveryCodes);

    public bool HasAccountMfaChallenge =>
        IsAuthenticated &&
        !string.IsNullOrWhiteSpace(_accountMfaChallengeToken);

    public bool ShowMfaEnrollmentChallenge =>
        HasAccountMfaChallenge &&
        _accountMfaChallengePurpose == "ENROLL";

    public bool ShowMfaProofChallenge =>
        HasAccountMfaChallenge &&
        _accountMfaChallengePurpose == "PROOF";

    public bool ShowMfaEnableActions =>
        IsAuthenticated &&
        AccountMfaStatus == "READY" &&
        !AccountMfaEnabled;

    public bool ShowMfaProtectedActions =>
        IsAuthenticated &&
        AccountMfaStatus == "READY" &&
        AccountMfaEnabled;

    public bool CanStartMfaEnrollment =>
        ShowMfaEnableActions &&
        !string.IsNullOrEmpty(AccountMfaCurrentPassword);

    public bool CanStartMfaProof =>
        ShowMfaProtectedActions &&
        !string.IsNullOrEmpty(AccountMfaCurrentPassword);

    public bool CanCompleteMfaEnrollment =>
        ShowMfaEnrollmentChallenge &&
        AccountMfaCode.Length is >= 6 and <= 32 &&
        !string.IsNullOrEmpty(AccountMfaCurrentPassword);

    public bool CanSubmitMfaProof =>
        ShowMfaProofChallenge &&
        AccountMfaCode.Length is >= 6 and <= 32 &&
        !string.IsNullOrEmpty(AccountMfaCurrentPassword);


    public string AccountPrivacyStatus
    {
        get => _accountPrivacyStatus;
        private set
        {
            SetField(ref _accountPrivacyStatus, value);
            RaiseAccountPrivacyCapabilities();
        }
    }

    public string AccountPrivacyMessage
    {
        get => _accountPrivacyMessage;
        private set => SetField(ref _accountPrivacyMessage, value);
    }

    public string AccountPrivacySummary
    {
        get => _accountPrivacySummary;
        set
        {
            SetField(ref _accountPrivacySummary, value);
            Raise(nameof(CanCreateAccountPrivacyRequest));
        }
    }

    public string? SelectedAccountPrivacyRequestType
    {
        get => _selectedAccountPrivacyRequestType;
        set
        {
            SetField(ref _selectedAccountPrivacyRequestType, value);
            Raise(nameof(CanCreateAccountPrivacyRequest));
        }
    }

    public bool CanRefreshAccountPrivacy =>
        IsAuthenticated &&
        AccountPrivacyStatus is not ("LOADING" or "CREATING");

    public bool CanCreateAccountPrivacyRequest =>
        IsAuthenticated &&
        AccountPrivacyStatus == "READY" &&
        !string.IsNullOrWhiteSpace(SelectedAccountPrivacyRequestType) &&
        AccountPrivacyRequestTypes.Contains(
            SelectedAccountPrivacyRequestType,
            StringComparer.Ordinal) &&
        !string.IsNullOrWhiteSpace(AccountPrivacySummary) &&
        AccountPrivacySummary.Trim().Length is >= 10 and <= 2_000;

    public bool ShowEmptyAccountPrivacyRequests =>
        AccountPrivacyStatus == "READY" &&
        AccountPrivacyRequests.Count == 0;

    public string AccountOrganizationStatus
    {
        get => _accountOrganizationStatus;
        private set
        {
            SetField(ref _accountOrganizationStatus, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string AccountOrganizationMessage
    {
        get => _accountOrganizationMessage;
        private set => SetField(ref _accountOrganizationMessage, value);
    }

    public string OrganizationCreateStatus
    {
        get => _organizationCreateStatus;
        private set
        {
            SetField(ref _organizationCreateStatus, value);
            Raise(nameof(CanCreateOrganization));
        }
    }

    public string OrganizationCreateMessage
    {
        get => _organizationCreateMessage;
        private set => SetField(ref _organizationCreateMessage, value);
    }

    public string OrganizationCreateDisplayName
    {
        get => _organizationCreateDisplayName;
        set
        {
            SetField(ref _organizationCreateDisplayName, value);
            Raise(nameof(CanCreateOrganization));
        }
    }

    public string OrganizationCreateLegalName
    {
        get => _organizationCreateLegalName;
        set
        {
            SetField(ref _organizationCreateLegalName, value);
            Raise(nameof(CanCreateOrganization));
        }
    }

    public string OrganizationCreateBillingEmail
    {
        get => _organizationCreateBillingEmail;
        set
        {
            SetField(ref _organizationCreateBillingEmail, value);
            Raise(nameof(CanCreateOrganization));
        }
    }

    public string OrganizationCreateRegistrationNumber
    {
        get => _organizationCreateRegistrationNumber;
        set
        {
            SetField(ref _organizationCreateRegistrationNumber, value);
            Raise(nameof(CanCreateOrganization));
        }
    }

    public string OrganizationCreateTaxId
    {
        get => _organizationCreateTaxId;
        set
        {
            SetField(ref _organizationCreateTaxId, value);
            Raise(nameof(CanCreateOrganization));
        }
    }

    public bool ShowOrganizationCreateSection => IsAuthenticated;

    public bool CanCreateOrganization =>
        ShowOrganizationCreateSection &&
        OrganizationCreateStatus != "CREATING" &&
        !_organizationCreateRetryBlocked &&
        ValidOrganizationCreateText(
            OrganizationCreateDisplayName,
            2,
            120) &&
        ValidOrganizationCreateText(
            OrganizationCreateLegalName,
            2,
            180) &&
        ValidOrganizationCreateEmail(
            OrganizationCreateBillingEmail) &&
        OrganizationCreateRegistrationNumber.Trim().Length <= 80 &&
        OrganizationCreateTaxId.Trim().Length <= 80;

    public bool OrganizationCreateRetryBlocked =>
        _organizationCreateRetryBlocked;

    public string OrganizationProfileUpdateStatus
    {
        get => _organizationProfileUpdateStatus;
        private set
        {
            SetField(ref _organizationProfileUpdateStatus, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationProfileUpdateMessage
    {
        get => _organizationProfileUpdateMessage;
        private set =>
            SetField(ref _organizationProfileUpdateMessage, value);
    }

    public string OrganizationEditDisplayName
    {
        get => _organizationEditDisplayName;
        set
        {
            SetField(ref _organizationEditDisplayName, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationEditLegalName
    {
        get => _organizationEditLegalName;
        set
        {
            SetField(ref _organizationEditLegalName, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationEditRegistrationNumber
    {
        get => _organizationEditRegistrationNumber;
        set
        {
            SetField(ref _organizationEditRegistrationNumber, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationEditBillingEmail
    {
        get => _organizationEditBillingEmail;
        set
        {
            SetField(ref _organizationEditBillingEmail, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationEditTaxId
    {
        get => _organizationEditTaxId;
        set
        {
            SetField(ref _organizationEditTaxId, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public bool ShowOrganizationSection =>
        IsAuthenticated &&
        _authenticatedAccountType == "ORGANIZATION" &&
        AccountOrganizationStatus != "NOT_ORGANIZATION";

    public bool CanRefreshAccountOrganization =>
        ShowOrganizationSection &&
        AccountOrganizationStatus != "LOADING";

    public bool OrganizationReady =>
        AccountOrganizationStatus == "READY" &&
        _organizationAccount is not null &&
        _organizationProfile is not null &&
        _organizationCounts is not null;

    public string OrganizationDisplayName =>
        _organizationAccount?.DisplayName ?? string.Empty;

    public string OrganizationRole =>
        _organizationAccount?.Role ?? string.Empty;

    public string OrganizationLifecycle =>
        _organizationAccount?.LifecycleState ?? string.Empty;

    public string OrganizationLegalName =>
        _organizationProfile?.LegalName ?? string.Empty;

    public string OrganizationRegistrationNumber =>
        _organizationProfile?.RegistrationNumber ?? string.Empty;

    public bool HasOrganizationRegistrationNumber =>
        !string.IsNullOrWhiteSpace(
            _organizationProfile?.RegistrationNumber);

    public string OrganizationBillingEmail =>
        _organizationBillingEmail ?? string.Empty;

    public bool HasOrganizationBillingEmail =>
        !string.IsNullOrWhiteSpace(_organizationBillingEmail);

    public string OrganizationTaxId =>
        _organizationTaxId ?? string.Empty;

    public bool HasOrganizationTaxId =>
        !string.IsNullOrWhiteSpace(_organizationTaxId);

    public bool ShowOrganizationBilling =>
        HasOrganizationBillingEmail ||
        HasOrganizationTaxId;

    public bool HasOrganizationLicenseCount =>
        _organizationCounts?.Licenses is not null;

    public bool HasOrganizationSubscriptionCount =>
        _organizationCounts?.Subscriptions is not null;

    public bool HasOrganizationOrderCount =>
        _organizationCounts?.Orders is not null;

    public string OrganizationLicenseCount =>
        _organizationCounts?.Licenses?.ToString(
            CultureInfo.InvariantCulture) ?? string.Empty;

    public string OrganizationSubscriptionCount =>
        _organizationCounts?.Subscriptions?.ToString(
            CultureInfo.InvariantCulture) ?? string.Empty;

    public string OrganizationOrderCount =>
        _organizationCounts?.Orders?.ToString(
            CultureInfo.InvariantCulture) ?? string.Empty;

    public bool ShowOrganizationUsage =>
        HasOrganizationLicenseCount ||
        HasOrganizationSubscriptionCount ||
        HasOrganizationOrderCount;

    public bool CanEditOrganizationIdentity =>
        OrganizationReady &&
        _organizationPermissions?.ManageMembers == true;

    public bool CanEditOrganizationBilling =>
        OrganizationReady &&
        _organizationPermissions?.ViewBilling == true;

    public bool ShowOrganizationProfileEditor =>
        CanEditOrganizationIdentity ||
        CanEditOrganizationBilling;

    public bool OrganizationIdentityProfileDirty =>
        CanEditOrganizationIdentity &&
        (
            !string.Equals(
                OrganizationEditDisplayName.Trim(),
                _organizationAccount?.DisplayName ?? string.Empty,
                StringComparison.Ordinal) ||
            !string.Equals(
                OrganizationEditLegalName.Trim(),
                _organizationProfile?.LegalName ?? string.Empty,
                StringComparison.Ordinal) ||
            !string.Equals(
                NormalizeOrganizationOptional(
                    OrganizationEditRegistrationNumber),
                _organizationProfile?.RegistrationNumber,
                StringComparison.Ordinal)
        );

    public bool OrganizationBillingProfileDirty =>
        CanEditOrganizationBilling &&
        (
            !string.Equals(
                OrganizationEditBillingEmail.Trim(),
                _organizationBillingEmail ?? string.Empty,
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(
                NormalizeOrganizationOptional(
                    OrganizationEditTaxId),
                _organizationTaxId,
                StringComparison.Ordinal)
        );

    public bool CanSaveOrganizationProfile =>
        ShowOrganizationProfileEditor &&
        OrganizationProfileUpdateStatus != "UPDATING" &&
        (OrganizationIdentityProfileDirty ||
         OrganizationBillingProfileDirty) &&
        (!OrganizationIdentityProfileDirty ||
            (ValidOrganizationCreateText(
                OrganizationEditDisplayName,
                2,
                120) &&
             ValidOrganizationCreateText(
                OrganizationEditLegalName,
                2,
                180) &&
             OrganizationEditRegistrationNumber.Trim().Length <= 80)) &&
        (!OrganizationBillingProfileDirty ||
            (ValidOrganizationCreateEmail(
                OrganizationEditBillingEmail) &&
             OrganizationEditTaxId.Trim().Length <= 80));

    public string OrganizationInvitationAcceptanceCode
    {
        get => _organizationInvitationAcceptanceCode;
        set
        {
            SetField(
                ref _organizationInvitationAcceptanceCode,
                value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationInvitationAcceptanceStatus
    {
        get => _organizationInvitationAcceptanceStatus;
        private set
        {
            SetField(
                ref _organizationInvitationAcceptanceStatus,
                value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationInvitationAcceptanceMessage
    {
        get => _organizationInvitationAcceptanceMessage;
        private set =>
            SetField(
                ref _organizationInvitationAcceptanceMessage,
                value);
    }

    public string OrganizationInvitationAcceptedRole
    {
        get => _organizationInvitationAcceptedRole;
        private set =>
            SetField(
                ref _organizationInvitationAcceptedRole,
                value);
    }

    public bool ShowOrganizationInvitationAcceptanceSection =>
        IsAuthenticated;

    public bool CanAcceptOrganizationInvitation =>
        IsAuthenticated &&
        OrganizationInvitationAcceptanceStatus != "ACCEPTING" &&
        ValidOrganizationInvitationAcceptanceCode(
            OrganizationInvitationAcceptanceCode);

    public bool OrganizationInvitationAccepted =>
        OrganizationInvitationAcceptanceStatus == "ACCEPTED";

    public bool ShowOrganizationInvitationAcceptanceSwitch =>
        OrganizationInvitationAcceptanceStatus is
            "ACCEPTED" or "OUTCOME_UNKNOWN";

    public string OrganizationInvitationStatus
    {
        get => _organizationInvitationStatus;
        private set
        {
            SetField(ref _organizationInvitationStatus, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationInvitationMessage
    {
        get => _organizationInvitationMessage;
        private set =>
            SetField(ref _organizationInvitationMessage, value);
    }

    public string OrganizationInvitationEmail
    {
        get => _organizationInvitationEmail;
        set
        {
            SetField(ref _organizationInvitationEmail, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationInvitationRole
    {
        get => _organizationInvitationRole;
        set
        {
            SetField(ref _organizationInvitationRole, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationInvitationCode
    {
        get => _organizationInvitationCode;
        private set
        {
            SetField(ref _organizationInvitationCode, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public bool HasOrganizationInvitationCode =>
        !string.IsNullOrWhiteSpace(OrganizationInvitationCode);

    public bool CanManageOrganizationInvitations =>
        OrganizationReady &&
        _organizationPermissions?.ManageMembers == true;

    public bool ShowOrganizationInviteSection =>
        CanManageOrganizationInvitations ||
        (IsAuthenticated && HasOrganizationInvitationCode);

    public bool CanManageOrganizationInvitationActions =>
        CanManageOrganizationInvitations &&
        OrganizationInvitationStatus is not ("ISSUING" or "MANAGING") &&
        !HasOrganizationInvitationCode;

    public bool CanInviteOrganizationMember =>
        CanManageOrganizationInvitationActions &&
        ValidOrganizationCreateEmail(OrganizationInvitationEmail) &&
        ValidOrganizationMemberRole(OrganizationInvitationRole);

    public string OrganizationMemberManagementStatus
    {
        get => _organizationMemberManagementStatus;
        private set
        {
            SetField(ref _organizationMemberManagementStatus, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationMemberManagementMessage
    {
        get => _organizationMemberManagementMessage;
        private set =>
            SetField(ref _organizationMemberManagementMessage, value);
    }

    public bool CanManageOrganizationMemberActions =>
        OrganizationReady &&
        _organizationPermissions?.ManageMembers == true &&
        OrganizationMemberManagementStatus != "MANAGING";

    public bool ShowOrganizationMembers =>
        OrganizationReady &&
        _organizationPermissions?.ManageMembers == true;

    public bool ShowEmptyOrganizationMembers =>
        ShowOrganizationMembers &&
        OrganizationMembers.Count == 0;

    public bool ShowEmptyOrganizationInvitations =>
        ShowOrganizationMembers &&
        OrganizationInvitations.Count == 0;

    public string OrganizationOwnershipTransferStatus
    {
        get => _organizationOwnershipTransferStatus;
        private set
        {
            SetField(ref _organizationOwnershipTransferStatus, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationOwnershipTransferMessage
    {
        get => _organizationOwnershipTransferMessage;
        private set =>
            SetField(ref _organizationOwnershipTransferMessage, value);
    }

    public bool ShowOrganizationOwnershipTransferSection =>
        OrganizationReady &&
        _organizationPermissions?.TransferOwnership == true;

    public bool CanTransferOrganizationOwnership =>
        ShowOrganizationOwnershipTransferSection &&
        OrganizationOwnershipTransferStatus != "TRANSFERRING" &&
        !_purchaseAttemptLocked;

    public string OrganizationLeaveStatus
    {
        get => _organizationLeaveStatus;
        private set
        {
            SetField(ref _organizationLeaveStatus, value);
            RaiseAccountOrganizationCapabilities();
        }
    }

    public string OrganizationLeaveMessage
    {
        get => _organizationLeaveMessage;
        private set => SetField(ref _organizationLeaveMessage, value);
    }

    public bool ShowOrganizationLeaveSection =>
        OrganizationReady &&
        _organizationPermissions?.LeaveOrganization == true;

    public bool CanLeaveOrganization =>
        ShowOrganizationLeaveSection &&
        OrganizationLeaveStatus != "LEAVING" &&
        !_purchaseAttemptLocked;

    public NativeBkeAccountChoice? SelectedAccount
    {
        get => _selectedAccount;
        set => SetField(ref _selectedAccount, value);
    }

    public bool HasAccountChoices => AvailableAccounts.Count > 0;

    public bool IsAuthenticated =>
        string.Equals(SessionStatus, "AUTHENTICATED", StringComparison.Ordinal);

    public bool ShowLoginPage => !IsAuthenticated;
    public bool ShowAuthenticatedShell => IsAuthenticated;
    public bool CanRequestPasswordReset =>
        ShowLoginPage &&
        !ShowNativeMfaChallenge &&
        PasswordResetStatus != "REQUESTING" &&
        !string.IsNullOrWhiteSpace(Email);

    public int SelectedModuleIndex
    {
        get => _selectedModuleIndex;
        set => SetField(ref _selectedModuleIndex, value);
    }

    public bool ShowAccountSurface
    {
        get => _showAccountSurface;
        private set => SetField(ref _showAccountSurface, value);
    }

    public string SessionStatus
    {
        get => _sessionStatus;
        private set
        {
            SetField(ref _sessionStatus, value);
            Raise(nameof(IsAuthenticated));
            Raise(nameof(ShowLoginPage));
            Raise(nameof(ShowAuthenticatedShell));
            RaiseRegistrationCapabilities();
            Raise(nameof(CanRequestPasswordReset));
            Raise(nameof(CanRedeemClaimCode));
            Raise(nameof(CanChangePassword));
            Raise(nameof(ShowNativeMfaChallenge));
            Raise(nameof(CanVerifyNativeMfa));
            RaiseAccountMfaCapabilities();
            RaiseAccountPrivacyCapabilities();
            RaiseAccountOrganizationCapabilities();
            Raise(nameof(CanRefreshNotifications));
            Raise(nameof(CanCheckCheckoutStatus));
            Raise(nameof(CanRetryOriginalCheckout));
            Raise(nameof(CanSwitchAccount));
            Raise(nameof(SwitchAccountHint));
        }
    }

    public string AccountDisplay
    {
        get => _accountDisplay;
        private set => SetField(ref _accountDisplay, value);
    }

    public string AccountTypeLabel =>
        _authenticatedAccountType switch
        {
            "INDIVIDUAL" => "Personal account",
            "ORGANIZATION" => "Organization account",
            _ => string.Empty,
        };

    public bool CanSwitchAccount =>
        IsAuthenticated &&
        !_purchaseAttemptLocked;

    public string SwitchAccountHint =>
        _purchaseAttemptLocked
            ? "Account switching is locked until the existing checkout attempt is resolved for this exact BKE identity and account."
            : "Switching signs this machine out first, then requires fresh password/MFA and authoritative account selection.";

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
        private set
        {
            SetField(ref _claimStatus, value);
            Raise(nameof(CanRedeemClaimCode));
        }
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

    public string NotificationStatus
    {
        get => _notificationStatus;
        private set => SetField(ref _notificationStatus, value);
    }

    public string NotificationMessage
    {
        get => _notificationMessage;
        private set => SetField(ref _notificationMessage, value);
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
        private set
        {
            SetField(ref _purchaseCheckoutStatus, value);
            Raise(nameof(CanRetryOriginalCheckout));
        }
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

    public bool CanRetryOriginalCheckout =>
        string.Equals(SessionStatus, "AUTHENTICATED", StringComparison.Ordinal) &&
        PurchaseCheckoutStatus == "RETRY_AVAILABLE" &&
        _purchaseAttemptLocked &&
        !_checkoutRecoveryStateBlocked &&
        _checkoutRecoveryState?.HasResumeIntent == true &&
        !string.IsNullOrWhiteSpace(_checkoutRecoveryCorrelationId);

    public bool CanOpenExistingCheckout =>
        !_checkoutRecoveryStateBlocked &&
        !string.IsNullOrWhiteSpace(_recoverableCheckoutUrl);

    public bool ShowPurchaseCheckoutState =>
        ShowPurchaseActions ||
        _checkoutRecoveryStateBlocked ||
        !string.IsNullOrWhiteSpace(_checkoutRecoveryCorrelationId);

    public bool CanRedeemClaimCode =>
        string.Equals(SessionStatus, "AUTHENTICATED", StringComparison.Ordinal) &&
        ClaimStatus != "REDEEMING" &&
        !_purchaseAttemptLocked &&
        LooksLikeClaimCode(ClaimCode);

    public bool CanChangePassword =>
        IsAuthenticated &&
        PasswordChangeStatus != "CHANGING" &&
        !string.IsNullOrEmpty(CurrentPassword) &&
        !string.IsNullOrEmpty(NewPassword) &&
        !string.IsNullOrEmpty(ConfirmNewPassword);

    public bool CanRefreshNotifications =>
        string.Equals(SessionStatus, "AUTHENTICATED", StringComparison.Ordinal);

    public bool ShowEmptyProducts => Products.Count == 0;
    public bool ShowEmptyStore => StoreProducts.Count == 0;
    public bool ShowEmptyNotifications => Notifications.Count == 0;

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        try
        {
            SessionStatus = "CHECKING_SESSION";
            Message = "Checking the BKE account session held by the Licensing Agent…";
            var response = await _accountSession.StatusAsync(cancellationToken);
            ApplyStatus(response);

            if (response.Status == "AUTHENTICATED")
            {
                ResetShellSurface();
                return;
            }

            ResetShellSurface();
            ClearCatalog("AUTH_REQUIRED", "Sign in to load your BKE software.");
            ClearStore("AUTH_REQUIRED", "Sign in to browse the BKE Store.");
            ClearNotifications("AUTH_REQUIRED", "Sign in to view BKE notifications.");
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            ResetShellSurface();
            SessionStatus = "AGENT_UNAVAILABLE";
            AccountDisplay = "Not available";
            _authenticatedAccountEmail = string.Empty;
            _authenticatedAccountType = string.Empty;
            Raise(nameof(AccountTypeLabel));
            RaiseAccountOrganizationCapabilities();
            Message = "BKE Licensing Agent is unavailable or returned an invalid response.";
            ClearCatalog("AGENT_UNAVAILABLE", "Software catalog is unavailable while the Agent cannot be reached.");
            ClearStore("AGENT_UNAVAILABLE", "BKE Store is unavailable while the Agent cannot be reached.");
            ClearNotifications("AGENT_UNAVAILABLE", "Notifications are unavailable while the Agent cannot be reached.");
        }
    }

    public void OpenAccountSurface()
    {
        if (!IsAuthenticated)
        {
            return;
        }

        SelectedModuleIndex = -1;
        ShowAccountSurface = true;
        if (string.IsNullOrWhiteSpace(
                OrganizationCreateBillingEmail) &&
            !string.IsNullOrWhiteSpace(_authenticatedAccountEmail))
        {
            OrganizationCreateBillingEmail =
                _authenticatedAccountEmail;
        }
    }

    public async Task OpenModuleAsync(
        int moduleIndex,
        CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            return;
        }

        ShowAccountSurface = false;
        SelectedModuleIndex = moduleIndex;

        switch (moduleIndex)
        {
            case 0:
                await RefreshCatalogAsync(cancellationToken);
                break;
            case 1:
                await RefreshNotificationsAsync(cancellationToken);
                break;
            case 2:
                await RefreshStoreAsync(cancellationToken);
                break;
        }
    }

    public async Task OpenRegistrationAsync(CancellationToken cancellationToken)
    {
        if (IsAuthenticated)
        {
            return;
        }

        ClearNativeMfaState();
        Password = string.Empty;
        ResetPasswordResetState();
        ResetRegistrationFields(clearEmail: true);
        _showRegistration = true;
        RegistrationStatus = "LOADING";
        RegistrationMessage =
            "Loading the current BKE Terms of Service and Privacy Policy…";
        RaiseRegistrationCapabilities();

        try
        {
            var result = await _nativeRegistration.LoadAsync(cancellationToken);
            RegistrationLegalDocuments.Clear();

            if (result.Status != "READY")
            {
                RegistrationStatus = result.Status;
                RegistrationMessage =
                    result.ErrorMessage ??
                    "BKE account registration is temporarily unavailable.";
                return;
            }

            foreach (var document in result.LegalDocuments)
            {
                var item = RegistrationLegalDocumentViewModel.From(document);
                item.PropertyChanged += RegistrationLegalAcceptanceChanged;
                RegistrationLegalDocuments.Add(item);
            }

            RegistrationStatus = "READY";
            RegistrationMessage =
                "Review and explicitly accept the exact current legal documents before creating your account.";
            RaiseRegistrationCapabilities();
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            RegistrationStatus = "UNAVAILABLE";
            RegistrationMessage =
                "BKE account registration is temporarily unavailable.";
        }
    }

    public void CancelRegistration()
    {
        if (!string.IsNullOrWhiteSpace(RegistrationEmail))
        {
            Email = RegistrationEmail.Trim();
        }

        ResetRegistrationFields(clearEmail: true);
        _showRegistration = false;
        RegistrationStatus = "IDLE";
        RegistrationMessage =
            "Create your BKE account without leaving the Launcher.";
        RaiseRegistrationCapabilities();
    }

    public async Task CreateNativeAccountAsync(
        CancellationToken cancellationToken)
    {
        if (!CanCreateAccount)
        {
            RegistrationStatus = "INVALID_INPUT";
            RegistrationMessage =
                "Enter your name, email, password, and accept both current legal documents.";
            return;
        }

        var password = RegistrationPassword;
        var legalVersionIds = RegistrationLegalDocuments
            .Where(document => document.IsAccepted)
            .Select(document => document.VersionId)
            .ToArray();

        RegistrationStatus = "CREATING";
        RegistrationMessage = "Creating your BKE account…";

        try
        {
            var result = await _nativeRegistration.RegisterAsync(
                RegistrationEmail,
                RegistrationName,
                password,
                legalVersionIds,
                cancellationToken);

            RegistrationStatus = result.Status;
            RegistrationMessage =
                result.ErrorMessage ??
                "BKE account registration could not be completed.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            RegistrationStatus = "UNAVAILABLE";
            RegistrationMessage =
                "BKE account registration is temporarily unavailable.";
        }
        finally
        {
            RegistrationPassword = string.Empty;
        }
    }

    public async Task VerifyRegistrationEmailAsync(
        CancellationToken cancellationToken)
    {
        if (!CanVerifyRegistrationEmail)
        {
            RegistrationMessage =
                "Enter the 8-character verification code sent to your email.";
            return;
        }

        var code = RegistrationCode;
        RegistrationCode = string.Empty;
        RegistrationStatus = "VERIFYING";
        RegistrationMessage = "Verifying your BKE account email…";

        try
        {
            var result = await _nativeRegistration.VerifyEmailAsync(
                RegistrationEmail,
                code,
                cancellationToken);

            if (result.Status != "VERIFIED")
            {
                RegistrationStatus = "VERIFICATION_REQUIRED";
                RegistrationMessage =
                    result.ErrorMessage ??
                    "The verification code could not be confirmed.";
                return;
            }

            var verifiedEmail = RegistrationEmail.Trim();
            ResetRegistrationFields(clearEmail: true);
            _showRegistration = false;
            RegistrationStatus = "VERIFIED";
            RegistrationMessage = result.ErrorMessage ??
                "Email verified. Sign in to finish connecting BKE on this machine.";
            Email = verifiedEmail;
            Password = string.Empty;
            Message =
                "Email verified. Sign in with your new BKE account to connect this machine.";
            RaiseRegistrationCapabilities();
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            RegistrationStatus = "VERIFICATION_REQUIRED";
            RegistrationMessage =
                "BKE email verification is temporarily unavailable.";
        }
    }

    public async Task ResendRegistrationVerificationAsync(
        CancellationToken cancellationToken)
    {
        if (!CanResendRegistrationEmail)
        {
            return;
        }

        RegistrationStatus = "RESENDING";
        RegistrationMessage = "Requesting a new verification code…";

        try
        {
            var result = await _nativeRegistration.ResendAsync(
                RegistrationEmail,
                cancellationToken);
            RegistrationStatus = "VERIFICATION_REQUIRED";
            RegistrationMessage =
                result.ErrorMessage ??
                "If this account still needs verification, a new code will be sent.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            RegistrationStatus = "VERIFICATION_REQUIRED";
            RegistrationMessage =
                "Verification-code resend is temporarily unavailable.";
        }
    }

    public async Task RequestPasswordResetAsync(
        CancellationToken cancellationToken)
    {
        if (IsAuthenticated)
        {
            PasswordResetStatus = "AUTHENTICATED";
            PasswordResetMessage =
                "Password reset is available from the sign-in screen.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            PasswordResetStatus = "INVALID_INPUT";
            PasswordResetMessage = "Enter your BKE account email first.";
            return;
        }

        PasswordResetStatus = "REQUESTING";
        PasswordResetMessage = "Requesting a one-time BKE password reset…";

        try
        {
            var response = await _passwordResetRequest.RequestAsync(
                Email,
                cancellationToken);

            Password = string.Empty;

            if (response.Status == "accepted")
            {
                PasswordResetStatus = "ACCEPTED";
                PasswordResetMessage =
                    "If a BKE account exists for this email, a one-time reset link has been sent. Complete the reset, then return here and sign in with the new password.";
                return;
            }

            PasswordResetStatus = response.Error == "INVALID_INPUT"
                ? "INVALID_INPUT"
                : "UNAVAILABLE";
            PasswordResetMessage = response.Error == "INVALID_INPUT"
                ? "Enter a valid BKE account email."
                : "BKE password recovery is temporarily unavailable.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            Password = string.Empty;
            PasswordResetStatus = "UNAVAILABLE";
            PasswordResetMessage =
                "BKE password recovery is temporarily unavailable.";
        }
    }

    public async Task NativeSignInAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrEmpty(Password))
        {
            SessionStatus = "SIGNED_OUT";
            Message = "Enter your BKE email and password.";
            return;
        }

        ClearNativeMfaState();

        try
        {
            SessionStatus = "SIGNING_IN";
            ClearNotifications("AUTH_REQUIRED", "Sign in to view BKE notifications.");
            Message = "Authenticating directly with BKE Digital Solutions…";

            var result = await _nativeSignIn.SignInAsync(
                Email,
                Password,
                SelectedAccount?.AccountId,
                cancellationToken);

            if (result.Status == "MFA_CHALLENGE_REQUIRED")
            {
                Password = string.Empty;
                _nativeMfaChallengeToken = result.ChallengeToken;
                NativeMfaReference = result.MfaReference is null
                    ? string.Empty
                    : $"Reference: {result.MfaReference}";
                NativeMfaMessage = result.EmailSent
                    ? "Enter the verification code sent to your account email."
                    : "Enter your BKE multi-factor verification code.";
                SessionStatus = "MFA_CHALLENGE_REQUIRED";
                Message = "Multi-factor verification is required before this machine can receive an Agent handoff.";
                Raise(nameof(ShowNativeMfaChallenge));
                Raise(nameof(CanVerifyNativeMfa));
                return;
            }

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

            ApplyNativeSignInResult(result);
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            Password = string.Empty;
            ClearNativeMfaState();
            ResetPasswordChangeState();
            ResetAccountMfaState(clearRecoveryCodes: true);
            ResetAccountPrivacyState();
        ResetAccountOrganizationState();
            SessionStatus = "SIGN_IN_UNAVAILABLE";
            AccountDisplay = "Not signed in";
            _authenticatedAccountEmail = string.Empty;
            _authenticatedAccountType = string.Empty;
            Raise(nameof(AccountTypeLabel));
            RaiseAccountOrganizationCapabilities();
            Message = "BKE native sign-in is unavailable or returned an invalid response.";
            ClearCatalog("AUTH_REQUIRED", "Sign in to load your BKE software.");
            ClearStore("AUTH_REQUIRED", "Sign in to browse the BKE Store.");
            ClearNotifications("AUTH_REQUIRED", "Sign in to view BKE notifications.");
        }
    }

    public async Task VerifyNativeMfaAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_nativeMfaChallengeToken) ||
            NativeMfaCode.Length is < 6 or > 32)
        {
            NativeMfaMessage = "Enter the verification code before continuing.";
            return;
        }

        var challengeToken = _nativeMfaChallengeToken;
        var code = NativeMfaCode;
        NativeMfaCode = string.Empty;

        try
        {
            SessionStatus = "VERIFYING_MFA";
            var result = await _nativeSignIn.VerifyMfaAsync(
                challengeToken,
                code,
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
                ClearNativeMfaState();
                SessionStatus = "ACCOUNT_SELECTION_REQUIRED";
                Message = AvailableAccounts.Count == 0
                    ? "No active BKE account is available for this identity."
                    : "Verification succeeded. Choose the account, re-enter your password, and sign in again to issue a fresh verification challenge.";
                return;
            }

            if (result.Status == "FAILED")
            {
                SessionStatus = "MFA_CHALLENGE_REQUIRED";
                NativeMfaMessage = result.ErrorMessage ?? "Verification failed.";
                if (result.ErrorCode == "INVALID_MFA_CHALLENGE")
                {
                    ClearNativeMfaState();
                    SessionStatus = "SIGNED_OUT";
                    Message = "The verification challenge expired or was invalid. Sign in again.";
                }
                return;
            }

            ClearNativeMfaState();
            ApplyNativeSignInResult(result);
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            ClearNativeMfaState();
            SessionStatus = "SIGNED_OUT";
            Message = "BKE multi-factor verification is unavailable. Sign in again before retrying.";
        }
    }

    private void ApplyNativeSignInResult(LauncherNativeSignInResult result)
    {
        Password = string.Empty;
        ResetRegistrationState();
        ResetPasswordResetState();
        ClaimCode = string.Empty;
        ClaimStatus = "AUTH_REQUIRED";
        ClaimMessage = "Sign in to redeem a Claim Code.";
        ResetPasswordChangeState();
        ResetAccountMfaState(clearRecoveryCodes: true);
        ResetAccountPrivacyState();
        ResetAccountOrganizationState();
        AvailableAccounts.Clear();
        SelectedAccount = null;
        Raise(nameof(HasAccountChoices));

        SessionStatus = result.Status;
        if (result.Status == "AUTHENTICATED" && result.Account is not null)
        {
            AccountDisplay = $"{result.Account.DisplayName} · {result.Account.Email}";
            _authenticatedAccountEmail = result.Account.Email;
            _authenticatedAccountType = result.Account.AccountType;
            Raise(nameof(AccountTypeLabel));
            RaiseAccountOrganizationCapabilities();
            Message = "Signed in. Durable account-session secrets are stored by the BKE Licensing Agent.";
            ClaimStatus = "READY";
            ClaimMessage =
                "Enter a one-time Claim Code to redeem it into the signed-in BKE account.";
            ResetPasswordResetState();
            ResetShellSurface();
            return;
        }

        AccountDisplay = "Not signed in";
        _authenticatedAccountEmail = string.Empty;
        _authenticatedAccountType = string.Empty;
        Raise(nameof(AccountTypeLabel));
        Message = result.ErrorMessage ?? "BKE account sign-in failed.";
        ClearCatalog("AUTH_REQUIRED", "Sign in to load your BKE software.");
        ClearStore("AUTH_REQUIRED", "Sign in to browse the BKE Store.");
        ClearNotifications("AUTH_REQUIRED", "Sign in to view BKE notifications.");
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
                ResetShellSurface();
            }
            else
            {
                ClearNotifications(
                    "AUTH_REQUIRED",
                    "Sign in to view BKE notifications.");
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

            if (response.Status != "AUTHENTICATED")
            {
                ClearNativeMfaState();
                ResetAccountMfaState(clearRecoveryCodes: true);
            ResetAccountPrivacyState();
        ResetAccountOrganizationState();
                ResetPasswordChangeState();
                ResetShellSurface();
                ClearCatalog(
                    "AUTH_REQUIRED",
                    "Sign in to load your BKE software.");
                ClearStore(
                    "AUTH_REQUIRED",
                    "Sign in to browse the BKE Store.");
                ClearNotifications(
                    "AUTH_REQUIRED",
                    "Sign in to view BKE notifications.");
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
            ClearNotifications(
                "AGENT_UNAVAILABLE",
                "Notifications are unavailable while the Agent cannot be reached.");
        }
    }

    public async Task RefreshNotificationsAsync(
        CancellationToken cancellationToken)
    {
        if (!CanRefreshNotifications)
        {
            ClearNotifications(
                "AUTH_REQUIRED",
                "Sign in to view BKE notifications.");
            return;
        }

        NotificationStatus = "LOADING";
        NotificationMessage =
            "Loading notifications through the BKE Licensing Agent…";

        try
        {
            var snapshot = await _notifications.GetAsync(
                100,
                cancellationToken);

            Notifications.Clear();
            foreach (var item in snapshot.Items)
            {
                Notifications.Add(NotificationViewModel.From(item));
            }

            NotificationStatus = snapshot.Status;
            NotificationMessage = snapshot.Status == "Succeeded"
                ? Notifications.Count == 0
                    ? "No notifications for this BKE account."
                    : $"{Notifications.Count} notification(s) for this BKE account."
                : snapshot.Message ?? "Notifications are unavailable.";
            Raise(nameof(ShowEmptyNotifications));
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentOutOfRangeException)
        {
            ClearNotifications(
                "AGENT_UNAVAILABLE",
                "Notifications are unavailable or the Agent returned an invalid feed.");
        }
    }

    public Task MarkNotificationReadAsync(
        NotificationViewModel notification,
        CancellationToken cancellationToken) =>
        MutateNotificationAsync(
            notification,
            "MARK_READ",
            cancellationToken);

    public Task DismissNotificationAsync(
        NotificationViewModel notification,
        CancellationToken cancellationToken) =>
        MutateNotificationAsync(
            notification,
            "DISMISS",
            cancellationToken);

    private async Task MutateNotificationAsync(
        NotificationViewModel notification,
        string action,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (!CanRefreshNotifications)
        {
            ClearNotifications(
                "AUTH_REQUIRED",
                "Sign in to update BKE notifications.");
            return;
        }

        NotificationStatus = "UPDATING";
        NotificationMessage = action == "MARK_READ"
            ? "Marking notification as read through the BKE Licensing Agent…"
            : "Dismissing notification through the BKE Licensing Agent…";

        try
        {
            var result = await _notifications.MutateAsync(
                notification.Id,
                action,
                cancellationToken);

            if (result.Status == "Failed")
            {
                NotificationStatus = "FAILED";
                NotificationMessage =
                    result.Message ?? "The notification update failed.";
                return;
            }

            await RefreshNotificationsAsync(cancellationToken);
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            NotificationStatus = "AGENT_UNAVAILABLE";
            NotificationMessage =
                "Notification update failed or the Agent returned an invalid receipt result.";
        }
    }

    public async Task RedeemClaimCodeAsync(CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            ClaimStatus = "AUTH_REQUIRED";
            ClaimMessage = "Sign in with BKE before redeeming a Claim Code.";
            return;
        }

        if (_purchaseAttemptLocked)
        {
            ClaimStatus = "BLOCKED";
            ClaimMessage =
                "Resolve the existing checkout attempt before redeeming a Claim Code. The pending purchase and redemption both change software ownership for this exact account.";
            return;
        }

        var code = ClaimCode.Trim();
        if (!LooksLikeClaimCode(code))
        {
            ClaimStatus = "INVALID_REQUEST";
            ClaimMessage =
                "Enter a valid BKE Claim Code in the BKE-CLM-XXXXX-XXXXX-XXXXX-XXXXX-XXXXX-XXXXX format.";
            return;
        }

        // The Claim Code is a one-time acquisition credential. Keep only the
        // local call copy and clear the bound UI field before network mutation.
        ClaimCode = string.Empty;
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
                    ClaimStatus = "CLAIMED";
                    ClaimMessage =
                        "Claim Code redeemed. Your BKE software and Store eligibility are being refreshed.";
                    await RefreshCatalogAsync(cancellationToken);
                    await RefreshStoreAsync(cancellationToken);
                    return;

                case "AUTH_REQUIRED":
                    EnterAccountMfaReauthentication(
                        response.Error?.Message ??
                            "Your BKE account session is no longer valid. Sign in again before redeeming a Claim Code.",
                        clearRecoveryCodes: true);
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
                    if (response.Error?.Retryable == true)
                    {
                        await RefreshCatalogAsync(cancellationToken);
                        await RefreshStoreAsync(cancellationToken);
                        ClaimMessage =
                            "Claim Code redemption could not be confirmed. BKE refreshed the authoritative software and Store state; inspect it before entering the code again. BKE will not retry redemption automatically.";
                        return;
                    }

                    ClaimMessage =
                        response.Error?.Message ??
                        "Claim Code redemption failed.";
                    return;
            }
        }
        catch (ArgumentException error)
        {
            ClaimStatus = "INVALID_REQUEST";
            ClaimMessage = error.Message;
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            await RefreshCatalogAsync(cancellationToken);
            await RefreshStoreAsync(cancellationToken);
            ClaimStatus = "RESULT_UNKNOWN";
            ClaimMessage =
                "Claim Code redemption could not be confirmed. BKE refreshed the authoritative software and Store state; inspect it before entering the code again. BKE will not replay the redemption automatically.";
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

        var recoveryState = new LauncherCheckoutRecoveryState(
            Guid.NewGuid().ToString("N"),
            _reviewedPurchasePlanId,
            purchaseMode,
            PurchaseLegalDocuments
                .Select(document => document.DocumentVersionId)
                .ToArray());
        try
        {
            _checkoutRecoveryStore.Write(recoveryState);
        }
        catch (Exception storageError) when (
            storageError is IOException or
            UnauthorizedAccessException or
            InvalidDataException or
            InvalidOperationException)
        {
            PurchaseCheckoutStatus = "RECOVERY_STORAGE_FAILED";
            PurchaseCheckoutMessage =
                "Checkout was not started because BKE could not persist its resumable recovery state safely.";
            return;
        }

        _purchaseAttemptLocked = true;
        _checkoutRecoveryState = recoveryState;
        _checkoutRecoveryCorrelationId = recoveryState.CorrelationId;
        _recoverableCheckoutUrl = null;
        _checkoutRecoveryStateBlocked = false;
        RaisePurchaseActionState();
        RaiseCheckoutRecoveryState();
        PurchaseCheckoutStatus = "STARTING";
        PurchaseCheckoutMessage =
            "Creating a secure checkout through the BKE Licensing Agent…";

        await ExecuteCheckoutStartAsync(
            recoveryState,
            cancellationToken);
    }

    public async Task RetryOriginalCheckoutAsync(
        CancellationToken cancellationToken)
    {
        var recoveryState = _checkoutRecoveryState;
        if (!CanRetryOriginalCheckout ||
            recoveryState is null ||
            !recoveryState.HasResumeIntent ||
            recoveryState.PurchasePlanId is null ||
            recoveryState.PurchaseMode is null)
        {
            PurchaseCheckoutMessage =
                "This saved checkout cannot be resumed safely. Check its status again; BKE will not create a new correlation.";
            return;
        }

        PurchaseCheckoutStatus = "RESUMING";
        PurchaseCheckoutMessage =
            "Resuming the original checkout with the same retained correlation and reviewed intent…";

        await ExecuteCheckoutStartAsync(
            recoveryState,
            cancellationToken);
    }

    private async Task ExecuteCheckoutStartAsync(
        LauncherCheckoutRecoveryState recoveryState,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _storeCheckoutStart.StartAsync(
                recoveryState.CorrelationId,
                recoveryState.PurchasePlanId!,
                recoveryState.PurchaseMode!,
                recoveryState.LegalVersionIds,
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
                    recoveryState.PurchaseMode == "GIFT"
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
                    "The original checkout correlation is already being created. Check this exact attempt instead of creating another correlation.",
                "RESULT_UNKNOWN" =>
                    "Checkout status is uncertain. Check this exact attempt first. Resume is allowed only after authoritative NOT_FOUND.",
                _ =>
                    "Checkout could not be started. Review the purchase again before another attempt.",
            };

            if (result.Status == "READY" &&
                result.Complimentary == true &&
                recoveryState.PurchaseMode == "GIFT")
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
                "The Agent checkout result could not be confirmed. Check this exact attempt. Do not retry automatically; resume only after authoritative NOT_FOUND.";
            RaiseCheckoutRecoveryState();
        }
        catch (Exception error) when (
            error is InvalidDataException or
            ArgumentException)
        {
            PurchaseCheckoutStatus = "RESULT_UNKNOWN";
            PurchaseCheckoutMessage =
                "The Agent checkout response was invalid after checkout-start. BKE preserved this exact recovery state. Check this attempt. Resume only after authoritative NOT_FOUND.";
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
                if (_checkoutRecoveryState?.HasResumeIntent == true)
                {
                    PurchaseCheckoutStatus = "RETRY_AVAILABLE";
                    PurchaseCheckoutMessage =
                        "No durable checkout is visible for the retained correlation. You may explicitly resume the original checkout with this same correlation and saved intent; BKE will not create a new correlation.";
                }
                else
                {
                    PurchaseCheckoutMessage =
                        "No durable checkout is visible for this legacy correlation, and its original intent was not retained. Recovery remains locked; BKE will not guess or create a new checkout.";
                }
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
                        "Sign in with the same BKE identity and account on this device to recover this gift Claim Code.";
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

    public async Task OpenLegalDocumentAsync(
        PurchaseLegalDocumentViewModel document,
        CancellationToken cancellationToken)
    {
        try
        {
            await _externalNavigator.OpenLegalDocumentAsync(
                document.Slug,
                cancellationToken);
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
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

    public async Task RefreshAccountMfaAsync(CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            AccountMfaStatus = "AUTH_REQUIRED";
            AccountMfaMessage = "Sign in with BKE before opening MFA settings.";
            return;
        }

        try
        {
            AccountMfaStatus = "UPDATING";
            AccountMfaMessage = "Loading MFA status through the BKE Licensing Agent…";
            var response = await _accountMfa.StatusAsync(cancellationToken);
            if (response.Status == "AUTH_REQUIRED")
            {
                EnterAccountMfaReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.",
                    clearRecoveryCodes: true);
                return;
            }

            AccountMfaStatus = response.Status;
            AccountMfaEnabled = response.Enabled;
            AccountMfaEnrollmentPending = response.EnrollmentPending;
            AccountMfaRecoveryCodesRemaining = response.RecoveryCodesRemaining;
            AccountMfaMessage = response.Error?.Message ?? response.Status switch
            {
                "READY" when response.Enabled => "MFA is enabled for this BKE account.",
                "READY" => "MFA is not enabled for this BKE account.",
                _ => "BKE MFA status is temporarily unavailable.",
            };
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            AccountMfaStatus = "FAILED";
            AccountMfaMessage = "BKE MFA status is temporarily unavailable.";
        }
    }

    public async Task StartMfaEnrollmentAsync(CancellationToken cancellationToken)
    {
        if (!IsAuthenticated || string.IsNullOrEmpty(AccountMfaCurrentPassword))
        {
            AccountMfaStatus = "INVALID_INPUT";
            AccountMfaMessage = "Enter your current password before enabling MFA.";
            return;
        }

        try
        {
            AccountMfaStatus = "UPDATING";
            var response = await _accountMfa.EnrollStartAsync(
                AccountMfaCurrentPassword,
                cancellationToken);
            ApplyAccountMfaChallenge(response, "ENROLL");
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            AccountMfaStatus = "FAILED";
            AccountMfaMessage = "MFA enrollment could not be started.";
        }
    }

    public async Task CompleteMfaEnrollmentAsync(CancellationToken cancellationToken)
    {
        if (!CanCompleteMfaEnrollment ||
            string.IsNullOrWhiteSpace(_accountMfaChallengeToken))
        {
            AccountMfaStatus = "INVALID_INPUT";
            AccountMfaMessage = "Enter the enrollment verification code.";
            return;
        }

        await RunAccountMfaMutationAsync(
            (password, challenge, code, ct) =>
                _accountMfa.EnrollCompleteAsync(password, challenge, code, ct),
            "MFA enrollment",
            cancellationToken);
    }

    public async Task StartMfaProofAsync(CancellationToken cancellationToken)
    {
        if (!IsAuthenticated || string.IsNullOrEmpty(AccountMfaCurrentPassword))
        {
            AccountMfaStatus = "INVALID_INPUT";
            AccountMfaMessage = "Enter your current password before changing MFA.";
            return;
        }

        try
        {
            AccountMfaStatus = "UPDATING";
            var response = await _accountMfa.ChallengeAsync(
                AccountMfaCurrentPassword,
                cancellationToken);
            ApplyAccountMfaChallenge(response, "PROOF");
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            AccountMfaStatus = "FAILED";
            AccountMfaMessage = "MFA verification could not be started.";
        }
    }

    public Task DisableMfaAsync(CancellationToken cancellationToken) =>
        RunAccountMfaMutationAsync(
            (password, challenge, code, ct) =>
                _accountMfa.DisableAsync(password, challenge, code, ct),
            "MFA disable",
            cancellationToken);

    public Task RegenerateMfaRecoveryAsync(CancellationToken cancellationToken) =>
        RunAccountMfaMutationAsync(
            (password, challenge, code, ct) =>
                _accountMfa.RegenerateRecoveryAsync(password, challenge, code, ct),
            "Recovery-code regeneration",
            cancellationToken);

    public void DismissMfaRecoveryCodes()
    {
        AccountMfaRecoveryCodes = string.Empty;
    }


    public async Task CreateAccountOrganizationAsync(
        CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            OrganizationCreateStatus = "AUTH_REQUIRED";
            OrganizationCreateMessage =
                "Sign in with BKE before creating an organization.";
            return;
        }

        if (!CanCreateOrganization)
        {
            if (_organizationCreateRetryBlocked)
            {
                OrganizationCreateStatus = "OUTCOME_UNKNOWN";
                OrganizationCreateMessage =
                    "The previous creation result is uncertain. Use Switch BKE account and check the available accounts before attempting another organization.";
                return;
            }

            OrganizationCreateStatus = "INVALID_INPUT";
            OrganizationCreateMessage =
                "Enter a 2–120 character display name, 2–180 character legal name, valid billing email, and optional registration/tax values of 80 characters or fewer.";
            return;
        }

        OrganizationCreateStatus = "CREATING";
        OrganizationCreateMessage =
            "Creating the organization through the BKE Licensing Agent…";

        try
        {
            var response = await _accountOrganization.CreateAsync(
                OrganizationCreateDisplayName,
                OrganizationCreateLegalName,
                OrganizationCreateBillingEmail,
                OrganizationCreateRegistrationNumber,
                OrganizationCreateTaxId,
                cancellationToken);

            if (response.Status == "AUTH_REQUIRED")
            {
                EnterAccountOrganizationReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            if (response.Status == "CREATED")
            {
                var displayName =
                    response.DisplayName ??
                    OrganizationCreateDisplayName.Trim();
                ClearOrganizationCreateFields();
                OrganizationCreateStatus = "CREATED";
                OrganizationCreateMessage =
                    $"{displayName} was created. Use Switch BKE account to select the new Organization; the current account was not changed automatically.";
                return;
            }

            if (response.Status == "OUTCOME_UNKNOWN")
            {
                _organizationCreateRetryBlocked = true;
                OrganizationCreateStatus = "OUTCOME_UNKNOWN";
                OrganizationCreateMessage =
                    response.Error?.Message ??
                    "The creation result could not be confirmed. Use Switch BKE account and check the available accounts before retrying.";
                RaiseAccountOrganizationCapabilities();
                return;
            }

            OrganizationCreateStatus = response.Status;
            OrganizationCreateMessage =
                response.Error?.Message ??
                "The organization was not created.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            _organizationCreateRetryBlocked = true;
            OrganizationCreateStatus = "OUTCOME_UNKNOWN";
            OrganizationCreateMessage =
                "The creation result could not be confirmed. Use Switch BKE account and check the available accounts before retrying; BKE will not blindly resubmit this request.";
            RaiseAccountOrganizationCapabilities();
        }
        catch (ArgumentException error)
        {
            OrganizationCreateStatus = "INVALID_INPUT";
            OrganizationCreateMessage = error.Message;
        }
    }

    public async Task RefreshAccountOrganizationAsync(
        CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            ClearAccountOrganization(
                "AUTH_REQUIRED",
                "Sign in with BKE before opening organization details.");
            return;
        }

        if (_authenticatedAccountType != "ORGANIZATION")
        {
            ClearAccountOrganization(
                "NOT_ORGANIZATION",
                "The selected BKE account is a Personal account.");
            return;
        }

        AccountOrganizationStatus = "LOADING";
        AccountOrganizationMessage =
            "Loading organization details through the BKE Licensing Agent…";

        try
        {
            var response = await _accountOrganization.GetAsync(
                cancellationToken);

            if (response.Status == "AUTH_REQUIRED")
            {
                ResetAccountOrganizationState();
                EnterAccountOrganizationReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            if (response.Status == "NOT_ORGANIZATION")
            {
                ClearAccountOrganization(
                    "NOT_ORGANIZATION",
                    "The selected BKE account is not an Organization account.");
                return;
            }

            if (response.Status != "READY" ||
                response.Account is null ||
                response.Permissions is null ||
                response.Organization is null ||
                response.Counts is null)
            {
                ClearAccountOrganization(
                    response.Status,
                    response.Error?.Message ??
                    "BKE organization details are temporarily unavailable.");
                return;
            }

            _organizationAccount = response.Account;
            _organizationPermissions = response.Permissions;
            _organizationProfile = response.Organization;
            _organizationCounts = response.Counts;
            _organizationBillingEmail = response.BillingEmail;
            _organizationTaxId = response.TaxId;
            LoadOrganizationProfileEditorFromAuthority();
            ResetOrganizationMemberManagementState();
            ResetOrganizationOwnershipTransferState();

            OrganizationMembers.Clear();
            foreach (var member in response.Members)
            {
                OrganizationMembers.Add(member);
            }

            OrganizationInvitations.Clear();
            foreach (var invitation in response.Invitations)
            {
                OrganizationInvitations.Add(invitation);
            }

            AccountOrganizationStatus = "READY";
            AccountOrganizationMessage =
                "Organization details loaded from BKE Digital Solutions through the Licensing Agent.";
            RaiseAccountOrganizationCapabilities();
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            ClearAccountOrganization(
                "AGENT_UNAVAILABLE",
                "Organization details are unavailable or the Licensing Agent returned an invalid response.");
        }
    }

    public async Task UpdateAccountOrganizationProfileAsync(
        CancellationToken cancellationToken)
    {
        if (!CanSaveOrganizationProfile)
        {
            OrganizationProfileUpdateStatus = "INVALID_INPUT";
            OrganizationProfileUpdateMessage =
                "Refresh the selected Organization, change an authorized field, and provide valid required values before saving.";
            return;
        }

        var updateIdentity = OrganizationIdentityProfileDirty;
        var updateBilling = OrganizationBillingProfileDirty;

        OrganizationProfileUpdateStatus = "UPDATING";
        OrganizationProfileUpdateMessage =
            "Updating the selected Organization through the BKE Licensing Agent…";

        try
        {
            var response = await _accountOrganization.UpdateProfileAsync(
                updateIdentity,
                updateIdentity ? OrganizationEditDisplayName : null,
                updateIdentity ? OrganizationEditLegalName : null,
                updateIdentity
                    ? OrganizationEditRegistrationNumber
                    : null,
                updateBilling,
                updateBilling ? OrganizationEditBillingEmail : null,
                updateBilling ? OrganizationEditTaxId : null,
                cancellationToken);

            if (response.Status == "AUTH_REQUIRED")
            {
                ResetAccountOrganizationState();
                EnterAccountOrganizationReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            if (response.Status == "NOT_ORGANIZATION")
            {
                ClearAccountOrganization(
                    "NOT_ORGANIZATION",
                    "The selected BKE account is not an Organization account.");
                return;
            }

            if (response.Status == "UPDATED")
            {
                await RefreshAccountOrganizationAsync(
                    cancellationToken);
                if (IsAuthenticated && OrganizationReady)
                {
                    OrganizationProfileUpdateStatus = "UPDATED";
                    OrganizationProfileUpdateMessage =
                        "Organization profile updated and refreshed from the authoritative account state.";
                }
                return;
            }

            if (response.Status == "OUTCOME_UNKNOWN")
            {
                await RefreshAccountOrganizationAsync(
                    cancellationToken);
                if (IsAuthenticated)
                {
                    OrganizationProfileUpdateStatus =
                        "OUTCOME_UNKNOWN";
                    OrganizationProfileUpdateMessage =
                        "The update result could not be confirmed. The authoritative Organization overview was refreshed; review the current values before making another edit.";
                }
                return;
            }

            OrganizationProfileUpdateStatus = response.Status;
            OrganizationProfileUpdateMessage =
                response.Error?.Message ??
                "The organization profile was not updated.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            try
            {
                await RefreshAccountOrganizationAsync(
                    cancellationToken);
            }
            catch
            {
                // RefreshAccountOrganizationAsync reports its own state.
            }

            if (IsAuthenticated)
            {
                OrganizationProfileUpdateStatus =
                    "OUTCOME_UNKNOWN";
                OrganizationProfileUpdateMessage =
                    "The update result could not be confirmed. The authoritative Organization overview was refreshed when possible; review the current values before making another edit.";
            }
        }
        catch (ArgumentException error)
        {
            OrganizationProfileUpdateStatus = "INVALID_INPUT";
            OrganizationProfileUpdateMessage = error.Message;
        }
    }

    public async Task AcceptAccountOrganizationInvitationAsync(
        CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            OrganizationInvitationAcceptanceStatus =
                "AUTH_REQUIRED";
            OrganizationInvitationAcceptanceMessage =
                "Sign in with BKE before accepting an Organization invitation.";
            return;
        }

        if (!CanAcceptOrganizationInvitation)
        {
            OrganizationInvitationAcceptanceStatus =
                "INVALID_INPUT";
            OrganizationInvitationAcceptanceMessage =
                "Enter a valid Organization invitation code before accepting it.";
            return;
        }

        var invitationCode =
            OrganizationInvitationAcceptanceCode.Trim();

        OrganizationInvitationAcceptanceCode = string.Empty;
        OrganizationInvitationAcceptedRole = string.Empty;
        OrganizationInvitationAcceptanceStatus = "ACCEPTING";
        OrganizationInvitationAcceptanceMessage =
            "Accepting the Organization invitation through the BKE Licensing Agent…";

        try
        {
            var response =
                await _accountOrganization.AcceptInvitationAsync(
                    invitationCode,
                    cancellationToken);
            invitationCode = string.Empty;

            if (response.Status == "AUTH_REQUIRED")
            {
                ResetAccountOrganizationState();
                EnterAccountOrganizationReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            if (response.Status == "ACCEPTED")
            {
                OrganizationInvitationAcceptedRole =
                    response.Role ?? string.Empty;
                OrganizationInvitationAcceptanceStatus =
                    "ACCEPTED";
                OrganizationInvitationAcceptanceMessage =
                    "Invitation accepted. The Organization is now available to your BKE account. Use Switch BKE account to refresh eligible accounts and explicitly select it; BKE did not switch accounts automatically.";
                return;
            }

            OrganizationInvitationAcceptanceStatus =
                response.Status;
            OrganizationInvitationAcceptanceMessage =
                response.Status == "OUTCOME_UNKNOWN"
                    ? "The invitation acceptance result could not be confirmed. BKE cleared the code from this screen and will not retry it automatically. Use Switch BKE account to refresh eligible accounts and check whether the Organization already appears before entering the code again."
                    : response.Error?.Message ??
                      "The Organization invitation was not accepted.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            invitationCode = string.Empty;
            OrganizationInvitationAcceptedRole = string.Empty;
            OrganizationInvitationAcceptanceStatus =
                "OUTCOME_UNKNOWN";
            OrganizationInvitationAcceptanceMessage =
                "The invitation acceptance result could not be confirmed. BKE cleared the code from this screen and will not retry it automatically. Use Switch BKE account to refresh eligible accounts and check whether the Organization already appears before entering the code again.";
        }
        catch (ArgumentException error)
        {
            invitationCode = string.Empty;
            OrganizationInvitationAcceptedRole = string.Empty;
            OrganizationInvitationAcceptanceStatus =
                "INVALID_INPUT";
            OrganizationInvitationAcceptanceMessage =
                error.Message;
        }
    }

    public async Task CreateAccountOrganizationInvitationAsync(
        CancellationToken cancellationToken)
    {
        if (!CanInviteOrganizationMember)
        {
            OrganizationInvitationStatus = "INVALID_INPUT";
            OrganizationInvitationMessage =
                "Refresh the selected Organization, enter a valid email, choose an allowed role, and finish delivery of any previous invitation code before issuing another invite.";
            return;
        }

        var invitedEmail = OrganizationInvitationEmail;
        var invitedRole = OrganizationInvitationRole;

        OrganizationInvitationStatus = "ISSUING";
        OrganizationInvitationMessage =
            "Issuing the Organization invitation through the BKE Licensing Agent…";

        try
        {
            var response = await _accountOrganization.CreateInvitationAsync(
                invitedEmail,
                invitedRole,
                cancellationToken);

            if (response.Status == "AUTH_REQUIRED")
            {
                ResetAccountOrganizationState();
                EnterAccountOrganizationReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            if (response.Status == "NOT_ORGANIZATION")
            {
                ClearAccountOrganization(
                    "NOT_ORGANIZATION",
                    "The selected BKE account is not an Organization account.");
                return;
            }

            if (response.Status == "CREATED" &&
                response.Invitation is not null &&
                !string.IsNullOrWhiteSpace(response.InvitationCode))
            {
                var invitation = response.Invitation;
                var invitationCode = response.InvitationCode;
                ClearOrganizationInvitationForm();

                await RefreshAccountOrganizationAsync(
                    cancellationToken);

                if (IsAuthenticated)
                {
                    OrganizationInvitationCode = invitationCode;
                    OrganizationInvitationStatus = "CREATED";
                    OrganizationInvitationMessage =
                        $"Invitation for {invitation.Email} ({invitation.Role}) was issued. Save or send the one-time invitation code before dismissing it; BKE does not persist a local copy.";
                }
                return;
            }

            if (response.Status == "OUTCOME_UNKNOWN")
            {
                ClearOrganizationInvitationForm();
                OrganizationInvitationCode = string.Empty;

                await RefreshAccountOrganizationAsync(
                    cancellationToken);

                if (IsAuthenticated)
                {
                    OrganizationInvitationStatus =
                        "OUTCOME_UNKNOWN";
                    OrganizationInvitationMessage =
                        "The invitation result could not be confirmed. Pending invitations were refreshed when possible; review the current Organization state before issuing another invitation.";
                }
                return;
            }

            OrganizationInvitationStatus = response.Status;
            OrganizationInvitationMessage =
                response.Error?.Message ??
                "The Organization invitation was not issued.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            ClearOrganizationInvitationForm();
            OrganizationInvitationCode = string.Empty;
            try
            {
                await RefreshAccountOrganizationAsync(
                    cancellationToken);
            }
            catch
            {
                // RefreshAccountOrganizationAsync reports its own state.
            }

            if (IsAuthenticated)
            {
                OrganizationInvitationStatus =
                    "OUTCOME_UNKNOWN";
                OrganizationInvitationMessage =
                    "The invitation result could not be confirmed. Pending invitations were refreshed when possible; review the current Organization state before issuing another invitation.";
            }
        }
        catch (ArgumentException error)
        {
            OrganizationInvitationStatus = "INVALID_INPUT";
            OrganizationInvitationMessage = error.Message;
        }
    }

    public async Task ManageAccountOrganizationInvitationAsync(
        AccountOrganizationInvitation invitation,
        string action,
        CancellationToken cancellationToken)
    {
        if (!CanManageOrganizationInvitationActions)
        {
            OrganizationInvitationStatus = "INVALID_INPUT";
            OrganizationInvitationMessage =
                "Finish delivery of any visible invitation code and refresh the selected Organization before managing another invitation.";
            return;
        }

        if (action is not ("RESEND" or "REVOKE") ||
            string.IsNullOrWhiteSpace(invitation.ManagementHandle))
        {
            OrganizationInvitationStatus = "INVALID_INPUT";
            OrganizationInvitationMessage =
                "The selected pending invitation cannot be managed safely. Refresh Organization details.";
            return;
        }

        OrganizationInvitationStatus = "MANAGING";
        OrganizationInvitationMessage =
            action == "RESEND"
                ? $"Resending the invitation for {invitation.Email} through the BKE Licensing Agent…"
                : $"Revoking the invitation for {invitation.Email} through the BKE Licensing Agent…";

        try
        {
            var response = await _accountOrganization.ManageInvitationAsync(
                action,
                invitation.ManagementHandle,
                cancellationToken);

            if (response.Status == "AUTH_REQUIRED")
            {
                ResetAccountOrganizationState();
                EnterAccountOrganizationReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            if (response.Status == "NOT_ORGANIZATION")
            {
                ClearAccountOrganization(
                    "NOT_ORGANIZATION",
                    "The selected BKE account is not an Organization account.");
                return;
            }

            if (response.Status == "RESENT" &&
                response.Invitation is not null &&
                !string.IsNullOrWhiteSpace(response.InvitationCode))
            {
                var issued = response.Invitation;
                var invitationCode = response.InvitationCode;
                await RefreshAccountOrganizationAsync(
                    cancellationToken);

                if (IsAuthenticated)
                {
                    OrganizationInvitationCode = invitationCode;
                    OrganizationInvitationStatus = "RESENT";
                    OrganizationInvitationMessage =
                        $"Invitation for {issued.Email} ({issued.Role}) was resent. Save or send the new one-time invitation code before dismissing it; BKE does not persist a local copy.";
                }
                return;
            }

            if (response.Status == "REVOKED" &&
                response.Invitation is not null &&
                response.InvitationCode is null)
            {
                var revoked = response.Invitation;
                await RefreshAccountOrganizationAsync(
                    cancellationToken);

                if (IsAuthenticated)
                {
                    OrganizationInvitationStatus = "REVOKED";
                    OrganizationInvitationMessage =
                        $"Invitation for {revoked.Email} was revoked. Pending invitations were refreshed from the authoritative Organization state.";
                }
                return;
            }

            if (response.Status is
                "OUTCOME_UNKNOWN" or
                "INVITATION_NOT_FOUND" or
                "INVITATION_NOT_PENDING" or
                "INVITATION_EXPIRED")
            {
                OrganizationInvitationCode = string.Empty;
                await RefreshAccountOrganizationAsync(
                    cancellationToken);

                if (IsAuthenticated)
                {
                    OrganizationInvitationStatus = response.Status;
                    OrganizationInvitationMessage =
                        response.Status == "OUTCOME_UNKNOWN"
                            ? "The invitation management result could not be confirmed. Pending invitations were refreshed when possible; review the current Organization state before resending or revoking again."
                            : response.Error?.Message ??
                              "The selected pending invitation changed. The authoritative Organization overview was refreshed.";
                }
                return;
            }

            OrganizationInvitationStatus = response.Status;
            OrganizationInvitationMessage =
                response.Error?.Message ??
                "The Organization invitation was not changed.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            OrganizationInvitationCode = string.Empty;
            try
            {
                await RefreshAccountOrganizationAsync(
                    cancellationToken);
            }
            catch
            {
                // RefreshAccountOrganizationAsync reports its own state.
            }

            if (IsAuthenticated)
            {
                OrganizationInvitationStatus = "OUTCOME_UNKNOWN";
                OrganizationInvitationMessage =
                    "The invitation management result could not be confirmed. Pending invitations were refreshed when possible; review the current Organization state before resending or revoking again.";
            }
        }
        catch (ArgumentException error)
        {
            OrganizationInvitationStatus = "INVALID_INPUT";
            OrganizationInvitationMessage = error.Message;
        }
    }

    public void CompleteOrganizationInvitationDelivery()
    {
        OrganizationInvitationCode = string.Empty;
        OrganizationInvitationStatus = "IDLE";
        OrganizationInvitationMessage =
            CanManageOrganizationInvitations
                ? "Invitation code dismissed. You may issue another Organization invitation."
                : "Refresh organization details before issuing another invitation.";
    }

    public async Task ManageAccountOrganizationMemberAsync(
        AccountOrganizationMember member,
        string action,
        string? role,
        CancellationToken cancellationToken)
    {
        if (!CanManageOrganizationMemberActions)
        {
            OrganizationMemberManagementStatus = "INVALID_INPUT";
            OrganizationMemberManagementMessage =
                "Refresh the selected Organization before managing members.";
            return;
        }

        if (action is not ("UPDATE_ROLE" or "REMOVE") ||
            string.IsNullOrWhiteSpace(member.ManagementHandle))
        {
            OrganizationMemberManagementStatus = "INVALID_INPUT";
            OrganizationMemberManagementMessage =
                "The selected member cannot be managed safely. Refresh Organization details.";
            return;
        }

        OrganizationMemberManagementStatus = "MANAGING";
        OrganizationMemberManagementMessage =
            action == "REMOVE"
                ? $"Removing {member.Email} through the BKE Licensing Agent…"
                : $"Updating {member.Email} to {role} through the BKE Licensing Agent…";

        try
        {
            var response = await _accountOrganization.ManageMemberAsync(
                action,
                member.ManagementHandle,
                role,
                cancellationToken);

            if (response.Status == "AUTH_REQUIRED")
            {
                ResetAccountOrganizationState();
                EnterAccountOrganizationReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            if (response.Status == "NOT_ORGANIZATION")
            {
                ClearAccountOrganization(
                    "NOT_ORGANIZATION",
                    "The selected BKE account is not an Organization account.");
                return;
            }

            if (response.Status is "UPDATED" or "REMOVED")
            {
                await RefreshAccountOrganizationAsync(
                    cancellationToken);

                if (IsAuthenticated)
                {
                    OrganizationMemberManagementStatus =
                        response.Status;
                    OrganizationMemberManagementMessage =
                        response.Status == "UPDATED"
                            ? $"Member role updated for {member.Email}. Organization members were refreshed from the authoritative account state."
                            : $"Member {member.Email} was removed. Organization members were refreshed from the authoritative account state.";
                }
                return;
            }

            if (response.Status is
                "OUTCOME_UNKNOWN" or
                "MEMBER_NOT_FOUND")
            {
                await RefreshAccountOrganizationAsync(
                    cancellationToken);

                if (IsAuthenticated)
                {
                    OrganizationMemberManagementStatus =
                        response.Status;
                    OrganizationMemberManagementMessage =
                        response.Status == "OUTCOME_UNKNOWN"
                            ? "The member-management result could not be confirmed. Organization members were refreshed when possible; review the current state before changing or removing the member again."
                            : response.Error?.Message ??
                              "The selected member changed. Organization members were refreshed.";
                }
                return;
            }

            OrganizationMemberManagementStatus = response.Status;
            OrganizationMemberManagementMessage =
                response.Error?.Message ??
                "The Organization member was not changed.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            try
            {
                await RefreshAccountOrganizationAsync(
                    cancellationToken);
            }
            catch
            {
                // RefreshAccountOrganizationAsync reports its own state.
            }

            if (IsAuthenticated)
            {
                OrganizationMemberManagementStatus =
                    "OUTCOME_UNKNOWN";
                OrganizationMemberManagementMessage =
                    "The member-management result could not be confirmed. Organization members were refreshed when possible; review the current state before changing or removing the member again.";
            }
        }
        catch (ArgumentException error)
        {
            OrganizationMemberManagementStatus = "INVALID_INPUT";
            OrganizationMemberManagementMessage = error.Message;
        }
    }

    public async Task TransferAccountOrganizationOwnershipAsync(
        AccountOrganizationMember member,
        CancellationToken cancellationToken)
    {
        if (!ShowOrganizationOwnershipTransferSection)
        {
            OrganizationOwnershipTransferStatus = "INVALID_INPUT";
            OrganizationOwnershipTransferMessage =
                "Refresh the selected Organization. Ownership transfer is available only when Digital Solutions authorizes it.";
            return;
        }

        if (_purchaseAttemptLocked)
        {
            OrganizationOwnershipTransferStatus = "BLOCKED";
            OrganizationOwnershipTransferMessage =
                "Resolve the existing checkout attempt before transferring Organization ownership. Checkout recovery is bound to the current BKE identity and account.";
            return;
        }

        if (string.IsNullOrWhiteSpace(member.ManagementHandle))
        {
            OrganizationOwnershipTransferStatus = "INVALID_INPUT";
            OrganizationOwnershipTransferMessage =
                "The selected member cannot receive ownership safely. Refresh Organization details.";
            return;
        }

        var organizationDisplayName = OrganizationDisplayName;
        OrganizationOwnershipTransferStatus = "TRANSFERRING";
        OrganizationOwnershipTransferMessage =
            $"Transferring {organizationDisplayName} ownership to {member.Email} through the BKE Licensing Agent…";

        try
        {
            var response = await _accountOrganization.TransferOwnershipAsync(
                member.ManagementHandle,
                cancellationToken);

            if (response.Status == "TRANSFERRED" &&
                response.ReauthenticationRequired)
            {
                EnterAccountOrganizationReauthentication(
                    $"{organizationDisplayName} ownership was transferred to {member.Email}. Sign in again to refresh your authoritative BKE account access.");
                return;
            }

            if (response.Status is
                "AUTH_REQUIRED" or
                "OUTCOME_UNKNOWN")
            {
                if (!response.ReauthenticationRequired)
                {
                    throw new InvalidDataException(
                        "Organization ownership-transfer reauthentication boundary drifted.");
                }

                EnterAccountOrganizationReauthentication(
                    response.Status == "OUTCOME_UNKNOWN"
                        ? "The Organization ownership-transfer result could not be confirmed. BKE will not replay the request. Sign in again and inspect the authoritative Organization state before deciding whether to try again."
                        : response.Error?.Message ??
                          "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            if (response.Status == "NOT_ORGANIZATION")
            {
                ClearAccountOrganization(
                    "NOT_ORGANIZATION",
                    "The selected BKE account is not an Organization account.");
                return;
            }

            OrganizationOwnershipTransferStatus = response.Status;
            OrganizationOwnershipTransferMessage =
                response.Error?.Message ??
                "Organization ownership was not changed.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            // The Agent may already have submitted a destructive transfer to
            // Digital Solutions. Never replay an ambiguous transfer and never
            // retain stale selected-account authorization assumptions.
            EnterAccountOrganizationReauthentication(
                "The Organization ownership-transfer result could not be confirmed. BKE will not replay the request. Sign in again and inspect the authoritative Organization state before deciding whether to try again.");
        }
        catch (ArgumentException error)
        {
            OrganizationOwnershipTransferStatus = "INVALID_INPUT";
            OrganizationOwnershipTransferMessage = error.Message;
        }
    }

    public async Task LeaveAccountOrganizationAsync(
        CancellationToken cancellationToken)
    {
        if (!ShowOrganizationLeaveSection)
        {
            OrganizationLeaveStatus = "INVALID_INPUT";
            OrganizationLeaveMessage =
                "Refresh the selected Organization. Leave is available only when Digital Solutions authorizes this membership.";
            return;
        }

        if (_purchaseAttemptLocked)
        {
            OrganizationLeaveStatus = "BLOCKED";
            OrganizationLeaveMessage =
                "Resolve the existing checkout attempt before leaving this Organization. Checkout recovery is bound to the current BKE identity and account.";
            return;
        }

        var organizationDisplayName = OrganizationDisplayName;
        OrganizationLeaveStatus = "LEAVING";
        OrganizationLeaveMessage =
            "Leaving the selected Organization through the BKE Licensing Agent…";

        try
        {
            var response = await _accountOrganization.LeaveAsync(
                cancellationToken);

            if (response.Status == "LEFT" &&
                response.ReauthenticationRequired)
            {
                EnterAccountOrganizationReauthentication(
                    $"You left {organizationDisplayName}. Sign in again to choose an available BKE account.");
                return;
            }

            if (response.Status is
                "AUTH_REQUIRED" or
                "MEMBER_NOT_FOUND" or
                "OUTCOME_UNKNOWN")
            {
                if (!response.ReauthenticationRequired)
                {
                    throw new InvalidDataException(
                        "Organization leave reauthentication boundary drifted.");
                }

                var message = response.Status switch
                {
                    "MEMBER_NOT_FOUND" =>
                        response.Error?.Message ??
                        "The selected Organization membership is no longer available. Sign in again.",
                    "OUTCOME_UNKNOWN" =>
                        "The Organization leave result could not be confirmed. BKE will not replay the request. Sign in again and check the available accounts before deciding whether to try again.",
                    _ =>
                        response.Error?.Message ??
                        "Your BKE account session is no longer valid. Sign in again.",
                };
                EnterAccountOrganizationReauthentication(message);
                return;
            }

            if (response.Status == "NOT_ORGANIZATION")
            {
                ClearAccountOrganization(
                    "NOT_ORGANIZATION",
                    "The selected BKE account is not an Organization account.");
                return;
            }

            OrganizationLeaveStatus = response.Status;
            OrganizationLeaveMessage =
                response.Error?.Message ??
                (response.Status == "OWNER_CANNOT_LEAVE"
                    ? "Transfer Organization ownership before leaving."
                    : "The Organization membership was not changed.");
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            // The local Agent may have received and committed the destructive
            // request even when Launcher did not receive a valid response.
            // Never replay it and never retain selected-account presentation.
            EnterAccountOrganizationReauthentication(
                "The Organization leave result could not be confirmed. BKE will not replay the request. Sign in again and check the available accounts before deciding whether to try again.");
        }
    }

    public async Task RefreshAccountPrivacyAsync(
        CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            ClearAccountPrivacy(
                "AUTH_REQUIRED",
                "Sign in with BKE before opening privacy requests.",
                clearSummary: true);
            return;
        }

        AccountPrivacyStatus = "LOADING";
        AccountPrivacyMessage =
            "Loading privacy requests through the BKE Licensing Agent…";

        try
        {
            var response = await _accountPrivacy.ListAsync(
                100,
                cancellationToken);

            if (response.Status == "AUTH_REQUIRED")
            {
                EnterAccountPrivacyReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            AccountPrivacyRequestTypes.Clear();
            AccountPrivacyRequests.Clear();
            SelectedAccountPrivacyRequestType = null;

            if (response.Status != "READY")
            {
                AccountPrivacyStatus = response.Status;
                AccountPrivacyMessage =
                    response.Error?.Message ??
                    "BKE privacy requests are temporarily unavailable.";
                RaiseAccountPrivacyCapabilities();
                return;
            }

            foreach (var requestType in response.RequestTypes)
            {
                AccountPrivacyRequestTypes.Add(requestType);
            }
            SelectedAccountPrivacyRequestType =
                AccountPrivacyRequestTypes.FirstOrDefault();

            foreach (var item in response.Items)
            {
                AccountPrivacyRequests.Add(
                    AccountPrivacyRequestViewModel.From(item));
            }

            AccountPrivacyStatus = "READY";
            AccountPrivacyMessage = AccountPrivacyRequests.Count == 0
                ? "No privacy requests are currently recorded for this BKE identity and selected account."
                : $"{AccountPrivacyRequests.Count} privacy request(s) loaded from BKE Digital Solutions.";
            RaiseAccountPrivacyCapabilities();
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentOutOfRangeException)
        {
            AccountPrivacyRequestTypes.Clear();
            AccountPrivacyRequests.Clear();
            SelectedAccountPrivacyRequestType = null;
            AccountPrivacyStatus = "AGENT_UNAVAILABLE";
            AccountPrivacyMessage =
                "Privacy requests are unavailable or the Licensing Agent returned an invalid response.";
            RaiseAccountPrivacyCapabilities();
        }
    }

    public async Task CreateAccountPrivacyRequestAsync(
        CancellationToken cancellationToken)
    {
        if (!CanCreateAccountPrivacyRequest ||
            string.IsNullOrWhiteSpace(SelectedAccountPrivacyRequestType))
        {
            AccountPrivacyStatus = "INVALID_INPUT";
            AccountPrivacyMessage =
                "Refresh privacy requests, choose an authoritative request type, and enter a 10–2,000 character summary.";
            return;
        }

        var requestType = SelectedAccountPrivacyRequestType;
        var summary = AccountPrivacySummary;

        AccountPrivacyStatus = "CREATING";
        AccountPrivacyMessage =
            "Submitting the privacy request through the BKE Licensing Agent…";

        try
        {
            var response = await _accountPrivacy.CreateAsync(
                requestType,
                summary,
                cancellationToken);

            if (response.Status == "AUTH_REQUIRED")
            {
                EnterAccountPrivacyReauthentication(
                    "Your BKE account session is no longer valid. Sign in again.");
                return;
            }

            if (response.Status == "CREATED")
            {
                AccountPrivacySummary = string.Empty;
                await RefreshAccountPrivacyAsync(cancellationToken);
                if (IsAuthenticated)
                {
                    AccountPrivacyMessage =
                        $"Privacy request {response.RequestId ?? string.Empty} was created and the request list was refreshed.";
                }
                return;
            }

            if (response.Status == "OUTCOME_UNKNOWN")
            {
                AccountPrivacySummary = string.Empty;
                await RefreshAccountPrivacyAsync(cancellationToken);
                if (IsAuthenticated)
                {
                    AccountPrivacyMessage =
                        "The create result could not be confirmed. The request list was refreshed; verify that a matching request is not already present before submitting again.";
                }
                return;
            }

            AccountPrivacyStatus = response.Status;
            AccountPrivacyMessage =
                response.Error?.Message ??
                "The privacy request was not created.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            AccountPrivacySummary = string.Empty;
            try
            {
                await RefreshAccountPrivacyAsync(cancellationToken);
            }
            catch
            {
                // RefreshAccountPrivacyAsync already reports its own failure state.
            }

            if (IsAuthenticated)
            {
                AccountPrivacyMessage =
                    "The create result could not be confirmed. The request list was refreshed when possible; verify that a matching request is not already present before submitting again.";
            }
        }
        catch (ArgumentException error)
        {
            AccountPrivacyStatus = "INVALID_INPUT";
            AccountPrivacyMessage = error.Message;
        }
    }

    private void ApplyAccountMfaChallenge(
        AccountMfaChallengeResponse response,
        string purpose)
    {
        AccountMfaStatus = response.Status;
        if (response.Status == "CHALLENGE_ISSUED" &&
            !string.IsNullOrWhiteSpace(response.ChallengeToken))
        {
            _accountMfaChallengeToken = response.ChallengeToken;
            _accountMfaChallengePurpose = purpose;
            AccountMfaReference = string.IsNullOrWhiteSpace(response.MfaReference)
                ? string.Empty
                : $"Reference: {response.MfaReference}";
            AccountMfaCode = string.Empty;
            AccountMfaMessage = response.EmailSent
                ? "Enter the verification code sent to your account email."
                : "Enter your BKE MFA verification code.";
            RaiseAccountMfaCapabilities();
            return;
        }

        AccountMfaMessage =
            response.Error?.Message ?? "MFA verification could not be started.";
        RaiseAccountMfaCapabilities();
    }

    private async Task RunAccountMfaMutationAsync(
        Func<string, string, string, CancellationToken, Task<AccountMfaMutationResponse>> operation,
        string operationName,
        CancellationToken cancellationToken)
    {
        if (!IsAuthenticated ||
            string.IsNullOrWhiteSpace(_accountMfaChallengeToken) ||
            string.IsNullOrEmpty(AccountMfaCurrentPassword) ||
            AccountMfaCode.Length is < 6 or > 32)
        {
            AccountMfaStatus = "INVALID_INPUT";
            AccountMfaMessage =
                $"Complete MFA verification before {operationName.ToLowerInvariant()}.";
            return;
        }

        var password = AccountMfaCurrentPassword;
        var challenge = _accountMfaChallengeToken;
        var code = AccountMfaCode;

        try
        {
            AccountMfaStatus = "UPDATING";
            var response = await operation(password, challenge, code, cancellationToken);
            AccountMfaCode = string.Empty;

            if (response.ReauthenticationRequired)
            {
                var recoveryCodes = response.RecoveryCodes;
                var message = response.Error?.Message ?? response.Status switch
                {
                    "MFA_ENABLED" =>
                        "MFA enabled. Save the recovery codes, then sign in again.",
                    "MFA_DISABLED" =>
                        "MFA disabled. Sign in again.",
                    "RECOVERY_CODES_REGENERATED" =>
                        "Recovery codes regenerated. Save them, then sign in again.",
                    _ =>
                        "The account-security result requires signing in again before retrying.",
                };

                EnterAccountMfaReauthentication(message, clearRecoveryCodes: true);
                if (recoveryCodes is { Count: > 0 })
                {
                    AccountMfaRecoveryCodes =
                        string.Join(Environment.NewLine, recoveryCodes);
                }
                return;
            }

            AccountMfaStatus = response.Status;
            AccountMfaMessage =
                response.Error?.Message ?? $"{operationName} was not completed.";
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException or
            ArgumentException)
        {
            EnterAccountMfaReauthentication(
                $"The {operationName.ToLowerInvariant()} result could not be confirmed. Sign in again before retrying.",
                clearRecoveryCodes: true);
        }
    }

    public async Task ChangePasswordAsync(CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            PasswordChangeStatus = "AUTH_REQUIRED";
            PasswordChangeMessage = "Sign in with BKE before changing your password.";
            return;
        }

        if (string.IsNullOrEmpty(CurrentPassword) ||
            string.IsNullOrEmpty(NewPassword) ||
            string.IsNullOrEmpty(ConfirmNewPassword))
        {
            PasswordChangeStatus = "INVALID_INPUT";
            PasswordChangeMessage = "Enter your current password, new password, and confirmation.";
            return;
        }

        if (CurrentPassword.Length > 128 ||
            NewPassword.Length > 128 ||
            ConfirmNewPassword.Length > 128)
        {
            PasswordChangeStatus = "INVALID_INPUT";
            PasswordChangeMessage = "Password fields must be 128 characters or fewer.";
            return;
        }

        if (!string.Equals(NewPassword, ConfirmNewPassword, StringComparison.Ordinal))
        {
            PasswordChangeStatus = "INVALID_INPUT";
            PasswordChangeMessage = "The new password and confirmation do not match.";
            return;
        }

        PasswordChangeStatus = "CHANGING";
        PasswordChangeMessage = "Changing your BKE password through the Licensing Agent…";

        try
        {
            var response = await _accountPasswordChange.ChangeAsync(
                CurrentPassword,
                NewPassword,
                cancellationToken);

            if (response.ReauthenticationRequired)
            {
                var message = response.Status == "CHANGED"
                    ? "Password changed. Sign in again with your new password."
                    : response.Error?.Message ??
                      "Your BKE session must be authenticated again before continuing.";
                EnterPasswordChangeReauthentication(response.Status, message);
                return;
            }

            PasswordChangeStatus = response.Status;
            PasswordChangeMessage = response.Error?.Message ?? response.Status switch
            {
                "INVALID_CREDENTIALS" => "The current password was not accepted.",
                "INVALID_INPUT" => "The new password does not satisfy BKE account requirements.",
                _ => "Password change could not be completed.",
            };
        }
        catch (Exception error) when (
            error is HttpRequestException or
            TaskCanceledException or
            InvalidDataException)
        {
            EnterPasswordChangeReauthentication(
                "REAUTHENTICATION_REQUIRED",
                "The password-change result could not be confirmed. Sign in again before retrying.");
        }
    }

    public async Task SwitchAccountAsync(
        CancellationToken cancellationToken)
    {
        if (!IsAuthenticated)
        {
            return;
        }

        if (_purchaseAttemptLocked)
        {
            Message =
                "Resolve the existing checkout attempt before switching accounts. Its retained correlation is bound to the current BKE identity and account.";
            return;
        }

        var preservedEmail = _authenticatedAccountEmail;
        await LogoutAsync(cancellationToken);

        if (SessionStatus != "SIGNED_OUT")
        {
            return;
        }

        Email = preservedEmail;
        Password = string.Empty;
        ClearNativeMfaState();
        AvailableAccounts.Clear();
        SelectedAccount = null;
        Raise(nameof(HasAccountChoices));

        var revokeWarning = Message.Contains(
            "remote revocation",
            StringComparison.OrdinalIgnoreCase);

        Message = revokeWarning
            ? Message + " Re-enter your password to choose another eligible BKE account."
            : "Current BKE account signed out. Re-enter your password to choose another eligible BKE account. Password and MFA must be completed again.";
    }

    public async Task LogoutAsync(CancellationToken cancellationToken)
    {
        var preserveCheckoutRecovery =
            _purchaseAttemptLocked &&
            !string.IsNullOrWhiteSpace(_checkoutRecoveryCorrelationId);

        try
        {
            var response = await _accountSession.LogoutAsync(cancellationToken);
            ClearNativeMfaState();
            ResetAccountMfaState(clearRecoveryCodes: true);
            ResetAccountPrivacyState();
        ResetAccountOrganizationState();
            ResetShellSurface();
            SessionStatus = response.Status;
            AccountDisplay = "Not signed in";
            _authenticatedAccountEmail = string.Empty;
            _authenticatedAccountType = string.Empty;
            Raise(nameof(AccountTypeLabel));
            RaiseAccountOrganizationCapabilities();
            UserCode = string.Empty;
            VerificationUri = string.Empty;
            Password = string.Empty;
            ClaimCode = string.Empty;
            ClaimStatus = "AUTH_REQUIRED";
            ClaimMessage = "Sign in to redeem a Claim Code.";
            AvailableAccounts.Clear();
            SelectedAccount = null;
            Raise(nameof(HasAccountChoices));
            GiftClaimCode = string.Empty;
            Message = preserveCheckoutRecovery
                ? "Signed out. The existing checkout correlation remains locked. Sign back in with the same BKE identity and account on this device to continue recovery."
                : response.Error?.Message ?? "Signed out on this machine.";
            ClearCatalog(
                "AUTH_REQUIRED",
                "Sign in to load your BKE software.");
            ClearStore(
                "AUTH_REQUIRED",
                "Sign in to browse the BKE Store.");
            ClearNotifications(
                "AUTH_REQUIRED",
                "Sign in to view BKE notifications.");
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or InvalidDataException)
        {
            SessionStatus = "AGENT_UNAVAILABLE";
            Message = "The local session could not be changed because the BKE Licensing Agent is unavailable.";
        }
    }

    private void ResetShellSurface()
    {
        ShowAccountSurface = false;
        SelectedModuleIndex = -1;
    }

    private void EnterPasswordChangeReauthentication(
        string status,
        string message)
    {
        ClearPasswordChangeFields();
        ClearNativeMfaState();
        ResetAccountMfaState(clearRecoveryCodes: true);
        ResetAccountPrivacyState();
        ResetAccountOrganizationState();
        PasswordChangeStatus = status;
        PasswordChangeMessage = message;
        ResetShellSurface();
        SessionStatus = "SIGNED_OUT";
        AccountDisplay = "Not signed in";
        _authenticatedAccountEmail = string.Empty;
        _authenticatedAccountType = string.Empty;
        Raise(nameof(AccountTypeLabel));
        UserCode = string.Empty;
        VerificationUri = string.Empty;
        Password = string.Empty;
        ClaimCode = string.Empty;
        ClaimStatus = "AUTH_REQUIRED";
        ClaimMessage = "Sign in to redeem a Claim Code.";
        AvailableAccounts.Clear();
        SelectedAccount = null;
        Raise(nameof(HasAccountChoices));
        GiftClaimCode = string.Empty;
        Message = message;
        ClearCatalog("AUTH_REQUIRED", "Sign in to load your BKE software.");
        ClearStore("AUTH_REQUIRED", "Sign in to browse the BKE Store.");
        ClearNotifications("AUTH_REQUIRED", "Sign in to view BKE notifications.");
    }

    private void ClearPasswordChangeFields()
    {
        CurrentPassword = string.Empty;
        NewPassword = string.Empty;
        ConfirmNewPassword = string.Empty;
    }

    private void ClearNativeMfaState()
    {
        _nativeMfaChallengeToken = null;
        NativeMfaCode = string.Empty;
        NativeMfaReference = string.Empty;
        NativeMfaMessage = string.Empty;
        Raise(nameof(ShowNativeMfaChallenge));
        Raise(nameof(CanVerifyNativeMfa));
    }

    private void ClearAccountMfaChallenge()
    {
        _accountMfaChallengeToken = null;
        _accountMfaChallengePurpose = string.Empty;
        AccountMfaCode = string.Empty;
        AccountMfaReference = string.Empty;
        RaiseAccountMfaCapabilities();
    }

    private void ResetAccountMfaState(bool clearRecoveryCodes)
    {
        AccountMfaCurrentPassword = string.Empty;
        ClearAccountMfaChallenge();
        AccountMfaStatus = "UNKNOWN";
        AccountMfaMessage = "Refresh MFA status to manage account security.";
        AccountMfaEnabled = false;
        AccountMfaEnrollmentPending = false;
        AccountMfaRecoveryCodesRemaining = 0;
        if (clearRecoveryCodes)
        {
            AccountMfaRecoveryCodes = string.Empty;
        }
    }

    private void EnterAccountMfaReauthentication(
        string message,
        bool clearRecoveryCodes)
    {
        ClearNativeMfaState();
        ResetAccountMfaState(clearRecoveryCodes);
        ResetAccountPrivacyState();
        ResetAccountOrganizationState();
        ResetPasswordChangeState();
        ResetShellSurface();
        SessionStatus = "SIGNED_OUT";
        AccountDisplay = "Not signed in";
        _authenticatedAccountEmail = string.Empty;
        _authenticatedAccountType = string.Empty;
        Raise(nameof(AccountTypeLabel));
        UserCode = string.Empty;
        VerificationUri = string.Empty;
        Password = string.Empty;
        ClaimCode = string.Empty;
        ClaimStatus = "AUTH_REQUIRED";
        ClaimMessage = "Sign in to redeem a Claim Code.";
        AvailableAccounts.Clear();
        SelectedAccount = null;
        Raise(nameof(HasAccountChoices));
        GiftClaimCode = string.Empty;
        Message = message;
        ClearCatalog("AUTH_REQUIRED", "Sign in to load your BKE software.");
        ClearStore("AUTH_REQUIRED", "Sign in to browse the BKE Store.");
        ClearNotifications("AUTH_REQUIRED", "Sign in to view BKE notifications.");
    }


    private void ResetAccountOrganizationState()
    {
        ClearOrganizationCreateFields();
        _organizationCreateRetryBlocked = false;
        OrganizationCreateStatus = "IDLE";
        OrganizationCreateMessage =
            "Create a BKE Organization, then switch accounts to select it.";
        ResetOrganizationProfileEditor();
        ResetOrganizationInvitationAcceptanceState();
        ResetOrganizationInvitationState();
        ResetOrganizationMemberManagementState();
        ResetOrganizationOwnershipTransferState();
        ResetOrganizationLeaveState();
        _organizationAccount = null;
        _organizationPermissions = null;
        _organizationProfile = null;
        _organizationCounts = null;
        _organizationBillingEmail = null;
        _organizationTaxId = null;
        OrganizationMembers.Clear();
        OrganizationInvitations.Clear();
        AccountOrganizationStatus = "UNKNOWN";
        AccountOrganizationMessage =
            "Refresh organization details to load the Agent-authoritative selected-account overview.";
        RaiseAccountOrganizationCapabilities();
    }

    private void ClearOrganizationCreateFields()
    {
        OrganizationCreateDisplayName = string.Empty;
        OrganizationCreateLegalName = string.Empty;
        OrganizationCreateBillingEmail = string.Empty;
        OrganizationCreateRegistrationNumber = string.Empty;
        OrganizationCreateTaxId = string.Empty;
    }

    private void ResetOrganizationMemberManagementState()
    {
        OrganizationMemberManagementStatus = "IDLE";
        OrganizationMemberManagementMessage =
            "Manage members after loading the Agent-authoritative Organization overview.";
    }

    private void ResetOrganizationOwnershipTransferState()
    {
        OrganizationOwnershipTransferStatus = "IDLE";
        OrganizationOwnershipTransferMessage =
            "Transfer ownership only when Digital Solutions authorizes the selected Organization.";
    }

    private void ResetOrganizationLeaveState()
    {
        OrganizationLeaveStatus = "IDLE";
        OrganizationLeaveMessage =
            "Leave is available only when Digital Solutions authorizes the selected Organization membership.";
    }

    private void ResetOrganizationInvitationAcceptanceState()
    {
        OrganizationInvitationAcceptanceCode = string.Empty;
        OrganizationInvitationAcceptedRole = string.Empty;
        OrganizationInvitationAcceptanceStatus = "IDLE";
        OrganizationInvitationAcceptanceMessage =
            "Enter an Organization invitation code to join it with the signed-in BKE identity.";
    }

    private void ResetOrganizationInvitationState()
    {
        ClearOrganizationInvitationForm();
        OrganizationInvitationCode = string.Empty;
        OrganizationInvitationStatus = "IDLE";
        OrganizationInvitationMessage =
            "Invite members after loading the Agent-authoritative Organization overview.";
    }

    private void ClearOrganizationInvitationForm()
    {
        OrganizationInvitationEmail = string.Empty;
        OrganizationInvitationRole = "MEMBER";
    }

    private static bool ValidOrganizationMemberRole(string? role) =>
        role is "OWNER" or "BILLING" or "LICENSE_MANAGER" or "MEMBER";

    private static bool ValidOrganizationInvitationAcceptanceCode(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim();
        return normalized.Length is >= 8 and <= 1024 &&
               normalized.All(character => character >= 32);
    }

    private void ResetOrganizationProfileEditor()
    {
        OrganizationEditDisplayName = string.Empty;
        OrganizationEditLegalName = string.Empty;
        OrganizationEditRegistrationNumber = string.Empty;
        OrganizationEditBillingEmail = string.Empty;
        OrganizationEditTaxId = string.Empty;
        OrganizationProfileUpdateStatus = "IDLE";
        OrganizationProfileUpdateMessage =
            "Refresh organization details before editing the selected Organization.";
    }

    private void LoadOrganizationProfileEditorFromAuthority()
    {
        OrganizationEditDisplayName =
            _organizationAccount?.DisplayName ?? string.Empty;
        OrganizationEditLegalName =
            _organizationProfile?.LegalName ?? string.Empty;
        OrganizationEditRegistrationNumber =
            _organizationProfile?.RegistrationNumber ?? string.Empty;
        OrganizationEditBillingEmail =
            _organizationBillingEmail ?? string.Empty;
        OrganizationEditTaxId =
            _organizationTaxId ?? string.Empty;
        OrganizationProfileUpdateStatus = "IDLE";
        OrganizationProfileUpdateMessage =
            _organizationPermissions?.ManageMembers == true ||
            _organizationPermissions?.ViewBilling == true
                ? "Edit only the fields allowed by the selected Organization role."
                : "This Organization role has no profile-edit permissions.";
    }

    private static string? NormalizeOrganizationOptional(
        string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? null
            : value.Trim();

    private static bool ValidOrganizationCreateText(
        string? value,
        int minimum,
        int maximum) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Trim().Length >= minimum &&
        value.Trim().Length <= maximum &&
        value.All(character => character >= 32);

    private static bool ValidOrganizationCreateEmail(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Length <= 320 &&
        System.Net.Mail.MailAddress.TryCreate(
            value.Trim(),
            out var parsed) &&
        string.Equals(
            parsed.Address,
            value.Trim(),
            StringComparison.OrdinalIgnoreCase);

    private void ClearAccountOrganization(
        string status,
        string message)
    {
        _organizationAccount = null;
        _organizationPermissions = null;
        _organizationProfile = null;
        _organizationCounts = null;
        _organizationBillingEmail = null;
        _organizationTaxId = null;
        ResetOrganizationProfileEditor();
        ResetOrganizationInvitationState();
        ResetOrganizationMemberManagementState();
        ResetOrganizationOwnershipTransferState();
        ResetOrganizationLeaveState();
        OrganizationMembers.Clear();
        OrganizationInvitations.Clear();
        AccountOrganizationStatus = status;
        AccountOrganizationMessage = message;
        RaiseAccountOrganizationCapabilities();
    }

    private void EnterAccountOrganizationReauthentication(
        string message)
    {
        EnterAccountMfaReauthentication(
            message,
            clearRecoveryCodes: true);
    }

    private void RaiseAccountOrganizationCapabilities()
    {
        Raise(nameof(ShowOrganizationCreateSection));
        Raise(nameof(CanCreateOrganization));
        Raise(nameof(OrganizationCreateRetryBlocked));
        Raise(nameof(OrganizationProfileUpdateStatus));
        Raise(nameof(OrganizationProfileUpdateMessage));
        Raise(nameof(ShowOrganizationSection));
        Raise(nameof(CanRefreshAccountOrganization));
        Raise(nameof(OrganizationReady));
        Raise(nameof(OrganizationDisplayName));
        Raise(nameof(OrganizationRole));
        Raise(nameof(OrganizationLifecycle));
        Raise(nameof(OrganizationLegalName));
        Raise(nameof(OrganizationRegistrationNumber));
        Raise(nameof(HasOrganizationRegistrationNumber));
        Raise(nameof(OrganizationBillingEmail));
        Raise(nameof(HasOrganizationBillingEmail));
        Raise(nameof(OrganizationTaxId));
        Raise(nameof(HasOrganizationTaxId));
        Raise(nameof(ShowOrganizationBilling));
        Raise(nameof(HasOrganizationLicenseCount));
        Raise(nameof(HasOrganizationSubscriptionCount));
        Raise(nameof(HasOrganizationOrderCount));
        Raise(nameof(OrganizationLicenseCount));
        Raise(nameof(OrganizationSubscriptionCount));
        Raise(nameof(OrganizationOrderCount));
        Raise(nameof(ShowOrganizationUsage));
        Raise(nameof(CanEditOrganizationIdentity));
        Raise(nameof(CanEditOrganizationBilling));
        Raise(nameof(ShowOrganizationProfileEditor));
        Raise(nameof(OrganizationIdentityProfileDirty));
        Raise(nameof(OrganizationBillingProfileDirty));
        Raise(nameof(CanSaveOrganizationProfile));
        Raise(nameof(OrganizationInvitationAcceptanceStatus));
        Raise(nameof(OrganizationInvitationAcceptanceMessage));
        Raise(nameof(OrganizationInvitationAcceptedRole));
        Raise(nameof(ShowOrganizationInvitationAcceptanceSection));
        Raise(nameof(CanAcceptOrganizationInvitation));
        Raise(nameof(OrganizationInvitationAccepted));
        Raise(nameof(ShowOrganizationInvitationAcceptanceSwitch));
        Raise(nameof(OrganizationInvitationStatus));
        Raise(nameof(OrganizationInvitationMessage));
        Raise(nameof(HasOrganizationInvitationCode));
        Raise(nameof(CanManageOrganizationInvitations));
        Raise(nameof(CanManageOrganizationInvitationActions));
        Raise(nameof(ShowOrganizationInviteSection));
        Raise(nameof(CanInviteOrganizationMember));
        Raise(nameof(OrganizationMemberManagementStatus));
        Raise(nameof(OrganizationMemberManagementMessage));
        Raise(nameof(CanManageOrganizationMemberActions));
        Raise(nameof(ShowOrganizationMembers));
        Raise(nameof(ShowEmptyOrganizationMembers));
        Raise(nameof(ShowEmptyOrganizationInvitations));
        Raise(nameof(OrganizationOwnershipTransferStatus));
        Raise(nameof(OrganizationOwnershipTransferMessage));
        Raise(nameof(ShowOrganizationOwnershipTransferSection));
        Raise(nameof(CanTransferOrganizationOwnership));
        Raise(nameof(OrganizationLeaveStatus));
        Raise(nameof(OrganizationLeaveMessage));
        Raise(nameof(ShowOrganizationLeaveSection));
        Raise(nameof(CanLeaveOrganization));
    }

    private void ResetAccountPrivacyState()
    {
        AccountPrivacySummary = string.Empty;
        SelectedAccountPrivacyRequestType = null;
        AccountPrivacyRequestTypes.Clear();
        AccountPrivacyRequests.Clear();
        AccountPrivacyStatus = "UNKNOWN";
        AccountPrivacyMessage =
            "Refresh privacy requests to load the authoritative request types and history.";
        RaiseAccountPrivacyCapabilities();
    }

    private void ClearAccountPrivacy(
        string status,
        string message,
        bool clearSummary)
    {
        if (clearSummary)
        {
            AccountPrivacySummary = string.Empty;
        }

        SelectedAccountPrivacyRequestType = null;
        AccountPrivacyRequestTypes.Clear();
        AccountPrivacyRequests.Clear();
        AccountPrivacyStatus = status;
        AccountPrivacyMessage = message;
        RaiseAccountPrivacyCapabilities();
    }

    private void EnterAccountPrivacyReauthentication(string message)
    {
        EnterAccountMfaReauthentication(
            message,
            clearRecoveryCodes: true);
    }

    private void RaiseAccountPrivacyCapabilities()
    {
        Raise(nameof(CanRefreshAccountPrivacy));
        Raise(nameof(CanCreateAccountPrivacyRequest));
        Raise(nameof(ShowEmptyAccountPrivacyRequests));
    }

    private void RaiseAccountMfaCapabilities()
    {
        Raise(nameof(AccountMfaSummary));
        Raise(nameof(HasAccountMfaChallenge));
        Raise(nameof(ShowMfaEnrollmentChallenge));
        Raise(nameof(ShowMfaProofChallenge));
        Raise(nameof(ShowMfaEnableActions));
        Raise(nameof(ShowMfaProtectedActions));
        Raise(nameof(CanStartMfaEnrollment));
        Raise(nameof(CanStartMfaProof));
        Raise(nameof(CanCompleteMfaEnrollment));
        Raise(nameof(CanSubmitMfaProof));
    }

    private void RegistrationLegalAcceptanceChanged(
        object? sender,
        PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(RegistrationLegalDocumentViewModel.IsAccepted))
        {
            Raise(nameof(CanCreateAccount));
        }
    }

    private void ResetRegistrationFields(bool clearEmail)
    {
        RegistrationPassword = string.Empty;
        RegistrationCode = string.Empty;
        RegistrationName = string.Empty;
        if (clearEmail)
        {
            RegistrationEmail = string.Empty;
        }
        RegistrationLegalDocuments.Clear();
        RaiseRegistrationCapabilities();
    }

    private void ResetRegistrationState()
    {
        ResetRegistrationFields(clearEmail: true);
        _showRegistration = false;
        RegistrationStatus = "IDLE";
        RegistrationMessage =
            "Create your BKE account without leaving the Launcher.";
        RaiseRegistrationCapabilities();
    }

    private void RaiseRegistrationCapabilities()
    {
        Raise(nameof(ShowSignInForm));
        Raise(nameof(ShowRegistration));
        Raise(nameof(ShowRegistrationEntry));
        Raise(nameof(ShowRegistrationVerification));
        Raise(nameof(CanCreateAccount));
        Raise(nameof(CanVerifyRegistrationEmail));
        Raise(nameof(CanResendRegistrationEmail));
    }

    private void ResetPasswordResetState()
    {
        PasswordResetStatus = "IDLE";
        PasswordResetMessage =
            "Forgot your password? Request a one-time reset link by email.";
    }

    private void ResetPasswordChangeState()
    {
        ClearPasswordChangeFields();
        PasswordChangeStatus = "IDLE";
        PasswordChangeMessage = "Use your current password to set a new BKE password.";
    }

    private void ApplyStatus(AccountSessionStatusResponse response)
    {
        SessionStatus = response.Status;

        if (response.Account is not null)
        {
            AccountDisplay = $"{response.Account.DisplayName} · {response.Account.Email}";
            _authenticatedAccountEmail = response.Account.Email;
            _authenticatedAccountType = response.Account.AccountType;
            Raise(nameof(AccountTypeLabel));
            RaiseAccountOrganizationCapabilities();

            if (response.Status == "AUTHENTICATED" &&
                ClaimStatus == "AUTH_REQUIRED")
            {
                ClaimStatus = "READY";
                ClaimMessage =
                    "Enter a one-time Claim Code to redeem it into the signed-in BKE account.";
            }
        }
        else if (response.Status is "SIGNED_OUT" or "DENIED" or "EXPIRED" or "FAILED")
        {
            AccountDisplay = "Not signed in";
            _authenticatedAccountEmail = string.Empty;
            _authenticatedAccountType = string.Empty;
            Raise(nameof(AccountTypeLabel));
            RaiseAccountOrganizationCapabilities();
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

    private void ClearNotifications(string status, string message)
    {
        NotificationStatus = status;
        NotificationMessage = message;
        Notifications.Clear();
        Raise(nameof(ShowEmptyNotifications));
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
                : "Sign in with the same BKE identity and account on this device, then check the saved checkout attempt. The retained correlation stays locked across Agent session replacement.";
        }

        ShowPurchaseReview = preserveRecovery;
        RaisePurchaseActionState();
        RaiseCheckoutRecoveryState();
    }

    private void RestoreCheckoutRecoveryState()
    {
        try
        {
            var recoveryState = _checkoutRecoveryStore.Read();
            if (recoveryState is null)
            {
                return;
            }

            _purchaseAttemptLocked = true;
            _checkoutRecoveryState = recoveryState;
            _checkoutRecoveryCorrelationId = recoveryState.CorrelationId;
            _recoverableCheckoutUrl = null;
            _checkoutRecoveryStateBlocked = false;
            ShowPurchaseReview = true;
            PurchaseReviewStatus = "RECOVERY_REQUIRED";
            PurchaseReviewMessage =
                "A previous checkout attempt must be recovered before another purchase can be reviewed.";
            PurchaseCheckoutStatus = "RECOVERY_REQUIRED";
            PurchaseCheckoutMessage = recoveryState.HasResumeIntent
                ? "Sign in with the same BKE identity and account on this device, then check this saved checkout attempt. If Digital Solutions returns NOT_FOUND, you can resume only this original correlation."
                : "Sign in with the same BKE identity and account on this device, then check this legacy saved checkout attempt. Its original purchase intent was not retained, so BKE will not replay it.";
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
            _checkoutRecoveryState = null;
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
        Raise(nameof(CanRetryOriginalCheckout));
        Raise(nameof(CanOpenExistingCheckout));
        Raise(nameof(CanCompleteGiftDelivery));
        Raise(nameof(ShowPurchaseCheckoutState));
        Raise(nameof(CanSwitchAccount));
        Raise(nameof(SwitchAccountHint));
        Raise(nameof(CanLeaveOrganization));
        Raise(nameof(CanTransferOrganizationOwnership));
        Raise(nameof(CanRedeemClaimCode));
    }

    private static bool LooksLikeClaimCode(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return false;
        }

        var value = code.Trim();
        if (!value.StartsWith(
                "BKE-CLM-",
                StringComparison.OrdinalIgnoreCase) ||
            value.Length != 43)
        {
            return false;
        }

        var groups = value[8..].Split('-');
        return groups.Length == 6 &&
            groups.All(group =>
                group.Length == 5 &&
                group.All(character =>
                    character is >= '0' and <= '9' ||
                    character is >= 'A' and <= 'F' ||
                    character is >= 'a' and <= 'f'));
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

public sealed record AccountPrivacyRequestViewModel(
    string Id,
    string RequestType,
    string Status,
    string ScopeLabel,
    string Summary,
    string ResponseSummary,
    bool HasResponseSummary,
    string CreatedLabel,
    string LifecycleLabel)
{
    public static AccountPrivacyRequestViewModel From(AccountPrivacyItem item)
    {
        var created = FormatTimestamp(item.CreatedAt);
        var reviewed = string.IsNullOrWhiteSpace(item.ReviewedAt)
            ? null
            : $"reviewed {FormatTimestamp(item.ReviewedAt)}";
        var closed = string.IsNullOrWhiteSpace(item.ClosedAt)
            ? null
            : $"closed {FormatTimestamp(item.ClosedAt)}";
        var lifecycle = string.Join(
            " · ",
            new[] { reviewed, closed }
                .Where(value => !string.IsNullOrWhiteSpace(value))!);

        return new AccountPrivacyRequestViewModel(
            item.Id,
            item.RequestType,
            item.Status,
            item.Scope == "ACCOUNT"
                ? "Selected account"
                : "BKE identity",
            item.Summary,
            item.ResponseSummary ?? string.Empty,
            !string.IsNullOrWhiteSpace(item.ResponseSummary),
            $"Created {created}",
            lifecycle);
    }

    private static string FormatTimestamp(string value) =>
        DateTimeOffset.TryParse(value, out var parsed)
            ? parsed.ToLocalTime().ToString("g", CultureInfo.CurrentCulture)
            : value;
}

public sealed record NotificationViewModel(
    string Id,
    string Title,
    string Body,
    string SourceLabel,
    string CategoryLabel,
    string SeverityLabel,
    string StateLabel,
    string CreatedLabel,
    string ProductLabel)
{
    public bool CanMarkRead =>
        string.Equals(StateLabel, "Unread", StringComparison.Ordinal);

    public static NotificationViewModel From(AccountNotificationItem item)
    {
        var created = DateTimeOffset.TryParse(item.CreatedAt, out var parsed)
            ? parsed.ToLocalTime().ToString("g", CultureInfo.CurrentCulture)
            : item.CreatedAt;

        return new NotificationViewModel(
            item.Id,
            item.Title,
            item.Body,
            item.Source,
            item.Category,
            item.Severity,
            item.State,
            created,
            string.IsNullOrWhiteSpace(item.ProductId)
                ? "BKE"
                : item.ProductId);
    }
}

public sealed class RegistrationLegalDocumentViewModel : INotifyPropertyChanged
{
    private bool _isAccepted;

    private RegistrationLegalDocumentViewModel(
        string documentType,
        string title,
        string slug,
        string versionId,
        int versionNumber,
        string? effectiveAt,
        string contentMarkdown)
    {
        DocumentType = documentType;
        Title = title;
        Slug = slug;
        VersionId = versionId;
        VersionNumber = versionNumber;
        EffectiveAt = effectiveAt ?? string.Empty;
        ContentMarkdown = contentMarkdown;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public string DocumentType { get; }
    public string Title { get; }
    public string Slug { get; }
    public string VersionId { get; }
    public int VersionNumber { get; }
    public string EffectiveAt { get; }
    public string ContentMarkdown { get; }
    public string VersionLabel =>
        string.IsNullOrWhiteSpace(EffectiveAt)
            ? $"Version {VersionNumber}"
            : $"Version {VersionNumber} · effective {EffectiveAt}";

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

    public static RegistrationLegalDocumentViewModel From(
        NativeBkeRegistrationLegalDocument document) =>
        new(
            document.DocumentType,
            document.Title,
            document.Slug,
            document.VersionId,
            document.VersionNumber,
            document.EffectiveAt,
            document.ContentMarkdown);
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
