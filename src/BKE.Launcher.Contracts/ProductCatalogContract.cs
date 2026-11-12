namespace BKE.Launcher.Contracts;

public enum ProductExecutionType
{
    LauncherPlugin,
    Standalone,
}

public static class ProductExecutionTypeWire
{
    public const string LauncherPlugin = "LAUNCHER_PLUGIN";
    public const string Standalone = "STANDALONE";

    public static string ToWireValue(ProductExecutionType value) => value switch
    {
        ProductExecutionType.LauncherPlugin => LauncherPlugin,
        ProductExecutionType.Standalone => Standalone,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown BKE product execution type."),
    };

    public static ProductExecutionType Parse(string value) => value switch
    {
        LauncherPlugin => ProductExecutionType.LauncherPlugin,
        Standalone => ProductExecutionType.Standalone,
        _ => throw new ArgumentException("Unknown BKE product execution type.", nameof(value)),
    };
}

/// <summary>
/// DS SQL ProductActivationPolicy credential mode. This is presentation
/// metadata only; the Agent owns machine authorization.
/// </summary>
public enum ProductActivationMode
{
    Account,
    LicenseKey,
    Either,
}

public enum LauncherProductState
{
    NotEntitled,
    Installable,
    Installed,
    UpdateAvailable,
    InstalledNotEntitled,
    PolicyUnassigned,
    ReleaseUnavailable,
    Unavailable,
    Installing,
    RepairRequired,
}

public sealed record LauncherProduct(
    string ProductId,
    string DisplayName,
    string Summary,
    ProductExecutionType? ExecutionType,
    LauncherProductState State,
    string? InstalledVersion = null,
    string? AvailableVersion = null,
    ProductActivationMode? ActivationMode = null);
