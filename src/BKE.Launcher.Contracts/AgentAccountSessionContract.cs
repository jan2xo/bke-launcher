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
    public const string SoftwareOpenCapabilityId = "bke.software-open";
    public const int SoftwareOpenContractVersion = 1;
    public const string SoftwareRemoveCapabilityId = "bke.software-remove";
    public const int SoftwareRemoveContractVersion = 1;
    public const string DefaultBaseAddress = "http://127.0.0.1:43873";
    public const string AccountSessionDeviceContextPath = "/v1/account-session/device-context";
    public const string AccountSessionCompletePath = "/v1/account-session/complete";
    public const string AccountSessionStartPath = "/v1/account-session/start";
    public const string AccountSessionStatusPath = "/v1/account-session/status";
    public const string AccountSessionLogoutPath = "/v1/account-session/logout";
    public const string SoftwareCatalogPath = "/v1/software/catalog";
    public const string SoftwareInstallPath = "/v1/software/install";
    public const string SoftwareOpenPath = "/v1/software/open";
    public const string SoftwareRemovePath = "/v1/software/remove";
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
