# Prompt Forge 4.0.0

Prompt Forge 4.0.0 is a major update focused on intent-aware prompt language, expanded artist guidance, stronger release packaging, and a cleaner demo-ready shipping flow.

## Highlights

- Added the new Vintage Bend intent as a dedicated semantic regime instead of a preset.
- Vintage Bend now uses its own slider language pack, artist influence phrasing, guardrails, and reversible routing back to the native prompt pool.
- Added a Vintage Bend-only modifier panel on the right side of Prompt Preview.
- Added Vintage Bend modifiers for Eastern Bloc / GDR, Thriller Undertone, Institutional Austerity, Surveillance-State Atmosphere, and Period Artifacts.
- Updated Vintage Bend wording to the early-1980s documentary lane.
- Brought the custom/manual slider UI into Vintage Bend so upgraded semantic controls are available directly inside the intent mode.

## Vintage Bend Details

Vintage Bend is now treated as a disciplined still-image prompt mode with:

- documentary realism first
- period-correct analog color second
- restrained analog texture
- practical believable light
- realism-first damping and negative prompt protection

The modifier layer is additive and intent-gated. It activates only inside Vintage Bend and cleanly drops away when another intent is selected.

## Artist System Improvements

- Added artist pair guidance support and matrix-backed compatibility lookups.
- Added artist phrase composition and override handling for stronger prompt phrasing control.
- Added artist quick-insert support and UI helpers for phrase editing.
- Added artist dropdown swap reliability fixes so editable combo boxes stay visually in sync during swaps.
- Added source data and tooling for artist pair matrix generation and coverage reporting.

## UI and Interaction Updates

- Expanded slider flyout behavior and helper text handling.
- Improved intent-aware slider labels and helper copy.
- Updated Prompt Preview card layout to support the Vintage Bend modifier rail.
- Refined actions/presets area behavior, demo feedback text, and related view model updates.
- Applied theme and skin touch-ups across the shipped skins.

## Demo / Licensing / Versioning

- Updated the app to Version 4.0.0.
- Set demo export allowance to 150 prompt exports.
- Restored the remaining-export count beside the Copy Prompt button.
- Isolated demo license state from full local license state so demo builds do not inherit a developer machine unlock.
- Updated the version / unlock window with the community showcase link and sizing adjustments.

## Release Engineering

- Hardened the signed release pipeline.
- Added timestamp server fallback and retry logic for Azure Trusted Signing.
- Fixed installer compiler path resolution and packaging-script reliability.
- Produced a signed installer artifact for this release.

## Signing Status

The shipped app binaries and installer for this release were verified as signed and timestamped successfully.

Included release asset:

- PromptForge-4.0.0-Setup.exe
