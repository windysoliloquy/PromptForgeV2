# Prompt Forge V5 Generator Package

This folder contains the staged V5 unlock-generator launcher.

## Current Behavior

- `Generate-UnlockFile-V5.ps1`
  - issues `Temporary` trusted-user portable unlocks through `PromptForge.LicenseTool.exe`
  - issues `MachineBound` unlocks when you paste an activation request code from the target machine
  - includes the selected entitlement profile in the signed payload

## Intended future behavior

- keep issuing signed portable `Temporary` unlocks
- keep requiring a request code for `MachineBound` unlocks
- keep writing lane selections as signed `AllowedLanes`
- future expansion may add broader entitlement-profile-to-lane mapping beyond explicit `AllowedLanes`
