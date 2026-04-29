# Releasing Prompt Forge Publicly

This repo now has two different jobs:

- local source development
- public binary release publishing

Do not mix them.

## Branch Roles

- `local/restore-source`
  This is the local working branch with the real source code.
  Do not push this branch to GitHub.

- `main`
  This is the public release-only branch.
  It should contain signed installers, release notes, and minimal public docs only.

## Recommended Release Flow

1. Stay on `local/restore-source`.
2. Make sure Azure CLI is installed and logged in with the Trusted Signing identity on the local machine.
3. Before any release-clean or first-run test that clears local app state, back up local runtime state:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\release\Manage-LocalRuntimeState.ps1 -Mode Backup
```

   The helper backs up Prompt Forge demo/license state, user Prompt Forge app data, and known UI event logs into `artifacts\local-state-backups\PromptForgeLocalState_<timestamp>`.
   Use `ClearForReleaseTest` or `Restore` only with an explicit `-BackupPath` that points at a completed helper backup.
4. Publish and sign the app + installer from the source branch.
   The signing wrapper now restores its local Trusted Signing runtime from the current user's NuGet cache if the `tools\trusted-signing\packages` folder is missing, but it still requires:
   - `tools\trusted-signing\metadata.json`
   - a valid Azure CLI login for the signing identity
   Keep `metadata.json`, the Trusted Signing package cache, and any exported certificate material local-only. They are signing inputs, not release assets.
5. Run the release builder. It now stages only self-contained `dotnet publish` output from `artifacts\publish\PromptForge-win-x64` and fails if the output is framework-dependent.

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\installer\Build-SignedRelease.ps1
```
6. Run:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\release\Prepare-PublicRelease.ps1 `
  -SourceBranch local/restore-source `
  -PublicBranch main `
  -ReleaseVersion 5.1.2 `
  -InstallerPath .\artifacts\installer\PromptForge-5.1.2-Setup.exe `
  -ReleaseNotesPath .\artifacts\installer\PromptForge-5.1.2-release-notes.md
```

7. Review the resulting `main` branch contents carefully.
8. Commit the release-only branch.
9. Force-push `main`.
10. Create or update the GitHub release from the cleaned tag on `main`.

## Safety Rules

- Never merge `local/restore-source` into `main`.
- Never create public release tags from the source branch.
- Never push source branches to the public repo unless you intentionally want source published.
- Check `git ls-tree -r --name-only HEAD` before pushing `main`.
- Never clear `%LOCALAPPDATA%\PromptForgeDemo`, `%APPDATA%\PromptForge`, or Prompt Forge UI event logs manually during release work. Use `tools\release\Manage-LocalRuntimeState.ps1` so the backup manifest can drive restore.
- Do not bundle `docs/artist-influence-vs-imitation-doctrine.md` into the release-only tree or installer unless a later pass explicitly stages it as a separate supplementary download asset.
- Do not ship `tools\trusted-signing\metadata.json`, `tools\trusted-signing\metadata.template.json`, `tools\trusted-signing\packages\`, Azure signing configuration, private keys, exported `.cer` / `.crt` / `.p7b` files, or any other signing-side certificate material in the installer, release ZIPs, or release-only branch.
- The customer-facing release should contain signed binaries only. The Authenticode signature travels with the signed `.exe` files and installer; end users do not need the local Trusted Signing metadata or a separate certificate file to install.
- If Windows still shows a reputation or unknown-publisher warning, do not "fix" that by bundling the signing config or exporting extra certificate files into the release. First confirm the app binaries and installer are actually signed, timestamped, and built from the normal signing flow.
- Only stage separate certificate-related files if a specific installer flow genuinely requires them and that requirement is documented for that exact release.
- The public tree should contain only:
  - `README.md`
  - `.gitignore`
  - release artifacts under `artifacts/installer`

## Practical Reminder

GitHub Releases always generate automatic `Source code` links for tags.
The only way to make those safe is to ensure the tag points at a release-only commit with no actual source code in the tree.
