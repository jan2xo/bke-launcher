using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed record LauncherStoreCheckoutStatusSnapshot(
    string Status,
    string CorrelationId,
    string? OrderId,
    string? OrderNumber,
    string? OrderStatus,
    string? FulfillmentMode,
    string? PaymentStatus,
    string? CheckoutUrl,
    string? PaidAt,
    string? ErrorCode,
    string? Message,
    bool ErrorRetryable);

public sealed class LauncherStoreCheckoutStatusService
{
    private readonly ILauncherAgentClient _agent;

    public LauncherStoreCheckoutStatusService(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<LauncherStoreCheckoutStatusSnapshot> CheckAsync(
        string correlationId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(correlationId) ||
            correlationId.Length > 128)
        {
            throw new ArgumentException(
                "Checkout correlation identifier is required.",
                nameof(correlationId));
        }

        var response = await _agent.CheckStoreCheckoutStatusAsync(
            new StoreCheckoutStatusRequest(correlationId),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.StoreCheckoutStatusCapabilityId ||
            response.ContractVersion != AgentLocalContract.StoreCheckoutStatusContractVersion ||
            response.CorrelationId != correlationId)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent Store checkout-status contract drifted.");
        }

        return new LauncherStoreCheckoutStatusSnapshot(
            response.Status,
            response.CorrelationId,
            response.OrderId,
            response.OrderNumber,
            response.OrderStatus,
            response.FulfillmentMode,
            response.PaymentStatus,
            response.CheckoutUrl,
            response.PaidAt,
            response.Error?.Code,
            response.Error?.Message,
            response.Error?.Retryable == true);
    }
}
