$ErrorActionPreference = "Stop"

$agentRoot = Join-Path $env:ProgramFiles "BKE Digital Solutions\Licensing Agent"
$cleanup = Join-Path $agentRoot "root-cleanup\bke-root-cleanup.exe"
$agentUninstallKey = "{BKE-Licensing-Agent}_is1"

function Fail([int]$Code, [string]$Message) {
    [Console]::Error.WriteLine($Message)
    exit $Code
}

function Resolve-AgentUninstaller {
    $registryPaths = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$agentUninstallKey",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\$agentUninstallKey"
    )

    $entries = @(
        foreach ($registryPath in $registryPaths) {
            if (Test-Path -LiteralPath $registryPath) {
                Get-ItemProperty -LiteralPath $registryPath
            }
        }
    )

    if ($entries.Count -ne 1) {
        Fail 31 "BKE root uninstall could not resolve exactly one trusted Licensing Agent uninstall registration."
    }

    $entry = $entries[0]
    if ([string]$entry.DisplayName -ne "BKE Licensing Agent") {
        Fail 32 "BKE root uninstall refused an unexpected Licensing Agent uninstall registration."
    }

    $command = [string]$entry.UninstallString
    if ([string]::IsNullOrWhiteSpace($command)) {
        Fail 33 "BKE root uninstall found no Licensing Agent uninstall command."
    }

    $uninstaller = $null
    if ($command -match '^\s*"([^"]+)"') {
        $uninstaller = $Matches[1]
    } elseif ($command -match '^\s*(\S+\.exe)(?:\s|$)') {
        $uninstaller = $Matches[1]
    }

    if ([string]::IsNullOrWhiteSpace($uninstaller)) {
        Fail 34 "BKE root uninstall could not parse the Licensing Agent uninstall command."
    }

    $fullRoot = [IO.Path]::GetFullPath($agentRoot).TrimEnd('\') + '\'
    $fullUninstaller = [IO.Path]::GetFullPath($uninstaller)

    if (-not $fullUninstaller.StartsWith($fullRoot, [StringComparison]::OrdinalIgnoreCase)) {
        Fail 35 "BKE root uninstall refused a Licensing Agent uninstaller outside the trusted Agent root."
    }

    if ([IO.Path]::GetFileName($fullUninstaller) -notmatch '^unins\d+\.exe$') {
        Fail 36 "BKE root uninstall refused an unexpected Licensing Agent uninstaller filename."
    }

    if (-not (Test-Path -LiteralPath $fullUninstaller -PathType Leaf)) {
        Fail 37 "BKE root uninstall could not find the registered Licensing Agent uninstaller."
    }

    return $fullUninstaller
}

if (-not (Test-Path -LiteralPath $cleanup -PathType Leaf)) {
    Fail 20 "BKE root uninstall cannot verify managed products because the Licensing Agent cleanup helper is missing."
}

& $cleanup --remove-managed-products
$cleanupExit = $LASTEXITCODE
if ($cleanupExit -ne 0) {
    Fail $cleanupExit "BKE root uninstall stopped because managed product cleanup failed with exit code $cleanupExit."
}

$agentUninstaller = Resolve-AgentUninstaller
$agentUninstall = Start-Process -FilePath $agentUninstaller -ArgumentList @(
    "/VERYSILENT",
    "/SUPPRESSMSGBOXES",
    "/NORESTART"
) -Wait -PassThru

if ($agentUninstall.ExitCode -ne 0) {
    Fail 40 "BKE root uninstall stopped because Licensing Agent uninstall failed with exit code $($agentUninstall.ExitCode)."
}

$deadline = [DateTime]::UtcNow.AddSeconds(30)
do {
    $service = Get-Service -Name "BKE-Licensing-Agent" -ErrorAction SilentlyContinue
    if ($null -eq $service) {
        break
    }
    Start-Sleep -Milliseconds 250
} while ([DateTime]::UtcNow -lt $deadline)

if ($null -ne (Get-Service -Name "BKE-Licensing-Agent" -ErrorAction SilentlyContinue)) {
    Fail 41 "BKE root uninstall verification failed because the Licensing Agent service still exists."
}

if (Test-Path -LiteralPath $agentRoot) {
    Fail 42 "BKE root uninstall verification failed because the Licensing Agent install root still exists."
}

$remainingRegistration = @(
    "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$agentUninstallKey",
    "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\$agentUninstallKey"
) | Where-Object { Test-Path -LiteralPath $_ }

if ($remainingRegistration.Count -ne 0) {
    Fail 43 "BKE root uninstall verification failed because the Licensing Agent uninstall registration still exists."
}

Write-Host "BKE root uninstall prerequisite cleanup: PASS"
exit 0
