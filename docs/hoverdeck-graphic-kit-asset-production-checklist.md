# HoverDeck Graphic Kit Asset Production Checklist

## Purpose

This document is the locked asset checklist for the first HoverDeck custom graphic kit.

It prepares asset generation and design work only.

It does not authorize:

- wiring assets into XAML
- behavior changes
- refactors
- HoverDeck state ownership changes

Current source-of-truth seams:

- HoverDeck header/brand plate lives in `HoverDeckCompactConsoleCard.xaml`
- HoverDeck body surfaces live in `HoverDeckExperimentalCompressedBody.xaml`
- HoverDeck right-edge launcher seam lives in `CompactShelfShellStudy.xaml`
- the right-edge launcher has two separate stubs:
  - `SteerStub`
  - `ActionStub`
- `Lighting` is a live exposed card, not a collapsed row
- header art must remain text-safe and utility-safe because the header contains:
  - brand/title
  - onboarding/help toggle
  - version/unlock button
  - theme combobox

## Style Rules

- dark forged console
- obsidian / black base
- warm gold / brass accents
- subtle circuit / light-path influence
- tactile click targets
- premium but slightly playful
- not neon-heavy
- not generic sci-fi
- not cartoon UI
- no baked readable text unless explicitly requested
- WPF should render text over the assets

## Locked Generation Order

1. `hd_helper_target_gold_24_*`
2. `hd_copy_prompt_plate_*`
3. `hd_row_plate_collapsed_obsidian_*`
4. `hd_stub_steer_obsidian_*`
5. `hd_stub_action_obsidian_*`
6. `hd_header_plate_obsidian_*`
7. `hd_card_frame_live_486x_auto.png`
8. divider / corner / overlay / badge assets

## Priority 1 Assets

### Targets

- Filenames:
  - `hd_helper_target_gold_24_n.png`
  - `hd_helper_target_gold_24_h.png`
  - `hd_helper_target_gold_24_p.png`
  - `hd_helper_target_gold_24_d.png`
- State variants:
  - normal
  - hover
  - pressed
  - disabled
- Intended folder:
  - `PromptForge.App/Assets/HoverDeck/Targets/`
- Construction type:
  - single image
- Target size guidance:
  - `24x24`
  - optional glow overscan may extend source bounds to `28x28` if needed
- Intended insertion surface:
  - onboarding/help toggle in `HoverDeckCompactConsoleCard.xaml`
- Design notes:
  - should establish the first tactile click language for HoverDeck
  - should read as brass/gold hardware rather than a stock circular icon button
  - should remain crisp at small size
  - hover/pressed read should come from shape, inset, and restrained light, not neon bloom
- Do-not-bake-text warning:
  - do not bake readable text

### Buttons

- Filenames:
  - `hd_copy_prompt_plate_n_l.png`
  - `hd_copy_prompt_plate_n_c.png`
  - `hd_copy_prompt_plate_n_r.png`
  - `hd_copy_prompt_plate_h_l.png`
  - `hd_copy_prompt_plate_h_c.png`
  - `hd_copy_prompt_plate_h_r.png`
  - `hd_copy_prompt_plate_p_l.png`
  - `hd_copy_prompt_plate_p_c.png`
  - `hd_copy_prompt_plate_p_r.png`
  - `hd_copy_prompt_plate_d_l.png`
  - `hd_copy_prompt_plate_d_c.png`
  - `hd_copy_prompt_plate_d_r.png`
- State variants:
  - normal
  - hover
  - pressed
  - disabled
- Intended folder:
  - `PromptForge.App/Assets/HoverDeck/Buttons/`
- Construction type:
  - stretchable 3-part piece
  - left / center / right
- Target size guidance:
  - assembled target about `112-132w x 30-36h`
- Intended insertion surface:
  - `Copy Prompt` button in the Subject header row in `HoverDeckExperimentalCompressedBody.xaml`
- Design notes:
  - should be the most tactile primary action asset in pass one
  - should feel forged/machined, not glossy web-ui
  - center segment must stay text-safe
  - left/right caps should carry most of the identity
- Do-not-bake-text warning:
  - do not bake readable text

### Rows

- Filenames:
  - `hd_row_plate_collapsed_obsidian_l.png`
  - `hd_row_plate_collapsed_obsidian_c.png`
  - `hd_row_plate_collapsed_obsidian_r.png`
  - `hd_row_plate_collapsed_obsidian_h_l.png`
  - `hd_row_plate_collapsed_obsidian_h_c.png`
  - `hd_row_plate_collapsed_obsidian_h_r.png`
  - `hd_row_plate_collapsed_obsidian_a_l.png`
  - `hd_row_plate_collapsed_obsidian_a_c.png`
  - `hd_row_plate_collapsed_obsidian_a_r.png`
- State variants:
  - normal
  - hover
  - active
- Intended folder:
  - `PromptForge.App/Assets/HoverDeck/Rows/`
- Construction type:
  - stretchable 3-part piece
  - left / center / right
- Target size guidance:
  - assembled target about `450-486w x 38-46h`
- Intended insertion surface:
  - collapsed row headers in `HoverDeckExperimentalCompressedBody.xaml`
  - `Style/Mood`
  - `Control Lighting/Image Finish`
  - `Scene Composition`
  - `Artist Influence`
- Design notes:
  - shared collapsed-row language only
  - `active` should signal engaged/opened state without becoming brighter than the main action button
  - hover should be visible but restrained because the whole row is already clickable
  - center segment must remain label-safe
- Do-not-bake-text warning:
  - do not bake readable text

### Launcher

- Filenames:
  - `hd_stub_steer_obsidian_n.png`
  - `hd_stub_steer_obsidian_h.png`
  - `hd_stub_steer_obsidian_a.png`
  - `hd_stub_action_obsidian_n.png`
  - `hd_stub_action_obsidian_h.png`
  - `hd_stub_action_obsidian_a.png`
- State variants:
  - normal
  - hover
  - active
- Intended folder:
  - `PromptForge.App/Assets/HoverDeck/Launcher/`
- Construction type:
  - single image
- Target size guidance:
  - `SteerStub`: about `84w x 52-58h`
  - `ActionStub`: about `76w x 46-52h`
- Intended insertion surface:
  - `SteerStub` in `CompactShelfShellStudy.xaml`
  - `ActionStub` in `CompactShelfShellStudy.xaml`
- Design notes:
  - treat as two sibling assets with shared visual language
  - should preserve the right-edge docked stub silhouette
  - should feel like premium side-mounted console tabs, not generic sidebar pills
  - avoid text-baked labels unless explicitly approved later
- Do-not-bake-text warning:
  - do not bake readable text

### Header

- Filenames:
  - `hd_header_plate_obsidian_l.png`
  - `hd_header_plate_obsidian_c.png`
  - `hd_header_plate_obsidian_r.png`
- State variants:
  - normal
- Intended folder:
  - `PromptForge.App/Assets/HoverDeck/Header/`
- Construction type:
  - stretchable 3-part piece
  - left / center / right
- Target size guidance:
  - overall target span about `460-490w x 92-112h`
  - left cap about `96-112w`
  - right cap about `96-112w`
  - center must be tileable or stretch-safe
- Intended insertion surface:
  - header/brand plate in `HoverDeckCompactConsoleCard.xaml`
- Design notes:
  - must stay text-safe and utility-safe
  - visual weight should bias left and upper-left rather than crowding the control zone
  - should extend the tactile language established by the target/button/stub assets
  - should feel premium and custom even when text and controls remain WPF-rendered
- Do-not-bake-text warning:
  - do not bake readable text

## Priority 2 Assets

### Frames

- Filenames:
  - `hd_card_frame_live_486x_auto.png`
- State variants:
  - normal
- Intended folder:
  - `PromptForge.App/Assets/HoverDeck/Frames/`
- Construction type:
  - overlay
- Target size guidance:
  - designed around live card widths near `486w`
- Intended insertion surface:
  - live `Intent`, `Subject`, and `Lighting` cards in `HoverDeckExperimentalCompressedBody.xaml`
- Design notes:
  - shared live-card frame overlay only
  - must coexist with existing borders and content padding

### Dividers

- Filenames:
  - `hd_divider_trace_short.png`
  - `hd_divider_trace_long.png`
- State variants:
  - normal
- Intended folder:
  - `PromptForge.App/Assets/HoverDeck/Dividers/`
- Construction type:
  - single image
- Target size guidance:
  - short: `72-120w x 2-6h`
  - long: `180-320w x 2-6h`
- Intended insertion surface:
  - internal header dividers
  - row accent rails
  - launcher interior accents
- Design notes:
  - subtle brass/circuit separators only

### Accents

- Filenames:
  - `hd_corner_accent_ul.png`
  - `hd_corner_accent_ur.png`
  - `hd_corner_accent_ll.png`
  - `hd_corner_accent_lr.png`
  - `hd_surface_overlay_trace_soft.png`
- State variants:
  - normal
- Intended folder:
  - `PromptForge.App/Assets/HoverDeck/Accents/`
- Construction type:
  - overlay
- Target size guidance:
  - corners: `24-40w x 24-40h`
  - overlay source width: about `256-512w`
- Intended insertion surface:
  - header corners
  - live card corners
  - collapsed row corners
  - soft shell overlays
- Design notes:
  - restrained finish assets only
  - should not become decorative clutter

### Badges

- Filenames:
  - `hd_badge_forged_gold_n.png`
  - `hd_badge_live_lane_n.png`
- State variants:
  - normal
- Intended folder:
  - `PromptForge.App/Assets/HoverDeck/Badges/`
- Construction type:
  - single image
- Target size guidance:
  - about `56-92w x 16-22h`
- Intended insertion surface:
  - `Forged` success area
  - optional small live/lane indicator surfaces
- Design notes:
  - optional supporting accents only
  - keep them secondary to the main tactile assets
- Do-not-bake-text warning:
  - do not bake readable text unless explicitly requested later

## Open Decisions

- Subject clear button is not part of pass one.
- Opened-row art is not part of pass one.
- `Lighting` does not get a separate Priority 1 plate.
- Header art must stay text-safe and utility-safe.

## Do Not Implement Yet

- Do not wire assets into XAML until image assets exist.
- Do not add skin picker or theme picker behavior.
- Do not change HoverDeck launch, resize, scaling, topmost, wrapper projection, or state behavior.
- Do not touch pressure-zone files.
