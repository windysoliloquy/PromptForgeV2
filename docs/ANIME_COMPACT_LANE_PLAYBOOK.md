# Anime Compact Lane Playbook

This document records the compact Anime lane work that has already been proven, what should be treated as the safe continuation path, and which files should be avoided unless a tiny host hook is genuinely required.

The purpose is continuity: another Codex instance should be able to extend the compact-lane work without rediscovering the same fragile seams or pushing more logic into the central runtime files than necessary.

## What Is Already Proven

The following are already working seams:

- Anime has a UI-only compact toggle:
  - `AnimeCompactPanelEnabled`
- Anime standard vs compact right-side panel switching works as a proof of switch mechanism only
- Anime compact left-side manual-stack replacement works through an extracted lane-local view
- Anime compact `Manual Image Finish` is live and reuses the existing shared slider state/binding path
- shared `SliderFlyout` now has an opt-in compact trigger presentation seam:
  - `UseCompactChipTrigger`

The compact-lane work is therefore no longer just a mockup. It has:

- a working visibility switch
- a working lane-local extracted view file
- a proven compact live-control hydration example
- a shared control seam that can be reused selectively

Important scope boundary:

- Anime remains the special compact prototype
- Anime is the source-of-truth for current compact shell feel and section rhythm
- Anime is not the universal ownership model for all compact lanes
- future ordinary/shared-standard compact lanes should adapt the current Anime rhythm without copying Anime-specific host ownership assumptions

## Current File Map

### Fragile host / routing surfaces

These were touched to get the experiment working, but should now be treated as protected:

- [PromptForge.App/MainWindow.xaml](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\MainWindow.xaml)
- [PromptForge.App/ViewModels/MainWindowViewModel.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\ViewModels\MainWindowViewModel.cs)

These files currently contain the Anime compact entry hooks:

- `AnimeCompactPanelEnabled`
- `ShowStandardAnimePanel`
- `ShowCompactAnimePanel`
- `ShowStandardManualStyleControlsCard`
- `ShowCompactAnimeManualStyleControlsCard`
- `ShowStandardManualMoodCard`
- `ShowCompactAnimeManualMoodCard`
- `ShowStandardManualLightingAndColorCard`
- `ShowCompactAnimeManualLightingAndColorCard`
- `ShowStandardManualImageFinishCard`
- `ShowCompactAnimeManualImageFinishCard`
- `ShowStandardManualOutputCard`
- `ShowCompactAnimeManualOutputCard`

Those hooks are already enough for the current Anime prototype. Do not keep multiplying this pattern lane after lane in `MainWindowViewModel.cs`.

### Proven lane-local surface

Anime compact work should now primarily live here:

- [PromptForge.App/Views/LaneReplacements/Anime/AnimeCompactManualStack.xaml](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Views\LaneReplacements\Anime\AnimeCompactManualStack.xaml)
- [PromptForge.App/Views/LaneReplacements/Anime/AnimeCompactManualStack.xaml.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Views\LaneReplacements\Anime\AnimeCompactManualStack.xaml.cs)

This extracted view is the correct place for:

- compact Anime placeholder layout work
- compact Anime live-control hydration work
- compact Anime presentation refinements

This extracted view is not the place for:

- prompt generation logic
- config capture/apply/reset logic
- lane semantics
- new application-wide routing policy

### Shared control seam already introduced

One narrow shared control seam was added and proven:

- [PromptForge.App/Controls/SliderFlyout.xaml](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Controls\SliderFlyout.xaml)
- [PromptForge.App/Controls/SliderFlyout.xaml.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Controls\SliderFlyout.xaml.cs)

Added property:

- `UseCompactChipTrigger`

Purpose:

- compact closed-trigger presentation only
- hide internal trigger body label text
- support chip-only compact trigger appearance
- preserve existing flyout behavior and popup content

This seam is safe to reuse selectively if a compact lane needs the same closed-trigger presentation.

It should not be widened casually into a full slider-system redesign.

## What The Anime Compact Stack Currently Contains

The extracted Anime compact manual stack currently covers:

- `Manual Style Controls`
- `Manual Mood`
- `Manual Lighting and Color`
- `Manual Image Finish`
- `Manual Output`

Current state by section:

- `Manual Style Controls`
  - placeholder only
  - compact row/shell presentation
- `Manual Mood`
  - placeholder only
  - compact row/shell presentation
- `Manual Lighting and Color`
  - placeholder only
  - compact row/shell presentation
- `Manual Image Finish`
  - live
  - reuses existing bindings/state
  - uses `UseCompactChipTrigger="True"` for:
    - `Focus / Depth of Field`
    - `Image Cleanliness`
    - `Detail Density`
- `Manual Output`
  - placeholder only
  - compact row/shell presentation

Artist Influence was intentionally left out of compact-lane work after earlier instability and should stay out until explicitly revisited.

## Safe Way To Continue

The proven continuation pattern is:

1. Keep the host seam small
- If a compact lane already has a host hook in `MainWindow.xaml`, do not widen it unless absolutely necessary.
- Prefer lane-local work in extracted view files.
- Today, until a separate host or mode-routing system exists, a new compact lane still requires:
  - a lane-local extracted view file
  - a minimal host seam in `MainWindow.xaml`
  - usually a minimal visibility or state seam in `MainWindowViewModel.cs`

2. Keep compact-lane UI in per-lane files
- For Anime, continue in:
  - `Views/LaneReplacements/Anime/...`
- For future lanes, prefer:
  - `Views/LaneReplacements/<LaneName>/...`

3. Hydrate one compact section at a time
- pick one placeholder section
- bind it to the exact same VM properties the standard panel already uses
- keep all other sections inert
- verify before moving on

4. Reuse shared controls only through narrow opt-in seams
- If compact trigger presentation is needed, use or extend:
  - `UseCompactChipTrigger`
- If a new shared seam is needed, keep it opt-in and presentation-only.

5. Do not create a second logic path
- compact lane views must reuse existing state
- compact lane views must not own prompt/config behavior

Important baseline note:

- the compact right-side Anime panel and switch work proved the visibility-switch mechanism only
- it is not the preferred stylistic baseline for future compact lanes
- it is not the preferred structural baseline beyond proving that a compact versus standard toggle can be routed safely

## Temporary Subject-Card Compact Seam

Current project truth:

- the subject-card compact visibility seam is still host-owned in `MainWindow.xaml`
- it is driven by `ShowSubjectContextFields`
- the old Anime-only subject/context seam should now be understood as an earlier transitional workaround, not the desired long-term compact rule
- current intended rule for compact-capable lanes:
  - compact manual presentation hides `Action`
  - compact manual presentation hides `Relationship / Interaction`
  - `Subject` remains visible
- hidden `Action` and `Relationship` values remain in state
- hidden `Action` and `Relationship` values can still affect prompt output if they already contain text

Do not treat the earlier Anime-specific workaround as the compact-lane rollout pattern.

Why:

- the earlier Anime-only version was not a clean lane-local compact-spine seam
- it reaches into the shared subject/context input surface above the compact manual stack
- it began as an Anime compact host workaround before the product decision was clarified
- future rollout should now follow compact manual presentation for compact-capable lanes unless a later lane explicitly proves it needs an exception

This means:

- the current rule is broader than Anime
- future compact-capable lanes should inherit the compact-manual subject rule by default
- future Codex passes must not try to "correct" this back into a per-lane Anime-only exception list unless explicitly asked

Leave this seam host-owned for now unless a future task explicitly decides the subject/context compact behavior.

Unresolved decisions for later workstation planning:

- whether hidden values should remain prompt-active without visible warning
- whether subject/context inputs belong to a future upper workstation shelf
- whether this host-owned seam should later move into a cleaner compact host/workstation seam without changing the current behavior rule

## Recommended Next-Step Pattern

If another Codex instance continues Anime compact work, the preferred order is:

1. choose one remaining placeholder section
2. hydrate only that section
3. keep behavior on the same existing bindings
4. keep all untouched sections inert
5. verify build
6. only then move to the next section

Good candidates follow this rule better than broad redesign:

- one compact section at a time
- one shared presentation seam at a time
- one host adjustment only if clearly blocked

## Files To Avoid Unless Absolutely Needed

These are high-blast-radius files for this line of work:

- [PromptForge.App/ViewModels/MainWindowViewModel.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\ViewModels\MainWindowViewModel.cs)
- [PromptForge.App/MainWindow.xaml](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\MainWindow.xaml)
- [PromptForge.Core/Services/PromptBuilderService.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\PromptBuilderService.cs)
- [PromptForge.Core/Services/SliderLanguageCatalog.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\SliderLanguageCatalog.cs)
- [PromptForge.Core/Services/LaneRegistry.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.Core\Services\LaneRegistry.cs)

Treat these as:

- do not touch unless there is a small host hook or compile-safe entry seam that cannot be avoided

For Anime compact lane work specifically:

- do not add more Anime-specific booleans in `MainWindowViewModel.cs` unless the task is blocked without them
- do not add more large Anime-specific markup blocks back into `MainWindow.xaml`
- do not move compact-lane semantics into core services

## Files That Are Acceptable To Touch For This Work

These are the preferred working surfaces:

- [PromptForge.App/Views/LaneReplacements/Anime/AnimeCompactManualStack.xaml](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Views\LaneReplacements\Anime\AnimeCompactManualStack.xaml)
- [PromptForge.App/Views/LaneReplacements/Anime/AnimeCompactManualStack.xaml.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Views\LaneReplacements\Anime\AnimeCompactManualStack.xaml.cs) only if a tiny presentation-only code-behind hook is truly required
- [PromptForge.App/Controls/SliderFlyout.xaml](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Controls\SliderFlyout.xaml) only for narrow shared presentation seams
- [PromptForge.App/Controls/SliderFlyout.xaml.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\PromptForge.App\Controls\SliderFlyout.xaml.cs) only for compile-safe dependency property wiring or similarly small presentation seams

## What Not To Rebuild

Do not redo these already-proven decisions unless the user explicitly wants a redesign:

- extracted lane-local Anime compact view instead of keeping all compact markup in `MainWindow.xaml`
- compact-live hydration through existing bindings rather than a second logic path
- compact chip trigger through a shared opt-in property rather than Anime-only hacks
- keeping Artist Influence out of scope for now
- keeping standard Anime behavior intact alongside compact Anime behavior

## Working Rule For Future Compact Lanes

If another lane gets a compact replacement path, follow the Anime pattern:

1. tiny host seam only
2. lane-local extracted view file
3. inert shell first
4. one live section at a time
5. opt-in shared presentation seam only if needed
6. no semantic/path duplication

If a task starts to require broader host/viewmodel growth, stop and reassess before pushing more work into the protected central files.

Important distinction:

- follow the Anime pattern for compact shell rhythm and presentation sequencing
- do not follow Anime-specific host ownership rules as if they are universal compact doctrine
- for sparse lanes, preserve the real lane panel honestly instead of forcing Anime-shaped right-side controls

## Latest Anime Template For Shared-Standard Rollout

Before adapting another ordinary/shared-standard lane, read:

- [docs/SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\docs\SHARED_STANDARD_COMPACT_ADAPTATION_CHECKLIST.md)

That checklist now records the corrected Cinematic rollout against the latest Anime compact state.

Important guardrail:

- do not adapt from memory
- do not adapt from an earlier Anime compact screenshot or stale notes
- open the current `AnimeCompactManualStack.xaml` before copying section rhythm

Current section rhythm to preserve unless a future task changes it:

- `Lighting` remains exposed without a `Hide`
- `Style Controls` + `Mood` collapse together, with one `Hide` on `Mood`
- `Mood` owns aspect ratio and output checkboxes
- `Control Lighting` + `Image Finish` collapse together, with one `Hide` on `Image Finish`
- `Scene Composition` is a standalone spanning section with internal `Composition` and `Output Controls` columns
- paired sections collapse into one spanning shell and use one persisted key

The Cinematic pass proved this can be adapted to an ordinary/shared-standard lane, but also proved the failure mode: copying an older Anime shape creates wrong pairings, wrong gate placement, and misplaced output controls.
