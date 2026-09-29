using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountPasswordChangeController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherAccountPasswordChangeController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public Task<AccountPasswordChangeResponse> ChangeAsync(
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken) =>
        _agent.ChangeAccountPasswordAsync(
            new AccountPasswordChangeRequest(
                Guid.NewGuid().ToString("N"),
                currentPassword,
                newPassword),
            cancellationToken);
}
