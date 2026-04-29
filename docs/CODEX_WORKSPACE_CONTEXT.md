# Prompt Forge Workspace Context

This note is a companion to `docs/CODEX_SECOND_INSTANCE_HANDOFF.md`.
It does not override the handoff: behavior rules, provenance rules, product-safety rules, and anti-churn rules still live in `docs/CODEX_SECOND_INSTANCE_HANDOFF.md`, while this file is only for runtime/repo/context traps.

Purpose:

- preserve a code-grounded mental model of how Prompt Forge actually runs
- call out repo quirks that are easy to miss during refactors
- record a few practical gaps that were not explicit in the handoff

## Product impact posture

Prompt Forge should be treated as upstream influence, not just a local prompt toy.

If this app scales, it can affect:

- how users learn to compose prompts
- how much prompt bloat gets normalized
- how repetitive lane language spreads across image workflows
- how much unnecessary semantic load gets pushed into image models

That means product mistakes here do not stay local.

Practical implication for future Codex work:

- do not reward prompt inflation just because it feels richer
- do not install lanes that encourage noisy, bloated, or weakly differentiated prompt language
- do not weaken compression, dedupe, or phrase-discipline safeguards casually
- evaluate lane changes not only for correctness, but also for model-burden risk and user-behavior shaping

## Current snapshot

- Workspace root: `C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge`
- Current branch when this note was written: `codex/4.1.4-release`
- Branches present locally include `local/restore-source`, `main`, and several `codex/*` branches
- The handoff describes `local/restore-source` as the normal source branch and `main` as the public release branch, but the current working branch may differ during active work
- Prompt Forge is intentionally hardened around `.NET 8`; the repo root should carry `global.json` pinning SDK `8.0.419`, and any unexpected `.NET 10` selection should be treated as environment drift rather than a cue to modernize.
- In this Codex sandbox, restore can bypass the denied roaming `NuGet.Config` only by using an explicit local config file plus repo-local `DOTNET_*` / `NUGET_*` overrides with absolute paths; relative `NUGET_PACKAGES` is not reliable.
- The prior `PromptForge.App\bin\Debug\net8.0-windows\PromptForge.Core.dll` copy lock was caused by the running app; once Prompt Forge was closed, the app build succeeded again under the established Codex-only override lane.

Do not assume the current checkout matches the preferred release-flow branch described in docs. Re-check before doing anything branch-sensitive.

## Repo map

Primary code-bearing areas:

- `PromptForge.App/`
- `PromptForge.Core/`
- `PromptForge.LicenseTool/`
- `PromptForge.Diagnostics/`

Operational/script areas:

- `tools/installer/`
- `tools/release/`
- `tools/trusted-signing/`
- `tools/licensing/`

Large data / analysis areas:

- `PromptForge.Core/Data/`
- `docs/artist-pairs/`
- `quarantine/`

Generated / local-runtime areas:

- `artifacts/`
- `AppOutput/`
- `.dotnet-cli/`
- `.dotnet-home/`
- `.codex-temp/`

## Runtime composition

App startup is manual and direct in `PromptForge.App/App.xaml.cs`.

There is no DI container. `OnStartup` constructs services directly:

- `ArtistProfileService`
- `ArtistPairGuidanceService`
- `PromptBuilderService`
- `PresetStorageService`
- `ClipboardService`
- `ThemeService`
- `DemoStateService`
- `LicenseService`
- `MainWindowViewModel`
- `MainWindow`

That means constructor changes ripple quickly. A service signature change is not isolated by a container.

`MainWindowViewModel` is the center of gravity for runtime behavior. It owns:

- live prompt regeneration
- UI state
- preset capture/apply/reset
- lane panel wiring
- copy/export behavior
- demo and license state refresh
- experimental-mode bridge behavior

`MainWindow.xaml.cs` is not empty glue. It also owns:

- copy-button and action-card animations
- flyout refresh behavior when intent changes
- popup drag/clamp behavior
- unlock window launch and license refresh callback

## Important namespace / layout quirk

`PromptForge.Core/PromptForge.Core.csproj` sets:

- `RootNamespace` = `PromptForge.App`
- `AssemblyName` = `PromptForge.Core`

As a result, most core types live in `PromptForge.App.Models` and `PromptForge.App.Services`, even though they are compiled from the `PromptForge.Core` project.

This is unusual and easy to forget.

Practical implication:

- namespace alone does not tell you which project owns a type
- moving files or cleaning up namespaces casually can create misleading breakage

## Files present but intentionally not compiled

`PromptForge.App/PromptForge.App.csproj` explicitly removes several app-local files from compilation/content:

- `Models/ArtistProfile.cs`
- `Models/PromptConfiguration.cs`
- `Models/PromptResult.cs`
- `Services/ArtistProfileService.cs`
- `Services/IArtistProfileService.cs`
- `Services/IPromptBuilderService.cs`
- `Services/PromptBuilderService.cs`
- `Data/artist_profiles.json`

These names also exist in `PromptForge.Core` and are the active compiled versions.

Practical implication:

- some files under `PromptForge.App/Models` and `PromptForge.App/Services` are legacy shadows
- editing the wrong duplicate file can appear successful while changing nothing at runtime

Pre-edit rule: before patching any model/service/data file with a duplicate-looking name, confirm which project actually compiles it. Do this before editing, not after. Editing the wrong duplicate is a real no-op failure mode, not a cosmetic mistake.

## Prompt system mental model

Main prompt authority is code-driven, not metadata-driven.

The main execution path is:

1. `MainWindowViewModel` captures current UI state into `PromptConfiguration`
2. `PromptBuilderService.Build(...)` decides between experimental and standard paths
3. standard path assembles ordered prompt fragments
4. lane-specific descriptor sets come primarily from `SliderLanguageCatalog`
5. optional compression runs through `PromptCompressionService`
6. the final positive and negative prompts are surfaced back to the view model

Important authority boundaries:

- `IntentModeCatalog`: intent names, summary defaults, high-level lane identity
- `LaneRegistry`: lane metadata, selectors, modifiers, caps, defaults, policy association
- `SliderLanguageCatalog`: phrase authority and descriptor text
- `PromptBuilderService`: ordering/orchestration
- `PromptCompressionService`: dedupe/compression cleanup

The roadmap doc is accurate about the architecture stance: metadata declares lanes, code interprets them.

## Shared-lane vs explicit-lane model

Prompt Forge is in a hybrid state.

Explicit/special lanes:

- `Vintage Bend`
- `Comic Book`

Shared standard-lane container path:

- `Children's Book`
- `Cinematic`
- `Photography`
- `Product Photography`
- `Food Photography`
- `Architecture / Archviz`
- `3D Render`
- `Concept Art`
- `Pixel Art`
- `Watercolor`

Anime still has explicit properties on `PromptConfiguration` and `MainWindowViewModel`, but lane metadata also exists for it. Treat it carefully because it straddles old and new patterns.

The ordinary lane container is:

- `PromptConfiguration.StandardLaneStates`
- `StandardLaneStateCollection`
- `StandardLaneState`

Bridge logic lives in:

- `PromptForge.Core/Services/StandardLaneStateAdapter.cs`
- `PromptForge.App/ViewModels/StandardLanePanelViewModels.cs`
- `PromptForge.App/ViewModels/StandardLaneBindingValidator.cs`

This adapter layer preserves compatibility with older top-level properties. That compatibility layer is a real seam, not dead weight.

## Lane panel spacing and installation notes

This is one of the easiest UI areas to disturb when adding a lane.

### Current rendering shape

The active lane panel in the prompt-preview area is rendered inside a fixed right-side column in `PromptForge.App/MainWindow.xaml`.

Important current layout facts:

- the prompt preview card uses a 3-column grid
- the lane panel column is fixed at `230` width
- the standard-lane UI is injected through a single `ContentControl`
- that `ContentControl` always uses `StandardLanePanelTemplate`

The shared template hard-codes most spacing:

- description margin: `0,4,0,10`
- selector row container margin: `0,0,0,10`
- accent-title block has no extra bottom spacing of its own
- modifier list starts with `Margin="0,10,0,0"`
- checkbox style adds `Margin="0,8,0,0"`
- field-label style adds `Margin="0,9,0,4"`

So lane spacing is currently not metadata-authored. It is mostly the product of one template plus the shared text/control styles.

### Important mismatch already present

`LanePanelLayout` exists in lane metadata and is carried into `StandardLanePanelViewModel`, but the shared XAML template does not branch on it.

Practical meaning:

- `SingleColumn` vs `SplitColumns` currently has no effect for shared-lane rendering
- Anime declares `SplitColumns`, but Anime does not use the shared template; it has a bespoke manual XAML panel instead
- a future shared lane that expects `SplitColumns` will still render as a single vertical stack unless the XAML is extended

This is a real hidden footgun for lane installation.

### Why lane installs break spacing

A new lane can be logically valid yet still render poorly if it introduces any of these:

- too many selectors before modifiers
- unusually long selector labels
- unusually long option labels in a `230`-pixel column
- too many modifiers for a single vertical stack
- title / help text / accent labels that wrap heavily
- an expectation of two-column grouping that the template does not implement

The current standard-lane template assumes:

- compact prose
- one or two selectors at most
- a moderate count of checkbox modifiers
- acceptable wrapping inside a narrow sidebar card

If a lane violates those assumptions, the install may feel "broken" even if bindings and prompt logic are correct.

### Lane installation is distributed, not one-step

Adding a shared lane is not just a `LaneRegistry` edit.

Current shared-lane wiring is spread across:

- `PromptForge.Core/Services/LaneRegistry.cs`
- `PromptForge.Core/Services/StandardLaneStateAdapter.cs`
- `PromptForge.App/ViewModels/MainWindowViewModel.cs`
- `PromptForge.App/ViewModels/StandardLaneBindingValidator.cs`
- `PromptForge.Core/Services/LaneRegressionHarness.cs`
- `PromptForge.App/MainWindow.xaml`

Especially important:

- `BuildSharedLanePanels()` in `MainWindowViewModel` is a hard-coded allowlist
- `StandardLaneBindingValidator` has a hard-coded shared-lane intent list
- `LaneRegressionHarness` has its own hard-coded shared-lane test list

So a lane can be present in registry metadata but still be absent from:

- the render path
- binding validation
- regression coverage

### Safe posture for future lane installs

Before adding a lane to the shared path, verify all of the following:

1. The lane truly fits the ordinary shared-path shape rather than needing an explicit panel.
2. The lane content is compact enough for the `230`-pixel sidebar.
3. The lane does not rely on `SplitColumns`, conditional control visibility, or other metadata the current template ignores.
4. The lane is added to the shared-panel construction path, not just the registry.
5. The lane is added to binding validation and regression coverage.
6. The lane survives preset capture/apply/reset round-trips through `StandardLaneStateAdapter`.

If any of those are not true, the safer move is usually:

- keep the lane explicit for now, or
- extend the shared lane template deliberately before installing the lane

## Experimental mode

Experimental mode is a separate assembly path, not just a variant toggle.

Key files:

- `PromptForge.Core/Services/experimental.cs`
- `PromptForge.App/ViewModels/MainWindowViewModel.ExperimentalMacros.cs`

Observations:

- `experimental.cs` is very large and dense
- it contains prompt-governance rules, precedence, thresholds, suppression logic, assembly grouping, and macro mapping
- `PromptBuilderService` falls back to standard prompt assembly if experimental governance throws

Practical implication:

- experimental edits have a wide blast radius
- a "small cleanup" in that file is not small

## Persistence and machine-local state

Prompt Forge uses several machine-local storage locations outside the repo.

Preset storage:

- `%APPDATA%\PromptForge\Presets`

Theme persistence:

- `%APPDATA%\PromptForge\theme.txt`

Demo state:

- `%LOCALAPPDATA%\PromptForgeDemo\demo-state.json`

License state:

- `%LOCALAPPDATA%\PromptForgeDemo\license-state.json`

UI event log:

- `AppContext.BaseDirectory\ui-event-log.txt`

Release-clean backup helper:

- `tools\release\Manage-LocalRuntimeState.ps1`
- `-Mode Backup` creates a manifest-backed copy under `artifacts\local-state-backups\PromptForgeLocalState_<timestamp>`
- `-Mode ClearForReleaseTest` and `-Mode Restore` require an explicit `-BackupPath`
- use this helper before clearing any local app-data state for release-first-run testing

Important nuance:

- demo and license state share the same local directory name while demo mode is enabled
- `DemoModeOptions.IsDemoMode` is currently hard-coded `true`
- `LicenseStateDirectoryName` therefore resolves to `PromptForgeDemo`

## Licensing system

License validation and signing are intentionally simple and sensitive.

Key facts:

- unlock files are JSON serialized `PromptForgeLicense`
- signature payload is canonical text built from product name, normalized email, license id, and issued timestamp
- signing uses RSA PKCS#1 SHA-256
- app-side validation uses an embedded public key resource
- the license tool can generate unlock files and can also generate a new keypair, but keypair generation is operationally dangerous unless explicitly intended

Key files:

- `PromptForge.App/Services/LicenseService.cs`
- `PromptForge.Core/Services/PromptForgeLicenseCodec.cs`
- `PromptForge.LicenseTool/Program.cs`
- `PromptForge.Core/Data/promptforge-license-public.pem`

Important current behavior:

- unlock import attempts best-effort destruction of the imported file after successful activation
- activation state is persisted locally and revalidated on load
- `UnlockWindow` exposes the mailto purchase path and unlock import flow

## Release system

The release flow is opinionated and stricter than the rest of the app.

Key files:

- `RELEASING.md`
- `tools/installer/Build-SignedRelease.ps1`
- `tools/release/Prepare-PublicRelease.ps1`
- `tools/trusted-signing/Sign-Artifact.ps1`

Important details confirmed in code:

- release publish is `win-x64`
- release publish is self-contained
- release publish is not single-file
- `Build-SignedRelease.ps1` signs published binaries and then the installer
- the script validates that publish output is truly self-contained before packaging

This area should be treated as operational infrastructure, not normal build plumbing.

## Validation surfaces already in repo

There is meaningful built-in validation coverage:

- `PromptForge.Diagnostics/Program.cs`
- `LaneRegistryValidator`
- `StandardLaneBindingValidator`
- `LaneRegressionHarness`

The diagnostics utility checks:

- malformed lane metadata
- missing view-model bindings for shared lanes
- shared-lane preset/state round-trips
- repeated anchor descriptors
- subtype influence regressions
- modifier-cap regressions
- duplicate prompt fragments

This is useful context for future work: the project already has internal guardrails for lane work, so new lane changes should usually extend those checks rather than bypass them.

## Validation note from this exploration pass

I attempted to run `dotnet run --project PromptForge.Diagnostics` from this workspace.

Observed result:

- first attempt failed due sandbox first-time-use writes under `C:\Users\CodexSandboxOffline\.dotnet`
- second attempt, redirected to repo-local `.dotnet-cli` / `.dotnet-home`, still failed during restore/build with no surfaced compile error text
- installed SDKs on this machine include `.NET 8.0.419` and `.NET 10.0.201`
- future Codex work should respect the root `global.json` and treat `.NET 8.0.419` as the intended baseline even if `.NET 10.0.201` is also installed on the machine

This may be a local SDK/tooling mismatch rather than a project-code failure, but it means diagnostics are not trivially runnable in the current Codex sandbox session without extra environment work.

## Current Diagnostics Verification Boundary

Under the established Codex-only NuGet/config override lane, `PromptForge.Core`, `PromptForge.LicenseTool`, and `PromptForge.App` verify successfully.

`PromptForge.Diagnostics` and full solution verification remain the special/problematic surfaces. The remaining issue is concentrated in Diagnostics/App/Core graph evaluation during restore/build machinery, not ordinary compilation, and is not currently treated as a core product blocker.

Future Codex work should not restart SDK-drift, roaming-NuGet, or app-lock forensics unless new evidence appears. If Diagnostics investigation resumes later, start from the known project-graph/evaluation findings rather than from environment-drift theories.

## Small but useful practical notes

- `AppOutput/PromptForge` is a convenience post-build copy target, not the canonical release artifact
- `WHERE_TO_FIND_THE_APP.md` still points at a desktop path outside the current repo-root location; treat it as user guidance, not a guaranteed exact path
- `UiEventLog` resets on app startup and logs prompt-preview / intent-change activity to the app base directory
- the app has substantial animation and popup behavior in XAML/code-behind; UI changes are not purely view-model work
- the repo has artist-pair data tooling and reports, but those are not on the hot path for licensing/release safety

## Recommended posture for future Codex work

- always confirm which duplicate file is actually compiled before editing
- treat `MainWindowViewModel`, `PromptConfiguration`, `LaneRegistry`, `SliderLanguageCatalog`, and `experimental.cs` as high-blast-radius surfaces
- when touching lanes, think in terms of metadata + adapter + bindings + diagnostics, not just one file
- when touching licensing, inspect both app-side and tool-side code together
- when touching release behavior, inspect scripts and docs together and assume branch hygiene matters
