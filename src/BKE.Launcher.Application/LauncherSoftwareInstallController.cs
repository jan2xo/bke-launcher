using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherSoftwareInstallController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherSoftwareInstallController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<SoftwareInstallResponse> InstallAsync(
        string productId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productId) ||
            productId.Length > 128)
        {
            throw new ArgumentException(
                "A BKE product id is required.",
                nameof(productId));
        }

        var response = await _agent.InstallSoftwareAsync(
            new SoftwareInstallRequest(
                Guid.NewGuid().ToString("N"),
                productId),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.SoftwareInstallCapabilityId ||
            response.ContractVersion != AgentLocalContract.SoftwareInstallContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent software-install contract drifted.");
        }

        return response;
    }
}
