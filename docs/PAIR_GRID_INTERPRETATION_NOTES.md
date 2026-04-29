# Prompt Forge Pair Grid Interpretation Notes

## Purpose

This document exists to guide future 2D paired-control, paired-grid, and paired-pad implementation work in Prompt Forge.

Its job is to preserve the intended interpretation layer for future paired-grid UI work before that UI is implemented in code.

This document should give future Codex instances and future human passes a stable planning surface to review before making axis-label, helper-text, or pair-meaning decisions for any eventual 2D control field.

## Standing Rule

A 2D paired grid is not automatically just two raw slider names placed on X and Y axes.

Each pair should be reviewed to decide whether:

- the raw slider names are already sufficient and human-readable on a 2D surface
- or the pairing expresses a more useful directional tension that should be presented with more human-readable axis language

Examples of the distinction this rule is protecting:

- some pairs may read clearly enough as direct slider-to-slider mappings
- some pairs may become much more understandable if the future pad presents them as a field of meaning rather than as two literal slider labels

Standing instruction for future implementation work:

- do not assume that current raw slider names are automatically the final X-axis and Y-axis labels for a future paired pad
- review each pair individually before deciding axis wording

## Implementation Boundary

This document is planning authority only.

It does **not** change any current live behavior.

Specifically, this document does **not** change:

- current pair execution
- current pair collapse logic
- current UI behavior
- lane metadata
- prompt semantics
- slider wording

This file is intentionally forward-looking.

Its purpose is to preserve interpretive intent for future paired-grid work while current pair execution and semantic lane work continue unchanged.

## Pair Record Template

Use the following template for future pair interpretation entries.

```md
### Pair Record

- Pair:
- Raw sliders:
- Candidate X-axis interpretation:
- Candidate Y-axis interpretation:
- Are raw slider names acceptable?: yes / no / needs review
- Notes for future helper text:
- Status: unresolved / approved / implemented
```

Template usage guidance:

- `Pair` should name the pair in a compact human-readable form.
- `Raw sliders` should record the actual current slider names used by the live system.
- `Candidate X-axis interpretation` should describe the most likely future X-axis meaning if a 2D pad is used.
- `Candidate Y-axis interpretation` should describe the most likely future Y-axis meaning if a 2D pad is used.
- `Are raw slider names acceptable?` should force an explicit review decision instead of letting raw slider names slip through by default.
- `Notes for future helper text` should capture how the pair may need to be explained in a future UI without changing current behavior.
- `Status` should remain `unresolved` until a future implementation pass explicitly approves or implements the interpretation.

## Current Planning Notes

Current planning guidance for future paired-grid work:

- some pairs may be understandable enough with raw slider labels
- some pairs may need more human-readable directional framing
- the eventual pad should feel like a field of meaning, not merely two sliders glued together
- final axis language should be decided pair-by-pair when the pad is actually implemented
- helper text for the future pad should explain directional meaning, not merely restate raw slider names
- paired-grid interpretation should be treated as an interface-language problem layered on top of current pair execution, not as a reason to rewrite current pairing semantics prematurely

Future Codex passes should also keep this distinction clear:

- current pair execution determines how paired semantics collapse or fuse in prompt output
- future paired-grid interpretation determines how a human-facing 2D control surface might describe and navigate those pair relationships

Those are related, but they are not identical responsibilities.

## Discoverability Note

Future Codex working on paired-grid, paired-pad, or 2D pair-control UI should read this document before:

- deciding axis labels
- deciding helper text
- deciding whether raw slider names are sufficient for a future pad
- deciding whether a pair should be framed as a more human-readable directional field

If future paired-grid work begins and this document has not been consulted first, that should be treated as a process miss.

## Usage Notes For Future Codex

If you are a future Codex instance reading this file during a paired-grid or pad implementation pass, use it like this:

1. Identify the specific pair or lane-local pair family you are implementing.
2. Record the raw live slider names before inventing any axis language.
3. Decide whether the raw names are actually user-facing enough for a 2D control.
4. If not, propose candidate axis interpretations that express the pair more clearly as directional meaning.
5. Keep the interpretation layer separate from current execution semantics unless an approved task explicitly joins them.
6. Do not treat this document as permission to rewrite pair routing, collapse behavior, metadata, or prompt wording.
7. Update this document with resolved pair records only when a future task explicitly approves that interpretation work.

Short rule for future discoverability:

- read this file first
- decide pair interpretation second
- implement UI labeling third

## Pair Authoring Reminder

Future pair-authoring passes should target the actual emitted prompt fragments seen by the live prompt-preview collapse path, not only the raw source-band phrases in isolation.

Practical rule:

- verify what the participating sliders actually emit in real prompt output
- author lane-local pair collapse entries against those emitted fragments
- confirm the fused phrase replaces both independent fragments in live output

This is an execution-verification rule, not permission to widen pair infrastructure or shared collapse machinery.

## Neutral-Band Emission Rule

Neutral-band suppression is a prompt-emission rule only.

For lanes whose semantic pairing map has been completed and explicitly marked ready, standalone slider fragments in the 40-59 middle band should be suppressed at prompt-build time when the slider is not a member of that lane's installed semantic pair map.

The rule must not:

- mutate slider values
- mutate preset/config state
- change nudges or defaults
- change helper text or guide text
- remove diagnostics source visibility
- alter pair-collapse input space

Any slider that is a member of an installed semantic pair in the active lane is exempt and must continue to emit normally across the full range, including 40-59. This keeps the complete paired state space available for current pair-collapse behavior and future control evolution.

Important rollout boundary:

- do not apply this rule just because a placeholder `*Pairs.cs` file exists
- do not apply this rule to a lane until the human has confirmed that lane's pairing map is complete and ready for neutral-band suppression
- pairing is completed lane by lane across multiple passes, so future Codex instances must wait for explicit lane readiness before adding a lane to the neutral-band suppression allowlist

Human-confirmed ready lanes as of the default-ownership / intent-entry campaign:

- Anime
- Photography
- Architecture / Archviz
- Product Photography
- Food Photography
- Lifestyle / Advertising Photography
- Cinematic
- 3D Render
- Watercolor
- Children's Book
- Concept Art
- Pixel Art
- Vintage Bend

Readiness is not the same as "not installed yet."

- Some ready lanes may already have neutral-band suppression active because they are present in `SliderLanguageCatalog.SemanticPairs.cs`.
- Before adding anything, inspect `InstalledSemanticPairSliderKeysByLane` and the active `ShouldSuppressNeutralStandaloneSliderEmission(...)` path.
- Add only missing ready lanes, and preserve the pair-member exemption for sliders listed in that lane's installed pair map.
- For the complete current gated-lane ledger, see `docs/NEUTRAL_BAND_GATED_LANES.md`.
- For selector option and subtype audit status, see `docs/NEUTRAL_BAND_SUBLANE_AUDIT_ROADMAP.md`.
- For lanes intentionally installed with no current pair-member exemptions, see `docs/NEUTRAL_BAND_PAIRLESS_GATED_LANES.md`.

Default-application checkpoint:

- after adding a lane to the neutral-band allowlist, verify the lane's real boot/intent-transition/default path actually applies the quiet slider defaults
- registry defaults alone are not enough if the lane is missing from `RunIntentTransitionDefaultingWorkflow` or another active default-application surface
- if old heavier values remain live, the no-emitter rule may look broken even though the suppression logic is correct
- Infographic / Data Visualization exposed this trap: its registry defaults and Data Viz selector nudges were quiet, but the lane was not applying those defaults on intent transition until `ApplyInfographicDataVisualizationIntentDefaults()` was added to the workflow
- do not "fix" this by changing phrase authority or pair-collapse wording; first confirm the active slider values reaching prompt build time

Future Codex blocker note:

- I initially audited prompt output from the live code before reading this neutral-band rule. That was a process miss. Read this section before diagnosing balanced 40-59 slider emission.
- The main blocker was missing context, not code ambiguity. Without this rule, neutral words such as `balanced ...` can look like ordinary lane phrasing instead of a suppression contract violation.
- When investigating no-emittance failures, classify the lane first: ready allowlist, active intent-entry default path, pair-member exemption, then standalone prompt-build suppression.
- Do not repair these failures by changing defaults, nudges, phrase wording, selector descriptors, or pair phrases unless the ownership and suppression path prove that surface is actually responsible.
