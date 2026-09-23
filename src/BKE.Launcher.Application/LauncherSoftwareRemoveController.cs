using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherSoftwareRemoveController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherSoftwareRemoveController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<SoftwareRemoveResponse> RemoveAsync(
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

        var response = await _agent.RemoveSoftwareAsync(
            new SoftwareRemoveRequest(
                Guid.NewGuid().ToString("N"),
                productId),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.SoftwareRemoveCapabilityId ||
            response.ContractVersion != AgentLocalContract.SoftwareRemoveContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent software-remove contract drifted.");
        }

        return response;
    }
}
