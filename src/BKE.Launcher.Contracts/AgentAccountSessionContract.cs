using System.Text.Json.Serialization;

namespace BKE.Launcher.Contracts;

public static class AgentLocalContract
{
    public const string CapabilityId = "bke.account-session";
    public const int ContractVersion = 1;
    public const string SoftwareCatalogCapabilityId = "bke.software-catalog";
    public const int SoftwareCatalogContractVersion = 1;
    public const string SoftwareInstallCapabilityId = "bke.software-install";
    public const int SoftwareInstallContractVersion = 1;
    public const string SoftwareUpdateCapabilityId = "bke.software-update";
    public const int SoftwareUpdateContractVersion = 1;
    public const string SoftwareRepairCapabilityId = "bke.software-repair";
    public const int SoftwareRepairContractVersion = 1;
    public const string SoftwareOpenCapabilityId = "bke.software-open";
    public const int SoftwareOpenContractVersion = 1;
    public const string SoftwareRemoveCapabilityId = "bke.software-remove";
    public const int SoftwareRemoveContractVersion = 1;
    public const string ClaimCodeRedemptionCapabilityId = "bke.claim-code-redemption";
    public const int ClaimCodeRedemptionContractVersion = 1;
    public const string StoreCatalogCapabilityId = "bke.store-catalog";
    public const int StoreCatalogContractVersion = 1;
    public const string StoreCheckoutReviewCapabilityId = "bke.store-checkout-review";
    public const int StoreCheckoutReviewContractVersion = 1;
    public const string StoreCheckoutStartCapabilityId = "bke.store-checkout-start";
    public const int StoreCheckoutStartContractVersion = 1;
    public const string StoreCheckoutStatusCapabilityId = "bke.store-checkout-status";
    public const int StoreCheckoutStatusContractVersion = 1;
    public const string DefaultBaseAddress = "http://127.0.0.1:43873";
    public const string AccountSessionDeviceContextPath = "/v1/account-session/device-context";
    public const string AccountSessionCompletePath = "/v1/account-session/complete";
    public const string AccountSessionStartPath = "/v1/account-session/start";
    public const string AccountSessionStatusPath = "/v1/account-session/status";
    public const string AccountSessionLogoutPath = "/v1/account-session/logout";
    public const string SoftwareCatalogPath = "/v1/software/catalog";
    public const string SoftwareInstallPath = "/v1/software/install";
    public const string SoftwareUpdatePath = "/v1/software/update";
    public const string SoftwareRepairPath = "/v1/software/repair";
    public const string SoftwareOpenPath = "/v1/software/open";
    public const string SoftwareRemovePath = "/v1/software/remove";
    public const string ClaimCodeRedeemPath = "/v1/claims/redeem";
    public const string StoreCatalogPath = "/v1/store/catalog";
    public const string StoreCheckoutReviewPath = "/v1/store/checkout-review";
    public const string StoreCheckoutStartPath = "/v1/store/checkout-start";
    public const string StoreCheckoutStatusPath = "/v1/store/checkout-status";
}

public sealed record AccountSessionDeviceContextRequest(
    [property: JsonPropertyName("correlation_id")] string CorrelationId);

public sealed record AccountSessionDeviceContextResponse(
    [property: JsonPropertyName("capability_id")] string CapabilityId,
    [property: JsonPropertyName("contract_version")] int ContractVersion,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("device_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? DeviceId,
    [property: JsonPropertyName("device_name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? DeviceName,
    [property: JsonPropertyName("platform"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Platform,
    [property: JsonPropertyName("architecture"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Architecture,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] AccountSessionError? Error);

public sealed record AccountSessionCompleteRequest(
    [property: JsonPropertyName("correlation_id")] string CorrelationId,
    [property: JsonPropertyName("handoff_code")] string HandoffCode);

public sealed record AccountSessionCompleteResponse(
    [property: JsonPropertyName("capability_id")] string CapabilityId,
    [property: JsonPropertyName("contract_version")] int ContractVersion,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("account"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] AccountSessionAccount? Account,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] AccountSessionError? Error);

public sealed record AccountSessionStartRequest(
    [property: JsonPropertyName("correlation_id")] string CorrelationId);

public sealed record AccountSessionStartResponse(
    [property: JsonPropertyName("capability_id")] string CapabilityId,
    [property: JsonPropertyName("contract_version")] int ContractVersion,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("verification_uri"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? VerificationUri,
    [property: JsonPropertyName("user_code"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? UserCode,
    [property: JsonPropertyName("expires_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ExpiresAt,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] AccountSessionError? Error);

public sealed record AccountSessionStatusRequest(
    [property: JsonPropertyName("correlation_id")] string CorrelationId);

public sealed record AccountSessionStatusResponse(
    [property: JsonPropertyName("capability_id")] string CapabilityId,
    [property: JsonPropertyName("contract_version")] int ContractVersion,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("account"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] AccountSessionAccount? Account,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] AccountSessionError? Error);

public sealed record AccountSessionLogoutRequest(
    [property: JsonPropertyName("correlation_id")] string CorrelationId);

public sealed record AccountSessionLogoutResponse(
    [property: JsonPropertyName("capability_id")] string CapabilityId,
    [property: JsonPropertyName("contract_version")] int ContractVersion,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] AccountSessionError? Error);

public sealed record AccountSessionAccount(
    [property: JsonPropertyName("user_id")] string UserId,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("account_id")] string AccountId,
    [property: JsonPropertyName("account_type")] string AccountType,
    [property: JsonPropertyName("display_name")] string DisplayName);

public sealed record AccountSessionError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("retryable")] bool Retryable);
