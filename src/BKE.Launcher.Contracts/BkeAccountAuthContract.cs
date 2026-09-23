using System.Text.Json.Serialization;

namespace BKE.Launcher.Contracts;

public static class BkeAccountAuthContract
{
    public const string ProtocolVersion = "bke.account-session.v1";
    public const string NativeLoginPath = "/api/agent-sessions/native/login";
}

public sealed record NativeAccountLoginRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("customer_account_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? CustomerAccountId,
    [property: JsonPropertyName("device_id")] string DeviceId,
    [property: JsonPropertyName("device_name"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? DeviceName,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("architecture")] string Architecture);

public sealed record NativeAccountSummary(
    [property: JsonPropertyName("account_id")] string AccountId,
    [property: JsonPropertyName("account_type")] string AccountType,
    [property: JsonPropertyName("account_display_name")] string DisplayName);

public sealed record NativeAccountLoginResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("handoff_code"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? HandoffCode,
    [property: JsonPropertyName("expires_in"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? ExpiresIn,
    [property: JsonPropertyName("account"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] NativeAccountSummary? Account,
    [property: JsonPropertyName("accounts"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IReadOnlyList<NativeAccountSummary>? Accounts,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error);
