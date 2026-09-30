using System.Reflection;
using System.Text.Json;
using BKE.Launcher.AgentClient;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;
using BKE.Launcher.Infrastructure;
using BKE.Launcher.PluginHost;
using BKE.Launcher.Presentation;

var agentBase = new Uri(AgentLocalContract.DefaultBaseAddress, UriKind.Absolute);
Require(agentBase.IsLoopback, "Agent default address is not loopback.");
Require(agentBase.Scheme == Uri.UriSchemeHttp, "Agent default address must use local HTTP.");

Require(AgentLocalContract.PlatformAuthorityPath == "/v1/runtime/platform-authority", "platform-authority path drifted");
Require(AgentLocalContract.PlatformAuthorityCapabilityId == "bke.platform-authority", "platform-authority capability id drifted");
Require(AgentLocalContract.PlatformAuthorityContractVersion == 1, "platform-authority contract version drifted");
Require(AgentLocalContract.AccountSessionDeviceContextPath == "/v1/account-session/device-context", "account-session device-context path drifted");
Require(AgentLocalContract.AccountSessionCompletePath == "/v1/account-session/complete", "account-session complete path drifted");
Require(AgentLocalContract.AccountSessionStartPath == "/v1/account-session/start", "account-session start path drifted");
Require(AgentLocalContract.AccountSessionStatusPath == "/v1/account-session/status", "account-session status path drifted");
Require(AgentLocalContract.AccountSessionLogoutPath == "/v1/account-session/logout", "account-session logout path drifted");
Require(AgentLocalContract.AccountPasswordChangePath == "/v1/account/password-change", "account password-change path drifted");
Require(AgentLocalContract.AccountPasswordChangeCapabilityId == "bke.account-password-change", "account password-change capability id drifted");
Require(AgentLocalContract.AccountPasswordChangeContractVersion == 1, "account password-change contract version drifted");
Require(AgentLocalContract.AccountMfaStatusPath == "/v1/account/mfa/status", "account MFA status path drifted");
Require(AgentLocalContract.AccountMfaEnrollStartPath == "/v1/account/mfa/enroll/start", "account MFA enroll-start path drifted");
Require(AgentLocalContract.AccountMfaEnrollCompletePath == "/v1/account/mfa/enroll/complete", "account MFA enroll-complete path drifted");
Require(AgentLocalContract.AccountMfaChallengePath == "/v1/account/mfa/challenge", "account MFA challenge path drifted");
Require(AgentLocalContract.AccountMfaDisablePath == "/v1/account/mfa/disable", "account MFA disable path drifted");
Require(AgentLocalContract.AccountMfaRecoveryRegeneratePath == "/v1/account/mfa/recovery/regenerate", "account MFA recovery path drifted");
Require(AgentLocalContract.AccountMfaCapabilityId == "bke.account-mfa", "account MFA capability id drifted");
Require(AgentLocalContract.AccountMfaContractVersion == 1, "account MFA contract version drifted");
Require(AgentLocalContract.AccountPrivacyListPath == "/v1/account/privacy/requests/list", "account privacy list path drifted");
Require(AgentLocalContract.AccountPrivacyCreatePath == "/v1/account/privacy/requests/create", "account privacy create path drifted");
Require(AgentLocalContract.AccountPrivacyCapabilityId == "bke.account-privacy", "account privacy capability id drifted");
Require(AgentLocalContract.AccountPrivacyContractVersion == 1, "account privacy contract version drifted");
Require(BkePlatformContract.NativeMfaVerifyPath == "/api/agent-sessions/native/mfa/verify", "native MFA verify path drifted");
Require(AgentLocalContract.SoftwareCatalogPath == "/v1/software/catalog", "software catalog path drifted");
Require(AgentLocalContract.SoftwareCatalogCapabilityId == "bke.software-catalog", "software catalog capability id drifted");
Require(AgentLocalContract.SoftwareCatalogContractVersion == 1, "software catalog contract version drifted");
Require(AgentLocalContract.SoftwareInstallPath == "/v1/software/install", "software install path drifted");
Require(AgentLocalContract.SoftwareInstallCapabilityId == "bke.software-install", "software install capability id drifted");
Require(AgentLocalContract.SoftwareInstallContractVersion == 1, "software install contract version drifted");
Require(AgentLocalContract.SoftwareUpdatePath == "/v1/software/update", "software update path drifted");
Require(AgentLocalContract.SoftwareUpdateCapabilityId == "bke.software-update", "software update capability id drifted");
Require(AgentLocalContract.SoftwareUpdateContractVersion == 1, "software update contract version drifted");
Require(AgentLocalContract.SoftwareRepairPath == "/v1/software/repair", "software Repair path drifted");
Require(AgentLocalContract.SoftwareRepairCapabilityId == "bke.software-repair", "software Repair capability id drifted");
Require(AgentLocalContract.SoftwareRepairContractVersion == 1, "software Repair contract version drifted");
Require(AgentLocalContract.SoftwareOpenPath == "/v1/software/open", "software open path drifted");
Require(AgentLocalContract.SoftwareOpenCapabilityId == "bke.software-open", "software open capability id drifted");
Require(AgentLocalContract.SoftwareOpenContractVersion == 1, "software open contract version drifted");
Require(AgentLocalContract.ClaimCodeRedeemPath == "/v1/claims/redeem", "Claim Code redemption path drifted");
Require(AgentLocalContract.ClaimCodeRedemptionCapabilityId == "bke.claim-code-redemption", "Claim Code redemption capability id drifted");
Require(AgentLocalContract.ClaimCodeRedemptionContractVersion == 1, "Claim Code redemption contract version drifted");
Require(AgentLocalContract.StoreCatalogPath == "/v1/store/catalog", "Store catalog path drifted");
Require(AgentLocalContract.StoreCatalogCapabilityId == "bke.store-catalog", "Store catalog capability id drifted");
Require(AgentLocalContract.StoreCatalogContractVersion == 1, "Store catalog contract version drifted");
Require(AgentLocalContract.StoreCheckoutReviewPath == "/v1/store/checkout-review", "Store checkout-review path drifted");
Require(AgentLocalContract.StoreCheckoutReviewCapabilityId == "bke.store-checkout-review", "Store checkout-review capability id drifted");
Require(AgentLocalContract.StoreCheckoutReviewContractVersion == 1, "Store checkout-review contract version drifted");
Require(AgentLocalContract.StoreCheckoutStartPath == "/v1/store/checkout-start", "Store checkout-start path drifted");
Require(AgentLocalContract.StoreCheckoutStartCapabilityId == "bke.store-checkout-start", "Store checkout-start capability id drifted");
Require(AgentLocalContract.StoreCheckoutStartContractVersion == 1, "Store checkout-start contract version drifted");
Require(AgentLocalContract.StoreCheckoutStatusPath == "/v1/store/checkout-status", "Store checkout-status path drifted");
Require(AgentLocalContract.StoreCheckoutStatusCapabilityId == "bke.store-checkout-status", "Store checkout-status capability id drifted");
Require(AgentLocalContract.StoreCheckoutStatusContractVersion == 1, "Store checkout-status contract version drifted");

Require(ProductExecutionTypeWire.ToWireValue(ProductExecutionType.LauncherPlugin) == "LAUNCHER_PLUGIN", "launcher plugin execution type drifted");
Require(ProductExecutionTypeWire.ToWireValue(ProductExecutionType.Standalone) == "STANDALONE", "standalone execution type drifted");

var localResponseProperties = typeof(PlatformAuthorityResponse).GetProperties()
    .Concat(typeof(PlatformAuthorityError).GetProperties())
    .Concat(typeof(AccountSessionDeviceContextResponse).GetProperties())
    .Concat(typeof(AccountSessionCompleteResponse).GetProperties())
    .Concat(typeof(AccountSessionStartResponse).GetProperties())
    .Concat(typeof(AccountSessionStatusResponse).GetProperties())
    .Concat(typeof(AccountSessionLogoutResponse).GetProperties())
    .Concat(typeof(AccountPasswordChangeResponse).GetProperties())
    .Concat(typeof(AccountPasswordChangeError).GetProperties())
    .Concat(typeof(AccountMfaStatusResponse).GetProperties())
    .Concat(typeof(AccountMfaChallengeResponse).GetProperties())
    .Concat(typeof(AccountMfaMutationResponse).GetProperties())
    .Concat(typeof(AccountMfaError).GetProperties())
    .Concat(typeof(AccountPrivacyListResponse).GetProperties())
    .Concat(typeof(AccountPrivacyCreateResponse).GetProperties())
    .Concat(typeof(AccountPrivacyItem).GetProperties())
    .Concat(typeof(AccountPrivacyError).GetProperties())
    .Concat(typeof(AccountOrganizationOverviewResponse).GetProperties())
    .Concat(typeof(AccountOrganizationCreateResponse).GetProperties())
    .Concat(typeof(AccountOrganizationProfileUpdateResponse).GetProperties())
    .Concat(typeof(AccountOrganizationInvitationCreateResponse).GetProperties())
    .Concat(typeof(AccountOrganizationInvitationIssued).GetProperties())
    .Concat(typeof(AccountOrganizationAccount).GetProperties())
    .Concat(typeof(AccountOrganizationPermissions).GetProperties())
    .Concat(typeof(AccountOrganizationProfile).GetProperties())
    .Concat(typeof(AccountOrganizationCounts).GetProperties())
    .Concat(typeof(AccountOrganizationMember).GetProperties())
    .Concat(typeof(AccountOrganizationInvitation).GetProperties())
    .Concat(typeof(AccountOrganizationError).GetProperties())
    .Concat(typeof(SoftwareCatalogResponse).GetProperties())
    .Concat(typeof(SoftwareCatalogItem).GetProperties())
    .Concat(typeof(SoftwareInstallResponse).GetProperties())
    .Concat(typeof(SoftwareInstallError).GetProperties())
    .Concat(typeof(SoftwareUpdateResponse).GetProperties())
    .Concat(typeof(SoftwareUpdateError).GetProperties())
    .Concat(typeof(SoftwareRepairResponse).GetProperties())
    .Concat(typeof(SoftwareRepairError).GetProperties())
    .Concat(typeof(SoftwareOpenResponse).GetProperties())
    .Concat(typeof(SoftwareOpenError).GetProperties())
    .Concat(typeof(SoftwareRemoveResponse).GetProperties())
    .Concat(typeof(SoftwareRemoveError).GetProperties())
    .Concat(typeof(ClaimCodeRedeemResponse).GetProperties())
    .Concat(typeof(ClaimCodeRedeemError).GetProperties())
    .Concat(typeof(StoreCatalogResponse).GetProperties())
    .Concat(typeof(StoreCatalogProduct).GetProperties())
    .Concat(typeof(StoreCatalogEdition).GetProperties())
    .Concat(typeof(StoreCatalogPlan).GetProperties())
    .Concat(typeof(StoreCatalogError).GetProperties())
    .Concat(typeof(StoreCheckoutReviewResponse).GetProperties())
    .Concat(typeof(StoreCheckoutReviewProduct).GetProperties())
    .Concat(typeof(StoreCheckoutReviewEdition).GetProperties())
    .Concat(typeof(StoreCheckoutReviewLegalDocument).GetProperties())
    .Concat(typeof(StoreCheckoutReviewPendingLegalDocument).GetProperties())
    .Concat(typeof(StoreCheckoutReviewError).GetProperties())
    .Concat(typeof(StoreCheckoutStartResponse).GetProperties())
    .Concat(typeof(StoreCheckoutStartError).GetProperties())
    .Concat(typeof(StoreCheckoutStatusResponse).GetProperties())
    .Concat(typeof(StoreCheckoutStatusError).GetProperties())
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);

Require(!localResponseProperties.Contains("AccessToken"), "Launcher local response contract exposes access tokens.");
Require(!localResponseProperties.Contains("RefreshToken"), "Launcher local response contract exposes refresh tokens.");
Require(!localResponseProperties.Contains("DeviceCode"), "Launcher local response contract exposes the secret device code.");
Require(!localResponseProperties.Contains("HandoffCode"), "Launcher local response contract reflects native handoff secret.");
Require(!localResponseProperties.Contains("Password"), "Launcher Agent response contract exposes password material.");
Require(!localResponseProperties.Any(name => name.Contains("DownloadUrl", StringComparison.OrdinalIgnoreCase)), "Launcher catalog exposes download URLs.");
Require(!localResponseProperties.Any(name => name.Contains("SigningKey", StringComparison.OrdinalIgnoreCase)), "Launcher catalog exposes signing keys.");
Require(!localResponseProperties.Any(name => name.Contains("InstallPath", StringComparison.OrdinalIgnoreCase)), "Launcher local contracts expose privileged install paths.");
Require(!localResponseProperties.Any(name => name.Contains("Repository", StringComparison.OrdinalIgnoreCase)), "Launcher local contracts expose GitHub repository identity.");
Require(!localResponseProperties.Any(name => name.Contains("TargetPolicy", StringComparison.OrdinalIgnoreCase)), "Launcher local contracts expose target policy material.");
Require(!localResponseProperties.Any(name => name.Contains("EntryPoint", StringComparison.OrdinalIgnoreCase)), "Launcher local contracts expose product entry points.");
Require(!localResponseProperties.Any(name => name.Contains("Executable", StringComparison.OrdinalIgnoreCase)), "Launcher local contracts expose executable paths.");

var agentMethods = typeof(ILauncherAgentClient)
    .GetMethods()
    .Select(method => method.Name)
    .ToHashSet(StringComparer.Ordinal);

Require(agentMethods.SetEquals([
    "GetPlatformAuthorityAsync",
    "GetAccountSessionDeviceContextAsync",
    "CompleteAccountSessionAsync",
    "StartAccountSessionAsync",
    "GetAccountSessionStatusAsync",
    "LogoutAccountSessionAsync",
    "ChangeAccountPasswordAsync",
    "GetAccountMfaStatusAsync",
    "StartAccountMfaEnrollmentAsync",
    "CompleteAccountMfaEnrollmentAsync",
    "StartAccountMfaProofAsync",
    "DisableAccountMfaAsync",
    "RegenerateAccountMfaRecoveryAsync",
    "GetAccountPrivacyRequestsAsync",
    "CreateAccountPrivacyRequestAsync",
    "GetAccountOrganizationAsync",
    "CreateAccountOrganizationAsync",
    "UpdateAccountOrganizationProfileAsync",
    "CreateAccountOrganizationInvitationAsync",
    "ManageAccountOrganizationInvitationAsync",
    "ManageAccountOrganizationMemberAsync",
    "LeaveAccountOrganizationAsync",
    "GetAccountNotificationsAsync",
    "MutateAccountNotificationAsync",
    "RedeemClaimCodeAsync",
    "GetStoreCatalogAsync",
    "ReviewStoreCheckoutAsync",
    "StartStoreCheckoutAsync",
    "CheckStoreCheckoutStatusAsync",
    "RevealStoreGiftClaimCodeAsync",
    "GetSoftwareCatalogAsync",
    "InstallSoftwareAsync",
    "UpdateSoftwareAsync",
    "RepairSoftwareAsync",
    "OpenSoftwareAsync",
    "RemoveSoftwareAsync"
]), "Launcher Agent client port drifted.");

var platformAuthorityRequestProperties = typeof(PlatformAuthorityRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    platformAuthorityRequestProperties.SequenceEqual(["CorrelationId"]),
    "Launcher widened the Agent platform-authority request.");
Require(
    typeof(PlatformAuthorityResponse).GetProperties().All(property =>
        !property.Name.Contains("Token", StringComparison.OrdinalIgnoreCase) &&
        !property.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) &&
        !property.Name.Contains("Handoff", StringComparison.OrdinalIgnoreCase)),
    "Launcher platform-authority response exposes secret material.");

var completeRequestProperties = typeof(AccountSessionCompleteRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    completeRequestProperties.SequenceEqual(["CorrelationId", "HandoffCode"]),
    "Launcher widened the Agent native-complete request.");
Require(
    !completeRequestProperties.Any(name =>
        name.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Email", StringComparison.OrdinalIgnoreCase)),
    "Launcher Agent native-complete request contains credentials.");

var passwordChangeRequestProperties = typeof(AccountPasswordChangeRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    passwordChangeRequestProperties.SequenceEqual([
        "CorrelationId",
        "CurrentPassword",
        "NewPassword"
    ]),
    "Launcher widened the Agent password-change request.");

using (var passwordChangeRequestDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new AccountPasswordChangeRequest(
            "cert-password-correlation",
            "current-password-cert",
            "new-password-cert"))))
{
    var passwordChangeWireFields = passwordChangeRequestDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        passwordChangeWireFields.SequenceEqual([
            "correlation_id",
            "current_password",
            "new_password"
        ]),
        "Launcher password-change wire request drifted.");
}

Require(
    typeof(AccountPasswordChangeResponse)
        .GetProperties()
        .All(property =>
            !property.Name.Contains("CurrentPassword", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("NewPassword", StringComparison.OrdinalIgnoreCase)),
    "Launcher password-change response reflects credential material.");


var accountMfaMutationRequestProperties = typeof(AccountMfaMutationRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    accountMfaMutationRequestProperties.SequenceEqual([
        "CorrelationId",
        "CurrentPassword",
        "ChallengeToken",
        "Code"
    ]),
    "Launcher widened the Agent account MFA mutation request.");
Require(
    typeof(AccountMfaMutationResponse)
        .GetProperties()
        .All(property =>
            !property.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("AccessToken", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("RefreshToken", StringComparison.OrdinalIgnoreCase)),
    "Launcher account MFA response exposes durable session or password material.");


var privacyListRequestProperties = typeof(AccountPrivacyListRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    privacyListRequestProperties.SequenceEqual([
        "CorrelationId",
        "Limit"
    ]),
    "Launcher widened the Agent account privacy list request.");

var privacyCreateRequestProperties = typeof(AccountPrivacyCreateRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    privacyCreateRequestProperties.SequenceEqual([
        "CorrelationId",
        "RequestType",
        "Summary"
    ]),
    "Launcher widened the Agent account privacy create request.");

Require(
    typeof(AccountPrivacyListResponse)
        .GetProperties()
        .Concat(typeof(AccountPrivacyCreateResponse).GetProperties())
        .Concat(typeof(AccountPrivacyItem).GetProperties())
        .All(property =>
            !property.Name.Contains("AccessToken", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("RefreshToken", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Handoff", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Equals("AccountId", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Equals("UserId", StringComparison.OrdinalIgnoreCase)),
    "Launcher account privacy response exposes cloud/session authority material.");

using (var privacyCreateDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new AccountPrivacyCreateRequest(
            "privacy-cert-correlation",
            "EXPORT",
            "Please export the data associated with this account."))))
{
    var fields = privacyCreateDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        fields.SequenceEqual([
            "correlation_id",
            "request_type",
            "summary"
        ]),
        "Launcher account privacy create wire request widened.");
}

Require(
    File.ReadAllText(
        Path.Combine("eng", "licensing-agent-source.sha")).Trim() ==
        "095c47e40b9b180525294ef37e4a9a5e24a5c1bd",
    "Launcher is not pinned to the merged Agent organization-self-leave authority.");

Require(
    AgentLocalContract.AccountOrganizationOverviewPath ==
        "/v1/account/organization" &&
    AgentLocalContract.AccountOrganizationCreatePath ==
        "/v1/account/organization/create" &&
    AgentLocalContract.AccountOrganizationProfileUpdatePath ==
        "/v1/account/organization/profile" &&
    AgentLocalContract.AccountOrganizationInvitationCreatePath ==
        "/v1/account/organization/invitations/create" &&
    AgentLocalContract.AccountOrganizationInvitationManagePath ==
        "/v1/account/organization/invitations/manage" &&
    AgentLocalContract.AccountOrganizationMemberManagePath ==
        "/v1/account/organization/members/manage" &&
    AgentLocalContract.AccountOrganizationLeavePath ==
        "/v1/account/organization/leave" &&
    AgentLocalContract.AccountOrganizationCapabilityId ==
        "bke.account-organization" &&
    AgentLocalContract.AccountOrganizationContractVersion == 1,
    "Launcher organization Agent contract drifted.");

Require(
    typeof(AccountOrganizationOverviewRequest)
        .GetProperties()
        .Select(property => property.Name)
        .SequenceEqual(["CorrelationId"]),
    "Launcher widened the Agent organization request.");

Require(
    typeof(AccountOrganizationCreateRequest)
        .GetProperties()
        .Select(property => property.Name)
        .SequenceEqual([
            "CorrelationId",
            "DisplayName",
            "LegalName",
            "BillingEmail",
            "RegistrationNumber",
            "TaxId"
        ]),
    "Launcher widened the Agent organization-create request.");

using (var organizationCreateDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new AccountOrganizationCreateRequest(
            "organization-create-cert",
            "Certification Org",
            "Certification Organization Legal",
            "billing@example.test",
            "REG-001",
            "TAX-001"))))
{
    var fields = organizationCreateDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        fields.SequenceEqual([
            "correlation_id",
            "display_name",
            "legal_name",
            "billing_email",
            "registration_number",
            "tax_id"
        ]),
        "Launcher organization-create wire request drifted.");
}

Require(
    typeof(AccountOrganizationProfileUpdateRequest)
        .GetProperties()
        .Select(property => property.Name)
        .SequenceEqual([
            "CorrelationId",
            "UpdateOrganizationProfile",
            "DisplayName",
            "LegalName",
            "RegistrationNumber",
            "UpdateBillingProfile",
            "BillingEmail",
            "TaxId"
        ]),
    "Launcher widened the Agent organization-profile request.");

using (var organizationProfileDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new AccountOrganizationProfileUpdateRequest(
            "organization-profile-cert",
            true,
            "Updated Certification Org",
            "Updated Certification Organization Legal",
            null,
            false,
            null,
            null))))
{
    var root = organizationProfileDocument.RootElement;
    Require(
        root.GetProperty("update_organization_profile").GetBoolean() &&
        !root.GetProperty("update_billing_profile").GetBoolean() &&
        root.GetProperty("display_name").GetString() ==
            "Updated Certification Org" &&
        root.GetProperty("registration_number").ValueKind ==
            JsonValueKind.Null &&
        !root.TryGetProperty("billing_email", out _) &&
        root.GetProperty("tax_id").ValueKind == JsonValueKind.Null &&
        !root.TryGetProperty("account_id", out _) &&
        !root.TryGetProperty("user_id", out _) &&
        !root.TryGetProperty("owner_id", out _),
        "Launcher organization-profile wire request widened or hid disabled-group authority.");
}

Require(
    typeof(AccountOrganizationInvitationCreateRequest)
        .GetProperties()
        .Select(property => property.Name)
        .SequenceEqual([
            "CorrelationId",
            "Email",
            "Role"
        ]),
    "Launcher widened the Agent organization-invitation request.");

using (var organizationInvitationDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new AccountOrganizationInvitationCreateRequest(
            "organization-invitation-cert",
            "new-member@example.test",
            "MEMBER"))))
{
    var root = organizationInvitationDocument.RootElement;
    var fields = root.EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        fields.SequenceEqual([
            "correlation_id",
            "email",
            "role"
        ]) &&
        root.GetProperty("email").GetString() ==
            "new-member@example.test" &&
        root.GetProperty("role").GetString() == "MEMBER" &&
        !root.TryGetProperty("account_id", out _) &&
        !root.TryGetProperty("user_id", out _) &&
        !root.TryGetProperty("owner_id", out _) &&
        !root.TryGetProperty("invitation_id", out _),
        "Launcher organization-invitation wire request widened.");
}

Require(
    typeof(AccountOrganizationInvitationManageRequest)
        .GetProperties()
        .Select(property => property.Name)
        .SequenceEqual([
            "CorrelationId",
            "Action",
            "ManagementHandle"
        ]),
    "Launcher widened the Agent organization-invitation-management request.");

using (var organizationInvitationManageDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new AccountOrganizationInvitationManageRequest(
            "organization-invitation-manage-cert",
            "RESEND",
            "bke-org-invite-v1_" + new string('b', 64)))))
{
    var root = organizationInvitationManageDocument.RootElement;
    var fields = root.EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        fields.SequenceEqual([
            "correlation_id",
            "action",
            "management_handle"
        ]) &&
        root.GetProperty("action").GetString() == "RESEND" &&
        root.GetProperty("management_handle").GetString() ==
            "bke-org-invite-v1_" + new string('b', 64) &&
        !root.TryGetProperty("account_id", out _) &&
        !root.TryGetProperty("user_id", out _) &&
        !root.TryGetProperty("owner_id", out _) &&
        !root.TryGetProperty("invitation_id", out _),
        "Launcher organization-invitation-management wire request widened.");
}

Require(
    typeof(AccountOrganizationMemberManageRequest)
        .GetProperties()
        .Select(property => property.Name)
        .SequenceEqual([
            "CorrelationId",
            "Action",
            "ManagementHandle",
            "Role"
        ]),
    "Launcher widened the Agent organization-member-management request.");

using (var organizationMemberManageDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new AccountOrganizationMemberManageRequest(
            "organization-member-manage-cert",
            "UPDATE_ROLE",
            "bke-org-member-v1_" + new string('c', 64),
            "BILLING"))))
{
    var root = organizationMemberManageDocument.RootElement;
    var fields = root.EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        fields.SequenceEqual([
            "correlation_id",
            "action",
            "management_handle",
            "role"
        ]) &&
        root.GetProperty("action").GetString() == "UPDATE_ROLE" &&
        root.GetProperty("management_handle").GetString() ==
            "bke-org-member-v1_" + new string('c', 64) &&
        root.GetProperty("role").GetString() == "BILLING" &&
        !root.TryGetProperty("account_id", out _) &&
        !root.TryGetProperty("user_id", out _) &&
        !root.TryGetProperty("member_id", out _) &&
        !root.TryGetProperty("membership_id", out _) &&
        !root.TryGetProperty("owner_id", out _),
        "Launcher organization-member-management wire request widened.");
}

using (var organizationMemberRemoveDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new AccountOrganizationMemberManageRequest(
            "organization-member-remove-cert",
            "REMOVE",
            "bke-org-member-v1_" + new string('c', 64),
            null))))
{
    var root = organizationMemberRemoveDocument.RootElement;
    Require(
        root.GetProperty("action").GetString() == "REMOVE" &&
        !root.TryGetProperty("role", out _),
        "Launcher organization-member removal leaked a role payload.");
}

Require(
    typeof(AccountOrganizationLeaveRequest)
        .GetProperties()
        .Select(property => property.Name)
        .SequenceEqual(["CorrelationId"]),
    "Launcher widened the Agent organization self-leave request.");

using (var organizationLeaveDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new AccountOrganizationLeaveRequest(
            "organization-leave-cert"))))
{
    var root = organizationLeaveDocument.RootElement;
    var fields = root.EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        fields.SequenceEqual(["correlation_id"]) &&
        !root.TryGetProperty("account_id", out _) &&
        !root.TryGetProperty("user_id", out _) &&
        !root.TryGetProperty("member_id", out _) &&
        !root.TryGetProperty("membership_id", out _) &&
        !root.TryGetProperty("owner_id", out _) &&
        !root.TryGetProperty("management_handle", out _),
        "Launcher organization self-leave wire request widened beyond correlation-only intent.");
}

foreach (var type in new[]
{
    typeof(AccountOrganizationOverviewResponse),
    typeof(AccountOrganizationCreateResponse),
    typeof(AccountOrganizationProfileUpdateResponse),
    typeof(AccountOrganizationInvitationCreateResponse),
    typeof(AccountOrganizationInvitationManageResponse),
    typeof(AccountOrganizationMemberManageResponse),
    typeof(AccountOrganizationLeaveResponse),
    typeof(AccountOrganizationInvitationIssued),
    typeof(AccountOrganizationAccount),
    typeof(AccountOrganizationMember),
    typeof(AccountOrganizationInvitation),
})
{
    Require(
        type.GetProperties().All(property =>
            !property.Name.Contains(
                "AccessToken",
                StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains(
                "RefreshToken",
                StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains(
                "Handoff",
                StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Equals(
                "AccountId",
                StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Equals(
                "UserId",
                StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Equals(
                "OwnerId",
                StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Equals(
                "InvitationId",
                StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Equals(
                "MemberId",
                StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Equals(
                "MembershipId",
                StringComparison.OrdinalIgnoreCase)),
        $"Launcher organization contract {type.Name} exposes authority/mutation identifiers.");
}

var organizationControllerSource = File.ReadAllText(
    Path.Combine(
        "src",
        "BKE.Launcher.Application",
        "LauncherAccountOrganizationController.cs"));
var organizationClientSource = File.ReadAllText(
    Path.Combine(
        "src",
        "BKE.Launcher.AgentClient",
        "AgentLoopbackClient.cs"));
Require(
    !organizationControllerSource.Contains(
        "/api/agent-sessions/account/organization",
        StringComparison.OrdinalIgnoreCase) &&
    !organizationClientSource.Contains(
        "/api/agent-sessions/account/organization",
        StringComparison.OrdinalIgnoreCase) &&
    organizationClientSource.Contains(
        "AgentLocalContract.AccountOrganizationOverviewPath",
        StringComparison.Ordinal) &&
    organizationClientSource.Contains(
        "AgentLocalContract.AccountOrganizationCreatePath",
        StringComparison.Ordinal) &&
    organizationClientSource.Contains(
        "AgentLocalContract.AccountOrganizationProfileUpdatePath",
        StringComparison.Ordinal) &&
    organizationClientSource.Contains(
        "AgentLocalContract.AccountOrganizationInvitationCreatePath",
        StringComparison.Ordinal) &&
    organizationClientSource.Contains(
        "AgentLocalContract.AccountOrganizationInvitationManagePath",
        StringComparison.Ordinal) &&
    organizationClientSource.Contains(
        "AgentLocalContract.AccountOrganizationMemberManagePath",
        StringComparison.Ordinal) &&
    organizationClientSource.Contains(
        "AgentLocalContract.AccountOrganizationLeavePath",
        StringComparison.Ordinal),
    "Launcher bypassed the local Agent organization authority.");

Require(
    accountSwitchViewModelSource.Contains(
        "_organizationPermissions?.LeaveOrganization == true",
        StringComparison.Ordinal) &&
    accountSwitchViewModelSource.Contains(
        "Resolve the existing checkout attempt before leaving this Organization.",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "IsVisible=\"{Binding ShowOrganizationLeaveSection}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "IsEnabled=\"{Binding CanLeaveOrganization}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"LeaveAccountOrganization\"",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "ConfirmOrganizationLeaveAsync",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "This action does not delete the Organization.",
        StringComparison.Ordinal),
    "Launcher organization self-leave permission/checkout/confirmation boundary drifted.");

var nativeMfaVerifyRequestProperties = typeof(NativeBkeMfaVerifyRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    nativeMfaVerifyRequestProperties.SequenceEqual([
        "ChallengeToken",
        "Code",
        "CustomerAccountId",
        "DeviceId",
        "DeviceName",
        "Platform",
        "Architecture"
    ]),
    "Launcher native MFA verification request drifted.");

var claimRequestProperties = typeof(ClaimCodeRedeemRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    claimRequestProperties.SequenceEqual(["CorrelationId", "Code"]),
    "Launcher widened the Agent Claim Code redemption request.");

using (var claimRequestDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new ClaimCodeRedeemRequest(
            "cert-claim-correlation",
            "BKE-CLM-AAAAA-BBBBB-CCCCC-DDDDD-EEEEE-FFFFF"))))
{
    var claimRequestWireFields = claimRequestDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        claimRequestWireFields.SequenceEqual(["correlation_id", "code"]),
        "Launcher Claim Code redemption wire request widened.");
}

Require(
    typeof(ClaimCodeRedeemResponse)
        .GetProperties()
        .All(property => !string.Equals(property.Name, "Code", StringComparison.OrdinalIgnoreCase)),
    "Launcher Claim Code response reflects the one-time code.");

var storeRequestProperties = typeof(StoreCatalogRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    storeRequestProperties.SequenceEqual(["CorrelationId"]),
    "Launcher widened the Agent Store request.");

using (var storeRequestDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new StoreCatalogRequest("cert-store-correlation"))))
{
    var storeRequestWireFields = storeRequestDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        storeRequestWireFields.SequenceEqual(["correlation_id"]),
        "Launcher Store wire request widened.");
}

Require(
    typeof(StoreCatalogResponse)
        .GetProperties()
        .All(property =>
            !property.Name.Contains("CheckoutUrl", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("AccountId", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Payment", StringComparison.OrdinalIgnoreCase)),
    "Launcher Store response absorbed cloud checkout/payment/account authority.");

var checkoutReviewRequestProperties = typeof(StoreCheckoutReviewRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    checkoutReviewRequestProperties.SequenceEqual(["CorrelationId", "PurchasePlanId"]),
    "Launcher widened the Agent Store checkout-review request.");

using (var checkoutReviewRequestDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new StoreCheckoutReviewRequest(
            "cert-review-correlation",
            "plan-perpetual"))))
{
    var checkoutReviewWireFields = checkoutReviewRequestDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        checkoutReviewWireFields.SequenceEqual([
            "correlation_id",
            "purchase_plan_id"
        ]),
        "Launcher Store checkout-review wire request widened.");
}

Require(
    typeof(StoreCheckoutReviewResponse)
        .GetProperties()
        .All(property =>
            !property.Name.Contains("CheckoutUrl", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("AccountId", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Payment", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Provider", StringComparison.OrdinalIgnoreCase)),
    "Launcher checkout review absorbed cloud payment/account/provider authority.");


var checkoutStartRequestProperties = typeof(StoreCheckoutStartRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    checkoutStartRequestProperties.SequenceEqual([
        "CorrelationId",
        "PurchasePlanId",
        "PurchaseMode",
        "LegalVersionIds"
    ]),
    "Launcher widened the Agent Store checkout-start request.");

using (var checkoutStartRequestDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new StoreCheckoutStartRequest(
            "cert-checkout-correlation",
            "plan-perpetual",
            "GIFT",
            ["terms-v3", "privacy-v2"]))))
{
    var checkoutStartWireFields = checkoutStartRequestDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        checkoutStartWireFields.SequenceEqual([
            "correlation_id",
            "purchase_plan_id",
            "purchase_mode",
            "legal_version_ids"
        ]),
        "Launcher Store checkout-start wire request widened.");
}

Require(
    typeof(StoreCheckoutStartResponse)
        .GetProperties()
        .All(property =>
            !property.Name.Contains("AccountId", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Payment", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Provider", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Price", StringComparison.OrdinalIgnoreCase)),
    "Launcher checkout-start response absorbed cloud account/payment/provider/pricing authority.");


var checkoutStatusRequestProperties = typeof(StoreCheckoutStatusRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    checkoutStatusRequestProperties.SequenceEqual(["CorrelationId"]),
    "Launcher widened the Agent Store checkout-status request.");

using (var checkoutStatusRequestDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new StoreCheckoutStatusRequest(
            "cert-checkout-status-correlation"))))
{
    var checkoutStatusWireFields = checkoutStatusRequestDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        checkoutStatusWireFields.SequenceEqual(["correlation_id"]),
        "Launcher Store checkout-status wire request widened.");
}

Require(
    typeof(StoreCheckoutStatusResponse)
        .GetProperties()
        .All(property =>
            !property.Name.Contains("AccountId", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Provider", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Payer", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Price", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Amount", StringComparison.OrdinalIgnoreCase)),
    "Launcher checkout-status response absorbed cloud account/provider/pricing authority.");

var updateRequestProperties = typeof(SoftwareUpdateRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    updateRequestProperties.SequenceEqual(["CorrelationId", "ProductId"]),
    "Launcher widened the Agent software-update request.");

using (var updateRequestDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new SoftwareUpdateRequest(
            "cert-update-correlation",
            "bke-cert-product"))))
{
    var updateRequestWireFields = updateRequestDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        updateRequestWireFields.SequenceEqual(["correlation_id", "product_id"]),
        "Launcher software-update wire request widened.");
}


var repairRequestProperties = typeof(SoftwareRepairRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(
    repairRequestProperties.SequenceEqual(["CorrelationId", "ProductId"]),
    "Launcher widened the Agent software-Repair request.");

using (var repairRequestDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new SoftwareRepairRequest(
            "cert-repair-correlation",
            "bke-cert-product"))))
{
    var repairRequestWireFields = repairRequestDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(
        repairRequestWireFields.SequenceEqual(["correlation_id", "product_id"]),
        "Launcher software-Repair wire request widened.");
}

var nativeLoginResponseProperties = typeof(NativeBkeLoginResponse)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(!nativeLoginResponseProperties.Contains("AccessToken"), "Launcher platform response exposes access token.");
Require(!nativeLoginResponseProperties.Contains("RefreshToken"), "Launcher platform response exposes refresh token.");
var nativePasswordResetResponseProperties = typeof(NativeBkePasswordResetResponse)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(nativePasswordResetResponseProperties.SetEquals(["Status", "Error"]),
    "Launcher native password-reset response widened beyond generic status/error.");
Require(!nativePasswordResetResponseProperties.Any(name =>
        name.Contains("Token", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Delivery", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Recipient", StringComparison.OrdinalIgnoreCase)),
    "Launcher native password-reset response exposes reset material or delivery identity.");

var registrationPreflightProperties = typeof(NativeBkeRegistrationPreflightResponse)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(registrationPreflightProperties.SetEquals(["Status", "LegalDocuments", "Error"]),
    "Launcher native registration preflight response drifted.");

var registrationResponseProperties = typeof(NativeBkeRegistrationResponse)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(registrationResponseProperties.SetEquals(["Status", "Error"]),
    "Launcher native registration response widened beyond status/error.");

var registrationVerifyResponseProperties = typeof(NativeBkeEmailVerificationResponse)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(registrationVerifyResponseProperties.SetEquals(["Status", "Error"]),
    "Launcher native registration verification response widened beyond status/error.");

var registrationResendResponseProperties = typeof(NativeBkeVerificationResendResponse)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(registrationResendResponseProperties.SetEquals(["Status", "Error"]),
    "Launcher native verification-resend response widened beyond status/error.");

foreach (var responseType in new[]
{
    typeof(NativeBkeRegistrationResponse),
    typeof(NativeBkeEmailVerificationResponse),
    typeof(NativeBkeVerificationResendResponse),
})
{
    Require(responseType.GetProperties().All(property =>
            !property.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Code", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Token", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Handoff", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Session", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Access", StringComparison.OrdinalIgnoreCase) &&
            !property.Name.Contains("Refresh", StringComparison.OrdinalIgnoreCase)),
        $"Launcher native registration response {responseType.Name} exposes secret/session material.");
}

var registrationRequestProperties = typeof(NativeBkeRegistrationRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(registrationRequestProperties.SequenceEqual([
    "Email",
    "Name",
    "Password",
    "LegalVersionIds"
]), "Launcher native registration request drifted.");

using (var registrationRequestDocument = JsonDocument.Parse(
    JsonSerializer.Serialize(
        new NativeBkeRegistrationRequest(
            "new-customer@example.test",
            "New Customer",
            "transient-registration-password",
            ["terms-current", "privacy-current"]))))
{
    var fields = registrationRequestDocument.RootElement
        .EnumerateObject()
        .Select(property => property.Name)
        .ToArray();
    Require(fields.SequenceEqual([
        "email",
        "name",
        "password",
        "legal_version_ids"
    ]), "Launcher native registration wire request drifted.");
}

var registrationClientMethods = typeof(ILauncherRegistrationClient)
    .GetMethods()
    .Select(method => method.Name)
    .ToHashSet(StringComparer.Ordinal);
Require(registrationClientMethods.SetEquals([
    "GetRegistrationPreflightAsync",
    "RegisterAsync",
    "VerifyEmailAsync",
    "ResendVerificationAsync"
]), "Launcher native registration client port drifted.");

var identityMethods = typeof(ILauncherIdentityClient)
    .GetMethods()
    .Select(method => method.Name)
    .ToHashSet(StringComparer.Ordinal);
Require(identityMethods.SetEquals([
    "LoginAsync",
    "VerifyMfaAsync",
    "RequestPasswordResetAsync"
]), "Launcher identity client port drifted.");
Require(!typeof(BkePlatformContract).GetFields(BindingFlags.Public | BindingFlags.Static)
        .Any(field => field.Name.Contains("DefaultBaseAddress", StringComparison.Ordinal)),
    "Launcher platform contract regained an independent default authority.");
Require(BkePlatformContract.NativeLoginPath == "/api/agent-sessions/native/login", "native login path drifted.");
Require(BkePlatformContract.NativePasswordResetRequestPath == "/api/agent-sessions/native/password-reset/request", "native password-reset request path drifted.");
Require(BkePlatformContract.NativeRegistrationPreflightPath == "/api/agent-sessions/native/registration", "native registration preflight path drifted.");
Require(BkePlatformContract.NativeRegistrationPath == "/api/agent-sessions/native/register", "native registration path drifted.");
Require(BkePlatformContract.NativeEmailVerifyPath == "/api/agent-sessions/native/verify-email", "native registration verify path drifted.");
Require(BkePlatformContract.NativeVerificationResendPath == "/api/agent-sessions/native/verification/resend", "native registration resend path drifted.");
Require(BkePlatformContract.AccountSessionProtocolVersion == "bke.account-session.v1", "account-session protocol version drifted.");

var platformIdentitySource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Infrastructure", "PlatformIdentityClient.cs"));
Require(!platformIdentitySource.Contains(
        "BKE_PLATFORM_BASE_URL",
        StringComparison.Ordinal),
    "Launcher platform identity client still owns machine platform configuration.");
Require(!platformIdentitySource.Contains(
        "jl-bke.com",
        StringComparison.OrdinalIgnoreCase),
    "Launcher platform identity client still owns a production authority fallback.");
Require(platformIdentitySource.Contains(
        "ValidatePlatformBaseAddress(platformBaseAddress);",
        StringComparison.Ordinal),
    "Launcher platform identity client does not validate the Agent-supplied origin.");
Require(platformIdentitySource.Contains(
        "value.Scheme != Uri.UriSchemeHttps",
        StringComparison.Ordinal) &&
    platformIdentitySource.Contains(
        "value.AbsolutePath != \"/\"",
        StringComparison.Ordinal),
    "Launcher platform identity client no longer requires an HTTPS origin.");
Require(platformIdentitySource.Contains(
        "RequestPasswordResetAsync(",
        StringComparison.Ordinal) &&
    platformIdentitySource.Contains(
        "BkePlatformContract.NativePasswordResetRequestPath",
        StringComparison.Ordinal),
    "Launcher platform identity client lacks native password-reset request support.");
Require(platformIdentitySource.Contains(
        "EnsureNativeProtocol(response);",
        StringComparison.Ordinal),
    "Launcher native password-reset request does not verify the DS protocol response.");

Require(platformIdentitySource.Contains(
        "GetRegistrationPreflightAsync(",
        StringComparison.Ordinal) &&
    platformIdentitySource.Contains(
        "BkePlatformContract.NativeRegistrationPreflightPath",
        StringComparison.Ordinal) &&
    platformIdentitySource.Contains(
        "BkePlatformContract.NativeRegistrationPath",
        StringComparison.Ordinal) &&
    platformIdentitySource.Contains(
        "BkePlatformContract.NativeEmailVerifyPath",
        StringComparison.Ordinal) &&
    platformIdentitySource.Contains(
        "BkePlatformContract.NativeVerificationResendPath",
        StringComparison.Ordinal),
    "Launcher platform client lacks the canonical native registration routes.");
Require(platformIdentitySource.Contains(
        "AddNativeHeaders(message);",
        StringComparison.Ordinal) &&
    platformIdentitySource.Contains(
        "EnsureNativeProtocol(response);",
        StringComparison.Ordinal),
    "Launcher native registration client does not enforce the shared protocol boundary.");

var nativeRegistrationControllerSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherNativeRegistrationController.cs"));
Require(nativeRegistrationControllerSource.Contains(
        "await _platformAuthority.ResolveAsync(cancellationToken)",
        StringComparison.Ordinal),
    "Launcher registration does not inherit Digital Solutions authority from the Agent.");
Require(!nativeRegistrationControllerSource.Contains(
        "AgentLoopbackClient",
        StringComparison.Ordinal) &&
    !nativeRegistrationControllerSource.Contains(
        "BKE_PLATFORM_BASE_URL",
        StringComparison.Ordinal) &&
    !nativeRegistrationControllerSource.Contains(
        "jl-bke.com",
        StringComparison.OrdinalIgnoreCase),
    "Launcher registration absorbed independent Agent/cloud authority.");

var platformAuthorityResolverSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherPlatformAuthorityResolver.cs"));
Require(platformAuthorityResolverSource.Contains(
        "await _agent.GetPlatformAuthorityAsync(",
        StringComparison.Ordinal),
    "Launcher platform authority resolver does not inherit authority from the Agent.");
Require(platformAuthorityResolverSource.Contains(
        "response.CapabilityId != AgentLocalContract.PlatformAuthorityCapabilityId",
        StringComparison.Ordinal) &&
    platformAuthorityResolverSource.Contains(
        "response.ContractVersion != AgentLocalContract.PlatformAuthorityContractVersion",
        StringComparison.Ordinal),
    "Launcher platform authority resolver does not verify the Agent capability contract.");
Require(platformAuthorityResolverSource.Contains(
        "authority.Scheme != Uri.UriSchemeHttps",
        StringComparison.Ordinal) &&
    platformAuthorityResolverSource.Contains(
        "authority.AbsolutePath != \"/\"",
        StringComparison.Ordinal),
    "Launcher platform authority resolver does not require an HTTPS origin.");
Require(!platformAuthorityResolverSource.Contains(
        "BKE_PLATFORM_BASE_URL",
        StringComparison.Ordinal) &&
    !platformAuthorityResolverSource.Contains(
        "jl-bke.com",
        StringComparison.OrdinalIgnoreCase),
    "Launcher platform authority resolver regained independent machine authority.");

var nativeSignInControllerSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherNativeSignInController.cs"));
Require(nativeSignInControllerSource.Contains(
        "await _platformAuthority.ResolveAsync(cancellationToken)",
        StringComparison.Ordinal),
    "Native Launcher sign-in does not use the shared Agent authority resolver.");
Require(nativeSignInControllerSource.Contains(
        "await _identity.LoginAsync(\n            platformBaseAddress,",
        StringComparison.Ordinal),
    "Native Launcher sign-in does not bind credential validation to the Agent authority.");

var passwordResetControllerSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherPasswordResetRequestController.cs"));
Require(passwordResetControllerSource.Contains(
        "await _platformAuthority.ResolveAsync(cancellationToken)",
        StringComparison.Ordinal),
    "Launcher password reset does not inherit Digital Solutions authority from the Agent.");
Require(passwordResetControllerSource.Contains(
        "await _identity.RequestPasswordResetAsync(",
        StringComparison.Ordinal),
    "Launcher password reset does not delegate issuance to Digital Solutions.");
Require(!passwordResetControllerSource.Contains(
        "AgentLoopbackClient",
        StringComparison.Ordinal),
    "Launcher password reset incorrectly moved unauthenticated recovery into Agent session custody.");

var mainWindowSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Desktop", "MainWindow.axaml.cs"));
var mainWindowMarkup = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Desktop", "MainWindow.axaml"));
var desktopProjectSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Desktop", "BKE.Launcher.Desktop.csproj"));
Require(mainWindowMarkup.Contains(
        "I have reviewed and accept this exact published version.",
        StringComparison.Ordinal),
    "Launcher Create Account UX does not require explicit exact-version legal acceptance.");
Require(mainWindowMarkup.Contains(
        "Text=\"{Binding ContentMarkdown}\"",
        StringComparison.Ordinal),
    "Launcher Create Account UX does not present authoritative registration legal text.");
Require(mainWindowMarkup.Contains(
        "Click=\"CreateNativeAccount\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"VerifyRegistrationEmail\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"ResendRegistrationVerification\"",
        StringComparison.Ordinal),
    "Launcher Create Account UX lacks registration/verification actions.");

var registrationViewModelSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Presentation", "MainWindowViewModel.cs"));
Require(registrationViewModelSource.Contains(
        "finally\n        {\n            RegistrationPassword = string.Empty;",
        StringComparison.Ordinal),
    "Launcher registration UX does not clear the transient password after registration.");
Require(registrationViewModelSource.Contains(
        "var code = RegistrationCode;\n        RegistrationCode = string.Empty;",
        StringComparison.Ordinal),
    "Launcher registration UX does not clear the transient verification code before submission.");
Require(registrationViewModelSource.Contains(
        "Email = verifiedEmail;",
        StringComparison.Ordinal) &&
    registrationViewModelSource.Contains(
        "_showRegistration = false;",
        StringComparison.Ordinal),
    "Launcher verified-registration UX does not return to the normal sign-in surface.");

Require(desktopProjectSource.Contains("Avalonia\" Version=\"12.1.3\"", StringComparison.Ordinal),
    "Launcher Avalonia package baseline is not 12.1.3.");
Require(!desktopProjectSource.Contains("12.1.1", StringComparison.Ordinal),
    "Launcher still references the superseded Avalonia 12.1.1 baseline.");
Require(mainWindowMarkup.Contains("IsVisible=\"{Binding ShowLoginPage}\"", StringComparison.Ordinal),
    "Launcher does not gate unauthenticated startup on the Login page.");
Require(mainWindowMarkup.Contains("IsVisible=\"{Binding ShowAuthenticatedShell}\"", StringComparison.Ordinal),
    "Launcher does not gate authenticated startup on the BKE shell.");
Require(mainWindowMarkup.Contains("SelectedIndex=\"{Binding SelectedModuleIndex, Mode=TwoWay}\"", StringComparison.Ordinal),
    "Launcher shell does not preserve an explicitly unselected module state.");
Require(mainWindowMarkup.Contains("IsVisible=\"{Binding ShowAccountSurface}\"", StringComparison.Ordinal),
    "Launcher Account surface is not explicitly user-selected.");
Require(
    mainWindowMarkup.Contains(
        "IsVisible=\"{Binding ShowOrganizationSection}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"RefreshAccountOrganization\"",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "RefreshAccountOrganizationAsync",
        StringComparison.Ordinal),
    "Launcher organization Account surface is missing.");
Require(
    mainWindowMarkup.Contains(
        "IsVisible=\"{Binding ShowOrganizationCreateSection}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Text=\"{Binding OrganizationCreateDisplayName, Mode=TwoWay}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "IsEnabled=\"{Binding CanCreateOrganization}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"CreateAccountOrganization\"",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "CreateAccountOrganizationAsync",
        StringComparison.Ordinal),
    "Launcher organization-create Account surface is missing.");
Require(
    mainWindowMarkup.Contains(
        "IsVisible=\"{Binding ShowOrganizationProfileEditor}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "IsVisible=\"{Binding CanEditOrganizationIdentity}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "IsVisible=\"{Binding CanEditOrganizationBilling}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "IsEnabled=\"{Binding CanSaveOrganizationProfile}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"UpdateAccountOrganizationProfile\"",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "UpdateAccountOrganizationProfileAsync",
        StringComparison.Ordinal),
    "Launcher permission-aware organization profile editor is missing.");
Require(
    mainWindowMarkup.Contains(
        "IsVisible=\"{Binding ShowOrganizationInviteSection}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "ItemsSource=\"{Binding OrganizationInvitationRoles}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "IsEnabled=\"{Binding CanInviteOrganizationMember}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Text=\"{Binding OrganizationInvitationCode, Mode=OneWay}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Content=\"I've saved or sent this invitation code\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "IsReadOnly=\"True\"",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "CreateAccountOrganizationInvitationAsync",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "CompleteOrganizationInvitationDelivery",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Content=\"Resend\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Content=\"Revoke\"",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "ResendOrganizationInvitation",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "RevokeOrganizationInvitation",
        StringComparison.Ordinal),
    "Launcher transient Organization invitation-code delivery and management UX is missing.");
Require(
    mainWindowMarkup.Contains(
        "Content=\"License manager\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"SetOrganizationMemberRoleMember\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"SetOrganizationMemberRoleLicenseManager\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"SetOrganizationMemberRoleBilling\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"SetOrganizationMemberRoleOwner\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"RemoveOrganizationMember\"",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "ManageAccountOrganizationMemberAsync",
        StringComparison.Ordinal),
    "Launcher Organization member-management UX is missing.");
Require(mainWindowSource.Contains("await ViewModel.InitializeAsync(CancellationToken.None);", StringComparison.Ordinal),
    "Launcher does not resolve Agent-owned authentication state on startup.");
Require(mainWindowSource.Contains("ViewModel.OpenModuleAsync(", StringComparison.Ordinal),
    "Launcher modules are not opened through explicit user navigation.");
Require(mainWindowSource.Contains("ViewModel.OpenAccountSurface();", StringComparison.Ordinal),
    "Launcher Account surface lacks explicit navigation.");

Require(!mainWindowSource.Contains("Process.Start", StringComparison.Ordinal), "Native sign-in still launches a browser.");
Require(!mainWindowMarkup.Contains("Device code", StringComparison.Ordinal), "Device-code UX remains visible in Launcher.");
Require(mainWindowMarkup.Contains("Sign in with BKE", StringComparison.Ordinal), "Native sign-in action is missing.");
Require(mainWindowMarkup.Contains("Content=\"Forgot password?\"", StringComparison.Ordinal),
    "Native Forgot Password action is missing.");
Require(mainWindowMarkup.Contains("IsEnabled=\"{Binding CanRequestPasswordReset}\"", StringComparison.Ordinal),
    "Native Forgot Password action is not recovery-state-bound.");
Require(mainWindowSource.Contains("RequestPasswordReset", StringComparison.Ordinal),
    "Native Forgot Password click handler is missing.");
Require(mainWindowMarkup.Contains("PasswordChar", StringComparison.Ordinal), "Native password field is not masked.");
Require(mainWindowMarkup.Contains("Text=\"{Binding GiftClaimCode, Mode=OneWay}\"", StringComparison.Ordinal),
    "Launcher Store does not render the recovered gift Claim Code.");
Require(mainWindowMarkup.Contains("IsReadOnly=\"True\"", StringComparison.Ordinal),
    "Launcher gift Claim Code field is not read-only.");
Require(mainWindowMarkup.Contains("Content=\"I've saved this Claim Code\"", StringComparison.Ordinal),
    "Launcher Store lacks explicit gift delivery acknowledgement.");
Require(mainWindowMarkup.Contains("Content=\"Resume original checkout\"", StringComparison.Ordinal),
    "Launcher Store lacks explicit same-correlation resume UX.");
Require(mainWindowSource.Contains("RetryOriginalCheckout", StringComparison.Ordinal),
    "Launcher same-correlation resume handler is missing.");
Require(mainWindowSource.Contains("CompleteGiftClaimDelivery", StringComparison.Ordinal),
    "Launcher gift delivery acknowledgement handler is missing.");
Require(mainWindowMarkup.Contains("Content=\"Update\"", StringComparison.Ordinal), "Software Update action is missing.");
Require(mainWindowMarkup.Contains("IsVisible=\"{Binding CanUpdate}\"", StringComparison.Ordinal), "Software Update visibility is not state-bound.");
Require(mainWindowSource.Contains("UpdateProduct", StringComparison.Ordinal), "Software Update click handler is missing.");
Require(mainWindowMarkup.Contains("Content=\"Repair\"", StringComparison.Ordinal), "Software Repair action is missing.");
Require(mainWindowMarkup.Contains("IsVisible=\"{Binding CanRepair}\"", StringComparison.Ordinal), "Software Repair visibility is not state-bound.");
Require(mainWindowSource.Contains("RepairProduct", StringComparison.Ordinal), "Software Repair click handler is missing.");
Require(mainWindowMarkup.Contains("Content=\"Redeem Claim Code\"", StringComparison.Ordinal), "Claim Code redemption action is missing.");
Require(mainWindowMarkup.Contains("IsEnabled=\"{Binding CanRedeemClaimCode}\"", StringComparison.Ordinal), "Claim Code redemption action is not session-bound.");
Require(mainWindowSource.Contains("RedeemClaimCode", StringComparison.Ordinal), "Claim Code redemption click handler is missing.");
Require(mainWindowMarkup.Contains("Text=\"Security\"", StringComparison.Ordinal),
    "Launcher Account Security section is missing.");
Require(mainWindowMarkup.Contains("Text=\"{Binding CurrentPassword, Mode=TwoWay}\"", StringComparison.Ordinal),
    "Launcher current-password field is missing.");
Require(mainWindowMarkup.Contains("Text=\"{Binding NewPassword, Mode=TwoWay}\"", StringComparison.Ordinal),
    "Launcher new-password field is missing.");
Require(mainWindowMarkup.Contains("Text=\"{Binding ConfirmNewPassword, Mode=TwoWay}\"", StringComparison.Ordinal),
    "Launcher password confirmation field is missing.");
Require(mainWindowMarkup.Contains("Content=\"Change password\"", StringComparison.Ordinal),
    "Launcher Change Password action is missing.");
Require(mainWindowMarkup.Contains("IsEnabled=\"{Binding CanChangePassword}\"", StringComparison.Ordinal),
    "Launcher Change Password action is not state-bound.");
Require(mainWindowSource.Contains("ChangePassword", StringComparison.Ordinal),
    "Launcher Change Password click handler is missing.");
Require(mainWindowMarkup.Contains("Header=\"Notifications\"", StringComparison.Ordinal),
    "BKE Notifications tab is missing.");
Require(mainWindowMarkup.Contains("ItemsSource=\"{Binding Notifications}\"", StringComparison.Ordinal),
    "BKE Notifications are not Agent-projected into the UI.");
Require(mainWindowMarkup.Contains("Content=\"Refresh notifications\"", StringComparison.Ordinal),
    "BKE Notifications refresh action is missing.");
Require(mainWindowMarkup.Contains("IsEnabled=\"{Binding CanRefreshNotifications}\"", StringComparison.Ordinal),
    "BKE Notifications refresh action is not session-bound.");
Require(mainWindowMarkup.Contains("Mark Read and Dismiss are server-authoritative receipt actions.", StringComparison.Ordinal),
    "BKE Notifications UI does not state the server-authoritative receipt boundary.");
Require(mainWindowMarkup.Contains("Content=\"Mark read\"", StringComparison.Ordinal),
    "BKE Notifications Mark Read action is missing.");
Require(mainWindowMarkup.Contains("Content=\"Dismiss\"", StringComparison.Ordinal),
    "BKE Notifications Dismiss action is missing.");
Require(mainWindowSource.Contains("MarkNotificationRead", StringComparison.Ordinal),
    "BKE Notifications Mark Read click handler is missing.");
Require(mainWindowSource.Contains("DismissNotification", StringComparison.Ordinal),
    "BKE Notifications Dismiss click handler is missing.");
Require(mainWindowSource.Contains("RefreshNotifications", StringComparison.Ordinal),
    "BKE Notifications refresh click handler is missing.");
Require(mainWindowMarkup.Contains("Header=\"Store\"", StringComparison.Ordinal), "BKE Store tab is missing.");
Require(mainWindowMarkup.Contains("ItemsSource=\"{Binding StoreProducts}\"", StringComparison.Ordinal), "BKE Store products are not Agent-projected into the UI.");
Require(mainWindowMarkup.Contains("Text=\"{Binding GiftCheckoutLabel}\"", StringComparison.Ordinal), "BKE Store gift availability is not presentation-bound.");
Require(mainWindowSource.Contains("RefreshStore", StringComparison.Ordinal), "BKE Store refresh handler is missing.");
Require(mainWindowMarkup.Contains("Content=\"Review purchase\"", StringComparison.Ordinal), "BKE Store purchase review action is missing.");
Require(mainWindowMarkup.Contains("Text=\"{Binding PurchaseReviewPriceLabel}\"", StringComparison.Ordinal), "BKE Store canonical review price is not presentation-bound.");
Require(mainWindowMarkup.Contains("Text=\"{Binding PurchaseReviewModesLabel}\"", StringComparison.Ordinal), "BKE Store purchase modes are not presentation-bound.");
Require(mainWindowMarkup.Contains("Text=\"{Binding PurchaseReviewLegalLabel}\"", StringComparison.Ordinal), "BKE Store Legal requirements are not presentation-bound.");
Require(mainWindowMarkup.Contains("no order, payment, or checkout has been created", StringComparison.OrdinalIgnoreCase), "BKE Store review does not state its read-only boundary.");
Require(mainWindowSource.Contains("ReviewPurchase", StringComparison.Ordinal), "BKE Store purchase review click handler is missing.");
Require(mainWindowMarkup.Contains("Content=\"Buy for myself\"", StringComparison.Ordinal), "BKE Store SELF purchase action is missing.");
Require(mainWindowMarkup.Contains("Content=\"Buy as gift / Claim Code\"", StringComparison.Ordinal), "BKE Store GIFT purchase action is missing.");
Require(mainWindowMarkup.Contains("ItemsSource=\"{Binding PurchaseLegalDocuments}\"", StringComparison.Ordinal), "BKE Store Legal acceptance list is missing.");
Require(mainWindowMarkup.Contains("IsChecked=\"{Binding IsAccepted, Mode=TwoWay}\"", StringComparison.Ordinal), "BKE Store Legal acceptance is not explicit per document.");
Require(mainWindowSource.Contains("BuyForSelf", StringComparison.Ordinal), "BKE Store SELF purchase handler is missing.");
Require(mainWindowSource.Contains("BuyAsGift", StringComparison.Ordinal), "BKE Store GIFT purchase handler is missing.");
Require(mainWindowMarkup.Contains("Content=\"Check checkout status\"", StringComparison.Ordinal), "BKE Store checkout-status recovery action is missing.");
Require(mainWindowMarkup.Contains("Content=\"Open existing checkout\"", StringComparison.Ordinal), "BKE Store existing-checkout action is missing.");
Require(mainWindowMarkup.Contains("IsEnabled=\"{Binding CanCheckCheckoutStatus}\"", StringComparison.Ordinal), "BKE Store checkout-status action is not recovery-state-bound.");
Require(mainWindowMarkup.Contains("IsEnabled=\"{Binding CanOpenExistingCheckout}\"", StringComparison.Ordinal), "BKE Store existing-checkout action is not URL-state-bound.");
Require(mainWindowMarkup.Contains("IsVisible=\"{Binding ShowPurchaseCheckoutState}\"", StringComparison.Ordinal), "BKE Store checkout recovery is not visible independently of the current purchase review.");
Require(mainWindowSource.Contains("CheckPurchaseCheckoutStatus", StringComparison.Ordinal), "BKE Store checkout-status click handler is missing.");
Require(mainWindowSource.Contains("OpenExistingCheckout", StringComparison.Ordinal), "BKE Store existing-checkout click handler is missing.");
Require(mainWindowSource.Contains("OpenPurchaseLegalDocument", StringComparison.Ordinal), "BKE Store Legal document navigation is missing.");
var viewModelSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Presentation", "MainWindowViewModel.cs"));
var normalizedViewModelSource = viewModelSource.Replace("\r\n", "\n", StringComparison.Ordinal);
Require(normalizedViewModelSource.Contains(
    "await _notifications.GetAsync(",
    StringComparison.Ordinal),
    "Launcher Notifications UX does not delegate feed authority to its Agent-backed service.");
Require(normalizedViewModelSource.Contains(
    "public async Task InitializeAsync(CancellationToken cancellationToken)",
    StringComparison.Ordinal),
    "Launcher lacks explicit startup authentication-state initialization.");
Require(normalizedViewModelSource.Contains(
    "public async Task OpenModuleAsync(",
    StringComparison.Ordinal),
    "Launcher lacks explicit module-open intent.");
Require(normalizedViewModelSource.Contains(
    "SelectedModuleIndex = -1;",
    StringComparison.Ordinal),
    "Launcher does not preserve the naked authenticated shell state.");
Require(normalizedViewModelSource.Contains(
    "ShowAccountSurface = false;",
    StringComparison.Ordinal),
    "Launcher naked shell does not close the Account surface.");

var nativeSignInStart = normalizedViewModelSource.IndexOf(
    "public async Task NativeSignInAsync(",
    StringComparison.Ordinal);
var legacySignInStart = normalizedViewModelSource.IndexOf(
    "public async Task StartSignInAsync(",
    StringComparison.Ordinal);
Require(nativeSignInStart >= 0 && legacySignInStart > nativeSignInStart,
    "Launcher native sign-in method boundaries are unavailable for startup certification.");
var nativeSignInSource = normalizedViewModelSource[
    nativeSignInStart..legacySignInStart];
Require(!nativeSignInSource.Contains(
    "await RefreshCatalogAsync(cancellationToken);",
    StringComparison.Ordinal),
    "Launcher native sign-in still auto-opens/loads My Software.");
Require(!nativeSignInSource.Contains(
    "await RefreshStoreAsync(cancellationToken);",
    StringComparison.Ordinal),
    "Launcher native sign-in still auto-opens/loads Store.");
Require(!nativeSignInSource.Contains(
    "await RefreshNotificationsAsync(cancellationToken);",
    StringComparison.Ordinal),
    "Launcher native sign-in still auto-opens/loads Notifications.");

var refreshStatusStart = normalizedViewModelSource.IndexOf(
    "public async Task RefreshStatusAsync(",
    StringComparison.Ordinal);
var refreshNotificationsStart = normalizedViewModelSource.IndexOf(
    "public async Task RefreshNotificationsAsync(",
    StringComparison.Ordinal);
Require(refreshStatusStart >= 0 && refreshNotificationsStart > refreshStatusStart,
    "Launcher status-refresh method boundaries are unavailable for startup certification.");
var refreshStatusSource = normalizedViewModelSource[
    refreshStatusStart..refreshNotificationsStart];
Require(!refreshStatusSource.Contains(
    "await RefreshCatalogAsync(cancellationToken);",
    StringComparison.Ordinal) &&
    !refreshStatusSource.Contains(
        "await RefreshStoreAsync(cancellationToken);",
        StringComparison.Ordinal) &&
    !refreshStatusSource.Contains(
        "await RefreshNotificationsAsync(cancellationToken);",
        StringComparison.Ordinal),
    "Launcher account-session refresh still auto-loads a customer module.");
Require(normalizedViewModelSource.Contains(
    "await _accountPasswordChange.ChangeAsync(",
    StringComparison.Ordinal),
    "Launcher Account Security UX does not delegate password mutation to the Agent.");
Require(normalizedViewModelSource.Contains(
    "EnterPasswordChangeReauthentication(",
    StringComparison.Ordinal),
    "Launcher lacks fail-closed password-change reauthentication handling.");
Require(normalizedViewModelSource.Contains(
    "ClearPasswordChangeFields();",
    StringComparison.Ordinal),
    "Launcher does not clear transient password-change fields on reauthentication.");
Require(!normalizedViewModelSource.Contains(
    "/api/agent-sessions/account/password-change",
    StringComparison.OrdinalIgnoreCase),
    "Launcher Account Security UX bypasses the Agent loopback boundary.");

Require(normalizedViewModelSource.Contains(
    "await _passwordResetRequest.RequestAsync(",
    StringComparison.Ordinal),
    "Launcher login recovery does not delegate reset issuance to its recovery controller.");
Require(normalizedViewModelSource.Contains(
    "If a BKE account exists for this email",
    StringComparison.Ordinal),
    "Launcher recovery UX lost its enumeration-safe generic confirmation.");
Require(!normalizedViewModelSource.Contains(
    "ResetToken",
    StringComparison.Ordinal) &&
    !normalizedViewModelSource.Contains(
        "reset_token",
        StringComparison.OrdinalIgnoreCase),
    "Launcher presentation absorbed password-reset token material.");

Require(normalizedViewModelSource.Contains(
    "ClearNotifications(",
    StringComparison.Ordinal),
    "Launcher does not clear account notification presentation across session changes.");
Require(!normalizedViewModelSource.Contains(
    "/api/agent-sessions/",
    StringComparison.OrdinalIgnoreCase),
    "Launcher Notifications UX bypasses the Agent loopback boundary.");

Require(normalizedViewModelSource.Contains(
    "await _nativeSignIn.VerifyMfaAsync(",
    StringComparison.Ordinal),
    "Launcher native sign-in UX does not complete customer MFA through the native identity controller.");
Require(normalizedViewModelSource.Contains(
    "await _accountMfa.StatusAsync(",
    StringComparison.Ordinal) &&
    normalizedViewModelSource.Contains(
        "_accountMfa.EnrollCompleteAsync(",
        StringComparison.Ordinal) &&
    normalizedViewModelSource.Contains(
        "_accountMfa.DisableAsync(",
        StringComparison.Ordinal) &&
    normalizedViewModelSource.Contains(
        "_accountMfa.RegenerateRecoveryAsync(",
        StringComparison.Ordinal),
    "Launcher Account Security UX does not delegate MFA authority through the Agent-backed controller.");
Require(!normalizedViewModelSource.Contains(
    "/api/agent-sessions/account/mfa",
    StringComparison.OrdinalIgnoreCase),
    "Launcher presentation bypasses Agent account-MFA mediation.");
Require(normalizedViewModelSource.Contains(
    "AccountMfaRecoveryCodes = string.Empty;",
    StringComparison.Ordinal),
    "Launcher lacks explicit in-memory recovery-code clearing.");
Require(normalizedViewModelSource.Contains(
    "EnterAccountMfaReauthentication(",
    StringComparison.Ordinal),
    "Launcher lacks fail-closed MFA reauthentication handling.");


var accountPrivacyControllerSource = File.ReadAllText(
    Path.Combine(
        "src",
        "BKE.Launcher.Application",
        "LauncherAccountPrivacyController.cs"));
Require(
    accountPrivacyControllerSource.Contains(
        "GetAccountPrivacyRequestsAsync",
        StringComparison.Ordinal) &&
    accountPrivacyControllerSource.Contains(
        "CreateAccountPrivacyRequestAsync",
        StringComparison.Ordinal),
    "Launcher Account Privacy controller does not delegate to the Agent loopback client.");
Require(
    !accountPrivacyControllerSource.Contains(
        "/api/agent-sessions/",
        StringComparison.OrdinalIgnoreCase) &&
    !accountPrivacyControllerSource.Contains(
        "\"ACCESS\"",
        StringComparison.Ordinal) &&
    !accountPrivacyControllerSource.Contains(
        "\"EXPORT\"",
        StringComparison.Ordinal) &&
    !accountPrivacyControllerSource.Contains(
        "\"DELETION\"",
        StringComparison.Ordinal),
    "Launcher Account Privacy controller absorbed cloud routing or canonical request-type policy.");

Require(
    normalizedViewModelSource.Contains(
        "await _accountPrivacy.ListAsync(",
        StringComparison.Ordinal) &&
    normalizedViewModelSource.Contains(
        "await _accountPrivacy.CreateAsync(",
        StringComparison.Ordinal),
    "Launcher Account Privacy UX does not delegate through the Agent-backed controller.");
Require(
    normalizedViewModelSource.Contains(
        "AccountPrivacyRequestTypes.Contains(",
        StringComparison.Ordinal),
    "Launcher Account Privacy create action is not restricted to authoritative Agent-returned request types.");
Require(
    normalizedViewModelSource.Contains(
        "AccountPrivacySummary = string.Empty;",
        StringComparison.Ordinal) &&
    normalizedViewModelSource.Contains(
        "await RefreshAccountPrivacyAsync(cancellationToken);",
        StringComparison.Ordinal),
    "Launcher Account Privacy mutation does not clear transient summary and refresh authoritative state.");
Require(
    !normalizedViewModelSource.Contains(
        "/api/agent-sessions/privacy/requests",
        StringComparison.OrdinalIgnoreCase),
    "Launcher presentation bypasses Agent account-privacy mediation.");
Require(
    mainWindowMarkup.Contains(
        "ItemsSource=\"{Binding AccountPrivacyRequestTypes}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "ItemsSource=\"{Binding AccountPrivacyRequests}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"RefreshAccountPrivacy\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"CreateAccountPrivacyRequest\"",
        StringComparison.Ordinal),
    "Launcher Account Privacy desktop surface is incomplete.");
Require(
    mainWindowSource.Contains(
        "RefreshAccountPrivacy",
        StringComparison.Ordinal) &&
    mainWindowSource.Contains(
        "CreateAccountPrivacyRequest",
        StringComparison.Ordinal),
    "Launcher Account Privacy desktop handlers are missing.");

Require(normalizedViewModelSource.Contains(
    "product.ExecutionType == ProductExecutionType.Standalone &&\n            product.State == LauncherProductState.UpdateAvailable,",
    StringComparison.Ordinal),
    "Launcher Update action is not restricted to STANDALONE + UPDATE_AVAILABLE.");

Require(normalizedViewModelSource.Contains(
    "product.ExecutionType == ProductExecutionType.Standalone &&\n            product.State is LauncherProductState.Installed\n                or LauncherProductState.UpdateAvailable\n                or LauncherProductState.RepairRequired,",
    StringComparison.Ordinal),
    "Launcher Repair action is not restricted to entitled installed standalone states.");
Require(!normalizedViewModelSource.Contains(
    "CanRepair = product.State == LauncherProductState.InstalledNotEntitled",
    StringComparison.Ordinal),
    "Launcher Repair action was exposed for installed-but-not-entitled software.");

Require(normalizedViewModelSource.Contains(
    "ClaimCode = string.Empty;",
    StringComparison.Ordinal),
    "Launcher does not clear the transient Claim Code after successful redemption or logout.");
Require(normalizedViewModelSource.Contains(
    "await _claimCodeRedemption.RedeemAsync(",
    StringComparison.Ordinal),
    "Launcher Claim Code UX does not delegate redemption to the Agent.");
Require(normalizedViewModelSource.Contains(
    "await RefreshCatalogAsync(cancellationToken);",
    StringComparison.Ordinal),
    "Launcher does not refresh My Software after redemption.");

var notificationServiceSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherNotificationInboxService.cs"));
Require(notificationServiceSource.Contains("GetAccountNotificationsAsync", StringComparison.Ordinal),
    "Launcher Notifications does not delegate to the Agent account inbox.");
Require(notificationServiceSource.Contains("AccountNotificationInboxCapabilityId", StringComparison.Ordinal),
    "Launcher Notifications does not verify Agent capability identity.");
Require(notificationServiceSource.Contains("AllowedAudienceKinds", StringComparison.Ordinal),
    "Launcher Notifications lacks audience defense-in-depth.");
Require(notificationServiceSource.Contains("ALL_ACTIVE_CLIENTS", StringComparison.Ordinal),
    "Launcher Notifications active-client audience support drifted.");
Require(!notificationServiceSource.Contains("ADMINISTRATORS", StringComparison.Ordinal),
    "Launcher Notifications widened into administrator audience authority.");
Require(!notificationServiceSource.Contains("/api/agent-sessions/", StringComparison.OrdinalIgnoreCase),
    "Launcher Notifications bypasses the Agent loopback boundary.");
Require(!notificationServiceSource.Contains("account_id", StringComparison.OrdinalIgnoreCase),
    "Launcher Notifications can choose or consume a cloud account identifier.");
Require(!notificationServiceSource.Contains("access_token", StringComparison.OrdinalIgnoreCase) &&
        !notificationServiceSource.Contains("refresh_token", StringComparison.OrdinalIgnoreCase),
    "Launcher Notifications absorbed Agent-owned cloud session secrets.");
Require(notificationServiceSource.Contains("MutateAccountNotificationAsync", StringComparison.Ordinal),
    "Launcher notification receipt actions do not delegate to the Agent.");
Require(notificationServiceSource.Contains("response.State == expectedState", StringComparison.Ordinal),
    "Launcher notification receipt actions do not validate authoritative state.");

var accountNotificationRequestProperties = typeof(AccountNotificationFeedRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(accountNotificationRequestProperties.SequenceEqual(["Limit"]),
    "Launcher account notification request widened beyond limit.");
var accountNotificationResponseProperties = typeof(AccountNotificationFeedResponse)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(!accountNotificationResponseProperties.Contains("AccountId"),
    "Launcher account notification response exposes cloud account authority.");
Require(!accountNotificationResponseProperties.Contains("Data"),
    "Launcher account notification response exposes arbitrary cloud notification data.");

var accountNotificationReceiptRequestProperties = typeof(AccountNotificationReceiptRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();
Require(accountNotificationReceiptRequestProperties.SequenceEqual(["NotificationId", "Action"]),
    "Launcher account notification receipt request widened beyond notification id + action.");
var accountNotificationReceiptResponseProperties = typeof(AccountNotificationReceiptResponse)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(!accountNotificationReceiptResponseProperties.Contains("AccountId"),
    "Launcher account notification receipt response exposes cloud account authority.");
Require(!accountNotificationReceiptResponseProperties.Contains("Data"),
    "Launcher account notification receipt response exposes arbitrary cloud notification data.");
Require(normalizedViewModelSource.Contains(
    "await _notifications.MutateAsync(",
    StringComparison.Ordinal),
    "Launcher notification actions do not delegate through the centralized notification service.");
Require(!normalizedViewModelSource.Contains(
    "Notifications.Remove",
    StringComparison.Ordinal),
    "Launcher notification Dismiss optimistically removes local state.");

var storeServiceSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherStoreService.cs"));
Require(storeServiceSource.Contains("GetStoreCatalogAsync", StringComparison.Ordinal),
    "Launcher Store does not delegate catalog authority to the Agent.");
Require(!storeServiceSource.Contains("/api/agent-sessions/", StringComparison.OrdinalIgnoreCase),
    "Launcher Store bypasses the Agent loopback boundary.");
Require(!storeServiceSource.Contains("PAYMONGO", StringComparison.OrdinalIgnoreCase),
    "Launcher Store absorbed payment-provider authority.");
Require(!storeServiceSource.Contains("checkout_url", StringComparison.OrdinalIgnoreCase),
    "Launcher Store absorbed checkout URL authority.");

var checkoutReviewServiceSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherStoreCheckoutReviewService.cs"));
Require(checkoutReviewServiceSource.Contains("ReviewStoreCheckoutAsync", StringComparison.Ordinal),
    "Launcher Store checkout review does not delegate authority to the Agent.");
Require(!checkoutReviewServiceSource.Contains("/api/agent-sessions/", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout review bypasses the Agent loopback boundary.");
Require(!checkoutReviewServiceSource.Contains("PAYMONGO", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout review absorbed payment-provider authority.");
Require(!checkoutReviewServiceSource.Contains("checkout_url", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout review absorbed checkout URL authority.");
Require(!checkoutReviewServiceSource.Contains("amount_minor =", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout review hardcoded authoritative pricing.");

Require(normalizedViewModelSource.Contains(
    "await _storeCheckoutReview.ReviewAsync(",
    StringComparison.Ordinal),
    "Launcher Store UX does not delegate purchase review to the Agent.");
Require(!normalizedViewModelSource.Contains(
    "PayMongo",
    StringComparison.OrdinalIgnoreCase),
    "Launcher Store UX absorbed payment-provider behavior.");


var checkoutStartServiceSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherStoreCheckoutStartService.cs"));
Require(checkoutStartServiceSource.Contains("StartStoreCheckoutAsync", StringComparison.Ordinal),
    "Launcher Store checkout start does not delegate mutation authority to the Agent.");
Require(!checkoutStartServiceSource.Contains("/api/agent-sessions/", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout start bypasses the Agent loopback boundary.");
Require(!checkoutStartServiceSource.Contains("PAYMONGO", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout start absorbed payment-provider authority.");
Require(!checkoutStartServiceSource.Contains("amount_minor", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout start hardcoded authoritative pricing.");
Require(!checkoutStartServiceSource.Contains("account_id", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout start can choose a cloud destination account.");
Require(!checkoutStartServiceSource.Contains("recipient", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout start introduced gift recipient identity.");


var checkoutStatusServiceSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherStoreCheckoutStatusService.cs"));
Require(checkoutStatusServiceSource.Contains("CheckStoreCheckoutStatusAsync", StringComparison.Ordinal),
    "Launcher Store checkout-status recovery does not delegate read authority to the Agent.");
Require(!checkoutStatusServiceSource.Contains("/api/agent-sessions/", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout-status recovery bypasses the Agent loopback boundary.");
Require(!checkoutStatusServiceSource.Contains("PAYMONGO", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout-status recovery absorbed payment-provider authority.");
Require(!checkoutStatusServiceSource.Contains("account_id", StringComparison.OrdinalIgnoreCase),
    "Launcher Store checkout-status recovery can choose a cloud account.");

var giftClaimRevealServiceSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherStoreGiftClaimRevealService.cs"));
Require(giftClaimRevealServiceSource.Contains("RevealStoreGiftClaimCodeAsync", StringComparison.Ordinal),
    "Launcher Store gift Claim Code delivery does not delegate reveal authority to the Agent.");
Require(!giftClaimRevealServiceSource.Contains("/api/agent-sessions/", StringComparison.OrdinalIgnoreCase),
    "Launcher Store gift Claim Code delivery bypasses the Agent loopback boundary.");
Require(!giftClaimRevealServiceSource.Contains("PAYMONGO", StringComparison.OrdinalIgnoreCase),
    "Launcher Store gift Claim Code delivery absorbed payment-provider authority.");
Require(!giftClaimRevealServiceSource.Contains("recipient", StringComparison.OrdinalIgnoreCase),
    "Launcher Store gift Claim Code delivery introduced recipient identity.");
Require(!giftClaimRevealServiceSource.Contains("File.", StringComparison.Ordinal),
    "Launcher Store gift Claim Code service persists one-time code material.");
Require(giftClaimRevealServiceSource.Contains(
    "response.Status == \"AVAILABLE\"",
    StringComparison.Ordinal),
    "Launcher Store gift Claim Code service does not validate AVAILABLE payloads.");
Require(giftClaimRevealServiceSource.Contains(
    "response.Status != \"AVAILABLE\" &&",
    StringComparison.Ordinal),
    "Launcher Store gift Claim Code service can accept plaintext outside AVAILABLE state.");

Require(normalizedViewModelSource.Contains(
    "await _storeCheckoutStatus.CheckAsync(",
    StringComparison.Ordinal),
    "Launcher Store UX does not recover checkout state through the Agent.");
Require(normalizedViewModelSource.Contains(
    "new LauncherCheckoutRecoveryState(",
    StringComparison.Ordinal),
    "Launcher does not create durable resumable checkout state before mutation.");
Require(normalizedViewModelSource.Contains(
    "_checkoutRecoveryStore.Write(recoveryState);",
    StringComparison.Ordinal),
    "Launcher does not persist resumable checkout intent before mutation.");
var recoveryWriteIndex = normalizedViewModelSource.IndexOf(
    "_checkoutRecoveryStore.Write(recoveryState);",
    StringComparison.Ordinal);
var checkoutMutationIndex = normalizedViewModelSource.IndexOf(
    "await _storeCheckoutStart.StartAsync(",
    StringComparison.Ordinal);
Require(
    recoveryWriteIndex >= 0 &&
    checkoutMutationIndex > recoveryWriteIndex,
    "Launcher can start checkout before durable recovery state is written.");
Require(normalizedViewModelSource.Contains(
    "recoveryState.CorrelationId,",
    StringComparison.Ordinal),
    "Launcher checkout-start does not use the retained recovery correlation.");
Require(normalizedViewModelSource.Contains(
    "public async Task RetryOriginalCheckoutAsync(",
    StringComparison.Ordinal),
    "Launcher lacks an explicit same-correlation checkout resume action.");
Require(normalizedViewModelSource.Contains(
    "PurchaseCheckoutStatus == \"RETRY_AVAILABLE\"",
    StringComparison.Ordinal),
    "Launcher can resume checkout without authoritative NOT_FOUND.");
Require(normalizedViewModelSource.Contains(
    "BKE will not create a new correlation.",
    StringComparison.Ordinal),
    "Launcher resume UX does not preserve the no-new-correlation boundary.");
var checkoutStartCall = "await _storeCheckoutStart.StartAsync(";
Require(
    normalizedViewModelSource.IndexOf(checkoutStartCall, StringComparison.Ordinal) ==
    normalizedViewModelSource.LastIndexOf(checkoutStartCall, StringComparison.Ordinal),
    "Launcher checkout-start authority is duplicated outside the centralized executor.");
Require(normalizedViewModelSource.Contains(
    "Resume is allowed only after authoritative NOT_FOUND.",
    StringComparison.Ordinal),
    "Launcher ambiguous-result UX can replay checkout before a read-only NOT_FOUND.");
Require(normalizedViewModelSource.Contains(
    "Resolve the existing checkout attempt before reviewing or starting another purchase.",
    StringComparison.Ordinal),
    "Launcher can discard an unresolved checkout correlation by re-reviewing.");
Require(normalizedViewModelSource.Contains(
    "RestoreCheckoutRecoveryState();",
    StringComparison.Ordinal),
    "Launcher does not restore unresolved checkout recovery state after restart.");
Require(normalizedViewModelSource.Contains(
    "Opened the existing secure checkout. No new order or payment attempt was created.",
    StringComparison.Ordinal),
    "Launcher existing-checkout resume UX does not state its no-new-mutation boundary.");
Require(!normalizedViewModelSource.Contains(
    "same Agent account session",
    StringComparison.Ordinal),
    "Launcher still claims checkout recovery is bound to one Agent session.");
Require(normalizedViewModelSource.Contains(
    "same BKE identity and account on this device",
    StringComparison.Ordinal),
    "Launcher recovery UX does not state the same-identity/account/device recovery boundary.");
Require(normalizedViewModelSource.Contains(
    "var preserveCheckoutRecovery =",
    StringComparison.Ordinal),
    "Launcher sign-out does not explicitly preserve unresolved checkout recovery.");
Require(normalizedViewModelSource.Contains(
    "Signed out. The existing checkout correlation remains locked.",
    StringComparison.Ordinal),
    "Launcher sign-out can imply that unresolved checkout recovery was discarded.");
Require(normalizedViewModelSource.Contains(
    "GiftClaimCode = string.Empty;\n            Message = preserveCheckoutRecovery",
    StringComparison.Ordinal),
    "Launcher can retain revealed gift Claim Code plaintext after sign-out.");

Require(!normalizedViewModelSource.Contains(
    "GiftClaimCodeDeliverySupported = false",
    StringComparison.Ordinal),
    "Launcher still hard-blocks GIFT after certified Claim Code delivery became available.");
Require(normalizedViewModelSource.Contains(
    "GiftCheckoutEnabled &&",
    StringComparison.Ordinal),
    "Launcher GIFT action is not gated by authoritative Store availability.");
Require(normalizedViewModelSource.Contains(
    "result.FulfillmentMode == \"CLAIM_CODE\"",
    StringComparison.Ordinal),
    "Launcher does not recognize Digital Solutions CLAIM_CODE fulfillment.");
Require(normalizedViewModelSource.Contains(
    "await _storeGiftClaimReveal.RevealAsync(",
    StringComparison.Ordinal),
    "Launcher does not recover the settled gift Claim Code through the Agent.");
Require(normalizedViewModelSource.Contains(
    "\"GIFT_CLAIM_CODE_READY\"",
    StringComparison.Ordinal),
    "Launcher does not expose a distinct recovered gift Claim Code state.");
Require(normalizedViewModelSource.Contains(
    "BKE Launcher does not persist this plaintext code.",
    StringComparison.Ordinal),
    "Launcher gift delivery UX does not state the transient plaintext boundary.");
Require(normalizedViewModelSource.Contains(
    "public void CompleteGiftClaimDelivery()",
    StringComparison.Ordinal),
    "Launcher lacks explicit purchaser acknowledgement before clearing gift recovery.");
Require(normalizedViewModelSource.Contains(
    "A revealed gift Claim Code must remain visible until you save it and acknowledge delivery.",
    StringComparison.Ordinal),
    "Launcher can dismiss gift recovery before explicit purchaser acknowledgement.");
Require(normalizedViewModelSource.Contains(
    "Gift Claim Code delivery acknowledged. Start another purchase only after reviewing the current plan and Legal terms again.",
    StringComparison.Ordinal),
    "Launcher does not force a fresh review after gift delivery acknowledgement.");
Require(normalizedViewModelSource.Contains(
    "This gift fulfillment is terminal. Review the current plan and Legal terms again before another purchase.",
    StringComparison.Ordinal),
    "Launcher terminal gift fulfillment can reuse a stale purchase review.");
Require(!normalizedViewModelSource.Contains(
    "if (TryClearCheckoutRecoveryState())\n                    {\n                        _purchaseAttemptLocked = false;\n                        RaisePurchaseActionState();\n                    }\n                    break;",
    StringComparison.Ordinal),
    "Launcher terminal gift fulfillment unlocks stale purchase review state.");
Require(normalizedViewModelSource.Contains(
    "\"GIFT_FULFILLMENT_PENDING\"",
    StringComparison.Ordinal),
    "Launcher does not preserve retryable settled gift fulfillment while Claim Code materialization is pending.");


var checkoutRecoveryStoreSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Infrastructure", "FileLauncherCheckoutRecoveryStore.cs"));
Require(checkoutRecoveryStoreSource.Contains("LauncherStoragePaths.UserDataRoot()", StringComparison.Ordinal),
    "Launcher checkout recovery state is not stored under the Launcher-owned user-data root.");
Require(checkoutRecoveryStoreSource.Contains("checkout-recovery.json", StringComparison.Ordinal),
    "Launcher resumable checkout recovery store is missing.");
Require(checkoutRecoveryStoreSource.Contains("checkout-recovery.id", StringComparison.Ordinal),
    "Launcher does not preserve legacy correlation-only recovery compatibility.");
Require(checkoutRecoveryStoreSource.Contains("JsonSerializer.Serialize(state)", StringComparison.Ordinal),
    "Launcher does not serialize resumable checkout intent atomically.");
Require(checkoutRecoveryStoreSource.Contains("PurchasePlanId", StringComparison.Ordinal),
    "Launcher recovery state omits the original purchase-plan identity.");
Require(checkoutRecoveryStoreSource.Contains("PurchaseMode", StringComparison.Ordinal),
    "Launcher recovery state omits SELF/GIFT intent.");
Require(checkoutRecoveryStoreSource.Contains("LegalVersionIds", StringComparison.Ordinal),
    "Launcher recovery state omits reviewed Legal version identities.");
Require(checkoutRecoveryStoreSource.Contains("Guid.TryParseExact", StringComparison.Ordinal),
    "Launcher checkout recovery store does not validate correlation identity.");
Require(!checkoutRecoveryStoreSource.Contains("checkout_url", StringComparison.OrdinalIgnoreCase),
    "Launcher persisted checkout URL material in recovery state.");
Require(!checkoutRecoveryStoreSource.Contains("PaymentStatus", StringComparison.OrdinalIgnoreCase),
    "Launcher persisted payment status in recovery state.");
Require(!checkoutRecoveryStoreSource.Contains("AmountMinor", StringComparison.OrdinalIgnoreCase),
    "Launcher persisted authoritative pricing in recovery state.");
Require(!checkoutRecoveryStoreSource.Contains("provider", StringComparison.OrdinalIgnoreCase),
    "Launcher persisted provider data in recovery state.");
Require(!checkoutRecoveryStoreSource.Contains("claim_code", StringComparison.OrdinalIgnoreCase),
    "Launcher persisted Claim Code material in checkout recovery state.");
Require(!checkoutRecoveryStoreSource.Contains("GiftClaimCode", StringComparison.Ordinal),
    "Launcher persisted gift Claim Code plaintext in checkout recovery state.");

var externalNavigatorSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Infrastructure", "ExternalBrowserNavigator.cs"));
Require(externalNavigatorSource.Contains("OpenCheckout", StringComparison.Ordinal),
    "Launcher secure checkout navigation is missing.");
Require(externalNavigatorSource.Contains("OpenLegalDocumentAsync", StringComparison.Ordinal),
    "Launcher Agent-authority Legal navigation is missing.");
Require(externalNavigatorSource.Contains(
        "await _platformAuthority.ResolveAsync(cancellationToken)",
        StringComparison.Ordinal),
    "Launcher Legal navigation does not inherit the Agent platform authority.");
Require(externalNavigatorSource.Contains("Uri.UriSchemeHttps", StringComparison.Ordinal),
    "Launcher secure checkout navigation does not require HTTPS.");
Require(!externalNavigatorSource.Contains("BKE_PLATFORM_BASE_URL", StringComparison.Ordinal) &&
    !externalNavigatorSource.Contains("DefaultBaseAddress", StringComparison.Ordinal) &&
    !externalNavigatorSource.Contains("jl-bke.com", StringComparison.OrdinalIgnoreCase),
    "Launcher external navigation still owns an independent platform authority.");
Require(!externalNavigatorSource.Contains("PayMongo", StringComparison.OrdinalIgnoreCase),
    "Launcher external navigator contains provider-specific authority.");
Require(!externalNavigatorSource.Contains("/api/agent-sessions/", StringComparison.OrdinalIgnoreCase),
    "Launcher external navigator calls a Digital Solutions Agent API directly.");
Require(normalizedViewModelSource.Contains(
        "await _externalNavigator.OpenLegalDocumentAsync(",
        StringComparison.Ordinal),
    "Launcher Legal-document UX does not await Agent-authority navigation.");
Require(mainWindowSource.Contains(
        "await ViewModel.OpenLegalDocumentAsync(",
        StringComparison.Ordinal),
    "Launcher Legal-document click handler does not await Agent-authority navigation.");

Require(normalizedViewModelSource.Contains(
    "await _storeCheckoutStart.StartAsync(",
    StringComparison.Ordinal),
    "Launcher Store UX does not delegate checkout start to the Agent.");
Require(normalizedViewModelSource.Contains(
    "_externalNavigator.OpenCheckout(result.CheckoutUrl)",
    StringComparison.Ordinal),
    "Launcher does not navigate only from the Agent-returned checkout target.");
Require(normalizedViewModelSource.Contains(
    "\"RESULT_UNKNOWN\"",
    StringComparison.Ordinal),
    "Launcher does not surface ambiguous checkout mutation results.");
Require(normalizedViewModelSource.Contains(
    "Do not retry automatically",
    StringComparison.OrdinalIgnoreCase),
    "Launcher checkout UX does not preserve the no-blind-retry mutation rule.");
Require(!normalizedViewModelSource.Contains(
    "/api/checkout",
    StringComparison.OrdinalIgnoreCase),
    "Launcher Store UX directly calls Digital Solutions checkout.");
Require(!normalizedViewModelSource.Contains(
    "PayMongo",
    StringComparison.OrdinalIgnoreCase),
    "Launcher Store UX absorbed payment-provider behavior.");

var claimControllerSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Application", "LauncherClaimCodeRedemptionController.cs"));
Require(!claimControllerSource.Contains("account_id", StringComparison.OrdinalIgnoreCase),
    "Launcher Claim Code intent can choose the destination account.");
Require(!claimControllerSource.Contains("/api/agent-sessions/", StringComparison.OrdinalIgnoreCase),
    "Launcher Claim Code controller bypasses the Agent loopback boundary.");
Require(!claimControllerSource.Contains("File.", StringComparison.Ordinal),
    "Launcher Claim Code controller persists one-time code material.");

var launcherSource = string.Join(
    "\n",
    Directory.EnumerateFiles("src", "*.cs", SearchOption.AllDirectories)
        .Where(path =>
            !path.Split(Path.DirectorySeparatorChar)
                .Any(segment => segment is "bin" or "obj"))
        .Select(File.ReadAllText));
var forbiddenUpdateAuthorityMarkers = new[]
{
    "/api/agent-sessions/update/standalone",
    "api.github.com/repos/",
    "/releases/tags/",
    "artifact_sha256",
    "artifact_size",
    "install_root",
    "bke.privileged-update-request",
    "/api/agent-sessions/repair/standalone",
    "/api/agent-sessions/claims/redeem",
    "/api/agent-sessions/store",
    "PAYMONGO",
    "CLAIM_ENTITLEMENT",
    "CLAIM_CODE_CHECKOUT_ENABLED",
    "bke.repair-policy.v1",
    "repair_policy_sha256",
    "bke.privileged-repair-request",
    "privileged_arguments",
};
foreach (var marker in forbiddenUpdateAuthorityMarkers)
{
    Require(
        !launcherSource.Contains(marker, StringComparison.OrdinalIgnoreCase),
        $"Launcher absorbed forbidden software-update authority: {marker}");
}

var contextProperties = typeof(ILauncherContext)
    .GetProperties()
    .Select(property => property.Name)
    .ToArray();

Require(contextProperties.SequenceEqual(["Authorization"]), "Plugin context widened beyond the approved capability boundary.");
Require(typeof(ILauncherContext).GetProperties().All(property =>
    !property.Name.Contains("Token", StringComparison.OrdinalIgnoreCase)), "Plugin context exposes token material.");

var rejectedNonLoopback = false;
try
{
    using var _ = new AgentLoopbackClient(baseAddress: new Uri("https://jl-bke.com", UriKind.Absolute));
}
catch (ArgumentException)
{
    rejectedNonLoopback = true;
}
Require(rejectedNonLoopback, "Launcher Agent client accepted a non-loopback endpoint.");
Require(AgentLocalContract.StoreGiftClaimRevealPath == "/v1/store/gift-claim-code",
    "Launcher Agent gift Claim Code loopback path drifted.");
Require(AgentLocalContract.StoreGiftClaimRevealCapabilityId == "bke.store-gift-claim-reveal",
    "Launcher Agent gift Claim Code capability id drifted.");
Require(AgentLocalContract.StoreGiftClaimRevealContractVersion == 1,
    "Launcher Agent gift Claim Code contract version drifted.");
Require(AgentLocalContract.AccountNotificationFeedPath == "/v1/notifications/account-feed",
    "Launcher Agent account notification loopback path drifted.");
Require(AgentLocalContract.AccountNotificationReceiptPath == "/v1/notifications/account-receipt",
    "Launcher Agent account notification receipt loopback path drifted.");
Require(AgentLocalContract.AccountNotificationInboxCapabilityId == "bke.account-notifications",
    "Launcher Agent account notification capability id drifted.");
Require(AgentLocalContract.AccountNotificationInboxContractVersion == 1,
    "Launcher Agent account notification contract version drifted.");

var defaultTimeoutField = typeof(AgentLoopbackClient).GetField(
    "DefaultRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var passwordChangeTimeoutField = typeof(AgentLoopbackClient).GetField(
    "AccountPasswordChangeRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var privacyTimeoutField = typeof(AgentLoopbackClient).GetField(
    "AccountPrivacyRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var installTimeoutField = typeof(AgentLoopbackClient).GetField(
    "InstallRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var updateTimeoutField = typeof(AgentLoopbackClient).GetField(
    "UpdateRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var repairTimeoutField = typeof(AgentLoopbackClient).GetField(
    "RepairRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var removeTimeoutField = typeof(AgentLoopbackClient).GetField(
    "RemoveRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var defaultTimeoutValue = defaultTimeoutField?.GetValue(null);
var passwordChangeTimeoutValue = passwordChangeTimeoutField?.GetValue(null);
var privacyTimeoutValue = privacyTimeoutField?.GetValue(null);
var installTimeoutValue = installTimeoutField?.GetValue(null);
var updateTimeoutValue = updateTimeoutField?.GetValue(null);
var repairTimeoutValue = repairTimeoutField?.GetValue(null);
var removeTimeoutValue = removeTimeoutField?.GetValue(null);
Require(defaultTimeoutValue is TimeSpan,
    "Launcher default loopback timeout field is unavailable.");
Require(passwordChangeTimeoutValue is TimeSpan,
    "Launcher password-change timeout field is unavailable.");
Require(privacyTimeoutValue is TimeSpan,
    "Launcher account-privacy timeout field is unavailable.");
Require(installTimeoutValue is TimeSpan,
    "Launcher install-operation timeout field is unavailable.");
Require(updateTimeoutValue is TimeSpan,
    "Launcher update-operation timeout field is unavailable.");
Require(repairTimeoutValue is TimeSpan,
    "Launcher Repair-operation timeout field is unavailable.");
Require(removeTimeoutValue is TimeSpan,
    "Launcher remove-operation timeout field is unavailable.");
var defaultTimeout = (TimeSpan)defaultTimeoutValue!;
var passwordChangeTimeout = (TimeSpan)passwordChangeTimeoutValue!;
var privacyTimeout = (TimeSpan)privacyTimeoutValue!;
var installTimeout = (TimeSpan)installTimeoutValue!;
var updateTimeout = (TimeSpan)updateTimeoutValue!;
var repairTimeout = (TimeSpan)repairTimeoutValue!;
var removeTimeout = (TimeSpan)removeTimeoutValue!;
Require(defaultTimeout == TimeSpan.FromSeconds(5),
    "Launcher default loopback timeout drifted.");
Require(passwordChangeTimeout == TimeSpan.FromSeconds(30),
    "Launcher password-change timeout drifted.");
Require(passwordChangeTimeout > defaultTimeout,
    "Launcher password change does not have a dedicated bounded mutation timeout.");
Require(privacyTimeout == TimeSpan.FromSeconds(30),
    "Launcher account-privacy timeout drifted.");
Require(privacyTimeout > defaultTimeout,
    "Launcher account privacy does not have a dedicated bounded timeout.");
Require(installTimeout == TimeSpan.FromMinutes(10),
    "Launcher install-operation timeout drifted.");
Require(installTimeout > defaultTimeout,
    "Launcher install operation does not have a dedicated long-running timeout.");
Require(updateTimeout == TimeSpan.FromMinutes(10),
    "Launcher update-operation timeout drifted.");
Require(updateTimeout > defaultTimeout,
    "Launcher update operation does not have a dedicated long-running timeout.");
Require(repairTimeout == TimeSpan.FromMinutes(10),
    "Launcher Repair-operation timeout drifted.");
Require(repairTimeout > defaultTimeout,
    "Launcher Repair operation does not have a dedicated long-running timeout.");
Require(removeTimeout == TimeSpan.FromMinutes(10),
    "Launcher remove-operation timeout drifted.");
Require(removeTimeout > defaultTimeout,
    "Launcher remove operation does not have a dedicated long-running timeout.");

var accountSwitchViewModelSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Presentation", "MainWindowViewModel.cs"));
Require(
    accountSwitchViewModelSource.Contains(
        "public async Task SwitchAccountAsync(",
        StringComparison.Ordinal) &&
    accountSwitchViewModelSource.Contains(
        "await LogoutAsync(cancellationToken);",
        StringComparison.Ordinal),
    "Launcher account switching does not revoke/clear the Agent-owned session first.");
Require(
    accountSwitchViewModelSource.Contains(
        "var preservedEmail = _authenticatedAccountEmail;",
        StringComparison.Ordinal) &&
    accountSwitchViewModelSource.Contains(
        "Password = string.Empty;",
        StringComparison.Ordinal) &&
    accountSwitchViewModelSource.Contains(
        "ClearNativeMfaState();",
        StringComparison.Ordinal),
    "Launcher account switching retained credential/MFA material or failed to preserve only email.");
Require(
    accountSwitchViewModelSource.Contains(
        "if (_purchaseAttemptLocked)",
        StringComparison.Ordinal) &&
    accountSwitchViewModelSource.Contains(
        "Resolve the existing checkout attempt before switching accounts.",
        StringComparison.Ordinal),
    "Launcher account switching does not respect checkout-recovery account binding.");
Require(
    !accountSwitchViewModelSource.Contains(
        "/api/agent-sessions/",
        StringComparison.OrdinalIgnoreCase) &&
    !accountSwitchViewModelSource.Contains(
        "customer_account_id",
        StringComparison.OrdinalIgnoreCase),
    "Launcher account switching absorbed cloud/session mutation authority.");
Require(
    mainWindowMarkup.Contains(
        "Content=\"Switch BKE account\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "IsEnabled=\"{Binding CanSwitchAccount}\"",
        StringComparison.Ordinal) &&
    mainWindowMarkup.Contains(
        "Click=\"SwitchAccount\"",
        StringComparison.Ordinal),
    "Launcher safe account-switch desktop action is missing.");
Require(
    mainWindowSource.Contains(
        "SwitchAccount",
        StringComparison.Ordinal),
    "Launcher safe account-switch desktop handler is missing.");

await CertifyNativePasswordResetRequestAsync();
await CertifyNativeRegistrationJourneyAsync();
await CertifyAccountPasswordChangeSettingsAsync();
await CertifyAccountPrivacySettingsAsync();
await CertifyAccountOrganizationSettingsAsync();
await CertifySafeAccountSwitchingAsync();
await CertifyCustomerAcquisitionToMySoftwareAsync();

Console.WriteLine("BKE Launcher contract certification: PASS");
Console.WriteLine("Agent-owned account session boundary certified");
Console.WriteLine("Native Forgot Password enumeration-safe recovery composition certified");
Console.WriteLine("Native Create Account legal acceptance and email verification composition certified");
Console.WriteLine("Native Account Security password-change composition certified");
Console.WriteLine("Agent-mediated selected-account Privacy Requests composition certified");
Console.WriteLine("Agent-mediated Organization Overview/Create/member-management/self-leave presentation and reauthentication boundaries certified");
Console.WriteLine("Safe Personal/Organization account switching with checkout-lock protection certified");
Console.WriteLine("Agent-mediated selected-account Notifications presentation boundary certified");
Console.WriteLine("Agent-owned Claim Code redemption intent and transient-code boundary certified");
Console.WriteLine("Agent-owned Store catalog presentation boundary certified");
Console.WriteLine("Agent-owned Store checkout-review presentation boundary certified");
Console.WriteLine("Customer acquisition -> entitlement -> My Software composition certified");
Console.WriteLine("Agent-owned platform authority for native login and Legal navigation certified");
Console.WriteLine("Native Launcher credential -> Agent-owned authority -> DS -> one-time Agent handoff boundary certified");
Console.WriteLine("Agent-owned software catalog boundary certified");
Console.WriteLine("Agent-owned standalone install intent boundary certified");
Console.WriteLine("Bounded long-running install transport certified");
Console.WriteLine("Agent-owned software Update intent boundary certified");
Console.WriteLine("Bounded long-running update transport certified");
Console.WriteLine("Agent-owned same-version software Repair intent boundary certified");
Console.WriteLine("Bounded long-running Repair transport certified");
Console.WriteLine("Agent-owned standalone Open intent boundary certified");
Console.WriteLine("Agent-owned software Remove intent boundary certified");
Console.WriteLine("Bounded long-running remove transport certified");
Console.WriteLine("Owner-controlled LAUNCHER_PLUGIN/STANDALONE types certified");
return;


static async Task CertifyNativeRegistrationJourneyAsync()
{
    var catalog = new CustomerJourneyCatalogSource();
    var agent = new CustomerJourneyAgentClient(catalog)
    {
        Authenticated = false,
    };
    var identity = new CustomerJourneyIdentityClient();
    var viewModel = BuildCustomerJourneyViewModel(
        agent,
        catalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator(),
        identity);

    await viewModel.InitializeAsync(CancellationToken.None);
    Require(viewModel.ShowSignInForm && !viewModel.IsAuthenticated,
        "Native registration certification did not begin on the signed-out sign-in surface.");

    await viewModel.OpenRegistrationAsync(CancellationToken.None);

    Require(identity.RegistrationPreflightCount == 1 &&
            identity.LastRegistrationAuthority ==
                new Uri("https://digital-solutions.example.test/"),
        "Launcher registration preflight did not use the Agent-inherited Digital Solutions authority.");
    Require(agent.PlatformAuthorityCount == 1,
        "Launcher registration preflight did not resolve platform authority exactly once.");
    Require(viewModel.ShowRegistration &&
            viewModel.RegistrationStatus == "READY" &&
            viewModel.RegistrationLegalDocuments.Count == 2 &&
            viewModel.RegistrationLegalDocuments.All(document =>
                !string.IsNullOrWhiteSpace(document.ContentMarkdown)),
        "Launcher did not present both exact authoritative registration legal documents.");

    viewModel.RegistrationName = "New Customer";
    viewModel.RegistrationEmail = "new-customer@example.test";
    viewModel.RegistrationPassword = "transient-registration-password";
    Require(!viewModel.CanCreateAccount,
        "Launcher enabled Create Account before both exact legal versions were accepted.");

    foreach (var document in viewModel.RegistrationLegalDocuments)
    {
        document.IsAccepted = true;
    }
    Require(viewModel.CanCreateAccount,
        "Launcher did not enable Create Account after complete legal acceptance.");

    await viewModel.CreateNativeAccountAsync(CancellationToken.None);

    Require(identity.RegistrationCount == 1 &&
            identity.LastRegistrationRequest is not null &&
            identity.LastRegistrationRequest.Email == "new-customer@example.test" &&
            identity.LastRegistrationRequest.Name == "New Customer" &&
            identity.LastRegistrationRequest.Password == "transient-registration-password" &&
            identity.LastRegistrationRequest.LegalVersionIds.SequenceEqual([
                "terms-current",
                "privacy-current"
            ]),
        "Launcher registration did not submit the transient credential and exact accepted legal version IDs.");
    Require(string.IsNullOrEmpty(viewModel.RegistrationPassword),
        "Launcher retained the registration password after account creation.");
    Require(viewModel.RegistrationStatus == "VERIFICATION_REQUIRED" &&
            viewModel.ShowRegistrationVerification &&
            !viewModel.IsAuthenticated,
        "Launcher registration incorrectly created an authenticated Agent session or skipped email verification.");

    await viewModel.ResendRegistrationVerificationAsync(CancellationToken.None);
    Require(identity.ResendCount == 1 &&
            viewModel.RegistrationStatus == "VERIFICATION_REQUIRED" &&
            viewModel.RegistrationMessage.Contains(
                "If this account still needs verification",
                StringComparison.Ordinal),
        "Launcher verification resend did not preserve the generic enumeration-resistant UX.");

    viewModel.RegistrationCode = "ABCD2345";
    await viewModel.VerifyRegistrationEmailAsync(CancellationToken.None);

    Require(identity.VerificationCount == 1 &&
            identity.LastVerificationCode == "ABCD2345",
        "Launcher did not submit the transient native email verification code.");
    Require(string.IsNullOrEmpty(viewModel.RegistrationCode),
        "Launcher retained the native email verification code after submission.");
    Require(viewModel.RegistrationStatus == "VERIFIED" &&
            viewModel.ShowSignInForm &&
            !viewModel.ShowRegistration &&
            viewModel.Email == "new-customer@example.test" &&
            string.IsNullOrEmpty(viewModel.Password) &&
            !viewModel.IsAuthenticated,
        "Verified registration did not return to normal signed-out BKE login with only the verified email prefilled.");
}

static async Task CertifyNativePasswordResetRequestAsync()
{
    var catalog = new CustomerJourneyCatalogSource();
    var agent = new CustomerJourneyAgentClient(catalog)
    {
        Authenticated = false,
    };
    var identity = new CustomerJourneyIdentityClient();
    var viewModel = BuildCustomerJourneyViewModel(
        agent,
        catalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator(),
        identity);

    await viewModel.InitializeAsync(CancellationToken.None);
    Require(viewModel.ShowLoginPage && !viewModel.IsAuthenticated,
        "Password-reset certification did not begin on the signed-out native login surface.");

    viewModel.Email = "customer@example.test";
    viewModel.Password = "stale-password-cert";

    await viewModel.RequestPasswordResetAsync(CancellationToken.None);

    Require(agent.PlatformAuthorityCount == 1,
        "Launcher password reset did not inherit Digital Solutions authority from the Agent.");
    Require(identity.ResetRequestCount == 1 &&
            identity.LastResetEmail == "customer@example.test" &&
            identity.LastResetAuthority == new Uri("https://digital-solutions.example.test/"),
        "Launcher password reset did not delegate exactly one generic reset request to Digital Solutions.");
    Require(viewModel.PasswordResetStatus == "ACCEPTED" &&
            viewModel.PasswordResetMessage.Contains(
                "If a BKE account exists for this email",
                StringComparison.Ordinal),
        "Launcher password reset did not preserve enumeration-safe accepted UX.");
    Require(string.IsNullOrEmpty(viewModel.Password),
        "Launcher password reset retained the stale sign-in password field.");
    Require(viewModel.ShowLoginPage && !viewModel.IsAuthenticated,
        "Password reset request incorrectly created or mutated an authenticated Agent session.");

    viewModel.Email = string.Empty;
    await viewModel.RequestPasswordResetAsync(CancellationToken.None);
    Require(identity.ResetRequestCount == 1 &&
            viewModel.PasswordResetStatus == "INVALID_INPUT",
        "Launcher submitted password reset without an email.");
}

static async Task CertifyAccountPasswordChangeSettingsAsync()
{
    var successCatalog = new CustomerJourneyCatalogSource();
    var successAgent = new CustomerJourneyAgentClient(successCatalog);
    var successViewModel = BuildCustomerJourneyViewModel(
        successAgent,
        successCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());

    await successViewModel.InitializeAsync(CancellationToken.None);
    successViewModel.OpenAccountSurface();
    successViewModel.CurrentPassword = "current-password-cert";
    successViewModel.NewPassword = "new-password-cert";
    successViewModel.ConfirmNewPassword = "new-password-cert";

    await successViewModel.ChangePasswordAsync(CancellationToken.None);

    Require(successAgent.PasswordChangeCount == 1,
        "Launcher password change did not invoke the Agent exactly once.");
    Require(successAgent.LastPasswordChangeRequest is not null &&
            successAgent.LastPasswordChangeRequest.CurrentPassword == "current-password-cert" &&
            successAgent.LastPasswordChangeRequest.NewPassword == "new-password-cert",
        "Launcher password change did not forward the transient credential intent.");
    Require(!successViewModel.IsAuthenticated &&
            successViewModel.ShowLoginPage &&
            successViewModel.PasswordChangeStatus == "CHANGED",
        "Successful password change did not require clean native reauthentication.");
    Require(string.IsNullOrEmpty(successViewModel.CurrentPassword) &&
            string.IsNullOrEmpty(successViewModel.NewPassword) &&
            string.IsNullOrEmpty(successViewModel.ConfirmNewPassword),
        "Successful password change retained transient credential fields.");

    var deniedCatalog = new CustomerJourneyCatalogSource();
    var deniedAgent = new CustomerJourneyAgentClient(deniedCatalog)
    {
        PasswordChangeOutcome = "INVALID_CREDENTIALS",
    };
    var deniedViewModel = BuildCustomerJourneyViewModel(
        deniedAgent,
        deniedCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());

    await deniedViewModel.InitializeAsync(CancellationToken.None);
    deniedViewModel.OpenAccountSurface();
    deniedViewModel.CurrentPassword = "wrong-current-cert";
    deniedViewModel.NewPassword = "new-password-cert";
    deniedViewModel.ConfirmNewPassword = "new-password-cert";

    await deniedViewModel.ChangePasswordAsync(CancellationToken.None);

    Require(deniedAgent.PasswordChangeCount == 1,
        "Invalid-credential password change did not reach the Agent exactly once.");
    Require(deniedViewModel.IsAuthenticated &&
            deniedViewModel.ShowAuthenticatedShell &&
            deniedViewModel.PasswordChangeStatus == "INVALID_CREDENTIALS",
        "Explicit invalid credentials incorrectly destroyed the authenticated shell.");

    var mismatchCatalog = new CustomerJourneyCatalogSource();
    var mismatchAgent = new CustomerJourneyAgentClient(mismatchCatalog);
    var mismatchViewModel = BuildCustomerJourneyViewModel(
        mismatchAgent,
        mismatchCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());

    await mismatchViewModel.InitializeAsync(CancellationToken.None);
    mismatchViewModel.CurrentPassword = "current-password-cert";
    mismatchViewModel.NewPassword = "new-password-cert";
    mismatchViewModel.ConfirmNewPassword = "different-password-cert";

    await mismatchViewModel.ChangePasswordAsync(CancellationToken.None);

    Require(mismatchAgent.PasswordChangeCount == 0 &&
            mismatchViewModel.IsAuthenticated &&
            mismatchViewModel.PasswordChangeStatus == "INVALID_INPUT",
        "Launcher submitted a password mutation despite local confirmation mismatch.");
}

static async Task CertifyAccountPrivacySettingsAsync()
{
    var catalog = new CustomerJourneyCatalogSource();
    var agent = new CustomerJourneyAgentClient(catalog);
    var viewModel = BuildCustomerJourneyViewModel(
        agent,
        catalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());

    await viewModel.InitializeAsync(CancellationToken.None);
    Require(viewModel.IsAuthenticated,
        "Account Privacy certification did not begin authenticated.");

    viewModel.OpenAccountSurface();
    await viewModel.RefreshAccountPrivacyAsync(CancellationToken.None);

    Require(agent.PrivacyListCount == 1 &&
            viewModel.AccountPrivacyStatus == "READY" &&
            viewModel.AccountPrivacyRequestTypes.SequenceEqual([
                "ACCESS",
                "EXPORT",
                "DELETION"
            ]) &&
            viewModel.SelectedAccountPrivacyRequestType == "ACCESS" &&
            viewModel.AccountPrivacyRequests.Count == 0,
        "Launcher did not load authoritative Agent-mediated privacy request types.");

    viewModel.SelectedAccountPrivacyRequestType = "BREACH_REPORT";
    viewModel.AccountPrivacySummary =
        "This request type was not returned by the authoritative privacy list.";
    Require(!viewModel.CanCreateAccountPrivacyRequest,
        "Launcher enabled an unadvertised privacy request type.");

    viewModel.SelectedAccountPrivacyRequestType = "EXPORT";
    viewModel.AccountPrivacySummary =
        "Please export the personal data associated with this account.";
    Require(viewModel.CanCreateAccountPrivacyRequest,
        "Launcher did not enable an authoritative privacy request type.");

    await viewModel.CreateAccountPrivacyRequestAsync(
        CancellationToken.None);

    Require(agent.PrivacyCreateCount == 1 &&
            agent.PrivacyListCount == 2 &&
            viewModel.AccountPrivacyStatus == "READY" &&
            string.IsNullOrEmpty(viewModel.AccountPrivacySummary) &&
            viewModel.AccountPrivacyRequests.Count == 1 &&
            viewModel.AccountPrivacyRequests[0].RequestType == "EXPORT" &&
            viewModel.AccountPrivacyRequests[0].Status == "OPEN",
        "Launcher privacy create did not clear transient summary and refresh authoritative history.");

    agent.PrivacyCreateOutcomeUnknown = true;
    viewModel.SelectedAccountPrivacyRequestType = "ACCESS";
    viewModel.AccountPrivacySummary =
        "Please provide the personal data associated with this account.";

    await viewModel.CreateAccountPrivacyRequestAsync(
        CancellationToken.None);

    Require(agent.PrivacyCreateCount == 2 &&
            agent.PrivacyListCount == 3 &&
            viewModel.AccountPrivacyRequests.Count == 1 &&
            string.IsNullOrEmpty(viewModel.AccountPrivacySummary) &&
            viewModel.AccountPrivacyMessage.Contains(
                "could not be confirmed",
                StringComparison.OrdinalIgnoreCase),
        "Launcher ambiguous privacy create replayed or failed to refresh safely.");

    agent.Authenticated = false;
    await viewModel.RefreshAccountPrivacyAsync(
        CancellationToken.None);

    Require(!viewModel.IsAuthenticated &&
            viewModel.AccountPrivacyRequests.Count == 0 &&
            viewModel.AccountPrivacyRequestTypes.Count == 0,
        "Launcher retained privacy state after Agent session invalidation.");
}

static async Task CertifyAccountOrganizationSettingsAsync()
{
    var createCatalog = new CustomerJourneyCatalogSource();
    var createAgent = new CustomerJourneyAgentClient(createCatalog);
    var createViewModel = BuildCustomerJourneyViewModel(
        createAgent,
        createCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());

    await createViewModel.InitializeAsync(CancellationToken.None);
    createViewModel.OpenAccountSurface();

    Require(
        createViewModel.IsAuthenticated &&
        createViewModel.AccountTypeLabel == "Personal account" &&
        createViewModel.ShowOrganizationCreateSection &&
        createViewModel.OrganizationCreateBillingEmail ==
            "customer@example.test",
        "Organization creation certification did not begin from the authenticated Personal account.");

    createViewModel.OrganizationCreateDisplayName =
        "Created Certification Org";
    createViewModel.OrganizationCreateLegalName =
        "Created Certification Organization Legal";
    createViewModel.OrganizationCreateRegistrationNumber =
        "REG-CREATE";
    createViewModel.OrganizationCreateTaxId = "TAX-CREATE";

    Require(
        createViewModel.CanCreateOrganization,
        "Launcher did not enable valid organization creation input.");

    createAgent.OrganizationCreateOutcome =
        "EMAIL_NOT_VERIFIED";
    await createViewModel.CreateAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        createViewModel.IsAuthenticated &&
        createViewModel.OrganizationCreateStatus ==
            "EMAIL_NOT_VERIFIED" &&
        createViewModel.OrganizationCreateDisplayName ==
            "Created Certification Org",
        "EMAIL_NOT_VERIFIED incorrectly destroyed organization creation state or session.");

    createAgent.OrganizationCreateOutcome =
        "LEGAL_REACCEPTANCE_REQUIRED";
    await createViewModel.CreateAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        createViewModel.IsAuthenticated &&
        createViewModel.OrganizationCreateStatus ==
            "LEGAL_REACCEPTANCE_REQUIRED" &&
        createViewModel.OrganizationCreateDisplayName ==
            "Created Certification Org",
        "LEGAL_REACCEPTANCE_REQUIRED incorrectly destroyed organization creation state or session.");

    createAgent.OrganizationCreateOutcome = "CREATED";
    await createViewModel.CreateAccountOrganizationAsync(
        CancellationToken.None);

    Require(
        createAgent.OrganizationCreateCount == 3 &&
        createAgent.LastOrganizationCreateRequest is not null &&
        createAgent.LastOrganizationCreateRequest.DisplayName ==
            "Created Certification Org" &&
        createAgent.LastOrganizationCreateRequest.LegalName ==
            "Created Certification Organization Legal" &&
        createAgent.LastOrganizationCreateRequest.BillingEmail ==
            "customer@example.test" &&
        createAgent.LastOrganizationCreateRequest.RegistrationNumber ==
            "REG-CREATE" &&
        createAgent.LastOrganizationCreateRequest.TaxId ==
            "TAX-CREATE" &&
        createViewModel.OrganizationCreateStatus == "CREATED" &&
        !createViewModel.OrganizationCreateRetryBlocked &&
        string.IsNullOrEmpty(
            createViewModel.OrganizationCreateDisplayName) &&
        createViewModel.AccountTypeLabel == "Personal account" &&
        createViewModel.OrganizationCreateMessage.Contains(
            "Switch BKE account",
            StringComparison.Ordinal),
        "Launcher organization creation did not preserve explicit account switching or clear transient form state.");

    createViewModel.OpenAccountSurface();
    createViewModel.OrganizationCreateDisplayName =
        "Ambiguous Certification Org";
    createViewModel.OrganizationCreateLegalName =
        "Ambiguous Certification Organization Legal";
    createAgent.OrganizationCreateOutcomeUnknown = true;

    await createViewModel.CreateAccountOrganizationAsync(
        CancellationToken.None);

    Require(
        createAgent.OrganizationCreateCount == 4 &&
        createViewModel.OrganizationCreateStatus ==
            "OUTCOME_UNKNOWN" &&
        createViewModel.OrganizationCreateRetryBlocked &&
        !createViewModel.CanCreateOrganization &&
        createViewModel.OrganizationCreateMessage.Contains(
            "could not be confirmed",
            StringComparison.OrdinalIgnoreCase),
        "Launcher ambiguous organization creation remained replayable.");

    await createViewModel.CreateAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        createAgent.OrganizationCreateCount == 4,
        "Launcher blindly replayed an ambiguous organization creation.");

    await createViewModel.SwitchAccountAsync(
        CancellationToken.None);
    Require(
        createViewModel.SessionStatus == "SIGNED_OUT" &&
        !createViewModel.OrganizationCreateRetryBlocked &&
        string.IsNullOrEmpty(
            createViewModel.OrganizationCreateDisplayName) &&
        string.IsNullOrEmpty(
            createViewModel.OrganizationCreateBillingEmail),
        "Safe account switching retained organization-create mutation state.");

    var ownerCatalog = new CustomerJourneyCatalogSource();
    var ownerAgent = new CustomerJourneyAgentClient(ownerCatalog)
    {
        AccountType = "ORGANIZATION",
        OrganizationRole = "OWNER",
        OrganizationDisplayName = "Certification Org",
    };
    var ownerViewModel = BuildCustomerJourneyViewModel(
        ownerAgent,
        ownerCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());

    await ownerViewModel.InitializeAsync(CancellationToken.None);

    Require(
        ownerViewModel.IsAuthenticated &&
        ownerViewModel.AccountTypeLabel == "Organization account" &&
        ownerViewModel.ShowOrganizationSection,
        "Organization overview certification did not begin in an Organization account.");

    ownerViewModel.OpenAccountSurface();
    await ownerViewModel.RefreshAccountOrganizationAsync(
        CancellationToken.None);

    Require(
        ownerAgent.OrganizationReadCount == 1 &&
        ownerViewModel.AccountOrganizationStatus == "READY" &&
        ownerViewModel.OrganizationReady &&
        ownerViewModel.OrganizationDisplayName == "Certification Org" &&
        ownerViewModel.OrganizationRole == "OWNER" &&
        ownerViewModel.OrganizationLegalName ==
            "Certification Organization Legal" &&
        ownerViewModel.HasOrganizationRegistrationNumber &&
        ownerViewModel.HasOrganizationBillingEmail &&
        ownerViewModel.HasOrganizationTaxId &&
        ownerViewModel.HasOrganizationLicenseCount &&
        ownerViewModel.HasOrganizationSubscriptionCount &&
        ownerViewModel.HasOrganizationOrderCount &&
        ownerViewModel.OrganizationMembers.Count == 2 &&
        ownerViewModel.OrganizationMembers.Any(member =>
            member.Email == "manager@example.test" &&
            member.ManagementHandle ==
                CustomerJourneyAgentClient.OrganizationManagedMemberHandle) &&
        ownerViewModel.OrganizationInvitations.Count == 1 &&
        ownerViewModel.OrganizationInvitations[0].ManagementHandle ==
            CustomerJourneyAgentClient.OrganizationInvitationHandle &&
        ownerViewModel.ShowOrganizationMembers &&
        !ownerViewModel.ShowOrganizationLeaveSection &&
        !ownerViewModel.CanLeaveOrganization,
        "Launcher did not render the complete Agent-supplied OWNER organization overview or incorrectly enabled owner self-leave.");

    Require(
        ownerViewModel.ShowOrganizationProfileEditor &&
        ownerViewModel.CanEditOrganizationIdentity &&
        ownerViewModel.CanEditOrganizationBilling &&
        ownerViewModel.OrganizationEditDisplayName ==
            "Certification Org" &&
        ownerViewModel.OrganizationEditLegalName ==
            "Certification Organization Legal" &&
        ownerViewModel.OrganizationEditRegistrationNumber ==
            "REG-001" &&
        ownerViewModel.OrganizationEditBillingEmail ==
            "billing@example.test" &&
        ownerViewModel.OrganizationEditTaxId ==
            "TAX-001" &&
        !ownerViewModel.CanSaveOrganizationProfile,
        "OWNER organization profile editor did not load authoritative values and permissions.");

    ownerViewModel.OrganizationEditDisplayName =
        "Updated Certification Org";
    ownerViewModel.OrganizationEditLegalName =
        "Updated Certification Organization Legal";
    ownerViewModel.OrganizationEditRegistrationNumber = string.Empty;

    Require(
        ownerViewModel.OrganizationIdentityProfileDirty &&
        !ownerViewModel.OrganizationBillingProfileDirty &&
        ownerViewModel.CanSaveOrganizationProfile,
        "OWNER organization-only profile edit did not become saveable.");

    await ownerViewModel.UpdateAccountOrganizationProfileAsync(
        CancellationToken.None);

    Require(
        ownerAgent.OrganizationProfileUpdateCount == 1 &&
        ownerAgent.LastOrganizationProfileUpdateRequest is
            {
                UpdateOrganizationProfile: true,
                UpdateBillingProfile: false,
                DisplayName: "Updated Certification Org",
                LegalName: "Updated Certification Organization Legal",
                RegistrationNumber: null,
                BillingEmail: null,
                TaxId: null
            } &&
        ownerAgent.OrganizationReadCount == 2 &&
        ownerViewModel.OrganizationProfileUpdateStatus == "UPDATED" &&
        ownerViewModel.OrganizationDisplayName ==
            "Updated Certification Org" &&
        ownerViewModel.OrganizationLegalName ==
            "Updated Certification Organization Legal" &&
        string.IsNullOrEmpty(
            ownerViewModel.OrganizationRegistrationNumber) &&
        !ownerViewModel.CanSaveOrganizationProfile,
        "OWNER organization-profile update widened the billing group or failed authoritative refresh.");

    Require(
        ownerViewModel.CanManageOrganizationInvitations &&
        ownerViewModel.ShowOrganizationInviteSection &&
        !ownerViewModel.HasOrganizationInvitationCode,
        "OWNER did not receive MANAGE_MEMBERS invitation capability.");

    ownerViewModel.OrganizationInvitationEmail =
        "new-member@example.test";
    ownerViewModel.OrganizationInvitationRole = "MEMBER";
    Require(
        ownerViewModel.CanInviteOrganizationMember,
        "Valid OWNER invitation did not become issuable.");

    await ownerViewModel.CreateAccountOrganizationInvitationAsync(
        CancellationToken.None);

    Require(
        ownerAgent.OrganizationInvitationCreateCount == 1 &&
        ownerAgent.LastOrganizationInvitationRequest is
            {
                Email: "new-member@example.test",
                Role: "MEMBER"
            } &&
        ownerViewModel.OrganizationInvitationStatus == "CREATED" &&
        ownerViewModel.OrganizationInvitationCode ==
            "organization-invitation-code-cert" &&
        ownerViewModel.HasOrganizationInvitationCode &&
        !ownerViewModel.CanInviteOrganizationMember &&
        ownerViewModel.OrganizationInvitations.Count == 2 &&
        ownerAgent.OrganizationReadCount == 3,
        "OWNER invitation did not preserve transient code delivery or authoritative refresh.");

    await ownerViewModel.CreateAccountOrganizationInvitationAsync(
        CancellationToken.None);
    Require(
        ownerAgent.OrganizationInvitationCreateCount == 1,
        "Launcher overwrote an undelivered Organization invitation code with another issuance.");

    ownerViewModel.CompleteOrganizationInvitationDelivery();
    Require(
        !ownerViewModel.HasOrganizationInvitationCode &&
        string.IsNullOrEmpty(ownerViewModel.OrganizationInvitationCode) &&
        ownerViewModel.OrganizationInvitationStatus == "IDLE",
        "Launcher retained Organization invitation code after explicit delivery acknowledgement.");

    var managedInvitation =
        ownerViewModel.OrganizationInvitations.Single(
            invitation =>
                invitation.Email == "new-member@example.test");
    var readsBeforeResend = ownerAgent.OrganizationReadCount;
    await ownerViewModel.ManageAccountOrganizationInvitationAsync(
        managedInvitation,
        "RESEND",
        CancellationToken.None);

    Require(
        ownerAgent.OrganizationInvitationManageCount == 1 &&
        ownerAgent.LastOrganizationInvitationManageRequest is
            {
                Action: "RESEND",
                ManagementHandle:
                    CustomerJourneyAgentClient.NewOrganizationInvitationHandle
            } &&
        ownerAgent.OrganizationReadCount == readsBeforeResend + 1 &&
        ownerViewModel.OrganizationInvitationStatus == "RESENT" &&
        ownerViewModel.OrganizationInvitationCode ==
            "organization-invitation-resend-code-cert" &&
        ownerViewModel.HasOrganizationInvitationCode &&
        !ownerViewModel.CanManageOrganizationInvitationActions,
        "Launcher resend did not preserve opaque-handle intent, authoritative refresh, or transient delivery code.");

    await ownerViewModel.ManageAccountOrganizationInvitationAsync(
        managedInvitation,
        "REVOKE",
        CancellationToken.None);
    Require(
        ownerAgent.OrganizationInvitationManageCount == 1,
        "Launcher allowed another invitation mutation while a one-time resend code was still visible.");

    ownerViewModel.CompleteOrganizationInvitationDelivery();
    var readsBeforeRevoke = ownerAgent.OrganizationReadCount;
    managedInvitation =
        ownerViewModel.OrganizationInvitations.Single(
            invitation =>
                invitation.Email == "new-member@example.test");
    await ownerViewModel.ManageAccountOrganizationInvitationAsync(
        managedInvitation,
        "REVOKE",
        CancellationToken.None);

    Require(
        ownerAgent.OrganizationInvitationManageCount == 2 &&
        ownerAgent.LastOrganizationInvitationManageRequest is
            {
                Action: "REVOKE",
                ManagementHandle:
                    CustomerJourneyAgentClient.NewOrganizationInvitationHandle
            } &&
        ownerAgent.OrganizationReadCount == readsBeforeRevoke + 1 &&
        ownerViewModel.OrganizationInvitationStatus == "REVOKED" &&
        !ownerViewModel.HasOrganizationInvitationCode &&
        ownerViewModel.OrganizationInvitations.Count == 1,
        "Launcher revoke did not refresh authoritative pending invitations or incorrectly exposed a delivery code.");

    ownerAgent.OrganizationInvitationManageOutcomeUnknown = true;
    var ambiguousManagedInvitation =
        ownerViewModel.OrganizationInvitations[0];
    var readsBeforeManageUnknown =
        ownerAgent.OrganizationReadCount;
    await ownerViewModel.ManageAccountOrganizationInvitationAsync(
        ambiguousManagedInvitation,
        "RESEND",
        CancellationToken.None);

    Require(
        ownerAgent.OrganizationInvitationManageCount == 3 &&
        ownerAgent.OrganizationReadCount ==
            readsBeforeManageUnknown + 1 &&
        ownerViewModel.OrganizationInvitationStatus ==
            "OUTCOME_UNKNOWN" &&
        !ownerViewModel.HasOrganizationInvitationCode &&
        ownerViewModel.OrganizationInvitationMessage.Contains(
            "review",
            StringComparison.OrdinalIgnoreCase),
        "Ambiguous invitation management replayed, leaked a code, or skipped authoritative refresh.");

    ownerViewModel.OrganizationInvitationEmail =
        "ambiguous@example.test";
    ownerAgent.OrganizationInvitationOutcomeUnknown = true;
    var ownerReadsBeforeAmbiguousInvitation =
        ownerAgent.OrganizationReadCount;
    await ownerViewModel.CreateAccountOrganizationInvitationAsync(
        CancellationToken.None);

    Require(
        ownerAgent.OrganizationInvitationCreateCount == 2 &&
        ownerAgent.OrganizationReadCount ==
            ownerReadsBeforeAmbiguousInvitation + 1 &&
        ownerViewModel.OrganizationInvitationStatus ==
            "OUTCOME_UNKNOWN" &&
        !ownerViewModel.HasOrganizationInvitationCode &&
        string.IsNullOrEmpty(
            ownerViewModel.OrganizationInvitationEmail) &&
        ownerViewModel.OrganizationInvitationMessage.Contains(
            "review",
            StringComparison.OrdinalIgnoreCase),
        "Ambiguous Organization invitation replayed, leaked a code, or skipped authoritative refresh.");

    var managedMember =
        ownerViewModel.OrganizationMembers.Single(
            member => member.Email == "manager@example.test");
    var readsBeforeMemberRole =
        ownerAgent.OrganizationReadCount;
    await ownerViewModel.ManageAccountOrganizationMemberAsync(
        managedMember,
        "UPDATE_ROLE",
        "BILLING",
        CancellationToken.None);

    Require(
        ownerAgent.OrganizationMemberManageCount == 1 &&
        ownerAgent.LastOrganizationMemberManageRequest is
            {
                Action: "UPDATE_ROLE",
                ManagementHandle:
                    CustomerJourneyAgentClient.OrganizationManagedMemberHandle,
                Role: "BILLING"
            } &&
        ownerAgent.OrganizationReadCount ==
            readsBeforeMemberRole + 1 &&
        ownerViewModel.OrganizationMemberManagementStatus ==
            "UPDATED" &&
        ownerViewModel.OrganizationMembers.Single(
            member => member.Email == "manager@example.test").Role ==
            "BILLING",
        "Launcher member role update widened identity authority or skipped authoritative refresh.");

    ownerAgent.OrganizationMemberManageOutcome =
        "LAST_OWNER_REQUIRED";
    var ownerMember =
        ownerViewModel.OrganizationMembers.Single(
            member => member.Email == "owner@example.test");
    await ownerViewModel.ManageAccountOrganizationMemberAsync(
        ownerMember,
        "UPDATE_ROLE",
        "MEMBER",
        CancellationToken.None);
    Require(
        ownerAgent.OrganizationMemberManageCount == 2 &&
        ownerViewModel.OrganizationMemberManagementStatus ==
            "LAST_OWNER_REQUIRED" &&
        ownerViewModel.OrganizationMembers.Single(
            member => member.Email == "owner@example.test").Role ==
            "OWNER",
        "Launcher weakened last-owner protection or optimistically changed the member list.");
    ownerAgent.OrganizationMemberManageOutcome = "READY";

    ownerAgent.OrganizationMemberManageOutcomeUnknown = true;
    managedMember =
        ownerViewModel.OrganizationMembers.Single(
            member => member.Email == "manager@example.test");
    var readsBeforeMemberUnknown =
        ownerAgent.OrganizationReadCount;
    await ownerViewModel.ManageAccountOrganizationMemberAsync(
        managedMember,
        "UPDATE_ROLE",
        "LICENSE_MANAGER",
        CancellationToken.None);
    Require(
        ownerAgent.OrganizationMemberManageCount == 3 &&
        ownerAgent.OrganizationReadCount ==
            readsBeforeMemberUnknown + 1 &&
        ownerViewModel.OrganizationMemberManagementStatus ==
            "OUTCOME_UNKNOWN" &&
        ownerViewModel.OrganizationMembers.Single(
            member => member.Email == "manager@example.test").Role ==
            "BILLING" &&
        ownerViewModel.OrganizationMemberManagementMessage.Contains(
            "review",
            StringComparison.OrdinalIgnoreCase),
        "Ambiguous member management replayed or skipped authoritative refresh.");

    managedMember =
        ownerViewModel.OrganizationMembers.Single(
            member => member.Email == "manager@example.test");
    var readsBeforeMemberRemove =
        ownerAgent.OrganizationReadCount;
    await ownerViewModel.ManageAccountOrganizationMemberAsync(
        managedMember,
        "REMOVE",
        null,
        CancellationToken.None);
    Require(
        ownerAgent.OrganizationMemberManageCount == 4 &&
        ownerAgent.LastOrganizationMemberManageRequest is
            {
                Action: "REMOVE",
                ManagementHandle:
                    CustomerJourneyAgentClient.OrganizationManagedMemberHandle,
                Role: null
            } &&
        ownerAgent.OrganizationReadCount ==
            readsBeforeMemberRemove + 1 &&
        ownerViewModel.OrganizationMemberManagementStatus ==
            "REMOVED" &&
        ownerViewModel.OrganizationMembers.Count == 1 &&
        ownerViewModel.OrganizationMembers[0].Email ==
            "owner@example.test",
        "Launcher member removal leaked role intent or skipped authoritative refresh.");

    var billingCatalog = new CustomerJourneyCatalogSource();
    var billingAgent = new CustomerJourneyAgentClient(billingCatalog)
    {
        AccountType = "ORGANIZATION",
        OrganizationRole = "BILLING",
        OrganizationDisplayName = "Billing Org",
    };
    var billingViewModel = BuildCustomerJourneyViewModel(
        billingAgent,
        billingCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());

    await billingViewModel.InitializeAsync(CancellationToken.None);
    await billingViewModel.RefreshAccountOrganizationAsync(
        CancellationToken.None);

    Require(
        billingViewModel.AccountOrganizationStatus == "READY" &&
        billingViewModel.OrganizationRole == "BILLING" &&
        billingViewModel.HasOrganizationBillingEmail &&
        billingViewModel.HasOrganizationTaxId &&
        !billingViewModel.HasOrganizationLicenseCount &&
        billingViewModel.HasOrganizationSubscriptionCount &&
        billingViewModel.HasOrganizationOrderCount &&
        billingViewModel.OrganizationMembers.Count == 0 &&
        billingViewModel.OrganizationInvitations.Count == 0 &&
        !billingViewModel.ShowOrganizationMembers,
        "Launcher widened a reduced-role organization response beyond Agent-supplied fields.");

    Require(
        billingViewModel.ShowOrganizationProfileEditor &&
        !billingViewModel.CanEditOrganizationIdentity &&
        billingViewModel.CanEditOrganizationBilling &&
        !billingViewModel.CanSaveOrganizationProfile,
        "BILLING organization profile editor exposed organization identity fields.");
    Require(
        !billingViewModel.CanManageOrganizationInvitations &&
        !billingViewModel.ShowOrganizationInviteSection &&
        !billingViewModel.CanInviteOrganizationMember &&
        !billingViewModel.CanManageOrganizationMemberActions &&
        billingViewModel.ShowOrganizationLeaveSection &&
        billingViewModel.CanLeaveOrganization,
        "BILLING role received MANAGE_MEMBERS UX or lost independent Agent-authoritative self-leave permission.");

    billingViewModel.OrganizationEditBillingEmail =
        "billing-new@example.test";
    billingViewModel.OrganizationEditTaxId = string.Empty;
    await billingViewModel.UpdateAccountOrganizationProfileAsync(
        CancellationToken.None);

    Require(
        billingAgent.OrganizationProfileUpdateCount == 1 &&
        billingAgent.LastOrganizationProfileUpdateRequest is
            {
                UpdateOrganizationProfile: false,
                UpdateBillingProfile: true,
                DisplayName: null,
                LegalName: null,
                RegistrationNumber: null,
                BillingEmail: "billing-new@example.test",
                TaxId: null
            } &&
        billingViewModel.OrganizationProfileUpdateStatus == "UPDATED" &&
        billingViewModel.OrganizationBillingEmail ==
            "billing-new@example.test" &&
        string.IsNullOrEmpty(billingViewModel.OrganizationTaxId) &&
        !billingViewModel.CanSaveOrganizationProfile,
        "BILLING profile update widened organization identity fields or failed authoritative refresh.");

    billingViewModel.OrganizationEditBillingEmail =
        "billing-ambiguous@example.test";
    billingAgent.OrganizationProfileUpdateOutcomeUnknown = true;
    var readsBeforeAmbiguous =
        billingAgent.OrganizationReadCount;
    await billingViewModel.UpdateAccountOrganizationProfileAsync(
        CancellationToken.None);

    Require(
        billingAgent.OrganizationProfileUpdateCount == 2 &&
        billingAgent.OrganizationReadCount ==
            readsBeforeAmbiguous + 1 &&
        billingViewModel.OrganizationProfileUpdateStatus ==
            "OUTCOME_UNKNOWN" &&
        billingViewModel.OrganizationEditBillingEmail ==
            "billing-new@example.test" &&
        !billingViewModel.CanSaveOrganizationProfile &&
        billingViewModel.OrganizationProfileUpdateMessage.Contains(
            "review",
            StringComparison.OrdinalIgnoreCase),
        "Ambiguous organization-profile update replayed or failed to force authoritative review.");

    var ownerLeaveController =
        new LauncherAccountOrganizationController(ownerAgent);
    var ownerLeaveResponse = await ownerLeaveController.LeaveAsync(
        CancellationToken.None);
    Require(
        ownerAgent.OrganizationLeaveCount == 1 &&
        ownerLeaveResponse.Status == "OWNER_CANNOT_LEAVE" &&
        !ownerLeaveResponse.ReauthenticationRequired &&
        ownerAgent.Authenticated,
        "Launcher weakened owner self-leave protection or destroyed valid owner session state.");

    var leaveCatalog = new CustomerJourneyCatalogSource();
    var leaveAgent = new CustomerJourneyAgentClient(leaveCatalog)
    {
        AccountType = "ORGANIZATION",
        OrganizationRole = "MEMBER",
        OrganizationDisplayName = "Leave Certification Org",
    };
    var leaveViewModel = BuildCustomerJourneyViewModel(
        leaveAgent,
        leaveCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());
    await leaveViewModel.InitializeAsync(CancellationToken.None);
    await leaveViewModel.RefreshAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        leaveViewModel.ShowOrganizationLeaveSection &&
        leaveViewModel.CanLeaveOrganization &&
        !leaveViewModel.ShowOrganizationMembers,
        "Launcher did not expose DS-authoritative self-leave independently of MANAGE_MEMBERS.");

    await leaveViewModel.LeaveAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        leaveAgent.OrganizationLeaveCount == 1 &&
        leaveAgent.LastOrganizationLeaveRequest is not null &&
        !leaveAgent.Authenticated &&
        !leaveViewModel.IsAuthenticated &&
        leaveViewModel.ShowLoginPage &&
        !leaveViewModel.OrganizationReady &&
        !leaveViewModel.ShowOrganizationSection &&
        !leaveViewModel.ShowOrganizationLeaveSection &&
        leaveViewModel.OrganizationMembers.Count == 0 &&
        leaveViewModel.OrganizationInvitations.Count == 0 &&
        leaveViewModel.Message.Contains(
            "left",
            StringComparison.OrdinalIgnoreCase),
        "Successful Organization self-leave did not clear selected-account presentation and require fresh authentication.");

    var staleLeaveCatalog = new CustomerJourneyCatalogSource();
    var staleLeaveAgent = new CustomerJourneyAgentClient(
        staleLeaveCatalog)
    {
        AccountType = "ORGANIZATION",
        OrganizationRole = "MEMBER",
        OrganizationDisplayName = "Stale Membership Org",
        OrganizationLeaveOutcome = "MEMBER_NOT_FOUND",
    };
    var staleLeaveViewModel = BuildCustomerJourneyViewModel(
        staleLeaveAgent,
        staleLeaveCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());
    await staleLeaveViewModel.InitializeAsync(
        CancellationToken.None);
    await staleLeaveViewModel.RefreshAccountOrganizationAsync(
        CancellationToken.None);
    await staleLeaveViewModel.LeaveAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        staleLeaveAgent.OrganizationLeaveCount == 1 &&
        !staleLeaveAgent.Authenticated &&
        !staleLeaveViewModel.IsAuthenticated &&
        staleLeaveViewModel.Message.Contains(
            "no longer available",
            StringComparison.OrdinalIgnoreCase),
        "Stale Organization membership did not enter clean reauthentication exactly once.");

    var unknownLeaveCatalog = new CustomerJourneyCatalogSource();
    var unknownLeaveAgent = new CustomerJourneyAgentClient(
        unknownLeaveCatalog)
    {
        AccountType = "ORGANIZATION",
        OrganizationRole = "MEMBER",
        OrganizationDisplayName = "Ambiguous Leave Org",
        OrganizationLeaveOutcomeUnknown = true,
    };
    var unknownLeaveViewModel = BuildCustomerJourneyViewModel(
        unknownLeaveAgent,
        unknownLeaveCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());
    await unknownLeaveViewModel.InitializeAsync(
        CancellationToken.None);
    await unknownLeaveViewModel.RefreshAccountOrganizationAsync(
        CancellationToken.None);
    await unknownLeaveViewModel.LeaveAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        unknownLeaveAgent.OrganizationLeaveCount == 1 &&
        !unknownLeaveAgent.Authenticated &&
        !unknownLeaveViewModel.IsAuthenticated &&
        unknownLeaveViewModel.Message.Contains(
            "will not replay",
            StringComparison.OrdinalIgnoreCase),
        "Ambiguous Organization self-leave became replayable or retained selected-account presentation.");

    ownerAgent.Authenticated = false;
    await ownerViewModel.RefreshAccountOrganizationAsync(
        CancellationToken.None);

    Require(
        !ownerViewModel.IsAuthenticated &&
        !ownerViewModel.OrganizationReady &&
        ownerViewModel.OrganizationMembers.Count == 0 &&
        ownerViewModel.OrganizationInvitations.Count == 0 &&
        !ownerViewModel.ShowOrganizationSection,
        "AUTH_REQUIRED did not clear Launcher organization presentation state.");

    var switchCatalog = new CustomerJourneyCatalogSource();
    var switchAgent = new CustomerJourneyAgentClient(switchCatalog)
    {
        AccountType = "ORGANIZATION",
        OrganizationRole = "OWNER",
        OrganizationDisplayName = "First Org",
    };
    var switchViewModel = BuildCustomerJourneyViewModel(
        switchAgent,
        switchCatalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());

    await switchViewModel.InitializeAsync(CancellationToken.None);
    await switchViewModel.RefreshAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        switchViewModel.OrganizationDisplayName == "First Org" &&
        switchViewModel.OrganizationMembers.Count == 2,
        "Account-switch organization certification did not load initial organization state.");

    switchAgent.OrganizationMemberManageOutcome =
        "LAST_OWNER_REQUIRED";
    await switchViewModel.ManageAccountOrganizationMemberAsync(
        switchViewModel.OrganizationMembers.Single(
            member => member.Email == "owner@example.test"),
        "UPDATE_ROLE",
        "MEMBER",
        CancellationToken.None);
    Require(
        switchViewModel.OrganizationMemberManagementStatus ==
            "LAST_OWNER_REQUIRED",
        "Account-switch certification did not create member-management presentation state.");
    switchAgent.OrganizationMemberManageOutcome = "READY";

    await switchViewModel.LeaveAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        switchAgent.OrganizationLeaveCount == 0 &&
        switchViewModel.OrganizationLeaveStatus == "INVALID_INPUT",
        "OWNER-only Organization state incorrectly submitted self-leave intent.");

    switchViewModel.OrganizationInvitationEmail =
        "switch-clear@example.test";
    await switchViewModel.CreateAccountOrganizationInvitationAsync(
        CancellationToken.None);
    Require(
        switchViewModel.HasOrganizationInvitationCode,
        "Account-switch certification did not create transient invitation code state.");

    await switchViewModel.SwitchAccountAsync(CancellationToken.None);

    Require(
        switchViewModel.SessionStatus == "SIGNED_OUT" &&
        !switchViewModel.OrganizationReady &&
        switchViewModel.OrganizationMembers.Count == 0 &&
        switchViewModel.OrganizationInvitations.Count == 0,
        "Safe account switching retained old organization presentation state.");
    Require(
        string.IsNullOrEmpty(
            switchViewModel.OrganizationEditDisplayName) &&
        string.IsNullOrEmpty(
            switchViewModel.OrganizationEditBillingEmail) &&
        switchViewModel.OrganizationProfileUpdateStatus == "IDLE",
        "Safe account switching retained organization-profile edit state.");
    Require(
        string.IsNullOrEmpty(
            switchViewModel.OrganizationInvitationEmail) &&
        string.IsNullOrEmpty(
            switchViewModel.OrganizationInvitationCode) &&
        switchViewModel.OrganizationInvitationStatus == "IDLE",
        "Safe account switching retained transient Organization invitation delivery state.");
    Require(
        switchViewModel.OrganizationMemberManagementStatus == "IDLE",
        "Safe account switching retained Organization member-management state.");
    Require(
        switchViewModel.OrganizationLeaveStatus == "IDLE" &&
        !switchViewModel.ShowOrganizationLeaveSection,
        "Safe account switching retained Organization self-leave presentation state.");

    switchAgent.Authenticated = true;
    switchAgent.OrganizationDisplayName = "Second Org";
    await switchViewModel.InitializeAsync(CancellationToken.None);

    Require(
        switchViewModel.IsAuthenticated &&
        switchViewModel.AccountOrganizationStatus == "UNKNOWN" &&
        switchViewModel.OrganizationMembers.Count == 0,
        "Fresh Organization authentication inherited old organization state.");

    await switchViewModel.RefreshAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        switchViewModel.OrganizationDisplayName == "Second Org",
        "Fresh Organization authentication did not load the new Agent-authoritative organization.");
}

static async Task CertifySafeAccountSwitchingAsync()
{
    var catalog = new CustomerJourneyCatalogSource();
    var agent = new CustomerJourneyAgentClient(catalog);
    var viewModel = BuildCustomerJourneyViewModel(
        agent,
        catalog,
        new CustomerJourneyRecoveryStore(),
        new CustomerJourneyNavigator());

    await viewModel.InitializeAsync(CancellationToken.None);

    Require(
        viewModel.IsAuthenticated &&
        viewModel.AccountTypeLabel == "Personal account" &&
        viewModel.CanSwitchAccount,
        "Safe account switching did not begin from an authenticated Personal account.");

    await viewModel.RefreshAccountPrivacyAsync(CancellationToken.None);
    Require(
        viewModel.AccountPrivacyRequestTypes.Count > 0,
        "Account-switch certification did not load account-scoped presentation state.");

    viewModel.Email = "form-state-must-not-win@example.test";
    viewModel.Password = "credential-must-clear";

    await viewModel.SwitchAccountAsync(CancellationToken.None);

    Require(
        agent.LogoutCount == 1 &&
        !agent.Authenticated &&
        viewModel.SessionStatus == "SIGNED_OUT" &&
        viewModel.ShowLoginPage &&
        !viewModel.ShowAuthenticatedShell,
        "Safe account switching did not clear the current Agent session exactly once.");
    Require(
        viewModel.Email == "customer@example.test" &&
        string.IsNullOrEmpty(viewModel.Password) &&
        string.IsNullOrEmpty(viewModel.NativeMfaCode) &&
        string.IsNullOrEmpty(viewModel.AccountTypeLabel),
        "Safe account switching preserved more than the authenticated account email.");
    Require(
        viewModel.AccountPrivacyRequestTypes.Count == 0 &&
        viewModel.AccountPrivacyRequests.Count == 0,
        "Safe account switching retained account-scoped privacy presentation state.");
    Require(
        viewModel.Message.Contains(
            "fresh",
            StringComparison.OrdinalIgnoreCase) ||
        viewModel.Message.Contains(
            "password",
            StringComparison.OrdinalIgnoreCase),
        "Safe account switching did not tell the customer to authenticate again.");

    var lockedCatalog = new CustomerJourneyCatalogSource();
    var lockedAgent = new CustomerJourneyAgentClient(lockedCatalog)
    {
        AccountType = "ORGANIZATION",
        OrganizationRole = "MEMBER",
        OrganizationDisplayName = "Checkout Locked Org",
    };
    var recovery = new CustomerJourneyRecoveryStore();
    var navigator = new CustomerJourneyNavigator();
    var lockedViewModel = BuildCustomerJourneyViewModel(
        lockedAgent,
        lockedCatalog,
        recovery,
        navigator);

    await lockedViewModel.InitializeAsync(CancellationToken.None);
    await lockedViewModel.RefreshAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        lockedViewModel.CanLeaveOrganization,
        "Checkout-lock certification did not begin with an otherwise leave-authorized Organization membership.");
    await lockedViewModel.OpenModuleAsync(2, CancellationToken.None);
    await lockedViewModel.ReviewPurchaseAsync(
        CustomerJourneyAgentClient.PurchasePlanId,
        CancellationToken.None);
    foreach (var document in lockedViewModel.PurchaseLegalDocuments)
    {
        document.IsAccepted = true;
    }
    await lockedViewModel.StartPurchaseAsync(
        "SELF",
        CancellationToken.None);

    Require(
        recovery.State is not null &&
        !lockedViewModel.CanSwitchAccount &&
        !lockedViewModel.CanLeaveOrganization,
        "Checkout recovery did not lock account switching and Organization self-leave to the current identity/account.");

    await lockedViewModel.LeaveAccountOrganizationAsync(
        CancellationToken.None);
    Require(
        lockedAgent.OrganizationLeaveCount == 0 &&
        lockedAgent.Authenticated &&
        lockedViewModel.IsAuthenticated &&
        lockedViewModel.OrganizationLeaveStatus == "BLOCKED" &&
        lockedViewModel.OrganizationLeaveMessage.Contains(
            "Resolve the existing checkout attempt",
            StringComparison.Ordinal),
        "Launcher submitted Organization self-leave while checkout recovery remained account-bound.");

    await lockedViewModel.SwitchAccountAsync(CancellationToken.None);

    Require(
        lockedAgent.LogoutCount == 0 &&
        lockedAgent.Authenticated &&
        lockedViewModel.IsAuthenticated &&
        lockedViewModel.Message.Contains(
            "Resolve the existing checkout attempt",
            StringComparison.Ordinal),
        "Launcher allowed account switching while checkout recovery was bound to the current account.");
}

static async Task CertifyCustomerAcquisitionToMySoftwareAsync()
{
    await CertifySelfPurchaseRefreshesMySoftwareAsync();
    await CertifyGiftPurchaseStaysUnboundAsync();
    await CertifyClaimCodeRedemptionRefreshesMySoftwareAsync();
}

static async Task CertifySelfPurchaseRefreshesMySoftwareAsync()
{
    var catalog = new CustomerJourneyCatalogSource();
    var agent = new CustomerJourneyAgentClient(catalog);
    var recovery = new CustomerJourneyRecoveryStore();
    var navigator = new CustomerJourneyNavigator();
    var viewModel = BuildCustomerJourneyViewModel(agent, catalog, recovery, navigator);

    await viewModel.InitializeAsync(CancellationToken.None);
    Require(viewModel.IsAuthenticated, "Customer journey did not enter the authenticated BKE shell.");
    Require(viewModel.SelectedModuleIndex == -1,
        "Authenticated startup auto-selected a customer destination.");

    await viewModel.OpenModuleAsync(0, CancellationToken.None);
    Require(viewModel.Products.Count == 1 &&
            viewModel.Products[0].StateLabel == "Not entitled" &&
            !viewModel.Products[0].CanInstall,
        "SELF journey did not begin without software ownership.");

    await viewModel.OpenModuleAsync(2, CancellationToken.None);
    Require(viewModel.StoreProducts.Count == 1,
        "SELF journey could not load the Agent-mediated Store.");

    await viewModel.ReviewPurchaseAsync(
        CustomerJourneyAgentClient.PurchasePlanId,
        CancellationToken.None);
    foreach (var document in viewModel.PurchaseLegalDocuments)
    {
        document.IsAccepted = true;
    }

    Require(viewModel.CanBuySelf,
        "SELF journey did not expose purchase intent after current review and Legal acceptance.");

    await viewModel.StartPurchaseAsync("SELF", CancellationToken.None);
    Require(agent.CheckoutStartCount == 1,
        "SELF journey did not create exactly one checkout mutation.");
    Require(recovery.State is not null,
        "SELF journey did not retain checkout recovery state before settlement.");
    Require(navigator.LastCheckoutUrl is not null,
        "SELF journey did not open the existing secure checkout returned by authority.");

    await viewModel.CheckPurchaseCheckoutStatusAsync(CancellationToken.None);

    Require(agent.CheckoutStartCount == 1,
        "SELF settlement recovery created a duplicate checkout mutation.");
    Require(catalog.Owned,
        "SELF settlement did not make the authoritative catalog source expose ownership.");
    Require(recovery.State is null,
        "SELF settlement did not clear the resolved checkout recovery lock.");
    Require(viewModel.Products.Count == 1 &&
            viewModel.Products[0].StateLabel == "Installable" &&
            viewModel.Products[0].CanInstall,
        "SELF settlement did not refresh the acquired entitlement into My Software.");
}

static async Task CertifyGiftPurchaseStaysUnboundAsync()
{
    var catalog = new CustomerJourneyCatalogSource();
    var agent = new CustomerJourneyAgentClient(catalog);
    var recovery = new CustomerJourneyRecoveryStore();
    var navigator = new CustomerJourneyNavigator();
    var viewModel = BuildCustomerJourneyViewModel(agent, catalog, recovery, navigator);

    await viewModel.InitializeAsync(CancellationToken.None);
    await viewModel.OpenModuleAsync(0, CancellationToken.None);
    await viewModel.OpenModuleAsync(2, CancellationToken.None);
    await viewModel.ReviewPurchaseAsync(
        CustomerJourneyAgentClient.PurchasePlanId,
        CancellationToken.None);
    foreach (var document in viewModel.PurchaseLegalDocuments)
    {
        document.IsAccepted = true;
    }

    Require(viewModel.CanBuyGift,
        "GIFT journey did not expose Claim Code purchase after current review and Legal acceptance.");

    await viewModel.StartPurchaseAsync("GIFT", CancellationToken.None);
    await viewModel.CheckPurchaseCheckoutStatusAsync(CancellationToken.None);

    Require(agent.CheckoutStartCount == 1,
        "GIFT settlement recovery created a duplicate checkout mutation.");
    Require(!catalog.Owned,
        "GIFT settlement incorrectly granted the purchaser software ownership.");
    Require(viewModel.Products.Count == 1 &&
            viewModel.Products[0].StateLabel == "Not entitled",
        "GIFT settlement altered the purchaser's My Software entitlement.");
    Require(viewModel.GiftClaimCode == CustomerJourneyAgentClient.GiftClaimCode,
        "GIFT settlement did not reveal the unbound one-time Claim Code.");
    Require(recovery.State is not null && viewModel.CanCompleteGiftDelivery,
        "GIFT recovery was discarded before the purchaser acknowledged Claim Code delivery.");

    viewModel.CompleteGiftClaimDelivery();

    Require(string.IsNullOrEmpty(viewModel.GiftClaimCode),
        "Acknowledged GIFT delivery retained Claim Code plaintext in Launcher state.");
    Require(recovery.State is null,
        "Acknowledged GIFT delivery did not clear the resolved recovery state.");
}

static async Task CertifyClaimCodeRedemptionRefreshesMySoftwareAsync()
{
    var catalog = new CustomerJourneyCatalogSource();
    var agent = new CustomerJourneyAgentClient(catalog);
    var recovery = new CustomerJourneyRecoveryStore();
    var navigator = new CustomerJourneyNavigator();
    var viewModel = BuildCustomerJourneyViewModel(agent, catalog, recovery, navigator);

    await viewModel.InitializeAsync(CancellationToken.None);
    await viewModel.OpenModuleAsync(0, CancellationToken.None);
    Require(viewModel.Products.Count == 1 &&
            viewModel.Products[0].StateLabel == "Not entitled",
        "Claim Code recipient did not begin without software ownership.");

    viewModel.ClaimCode = CustomerJourneyAgentClient.GiftClaimCode;
    await viewModel.RedeemClaimCodeAsync(CancellationToken.None);

    Require(agent.RedeemCount == 1,
        "Claim Code redemption did not delegate exactly once to the Agent boundary.");
    Require(catalog.Owned,
        "Claim Code redemption did not make the authoritative catalog source expose ownership.");
    Require(viewModel.ClaimStatus == "CLAIMED" &&
            string.IsNullOrEmpty(viewModel.ClaimCode),
        "Successful Claim Code redemption retained plaintext or failed to report CLAIMED.");
    Require(viewModel.Products.Count == 1 &&
            viewModel.Products[0].StateLabel == "Installable" &&
            viewModel.Products[0].CanInstall,
        "Claim Code redemption did not refresh the recipient entitlement into My Software.");
}

static MainWindowViewModel BuildCustomerJourneyViewModel(
    CustomerJourneyAgentClient agent,
    CustomerJourneyCatalogSource catalog,
    CustomerJourneyRecoveryStore recovery,
    CustomerJourneyNavigator navigator,
    CustomerJourneyIdentityClient? identity = null)
{
    var platformAuthority = new LauncherPlatformAuthorityResolver(agent);
    identity ??= new CustomerJourneyIdentityClient();
    return new MainWindowViewModel(
        new LauncherAccountSessionController(agent),
        new LauncherNativeSignInController(
            agent,
            platformAuthority,
            identity),
        new LauncherNativeRegistrationController(
            platformAuthority,
            identity),
        new LauncherPasswordResetRequestController(
            platformAuthority,
            identity),
        new LauncherCatalogService(catalog),
        new LauncherStoreService(agent),
        new LauncherStoreCheckoutReviewService(agent),
        new LauncherStoreCheckoutStartService(agent),
        new LauncherStoreCheckoutStatusService(agent),
        new LauncherStoreGiftClaimRevealService(agent),
        new LauncherNotificationInboxService(agent),
        recovery,
        navigator,
        new LauncherSoftwareInstallController(agent),
        new LauncherSoftwareUpdateController(agent),
        new LauncherSoftwareRepairController(agent),
        new LauncherSoftwareOpenController(agent),
        new LauncherSoftwareRemoveController(agent),
        new LauncherClaimCodeRedemptionController(agent),
        new LauncherAccountPasswordChangeController(agent),
        new LauncherAccountMfaController(agent),
        new LauncherAccountPrivacyController(agent),
        new LauncherAccountOrganizationController(agent));
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}

sealed class CustomerJourneyCatalogSource : ILauncherCatalogSource
{
    public bool Owned { get; set; }

    public Task<LauncherCatalogSnapshot> GetProductsAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new LauncherCatalogSnapshot(
            "READY",
            new[]
            {
                new LauncherProduct(
                    CustomerJourneyAgentClient.ProductId,
                    "Render Dock",
                    "Certification product",
                    ProductExecutionType.Standalone,
                    Owned ? LauncherProductState.Installable : LauncherProductState.NotEntitled,
                    null,
                    "1.0.3"),
            },
            null));
    }
}

sealed class CustomerJourneyRecoveryStore : ILauncherCheckoutRecoveryStore
{
    public LauncherCheckoutRecoveryState? State { get; private set; }

    public LauncherCheckoutRecoveryState? Read() => State;

    public void Write(LauncherCheckoutRecoveryState state)
    {
        State = state;
    }

    public void Clear()
    {
        State = null;
    }
}

sealed class CustomerJourneyNavigator : ILauncherExternalNavigator
{
    public string? LastCheckoutUrl { get; private set; }

    public void OpenCheckout(string absoluteUrl)
    {
        LastCheckoutUrl = absoluteUrl;
    }

    public Task OpenLegalDocumentAsync(
        string slug,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}

sealed class CustomerJourneyIdentityClient :
    ILauncherIdentityClient,
    ILauncherRegistrationClient
{
    public int ResetRequestCount { get; private set; }
    public int RegistrationPreflightCount { get; private set; }
    public int RegistrationCount { get; private set; }
    public int VerificationCount { get; private set; }
    public int ResendCount { get; private set; }
    public Uri? LastRegistrationAuthority { get; private set; }
    public NativeBkeRegistrationRequest? LastRegistrationRequest { get; private set; }
    public string? LastVerificationCode { get; private set; }
    public string? LastResetEmail { get; private set; }
    public Uri? LastResetAuthority { get; private set; }

    public Task<NativeBkeLoginResponse> LoginAsync(
        Uri platformBaseAddress,
        NativeBkeLoginRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException(
            "Native credential login is outside this customer journey certification.");

    public Task<NativeBkeMfaVerifyResponse> VerifyMfaAsync(
        Uri platformBaseAddress,
        NativeBkeMfaVerifyRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<NativeBkeRegistrationPreflightResponse> GetRegistrationPreflightAsync(
        Uri platformBaseAddress,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RegistrationPreflightCount++;
        LastRegistrationAuthority = platformBaseAddress;
        return Task.FromResult(new NativeBkeRegistrationPreflightResponse(
            "ready",
            new[]
            {
                new NativeBkeRegistrationLegalDocument(
                    "TERMS_OF_SERVICE",
                    "Terms of Service",
                    "terms-of-service",
                    "terms-current",
                    7,
                    "2026-09-29T00:00:00.000Z",
                    "# Terms of Service\n\nCurrent certification terms."),
                new NativeBkeRegistrationLegalDocument(
                    "PRIVACY_POLICY",
                    "Privacy Policy",
                    "privacy-policy",
                    "privacy-current",
                    4,
                    "2026-09-29T00:00:00.000Z",
                    "# Privacy Policy\n\nCurrent certification privacy policy."),
            },
            null));
    }

    public Task<NativeBkeRegistrationResponse> RegisterAsync(
        Uri platformBaseAddress,
        NativeBkeRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RegistrationCount++;
        LastRegistrationAuthority = platformBaseAddress;
        LastRegistrationRequest = request;
        return Task.FromResult(
            new NativeBkeRegistrationResponse(
                "verification_required",
                null));
    }

    public Task<NativeBkeEmailVerificationResponse> VerifyEmailAsync(
        Uri platformBaseAddress,
        NativeBkeEmailVerificationRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        VerificationCount++;
        LastRegistrationAuthority = platformBaseAddress;
        LastVerificationCode = request.Code;
        return Task.FromResult(
            new NativeBkeEmailVerificationResponse("verified", null));
    }

    public Task<NativeBkeVerificationResendResponse> ResendVerificationAsync(
        Uri platformBaseAddress,
        NativeBkeVerificationResendRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ResendCount++;
        LastRegistrationAuthority = platformBaseAddress;
        return Task.FromResult(
            new NativeBkeVerificationResendResponse("accepted", null));
    }

    public Task<NativeBkePasswordResetResponse> RequestPasswordResetAsync(
        Uri platformBaseAddress,
        NativeBkePasswordResetRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ResetRequestCount++;
        LastResetEmail = request.Email;
        LastResetAuthority = platformBaseAddress;
        return Task.FromResult(
            new NativeBkePasswordResetResponse("accepted", null));
    }
}

sealed class CustomerJourneyAgentClient : ILauncherAgentClient
{
    public const string ProductId = "bke-render-dock";
    public const string PurchasePlanId = "plan-cert-render-dock";
    public const string GiftClaimCode = "BKE-CLM-CERT1-CERT2-CERT3-CERT4-CERT5-CERT6";
    public const string OrganizationInvitationHandle =
        "bke-org-invite-v1_aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
    public const string NewOrganizationInvitationHandle =
        "bke-org-invite-v1_bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb";
    public const string OrganizationOwnerMemberHandle =
        "bke-org-member-v1_cccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccccc";
    public const string OrganizationManagedMemberHandle =
        "bke-org-member-v1_dddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddddd";

    private const string AccountId = "acct-cert-recipient";
    private readonly CustomerJourneyCatalogSource _catalog;
    private string _purchaseMode = "SELF";

    public CustomerJourneyAgentClient(CustomerJourneyCatalogSource catalog)
    {
        _catalog = catalog;
    }

    public int CheckoutStartCount { get; private set; }
    public int LogoutCount { get; private set; }
    public int RedeemCount { get; private set; }
    public int PasswordChangeCount { get; private set; }
    public int PrivacyListCount { get; private set; }
    public int PrivacyCreateCount { get; private set; }
    public int OrganizationReadCount { get; private set; }
    public int OrganizationCreateCount { get; private set; }
    public int OrganizationProfileUpdateCount { get; private set; }
    public int OrganizationInvitationCreateCount { get; private set; }
    public int OrganizationInvitationManageCount { get; private set; }
    public int OrganizationMemberManageCount { get; private set; }
    public int OrganizationLeaveCount { get; private set; }
    public int PlatformAuthorityCount { get; private set; }
    public bool Authenticated { get; set; } = true;
    public string AccountType { get; set; } = "INDIVIDUAL";
    public string OrganizationRole { get; set; } = "OWNER";
    public string OrganizationDisplayName { get; set; } =
        "Certification Organization";
    public string OrganizationLegalName { get; set; } =
        "Certification Organization Legal";
    public string? OrganizationRegistrationNumber { get; set; } =
        "REG-001";
    public string? OrganizationBillingEmail { get; set; } =
        "billing@example.test";
    public string? OrganizationTaxId { get; set; } =
        "TAX-001";
    public string OrganizationProfileUpdateOutcome { get; set; } =
        "UPDATED";
    public bool OrganizationProfileUpdateOutcomeUnknown { get; set; }
    public AccountOrganizationProfileUpdateRequest? LastOrganizationProfileUpdateRequest
        { get; private set; }
    public string OrganizationInvitationOutcome { get; set; } =
        "CREATED";
    public bool OrganizationInvitationOutcomeUnknown { get; set; }
    public bool OrganizationInvitationIssued { get; private set; }
    public bool OrganizationInvitationManageOutcomeUnknown { get; set; }
    public string OrganizationInvitationManageOutcome { get; set; } =
        "READY";
    public AccountOrganizationInvitationCreateRequest? LastOrganizationInvitationRequest
        { get; private set; }
    public AccountOrganizationInvitationManageRequest? LastOrganizationInvitationManageRequest
        { get; private set; }
    public string OrganizationMemberManageOutcome { get; set; } =
        "READY";
    public bool OrganizationMemberManageOutcomeUnknown { get; set; }
    public bool OrganizationManagedMemberExists { get; private set; } = true;
    public string OrganizationManagedMemberRole { get; private set; } =
        "MEMBER";
    public AccountOrganizationMemberManageRequest? LastOrganizationMemberManageRequest
        { get; private set; }
    public string OrganizationLeaveOutcome { get; set; } = "LEFT";
    public bool OrganizationLeaveOutcomeUnknown { get; set; }
    public AccountOrganizationLeaveRequest? LastOrganizationLeaveRequest
        { get; private set; }
    public string OrganizationCreateOutcome { get; set; } =
        "CREATED";
    public bool OrganizationCreateOutcomeUnknown { get; set; }
    public AccountOrganizationCreateRequest? LastOrganizationCreateRequest
        { get; private set; }
    public bool PrivacyCreateOutcomeUnknown { get; set; }
    private readonly List<AccountPrivacyItem> _privacyRequests = [];
    public string PasswordChangeOutcome { get; set; } = "CHANGED";
    public AccountPasswordChangeRequest? LastPasswordChangeRequest { get; private set; }

    public Task<AccountSessionStatusResponse> GetAccountSessionStatusAsync(
        AccountSessionStatusRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Authenticated
            ? new AccountSessionStatusResponse(
                AgentLocalContract.CapabilityId,
                AgentLocalContract.ContractVersion,
                "AUTHENTICATED",
                new AccountSessionAccount(
                    "user-cert",
                    "customer@example.test",
                    AccountId,
                    AccountType,
                    AccountType == "ORGANIZATION"
                        ? OrganizationDisplayName
                        : "Certification Customer"),
                null)
            : new AccountSessionStatusResponse(
                AgentLocalContract.CapabilityId,
                AgentLocalContract.ContractVersion,
                "SIGNED_OUT",
                null,
                null));
    }

    public Task<StoreCatalogResponse> GetStoreCatalogAsync(
        StoreCatalogRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new StoreCatalogResponse(
            AgentLocalContract.StoreCatalogCapabilityId,
            AgentLocalContract.StoreCatalogContractVersion,
            "READY",
            true,
            new[]
            {
                new StoreCatalogProduct(
                    ProductId,
                    "render-dock",
                    "Render Dock",
                    "Certification product",
                    "Stateful customer journey certification product.",
                    "SOFTWARE",
                    ProductExecutionTypeWire.Standalone,
                    new[]
                    {
                        new StoreCatalogEdition(
                            "edition-cert-render-dock",
                            "standard",
                            "Standard",
                            "Certification edition",
                            new[] { "render" },
                            1,
                            2,
                            "ACTIVE",
                            new[]
                            {
                                new StoreCatalogPlan(
                                    PurchasePlanId,
                                    "PERPETUAL",
                                    "PHP",
                                    30000,
                                    "ONE_TIME",
                                    null,
                                    null,
                                    "NONE",
                                    0,
                                    null),
                            }),
                    }),
            },
            null));
    }

    public Task<StoreCheckoutReviewResponse> ReviewStoreCheckoutAsync(
        StoreCheckoutReviewRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (request.PurchasePlanId != PurchasePlanId)
        {
            throw new InvalidOperationException("Unexpected certification purchase plan.");
        }

        var plan = new StoreCatalogPlan(
            PurchasePlanId,
            "PERPETUAL",
            "PHP",
            30000,
            "ONE_TIME",
            null,
            null,
            "NONE",
            0,
            null);
        return Task.FromResult(new StoreCheckoutReviewResponse(
            AgentLocalContract.StoreCheckoutReviewCapabilityId,
            AgentLocalContract.StoreCheckoutReviewContractVersion,
            "READY",
            new[] { "SELF", "GIFT" },
            new StoreCheckoutReviewProduct(
                ProductId,
                "render-dock",
                "Render Dock",
                "Certification product"),
            new StoreCheckoutReviewEdition(
                "edition-cert-render-dock",
                "standard",
                "Standard",
                1,
                2,
                "ACTIVE"),
            plan,
            new[]
            {
                new StoreCheckoutReviewLegalDocument(
                    "TERMS",
                    "Terms of Service",
                    "terms",
                    "legal-terms-v1",
                    "1.0",
                    null,
                    false),
                new StoreCheckoutReviewLegalDocument(
                    "PRIVACY",
                    "Privacy Policy",
                    "privacy",
                    "legal-privacy-v1",
                    "1.0",
                    null,
                    false),
            },
            Array.Empty<StoreCheckoutReviewPendingLegalDocument>(),
            null));
    }

    public Task<StoreCheckoutStartResponse> StartStoreCheckoutAsync(
        StoreCheckoutStartRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        CheckoutStartCount++;
        _purchaseMode = request.PurchaseMode;

        return Task.FromResult(new StoreCheckoutStartResponse(
            AgentLocalContract.StoreCheckoutStartCapabilityId,
            AgentLocalContract.StoreCheckoutStartContractVersion,
            "READY",
            request.CorrelationId,
            _purchaseMode == "GIFT" ? "order-gift-cert" : "order-self-cert",
            "https://checkout.example.test/existing",
            false,
            null));
    }

    public Task<StoreCheckoutStatusResponse> CheckStoreCheckoutStatusAsync(
        StoreCheckoutStatusRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var gift = _purchaseMode == "GIFT";
        if (!gift)
        {
            _catalog.Owned = true;
        }

        return Task.FromResult(new StoreCheckoutStatusResponse(
            AgentLocalContract.StoreCheckoutStatusCapabilityId,
            AgentLocalContract.StoreCheckoutStatusContractVersion,
            "FOUND",
            request.CorrelationId,
            gift ? "order-gift-cert" : "order-self-cert",
            gift ? "BKE-2026-GIFT-CERT" : "BKE-2026-SELF-CERT",
            "FULFILLED",
            gift ? "CLAIM_CODE" : "ACCOUNT_ENTITLEMENT",
            "SETTLED",
            null,
            "2026-09-28T12:00:00Z",
            null));
    }

    public Task<StoreGiftClaimRevealResponse> RevealStoreGiftClaimCodeAsync(
        StoreGiftClaimRevealRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(new StoreGiftClaimRevealResponse(
            AgentLocalContract.StoreGiftClaimRevealCapabilityId,
            AgentLocalContract.StoreGiftClaimRevealContractVersion,
            "AVAILABLE",
            request.CorrelationId,
            "order-gift-cert",
            "claim-cert",
            GiftClaimCode,
            null));
    }

    public Task<ClaimCodeRedeemResponse> RedeemClaimCodeAsync(
        ClaimCodeRedeemRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        RedeemCount++;

        if (!string.Equals(request.Code, GiftClaimCode, StringComparison.Ordinal))
        {
            return Task.FromResult(new ClaimCodeRedeemResponse(
                AgentLocalContract.ClaimCodeRedemptionCapabilityId,
                AgentLocalContract.ClaimCodeRedemptionContractVersion,
                "NOT_FOUND",
                null,
                null,
                new ClaimCodeRedeemError(
                    "NOT_FOUND",
                    "Claim Code not found.",
                    false)));
        }

        _catalog.Owned = true;
        return Task.FromResult(new ClaimCodeRedeemResponse(
            AgentLocalContract.ClaimCodeRedemptionCapabilityId,
            AgentLocalContract.ClaimCodeRedemptionContractVersion,
            "CLAIMED",
            AccountId,
            "entitlement-cert",
            null));
    }

    public Task<PlatformAuthorityResponse> GetPlatformAuthorityAsync(
        PlatformAuthorityRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PlatformAuthorityCount++;
        return Task.FromResult(new PlatformAuthorityResponse(
            AgentLocalContract.PlatformAuthorityCapabilityId,
            AgentLocalContract.PlatformAuthorityContractVersion,
            "READY",
            "certification",
            "https://digital-solutions.example.test/",
            null));
    }

    public Task<AccountSessionDeviceContextResponse> GetAccountSessionDeviceContextAsync(
        AccountSessionDeviceContextRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountSessionCompleteResponse> CompleteAccountSessionAsync(
        AccountSessionCompleteRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountSessionStartResponse> StartAccountSessionAsync(
        AccountSessionStartRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountSessionLogoutResponse> LogoutAccountSessionAsync(
        AccountSessionLogoutRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        LogoutCount++;
        Authenticated = false;
        return Task.FromResult(new AccountSessionLogoutResponse(
            AgentLocalContract.CapabilityId,
            AgentLocalContract.ContractVersion,
            "SIGNED_OUT",
            null));
    }

    public Task<AccountPasswordChangeResponse> ChangeAccountPasswordAsync(
        AccountPasswordChangeRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PasswordChangeCount++;
        LastPasswordChangeRequest = request;

        return Task.FromResult(PasswordChangeOutcome switch
        {
            "CHANGED" => new AccountPasswordChangeResponse(
                AgentLocalContract.AccountPasswordChangeCapabilityId,
                AgentLocalContract.AccountPasswordChangeContractVersion,
                "CHANGED",
                true,
                null),
            "INVALID_CREDENTIALS" => new AccountPasswordChangeResponse(
                AgentLocalContract.AccountPasswordChangeCapabilityId,
                AgentLocalContract.AccountPasswordChangeContractVersion,
                "INVALID_CREDENTIALS",
                false,
                new AccountPasswordChangeError(
                    "INVALID_CREDENTIALS",
                    "The current password was not accepted.",
                    false)),
            "INVALID_INPUT" => new AccountPasswordChangeResponse(
                AgentLocalContract.AccountPasswordChangeCapabilityId,
                AgentLocalContract.AccountPasswordChangeContractVersion,
                "INVALID_INPUT",
                false,
                new AccountPasswordChangeError(
                    "INVALID_INPUT",
                    "The new password is invalid.",
                    false)),
            _ => new AccountPasswordChangeResponse(
                AgentLocalContract.AccountPasswordChangeCapabilityId,
                AgentLocalContract.AccountPasswordChangeContractVersion,
                "REAUTHENTICATION_REQUIRED",
                true,
                new AccountPasswordChangeError(
                    "PASSWORD_CHANGE_OUTCOME_UNKNOWN",
                    "Sign in again before retrying.",
                    false)),
        });
    }

    public Task<AccountMfaStatusResponse> GetAccountMfaStatusAsync(
        AccountMfaStatusRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountMfaChallengeResponse> StartAccountMfaEnrollmentAsync(
        AccountMfaEnrollStartRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountMfaMutationResponse> CompleteAccountMfaEnrollmentAsync(
        AccountMfaEnrollCompleteRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountMfaChallengeResponse> StartAccountMfaProofAsync(
        AccountMfaProofChallengeRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountMfaMutationResponse> DisableAccountMfaAsync(
        AccountMfaMutationRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountMfaMutationResponse> RegenerateAccountMfaRecoveryAsync(
        AccountMfaMutationRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountPrivacyListResponse> GetAccountPrivacyRequestsAsync(
        AccountPrivacyListRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PrivacyListCount++;

        if (!Authenticated)
        {
            return Task.FromResult(new AccountPrivacyListResponse(
                AgentLocalContract.AccountPrivacyCapabilityId,
                AgentLocalContract.AccountPrivacyContractVersion,
                "AUTH_REQUIRED",
                Array.Empty<string>(),
                Array.Empty<AccountPrivacyItem>(),
                new AccountPrivacyError(
                    "SESSION_INVALID",
                    "Sign in again.",
                    false)));
        }

        return Task.FromResult(new AccountPrivacyListResponse(
            AgentLocalContract.AccountPrivacyCapabilityId,
            AgentLocalContract.AccountPrivacyContractVersion,
            "READY",
            new[] { "ACCESS", "EXPORT", "DELETION" },
            _privacyRequests.Take(request.Limit).ToArray(),
            null));
    }

    public Task<AccountPrivacyCreateResponse> CreateAccountPrivacyRequestAsync(
        AccountPrivacyCreateRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        PrivacyCreateCount++;

        if (!Authenticated)
        {
            return Task.FromResult(new AccountPrivacyCreateResponse(
                AgentLocalContract.AccountPrivacyCapabilityId,
                AgentLocalContract.AccountPrivacyContractVersion,
                "AUTH_REQUIRED",
                null,
                null,
                null,
                new AccountPrivacyError(
                    "SESSION_INVALID",
                    "Sign in again.",
                    false)));
        }

        if (PrivacyCreateOutcomeUnknown)
        {
            PrivacyCreateOutcomeUnknown = false;
            return Task.FromResult(new AccountPrivacyCreateResponse(
                AgentLocalContract.AccountPrivacyCapabilityId,
                AgentLocalContract.AccountPrivacyContractVersion,
                "OUTCOME_UNKNOWN",
                null,
                null,
                null,
                new AccountPrivacyError(
                    "PRIVACY_CREATE_OUTCOME_UNKNOWN",
                    "The result could not be confirmed.",
                    false)));
        }

        var id = $"privacy-cert-{_privacyRequests.Count + 1}";
        var item = new AccountPrivacyItem(
            id,
            "ACCOUNT",
            request.RequestType,
            "OPEN",
            request.Summary,
            null,
            null,
            null,
            "2026-09-29T12:00:00.000Z");
        _privacyRequests.Insert(0, item);

        return Task.FromResult(new AccountPrivacyCreateResponse(
            AgentLocalContract.AccountPrivacyCapabilityId,
            AgentLocalContract.AccountPrivacyContractVersion,
            "CREATED",
            id,
            request.RequestType,
            "OPEN",
            null));
    }

    public Task<AccountOrganizationOverviewResponse> GetAccountOrganizationAsync(
        AccountOrganizationOverviewRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OrganizationReadCount++;

        if (!Authenticated)
        {
            return Task.FromResult(new AccountOrganizationOverviewResponse(
                AgentLocalContract.AccountOrganizationCapabilityId,
                AgentLocalContract.AccountOrganizationContractVersion,
                "AUTH_REQUIRED",
                null,
                null,
                null,
                null,
                null,
                null,
                Array.Empty<AccountOrganizationMember>(),
                Array.Empty<AccountOrganizationInvitation>(),
                new AccountOrganizationError(
                    "SESSION_INVALID",
                    "Sign in again.",
                    false)));
        }

        if (AccountType != "ORGANIZATION")
        {
            return Task.FromResult(new AccountOrganizationOverviewResponse(
                AgentLocalContract.AccountOrganizationCapabilityId,
                AgentLocalContract.AccountOrganizationContractVersion,
                "NOT_ORGANIZATION",
                null,
                null,
                null,
                null,
                null,
                null,
                Array.Empty<AccountOrganizationMember>(),
                Array.Empty<AccountOrganizationInvitation>(),
                null));
        }

        var owner = OrganizationRole == "OWNER";
        var billing = OrganizationRole == "BILLING";
        var licenseManager = OrganizationRole == "LICENSE_MANAGER";
        var manageMembers = owner;
        var leaveOrganization = !owner;
        var viewBilling = owner || billing;
        var viewLicenses = owner || licenseManager;

        return Task.FromResult(new AccountOrganizationOverviewResponse(
            AgentLocalContract.AccountOrganizationCapabilityId,
            AgentLocalContract.AccountOrganizationContractVersion,
            "READY",
            new AccountOrganizationAccount(
                OrganizationDisplayName,
                "ACTIVE",
                OrganizationRole),
            new AccountOrganizationPermissions(
                manageMembers,
                leaveOrganization,
                viewBilling,
                viewLicenses),
            new AccountOrganizationProfile(
                OrganizationLegalName,
                OrganizationRegistrationNumber),
            viewBilling ? OrganizationBillingEmail : null,
            viewBilling ? OrganizationTaxId : null,
            new AccountOrganizationCounts(
                viewLicenses ? 2 : null,
                viewBilling || viewLicenses ? 4 : null,
                viewBilling ? 3 : null),
            manageMembers
                ? OrganizationManagedMemberExists
                    ? new[]
                    {
                        new AccountOrganizationMember(
                            "owner@example.test",
                            "Owner",
                            "OWNER",
                            OrganizationOwnerMemberHandle),
                        new AccountOrganizationMember(
                            "manager@example.test",
                            "Manager",
                            OrganizationManagedMemberRole,
                            OrganizationManagedMemberHandle),
                    }
                    : new[]
                    {
                        new AccountOrganizationMember(
                            "owner@example.test",
                            "Owner",
                            "OWNER",
                            OrganizationOwnerMemberHandle),
                    }
                : Array.Empty<AccountOrganizationMember>(),
            manageMembers
                ? OrganizationInvitationIssued
                    ? new[]
                    {
                        new AccountOrganizationInvitation(
                            "invitee@example.test",
                            "MEMBER",
                            "PENDING",
                            "2026-10-01T12:00:00.000Z",
                            "2026-09-29T12:00:00.000Z",
                            OrganizationInvitationHandle),
                        new AccountOrganizationInvitation(
                            "new-member@example.test",
                            "MEMBER",
                            "PENDING",
                            "2026-10-07T00:00:00.000Z",
                            "2026-09-30T00:00:00.000Z",
                            NewOrganizationInvitationHandle),
                    }
                    : new[]
                    {
                        new AccountOrganizationInvitation(
                            "invitee@example.test",
                            "MEMBER",
                            "PENDING",
                            "2026-10-01T12:00:00.000Z",
                            "2026-09-29T12:00:00.000Z",
                            OrganizationInvitationHandle),
                    }
                : Array.Empty<AccountOrganizationInvitation>(),
            null));
    }

    public Task<AccountOrganizationCreateResponse> CreateAccountOrganizationAsync(
        AccountOrganizationCreateRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OrganizationCreateCount++;
        LastOrganizationCreateRequest = request;

        if (!Authenticated)
        {
            return Task.FromResult(
                new AccountOrganizationCreateResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "AUTH_REQUIRED",
                    null,
                    false,
                    new AccountOrganizationError(
                        "SESSION_INVALID",
                        "Sign in again.",
                        false)));
        }

        if (OrganizationCreateOutcomeUnknown)
        {
            OrganizationCreateOutcomeUnknown = false;
            return Task.FromResult(
                new AccountOrganizationCreateResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "OUTCOME_UNKNOWN",
                    null,
                    false,
                    new AccountOrganizationError(
                        "ORGANIZATION_CREATE_OUTCOME_UNKNOWN",
                        "The creation result could not be confirmed.",
                        false)));
        }

        if (OrganizationCreateOutcome != "CREATED")
        {
            var message = OrganizationCreateOutcome switch
            {
                "EMAIL_NOT_VERIFIED" =>
                    "Verify your BKE email before creating an organization.",
                "LEGAL_REACCEPTANCE_REQUIRED" =>
                    "Accept the current BKE Legal documents before creating an organization.",
                _ =>
                    "The organization was not created.",
            };
            return Task.FromResult(
                new AccountOrganizationCreateResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    OrganizationCreateOutcome,
                    null,
                    false,
                    new AccountOrganizationError(
                        OrganizationCreateOutcome,
                        message,
                        false)));
        }

        return Task.FromResult(
            new AccountOrganizationCreateResponse(
                AgentLocalContract.AccountOrganizationCapabilityId,
                AgentLocalContract.AccountOrganizationContractVersion,
                "CREATED",
                request.DisplayName,
                true,
                null));
    }

    public Task<AccountOrganizationProfileUpdateResponse> UpdateAccountOrganizationProfileAsync(
        AccountOrganizationProfileUpdateRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OrganizationProfileUpdateCount++;
        LastOrganizationProfileUpdateRequest = request;

        if (!Authenticated)
        {
            return Task.FromResult(
                new AccountOrganizationProfileUpdateResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "AUTH_REQUIRED",
                    new AccountOrganizationError(
                        "SESSION_INVALID",
                        "Sign in again.",
                        false)));
        }

        if (OrganizationProfileUpdateOutcomeUnknown)
        {
            OrganizationProfileUpdateOutcomeUnknown = false;
            return Task.FromResult(
                new AccountOrganizationProfileUpdateResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "OUTCOME_UNKNOWN",
                    new AccountOrganizationError(
                        "ORGANIZATION_PROFILE_UPDATE_OUTCOME_UNKNOWN",
                        "The update result could not be confirmed.",
                        false)));
        }

        if (OrganizationProfileUpdateOutcome != "UPDATED")
        {
            return Task.FromResult(
                new AccountOrganizationProfileUpdateResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    OrganizationProfileUpdateOutcome,
                    new AccountOrganizationError(
                        OrganizationProfileUpdateOutcome,
                        "The selected role cannot update these fields.",
                        false)));
        }

        if (request.UpdateOrganizationProfile)
        {
            OrganizationDisplayName =
                request.DisplayName ??
                throw new InvalidOperationException(
                    "Missing certification display name.");
            OrganizationLegalName =
                request.LegalName ??
                throw new InvalidOperationException(
                    "Missing certification legal name.");
            OrganizationRegistrationNumber =
                request.RegistrationNumber;
        }

        if (request.UpdateBillingProfile)
        {
            OrganizationBillingEmail =
                request.BillingEmail ??
                throw new InvalidOperationException(
                    "Missing certification billing email.");
            OrganizationTaxId = request.TaxId;
        }

        return Task.FromResult(
            new AccountOrganizationProfileUpdateResponse(
                AgentLocalContract.AccountOrganizationCapabilityId,
                AgentLocalContract.AccountOrganizationContractVersion,
                "UPDATED",
                null));
    }

    public Task<AccountOrganizationInvitationCreateResponse> CreateAccountOrganizationInvitationAsync(
        AccountOrganizationInvitationCreateRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OrganizationInvitationCreateCount++;
        LastOrganizationInvitationRequest = request;

        if (!Authenticated)
        {
            return Task.FromResult(
                new AccountOrganizationInvitationCreateResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "AUTH_REQUIRED",
                    null,
                    null,
                    new AccountOrganizationError(
                        "SESSION_INVALID",
                        "Sign in again.",
                        false)));
        }

        if (OrganizationInvitationOutcomeUnknown)
        {
            OrganizationInvitationOutcomeUnknown = false;
            return Task.FromResult(
                new AccountOrganizationInvitationCreateResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "OUTCOME_UNKNOWN",
                    null,
                    null,
                    new AccountOrganizationError(
                        "ORGANIZATION_INVITATION_OUTCOME_UNKNOWN",
                        "The invitation result could not be confirmed.",
                        false)));
        }

        if (OrganizationInvitationOutcome != "CREATED")
        {
            return Task.FromResult(
                new AccountOrganizationInvitationCreateResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    OrganizationInvitationOutcome,
                    null,
                    null,
                    new AccountOrganizationError(
                        OrganizationInvitationOutcome,
                        "The selected role cannot issue this invitation.",
                        false)));
        }

        OrganizationInvitationIssued = true;
        return Task.FromResult(
            new AccountOrganizationInvitationCreateResponse(
                AgentLocalContract.AccountOrganizationCapabilityId,
                AgentLocalContract.AccountOrganizationContractVersion,
                "CREATED",
                new AccountOrganizationInvitationIssued(
                    request.Email,
                    request.Role,
                    "PENDING",
                    "2026-10-07T00:00:00.000Z",
                    "2026-09-30T00:00:00.000Z"),
                "organization-invitation-code-cert",
                null));
    }

    public Task<AccountOrganizationInvitationManageResponse> ManageAccountOrganizationInvitationAsync(
        AccountOrganizationInvitationManageRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OrganizationInvitationManageCount++;
        LastOrganizationInvitationManageRequest = request;

        if (!Authenticated)
        {
            return Task.FromResult(
                new AccountOrganizationInvitationManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "AUTH_REQUIRED",
                    null,
                    null,
                    new AccountOrganizationError(
                        "SESSION_INVALID",
                        "Sign in again.",
                        false)));
        }

        if (OrganizationInvitationManageOutcomeUnknown)
        {
            OrganizationInvitationManageOutcomeUnknown = false;
            return Task.FromResult(
                new AccountOrganizationInvitationManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "OUTCOME_UNKNOWN",
                    null,
                    null,
                    new AccountOrganizationError(
                        "ORGANIZATION_INVITATION_MANAGEMENT_OUTCOME_UNKNOWN",
                        "The invitation management result could not be confirmed.",
                        false)));
        }

        if (OrganizationInvitationManageOutcome != "READY")
        {
            return Task.FromResult(
                new AccountOrganizationInvitationManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    OrganizationInvitationManageOutcome,
                    null,
                    null,
                    new AccountOrganizationError(
                        OrganizationInvitationManageOutcome,
                        "The selected invitation cannot be managed.",
                        false)));
        }

        if (request.Action == "RESEND")
        {
            return Task.FromResult(
                new AccountOrganizationInvitationManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "RESENT",
                    new AccountOrganizationInvitationIssued(
                        "new-member@example.test",
                        "MEMBER",
                        "PENDING",
                        "2026-10-08T00:00:00.000Z",
                        "2026-09-30T00:00:00.000Z"),
                    "organization-invitation-resend-code-cert",
                    null));
        }

        if (request.Action == "REVOKE")
        {
            OrganizationInvitationIssued = false;
            return Task.FromResult(
                new AccountOrganizationInvitationManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "REVOKED",
                    new AccountOrganizationInvitationIssued(
                        "new-member@example.test",
                        "MEMBER",
                        "REVOKED",
                        "2026-10-07T00:00:00.000Z",
                        "2026-09-30T00:00:00.000Z"),
                    null,
                    null));
        }

        throw new InvalidOperationException(
            "Unexpected certification invitation management action.");
    }

    public Task<AccountOrganizationMemberManageResponse> ManageAccountOrganizationMemberAsync(
        AccountOrganizationMemberManageRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OrganizationMemberManageCount++;
        LastOrganizationMemberManageRequest = request;

        if (!Authenticated)
        {
            return Task.FromResult(
                new AccountOrganizationMemberManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "AUTH_REQUIRED",
                    new AccountOrganizationError(
                        "SESSION_INVALID",
                        "Sign in again.",
                        false)));
        }

        if (OrganizationMemberManageOutcomeUnknown)
        {
            OrganizationMemberManageOutcomeUnknown = false;
            return Task.FromResult(
                new AccountOrganizationMemberManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "OUTCOME_UNKNOWN",
                    new AccountOrganizationError(
                        "ORGANIZATION_MEMBER_MANAGEMENT_OUTCOME_UNKNOWN",
                        "The member management result could not be confirmed.",
                        false)));
        }

        if (OrganizationMemberManageOutcome != "READY")
        {
            return Task.FromResult(
                new AccountOrganizationMemberManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    OrganizationMemberManageOutcome,
                    new AccountOrganizationError(
                        OrganizationMemberManageOutcome,
                        OrganizationMemberManageOutcome == "LAST_OWNER_REQUIRED"
                            ? "The last Organization owner cannot be demoted or removed."
                            : "The selected member cannot be managed.",
                        false)));
        }

        if (request.ManagementHandle != OrganizationManagedMemberHandle)
        {
            throw new InvalidOperationException(
                "Unexpected certification member management handle.");
        }

        if (request.Action == "UPDATE_ROLE")
        {
            OrganizationManagedMemberRole =
                request.Role ??
                throw new InvalidOperationException(
                    "Missing certification member role.");
            return Task.FromResult(
                new AccountOrganizationMemberManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "UPDATED",
                    null));
        }

        if (request.Action == "REMOVE")
        {
            if (request.Role is not null)
            {
                throw new InvalidOperationException(
                    "Certification member removal carried a role.");
            }
            OrganizationManagedMemberExists = false;
            return Task.FromResult(
                new AccountOrganizationMemberManageResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "REMOVED",
                    null));
        }

        throw new InvalidOperationException(
            "Unexpected certification member management action.");
    }

    public Task<AccountOrganizationLeaveResponse> LeaveAccountOrganizationAsync(
        AccountOrganizationLeaveRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OrganizationLeaveCount++;
        LastOrganizationLeaveRequest = request;

        if (!Authenticated)
        {
            return Task.FromResult(
                new AccountOrganizationLeaveResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "AUTH_REQUIRED",
                    true,
                    new AccountOrganizationError(
                        "SESSION_INVALID",
                        "Sign in again.",
                        false)));
        }

        if (OrganizationLeaveOutcomeUnknown)
        {
            OrganizationLeaveOutcomeUnknown = false;
            Authenticated = false;
            return Task.FromResult(
                new AccountOrganizationLeaveResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "OUTCOME_UNKNOWN",
                    true,
                    new AccountOrganizationError(
                        "ORGANIZATION_LEAVE_OUTCOME_UNKNOWN",
                        "The leave result could not be confirmed.",
                        false)));
        }

        if (OrganizationLeaveOutcome == "MEMBER_NOT_FOUND")
        {
            Authenticated = false;
            return Task.FromResult(
                new AccountOrganizationLeaveResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "MEMBER_NOT_FOUND",
                    true,
                    new AccountOrganizationError(
                        "MEMBER_NOT_FOUND",
                        "The selected Organization membership is no longer available.",
                        false)));
        }

        if (OrganizationRole == "OWNER" ||
            OrganizationLeaveOutcome == "OWNER_CANNOT_LEAVE")
        {
            return Task.FromResult(
                new AccountOrganizationLeaveResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "OWNER_CANNOT_LEAVE",
                    false,
                    new AccountOrganizationError(
                        "OWNER_CANNOT_LEAVE",
                        "Transfer Organization ownership before leaving.",
                        false)));
        }

        if (OrganizationLeaveOutcome == "LEFT")
        {
            Authenticated = false;
            return Task.FromResult(
                new AccountOrganizationLeaveResponse(
                    AgentLocalContract.AccountOrganizationCapabilityId,
                    AgentLocalContract.AccountOrganizationContractVersion,
                    "LEFT",
                    true,
                    null));
        }

        return Task.FromResult(
            new AccountOrganizationLeaveResponse(
                AgentLocalContract.AccountOrganizationCapabilityId,
                AgentLocalContract.AccountOrganizationContractVersion,
                "FAILED",
                false,
                new AccountOrganizationError(
                    "ORGANIZATION_LEAVE_UNAVAILABLE",
                    "The Organization membership was not changed.",
                    true)));
    }

    public Task<AccountNotificationFeedResponse> GetAccountNotificationsAsync(
        AccountNotificationFeedRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountNotificationReceiptResponse> MutateAccountNotificationAsync(
        AccountNotificationReceiptRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<SoftwareCatalogResponse> GetSoftwareCatalogAsync(
        SoftwareCatalogRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<SoftwareInstallResponse> InstallSoftwareAsync(
        SoftwareInstallRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<SoftwareUpdateResponse> UpdateSoftwareAsync(
        SoftwareUpdateRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<SoftwareRepairResponse> RepairSoftwareAsync(
        SoftwareRepairRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<SoftwareOpenResponse> OpenSoftwareAsync(
        SoftwareOpenRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<SoftwareRemoveResponse> RemoveSoftwareAsync(
        SoftwareRemoveRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}
