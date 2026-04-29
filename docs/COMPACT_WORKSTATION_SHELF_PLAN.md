# Compact Workstation Shelf Plan

This document records the current planning read for future host-level compact workstation shelves.

It is intentionally not an implementation plan for this pass. The purpose is to keep future Codex work from inventing upper/lower workstation shelf behavior inside lane-local compact spine files.

## Current Boundary

There are two different compact surface categories:

- lane-local compact spine
- host-level compact workstation shelves

These are separate phases and should remain separate until the host-level shelf behavior is explicitly approved.

The current Anime compact stack is the prototype for the lane-local compact spine only. It should not become the place where host-level workstation shelves are invented.

Architecture reminder:

- lane-local compact presenters are one concern
- host-level workstation shelves are a different concern
- future floating compact host work is a third host-level concern over the same shared session state
- future floating compact work should reuse compact presenters rather than replace them

## Global Compact Mode Direction

Current project direction:

- compact mode is eventually a global presentation mode above Intent
- Intent still chooses the lane
- compact mode changes the presentation/workstation regime
- this is roadmap direction, not an implementation instruction for the current pass
- future compact shelves and compact-spine rollout should reason from this direction
- current Anime compact behavior is still a prototype seam, not the final global compact-mode implementation

Do not treat current Anime-specific compact routing as the future global compact-mode design.

Specifically:

- do not copy Anime-specific host/VM visibility routing lane by lane
- do not assume the Anime compact toggle is the final global compact switch
- do not widen `MainWindowViewModel.cs` preemptively for global compact mode
- do not implement global compact mode while doing lane-local compact-spine work

## Nameplate Shell / Unfolding Workstation Direction

Current planning direction:

- the future compact workstation should begin in a closed state that visually reads as the branded nameplate/header shell
- the compact workstation should later unfold from that shell over the user's workspace
- this should not behave like a permanently side-affixed mini sidebar
- this is design/planning direction, not approval to implement unfolding behavior yet
- the future global compact-mode selector should be reasoned about as part of the header/nameplate shell
- the future global compact-mode selector should not be buried inside the Intent card
- this reinforces compact mode as a global presentation/workstation regime above Intent, not a lane-local setting

Upper/lower workstation shelves should be interpreted as later host-level unfolding surfaces of this workstation, not as independent lane-local add-ons.

The final floating/repositionable compact workstation should inherit from this branded shell model rather than from the current full-window/sidebar layout.

Guardrails:

- do not treat the compact workstation as merely a squeezed sidebar
- do not implement origami/unfolding behavior yet
- do not mistake the current shelf study for the final nameplate-shell implementation
- current header/button placement ideas remain planning-level, not approved behavior

## Preferred Advanced Host Style

Current planning direction:

- Compact remains the mode name
- Compact remains the global presentation/workstation mode above Intent
- the preferred first advanced Compact host style is a floating always-on-top companion window
- the floating companion window is intended to let users work directly in one or more model windows while Prompt Forge remains visible and interactive nearby
- overlay / transparent click-through behavior is not the current target
- overlay / transparent click-through behavior is explicitly deferred
- the future floating compact workstation should inherit from the nameplate-shell / unfolding-workstation model
- current in-window compact work remains valid as the nearer-term seam
- current in-window compact work should not be mistaken for the final floating host
- the current shelf study remains relevant as host-level workstation material for the later floating compact form

Guardrails:

- do not rename Compact mode to Overlay
- do not treat overlay as the first implementation target
- do not let window-host-style discussion change current compact extraction boundaries
- do not present floating companion-window behavior as approved for implementation
- do not over-specify drag model, position persistence, open/close lifecycle, or other window-management behavior until a dedicated implementation pass exists

## Shared Session State Read

The future floating companion should be understood as a second host/view over shared session state.

Current planning read:

- the main window and future floating companion are two projections/controllers over shared session state
- the floating companion should not become a second owner of lane state
- the floating companion should emit user actions back into shared session state
- the floating companion should render only the compact-visible subset it cares about
- the floating companion should own window state only:
  - position
  - size
  - always-on-top
  - host-specific shell state if needed

This remains planning doctrine, not implementation approval.

## Compact UI Persistence Posture

Current project direction:

- compact UI persistence remains UI-only
- compact UI persistence stays separate from prompt config, presets, savestates, and prompt semantics
- raw key/value persistence is the correct current posture
- stable namespaced keys are preferred
- typed compact persistence is deferred

The current raw key/value seam is intentionally small. Do not treat it as accidental tech debt.

Use stable namespaced keys such as:

- `anime/style-mood`
- `anime/lighting-image-finish`
- `anime/scene-composition`
- `shared/artist-influence`

Do not introduce a typed compact section model yet.

A typed model may be reconsidered later only if multiple compact lanes plus host-level shelf surfaces make that need clear.

Do not merge compact UI state into:

- prompt configuration
- presets
- savestates
- lane semantics
- prompt assembly behavior

## Upper Control Shelf

The upper control shelf is future host-level material.

Current host-owned material that appears relevant:

- `Prompt Preview` companion card in `MainWindow.xaml`
- lane-adjacent control region in the right-side host area
- extracted `AnimeLanePanel.xaml` hosted from `MainWindow.xaml`
- standard lane panel host region and explicit Comic Book / Vintage Bend modifier panels

Likely collapsed exposed controls:

- highest-value live steering controls only
- for current Anime prototype thinking:
  - `Anime Style`
  - `Anime Era`

Likely expanded-only material:

- full prompt preview text surface
- prompt preview helper text
- semantic pair interactions control
- full lane modifier/control panels
- full standard-lane panel content
- any detailed lane guidance or secondary controls that are not needed for quick steering

Why this is host-level:

- it crosses lane identity, prompt preview, and lane-adjacent steering surfaces
- it is not a manual compact card in the lane-local spine
- it must coordinate with whichever lane/intent is active
- it will eventually participate in the global compact presentation mode above Intent

Do not implement this shelf inside:

- `PromptForge.App/Views/LaneReplacements/Anime/AnimeCompactManualStack.xaml`
- `PromptForge.App/Views/LaneReplacements/Anime/AnimeCompactManualStack.xaml.cs`
- future lane-local compact spine files

## Lower Action Shelf

The lower action shelf is future host-level material.

Current host-owned material that appears relevant:

- `Actions and Presets` card in `MainWindow.xaml`
- `Copy Prompt`
- `Copy Negative Prompt`
- copy remaining/demo counter text
- preset name and saved preset controls
- savestate folder selector and create/delete controls
- `Save Preset`
- `Rename Preset`
- `Load Preset`
- `Delete Preset`
- bottom `StatusMessage` surface

Likely collapsed exposed controls:

- `Copy Prompt`

Possible collapsed companion information:

- copy remaining/demo counter text, if still needed for demo visibility
- a compact status indicator, if later approved

Likely expanded-only material:

- `Copy Negative Prompt`
- preset name input
- saved preset selector
- savestate folder selector
- savestate folder create/delete controls
- save/rename/load/delete preset controls
- full status text region

Why this is host-level:

- it owns app-wide commands and utility surfaces, not lane-local prompt steering
- preset and savestate behavior is global app behavior
- copy/export/status behavior is global app behavior
- it must not be coupled to a specific lane replacement view

Do not implement this shelf inside:

- `PromptForge.App/Views/LaneReplacements/Anime/AnimeCompactManualStack.xaml`
- `PromptForge.App/Views/LaneReplacements/Shared/CompactArtistInfluenceCard.xaml`
- future lane-local compact spine files

## Subject / Context Inputs

Subject/context handling is unresolved.

Current truth:

- `Subject` remains visible in compact Anime mode
- `Action` is hidden in compact Anime mode through `ShowSubjectContextFields`
- `Relationship / Interaction` is hidden in compact Anime mode through `ShowSubjectContextFields`
- hidden `Action` and `Relationship` values remain in state
- hidden values can still affect prompt output

Do not silently fold subject/context behavior into the upper shelf.

Unresolved decisions:

- whether global compact mode should hide `Action` and `Relationship / Interaction`
- whether hidden values should remain active without visible warning
- whether subject/context inputs belong in the future upper workstation shelf
- whether `ShowSubjectContextFields` should become a global compact presentation rule or be replaced

Until those are answered, subject/context should remain host-owned and explicitly unresolved.

## Visual Direction

The current dark card-container presentation is temporary relative to the final compact workstation direction.

Future compact workstation shelves are intended to feel more like lightweight docked micro-consoles than ordinary card buttons.

The final compact workstation is expected to move toward:

- lighter floating companion-window posture
- less heavy dark-card containment
- eventual repositionability
- usefulness over a browser or another app

Overlay and transparent click-through behavior remain deferred and should not be treated as the first advanced Compact host implementation target.

Do not use that future visual direction as permission to redesign the app shell during compact-spine extraction.

## Inert Shell Study Checkpoint

Current checkpoint truth:

- the current upper/lower shelf work is an inert host-level shell study only
- the current host seam is proven enough to keep as a study seam
- the current preferred direction is outward projection from the compact-spine edge
- the current preferred silhouette is blunt and latch-like rather than pointed or skeletal
- shelf geometry iteration is intentionally paused at this blunt outward checkpoint
- the shell study is not live behavior
- no expand/collapse logic has been approved or implemented
- no persistence has been approved or implemented
- no hit testing, commands, drag behavior, or workstation-mode switching has been approved or implemented
- the shell study must remain outside Anime compact spine files
- the upper shelf and lower shelf remain distinct host-level workstation surfaces
- further shelf refinement should wait until future live-control mapping work begins
- shelf implementation remains a later phase

What is proven:

- a host-level shelf-study view can be mounted without putting shelf material inside lane-local compact files
- the upper and lower shell placeholders can render as separate inert surfaces
- the current seam is useful for visual judgment without creating app behavior

What remains unproven:

- final geometry
- overlap behavior during scroll
- final anchor positioning
- live expand affordance behavior
- persistence
- repositionable/floating workstation behavior

Do not treat this checkpoint as approval to add live shelf behavior.

Do not continue blind shell-only shelf polish passes from here.

Future shelf geometry work may legitimately reopen only when real content changes the requirements, such as:

- mapping live dropdowns or controls onto the upper shelf
- mapping live action controls onto the lower shelf
- validating clipping and space needs under real interactive content
- designing approved expand-affordance behavior

Until then, keep the blunt outward shell study as the active checkpoint and leave shelf implementation for a later explicit host-level pass.

## Guardrails For Future Codex Work

- upper/lower workstation shelves are future host-level surfaces
- they must not be implemented inside Anime compact spine files
- compact-spine work and workstation-shelf work are separate phases
- current shelf shells are inert checkpointed studies, not live shelf behavior
- current shelf geometry is frozen at the blunt outward checkpoint
- do not continue blind shell-only shelf polish before live-control mapping work resumes
- compact mode is future global presentation mode above Intent, but that is not implemented yet
- the preferred first advanced Compact host style is a floating always-on-top companion window
- do not rename Compact mode to Overlay or treat click-through overlay as the first host target
- raw key/value compact UI persistence is the current intentional seam
- do not add shelf behavior to `AnimeCompactManualStack.xaml`
- do not add shelf behavior to compact Artist Influence support views
- do not create a generic shelf framework before a narrow host seam is approved
- do not introduce typed compact persistence before multiple lanes plus shelf surfaces prove it is needed
- do not move prompt/config/preset behavior into lane-local compact views
- do not treat `ShowSubjectContextFields` as the shelf implementation plan

If a task appears to require shelf behavior, stop and ask whether the pass is:

- a host-level shelf audit/planning pass
- a host-level parity extraction pass
- or an actual shelf implementation pass

Do not combine those with lane-local compact spine work.
