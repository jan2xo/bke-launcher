using BKE.Launcher.Application;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.AgentClient;

public sealed class AgentSoftwareCatalogSource : ILauncherCatalogSource
{
    private readonly ILauncherAgentClient _agent;

    public AgentSoftwareCatalogSource(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<LauncherCatalogSnapshot> GetProductsAsync(
        CancellationToken cancellationToken)
    {
        var response = await _agent.GetSoftwareCatalogAsync(
            new SoftwareCatalogRequest(Guid.NewGuid().ToString("N")),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.SoftwareCatalogCapabilityId ||
            response.ContractVersion != AgentLocalContract.SoftwareCatalogContractVersion)
        {
            throw new InvalidDataException("BKE Licensing Agent software catalog contract drifted.");
        }

        if (response.Status != "READY")
        {
            if (response.Items.Count != 0)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent returned catalog items while the catalog was not READY.");
            }

            return new LauncherCatalogSnapshot(
                response.Status,
                [],
                response.Error?.Message);
        }

        if (response.Error is not null)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent returned a READY catalog with an error.");
        }

        var products = response.Items
            .Select(Project)
            .ToArray();

        return new LauncherCatalogSnapshot(
            "READY",
            products,
            null);
    }

    private static LauncherProduct Project(SoftwareCatalogItem item)
    {
        if (string.IsNullOrWhiteSpace(item.ProductId) ||
            string.IsNullOrWhiteSpace(item.DisplayName) ||
            string.IsNullOrWhiteSpace(item.Summary))
        {
            throw new InvalidDataException("BKE Licensing Agent returned an invalid software product.");
        }

        var executionType = item.ExecutionType switch
        {
            null => (ProductExecutionType?)null,
            ProductExecutionTypeWire.LauncherPlugin => ProductExecutionType.LauncherPlugin,
            ProductExecutionTypeWire.Standalone => ProductExecutionType.Standalone,
            _ => throw new InvalidDataException("BKE Licensing Agent returned an unknown execution type."),
        };

        var state = item.State switch
        {
            "NOT_ENTITLED" => LauncherProductState.NotEntitled,
            "INSTALLABLE" => LauncherProductState.Installable,
            "INSTALLED" => LauncherProductState.Installed,
            "UPDATE_AVAILABLE" => LauncherProductState.UpdateAvailable,
            "INSTALLED_NOT_ENTITLED" => LauncherProductState.InstalledNotEntitled,
            "POLICY_UNASSIGNED" => LauncherProductState.PolicyUnassigned,
            "RELEASE_UNAVAILABLE" => LauncherProductState.ReleaseUnavailable,
            "UNAVAILABLE" => LauncherProductState.Unavailable,
            _ => throw new InvalidDataException("BKE Licensing Agent returned an unknown software state."),
        };

        if (item.Installable &&
            (!item.Entitled || executionType is null || item.LatestVersion is null))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent returned an inconsistent installability decision.");
        }

        if (state is LauncherProductState.Installed
            or LauncherProductState.UpdateAvailable
            or LauncherProductState.InstalledNotEntitled &&
            string.IsNullOrWhiteSpace(item.InstalledVersion))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent returned installed state without an installed version.");
        }

        return new LauncherProduct(
            item.ProductId,
            item.DisplayName,
            item.Summary,
            executionType,
            state,
            item.InstalledVersion,
            item.LatestVersion);
    }
}
