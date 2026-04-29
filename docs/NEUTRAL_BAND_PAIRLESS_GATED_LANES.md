# Neutral-Band Pairless Gated Lanes

This note records lanes that are intentionally installed into neutral-band standalone suppression even though they currently have no installed semantic pair-member exemptions.

Use this when auditing `InstalledSemanticPairSliderKeysByLane` in `SliderLanguageCatalog.SemanticPairs.cs`.

## Rule

- A lane may be authorized for neutral-band no-emittance before it has active semantic pair collapses.
- In that case, the lane should still be present in `InstalledSemanticPairSliderKeysByLane`.
- Its value may be `PairSliderSet()` when there are no current pair-member exemptions.
- This means all standalone slider fragments in the 40-59 band are suppressed for that lane.
- Do not treat an empty pair-member set as a missing implementation by itself.

## Current Entries

### Architecture / Archviz

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.ArchitectureArchvizName] = PairSliderSet()`.
- Reason: Architecture / Archviz was user-authorized for neutral-band standalone suppression, but its current `GetArchitectureArchvizSemanticPairCollapses(...)` body has no active pair collapses after the lane guard.
- Effect: Architecture / Archviz activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no Architecture / Archviz sliders are currently pair-exempt.
- Selector state: all View Mode options are quieted with explicit all-`50` `LanePromptDefaults` swatches in `LaneRegistry`.

### Editorial Illustration

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.EditorialIllustrationName] = PairSliderSet()`.
- Reason: Editorial Illustration was user-authorized for neutral-band standalone suppression, but its current `GetEditorialIllustrationSemanticPairCollapses(...)` body has no active pair collapses after the lane guard.
- Effect: Editorial Illustration activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no Editorial Illustration sliders are currently pair-exempt. This is separate from the Editorial checkbox/default slider suppression provider.

### Food Photography

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.FoodPhotographyName] = PairSliderSet()`.
- Reason: Food Photography was user-authorized for neutral-band standalone suppression, but its current `GetFoodPhotographySemanticPairCollapses(...)` body has no active pair collapses after the lane guard.
- Effect: Food Photography activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no Food Photography sliders are currently pair-exempt.
- Selector state: all Shot Mode options are quieted with explicit all-`50` `LanePromptDefaults` swatches in `LaneRegistry`.

### Lifestyle / Advertising Photography

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.LifestyleAdvertisingPhotographyName] = PairSliderSet()`.
- Reason: Lifestyle / Advertising Photography was user-authorized for neutral-band standalone suppression, but its current `GetLifestyleAdvertisingPhotographySemanticPairCollapses(...)` body has no active pair collapses after the lane guard.
- Effect: Lifestyle / Advertising Photography activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no Lifestyle / Advertising Photography sliders are currently pair-exempt.
- Selector state: all Shot Mode options are quieted with explicit all-`50` `LanePromptDefaults` swatches in `LaneRegistry`.

### Photography

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.PhotographyName] = PairSliderSet()`.
- Alias authority entry: `[IntentModeCatalog.PhotographicName] = PairSliderSet()`.
- Reason: Photography was user-authorized for neutral-band standalone suppression, but its current `GetPhotographySemanticPairCollapses(...)` body has no active pair collapses after the Photography lane guard.
- Effect: Photography activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no Photography sliders are currently pair-exempt.
- Selector state: all Photography Type and Era / Process options are quieted with explicit all-`50` `LanePromptDefaults` swatches in `LaneRegistry`.

### Pixel Art

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.PixelArtName] = PairSliderSet()`.
- Reason: Pixel Art was user-authorized for neutral-band standalone suppression, but its current `GetPixelArtSemanticPairCollapses(...)` body has no active pair collapses after the lane guard.
- Effect: Pixel Art activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no Pixel Art sliders are currently pair-exempt.
- Selector state: all Style options are quieted with explicit all-`50` `LanePromptDefaults` swatches in `LaneRegistry`.

### Product Photography

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.ProductPhotographyName] = PairSliderSet()`.
- Reason: Product Photography was user-authorized for neutral-band standalone suppression, but its current `GetProductPhotographySemanticPairCollapses(...)` body has no active pair collapses after the lane guard.
- Effect: Product Photography activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no Product Photography sliders are currently pair-exempt.
- Selector state: all Shot Type options are quieted with explicit all-`50` `LanePromptDefaults` swatches in `LaneRegistry`.

### Tattoo Art

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.TattooArtName] = PairSliderSet()`.
- Reason: Tattoo Art was user-authorized for neutral-band standalone suppression, but its current `GetTattooArtSemanticPairCollapses(...)` body has no active pair collapses after the lane guard.
- Effect: Tattoo Art activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no Tattoo Art sliders are currently pair-exempt.

### 3D Render

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.ThreeDRenderName] = PairSliderSet()`.
- Reason: 3D Render was user-authorized for neutral-band standalone suppression, but its current `GetThreeDRenderSemanticPairCollapses(...)` body has no active pair collapses after the lane guard.
- Effect: 3D Render activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no 3D Render sliders are currently pair-exempt.
- Selector state: all Style options are quieted with explicit all-`50` `LanePromptDefaults` swatches in `LaneRegistry`.

### Vintage Bend

- Status: installed as a pairless gated lane.
- Authority entry: `[IntentModeCatalog.VintageBendName] = PairSliderSet()`.
- Reason: Vintage Bend was user-authorized for neutral-band standalone suppression and currently has no `GetVintageBendSemanticPairCollapses(...)` pair surface.
- Effect: Vintage Bend activates `ShouldSuppressNeutralStandaloneSliderEmission(...)`; no Vintage Bend sliders are currently pair-exempt.

## Future Maintenance

- If a pairless gated lane gains active pair collapses later, update that lane's entry in `InstalledSemanticPairSliderKeysByLane` to list only the sliders that must remain pair-exempt.
- Do not change defaults, selector nudges, wording, or pair phrases while maintaining this install state.
- Before adding another pairless gated lane here, confirm that the user explicitly authorized that lane for neutral-band no-emittance.
