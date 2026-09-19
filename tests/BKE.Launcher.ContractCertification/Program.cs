using System.Reflection;
using BKE.Launcher.AgentClient;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;
using BKE.Launcher.PluginHost;

var agentBase = new Uri(AgentLocalContract.DefaultBaseAddress, UriKind.Absolute);
Require(agentBase.IsLoopback, "Agent default address is not loopback.");
Require(agentBase.Scheme == Uri.UriSchemeHttp, "Agent default address must use local HTTP.");

Require(AgentLocalContract.AccountSessionStartPath == "/v1/account-session/start", "account-session start path drifted");
Require(AgentLocalContract.AccountSessionStatusPath == "/v1/account-session/status", "account-session status path drifted");
Require(AgentLocalContract.AccountSessionLogoutPath == "/v1/account-session/logout", "account-session logout path drifted");
Require(AgentLocalContract.SoftwareCatalogPath == "/v1/software/catalog", "software catalog path drifted");
Require(AgentLocalContract.SoftwareCatalogCapabilityId == "bke.software-catalog", "software catalog capability id drifted");
Require(AgentLocalContract.SoftwareCatalogContractVersion == 1, "software catalog contract version drifted");

Require(ProductExecutionTypeWire.ToWireValue(ProductExecutionType.LauncherPlugin) == "LAUNCHER_PLUGIN", "launcher plugin execution type drifted");
Require(ProductExecutionTypeWire.ToWireValue(ProductExecutionType.Standalone) == "STANDALONE", "standalone execution type drifted");

var localResponseProperties = typeof(AccountSessionStartResponse).GetProperties()
    .Concat(typeof(AccountSessionStatusResponse).GetProperties())
    .Concat(typeof(AccountSessionLogoutResponse).GetProperties())
    .Concat(typeof(SoftwareCatalogResponse).GetProperties())
    .Concat(typeof(SoftwareCatalogItem).GetProperties())
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);

Require(!localResponseProperties.Contains("AccessToken"), "Launcher local response contract exposes access tokens.");
Require(!localResponseProperties.Contains("RefreshToken"), "Launcher local response contract exposes refresh tokens.");
Require(!localResponseProperties.Contains("DeviceCode"), "Launcher local response contract exposes the secret device code.");
Require(!localResponseProperties.Any(name => name.Contains("DownloadUrl", StringComparison.OrdinalIgnoreCase)), "Launcher catalog exposes download URLs.");
Require(!localResponseProperties.Any(name => name.Contains("SigningKey", StringComparison.OrdinalIgnoreCase)), "Launcher catalog exposes signing keys.");
Require(!localResponseProperties.Any(name => name.Contains("InstallPath", StringComparison.OrdinalIgnoreCase)), "Launcher catalog exposes privileged install paths.");

var agentMethods = typeof(ILauncherAgentClient)
    .GetMethods()
    .Select(method => method.Name)
    .ToHashSet(StringComparer.Ordinal);

Require(agentMethods.SetEquals([
    "StartAccountSessionAsync",
    "GetAccountSessionStatusAsync",
    "LogoutAccountSessionAsync",
    "GetSoftwareCatalogAsync"
]), "Launcher Agent client port drifted.");

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

Console.WriteLine("BKE Launcher contract certification: PASS");
Console.WriteLine("Agent-owned account session boundary certified");
Console.WriteLine("Agent-owned software catalog boundary certified");
Console.WriteLine("Owner-controlled LAUNCHER_PLUGIN/STANDALONE types certified");
return;

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
