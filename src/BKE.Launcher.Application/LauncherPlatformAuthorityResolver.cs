using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherPlatformAuthorityResolver
{
    private readonly ILauncherAgentClient _agent;

    public LauncherPlatformAuthorityResolver(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<Uri> ResolveAsync(CancellationToken cancellationToken)
    {
        var response = await _agent.GetPlatformAuthorityAsync(
            new PlatformAuthorityRequest(Guid.NewGuid().ToString("N")),
            cancellationToken);

        if (response.CapabilityId != AgentLocalContract.PlatformAuthorityCapabilityId ||
            response.ContractVersion != AgentLocalContract.PlatformAuthorityContractVersion ||
            response.Status != "READY" ||
            string.IsNullOrWhiteSpace(response.Environment) ||
            string.IsNullOrWhiteSpace(response.PlatformBaseUrl) ||
            !Uri.TryCreate(response.PlatformBaseUrl, UriKind.Absolute, out var authority) ||
            authority.Scheme != Uri.UriSchemeHttps ||
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
