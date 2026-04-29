# Demo Lock Memo

## Purpose

This memo records how the current demo lock, app unlock, signed license import, and lane-access gating work in the Prompt Forge workspace as of this snapshot.

This is a behavior note for release safety. It is not a redesign plan.

## Primary Sources

- `PromptForge.App/Services/DemoModeOptions.cs`
- `PromptForge.App/Services/DemoStateService.cs`
- `PromptForge.App/Services/LicenseService.cs`
- `PromptForge.App/Services/LaneUnlockStateService.cs`
- `PromptForge.App/Models/UnlockState.cs`
- `PromptForge.App/ViewModels/MainWindowViewModel.cs`
- `PromptForge.App/ViewModels/MainWindowViewModel.AccessGating.cs`
- `PromptForge.App/ViewModels/MainWindowViewModel.Presets.cs`
- `PromptForge.App/UnlockWindow.xaml.cs`

## Current Release Gates

Prompt Forge currently has three separate access concepts:

- demo/export access
- app/license unlock state
- lane access

These gates intentionally remain separate.

Important rule:

- App unlock removes demo/export-limit behavior.
- App unlock does not automatically unlock premium lanes.
- Premium lane access comes from either local lane unlock state or signed `AllowedLanes`.

## Demo Mode

`DemoModeOptions.IsDemoMode` is hard-coded to `true`.

`DemoModeOptions.MaxDemoCopies` is currently:

- `30`

The computed app demo state is:

- `IsDemoMode`
  - `DemoModeOptions.IsDemoMode && !IsUnlocked`
- `IsDemoExpired`
  - `IsDemoMode && RemainingDemoCopies <= 0`

Demo state persists at:

- `%LOCALAPPDATA%\PromptForgeDemo\demo-state.json`

The demo counter:

- starts at `30`
- is consumed only by prompt-copy/export behavior while `IsDemoMode` is true
- survives app restart
- is not stored in presets
- is not reset by normal app reset

`DemoStateService.NormalizeState(...)` clamps corrupt or older state to the current max. If a previous max was lower than the current max, remaining copies are topped up by the max increase.

## App Unlock / License State

App unlock state is owned by `LicenseService`.

License state persists at:

- `%LOCALAPPDATA%\PromptForgeDemo\license-state.json`

Current unlock state shape includes:

- `IsUnlocked`
- `ProductName`
- `PurchaserEmail`
- `LicenseId`
- `IssuedUtc`
- `LicenseMode`
- `MachineToken`
- `EntitlementProfile`
- `AllowedLanes`
- `ValidationToken`
- `SignedLicenses`

`IsUnlocked` comes from `LicenseService.IsUnlocked`.

When `IsUnlocked` is true:

- `IsDemoMode` becomes false
- demo export limits are bypassed
- the Version button label changes from `How to Unlock` to `Version Info`
- lane access is not automatically granted unless `AllowedLanes` contains that lane

## Unlock Import Behavior

The visible unlock/import entry point remains reachable after normal app unlock.

Current path:

- Main window Version button opens `UnlockWindow`
- `UnlockWindow` keeps `Import Unlock File` visible and enabled
- import calls `LicenseService.ImportUnlockFile(...)`
- success shows a `Prompt Forge` information popup
- failure shows a `Prompt Forge` warning popup

Current success wording:

- `Activation succeeded. Prompt Forge is now unlocked.`
- `Activation succeeded. Prompt Forge is now unlocked on this machine.`
- either may append:
  - `The original unlock file could not be removed automatically.`

Current unlock-window status wording after app unlock:

- `Version: App Unlock`
- `Prompt Forge is unlocked on this machine.`

The license summary still reports factual license data:

- `Unlocked for {PurchaserEmail}.`
- `License ID: {LicenseId}`
- `Issued: {IssuedUtc as local time}`
- `Mode: {LicenseMode}`
- `Entitlement: {EntitlementProfile}`

Do not use `Full` as generic app-unlocked wording. `Full` is only acceptable where it is the factual `LicenseMode` or `EntitlementProfile` value.

## Merge-Safe Signed Imports

Repeated signed imports are supported and are no longer treated as blind state replacement.

`LicenseService.ImportUnlockFile(...)` currently:

- parses the selected JSON file
- validates the signature through `PromptForgeLicenseCodec`
- validates machine binding for `MachineBound` licenses
- normalizes the incoming license
- merges the incoming signed license into current signed license state
- saves the merged state
- refreshes app state through the normal license refresh path

Merge behavior:

- valid signed licenses are stored in `SignedLicenses`
- duplicate signed licenses are deduplicated
- root `AllowedLanes` is derived from the union of valid signed license `AllowedLanes`
- `AllowedLanes` are distinct case-insensitively
- invalid imports leave current state unchanged
- save failure leaves current runtime state unchanged

`EntitlementProfile` is persisted and displayed, but it is not authoritative for lane access in current gating.

## Machine-Bound vs Temporary Licenses

`MachineBound` licenses:

- must include a machine token
- are accepted only when the signed token matches the current machine
- produce success wording that says Prompt Forge is unlocked on this machine

`Temporary` licenses:

- are still signature-validated
- do not require the current machine token
- are portable by design

Both modes may carry `AllowedLanes`.

## Lane Access Model

Lane locking is now base-allowlist based, not locked-map based.

The current effective lane access rule is:

- base/free lane
- OR local durable lane unlock in `lane-unlocks.json`
- OR signed license `AllowedLanes` contains the intent name

In code:

- `HasLaneAccess(string? intentMode)`
  - `!RequiresLaneUnlock(intentMode)`
  - OR `_laneUnlockStateService.IsUnlocked(intentMode)`
  - OR `_licenseService.HasAllowedLane(intentMode)`

The active locked-lane state is:

- `IsLockedLaneActive`
  - `RequiresLaneUnlock(IntentMode) && !HasLockedLaneAccess`

Compatibility aliases still exist:

- `IsVintageBendLocked` forwards to `IsLockedLaneActive`
- `ShowVintageBendAuthoringSections` forwards to `ShowLockedLaneAuthoringSections`
- `ShowVintageBendLockedPane` forwards to `ShowLockedLanePane`

## Base Free Lanes

These lanes are open by default:

- `Photography`
- `Photographic` compatibility alias
- `Cinematic`
- `Watercolor`
- `Pixel Art`
- `Graphic Design`

Every other normal lane requires lane access unless it is explicitly exempted as a special non-user lane.

Special non-premium exceptions:

- blank/null intent does not require lane unlock
- `Custom` does not require lane unlock
- `Experimental` does not require lane unlock through `IntentModeCatalog.IsExperimental(...)`

New normal lanes fail closed unless explicitly added to the base-free set.

## Premium Lane Behavior

Premium lanes without local unlock state and without signed `AllowedLanes` lock by default.

This includes premium lanes even when they do not have a legacy preset ritual mapping.

Important implication:

- the legacy preset-code map no longer defines which lanes are locked
- the map only defines the old saved-preset ritual for lanes that already had one
- unmapped premium lanes may be locked but not self-unlockable through the preset ritual yet

## Legacy Preset Ritual Unlocks

The legacy unlock preset map remains protected and unchanged:

- `Vintage Bend` -> `VB`
- `Product Photography` -> `Product`
- `Food Photography` -> `Food`
- `Lifestyle / Advertising Photography` -> `Lifestyle`

These values are credentials/tokens. Do not print them in user-facing status text.

Legacy lane unlock state persists at:

- `%APPDATA%\PromptForge\lane-unlocks.json`

Example shape:

```json
{
  "UnlockedIntentModes": [
    "Food Photography"
  ]
}
```

The lane unlock state:

- stores intent display names
- uses case-insensitive matching
- survives app restart
- is not stored in presets
- is not reset by normal app reset
- is not revoked if the original saved preset is deleted

Startup migration:

- `LaneUnlockStateService.MigrateFromLegacyPresetMarkers(...)` checks the default preset folder for legacy unlock preset names
- matching legacy presets migrate into `lane-unlocks.json`

## Preset Ritual Status Secrecy

The preset save path intentionally avoids exposing unlock tokens in visible status text.

Current locked-preset failure status:

- `Ready`

Current successful local lane unlock status:

- `{IntentMode} unlocked on this machine.`

This may show public lane names such as `Food Photography`, but must not print secret tokens such as `Food`, `Product`, `Lifestyle`, or `VB`.

Do not reintroduce the actual required preset name into status text, logs intended for normal users, or hover/console status output.

## Locked-Lane UI Behavior

When `IsLockedLaneActive` is true:

- `RegeneratePrompt()` blocks prompt generation
- `ApplyBlockedPromptPreviewState()` clears prompt outputs
- prompt preview is hidden
- negative prompt preview is hidden
- copy commands are disabled/blocked
- standard lane panels are hidden
- compact lane panels that honor `!IsLockedLaneActive` are hidden
- locked-lane pane is shown when demo is not expired

Current locked-pane copy:

- headline: `Locked.`
- body: empty string

There is no direct unlock form in the locked pane. Legacy local unlock acquisition remains indirect through the saved-preset ritual for mapped lanes, and signed lane access comes through imported license files with `AllowedLanes`.

## HoverDeck Lock Projection

HoverDeck is projection-only. It must not own lock state.

Current HoverDeck lock presentation:

- active-lane helper text uses `MainWindowViewModel.IsLockedLaneActive`
- per-item dimming in the intent dropdown uses the real access helper, not hardcoded lane names
- locked lanes remain visible in the dropdown
- lanes unlocked by signed `AllowedLanes` return to normal brightness
- lanes unlocked by local `lane-unlocks.json` return to normal brightness

The active locked helper text is:

- `Contact Windy Soliloquy to Unlock This Lane.`

## Runtime Effects

### Prompt Regeneration

`RegeneratePrompt()` blocks prompt generation when either of these is true:

- `IsDemoExpired`
- `IsLockedLaneActive`

When blocked, it calls `ApplyBlockedPromptPreviewState()`, which:

- clears `PromptPreview`
- clears `NegativePromptPreview`
- syncs standard lane panels
- refreshes artist blend summary state
- refreshes copy-command availability

### Preview Visibility

The current preview gates are:

- `ShowInteractivePromptPreview`
  - visible only when not in demo mode, not demo-expired, and not locked
- `ShowDemoPromptPreview`
  - visible only when in demo mode, not demo-expired, and not locked
- `ShowAuthoringWorkspace`
  - hidden when demo-expired
- `ShowDemoExpiredLockScreen`
  - shown when demo-expired
- `ShowLockedLanePane`
  - shown when locked lane is active and demo is not expired
- `ShowNegativePrompt`
  - shown only when negative prompt is enabled, demo is not expired, and lane is not locked

### Copy Behavior

Copy gating currently works like this:

- `CanCopyPrompt()`
  - requires not demo-expired
  - requires not locked
  - and, in demo mode, requires `RemainingDemoCopies > 0`
- `CanCopyNegativePrompt()`
  - requires not demo-expired
  - requires not locked

`CopyExportText(...)`:

- refuses prompt export when demo mode is active and no copies remain
- consumes a demo copy only in demo mode
- updates `RemainingDemoCopies` from `_demoStateService.TryConsumeCopy(...)`
- updates status text to reflect remaining copies or expiration

## State Mutation Owners

The live trigger path for demo, license, and lock state is still in `MainWindowViewModel`:

- `RemainingDemoCopies`
- `RefreshLicenseState()`
- `UpdateLockedLaneAccessState()`
- `CopyExportText(...)` for demo-copy consumption
- `SavePreset()` for legacy local lane unlock acquisition

The reporting/computed surface lives in `MainWindowViewModel.AccessGating.cs`.

## Notification and Refresh Coupling

These paths are timing-sensitive.

`RemainingDemoCopies`, `RefreshLicenseState()`, and `UpdateLockedLaneAccessState()` each raise batches of `OnPropertyChanged(...)` notifications that affect:

- demo-expired state
- locked-lane state
- preview visibility
- authoring visibility
- negative-prompt visibility
- standard-lane panel visibility
- compact-lane panel visibility
- copy-command availability
- Version button text
- HoverDeck projections that bind to the same view model

Two important side effects remain coupled to this trigger path:

- `RaiseCopyCommandCanExecuteChanged()`
- `RegeneratePrompt()`

## Safe Reading of the Current Design

The demo lock is not one single switch.

It is the combined effect of:

- hard-coded demo-mode enablement
- signed app/license state
- remaining demo copy count
- base-free lane allowlist
- local durable lane unlocks through `%APPDATA%\PromptForge\lane-unlocks.json`
- signed license `AllowedLanes`
- preview gating
- copy-command gating
- prompt-regeneration blocking

## Release-Clean Local State

For a locked demo copy with no local premium lanes open:

- remove or reset `%LOCALAPPDATA%\PromptForgeDemo\license-state.json`
- keep or reset `%LOCALAPPDATA%\PromptForgeDemo\demo-state.json` to the desired demo-copy count
- reset `%APPDATA%\PromptForge\lane-unlocks.json` to an empty `UnlockedIntentModes` list

The app must be restarted after runtime state files are changed externally.

Before any release-clean or first-run test that clears local demo, license, lane, preset, or UI-log state, use:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\release\Manage-LocalRuntimeState.ps1 -Mode Backup
```

That helper records a manifest-backed backup under `artifacts\local-state-backups\PromptForgeLocalState_<timestamp>` for the local runtime surfaces it knows about, including `%LOCALAPPDATA%\PromptForgeDemo`, `%APPDATA%\PromptForge`, and known UI event logs.

Only run `ClearForReleaseTest` or `Restore` with an explicit `-BackupPath` from a completed helper backup. Do not manually delete app-data state during release prep if the exact prior state must be restored later.

## Installer / Shipping Caution

Do not include local runtime unlock files in the installer.

Especially exclude:

- `%LOCALAPPDATA%\PromptForgeDemo\license-state.json`
- `%LOCALAPPDATA%\PromptForgeDemo\demo-state.json`
- `%APPDATA%\PromptForge\lane-unlocks.json`

Those files are per-user runtime state. A user's app should create or update them on that user's machine when they consume demo exports, import a signed unlock file, or earn a lane unlock.

## Caution

Any future code change around demo, license, or lane locking should treat these as behavior-sensitive seams:

- `DemoModeOptions.IsDemoMode`
- `DemoModeOptions.MaxDemoCopies`
- `IsUnlocked`
- `IsDemoMode`
- `IsDemoExpired`
- `IsLockedLaneActive`
- `HasLaneAccess(...)`
- `RequiresLaneUnlock(...)`
- `BaseUnlockedLaneNames`
- `LockedLaneUnlockPresetNames`
- `LaneUnlockStateService`
- `LicenseService.HasAllowedLane(...)`
- `LicenseService.ImportUnlockFile(...)`
- `LicenseService.MergeUnlockStates(...)`
- `RemainingDemoCopies`
- `RefreshLicenseState()`
- `UpdateLockedLaneAccessState()`
- `RegeneratePrompt()`
- `CopyExportText(...)`
- `SavePreset()`

Small ordering changes here can alter:

- whether previews are visible
- whether prompts are cleared
- whether copy buttons enable or disable correctly
- whether demo expiration and locked-lane states surface at the right time
- whether premium lanes fail closed
- whether signed `AllowedLanes` are honored
- whether local lane unlocks survive
- whether secret preset ritual tokens leak into visible status text
