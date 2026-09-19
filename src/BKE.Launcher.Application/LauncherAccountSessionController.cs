using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountSessionController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherAccountSessionController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public Task<AccountSessionStartResponse> StartAsync(CancellationToken cancellationToken) =>
        _agent.StartAccountSessionAsync(
            new AccountSessionStartRequest(NewCorrelationId()),
            cancellationToken);

    public Task<AccountSessionStatusResponse> StatusAsync(CancellationToken cancellationToken) =>
        _agent.GetAccountSessionStatusAsync(
            new AccountSessionStatusRequest(NewCorrelationId()),
            cancellationToken);

    public Task<AccountSessionLogoutResponse> LogoutAsync(CancellationToken cancellationToken) =>
        _agent.LogoutAccountSessionAsync(
            new AccountSessionLogoutRequest(NewCorrelationId()),
            cancellationToken);

    private static string NewCorrelationId() => Guid.NewGuid().ToString("N");
}
