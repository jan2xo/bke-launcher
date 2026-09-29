using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountOrganizationController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherAccountOrganizationController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<AccountOrganizationOverviewResponse> GetAsync(
        CancellationToken cancellationToken)
    {
        var response = await _agent.GetAccountOrganizationAsync(
            new AccountOrganizationOverviewRequest(
                Guid.NewGuid().ToString("N")),
            cancellationToken);

        ValidateContract(response);

        if (response.Status == "READY")
        {
            ValidateReady(response);
        }
        else if (response.Status == "NOT_ORGANIZATION")
        {
            if (response.Account is not null ||
                response.Permissions is not null ||
                response.Organization is not null ||
                response.BillingEmail is not null ||
                response.TaxId is not null ||
                response.Counts is not null ||
                response.Members.Count != 0 ||
                response.Invitations.Count != 0)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent organization no-op response exposed organization data.");
            }
        }

        return response;
    }

    private static void ValidateContract(
        AccountOrganizationOverviewResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountOrganizationCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountOrganizationContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization contract drifted.");
        }

        if (response.Members is null || response.Invitations is null)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization lists are missing.");
        }
    }

    private static void ValidateReady(
        AccountOrganizationOverviewResponse response)
    {
        if (response.Account is null ||
            response.Permissions is null ||
            response.Organization is null ||
            response.Counts is null)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization overview is incomplete.");
        }

        RequireBounded(response.Account.DisplayName, 120, "display name");
        RequireToken(response.Account.LifecycleState, 64, "lifecycle");
        RequireRole(response.Account.Role);
        RequireBounded(response.Organization.LegalName, 180, "legal name");
        RequireOptionalBounded(
            response.Organization.RegistrationNumber,
            80,
            "registration number");
        RequireOptionalBounded(response.BillingEmail, 320, "billing email");
        RequireOptionalBounded(response.TaxId, 80, "tax ID");

        RequireNonNegative(response.Counts.Licenses, "license count");
        RequireNonNegative(
            response.Counts.Subscriptions,
            "subscription count");
        RequireNonNegative(response.Counts.Orders, "order count");

        if (!response.Permissions.ViewBilling &&
            (response.BillingEmail is not null ||
             response.TaxId is not null ||
             response.Counts.Orders is not null))
        {
            throw new InvalidDataException(
                "Organization response exposed billing data without Agent billing permission.");
        }

        if (!response.Permissions.ViewLicenses &&
            response.Counts.Licenses is not null)
        {
            throw new InvalidDataException(
                "Organization response exposed license data without Agent license permission.");
        }

        if (!response.Permissions.ManageMembers &&
            (response.Members.Count != 0 ||
             response.Invitations.Count != 0))
        {
            throw new InvalidDataException(
                "Organization response exposed members without Agent member permission.");
        }

        if (response.Members.Count > 500 ||
            response.Invitations.Count > 500)
        {
            throw new InvalidDataException(
                "Organization response exceeded Launcher presentation limits.");
        }

        foreach (var member in response.Members)
        {
            RequireBounded(member.Email, 320, "member email");
            RequireOptionalBounded(member.Name, 160, "member name");
            RequireRole(member.Role);
        }

        foreach (var invitation in response.Invitations)
        {
            RequireBounded(invitation.Email, 320, "invitation email");
            RequireRole(invitation.Role);
            RequireToken(invitation.Status, 32, "invitation status");
            RequireTimestamp(invitation.ExpiresAt, "invitation expiry");
            RequireTimestamp(invitation.CreatedAt, "invitation creation");
        }
    }

    private static void RequireRole(string value)
    {
        if (value is not (
            "OWNER" or
            "BILLING" or
            "LICENSE_MANAGER" or
            "MEMBER"))
        {
            throw new InvalidDataException(
                "Organization role drifted.");
        }
    }

    private static void RequireToken(
        string value,
        int maximum,
        string label)
    {
        RequireBounded(value, maximum, label);
        if (value.Any(character =>
            character is not (>= 'A' and <= 'Z') &&
            character != '_'))
        {
            throw new InvalidDataException(
                $"Organization {label} drifted.");
        }
    }

    private static void RequireBounded(
        string? value,
        int maximum,
        string label)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Length > maximum)
        {
            throw new InvalidDataException(
                $"Organization {label} drifted.");
        }
    }

    private static void RequireOptionalBounded(
        string? value,
        int maximum,
        string label)
    {
        if (value is not null &&
            (string.IsNullOrWhiteSpace(value) ||
             value.Length > maximum))
        {
            throw new InvalidDataException(
                $"Organization {label} drifted.");
        }
    }

    private static void RequireNonNegative(
        int? value,
        string label)
    {
        if (value < 0)
        {
            throw new InvalidDataException(
                $"Organization {label} drifted.");
        }
    }

    private static void RequireTimestamp(
        string value,
        string label)
    {
        if (!DateTimeOffset.TryParse(value, out _))
        {
            throw new InvalidDataException(
                $"Organization {label} drifted.");
        }
    }
}
