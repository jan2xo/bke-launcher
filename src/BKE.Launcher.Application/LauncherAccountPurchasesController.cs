using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountPurchasesController
{
    private const int MaximumItems = 50;
    private const int MaximumOrderItems = 100;

    private readonly ILauncherAgentClient _agent;

    public LauncherAccountPurchasesController(
        ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<AccountPurchasesResponse> GetAsync(
        CancellationToken cancellationToken)
    {
        var response = await _agent.GetAccountPurchasesAsync(
            new AccountPurchasesRequest(
                Guid.NewGuid().ToString("N")),
            cancellationToken);

        ValidateContract(response);
        return response;
    }

    private static void ValidateContract(
        AccountPurchasesResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountPurchasesCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountPurchasesContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account purchases contract drifted.");
        }

        if (response.Status is not (
            "READY" or
            "AUTH_REQUIRED" or
            "FORBIDDEN" or
            "FAILED" or
            "INVALID_INPUT"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account purchases status drifted.");
        }

        if (response.Status != "READY")
        {
            if (response.Account is not null ||
                response.Permissions is not null ||
                response.Licenses is null ||
                response.Subscriptions is null ||
                response.Orders is null ||
                response.Licenses.Count != 0 ||
                response.Subscriptions.Count != 0 ||
                response.Orders.Count != 0 ||
                response.Error is null ||
                string.IsNullOrWhiteSpace(response.Error.Code) ||
                string.IsNullOrWhiteSpace(response.Error.Message))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent account purchases failure state drifted.");
            }

            return;
        }

        if (response.Account is null ||
            response.Permissions is null ||
            response.Licenses is null ||
            response.Subscriptions is null ||
            response.Orders is null ||
            response.Error is not null ||
            response.Licenses.Count > MaximumItems ||
            response.Subscriptions.Count > MaximumItems ||
            response.Orders.Count > MaximumItems)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account purchases ready state drifted.");
        }

        ValidateAccount(response.Account);

        foreach (var license in response.Licenses)
        {
            if (license is null ||
                !ValidText(license.ProductName, 1, 200) ||
                !ValidOptionalText(license.EditionName, 120) ||
                !ValidPlanType(license.PlanType) ||
                !ValidStatus(license.Status) ||
                string.IsNullOrEmpty(license.KeyLastFour) ||
                license.KeyLastFour.Length != 4 ||
                license.KeyLastFour.Any(character => character < 32) ||
                !ValidOptionalTimestamp(license.ExpiresAt) ||
                license.MaxDevices < 0 ||
                license.ActiveDevices < 0 ||
                license.ActiveDevices > license.MaxDevices ||
                license.MaxSeats < 0 ||
                license.AssignedSeats < 0 ||
                license.AssignedSeats > license.MaxSeats ||
                license.SeatManagementHandle is not null &&
                    !LauncherAccountLicenseSeatsController.ValidLicenseHandle(
                        license.SeatManagementHandle) ||
                license.DeviceManagementHandle is not null &&
                    !LauncherAccountLicenseDevicesController.ValidLicenseHandle(
                        license.DeviceManagementHandle))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent account license item drifted.");
            }
        }

        if (!response.Permissions.ManageLicenseSeats &&
            response.Licenses.Any(license =>
                license.SeatManagementHandle is not null))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent exposed seat-management handles without permission.");
        }

        if (!response.Permissions.ManageDevices &&
            response.Licenses.Any(license =>
                license.DeviceManagementHandle is not null))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent exposed device-management handles without permission.");
        }

        foreach (var subscription in response.Subscriptions)
        {
            if (subscription is null ||
                !ValidText(subscription.ProductName, 1, 200) ||
                !ValidOptionalText(subscription.EditionName, 120) ||
                !ValidPlanType(subscription.PlanType) ||
                !ValidStatus(subscription.Status) ||
                subscription.Seats < 1 ||
                !DateTimeOffset.TryParse(
                    subscription.CurrentPeriodEnd,
                    out _))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent account subscription item drifted.");
            }
        }

        foreach (var order in response.Orders)
        {
            if (order is null ||
                !ValidText(order.Number, 1, 120) ||
                !ValidStatus(order.Status) ||
                order.TotalMinor < 0 ||
                string.IsNullOrEmpty(order.Currency) ||
                order.Currency.Length != 3 ||
                order.Currency.Any(character =>
                    character is not (>= 'A' and <= 'Z')) ||
                !DateTimeOffset.TryParse(order.CreatedAt, out _) ||
                order.ContinueHandle is not null &&
                    !LauncherAccountPendingOrdersController.ValidContinueHandle(
                        order.ContinueHandle) ||
                order.CancelHandle is not null &&
                    !LauncherAccountPendingOrdersController.ValidCancelHandle(
                        order.CancelHandle) ||
                order.Status != "PENDING" &&
                    (order.ContinueHandle is not null ||
                     order.CancelHandle is not null) ||
                response.Account.LifecycleState != "ACTIVE" &&
                    (order.ContinueHandle is not null ||
                     order.CancelHandle is not null) ||
                !response.Permissions.ContinuePendingOrders &&
                    order.ContinueHandle is not null ||
                !response.Permissions.CancelPendingOrders &&
                    order.CancelHandle is not null ||
                response.Account.LifecycleState == "ACTIVE" &&
                    order.Status == "PENDING" &&
                    response.Permissions.ContinuePendingOrders &&
                    order.ContinueHandle is null ||
                response.Account.LifecycleState == "ACTIVE" &&
                    order.Status == "PENDING" &&
                    response.Permissions.CancelPendingOrders &&
                    order.CancelHandle is null ||
                order.Items is null ||
                order.Items.Count > MaximumOrderItems ||
                order.Items.Any(item =>
                    item is null ||
                    !ValidText(item.ProductName, 1, 200) ||
                    !ValidOptionalText(item.EditionName, 120) ||
                    !ValidOptionalText(item.PlanName, 120)))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent account order item drifted.");
            }
        }
    }

    private static void ValidateAccount(
        AccountPurchasesAccount account)
    {
        if (account.Type is not ("INDIVIDUAL" or "ORGANIZATION") ||
            !ValidText(account.DisplayName, 1, 120) ||
            !ValidStatus(account.LifecycleState) ||
            account.Role is not (
                "OWNER" or
                "BILLING" or
                "LICENSE_MANAGER" or
                "MEMBER"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent purchases account metadata drifted.");
        }
    }

    private static bool ValidPlanType(string? value) =>
        value is null or "PERPETUAL" or "MONTHLY" or "ANNUAL";

    private static bool ValidStatus(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Length <= 64 &&
        value.All(character =>
            character is >= 'A' and <= 'Z' ||
            character == '_');

    private static bool ValidOptionalTimestamp(string? value) =>
        value is null ||
        DateTimeOffset.TryParse(value, out _);

    private static bool ValidOptionalText(
        string? value,
        int maximum) =>
        value is null ||
        ValidText(value, 0, maximum);

    private static bool ValidText(
        string? value,
        int minimum,
        int maximum) =>
        value is not null &&
        value.Length >= minimum &&
        value.Length <= maximum &&
        value.All(character => character >= 32);
}
