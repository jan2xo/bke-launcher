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

var identityMethods = typeof(ILauncherIdentityClient)
    .GetMethods()
    .Select(method => method.Name)
    .ToHashSet(StringComparer.Ordinal);
Require(identityMethods.SetEquals([
    "LoginAsync",
    "RequestPasswordResetAsync"
]), "Launcher identity client port drifted.");
Require(!typeof(BkePlatformContract).GetFields(BindingFlags.Public | BindingFlags.Static)
        .Any(field => field.Name.Contains("DefaultBaseAddress", StringComparison.Ordinal)),
    "Launcher platform contract regained an independent default authority.");
Require(BkePlatformContract.NativeLoginPath == "/api/agent-sessions/native/login", "native login path drifted.");
Require(BkePlatformContract.NativePasswordResetRequestPath == "/api/agent-sessions/native/password-reset/request", "native password-reset request path drifted.");
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
var installTimeoutValue = installTimeoutField?.GetValue(null);
var updateTimeoutValue = updateTimeoutField?.GetValue(null);
var repairTimeoutValue = repairTimeoutField?.GetValue(null);
var removeTimeoutValue = removeTimeoutField?.GetValue(null);
Require(defaultTimeoutValue is TimeSpan,
    "Launcher default loopback timeout field is unavailable.");
Require(passwordChangeTimeoutValue is TimeSpan,
    "Launcher password-change timeout field is unavailable.");
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

await CertifyNativePasswordResetRequestAsync();
await CertifyAccountPasswordChangeSettingsAsync();
await CertifyCustomerAcquisitionToMySoftwareAsync();

Console.WriteLine("BKE Launcher contract certification: PASS");
Console.WriteLine("Agent-owned account session boundary certified");
Console.WriteLine("Native Forgot Password enumeration-safe recovery composition certified");
Console.WriteLine("Native Account Security password-change composition certified");
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
        new LauncherAccountMfaController(agent));
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

sealed class CustomerJourneyIdentityClient : ILauncherIdentityClient
{
    public int ResetRequestCount { get; private set; }
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

    private const string AccountId = "acct-cert-recipient";
    private readonly CustomerJourneyCatalogSource _catalog;
    private string _purchaseMode = "SELF";

    public CustomerJourneyAgentClient(CustomerJourneyCatalogSource catalog)
    {
        _catalog = catalog;
    }

    public int CheckoutStartCount { get; private set; }
    public int RedeemCount { get; private set; }
    public int PasswordChangeCount { get; private set; }
    public int PlatformAuthorityCount { get; private set; }
    public bool Authenticated { get; set; } = true;
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
                    "INDIVIDUAL",
                    "Certification Customer"),
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
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

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
