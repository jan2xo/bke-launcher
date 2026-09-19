using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherCatalogService
{
    private readonly ILauncherCatalogSource _source;

    public LauncherCatalogService(ILauncherCatalogSource source)
    {
        _source = source;
    }

    public async Task<IReadOnlyList<LauncherProduct>> GetProductsAsync(
        CancellationToken cancellationToken)
    {
        var products = await _source.GetProductsAsync(cancellationToken);
        return products
            .OrderBy(product => product.DisplayName, StringComparer.OrdinalIgnoreCase)
            .ThenBy(product => product.ProductId, StringComparer.Ordinal)
            .ToArray();
    }
}
