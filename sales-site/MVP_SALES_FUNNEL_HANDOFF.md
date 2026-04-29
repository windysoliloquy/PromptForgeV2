# Prompt Forge Sales Site Handoff

This file is the broad cold-start handoff for future Codex work inside `sales-site/`.

Use it to understand:

- the current website model
- what the homepage now says
- what has already been fixed
- how Netlify deploys actually work in this repo
- which issues were solved
- which issue is still unresolved

This is web-surface handoff only. It is not app architecture doctrine.

## What This Site Is For Now

The active public model is:

- free public signed installer download
- user installs Prompt Forge and tries the capped demo
- upgrade starts inside the app
- unlock is fulfilled manually
- unlock file is imported into the app
- premium/group-session access is secondary

Do not drift this site back toward:

- checkout-first funneling
- fake instant-license automation
- vague SaaS-style support-tier language
- treating premium as the main first action

## Site Scope

The isolated site lives under:

- `sales-site/`

It is:

- static HTML/CSS/JS
- Netlify-friendly
- no framework
- no build step

Primary files:

- `sales-site/index.html`
- `sales-site/download/index.html`
- `sales-site/unlock/index.html`
- `sales-site/thanks/index.html`
- `sales-site/assets/styles.css`
- `sales-site/assets/site.js`
- `sales-site/netlify.toml`

Reference docs:

- `sales-site/NETLIFY_ACCESS_HANDOFF.md`
- `sales-site/PRICING_ALIGNMENT_REFERENCE.md`

## Current Homepage State

The homepage has already been through multiple copy and layout passes. Do not casually redo them.

Important current state:

- hero headline stays:
  - `Give structure to visual intent before the model ever sees it.`
- hero subhead stays:
  - `Prompt Forge is a native Windows app for controlled prompt construction through deliberate visual controls. Built for people who would rather direct the image than beg for it.`
- homepage pricing was realigned to match the in-app unlock model
- homepage gallery eyebrow labels were cleaned up
- hero sizing was reduced after a regression
- the homepage visual direction is intentionally dark, sparse, and premium

### Current Homepage Gallery Eyebrow Labels

These were explicitly installed and should not drift back to repeated generic labels:

- `WORKSTATION OVERVIEW`
- `PRECISION CONTROLS`
- `ARTIST INFLUENCE`
- `PAIR GUIDANCE`

### Current Homepage Pricing Hierarchy

The homepage pricing section is no longer the old simplified three-card MVP.

Current intended hierarchy:

1. `Free Demo Download`
   - `$0`
   - public signed installer
   - includes 5 free lanes:
     - Photography
     - Cinematic
     - Watercolor
     - Pixel Art
     - Graphic Design

2. `Prompt Forge App Unlock`
   - `$19.99`
   - one-time purchase
   - this is the dominant card
   - removes the demo prompt-copy cap
   - activates the app through the unlock-file flow
   - includes 1 locked premium lane of choice
   - includes 1 week complimentary group-session access
   - includes limited email help/tips for one specific image project
   - purchaser suggestions get higher-priority consideration

3. `Group Session Access`
   - visually subordinate to App Unlock
   - Monthly `$6.99`
   - Quarterly `$17.99`
   - Yearly `$49.99`
   - lane unlocks bundled with these plans are permanent
   - group-session access itself is time-based

4. `Manual Fulfillment`
   - direct purchase
   - manual unlock
   - real contact
   - accepted payments:
     - PayPal
     - Cash App
     - credit/debit card
   - commercial/custom lane work is quoted separately

If the homepage pricing needs edits, use:

- `sales-site/PRICING_ALIGNMENT_REFERENCE.md`

That file exists specifically to stop future drift.

Important:

- pricing drift already happened once because the pricing model was not canonized early enough
- future Codex should treat `sales-site/PRICING_ALIGNMENT_REFERENCE.md` as the canonical homepage pricing source unless the user explicitly changes the commercial model again
- do not "summarize" the pricing section from memory when editing it

## What Was Learned In This Thread

### 1. The user needs raw URLs, not UI preview buttons

Inside this Codex app/browser setup, the user repeatedly reported that preview/open buttons were unreliable.

If the user asks for a site link:

- return the raw URL in plain text
- do not rely on the app's preview button UX

### 2. Draft deploy URLs go stale fast

This caused repeated confusion.

Netlify draft URLs are per deploy. If the site is edited after a draft deploy:

- the old draft URL may not show the current work
- the user may think the changes are missing
- the correct fix is usually a new deploy or a production deploy

Do not assume an older draft URL still reflects the latest local state.

### 3. The stable production URL is the one to prefer

Current production site URL:

- `https://windyspromptforge.netlify.app`

If the user wants to review the current live site, this is the default URL to give unless a newer draft deploy is intentionally being reviewed.

### 4. Netlify CLI should be local, not global

Do not start by assuming global Netlify CLI or `npx netlify` is the happy path.

The proven path is:

- local CLI in `sales-site/node_modules/.bin/netlify.cmd`

See:

- `sales-site/NETLIFY_ACCESS_HANDOFF.md`

### 5. Homepage work and secondary-page work are separate concerns

This thread included both:

- homepage alignment/polish work
- secondary-page/nav/form consistency work

Do not mix them casually in future passes unless the user explicitly expands scope.

### 6. Pricing must not be reconstructed from memory

This thread had to correct homepage pricing drift after the site over-simplified the real unlock and group-session model.

Future Codex should assume:

- the pricing reference doc exists because memory-level summaries are not safe enough here
- homepage pricing edits should be checked against the screenshots and `sales-site/PRICING_ALIGNMENT_REFERENCE.md`
- if pricing language seems unclear, stop and inspect the canon instead of improvising

## Secondary Pages: Honest Current State

Secondary pages were edited in this thread:

- `download/`
- `unlock/`
- `thanks/`

Those pages now have better trust language and more consistent nav copy, but there is one important unresolved blocker:

## Known Unresolved Blocker

The Netlify form submit path still returned live `404` during direct POST verification.

Important:

- HTML-side form cleanup was attempted
- hidden `form-name` exists
- Netlify attributes were added
- success page exists at `/thanks/`
- a hidden static detection form was also added to help Netlify detect the form

Despite that, real POST checks still returned `404`.

So the current honest state is:

- the secondary-page trust/copy work exists
- the unlock request form is not fully launch-safe yet
- future Codex should verify/fix Netlify Forms at the Netlify detection/config layer before claiming it is solved

Do not write a handoff that says the form is fully fixed unless a real deployed POST test succeeds.

## Safe Editing Posture For Future Codex

For normal website work:

1. edit only the files inside `sales-site/`
2. keep changes scoped to the user-approved pass
3. do not casually change app logic, signing flow, release logic, or licensing model
4. preview locally if useful
5. deploy with the local Netlify CLI
6. give the user the raw URL

## Local Preview

Use:

```powershell
cd "C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\sales-site"
python -m http.server 4173 --bind 127.0.0.1
```

Then review:

- `http://127.0.0.1:4173/`

## When Editing Copy Or Layout

Keep these truths stable unless the user explicitly changes them:

- Prompt Forge is a serious controlled prompt instrument
- the demo is free and intentional
- upgrade happens in the app
- unlock is manual by design
- the site should feel direct, sparse, and credible
- no fake hype
- no fake automation
- no generic AI landing-page fluff

## Best Next Step For Future Codex

If the next task is not a business-model change, the usual safest sequence is:

1. inspect the current homepage and reference docs
2. edit only the approved surface
3. check local preview
4. deploy with the documented Netlify CLI path
5. share the raw production or draft URL
6. if forms are involved, verify real POST behavior instead of assuming HTML correctness
