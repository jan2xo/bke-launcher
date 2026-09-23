using System.Reflection;
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
Require(AgentLocalContract.SoftwareOpenPath == "/v1/software/open", "software open path drifted");
Require(AgentLocalContract.SoftwareOpenCapabilityId == "bke.software-open", "software open capability id drifted");
Require(AgentLocalContract.SoftwareOpenContractVersion == 1, "software open contract version drifted");

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
    .Concat(typeof(SoftwareOpenResponse).GetProperties())
    .Concat(typeof(SoftwareOpenError).GetProperties())
    .Concat(typeof(SoftwareRemoveResponse).GetProperties())
    .Concat(typeof(SoftwareRemoveError).GetProperties())
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
    "GetSoftwareCatalogAsync",
    "InstallSoftwareAsync",
    "UpdateSoftwareAsync",
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
Require(mainWindowMarkup.Contains("Content=\"Update\"", StringComparison.Ordinal), "Software Update action is missing.");
Require(mainWindowMarkup.Contains("IsVisible=\"{Binding CanUpdate}\"", StringComparison.Ordinal), "Software Update visibility is not state-bound.");
Require(mainWindowSource.Contains("UpdateProduct", StringComparison.Ordinal), "Software Update click handler is missing.");
var viewModelSource = File.ReadAllText(
    Path.Combine("src", "BKE.Launcher.Presentation", "MainWindowViewModel.cs"));
Require(viewModelSource.Contains("product.State == LauncherProductState.UpdateAvailable", StringComparison.Ordinal),
    "Launcher Update action widened beyond UPDATE_AVAILABLE.");

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

var defaultTimeoutField = typeof(AgentLoopbackClient).GetField(
    "DefaultRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var installTimeoutField = typeof(AgentLoopbackClient).GetField(
    "InstallRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var updateTimeoutField = typeof(AgentLoopbackClient).GetField(
    "UpdateRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var removeTimeoutField = typeof(AgentLoopbackClient).GetField(
    "RemoveRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var defaultTimeoutValue = defaultTimeoutField?.GetValue(null);
var installTimeoutValue = installTimeoutField?.GetValue(null);
var updateTimeoutValue = updateTimeoutField?.GetValue(null);
var removeTimeoutValue = removeTimeoutField?.GetValue(null);
Require(defaultTimeoutValue is TimeSpan,
    "Launcher default loopback timeout field is unavailable.");
Require(installTimeoutValue is TimeSpan,
    "Launcher install-operation timeout field is unavailable.");
Require(updateTimeoutValue is TimeSpan,
    "Launcher update-operation timeout field is unavailable.");
Require(removeTimeoutValue is TimeSpan,
    "Launcher remove-operation timeout field is unavailable.");
var defaultTimeout = (TimeSpan)defaultTimeoutValue!;
var installTimeout = (TimeSpan)installTimeoutValue!;
var updateTimeout = (TimeSpan)updateTimeoutValue!;
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
Require(removeTimeout == TimeSpan.FromMinutes(10),
    "Launcher remove-operation timeout drifted.");
Require(removeTimeout > defaultTimeout,
    "Launcher remove operation does not have a dedicated long-running timeout.");

Console.WriteLine("BKE Launcher contract certification: PASS");
Console.WriteLine("Agent-owned account session boundary certified");
Console.WriteLine("Native Launcher credential -> DS -> one-time Agent handoff boundary certified");
Console.WriteLine("Agent-owned software catalog boundary certified");
Console.WriteLine("Agent-owned standalone install intent boundary certified");
Console.WriteLine("Bounded long-running install transport certified");
Console.WriteLine("Agent-owned software Update intent boundary certified");
Console.WriteLine("Bounded long-running update transport certified");
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
