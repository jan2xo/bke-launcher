using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherSoftwareUpdateController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherSoftwareUpdateController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<SoftwareUpdateResponse> UpdateAsync(
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

        var response = await _agent.UpdateSoftwareAsync(
            new SoftwareUpdateRequest(
                Guid.NewGuid().ToString("N"),
                productId),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.SoftwareUpdateCapabilityId ||
            response.ContractVersion != AgentLocalContract.SoftwareUpdateContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent software-update contract drifted.");
        }

        return response;
    }
}
