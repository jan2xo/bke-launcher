using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed record LauncherStoreCheckoutStartSnapshot(
    string Status,
    string CorrelationId,
    string? OrderId,
    string? CheckoutUrl,
    bool? Complimentary,
    string? ErrorCode,
    string? Message);

public sealed class LauncherStoreCheckoutStartService
{
    private readonly ILauncherAgentClient _agent;

    public LauncherStoreCheckoutStartService(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<LauncherStoreCheckoutStartSnapshot> StartAsync(
        string correlationId,
        string purchasePlanId,
        string purchaseMode,
        IReadOnlyList<string> legalVersionIds,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(correlationId) ||
            correlationId.Length > 128)
        {
            throw new ArgumentException(
                "Checkout correlation identifier is required.",
                nameof(correlationId));
        }

        if (string.IsNullOrWhiteSpace(purchasePlanId))
        {
            throw new ArgumentException(
                "Purchase plan identifier is required.",
                nameof(purchasePlanId));
        }

        if (purchaseMode is not ("SELF" or "GIFT"))
        {
            throw new ArgumentException(
                "Purchase mode must be SELF or GIFT.",
                nameof(purchaseMode));
        }

        if (legalVersionIds is not { Count: >= 2 and <= 3 } ||
            legalVersionIds.Any(string.IsNullOrWhiteSpace) ||
            legalVersionIds.Distinct(StringComparer.Ordinal).Count() !=
                legalVersionIds.Count)
        {
            throw new ArgumentException(
                "Checkout requires the exact reviewed Legal document versions.",
                nameof(legalVersionIds));
        }

        var response = await _agent.StartStoreCheckoutAsync(
            new StoreCheckoutStartRequest(
                correlationId,
                purchasePlanId,
                purchaseMode,
                legalVersionIds),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.StoreCheckoutStartCapabilityId ||
            response.ContractVersion != AgentLocalContract.StoreCheckoutStartContractVersion ||
            response.CorrelationId != correlationId)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent Store checkout-start contract drifted.");
        }

        return new LauncherStoreCheckoutStartSnapshot(
            response.Status,
            response.CorrelationId,
            response.OrderId,
            response.CheckoutUrl,
            response.Complimentary,
            response.Error?.Code,
            response.Error?.Message);
    }
}
