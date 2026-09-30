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

    public async Task<AccountOrganizationCreateResponse> CreateAsync(
        string displayName,
        string legalName,
        string billingEmail,
        string? registrationNumber,
        string? taxId,
        CancellationToken cancellationToken)
    {
        RequireInput(displayName, 2, 120, "display name");
        RequireInput(legalName, 2, 180, "legal name");
        RequireEmail(billingEmail);
        RequireOptionalInput(registrationNumber, 80, "registration number");
        RequireOptionalInput(taxId, 80, "tax ID");

        var response = await _agent.CreateAccountOrganizationAsync(
            new AccountOrganizationCreateRequest(
                Guid.NewGuid().ToString("N"),
                displayName.Trim(),
                legalName.Trim(),
                billingEmail.Trim(),
                NormalizeOptional(registrationNumber),
                NormalizeOptional(taxId)),
            cancellationToken);

        ValidateCreateContract(response);
        return response;
    }

    public async Task<AccountOrganizationProfileUpdateResponse> UpdateProfileAsync(
        bool updateOrganizationProfile,
        string? displayName,
        string? legalName,
        string? registrationNumber,
        bool updateBillingProfile,
        string? billingEmail,
        string? taxId,
        CancellationToken cancellationToken)
    {
        if (!updateOrganizationProfile && !updateBillingProfile)
        {
            throw new ArgumentException(
                "Choose at least one organization profile group to update.");
        }

        if (updateOrganizationProfile)
        {
            RequireInput(displayName, 2, 120, "display name");
            RequireInput(legalName, 2, 180, "legal name");
            RequireOptionalInput(
                registrationNumber,
                80,
                "registration number");
        }
        else if (displayName is not null ||
                 legalName is not null ||
                 registrationNumber is not null)
        {
            throw new ArgumentException(
                "Disabled organization-profile fields must be empty.");
        }

        if (updateBillingProfile)
        {
            RequireEmail(billingEmail);
            RequireOptionalInput(taxId, 80, "tax ID");
        }
        else if (billingEmail is not null || taxId is not null)
        {
            throw new ArgumentException(
                "Disabled billing-profile fields must be empty.");
        }

        var response =
            await _agent.UpdateAccountOrganizationProfileAsync(
                new AccountOrganizationProfileUpdateRequest(
                    Guid.NewGuid().ToString("N"),
                    updateOrganizationProfile,
                    updateOrganizationProfile
                        ? displayName!.Trim()
                        : null,
                    updateOrganizationProfile
                        ? legalName!.Trim()
                        : null,
                    updateOrganizationProfile
                        ? NormalizeOptional(registrationNumber)
                        : null,
                    updateBillingProfile,
                    updateBillingProfile
                        ? billingEmail!.Trim()
                        : null,
                    updateBillingProfile
                        ? NormalizeOptional(taxId)
                        : null),
                cancellationToken);

        ValidateProfileUpdateContract(response);
        return response;
    }

    public async Task<AccountOrganizationInvitationCreateResponse> CreateInvitationAsync(
        string email,
        string role,
        CancellationToken cancellationToken)
    {
        RequireMemberEmail(email);
        if (role is not (
            "OWNER" or
            "BILLING" or
            "LICENSE_MANAGER" or
            "MEMBER"))
        {
            throw new ArgumentException(
                "Organization invitation role is invalid.");
        }

        var response =
            await _agent.CreateAccountOrganizationInvitationAsync(
                new AccountOrganizationInvitationCreateRequest(
                    Guid.NewGuid().ToString("N"),
                    email.Trim(),
                    role),
                cancellationToken);

        ValidateInvitationCreateContract(response);
        return response;
    }

    public async Task<AccountOrganizationInvitationManageResponse> ManageInvitationAsync(
        string action,
        string managementHandle,
        CancellationToken cancellationToken)
    {
        if (action is not ("RESEND" or "REVOKE"))
        {
            throw new ArgumentException(
                "Organization invitation action is invalid.");
        }
        RequireInvitationManagementHandle(managementHandle);

        var response =
            await _agent.ManageAccountOrganizationInvitationAsync(
                new AccountOrganizationInvitationManageRequest(
                    Guid.NewGuid().ToString("N"),
                    action,
                    managementHandle),
                cancellationToken);

        ValidateInvitationManageContract(response);
        return response;
    }

    public async Task<AccountOrganizationMemberManageResponse> ManageMemberAsync(
        string action,
        string managementHandle,
        string? role,
        CancellationToken cancellationToken)
    {
        if (action is not ("UPDATE_ROLE" or "REMOVE"))
        {
            throw new ArgumentException(
                "Organization member action is invalid.");
        }

        RequireMemberManagementHandle(managementHandle);

        if (action == "UPDATE_ROLE")
        {
            if (role is not (
                "OWNER" or
                "BILLING" or
                "LICENSE_MANAGER" or
                "MEMBER"))
            {
                throw new ArgumentException(
                    "Organization member role is invalid.");
            }
        }
        else if (role is not null)
        {
            throw new ArgumentException(
                "Organization member removal must not include a role.");
        }

        var response =
            await _agent.ManageAccountOrganizationMemberAsync(
                new AccountOrganizationMemberManageRequest(
                    Guid.NewGuid().ToString("N"),
                    action,
                    managementHandle,
                    role),
                cancellationToken);

        ValidateMemberManageContract(response);
        return response;
    }

    public async Task<AccountOrganizationLeaveResponse> LeaveAsync(
        CancellationToken cancellationToken)
    {
        var response = await _agent.LeaveAccountOrganizationAsync(
            new AccountOrganizationLeaveRequest(
                Guid.NewGuid().ToString("N")),
            cancellationToken);

        ValidateLeaveContract(response);
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
            RequireContractMemberManagementHandle(
                member.ManagementHandle);
        }

        foreach (var invitation in response.Invitations)
        {
            RequireBounded(invitation.Email, 320, "invitation email");
            RequireRole(invitation.Role);
            RequireToken(invitation.Status, 32, "invitation status");
            RequireTimestamp(invitation.ExpiresAt, "invitation expiry");
            RequireTimestamp(invitation.CreatedAt, "invitation creation");
            RequireContractInvitationManagementHandle(
                invitation.ManagementHandle);
        }
    }

    private static void ValidateMemberManageContract(
        AccountOrganizationMemberManageResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountOrganizationCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountOrganizationContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-member management contract drifted.");
        }

        if (response.Status is not (
            "UPDATED" or
            "REMOVED" or
            "INVALID_INPUT" or
            "NOT_ORGANIZATION" or
            "ACCOUNT_FORBIDDEN" or
            "MEMBER_NOT_FOUND" or
            "LAST_OWNER_REQUIRED" or
            "CLOSED_ACCOUNT" or
            "SUSPENDED_ACCOUNT" or
            "AUTH_REQUIRED" or
            "OUTCOME_UNKNOWN" or
            "FAILED"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-member management status drifted.");
        }

        if (response.Status is "UPDATED" or "REMOVED")
        {
            if (response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent organization-member success exposed error state.");
            }
            return;
        }

        if (response.Error is null ||
            string.IsNullOrWhiteSpace(response.Error.Code) ||
            string.IsNullOrWhiteSpace(response.Error.Message))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-member failure is missing error state.");
        }
    }

    private static void ValidateLeaveContract(
        AccountOrganizationLeaveResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountOrganizationCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountOrganizationContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-leave contract drifted.");
        }

        if (response.Status is not (
            "LEFT" or
            "INVALID_INPUT" or
            "NOT_ORGANIZATION" or
            "OWNER_CANNOT_LEAVE" or
            "MEMBER_NOT_FOUND" or
            "AUTH_REQUIRED" or
            "OUTCOME_UNKNOWN" or
            "FAILED"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-leave status drifted.");
        }

        if (response.Status == "LEFT")
        {
            if (!response.ReauthenticationRequired ||
                response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent organization-leave success drifted.");
            }
            return;
        }

        var requiresReauthentication = response.Status is
            "AUTH_REQUIRED" or
            "MEMBER_NOT_FOUND" or
            "OUTCOME_UNKNOWN";

        if (response.ReauthenticationRequired !=
                requiresReauthentication ||
            response.Error is null ||
            string.IsNullOrWhiteSpace(response.Error.Code) ||
            string.IsNullOrWhiteSpace(response.Error.Message))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-leave failure state drifted.");
        }
    }

    private static void ValidateInvitationManageContract(
        AccountOrganizationInvitationManageResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountOrganizationCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountOrganizationContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-invitation management contract drifted.");
        }

        if (response.Status is not (
            "RESENT" or
            "REVOKED" or
            "INVALID_INPUT" or
            "NOT_ORGANIZATION" or
            "ACCOUNT_FORBIDDEN" or
            "INVITATION_NOT_FOUND" or
            "INVITATION_NOT_PENDING" or
            "INVITATION_EXPIRED" or
            "AUTH_REQUIRED" or
            "OUTCOME_UNKNOWN" or
            "FAILED"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-invitation management status drifted.");
        }

        if (response.Status == "RESENT")
        {
            if (response.Invitation is null ||
                string.IsNullOrWhiteSpace(response.InvitationCode) ||
                response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent organization-invitation resend response is incomplete.");
            }

            ValidateIssuedInvitation(response.Invitation);
            RequireInvitationCode(response.InvitationCode);
            return;
        }

        if (response.Status == "REVOKED")
        {
            if (response.Invitation is null ||
                response.InvitationCode is not null ||
                response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent organization-invitation revoke response is incomplete.");
            }

            ValidateIssuedInvitation(response.Invitation);
            return;
        }

        if (response.Invitation is not null ||
            response.InvitationCode is not null)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-invitation management failure exposed mutation state.");
        }

        if (response.Error is null ||
            string.IsNullOrWhiteSpace(response.Error.Code) ||
            string.IsNullOrWhiteSpace(response.Error.Message))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-invitation management failure is missing error state.");
        }
    }

    private static void ValidateIssuedInvitation(
        AccountOrganizationInvitationIssued invitation)
    {
        RequireContractEmail(invitation.Email, "invitation email");
        RequireRole(invitation.Role);
        RequireToken(invitation.Status, 32, "invitation status");
        RequireTimestamp(invitation.ExpiresAt, "invitation expiry");
        RequireTimestamp(invitation.CreatedAt, "invitation creation");
    }

    private static void RequireInvitationCode(string value)
    {
        if (value.Length is < 8 or > 1024 ||
            value.Any(character => character < 32))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization invitation code drifted.");
        }
    }

    private static void ValidateInvitationCreateContract(
        AccountOrganizationInvitationCreateResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountOrganizationCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountOrganizationContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-invitation contract drifted.");
        }

        if (response.Status is not (
            "CREATED" or
            "INVALID_INPUT" or
            "NOT_ORGANIZATION" or
            "ACCOUNT_FORBIDDEN" or
            "CONFLICT" or
            "AUTH_REQUIRED" or
            "OUTCOME_UNKNOWN" or
            "FAILED"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-invitation status drifted.");
        }

        if (response.Status == "CREATED")
        {
            if (response.Invitation is null ||
                string.IsNullOrWhiteSpace(response.InvitationCode) ||
                response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent organization-invitation success is incomplete.");
            }

            ValidateIssuedInvitation(response.Invitation);
            RequireInvitationCode(response.InvitationCode);
            return;
        }

        if (response.Invitation is not null ||
            response.InvitationCode is not null)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-invitation failure exposed delivery state.");
        }

        if (response.Error is null ||
            string.IsNullOrWhiteSpace(response.Error.Code) ||
            string.IsNullOrWhiteSpace(response.Error.Message))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-invitation failure is missing bounded error state.");
        }
    }

    private static void ValidateProfileUpdateContract(
        AccountOrganizationProfileUpdateResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountOrganizationCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountOrganizationContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-profile contract drifted.");
        }

        if (response.Status is not (
            "UPDATED" or
            "INVALID_INPUT" or
            "NOT_ORGANIZATION" or
            "ACCOUNT_FORBIDDEN" or
            "AUTH_REQUIRED" or
            "OUTCOME_UNKNOWN" or
            "FAILED"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-profile status drifted.");
        }

        if (response.Status == "UPDATED")
        {
            if (response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent organization-profile success exposed error state.");
            }
            return;
        }

        if (response.Error is null ||
            string.IsNullOrWhiteSpace(response.Error.Code) ||
            string.IsNullOrWhiteSpace(response.Error.Message))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-profile failure is missing bounded error state.");
        }
    }

    private static void ValidateCreateContract(
        AccountOrganizationCreateResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountOrganizationCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountOrganizationContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-create contract drifted.");
        }

        if (response.Status is not (
            "CREATED" or
            "INVALID_INPUT" or
            "EMAIL_NOT_VERIFIED" or
            "LEGAL_REACCEPTANCE_REQUIRED" or
            "AUTH_REQUIRED" or
            "OUTCOME_UNKNOWN" or
            "FAILED"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-create status drifted.");
        }

        if (response.Status == "CREATED")
        {
            RequireBounded(
                response.DisplayName,
                120,
                "created display name");
            if (!response.SwitchRequired || response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent organization-create success drifted.");
            }
            return;
        }

        if (response.DisplayName is not null ||
            response.SwitchRequired)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-create failure exposed success state.");
        }

        if (response.Error is null ||
            string.IsNullOrWhiteSpace(response.Error.Code) ||
            string.IsNullOrWhiteSpace(response.Error.Message))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization-create failure is missing bounded error state.");
        }
    }

    private static void RequireInput(
        string? value,
        int minimum,
        int maximum,
        string label)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Trim().Length < minimum ||
            value.Trim().Length > maximum ||
            value.Any(character => character < 32))
        {
            throw new ArgumentException(
                $"Organization {label} is invalid.");
        }
    }

    private static void RequireOptionalInput(
        string? value,
        int maximum,
        string label)
    {
        if (value is null)
        {
            return;
        }

        if (value.Trim().Length > maximum ||
            value.Any(character => character < 32))
        {
            throw new ArgumentException(
                $"Organization {label} is invalid.");
        }
    }

    private static void RequireContractMemberManagementHandle(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                value,
                "^bke-org-member-v1_[0-9a-f]{64}$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization member management handle drifted.");
        }
    }

    private static void RequireMemberManagementHandle(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                value,
                "^bke-org-member-v1_[0-9a-f]{64}$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant))
        {
            throw new ArgumentException(
                "Organization member management handle is invalid.");
        }
    }

    private static void RequireContractInvitationManagementHandle(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                value,
                "^bke-org-invite-v1_[0-9a-f]{64}$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent organization invitation management handle drifted.");
        }
    }

    private static void RequireInvitationManagementHandle(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !System.Text.RegularExpressions.Regex.IsMatch(
                value,
                "^bke-org-invite-v1_[0-9a-f]{64}$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant))
        {
            throw new ArgumentException(
                "Organization invitation management handle is invalid.");
        }
    }

    private static void RequireMemberEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Length > 320 ||
            !System.Net.Mail.MailAddress.TryCreate(
                value.Trim(),
                out var parsed) ||
            !string.Equals(
                parsed.Address,
                value.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Organization invitation email is invalid.");
        }
    }

    private static void RequireContractEmail(
        string? value,
        string label)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Length > 320 ||
            !System.Net.Mail.MailAddress.TryCreate(
                value.Trim(),
                out var parsed) ||
            !string.Equals(
                parsed.Address,
                value.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                $"Organization {label} drifted.");
        }
    }

    private static void RequireEmail(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Length > 320 ||
            !System.Net.Mail.MailAddress.TryCreate(
                value.Trim(),
                out var parsed) ||
            !string.Equals(
                parsed.Address,
                value.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Organization billing email is invalid.");
        }
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

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
