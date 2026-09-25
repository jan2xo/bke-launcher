using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed record LauncherStoreGiftClaimRevealSnapshot(
    string Status,
    string CorrelationId,
    string? OrderId,
    string? ClaimCodeId,
    string? ClaimCode,
    string? ErrorCode,
    string? Message,
    bool ErrorRetryable);

public sealed class LauncherStoreGiftClaimRevealService
{
    private readonly ILauncherAgentClient _agent;

    public LauncherStoreGiftClaimRevealService(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<LauncherStoreGiftClaimRevealSnapshot> RevealAsync(
        string correlationId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(correlationId) ||
            correlationId.Length > 128)
        {
            throw new ArgumentException(
                "Gift checkout correlation identifier is required.",
                nameof(correlationId));
        }

        var response = await _agent.RevealStoreGiftClaimCodeAsync(
            new StoreGiftClaimRevealRequest(correlationId),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.StoreGiftClaimRevealCapabilityId ||
            response.ContractVersion != AgentLocalContract.StoreGiftClaimRevealContractVersion ||
            response.CorrelationId != correlationId)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent Store gift Claim Code contract drifted.");
        }

        return new LauncherStoreGiftClaimRevealSnapshot(
            response.Status,
            response.CorrelationId,
            response.OrderId,
            response.ClaimCodeId,
            response.ClaimCode,
            response.Error?.Code,
            response.Error?.Message,
            response.Error?.Retryable == true);
    }
}
