using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherCatalogService
{
    private readonly ILauncherCatalogSource _source;

    public LauncherCatalogService(ILauncherCatalogSource source)
    {
        _source = source;
    }

    public async Task<LauncherCatalogSnapshot> GetProductsAsync(
        CancellationToken cancellationToken)
    {
        var snapshot = await _source.GetProductsAsync(cancellationToken);
        var products = snapshot.Products
            .OrderBy(product => product.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(product => product.ProductId, StringComparer.Ordinal)
            .ToArray();

        return snapshot with { Products = products };
    }
}
