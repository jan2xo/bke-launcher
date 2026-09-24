using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed record LauncherStoreCheckoutReviewSnapshot(
    string Status,
    IReadOnlyList<string> PurchaseModes,
    StoreCheckoutReviewProduct? Product,
    StoreCheckoutReviewEdition? Edition,
    StoreCatalogPlan? Plan,
    IReadOnlyList<StoreCheckoutReviewLegalDocument> LegalDocuments,
    IReadOnlyList<StoreCheckoutReviewPendingLegalDocument> PendingLegal,
    string? Message);

public sealed class LauncherStoreCheckoutReviewService
{
    private readonly ILauncherAgentClient _agent;

    public LauncherStoreCheckoutReviewService(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<LauncherStoreCheckoutReviewSnapshot> ReviewAsync(
        string purchasePlanId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(purchasePlanId))
        {
            throw new ArgumentException(
                "Purchase plan identifier is required.",
                nameof(purchasePlanId));
        }

        var response = await _agent.ReviewStoreCheckoutAsync(
            new StoreCheckoutReviewRequest(
                Guid.NewGuid().ToString("N"),
                purchasePlanId),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.StoreCheckoutReviewCapabilityId ||
            response.ContractVersion != AgentLocalContract.StoreCheckoutReviewContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent Store checkout-review contract drifted.");
        }

        return new LauncherStoreCheckoutReviewSnapshot(
            response.Status,
            response.PurchaseModes,
            response.Product,
            response.Edition,
            response.Plan,
            response.LegalDocuments,
            response.PendingLegal,
            response.Error?.Message);
    }
}
