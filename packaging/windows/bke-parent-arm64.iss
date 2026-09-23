#ifndef BkeVersion
#define BkeVersion "0.1.0"
#endif
#ifndef AgentVersion
#define AgentVersion "2.0.0"
#endif
#define AppName "BKE"
#define AppPublisher "BKE Digital Solutions"
#define AgentServiceName "BKE-Licensing-Agent"
#define InstallDir "{autopf}\BKE Digital Solutions\BKE"
#define AgentInstaller "BKE-Licensing-Agent-" + AgentVersion + "-Windows-arm64.exe"

[Setup]
AppId={{BKE-Parent-PREPRODUCTION}}
AppName={#AppName}
AppVersion={#BkeVersion}
AppPublisher={#AppPublisher}
DefaultDirName={#InstallDir}
DefaultGroupName={#AppName}
OutputDir=..\..\dist\installer
OutputBaseFilename=BKE-{#BkeVersion}-PREPRODUCTION-Windows-arm64
ArchitecturesAllowed=arm64
ArchitecturesInstallIn64BitMode=arm64
PrivilegesRequired=admin
Compression=lzma
SolidCompression=yes
WizardStyle=modern
CloseApplications=yes
RestartApplications=no
Uninstallable=yes
CreateUninstallRegKey=yes
UninstallDisplayName={#AppName}
VersionInfoCompany={#AppPublisher}
VersionInfoDescription=BKE parent installer PREPRODUCTION
VersionInfoProductName={#AppName}

[Files]
Source: "..\..\dist\windows-arm64\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
Source: "..\..\agent-src\dist\installer\{#AgentInstaller}"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "..\..\dist\parent\COMPONENT-MANIFEST.json"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\BKE"; Filename: "{app}\bke-launcher.exe"
Name: "{autodesktop}\BKE"; Filename: "{app}\bke-launcher.exe"

[Run]
Filename: "{app}\bke-launcher.exe"; Description: "Open BKE"; Flags: postinstall nowait skipifsilent

[Code]
function AgentHealthy: Boolean;
var
  ResultCode: Integer;
  PowerShell: String;
  Parameters: String;
begin
  PowerShell := ExpandConstant('{sys}\WindowsPowerShell\v1.0\powershell.exe');
  Parameters := '-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command "' +
    '$deadline=[DateTime]::UtcNow.AddSeconds(60); ' +
    'while ([DateTime]::UtcNow -lt $deadline) { ' +
    '$s=Get-Service -Name ''' + '{#AgentServiceName}' + ''' -ErrorAction SilentlyContinue; ' +
    'if (($null -ne $s) -and ($s.Status -eq ''Running'')) { try { ' +
    '$r=Invoke-WebRequest -UseBasicParsing -Uri ''http://127.0.0.1:43873/license-center?product_id=bke-parent-health&version={#AgentVersion}&installation_id=bke-parent-installer'' -TimeoutSec 2; ' +
    'if ($r.StatusCode -eq 200) { exit 0 } } catch {} }; ' +
    'Start-Sleep -Milliseconds 500 }; exit 1"';
  if not Exec(PowerShell, Parameters, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    Result := False;
    Exit;
  end;
  Result := ResultCode = 0;
end;

procedure RemoveAgentCustomerShortcut;
begin
  DelTree(
    ExpandConstant('{commonprograms}\BKE Licensing Agent'),
    True,
    True,
    True);
end;

function AgentRoot: String;
begin
  Result := ExpandConstant('{autopf}\BKE Digital Solutions\Licensing Agent');
end;

function AgentDataRoot: String;
begin
  Result := ExpandConstant('{commonappdata}\BKE Digital Solutions\Licensing Agent');
end;

procedure RunManagedProductCleanup;
var
  ResultCode: Integer;
  CleanupExecutable: String;
begin
  CleanupExecutable := AgentRoot + '\root-cleanup\bke-root-cleanup.exe';
  if not FileExists(CleanupExecutable) then
    RaiseException(
      'BKE cannot safely uninstall because the Licensing Agent root-cleanup helper is missing. Repair BKE and try again.');

  if not Exec(
    CleanupExecutable,
    '--remove-managed-products',
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode) then
    RaiseException('BKE could not start managed-product cleanup.');

  if ResultCode <> 0 then
    RaiseException(
      Format(
        'BKE stopped uninstall because one or more managed products could not be removed safely (cleanup exit code %d).',
        [ResultCode]));

  Log('All BKE-managed products were removed or verified absent before platform teardown.');
end;

procedure WaitForAgentServiceRemoval;
var
  ResultCode: Integer;
  PowerShell: String;
  Parameters: String;
begin
  PowerShell := ExpandConstant('{sys}\WindowsPowerShell\v1.0\powershell.exe');
  Parameters := '-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command "' +
    '$deadline=[DateTime]::UtcNow.AddSeconds(60); ' +
    'while ([DateTime]::UtcNow -lt $deadline) { ' +
    '$s=Get-Service -Name ''' + '{#AgentServiceName}' + ''' -ErrorAction SilentlyContinue; ' +
    'if ($null -eq $s) { exit 0 }; Start-Sleep -Milliseconds 500 }; exit 1"';

  if not Exec(
    PowerShell,
    Parameters,
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode) then
    RaiseException('BKE could not verify Licensing Agent service removal.');

  if ResultCode <> 0 then
    RaiseException('BKE Licensing Agent service remained registered after Agent uninstall.');
end;

procedure UninstallLicensingAgent;
var
  ResultCode: Integer;
  AgentUninstaller: String;
begin
  AgentUninstaller := AgentRoot + '\unins000.exe';
  if not FileExists(AgentUninstaller) then
    RaiseException(
      'BKE cannot safely uninstall because the Licensing Agent uninstaller is missing. Repair BKE and try again.');

  if not Exec(
    AgentUninstaller,
    '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART',
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode) then
    RaiseException('BKE could not start the Licensing Agent uninstaller.');

  if ResultCode <> 0 then
    RaiseException(
      Format(
        'BKE stopped uninstall because the Licensing Agent uninstaller failed with exit code %d.',
        [ResultCode]));

  WaitForAgentServiceRemoval;

  if DirExists(AgentRoot) then
    RaiseException('BKE Licensing Agent files remained after Agent uninstall.');

  { Agent ProgramData is platform machine state, not user projects/exports. }
  DelTree(AgentDataRoot, True, True, True);
  if DirExists(AgentDataRoot) then
    RaiseException('BKE Licensing Agent machine state could not be removed.');

  Log('BKE Licensing Agent and machine state removed.');
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep = usUninstall then
  begin
    Log('BKE root uninstall started.');
    RunManagedProductCleanup;
    UninstallLicensingAgent;
    Log('BKE root dependencies removed; continuing BKE application uninstall.');
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ResultCode: Integer;
  AgentInstallerPath: String;
begin
  if CurStep <> ssPostInstall then
    Exit;

  AgentInstallerPath := ExpandConstant('{tmp}\{#AgentInstaller}');
  if not Exec(
    AgentInstallerPath,
    '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART',
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode) then
    RaiseException('BKE could not start the bundled Licensing Agent installer.');

  if ResultCode <> 0 then
    RaiseException(Format('Bundled Licensing Agent installation failed with exit code %d.', [ResultCode]));

  if not AgentHealthy then
    RaiseException('Bundled Licensing Agent did not become healthy after installation.');

  RemoveAgentCustomerShortcut;
  Log('Bundled Licensing Agent is healthy; BKE remains the only customer-facing desktop entry.');
end;
