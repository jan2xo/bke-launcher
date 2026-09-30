using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public interface ILauncherAgentClient
{
    Task<PlatformAuthorityResponse> GetPlatformAuthorityAsync(
        PlatformAuthorityRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionDeviceContextResponse> GetAccountSessionDeviceContextAsync(
        AccountSessionDeviceContextRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionCompleteResponse> CompleteAccountSessionAsync(
        AccountSessionCompleteRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionStartResponse> StartAccountSessionAsync(
        AccountSessionStartRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionStatusResponse> GetAccountSessionStatusAsync(
        AccountSessionStatusRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionLogoutResponse> LogoutAccountSessionAsync(
        AccountSessionLogoutRequest request,
        CancellationToken cancellationToken);

    Task<AccountPasswordChangeResponse> ChangeAccountPasswordAsync(
        AccountPasswordChangeRequest request,
        CancellationToken cancellationToken);

    Task<AccountMfaStatusResponse> GetAccountMfaStatusAsync(
        AccountMfaStatusRequest request,
        CancellationToken cancellationToken);

    Task<AccountMfaChallengeResponse> StartAccountMfaEnrollmentAsync(
        AccountMfaEnrollStartRequest request,
        CancellationToken cancellationToken);

    Task<AccountMfaMutationResponse> CompleteAccountMfaEnrollmentAsync(
        AccountMfaEnrollCompleteRequest request,
        CancellationToken cancellationToken);

    Task<AccountMfaChallengeResponse> StartAccountMfaProofAsync(
        AccountMfaProofChallengeRequest request,
        CancellationToken cancellationToken);

    Task<AccountMfaMutationResponse> DisableAccountMfaAsync(
        AccountMfaMutationRequest request,
        CancellationToken cancellationToken);

    Task<AccountMfaMutationResponse> RegenerateAccountMfaRecoveryAsync(
        AccountMfaMutationRequest request,
        CancellationToken cancellationToken);

    Task<AccountPrivacyListResponse> GetAccountPrivacyRequestsAsync(
        AccountPrivacyListRequest request,
        CancellationToken cancellationToken);

    Task<AccountPrivacyCreateResponse> CreateAccountPrivacyRequestAsync(
        AccountPrivacyCreateRequest request,
        CancellationToken cancellationToken);

    Task<AccountOrganizationOverviewResponse> GetAccountOrganizationAsync(
        AccountOrganizationOverviewRequest request,
        CancellationToken cancellationToken);

    Task<AccountOrganizationCreateResponse> CreateAccountOrganizationAsync(
        AccountOrganizationCreateRequest request,
        CancellationToken cancellationToken);

    Task<AccountOrganizationProfileUpdateResponse> UpdateAccountOrganizationProfileAsync(
        AccountOrganizationProfileUpdateRequest request,
        CancellationToken cancellationToken);

    Task<AccountOrganizationInvitationCreateResponse> CreateAccountOrganizationInvitationAsync(
        AccountOrganizationInvitationCreateRequest request,
        CancellationToken cancellationToken);

    Task<AccountOrganizationInvitationManageResponse> ManageAccountOrganizationInvitationAsync(
        AccountOrganizationInvitationManageRequest request,
        CancellationToken cancellationToken);

    Task<AccountOrganizationMemberManageResponse> ManageAccountOrganizationMemberAsync(
        AccountOrganizationMemberManageRequest request,
        CancellationToken cancellationToken);

    Task<AccountOrganizationOwnershipTransferResponse> TransferAccountOrganizationOwnershipAsync(
        AccountOrganizationOwnershipTransferRequest request,
        CancellationToken cancellationToken);

    Task<AccountOrganizationLeaveResponse> LeaveAccountOrganizationAsync(
        AccountOrganizationLeaveRequest request,
        CancellationToken cancellationToken);

    Task<AccountNotificationFeedResponse> GetAccountNotificationsAsync(
        AccountNotificationFeedRequest request,
        CancellationToken cancellationToken);

    Task<AccountNotificationReceiptResponse> MutateAccountNotificationAsync(
        AccountNotificationReceiptRequest request,
        CancellationToken cancellationToken);

    Task<ClaimCodeRedeemResponse> RedeemClaimCodeAsync(
        ClaimCodeRedeemRequest request,
        CancellationToken cancellationToken);

    Task<StoreCatalogResponse> GetStoreCatalogAsync(
        StoreCatalogRequest request,
        CancellationToken cancellationToken);

    Task<StoreCheckoutReviewResponse> ReviewStoreCheckoutAsync(
        StoreCheckoutReviewRequest request,
        CancellationToken cancellationToken);

    Task<StoreCheckoutStartResponse> StartStoreCheckoutAsync(
        StoreCheckoutStartRequest request,
        CancellationToken cancellationToken);

    Task<StoreCheckoutStatusResponse> CheckStoreCheckoutStatusAsync(
        StoreCheckoutStatusRequest request,
        CancellationToken cancellationToken);

    Task<StoreGiftClaimRevealResponse> RevealStoreGiftClaimCodeAsync(
        StoreGiftClaimRevealRequest request,
        CancellationToken cancellationToken);

    Task<SoftwareCatalogResponse> GetSoftwareCatalogAsync(
        SoftwareCatalogRequest request,
        CancellationToken cancellationToken);

    Task<SoftwareInstallResponse> InstallSoftwareAsync(
        SoftwareInstallRequest request,
        CancellationToken cancellationToken);

    Task<SoftwareUpdateResponse> UpdateSoftwareAsync(
        SoftwareUpdateRequest request,
        CancellationToken cancellationToken);

    Task<SoftwareRepairResponse> RepairSoftwareAsync(
        SoftwareRepairRequest request,
        CancellationToken cancellationToken);

    Task<SoftwareOpenResponse> OpenSoftwareAsync(
        SoftwareOpenRequest request,
        CancellationToken cancellationToken);

    Task<SoftwareRemoveResponse> RemoveSoftwareAsync(
        SoftwareRemoveRequest request,
        CancellationToken cancellationToken);
}

public sealed record LauncherCatalogSnapshot(
    string Status,
    IReadOnlyList<LauncherProduct> Products,
    string? Message);

public interface ILauncherCatalogSource
{
    Task<LauncherCatalogSnapshot> GetProductsAsync(CancellationToken cancellationToken);
}


public interface ILauncherIdentityClient
{
    Task<NativeBkeLoginResponse> LoginAsync(
        Uri platformBaseAddress,
        NativeBkeLoginRequest request,
        CancellationToken cancellationToken);

    Task<NativeBkeMfaVerifyResponse> VerifyMfaAsync(
        Uri platformBaseAddress,
        NativeBkeMfaVerifyRequest request,
        CancellationToken cancellationToken);

    Task<NativeBkePasswordResetResponse> RequestPasswordResetAsync(
        Uri platformBaseAddress,
        NativeBkePasswordResetRequest request,
        CancellationToken cancellationToken);
}

public interface ILauncherRegistrationClient
{
    Task<NativeBkeRegistrationPreflightResponse> GetRegistrationPreflightAsync(
        Uri platformBaseAddress,
        CancellationToken cancellationToken);

    Task<NativeBkeRegistrationResponse> RegisterAsync(
        Uri platformBaseAddress,
        NativeBkeRegistrationRequest request,
        CancellationToken cancellationToken);

    Task<NativeBkeEmailVerificationResponse> VerifyEmailAsync(
        Uri platformBaseAddress,
        NativeBkeEmailVerificationRequest request,
        CancellationToken cancellationToken);

    Task<NativeBkeVerificationResendResponse> ResendVerificationAsync(
        Uri platformBaseAddress,
        NativeBkeVerificationResendRequest request,
        CancellationToken cancellationToken);
}

public sealed record LauncherNativeSignInResult(
    string Status,
    IReadOnlyList<NativeBkeAccountChoice> Accounts,
    AccountSessionAccount? Account,
    string? ErrorCode,
    string? ErrorMessage,
    string? ChallengeToken = null,
    string? ExpiresAt = null,
    bool EmailSent = false,
    string? MfaReference = null);


public interface ILauncherExternalNavigator
{
    void OpenCheckout(string absoluteUrl);
    Task OpenLegalDocumentAsync(
        string slug,
        CancellationToken cancellationToken);
}

public sealed record LauncherCheckoutRecoveryState(
    string CorrelationId,
    string? PurchasePlanId,
    string? PurchaseMode,
    IReadOnlyList<string> LegalVersionIds)
{
    public bool HasResumeIntent =>
        !string.IsNullOrWhiteSpace(PurchasePlanId) &&
        PurchaseMode is "SELF" or "GIFT" &&
        LegalVersionIds is { Count: >= 2 and <= 3 };
}

public interface ILauncherCheckoutRecoveryStore
{
    LauncherCheckoutRecoveryState? Read();
    void Write(LauncherCheckoutRecoveryState state);
    void Clear();
}
