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
UninstallDisplayIcon={app}\bke-launcher.exe
VersionInfoCompany={#AppPublisher}
VersionInfoDescription=BKE parent installer PREPRODUCTION
VersionInfoProductName={#AppName}

[Files]
Source: "..\..\dist\windows-arm64\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
Source: "..\..\agent-src\dist\installer\{#AgentInstaller}"; DestDir: "{tmp}"; Flags: deleteafterinstall
Source: "..\..\dist\parent\COMPONENT-MANIFEST.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\packaging\windows\bke-root-uninstall.ps1"; DestDir: "{app}\lifecycle"; Flags: ignoreversion

[UninstallDelete]
Type: filesandordirs; Name: "{app}"

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


function InitializeUninstall(): Boolean;
var
  ResultCode: Integer;
  PowerShell: String;
  ScriptPath: String;
  Parameters: String;
begin
  Result := False;
  ScriptPath := ExpandConstant('{app}\lifecycle\bke-root-uninstall.ps1');

  if not FileExists(ScriptPath) then
  begin
    Log('BKE root uninstall refused teardown because the lifecycle orchestrator is missing.');
    Exit;
  end;

  PowerShell := ExpandConstant('{sys}\WindowsPowerShell\v1.0\powershell.exe');
  Parameters := '-NoProfile -NonInteractive -ExecutionPolicy Bypass -File "' + ScriptPath + '"';

  if not Exec(
    PowerShell,
    Parameters,
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode) then
  begin
    Log('BKE root uninstall could not execute the lifecycle orchestrator.');
    Exit;
  end;

  if ResultCode <> 0 then
  begin
    Log(Format('BKE root uninstall prerequisite cleanup failed with exit code %d; BKE will remain installed.', [ResultCode]));
    Exit;
  end;

  Log('BKE managed products and Licensing Agent were removed safely; continuing BKE uninstall.');
  Result := True;
end;
