namespace BKE.Launcher.PluginHost;

public static class LauncherPluginContract
{
    public const int Version = 1;
}

public sealed record LauncherPluginIdentity(
    string ProductId,
    string PluginVersion,
    int MinimumHostContractVersion);

public sealed record ProductAuthorizationResult(
    bool Authorized,
    string Reason);

public interface IProductAuthorizationGateway
{
    Task<ProductAuthorizationResult> AuthorizeAsync(
        string productId,
        CancellationToken cancellationToken);
}

public interface ILauncherContext
{
    IProductAuthorizationGateway Authorization { get; }
}

public interface IBkeLauncherPlugin
{
    LauncherPluginIdentity Identity { get; }

    Task InitializeAsync(ILauncherContext context, CancellationToken cancellationToken);

    Task OpenAsync(CancellationToken cancellationToken);

    Task ShutdownAsync(CancellationToken cancellationToken);
}
