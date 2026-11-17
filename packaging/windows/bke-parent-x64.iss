#ifndef BkeVersion
#define BkeVersion "0.1.0"
#endif
#ifndef AgentVersion
#define AgentVersion "2.0.2"
#endif
#define AppName "BKE"
#define AppPublisher "BKE Digital Solutions"
#define AgentServiceName "BKE-Licensing-Agent"
#define InstallDir "{autopf}\BKE Digital Solutions\BKE"
#define AgentInstaller "BKE-Licensing-Agent-" + AgentVersion + "-Windows-x64.exe"

[Setup]
AppId={{BKE-Parent-PREPRODUCTION}}
AppName={#AppName}
AppVersion={#BkeVersion}
AppPublisher={#AppPublisher}
DefaultDirName={#InstallDir}
DefaultGroupName={#AppName}
OutputDir=..\..\dist\installer
OutputBaseFilename=BKE-{#BkeVersion}-PREPRODUCTION-Windows
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
UninstallDisplayIcon={app}\bke-launcher.exe
VersionInfoCompany={#AppPublisher}
VersionInfoDescription=BKE Windows x64 installer PREPRODUCTION
VersionInfoProductName={#AppName}

[Files]
Source: "..\..\dist\windows-x64\*"; DestDir: "{app}"; Flags: recursesubdirs ignoreversion
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
function CompareVersionPart(Part1, Part2: String): Integer;
var
  N1, N2: Integer;
begin
  N1 := StrToIntDef(Part1, 0);
  N2 := StrToIntDef(Part2, 0);
  if N1 > N2 then
    Result := 1
  else if N1 < N2 then
    Result := -1
  else
    Result := 0;
end;

function CompareVersions(V1, V2: String): Integer;
var
  P1, P2: Integer;
  Part1, Part2: String;
  C: Integer;
begin
  Result := 0;
  while (Length(V1) > 0) or (Length(V2) > 0) do
  begin
    P1 := Pos('.', V1);
    if P1 > 0 then
    begin
      Part1 := Copy(V1, 1, P1 - 1);
      Delete(V1, 1, P1);
    end
    else
    begin
      Part1 := V1;
      V1 := '';
    end;

    P2 := Pos('.', V2);
    if P2 > 0 then
    begin
      Part2 := Copy(V2, 1, P2 - 1);
      Delete(V2, 1, P2);
    end
    else
    begin
      Part2 := V2;
      V2 := '';
    end;

    C := CompareVersionPart(Part1, Part2);
    if C <> 0 then
    begin
      Result := C;
      Exit;
    end;
  end;
end;

function GetInstalledAgentVersion(var VersionStr: String): Boolean;
var
  ExePath: String;
begin
  Result := False;
  VersionStr := '';

  if RegQueryStringValue(HKLM64, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{BKE-Licensing-Agent}_is1', 'DisplayVersion', VersionStr) or
     RegQueryStringValue(HKLM32, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\{BKE-Licensing-Agent}_is1', 'DisplayVersion', VersionStr) or
     RegQueryStringValue(HKLM64, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\BKE-Licensing-Agent_is1', 'DisplayVersion', VersionStr) or
     RegQueryStringValue(HKLM32, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\BKE-Licensing-Agent_is1', 'DisplayVersion', VersionStr) then
  begin
    VersionStr := Trim(VersionStr);
    if Length(VersionStr) > 0 then
    begin
      Result := True;
      Exit;
    end;
  end;

  ExePath := ExpandConstant('{autopf}\BKE Digital Solutions\Licensing Agent\service\bke-licensing-agent-service.exe');
  if FileExists(ExePath) and GetVersionNumbersString(ExePath, VersionStr) then
  begin
    VersionStr := Trim(VersionStr);
    if Length(VersionStr) > 0 then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

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
  InstalledAgentVer: String;
  Comparison: Integer;
  ShouldRunInstaller: Boolean;
begin
  if CurStep <> ssPostInstall then
    Exit;

  ShouldRunInstaller := True;
  if GetInstalledAgentVersion(InstalledAgentVer) then
  begin
    Log(Format('Detected installed Licensing Agent version: %s (bundled version: %s)', [InstalledAgentVer, '{#AgentVersion}']));
    Comparison := CompareVersions(InstalledAgentVer, '{#AgentVersion}');
    if Comparison > 0 then
    begin
      Log('Installed Licensing Agent is strictly newer than bundled version. Preserving newer Agent to prevent unsafe downgrade.');
      ShouldRunInstaller := False;
    end
    else if Comparison = 0 then
    begin
      Log('Installed Licensing Agent version matches bundled version.');
      if AgentHealthy then
      begin
        Log('Existing Licensing Agent is healthy; skipping reinstall.');
        ShouldRunInstaller := False;
      end
      else
      begin
        Log('Existing Licensing Agent is not responding; running bundled installer to repair.');
        ShouldRunInstaller := True;
      end;
    end
    else
    begin
      Log('Installed Licensing Agent is older than bundled version. Proceeding with upgrade.');
      ShouldRunInstaller := True;
    end;
  end
  else
  begin
    Log('No existing Licensing Agent detected. Proceeding with fresh installation.');
    ShouldRunInstaller := True;
  end;

  if ShouldRunInstaller then
  begin
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
  end;

  if not AgentHealthy then
    RaiseException('Licensing Agent did not become healthy after installation.');

  RemoveAgentCustomerShortcut;
  Log('Licensing Agent is healthy; BKE remains the only customer-facing desktop entry.');
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
