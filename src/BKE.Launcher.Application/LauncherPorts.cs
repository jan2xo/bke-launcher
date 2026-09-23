using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public interface ILauncherAgentClient
{
    Task<AccountSessionDeviceContextResponse> GetAccountSessionDeviceContextAsync(
        AccountSessionDeviceContextRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionCompleteResponse> CompleteAccountSessionAsync(
        AccountSessionCompleteRequest request,
        CancellationToken cancellationToken);

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

    Task<SoftwareInstallResponse> InstallSoftwareAsync(
        SoftwareInstallRequest request,
        CancellationToken cancellationToken);

    Task<SoftwareOpenResponse> OpenSoftwareAsync(
        SoftwareOpenRequest request,
        CancellationToken cancellationToken);

    Task<SoftwareRemoveResponse> RemoveSoftwareAsync(
        SoftwareRemoveRequest request,
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


public interface ILauncherIdentityClient
{
    Task<NativeBkeLoginResponse> LoginAsync(
        NativeBkeLoginRequest request,
        CancellationToken cancellationToken);
}

public sealed record LauncherNativeSignInResult(
    string Status,
    IReadOnlyList<NativeBkeAccountChoice> Accounts,
    AccountSessionAccount? Account,
    string? ErrorCode,
    string? ErrorMessage);
