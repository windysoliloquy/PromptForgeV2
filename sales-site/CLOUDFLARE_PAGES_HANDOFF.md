# Prompt Forge Sales Site Cloudflare Pages Handoff

This note documents the minimal Cloudflare Pages setup for the existing static sales site.

## Chosen Project Name

- `windyspromptforge`

If that is unavailable in Cloudflare Pages at creation time, fall back to:

1. `promptforge-workstation`
2. `windy-prompt-forge`
3. `prompt-forge-sales`
4. `promptforge-app`
5. `promptforge-demo`

## Expected `pages.dev` URL

If the chosen project name is available:

- `https://windyspromptforge.pages.dev`

If you must use the original preferred project name instead:

- `https://promptforge-workstation.pages.dev`

## Actual Publish Directory

The static publish directory is:

- repo-root deploy path: `sales-site`
- inside `sales-site`, Wrangler output directory: `.`

## Build Step

- build command: `none`

This site is plain static HTML/CSS/JS. No framework build step is required.

## Minimal Wrangler Config

Cloudflare Pages config lives in:

- `sales-site/wrangler.toml`

Configured values:

- `name = "windyspromptforge"`
- `pages_build_output_dir = "."`
- `compatibility_date = "2026-04-27"`

## Wrangler Commands

From repo root:

```powershell
npx wrangler pages project create windyspromptforge --production-branch main
npx wrangler pages deploy sales-site --project-name windyspromptforge
```

From inside `sales-site`:

```powershell
npx wrangler pages deploy . --project-name windyspromptforge
```

If `windyspromptforge` is unavailable, substitute the chosen fallback project name in both commands.

## Forms Limitation

The site currently keeps Netlify form markup in place.

Important:

- Cloudflare Pages will serve the static HTML
- Cloudflare Pages will not process Netlify form attributes by itself
- the existing Netlify form markup is effectively inert on Pages unless separate form handling is added later

Leave the current markup alone for now unless the deployment plan explicitly adds a Cloudflare-native form backend.

## Download Links

Installer hosting remains external and should stay that way.

Do not move installer artifacts into this repo or into Cloudflare Pages.

Current intended download target:

- `https://github.com/windysoliloquy/PromptForgeV2/releases/download/v5.1.1/PromptForge-5.1.1-Setup.exe`
