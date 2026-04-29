# Prompt Forge Sales Site Netlify Access Handoff

This file is the operational handoff for future Codex runs that need to edit `sales-site/` and then deploy it without rediscovering the same Netlify problems.

Read this before trying to deploy.

## Current Working Netlify Target

Authenticated account used in this workspace:

- email: `windysoliloquy@gmail.com`
- display name: `Jason Schaffer`
- team/account name: `Soliloquy Art`
- team slug: `windysoliloquy`

Current Netlify site:

- site name: `windyspromptforge`
- site id: `35ad88c2-9a15-4278-96b4-0a7933584fb7`
- admin URL: `https://app.netlify.com/projects/windyspromptforge`
- production URL: `https://windyspromptforge.netlify.app`

This production URL is the clean public URL to use after a production deploy.

## Most Important Operational Lessons

### 1. Use the local Netlify CLI, not a global guess

Do not assume:

- global `netlify` exists
- `npx netlify` will behave cleanly

Use:

- `sales-site/node_modules/.bin/netlify.cmd`

This local CLI path already works in this repo.

### 2. Draft deploy URLs are disposable

Draft URLs are per deploy.

If the site changes and the user still has an old draft URL open, they may not see the latest work. This happened repeatedly in this thread.

Normal fix:

- redeploy
- provide the newest draft URL

Better fix when the user wants the current live site:

- production deploy
- give the production URL

### 3. In this Codex app, give raw URLs

The user repeatedly reported that preview/open buttons in the app were not reliable enough.

When sharing the site:

- give the raw URL directly
- do not rely on the preview button UX

## Site Structure

The site is isolated under:

- `sales-site/`

It is:

- static HTML/CSS/JS
- no framework
- no build step

Deploys should upload the static site directly from `sales-site`.

## Local CLI Install

The local install exists because `npx` was unreliable in this environment.

Relevant files:

- `sales-site/package.json`
- `sales-site/package-lock.json`
- `sales-site/node_modules/netlify-cli`
- `sales-site/node_modules/.bin/netlify.cmd`

## Local Auth Token Note

During the site-rename pass, the working local Netlify auth token was found in the Windows roaming config here:

- `%APPDATA%\\netlify\\Config\\config.json`

Important:

- do not write the token value into repo files
- use this only as an operational recovery note if the CLI wrapper/API path needs direct authenticated troubleshooting again

## Known Problems Already Solved

### `npx netlify` / dynamic bootstrap problems

This environment previously hit:

- timeouts
- dynamic CLI bootstrap weirdness
- npm cleanup trouble

Do not make `npx netlify` the default path again.

### Windows npm cache `EPERM`

When local install/bootstrap acts up, the clean local workaround is:

```powershell
$env:npm_config_cache='C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\npm-cache'
```

### AppData / Netlify config path issues

The CLI previously hit permission trouble under the default Netlify config folder.

If needed, temporarily isolate config into workspace-local temp paths:

```powershell
$env:APPDATA='C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\appdata'
$env:LOCALAPPDATA='C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\.codex-temp\localappdata'
```

### Bad `build.base` path confusion

At one point the CLI complained that `build.base` was outside the repo root. The practical answer was not site surgery. The practical answer was:

- run the local CLI from the repo context
- deploy with explicit `--site`

## Deploy Commands That Work

Run from repo root:

### Draft deploy

```powershell
.\sales-site\node_modules\.bin\netlify.cmd deploy --dir sales-site --no-build --json --site 35ad88c2-9a15-4278-96b4-0a7933584fb7
```

### Production deploy

```powershell
.\sales-site\node_modules\.bin\netlify.cmd deploy --dir sales-site --no-build --prod --site 35ad88c2-9a15-4278-96b4-0a7933584fb7
```

After production deploy, the URL to share is:

- `https://windyspromptforge.netlify.app`

## Local Preview

If you want a quick local check before Netlify:

```powershell
cd "C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge\sales-site"
python -m http.server 4173 --bind 127.0.0.1
```

Then use:

- `http://127.0.0.1:4173/`

## Netlify Forms: Important Unresolved Blocker

Do not assume the unlock form is fixed just because the HTML looks correct.

What was done already:

- `method="POST"` confirmed
- `action="/thanks/"` confirmed
- `data-netlify="true"` added/kept
- `netlify` attribute added
- hidden `form-name` field confirmed
- named fields confirmed
- honeypot field present
- success page exists at `/thanks/`
- a hidden static copy of the form was added to help Netlify detect the form

What still happened:

- direct live POST verification still returned `404`

Meaning:

- the remaining problem is likely Netlify-side form detection/config behavior, not just HTML markup

Future Codex should:

- verify the form in the Netlify dashboard
- check whether the form is actually detected by Netlify
- run a real end-to-end submission test

Do not claim this is solved unless the deployed POST path actually succeeds.

## Other Site-State Notes Worth Knowing

Homepage pricing was realigned and documented separately in:

- `sales-site/PRICING_ALIGNMENT_REFERENCE.md`

That file should be used when editing homepage pricing so it does not drift back to the earlier generic premium/support framing.

Important:

- pricing drift already happened once before the pricing model was canonized
- future Codex should treat `sales-site/PRICING_ALIGNMENT_REFERENCE.md` as canonical for homepage pricing/access edits unless the user explicitly replaces that model
- do not rely on older handoff text or memory summaries for pricing decisions

## Old Path To Avoid

An earlier anonymous Netlify Drop existed and caused confusion. Do not reuse it as the normal route.

It was not the final authenticated workflow.

## Recommended Future Workflow

For a normal `sales-site` task:

1. edit only inside `sales-site`
2. keep scope tight
3. preview locally if useful
4. deploy with the local CLI and explicit `--site`
5. share the raw URL
6. if the work touches forms, verify actual POST behavior

## If Netlify Access Breaks Again

Check in this order:

1. does `sales-site/node_modules/.bin/netlify.cmd --version` run
2. does `sales-site/node_modules/.bin/netlify.cmd status` show the right account
3. is the deploy using explicit `--site 35ad88c2-9a15-4278-96b4-0a7933584fb7`
4. if auth is missing, run:

```powershell
.\sales-site\node_modules\.bin\netlify.cmd login
```

5. if local CLI is broken because of cache/config issues, apply the workspace-local `npm_config_cache`, `APPDATA`, and `LOCALAPPDATA` workarounds above

That is the cleanest known recovery path in this repo.
