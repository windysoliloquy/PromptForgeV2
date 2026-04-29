# Prompt Forge Sales Site Parallax Background Doctrine

This file exists to stop future website passes from casually "fixing" the homepage parallax background system.

Use it before changing background-layer math, repeat behavior, panel assets, opacity, or scroll behavior in `sales-site/assets/site.js` or `sales-site/assets/styles.css`.

## Purpose

The sales-site background is not decorative filler.

It is an intentional layered visual system built to make the homepage feel:

- engineered
- deep
- slow-burning
- premium
- structurally different from generic flat landing pages

The parallax system is part of that identity.

## Current Layer Model

The homepage background currently uses distinct layers with different jobs:

### 1. Far background track

Owned by:

- `.site-background-track` in `assets/styles.css`
- background asset variables assigned in `assets/site.js`

Purpose:

- provide the large structural gold circuit-panel field
- create the slowest visual movement
- sit behind all readable content

Important behavior:

- slower parallax than the transparent middle layer
- repeated vertically through CSS background repetition
- bounded to site content height instead of infinite DOM image generation

### 2. Transparent middle background layer

Owned by:

- `.site-background-middle`
- `.site-background-middle-stack`
- middle-layer sizing math in `syncSiteBackgroundMiddleStack()` inside `assets/site.js`

Purpose:

- add atmospheric translucent structure over the far background
- create a second motion plane
- increase perceived depth during long-page scrolling

Important behavior:

- intentionally scrolls faster than the far background layer
- uses transparency on purpose
- repeated vertically through CSS background repetition
- bounded to content height plus overscan instead of infinite DOM node creation

### 3. Foreground content surfaces

Owned by:

- the actual page layout, cards, shells, media frames, and hero proof surfaces

Purpose:

- sit above the moving layers
- preserve legibility while letting the page feel alive underneath

## Intentional Non-Standard Behavior

Some of this system may look unusual if judged against generic landing-page conventions.

That does not make it wrong.

The following are intentional:

- the background split between far layer and transparent mid layer
- different parallax rates between those layers
- visual depth created by transparency rather than flat opacity-free tiling
- bounded repeated backgrounds sized from content height
- some background coverage extending slightly past the footer area for safety

Do not treat these as cleanup targets by default.

## What Must Not Be Casually Changed

Do not casually change:

- `promptForgeMiddleBackgroundRate`
- the relative speed relationship between far and middle layers
- the asset assignment pattern for the far background
- the asset assignment pattern for the transparent middle layer
- the decision to use repeated CSS backgrounds instead of endlessly stacked DOM images
- the bounded-height model tied to page/footer measurement
- opacity balance of the middle layer without visual review
- the background system simply to make it more "standard"

## Why CSS Repetition Is Used

The background assets were intentionally prepared to repeat cleanly.

Because of that, the safer implementation is:

- bounded background paint areas
- CSS `repeat-y`
- content-height measurement

instead of:

- endlessly adding repeated `<img>` nodes
- guessing how many panels are needed
- allowing background coverage to run out before the end of the page

This is both a visual decision and a performance/safety decision.

## Safe Editing Posture

If future work touches this system, use this order:

1. identify which layer is being changed
2. decide whether the task is about:
   - asset swap
   - opacity tuning
   - parallax speed
   - coverage height
   - repeat format
3. preserve all unrelated layer behavior
4. review locally in the real browser view
5. inspect both tall-page and reduced-width layouts before calling it done

## What Counts As A Legitimate Change

Reasonable changes can include:

- replacing a background asset with a new seamless version
- extending bounded coverage if the layer ends too early
- adjusting footer overscan slightly
- refining opacity after visual review
- revising repetition format only if the seamless background assets change

## What Does Not Count As A Legitimate Change By Itself

These are not sufficient reasons on their own:

- "this is not industry standard"
- "there are two different background treatments"
- "this could be simplified"
- "this would be cleaner if all containers matched"
- "this feels custom"

The system is custom on purpose.

## File Ownership

Main implementation surfaces:

- `sales-site/assets/site.js`
- `sales-site/assets/styles.css`

If this doctrine becomes outdated because the parallax system is intentionally redesigned, update this file in the same pass.
