using System.Text.Json.Serialization;

namespace BKE.Launcher.Contracts;

public static class AgentLocalContract
{
    public const string CapabilityId = "bke.account-session";
    public const int ContractVersion = 1;
    public const string DefaultBaseAddress = "http://127.0.0.1:43873";
    public const string AccountSessionStartPath = "/v1/account-session/start";
    public const string AccountSessionStatusPath = "/v1/account-session/status";
    public const string AccountSessionLogoutPath = "/v1/account-session/logout";
}

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
