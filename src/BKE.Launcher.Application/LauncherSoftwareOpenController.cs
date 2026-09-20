using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherSoftwareOpenController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherSoftwareOpenController(
        ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<SoftwareOpenResponse> OpenAsync(
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

        var response = await _agent.OpenSoftwareAsync(
            new SoftwareOpenRequest(
                Guid.NewGuid().ToString("N"),
                productId),
            cancellationToken);

        if (response.CapabilityId !=
                AgentLocalContract.SoftwareOpenCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.SoftwareOpenContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent software-open contract drifted.");
        }

        return response;
    }
}
