using System.Reflection;
using BKE.Launcher.AgentClient;
using BKE.Launcher.Application;
using BKE.Launcher.Contracts;
using BKE.Launcher.Infrastructure;
using BKE.Launcher.PluginHost;

var agentBase = new Uri(AgentLocalContract.DefaultBaseAddress, UriKind.Absolute);
Require(agentBase.IsLoopback, "Agent default address is not loopback.");
Require(agentBase.Scheme == Uri.UriSchemeHttp, "Agent default address must use local HTTP.");

Require(AgentLocalContract.AccountSessionStartPath == "/v1/account-session/start", "account-session start path drifted");
Require(AgentLocalContract.AccountSessionNativeContextPath == "/v1/account-session/native/context", "native account-session context path drifted");
Require(AgentLocalContract.AccountSessionNativeCompletePath == "/v1/account-session/native/complete", "native account-session complete path drifted");
Require(AgentLocalContract.AccountSessionStatusPath == "/v1/account-session/status", "account-session status path drifted");
Require(AgentLocalContract.AccountSessionLogoutPath == "/v1/account-session/logout", "account-session logout path drifted");
Require(AgentLocalContract.SoftwareCatalogPath == "/v1/software/catalog", "software catalog path drifted");
Require(AgentLocalContract.SoftwareCatalogCapabilityId == "bke.software-catalog", "software catalog capability id drifted");
Require(AgentLocalContract.SoftwareCatalogContractVersion == 1, "software catalog contract version drifted");
Require(AgentLocalContract.SoftwareInstallPath == "/v1/software/install", "software install path drifted");
Require(AgentLocalContract.SoftwareInstallCapabilityId == "bke.software-install", "software install capability id drifted");
Require(AgentLocalContract.SoftwareInstallContractVersion == 1, "software install contract version drifted");
Require(AgentLocalContract.SoftwareOpenPath == "/v1/software/open", "software open path drifted");
Require(AgentLocalContract.SoftwareOpenCapabilityId == "bke.software-open", "software open capability id drifted");
Require(AgentLocalContract.SoftwareOpenContractVersion == 1, "software open contract version drifted");

Require(ProductExecutionTypeWire.ToWireValue(ProductExecutionType.LauncherPlugin) == "LAUNCHER_PLUGIN", "launcher plugin execution type drifted");
Require(ProductExecutionTypeWire.ToWireValue(ProductExecutionType.Standalone) == "STANDALONE", "standalone execution type drifted");

var localResponseProperties = typeof(AccountSessionStartResponse).GetProperties()
    .Concat(typeof(AccountSessionNativeContextResponse).GetProperties())
    .Concat(typeof(AccountSessionNativeCompleteResponse).GetProperties())
    .Concat(typeof(AccountSessionStatusResponse).GetProperties())
    .Concat(typeof(AccountSessionLogoutResponse).GetProperties())
    .Concat(typeof(SoftwareCatalogResponse).GetProperties())
    .Concat(typeof(SoftwareCatalogItem).GetProperties())
    .Concat(typeof(SoftwareInstallResponse).GetProperties())
    .Concat(typeof(SoftwareInstallError).GetProperties())
    .Concat(typeof(SoftwareOpenResponse).GetProperties())
    .Concat(typeof(SoftwareOpenError).GetProperties())
    .Concat(typeof(SoftwareRemoveResponse).GetProperties())
    .Concat(typeof(SoftwareRemoveError).GetProperties())
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);

Require(!localResponseProperties.Contains("AccessToken"), "Launcher local response contract exposes access tokens.");
Require(!localResponseProperties.Contains("RefreshToken"), "Launcher local response contract exposes refresh tokens.");
Require(!localResponseProperties.Contains("DeviceCode"), "Launcher local response contract exposes the secret device code.");
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
    "StartAccountSessionAsync",
    "GetNativeAccountSessionContextAsync",
    "CompleteNativeAccountSessionAsync",
    "GetAccountSessionStatusAsync",
    "LogoutAccountSessionAsync",
    "GetSoftwareCatalogAsync",
    "InstallSoftwareAsync",
    "OpenSoftwareAsync",
    "RemoveSoftwareAsync"
]), "Launcher Agent client port drifted.");

var authMethods = typeof(ILauncherAccountAuthClient)
    .GetMethods()
    .Select(method => method.Name)
    .ToHashSet(StringComparer.Ordinal);
Require(authMethods.SetEquals(["AuthenticateAsync"]), "Launcher native auth client port drifted.");

var nativeResultProperties = typeof(LauncherNativeSignInResult)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(!nativeResultProperties.Contains("HandoffCode"), "Launcher presentation result exposes handoff code.");
Require(!nativeResultProperties.Contains("Password"), "Launcher presentation result exposes password.");
Require(!nativeResultProperties.Any(name => name.Contains("Token", StringComparison.OrdinalIgnoreCase)),
    "Launcher presentation result exposes cloud token material.");

var nativeAgentRequestProperties = typeof(AccountSessionNativeCompleteRequest)
    .GetProperties()
    .Select(property => property.Name)
    .ToHashSet(StringComparer.OrdinalIgnoreCase);
Require(nativeAgentRequestProperties.SetEquals(["CorrelationId", "HandoffCode"]),
    "Launcher widened the Agent native-complete request.");
Require(!nativeAgentRequestProperties.Contains("Email"), "Launcher sends email to Agent native completion.");
Require(!nativeAgentRequestProperties.Contains("Password"), "Launcher sends password to Agent native completion.");

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
var removeTimeoutField = typeof(AgentLoopbackClient).GetField(
    "RemoveRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
var defaultTimeoutValue = defaultTimeoutField?.GetValue(null);
var installTimeoutValue = installTimeoutField?.GetValue(null);
var removeTimeoutValue = removeTimeoutField?.GetValue(null);
Require(defaultTimeoutValue is TimeSpan,
    "Launcher default loopback timeout field is unavailable.");
Require(installTimeoutValue is TimeSpan,
    "Launcher install-operation timeout field is unavailable.");
Require(removeTimeoutValue is TimeSpan,
    "Launcher remove-operation timeout field is unavailable.");
var defaultTimeout = (TimeSpan)defaultTimeoutValue!;
var installTimeout = (TimeSpan)installTimeoutValue!;
var removeTimeout = (TimeSpan)removeTimeoutValue!;
Require(defaultTimeout == TimeSpan.FromSeconds(5),
    "Launcher default loopback timeout drifted.");
Require(installTimeout == TimeSpan.FromMinutes(10),
    "Launcher install-operation timeout drifted.");
Require(installTimeout > defaultTimeout,
    "Launcher install operation does not have a dedicated long-running timeout.");
Require(removeTimeout == TimeSpan.FromMinutes(10),
    "Launcher remove-operation timeout drifted.");
Require(removeTimeout > defaultTimeout,
    "Launcher remove operation does not have a dedicated long-running timeout.");

var nativeAuthTimeoutField = typeof(NativeBkeAccountAuthClient).GetField(
    "DefaultRequestTimeout",
    BindingFlags.Static | BindingFlags.NonPublic);
Require(nativeAuthTimeoutField?.GetValue(null) is TimeSpan nativeAuthTimeout &&
        nativeAuthTimeout == TimeSpan.FromSeconds(20),
    "Launcher native cloud-auth timeout drifted.");

var rejectedInsecureCloud = false;
try
{
    using var _ = new NativeBkeAccountAuthClient(
        baseAddress: new Uri("http://example.com", UriKind.Absolute));
}
catch (ArgumentException)
{
    rejectedInsecureCloud = true;
}
Require(rejectedInsecureCloud, "Launcher native auth accepted insecure remote HTTP.");

var fakeAgent = new FakeNativeAgentClient();
var fakeAuth = new FakeNativeAccountAuthClient();
var nativeController = new LauncherNativeSignInController(fakeAgent, fakeAuth);
var nativeResult = await nativeController.SignInAsync(
    "buyer@example.com",
    "transient-password",
    null,
    CancellationToken.None);

Require(nativeResult.Status == "AUTHENTICATED", "Launcher native sign-in did not authenticate.");
Require(nativeResult.Account?.AccountId == "account-native", "Launcher native sign-in account drifted.");
Require(fakeAuth.LastRequest?.Email == "buyer@example.com", "Launcher native auth did not forward email to Digital Solutions client.");
Require(fakeAuth.LastRequest?.Password == "transient-password", "Launcher native auth credential transit drifted.");
Require(fakeAuth.LastRequest?.DeviceId == "device-native-0123456789", "Launcher native auth did not use Agent device context.");
Require(fakeAgent.LastHandoffCode == "opaque-native-handoff-0123456789ABCDE", "Launcher did not forward opaque handoff to Agent.");
Require(!System.Text.Json.JsonSerializer.Serialize(nativeResult).Contains("opaque-native-handoff", StringComparison.Ordinal),
    "Launcher native result leaked handoff material.");

Console.WriteLine("BKE Launcher contract certification: PASS");
Console.WriteLine("Agent-owned account session boundary certified");
Console.WriteLine("Native BKE credential-to-handoff orchestration certified");
Console.WriteLine("Agent-owned software catalog boundary certified");
Console.WriteLine("Agent-owned standalone install intent boundary certified");
Console.WriteLine("Bounded long-running install transport certified");
Console.WriteLine("Agent-owned standalone Open intent boundary certified");
Console.WriteLine("Agent-owned software Remove intent boundary certified");
Console.WriteLine("Bounded long-running remove transport certified");
Console.WriteLine("Owner-controlled LAUNCHER_PLUGIN/STANDALONE types certified");
return;

sealed class FakeNativeAccountAuthClient : ILauncherAccountAuthClient
{
    public NativeAccountLoginRequest? LastRequest { get; private set; }

    public Task<NativeAccountLoginResponse> AuthenticateAsync(
        NativeAccountLoginRequest request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(new NativeAccountLoginResponse(
            "handoff_issued",
            "opaque-native-handoff-0123456789ABCDE",
            90,
            new NativeAccountSummary(
                "account-native",
                "INDIVIDUAL",
                "Native Buyer"),
            null,
            null));
    }
}

sealed class FakeNativeAgentClient : ILauncherAgentClient
{
    public string? LastHandoffCode { get; private set; }

    public Task<AccountSessionNativeContextResponse> GetNativeAccountSessionContextAsync(
        AccountSessionNativeContextRequest request,
        CancellationToken cancellationToken) =>
        Task.FromResult(new AccountSessionNativeContextResponse(
            AgentLocalContract.CapabilityId,
            AgentLocalContract.ContractVersion,
            "READY",
            "device-native-0123456789",
            "WIN-NATIVE",
            "windows",
            "x64",
            null));

    public Task<AccountSessionNativeCompleteResponse> CompleteNativeAccountSessionAsync(
        AccountSessionNativeCompleteRequest request,
        CancellationToken cancellationToken)
    {
        LastHandoffCode = request.HandoffCode;
        return Task.FromResult(new AccountSessionNativeCompleteResponse(
            AgentLocalContract.CapabilityId,
            AgentLocalContract.ContractVersion,
            "AUTHENTICATED",
            new AccountSessionAccount(
                "user-native",
                "buyer@example.com",
                "account-native",
                "INDIVIDUAL",
                "Native Buyer"),
            null));
    }

    public Task<AccountSessionStartResponse> StartAccountSessionAsync(
        AccountSessionStartRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountSessionStatusResponse> GetAccountSessionStatusAsync(
        AccountSessionStatusRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<AccountSessionLogoutResponse> LogoutAccountSessionAsync(
        AccountSessionLogoutRequest request,
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

    public Task<SoftwareOpenResponse> OpenSoftwareAsync(
        SoftwareOpenRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();

    public Task<SoftwareRemoveResponse> RemoveSoftwareAsync(
        SoftwareRemoveRequest request,
        CancellationToken cancellationToken) =>
        throw new NotSupportedException();
}

static void Require(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
