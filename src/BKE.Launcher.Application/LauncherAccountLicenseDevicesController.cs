using System.Text.RegularExpressions;
using BKE.Launcher.Contracts;

namespace BKE.Launcher.Application;

public sealed class LauncherAccountLicenseDevicesController
{
    private const int MaximumDevices = 100;

    private static readonly Regex LicenseHandlePattern = new(
        "^bke-license-device-v1_[0-9a-f]{64}$",
        RegexOptions.CultureInvariant);

    private static readonly Regex DeviceHandlePattern = new(
        "^bke-license-device-target-v1_[0-9a-f]{64}$",
        RegexOptions.CultureInvariant);

    private readonly ILauncherAgentClient _agent;

    public LauncherAccountLicenseDevicesController(
        ILauncherAgentClient agent)
    {
        _agent = agent;
    }

    public async Task<AccountLicenseDevicesResponse> GetAsync(
        string licenseManagementHandle,
        CancellationToken cancellationToken)
    {
        if (!ValidLicenseHandle(licenseManagementHandle))
        {
            throw new InvalidDataException(
                "The authorized-device license handle is invalid.");
        }

        var response = await _agent.GetAccountLicenseDevicesAsync(
            new AccountLicenseDevicesRequest(
                Guid.NewGuid().ToString("N"),
                licenseManagementHandle),
            cancellationToken);

        ValidateRead(response);
        return response;
    }

    public async Task<AccountLicenseDeviceDeactivateResponse> DeactivateAsync(
        string licenseManagementHandle,
        string deviceManagementHandle,
        CancellationToken cancellationToken)
    {
        if (!ValidLicenseHandle(licenseManagementHandle) ||
            !ValidDeviceHandle(deviceManagementHandle))
        {
            throw new InvalidDataException(
                "The authorized-device deactivation intent is invalid.");
        }

        var response =
            await _agent.DeactivateAccountLicenseDeviceAsync(
                new AccountLicenseDeviceDeactivateRequest(
                    Guid.NewGuid().ToString("N"),
                    licenseManagementHandle,
                    deviceManagementHandle),
                cancellationToken);

        ValidateDeactivate(response);
        return response;
    }

    private static void ValidateRead(
        AccountLicenseDevicesResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountLicenseDevicesCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountLicenseDevicesContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent authorized-device contract drifted.");
        }

        if (response.Status is not (
            "READY" or
            "AUTH_REQUIRED" or
            "FORBIDDEN" or
            "NOT_FOUND" or
            "ACCOUNT_NOT_ACTIVE" or
            "FAILED" or
            "INVALID_INPUT"))
        {
            throw new InvalidDataException(
                "BKE Licensing Agent authorized-device status drifted.");
        }

        if (response.Devices is null)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent authorized-device list drifted.");
        }

        if (response.Status != "READY")
        {
            if (response.License is not null ||
                response.Devices.Count != 0 ||
                response.Error is null ||
                string.IsNullOrWhiteSpace(response.Error.Code) ||
                string.IsNullOrWhiteSpace(response.Error.Message))
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent authorized-device failure state drifted.");
            }

            return;
        }

        if (response.License is null ||
            response.Error is not null ||
            response.Devices.Count > MaximumDevices)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent authorized-device ready state drifted.");
        }

        if (!ValidText(response.License.ProductName, 1, 200) ||
            !ValidOptionalText(response.License.EditionName, 120) ||
            string.IsNullOrEmpty(response.License.KeyLastFour) ||
            response.License.KeyLastFour.Length != 4 ||
            response.License.KeyLastFour.Any(character => character < 32) ||
            response.License.MaxDevices < 0 ||
            response.License.ActiveDevices < 0 ||
            response.License.ActiveDevices > response.License.MaxDevices)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent authorized-device capacity drifted.");
        }

        foreach (var device in response.Devices)
        {
            if (device is null ||
                !ValidOptionalText(device.Label, 200) ||
                !ValidOptionalText(device.OperatingSystem, 120) ||
                !ValidOptionalText(device.Architecture, 80) ||
                !DateTimeOffset.TryParse(device.LastSeenAt, out _) ||
                !DateTimeOffset.TryParse(device.ActivatedAt, out _) ||
                device.Active && !ValidDeviceHandle(device.ManagementHandle) ||
                !device.Active && device.ManagementHandle is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent authorized-device item drifted.");
            }
        }
    }

    private static void ValidateDeactivate(
        AccountLicenseDeviceDeactivateResponse response)
    {
        if (response.CapabilityId !=
                AgentLocalContract.AccountLicenseDevicesCapabilityId ||
            response.ContractVersion !=
                AgentLocalContract.AccountLicenseDevicesContractVersion)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent authorized-device mutation contract drifted.");
        }

        if (response.Status == "DEACTIVATED")
        {
            if (response.Error is not null)
            {
                throw new InvalidDataException(
                    "BKE Licensing Agent authorized-device success returned an error.");
            }

            return;
        }

        if (response.Status is not (
                "AUTH_REQUIRED" or
                "FORBIDDEN" or
                "NOT_FOUND" or
                "ACCOUNT_NOT_ACTIVE" or
                "INVALID_INPUT" or
                "OUTCOME_UNKNOWN" or
                "FAILED") ||
            response.Error is null ||
            string.IsNullOrWhiteSpace(response.Error.Code) ||
            string.IsNullOrWhiteSpace(response.Error.Message) ||
            response.Status == "OUTCOME_UNKNOWN" &&
                response.Error.Retryable)
        {
            throw new InvalidDataException(
                "BKE Licensing Agent authorized-device mutation failure drifted.");
        }
    }

    internal static bool ValidLicenseHandle(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        LicenseHandlePattern.IsMatch(value);

    internal static bool ValidDeviceHandle(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        DeviceHandlePattern.IsMatch(value);

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
