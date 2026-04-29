[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet('Backup', 'ClearForReleaseTest', 'Restore')]
    [string]$Mode,

    [string]$BackupRoot,
    [string]$BackupPath
)

$ErrorActionPreference = 'Stop'

<#
Prompt Forge release-state helper.

Purpose:
- protect developer-local Prompt Forge state before first-run/release testing
- clear only machine-local runtime state when explicitly requested
- restore the protected state after release verification

This script never touches source files, signing keys, installer scripts, or release
artifacts. It only works against the runtime state paths listed below.
#>

$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..')).Path

if (-not $BackupRoot) {
    $BackupRoot = Join-Path $repoRoot 'artifacts\local-state-backups'
}

$localAppData = [Environment]::GetFolderPath([Environment+SpecialFolder]::LocalApplicationData)
$roamingAppData = [Environment]::GetFolderPath([Environment+SpecialFolder]::ApplicationData)

$runtimeTargets = @(
    [PSCustomObject]@{
        Name = 'PromptForgeDemo'
        Path = Join-Path $localAppData 'PromptForgeDemo'
        Kind = 'Directory'
    },
    [PSCustomObject]@{
        Name = 'PromptForgeAppData'
        Path = Join-Path $roamingAppData 'PromptForge'
        Kind = 'Directory'
    },
    [PSCustomObject]@{
        Name = 'RepoSharedUiEventLog'
        Path = Join-Path $repoRoot 'ui-event-log.shared.txt'
        Kind = 'File'
    },
    [PSCustomObject]@{
        Name = 'PublishedUiEventLog'
        Path = Join-Path $repoRoot 'artifacts\publish\PromptForge-win-x64\ui-event-log.txt'
        Kind = 'File'
    },
    [PSCustomObject]@{
        Name = 'AppOutputUiEventLog'
        Path = Join-Path $repoRoot 'AppOutput\PromptForge\ui-event-log.txt'
        Kind = 'File'
    }
)

function New-ManifestEntry {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Target
    )

    [PSCustomObject]@{
        Name = $Target.Name
        Path = $Target.Path
        Kind = $Target.Kind
        Exists = Test-Path -LiteralPath $Target.Path
    }
}

function Write-Manifest {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Destination
    )

    $manifestPath = Join-Path $Destination 'manifest.json'
    $manifest = [PSCustomObject]@{
        CreatedAt = (Get-Date).ToString('O')
        Machine = $env:COMPUTERNAME
        User = $env:USERNAME
        RepoRoot = $repoRoot
        Targets = @($runtimeTargets | ForEach-Object { New-ManifestEntry -Target $_ })
    }

    $manifest | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $manifestPath -Encoding UTF8
}

function Copy-StateToBackup {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Target,

        [Parameter(Mandatory = $true)]
        [string]$Destination
    )

    if (-not (Test-Path -LiteralPath $Target.Path)) {
        Write-Host "Missing, skipped: $($Target.Path)"
        return
    }

    $targetDestination = Join-Path $Destination $Target.Name
    if ($Target.Kind -eq 'Directory') {
        Copy-Item -LiteralPath $Target.Path -Destination $targetDestination -Recurse -Force
        Write-Host "Backed up directory: $($Target.Path)"
        return
    }

    New-Item -ItemType Directory -Path $targetDestination -Force | Out-Null
    Copy-Item -LiteralPath $Target.Path -Destination (Join-Path $targetDestination ([IO.Path]::GetFileName($Target.Path))) -Force
    Write-Host "Backed up file: $($Target.Path)"
}

function Restore-StateFromBackup {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Target,

        [Parameter(Mandatory = $true)]
        [string]$Source
    )

    $targetBackup = Join-Path $Source $Target.Name
    if (-not (Test-Path -LiteralPath $targetBackup)) {
        Write-Host "No backup entry, skipped: $($Target.Name)"
        return
    }

    if ($Target.Kind -eq 'Directory') {
        if (Test-Path -LiteralPath $Target.Path) {
            Remove-Item -LiteralPath $Target.Path -Recurse -Force
        }

        $parent = Split-Path -Parent $Target.Path
        New-Item -ItemType Directory -Path $parent -Force | Out-Null
        Copy-Item -LiteralPath $targetBackup -Destination $Target.Path -Recurse -Force
        Write-Host "Restored directory: $($Target.Path)"
        return
    }

    $fileName = [IO.Path]::GetFileName($Target.Path)
    $sourceFile = Join-Path $targetBackup $fileName
    if (-not (Test-Path -LiteralPath $sourceFile)) {
        Write-Host "No backup file, skipped: $sourceFile"
        return
    }

    $parent = Split-Path -Parent $Target.Path
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
    Copy-Item -LiteralPath $sourceFile -Destination $Target.Path -Force
    Write-Host "Restored file: $($Target.Path)"
}

function Remove-RuntimeTarget {
    param(
        [Parameter(Mandatory = $true)]
        [object]$Target
    )

    if (-not (Test-Path -LiteralPath $Target.Path)) {
        Write-Host "Missing, skipped: $($Target.Path)"
        return
    }

    if ($PSCmdlet.ShouldProcess($Target.Path, 'Remove Prompt Forge local runtime state')) {
        if ($Target.Kind -eq 'Directory') {
            Remove-Item -LiteralPath $Target.Path -Recurse -Force
        }
        else {
            Remove-Item -LiteralPath $Target.Path -Force
        }

        Write-Host "Removed: $($Target.Path)"
    }
}

switch ($Mode) {
    'Backup' {
        $stamp = Get-Date -Format 'yyyy-MM-dd_HH-mm-ss'
        $destination = Join-Path $BackupRoot "PromptForgeLocalState_$stamp"
        New-Item -ItemType Directory -Path $destination -Force | Out-Null

        foreach ($target in $runtimeTargets) {
            Copy-StateToBackup -Target $target -Destination $destination
        }

        Write-Manifest -Destination $destination
        Write-Host "Backup complete: $destination"
    }

    'ClearForReleaseTest' {
        if (-not $BackupPath) {
            throw 'ClearForReleaseTest requires -BackupPath pointing to a completed backup folder.'
        }

        if (-not (Test-Path -LiteralPath (Join-Path $BackupPath 'manifest.json'))) {
            throw "BackupPath does not look like a completed backup folder: $BackupPath"
        }

        foreach ($target in $runtimeTargets) {
            Remove-RuntimeTarget -Target $target
        }

        Write-Host 'Local runtime state cleared for release testing.'
    }

    'Restore' {
        if (-not $BackupPath) {
            throw 'Restore requires -BackupPath pointing to a completed backup folder.'
        }

        if (-not (Test-Path -LiteralPath (Join-Path $BackupPath 'manifest.json'))) {
            throw "BackupPath does not look like a completed backup folder: $BackupPath"
        }

        foreach ($target in $runtimeTargets) {
            Restore-StateFromBackup -Target $target -Source $BackupPath
        }

        Write-Host 'Local runtime state restored.'
    }
}
