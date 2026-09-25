using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed record LauncherNotificationSnapshot(
    string Status,
    IReadOnlyList<AccountNotificationItem> Items,
    string? Message);

public sealed record LauncherNotificationMutationResult(
    string Status,
    string? State,
    string? Message);

public sealed class LauncherNotificationInboxService
{
    private static readonly HashSet<string> AllowedAudienceKinds =
        new(StringComparer.Ordinal)
        {
            "ACCOUNT",
            "PRINCIPAL",
            "ALL_USERS",
            "ALL_ACTIVE_CLIENTS",
        };

    private readonly ILauncherAgentClient _agent;

    public LauncherNotificationInboxService(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<LauncherNotificationSnapshot> GetAsync(
        int limit,
        CancellationToken cancellationToken)
    {
        if (limit is < 1 or > 200)
        {
            throw new ArgumentOutOfRangeException(
                nameof(limit),
                "Notification limit must be between 1 and 200.");
        }

        var response = await _agent.GetAccountNotificationsAsync(
            new AccountNotificationFeedRequest(limit),
            cancellationToken);

        if (response.CapabilityId !=
                AgentLocalContract.AccountNotificationInboxCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountNotificationInboxContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent notification contract drifted.");
        }

        if (response.Status is not ("Succeeded" or "Failed") ||
            response.Items is null ||
            response.Items.Count > limit ||
            response.Items.Any(item =>
                !AllowedAudienceKinds.Contains(item.AudienceKind)) ||
            response.Items.Any(item =>
                item.State is not ("Unread" or "Read")) ||
            response.Items.Any(item =>
                item.Severity is not ("Information" or "Warning")) ||
            (response.Status == "Failed" &&
                (response.Items.Count != 0 || response.Error is null)))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent returned an invalid notification feed.");
        }

        var items = response.Items
            .OrderByDescending(item =>
                DateTimeOffset.TryParse(item.CreatedAt, out var created)
                    ? created
                    : DateTimeOffset.MinValue)
            .ThenBy(item => item.Id, StringComparer.Ordinal)
            .ToArray();

        return new LauncherNotificationSnapshot(
            response.Status,
            items,
            response.Error?.Message);
    }

    public async Task<LauncherNotificationMutationResult> MutateAsync(
        string notificationId,
        string action,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(notificationId) ||
            notificationId.Length > 160)
        {
            throw new ArgumentException(
                "Notification identifier is required.",
                nameof(notificationId));
        }
        if (action is not ("MARK_READ" or "DISMISS"))
        {
            throw new ArgumentOutOfRangeException(
                nameof(action),
                "Notification action must be MARK_READ or DISMISS.");
        }

        var response = await _agent.MutateAccountNotificationAsync(
            new AccountNotificationReceiptRequest(
                notificationId,
                action),
            cancellationToken);

        if (response.CapabilityId !=
                AgentLocalContract.AccountNotificationInboxCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountNotificationInboxContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent notification receipt contract drifted.");
        }

        var expectedState = action == "MARK_READ"
            ? "READ"
            : "DISMISSED";
        var validSucceeded =
            response.Status == "Succeeded" &&
            response.MutationStatus is ("UPDATED" or "UNCHANGED") &&
            response.State == expectedState &&
            response.Error is null;
        var validNotFound =
            response.Status == "NotFound" &&
            response.MutationStatus == "NOT_FOUND" &&
            response.State is null &&
            response.Error is null;
        var validFailed =
            response.Status == "Failed" &&
            response.MutationStatus is null &&
            response.State is null &&
            response.Error is not null;

        if (!validSucceeded && !validNotFound && !validFailed)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent returned an invalid notification receipt result.");
        }

        return new LauncherNotificationMutationResult(
            response.Status,
            response.State,
            response.Error?.Message);
    }
}
