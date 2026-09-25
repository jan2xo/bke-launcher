using System.Text.Json.Serialization;

namespace BKE.Launcher.Contracts;

public sealed record AccountNotificationFeedRequest(
    [property: JsonPropertyName("limit")] int Limit);

public sealed record AccountNotificationItem(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("source")] string Source,
    [property: JsonPropertyName("event")] string Event,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("body")] string Body,
    [property: JsonPropertyName("category")] string Category,
    [property: JsonPropertyName("severity")] string Severity,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("audience_kind")] string AudienceKind,
    [property: JsonPropertyName("product_id"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ProductId,
    [property: JsonPropertyName("created_at")] string CreatedAt,
    [property: JsonPropertyName("expires_at"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ExpiresAt);

public sealed record AccountNotificationFeedResponse(
    [property: JsonPropertyName("capability_id")] string CapabilityId,
    [property: JsonPropertyName("contract_version")] int ContractVersion,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("items")] IReadOnlyList<AccountNotificationItem> Items,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] AccountNotificationError? Error);

public sealed record AccountNotificationError(
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("retryable")] bool Retryable);
