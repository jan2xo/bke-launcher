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
}

public interface ILauncherCatalogSource
{
    Task<IReadOnlyList<LauncherProduct>> GetProductsAsync(CancellationToken cancellationToken);
}
