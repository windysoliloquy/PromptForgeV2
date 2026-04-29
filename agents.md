# AGENTS.md

## Purpose

This file defines the standing operating rules for agent work inside the Prompt Forge repository.

Use it to keep implementation prompts shorter, reduce repeated guardrail text, and preserve architectural discipline across lane work, UI work, defaults work, and refactor passes.

This file is standing doctrine. Per-task prompts should only add the exact lane, files, values, and behavior for the current pass.

---

## Core Architecture Rule

**Metadata declares the lane. Code interprets the lane.**

Do not invert that relationship.

### Ownership boundaries

- `LaneRegistry` owns stable lane metadata:
  - lane id
  - display title
  - summary/help copy
  - anchor label
  - selectors
  - selector options
  - defaults
  - selector default nudges
  - modifiers
  - modifier caps
  - behavior flags
  - panel/layout metadata
- `SliderLanguageCatalog.[Lane].cs` owns lane phrase authority:
  - anchors
  - selector descriptors
  - modifier descriptors
  - slider bands
  - guide text
  - lighting descriptors
  - lane-local guardrails
  - phrase economy / cleanup hints that belong to lane phrasing
- shared `SliderLanguageCatalog.cs` stays limited to routing and shared helpers.
- `PromptBuilderService` owns orchestration and prompt assembly order.
- `MainWindowViewModel` owns:
  - UI binding
  - capture/apply/reset flow
  - default application timing
  - selector default nudge application
  - visible UI state sync
  - prompt-preview refresh behavior
- `PromptConfiguration` state must stay coherent across Core and App when applicable.

### Never do these casually

- do not move phrase logic into metadata
- do not widen metadata into a phrase DSL or compression DSL
- do not reorder `PromptBuilderService` assembly casually
- do not move lane phrasing into shared dispatch just because patterns exist
- do not treat build success as equivalent to safety

---

## Lane Classification Rule

Before implementing any lane work, classify the lane honestly:

- **ordinary**
- **special**
- **non-default-policy** only if true exception logic exists

### Preferred order

1. ordinary + `DefaultLanePolicy`
2. special but explicit handling
3. custom policy only when truly required

### Do not assume

- special lane = custom policy
- ordinary lane = no code-driven semantics

Even ordinary lanes still rely on code-owned phrasing and guardrails in `SliderLanguageCatalog`.

---

## Work Types

Always identify the work type explicitly in the task prompt.

1. Shared slider wording update
2. Selector-sensitive slider bands
3. New lane baseline install
4. Defaults update
5. Selector default nudges
6. Checkbox/modifier descriptor install
7. Checkbox/modifier suppression behavior
8. UI feature behavior
9. Refactor-only pass

Per-task prompts should include only the sections needed for that work type.

---

## Prompting Posture For Agents

### Default posture

- be precise
- be bounded
- do not fill holes creatively
- do not widen scope
- if a field is unknown, mark it `LEAVE UNCHANGED` or `NEEDS DECISION`
- optional suggestions must stay clearly advisory

### Per-task prompts should include only:

- exact lane or surface
- exact work type
- exact files in scope
- exact values/maps/defaults being changed
- exact unchanged surfaces for that pass
- exact verification required

### Do not pad prompts with:

- full architecture restatements unless the task touches a pressure-zone seam
- lane metadata when only nudges are being changed
- speculative fills for unresolved fields
- unrelated behavior constraints

---

## Semantic Authoring Rules

### Root-restraint rule

Lane-root words are limited-use anchors, not default adjectives.

- use the lane root once by default as the main anchor
- allow a second occurrence only if structurally justified
- do not build slider phrase ladders by repeatedly prefixing the lane root
- do not rely on compression to rescue weak repeated-root phrasing

### Fragment uniqueness rule

Descriptor fragments should be unique across the pack by default.

- one use by default
- second use only if truly necessary
- avoid fake variety through tiny adjective swaps

### Full-stack coherence rule

Phrases must survive real comma-stacked output.

Ask:
- does the full prompt remain readable?
- do the sliders still own distinct semantic territory?
- does the stack avoid sludge, contradiction, and filler?
- does it still sound like Prompt Forge?

### Other standing semantic rules

- no meta phrasing
- no low-signal filler
- no adjective ladders pretending to be progression
- each slider must own distinct semantic territory
- compression is fallback hygiene, not primary intelligence

---

## Pairing Rules

Pair work stays strictly inside the existing pair-collapse system.

### Allowed

- lane-local pair authoring
- fused phrase design
- execution verification
- X/Y axis scaffolding notes for future interpretation

### Forbidden during pair passes

- UI changes
- metadata changes
- prompt assembly order changes
- new abstractions
- state-model changes
- slider renaming

### Pair selection rule

Do not force 25-cell grids just because two sliders exist.

A pair is only good if:
- both sliders own distinct semantic territory
- fused results remain descriptive
- output does not collapse into sludge
- the pair reads as a plausible X/Y field

### Pair authoring rule

Author pair collapse against **actual emitted fragments**, not assumed raw source strings.

- inspect prompt output
- target what the system actually emits
- verify fused phrase appears
- verify both independent fragments are absent
- keep fixes lane-local unless a shared seam is proven

### No internal commas

If the project is treating “no internal commas in pair phrases” as a standing rule, maintain that rule unless explicitly told otherwise.

---

## Defaults And Nudges Doctrine

### First rule: determine runtime owner before editing defaults

Before changing any lane baseline or selector nudge values, identify what actually owns runtime state for that lane.

A lane may be:
- registry-backed on intent entry
- hardcoded in `MainWindowViewModel`
- mixed
- unwired, where prior state wins

Do not assume `LaneRegistry.Defaults` are active just because they exist.

### Intent-entry precheck

For any defaults campaign:
1. inspect intent-entry workflow
2. inspect selector `defaultNudges`
3. determine which surface wins at runtime

### Nullable `LanePromptDefaults` rule

`LanePromptDefaults` properties are nullable.

- omitted property = no-op
- omitted property does **not** reset to `50`
- if neutrality is intended, write `SliderName = 50` explicitly

### Safe sequencing

1. audit runtime owner
2. fix intent-entry ownership if broken
3. set lane baseline defaults
4. set selector default nudges
5. smoke test:
   - fresh intent entry
   - selector change
   - reset
   - preset round-trip

### Scope rule

Do not bulk-edit multiple lanes unless the current task explicitly authorizes it.

---

## Refactor Doctrine

### Pressure-zone rule

Not all files are equal refactor surfaces.

Likely pressure zones include:
- `MainWindowViewModel`
- `PromptBuilderService`
- shared dispatch surfaces where many lanes converge
- any file where UI state, defaults, lane identity, and prompt emission cross together

### For pressure-zone files

- audit first
- discover one exact seam
- one seam per pass
- do not begin with cleanup appetite
- if no seam exists, `LEAVE UNCHANGED`

### Refactor approval threshold

A refactor should usually specify:
- work type 9
- exact files
- exact unchanged surfaces
- behavior-preserving intent
- risk being reduced
- verification step
- narrow stopping point

### Never casually change

- prompt wording
- assembly order
- compression timing
- default application timing
- selector nudge behavior
- UI visibility behavior
- preview timing
- policy behavior
- labels
- capture/apply/reset order

---

## Shared UI / Lane UI Rules

### Shared lane path

Use the shared standard-lane panel path for structurally ordinary lanes.

### Explicit lane path

Keep special or asymmetrical lanes explicit when that is clearer and safer.

Do not force all lanes onto the shared path for neatness.

### Floating/companion UI rule

A floating companion or alternate shell is a second projection of the same shared session state, not a second prompt system or second state owner.

It may own only host-local window state such as:
- position
- size
- topmost
- host-local expanded/collapsed shell state if explicitly intended

It must not own a second copy of lane semantics or prompt state.

---

## Defaults Campaign Rule

When running a defaults/nudges campaign across many lanes, use this sequence per lane:

1. audit runtime ownership
2. convert hardcoded or unwired intent-entry path to registry-backed if needed
3. then update lane `Defaults`
4. then update selector `defaultNudges`

Do not skip step 1.

Do not assume registry edits matter if VM hardcoded values still win.

---

## Deliverable Format For Agent Responses

When reporting implementation work, use:

- files changed
- methods/sections touched
- exact maps/defaults/options installed or exact behavior change made
- fallback behavior
- whether helper was introduced or expanded
- mismatches/risks
- build/diagnostics result
- unresolved blockers

When reporting audit work, use:

- files inspected
- ownership classification / exact state found
- exact responsible methods/sections
- smallest safe next step
- explicit statement that no code changes were made

---

## General Scope Template

Unless explicitly requested otherwise:

- implement only the approved spec
- do not redesign the lane
- do not widen scope
- do not change unrelated lanes
- do not change prompt assembly order
- do not change policy behavior
- do not change defaults/labels/modifiers/guardrails unless the work type requires it
- preserve fallback behavior unless the task explicitly changes it

---

## Standing Practical Rules

- smaller prompts are better when they state exactly what changes and what must not change
- if a field is unresolved, keep it unresolved
- do not let advisory suggestions become implementation by accident
- stabilize, validate, smoke test, then expand
- pair for clean fused output now; do not implement future UI ideas during current pairing work
- do not touch bloated orchestration files without first proving the cut line

---

## Final Rule

**Prompt Forge should grow by proven increments, not by abstraction pressure or prompt bloat.**

