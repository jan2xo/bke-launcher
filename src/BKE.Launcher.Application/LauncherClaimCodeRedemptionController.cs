using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherClaimCodeRedemptionController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherClaimCodeRedemptionController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public Task<ClaimCodeRedeemResponse> RedeemAsync(
        string code,
        CancellationToken cancellationToken) =>
        _agent.RedeemClaimCodeAsync(
            new ClaimCodeRedeemRequest(
                Guid.NewGuid().ToString("N"),
                code.Trim()),
            cancellationToken);
}
