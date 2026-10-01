using System.Text.RegularExpressions;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountLicenseSeatsController
{
    private const int MaximumTargets = 500;

    private static readonly Regex LicenseHandlePattern = new(
        "^bke-license-seat-v1_[0-9a-f]{64}$",
        RegexOptions.CultureInvariant);

    private static readonly Regex TargetHandlePattern = new(
        "^bke-license-seat-user-v1_[0-9a-f]{64}$",
        RegexOptions.CultureInvariant);

    private readonly ILauncherAgentClient _agent;

    public LauncherAccountLicenseSeatsController(
        ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<AccountLicenseSeatsResponse> GetAsync(
        string licenseManagementHandle,
        CancellationToken cancellationToken)
    {
        if (!ValidLicenseHandle(licenseManagementHandle))
        {
            throw new InvalidDataException(
                "The license seat management handle is invalid.");
        }

        var response = await _agent.GetAccountLicenseSeatsAsync(
            new AccountLicenseSeatsRequest(
                Guid.NewGuid().ToString("N"),
                licenseManagementHandle),
            cancellationToken);

        ValidateRead(response);
        return response;
    }

    public async Task<AccountLicenseSeatsManageResponse> ManageAsync(
        string action,
        string licenseManagementHandle,
        string targetManagementHandle,
        CancellationToken cancellationToken)
    {
        if (action is not ("ASSIGN" or "REMOVE") ||
            !ValidLicenseHandle(licenseManagementHandle) ||
            !ValidTargetHandle(targetManagementHandle))
        {
            throw new InvalidDataException(
                "The license seat change intent is invalid.");
        }

        var response = await _agent.ManageAccountLicenseSeatsAsync(
            new AccountLicenseSeatsManageRequest(
                Guid.NewGuid().ToString("N"),
                action,
                licenseManagementHandle,
                targetManagementHandle),
            cancellationToken);

        ValidateManage(response);
        return response;
    }

    private static void ValidateRead(
        AccountLicenseSeatsResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountLicenseSeatsCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountLicenseSeatsContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account license seat contract drifted.");
        }

        if (response.Status is not (
            "READY" or
            "AUTH_REQUIRED" or
            "FORBIDDEN" or
            "NOT_FOUND" or
            "ACCOUNT_NOT_ACTIVE" or
            "LICENSE_NOT_ACTIVE" or
            "FAILED" or
            "INVALID_INPUT"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account license seat status drifted.");
        }

        if (response.Targets is null)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account license seat targets drifted.");
        }

        if (response.Status != "READY")
        {
            if (response.License is not null ||
                response.Targets.Count != 0 ||
                response.Error is null ||
                string.IsNullOrWhiteSpace(response.Error.Code) ||
                string.IsNullOrWhiteSpace(response.Error.Message))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent account license seat failure state drifted.");
            }

            return;
        }

        if (response.License is null ||
            response.Error is not null ||
            response.Targets.Count > MaximumTargets)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account license seat ready state drifted.");
        }

        ValidateLicense(response.License);

        foreach (var target in response.Targets)
        {
            if (target is null ||
                !ValidText(target.Email, 3, 320) ||
                !target.Email.Contains('@', StringComparison.Ordinal) ||
                !ValidOptionalText(target.Name, 160) ||
                !ValidTargetHandle(target.ManagementHandle) ||
                target.Assigned && !target.Eligible &&
                    string.IsNullOrWhiteSpace(target.Email))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent account license seat target drifted.");
            }
        }
    }

    private static void ValidateManage(
        AccountLicenseSeatsManageResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountLicenseSeatsCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountLicenseSeatsContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account license seat mutation contract drifted.");
        }

        var success = response.Status is
            "ASSIGNED" or
            "EXISTING" or
            "REMOVED" or
            "NOT_ASSIGNED";

        if (!success &&
            response.Status is not (
                "AUTH_REQUIRED" or
                "FORBIDDEN" or
                "NOT_FOUND" or
                "ACCOUNT_NOT_ACTIVE" or
                "LICENSE_NOT_ACTIVE" or
                "CONFLICT" or
                "INVALID_INPUT" or
                "OUTCOME_UNKNOWN" or
                "FAILED"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account license seat mutation status drifted.");
        }

        if (success)
        {
            if (response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent account license seat success returned an error.");
            }

            return;
        }

        if (response.Error is null ||
            string.IsNullOrWhiteSpace(response.Error.Code) ||
            string.IsNullOrWhiteSpace(response.Error.Message) ||
            response.Status == "OUTCOME_UNKNOWN" &&
                response.Error.Retryable)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account license seat mutation failure drifted.");
        }
    }

    private static void ValidateLicense(
        AccountLicenseSeatInfo license)
    {
        if (!ValidText(license.ProductName, 1, 200) ||
            !ValidOptionalText(license.EditionName, 120) ||
            string.IsNullOrEmpty(license.KeyLastFour) ||
            license.KeyLastFour.Length != 4 ||
            license.KeyLastFour.Any(character => character < 32) ||
            license.MaxSeats < 0 ||
            license.AssignedSeats < 0 ||
            license.AvailableSeats < 0 ||
            license.AssignedSeats > license.MaxSeats ||
            license.AvailableSeats !=
                Math.Max(0, license.MaxSeats - license.AssignedSeats))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent account license seat capacity drifted.");
        }
    }

    internal static bool ValidLicenseHandle(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        LicenseHandlePattern.IsMatch(value);

    internal static bool ValidTargetHandle(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        TargetHandlePattern.IsMatch(value);

    private static bool ValidOptionalText(
        string? value,
        int maximum) =>
        value is null ||
        ValidText(value, 0, maximum);

    private static bool ValidText(
        string? value,
        int minimum,
        int maximum) =>
        value is not null &&
        value.Length >= minimum &&
        value.Length <= maximum &&
        value.All(character => character >= 32);
}
