#ifndef SourceDir
  #define SourceDir "..\..\AppOutput\PromptForge"
#endif

#ifndef OutputDir
  #define OutputDir "..\..\artifacts\installer"
#endif

#ifndef SetupBaseName
  #define SetupBaseName "PromptForge-4.1.2-Setup"
#endif

#ifndef AppVersion
  #define AppVersion "4.1.2"
#endif

#ifndef AppExeName
  #define AppExeName "PromptForge.App.exe"
#endif

; Optional signing hook:
; 1. Install Inno Setup and signtool/certificate tooling.
; 2. Uncomment SignTool below and replace the placeholder command with your real signing command.
; SignTool=byparam powershell -ExecutionPolicy Bypass -File ".\Sign-File.ps1" $f

[Setup]
AppId={{D0A51F85-9574-4D85-9781-43D80F2749D8}
AppName=Prompt Forge
AppVersion={#AppVersion}
AppPublisher=Prompt Forge
DefaultDirName={localappdata}\Programs\Prompt Forge
DefaultGroupName=Prompt Forge
DisableProgramGroupPage=yes
SetupIconFile=..\..\PromptForge.App\Assets\PromptForge.ico
UninstallDisplayIcon={app}\{#AppExeName}
OutputDir={#OutputDir}
OutputBaseFilename={#SetupBaseName}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\Prompt Forge"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\Prompt Forge"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch Prompt Forge"; Flags: nowait postinstall skipifsilent
