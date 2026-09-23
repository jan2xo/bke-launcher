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
#define AgentInstallDir "{autopf}\BKE Digital Solutions\Licensing Agent"
#define AgentDataDir "{commonappdata}\BKE Digital Solutions\Licensing Agent"
#define AgentInstaller "BKE-Licensing-Agent-" + AgentVersion + "-Windows-x64.exe"
#define AgentUninstallKey "SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{BKE-Licensing-Agent}_is1"

[Setup]
AppId={{BKE-Parent-PREPRODUCTION}}
AppName={#AppName}
AppVersion={#BkeVersion}
AppPublisher={#AppPublisher}
DefaultDirName={#InstallDir}
DefaultGroupName={#AppName}
OutputDir=..\..\dist\installer
OutputBaseFilename=BKE-{#BkeVersion}-PREPRODUCTION-Windows-x64
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
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
Source: "..\..\dist\windows-x64\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
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

function AgentServiceGone: Boolean;
var
  ResultCode: Integer;
  PowerShell: String;
  Parameters: String;
begin
  PowerShell := ExpandConstant('{sys}\WindowsPowerShell\v1.0\powershell.exe');
  Parameters := '-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command "' +
    '$deadline=[DateTime]::UtcNow.AddSeconds(45); ' +
    'while ([DateTime]::UtcNow -lt $deadline) { ' +
    'if ($null -eq (Get-Service -Name ''' + '{#AgentServiceName}' + ''' -ErrorAction SilentlyContinue)) { exit 0 }; ' +
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

procedure HideAgentFromInstalledApps;
begin
  RegWriteDWordValue(
    HKLM,
    '{#AgentUninstallKey}',
    'SystemComponent',
    1);
end;

function RemoveManagedProductsForRootUninstall: Boolean;
var
  ResultCode: Integer;
  HelperPath: String;
  AgentRoot: String;
  AgentData: String;
begin
  AgentRoot := ExpandConstant('{#AgentInstallDir}');
  AgentData := ExpandConstant('{#AgentDataDir}');
  HelperPath := AgentRoot + '\root-cleanup\bke-root-cleanup.exe';

  if (not DirExists(AgentRoot)) and (not DirExists(AgentData)) then
  begin
    Result := True;
    Exit;
  end;

  if not FileExists(HelperPath) then
  begin
    Log('BKE root uninstall refused: Agent root cleanup helper is missing.');
    Result := False;
    Exit;
  end;

  if not Exec(
    HelperPath,
    '--remove-managed-products',
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode) then
  begin
    Log('BKE root uninstall could not start Agent root cleanup helper.');
    Result := False;
    Exit;
  end;

  Result := ResultCode = 0;
  if not Result then
    Log(Format('BKE root uninstall product cleanup failed with exit code %d.', [ResultCode]));
end;

function RemoveLicensingAgent: Boolean;
var
  ResultCode: Integer;
  AgentRoot: String;
  Uninstaller: String;
begin
  AgentRoot := ExpandConstant('{#AgentInstallDir}');
  Uninstaller := AgentRoot + '\unins000.exe';

  if not DirExists(AgentRoot) then
  begin
    Result := AgentServiceGone;
    Exit;
  end;

  if not FileExists(Uninstaller) then
  begin
    Log('BKE root uninstall refused: Agent uninstaller is missing.');
    Result := False;
    Exit;
  end;

  if not Exec(
    Uninstaller,
    '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART',
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode) then
  begin
    Log('BKE root uninstall could not start Agent uninstaller.');
    Result := False;
    Exit;
  end;

  if ResultCode <> 0 then
  begin
    Log(Format('Agent uninstaller failed with exit code %d.', [ResultCode]));
    Result := False;
    Exit;
  end;

  Result := AgentServiceGone and (not DirExists(AgentRoot));
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
  HideAgentFromInstalledApps;
  Log('Bundled Licensing Agent is healthy and hidden as a BKE implementation component.');
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if CurUninstallStep <> usUninstall then
    Exit;

  if not RemoveManagedProductsForRootUninstall then
  begin
    MsgBox(
      'BKE could not safely remove every managed product. No BKE platform components will be removed. Close running BKE software or repair the affected product, then try again.',
      mbError,
      MB_OK);
    RaiseException('BKE root uninstall stopped because managed product cleanup failed.');
  end;

  if not RemoveLicensingAgent then
  begin
    MsgBox(
      'BKE removed managed products but could not safely remove the Licensing Agent. BKE uninstall has been stopped.',
      mbError,
      MB_OK);
    RaiseException('BKE root uninstall stopped because Licensing Agent teardown failed.');
  end;

  DelTree(
    ExpandConstant('{#AgentDataDir}'),
    True,
    True,
    True);

  Log('BKE root uninstall removed all managed products and the Licensing Agent. BKE files may now be removed.');
end;
