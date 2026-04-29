# Neutral-Band Sublane Audit Roadmap

This roadmap tracks selector option, subtype, and subdomain surfaces that need auditing for the no-emittance rule.

The prompt-build gate is lane-wide, but selector `defaultNudges` can still push sliders outside the 40-59 neutral band. Those values are not suppressed by the neutral-band rule. This roadmap exists so future passes do not mistake a gated lane for a fully quiet selector surface.

## Proven Method

1. Confirm the lane is already installed in `InstalledSemanticPairSliderKeysByLane`.
2. Read `docs/NEUTRAL_BAND_GATED_LANES.md` for that lane's pair-member exemptions.
3. Inspect every selector option `defaultNudges` block in `LaneRegistry`.
4. For pairless gated lanes, a quiet selector option must use an explicit `new LanePromptDefaults { ... }` neutral swatch for every prompt slider that should not emit.
5. For paired gated lanes, do not neutralize sliders listed as pair-member exemptions unless explicitly requested. Those sliders are allowed to emit in the 40-59 band for pair-collapse input.
6. For paired gated lanes, audit and quiet only standalone non-paired sliders that should not emit.
7. Do not edit prompt wording, pair phrases, prompt assembly order, selector descriptors, or modifier behavior during sublane no-emittance passes.
8. If a lane or sublane is pairless and intentionally gated, keep `docs/NEUTRAL_BAND_PAIRLESS_GATED_LANES.md` accurate.

## Current Lane Gate State

All lanes in this roadmap are already installed in the neutral-band authority according to `docs/NEUTRAL_BAND_GATED_LANES.md`.

## Applied Roadmap

### Architecture / Archviz

- Gate state: pairless gated.
- Pair-member exemptions: none.
- Selector: `view-mode`.
- Options: `exterior`, `interior`, `streetscape`, `aerial-masterplan`, `twilight-marketing`.
- Sublane no-emittance state: applied. Every View Mode option has an explicit all-`50` `LanePromptDefaults` swatch.

### Photography

- Gate state: pairless gated.
- Intent aliases gated: `Photography`, `Photographic`.
- Pair-member exemptions: none.
- Selectors: `type`, `era`.
- Type options: `portrait`, `lifestyle-editorial`, `documentary-street`, `fine-art-photography`, `commercial-photography`.
- Era options: `contemporary`, `nineteenth-century-process`.
- Sublane no-emittance state: applied. Every Type and Era / Process option has an explicit all-`50` `LanePromptDefaults` swatch.

### Product Photography

- Gate state: pairless gated.
- Pair-member exemptions: none.
- Selector: `shot-type`.
- Options: `packshot`, `hero-studio`, `editorial-still-life`, `macro-detail`, `lifestyle-placement`.
- Sublane no-emittance state: applied. Every Shot Type option has an explicit all-`50` `LanePromptDefaults` swatch.

### Food Photography

- Gate state: pairless gated.
- Pair-member exemptions: none.
- Selector: `shot-mode`.
- Options: `plated-hero`, `tabletop-spread`, `macro-detail`, `beverage-service`, `hospitality-campaign`.
- Sublane no-emittance state: applied. Every Shot Mode option has an explicit all-`50` `LanePromptDefaults` swatch.

### Lifestyle / Advertising Photography

- Gate state: pairless gated.
- Pair-member exemptions: none.
- Selector: `shot-mode`.
- Options: `everyday-lifestyle`, `premium-brand-campaign`, `business-lifestyle`, `home-family-life`, `wellness-leisure`.
- Sublane no-emittance state: applied. Every Shot Mode option has an explicit all-`50` `LanePromptDefaults` swatch.

### 3D Render

- Gate state: pairless gated.
- Pair-member exemptions: none.
- Selector: `style`.
- Options: `general-cgi`, `stylized-3d`, `photoreal-3d`, `game-asset`, `animated-feature`, `product-visualization`, `sci-fi-hard-surface`.
- Sublane no-emittance state: applied. Every Style option has an explicit all-`50` `LanePromptDefaults` swatch.

### Pixel Art

- Gate state: pairless gated.
- Pair-member exemptions: none.
- Selector: `style`.
- Options: `retro-arcade`, `console-sprite`, `isometric-pixel`, `pixel-platformer`, `rpg-tileset`, `pixel-portrait`, `pixel-scene`.
- Sublane no-emittance state: applied. Every Style option has an explicit all-`50` `LanePromptDefaults` swatch.

### Watercolor

- Gate state: paired gated.
- Pair-member exemptions: `Stylization`, `Realism`.
- Selector: `style`.
- Options: `general-watercolor`, `botanical-watercolor`, `storybook-watercolor`, `landscape-watercolor`, `architectural-watercolor`.
- Sublane no-emittance state: applied. Every Style option explicitly sets standalone non-paired sliders to `50`. Existing `Stylization` and `Realism` pair-member nudges were preserved when present.

### Children's Book

- Gate state: paired gated.
- Pair-member exemptions: `Stylization`, `Realism`, `NarrativeDensity`, `BackgroundComplexity`, `TextureDepth`, `ImageCleanliness`, `MotionEnergy`, `Chaos`, `Whimsy`, `Tension`, `Awe`, `AtmosphericDepth`.
- Selector: `style`.
- Options: `general-childrens-book`, `storybook-classic`, `playful-cartoon`, `gentle-bedtime`, `educational-illustration`, `whimsical-fantasy`.
- Sublane no-emittance state: applied. Every Style option explicitly sets standalone non-paired sliders to `50`. Existing listed pair-member nudges were preserved when present.

### Concept Art

- Gate state: paired gated.
- Pair-member exemptions: `Temperature`, `LightingIntensity`, `Saturation`, `Contrast`, `Framing`, `CameraDistance`, `AtmosphericDepth`, `FocusDepth`, `Tension`, `Awe`.
- Selector: `style`.
- Options: `keyframe-concept`, `environment-concept`, `character-concept`, `creature-concept`, `costume-concept`, `prop-concept`, `vehicle-concept`.
- Sublane no-emittance state: applied. Every Style option explicitly sets standalone non-paired sliders to `50`. Existing listed pair-member nudges were preserved when present.

### Cinematic

- Gate state: paired gated.
- Pair-member exemptions: `NarrativeDensity`, `BackgroundComplexity`, `MotionEnergy`, `Chaos`, `TextureDepth`, `ImageCleanliness`, `Awe`, `AtmosphericDepth`.
- Selector: `style`.
- Options: `general-film-still`, `prestige-drama`, `thriller-suspense`, `noir-neo-noir`, `epic-blockbuster`, `intimate-indie`, `sci-fi-cinema`.
- Sublane no-emittance state: applied. Every Style option explicitly sets standalone non-paired sliders to `50`. Existing listed pair-member nudges were preserved when present.

### Comic Book

- Gate state: paired gated.
- Pair-member exemptions: `TextureDepth`, `ImageCleanliness`, `Awe`, `AtmosphericDepth`, `MotionEnergy`, `Chaos`, `NarrativeDensity`, `BackgroundComplexity`, `Stylization`, `Realism`, `Whimsy`, `Tension`.
- Selector: `style`.
- Options: `general-comic`, `superhero-comic`, `noir-comic`, `graphic-novel`, `vintage-comic`, `modern-comic`.
- Sublane no-emittance state: applied. Every Style option explicitly sets standalone non-paired sliders to `50`. Existing listed pair-member nudges were preserved when present.
- Process note: Comic Book was already installed in `docs/NEUTRAL_BAND_GATED_LANES.md`, but it was missing from this roadmap's applied queue. The earlier roadmap pass followed this file too narrowly and therefore skipped Comic Book selector nudges.
- Safe-fix note: the repair only touched Comic Book `defaultNudges` in `LaneRegistry`; it did not change the installed authority map, pair phrases, prompt assembly, selector descriptors, modifiers, lane defaults, policy hooks, or helper structure.

## Completion Rules

- Mark a sublane surface done only after rereading its exact `defaultNudges` block and confirming it matches the lane's pairless or paired gating rule.
- For pairless lanes, document any newly quieted sublane surface in this roadmap.
- For paired lanes, document which standalone non-paired sliders were made quiet and which pair-member sliders were intentionally left alone.
- Before assuming the roadmap is complete, cross-check every paired or pairless lane in `docs/NEUTRAL_BAND_GATED_LANES.md` against this file. The Comic Book miss happened because the gated-lane ledger was broader than the roadmap queue.
- Do not use helper methods or abstraction layers for defaults.
- Do not create a second no-emittance authority outside `InstalledSemanticPairSliderKeysByLane` and `ShouldSuppressNeutralStandaloneSliderEmission(...)`.
