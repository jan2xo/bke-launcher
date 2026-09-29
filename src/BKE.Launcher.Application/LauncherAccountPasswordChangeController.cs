using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountPasswordChangeController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherAccountPasswordChangeController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<AccountPasswordChangeResponse> ChangeAsync(
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentPassword) ||
            currentPassword.Length > 128)
        {
            throw new ArgumentException(
                "A valid current password is required.",
                nameof(currentPassword));
        }

        if (string.IsNullOrEmpty(newPassword) ||
            newPassword.Length > 128)
        {
            throw new ArgumentException(
                "A valid new password is required.",
                nameof(newPassword));
        }

        var response = await _agent.ChangeAccountPasswordAsync(
            new AccountPasswordChangeRequest(
                Guid.NewGuid().ToString("N"),
                currentPassword,
                newPassword),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.AccountPasswordChangeCapabilityId ||
            response.ContractVersion != AgentLocalContract.AccountPasswordChangeContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account password-change contract drifted.");
        }

        return response;
    }
}
