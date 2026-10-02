using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.AgentClient;

public sealed class AgentLoopbackClient : ILauncherAgentClient, IDisposable
{
    internal static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(5);
    internal static readonly TimeSpan AccountPasswordChangeRequestTimeout = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan AccountMfaRequestTimeout = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan AccountPrivacyRequestTimeout = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan AccountPurchasesRequestTimeout = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan AccountBillingRequestTimeout = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan AccountPendingOrderRequestTimeout = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan AccountLicenseSeatsRequestTimeout = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan AccountLicenseDevicesRequestTimeout = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan AccountOrganizationRequestTimeout = TimeSpan.FromSeconds(30);
    internal static readonly TimeSpan InstallRequestTimeout = TimeSpan.FromMinutes(10);
    internal static readonly TimeSpan UpdateRequestTimeout = TimeSpan.FromMinutes(10);
    internal static readonly TimeSpan RepairRequestTimeout = TimeSpan.FromMinutes(10);
    internal static readonly TimeSpan StoreCheckoutRequestTimeout = TimeSpan.FromSeconds(40);
    internal static readonly TimeSpan RemoveRequestTimeout = TimeSpan.FromMinutes(10);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
    };

    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;

    public AgentLoopbackClient(HttpClient? httpClient = null, Uri? baseAddress = null)
    {
        var resolvedBaseAddress = baseAddress ?? new Uri(AgentLocalContract.DefaultBaseAddress, UriKind.Absolute);
        ValidateLoopbackBaseAddress(resolvedBaseAddress);

        if (httpClient is null)
        {
            _http = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
            })
            {
                BaseAddress = resolvedBaseAddress,
                Timeout = Timeout.InfiniteTimeSpan,
            };
            _ownsHttpClient = true;
        }
        else
        {
            _http = httpClient;
            _http.BaseAddress = resolvedBaseAddress;
            _http.Timeout = Timeout.InfiniteTimeSpan;
            _ownsHttpClient = false;
        }
    }

    public Task<PlatformAuthorityResponse> GetPlatformAuthorityAsync(
        PlatformAuthorityRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<PlatformAuthorityRequest, PlatformAuthorityResponse>(
            AgentLocalContract.PlatformAuthorityPath,
            request,
            cancellationToken);

    public Task<AccountSessionDeviceContextResponse> GetAccountSessionDeviceContextAsync(
        AccountSessionDeviceContextRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionDeviceContextRequest, AccountSessionDeviceContextResponse>(
            AgentLocalContract.AccountSessionDeviceContextPath,
            request,
            cancellationToken);

    public Task<AccountSessionCompleteResponse> CompleteAccountSessionAsync(
        AccountSessionCompleteRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionCompleteRequest, AccountSessionCompleteResponse>(
            AgentLocalContract.AccountSessionCompletePath,
            request,
            cancellationToken);

    public Task<AccountSessionStartResponse> StartAccountSessionAsync(
        AccountSessionStartRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionStartRequest, AccountSessionStartResponse>(
            AgentLocalContract.AccountSessionStartPath,
            request,
            cancellationToken);

    public Task<AccountSessionStatusResponse> GetAccountSessionStatusAsync(
        AccountSessionStatusRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionStatusRequest, AccountSessionStatusResponse>(
            AgentLocalContract.AccountSessionStatusPath,
            request,
            cancellationToken);

    public Task<AccountSessionLogoutResponse> LogoutAccountSessionAsync(
        AccountSessionLogoutRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionLogoutRequest, AccountSessionLogoutResponse>(
            AgentLocalContract.AccountSessionLogoutPath,
            request,
            cancellationToken);

    public Task<AccountPasswordChangeResponse> ChangeAccountPasswordAsync(
        AccountPasswordChangeRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountPasswordChangeRequest, AccountPasswordChangeResponse>(
            AgentLocalContract.AccountPasswordChangePath,
            request,
            AccountPasswordChangeRequestTimeout,
            cancellationToken);

    public Task<AccountMfaStatusResponse> GetAccountMfaStatusAsync(
        AccountMfaStatusRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountMfaStatusRequest, AccountMfaStatusResponse>(
            AgentLocalContract.AccountMfaStatusPath,
            request,
            AccountMfaRequestTimeout,
            cancellationToken);

    public Task<AccountMfaChallengeResponse> StartAccountMfaEnrollmentAsync(
        AccountMfaEnrollStartRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountMfaEnrollStartRequest, AccountMfaChallengeResponse>(
            AgentLocalContract.AccountMfaEnrollStartPath,
            request,
            AccountMfaRequestTimeout,
            cancellationToken);

    public Task<AccountMfaMutationResponse> CompleteAccountMfaEnrollmentAsync(
        AccountMfaEnrollCompleteRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountMfaEnrollCompleteRequest, AccountMfaMutationResponse>(
            AgentLocalContract.AccountMfaEnrollCompletePath,
            request,
            AccountMfaRequestTimeout,
            cancellationToken);

    public Task<AccountMfaChallengeResponse> StartAccountMfaProofAsync(
        AccountMfaProofChallengeRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountMfaProofChallengeRequest, AccountMfaChallengeResponse>(
            AgentLocalContract.AccountMfaChallengePath,
            request,
            AccountMfaRequestTimeout,
            cancellationToken);

    public Task<AccountMfaMutationResponse> DisableAccountMfaAsync(
        AccountMfaMutationRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountMfaMutationRequest, AccountMfaMutationResponse>(
            AgentLocalContract.AccountMfaDisablePath,
            request,
            AccountMfaRequestTimeout,
            cancellationToken);

    public Task<AccountMfaMutationResponse> RegenerateAccountMfaRecoveryAsync(
        AccountMfaMutationRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountMfaMutationRequest, AccountMfaMutationResponse>(
            AgentLocalContract.AccountMfaRecoveryRegeneratePath,
            request,
            AccountMfaRequestTimeout,
            cancellationToken);

    public Task<AccountRecentAuthResponse> StartAccountRecentAuthAsync(
        AccountRecentAuthStartRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountRecentAuthStartRequest, AccountRecentAuthResponse>(
            AgentLocalContract.AccountRecentAuthStartPath,
            request,
            AccountMfaRequestTimeout,
            cancellationToken);

    public Task<AccountRecentAuthResponse> CompleteAccountRecentAuthAsync(
        AccountRecentAuthCompleteRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountRecentAuthCompleteRequest, AccountRecentAuthResponse>(
            AgentLocalContract.AccountRecentAuthCompletePath,
            request,
            AccountMfaRequestTimeout,
            cancellationToken);

    public Task<AccountPrivacyListResponse> GetAccountPrivacyRequestsAsync(
        AccountPrivacyListRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountPrivacyListRequest, AccountPrivacyListResponse>(
            AgentLocalContract.AccountPrivacyListPath,
            request,
            AccountPrivacyRequestTimeout,
            cancellationToken);

    public Task<AccountPrivacyCreateResponse> CreateAccountPrivacyRequestAsync(
        AccountPrivacyCreateRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountPrivacyCreateRequest, AccountPrivacyCreateResponse>(
            AgentLocalContract.AccountPrivacyCreatePath,
            request,
            AccountPrivacyRequestTimeout,
            cancellationToken);

    public Task<AccountPurchasesResponse> GetAccountPurchasesAsync(
        AccountPurchasesRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountPurchasesRequest, AccountPurchasesResponse>(
            AgentLocalContract.AccountPurchasesPath,
            request,
            AccountPurchasesRequestTimeout,
            cancellationToken);

    public Task<AccountBillingResponse> GetAccountBillingAsync(
        AccountBillingRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountBillingRequest, AccountBillingResponse>(
            AgentLocalContract.AccountBillingPath,
            request,
            AccountBillingRequestTimeout,
            cancellationToken);

    public Task<AccountPendingOrderContinueResponse> ContinueAccountPendingOrderAsync(
        AccountPendingOrderContinueRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountPendingOrderContinueRequest, AccountPendingOrderContinueResponse>(
            AgentLocalContract.AccountPendingOrderContinuePath,
            request,
            AccountPendingOrderRequestTimeout,
            cancellationToken);

    public Task<AccountPendingOrderCancelResponse> CancelAccountPendingOrderAsync(
        AccountPendingOrderCancelRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountPendingOrderCancelRequest, AccountPendingOrderCancelResponse>(
            AgentLocalContract.AccountPendingOrderCancelPath,
            request,
            AccountPendingOrderRequestTimeout,
            cancellationToken);

    public Task<AccountLicenseSeatsResponse> GetAccountLicenseSeatsAsync(
        AccountLicenseSeatsRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountLicenseSeatsRequest, AccountLicenseSeatsResponse>(
            AgentLocalContract.AccountLicenseSeatsPath,
            request,
            AccountLicenseSeatsRequestTimeout,
            cancellationToken);

    public Task<AccountLicenseSeatsManageResponse> ManageAccountLicenseSeatsAsync(
        AccountLicenseSeatsManageRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountLicenseSeatsManageRequest, AccountLicenseSeatsManageResponse>(
            AgentLocalContract.AccountLicenseSeatsManagePath,
            request,
            AccountLicenseSeatsRequestTimeout,
            cancellationToken);

    public Task<AccountLicenseDevicesResponse> GetAccountLicenseDevicesAsync(
        AccountLicenseDevicesRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountLicenseDevicesRequest, AccountLicenseDevicesResponse>(
            AgentLocalContract.AccountLicenseDevicesPath,
            request,
            AccountLicenseDevicesRequestTimeout,
            cancellationToken);

    public Task<AccountLicenseDeviceDeactivateResponse> DeactivateAccountLicenseDeviceAsync(
        AccountLicenseDeviceDeactivateRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountLicenseDeviceDeactivateRequest, AccountLicenseDeviceDeactivateResponse>(
            AgentLocalContract.AccountLicenseDevicesManagePath,
            request,
            AccountLicenseDevicesRequestTimeout,
            cancellationToken);

    public Task<AccountOrganizationOverviewResponse> GetAccountOrganizationAsync(
        AccountOrganizationOverviewRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountOrganizationOverviewRequest, AccountOrganizationOverviewResponse>(
            AgentLocalContract.AccountOrganizationOverviewPath,
            request,
            AccountOrganizationRequestTimeout,
            cancellationToken);

    public Task<AccountOrganizationCreateResponse> CreateAccountOrganizationAsync(
        AccountOrganizationCreateRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountOrganizationCreateRequest, AccountOrganizationCreateResponse>(
            AgentLocalContract.AccountOrganizationCreatePath,
            request,
            AccountOrganizationRequestTimeout,
            cancellationToken);

    public Task<AccountOrganizationProfileUpdateResponse> UpdateAccountOrganizationProfileAsync(
        AccountOrganizationProfileUpdateRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountOrganizationProfileUpdateRequest, AccountOrganizationProfileUpdateResponse>(
            AgentLocalContract.AccountOrganizationProfileUpdatePath,
            request,
            AccountOrganizationRequestTimeout,
            cancellationToken);

    public Task<AccountOrganizationInvitationCreateResponse> CreateAccountOrganizationInvitationAsync(
        AccountOrganizationInvitationCreateRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountOrganizationInvitationCreateRequest, AccountOrganizationInvitationCreateResponse>(
            AgentLocalContract.AccountOrganizationInvitationCreatePath,
            request,
            AccountOrganizationRequestTimeout,
            cancellationToken);

    public Task<AccountOrganizationInvitationAcceptResponse> AcceptAccountOrganizationInvitationAsync(
        AccountOrganizationInvitationAcceptRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountOrganizationInvitationAcceptRequest, AccountOrganizationInvitationAcceptResponse>(
            AgentLocalContract.AccountOrganizationInvitationAcceptPath,
            request,
            AccountOrganizationRequestTimeout,
            cancellationToken);

    public Task<AccountOrganizationInvitationManageResponse> ManageAccountOrganizationInvitationAsync(
        AccountOrganizationInvitationManageRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountOrganizationInvitationManageRequest, AccountOrganizationInvitationManageResponse>(
            AgentLocalContract.AccountOrganizationInvitationManagePath,
            request,
            AccountOrganizationRequestTimeout,
            cancellationToken);

    public Task<AccountOrganizationMemberManageResponse> ManageAccountOrganizationMemberAsync(
        AccountOrganizationMemberManageRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountOrganizationMemberManageRequest, AccountOrganizationMemberManageResponse>(
            AgentLocalContract.AccountOrganizationMemberManagePath,
            request,
            AccountOrganizationRequestTimeout,
            cancellationToken);

    public Task<AccountOrganizationOwnershipTransferResponse> TransferAccountOrganizationOwnershipAsync(
        AccountOrganizationOwnershipTransferRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountOrganizationOwnershipTransferRequest, AccountOrganizationOwnershipTransferResponse>(
            AgentLocalContract.AccountOrganizationOwnershipTransferPath,
            request,
            AccountOrganizationRequestTimeout,
            cancellationToken);

    public Task<AccountOrganizationLeaveResponse> LeaveAccountOrganizationAsync(
        AccountOrganizationLeaveRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountOrganizationLeaveRequest, AccountOrganizationLeaveResponse>(
            AgentLocalContract.AccountOrganizationLeavePath,
            request,
            AccountOrganizationRequestTimeout,
            cancellationToken);

    public Task<AccountNotificationFeedResponse> GetAccountNotificationsAsync(
        AccountNotificationFeedRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountNotificationFeedRequest, AccountNotificationFeedResponse>(
            AgentLocalContract.AccountNotificationFeedPath,
            request,
            cancellationToken);

    public Task<AccountNotificationReceiptResponse> MutateAccountNotificationAsync(
        AccountNotificationReceiptRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountNotificationReceiptRequest, AccountNotificationReceiptResponse>(
            AgentLocalContract.AccountNotificationReceiptPath,
            request,
            cancellationToken);

    public Task<ClaimCodeRedeemResponse> RedeemClaimCodeAsync(
        ClaimCodeRedeemRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<ClaimCodeRedeemRequest, ClaimCodeRedeemResponse>(
            AgentLocalContract.ClaimCodeRedeemPath,
            request,
            cancellationToken);

    public Task<StoreCatalogResponse> GetStoreCatalogAsync(
        StoreCatalogRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<StoreCatalogRequest, StoreCatalogResponse>(
            AgentLocalContract.StoreCatalogPath,
            request,
            cancellationToken);

    public Task<StoreCheckoutReviewResponse> ReviewStoreCheckoutAsync(
        StoreCheckoutReviewRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<StoreCheckoutReviewRequest, StoreCheckoutReviewResponse>(
            AgentLocalContract.StoreCheckoutReviewPath,
            request,
            cancellationToken);

    public Task<StoreCheckoutStartResponse> StartStoreCheckoutAsync(
        StoreCheckoutStartRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<StoreCheckoutStartRequest, StoreCheckoutStartResponse>(
            AgentLocalContract.StoreCheckoutStartPath,
            request,
            StoreCheckoutRequestTimeout,
            cancellationToken);

    public Task<StoreTrialStartResponse> StartStoreTrialAsync(
        StoreTrialStartRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<StoreTrialStartRequest, StoreTrialStartResponse>(
            AgentLocalContract.StoreTrialStartPath,
            request,
            StoreCheckoutRequestTimeout,
            cancellationToken);

    public Task<StoreCheckoutStatusResponse> CheckStoreCheckoutStatusAsync(
        StoreCheckoutStatusRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<StoreCheckoutStatusRequest, StoreCheckoutStatusResponse>(
            AgentLocalContract.StoreCheckoutStatusPath,
            request,
            StoreCheckoutRequestTimeout,
            cancellationToken);

    public Task<StoreGiftClaimRevealResponse> RevealStoreGiftClaimCodeAsync(
        StoreGiftClaimRevealRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<StoreGiftClaimRevealRequest, StoreGiftClaimRevealResponse>(
            AgentLocalContract.StoreGiftClaimRevealPath,
            request,
            StoreCheckoutRequestTimeout,
            cancellationToken);

    public Task<StoreGiftClaimsResponse> GetStoreGiftClaimsAsync(
        StoreGiftClaimsRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<StoreGiftClaimsRequest, StoreGiftClaimsResponse>(
            AgentLocalContract.StoreGiftClaimsPath,
            request,
            StoreCheckoutRequestTimeout,
            cancellationToken);

    public Task<StoreGiftClaimPersistentRevealResponse> RevealPersistentStoreGiftClaimAsync(
        StoreGiftClaimPersistentRevealRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<StoreGiftClaimPersistentRevealRequest, StoreGiftClaimPersistentRevealResponse>(
            AgentLocalContract.StoreGiftClaimPersistentRevealPath,
            request,
            StoreCheckoutRequestTimeout,
            cancellationToken);

    public Task<SoftwareCatalogResponse> GetSoftwareCatalogAsync(
        SoftwareCatalogRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareCatalogRequest, SoftwareCatalogResponse>(
            AgentLocalContract.SoftwareCatalogPath,
            request,
            cancellationToken);

    public Task<LauncherPluginAuthorizeResponse> AuthorizeLauncherPluginAsync(
        LauncherPluginAuthorizeRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<LauncherPluginAuthorizeRequest, LauncherPluginAuthorizeResponse>(
            AgentLocalContract.LauncherPluginAuthorizePath,
            request,
            cancellationToken);

    public Task<SoftwareInstallResponse> InstallSoftwareAsync(
        SoftwareInstallRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareInstallRequest, SoftwareInstallResponse>(
            AgentLocalContract.SoftwareInstallPath,
            request,
            InstallRequestTimeout,
            cancellationToken);

    public Task<SoftwareUpdateResponse> UpdateSoftwareAsync(
        SoftwareUpdateRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareUpdateRequest, SoftwareUpdateResponse>(
            AgentLocalContract.SoftwareUpdatePath,
            request,
            UpdateRequestTimeout,
            cancellationToken);

    public Task<SoftwareRepairResponse> RepairSoftwareAsync(
        SoftwareRepairRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareRepairRequest, SoftwareRepairResponse>(
            AgentLocalContract.SoftwareRepairPath,
            request,
            RepairRequestTimeout,
            cancellationToken);

    public Task<SoftwareOpenResponse> OpenSoftwareAsync(
        SoftwareOpenRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareOpenRequest, SoftwareOpenResponse>(
            AgentLocalContract.SoftwareOpenPath,
            request,
            cancellationToken);

    public Task<SoftwareRemoveResponse> RemoveSoftwareAsync(
        SoftwareRemoveRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareRemoveRequest, SoftwareRemoveResponse>(
            AgentLocalContract.SoftwareRemovePath,
            request,
            RemoveRequestTimeout,
            cancellationToken);

    private Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<TRequest, TResponse>(
            path,
            request,
            DefaultRequestTimeout,
            cancellationToken);

    private async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);
        timeoutSource.CancelAfter(timeout);

        using var response = await _http.PostAsJsonAsync(
            path,
            request,
            JsonOptions,
            timeoutSource.Token);

        if (IsRedirect(response.StatusCode))
        {
            throw new HttpRequestException("BKE Licensing Agent loopback endpoint redirected.");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(
            JsonOptions,
            timeoutSource.Token);

        return result ?? throw new InvalidDataException(
            "BKE Licensing Agent returned an empty local response.");
    }

    private static void ValidateLoopbackBaseAddress(Uri value)
    {
        if (!value.IsAbsoluteUri ||
            value.Scheme != Uri.UriSchemeHttp ||
            !value.IsLoopback ||
            !string.IsNullOrEmpty(value.Query) ||
            !string.IsNullOrEmpty(value.Fragment))
        {
            throw new ArgumentException(
                "Launcher Agent transport must use an absolute HTTP loopback address.",
                nameof(value));
        }
    }

    private static bool IsRedirect(HttpStatusCode statusCode) =>
        (int)statusCode is >= 300 and <= 399;

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _http.Dispose();
        }
    }
}
