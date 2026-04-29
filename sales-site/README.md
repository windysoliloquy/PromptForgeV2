# Prompt Forge Sales Site

Static, Netlify-ready sales funnel for Prompt Forge.

Current commercial model:

- free public download
- limited demo usage inside the app
- paid App Unlock removes the demo cap
- premium tier remains secondary

## Routes

- `/` landing page
- `/download/` free demo installer page
- `/thanks/` post-purchase activation steps
- `/unlock/` Netlify Forms-compatible unlock request form

## Update Points

Edit `assets/site.js` to replace:

- `appCheckout`
- `premiumCheckout`
- `version`
- `installerName`
- `installer`
- `release`
- `supportEmail`

The release page now points to `releases/latest` so release-note links stay fresher without updating every page. The
installer download remains explicit to avoid guessing at a versioned asset path.

## Gallery Stub

The landing page includes a future-proof gallery shell. Placeholder cards are rendered from `promptForgeGallery` in
`assets/site.js`.

Intended future gallery item shape:

- `category`
- `title`
- `description`
- `status`
- later fields can include `thumb`, `full`, `sortKey`, and `tags`

Suggested category set for future build-out:

- `App UI`
- `Generated Outputs`
- `Workflow / Process`
- `Before / After`

## Netlify

Use `sales-site` as the Netlify base/publish directory. There is no build command.

For manual CLI deploys after Netlify auth is configured:

```powershell
npx.cmd netlify deploy --dir sales-site
npx.cmd netlify deploy --prod --dir sales-site
```

Netlify Forms must be enabled in the Netlify UI for the unlock request form to collect submissions.
