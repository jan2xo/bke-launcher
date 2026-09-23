using System.Text.Json.Serialization;

namespace BKE.Launcher.Contracts;

public static class BkePlatformContract
{
    public const string DefaultBaseAddress = "https://jl-bke.com";
    public const string AccountSessionProtocolVersion = "bke.account-session.v1";
    public const string NativeLoginPath = "/api/agent-sessions/native/login";
}

public sealed record NativeBkeLoginRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("customer_account_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? CustomerAccountId,
    [property: JsonPropertyName("device_id")] string DeviceId,
    [property: JsonPropertyName("device_name")] string DeviceName,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("architecture")] string Architecture);

public sealed record NativeBkeAccountChoice(
    [property: JsonPropertyName("account_id")] string AccountId,
    [property: JsonPropertyName("account_type")] string AccountType,
    [property: JsonPropertyName("account_display_name")] string AccountDisplayName);

public sealed record NativeBkeLoginAccount(
    [property: JsonPropertyName("account_id")] string AccountId,
    [property: JsonPropertyName("account_type")] string AccountType,
    [property: JsonPropertyName("account_display_name")] string AccountDisplayName);

public sealed record NativeBkeLoginResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("accounts"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] IReadOnlyList<NativeBkeAccountChoice>? Accounts,
    [property: JsonPropertyName("handoff_code"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? HandoffCode,
    [property: JsonPropertyName("expires_in"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] int? ExpiresIn,
    [property: JsonPropertyName("account"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] NativeBkeLoginAccount? Account,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error);
