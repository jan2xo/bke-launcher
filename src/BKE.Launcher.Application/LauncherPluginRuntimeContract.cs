namespace BKE.Launcher.Application;

public sealed record LauncherPluginAuthorizationDecision(
    string Status,
    bool Authorized,
    string Reason,
    string? Message);

public interface ILauncherPluginAuthorizationPort
{
    Task<LauncherPluginAuthorizationDecision> AuthorizeAsync(
        string productId,
        string version,
        CancellationToken cancellationToken);
}

public sealed record LauncherPluginOpenResult(
    string Status,
    string Reason,
    string? Message = null);

public interface ILauncherPluginRuntime
{
    bool IsRegistered(
        string productId,
        string version);

    Task<LauncherPluginOpenResult> OpenAsync(
        string productId,
        string version,
        CancellationToken cancellationToken);

    Task ShutdownAsync(
        CancellationToken cancellationToken);
}
