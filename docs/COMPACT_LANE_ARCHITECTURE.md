# Compact Lane Architecture

This document records the current compact-lane architecture posture after multiple compact-capable lanes have been proven.

It exists to separate what is already proven from what is still transitional, and to keep future Codex work from rediscovering the same compact rollout traps.

## Current Compact Proof State

Current proven compact lanes:

- `Anime`
- `Cinematic`
- `Watercolor`
- `Children's Book`
- `3D Render`
- `Tattoo Art`

Current interpretation:

- `Anime` remains the special compact prototype
- `Cinematic` was the first ordinary/shared-standard compact proof
- `Watercolor`, `Children's Book`, and `3D Render` extended the ordinary/shared-standard proof
- `Tattoo Art` proved that a sparse lane can compact honestly without fake controls

Important warning:

- Anime remains the visual/behavior prototype
- Anime must not be copied blindly as the universal compact ownership model

## Shared Compact Presenter Rule

Compact presenters must reuse existing lane state.

Required posture:

- reuse existing selector state
- reuse existing modifier state
- reuse existing slider/manual state
- reuse existing helper/source pools
- reuse existing prompt-semantic paths

Do not create:

- compact-only selector collections
- compact-only modifier pools
- compact-only slider sources
- compact-only prompt builders
- compact-only semantic paths

Compact UI is a presentation layer over existing lane state, not a second logic path.

## Sparse-Lane Rule

A sparse lane must stay honest in compact mode.

Do not invent:

- fake selectors
- fake modifiers
- fake subdomains
- fake right-side control cards just to match richer lanes

`Tattoo Art` is the standing example.

What Tattoo proves:

- sparse control surface is still a real lane
- contributor-owned prompt descriptors do not automatically make the lane a host-special case
- the helper/instruction container on the right is the real lane panel
- compact right-side treatment should preserve that real helper content

Operational rule:

- preserve the helper box, not just title + description shell for lanes that have very little in their lane card

## Subject / Context Compact Rule

Subject/context visibility now intentionally follows compact manual presentation for compact-capable lanes.

Current rule:

- `Subject` remains visible
- `Action` is hidden
- `Relationship / Interaction` is hidden

Important boundary:

- this is an intentional current rollout rule
- this is no longer an unresolved accidental widening
- the older Anime-only version was a transitional workaround
- hidden values remain in state unless a later product decision changes that behavior

Future Codex work must not revert this into a per-lane exception list unless explicitly asked.

## Compact Persistence Rule

Compact section persistence remains UI-only.

Current persistence posture:

- `CompactSectionUiStateService` remains the small persistence seam
- compact section state uses stable namespaced keys
- compact UI persistence remains separate from prompt config
- compact UI persistence remains separate from presets
- compact UI persistence remains separate from savestates
- compact UI persistence remains separate from prompt semantics

This seam is intentionally small.

## Host Pressure-Zone Rule

`MainWindow.xaml` and `MainWindowViewModel.cs` are still pressure-zone host seams.

Current posture:

- explicit host routing and suppression seams are tolerated for now
- those seams are not the desired final generalized compact framework
- current lane rollout should prefer lane-local presenters with thin host seams

Do not:

- grow a generic compact framework inside those host files lane by lane
- mistake a currently working host suppression seam for final architecture

Future cleanup should happen only in a deliberate normalization pass.

## Lane-Local Versus Host-Level Work

Lane-local compact presenters and host-level compact workstation surfaces are different concerns.

Lane-local concern:

- compact manual stacks
- compact lane panels
- lane-bounded compact collapse/compression behavior

Host-level concern:

- workstation shelves
- future nameplate/header shell
- future floating compact companion host

Do not mix lane-local compact presenter work with workstation-shelf or floating-host implementation.

Future host-level compact work should reuse compact presenters rather than replace them.

## Floating Companion Architecture Rule

The future floating companion should be understood as a second host/view over shared session state.

That means:

- it is not a second owner of lane state
- it should emit user actions back into shared session state
- it should render only the compact-visible subset it cares about
- the main window and floating companion are two projections/controllers over the same underlying session state

The floating companion should own window state only, such as:

- position
- size
- always-on-top
- host-specific shell state if needed

Do not treat the floating companion as a second lane-state owner.

## Proven Grouping Rhythm

The current proven shared-standard compact rhythm is:

- `Lighting`
  - exposed live
  - no `Hide`
- paired section collapse where appropriate
  - one gate on the right-hand card
  - one collapsed shell for the paired unit
  - one persisted key for the paired unit
- `Scene Composition`
  - standalone collapse unit
  - internal `Composition` and `Output Controls` columns

This is a proven rhythm, not a license to flatten every lane into generic labels without lane honesty.

Lane-native labeling may still be approved later.

## Standard-Lane Source Card To Compact Group Map

This map records the real visual movement from standard-lane source cards into the current compact/HoverDeck grouping model. It is intended for future wrapper/projection work. It is not permission to create compact-only state, compact-only semantic pools, or duplicate prompt paths.

Use current compact proof behavior as the source of truth. Older screenshots, stale prompts, or early incorrect compact attempts should not override this map.

### `Subject`

Standard source card:

- `Subject`
- `Action`
- `Relationship / Interaction`

Compact destination:

- trims to a subject-only compact surface
- `Action` and `Relationship / Interaction` do not remain in the compact body
- hidden values remain in state unless a later product decision changes that behavior

Collapsed ownership:

- no current collapsed compact unit
- this is a visibility/compression rule, not a semantic-state deletion rule

Button ownership:

- `Copy Prompt` in HoverDeck's Subject header is host-level action projection
- `Copy Prompt` is not part of the core standard-source-card transformation rule

### `Lighting`

Standard source:

- `Lighting` selector inside `Manual Lighting and Color`

Compact destination:

- becomes its own exposed live compact card
- remains immediately available

Collapsed ownership:

- no collapse state
- no collapsed shell

Button ownership:

- no `Hide` button
- do not add a fake collapse gate for symmetry

### `Manual Style Controls` + `Manual Mood`

Standard source cards:

- `Manual Style Controls`
- `Manual Mood`
- non-composition pieces of `Manual Output`

Compact destination:

- becomes one paired compact unit:
  - `Style Controls`
  - `Mood`
- `Mood` also owns:
  - `Aspect Ratio`
  - `Print-ready`
  - `Transparent background`
  - `Include negative prompt`

Collapsed ownership:

- one shared collapsed state for both expanded cards
- one collapsed spanning shell labeled `Style Controls/Mood`
- one persisted compact UI key per lane for the paired unit

Button ownership:

- only one `Hide` button
- that button belongs on the right-hand `Mood` card
- no `Hide` button belongs on the left-hand `Style Controls` card

### `Manual Lighting and Color` + `Manual Image Finish`

Standard source cards:

- `Manual Lighting and Color`
- `Manual Image Finish`

Compact destination:

- becomes one paired compact unit:
  - `Control Lighting`
  - `Image Finish`

Collapsed ownership:

- one shared collapsed state for both expanded cards
- one collapsed spanning shell labeled `Control Lighting/Image Finish`
- one persisted compact UI key per lane for the paired unit

Button ownership:

- only one `Hide` button
- that button belongs on the right-hand `Image Finish` card
- any extra `Hide` button beside `Control Lighting` is a mistaken artifact and is not part of the intended rule

### `Manual Output`

Standard source card:

- `Aspect Ratio`
- `Print-ready`
- `Transparent background`
- `Include negative prompt`
- composition/output sliders:
  - `Framing`
  - `Camera Distance`
  - `Camera Angle`
  - `Background Complexity`
  - `Motion Energy`
  - `Atmospheric Depth`
  - `Chaos`

Compact destination:

- does not survive as its own compact card
- splits across:
  - `Mood` for `Aspect Ratio`, `Print-ready`, `Transparent background`, and `Include negative prompt`
  - `Scene Composition` for the composition/output sliders

Collapsed ownership:

- no `Manual Output` collapsed state exists as an intact card
- output checkbox/dropdown pieces follow the `Style Controls/Mood` collapsed unit because they live in `Mood`
- composition/output sliders follow the `Scene Composition` collapsed unit

Button ownership:

- no `Manual Output` button ownership remains
- do not recreate a `Manual Output` compact card just because it exists in the standard source layout

### `Scene Composition`

Standard source:

- composition/output sliders formerly grouped under old output/composition framing

Compact destination:

- standalone compact card named `Scene Composition`
- this is the better final compact name than the old `Manual Output`
- expanded internal columns:
  - `Composition`
    - `Framing`
    - `Camera Distance`
    - `Camera Angle`
    - `Background Complexity`
  - `Output Controls`
    - `Motion Energy`
    - `Atmospheric Depth`
    - `Chaos`

Collapsed ownership:

- one standalone collapsed state
- one collapsed spanning shell labeled `Scene Composition`
- one persisted compact UI key per lane for this unit

Button ownership:

- one `Hide` button
- the button belongs on `Scene Composition` itself

### `Artist Influence`

`Artist Influence` is already handled as a shared/global compact-support card. Do not reopen it as an unresolved source-card classification problem during standard-lane wrapper work.

Current rule:

- keep the existing shared compact-support behavior
- preserve its own `Reset` / `Hide` / collapsed-gate behavior
- do not absorb it into lane-local manual source-card transformations unless explicitly approved

### HoverDeck wrapper implication

HoverDeck should emulate this visual compression as a projection/wrapper over existing UI/session state. Future wrappers should preserve:

- source-card movement
- expanded compact destination
- collapsed ownership unit
- button ownership
- the fact that `Manual Output` is split, not preserved

Do not flatten the map into generic "manual controls" language that loses where controls actually moved.

## Third Presentation State: Fully Collapsed HoverDeck

The compact transformation chain now has three visual states:

1. source standard-lane cards
2. expanded compact grouping
3. fully collapsed HoverDeck top-state

Future HoverDeck wrapper/projection work must reason across the full chain. Do not stop at the source-card to expanded-compact map and assume the final HoverDeck top-state is just "smaller cards."

### Fully collapsed top-state map

`Subject`:

- remains visible in compressed subject form
- preserves the HoverDeck-only secondary `Copy Prompt` emitter
- the `Copy Prompt` emitter is host-owned injected behavior inside the subject shell
- subject compression must preserve the placement seam for that emitter
- do not treat the `Copy Prompt` button as part of the generic source-card transformation

`Lighting`:

- remains exposed and live
- does not join grouped collapsed button-row behavior
- has no hide/collapse ownership
- must not be folded into a grouped collapsed row for symmetry

`Style Controls/Mood`:

- fully collapses into one collapsed row
- uses one shared expand/collapse control
- inherits the paired compact unit ownership from `Style Controls` + `Mood`
- shares the same collapsed ownership/state already documented for the expanded paired unit

`Control Lighting/Image Finish`:

- fully collapses into one collapsed row
- uses one shared expand/collapse control
- inherits the paired compact unit ownership from `Control Lighting` + `Image Finish`
- shares the same collapsed ownership/state already documented for the expanded paired unit

`Scene Composition`:

- fully collapses into one collapsed row
- uses one shared expand/collapse control
- inherits its standalone compact unit ownership/state

`Artist Influence`:

- fully collapses into one collapsed row
- uses one shared expand/collapse control
- remains the already-solved shared/global compact-support surface
- do not reopen `Artist Influence` as a standard source-card classification problem

### Collapsed affordance rule

Grouped collapsed rows are operated by the same shared button language already used with the compact hide/collapse system.

The current gold-touched middle-button treatment is the shared expand/collapse affordance language for these collapsed grouped rows. It is visual/control language only. Do not reinterpret those controls as new semantic ownership, new prompt ownership, or compact-only state ownership.

### Transformation-chain rule

The intended chain is:

- source cards map to expanded compact groups
- expanded compact groups map to fully collapsed HoverDeck rows
- HoverDeck wrappers must preserve both mappings

Practical consequence:

- `Manual Output` is already split before the fully collapsed top-state
- `Lighting` remains exposed even when grouped cards collapse
- `Subject` remains visible and must preserve the HoverDeck-only copy-emitter seam
- collapsed grouped rows represent existing compact ownership units, not new semantic objects

## Transitional-Seam Warning

The current right-side compact routing/suppression seam is still transitional.

Current truth:

- current seams work
- current seams are not final normalized architecture
- no generic compact framework should be declared finished yet

Future cleanup or normalization should happen only in a deliberate follow-on phase, not opportunistically during ordinary lane rollout.

## Presentation Mode Selector Visibility

The visible `Standard` / `Compact` presentation-mode combobox has been intentionally removed from both brand/header card hosts:

- main window brand card: `PromptForge.App/MainWindow.xaml`
- HoverDeck brand card: `PromptForge.App/Views/CompactWorkstation/HoverDeckCompactConsoleCard.xaml`

This was a UI-surface removal only. The underlying presentation-mode wiring remains in the app and should not be treated as deleted.

Current places to inspect if the selector or mode behavior is needed later:

- `PromptForge.App/ViewModels/MainWindowViewModel.cs`
  - `StandardPresentationMode`
  - `CompactPresentationMode`
  - `PresentationModes`
  - `PresentationMode`
  - `IsCompactPresentationMode`
  - `NormalizePresentationMode`
- compact visibility/routing properties in `MainWindowViewModel.cs` that still depend on `IsCompactPresentationMode`
- compact presenter visibility bindings in `PromptForge.App/MainWindow.xaml`

Future work may reintroduce a presentation-mode control in a new location, but should not assume the old brand-card combobox is still present.
