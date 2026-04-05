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
3. Publish and sign the app + installer from the source branch.
   The signing wrapper now restores its local Trusted Signing runtime from the current user's NuGet cache if the `tools\trusted-signing\packages` folder is missing, but it still requires:
   - `tools\trusted-signing\metadata.json`
   - a valid Azure CLI login for the signing identity
4. Run the release builder. It now stages only self-contained `dotnet publish` output from `artifacts\publish\PromptForge-win-x64` and fails if the output is framework-dependent.

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\installer\Build-SignedRelease.ps1
```
5. Run:

```powershell
powershell -ExecutionPolicy Bypass -File .\tools\release\Prepare-PublicRelease.ps1 `
  -SourceBranch local/restore-source `
  -PublicBranch main `
  -ReleaseVersion 4.1.4 `
  -InstallerPath .\artifacts\installer\PromptForge-4.1.4-Setup.exe `
  -ReleaseNotesPath .\artifacts\installer\PromptForge-4.1.4-release-notes.md
```

6. Review the resulting `main` branch contents carefully.
7. Commit the release-only branch.
8. Force-push `main`.
9. Create or update the GitHub release from the cleaned tag on `main`.

## Safety Rules

- Never merge `local/restore-source` into `main`.
- Never create public release tags from the source branch.
- Never push source branches to the public repo unless you intentionally want source published.
- Check `git ls-tree -r --name-only HEAD` before pushing `main`.
- The public tree should contain only:
  - `README.md`
  - `.gitignore`
  - release artifacts under `artifacts/installer`

## Practical Reminder

GitHub Releases always generate automatic `Source code` links for tags.
The only way to make those safe is to ensure the tag points at a release-only commit with no actual source code in the tree.
