using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.AgentClient;

public sealed class AgentLoopbackClient : ILauncherAgentClient, IDisposable
{
    internal static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(5);
    internal static readonly TimeSpan InstallRequestTimeout = TimeSpan.FromMinutes(10);
    internal static readonly TimeSpan UpdateRequestTimeout = TimeSpan.FromMinutes(10);
    internal static readonly TimeSpan RepairRequestTimeout = TimeSpan.FromMinutes(10);
    internal static readonly TimeSpan RemoveRequestTimeout = TimeSpan.FromMinutes(10);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = false,
    };

    private readonly HttpClient _http;
    private readonly bool _ownsHttpClient;

    public AgentLoopbackClient(HttpClient? httpClient = null, Uri? baseAddress = null)
    {
        var resolvedBaseAddress = baseAddress ?? new Uri(AgentLocalContract.DefaultBaseAddress, UriKind.Absolute);
        ValidateLoopbackBaseAddress(resolvedBaseAddress);

        if (httpClient is null)
        {
            _http = new HttpClient(new HttpClientHandler
            {
                AllowAutoRedirect = false,
            })
            {
                BaseAddress = resolvedBaseAddress,
                Timeout = Timeout.InfiniteTimeSpan,
            };
            _ownsHttpClient = true;
        }
        else
        {
            _http = httpClient;
            _http.BaseAddress = resolvedBaseAddress;
            _http.Timeout = Timeout.InfiniteTimeSpan;
            _ownsHttpClient = false;
        }
    }

    public Task<AccountSessionDeviceContextResponse> GetAccountSessionDeviceContextAsync(
        AccountSessionDeviceContextRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionDeviceContextRequest, AccountSessionDeviceContextResponse>(
            AgentLocalContract.AccountSessionDeviceContextPath,
            request,
            cancellationToken);

    public Task<AccountSessionCompleteResponse> CompleteAccountSessionAsync(
        AccountSessionCompleteRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionCompleteRequest, AccountSessionCompleteResponse>(
            AgentLocalContract.AccountSessionCompletePath,
            request,
            cancellationToken);

    public Task<AccountSessionStartResponse> StartAccountSessionAsync(
        AccountSessionStartRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionStartRequest, AccountSessionStartResponse>(
            AgentLocalContract.AccountSessionStartPath,
            request,
            cancellationToken);

    public Task<AccountSessionStatusResponse> GetAccountSessionStatusAsync(
        AccountSessionStatusRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionStatusRequest, AccountSessionStatusResponse>(
            AgentLocalContract.AccountSessionStatusPath,
            request,
            cancellationToken);

    public Task<AccountSessionLogoutResponse> LogoutAccountSessionAsync(
        AccountSessionLogoutRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<AccountSessionLogoutRequest, AccountSessionLogoutResponse>(
            AgentLocalContract.AccountSessionLogoutPath,
            request,
            cancellationToken);

    public Task<ClaimCodeRedeemResponse> RedeemClaimCodeAsync(
        ClaimCodeRedeemRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<ClaimCodeRedeemRequest, ClaimCodeRedeemResponse>(
            AgentLocalContract.ClaimCodeRedeemPath,
            request,
            cancellationToken);

    public Task<SoftwareCatalogResponse> GetSoftwareCatalogAsync(
        SoftwareCatalogRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareCatalogRequest, SoftwareCatalogResponse>(
            AgentLocalContract.SoftwareCatalogPath,
            request,
            cancellationToken);

    public Task<SoftwareInstallResponse> InstallSoftwareAsync(
        SoftwareInstallRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareInstallRequest, SoftwareInstallResponse>(
            AgentLocalContract.SoftwareInstallPath,
            request,
            InstallRequestTimeout,
            cancellationToken);

    public Task<SoftwareUpdateResponse> UpdateSoftwareAsync(
        SoftwareUpdateRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareUpdateRequest, SoftwareUpdateResponse>(
            AgentLocalContract.SoftwareUpdatePath,
            request,
            UpdateRequestTimeout,
            cancellationToken);

    public Task<SoftwareRepairResponse> RepairSoftwareAsync(
        SoftwareRepairRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareRepairRequest, SoftwareRepairResponse>(
            AgentLocalContract.SoftwareRepairPath,
            request,
            RepairRequestTimeout,
            cancellationToken);

    public Task<SoftwareOpenResponse> OpenSoftwareAsync(
        SoftwareOpenRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareOpenRequest, SoftwareOpenResponse>(
            AgentLocalContract.SoftwareOpenPath,
            request,
            cancellationToken);

    public Task<SoftwareRemoveResponse> RemoveSoftwareAsync(
        SoftwareRemoveRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareRemoveRequest, SoftwareRemoveResponse>(
            AgentLocalContract.SoftwareRemovePath,
            request,
            RemoveRequestTimeout,
            cancellationToken);

    private Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<TRequest, TResponse>(
            path,
            request,
            DefaultRequestTimeout,
            cancellationToken);

    private async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        using var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken);
        timeoutSource.CancelAfter(timeout);

        using var response = await _http.PostAsJsonAsync(
            path,
            request,
            JsonOptions,
            timeoutSource.Token);

        if (IsRedirect(response.StatusCode))
        {
            throw new HttpRequestException("BKE Licensing Agent loopback endpoint redirected.");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(
            JsonOptions,
            timeoutSource.Token);

        return result ?? throw new InvalidDataException(
            "BKE Licensing Agent returned an empty local response.");
    }

    private static void ValidateLoopbackBaseAddress(Uri value)
    {
        if (!value.IsAbsoluteUri ||
            value.Scheme != Uri.UriSchemeHttp ||
            !value.IsLoopback ||
            !string.IsNullOrEmpty(value.Query) ||
            !string.IsNullOrEmpty(value.Fragment))
        {
            throw new ArgumentException(
                "Launcher Agent transport must use an absolute HTTP loopback address.",
                nameof(value));
        }
    }

    private static bool IsRedirect(HttpStatusCode statusCode) =>
        (int)statusCode is >= 300 and <= 399;

    public void Dispose()
    {
        if (_ownsHttpClient)
        {
            _http.Dispose();
        }
    }
}
