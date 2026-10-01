using System.Text.RegularExpressions;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountPendingOrdersController
{
    private static readonly Regex ContinueHandlePattern = new(
        "^bke-order-continue-v1_[0-9a-f]{64}$",
        RegexOptions.CultureInvariant);

    private static readonly Regex CancelHandlePattern = new(
        "^bke-order-cancel-v1_[0-9a-f]{64}$",
        RegexOptions.CultureInvariant);

    private readonly ILauncherAgentClient _agent;

    public LauncherAccountPendingOrdersController(
        ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<AccountPendingOrderContinueResponse> ContinueAsync(
        string orderContinueHandle,
        CancellationToken cancellationToken)
    {
        if (!ValidContinueHandle(orderContinueHandle))
        {
            throw new InvalidDataException(
                "The pending-order continuation handle is invalid.");
        }

        var response =
            await _agent.ContinueAccountPendingOrderAsync(
                new AccountPendingOrderContinueRequest(
                    Guid.NewGuid().ToString("N"),
                    orderContinueHandle),
                cancellationToken);

        ValidateContinue(response);
        return response;
    }

    public async Task<AccountPendingOrderCancelResponse> CancelAsync(
        string orderCancelHandle,
        CancellationToken cancellationToken)
    {
        if (!ValidCancelHandle(orderCancelHandle))
        {
            throw new InvalidDataException(
                "The pending-order cancellation handle is invalid.");
        }

        var response =
            await _agent.CancelAccountPendingOrderAsync(
                new AccountPendingOrderCancelRequest(
                    Guid.NewGuid().ToString("N"),
                    orderCancelHandle),
                cancellationToken);

        ValidateCancel(response);
        return response;
    }

    internal static bool ValidContinueHandle(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        ContinueHandlePattern.IsMatch(value);

    internal static bool ValidCancelHandle(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        CancelHandlePattern.IsMatch(value);

    private static void ValidateContinue(
        AccountPendingOrderContinueResponse response)
    {
        ValidateEnvelope(
            response.CapabilityId,
            response.ContractVersion);

        if (response.Status == "CONTINUED")
        {
            if (response.Error is not null ||
                !ValidCheckoutUrl(response.CheckoutUrl))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent pending-order continuation success drifted.");
            }

            return;
        }

        if (response.Status is not (
                "AUTH_REQUIRED" or
                "FORBIDDEN" or
                "NOT_FOUND" or
                "ACCOUNT_NOT_ACTIVE" or
                "CHECKOUT_CREATION_IN_PROGRESS" or
                "LEGAL_ACCEPTANCE_REQUIRED" or
                "LEGAL_REACCEPTANCE_REQUIRED" or
                "INVALID_INPUT" or
                "OUTCOME_UNKNOWN" or
                "FAILED") ||
            response.CheckoutUrl is not null ||
            !ValidError(response.Error))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent pending-order continuation state drifted.");
        }
    }

    private static void ValidateCancel(
        AccountPendingOrderCancelResponse response)
    {
        ValidateEnvelope(
            response.CapabilityId,
            response.ContractVersion);

        if (response.Status == "CANCELLED")
        {
            if (response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent pending-order cancellation success drifted.");
            }

            return;
        }

        if (response.Status is not (
                "AUTH_REQUIRED" or
                "FORBIDDEN" or
                "NOT_FOUND" or
                "ACCOUNT_NOT_ACTIVE" or
                "LEGAL_ACCEPTANCE_REQUIRED" or
                "LEGAL_REACCEPTANCE_REQUIRED" or
                "INVALID_INPUT" or
                "OUTCOME_UNKNOWN" or
                "FAILED") ||
            !ValidError(response.Error))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent pending-order cancellation state drifted.");
        }
    }

    private static void ValidateEnvelope(
        string capabilityId,
        int contractVersion)
    {
        if (capabilityId !=
                AgentLocalContract.AccountPendingOrdersCapabilityId ||
            contractVersion !=
                AgentLocalContract.AccountPendingOrdersContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent pending-order contract drifted.");
        }
    }

    private static bool ValidError(
        AccountPendingOrderError? error) =>
        error is not null &&
        !string.IsNullOrWhiteSpace(error.Code) &&
        !string.IsNullOrWhiteSpace(error.Message);

    private static bool ValidCheckoutUrl(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
            !string.IsNullOrEmpty(uri.UserInfo) ||
            !string.IsNullOrEmpty(uri.Fragment))
        {
            return false;
        }

        return uri.Scheme == Uri.UriSchemeHttps ||
            uri.Scheme == Uri.UriSchemeHttp && uri.IsLoopback;
    }
}
