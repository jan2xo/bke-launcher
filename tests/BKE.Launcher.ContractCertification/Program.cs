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

Require(ProductExecutionTypeWire.ToWireValue(ProductExecutionType.LauncherPlugin) == "LAUNCHER_PLUGIN", "launcher plugin execution type drifted");
Require(ProductExecutionTypeWire.ToWireValue(ProductExecutionType.Standalone) == "STANDALONE", "standalone execution type drifted");

var responseProperties = typeof(AccountSessionStartResponse).GetProperties()
    .Concat(typeof(AccountSessionStatusResponse).GetProperties())
    .Concat(typeof(AccountSessionLogoutResponse).GetProperties())
    .Select(property => property.Name)
    .ToHashSet(StringComparer.Ordinal);

Require(!responseProperties.Contains("AccessToken"), "Launcher local response contract exposes access tokens.");
Require(!responseProperties.Contains("RefreshToken"), "Launcher local response contract exposes refresh tokens.");
Require(!responseProperties.Contains("DeviceCode"), "Launcher local response contract exposes the secret device code.");

var agentMethods = typeof(ILauncherAgentClient)
    .GetMethods()
    .Select(method => method.Name)
    .ToHashSet(StringComparer.Ordinal);

Require(agentMethods.SetEquals([
    "StartAccountSessionAsync",
    "GetAccountSessionStatusAsync",
    "LogoutAccountSessionAsync"
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
Console.WriteLine("Owner-controlled LAUNCHER_PLUGIN/STANDALONE types certified");
return;

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
