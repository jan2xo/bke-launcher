using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed record LauncherStoreTrialStartSnapshot(
    string Status,
    string CorrelationId,
    string? TrialEndsAt,
    string? GraceEndsAt,
    string? ErrorCode,
    string? Message);

public sealed class LauncherStoreTrialStartService
{
    private readonly ILauncherAgentClient _agent;

    public LauncherStoreTrialStartService(
        ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<LauncherStoreTrialStartSnapshot> StartAsync(
        string editionId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(editionId) ||
            editionId.Length > 256 ||
            editionId.Any(character => character < 32))
        {
            throw new ArgumentException(
                "Store edition identifier is invalid.",
                nameof(editionId));
        }

        var correlationId = Guid.NewGuid().ToString("N");
        var response = await _agent.StartStoreTrialAsync(
            new StoreTrialStartRequest(
                correlationId,
                editionId),
            cancellationToken);

        if (response.CapabilityId !=
                AgentLocalContract.StoreTrialStartCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.StoreTrialStartContractVersion ||
            response.CorrelationId != correlationId)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent Store trial-start contract drifted.");
        }

        if (response.Status == "STARTED")
        {
            if (response.Error is not null ||
                string.IsNullOrWhiteSpace(response.TrialEndsAt) ||
                string.IsNullOrWhiteSpace(response.GraceEndsAt) ||
                !DateTimeOffset.TryParse(response.TrialEndsAt, out _) ||
                !DateTimeOffset.TryParse(response.GraceEndsAt, out _))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent Store trial-start success response drifted.");
            }
        }
        else if (response.TrialEndsAt is not null ||
                 response.GraceEndsAt is not null ||
                 response.Error is null ||
                 response.Error.Retryable)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent Store trial-start failure response drifted.");
        }

        return new LauncherStoreTrialStartSnapshot(
            response.Status,
            response.CorrelationId,
            response.TrialEndsAt,
            response.GraceEndsAt,
            response.Error?.Code,
            response.Error?.Message);
    }
}
