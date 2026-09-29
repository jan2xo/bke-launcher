using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountMfaController
{
    private readonly ILauncherAgentClient _agent;

    public LauncherAccountMfaController(ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<AccountMfaStatusResponse> StatusAsync(
        CancellationToken cancellationToken) =>
        Validate(await _agent.GetAccountMfaStatusAsync(
            new AccountMfaStatusRequest(NewCorrelationId()),
            cancellationToken));

    public async Task<AccountMfaChallengeResponse> EnrollStartAsync(
        string currentPassword,
        CancellationToken cancellationToken)
    {
        ValidatePassword(currentPassword);
        return Validate(await _agent.StartAccountMfaEnrollmentAsync(
            new AccountMfaEnrollStartRequest(NewCorrelationId(), currentPassword),
            cancellationToken));
    }

    public async Task<AccountMfaMutationResponse> EnrollCompleteAsync(
        string currentPassword,
        string challengeToken,
        string code,
        CancellationToken cancellationToken)
    {
        ValidateProof(currentPassword, challengeToken, code);
        return Validate(await _agent.CompleteAccountMfaEnrollmentAsync(
            new AccountMfaEnrollCompleteRequest(
                NewCorrelationId(),
                currentPassword,
                challengeToken,
                code),
            cancellationToken));
    }

    public async Task<AccountMfaChallengeResponse> ChallengeAsync(
        string currentPassword,
        CancellationToken cancellationToken)
    {
        ValidatePassword(currentPassword);
        return Validate(await _agent.StartAccountMfaProofAsync(
            new AccountMfaProofChallengeRequest(NewCorrelationId(), currentPassword),
            cancellationToken));
    }

    public async Task<AccountMfaMutationResponse> DisableAsync(
        string currentPassword,
        string challengeToken,
        string code,
        CancellationToken cancellationToken)
    {
        ValidateProof(currentPassword, challengeToken, code);
        return Validate(await _agent.DisableAccountMfaAsync(
            new AccountMfaMutationRequest(
                NewCorrelationId(),
                currentPassword,
                challengeToken,
                code),
            cancellationToken));
    }

    public async Task<AccountMfaMutationResponse> RegenerateRecoveryAsync(
        string currentPassword,
        string challengeToken,
        string code,
        CancellationToken cancellationToken)
    {
        ValidateProof(currentPassword, challengeToken, code);
        return Validate(await _agent.RegenerateAccountMfaRecoveryAsync(
            new AccountMfaMutationRequest(
                NewCorrelationId(),
                currentPassword,
                challengeToken,
                code),
            cancellationToken));
    }

    private static void ValidatePassword(string value)
    {
        if (string.IsNullOrEmpty(value) || value.Length > 128)
        {
            throw new ArgumentException(
                "A valid current password is required.",
                nameof(value));
        }
    }

    private static void ValidateProof(
        string currentPassword,
        string challengeToken,
        string code)
    {
        ValidatePassword(currentPassword);
        if (string.IsNullOrWhiteSpace(challengeToken) ||
            challengeToken.Length is < 16 or > 512)
        {
            throw new ArgumentException(
                "A valid MFA challenge is required.",
                nameof(challengeToken));
        }
        if (string.IsNullOrWhiteSpace(code) ||
            code.Length is < 6 or > 32)
        {
            throw new ArgumentException(
                "A valid MFA code is required.",
                nameof(code));
        }
    }

    private static AccountMfaStatusResponse Validate(AccountMfaStatusResponse response)
    {
        ValidateContract(response.CapabilityId, response.ContractVersion);
        return response;
    }

    private static AccountMfaChallengeResponse Validate(AccountMfaChallengeResponse response)
    {
        ValidateContract(response.CapabilityId, response.ContractVersion);
        return response;
    }

    private static AccountMfaMutationResponse Validate(AccountMfaMutationResponse response)
    {
        ValidateContract(response.CapabilityId, response.ContractVersion);
        return response;
    }

    private static void ValidateContract(string capabilityId, int contractVersion)
    {
        if (capabilityId != AgentLocalContract.AccountMfaCapabilityId ||
            contractVersion != AgentLocalContract.AccountMfaContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account MFA contract drifted.");
        }
    }

    private static string NewCorrelationId() => Guid.NewGuid().ToString("N");
}
