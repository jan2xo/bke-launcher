using BKE.Launcher.PluginHost;

Require(LauncherPluginContract.Version == 1, "Launcher plugin contract version drifted.");

var pluginMethods = typeof(IBkeLauncherPlugin)
    .GetMethods()
    .Select(method => method.Name)
    .ToHashSet(StringComparer.Ordinal);

Require(pluginMethods.SetEquals([
    "get_Identity",
    "InitializeAsync",
    "OpenAsync",
    "ShutdownAsync"
]), "Launcher plugin lifecycle surface drifted.");

var contextProperties = typeof(ILauncherContext).GetProperties();
Require(contextProperties.Length == 1, "Launcher plugin context must remain narrow.");
Require(contextProperties[0].PropertyType == typeof(IProductAuthorizationGateway), "Launcher plugin authorization gateway drifted.");

var gatewayMethods = typeof(IProductAuthorizationGateway)
    .GetMethods()
    .Select(method => method.Name)
    .ToHashSet(StringComparer.Ordinal);

Require(gatewayMethods.SetEquals(["AuthorizeAsync"]), "Product authorization gateway widened unexpectedly.");

var allPublicNames = typeof(ILauncherContext).GetMembers()
    .Concat(typeof(IProductAuthorizationGateway).GetMembers())
    .Select(member => member.Name)
    .ToArray();

Require(allPublicNames.All(name =>
    !name.Contains("AccessToken", StringComparison.OrdinalIgnoreCase) &&
    !name.Contains("RefreshToken", StringComparison.OrdinalIgnoreCase)), "Plugin API exposes cloud token semantics.");

Console.WriteLine("BKE Launcher plugin certification: PASS");
Console.WriteLine("Plugin contract v1 exposes capabilities, not cloud credentials");
return;

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
