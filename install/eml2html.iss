; eml2html Inno Setup Script
; Builds a user-friendly installer for eml2html

#define MyAppName "eml2html"
#define MyAppVersion GetEnv('EML2HTML_VERSION')
#if MyAppVersion == ""
  #define MyAppVersion "1.0.0"
#endif
#define MyAppPublisher "Blackline IT"
#define MyAppURL "https://github.com/blacklineit/eml2html"
#define MyAppExeName "eml2html.exe"

[Setup]
AppId={{5CC6A755-C8DC-4301-B092-0EA65B1A90B8}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}/issues
AppUpdatesURL={#MyAppURL}/releases
DefaultDirName={localappdata}\{#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=..\LICENSE
OutputDir=Output
OutputBaseFilename=eml2html-{#MyAppVersion}-setup
SetupIconFile=eml2html.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Registry]
; Shell extension flyout menu
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.eml\shell\eml2html"; ValueType: string; ValueName: "MUIVerb"; ValueData: "eml2html"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.eml\shell\eml2html"; ValueType: string; ValueName: "SubCommands"; ValueData: ""; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.eml\shell\eml2html"; ValueType: string; ValueName: "Icon"; ValueData: "{app}\{#MyAppExeName}"; Flags: uninsdeletekey

; Extract to HTML
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.eml\shell\eml2html\shell\html"; ValueType: string; ValueName: ""; ValueData: "Extract to HTML"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.eml\shell\eml2html\shell\html\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" --mode html ""%1"""; Flags: uninsdeletekey

; Extract to Folder
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.eml\shell\eml2html\shell\folder"; ValueType: string; ValueName: ""; ValueData: "Extract to Folder"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\Classes\SystemFileAssociations\.eml\shell\eml2html\shell\folder\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" --mode folder ""%1"""; Flags: uninsdeletekey
