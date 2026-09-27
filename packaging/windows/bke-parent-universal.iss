#ifndef BkeVersion
#define BkeVersion "0.1.0"
#endif
#define AppName "BKE"
#define AppPublisher "BKE Digital Solutions"
#define X64Parent "BKE-" + BkeVersion + "-PREPRODUCTION-Windows-x64.exe"
#define Arm64Parent "BKE-" + BkeVersion + "-PREPRODUCTION-Windows-arm64.exe"
#define X64ParentSource "..\..\dist\installer\" + X64Parent
#define Arm64ParentSource "..\..\dist\installer\" + Arm64Parent
#define X64ParentSha256 GetSHA256OfFile(X64ParentSource)
#define Arm64ParentSha256 GetSHA256OfFile(Arm64ParentSource)

[Setup]
AppId={{BKE-Universal-Parent-PREPRODUCTION}}
AppName={#AppName}
AppVersion={#BkeVersion}
AppPublisher={#AppPublisher}
DefaultDirName={tmp}\BKE Universal Setup
CreateAppDir=no
DisableDirPage=yes
DisableProgramGroupPage=yes
OutputDir=..\..\dist\installer
OutputBaseFilename=BKE-{#BkeVersion}-PREPRODUCTION-Windows
ArchitecturesAllowed=win64
ArchitecturesInstallIn64BitMode=win64
PrivilegesRequired=admin
Compression=lzma2
SolidCompression=no
WizardStyle=modern
CloseApplications=yes
RestartApplications=no
Uninstallable=no
CreateUninstallRegKey=no
VersionInfoCompany={#AppPublisher}
VersionInfoDescription=BKE universal Windows installer PREPRODUCTION
VersionInfoProductName={#AppName}

[Files]
Source: "{#X64ParentSource}"; Flags: dontcopy
Source: "{#Arm64ParentSource}"; Flags: dontcopy

[Run]
Filename: "{commonpf64}\BKE Digital Solutions\BKE\bke-launcher.exe"; Description: "Open BKE"; Flags: postinstall nowait skipifsilent runasoriginaluser; Check: BkeLauncherExists

[Code]
function SelectedParentName: String;
begin
  if IsArm64 then
    Result := '{#Arm64Parent}'
  else if IsX64OS then
    Result := '{#X64Parent}'
  else
    RaiseException('BKE supports Windows x64 and Windows ARM64 only.');
end;

function ExpectedParentSha256: String;
begin
  if IsArm64 then
    Result := '{#Arm64ParentSha256}'
  else if IsX64OS then
    Result := '{#X64ParentSha256}'
  else
    RaiseException('BKE supports Windows x64 and Windows ARM64 only.');
end;

function BkeLauncherExists: Boolean;
begin
  Result := FileExists(
    ExpandConstant('{commonpf64}\BKE Digital Solutions\BKE\bke-launcher.exe'));
end;

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  ParentName: String;
  ParentPath: String;
  ExpectedSha256: String;
  ActualSha256: String;
  ResultCode: Integer;
begin
  Result := '';
  NeedsRestart := False;

  ParentName := SelectedParentName;
  ExpectedSha256 := ExpectedParentSha256;

  Log(Format('BKE universal selector chose %s.', [ParentName]));
  ExtractTemporaryFile(ParentName);
  ParentPath := ExpandConstant('{tmp}\') + ParentName;

  if not FileExists(ParentPath) then
  begin
    Result := 'The selected BKE architecture payload could not be extracted.';
    Exit;
  end;

  ActualSha256 := GetSHA256OfFile(ParentPath);
  if CompareText(ActualSha256, ExpectedSha256) <> 0 then
  begin
    Result := 'The selected BKE architecture payload failed SHA-256 verification.';
    Exit;
  end;

  Log(Format('Selected BKE parent SHA-256 verified: %s.', [ActualSha256]));

  if not Exec(
    ParentPath,
    '/VERYSILENT /SUPPRESSMSGBOXES /NORESTART',
    '',
    SW_HIDE,
    ewWaitUntilTerminated,
    ResultCode) then
  begin
    Result := 'BKE could not start the selected architecture installer.';
    Exit;
  end;

  if ResultCode <> 0 then
  begin
    Result := Format(
      'The selected BKE architecture installer failed with exit code %d.',
      [ResultCode]);
    Exit;
  end;

  if not BkeLauncherExists then
  begin
    Result := 'BKE installation completed without the expected Launcher executable.';
    Exit;
  end;

  Log('BKE universal Windows installation completed successfully.');
end;
