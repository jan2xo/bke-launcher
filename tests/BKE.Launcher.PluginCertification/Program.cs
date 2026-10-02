using BKE.Launcher.Application;
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
    !name.Contains("RefreshToken", StringComparison.OrdinalIgnoreCase)),
    "Plugin API exposes cloud token semantics.");

await CertifyAuthorizedLifecycle();
await CertifyDenialBeforePluginCode();
await CertifyVersionAndHostContractGates();
CertifyDuplicateRegistrationFailsClosed();

Console.WriteLine("BKE Launcher plugin certification: PASS");
Console.WriteLine("Plugin contract v1 exposes capabilities, not cloud credentials");
Console.WriteLine("Bundled plugin runtime requires Agent authorization before plugin code runs");
return;

static async Task CertifyAuthorizedLifecycle()
{
    var events = new List<string>();
    var authorization = new FakeAuthorizationPort(events);
    var plugin = new TestPlugin(
        new LauncherPluginIdentity(
            "bke-plugin-tool",
            "1.2.3",
            LauncherPluginContract.Version),
        events,
        probeCrossProductAuthorization: true);
    var runtime = new LauncherPluginRuntime(
        [plugin],
        authorization);

    Require(
        runtime.IsRegistered("bke-plugin-tool", "1.2.3"),
        "Exact bundled plugin registration was not recognized.");
    Require(
        !runtime.IsRegistered("bke-plugin-tool", "1.2.2"),
        "Launcher treated a stale bundled plugin version as registered.");
    Require(
        !runtime.IsRegistered("bke-other", "1.2.3"),
        "Launcher invented a plugin registration.");

    var first = await runtime.OpenAsync(
        "bke-plugin-tool",
        "1.2.3",
        CancellationToken.None);

    Require(first.Status == "OPENED", "Authorized bundled plugin did not open.");
    Require(plugin.InitializeCount == 1, "Bundled plugin was not initialized exactly once.");
    Require(plugin.OpenCount == 1, "Bundled plugin OpenAsync was not invoked.");
    Require(
        plugin.CrossProductAuthorization is { Authorized: false, Reason: "product_scope_mismatch" },
        "Plugin context allowed cross-product authorization.");
    Require(
        authorization.Requests.Count == 1 &&
        authorization.Requests[0] == ("bke-plugin-tool", "1.2.3"),
        "Plugin runtime did not authorize the exact bundled identity.");
    Require(
        events.Take(2).SequenceEqual(["authorize:bke-plugin-tool:1.2.3", "initialize"]),
        "Plugin code ran before Agent-mediated authorization.");

    var second = await runtime.OpenAsync(
        "bke-plugin-tool",
        "1.2.3",
        CancellationToken.None);

    Require(second.Status == "OPENED", "Second authorized plugin open failed.");
    Require(plugin.InitializeCount == 1, "Plugin was initialized more than once.");
    Require(plugin.OpenCount == 2, "Plugin did not receive the second OpenAsync.");
    Require(
        authorization.Requests.Count == 2,
        "Launcher did not reauthorize the plugin on each Open intent.");

    await runtime.ShutdownAsync(CancellationToken.None);
    Require(plugin.ShutdownCount == 1, "Initialized plugin was not shut down.");

    await runtime.ShutdownAsync(CancellationToken.None);
    Require(plugin.ShutdownCount == 1, "Plugin shutdown was repeated after state was cleared.");
}

static async Task CertifyDenialBeforePluginCode()
{
    var events = new List<string>();
    var authorization = new FakeAuthorizationPort(
        events,
        new LauncherPluginAuthorizationDecision(
            "DENIED",
            false,
            "not_entitled",
            null));
    var plugin = new TestPlugin(
        new LauncherPluginIdentity(
            "bke-plugin-locked",
            "1.0.0",
            LauncherPluginContract.Version),
        events);
    var runtime = new LauncherPluginRuntime(
        [plugin],
        authorization);

    var result = await runtime.OpenAsync(
        "bke-plugin-locked",
        "1.0.0",
        CancellationToken.None);

    Require(
        result.Status == "DENIED" &&
        result.Reason == "not_entitled",
        "Agent authorization denial did not reach the Launcher runtime.");
    Require(plugin.InitializeCount == 0, "Denied plugin was initialized.");
    Require(plugin.OpenCount == 0, "Denied plugin was opened.");
    Require(
        events.SequenceEqual(["authorize:bke-plugin-locked:1.0.0"]),
        "Denied plugin executed code before authorization.");
}

static async Task CertifyVersionAndHostContractGates()
{
    var events = new List<string>();
    var authorization = new FakeAuthorizationPort(events);
    var plugin = new TestPlugin(
        new LauncherPluginIdentity(
            "bke-plugin-tool",
            "1.2.3",
            LauncherPluginContract.Version),
        events);
    var runtime = new LauncherPluginRuntime(
        [plugin],
        authorization);

    var stale = await runtime.OpenAsync(
        "bke-plugin-tool",
        "1.2.2",
        CancellationToken.None);
    Require(
        stale.Status == "UNAVAILABLE" &&
        stale.Reason == "plugin_version_mismatch",
        "Wrong bundled plugin version did not fail closed.");
    Require(
        authorization.Requests.Count == 0 &&
        plugin.InitializeCount == 0 &&
        plugin.OpenCount == 0,
        "Wrong-version plugin reached authorization or plugin code.");

    var missing = await runtime.OpenAsync(
        "bke-plugin-missing",
        "1.0.0",
        CancellationToken.None);
    Require(
        missing.Status == "UNAVAILABLE" &&
        missing.Reason == "plugin_not_bundled",
        "Unregistered plugin did not fail closed.");

    var future = new TestPlugin(
        new LauncherPluginIdentity(
            "bke-plugin-future",
            "1.0.0",
            LauncherPluginContract.Version + 1),
        events);
    var futureRuntime = new LauncherPluginRuntime(
        [future],
        authorization);

    Require(
        !futureRuntime.IsRegistered("bke-plugin-future", "1.0.0"),
        "Plugin requiring a newer host contract was advertised as openable.");

    var unsupported = await futureRuntime.OpenAsync(
        "bke-plugin-future",
        "1.0.0",
        CancellationToken.None);
    Require(
        unsupported.Status == "UNAVAILABLE" &&
        unsupported.Reason == "host_contract_too_old",
        "Newer plugin host-contract requirement did not fail closed.");
    Require(
        future.InitializeCount == 0 &&
        future.OpenCount == 0,
        "Unsupported plugin host contract executed plugin code.");
}

static void CertifyDuplicateRegistrationFailsClosed()
{
    var events = new List<string>();
    var authorization = new FakeAuthorizationPort(events);
    var first = new TestPlugin(
        new LauncherPluginIdentity(
            "bke-plugin-duplicate",
            "1.0.0",
            1),
        events);
    var second = new TestPlugin(
        new LauncherPluginIdentity(
            "bke-plugin-duplicate",
            "2.0.0",
            1),
        events);

    try
    {
        _ = new LauncherPluginRuntime(
            [first, second],
            authorization);
        throw new InvalidOperationException(
            "Duplicate Launcher plugin registration was accepted.");
    }
    catch (InvalidDataException)
    {
        // Duplicate product identity must fail closed at composition time.
    }
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}


sealed class FakeAuthorizationPort(
    List<string> events,
    LauncherPluginAuthorizationDecision? decision = null)
    : ILauncherPluginAuthorizationPort
{
    private readonly LauncherPluginAuthorizationDecision _decision =
        decision ??
        new LauncherPluginAuthorizationDecision(
            "AUTHORIZED",
            true,
            "authorized",
            null);

    public List<(string ProductId, string Version)> Requests { get; } = [];

    public Task<LauncherPluginAuthorizationDecision> AuthorizeAsync(
        string productId,
        string version,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Requests.Add((productId, version));
        events.Add($"authorize:{productId}:{version}");
        return Task.FromResult(_decision);
    }
}

sealed class TestPlugin(
    LauncherPluginIdentity identity,
    List<string> events,
    bool probeCrossProductAuthorization = false)
    : IBkeLauncherPlugin
{
    public LauncherPluginIdentity Identity { get; } = identity;
    public int InitializeCount { get; private set; }
    public int OpenCount { get; private set; }
    public int ShutdownCount { get; private set; }
    public ProductAuthorizationResult? CrossProductAuthorization { get; private set; }

    public async Task InitializeAsync(
        ILauncherContext context,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        InitializeCount++;
        events.Add("initialize");

        if (probeCrossProductAuthorization)
        {
            CrossProductAuthorization =
                await context.Authorization.AuthorizeAsync(
                    "bke-other-product",
                    cancellationToken);
        }
    }

    public Task OpenAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        OpenCount++;
        events.Add("open");
        return Task.CompletedTask;
    }

    public Task ShutdownAsync(
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        ShutdownCount++;
        events.Add("shutdown");
        return Task.CompletedTask;
    }
}

