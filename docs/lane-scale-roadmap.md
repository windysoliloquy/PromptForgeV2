# Prompt Forge Lane Scale Roadmap

## Purpose

This memo defines the next implementation step for scaling Prompt Forge from the current set of semantic packs to a few dozen packs without replacing the current hybrid lane architecture.

Core rule:

- Metadata declares the lane.
- Code interprets the lane.

This document does not propose a new prompt engine, a metadata-authored phrase system, or a generic rules engine.

## Current Architecture To Preserve

These boundaries should remain intact:

- `LaneRegistry` owns stable lane metadata only.
- `PromptBuilderService` owns orchestration and prompt assembly order.
- `SliderLanguageCatalog` owns phrase authority, semantic phrasing, and phrase-level guardrails.
- policy hooks handle exceptions and default application edge cases.
- shared standard-lane UI remains for structurally normal lanes only.
- special lanes remain explicit until they clearly justify a narrower abstraction.

## Target Scaling Shape

Prompt Forge should optimize for roughly 20-30 curated semantic packs.

It should not optimize for:

- an unlimited marketplace
- user-authored semantic grammars
- registry-authored phrase logic
- generic prompt-rule composition

The intended end state is:

- global prompt state remains explicit
- special-lane state remains explicit
- ordinary shared-path lanes use a structured typed lane-state container
- phrase logic remains code-driven, but internally partitioned by family
- validation and diagnostics scale with pack count without becoming heavyweight

## Lane Classification

Use this classification when deciding what can be containerized.

### Ordinary Shared-Path Lanes

These lanes currently fit the shared-path model and should be the only candidates for state containerization:

- Anime
- Children's Book
- Cinematic
- 3D Render
- Concept Art
- Pixel Art
- Watercolor

### Special Explicit Lanes

These lanes should remain explicit for now:

- Vintage Bend
- Comic Book

Reason:

- both carry clearer exception risk
- both are more likely to expand taste-sensitive custom behavior
- both are poor candidates for premature flattening

## State-Model Plan

## Goals

- reduce per-pack property growth in `MainWindowViewModel`
- reduce per-pack property growth in `PromptConfiguration`
- preserve capture/apply/reset/clone/preset round-trips
- preserve WPF binding sanity
- avoid raw dictionary-driven UI state

## Recommended Shape

Introduce a typed container for ordinary lanes only.

Suggested model shape:

- `PromptConfiguration.StandardLaneStates`
- `StandardLaneStateCollection`
- `StandardLaneState`

Suggested contents of `StandardLaneState`:

- `LaneId`
- typed selector value storage by known selector key
- typed modifier state storage by known modifier key

Important constraints:

- keys must be registry-backed and validated
- selectors remain string-valued because selector labels are already registry-driven
- modifiers remain bool-valued
- the container should expose helper methods such as `GetSelector`, `SetSelector`, `GetModifier`, `SetModifier`, `Clone`, `ResetToDefaults`
- this is a narrow typed state container, not an open-ended metadata bag

## What Stays Explicit

Keep these fields explicit in both configuration and view model:

- global sliders and global prompt controls
- artist influence state
- negative prompt toggles
- output controls
- special-lane state for Vintage Bend
- special-lane state for Comic Book

## Migration Boundary

Use adapters during migration rather than switching the whole app at once.

Phase the boundary like this:

1. Add `StandardLaneStateCollection` to configuration.
2. Seed it from registry defaults for ordinary lanes.
3. Add adapter helpers that read/write current ordinary lane properties from the container.
4. Move capture/apply/reset/clone logic for ordinary lanes to the container path.
5. Replace direct ordinary-lane property blocks in the shared panel path with lane-state view models.
6. Remove obsolete duplicated ordinary-lane top-level properties only after preset and diagnostics coverage is stable.

This lets existing code keep functioning while reducing future property growth.

## WPF Binding Plan

Do not bind WPF directly to raw dictionaries or string indexers.

Instead:

- keep global bindings explicit
- keep special-lane bindings explicit
- bind shared-path lane panels to dedicated ordinary-lane view models

Suggested UI shape:

- `StandardLanePanelViewModel`
- `StandardLaneStateViewModel`
- child selector/modifier view models backed by typed state accessors

This preserves current shared-panel clarity while removing the need to add new top-level properties for every ordinary pack.

## Capture / Apply / Reset / Clone / Preset Plan

These flows must remain safe and boring.

### Capture

`CaptureConfiguration()` should:

- capture globals explicitly
- capture specials explicitly
- clone `StandardLaneStateCollection` for ordinary lanes

### Apply

`ApplyConfiguration()` should:

- apply globals explicitly
- apply specials explicitly
- replace ordinary lane-state container in one controlled path
- notify shared lane panels once after apply

### Reset

`Reset()` should:

- still define global defaults explicitly
- still define special-lane defaults explicitly
- initialize ordinary lane defaults from registry-backed lane-state factory

### Clone

`PromptConfiguration.Clone()` should:

- deep clone globals and explicit objects as today
- deep clone `StandardLaneStateCollection`

### Presets

Preset storage should round-trip the container as first-class structured data, not as inferred top-level expansion.

If preset serialization currently expects flat properties, add a compatibility layer first and remove it later.

## Phrase-Authority Plan

`SliderLanguageCatalog` should remain the public phrase authority boundary, but its internal implementation should be split.

Recommended internal partition:

- `StandardSliderPhraseResolver`
- `RenderFamilyPhraseResolver`
- `IllustrativeFamilyPhraseResolver`
- `DesignFamilyPhraseResolver`
- `AnimePhraseResolver`
- `WatercolorPhraseResolver`
- `ComicBookPhraseResolver`
- `VintageBendPhraseResolver`

The public catalog can continue routing calls such as:

- `ResolvePhrase(...)`
- `ResolveXPhrase(...)`
- `ResolveXDescriptors(...)`
- `ResolveXGuideText(...)`

This keeps phrase generation in code while shrinking the blast radius of future lane additions.

## Standard Lane Family Guidance

Introduce lane families as authoring guidance for ordinary lanes, not as a new execution layer.

### Recommended Families

- render family: Cinematic, 3D Render
- illustrative family: Anime, Children's Book, Watercolor, Pixel Art
- design/presentation family: Concept Art

### Families Should Standardize

- authoring checklist
- expected subtype shape
- expected modifier shape
- modifier-cap expectations
- snapshot expectations
- regression expectations
- typical phrase-governance concerns

### Families Should Not Standardize

- actual phrase text
- prompt assembly order
- compression rules
- dedupe rules
- custom exception logic
- semantic taste decisions

Family templates should reduce authoring cost without making different lanes sound the same.

## Policy-Hook Threshold

`DefaultLanePolicy` should remain the default choice for most packs.

Use custom policy only when a lane needs pre-assembly default behavior that cannot be cleanly represented through:

- registry defaults
- ordinary shared-path state
- normal phrase resolution

Custom policy is justified when:

- lane defaults are conditional in lane-specific ways
- defaults must be normalized before standard prompt flow begins
- a lane requires pre-phrase adjustment that would otherwise leak into metadata

Custom policy is not justified for:

- ordinary subtype variation
- modifier prioritization
- descriptor wording
- pack-local phrase economy
- pack-local guardrail wording

If custom policies start appearing often, treat that as a warning that a lane should remain explicit rather than abstracted.

## Validation And Release Gate

The current validation foundation is correct. Expand it incrementally.

Required automatic checks:

- lane registry validation
- shared-lane binding validation
- regression harness
- diagnostics runner

Recommended next expansions:

- include all ordinary/shared-path lanes in regression harness
- add ordinary-lane snapshot coverage for:
  - baseline prompt
  - alternate subtype prompt
  - all-modifiers-enabled prompt
- ensure snapshot review focuses on drift signals, not frozen exact wording at every minor phrase choice

Required manual review for new packs:

- baseline prompt quality
- subtype distinctness
- modifier cap behavior
- duplicate-fragment scan
- overall taste review

This should remain a practical release gate, not a bureaucracy.

## Decision Thresholds

### Around 15 Packs

Reassess:

- whether ordinary-lane top-level property growth is slowing implementation
- whether capture/apply/reset/clone blocks are becoming the dominant source of churn
- whether shared-path lane additions still feel structurally similar

Expected action:

- begin ordinary-lane state containerization if the friction is real

### Around 24-30 Packs

Reassess:

- whether phrase logic is still locally understandable after resolver partitioning
- whether family templates are helping without flattening semantics
- whether custom policy remains rare
- whether any new lane is forcing hidden special behavior into ordinary abstractions

Expected action:

- keep extending the current architecture if ordinary lanes still fit cleanly
- mark outlier packs as explicit special lanes rather than stretching abstractions

## Risks And Tradeoffs

### Main Risk If We Do Too Little

- `MainWindowViewModel` and `PromptConfiguration` become dominated by ordinary-lane duplication
- future pack additions create repetitive boilerplate and reset/capture drift

### Main Risk If We Do Too Much

- Prompt Forge turns into a fake generic system
- phrase semantics become harder to reason about
- special-lane quality gets flattened into ordinary abstractions

### Chosen Tradeoff

Accept some explicit code duplication in exchange for:

- clear taste reasoning
- safe exceptions
- strong ownership boundaries

The scaling move is selective containerization and resolver partitioning, not genericization.

## Do Not Do This

- do not move phrase logic into metadata
- do not widen the registry into a phrase DSL
- do not move compression logic out of its current semantic boundary casually
- do not let policy hooks become a second prompt engine
- do not bind ordinary lane UI to raw dictionaries
- do not flatten Comic Book or Vintage Bend into standard-lane abstractions prematurely
- do not reorder prompt assembly in `PromptBuilderService` as part of this scaling work

## Best Practical Next Step

The next implementation session should do one bounded design-and-scaffold pass:

1. define `StandardLaneStateCollection` and `StandardLaneState`
2. classify current lanes as ordinary or special in code comments/docs
3. add registry-backed state factory/default helpers for ordinary lanes
4. wire capture/apply/reset/clone for ordinary lanes through adapters first
5. leave prompt assembly, phrase generation, compression, and special lanes untouched

That is the safest path to a few-dozen-pack architecture while preserving Prompt Forge's current reasoning clarity.
