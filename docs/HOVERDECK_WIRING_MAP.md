# HoverDeck Wiring Map

This document records the current HoverDeck implementation seams in the repo as they exist now.

It is meant to stop future sessions from rediscovering the same host/state/pop-up ownership rules, and to make it obvious which pieces are:

- shared session-state projections
- HoverDeck-local presentation seams
- main-window ownership seams that still matter

This is a wiring map, not a redesign plan.

## Purpose

HoverDeck is currently a second host over the same session state used by the main app window.

Important rule:

- HoverDeck is not a second prompt system
- HoverDeck is not a second lane-state owner
- HoverDeck should project existing state and controls whenever possible

The main implementation question is usually not "where is the new logic?" but:

- which host owns the window or popup
- which view projects the existing state
- whether the seam is shared-state, HoverDeck-local UI state, or main-window lifecycle

## Primary Files

### App / host startup

- `PromptForge.App/App.xaml.cs`
  - app startup currently opens HoverDeck directly through `mainWindow.OpenHoverDeckCard(minimizeMainWindow: true)`
  - the app still constructs `MainWindow` and shared `MainWindowViewModel`

### Main window host / lifecycle

- `PromptForge.App/MainWindow.xaml.cs`
  - owns HoverDeck launch, spawn bounds, minimize/restore enforcement, and shutdown coordination
  - owns the authoritative shared `MainWindowViewModel` instance
  - owns `ShowVersionInfoDialog(...)`
  - owns the main-host gating flag for the Artist Phrase Editor popup

- `PromptForge.App/MainWindow.xaml`
  - still contains many shared UI surfaces
  - still contains the main-host Artist Phrase Editor popup path, but that path is gated off while HoverDeck owns it

### HoverDeck window shell

- `PromptForge.App/HoverDeckCardWindow.xaml`
  - the custom HoverDeck window shell
  - contains the floating title/header shell
  - hosts `HoverDeckCompactConsoleCard`
  - hosts `HoverDeckArtistPhraseEditorHost`

- `PromptForge.App/HoverDeckCardWindow.xaml.cs`
  - drag behavior
  - size / location diagnostics
  - close behavior currently routes to app shutdown through `MainWindow.ShutdownFromHoverDeck()`

### HoverDeck console / body

- `PromptForge.App/Views/CompactWorkstation/HoverDeckCompactConsoleCard.xaml`
  - the top-level console body mounted inside the HoverDeck window
  - contains the HoverDeck brand card
  - contains HoverDeck-local brand-card actions and controls
  - gates safe body vs experimental compressed body

- `PromptForge.App/Views/CompactWorkstation/HoverDeckCompactConsoleCard.xaml.cs`
  - brand-card click handlers such as:
    - unlock/version dialog
    - image-gallery prompt window

- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml`
  - the current experimental compressed body used in HoverDeck
  - contains HoverDeck-local projections for:
    - Intent
    - Subject
    - Lighting
    - collapsed/opened compact grouped rows

- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml.cs`
  - HoverDeck-local behavior for:
    - copy-prompt feedback
    - subject clear/select-all helpers
    - Style/Mood long-press backdoor
    - HoverDeck-only intent ordering
    - HoverDeck-only locked-lane dimming and helper text projection
    - open/close behavior for experimental grouped rows

### HoverDeck popup / companion ownership seams

- `PromptForge.App/Views/CompactWorkstation/HoverDeckArtistPhraseEditorHost.xaml`
- `PromptForge.App/Views/CompactWorkstation/HoverDeckArtistPhraseEditorHost.xaml.cs`
  - HoverDeck-owned Artist Phrase popup host
  - exists because opening phrase editing from HoverDeck cannot safely target the hidden main window host

- `PromptForge.App/Views/CompactWorkstation/CompactShelfShellStudy.xaml`
- `PromptForge.App/Views/CompactWorkstation/CompactShelfShellStudy.xaml.cs`
  - current shelf/stub launcher seam
  - HoverDeck-only launcher behavior for lightweight popup projections

### Companion popup checkpoint (accepted v1)

- `LaneCardCompanionPopup` now uses a lighter HoverDeck-native shell instead of the ornate Lane frame.
  - this was chosen because the ornate frame dominated smaller, variable lane content
  - the Lane companion is accepted for v1 in this lighter-shell form

- `ActionsCompanionPopup` keeps the ornate Actions frame.
  - this was chosen because the Actions panel is wider, form-like, and better suited to the heavier frame
  - the Actions companion is accepted for v1 unless a later tiny placement polish is explicitly requested

- `LiveLaneCardProjection` and `LiveActionsPresetProjection` remain live projected UI surfaces.
  - bindings, commands, prompt state, lane state, popup lifecycle, and companion launch behavior remain preserved

- no generic re-emission framework was introduced for companion popups.
  - companion presentation changes remain HoverDeck-local host seams, not a new shared popup system

### Companion re-emission reference seam

If future companion popup work needs a HoverDeck-native presentation instead of wrapping stock live card chrome, the existing reference seam is the compressed-body projection pattern in:

- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml`
- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml.cs`

This is not a generic framework. It is a proven local pattern:

- shared app/session state remains owned by `MainWindowViewModel`
- HoverDeck owns only host-local presentation state such as opened/collapsed visibility
- HoverDeck re-emits selected live controls/state inside host-owned layout and chrome
- commands and bindings remain live; only the visible presentation is re-authored

Current concrete examples:

- `StyleMoodProjectedContent`
- `ControlLightingImageFinishProjectedContent`
- `SceneCompositionProjectedContent`
- `ArtistInfluenceProjectedContent`

Supporting host-local behavior lives in:

- `UpdateStyleMoodProjection()`
- `UpdateControlLightingImageFinishProjection()`
- `UpdateSceneCompositionProjection()`
- `UpdateArtistInfluenceProjection()`

The `ArtistInfluenceProjectedContent` path is the strongest popup-adjacent precedent because:

- it uses a shared live projected control
- HoverDeck owns the surrounding layout and affordances
- `UseHostCollapseRouting` and `CollapseRequested` prove that host-owned presentation can change without moving prompt/lane/state ownership

Companion-popup implication:

- `LaneCardCompanionPopup` is the best candidate for a purpose-built HoverDeck-native re-emitted presentation surface if stock lane-card chrome becomes the limiting factor
- `ActionsCompanionPopup` can usually stay closer to its current live projection, with host-owned shell/layout doing most of the work
- if future work follows this pattern, what remains live should be:
  - shared bindings
  - command bindings
  - prompt state
  - lane state
  - popup lifecycle
- what may be visually re-emitted should be:
  - local chrome
  - grouping/layout
  - opened/collapsed host affordances

### Shared state / lock / access seams HoverDeck depends on

- `PromptForge.App/ViewModels/MainWindowViewModel.cs`
  - shared session state
  - `IntentMode`
  - `Subject`
  - all slider values
  - selectors, modifiers, prompt preview, theme selection

- `PromptForge.App/ViewModels/MainWindowViewModel.AccessGating.cs`
  - current source of truth for:
    - `IsLockedLaneActive`
    - `IsDemoMode`
    - `CopyPromptRemainingText`
    - `VersionButtonText`

- `PromptForge.App/Services/LaneUnlockStateService.cs`
  - local lane-unlock persistence

- `PromptForge.App/Services/LicenseService.cs`
  - app/demo unlock state
  - signed license allowed-lane access

## Host Ownership Model

## Shared session state

The shared session state lives in `MainWindowViewModel`.

HoverDeck should project and mutate that same state by binding to it directly.

Examples:

- HoverDeck Subject textbox binds to `Subject`
- HoverDeck intent combobox changes `IntentMode`
- HoverDeck skin combobox changes `SelectedThemeName`
- HoverDeck copy prompt uses the same `CopyPromptCommand`

## HoverDeck-local UI state

These are currently HoverDeck-local and are allowed to be local:

- window position
- window size
- drag/resize shell behavior
- some local open/closed projection state in `HoverDeckExperimentalCompressedBody.xaml.cs`
- local long-press backdoor timing

Important boundary:

- do not move prompt semantics or lane ownership into these local fields

## Main-window lifecycle ownership

`MainWindow.xaml.cs` still owns:

- opening the HoverDeck window
- spawn bounds
- main-window minimize/hide enforcement while HoverDeck is active
- restoring the main window
- app shutdown coordination

That means:

- HoverDeck window behavior still depends on `MainWindow` host methods even though the main UI itself is hidden at runtime

## Startup / shutdown path

Current startup:

1. app creates services
2. app creates `MainWindowViewModel`
3. app creates `MainWindow`
4. app sets `MainWindow = mainWindow`
5. app immediately calls:
   - `mainWindow.OpenHoverDeckCard(minimizeMainWindow: true)`

Important implication:

- the app starts with HoverDeck visible
- the shared main window still exists as the root host/state owner

Current close path:

- HoverDeck `X` in `HoverDeckCardWindow.xaml.cs` routes to:
  - `MainWindow.ShutdownFromHoverDeck()`
- app shutdown is intentional
- the old "close HoverDeck and restore main UI" behavior is bypassed during shutdown through `_isShuttingDownFromHoverDeck`

## Current HoverDeck Brand Card Wires

Current HoverDeck brand card lives in:

- `PromptForge.App/Views/CompactWorkstation/HoverDeckCompactConsoleCard.xaml`

Important current elements:

- onboarding/help info popup
- `Prompt Forge` clickable title
  - opens `ImageGalleryVisitPromptWindow`
- unlock/version button
  - binds to `VersionButtonText`
  - opens `ShowVersionInfoDialog(...)`
- skin combobox
  - binds to `Themes` / `SelectedThemeName`
- hidden HoverDeck experimental compression checkbox seam
  - local to HoverDeck

## Current HoverDeck Subject Card Wires

Current Subject section lives in:

- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml`

Current subject-specific HoverDeck behavior:

- `Copy Prompt` button in the header
- demo/locked helper text near the button
- `Forged` feedback text
- compact clear button in the title row
- right-click menu with:
  - Cut
  - Copy
  - Paste
  - Select All

Important local behavior:

- the clear button now clears both the HoverDeck textbox and the underlying binding source to avoid stale text being pushed back from focus/edit state

This logic currently lives in:

- `OnHoverDeckClearSubjectClick(...)`
- `OnHoverDeckSubjectSelectAllClick(...)`

## Current Intent Combobox Wires

Main point:

- HoverDeck intent ordering is now HoverDeck-only

It no longer directly uses the shared `IntentModes` ordering for presentation.

Current HoverDeck ordered list:

- `HoverDeckIntentModes` in `HoverDeckExperimentalCompressedBody.xaml.cs`

Important behavior:

- HoverDeck selection now bridges by selected string, not by shared index
- this was necessary because HoverDeck order diverged from the main UI order

Do not revert this back to `IntentModeSelectedIndex` unless HoverDeck and main UI order are intentionally unified again.

## Locked-Lane Projection Wires

Current lock truth:

- `MainWindowViewModel.IsLockedLaneActive`

Current HoverDeck lock projection:

- helper text next to Subject `Copy Prompt`
  - when locked:
    - `Contact Windy Soliloquy to Unlock This Lane.`
  - otherwise:
    - normal `CopyPromptRemainingText`

- intent dropdown dimming for locked lanes
  - implemented in `HoverDeckExperimentalCompressedBody.xaml.cs`
  - applied through `RefreshHoverDeckIntentItemAccessVisuals()` and `ApplyHoverDeckIntentItemAccessVisual(...)`

Important rule:

- HoverDeck should read lock/access state
- HoverDeck should not compute a second entitlement model

## HoverDeck Phrase Editor Ownership

This is a known important seam.

Problem that existed:

- HoverDeck projected `CompactArtistInfluenceCard`
- clicking `Edit phrase` set shared VM state
- the popup was originally owned by `MainWindow.xaml`
- main window was hidden/minimized
- popup placement was wrong

Current solution:

- HoverDeck has its own phrase-editor popup host:
  - `HoverDeckArtistPhraseEditorHost`
- it reads the same shared VM state and commands
- only popup ownership/placement changed

This is the standing example of:

- duplicate host projection is allowed
- duplicate phrase/business logic is not allowed

## Unlock / Version Window Ownership

Current unlock window:

- `PromptForge.App/UnlockWindow.xaml`
- `PromptForge.App/UnlockWindow.xaml.cs`

Important current owner seam:

- `MainWindow.ShowVersionInfoDialog(Window? owner)`

Why:

- when app starts directly into HoverDeck, `MainWindow` may exist but not be the visible owner
- HoverDeck button must pass the visible HoverDeck window as the owner

If this is broken, the unlock popup can crash or open incorrectly.

## Tiny Companion / Popup Examples Already Present

Examples of current host-local secondary surfaces:

- `HoverDeckArtistPhraseEditorHost`
- `ImageGalleryVisitPromptWindow`
- shelf-study popup launchers in `CompactShelfShellStudy`

These are useful precedents when deciding whether something should:

- be embedded inline
- be a host-local popup
- be a second owned window

## Fragile / Pressure-Zone Areas

These are the main files to avoid widening casually:

- `PromptForge.App/MainWindow.xaml.cs`
- `PromptForge.App/ViewModels/MainWindowViewModel.cs`
- `PromptForge.App/ViewModels/MainWindowViewModel.AccessGating.cs`

Why:

- lifecycle, host visibility, lane lock state, shared session state, and prompt-preview behavior all meet there

Current safe posture:

- prefer HoverDeck-local projection changes first
- use shared VM properties/commands when possible
- only touch host lifecycle if ownership truly requires it

## Good Future HoverDeck Work

Usually safe:

- HoverDeck-local projection layout changes
- HoverDeck-local helper text / affordance changes
- HoverDeck-local popup ownership changes
- projecting additional existing commands/state into HoverDeck
- visual dimming/annotation that reads existing state

Usually unsafe unless explicitly approved:

- new prompt logic in HoverDeck
- new lane semantics in HoverDeck
- duplicate state stacks
- converting shared VM semantics into HoverDeck-local copies
- broad `MainWindowViewModel` redesign just to support one HoverDeck affordance

## Practical "Where Do I Touch It?" Guide

If the task is about...

### HoverDeck window shell

Start with:

- `HoverDeckCardWindow.xaml`
- `HoverDeckCardWindow.xaml.cs`

### HoverDeck brand/header card

Start with:

- `HoverDeckCompactConsoleCard.xaml`
- `HoverDeckCompactConsoleCard.xaml.cs`

### HoverDeck compressed intent/subject/lighting/groups

Start with:

- `HoverDeckExperimentalCompressedBody.xaml`
- `HoverDeckExperimentalCompressedBody.xaml.cs`

### HoverDeck phrase editor popup

Start with:

- `HoverDeckArtistPhraseEditorHost.xaml`
- `HoverDeckArtistPhraseEditorHost.xaml.cs`

### HoverDeck lock/dimming/export-helper behavior

Start with:

- `MainWindowViewModel.AccessGating.cs` for truth
- `HoverDeckExperimentalCompressedBody.xaml(.cs)` for projection

### HoverDeck startup / hidden-main-window behavior

Start with:

- `App.xaml.cs`
- `MainWindow.xaml.cs`

## Current Known Mismatches / Things To Remember

- HoverDeck is still implemented through a real `MainWindow` host plus a separate `HoverDeckCardWindow`; it is not a pure standalone shell
- several HoverDeck projection seams are intentionally local and slightly manual because they were installed as safe bounded proofs
- the unlock window and phrase editor already proved that popup/window ownership matters more than shared VM state alone
- HoverDeck intent ordering is intentionally different from the main UI and must stay string-selected, not index-selected

## Graphics Wiring Map

This section records where the visible HoverDeck chrome and panel graphics actually come from, and which events or states make them change.

### Header chrome state machine

Current HoverDeck header chrome lives in:

- `PromptForge.App/HoverDeckCardWindow.xaml`

The top header plate is not one bitmap. It is a three-slice plate with three full visual states:

- normal
  - `hd_header_plate_obsidian_n_l.png`
  - `hd_header_plate_obsidian_n_c.png`
  - `hd_header_plate_obsidian_n_r.png`
- hover / focus
  - `hd_header_plate_obsidian_f_l.png`
  - `hd_header_plate_obsidian_f_c.png`
  - `hd_header_plate_obsidian_f_r.png`
- active window
  - `hd_header_plate_obsidian_a_l.png`
  - `hd_header_plate_obsidian_a_c.png`
  - `hd_header_plate_obsidian_a_r.png`

Important current rule:

- the active art is currently driven by `Window.IsActive`
- not by a click timer
- not by a pressed state

Current trigger seams:

- `HoverDeckHeaderPlateNormalStateStyle`
- `HoverDeckHeaderPlateFocusStateStyle`
- `HoverDeckHeaderPlateActiveStateStyle`
- host element: `HoverDeckHeaderPlateHost`

Practical interpretation:

- inactive window + no header hover = normal art
- inactive window + header hover = focus art
- active window = active art

The `X` close button is separate from the plate art and uses:

- `HoverDeckCloseButtonStyle`
- defined in `PromptForge.App/Styles/HoverDeck/HoverDeckSkin.ForgeObsidian.xaml`

### HoverDeck brand / Intent / Subject / Lighting panel bodies

Current HoverDeck brand card lives in:

- `PromptForge.App/Views/CompactWorkstation/HoverDeckCompactConsoleCard.xaml`

Current HoverDeck compressed cards live in:

- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml`

The visible card-body seams for the specific surfaces below are:

- brand card
  - `HoverDeckConsoleHeaderCardStyle`
  - based on `AppHeaderBorderStyle`
  - now explicitly repointed to:
    - `HoverDeckInnerShellGradientBrush`
    - `HoverDeckCardBorderBrush`
- `Intent`
  - `HoverDeckCompressedSectionCardStyle`
- `Subject`
  - `HoverDeckCompressedSectionCardStyle`
- `Lighting`
  - `HoverDeckCompressedSectionCardStyle`

Important rule:

- the blue read on these cards was not coming from their controls first
- it was coming from the card-body seams still using shared app card brushes
- the smallest safe fix was to repoint the HoverDeck-local card styles, not the shared app card styles

Current HoverDeck compressed card style:

- `HoverDeckCompressedSectionCardStyle`
- based on `SectionCardStyle`
- currently uses:
  - `Background="{DynamicResource HoverDeckInnerShellGradientBrush}"`
  - `BorderBrush="{DynamicResource HoverDeckCardBorderBrush}"`

### Launcher stubs and companion popup shells

Current stub launcher chrome lives in:

- `PromptForge.App/Views/CompactWorkstation/CompactShelfShellStudy.xaml`

Current launcher button style:

- `HoverDeckLauncherStubButtonStyle`

Current launcher click handlers:

- `OnSteerStubClick`
- `OnActionStubClick`

Current popup hosts:

- `LaneCardCompanionPopup`
- `ActionsCompanionPopup`

Current accepted shell rule:

- both companion popups currently use the lighter HoverDeck-native shell pattern in `CompactShelfShellStudy.xaml`
- they do not currently rely on the old Actions ornate-frame anchoring experiment

Shared shell ingredients:

- obsidian outer border
- brass border
- bloom layer
- top accent rail
- bottom accent line
- inner content border

### Collapsed row plate graphics

Collapsed row plate graphics live in:

- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml`

Relevant assets:

- normal row
  - `hd_row_plate_collapsed_obsidian_n_l.png`
  - `hd_row_plate_collapsed_obsidian_n_c.png`
  - `hd_row_plate_collapsed_obsidian_n_r.png`
- hover row
  - `hd_row_plate_collapsed_obsidian_h_l.png`
  - `hd_row_plate_collapsed_obsidian_h_c.png`
  - `hd_row_plate_collapsed_obsidian_h_r.png`
- active row
  - `hd_row_plate_collapsed_obsidian_a_l.png`
  - `hd_row_plate_collapsed_obsidian_a_c.png`
  - `hd_row_plate_collapsed_obsidian_a_r.png`

Current style:

- `HoverDeckCollapsedRowHeaderButtonStyle`

Current trigger rule:

- normal by default
- active when row `Tag` is `Visibility.Visible`
- hover when:
  - `IsMouseOver=True`
  - row `Tag` is `Visibility.Collapsed`

Important implementation note:

- this trigger seam must compare against real `Visibility` enums
- not string literals like `"Visible"` or `"Collapsed"`

### Copy Prompt plate graphics and event chain

Current HoverDeck copy plate lives in:

- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml`

Current button style:

- `HoverDeckCopyPromptButtonStyle`

Relevant assets:

- normal
  - `hd_copy_prompt_plate_n_l.png`
  - `hd_copy_prompt_plate_n_c.png`
  - `hd_copy_prompt_plate_n_r.png`
- hover
  - `hd_copy_prompt_plate_h_l.png`
  - `hd_copy_prompt_plate_h_c.png`
  - `hd_copy_prompt_plate_h_r.png`
- pressed
  - `hd_copy_prompt_plate_p_l.png`
  - `hd_copy_prompt_plate_p_c.png`
  - `hd_copy_prompt_plate_p_r.png`
- disabled
  - `hd_copy_prompt_plate_d_l.png`
  - `hd_copy_prompt_plate_d_c.png`
  - `hd_copy_prompt_plate_d_r.png`

Important split:

- real copy behavior comes from `CopyPromptCommand`
- HoverDeck chrome feedback comes from `OnHoverDeckCopyPromptClick`

Current event chain:

1. user clicks the image-backed `Copy Prompt` plate
2. WPF executes `CopyPromptCommand`
3. WPF also calls `OnHoverDeckCopyPromptClick`
4. that handler only arms local feedback by setting `_isHoverDeckCopyPromptFeedbackPending`
5. the local feedback waits for `MainWindowViewModel.CopyPromptFeedbackTick`
6. when that property change arrives, `StartHoverDeckCopyPromptFeedbackAnimation()` runs

Current visual targets of that animation:

- `HoverDeckSubjectCardBorderOverlayBrush`
- `HoverDeckSubjectCardBorderOverlay`
- `HoverDeckCopyPromptSuccessText`
- `HoverDeckForgeSuccessBrush`

Current storyboard keys:

- `HoverDeckCopyPromptFeedbackStoryboard`
- `HoverDeckCopyPromptFeedbackReducedMotionStoryboard`

Current behavior note:

- these storyboards now use `FillBehavior="Stop"`
- and their feedback clears automatically after about 1.5 seconds instead of hanging

### Hide-gate graphics

Image-backed Hide gates currently use:

- `HoverDeckImageHideGateButtonStyle`

Relevant assets:

- `hd_hide_gate_obsidian_n.png`
- `hd_hide_gate_obsidian_h.png`
- `hd_hide_gate_obsidian_p.png`
- `hd_hide_gate_obsidian_d.png`

These are used by:

- opened grouped rows in `HoverDeckExperimentalCompressedBody.xaml`
- the in-card Artist Influence hide gate in `CompactArtistInfluenceCard.xaml`

### Artist Influence projection seam

Artist Influence is the strongest proof that HoverDeck can change host presentation without taking over state ownership.

Key files:

- `PromptForge.App/Views/LaneReplacements/Shared/CompactArtistInfluenceCard.xaml`
- `PromptForge.App/Views/LaneReplacements/Shared/CompactArtistInfluenceCard.xaml.cs`
- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml`
- `PromptForge.App/Views/CompactWorkstation/HoverDeckExperimentalCompressedBody.xaml.cs`

Important current rules:

- the live card itself owns the Hide gate
- HoverDeck does not keep a second external Hide button for that opened row
- `UseHostCollapseRouting="True"` allows HoverDeck to intercept collapse without taking over content/state
- `CollapseRequested` is the event seam proving host-local presentation can change while prompt/lane/state ownership stays shared

### Actions companion top action rails

Current Actions companion plate buttons live in:

- `PromptForge.App/Views/CompactWorkstation/LiveActionsPresetProjection.xaml`

Current image-backed styles:

- `ActionsCompanionPrimaryPlateButtonStyle`
- `ActionsCompanionSecondaryPlateButtonStyle`

These are graphic chrome only.

Real behavior stays on the underlying commands:

- `CopyPromptCommand`
- `CopyNegativePromptCommand`

### Main-window live card overrides added for comparison work

The main live UI now has local comparison styles in:

- `PromptForge.App/MainWindow.xaml`

Current local styles:

- `HoverDeckLiveHeaderPanelStyle`
- `HoverDeckLiveSectionPanelStyle`

These exist only as bounded local surface overrides for the live comparison panels in the main window and should not be mistaken for shared app-wide theme ownership.

## Summary Rule

When working on HoverDeck, ask these in order:

1. is this shared session state or just HoverDeck-local UI state
2. does the current visible host own the popup/window correctly
3. can this be projected from existing VM state/commands
4. can the change stay inside HoverDeck-local files

If yes, keep it local.

If not, touch the main host only at the exact ownership seam.
