using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public interface ILauncherAgentClient
{
    Task<AccountSessionStartResponse> StartAccountSessionAsync(
        AccountSessionStartRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionStatusResponse> GetAccountSessionStatusAsync(
        AccountSessionStatusRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionLogoutResponse> LogoutAccountSessionAsync(
        AccountSessionLogoutRequest request,
        CancellationToken cancellationToken);

    Task<SoftwareCatalogResponse> GetSoftwareCatalogAsync(
        SoftwareCatalogRequest request,
        CancellationToken cancellationToken);
}

public sealed record LauncherCatalogSnapshot(
    string Status,
    IReadOnlyList<LauncherProduct> Products,
    string? Message);

public interface ILauncherCatalogSource
{
    Task<LauncherCatalogSnapshot> GetProductsAsync(CancellationToken cancellationToken);
}
