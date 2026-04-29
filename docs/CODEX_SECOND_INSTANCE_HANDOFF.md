# Prompt Forge Second Codex Handoff

This file is for a second Codex instance working in this repository.

Primary goal: help without disrupting the active local workflow, the release/signing setup, or the Prompt Forge licensing system.

## Mental Model

Prompt Forge is normal in architecture, unusual in release discipline.

Normal architecture:

- `.NET 8`
- WPF desktop app
- one core library
- one small license-generation tool
- one diagnostics utility

Unusual operational constraints:

- signed Windows release flow
- strict separation between source branch and public release branch
- stable license keypair that should not be rotated casually
- demo/unlock behavior partly driven by machine-local state

Approach the code as a standard desktop app, but approach releases/licensing as sensitive operational infrastructure.

## First Rules

1. Start by running `git status --short`.
2. Assume any existing uncommitted change belongs to the human or another Codex unless the task clearly says otherwise.
3. Do not revert, rewrite, reformat broadly, or "clean up" unrelated files.
4. Make the smallest change that solves the assigned task.
5. If a task touches licensing, signing, release, or demo-lock behavior, read this whole file first.

## Current Repo Roles

- Source/dev branch: `local/restore-source`
- Public release branch: `main`

Do not publish source code from `local/restore-source`.
Do not merge `local/restore-source` into `main`.
Do not create release tags from the source branch.

Primary release flow is documented in [RELEASING.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\RELEASING.md).

## Repo Shape

Primary projects:

- [PromptForge.App/PromptForge.App.csproj](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\PromptForge.App.csproj)
- [PromptForge.Core/PromptForge.Core.csproj](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\PromptForge.Core.csproj)
- [PromptForge.LicenseTool/PromptForge.LicenseTool.csproj](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.LicenseTool\PromptForge.LicenseTool.csproj)
- [PromptForge.Diagnostics/PromptForge.Diagnostics.csproj](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Diagnostics\PromptForge.Diagnostics.csproj)

Useful architectural notes:

- `PromptForge.App` is a WPF `net8.0-windows` app.
- Release publishes are enforced as `win-x64`, self-contained, and not single-file.
- `PromptForge.Core` embeds important data assets, including the public license key and prompt-related data files.
- `PromptForge.App` copies local run output to `AppOutput\PromptForge`, which is convenient but is not the same thing as the signed release output.

## Source Boundary

- Workspace files are the immediate authority available to Codex in this repo.
- Broader project-container guidance may exist outside the repo.
- If a rule is already specified in the approved handoff/update task, implement it directly rather than pretending a missing repo file was consulted.
- Do not fabricate source provenance.

## Structural Stabilization Baseline

Structural decomposition is currently paused.

Treat these docs as the current structural baseline:

- [docs/Prompt Forge — Pressure-Zone Decomposition Plan.txt](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\docs\Prompt Forge — Pressure-Zone Decomposition Plan.txt)
- [docs/Prompt Forge — Target Structure From Current State.txt](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\docs\Prompt Forge — Target Structure From Current State.txt)
- [docs/Prompt Forge — Structural Stabilization Checkpoint.txt](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\docs\Prompt Forge — Structural Stabilization Checkpoint.txt)

Operating rules for future Codex work:

- do not reopen structural extraction planning unless there is:
  - a material code change
  - a newly emerged pressure zone
  - or fresh evidence that a current boundary is failing
- preserve the contained partial boundaries already established
- treat these files as intentional central ownership for now:
  - `MainWindowViewModel.cs`
  - `PromptBuilderService.cs`
  - `SliderLanguageCatalog.cs`
- prefer feature implementation, bug fixing, and product work over structural cleanup
- if a requested change risks crossing one of the stabilized boundaries, implement the smallest safe change first and note the pressure point rather than expanding scope casually

When responding to feature or bug tasks:

1. identify the smallest safe implementation surface
2. avoid opportunistic refactors
3. preserve current call-order ownership unless behavior change is explicitly requested
4. only flag a structural concern if it is directly blocking the requested work

## Active Caution Areas

At the time this handoff was written, local uncommitted changes existed in:

- [PromptForge.App/Services/LicenseService.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Services\LicenseService.cs)
- [PromptForge.Core/Services/PromptForgeLicenseCodec.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\PromptForgeLicenseCodec.cs)
- [PromptForge.LicenseTool/Program.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.LicenseTool\Program.cs)

There were also untracked local folders:

- `.codex-temp/`
- `.dotnet-cli/`
- `.dotnet-home/`
- `quarantine/`

Before editing anything, re-check the working tree because this list may be stale.

## Folder Guidance

Treat these folders differently:

- `PromptForge.App/`, `PromptForge.Core/`, `PromptForge.LicenseTool/`, `PromptForge.Diagnostics/`
  These are source-bearing folders. Edit only what the task needs.
- `tools/trusted-signing/`, `tools/installer/`, `tools/release/`
  These are operational/release-critical. Do not change casually.
- `artifacts/`
  Generated publish and installer outputs live here. Inspect as needed, but do not confuse generated output with source.
- `AppOutput/`
  Local convenience build output. Useful for quick local runs, but not the canonical release artifact path.
- `quarantine/`
  Large analysis/output area. Avoid broad searches here unless the task specifically involves it.
- `.codex-temp/`, `.dotnet-cli/`, `.dotnet-home/`
  Local operational folders. Usually not source-of-truth and usually not something to edit.
  For Codex-only restore experiments, prefer explicit repo-local NuGet/config/cache overrides here, and use absolute paths rather than relative `NUGET_PACKAGES` values.
  If `PromptForge.App` fails copying `PromptForge.Core.dll` into `bin\Debug\net8.0-windows`, first confirm the app itself is not still running.

## Signing System

The signing wrapper lives here:

- [tools/trusted-signing/Sign-Artifact.ps1](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\trusted-signing\Sign-Artifact.ps1)

Release build/sign orchestration lives here:

- [tools/installer/Build-SignedRelease.ps1](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\installer\Build-SignedRelease.ps1)
- [tools/release/Prepare-PublicRelease.ps1](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\release\Prepare-PublicRelease.ps1)

### Important signing paths

- Trusted Signing metadata expected at:
  `tools\trusted-signing\metadata.json`
- Metadata template:
  `tools\trusted-signing\metadata.template.json`
- Local SignTool target path used by the script:
  `tools\trusted-signing\packages\Microsoft.Windows.SDK.BuildTools\bin\10.0.26100.0\x64\signtool.exe`
- Local Azure signing dlib target path used by the script:
  `tools\trusted-signing\packages\Microsoft.ArtifactSigning.Client\bin\x64\Azure.CodeSigning.Dlib.dll`
- NuGet cache fallback source used by the script:
  `%USERPROFILE%\.nuget\packages\microsoft.windows.sdk.buildtools\10.0.26100.7705\bin\10.0.26100.0\x64`
  `%USERPROFILE%\.nuget\packages\microsoft.artifactsigning.client\1.0.115\bin\x64`

### Signing prerequisites

- Azure CLI must be installed.
- Azure CLI must already be logged in with the Trusted Signing identity.
- `metadata.json` must already exist and be valid.

### Do not do these things

- Do not rotate signing settings.
- Do not edit `metadata.json` unless explicitly asked by the human.
- Do not replace package versions in the signing script "just to modernize them."
- Do not invent a second signing path or alternate signing flow unless the task specifically requires it.

## Publish and Installer Paths

Self-contained publish output is expected here:

- `artifacts\publish\PromptForge-win-x64`

The runnable publish exe should be:

- `artifacts\publish\PromptForge-win-x64\PromptForge.App.exe`

Installer output is expected here:

- `artifacts\installer`

The app build notes also live here:

- [WHERE_TO_FIND_THE_APP.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\WHERE_TO_FIND_THE_APP.md)

## Version Baseline

Treat Prompt Forge version references as intentionally split across older public/docs surfaces and newer local build/release surfaces.

Current known anchors in this workspace:

- Local app/build/installer metadata should now target `5.1.2`, including [PromptForge.App/PromptForge.App.csproj](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\PromptForge.App.csproj), [tools/installer/PromptForge.iss](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\installer\PromptForge.iss), and example commands in [RELEASING.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\RELEASING.md).
- Public-facing [RELEASE_NOTES.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\RELEASE_NOTES.md) should present the release as `Prompt Forge 5.1`.
- For local planning and Codex-to-Codex coordination, treat the active working line as `Version 5.1`.

Operational rule for future Codex work:

- If a task says `Version 5.1.2`, interpret that as the current local target and keep the main release/version surfaces aligned to `5.1.2` / `Prompt Forge 5.1`.
- Do not silently "normalize" every version string during unrelated work.
- When the real version-bump pass happens, update app metadata, installer metadata, release examples, and public-facing notes together as one coordinated release/versioning task rather than as scattered incidental edits.

## Licensing System

License generation tool:

- [PromptForge.LicenseTool/Program.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.LicenseTool\Program.cs)

App-side license import/state logic:

- [PromptForge.App/Services/LicenseService.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Services\LicenseService.cs)
- [PromptForge.Core/Services/PromptForgeLicenseCodec.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\PromptForgeLicenseCodec.cs)

### Key paths

Public key file embedded in repo:

- [PromptForge.Core/Data/promptforge-license-public.pem](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Data\promptforge-license-public.pem)

Private key default path:

- `tools\licensing\private\license-private-key.pem`

Private key environment variable override:

- `PROMPTFORGE_LICENSE_PRIVATE_KEY_PATH`

Public key environment variable override:

- `PROMPTFORGE_LICENSE_PUBLIC_KEY_PATH`

### Critical rule about keys

Do not generate a new keypair unless the human explicitly asks for a licensing reset/rotation.

Why:

- A new private key changes unlock-file signatures.
- A new public key requires rebuilding the app.
- Existing unlock files can stop validating if the keypair changes.
- This is one of the easiest ways to disrupt the current workflow.

### Safe licensing behavior

- Read the public key file if needed.
- Assume the private key is sensitive and local-only.
- Never print private key contents into logs, docs, commits, or chat.
- Never commit private key material.
- Never move the private key.
- Never replace the public key to "match" a newly generated private key unless explicitly instructed.

## Demo Lock and Local State

Demo-mode settings live here:

- [PromptForge.App/Services/DemoModeOptions.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Services\DemoModeOptions.cs)
- [PromptForge.App/Services/DemoStateService.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Services\DemoStateService.cs)

Current demo behavior in code:

- `IsDemoMode = true`
- `MaxDemoCopies = 30`
- License/demo state directory name resolves to `PromptForgeDemo`

### Local app-data paths

Demo counter state:

- `%LOCALAPPDATA%\PromptForgeDemo\demo-state.json`

License unlock state:

- `%LOCALAPPDATA%\PromptForgeDemo\license-state.json`

These are machine-local runtime files, not source assets.

### Rules for demo-lock handling

- Do not delete, reset, or edit local app-data demo/license state unless the human explicitly asks for that exact action.
- Do not "helpfully" consume demo copies during testing if the task does not require it.
- Prefer code inspection over state mutation.
- If you must test unlock import, call out exactly what local state you changed.
- Before any release-clean or first-run test that clears local runtime state, run [tools/release/Manage-LocalRuntimeState.ps1](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\release\Manage-LocalRuntimeState.ps1) with `-Mode Backup`.
- Do not clear `%LOCALAPPDATA%\PromptForgeDemo`, `%APPDATA%\PromptForge`, or known Prompt Forge UI event logs manually during release work. Use the helper's `ClearForReleaseTest` and `Restore` modes with an explicit `-BackupPath` so the manifest controls what is removed and restored.

Current local-state safety checkpoint:

- Backup helper created and verified a local runtime-state backup at `C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\artifacts\local-state-backups\PromptForgeLocalState_2026-04-22_15-08-45`.
- Manifest path: `C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\artifacts\local-state-backups\PromptForgeLocalState_2026-04-22_15-08-45\manifest.json`.
- Backed-up sources included `%LOCALAPPDATA%\PromptForgeDemo`, `%APPDATA%\PromptForge`, repo `ui-event-log.shared.txt`, and `AppOutput\PromptForge\ui-event-log.txt`.

## Anti-Churn Rules

- Do not rename files unless required.
- Do not reformat whole files for style consistency.
- Do not rewrite stable architecture just because you see a cleaner pattern.
- Do not touch release scripts while doing unrelated feature work.
- Do not touch licensing code while doing unrelated UI/data work.
- Do not change branch flow, installer flow, or signing flow without explicit approval.

## Prompt Forge Product-Safety Rules

Prompt Forge is not a generic prompt builder. Wording, defaults, lane classification, prompt assembly, and compression behavior are part of the product.

Do not make local judgment calls about:

- lane classification
- ordinary/shared-path vs special/explicit status
- whether `DefaultLanePolicy` is sufficient
- semantic direction
- premium-feel wording
- UI/UX taste
- selector display labels
- naming
- compression nuance
- fallback defaults that affect tone or feel

If any of those are unsettled, stop and hand the decision back to the reasoner instead of improvising.

Internal ids may be machine-shaped. Visible UI labels must remain human-readable. Do not silently substitute or "clean up" labels without instruction.

## Prompt Forge Ownership Boundaries

These are hard boundaries:

- `PromptBuilderService` owns orchestration and prompt assembly order.
- `SliderLanguageCatalog` owns phrase authority, semantic phrasing, and phrase-level guardrails.
- registry / lane metadata owns stable lane shape only.
- policy hooks isolate real exceptions only.

Do not move phrase logic into metadata.
Do not widen metadata into a phrase DSL or compression DSL.
Do not change prompt assembly order casually.

## Lane Classification And Policy Threshold

Keep these categories distinct:

- ordinary lane
- special lane
- non-default-policy lane

Special lane and non-default-policy lane are not identical categories.

Current practical classification:

- clearly non-default-policy: `Vintage Bend`, `Comic Book`
- explicit/special for safety: `Experimental`
- most other lanes are ordinary/default-policy candidates unless proven otherwise

The shared standard-lane panel path should only be used when the lane has already been judged ordinary/shared-path under current rules.

## Add-A-Pack Method

New semantic-pack work should follow this flow:

1. classify first
2. registry shape first
3. subtype structure only if justified
4. modifier structure only if justified
5. defaults / caps / weight groups
6. prefer `DefaultLanePolicy` unless a real exception exists
7. keep phrase generation in code
8. keep assembly in code
9. require validation, regression review, and manual taste review before calling the pack done

## Semantic Authoring Discipline

- lane-root words are expensive limited-use anchors, not default adjectives
- use one strong root occurrence by default
- allow a second root occurrence only when it is structurally justified and clearly doing different work
- do not use the lane root as a general-purpose adjective across slider phrases
- if compression removes a repeated root word, the remaining phrase must still sound strong
- if root removal leaves weak mush, rewrite or remove the phrase
- a descriptor fragment should appear once by default across the pack
- a second use is an exception, not normal authoring
- do not reuse descriptor fragments casually across sliders, modifiers, or subtypes
- phrase banks must be reviewed as real comma-separated prompt output, not just in isolation
- a phrase that sounds clever alone but becomes muddy in stack context is a failed phrase
- compression is cleanup/fallback, not the first intelligence
- prompts should read cleanly before compression
- do not rely on compression to rescue weak authoring
- require pre-compression and post-compression review for semantic-pack changes
- require manual taste review for semantic/lane wording work
- keep distinct slider ownership clear: framing should sound like framing, camera angle like angle/vantage, focus depth like focus distribution/isolation, lighting intensity like light strength, atmospheric depth like spatial recession/depth layering, and detail density like detail load rather than symbolism or lighting

## Prompt Forge Architecture Fragile Points

1. `PromptBuilderService` assembly order
   Changing order changes product semantics, not just formatting. Do not change it casually.
2. `SliderLanguageCatalog`
   Phrase economy, lane-native wording, root restraint, and stack coherence live here. Do not do broad semantic rewrites unless explicitly assigned.
3. lane registry / metadata
   Stable lane shape only. Do not move phrase logic or compression behavior into metadata.
4. policy hooks
   Use `DefaultLanePolicy` unless a real exception is proven. Do not create a custom policy just because a lane feels important.
5. shared-lane vs explicit-lane UI path
   Do not force a lane onto the shared standard-lane path just because the path exists. Shared-path eligibility must already be decided before using that path, and Codex must not infer it on its own. Keep structurally unusual lanes explicit.
6. mirrored config/state surfaces
   New lane/config fields are not done unless clone/copy behavior and any mirrored config surfaces still kept in source preserve them.
7. `MainWindowViewModel` state flow
   `CaptureConfiguration`, `ApplyConfiguration`, intent switching, `Reset()`, and preset round-trip surfaces are linked failure points. Treat changes there as round-trip work, not one-direction work.
8. XAML visibility/binding contracts
   Do not casually break existing `Show...` / `Is...` visibility patterns.
9. prompt-preview behavior
   Semantic or config work is unfinished if prompt preview no longer reflects state immediately and correctly.
10. compression interaction
   Do not assume cleanup will rescue weak authoring.
11. Experimental path
   Treat it as explicitly special. Do not normalize it into ordinary lane behavior casually.
12. premium-feel labels/defaults
   Do not silently substitute user-facing labels or taste-sensitive defaults.

## Incomplete Specs Or Ambiguity

If a lane/spec omits classification, defaults, selector labels, modifier phrasing, or other taste-sensitive decisions, do not invent them.

- report the missing items as risks instead of auto-filling them
- if the task becomes a design/taste/policy question, hand it back to the reasoner

## Task-Type Verification Gates

If UI/XAML changed:

- verify bindings
- verify visibility state
- verify no broken layout contract

If config/state changed:

- verify capture/apply/reset/default paths
- verify clone/copy/preset round-trip surfaces as relevant

If semantic/lane logic changed:

- inspect real prompt output
- check repeated anchor sludge
- check subtype differentiation
- check modifier-cap behavior
- check compression interaction
- require manual taste review

If licensing/release changed:

- remain narrow
- do not mutate local machine state unless explicitly required
- document any local state touched

## Four-Layer Lane Requirement

Most lane additions require all four:

- intent/lane registration
- viewmodel state + visibility
- UI rendering/panel path
- prompt-language routing

Most lane work must preserve all of these together:

- registration
- config/state
- visibility/rendering
- prompt-language routing
- reset/default behavior
- validation/manual taste review

Do not call lane work done when only one or two layers are wired.

## Definition Of Done

A Prompt Forge change is not done because it compiles.

Semantic-pack and lane work must preserve:

- diagnostics / validation as applicable
- reset/default behavior
- prompt-preview behavior
- stack readability
- compression review
- manual taste review where wording quality is affected

## Git And Workflow Prohibitions

Do not do any of the following unless the human explicitly asks:

- commit
- push
- tag
- merge
- rebase
- stash
- cherry-pick
- switch branches

## Known Environment Friction

These are practical issues observed on this machine/workflow. Plan around them.

### Windows-specific friction

- This is a Windows + PowerShell workspace, so path handling matters more than on Unix-like repos.
- Prefer backslash-aware reasoning when reading scripts, but use exact existing file style when editing code/docs.
- Be careful with quoted paths containing spaces, especially under `C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge`.
- Do not assume bash utilities or bash path semantics.
- Native Windows tools and PowerShell scripts are part of the intended workflow here, especially for release work.
- Respect the repo `global.json` and do not "helpfully" test or retarget this repo to `.NET 10`; if SDK selection disagrees with the pinned `.NET 8` baseline, report environment drift instead of improvising.

### Patch-size friction

- Large patches are higher-risk here than a series of smaller targeted patches.
- Avoid giant apply-patch edits against long files unless absolutely necessary.
- Prefer several focused edits over one huge rewrite.
- Before patching a long file, read only the relevant section and patch that section narrowly.
- If a file is already locally modified, large patches increase the chance of stomping active work.

### Output-size friction

- Repository searches can produce extremely large output, especially across `quarantine/`, generated data, or large JSON/CSV assets.
- Always narrow searches by folder, pattern, or file type before running broad `rg` commands.
- Avoid loading huge data files unless the task truly depends on them.

### Local-state friction

- Some behavior depends on machine-local files under `%LOCALAPPDATA%`, not just repo files.
- The demo counter and unlock state are local runtime state, so code inspection is safer than mutating those files.
- Release/signing also depends on machine-local tooling state such as Azure CLI login and local package caches.

### File-lock and build friction

- Release/build steps can fail if Prompt Forge is currently running and holding files open.
- Do not assume a failed copy/build means the script is wrong; check for file locks first.
- If a build or installer step fails, distinguish between code failure, credential failure, missing local tooling, and locked-file failure.
- Do not treat the unresolved `PromptForge.Diagnostics` / solution graph issue as evidence that Prompt Forge is generally unbuildable; for now, treat Diagnostics as a known special-case verification surface and do not reopen settled SDK-drift, roaming-NuGet, or app-lock investigations unless new evidence justifies it.

### Collaboration friction

- This repo may have active uncommitted work from the human or another Codex at the same time.
- Re-check `git status --short` before and after edits.
- Favor additive notes and small diffs over cleanup passes.

## Safe Working Style

Preferred workflow:

1. Inspect only the files needed for the task.
2. Patch the smallest surface area possible.
3. If the task is risky, stop at a focused diff instead of broad refactors.
4. Re-run `git status --short` before finishing.
5. Summarize exactly which files changed and why.
6. If a conclusion came from the human's task brief rather than a repo-local authority file, say so plainly.
7. Do not pad notes with fake provenance.

## Safe Commands

Prefer these command shapes:

- Check worktree:
  `git status --short`
- Narrow search:
  `rg -n "pattern" PromptForge.App PromptForge.Core PromptForge.LicenseTool PromptForge.Diagnostics docs tools`
- Build app project:
  `dotnet build PromptForge.App\\PromptForge.App.csproj`
- Build solution:
  `dotnet build PromptForge.sln`
- Run license tool help:
  `dotnet run --project PromptForge.LicenseTool -- --help`

Use extra caution with these:

- `powershell -ExecutionPolicy Bypass -File .\tools\installer\Build-SignedRelease.ps1`
- `powershell -ExecutionPolicy Bypass -File .\tools\release\Prepare-PublicRelease.ps1 ...`

Never use broad destructive cleanup commands unless the human explicitly asks.

## Do Not Touch Unless Asked

- `tools\trusted-signing\metadata.json`
- the private license key file
- release branch strategy
- signing package versions
- demo/unlock app-data files under `%LOCALAPPDATA%`
- release artifacts on `main` unless the task is specifically about publishing

## Codex-To-Codex Turn Log

Use this same file as the baton between Codex instances.

Rules:

- Append only.
- Do not rewrite or "clean up" earlier entries except to fix your own immediately previous typo.
- Keep each turn compact and factual.
- Log every project edit in this handoff file in chronological order; do not skip a turn because the change felt small.
- If you changed files, name them explicitly.
- If you need something from the next Codex, ask for it in the `Needs next` line.
- If you need something from the human, say so in the `Needs human` line.

### Required handoff line

At the end of your session, leave exactly one compact summary line in this format:

`HANDOFF: status=<done|partial|blocked> task="<short task name>" files="<comma-separated paths or none>" blockers="<none or short blocker>" next="<single next action>"`

Example:

`HANDOFF: status=partial task="license import validation" files="PromptForge.App/Services/LicenseService.cs, PromptForge.Core/Services/PromptForgeLicenseCodec.cs" blockers="need human decision on unlock-file retention behavior" next="confirm whether imported unlock json should always be destroyed after successful activation"`

### Turn entry template

Copy this block and append a new entry at the bottom when you finish a working turn:

```text
### Turn
Date: YYYY-MM-DD HH:MM local
Agent: Codex-Primary or Codex-Secondary
Task: <what you worked on>
Status: done | partial | blocked
Files changed: <path list or none>
Tests run: <commands run or none>
Observed risks: <short note or none>
Needs next: <specific request for the next Codex or none>
Needs human: <specific question/decision or none>
Notes: <brief factual summary>
HANDOFF: status=<done|partial|blocked> task="<short task name>" files="<comma-separated paths or none>" blockers="<none or short blocker>" next="<single next action>"
```

### Reading order

When resuming:

1. Read the latest `HANDOFF:` line first.
2. Then read the most recent `### Turn` block.
3. Then run `git status --short`.
4. Only after that should you inspect or edit code.

## If You Need To Build Or Test

- Prefer targeted validation over full-system churn.
- If testing release/signing, do not improvise around missing credentials or metadata.
- If Azure login, signing metadata, or private keys are missing, stop and report that clearly.
- Do not create placeholder secrets just to make the scripts pass farther.

## If You Are Asked To Generate Unlock Files

Use the existing licensing tool and existing keypair only.

Typical command shape:

```powershell
dotnet run --project PromptForge.LicenseTool -- --email buyer@example.com
```

Do not run:

```powershell
dotnet run --project PromptForge.LicenseTool -- --generate-keypair
```

unless the human explicitly asks for a key rotation.

## If You Are Asked To Prepare A Release

Read these first:

- [RELEASING.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\RELEASING.md)
- [tools/installer/Build-SignedRelease.ps1](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\installer\Build-SignedRelease.ps1)
- [tools/release/Prepare-PublicRelease.ps1](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\release\Prepare-PublicRelease.ps1)

Do not shortcut the source-branch/public-branch separation.

## Default Posture

When in doubt:

- preserve the existing keypair
- preserve the existing signing flow
- preserve machine-local demo/unlock state
- preserve unrelated edits
- prefer a narrow fix over a clever rewrite

### Turn
Date: 2026-04-05 20:44 local
Agent: Codex-Primary
Task: Add Prompt Forge product-safety, lane-boundary, and verification rules to handoff
Status: done
Files changed: docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; markdown inspection of updated handoff
Observed risks: Several authority docs named in ChatGPT were not present verbatim in this Codex workspace, so the wording was anchored to the existing handoff, `docs/lane-scale-roadmap.md`, and live code boundaries.
Needs next: none
Needs human: none
Notes: Added hard product-safety rules, ownership boundaries, lane/policy thresholds, semantic-authoring discipline, fragile architecture points, task-type verification gates, four-layer lane reminder, stronger git prohibitions, ambiguity stop rules, and definition-of-done corrections without removing the existing operational safeguards.
HANDOFF: status=done task="handoff product-safety rules" files="docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="read latest handoff line before any future edit"

### Turn
Date: 2026-04-05 20:49 local
Agent: Codex-Primary
Task: Tighten handoff semantic-authoring and source-boundary rules
Status: done
Files changed: docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; markdown inspection of updated handoff; reread `docs/lane-scale-roadmap.md`
Observed risks: Broader project-container notes may exist outside this workspace, so this pass explicitly avoids claiming missing repo files were reviewed.
Needs next: none
Needs human: none
Notes: Added a source-boundary section, strengthened semantic-authoring rules around root restraint, post-removal strength, descriptor reuse, stack review, compression posture, and distinct slider ownership, tightened fragile-point rules for round-trip state and preview behavior, and added a no-fake-sourcing note in working style.
HANDOFF: status=done task="handoff v2 semantic-source tightening" files="docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="use workspace-local provenance honestly in future handoff updates"

### Turn
Date: 2026-04-05 23:07 local
Agent: Codex-Secondary
Task: Tighten workspace companion intro and duplicate-file warning; make handoff logging requirement explicit
Status: done
Files changed: docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; markdown inspection of both doc edits
Observed risks: none
Needs next: continue logging every edit in this handoff file in chronological order
Needs human: none
Notes: Added a blunt companion-vs-handoff hierarchy sentence near the top of the workspace context note, strengthened the duplicate-file warning into an explicit pre-edit rule, and added a visible handoff rule requiring every project edit to be logged here in order.
HANDOFF: status=done task="tighten workspace-context intro and logging reminder" files="docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="keep logging every project edit in this handoff file in order"

### Turn
Date: 2026-04-05 23:46 local
Agent: Codex-Secondary
Task: Pin repo to .NET 8.0.419 and verify under the pin
Status: partial
Files changed: global.json, docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; `dotnet --version`; `dotnet build PromptForge.sln -v minimal`; `dotnet build PromptForge.App/PromptForge.App.csproj -v minimal`; `dotnet build PromptForge.Core/PromptForge.Core.csproj -v minimal`; `dotnet build PromptForge.LicenseTool/PromptForge.LicenseTool.csproj -v minimal`; `dotnet build PromptForge.Diagnostics/PromptForge.Diagnostics.csproj -v minimal`
Observed risks: SDK pin is active and selects `.NET 8.0.419`, but build verification is blocked by unauthorized access to `C:\Users\windy\AppData\Roaming\NuGet\NuGet.Config`; `PromptForge.Diagnostics` still fails without surfacing a compile error even after first-time CLI flags were disabled.
Needs next: if build verification is needed beyond this point, resolve or bypass the machine-local NuGet config permission issue without retargeting the repo
Needs human: none
Notes: Added a strict root `global.json` pin for `.NET 8.0.419`, updated the workspace context note to treat `.NET 8` as the intended baseline and `.NET 10` as drift, and added a small handoff rule telling future Codex runs to respect the repo pin rather than improvising around newer installed SDKs.
HANDOFF: status=partial task="pin repo to .NET 8.0.419" files="global.json, docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="build verification blocked by denied access to C:\\Users\\windy\\AppData\\Roaming\\NuGet\\NuGet.Config" next="treat .NET 8.0.419 as baseline and resolve NuGet permission drift before deeper build verification"

### Turn
Date: 2026-04-06 00:00 local
Agent: Codex-Secondary
Task: Diagnose NuGet config permission failure blocking .NET 8 build verification
Status: done
Files changed: docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; `dotnet --version`; `dotnet --info`; `dotnet --list-sdks`; environment-variable inspection for `DOTNET*` / `NUGET*` / `HOME` / `USERPROFILE`; `dotnet nuget locals all --list`; read-only inspection of `C:\Users\windy\AppData\Roaming\NuGet\NuGet.Config`; read-only inspection of `C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.FallbackLocation.config`; read-only inspection of `C:\Program Files (x86)\NuGet\Config\Microsoft.VisualStudio.Offline.config`; `dotnet restore PromptForge.Core/PromptForge.Core.csproj -v diag`; `dotnet build PromptForge.Core/PromptForge.Core.csproj -v diag`
Observed risks: The first real failure is user-wide NuGet config access under the non-escalated sandbox, not SDK drift; elevated reads show the roaming `NuGet.Config` is valid XML with normal-looking ACLs, so the denial appears to be execution-context/sandbox-specific rather than a malformed config file.
Needs next: if full build verification is required, choose the lowest-risk path between fixing access to `C:\Users\windy\AppData\Roaming\NuGet\NuGet.Config`, giving Codex a safe alternate user-level NuGet config path, or documenting that full restore cannot run in the default sandbox
Needs human: none
Notes: Confirmed the repo pin selects `.NET 8.0.419`; observed no relevant `DOTNET_*` or `NUGET_*` overrides in the default environment; confirmed there is no repo-local `NuGet.config`; confirmed NuGet tries to load the standard user-wide config chain and fails at `C:\Users\windy\AppData\Roaming\NuGet\NuGet.Config` before source access or package-cache use; machine-wide Visual Studio fallback/offline config layers also exist but are not the first failure point.
HANDOFF: status=done task="diagnose NuGet config permission failure" files="docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="non-escalated access to C:\\Users\\windy\\AppData\\Roaming\\NuGet\\NuGet.Config is denied before restore proceeds" next="pick the lowest-risk NuGet config remediation path before retrying full build verification"

### Turn
Date: 2026-04-06 00:00 local
Agent: Codex-Secondary
Task: Run Codex-only NuGet override experiment for sandboxed build verification
Status: partial
Files changed: .codex-temp/NuGet.Config, docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; repo-root check for `NuGet.config`; `dotnet --version`; `dotnet restore PromptForge.Core/PromptForge.Core.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.Core/PromptForge.Core.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.App/PromptForge.App.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.LicenseTool/PromptForge.LicenseTool.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.sln --configfile .codex-temp\\NuGet.Config -v minimal`; diagnostic re-runs on Diagnostics and solution builds
Observed risks: The original roaming `NuGet.Config` denial can be bypassed in this sandbox only with an explicit local config file and absolute repo-local `DOTNET_*` / `NUGET_*` overrides; relative `NUGET_PACKAGES` values cause their own restore error. After that fix, `PromptForge.Core` and `PromptForge.LicenseTool` build, but `PromptForge.App` is blocked by a file lock on `PromptForge.App\\bin\\Debug\\net8.0-windows\\PromptForge.Core.dll`, and Diagnostics/solution builds still fail when traversing the app project without surfacing a normal compiler error.
Needs next: if fuller build verification is needed inside Codex, either clear the app-output file lock or verify from a context where the app binaries are not in use, while keeping the Codex-only absolute-path NuGet override pattern
Needs human: none
Notes: Added a minimal sandbox note to the workspace context and a compact handoff rule preserving the safest known Codex-only restore pattern; `.codex-temp\\NuGet.Config` is an intentionally local experiment file, not a repo-wide NuGet policy change.
HANDOFF: status=partial task="Codex-only NuGet override experiment" files=".codex-temp/NuGet.Config, docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="PromptForge.App build blocked by file lock on PromptForge.App\\bin\\Debug\\net8.0-windows\\PromptForge.Core.dll; Diagnostics/solution still fail when traversing app restore/build" next="reuse absolute repo-local DOTNET/NUGET overrides for Codex builds and clear the app-output lock before expecting full solution verification"

### Turn
Date: 2026-04-06 00:02 local
Agent: Codex-Secondary
Task: Re-run Codex-only verification after closing Prompt Forge
Status: partial
Files changed: docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; `dotnet --version`; `dotnet restore PromptForge.Core/PromptForge.Core.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.Core/PromptForge.Core.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.LicenseTool/PromptForge.LicenseTool.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.App/PromptForge.App.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.sln --configfile .codex-temp\\NuGet.Config -v minimal`; diagnostic re-runs on Diagnostics and solution builds
Observed risks: Closing the running app resolves the prior `PromptForge.Core.dll` copy lock and restores successful app builds under the established override lane, but `PromptForge.Diagnostics` and `PromptForge.sln` still fail during restore-graph traversal with no surfaced compiler error. The only consistent surfaced signals remain the workload-resolver `MSB4276` messages for `Microsoft.NET.SDK.WorkloadAutoImportPropsLocator` and `Microsoft.NET.SDK.WorkloadManifestTargetsLocator`.
Needs next: if diagnostics or full-solution verification is required inside Codex, isolate whether those workload-resolver failures are benign noise or the actual restore-graph blocker before making any broader environment change
Needs human: none
Notes: Confirmed the app-lock blocker was caused by Prompt Forge itself being open. Preserved the same Codex-only absolute-path override lane without inventing a new restore path.
HANDOFF: status=partial task="rerun Codex-only verification after closing app" files="docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="PromptForge.Diagnostics and PromptForge.sln still fail during restore-graph traversal; app-lock blocker resolved" next="keep using the same absolute-path override lane and isolate the remaining diagnostics/solution restore-graph failure"

### Turn
Date: 2026-04-06 00:06 local
Agent: Codex-Secondary
Task: Forensically diagnose remaining Diagnostics/solution failure under Codex-only override lane
Status: partial
Files changed: .codex-temp/app_build_diag.log, .codex-temp/diagnostics_restore_diag.log, .codex-temp/solution_restore_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `PromptForge.Diagnostics/PromptForge.Diagnostics.csproj`; read `PromptForge.App/PromptForge.App.csproj`; read `PromptForge.sln`; read `global.json`; repo-root check for `Directory.Build.props`, `Directory.Build.targets`, `NuGet.config`, `nuget.config`; `dotnet --version`; `dotnet build PromptForge.App/PromptForge.App.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.App/PromptForge.App.csproj --configfile .codex-temp\\NuGet.Config -v diag`; `dotnet restore PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\\NuGet.Config -v diag`; `dotnet build PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\\NuGet.Config -v diag`; `dotnet restore PromptForge.sln --configfile .codex-temp\\NuGet.Config -v diag`; `dotnet build PromptForge.sln --configfile .codex-temp\\NuGet.Config -v diag`; targeted log inspection with `Select-String`
Observed risks: The workload-resolver `MSB4276` messages are present even during a successful app build, so they are not sufficient by themselves to explain the remaining failures. Diagnostics fails during NuGet restore-graph traversal before actual compilation begins: `PromptForge.Diagnostics -> PromptForge.App -> PromptForge.Core` is discovered, but the nested `MSBuild` call walking App's Core reference returns failed without surfacing a normal compiler or package-source error. Solution restore/build appears to fail for the same broader graph reason because the solution includes Diagnostics.
Needs next: if full diagnostics verification is still required inside Codex, isolate the Diagnostics project-graph edge more directly before changing environment assumptions; likely suspects are the Diagnostics reference shape or an SDK/NuGet static-graph edge case rather than the visible workload-resolver noise alone
Needs human: none
Notes: Created three local forensic logs under `.codex-temp` only. No repo-wide restore behavior changed.
HANDOFF: status=partial task="forensic diagnosis of Diagnostics/solution failure" files=".codex-temp/app_build_diag.log, .codex-temp/diagnostics_restore_diag.log, .codex-temp/solution_restore_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="Diagnostics and solution still fail during restore-graph traversal without a surfaced compiler error; MSB4276 appears to be adjacent noise rather than sole cause" next="treat MSB4276 as non-proven noise and isolate the nested Diagnostics->App->Core restore-graph failure"

### Turn
Date: 2026-04-06 00:11 local
Agent: Codex-Secondary
Task: Temporarily remove direct Diagnostics->Core reference to test project-graph hypothesis
Status: done
Files changed: PromptForge.Diagnostics/PromptForge.Diagnostics.csproj (temporary, restored), .codex-temp/diagnostics_build_without_direct_core_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `PromptForge.Diagnostics/PromptForge.Diagnostics.csproj`; read `PromptForge.App/PromptForge.App.csproj`; temporary removal of `..\PromptForge.Core\PromptForge.Core.csproj` from `PromptForge.Diagnostics.csproj`; `dotnet restore PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\\NuGet.Config -v minimal`; `dotnet build PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\\NuGet.Config -v diag`; `dotnet build PromptForge.sln --configfile .codex-temp\\NuGet.Config -v minimal`; restore `PromptForge.Diagnostics.csproj` to original contents; final `git status --short`
Observed risks: Removing the direct Core reference materially changes Diagnostics restore behavior: restore succeeds immediately under the existing Codex-only lane, which strongly implicates the original Diagnostics project-reference shape in the restore-graph failure. Diagnostics build still fails, but the failure moved forward into project-reference target evaluation (`_GetProjectReferenceTargetFrameworkProperties` / nested `GetTargetFrameworks`) rather than the earlier restore-graph walk failure. Solution build still fails in minimal output, so the graph change improved Diagnostics restore but did not prove full solution verification.
Needs next: if deeper diagnosis is needed, treat the direct Diagnostics->Core edge as a strong restore-graph suspect and inspect the later project-reference target-evaluation failure separately from restore
Needs human: none
Notes: The temporary project-file edit was reverted immediately after the experiment. No permanent project-graph change was kept.
HANDOFF: status=done task="temporary Diagnostics graph experiment" files="PromptForge.Diagnostics/PromptForge.Diagnostics.csproj (restored), .codex-temp/diagnostics_build_without_direct_core_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="original Diagnostics->App plus Diagnostics->Core graph strongly implicated in restore failure; later build-time project-reference evaluation still unresolved" next="keep the original project file unless a deliberate follow-up chooses to alter graph shape, and distinguish restore-graph improvement from later build failure"

### Turn
Date: 2026-04-06 00:14 local
Agent: Codex-Secondary
Task: Diagnose later Diagnostics failure point after temporary graph simplification
Status: done
Files changed: PromptForge.Diagnostics/PromptForge.Diagnostics.csproj (temporary, restored), .codex-temp/core_gettargetframeworks_diag.log, .codex-temp/app_getprojectreftfm_diag.log, .codex-temp/diagnostics_getprojectreftfm_without_direct_core_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `PromptForge.Diagnostics/PromptForge.Diagnostics.csproj`; read `PromptForge.App/PromptForge.App.csproj`; read `PromptForge.Core/PromptForge.Core.csproj`; targeted `Select-String` inspection of `.codex-temp/diagnostics_build_without_direct_core_diag.log`; `dotnet msbuild PromptForge.Core/PromptForge.Core.csproj -t:GetTargetFrameworks /v:diag`; `dotnet msbuild PromptForge.App/PromptForge.App.csproj -t:_GetProjectReferenceTargetFrameworkProperties /v:diag`; temporary removal of `..\PromptForge.Core\PromptForge.Core.csproj` from `PromptForge.Diagnostics.csproj`; `dotnet msbuild PromptForge.Diagnostics/PromptForge.Diagnostics.csproj -t:_GetProjectReferenceTargetFrameworkProperties /m:1 /v:diag`; restore `PromptForge.Diagnostics.csproj` to original contents; final `git status --short`
Observed risks: Standalone `PromptForge.Core` `GetTargetFrameworks` succeeds and reports `TargetPlatformMonikers=None`; standalone App `_GetProjectReferenceTargetFrameworkProperties` also succeeds. In the earlier temporary Diagnostics build, the later failure depended on full default-build context after `IncludeTransitiveProjectReferences` had added Core back into `ProjectReferenceWithConfiguration`, so Diagnostics ended up evaluating both App and Core in `_GetProjectReferenceTargetFrameworkProperties` even after the direct Core reference was removed. The same target succeeds when invoked directly against Diagnostics under the temporary graph edit, which indicates the later failure is context-dependent inside the full build rather than an intrinsic inability to evaluate App/Core target frameworks.
Needs next: if deeper diagnosis is required, inspect why full default-build context plus transitive project-reference expansion causes Diagnostics to fail while direct invocation of `_GetProjectReferenceTargetFrameworkProperties` does not; treat this as a build-context/project-graph issue rather than a proven SDK pin or package issue
Needs human: none
Notes: The temporary project-file edit was reverted immediately after the focused target experiment. No permanent project change was kept.
HANDOFF: status=done task="diagnose later Diagnostics target-framework failure" files="PromptForge.Diagnostics/PromptForge.Diagnostics.csproj (restored), .codex-temp/core_gettargetframeworks_diag.log, .codex-temp/app_getprojectreftfm_diag.log, .codex-temp/diagnostics_getprojectreftfm_without_direct_core_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="later Diagnostics failure depends on full build context with transitive project-reference expansion; exact inner MSBuild failure still not surfaced as a normal compiler error" next="treat the remaining issue as context-dependent project-reference evaluation, not a proven permanent graph fix"

### Turn
Date: 2026-04-06 00:20 local
Agent: Codex-Secondary
Task: Compare effective project-reference items in failing full Diagnostics path vs successful focused path
Status: done
Files changed: .codex-temp/diagnostics_full_default_build_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `PromptForge.Diagnostics/PromptForge.Diagnostics.csproj`; read `PromptForge.App/PromptForge.App.csproj`; read `PromptForge.Core/PromptForge.Core.csproj`; `dotnet build PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\\NuGet.Config -v diag`; targeted `Select-String` / `rg` inspection of `.codex-temp/diagnostics_full_default_build_diag.log`, `.codex-temp/diagnostics_build_without_direct_core_diag.log`, `.codex-temp/diagnostics_getprojectreftfm_without_direct_core_diag.log`, and `.codex-temp/app_getprojectreftfm_diag.log`; final `git status --short`
Observed risks: The successful focused Diagnostics `_GetProjectReferenceTargetFrameworkProperties` run does not surface a populated `ProjectReferenceWithConfiguration` or transitive-reference item set at all; it simply invokes the target and succeeds with `_ProjectReferenceTargetFrameworkPossibilities` empty. In the failing later full-build path from the temporary graph experiment, `ResolvePackageAssets` outputs `_TransitiveProjectReferences=..\PromptForge.Core\PromptForge.Core.csproj` with `NuGetPackageId=PromptForge.Core` / `NuGetPackageVersion=1.0.0`, `IncludeTransitiveProjectReferences` adds that Core item back into `ProjectReference`, and `AssignProjectConfiguration` carries App plus Core into `_ProjectReferenceWithConfiguration`, `ProjectReferenceWithConfiguration`, and `_MSBuildProjectReferenceExistent` before `_GetProjectReferenceTargetFrameworkProperties` fails. No concrete differences were observed for `SetTargetFramework`, `SkipGetTargetFrameworkProperties`, `Private`, or `AdditionalProperties`; the clearest proven difference is effective reference-set expansion plus the transitive Core metadata.
Needs next: if deeper diagnosis is still needed, compare the full-build path after `AssignProjectConfiguration` with a controlled build path that preserves the same targets but suppresses transitive reference expansion, so the next experiment can test whether reference-set expansion alone is sufficient
Needs human: none
Notes: No project files were edited in this pass. The later-stage reference-set comparison necessarily relies on the prior temporary-graph forensic log because the original graph currently fails earlier during restore.
HANDOFF: status=done task="compare failing Diagnostics reference-set metadata" files=".codex-temp/diagnostics_full_default_build_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="proven difference is transitive Core reference expansion into ProjectReferenceWithConfiguration; exact inner failure after that expansion remains unsurfaced" next="treat reference-set expansion as the clearest culprit and design the next smallest experiment around that, not around SDK/package changes"

### Turn
Date: 2026-04-06 00:27 local
Agent: Codex-Secondary
Task: Temporarily suppress transitive project-reference expansion in Diagnostics
Status: done
Files changed: PromptForge.Diagnostics/PromptForge.Diagnostics.csproj (temporary, restored), .codex-temp/diagnostics_build_disable_transitive_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `docs/CODEX_SECOND_INSTANCE_HANDOFF.md`; read `docs/CODEX_WORKSPACE_CONTEXT.md`; read `PromptForge.Diagnostics/PromptForge.Diagnostics.csproj`; read `PromptForge.App/PromptForge.App.csproj`; temporary addition of `<DisableTransitiveProjectReferences>true</DisableTransitiveProjectReferences>` to `PromptForge.Diagnostics.csproj`; `dotnet build PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\NuGet.Config -v minimal`; `dotnet build PromptForge.Diagnostics/PromptForge.Diagnostics.csproj --configfile .codex-temp\NuGet.Config -v diag`; targeted log inspection with `Select-String`; restore `PromptForge.Diagnostics.csproj` to original contents; final `git status --short`
Observed risks: The temporary property is visible in diagnostic output as `DisableTransitiveProjectReferences = true`, but Diagnostics still fails in `_GenerateRestoreProjectPathWalk` with `0 Error(s)`, so this simple suppression attempt did not move the failure forward. `PromptForge.Core` still appears in the restore-walk project set and App still traverses into Core, which means this experiment did not remove Core participation from the failing path.
Needs next: if further diagnosis is required, the next experiment should distinguish direct Core participation from transitive Core participation more cleanly than a bare `DisableTransitiveProjectReferences` toggle on Diagnostics
Needs human: none
Notes: No solution build was rerun because Diagnostics did not improve materially. The temporary project-file edit was reverted immediately after the experiment.
HANDOFF: status=done task="temporary Diagnostics transitive-suppression experiment" files="PromptForge.Diagnostics/PromptForge.Diagnostics.csproj (restored), .codex-temp/diagnostics_build_disable_transitive_diag.log, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="DisableTransitiveProjectReferences did not remove the failing restore-walk path; Core still participates and Diagnostics still fails with 0 surfaced errors" next="treat simple transitive-suppression as insufficient evidence for a fix and design the next smallest graph-isolation experiment"

### Turn
Date: 2026-04-06 00:33 local
Agent: Codex-Secondary
Task: Document current Diagnostics verification boundary and stop deeper forensics
Status: done
Files changed: docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `docs/CODEX_SECOND_INSTANCE_HANDOFF.md`; read `docs/CODEX_WORKSPACE_CONTEXT.md`; final `git status --short`
Observed risks: The unresolved Diagnostics/solution graph issue should not be allowed to blur the broader build picture. Under the established Codex-only override lane, `PromptForge.Core`, `PromptForge.LicenseTool`, and `PromptForge.App` verify successfully; the remaining weirdness is concentrated in Diagnostics/App/Core graph evaluation during restore/build machinery and is not currently treated as a core product blocker.
Needs next: if Diagnostics investigation resumes later, start from the known project-graph/evaluation findings rather than reopening settled SDK-drift, roaming-NuGet, or app-lock theories
Needs human: none
Notes: Added a compact workspace-context boundary section and a short handoff rule to contain future investigation scope. No project/source files were changed.
HANDOFF: status=done task="document Diagnostics verification boundary" files="docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="PromptForge.Diagnostics and full solution verification remain special-case graph/evaluation surfaces" next="treat Core, LicenseTool, and App as verified under the Codex-only lane and keep future Diagnostics work narrowly graph-focused if it resumes"

### Turn
Date: 2026-04-06 00:49 local
Agent: Codex-Secondary
Task: Install approved Anime semantic wording replacement map
Status: done
Files changed: PromptForge.Core/Services/SliderLanguageCatalog.cs, docs/MOJIBAKE_LOG.txt, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `docs/CODEX_SECOND_INSTANCE_HANDOFF.md`; read `docs/CODEX_WORKSPACE_CONTEXT.md`; read `PromptForge.Core/Services/SliderLanguageCatalog.cs`; read `PromptForge.Core/Services/LaneRegistry.cs`; read `PromptForge.Core/Services/PromptBuilderService.cs`; targeted `Select-String` inspection of Anime phrase surfaces before edit; targeted post-edit `Select-String` verification that replaced Anime phrases no longer remained in `SliderLanguageCatalog.cs`; targeted `Select-String` verification that Anime selector structure/default anchors in `LaneRegistry.cs` were unchanged; `git diff -- PromptForge.Core/Services/SliderLanguageCatalog.cs`; final `git status --short`
Observed risks: This pass changed wording only inside Anime phrase-authority surfaces. No lane registration/classification, selector labels, modifier labels, defaults, modifier cap behavior, guardrail trigger shape, or prompt assembly order were changed. One apparent mojibake instance (`Classic Anime (1960sâ€“1970s)`) was encountered during shell readout, but the underlying source in both `LaneRegistry.cs` and `SliderLanguageCatalog.cs` was already the clean `Classic Anime (1960s–1970s)` form; it was logged as a display artifact rather than patched as source corruption.
Needs next: if validation is needed, inspect prompt output for Anime wording quality only; do not reinterpret this as a structural Anime lane change
Needs human: none
Notes: Installed the approved replacement map across Anime base descriptors, style descriptors, era descriptors, modifier phrases, slider-band phrases, guide-text mirrors, and Anime guardrail collapse phrases. Added `docs/MOJIBAKE_LOG.txt` to record the encountered display-only mojibake artifact from this pass.
HANDOFF: status=done task="install Anime semantic wording replacement map" files="PromptForge.Core/Services/SliderLanguageCatalog.cs, docs/MOJIBAKE_LOG.txt, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat Anime wording update as installed and keep future follow-up on prompt quality rather than lane structure"

### Turn
Date: 2026-04-06 00:56 local
Agent: Codex-Secondary
Task: Install approved Anime default-value update
Status: done
Files changed: PromptForge.Core/Services/LaneRegistry.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `docs/CODEX_SECOND_INSTANCE_HANDOFF.md`; read `docs/CODEX_WORKSPACE_CONTEXT.md`; read `PromptForge.Core/Services/LaneRegistry.cs`; targeted inspection of `PromptForge.App/ViewModels/MainWindowViewModel.cs` to confirm `ApplyAnimeIntentDefaults()` only delegates selector defaults and resets Anime modifiers; `git diff -- PromptForge.Core/Services/LaneRegistry.cs`; targeted `Select-String` verification of Anime default lines in `LaneRegistry.cs`; final `git status --short`
Observed risks: This pass changed Anime defaults only in the lane metadata block. No wording, selector labels, modifier labels, modifier cap behavior, lane registration/classification, guardrail logic shape, or prompt assembly order changed. The App-side Anime initializer does not own the numeric slider defaults; it only applies selector defaults and clears Anime modifiers, so no second default surface needed patching.
Needs next: if validation is needed, treat this as a metadata-default shift only and review resulting Anime starting posture rather than reopening wording or structural work
Needs human: none
Notes: Updated Anime defaults to the approved values, including newly explicit `Symbolism`, `Framing`, `Temperature`, and `LightingIntensity`, while keeping the Anime lighting preset unchanged as `Soft glow`.
HANDOFF: status=done task="install Anime default-value update" files="PromptForge.Core/Services/LaneRegistry.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat Anime defaults as updated in metadata and keep any follow-up focused on starting posture rather than lane structure"

### Turn
Date: 2026-04-06 10:11 local
Agent: Codex-Secondary
Task: Install approved Pixel Art semantic wording replacement map
Status: done
Files changed: PromptForge.Core/Services/SliderLanguageCatalog.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `docs/CODEX_SECOND_INSTANCE_HANDOFF.md`; read `docs/CODEX_WORKSPACE_CONTEXT.md`; read `PromptForge.Core/Services/SliderLanguageCatalog.cs`; read `PromptForge.Core/Services/LaneRegistry.cs`; read `PromptForge.Core/Services/PromptBuilderService.cs`; targeted `rg` inspection of Pixel phrase-authority surfaces before edit; `git diff -- PromptForge.Core/Services/SliderLanguageCatalog.cs`; targeted `rg` verification that replaced Pixel source phrases no longer remained in active Pixel helpers; final `git status --short`
Observed risks: This pass changed wording only inside Pixel Art phrase-authority surfaces. No defaults, subtype labels, modifier labels, modifier cap behavior, guardrail trigger shape, lane registration/classification, or prompt assembly order changed. The shared modified `SliderLanguageCatalog.cs` diff also contains earlier Anime wording work; that is pre-existing task history, not new Pixel scope drift.
Needs next: if validation is needed, inspect Pixel prompt output/taste only; do not reinterpret this wording install as a structural Pixel lane change
Needs human: none
Notes: Installed the approved replacement map across Pixel subtype descriptors, modifier phrases, slider-band phrases, guide-text mirrors, and Pixel guardrail collapse phrases. The lane shell entries were approved no-ops and therefore remained text-identical.
HANDOFF: status=done task="install Pixel Art semantic wording replacement map" files="PromptForge.Core/Services/SliderLanguageCatalog.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat Pixel wording update as installed and keep any follow-up focused on prompt quality rather than lane structure"

### Turn
Date: 2026-04-06 10:15 local
Agent: Codex-Secondary
Task: Install approved Pixel Art default-value update
Status: done
Files changed: PromptForge.Core/Services/LaneRegistry.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read Pixel Art default block in `PromptForge.Core/Services/LaneRegistry.cs`; `git diff -- PromptForge.Core/Services/LaneRegistry.cs`; final `git status --short`
Observed risks: This pass changed Pixel Art defaults only in the lane metadata block. No wording, subtype labels, modifier labels, modifier cap behavior, lane registration/classification, guardrail logic shape, or prompt assembly order changed. The shared `LaneRegistry.cs` diff also contains earlier Anime default work; that is pre-existing task history, not new Pixel scope drift.
Needs next: if validation is needed, treat this as a Pixel starting-posture shift only and review the new lane opening state rather than reopening wording or structural work
Needs human: none
Notes: Updated Pixel Art defaults to the approved values, including newly explicit `Symbolism`, `Framing`, and `CameraDistance`, while keeping the Pixel Art lighting preset unchanged as `Soft daylight`.
HANDOFF: status=done task="install Pixel Art default-value update" files="PromptForge.Core/Services/LaneRegistry.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat Pixel defaults as updated in metadata and keep any follow-up focused on starting posture rather than lane structure"

### Turn
Date: 2026-04-06 12:31 local
Agent: Codex-Secondary
Task: Install Cinematic Semantic Dictionary v2 as a strict phrase-authority replacement
Status: done
Files changed: PromptForge.Core/Services/SliderLanguageCatalog.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `docs/CODEX_SECOND_INSTANCE_HANDOFF.md`; read `docs/CODEX_WORKSPACE_CONTEXT.md`; read Cinematic sections in `PromptForge.Core/Services/SliderLanguageCatalog.cs`; read `PromptForge.Core/Services/PromptBuilderService.cs`; read `PromptForge.Core/Services/LaneRegistry.cs`; targeted `git diff -- PromptForge.Core/Services/SliderLanguageCatalog.cs`; targeted `rg` verification of old Cinematic source phrases; final `git status --short`
Observed risks: This pass changed wording only in the dedicated Cinematic phrase-authority surfaces. No lane metadata, defaults, subtype labels, modifier labels, guardrail trigger shape, or prompt assembly order changed. Some old Cinematic-adjacent strings still exist elsewhere in broader shared definition/variant tables inside `SliderLanguageCatalog.cs`; those were outside the requested dedicated Cinematic install surfaces and were not patched in this pass.
Needs next: if validation is needed, inspect Cinematic prompt output/taste only; do not reinterpret this wording install as a structural Cinematic lane change
Needs human: none
Notes: Installed the approved Cinematic subtype descriptors, modifier descriptors, slider-band phrases, guide-text mirrors, lighting mapping outputs, and guardrail collapse phrases. Initial lighting-key mismatch on the volumetric entry was resolved by explicit human clarification to use `volumetric light`; the warm-directional output was aligned to `warm directional light` under the same root-cleanup intent while keeping the source key unchanged.
HANDOFF: status=done task="install Cinematic Semantic Dictionary v2" files="PromptForge.Core/Services/SliderLanguageCatalog.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat Cinematic wording update as installed and keep any follow-up focused on prompt quality rather than lane structure"

### Turn
Date: 2026-04-06 12:36 local
Agent: Codex-Secondary
Task: Remove stray shared/default `language` endings from four phrase strings
Status: done
Files changed: PromptForge.Core/Services/SliderLanguageCatalog.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: targeted source inspection of shared/default phrase definitions; final phrase-only patch
Observed risks: This was a tiny shared-definition wording cleanup only. No lane metadata, defaults, labels, caps, guardrail logic, or assembly order changed.
Needs next: none
Needs human: none
Notes: Removed only the trailing word `language` from four shared/default phrases believed to be command-string leakage: `noticeably stylized image language`, `strong authored visual language`, `highly stylized visual language`, and `primarily literal image language`.
HANDOFF: status=done task="remove stray shared/default language endings" files="PromptForge.Core/Services/SliderLanguageCatalog.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat shared/default language-ending cleanup as installed"

### Turn
Date: 2026-04-06 13:28 local
Agent: Codex-Secondary
Task: Install updated Cinematic default-value set
Status: done
Files changed: PromptForge.Core/Services/LaneRegistry.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read Cinematic default block in `PromptForge.Core/Services/LaneRegistry.cs`; targeted inspection of `PromptForge.App/ViewModels/MainWindowViewModel.cs` for Cinematic open/default behavior; `git diff -- PromptForge.Core/Services/LaneRegistry.cs`; final `git status --short`
Observed risks: This pass changed Cinematic defaults only in the lane metadata block. No wording, subtype labels, modifier labels, modifier cap behavior, lane registration/classification, guardrail logic shape, or prompt assembly order changed. The App-side shared reset path already uses `Lighting = "Soft daylight"`, so the requested lighting default aligns with existing open/reset behavior.
Needs next: if validation is needed, treat this as a Cinematic starting-posture shift only and review the new lane opening state rather than reopening wording or structural work
Needs human: none
Notes: Updated Cinematic defaults to the approved values, including newly explicit `Symbolism`, `Framing`, `CameraDistance`, `CameraAngle`, `Chaos`, `Temperature`, and `LightingIntensity`, and set the lighting default to remain `Soft daylight`.
HANDOFF: status=done task="install Cinematic default-value update" files="PromptForge.Core/Services/LaneRegistry.cs, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat Cinematic defaults as updated in metadata and keep any follow-up focused on starting posture rather than lane structure"

### Turn
Date: 2026-04-08 18:00 local
Agent: Codex-Primary
Task: Add shared version-history summary and declare Version 5 local working label
Status: done
Files changed: docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `docs/CODEX_SECOND_INSTANCE_HANDOFF.md`; read `RELEASE_NOTES.md`; targeted `rg` inspection of `3.1` and `4.1.4` version anchors in app, installer, and release docs
Observed risks: Version references are currently split across at least two eras: public-facing release notes still say `3.1`, while local build/release metadata uses `4.1.4`. This handoff update establishes `Version 5` as the shared local label only; it does not itself perform the coordinated version-string update across code, installer, and public release surfaces.
Needs next: when the actual version-bump task begins, update all authoritative version surfaces together and keep `Version 5` as the expected target label
Needs human: none
Notes: Added a stable handoff section so the other Codex instance can quickly understand the version progression without guessing which file is authoritative for naming versus shipping metadata.
HANDOFF: status=done task="declare Version 5 handoff baseline" files="docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat Version 5 as the active local target until the coordinated version-bump pass updates all release surfaces"

### Turn
Date: 2026-04-08 18:07 local
Agent: Codex-Primary
Task: Bump local release/version surfaces to Version 5
Status: done
Files changed: PromptForge.App/PromptForge.App.csproj, tools/installer/PromptForge.iss, RELEASING.md, RELEASE_NOTES.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `git status --short`; read `PromptForge.App/PromptForge.App.csproj`; read `tools/installer/PromptForge.iss`; read `RELEASING.md`; read `RELEASE_NOTES.md`; targeted `rg` inspection of `4.1.4`, `3.1`, and `5.0.0` anchors in version-related surfaces
Observed risks: This pass updates the primary release/version anchors only. It does not verify a built installer, generated release-notes filename, or any branch-naming/doc-history references that intentionally preserve past context such as `codex/4.1.4-release`.
Needs next: use `5.0.0` for upcoming installer/release output names and keep any remaining historical references untouched unless they are meant to represent the current shipping version
Needs human: none
Notes: Standardized app metadata and installer naming on `5.0.0` while presenting the human-facing release heading as `Prompt Forge 5`.
HANDOFF: status=done task="bump release surfaces to version 5" files="PromptForge.App/PromptForge.App.csproj, tools/installer/PromptForge.iss, RELEASING.md, RELEASE_NOTES.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="historical entry; current release packaging target was later bumped to 5.1.2"

### Turn
Date: 2026-04-08 19:00 local
Agent: Codex-Primary
Task: Implement V5 machine-bound and Temporary licensing schema plus hard demo-expiry lock
Status: done
Files changed: PromptForge.App/ViewModels/MainWindowViewModel.cs, PromptForge.App/MainWindow.xaml, PromptForge.App/UnlockWindow.xaml, PromptForge.App/UnlockWindow.xaml.cs, PromptForge.App/Models/UnlockState.cs, PromptForge.App/Services/ILicenseService.cs, PromptForge.App/Services/LicenseService.cs, PromptForge.Core/Models/PromptForgeLicense.cs, PromptForge.Core/Models/PromptForgeLicenseModes.cs, PromptForge.Core/Services/PromptForgeLicenseCodec.cs, PromptForge.Core/Services/PromptForgeMachineBindingService.cs, PromptForge.LicenseTool/Program.cs, tools/licensing/Generate-UnlockFile-V5.ps1, tools/licensing/V5_GENERATOR_README.md, docs/V5_LICENSE_PLAN.md
Tests run: `dotnet build PromptForge.App/PromptForge.App.csproj --no-restore`; `dotnet build PromptForge.LicenseTool/PromptForge.LicenseTool.csproj --no-restore`; `dotnet run --project PromptForge.LicenseTool --no-build -- --show-request-code`; generated one Temporary and one MachineBound unlock under `.codex-temp/license-tests/`
Observed risks: Signed license modes, machine matching, and signed allowed-lane gating are wired, but local unlock persistence is still plain JSON rather than DPAPI-protected state.
Needs next: decide whether to DPAPI-protect unlock and demo state before release packaging, and whether future `EntitlementProfile` names should expand into lane bundles beyond explicit `AllowedLanes`
Needs human: none
Notes: Demo expiry now hard-hides prompt output and authoring controls when copies reach zero. Unlock window now shows the local activation request code, and the V5 generator package now issues both Temporary and MachineBound unlocks for real.
HANDOFF: status=done task="implement V5 licensing modes and demo hard lock" files="PromptForge.App/ViewModels/MainWindowViewModel.cs, PromptForge.App/MainWindow.xaml, PromptForge.App/UnlockWindow.xaml, PromptForge.App/UnlockWindow.xaml.cs, PromptForge.App/Models/UnlockState.cs, PromptForge.App/Services/ILicenseService.cs, PromptForge.App/Services/LicenseService.cs, PromptForge.Core/Models/PromptForgeLicense.cs, PromptForge.Core/Models/PromptForgeLicenseModes.cs, PromptForge.Core/Services/PromptForgeLicenseCodec.cs, PromptForge.Core/Services/PromptForgeMachineBindingService.cs, PromptForge.LicenseTool/Program.cs, tools/licensing/Generate-UnlockFile-V5.ps1, tools/licensing/V5_GENERATOR_README.md, docs/V5_LICENSE_PLAN.md" blockers="storage hardening still pending; future profile-expansion rules still pending" next="keep AllowedLanes-based lane gating active, then decide whether to add DPAPI protection and broader EntitlementProfile bundle mapping before release packaging"

## Recent Semantic Lane Work Boundary Update

This section is intentionally more operational than the older turn log. It captures the current working boundary for the large semantic-lane and pairing push that happened after the earlier entries above.

### Protected Shared Surfaces: Avoid Unless There Is No Other Choice

The recent lane work was done under repeated "smallest lane-local surface" rules. Future Codex work should preserve that discipline.

Protected shared files and the current expectation for them:

- [PromptForge.Core/Services/PromptSemanticPairCollapseService.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\PromptSemanticPairCollapseService.cs)
  - Treat as protected.
  - Do not edit during routine pair-authoring passes unless a brand-new lane truly has no dispatch route yet.
  - This file owns lane dispatch into pair maps plus the exact-fragment collapse path. It is deliberately fragile and should not be casually touched just because a lane needs new pair content.
- [PromptForge.Core/Services/SliderLanguageCatalog.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.cs)
  - Treat as protected for pairing work.
  - Do not widen shared phrase/fallback logic during lane-local pairing installs.
  - If a lane already has a dedicated `...Pairs.cs` or dedicated lane-local phrase file, author there instead.
- [PromptForge.Core/Services/PromptBuilderService.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\PromptBuilderService.cs)
  - Treat as protected.
  - Prompt assembly order is explicitly not to be changed during semantic lane work.
- [PromptForge.App/ViewModels/MainWindowViewModel.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\ViewModels\MainWindowViewModel.cs)
  - Avoid unless existing state-sync wiring truly requires a narrow hook.
  - Most recent work preferred dedicated partials such as `MainWindowViewModel.SliderSuppressions.cs`, `MainWindowViewModel.StandardLanePanels.cs`, `MainWindowViewModel.ConfigurationMapping.cs`, and other split surfaces instead of widening the monolith.
- [PromptForge.Core/Services/SliderLanguageCatalog.ConceptArt.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ConceptArt.cs)
  - During Concept Art pairing passes this was treated as phrase-authority only.
  - Do not use it as the first-choice surface for pair installs; those belong in `SliderLanguageCatalog.ConceptArtPairs.cs`.
  - Only use this file when the task is specifically about subtype-aware slider-band wording, guide-text alignment, or lane-local omission/suppression wording behavior.

Working rule that was enforced repeatedly:

- prefer lane-local `SliderLanguageCatalog.<Lane>Pairs.cs` files for pair work
- leave prompt assembly order unchanged
- leave guardrail ownership unchanged
- do not widen metadata into phrase logic
- do not assume a suppressed slider means its pair logic should disappear forever
- if a subtype-applied exclusion exists, treat it as default emission behavior only; the pair map should still exist if the user re-enables the slider

### Pairing Infrastructure: What Was Added and How It Is Intended To Be Used

The repository now has a much larger dedicated semantic-pairing surface than the older handoff entries reflect.

Key new/active pairing files include:

- [PromptForge.Core/Services/PromptSemanticPairCollapseService.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\PromptSemanticPairCollapseService.cs)
- [PromptForge.Core/Services/SliderLanguageCatalog.SemanticPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.SemanticPairs.cs)
- lane-local pair files such as:
  - [PromptForge.Core/Services/SliderLanguageCatalog.ConceptArtPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ConceptArtPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.CinematicPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.CinematicPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.ComicBookPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ComicBookPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.AnimePairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.AnimePairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.PhotographyPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.PhotographyPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.ProductPhotographyPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ProductPhotographyPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.FoodPhotographyPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.FoodPhotographyPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.LifestyleAdvertisingPhotographyPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.LifestyleAdvertisingPhotographyPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.ArchitectureArchvizPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ArchitectureArchvizPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.ThreeDRenderPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ThreeDRenderPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.PixelArtPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.PixelArtPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.WatercolorPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.WatercolorPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.GraphicDesignPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.GraphicDesignPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.TattooArtPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.TattooArtPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.ChildrensBookPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ChildrensBookPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.EditorialIllustrationPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.EditorialIllustrationPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.FantasyIllustrationPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.FantasyIllustrationPairs.cs)
  - [PromptForge.Core/Services/SliderLanguageCatalog.InfographicDataVisualizationPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.InfographicDataVisualizationPairs.cs)

The active design pattern is:

1. Phrase authority stays in the lane-local slider-language surfaces.
2. Pair authority stays in lane-local `...Pairs.cs` files.
3. `PromptSemanticPairCollapseService` routes by lane and tries exact-fragment collapse only when both independent fragments would otherwise emit.
4. If either participating slider is excluded from prompt emission, the pair collapse should not force a fused phrase.
5. If pair interactions are disabled, lanes fall back to normal independent slider emission.

### Important Pairing Fragility Notes

These are known risks, but the human explicitly said not to address fragility until the lane work is finished.

- Pair collapses must target the actual emitted prompt fragments, not just the raw source-band phrases in isolation.
  - Practical consequence: always verify what the live prompt-preview path is actually emitting before finalizing a lane-local pair map.
  - If a lane emits cleaned, fallback-adjusted, suppressed, or otherwise transformed wording, author the lane-local collapse against those emitted fragments so the exact-fragment replacement actually fires.
  - Do not treat this as permission to widen shared collapse machinery; prefer the smallest lane-local fix that matches live emitted output.
- `PromptSemanticPairCollapseService` currently tokenizes the prompt by comma and then exact-matches fragments.
  - Practical consequence: do not casually introduce commas inside slider phrases or fused pair phrases.
  - The system currently relies on the unstated invariant that emitted fragments are comma-safe.
- Pair dispatch is a manual lane-routing surface.
  - A lane-local pair file can compile and look complete while never running if dispatch was not wired.
- Most subtype/style branches key off raw strings.
  - Renaming a subtype/style value can silently disable a pair branch.
- [PromptForge.Core/Services/SliderLanguageCatalog.ComicBookPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ComicBookPairs.cs)
  contains a future maintenance trap:
  - there is a `yield break` guard tied to `General Comic`
  - any future style-specific branch appended below the wrong point can become dead code without obvious symptoms
- [PromptForge.Core/Services/SliderLanguageCatalog.ConceptArtPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ConceptArtPairs.cs)
  has become a large subtype-branch monolith.
  - It is still the correct current home for Concept Art pair maps.
  - It should not be "cleaned up" opportunistically during product work.

### Future Pair-Grid Interpretation Note

If future work begins on paired-grid, paired-pad, or other 2D pair-control UI, read
[docs/PAIR_GRID_INTERPRETATION_NOTES.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\docs\PAIR_GRID_INTERPRETATION_NOTES.md)
before deciding axis labels, helper text, or whether raw slider names are sufficient for a human-facing 2D control surface.

### Concept Art: Recent Work Was Large and Is Easy To Underestimate

The Concept Art lane received the densest recent semantic work.

Files directly involved:

- [PromptForge.Core/Services/SliderLanguageCatalog.ConceptArt.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ConceptArt.cs)
- [PromptForge.Core/Services/SliderLanguageCatalog.ConceptArtPairs.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.ConceptArtPairs.cs)
- [PromptForge.App/ViewModels/MainWindowViewModel.SliderSuppressions.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\ViewModels\MainWindowViewModel.SliderSuppressions.cs)
- [PromptForge.App/ViewModels/MainWindowViewModel.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\ViewModels\MainWindowViewModel.cs)

#### Concept Art subtype-sensitive slider-band installs

Approved subtype maps were installed for:

- `keyframe-concept`
- `environment-concept`
- `character-concept`
- `creature-concept`
- `costume-concept`
- `prop-concept`
- `vehicle-concept`

The pattern used throughout:

- runtime prompt output and guide text must align through the same subtype-aware helper path
- no redesign of the lane shell
- no default/nudge/guardrail/policy/assembly-order changes
- only the approved slider-band wording is changed per subtype

#### Concept Art default subtype-applied exclusions

The current tracked subtype suppression map is:

- `keyframe-concept` => `NarrativeDensity`
- `environment-concept` => `Chaos`
- `character-concept` => `BackgroundComplexity`
- `costume-concept` => `BackgroundComplexity`
- `prop-concept` => `BackgroundComplexity`
- `vehicle-concept` => `BackgroundComplexity`
- `creature-concept` => none

Important state rule:

- these are applied through the existing tracked exclusion path
- the UI "exclude from prompt" state reflects them
- the underlying slider values are not mutated
- manual user exclusions are preserved
- switching away auto-restores only exclusions that the subtype path itself applied

#### Concept Art pair-family installs

Shared-safe pair families installed for Concept Art:

- `Temperature × LightingIntensity`
- `Saturation × Contrast`

Subtype-sensitive pair families installed:

- `keyframe-concept`
  - `Framing × CameraDistance`
  - `AtmosphericDepth × FocusDepth`
- `environment-concept`
  - `Framing × CameraDistance`
  - `AtmosphericDepth × FocusDepth`
- `character-concept`
  - `Framing × CameraDistance`
  - `AtmosphericDepth × FocusDepth`
- `costume-concept`
  - `Framing × CameraDistance`
  - `AtmosphericDepth × FocusDepth`
- `prop-concept`
  - `Framing × CameraDistance`
  - `AtmosphericDepth × FocusDepth`
- `vehicle-concept`
  - `Framing × CameraDistance`
  - `AtmosphericDepth × FocusDepth`
- `creature-concept`
  - `Framing × CameraDistance`
  - `Tension × Awe`

#### Concept Art post-install taste audit and micro-polish

A dedicated audit pass reviewed:

- shared-safe pairs
- keyframe/environment pairs
- character/costume pairs
- prop/vehicle pairs
- creature pairs

The audit concluded the overall surface was strong, but a tiny micro-polish pass was warranted.

Approved weak-cell replacements that were actually applied:

- shared-safe `Saturation × Contrast`
  - `(2,2)` => `balanced chroma field`
  - `(2,4)` => `balanced striking separation`
- `keyframe-concept` `AtmosphericDepth × FocusDepth`
  - `(2,1)` => `clear staged depth read`
- `environment-concept` `AtmosphericDepth × FocusDepth`
  - `(2,1)` => `clear environmental spatial read`
- `prop-concept` `AtmosphericDepth × FocusDepth`
  - `(2,1)` => `clean broad object read`
  - `(2,3)` => `clean feature-led focus`
- `vehicle-concept` `AtmosphericDepth × FocusDepth`
  - `(2,1)` => `clean broad machine read`
  - `(2,3)` => `clean engineering-focus pull`

If future work touches Concept Art pair wording, start from the already-installed map and micro-polish state rather than re-deriving the family structure.

### Editorial Illustration: Recent Suppression Behavior

Editorial Illustration received prompt-emission suppression work, but the approved implementation path was to reuse existing exclusion-from-prompt wiring rather than invent new hidden omission logic.

Files involved:

- [PromptForge.Core/Services/Lanes/EditorialIllustrationLane.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\Lanes\EditorialIllustrationLane.cs)
- [PromptForge.App/ViewModels/MainWindowViewModel.SliderSuppressions.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\ViewModels\MainWindowViewModel.SliderSuppressions.cs)
- [PromptForge.Core/Services/SliderLanguageCatalog.EditorialIllustration.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.EditorialIllustration.cs)

Current Editorial default suppression behavior:

- `Stylization` => excluded from prompt by default
- `Chaos` => excluded from prompt by default
- `BackgroundComplexity` => excluded from prompt by default

Additional existing Editorial behavior preserved:

- black-and-white monochrome still additionally suppresses `Saturation` and `Temperature`

Operational note:

- these suppressions now ride the same exclusion checkbox architecture the user sees in the UI
- sliders stay visible
- slider values stay intact
- the UI should reflect the suppression state instead of hiding the omission behind a secret resolver-only branch

### Recent UI / Lane-Help / Small UX Work

This work happened after the earlier handoff entries and should be considered part of the current working state.

#### Lane help tooltip/footer system

Files:

- [PromptForge.App/MainWindow.xaml](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\MainWindow.xaml)
- [PromptForge.App/ViewModels/StandardLanePanelViewModels.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\ViewModels\StandardLanePanelViewModels.cs)
- [PromptForge.App/ViewModels/MainWindowViewModel.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\ViewModels\MainWindowViewModel.cs)
- [PromptForge.App/Services/LaneHelpTooltipCatalog.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Services\LaneHelpTooltipCatalog.cs)

What was added:

- a compact footer help/contact surface in lane panels
- a small info icon opening a compact popup with lane-specific micro-guidance
- lane tooltip content with the structure:
  - `Principle`
  - `Weak`
  - `Stronger`
- a clickable lane footer email link for `WindySoliloquy@gmail.com`
- a popup action to copy the email address

Important fixes that already happened:

- startup XAML crash fixed by defining `LaneHelpFooterTemplate` before it was referenced
- footer clipping fixed by changing the contact block layout
- blank email rendering fixed by binding the toggle button's `Content` correctly
- missing lane entries added for `Fantasy Illustration` and `Infographic / Data Visualization`

Current expectation:

- actual lanes in current use should show the footer/email/info icon if they have help content
- the shared standard lane panel path and the custom Anime / Comic Book / Vintage Bend panels are already wired

#### Subject helper icon

File:

- [PromptForge.App/MainWindow.xaml](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\MainWindow.xaml)

What was added:

- a helper icon beside the shared Subject field label, visually matching the artist-influence helper style
- tooltip copy explaining the fast subject replacement shortcut:
  - place cursor at end of subject
  - `Ctrl + Shift + Up Arrow` to select the full line
  - type a replacement or paste from clipboard
  - LLM-generated subject ideation can be pasted here for rapid iteration
- tooltip body text was later changed to black for readability

#### Pixel Art realism helper-text fix

File:

- [PromptForge.App/ViewModels/MainWindowViewModel.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\ViewModels\MainWindowViewModel.cs)

What changed:

- Pixel Art `Realism` at the minimal band intentionally emits no explicit prompt phrase
- that had made the button helper look blank at the lowest setting
- helper text now explicitly shows `Pixel Art: omit explicit realism` for that minimal band while leaving prompt behavior unchanged

### Build / Verification Notes For Recent Work

- When Prompt Forge itself is running, full app or solution builds can fail because `PromptForge.App.exe` or copied output files are locked.
- For core-only semantic work, `dotnet build PromptForge.Core\PromptForge.Core.csproj --no-restore` was often the practical verification surface.
- For UI/XAML work, `dotnet build PromptForge.sln --no-restore` was used when the app was not open.
- Several recent build failures were file-lock-only failures rather than code failures.

### Prompt Simulation Harness Note

A disposable read-only prompt-simulation harness now exists for current semantic-lane audit work.

Current location:

- [C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness)

Current purpose:

- exercise the real `MainWindowViewModel` -> `CaptureConfiguration()` -> `PromptBuilderService.Build(...)` -> `PromptSemanticPairCollapseService.Apply(...)` path without editing production code
- produce actual assembled positive-prompt output for requested audit cases
- verify whether pair collapses really remove both base fragments once `SemanticPairInteractions` is enabled

Practical rules:

- treat `.codex-temp` harnesses as disposable local tooling, not product architecture
- prefer a fresh `MainWindowViewModel` per simulated case so subtype nudges, modifier state, and intent defaults do not leak across cases
- if the audit concerns semantic pairing, explicitly decide whether `SemanticPairInteractions` should be on or off before claiming the outcome proves anything
- if the next Codex instance needs to reuse or adapt this path, read the dedicated usage doc first instead of reverse-engineering the throwaway harness in isolation

Usage doc:

- [docs/GRAPHIC_DESIGN_PROMPT_SIMULATION.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\docs\GRAPHIC_DESIGN_PROMPT_SIMULATION.md)

### Current Working-Tree Reminder

The working tree now contains many modified and untracked files from the broader semantic-lane/pairing push. Do not assume this handoff section lists every touched file in the repository-wide change set.

Before doing anything destructive or broad:

1. run `git status --short`
2. assume uncommitted changes may belong to the human or another Codex
3. avoid sweeping edits across pairing files just because the pattern now looks repetitive
4. prefer narrow, lane-local edits unless the task explicitly calls for a shared-surface change

### Recommendation To The Next Codex Instance

If the next task is another semantic lane pass:

- start by locating the lane-local phrase file
- then locate the lane-local pair file
- only touch the shared collapse/routing surfaces if you can prove the lane has no current route
- do not "clean up" the Concept Art or Comic Book pair files as a side quest
- do not address pairing fragility yet unless the human explicitly pivots to that work

### Turn
Date: 2026-04-17 21:00 local
Agent: Codex-Primary
Task: Document read-only Graphic Design prompt simulation path for future Codex reuse
Status: done
Files changed: docs/CODEX_SECOND_INSTANCE_HANDOFF.md, docs/GRAPHIC_DESIGN_PROMPT_SIMULATION.md, .codex-temp/graphic-design-audit-harness/graphic-design-audit-harness.csproj, .codex-temp/graphic-design-audit-harness/Program.cs
Tests run: `dotnet build PromptForge.sln --no-restore`; `dotnet build .codex-temp\\graphic-design-audit-harness\\graphic-design-audit-harness.csproj --no-restore`; `dotnet run --no-build --project .codex-temp\\graphic-design-audit-harness\\graphic-design-audit-harness.csproj`
Observed risks: The harness is intentionally disposable and currently Graphic-Design-focused; future reuse across other lanes may require small harness edits rather than a zero-change rerun. Prompt-audit conclusions are also sensitive to the global `SemanticPairInteractions` flag, so forgetting that toggle will produce misleading "pair failed" results.
Needs next: reuse the harness/doc for future read-only prompt audits instead of hand-reconstructing prompt output when the user asks for actual assembled prompts
Needs human: none
Notes: Added a compact handoff note recording that a real prompt-simulation path now exists, where it lives, what it validates, and the rule that fresh viewmodels plus explicit pair-toggle posture are required for trustworthy audit results. Added a dedicated usage doc for the other Codex instance so it can rerun or adapt the harness without treating `.codex-temp` tooling as product code.
HANDOFF: status=done task="document prompt simulation harness" files="docs/CODEX_SECOND_INSTANCE_HANDOFF.md, docs/GRAPHIC_DESIGN_PROMPT_SIMULATION.md, .codex-temp/graphic-design-audit-harness/graphic-design-audit-harness.csproj, .codex-temp/graphic-design-audit-harness/Program.cs" blockers="none" next="use the documented read-only harness path for future actual-output prompt audits"

### Turn
Date: 2026-04-19 local
Agent: Codex-Primary
Task: Document the proven Anime compact-lane seam and continuation rules for future Codex reuse
Status: done
Files changed: docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: The current Anime compact prototype still relies on a small number of host/viewmodel routing hooks in `MainWindow.xaml` and `MainWindowViewModel.cs`; future work should extend the lane-local extracted view and narrow shared presentation seams rather than continuing to grow those central files.
Needs next: continue compact-lane work from the Anime lane-local extracted view and reuse the documented "inert shell first, one live section at a time" pattern before attempting another lane
Needs human: none
Notes: Added a dedicated Anime compact-lane playbook documenting the proven extracted-view seam, the existing shared `SliderFlyout` compact-trigger seam, the current file map, which central files are protected, and the recommended path for future compact-lane expansion without blowing up the fragile host/viewmodel surfaces.
HANDOFF: status=done task="document Anime compact lane playbook" files="docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="use the Anime compact playbook to continue lane-local compact work without growing MainWindow.xaml/MainWindowViewModel.cs further"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Document the subject-card compact visibility seam as a temporary Anime compact host workaround
Status: done
Files changed: docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: `ShowSubjectContextFields` is host-owned and Anime-specific. It hides `Action` and `Relationship / Interaction` in compact Anime mode while leaving `Subject` visible, but hidden `Action` and `Relationship` values remain in state and can still affect prompt output.
Needs next: do not copy `ShowSubjectContextFields` as the rollout pattern for future compact lanes. Leave subject/context behavior host-owned until a later workstation or upper-shelf product decision explicitly resolves it.
Needs human: decide later whether global compact mode should hide Action/Relationship, whether hidden values need visible warning, and whether subject/context inputs belong to a future upper workstation shelf.
Notes: Added a direct warning to the Anime compact lane playbook that the subject-card seam is not a clean lane-local compact-spine pattern. It is a temporary Anime compact workaround and should not be generalized during compact-lane rollout.
HANDOFF: status=done task="document subject-card compact seam warning" files="docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="keep subject/context compact behavior unresolved and host-owned until explicitly decided"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Document future compact workstation shelf planning boundaries
Status: done
Files changed: docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation/planning-only pass
Observed risks: Upper and lower compact workstation shelves are host-level surfaces, not lane-local compact spine work. The prompt preview, lane-adjacent steering panel, actions/presets, and status regions are at risk of being incorrectly folded into Anime compact views if future work skips the boundary read.
Needs next: keep shelf work separate from lane-local compact-spine extraction. Do not implement shelves inside `AnimeCompactManualStack.xaml`, compact Artist Influence views, or future lane-local compact files.
Needs human: later decide the exact collapsed/expanded shelf behavior, whether subject/context inputs belong in the upper shelf, and how light/floating/repositionable the final workstation shell should become.
Notes: Added `docs/COMPACT_WORKSTATION_SHELF_PLAN.md` to record that the upper shelf should likely preserve highest-value steering controls such as Anime Style/Era in collapsed form, while the lower shelf should likely preserve Copy Prompt. The doc explicitly keeps subject/context unresolved and warns not to silently fold it into shelf implementation.
HANDOFF: status=done task="document compact workstation shelf plan" files="docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="use the shelf plan before any host-level compact workstation implementation"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Document global compact-mode direction and compact UI persistence posture
Status: done
Files changed: docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: Current Anime compact routing is still prototype-stage and should not be mistaken for the final global compact-mode design. Compact UI persistence is intentionally raw key/value UI state and should not be merged into prompt config, presets, savestates, or semantic behavior.
Needs next: future compact-spine rollout and shelf planning should reason from compact mode as a future global presentation mode above Intent, while still avoiding implementation until a narrow host seam is approved.
Needs human: later decide the actual global compact switch/host seam and only reconsider typed compact persistence if multiple compact lanes plus shelf surfaces make raw keys insufficient.
Notes: Added explicit shelf-plan sections for global compact-mode direction and compact UI persistence posture. Recorded that Intent continues to choose the lane, compact mode changes the presentation/workstation regime, stable namespaced keys are preferred, and typed compact persistence is deferred.
HANDOFF: status=done task="document global compact roadmap and persistence posture" files="docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="keep compact persistence raw key/value and keep global compact mode as roadmap direction until explicitly implemented"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Checkpoint the inert host-level compact workstation shelf shell study
Status: done
Files changed: docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: The current upper/lower shelf shells are inert host-level studies only. They do not implement expand/collapse behavior, persistence, hit testing, commands, drag behavior, or workstation-mode switching. Final geometry, scroll overlap, final anchors, live affordance behavior, persistence, and repositionable/floating workstation behavior are still unproven.
Needs next: do not do blind shelf polish or transition the shell study into live shelf implementation without human visual review in the running app and a new explicit task.
Needs human: visually inspect the current shell study in-app before approving further geometry refinement or any live shelf behavior.
Notes: Added the shelf-plan checkpoint language recording that the current host seam is acceptable as an inert study seam, that the shell study must remain outside Anime compact spine files, and that upper/lower shelves remain distinct host-level workstation surfaces.
HANDOFF: status=done task="checkpoint inert workstation shelf shell study" files="docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="wait for human visual review before further shell refinement or shelf implementation"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Freeze the blunt outward compact workstation shelf shell checkpoint
Status: done
Files changed: docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: The current shelf study is still inert and host-level. The preferred visual direction is now outward projection from the compact-spine edge with a blunter, less pointed latch silhouette. Geometry iteration is intentionally paused here.
Needs next: do not continue shell-only shelf polish. Reopen shelf geometry only when future live-control mapping work creates real requirements, such as mapping live upper-shelf dropdowns/controls, mapping lower-shelf action controls, validating clipping/space with real interactive content, or designing approved expand-affordance behavior.
Needs human: later approve the live-control mapping phase before any further shelf geometry or behavior work.
Notes: Updated the shelf plan to freeze the blunt outward shell as the active checkpoint and explicitly defer further geometry work until live control mapping resumes.
HANDOFF: status=done task="freeze blunt outward shelf checkpoint" files="docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="leave shelf geometry frozen until live-control mapping work resumes"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Document compact workstation nameplate-shell direction
Status: done
Files changed: docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: The future compact workstation should not be treated as a squeezed sidebar or as lane-local compact spine material. The closed state is now understood as the branded nameplate/header shell, with the open workstation later unfolding from that shell over the user's workspace. This is planning direction only, not implementation approval.
Needs next: reason about any future global compact-mode selector as part of the header/nameplate shell rather than burying it inside the Intent card. Keep upper/lower shelves as later host-level unfolding surfaces, not independent lane-local add-ons.
Needs human: later approve any actual nameplate-shell placement, compact selector behavior, origami/unfolding behavior, or floating/repositionable workstation implementation.
Notes: Added shelf-plan language distinguishing the future nameplate-shell model from the current inert shelf study and from the current full-window/sidebar layout.
HANDOFF: status=done task="document compact workstation nameplate shell" files="docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="keep nameplate-shell/origami workstation as planning direction until explicitly implemented"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Document preferred advanced Compact host style
Status: done
Files changed: docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: Compact remains the broad global presentation/workstation mode above Intent. The preferred first advanced host style is now documented as a floating always-on-top companion window, not overlay/click-through behavior. This is planning direction only and does not approve implementation.
Needs next: keep current in-window compact extraction boundaries unchanged. Do not rename Compact mode to Overlay, do not treat overlay as the first target, and do not over-specify drag, position persistence, or window lifecycle until a dedicated implementation pass exists.
Needs human: later approve an actual floating companion-window implementation pass and any unresolved window-management details.
Notes: Updated the shelf plan to state that the floating companion window should inherit from the nameplate-shell/unfolding-workstation model, while the current shelf study remains relevant as later host-level material.
HANDOFF: status=done task="document floating companion compact host direction" files="docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat floating companion window as planning direction, with overlay/click-through deferred"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Document shared-standard compact adaptation checklist
Status: done
Files changed: docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: Cinematic currently proves ordinary/shared-standard compact content seams, including right-side compact presenter routing, left-side compact manual-stack rendering, and reuse of existing shared-standard lane state. It does not yet prove Anime-style compact collapse/compression behavior, so it must not be treated as the final shared-standard compact template.
Needs next: use the shared-standard checklist before compacting another ordinary lane. Keep compact content rendering separate from collapse/compression behavior, and do not copy Anime-local ownership assumptions into shared-standard lanes.
Needs human: later decide when a second ordinary lane should prove the pattern and whether Anime-style collapse/compression is intended for that lane.
Notes: Added `docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md` with the two-layer rule, source-of-truth rule, Anime-copy rule, collapse wiring rule, readiness rule, and proof-before-abstraction rule. The checklist explicitly forbids compact-only selector collections, modifier pools, slider sources, or prompt fragment builders.
HANDOFF: status=done task="document shared-standard compact checklist" files="docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="consult checklist before adapting another ordinary/shared-standard lane"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Update compact rollout cheat sheet after corrected Cinematic adaptation
Status: done
Files changed: docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: The first Cinematic compact adaptation copied a stale/incorrect Anime compact shape. It temporarily placed aspect/output checkboxes in Scene Composition, joined Image Finish to Scene Composition, added the wrong gate rhythm, and used a temporary `cinematic/image-finish-scene-composition` key. The corrected pattern uses the current Anime compact state: Lighting exposed, Style Controls/Mood paired, Control Lighting/Image Finish paired, Scene Composition standalone with two internal columns, and subject compact mode showing Subject only.
Needs next: before adapting another ordinary/shared-standard lane, open the current `AnimeCompactManualStack.xaml` and read `docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md`. Assign section keys only after deciding which sections are exposed, paired, or standalone. Do not build a generic framework yet.
Needs human: later choose the next ordinary/shared-standard lane to prove the corrected pattern before abstraction.
Notes: Updated the checklist into a practical cheat sheet covering the latest Anime template snapshot, corrected Cinematic structure, compact gate rhythm, valid section keys, and mistakes to avoid. Added an Anime playbook cross-reference warning future Codex instances not to adapt compact lanes from memory or stale screenshots.
HANDOFF: status=done task="update compact rollout cheat sheet after Cinematic corrections" files="docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="use latest Anime XAML plus checklist before installing compact buttons/sections for another ordinary lane"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Implement Watercolor compact support as the second ordinary/shared-standard compact lane
Status: done
Files changed: PromptForge.App/MainWindow.xaml, PromptForge.App/ViewModels/MainWindowViewModel.cs, PromptForge.App/Views/LaneReplacements/Shared/WatercolorCompactPanel.xaml, PromptForge.App/Views/LaneReplacements/Shared/WatercolorCompactManualStack.xaml, PromptForge.App/Views/LaneReplacements/Shared/WatercolorCompactManualStack.xaml.cs
Tests run: `dotnet build PromptForge.App/PromptForge.App.csproj --no-restore -p:OutDir=bin\\CodexVerify\\`
Observed risks: The first implementation attempt almost missed one host-routing detail: adding `ShowCompact...` seams is not enough by itself if the existing shared standard-lane `ContentControl` is still bound to a broad standard-panel visibility property. Without an explicit suppression seam, the new compact presenter can coexist with the normal shared panel instead of replacing it cleanly.
Needs next: when compacting the next ordinary/shared-standard lane, audit compact routing as a two-sided problem:
1. what turns the new compact surfaces on
2. what turns the old shared standard panel off
Do not assume the standard shared panel disappears automatically just because the compact presenter exists.
Needs human: none
Notes: Watercolor now proves a second ordinary/shared-standard compact lane using the corrected Anime/Cinematic section rhythm. It also proved a host-routing lesson worth preserving: the right-side host panel needed an explicit resolved visibility seam (`ShowResolvedStandardLanePanel`) so Watercolor compact could suppress the normal shared panel while leaving other unsupported lanes on the standard fallback path. Added this lesson to the shared-standard compact checklist so future Codex passes do not repeat the same miss.
HANDOFF: status=done task="implement Watercolor compact support and document host suppression miss" files="PromptForge.App/MainWindow.xaml, PromptForge.App/ViewModels/MainWindowViewModel.cs, PromptForge.App/Views/LaneReplacements/Shared/WatercolorCompactPanel.xaml, PromptForge.App/Views/LaneReplacements/Shared/WatercolorCompactManualStack.xaml, PromptForge.App/Views/LaneReplacements/Shared/WatercolorCompactManualStack.xaml.cs, docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="for the next shared-standard compact lane, verify both the compact-positive routing seams and the standard-panel suppression seam before calling the rollout complete"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Record the compact subject/context rule as an intentional rollout rule rather than an accidental widening
Status: done
Files changed: docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: Older docs still described the subject/context seam as Anime-specific and unresolved, which created a drift risk: a future Codex could wrongly "fix" `ShowSubjectContextFields` back into a per-lane exception list even though the current code now intentionally follows compact manual presentation for compact-capable lanes.
Needs next: future compact-capable lanes should assume `Subject`-only compact subject-card behavior unless a later lane explicitly proves it needs an exception. Do not revert this back to a lane-by-lane exception list unless the user explicitly asks.
Needs human: later decide only whether hidden subject-context values should remain prompt-active and whether the host-owned seam should eventually move into a cleaner workstation/host seam.
Notes: Updated the shared-standard checklist and Anime playbook so they now record the clarified rule: subject/context visibility follows compact manual presentation for compact-capable lanes. The old Anime-only subject/context seam is now documented as an earlier transitional workaround, not the desired long-term rollout pattern.
HANDOFF: status=done task="document intentional compact subject-context rule" files="docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat subject/context compact visibility as the current rollout rule for compact-capable lanes unless an explicit exception is approved"

### Turn
Date: 2026-04-20 local
Agent: Codex-Primary
Task: Consolidate compact-lane doctrine after multiple proven compact lanes
Status: done
Files changed: docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/COMPACT_LANE_ARCHITECTURE.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: The compact rollout now has enough proven lanes to create drift in two directions if the doctrine is not explicit:
1. future Codex instances may over-copy Anime ownership assumptions instead of only reusing the current compact shell rhythm
2. future Codex instances may treat current host routing/suppression seams as a finished generic framework even though `MainWindow.xaml` and `MainWindowViewModel.cs` remain pressure-zone transitional seams
Needs next: before compacting another lane, read the shared-standard checklist plus the new compact architecture doc. Keep compact presenters lane-local, reuse existing lane state, keep compact persistence UI-only, and treat workstation shelves/floating companion work as separate host-level phases.
Needs human: later decide whether any future lane needs approved lane-native compact labels beyond the current proven shared-standard rhythm, and when the transitional host routing seams should get a deliberate cleanup/normalization pass.
Notes: Consolidated the compact doctrine into a dedicated architecture note and refreshed the adaptation docs to reflect the current proof state:
- Anime remains the special compact prototype, not the universal ownership model
- Cinematic, Watercolor, Children's Book, and 3D Render prove the ordinary/shared-standard compact path
- Tattoo Art proves sparse-lane honesty
- preserve the helper box, not just title + description shell for lanes that have very little in their lane card
- subject/context visibility now intentionally follows compact manual presentation for compact-capable lanes
- compact persistence remains UI-only through `CompactSectionUiStateService`
- lane-local compact presenters are distinct from workstation shelves and from the future floating companion host
- the floating companion is planned as a second host/view over shared session state, not a second owner of lane state
HANDOFF: status=done task="consolidate compact-lane doctrine after multiple proven lanes" files="docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/ANIME_COMPACT_LANE_PLAYBOOK.md, docs/COMPACT_WORKSTATION_SHELF_PLAN.md, docs/COMPACT_LANE_ARCHITECTURE.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="use the compact architecture doc plus the shared-standard checklist before any further lane rollout or host-level compact work"

### Turn
Date: 2026-04-21 local
Agent: Codex-Primary
Task: Document standard-lane source card to compact group visual map for HoverDeck wrappers
Status: done
Files changed: docs/COMPACT_LANE_ARCHITECTURE.md, docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: Future HoverDeck wrapper work could misread compact proofs as generic manual-card compression and miss the actual card movement: `Manual Output` does not survive intact, `Mood` absorbs aspect/output toggles, `Scene Composition` owns composition/output sliders, and paired sections have a single right-hand `Hide` owner. A stray/extra `Hide` beside `Control Lighting` must be treated as a mistaken artifact, not as precedent.
Needs next: for the first HoverDeck wrapper implementation, use the documented source-card map before touching XAML. Preserve existing bindings/state and emulate the compact visual grouping without creating compact-only state or duplicate prompt behavior.
Needs human: choose the exact first wrapper target, likely one bounded standard-lane manual group, before implementation.
Notes: Added `Standard-Lane Source Card To Compact Group Map` to the compact architecture doc and cross-referenced it from the shared-standard checklist. The map records source card, expanded compact destination, collapsed ownership, button ownership, and mistakes to avoid for `Subject`, `Lighting`, `Manual Style Controls` + `Manual Mood`, `Manual Lighting and Color` + `Manual Image Finish`, `Manual Output`, `Scene Composition`, and the already-solved `Artist Influence` support card.
HANDOFF: status=done task="document HoverDeck compact visual source-card map" files="docs/COMPACT_LANE_ARCHITECTURE.md, docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="use the source-card map before implementing any HoverDeck standard-lane wrapper"

### Turn
Date: 2026-04-21 local
Agent: Codex-Primary
Task: Document fully collapsed HoverDeck top-state as the third compact presentation state
Status: done
Files changed: docs/COMPACT_LANE_ARCHITECTURE.md, docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: Future HoverDeck wrapper work could stop at expanded compact grouping and lose the final top-state behavior. The final HoverDeck top-state is not just smaller expanded cards: `Subject` stays visible with a host-owned Copy Prompt emitter seam, `Lighting` remains exposed/live, and the grouped sections collapse into rows using existing compact ownership/state.
Needs next: before implementing wrappers, preserve the three-state chain: source standard-lane cards -> expanded compact grouping -> fully collapsed HoverDeck rows. Keep the gold-touched middle-button affordance as visual expand/collapse language only, not new semantic ownership.
Needs human: approve the first implementation target after deciding which collapsed row/wrapper should be proven first.
Notes: Added `Third Presentation State: Fully Collapsed HoverDeck` to the compact architecture doc and a short warning in the shared-standard checklist. The documented top-state keeps `Subject`, `Lighting`, collapsed `Style Controls/Mood`, collapsed `Control Lighting/Image Finish`, collapsed `Scene Composition`, and collapsed `Artist Influence` as distinct presentation outcomes.
HANDOFF: status=done task="document fully collapsed HoverDeck top-state" files="docs/COMPACT_LANE_ARCHITECTURE.md, docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="preserve the three-state chain before implementing any HoverDeck wrapper"

### Turn
Date: 2026-04-21 local
Agent: Codex-Primary
Task: Remove visible Standard/Compact presentation-mode comboboxes from brand cards and document remaining wiring
Status: done
Files changed: PromptForge.App/MainWindow.xaml, PromptForge.App/Views/CompactWorkstation/HoverDeckCompactConsoleCard.xaml, docs/COMPACT_LANE_ARCHITECTURE.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `dotnet build PromptForge.App\PromptForge.App.csproj --no-restore -v:minimal`
Observed risks: The visible brand-card presentation-mode selectors are gone, but the underlying presentation-mode state and compact routing remain in `MainWindowViewModel.cs`. Future work should not mistake the UI removal for a state/model removal.
Needs next: if a presentation-mode control is needed later, reintroduce it deliberately in a new approved location. Inspect `StandardPresentationMode`, `CompactPresentationMode`, `PresentationModes`, `PresentationMode`, `IsCompactPresentationMode`, and `NormalizePresentationMode` in `PromptForge.App/ViewModels/MainWindowViewModel.cs`.
Needs human: decide whether any replacement presentation-mode entry point should exist and where it belongs.
Notes: Added `Presentation Mode Selector Visibility` to `docs/COMPACT_LANE_ARCHITECTURE.md` documenting that the main and HoverDeck brand-card comboboxes were removed from `MainWindow.xaml` and `HoverDeckCompactConsoleCard.xaml` only.
HANDOFF: status=done task="document presentation mode combobox removal" files="PromptForge.App/MainWindow.xaml, PromptForge.App/Views/CompactWorkstation/HoverDeckCompactConsoleCard.xaml, docs/COMPACT_LANE_ARCHITECTURE.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="do not assume presentation-mode wiring was removed; only the old visible brand-card selectors were removed"

### Turn
Date: 2026-04-21 local
Agent: Codex-Primary
Task: Install and document neutral-band standalone slider suppression for completed paired lanes
Status: done
Files changed: PromptForge.Core/Services/PromptBuilderService.cs, PromptForge.Core/Services/SliderLanguageCatalog.SemanticPairs.cs, docs/PAIR_GRID_INTERPRETATION_NOTES.md
Tests run: `dotnet build PromptForge.App/PromptForge.App.csproj --no-restore -p:OutDir=bin\\CodexVerify\\`
Observed risks: The rule is intentionally not universal. Placeholder pair files do not mean a lane is ready. Only lanes with completed, human-approved installed semantic pair maps should enter the neutral-band suppression allowlist.
Needs next: when a future lane's semantic pairing is completed, explicitly add that lane's pair-member slider keys to `InstalledSemanticPairSliderKeysByLane` and only then allow neutral-band standalone suppression for that lane.
Needs human: tell Codex when a lane is complete and ready for this rule. Do not infer readiness from the presence of a `*Pairs.cs` file alone.
Notes: Neutral-band suppression is prompt-emission only. For completed paired lanes, non-pair sliders in the 40-59 band suppress standalone slider-fragment emission. Pair-member sliders remain exempt and emit across the full range so pair-collapse input space stays intact. This must not mutate slider values, preset/config state, nudges/defaults, helper/guide text, diagnostics source visibility, or pair-collapse input space.
HANDOFF: status=done task="install neutral-band emission rule for completed paired lanes" files="PromptForge.Core/Services/PromptBuilderService.cs, PromptForge.Core/Services/SliderLanguageCatalog.SemanticPairs.cs, docs/PAIR_GRID_INTERPRETATION_NOTES.md" blockers="none" next="only add future lanes to the neutral-band suppression allowlist after human confirms that lane's pairing map is complete"

### Turn
Date: 2026-04-22 local
Agent: Codex-Primary
Task: Add release-clean local runtime-state backup helper and document the safety checkpoint
Status: done
Files changed: tools/release/Manage-LocalRuntimeState.ps1, RELEASING.md, docs/DEMO_LOCK_MEMO.md, docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: `tools/release/Manage-LocalRuntimeState.ps1 -Mode Backup` with escalation after sandbox denied AppData read access; no build run because this was tooling/docs only
Observed risks: Release-clean or first-run testing can destroy developer-local demo/license/lane/preset/UI-log state if done manually. The backup helper should be used before any such test, and clear/restore should be driven by its manifest rather than ad hoc AppData deletion.
Needs next: before converting the dev app into a shipping-clean state, run or confirm a fresh `-Mode Backup`, then use `ClearForReleaseTest` only with an explicit `-BackupPath`. After shipping/verification, restore from that same backup if the user's local dev state should come back.
Needs human: confirm when to run the clear-for-release-test mode; backup already exists.
Notes: Created `tools/release/Manage-LocalRuntimeState.ps1` and ran backup successfully. Current backup: `C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\artifacts\local-state-backups\PromptForgeLocalState_2026-04-22_15-08-45`. Manifest: `C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\artifacts\local-state-backups\PromptForgeLocalState_2026-04-22_15-08-45\manifest.json`.
HANDOFF: status=done task="document local runtime-state backup helper" files="tools/release/Manage-LocalRuntimeState.ps1, RELEASING.md, docs/DEMO_LOCK_MEMO.md, docs/CODEX_WORKSPACE_CONTEXT.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="use the helper before any release-clean local state clearing; do not manually delete app-data state"

### Turn
Date: 2026-04-24 local
Agent: Codex-Primary
Task: Install artist-influence doctrine doc and make it visible in public-facing repo docs without bundling it into release artifacts
Status: done
Files changed: docs/artist-influence-vs-imitation-doctrine.md, README.md, RELEASE_NOTES.md, RELEASING.md
Tests run: `dotnet build PromptForge.App\PromptForge.App.csproj --no-restore -p:OutDir=bin\\CodexVerify\\`
Observed risks: This doctrine is intentionally supplementary. It should stay visible in source/docs and product-facing repo surfaces, but it should not silently slip into the release-only tree or installer unless a later bounded pass explicitly stages it as a separate downloadable asset. Also note that the model-facing companion file (`PROMPT_FORGE_MODEL_SKILL.md` / `SKILL.md`) is still not installed because its approved source content was referenced through a missing sandbox attachment rather than a stable repo path.
Needs next: if the user wants the doctrine or a future model-skill companion to ship beside the app, do that as a separate bounded pass that explicitly stages supplementary download assets. Do not infer installer/release bundling from the presence of the doc in the source repo.
Needs human: provide the actual source text for `PROMPT_FORGE_MODEL_SKILL.md` if the model-facing companion resource should be installed next.
Notes: Added `docs/artist-influence-vs-imitation-doctrine.md` using the provided doctrine content, normalized the mojibake into clean Markdown, and added tiny cross-links in `README.md` and `RELEASE_NOTES.md` as `Supplementary doctrine: docs/artist-influence-vs-imitation-doctrine.md`. Added a release-safety reminder in `RELEASING.md` not to bundle this doctrine into the release-only tree or installer unless a later pass explicitly stages it as a separate supplementary download asset. No runtime behavior, prompt logic, or installer behavior changed in this pass.
HANDOFF: status=done task="install artist-influence doctrine doc and add tiny public-facing cross-links" files="docs/artist-influence-vs-imitation-doctrine.md, README.md, RELEASE_NOTES.md, RELEASING.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="model-skill source file still missing from stable repo/session path" next="keep the doctrine supplementary; install the separate model-skill companion only after its actual source text is available"

### Turn
Date: 2026-04-24 local
Agent: Codex-Primary
Task: Create dedicated HoverDeck wiring map doc for future session continuity
Status: done
Files changed: docs/HOVERDECK_WIRING_MAP.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md
Tests run: none; documentation-only pass
Observed risks: HoverDeck behavior now spans startup, host minimization, custom shell, brand-card wiring, compressed-body local projections, phrase-editor ownership, lock projections, and unlock-window ownership. Without a dedicated map, future sessions can easily touch the wrong seam or mistake a HoverDeck-local projection fix for a shared-state redesign.
Needs next: use `docs/HOVERDECK_WIRING_MAP.md` as the first-stop file before changing HoverDeck shell, brand card, compressed body, or popup ownership behavior.
Needs human: none.
Notes: Added a dedicated HoverDeck wiring map documenting the current host model, startup/shutdown path, primary files, shared-state versus HoverDeck-local seams, intent-ordering seam, lock projection seam, phrase-editor host ownership, unlock-window ownership, and practical "where do I touch it?" guidance.
HANDOFF: status=done task="add dedicated HoverDeck wiring map doc" files="docs/HOVERDECK_WIRING_MAP.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="open the HoverDeck wiring map first before future HoverDeck changes"
