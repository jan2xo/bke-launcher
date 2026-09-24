using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherSoftwareRepairController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherSoftwareRepairController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<SoftwareRepairResponse> RepairAsync(
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

        var response = await _agent.RepairSoftwareAsync(
            new SoftwareRepairRequest(
                Guid.NewGuid().ToString("N"),
                productId),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.SoftwareRepairCapabilityId ||
            response.ContractVersion != AgentLocalContract.SoftwareRepairContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent software-Repair contract drifted.");
        }

        return response;
    }
}
