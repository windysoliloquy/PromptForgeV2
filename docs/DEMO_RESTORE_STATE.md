# Demo Restore State

## Purpose

This note records the exact runtime-state restore flow used to bring Prompt Forge's local demo/license/lane state back after a failed restore attempt.

Use this for local recovery only. This is not a product/runtime redesign note.

## What Owns The State

Prompt Forge local access state is split across three machine-local files/directories:

- `%LOCALAPPDATA%\PromptForgeDemo\license-state.json`
  - app unlock state
  - signed licenses
  - signed `AllowedLanes`
- `%LOCALAPPDATA%\PromptForgeDemo\demo-state.json`
  - remaining demo export count
- `%APPDATA%\PromptForge\lane-unlocks.json`
  - local durable premium-lane unlocks

These are runtime files, not source files.

## Why The Earlier Restore Failed

The restore target is outside the repo workspace.

In Codex, reading or writing:

- `C:\Users\windy\AppData\Local\PromptForgeDemo`
- `C:\Users\windy\AppData\Roaming\PromptForge`

may require escalation even when the repo backup under `artifacts\local-state-backups\...` is readable.

So a Codex instance can often inspect the backup folder successfully but still fail to restore the live state unless it is allowed to write AppData.

## Exact Restore Used

Backup restored:

- `C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\artifacts\local-state-backups\PromptForgeLocalState_2026-04-27_19-34-53`

Command used from repo root:

```powershell
powershell -ExecutionPolicy Bypass -File ".\tools\release\Manage-LocalRuntimeState.ps1" -Mode Restore -BackupPath "C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\artifacts\local-state-backups\PromptForgeLocalState_2026-04-27_19-34-53"
```

Observed restore result:

- restored `%LOCALAPPDATA%\PromptForgeDemo`
- restored `%APPDATA%\PromptForge`
- skipped missing UI-log backup entries
- completed successfully

## Safe Restore Procedure

1. Close Prompt Forge fully.
2. Restore from a manifest-backed backup with `tools\release\Manage-LocalRuntimeState.ps1 -Mode Restore -BackupPath <backup-folder>`.
3. Restart Prompt Forge.
4. Verify:
   - version button reads `Version Info`
   - unlock window shows unlocked license data
   - licensed premium lanes are no longer dim/locked

## Why The Helper Is Preferred

Use [tools/release/Manage-LocalRuntimeState.ps1](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\release\Manage-LocalRuntimeState.ps1) instead of manual file copying because it restores the full known runtime surface together:

- local demo/license directory
- roaming Prompt Forge app-data directory
- known UI event logs when present

It also requires a completed backup folder with `manifest.json`, which reduces the chance of restoring from an incomplete path.

## Important Behavior Notes

- App unlock state and premium-lane access are not the same thing.
- Restoring `license-state.json` can restore both unlock state and signed `AllowedLanes`.
- `lane-unlocks.json` may still be empty if lane access came from signed licenses rather than local preset-earned unlocks.
- Prompt Forge may continue showing stale state until restart because the live view model refresh path is in-memory until the app reloads.

## Failure Modes To Check First

- Prompt Forge was still open during restore.
- The wrong backup folder was used.
- The backup folder did not contain `manifest.json`.
- AppData restore required elevation and the command was run without it.
- Restore succeeded, but the app was not restarted.

## Related Files

- [tools/release/Manage-LocalRuntimeState.ps1](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\release\Manage-LocalRuntimeState.ps1)
- [docs/DEMO_LOCK_MEMO.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\docs\DEMO_LOCK_MEMO.md)
- [RELEASING.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\RELEASING.md)
