using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.AgentClient;

public sealed class AgentLoopbackClient : ILauncherAgentClient, IDisposable
{
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
                Timeout = TimeSpan.FromSeconds(5),
            };
            _ownsHttpClient = true;
        }
        else
        {
            _http = httpClient;
            _http.BaseAddress = resolvedBaseAddress;
            _ownsHttpClient = false;
        }
    }

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

    public Task<SoftwareCatalogResponse> GetSoftwareCatalogAsync(
        SoftwareCatalogRequest request,
        CancellationToken cancellationToken) =>
        PostAsync<SoftwareCatalogRequest, SoftwareCatalogResponse>(
            AgentLocalContract.SoftwareCatalogPath,
            request,
            cancellationToken);

    private async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest request,
        CancellationToken cancellationToken)
    {
        using var response = await _http.PostAsJsonAsync(
            path,
            request,
            JsonOptions,
            cancellationToken);

        if (IsRedirect(response.StatusCode))
        {
            throw new HttpRequestException("BKE Licensing Agent loopback endpoint redirected.");
        }

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TResponse>(
            JsonOptions,
            cancellationToken);

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
