using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountBillingController
{
    private const int MaximumItems = 50;
    private const int MaximumInvoiceLines = 100;

    private readonly ILauncherAgentClient _agent;

    public LauncherAccountBillingController(
        ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<AccountBillingResponse> GetAsync(
        CancellationToken cancellationToken)
    {
        var response = await _agent.GetAccountBillingAsync(
            new AccountBillingRequest(
                Guid.NewGuid().ToString("N")),
            cancellationToken);

        Validate(response);
        return response;
    }

    private static void Validate(
        AccountBillingResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountBillingCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountBillingContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent billing contract drifted.");
        }

        if (response.Status is
            "AUTH_REQUIRED" or
            "FORBIDDEN" or
            "FAILED" or
            "INVALID_INPUT")
        {
            if (response.Account is not null ||
                response.Permissions is not null ||
                response.Invoices.Count != 0 ||
                response.Payments.Count != 0 ||
                response.Error is null ||
                string.IsNullOrWhiteSpace(response.Error.Code) ||
                string.IsNullOrWhiteSpace(response.Error.Message))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent billing failure state drifted.");
            }

            return;
        }

        if (response.Status != "READY" ||
            response.Account is null ||
            response.Permissions is null ||
            response.Error is not null ||
            response.Invoices.Count > MaximumItems ||
            response.Payments.Count > MaximumItems ||
            !ValidAccount(response.Account) ||
            !response.Permissions.ViewInvoices &&
                response.Invoices.Count != 0 ||
            !response.Permissions.ViewPayments &&
                response.Payments.Count != 0 ||
            response.Invoices.Any(invoice =>
                !ValidInvoice(invoice)) ||
            response.Payments.Any(payment =>
                !ValidPayment(payment)))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent billing response drifted.");
        }
    }

    private static bool ValidAccount(
        AccountBillingAccount account) =>
        account.Type is "INDIVIDUAL" or "ORGANIZATION" &&
        !string.IsNullOrWhiteSpace(account.DisplayName) &&
        account.DisplayName.Length <= 120 &&
        !string.IsNullOrWhiteSpace(account.LifecycleState) &&
        account.LifecycleState.Length <= 40 &&
        account.Role is
            "OWNER" or
            "BILLING" or
            "LICENSE_MANAGER" or
            "MEMBER";

    private static bool ValidInvoice(
        AccountBillingInvoice invoice) =>
        ValidText(invoice.Number, 1, 120) &&
        invoice.Status is "DRAFT" or "FINAL" or "VOID" &&
        ValidText(invoice.OrderNumber, 1, 120) &&
        ValidCurrency(invoice.Currency) &&
        invoice.SubtotalMinor >= 0 &&
        invoice.TaxMinor >= 0 &&
        invoice.TotalMinor >= 0 &&
        ValidOptionalTimestamp(invoice.IssuedAt) &&
        ValidTimestamp(invoice.CreatedAt) &&
        invoice.Lines.Count <= MaximumInvoiceLines &&
        invoice.Lines.All(line =>
            ValidText(line.Description, 1, 500) &&
            line.Quantity > 0 &&
            line.UnitAmountMinor >= 0 &&
            line.TotalMinor >= 0);

    private static bool ValidPayment(
        AccountBillingPayment payment) =>
        ValidText(payment.OrderNumber, 1, 120) &&
        payment.Status is
            "PENDING" or
            "PAID" or
            "FAILED" or
            "REFUNDED" or
            "PARTIALLY_REFUNDED" &&
        payment.AmountMinor >= 0 &&
        ValidCurrency(payment.Currency) &&
        ValidOptionalTimestamp(payment.PaidAt) &&
        ValidTimestamp(payment.CreatedAt);

    private static bool ValidCurrency(string value) =>
        value.Length == 3 &&
        value.All(character =>
            character is >= 'A' and <= 'Z');

    private static bool ValidTimestamp(string value) =>
        DateTimeOffset.TryParse(value, out _);

    private static bool ValidOptionalTimestamp(
        string? value) =>
        value is null ||
        DateTimeOffset.TryParse(value, out _);

    private static bool ValidText(
        string? value,
        int minimum,
        int maximum) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.Length >= minimum &&
        value.Length <= maximum;
}
