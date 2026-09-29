using System.Text.Json.Serialization;

namespace BKE.Launcher.Contracts;

public sealed record NativeBkePasswordResetRequest(
    [property: JsonPropertyName("email")] string Email);

public sealed record NativeBkePasswordResetResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("error"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? Error);
