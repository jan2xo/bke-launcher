$ErrorActionPreference = "Stop"

$agentRoot = Join-Path $env:ProgramFiles "BKE Digital Solutions\Licensing Agent"
$cleanup = Join-Path $agentRoot "root-cleanup\bke-root-cleanup.exe"

function Fail([int]$Code, [string]$Message) {
    [Console]::Error.WriteLine($Message)
    exit $Code
}

function Parse-InnoUninstaller([string]$Command) {
    if ([string]::IsNullOrWhiteSpace($Command)) {
        return $null
    }

    if ($Command -match '^\s*"([^"]+)"') {
        return $Matches[1]
    }

    if ($Command -match '^\s*(\S+\.exe)(?:\s|$)') {
        return $Matches[1]
    }

    return $null
}

function Get-AgentUninstallCandidates {
    $registryRoots = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
    )

    $fullRoot = [IO.Path]::GetFullPath($agentRoot).TrimEnd('\') + '\'

    foreach ($registryRoot in $registryRoots) {
        if (-not (Test-Path -LiteralPath $registryRoot)) {
            continue
        }

        foreach ($key in Get-ChildItem -LiteralPath $registryRoot -ErrorAction SilentlyContinue) {
            $entry = Get-ItemProperty -LiteralPath $key.PSPath -ErrorAction SilentlyContinue
            if ($null -eq $entry) {
                continue
            }

            if ([string]$entry.DisplayName -ne "BKE Licensing Agent" -or
                [string]$entry.Publisher -ne "BKE Digital Solutions") {
                continue
            }

            $uninstaller = Parse-InnoUninstaller ([string]$entry.UninstallString)
            if ([string]::IsNullOrWhiteSpace($uninstaller)) {
                continue
            }

            $fullUninstaller = [IO.Path]::GetFullPath($uninstaller)
            if (-not $fullUninstaller.StartsWith($fullRoot, [StringComparison]::OrdinalIgnoreCase)) {
                continue
            }

            if ([IO.Path]::GetFileName($fullUninstaller) -notmatch '^unins\d+\.exe$') {
                continue
            }

            [pscustomobject]@{
                RegistryPath = $key.PSPath
                Uninstaller = $fullUninstaller
            }
        }
    }
}

$candidates = @(Get-AgentUninstallCandidates)
if ($candidates.Count -ne 1) {
    Fail 31 "BKE root uninstall could not resolve exactly one trusted Licensing Agent uninstall registration."
}

$agentUninstaller = [string]$candidates[0].Uninstaller
if (-not (Test-Path -LiteralPath $agentUninstaller -PathType Leaf)) {
    Fail 37 "BKE root uninstall could not find the registered Licensing Agent uninstaller."
}

if (-not (Test-Path -LiteralPath $cleanup -PathType Leaf)) {
    Fail 20 "BKE root uninstall cannot verify managed products because the Licensing Agent cleanup helper is missing."
}

& $cleanup --remove-managed-products
$cleanupExit = $LASTEXITCODE
if ($cleanupExit -ne 0) {
    Fail $cleanupExit "BKE root uninstall stopped because managed product cleanup failed with exit code $cleanupExit."
}

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

$remaining = @(Get-AgentUninstallCandidates)
if ($remaining.Count -ne 0) {
    Fail 43 "BKE root uninstall verification failed because a trusted Licensing Agent uninstall registration still exists."
}

Write-Host "BKE root uninstall prerequisite cleanup: PASS"
exit 0
