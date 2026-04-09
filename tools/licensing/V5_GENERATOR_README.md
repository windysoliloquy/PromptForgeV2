# Prompt Forge V5 Generator Package

This folder contains the staged V5 unlock-generator launcher.

## Current Behavior

- `Generate-UnlockFile-V5.ps1`
  - issues `Temporary` trusted-user portable unlocks through `PromptForge.LicenseTool.exe`
  - issues `MachineBound` unlocks when you paste an activation request code from the target machine
  - includes the selected entitlement profile in the signed payload

## Intended future behavior

- `Temporary` will issue a signed portable trusted-user unlock
- `MachineBound` will require a request code and issue a signed machine-locked unlock
- entitlement profile selection will map to signed lane access
