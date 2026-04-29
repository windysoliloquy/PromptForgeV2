# Graphic Design Prompt Simulation

## Purpose

This note explains how to reuse the current read-only prompt-simulation path for Graphic Design semantic-lane audits.

Use it when a user asks for:

- actual assembled positive prompt output
- outcome verification for a Graphic Design pair
- proof that a pair collapse removed both base fragments
- modifier/default/subtype interaction checks using the real prompt-preview path

Do **not** use this document as permission to change production code.

The current harness is disposable local tooling only.

## Current Harness Location

- [C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness)

Files:

- [C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness\graphic-design-audit-harness.csproj](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness\graphic-design-audit-harness.csproj)
- [C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness\Program.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness\Program.cs)

## What It Actually Uses

The harness exercises the real current app pipeline:

1. instantiate `MainWindowViewModel`
2. set lane/subtype/modifier/slider values
3. call the private `CaptureConfiguration()` path through reflection
4. run `PromptBuilderService.Build(...)`
5. read the final collapsed prompt from `PromptPreview`

This means the output includes:

- current lane defaults
- current subtype default nudges
- current exclusion/default-suppression behavior
- current modifier behavior
- current pair-collapse routing
- current `SemanticPairInteractions` gate behavior

This is why it is useful for outcome audits: it is much closer to the real UI prompt-preview path than hand reconstruction.

## Important Rules

### 1. Fresh viewmodel per case

Use a fresh `MainWindowViewModel` for every simulated case.

Reason:

- intent changes
- subtype nudges
- modifier refreshes
- default suppression sync

can leak state across cases if the same instance is reused.

The current harness already follows this rule.

### 2. Decide the pairing gate explicitly

If the audit concerns pair behavior, explicitly choose whether:

- `SemanticPairInteractions = true`
- or `SemanticPairInteractions = false`

If you forget this, you can falsely conclude that a pair install is broken when the global pair toggle was simply off.

### 3. Treat `.codex-temp` as throwaway

The harness is not product code.

It is acceptable to adapt it for read-only audit work, but:

- do not treat it as a permanent API
- do not widen production architecture just to support it
- do not describe it as a shipped feature

## Current Command Flow

From the repo root:

```powershell
dotnet build PromptForge.sln --no-restore
dotnet build .codex-temp\graphic-design-audit-harness\graphic-design-audit-harness.csproj --no-restore
dotnet run --no-build --project .codex-temp\graphic-design-audit-harness\graphic-design-audit-harness.csproj
```

If the app is open and a build locks output files, close the running `PromptForge.App` process first and rerun.

## How To Change The Simulated Cases

Open:

- [C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness\Program.cs](C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\graphic-design-audit-harness\Program.cs)

The key editable area is the `cases` list.

Each `AuditCase` currently specifies:

- label
- Graphic Design subtype key
- `Minimal Layout`
- `Bold Hierarchy`
- `Whimsy` band index
- `Tension` band index

Band-to-slider mapping is currently:

- `0 -> 10`
- `1 -> 30`
- `2 -> 50`
- `3 -> 70`
- `4 -> 90`

That matches the repo's current five-band split:

- `<= 20`
- `<= 40`
- `<= 60`
- `<= 80`
- `> 80`

## What The Output Means

The harness currently prints:

- `CASE`
- `FUSED`
- `FIRST`
- `SECOND`
- `REMOVED`
- `PROMPT`

Interpretation:

- `FUSED` is the pair phrase the current pair map wants to emit
- `FIRST` and `SECOND` are the actual current collected base fragments
- `REMOVED=True` means both base fragments disappeared from the final assembled prompt
- `PROMPT` is the actual assembled positive prompt text

If `REMOVED=False`, check in this order:

1. was `SemanticPairInteractions` enabled?
2. are the collected fragments really the live emitted fragments?
3. is one of the participating sliders excluded from prompt emission?
4. did a subtype remap change one side of the collected phrase?

## Current Graphic Design Focus

The current harness is tailored to the Graphic Design `Whimsy × Tension` audit, but it can be adapted.

Likely future uses inside Graphic Design:

- `Stylization × Realism`
- subtype-by-subtype prompt audits
- `Minimal Layout` stress checks
- `Bold Hierarchy` stress checks
- default-exclusion confirmation

## If You Adapt It To Another Lane

Keep the same method:

- instantiate the real viewmodel
- set the real lane state
- capture the real configuration
- read the real `PromptPreview`

Do **not** replace that with hand-built strings unless the user explicitly accepts a non-live approximation.

If the next audit is for another lane, update:

- the case record shape
- the lane/subtype/modifier setters
- the collapse selection logic

but keep the fresh-viewmodel rule and explicit pair-toggle rule.

## Practical Warning

An outcome audit can still be truthful and misleading at the same time.

Example:

- if the global pair checkbox is off
- the harness will faithfully report that no collapse happened
- but that does **not** prove the pair install is broken

So always report whether the simulation ran with semantic pairing enabled.
