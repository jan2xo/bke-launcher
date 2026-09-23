using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public interface ILauncherAgentClient
{
    Task<AccountSessionStartResponse> StartAccountSessionAsync(
        AccountSessionStartRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionNativeContextResponse> GetNativeAccountSessionContextAsync(
        AccountSessionNativeContextRequest request,
        CancellationToken cancellationToken);

    Task<AccountSessionNativeCompleteResponse> CompleteNativeAccountSessionAsync(
        AccountSessionNativeCompleteRequest request,
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

public interface ILauncherAccountAuthClient
{
    Task<NativeAccountLoginResponse> AuthenticateAsync(
        NativeAccountLoginRequest request,
        CancellationToken cancellationToken);
}

public sealed record LauncherNativeAccountChoice(
    string AccountId,
    string AccountType,
    string DisplayName)
{
    public string DisplayLabel => $"{DisplayName} · {AccountType}";
}

public sealed record LauncherNativeSignInResult(
    string Status,
    AccountSessionAccount? Account,
    IReadOnlyList<LauncherNativeAccountChoice> Accounts,
    string? ErrorCode,
    string? Message);

public sealed record LauncherCatalogSnapshot(
    string Status,
    IReadOnlyList<LauncherProduct> Products,
    string? Message);

public interface ILauncherCatalogSource
{
    Task<LauncherCatalogSnapshot> GetProductsAsync(CancellationToken cancellationToken);
}
