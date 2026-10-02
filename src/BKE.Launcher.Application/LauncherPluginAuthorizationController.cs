using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherPluginAuthorizationController
    : ILauncherPluginAuthorizationPort
{
    private readonly ILauncherAgentClient _agent;

    public LauncherPluginAuthorizationController(
        ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<LauncherPluginAuthorizationDecision> AuthorizeAsync(
        string productId,
        string version,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(productId) ||
            productId.Length > 128)
        {
            throw new ArgumentException(
                "A BKE product id is required.",
                nameof(productId));
        }

        if (string.IsNullOrWhiteSpace(version) ||
            version.Length > 128)
        {
            throw new ArgumentException(
                "A BKE plugin version is required.",
                nameof(version));
        }

        var response = await _agent.AuthorizeLauncherPluginAsync(
            new LauncherPluginAuthorizeRequest(
                Guid.NewGuid().ToString("N"),
                productId,
                version),
            cancellationToken);

        if (response.CapabilityId !=
                AgentLocalContract.LauncherPluginAuthorizeCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.LauncherPluginAuthorizeContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent Launcher-plugin authorization contract drifted.");
        }

        if (response.Authorized)
        {
            if (response.Status != "AUTHORIZED" ||
                response.Reason != "authorized" ||
                response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent returned an inconsistent Launcher-plugin authorization grant.");
            }
        }
        else if (response.Status is not (
                     "AUTH_REQUIRED" or
                     "DENIED" or
                     "FAILED") ||
                 string.IsNullOrWhiteSpace(response.Reason))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent returned an inconsistent Launcher-plugin authorization denial.");
        }

        return new LauncherPluginAuthorizationDecision(
            response.Status,
            response.Authorized,
            response.Reason,
            response.Error?.Message);
    }
}
