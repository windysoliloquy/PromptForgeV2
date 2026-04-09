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

- Local app/build/installer metadata should now target `5.0.0`, including [PromptForge.App/PromptForge.App.csproj](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\PromptForge.App.csproj), [tools/installer/PromptForge.iss](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\tools\installer\PromptForge.iss), and example commands in [RELEASING.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\RELEASING.md).
- Public-facing [RELEASE_NOTES.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\RELEASE_NOTES.md) should present the release as `Prompt Forge 5`.
- For local planning and Codex-to-Codex coordination, treat the active working line as `Version 5`.

Operational rule for future Codex work:

- If a task says `Version 5`, interpret that as the current local target and keep the main release/version surfaces aligned to `5.0.0` / `Prompt Forge 5`.
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
- `MaxDemoCopies = 150`
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
HANDOFF: status=done task="bump release surfaces to version 5" files="PromptForge.App/PromptForge.App.csproj, tools/installer/PromptForge.iss, RELEASING.md, RELEASE_NOTES.md, docs/CODEX_SECOND_INSTANCE_HANDOFF.md" blockers="none" next="treat 5.0.0 as the current release version for local packaging and signing work"

### Turn
Date: 2026-04-08 19:00 local
Agent: Codex-Primary
Task: Implement V5 machine-bound and Temporary licensing schema plus hard demo-expiry lock
Status: done
Files changed: PromptForge.App/ViewModels/MainWindowViewModel.cs, PromptForge.App/MainWindow.xaml, PromptForge.App/UnlockWindow.xaml, PromptForge.App/UnlockWindow.xaml.cs, PromptForge.App/Models/UnlockState.cs, PromptForge.App/Services/ILicenseService.cs, PromptForge.App/Services/LicenseService.cs, PromptForge.Core/Models/PromptForgeLicense.cs, PromptForge.Core/Models/PromptForgeLicenseModes.cs, PromptForge.Core/Services/PromptForgeLicenseCodec.cs, PromptForge.Core/Services/PromptForgeMachineBindingService.cs, PromptForge.LicenseTool/Program.cs, tools/licensing/Generate-UnlockFile-V5.ps1, tools/licensing/V5_GENERATOR_README.md, docs/V5_LICENSE_PLAN.md
Tests run: `dotnet build PromptForge.App/PromptForge.App.csproj --no-restore`; `dotnet build PromptForge.LicenseTool/PromptForge.LicenseTool.csproj --no-restore`; `dotnet run --project PromptForge.LicenseTool --no-build -- --show-request-code`; generated one Temporary and one MachineBound unlock under `.codex-temp/license-tests/`
Observed risks: Signed license modes and machine matching are now wired, but signed entitlement fields are not yet enforcing lane access. Local unlock persistence is still plain JSON rather than DPAPI-protected state.
Needs next: wire entitlement-profile and allowed-lane data into actual lane access rules, then decide whether to DPAPI-protect unlock and demo state before release packaging
Needs human: none
Notes: Demo expiry now hard-hides prompt output and authoring controls when copies reach zero. Unlock window now shows the local activation request code, and the V5 generator package now issues both Temporary and MachineBound unlocks for real.
HANDOFF: status=done task="implement V5 licensing modes and demo hard lock" files="PromptForge.App/ViewModels/MainWindowViewModel.cs, PromptForge.App/MainWindow.xaml, PromptForge.App/UnlockWindow.xaml, PromptForge.App/UnlockWindow.xaml.cs, PromptForge.App/Models/UnlockState.cs, PromptForge.App/Services/ILicenseService.cs, PromptForge.App/Services/LicenseService.cs, PromptForge.Core/Models/PromptForgeLicense.cs, PromptForge.Core/Models/PromptForgeLicenseModes.cs, PromptForge.Core/Services/PromptForgeLicenseCodec.cs, PromptForge.Core/Services/PromptForgeMachineBindingService.cs, PromptForge.LicenseTool/Program.cs, tools/licensing/Generate-UnlockFile-V5.ps1, tools/licensing/V5_GENERATOR_README.md, docs/V5_LICENSE_PLAN.md" blockers="lane entitlement enforcement and storage hardening still pending" next="wire license entitlements into lane access before release packaging"
