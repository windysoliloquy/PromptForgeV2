[CmdletBinding()]
param(
    [string]$PublicBranch = 'main',
    [string]$SourceBranch = 'local/restore-source',
    [string]$ReleaseVersion,
    [string]$InstallerPath,
    [string]$ReleaseNotesPath,
    [string[]]$AdditionalArtifacts = @(),
    [switch]$SkipCheckout,
    [switch]$LeaveCurrentBranch
)

$ErrorActionPreference = 'Stop'

function Invoke-Git {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    & git @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "git $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

function Resolve-RequiredPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PathValue,

        [Parameter(Mandatory = $true)]
        [string]$Label
    )

    if ([string]::IsNullOrWhiteSpace($PathValue)) {
        throw "$Label is required."
    }

    return (Resolve-Path -LiteralPath $PathValue).Path
}

function Copy-Artifact {
    param(
        [Parameter(Mandatory = $true)]
        [string]$SourcePath,

        [Parameter(Mandatory = $true)]
        [string]$DestinationDirectory
    )

    New-Item -ItemType Directory -Path $DestinationDirectory -Force | Out-Null
    Copy-Item -LiteralPath $SourcePath -Destination (Join-Path $DestinationDirectory ([IO.Path]::GetFileName($SourcePath))) -Force
}

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path
$originalBranch = (& git branch --show-current).Trim()

if ([string]::IsNullOrWhiteSpace($originalBranch)) {
    throw 'Could not determine the current branch.'
}

$resolvedInstallerPath = Resolve-RequiredPath -PathValue $InstallerPath -Label 'InstallerPath'
$resolvedReleaseNotesPath = Resolve-RequiredPath -PathValue $ReleaseNotesPath -Label 'ReleaseNotesPath'

$resolvedAdditionalArtifacts = @()
foreach ($artifact in $AdditionalArtifacts) {
    $resolvedAdditionalArtifacts += Resolve-RequiredPath -PathValue $artifact -Label 'AdditionalArtifacts entry'
}

if (-not $SkipCheckout) {
    if (-not [string]::Equals($originalBranch, $SourceBranch, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Run this script from '$SourceBranch' or pass -SkipCheckout if you know what you're doing. Current branch: '$originalBranch'."
    }
}

Push-Location $repoRoot
try {
    if (-not $SkipCheckout) {
        Invoke-Git -Arguments @('checkout', $PublicBranch)
    }

    $trackedPaths = (& git ls-tree -r --name-only HEAD)
    foreach ($trackedPath in $trackedPaths) {
        & git rm --quiet -r --ignore-unmatch -- $trackedPath
    }

    $keepPaths = @('.git', '.gitignore')
    Get-ChildItem -Force | Where-Object { $keepPaths -notcontains $_.Name } | Remove-Item -Recurse -Force

    New-Item -ItemType Directory -Path 'artifacts\installer' -Force | Out-Null

    $readme = @"
# Prompt Forge Releases

This branch is release-only.

The source code is kept off the public branch on purpose. This branch should only contain signed installers, release notes, and minimal public-facing documentation.

Current release:

- Version: $ReleaseVersion
- Installer: $([IO.Path]::GetFileName($resolvedInstallerPath))

Release assets live in `artifacts/installer`.
"@
    Set-Content -LiteralPath 'README.md' -Value $readme -Encoding ASCII

    $gitIgnore = @"
*
!.gitignore
!README.md
!artifacts/
!artifacts/**
"@
    Set-Content -LiteralPath '.gitignore' -Value $gitIgnore -Encoding ASCII

    Copy-Artifact -SourcePath $resolvedInstallerPath -DestinationDirectory (Join-Path $repoRoot 'artifacts\installer')
    Copy-Artifact -SourcePath $resolvedReleaseNotesPath -DestinationDirectory (Join-Path $repoRoot 'artifacts\installer')

    foreach ($artifact in $resolvedAdditionalArtifacts) {
        Copy-Artifact -SourcePath $artifact -DestinationDirectory (Join-Path $repoRoot 'artifacts\installer')
    }

    Invoke-Git -Arguments @('add', '.')

    Write-Host ''
    Write-Host "Public release branch prepared on '$PublicBranch'."
    Write-Host 'Next steps:'
    Write-Host "  1. Review with: git status --short"
    Write-Host "  2. Commit with: git commit -m `"Publish Prompt Forge $ReleaseVersion release artifacts`""
    Write-Host "  3. Push with: git push origin $PublicBranch --force"
    Write-Host "  4. Create/update the tag and GitHub release from '$PublicBranch'"
}
finally {
    if ($LeaveCurrentBranch -and -not [string]::IsNullOrWhiteSpace($originalBranch) -and -not [string]::Equals((& git branch --show-current).Trim(), $originalBranch, [StringComparison]::OrdinalIgnoreCase)) {
        Invoke-Git -Arguments @('checkout', $originalBranch)
    }

    Pop-Location
}
