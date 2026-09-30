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
        CancellationToken cancellationToken)
    {
        var normalized = code.Trim();
        if (!ValidClaimCode(normalized))
        {
            throw new ArgumentException(
                "Enter a valid BKE Claim Code.",
                nameof(code));
        }

        return _agent.RedeemClaimCodeAsync(
            new ClaimCodeRedeemRequest(
                Guid.NewGuid().ToString("N"),
                normalized),
            cancellationToken);
    }

    private static bool ValidClaimCode(string code)
    {
        if (!code.StartsWith(
                "BKE-CLM-",
                StringComparison.OrdinalIgnoreCase) ||
            code.Length != 43)
        {
            return false;
        }

        var groups = code[8..].Split('-');
        return groups.Length == 6 &&
            groups.All(group =>
                group.Length == 5 &&
                group.All(character =>
                    character is >= '0' and <= '9' ||
                    character is >= 'A' and <= 'F' ||
                    character is >= 'a' and <= 'f'));
    }
}
