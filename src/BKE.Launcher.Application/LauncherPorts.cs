using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public interface ILauncherAgentClient
{
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

    Task<AccountNotificationFeedResponse> GetAccountNotificationsAsync(
        AccountNotificationFeedRequest request,
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
        NativeBkeLoginRequest request,
        CancellationToken cancellationToken);
}

public sealed record LauncherNativeSignInResult(
    string Status,
    IReadOnlyList<NativeBkeAccountChoice> Accounts,
    AccountSessionAccount? Account,
    string? ErrorCode,
    string? ErrorMessage);


public interface ILauncherExternalNavigator
{
    void OpenCheckout(string absoluteUrl);
    void OpenLegalDocument(string slug);
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
