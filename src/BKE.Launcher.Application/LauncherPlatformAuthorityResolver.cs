using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherPlatformAuthorityResolver
{
    private readonly ILauncherAgentClient _agent;

    public LauncherPlatformAuthorityResolver(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public static readonly HashSet<string> ApprovedNonproductionEnvironments =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "development",
            "certification",
            "utm",
            "test",
        };

    public static bool IsApprovedNonproductionEnvironment(string? environment) =>
        !string.IsNullOrWhiteSpace(environment) &&
        !string.Equals(environment.Trim(), "production", StringComparison.OrdinalIgnoreCase) &&
        ApprovedNonproductionEnvironments.Contains(environment.Trim());

    public async Task<Uri> ResolveAsync(CancellationToken cancellationToken)
    {
        var response = await _agent.GetPlatformAuthorityAsync(
            new PlatformAuthorityRequest(Guid.NewGuid().ToString("N")),
            cancellationToken);

        var allowInsecureLoopback = string.Equals(
            Environment.GetEnvironmentVariable("BKE_AGENT_ALLOW_INSECURE_LOOPBACK"),
            "1",
            StringComparison.Ordinal);

        var configuredEnv = Environment.GetEnvironmentVariable("BKE_ENVIRONMENT");
        var effectiveEnv = !string.IsNullOrWhiteSpace(configuredEnv)
            ? configuredEnv
            : response.Environment;

        var isApprovedNonprod =
            IsApprovedNonproductionEnvironment(response.Environment) &&
            IsApprovedNonproductionEnvironment(effectiveEnv);

        var isLoopbackHttp = allowInsecureLoopback &&
            isApprovedNonprod &&
            Uri.TryCreate(response.PlatformBaseUrl, UriKind.Absolute, out var testUri) &&
            testUri.Scheme == Uri.UriSchemeHttp &&
            testUri.IsLoopback;

        if (response.CapabilityId != AgentLocalContract.PlatformAuthorityCapabilityId ||
            response.ContractVersion != AgentLocalContract.PlatformAuthorityContractVersion ||
            response.Status != "READY" ||
            string.IsNullOrWhiteSpace(response.Environment) ||
            string.IsNullOrWhiteSpace(response.PlatformBaseUrl) ||
            !Uri.TryCreate(response.PlatformBaseUrl, UriKind.Absolute, out var authority) ||
            (!isLoopbackHttp && authority.Scheme != Uri.UriSchemeHttps) ||
            string.IsNullOrWhiteSpace(authority.Host) ||
            !string.IsNullOrEmpty(authority.UserInfo) ||
            authority.AbsolutePath != "/" ||
            !string.IsNullOrEmpty(authority.Query) ||
            !string.IsNullOrEmpty(authority.Fragment))
        {
            throw new InvalidDataException(
                response.Error?.Message ??
                "BKE Licensing Agent platform authority is unavailable or invalid.");
        }

        return authority;
    }
}
