using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed record LauncherStoreSnapshot(
    string Status,
    bool GiftCheckoutEnabled,
    IReadOnlyList<StoreCatalogProduct> Products,
    string? Message);

public sealed class LauncherStoreService
{
    private readonly ILauncherAgentClient _agent;

    public LauncherStoreService(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<LauncherStoreSnapshot> GetProductsAsync(
        CancellationToken cancellationToken)
    {
        var response = await _agent.GetStoreCatalogAsync(
            new StoreCatalogRequest(Guid.NewGuid().ToString("N")),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.StoreCatalogCapabilityId ||
            response.ContractVersion != AgentLocalContract.StoreCatalogContractVersion)
        {
            throw new InvalidDataException("BKE Licensing Agent Store contract drifted.");
        }

        var products = response.Products
            .OrderBy(product => product.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(product => product.ProductId, StringComparer.Ordinal)
            .ToArray();

        return new LauncherStoreSnapshot(
            response.Status,
            response.GiftCheckoutEnabled,
            products,
            response.Error?.Message);
    }
}
