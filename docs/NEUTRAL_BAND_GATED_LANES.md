# Neutral-Band Gated Lanes

This ledger records every lane currently installed in the neutral-band standalone suppression authority.

Runtime authority lives in `InstalledSemanticPairSliderKeysByLane` in `SliderLanguageCatalog.SemanticPairs.cs`. A lane is gated by the no-emittance rule only when that map returns true for the active intent. Do not add a second install path elsewhere.

Selector option and subtype audits are tracked separately in `docs/NEUTRAL_BAND_SUBLANE_AUDIT_ROADMAP.md`.

## How To Read This

- Paired gated lane: the lane is installed and has one or more pair-member slider exemptions.
- Pairless gated lane: the lane is installed with `PairSliderSet()` and has no current pair-member exemptions.
- A pair-member slider continues to emit in the 40-59 band so pair collapse has its input space.
- A non-paired slider in a gated lane is silent in the 40-59 band.

## Current Gated Lanes

### Anime

- Install state: paired gated.
- Pair-member exemptions: `Stylization`, `Realism`, `LightingIntensity`, `Contrast`, `FocusDepth`, `AtmosphericDepth`, `MotionEnergy`, `Tension`, `TextureDepth`, `ImageCleanliness`.

### Architecture / Archviz

- Install state: pairless gated.
- Pair-member exemptions: none.

### Children's Book

- Install state: paired gated.
- Pair-member exemptions: `Stylization`, `Realism`, `NarrativeDensity`, `BackgroundComplexity`, `TextureDepth`, `ImageCleanliness`, `MotionEnergy`, `Chaos`, `Whimsy`, `Tension`, `Awe`, `AtmosphericDepth`.

### Cinematic

- Install state: paired gated.
- Pair-member exemptions: `NarrativeDensity`, `BackgroundComplexity`, `MotionEnergy`, `Chaos`, `TextureDepth`, `ImageCleanliness`, `Awe`, `AtmosphericDepth`.

### Comic Book

- Install state: paired gated.
- Pair-member exemptions: `TextureDepth`, `ImageCleanliness`, `Awe`, `AtmosphericDepth`, `MotionEnergy`, `Chaos`, `NarrativeDensity`, `BackgroundComplexity`, `Stylization`, `Realism`, `Whimsy`, `Tension`.
- Selector state: Comic Book Style options have explicit `50` entries for standalone non-paired sliders in `LaneRegistry`; listed pair-member sliders remain exempt for pair-collapse input.

### Concept Art

- Install state: paired gated.
- Pair-member exemptions: `Temperature`, `LightingIntensity`, `Saturation`, `Contrast`, `Framing`, `CameraDistance`, `AtmosphericDepth`, `FocusDepth`, `Tension`, `Awe`.

### Fantasy Illustration

- Install state: paired gated.
- Pair-member exemptions: `Stylization`, `Realism`, `CameraDistance`, `Framing`, `Tension`, `Whimsy`, `LightingIntensity`, `Awe`.

### Editorial Illustration

- Install state: pairless gated.
- Pair-member exemptions: none.

### Food Photography

- Install state: pairless gated.
- Pair-member exemptions: none.

### Graphic Design

- Install state: paired gated.
- Pair-member exemptions: `Stylization`, `Realism`, `Whimsy`, `Tension`, `Awe`, `Contrast`.

### Infographic / Data Visualization

- Install state: paired gated with subdomain-sensitive exemptions.
- Generic pair-member exemptions: `Stylization`, `Realism`, `Framing`, `CameraDistance`, `FocusDepth`, `DetailDensity`.
- Data Viz pair-member exemptions: `Stylization`, `Realism`, `Framing`, `CameraDistance`, `FocusDepth`, `DetailDensity`, `Tension`, `Contrast`, `MotionEnergy`, `Awe`.

### Lifestyle / Advertising Photography

- Install state: pairless gated.
- Pair-member exemptions: none.

### Photography

- Install state: pairless gated.
- Intent aliases gated: `Photography`, `Photographic`.
- Pair-member exemptions: none.

### Pixel Art

- Install state: pairless gated.
- Pair-member exemptions: none.

### Product Photography

- Install state: pairless gated.
- Pair-member exemptions: none.

### Tattoo Art

- Install state: pairless gated.
- Pair-member exemptions: none.

### 3D Render

- Install state: pairless gated.
- Pair-member exemptions: none.

### Vintage Bend

- Install state: pairless gated.
- Pair-member exemptions: none.

### Watercolor

- Install state: paired gated.
- Pair-member exemptions: `Stylization`, `Realism`.

## Maintenance Rules

- Check this ledger and `InstalledSemanticPairSliderKeysByLane` before adding a lane.
- Do not add duplicate install paths for a lane that is already present in the map.
- When a pairless gated lane gains active pair collapses, update its map entry and this ledger together.
- Do not change defaults, nudges, wording, selector descriptors, or pair phrases while maintaining this ledger.
