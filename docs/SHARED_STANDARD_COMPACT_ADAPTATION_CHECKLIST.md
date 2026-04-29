# Shared-Standard Compact Adaptation Checklist

This checklist applies to ordinary/shared-standard lanes only.

It does not apply to Comic Book, Vintage Bend, Experimental, or other special/exception-heavy lanes unless a later task explicitly reclassifies those surfaces for compact rollout.

## Current Proof State

`Anime` remains the special compact prototype.

Important boundary:

- Anime proved the first compact spine feel and section rhythm
- Anime must not be copied blindly as the universal ownership pattern
- Anime-specific host seams are not the universal compact architecture

`Cinematic` is the first ordinary/shared-standard lane beyond Anime to prove compact presentation seams.

Current proven ordinary/shared-standard compact lanes:

- `Cinematic`
- `Watercolor`
- `Children's Book`
- `3D Render`

Current sparse compact proof:

- `Tattoo Art`

What this now proves:

- right-side compact routing can be added lane by lane without changing prompt semantics
- left-side compact manual-stack rendering can reuse the existing shared state/binding path
- Anime-style compact collapse/compression behavior can be adapted to ordinary/shared-standard lanes
- a sparse lane can compact honestly without fake controls

What this does not prove yet:

- a finished generic compact framework
- that every lane should copy the same labels or grouping names
- that host routing seams are normalized

Operational posture:

- keep using the corrected Anime rhythm as the source-of-truth visual/behavior baseline
- keep ordinary/shared-standard rollout lane-bounded
- treat current host routing/suppression seams as transitional, not final architecture

## Current Anime Template Snapshot

Use the current `AnimeCompactManualStack.xaml` state as the visual/behavior template, not an earlier compact Anime memory.

Current Anime compact manual-stack shape:

- `Lighting`
  - exposed live compact shell
  - no fake collapsed gate just for symmetry
- `Style Controls` + `Mood`
  - expanded as two side-by-side cards
  - one `Hide` gate on the right-hand `Mood` card
  - collapsed as one spanning shell labeled `Style Controls/Mood`
  - `Mood` owns `Aspect Ratio`, `Print-ready`, `Transparent background`, and `Include negative prompt`
- `Control Lighting` + `Image Finish`
  - expanded as two side-by-side cards
  - one `Hide` gate on the right-hand `Image Finish` card
  - collapsed as one spanning shell labeled `Control Lighting/Image Finish`
- `Scene Composition`
  - single spanning card
  - one `Hide` gate on the card
  - expanded as two internal columns:
    - `Composition`
    - `Output Controls`
  - `Composition` contains framing/camera/background controls
  - `Output Controls` contains motion/atmosphere/chaos style controls
- `Artist Influence`
  - shared compact-support card, not Anime-owned
  - expanded state keeps `Reset` and `Hide`
  - collapsed state shows the compact gate only

Use this as the working template for ordinary/shared-standard compact lane rollouts unless a future task explicitly changes the Anime compact baseline.

## Standard Source Card To Compact Group Map

For wrapper/projection work, use the full map in `docs/COMPACT_LANE_ARCHITECTURE.md` under `Standard-Lane Source Card To Compact Group Map`.

Short form:

- `Subject` trims to a subject-only compact surface. `Action` and `Relationship / Interaction` are not in the compact body.
- `Lighting` is exposed live as its own compact card and has no `Hide` / collapsed ownership.
- `Manual Style Controls` + `Manual Mood` become the paired `Style Controls` + `Mood` unit. One `Hide` belongs on right-hand `Mood`; both collapse to `Style Controls/Mood`.
- `Mood` also owns the non-composition pieces of `Manual Output`: `Aspect Ratio`, `Print-ready`, `Transparent background`, and `Include negative prompt`.
- `Manual Lighting and Color` + `Manual Image Finish` become the paired `Control Lighting` + `Image Finish` unit. One `Hide` belongs on right-hand `Image Finish`; both collapse to `Control Lighting/Image Finish`.
- An extra `Hide` beside `Control Lighting` is a mistaken artifact, not the intended rule.
- `Manual Output` does not survive as its own compact card. It is split between `Mood` and `Scene Composition`.
- `Scene Composition` owns `Framing`, `Camera Distance`, `Camera Angle`, `Background Complexity`, `Motion Energy`, `Atmospheric Depth`, and `Chaos`, arranged as `Composition` and `Output Controls` columns.
- `Artist Influence` is already a solved shared/global compact-support card; do not reopen it as a standard source-card classification problem.

Future HoverDeck wrappers should emulate this visual compression while projecting existing bindings/state. Do not create compact-only semantic/source pools, duplicate button behavior, or a recreated compact `Manual Output` card.

## Fully Collapsed HoverDeck Top-State Warning

The expanded compact map above is not the final HoverDeck top-state.

Future wrapper/projection work must preserve the full three-state chain:

1. source standard-lane cards
2. expanded compact grouping
3. fully collapsed HoverDeck top-state

Short collapsed-state rules:

- `Subject` remains visible as compressed subject input and must preserve the HoverDeck-only `Copy Prompt` emitter seam. That emitter is host-owned injected behavior, not part of the generic source-card transformation.
- `Lighting` remains exposed and live. It does not join grouped collapsed button rows and owns no hide/collapse state.
- `Style Controls/Mood` collapses into one row with one shared expand/collapse control.
- `Control Lighting/Image Finish` collapses into one row with one shared expand/collapse control.
- `Scene Composition` collapses into one row with one shared expand/collapse control.
- `Artist Influence` collapses into one row with one shared expand/collapse control and remains the solved shared/global support surface.

The gold-touched middle-button treatment is the shared expand/collapse affordance language for these collapsed rows. It is not new semantic ownership and must not create compact-only state or prompt behavior.

## Cinematic Correction Cheat Sheet

Cinematic was the first ordinary/shared-standard compact lane adapted from Anime. The corrected shape matters because the first attempt exposed several easy mistakes.

Corrected Cinematic compact structure:

- `Lighting`
  - remains exposed
  - no `Hide` control
  - no collapsed shell
- `Style Controls` + `Mood`
  - one collapsible section
  - expanded as two cards
  - one `Hide` on `Mood`
  - collapsed shell label: `Style Controls/Mood`
  - `Mood` owns `Aspect Ratio`, `Print-ready`, `Transparent background`, and `Include negative prompt`
- `Control Lighting` + `Image Finish`
  - one collapsible section
  - expanded as two cards
  - one `Hide` on `Image Finish`
  - collapsed shell label: `Control Lighting/Image Finish`
- `Scene Composition`
  - one standalone collapsible section
  - one `Hide` on `Scene Composition`
  - expanded as a two-column internal layout:
    - `Composition`: `Framing`, `Camera Distance`, `Camera Angle`, `Background Complexity`
    - `Output Controls`: `Motion Energy`, `Atmospheric Depth`, `Chaos`
- subject card in Cinematic compact mode
  - show `Subject` only
  - hide `Action`
  - hide `Relationship / Interaction`
  - hidden values remain in state and may still affect prompt output

Corrected Cinematic compact section keys:

- `cinematic/style-mood`
- `cinematic/control-lighting-image-finish`
- `cinematic/scene-composition`

Do not reuse the temporary/mistaken key:

- `cinematic/image-finish-scene-composition`

Do not split paired sections into separate left/right collapsed shells.

If two expanded cards collapse as one unit, they need:

- one persisted section key
- one collapsed spanning shell
- one visible `Hide` on the right-hand expanded card only
- one collapsed gate button on the spanning shell

## Compact Gate Rhythm Cheat Sheet

Use this rhythm unless the user explicitly approves a different section structure:

- exposed live single shell:
  - no `Hide`
  - no collapsed shell
  - example: `Lighting`
- paired two-card section:
  - no gate on the left card
  - one `Hide` on the right card
  - one collapsed spanning shell
  - example: `Style Controls` + `Mood`
  - example: `Control Lighting` + `Image Finish`
- standalone spanning section:
  - one `Hide` on the section itself
  - one collapsed spanning shell
  - example: `Scene Composition`
- shared support card:
  - keep its own established expanded/collapsed controls
  - do not absorb it into lane-local manual stacks unless explicitly approved
  - example: `Artist Influence`

The visible `Show` text is intentionally hidden in current compact gate presentation and rendered as the animated `***` spark treatment.

Do not reintroduce visible `Open` / `Show` text unless a future visual pass explicitly asks for it.

## Sparse-Lane Honesty Rule

A sparse lane must stay honest in compact mode.

Do not:

- invent subtype selectors
- invent modifiers
- invent subdomains
- invent fake right-side control shells just to make the lane look symmetrical

A minimal informational right-side shell is legitimate when that is the real lane shape.

Standing example:

- `Tattoo Art`

Current Tattoo compact lesson:

- sparse control surface is real lane shape, not missing content
- contributor-owned prompt descriptors do not make the lane structurally special enough to justify fake compact controls
- the helper/instruction container on the right is the real lane panel
- compact Tattoo should preserve that helper/instruction container rather than collapsing it into a fake generic side panel

Operational warning:

- preserve the helper box, not just title + description shell for lanes that have very little in their lane card

## Mistakes To Avoid From The Cinematic Run

Do not:

- copy an older Anime compact memory instead of opening the current `AnimeCompactManualStack.xaml`
- place `Aspect Ratio` / output checkboxes in `Scene Composition` when the current Anime template places them in `Mood`
- join `Image Finish` to `Scene Composition` if the current section rhythm says `Control Lighting` pairs with `Image Finish`
- put a `Hide` gate on the left card of a paired section
- keep or add a second `Hide` beside `Control Lighting`; `Image Finish` owns the only `Hide` for the paired `Control Lighting/Image Finish` unit
- give `Lighting` a fake `Hide` just for symmetry when it is intended to remain exposed
- split a paired collapsed section into two separate collapsed shells
- create separate persisted keys for two cards that collapse as one unit
- treat `Manual Output` as though it survives intact in compact mode; it is split into `Mood` and `Scene Composition`
- leave Scene Composition as a single long flat list when the current template uses `Composition` and `Output Controls` columns
- forget that subject/context compact behavior is still a visibility seam and does not clear hidden values

## Host Routing Misses To Avoid

The first Watercolor compact install exposed one more easy miss that is separate from section grouping.

Problem pattern:

- adding a new compact presenter with only positive `ShowCompact...` routing
- leaving the normal shared standard-lane panel still visible through its existing host binding
- assuming the compact presenter alone will replace the standard presenter automatically

Why this misses:

- the host still has an ordinary shared-panel `ContentControl`
- if that control is bound to a broad `ShowActiveStandardLanePanel` seam, a new compact lane may render both the normal right-side panel path and the new compact routing path unless the standard path is explicitly suppressed for that lane's compact case

Safe correction pattern:

- after adding a new compact lane path, audit both sides:
  - what turns the compact presenter on
  - what turns the standard presenter off
- verify the left compact manual-stack path and the right compact presenter path separately
- verify `Mode = Standard` still uses the standard panel
- verify `Mode = Compact` for the target lane both:
  - shows the compact presenter
  - suppresses the standard presenter

Current proven Watercolor solution:

- add the compact-positive seams:
  - `ShowCompactWatercolorManualStack`
  - `ShowCompactWatercolorPanel`
- add one explicit host-visible suppression seam for the normal shared panel:
  - `ShowResolvedStandardLanePanel`
- bind the host `ContentControl` to that resolved seam instead of assuming `ShowActiveStandardLanePanel` is sufficient for compact rollout

Operational rule:

- for future ordinary/shared-standard compact lanes, do not stop after wiring `ShowCompact...`
- always inspect the host XAML to see which existing standard panel surface must be explicitly suppressed during the new compact case

## Subject-Card Visibility Checkpoint

Watercolor exposed one more rollout checkpoint after the compact panel/manual seams were already working:

- the compact lane can be installed correctly
- the right panel can swap correctly
- the left compact stack can render correctly
- and the subject card can still be wrong if `ShowSubjectContextFields` does not include the new compact lane

Current proven compact subject-card behavior:

- Anime compact: `Subject` only
- Cinematic compact: `Subject` only
- Watercolor compact: `Subject` only
- Children's Book compact: `Subject` only

Current compact-rollout rule:

- subject/context visibility now intentionally follows compact manual presentation for compact-capable lanes
- this is no longer an unresolved accidental widening from Anime into later lanes
- future compact-capable lanes should follow the same `Subject`-only compact subject-card behavior unless a later lane explicitly proves it needs an exception
- future Codex passes must not "correct" this back into a growing per-lane exception list unless the user explicitly asks

Important boundary note:

- this is still a host-owned visibility seam
- hidden `Action` and `Relationship / Interaction` values remain in state unless a later product decision changes that behavior

Operational checkpoint:

- after routing a new compact-capable lane, explicitly verify the subject card in `Mode = Compact`
- do not assume the subject card follows the new compact lane automatically just because the panel/manual routing is correct
- if the compact manual presentation is active and the subject card still shows `Action` or `Relationship / Interaction`, the rollout is incomplete

Before adapting another ordinary/shared-standard lane:

1. open the current Anime compact XAML
2. identify which controls map to the latest Anime section roles
3. decide which sections are exposed, paired, or standalone
4. assign section keys only after the collapse units are known
5. wire gates according to the compact gate rhythm above
6. verify hidden subject/context values are an intentional visibility-only decision

## Two-Layer Rule

Keep these layers separate:

- compact content layer
- compact collapse/compression layer

Rendering compact controls is not proof that compact compression behavior exists.

A lane can have compact content rendering without yet having Anime-style collapsed gates, hide/show behavior, collapsed row density, or persisted compact section state.

Do not call an ordinary lane fully compact-capable just because its compact content renders.

## Source-Of-Truth Rule

Compact presenters must bind to the same underlying state and helper paths as the standard lane presentation.

Required:

- same selector state
- same modifier state
- same slider state
- same helper/source pools
- same prompt/config behavior

Forbidden:

- compact-only selector collections
- compact-only modifier pools
- compact-only slider sources
- compact-only prompt fragment builders
- duplicate semantic paths created only for compact presentation

If the compact presenter cannot reuse the existing shared-standard state path, stop and report the mismatch rather than inventing a second path.

Compact UI is a presentation layer over existing lane state, not a second logic path.

## Anime-Copy Rule

Future shared-standard compact work may copy Anime compact shell behavior.

Safe to adapt:

- compact shell language
- compact section concept
- compact row density
- compact chip trigger presentation
- compact gate/collapse behavior where explicitly approved

Do not copy:

- Anime-specific visibility assumptions
- Anime-specific subject/context behavior
- Anime-local routing/property names
- Anime-only ownership seams
- assumptions from `AnimeCompactManualStack.xaml` that only make sense for Anime

Anime is the prototype for compact surface feel, not the ownership model for every lane.

## Collapse Wiring Rule

If Anime-style `Hide` / collapsed-gate behavior is introduced for a shared-standard lane, each section needs explicit identity.

Required:

- stable lane id
- stable section key
- namespaced compact UI state keys
- presenter-local behavior or `CompactSectionUiStateService` ownership
- no prompt/config/preset persistence for compact UI state

Avoid:

- casual lane-specific VM booleans
- copying Anime property names
- putting collapse state into prompt configuration
- hiding values while leaving their prompt effect ambiguous unless a later product decision approves that behavior

Compact collapse state is UI presentation state only.

## Compact Persistence Rule

Compact section persistence remains UI-only.

Current posture:

- `CompactSectionUiStateService` remains the small persistence seam
- stable namespaced keys remain the rule
- compact persistence must stay separate from prompt config
- compact persistence must stay separate from presets
- compact persistence must stay separate from savestates
- compact persistence must stay separate from prompt semantics

Do not:

- widen compact persistence into prompt models
- merge compact UI state into semantic state
- treat UI-only compact persistence as accidental tech debt

## Readiness Rule

An ordinary/shared-standard lane is fully compact-capable only when all required pieces for that lane are proven:

- left compact surface works
- right compact surface works
- collapse/compression behavior works if intended for that lane
- standard fallback still works
- unsupported lanes still fall back safely
- no prompt behavior changes
- no compact-only state/source pools were introduced

If collapse/compression is not intended for a given pass, say so explicitly and do not imply completion beyond compact content rendering.

## Proven Shared-Standard Grouping Rhythm

The current proven shared-standard compact rhythm is:

- `Lighting`
  - exposed live
  - no `Hide`
- paired section collapse where appropriate
  - one gate on the right-hand card only
  - one collapsed shell for the paired unit
  - one persisted key for the paired unit
- `Scene Composition`
  - standalone collapse unit
  - internal `Composition` and `Output Controls` columns

This is now a proven shared-standard rhythm.

It is not permission to flatten every lane into generic labels without lane honesty.

Lane-native labels may still be approved later when a lane truly needs them.

## Proof-Before-Abstraction Rule

Do not build a reusable shared-standard compact collapse framework yet.

Require at least Cinematic plus one more ordinary shared-standard lane to prove the pattern first.

Until then:

- prefer lane-bounded presenters
- keep rollback cheap
- keep host routing explicit
- keep ViewModel changes narrow
- avoid creating a generic compact framework from a single ordinary-lane proof

## Transitional Host-Seam Warning

The current right-side compact routing and suppression seams are still transitional.

Current truth:

- explicit host routing and suppression seams are tolerated for now
- `MainWindow.xaml` and `MainWindowViewModel.cs` are still pressure-zone host seams
- current working seams are not proof of final normalized architecture

Do not:

- declare the generic compact framework complete
- keep growing host-level compact framework logic lane by lane
- opportunistically normalize host routing during ordinary lane rollout

Future cleanup or normalization should happen only in a deliberate follow-on phase, not as incidental cleanup during a new lane install.

## Implementation Checklist For Future Ordinary Lanes

Before implementation:

- confirm the lane is ordinary/shared-standard
- confirm it already uses shared-standard state/view-model surfaces
- identify the standard selector/modifier bindings to reuse
- identify whether compact collapse/compression is in scope or explicitly deferred

During implementation:

- create lane-bounded compact presenter surfaces
- bind to existing selector/modifier/slider state
- route only the target lane into compact mode
- preserve standard mode fallback
- preserve unsupported-lane fallback
- avoid touching prompt builder, slider language, lane registry, config, preset, or savestate surfaces

After implementation:

- verify standard mode still renders normally
- verify compact mode renders the target lane compact path
- verify selector/modifier/manual controls update the same underlying state
- verify prompt output semantics did not change
- document whether collapse/compression was proven or deferred
