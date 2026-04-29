# Prompt Forge Lane Insertion Checklist

Use this checklist for every new lane or semantic pack added to Prompt Forge.

Do not treat lane insertion as a one-file metadata task. In this repo, lane work is only complete when registration, UI/state, prompt routing, defaults, and phrase authority all agree.

## 1. Classify The Lane First

Before writing code, decide which kind of lane this is:

- ordinary shared-lane candidate
- explicit UI lane
- explicit prompt-contributor lane
- special-policy lane

Do not infer this casually from importance or aesthetics. Shared-lane eligibility is a structural decision.

Questions to answer first:

- Can the lane fit the shared sidecar/panel shape?
- Does it need custom suppression behavior?
- Does it need custom early descriptor assembly?
- Does it need explicit UI that the shared template cannot express?
- Does it need a non-default policy hook?

If any of that is unclear, stop and decide classification before installing the lane.

## 2. Register The Lane Shape

Author the lane metadata in:

- [LaneRegistry.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/LaneRegistry.cs)

Checklist:

- add the lane to `Definitions`
- choose a stable `Id`
- add `IntentNames`
- write `Summary`
- write `AnchorLabel`
- define panel title/help text
- define subtype selectors if needed
- define modifiers if needed
- define weight groups
- define `Defaults`
- set `ModifierCap`
- set `BehaviorFlags`
- set `PolicyKey` only if a real policy exception is required

Rules:

- keep phrase logic out of metadata
- keep compression logic out of metadata
- use subtype `DefaultNudges` only for posture/default shifts

## 3. Add Intent Identity

Wire the intent identity in:

- [IntentModeCatalog.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/IntentModeCatalog.cs)

Checklist:

- add intent name constant(s)
- add `IntentModeDefinition`
- add helper predicate like `Is<MyLane>()` if needed
- add `TryGet(...)` support if aliasing or intent family logic is needed
- add the intent to `Names` if it should appear in the main intent UI

Rules:

- `IntentModeCatalog` owns UI list presence and high-level summary/default identity
- do not confuse this file with phrase-authority surfaces
- if a lane should exist but not appear in the main dropdown, keep the definition but do not expose it through `Names`

## 4. Add Phrase Authority

Author lane-native semantics in:

- lane-specific `SliderLanguageCatalog.*.cs`
- and shared helpers in [SliderLanguageCatalog.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/SliderLanguageCatalog.cs) only when truly shared

Checklist:

- add `Resolve<MyLane>Descriptors(...)`
- add subtype descriptor resolution
- add modifier descriptor resolution
- add lane-specific `Resolve<MyLane>Phrase(...)`
- add lane-specific guide/help text resolution
- add lighting mapping if the lane owns a special lighting translation

Rules:

- `SliderLanguageCatalog` is phrase authority
- keep lane-root wording restrained
- check actual stacked prompt output, not just isolated phrases
- do not move phrase banks into registry metadata

## 5. Add Prompt Assembly Routing

Wire prompt composition in:

- [PromptBuilderService.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/PromptBuilderService.cs)

Checklist:

- decide whether lane uses standard section routing or explicit lane-contributor routing
- add intent checks in `BuildStandardPrompt(...)` if needed
- add builder section method if the lane contributes dedicated early descriptors
- add lighting/color routing support if lane owns a custom path there
- add negative prompt exceptions only if truly required

Rules:

- do not casually change assembly order
- special contributors belong in code, not metadata

## 6. Decide Shared Panel Vs Explicit UI

Shared panel path uses:

- [StandardLanePanelViewModels.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/StandardLanePanelViewModels.cs)
- [MainWindow.xaml](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/MainWindow.xaml)
- [MainWindowViewModel.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/MainWindowViewModel.cs)

If shared lane:

- confirm the lane fits the existing standard-lane template
- add it to `BuildSharedLanePanels()`
- add it to `StandardLaneBindingValidator`
- make sure `ActiveStandardLanePanel` can render it normally

If explicit lane:

- add explicit visibility properties if needed
- add explicit XAML sections/panels
- add explicit controls and bindings

Rules:

- do not force unusual lanes into the shared path just because the path exists
- shared template assumptions still matter

## 7. Add ViewModel State And Defaults

Main state surface:

- [MainWindowViewModel.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/MainWindowViewModel.cs)

Checklist:

- add backing fields
- add public properties
- add subtype/modifier setter wiring
- add `Apply<MyLane>IntentDefaults()` if lane needs explicit default initialization
- trigger it from `IntentMode` setter
- add summary/visibility support
- add panel exposure if shared or explicit UI requires it

Rules:

- intent switching is fragile linked behavior
- do not change only the property declarations and forget the switch/default path

## 8. Preserve Round-Trip Surfaces

Every lane must survive:

- capture
- apply
- reset
- preset save/load
- clone/copy

Check these surfaces:

- [PromptConfiguration.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/Models/PromptConfiguration.cs)
- `CaptureConfiguration()` in [MainWindowViewModel.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/MainWindowViewModel.cs)
- `ApplyConfiguration(...)` in [MainWindowViewModel.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/MainWindowViewModel.cs)
- `Reset()` in [MainWindowViewModel.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/MainWindowViewModel.cs)
- [StandardLaneStateAdapter.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/StandardLaneStateAdapter.cs)

Checklist:

- add config properties
- ensure clone/copy preserves them
- ensure reset initializes them correctly
- ensure standard-lane state capture/hydration handles them if shared
- ensure preset round-trip restores them exactly

## 9. Add Validation Coverage

Validation surfaces:

- [LaneRegistryValidator.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/LaneRegistryValidator.cs)
- [StandardLaneBindingValidator.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/StandardLaneBindingValidator.cs)
- [LaneRegressionHarness.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/LaneRegressionHarness.cs)

Checklist:

- confirm registry validation passes
- add shared-lane binding validation coverage if applicable
- add or extend regression harness coverage if the lane belongs in automated prompt checks

## 10. Wire Slider Phrase Preview Surfaces

The prompt-preview helper text path is separate from builder output.

Check:

- `Resolve<MyLane>Phrase(...)` usage in [MainWindowViewModel.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/MainWindowViewModel.cs)
- `Resolve<MyLane>GuideText(...)` usage in [MainWindowViewModel.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/MainWindowViewModel.cs)

Checklist:

- slider flyouts show lane-native phrase previews
- guide text resolves to lane-native guidance
- helper text stays aligned with lane meaning

## 11. Verify Hidden Product Contracts

Check these before calling the lane done:

- prompt preview updates immediately
- subtype change visibly changes posture if intended
- modifiers round-trip through presets
- compression does not destroy lane meaning
- repeated root words are restrained
- lane summary reads correctly
- intent dropdown exposure is intentional
- demo/license visibility rules still behave correctly if the lane is gated or special

## 12. Final Review Questions

Ask these at the end:

- Did we classify the lane correctly?
- Did we wire all four layers: registration, state, UI, prompt routing?
- Did we keep phrase logic in code and metadata clean?
- Did we preserve capture/apply/reset/preset round-trips?
- Does the lane read well before compression?
- Does the lane still read well after compression?
- Does the lane feel distinct from nearby lanes?

## Lane Insertion Minimum Files To Inspect

For every new lane, inspect at least:

- [LaneRegistry.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/LaneRegistry.cs)
- [IntentModeCatalog.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/IntentModeCatalog.cs)
- [PromptBuilderService.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/PromptBuilderService.cs)
- [SliderLanguageCatalog.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/SliderLanguageCatalog.cs)
- the lane-specific `SliderLanguageCatalog.*.cs` file
- [MainWindowViewModel.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/MainWindowViewModel.cs)
- [PromptConfiguration.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/Models/PromptConfiguration.cs)
- [MainWindow.xaml](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/MainWindow.xaml)

If shared lane:

- [StandardLanePanelViewModels.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/StandardLanePanelViewModels.cs)
- [StandardLaneBindingValidator.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.App/ViewModels/StandardLaneBindingValidator.cs)
- [StandardLaneStateAdapter.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/StandardLaneStateAdapter.cs)

If explicit prompt-contributor lane:

- [LanePromptContributorRegistry.cs](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/Lanes/LanePromptContributorRegistry.cs)
- the lane file in [PromptForge.Core/Services/Lanes](C:/Users/windy/OneDrive/Desktop/codex/Prompt Forge/PromptForge.Core/Services/Lanes)

## Working Rule

When adding a lane, use this order:

1. classify
2. register
3. phrase-authority
4. prompt routing
5. UI/state
6. round-trip
7. validation
8. taste review

If any of those remain unwired, the lane is not done.
