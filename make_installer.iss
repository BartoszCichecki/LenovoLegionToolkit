#include "InnoDependencies\install_dotnet.iss"

#define MyAppName "Lenovo Legion Toolkit"
#define MyAppNameCompact "LenovoLegionToolkit"
#define MyAppPublisher "Bartosz Cichecki"
#define MyAppURL "https://github.com/BartoszCichecki/LenovoLegionToolkit"
#define MyAppExeName "Lenovo Legion Toolkit.exe"

#ifndef MyAppVersion
  #define MyAppVersion "0.0.1"
#endif

[Setup]
UsedUserAreasWarning=false
AppId={{0C37B9AC-9C3D-4302-8ABB-125C7C7D83D5}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userpf}\{#MyAppNameCompact}
DisableProgramGroupPage=yes
LicenseFile=LICENSE
PrivilegesRequired=admin
OutputBaseFilename=LenovoLegionToolkitSetup
Compression=lzma2/ultra64  
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#MyAppExeName}
OutputDir=build_installer
ArchitecturesInstallIn64BitMode=x64

[Code]
function InitializeSetup: Boolean;
begin
  InstallDotNet6DesktopRuntime;
  Result := True;
end;

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl" 
Name: "czech"; MessagesFile: "compiler:Languages\Czech.isl" 
Name: "dutch"; MessagesFile: "compiler:Languages\Dutch.isl"
Name: "french"; MessagesFile: "compiler:Languages\French.isl"
Name: "german"; MessagesFile: "compiler:Languages\German.isl"
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"
Name: "portuguese"; MessagesFile: "compiler:Languages\Portuguese.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "slovak"; MessagesFile: "compiler:Languages\Slovak.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"
Name: "arabic"; MessagesFile: "InnoDependencies\Arabic.isl"
Name: "chinesesimplified"; MessagesFile: "InnoDependencies\ChineseSimplified.isl"
Name: "greek"; MessagesFile: "InnoDependencies\Greek.isl"
Name: "romanian"; MessagesFile: "InnoDependencies\Romanian.isl"
Name: "vietnamese"; MessagesFile: "InnoDependencies\Vietnamese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "build\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "LICENSE"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[InstallDelete]
Type: filesandordirs; Name: "{app}"

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: runascurrentuser nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\{#MyAppNameCompact}"

[UninstallRun]
RunOnceId: "DelAutorun"; Filename: "schtasks"; Parameters: "/Delete /TN ""LenovoLegionToolkit_Autorun_6efcc882-924c-4cbc-8fec-f45c25696f98"" /F"; Flags: runhidden 
