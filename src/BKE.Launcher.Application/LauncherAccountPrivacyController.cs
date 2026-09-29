using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountPrivacyController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherAccountPrivacyController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<AccountPrivacyListResponse> ListAsync(
        int limit,
        CancellationToken cancellationToken)
    {
        if (limit is < 1 or > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Privacy request list limit must be between 1 and 100.");
        }

        var response = await _agent.GetAccountPrivacyRequestsAsync(
            new AccountPrivacyListRequest(
                NewCorrelationId(),
                limit),
            cancellationToken);
        ValidateContract(
            response.CapabilityId,
            response.ContractVersion);

        if (response.Status == "READY")
        {
            if (response.RequestTypes.Count == 0 ||
                response.RequestTypes.Count > 32 ||
                response.RequestTypes.Any(type => !ValidRequestType(type)) ||
                response.RequestTypes.Distinct(StringComparer.Ordinal).Count() !=
                    response.RequestTypes.Count)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent privacy request types drifted.");
            }

            if (response.Items.Count > limit ||
                response.Items.Any(item =>
                    string.IsNullOrWhiteSpace(item.Id) ||
                    item.Scope is not ("USER" or "ACCOUNT") ||
                    !ValidRequestType(item.RequestType) ||
                    string.IsNullOrWhiteSpace(item.Status) ||
                    !ValidSummary(item.Summary) ||
                    !DateTimeOffset.TryParse(item.CreatedAt, out _)))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent privacy request list drifted.");
            }
        }

        return response;
    }

    public async Task<AccountPrivacyCreateResponse> CreateAsync(
        string requestType,
        string summary,
        CancellationToken cancellationToken)
    {
        if (!ValidRequestType(requestType))
        {
            throw new ArgumentException(
                "A valid privacy request type is required.",
                nameof(requestType));
        }

        if (!ValidSummary(summary))
        {
            throw new ArgumentException(
                "Privacy request summary must be between 10 and 2,000 characters.",
                nameof(summary));
        }

        var response = await _agent.CreateAccountPrivacyRequestAsync(
            new AccountPrivacyCreateRequest(
                NewCorrelationId(),
                requestType,
                summary.Trim()),
            cancellationToken);
        ValidateContract(
            response.CapabilityId,
            response.ContractVersion);
        return response;
    }

    private static void ValidateContract(
        string capabilityId,
        int contractVersion)
    {
        if (capabilityId != AgentLocalContract.AccountPrivacyCapabilityId ||
            contractVersion != AgentLocalContract.AccountPrivacyContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account privacy contract drifted.");
        }
    }

    private static bool ValidRequestType(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Length is >= 3 and <= 64 &&
        value.All(character =>
            character is >= 'A' and <= 'Z' ||
            character == '_');

    private static bool ValidSummary(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Trim().Length is >= 10 and <= 2_000 &&
        value.All(character =>
            character >= 32 ||
            character is '\r' or '\n' or '\t');

    private static string NewCorrelationId() =>
        Guid.NewGuid().ToString("N");
}
