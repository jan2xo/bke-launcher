using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherPersistentGiftClaimsController
{
    private const string GiftHandlePrefix = "bke-gift-claim-v1_";
    private readonly ILauncherAgentClient _agent;

    public LauncherPersistentGiftClaimsController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<StoreGiftClaimsResponse> GetAsync(
        CancellationToken cancellationToken)
    {
        var correlationId = NewCorrelationId();
        var response = await _agent.GetStoreGiftClaimsAsync(
            new StoreGiftClaimsRequest(correlationId),
            cancellationToken);
        ValidateList(response, correlationId);
        return response;
    }

    public async Task<StoreGiftClaimPersistentRevealResponse> RevealAsync(
        string giftClaimHandle,
        CancellationToken cancellationToken)
    {
        ValidateGiftHandle(giftClaimHandle);
        var correlationId = NewCorrelationId();
        var response = await _agent.RevealPersistentStoreGiftClaimAsync(
            new StoreGiftClaimPersistentRevealRequest(
                correlationId,
                giftClaimHandle),
            cancellationToken);
        ValidateReveal(response, correlationId, giftClaimHandle);
        return response;
    }

    public async Task<AccountRecentAuthResponse> StartRecentAuthAsync(
        string currentPassword,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(currentPassword) ||
            currentPassword.Length > 128)
        {
            throw new ArgumentException(
                "Current password is invalid.",
                nameof(currentPassword));
        }

        var correlationId = NewCorrelationId();
        var response = await _agent.StartAccountRecentAuthAsync(
            new AccountRecentAuthStartRequest(
                correlationId,
                currentPassword),
            cancellationToken);
        ValidateRecentAuth(response, correlationId);
        return response;
    }

    public async Task<AccountRecentAuthResponse> CompleteRecentAuthAsync(
        string challengeToken,
        string code,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(challengeToken) ||
            challengeToken.Length > 512 ||
            string.IsNullOrWhiteSpace(code) ||
            code.Length is < 6 or > 32)
        {
            throw new ArgumentException("MFA proof is invalid.");
        }

        var correlationId = NewCorrelationId();
        var response = await _agent.CompleteAccountRecentAuthAsync(
            new AccountRecentAuthCompleteRequest(
                correlationId,
                challengeToken,
                code),
            cancellationToken);
        ValidateRecentAuth(response, correlationId);
        return response;
    }

    private static void ValidateList(
        StoreGiftClaimsResponse response,
        string correlationId)
    {
        if (response.CapabilityId !=
                AgentLocalContract.StoreGiftClaimsCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.StoreGiftClaimsContractVersion ||
            response.CorrelationId != correlationId)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent persistent Gift Claim Code list contract drifted.");
        }

        if (response.Status == "READY")
        {
            if (string.IsNullOrWhiteSpace(
                    response.AccountLifecycleState) ||
                response.Claims.Count > 100)
            {
                throw new InvalidDataException(
                    "Persistent Gift Claim Code list omitted required metadata.");
            }

            foreach (var claim in response.Claims)
            {
                ValidateGiftHandle(claim.GiftClaimHandle);
                if (claim.Status is not (
                        "AVAILABLE" or "CLAIMED" or
                        "REVOKED" or "EXPIRED") ||
                    string.IsNullOrWhiteSpace(claim.OrderNumber) ||
                    string.IsNullOrWhiteSpace(claim.ProductName) ||
                    !DateTimeOffset.TryParse(
                        claim.CreatedAt,
                        out _) ||
                    (claim.ExpiresAt is not null &&
                     !DateTimeOffset.TryParse(
                        claim.ExpiresAt,
                        out _)))
                {
                    throw new InvalidDataException(
                        "Persistent Gift Claim Code metadata drifted.");
                }
            }
        }
        else if (response.Claims.Count != 0)
        {
            throw new InvalidDataException(
                "Persistent Gift Claim Code failure exposed list metadata.");
        }
    }

    private static void ValidateReveal(
        StoreGiftClaimPersistentRevealResponse response,
        string correlationId,
        string expectedHandle)
    {
        if (response.CapabilityId !=
                AgentLocalContract.StoreGiftClaimPersistentRevealCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.StoreGiftClaimPersistentRevealContractVersion ||
            response.CorrelationId != correlationId)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent persistent Gift Claim Code reveal contract drifted.");
        }

        if (response.Status == "AVAILABLE")
        {
            if (response.GiftClaimHandle != expectedHandle ||
                string.IsNullOrWhiteSpace(response.ClaimCode) ||
                !response.ClaimCode.StartsWith(
                    "BKE-CLM-",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidDataException(
                    "Persistent Gift Claim Code reveal omitted required secret material.");
            }
        }
        else if (response.ClaimCode is not null)
        {
            throw new InvalidDataException(
                "Persistent Gift Claim Code plaintext escaped outside AVAILABLE state.");
        }
    }

    private static void ValidateRecentAuth(
        AccountRecentAuthResponse response,
        string correlationId)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountRecentAuthCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountRecentAuthContractVersion ||
            response.CorrelationId != correlationId)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent recent-auth contract drifted.");
        }

        if (response.Status == "VERIFIED")
        {
            if (string.IsNullOrWhiteSpace(
                    response.RecentAuthenticatedUntil) ||
                !DateTimeOffset.TryParse(
                    response.RecentAuthenticatedUntil,
                    out _) ||
                response.ChallengeToken is not null)
            {
                throw new InvalidDataException(
                    "Recent-auth verified response drifted.");
            }
        }
        else if (response.Status == "MFA_CHALLENGE_ISSUED")
        {
            if (string.IsNullOrWhiteSpace(response.ChallengeToken) ||
                string.IsNullOrWhiteSpace(response.ExpiresAt) ||
                !DateTimeOffset.TryParse(response.ExpiresAt, out _) ||
                !response.EmailSent.HasValue ||
                string.IsNullOrWhiteSpace(response.MfaReference))
            {
                throw new InvalidDataException(
                    "Recent-auth MFA challenge response drifted.");
            }
        }
        else if (response.RecentAuthenticatedUntil is not null ||
                 response.ChallengeToken is not null)
        {
            throw new InvalidDataException(
                "Recent-auth failure exposed success/challenge material.");
        }
    }

    private static void ValidateGiftHandle(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            !value.StartsWith(
                GiftHandlePrefix,
                StringComparison.Ordinal) ||
            value.Length != GiftHandlePrefix.Length + 64 ||
            !value[GiftHandlePrefix.Length..].All(character =>
                character is >= '0' and <= '9' ||
                character is >= 'a' and <= 'f'))
        {
            throw new InvalidDataException(
                "Gift Claim Code handle is invalid.");
        }
    }

    private static string NewCorrelationId() =>
        Guid.NewGuid().ToString("N");
}
