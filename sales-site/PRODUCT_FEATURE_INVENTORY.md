# Prompt Forge Product Feature Inventory

This document consolidates product features and abilities that are described in repo-authored docs, so future website work does not have to reconstruct the product from scattered notes.

Purpose:

- collect documented product abilities into one place
- separate current documented abilities from planned or experimental directions
- reduce product-copy drift across `sales-site`

Scope note:

- this inventory is based on repo-authored docs only
- third-party dependency docs under `node_modules` were intentionally excluded
- internal architecture/process docs were used only when they described user-facing or product-level abilities

## Current Documented Product Identity

Prompt Forge is documented as:

- a native Windows desktop app
- built for image-generation prompt workflows
- centered on structured visual controls instead of slow chat-based drafting
- aimed at faster iteration, cleaner prompt language, and more deliberate image direction

The repo docs repeatedly frame Prompt Forge as a controlled prompt workstation rather than a loose prompt bank.

## Current Documented Core Abilities

### Prompt construction and iteration

Repo docs describe Prompt Forge as supporting:

- structured visual prompt construction
- near-instant prompt iteration
- live prompt updates / immediate prompt feedback
- guided prompt building
- room for manual refinement
- prompt preview output through the app workflow

Documented control surfaces include:

- style
- mood
- composition
- lighting
- color
- realism
- texture
- symbolism
- camera framing
- motion
- atmosphere
- output preferences

### Negative prompt support

Repo docs describe:

- guided negative prompt generation
- negative prompt preview visibility when enabled
- negative prompt copy/export behavior when not blocked by demo or lock state

### Reusable workflow support

Repo docs describe:

- local presets for repeat workflows
- prompt copy/export flow
- prompt preview workflow
- save/load style of repeatable prompt-building sessions

More detailed workflow controls mentioned in docs include:

- `Copy Prompt`
- `Copy Negative Prompt`
- `Save Preset`
- `Rename Preset`
- `Load Preset`
- `Delete Preset`
- savestate folder selection
- savestate folder create/delete controls

## Current Documented Lane System

The repo docs describe Prompt Forge as a lane-based system with 19 named lanes.

Documented lane roster:

- Vintage Bend
- Anime
- Children's Book
- Comic Book
- Cinematic
- Photography
- Product Photography
- Food Photography
- Lifestyle Advertising Photography
- Architecture / Archviz
- 3D Render
- Concept Art
- Pixel Art
- Fantasy Illustration
- Editorial Illustration
- Graphic Design
- Infographic / Data Visualization
- Tattoo Art
- Watercolor

The lane-taxonomy docs classify those lanes into these broad families:

- Illustration
- Photography
- Design
- Digital Render

Repo docs also distinguish different lane behavior shapes:

- shared-standard lanes
- lane-local resolver lanes
- contributor-owned lanes
- locked lane-local lanes

### Base free lanes

Current docs state that these lanes are open by default:

- Photography
- Cinematic
- Watercolor
- Pixel Art
- Graphic Design

Compatibility note from docs:

- `Photographic` is treated as a compatibility alias

### Premium / locked lane behavior

Docs describe premium lane access as gated separately from app unlock.

Lane access can come from:

- base/free access
- local durable lane unlock state
- signed license `AllowedLanes`

The docs explicitly say:

- app unlock removes demo/export limits
- app unlock does not automatically unlock premium lanes

## Current Documented Artist-Influence Abilities

Repo docs describe Prompt Forge as including an artist influence system with:

- dual artist influence blending
- artist influence integrated into the prompt workflow
- artist pairing / blend support
- artist-pair guidance
- artist influence used to shape:
  - composition
  - palette
  - surface character
  - mood

The doctrine docs define the intended artist posture as:

- visual-principle transfer
- not direct imitation
- not copied composition
- not signature-object recall
- not famous-work reconstruction

Documented artist-influence principle surfaces include:

- lighting behavior
- composition logic
- shape treatment
- color discipline
- texture handling
- symbolic pressure
- edge behavior
- staging tendencies
- realism vs. abstraction balance
- emotional gravity
- subject transformation logic
- surface finish
- spatial organization

### Artist roster / pairing coverage

The artist matrix coverage report documents:

- 107 artists in UI including `None`
- 106 artists excluding `None`
- 83 artists in the current matrix source
- 3403 current matrix pairs

This should be treated as documentation of current artist-system scope, not a marketing headline unless you want to surface it explicitly.

## Current Documented Demo / Unlock / License Model

Repo docs describe three separate access concepts:

- demo/export access
- app/license unlock state
- lane access

### Demo mode

Docs currently describe:

- demo mode enabled by default in the current release shape
- a capped demo-copy/export system
- demo state persisted locally
- demo count surviving restart
- demo count not being stored in presets

The current doc snapshot says:

- `MaxDemoCopies` is `150`

Important:

- if public release behavior changes later, update this inventory
- do not assume the documented demo limit here is still the intended public sales number without checking current release direction

### App unlock

Repo docs describe app unlock as:

- removing demo/export-limit behavior
- changing the version/unlock surface state
- leaving premium lane access separate unless granted by lane entitlements

### Unlock import behavior

Repo docs describe:

- signed unlock file import
- success/failure messaging
- visible unlock/version window
- unlock summary showing purchaser email, license id, issued date, mode, and entitlement

### Signed license model

Repo docs currently describe support for:

- signed license validation
- merge-safe repeated signed imports
- deduplicated signed license storage
- lane access derived from signed `AllowedLanes`
- machine-bound license validation
- temporary portable license validation

Documented license fields include:

- `ProductName`
- `PurchaserEmail`
- `LicenseId`
- `IssuedUtc`
- `LicenseMode`
- `MachineToken`
- `EntitlementProfile`
- `AllowedLanes`
- `ValidationToken`
- `SignedLicenses`

### Legacy local lane unlocks

Docs also describe a legacy local unlock path for certain lanes using saved-preset ritual markers.

Documented ritual-mapped lanes:

- Vintage Bend
- Product Photography
- Food Photography
- Lifestyle / Advertising Photography

This is a current documented behavior seam, not necessarily something that belongs in public sales copy.

## Current Documented HoverDeck / Compact Workstation Abilities

Repo docs describe HoverDeck as:

- a second host over the same shared session state
- not a second prompt system
- not a second lane-state owner

Current documented HoverDeck/workstation behaviors include:

- app startup directly into HoverDeck
- shared prompt/session state projection
- subject editing
- intent selection
- theme / skin selection
- unlock/version dialog access
- image-gallery prompt window launch
- copy prompt control
- demo/locked helper text projection
- subject clear/select-all helpers
- compact grouped rows
- locked-lane dimming in the intent dropdown
- HoverDeck-owned artist phrase editor popup host

The compact-workstation docs also describe broader compact-mode direction, but much of that remains planning-level rather than shipped.

## Current Documented Public Sales / Access Model

The sales-site docs currently describe the public commercial model as:

- free public signed installer download
- capped demo inside the app
- upgrade started from inside the app
- manual unlock fulfillment
- unlock-file import as activation
- premium / group-session access as secondary rather than the first action

### Current documented homepage pricing/access structure

Repo sales docs currently canonize this public structure:

1. Free Demo Download
   - `$0`
   - public signed installer
   - includes access to the 5 free lanes

2. Prompt Forge App Unlock
   - `$19.99`
   - one-time
   - removes demo prompt-copy cap
   - activates Prompt Forge through the unlock-file flow
   - includes 1 locked premium lane of choice
   - includes 1 week complimentary group-session access
   - includes limited project-specific email help/tips
   - gives purchaser feature suggestions higher-priority consideration

3. Group Session Access
   - Monthly `$6.99`
   - Quarterly `$17.99`
   - Yearly `$49.99`
   - includes permanent lane-unlock bonuses depending on plan
   - group-session access itself is time-based

4. Manual Fulfillment
   - direct purchase
   - manual unlock
   - real contact
   - accepted payments:
     - PayPal
     - Cash App
     - credit/debit card

### Custom / commercial work

Sales docs also describe:

- commercial/custom lane work as separate
- custom work quoted individually

## Documented Planned / Experimental Directions

These items appear in repo docs, but they should not automatically be treated as current public-facing shipped features.

### Compact-mode roadmap direction

Docs describe a future direction for:

- compact mode as a global presentation mode above Intent
- a branded nameplate/header shell
- an unfolding workstation feel
- upper and lower workstation shelves
- a floating always-on-top companion window

The docs are clear that these are planning/checkpoint directions, not approved final shipped behavior.

### V5 licensing expansion

The V5 licensing plan documents a target dual-mode offline licensing system with:

- `MachineBound` licenses
- `Temporary` licenses
- signed entitlement profiles
- signed lane entitlements
- machine request-code flow
- future DPAPI hardening

Important status note from the doc:

- parts of this are being wired into the app
- lane-entitlement enforcement is still described as a next follow-up in that planning note

### Taxonomy metadata direction

The lane-taxonomy docs describe future read-only classification/grouping uses such as:

- intent picker grouping
- lane-family presentation
- registry-adjacent metadata usage

Those docs explicitly warn against treating taxonomy as the new prompt engine or phrase authority.

## Claims That Need Care Before Public Reuse

Some repo docs are older or written for internal/product notes rather than current sales language.

Use extra care before reusing these directly in public copy:

- `live prompt updates`
- `see them update live`
- older pricing labels such as `Prompt Forge Full` / `Premium Group Access`
- any demo-copy count treated as final public release truth without checking current release intent
- internal unlock mechanics such as legacy preset ritual details

These are still useful for product understanding, but some of them may need confirmation before becoming public-facing claims.

## Recommended Use Of This Inventory

Use this file when:

- writing or revising sales-site copy
- building proof sections or feature sections
- checking whether a feature claim is already documented somewhere in the repo
- separating current product truth from roadmap/experimental direction

## Homepage Workflow-Proof Intent

For the homepage workflow-proof section around the `A controlled prompt instrument, not a slot machine.` heading:

- keep the `2.44 trillion potential supporting prompts` statement visible somewhere on the page
- do not rewrite that number into a different scale, rounded shorthand, or alternate wording
- preserve the workflow framing that Prompt Forge can reach image outcomes through a small prompt and only a few clicks
- preserve the contrast against waiting on an LLM to draft a prompt first
- when showing the ArchTree outcome and `WorkFlow1` through `WorkFlow5`, keep the homepage carousel thumbnail-sized and let full-size inspection happen through the existing expandable-image/lightbox behavior

This intent exists to stop future sales-site cleanup passes from deleting the number, flattening the workflow claim into generic copy, or enlarging the carousel instead of using expandable images.

Do not use this file as a substitute for:

- current code inspection when behavior accuracy matters
- the canonical homepage pricing doc:
  - `sales-site/PRICING_ALIGNMENT_REFERENCE.md`
- Netlify deploy handoff:
  - `sales-site/NETLIFY_ACCESS_HANDOFF.md`

## Primary Repo Docs Used

Core app/product docs:

- `README.md`
- `RELEASE_NOTES.md`
- `docs/DEMO_LOCK_MEMO.md`
- `docs/V5_LICENSE_PLAN.md`
- `docs/HOVERDECK_WIRING_MAP.md`
- `docs/COMPACT_WORKSTATION_SHELF_PLAN.md`
- `docs/lane-scale-roadmap.md`
- `docs/Prompt Forge — Lane Taxonomy Classification Map.txt`
- `docs/Prompt Forge — Lane Taxonomy Consumer Inventory.txt`
- `docs/artist-influence-vs-imitation-doctrine.md`
- `docs/artist-pairs/ui_artist_matrix_coverage_report.md`

Sales/public-model docs:

- `sales-site/MVP_SALES_FUNNEL_HANDOFF.md`
- `sales-site/PRICING_ALIGNMENT_REFERENCE.md`
- `sales-site/README.md`
