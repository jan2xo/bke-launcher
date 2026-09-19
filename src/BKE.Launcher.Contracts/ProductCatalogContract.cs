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

public enum LauncherProductState
{
    Available,
    Entitled,
    NotEntitled,
    Installable,
    Installing,
    Installed,
    UpdateAvailable,
    RepairRequired,
    Unavailable,
}

public sealed record LauncherProduct(
    string ProductId,
    string DisplayName,
    ProductExecutionType ExecutionType,
    LauncherProductState State,
    string? InstalledVersion = null,
    string? AvailableVersion = null);
