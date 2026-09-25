using System.Reflection;
using System.Text.Json;
using BKE.Launcher.AgentClient;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;
using BKE.Launcher.Infrastructure;
using BKE.Launcher.PluginHost;

var agentBase = new Uri(AgentLocalContract.DefaultBaseAddress, UriKind.Absolute);
Require(agentBase.IsLoopback, "Agent default address is not loopback.");
Require(agentBase.Scheme == Uri.UriSchemeHttp, "Agent default address must use local HTTP.");

Require(AgentLocalContract.AccountSessionDeviceContextPath == "/v1/account-session/device-context", "account-session device-context path drifted");
Require(AgentLocalContract.AccountSessionCompletePath == "/v1/account-session/complete", "account-session complete path drifted");
Require(AgentLocalContract.AccountSessionStartPath == "/v1/account-session/start", "account-session start path drifted");
Require(AgentLocalContract.AccountSessionStatusPath == "/v1/account-session/status", "account-session status path drifted");
Require(AgentLocalContract.AccountSessionLogoutPath == "/v1/account-session/logout", "account-session logout path drifted");
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

var localResponseProperties = typeof(AccountSessionDeviceContextResponse).GetProperties()
    .Concat(typeof(AccountSessionCompleteResponse).GetProperties())
    .Concat(typeof(AccountSessionStartResponse).GetProperties())
    .Concat(typeof(AccountSessionStatusResponse).GetProperties())
    .Concat(typeof(AccountSessionLogoutResponse).GetProperties())
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
    "GetAccountSessionDeviceContextAsync",
    "CompleteAccountSessionAsync",
    "StartAccountSessionAsync",
    "GetAccountSessionStatusAsync",
    "LogoutAccountSessionAsync",
    "GetAccountNotificationsAsync",
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
Require(BkePlatformContract.NativeLoginPath == "/api/agent-sessions/native/login", "native login path drifted.");
Require(BkePlatformContract.AccountSessionProtocolVersion == "bke.account-session.v1", "account-session protocol version drifted.");

var rejectedInsecurePlatform = false;
try
{
    using var _ = new PlatformIdentityClient(
        baseAddress: new Uri("http://jl-bke.com", UriKind.Absolute));
}
catch (ArgumentException)
{
    rejectedInsecurePlatform = true;
}
Require(rejectedInsecurePlatform, "Launcher platform identity client accepted insecure HTTP.");

var mainWindowSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Desktop", "MainWindow.axaml.cs"));
var mainWindowMarkup = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Desktop", "MainWindow.axaml"));
Require(!mainWindowSource.Contains("Process.Start", StringComparison.Ordinal), "Native sign-in still launches a browser.");
Require(!mainWindowMarkup.Contains("Device code", StringComparison.Ordinal), "Device-code UX remains visible in Launcher.");
Require(mainWindowMarkup.Contains("Sign in with BKE", StringComparison.Ordinal), "Native sign-in action is missing.");
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
Require(mainWindowMarkup.Contains("Header=\"Notifications\"", StringComparison.Ordinal),
    "BKE Notifications tab is missing.");
Require(mainWindowMarkup.Contains("ItemsSource=\"{Binding Notifications}\"", StringComparison.Ordinal),
    "BKE Notifications are not Agent-projected into the UI.");
Require(mainWindowMarkup.Contains("Content=\"Refresh notifications\"", StringComparison.Ordinal),
    "BKE Notifications refresh action is missing.");
Require(mainWindowMarkup.Contains("IsEnabled=\"{Binding CanRefreshNotifications}\"", StringComparison.Ordinal),
    "BKE Notifications refresh action is not session-bound.");
Require(mainWindowMarkup.Contains("This first Launcher surface is read-only.", StringComparison.Ordinal),
    "BKE Notifications UI does not state the read-only boundary.");
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
    "await RefreshNotificationsAsync(cancellationToken);",
    StringComparison.Ordinal),
    "Launcher does not refresh Notifications after authenticated session refresh.");
Require(normalizedViewModelSource.Contains(
    "ClearNotifications(",
    StringComparison.Ordinal),
    "Launcher does not clear account notification presentation across session changes.");
Require(!normalizedViewModelSource.Contains(
    "/api/agent-sessions/",
    StringComparison.OrdinalIgnoreCase),
    "Launcher Notifications UX bypasses the Agent loopback boundary.");

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
Require(externalNavigatorSource.Contains("Uri.UriSchemeHttps", StringComparison.Ordinal),
    "Launcher secure checkout navigation does not require HTTPS.");
Require(!externalNavigatorSource.Contains("PayMongo", StringComparison.OrdinalIgnoreCase),
    "Launcher external navigator contains provider-specific authority.");
Require(!externalNavigatorSource.Contains("/api/agent-sessions/", StringComparison.OrdinalIgnoreCase),
    "Launcher external navigator calls a Digital Solutions Agent API directly.");

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
Require(AgentLocalContract.AccountNotificationInboxCapabilityId == "bke.account-notifications",
    "Launcher Agent account notification capability id drifted.");
Require(AgentLocalContract.AccountNotificationInboxContractVersion == 1,
    "Launcher Agent account notification contract version drifted.");

var defaultTimeoutField = typeof(AgentLoopbackClient).GetField(
    "DefaultRequestTimeout",
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
var installTimeoutValue = installTimeoutField?.GetValue(null);
var updateTimeoutValue = updateTimeoutField?.GetValue(null);
var repairTimeoutValue = repairTimeoutField?.GetValue(null);
var removeTimeoutValue = removeTimeoutField?.GetValue(null);
Require(defaultTimeoutValue is TimeSpan,
    "Launcher default loopback timeout field is unavailable.");
Require(installTimeoutValue is TimeSpan,
    "Launcher install-operation timeout field is unavailable.");
Require(updateTimeoutValue is TimeSpan,
    "Launcher update-operation timeout field is unavailable.");
Require(repairTimeoutValue is TimeSpan,
    "Launcher Repair-operation timeout field is unavailable.");
Require(removeTimeoutValue is TimeSpan,
    "Launcher remove-operation timeout field is unavailable.");
var defaultTimeout = (TimeSpan)defaultTimeoutValue!;
var installTimeout = (TimeSpan)installTimeoutValue!;
var updateTimeout = (TimeSpan)updateTimeoutValue!;
var repairTimeout = (TimeSpan)repairTimeoutValue!;
var removeTimeout = (TimeSpan)removeTimeoutValue!;
Require(defaultTimeout == TimeSpan.FromSeconds(5),
    "Launcher default loopback timeout drifted.");
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

Console.WriteLine("BKE Launcher contract certification: PASS");
Console.WriteLine("Agent-owned account session boundary certified");
Console.WriteLine("Agent-mediated selected-account Notifications presentation boundary certified");
Console.WriteLine("Agent-owned Claim Code redemption intent and transient-code boundary certified");
Console.WriteLine("Agent-owned Store catalog presentation boundary certified");
Console.WriteLine("Agent-owned Store checkout-review presentation boundary certified");
Console.WriteLine("Native Launcher credential -> DS -> one-time Agent handoff boundary certified");
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

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
