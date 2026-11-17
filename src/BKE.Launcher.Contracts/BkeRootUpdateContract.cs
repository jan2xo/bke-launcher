using System.Text.Json.Serialization;

namespace BKE.Launcher.Contracts;

public enum BkeUpdateStatus
{
    Idle,
    Checking,
    UpdateAvailable,
    UpToDate,
    Staging,
    ReadyToRestart,
    Failed
}

public sealed record BkeUpdateRelease(
    [property: JsonPropertyName("current_version")] string CurrentVersion,
    [property: JsonPropertyName("target_version")] string TargetVersion,
    [property: JsonPropertyName("channel")] string Channel,
    [property: JsonPropertyName("download_url")] string DownloadUrl,
    [property: JsonPropertyName("sha256")] string Sha256,
    [property: JsonPropertyName("size_bytes")] long SizeBytes,
    [property: JsonPropertyName("release_summary")] string ReleaseSummary,
    [property: JsonPropertyName("published_at")] DateTimeOffset PublishedAt);

public sealed record BkeUpdateCheckResult(
    [property: JsonPropertyName("status")] BkeUpdateStatus Status,
    [property: JsonPropertyName("release"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] BkeUpdateRelease? Release,
    [property: JsonPropertyName("error_message"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] string? ErrorMessage)
{
    public static BkeUpdateCheckResult UpToDate() =>
        new(BkeUpdateStatus.UpToDate, null, null);

    public static BkeUpdateCheckResult Available(BkeUpdateRelease release) =>
        new(BkeUpdateStatus.UpdateAvailable, release, null);

    public static BkeUpdateCheckResult Failed(string message) =>
        new(BkeUpdateStatus.Failed, null, message);
}
