[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$SourceDir,
    [string]$OutputDir,
    [string]$MetadataPath,
    [switch]$SkipBuild,
    [switch]$SkipBinarySigning,
    [switch]$SkipInstallerSigning
)

$ErrorActionPreference = 'Stop'

<#
Release checklist:
1. Confirm Prompt Forge is not currently running, or the release copy step may fail on locked files.
2. Confirm trusted-signing metadata exists at tools\trusted-signing\metadata.json.
3. Confirm Inno Setup 6 is installed. Check Program Files first, then LocalAppData\Programs\Inno Setup 6 before assuming it is missing.
4. Run this script from the repo root or by full path.

Release order used here:
1. Build Release output.
2. Sign shipped app binaries in AppOutput\PromptForge.
3. Build the installer from tools\installer\PromptForge.iss.
4. Sign the generated installer exe.

Typical command:
powershell -ExecutionPolicy Bypass -File ".\tools\installer\Build-SignedRelease.ps1"
#>

$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$solutionPath = Join-Path $repoRoot 'PromptForge.sln'
$appProjectPath = Join-Path $repoRoot 'PromptForge.App\PromptForge.App.csproj'
$issPath = Join-Path $PSScriptRoot 'PromptForge.iss'
$signScript = Join-Path $repoRoot 'tools\trusted-signing\Sign-Artifact.ps1'

if (-not $SourceDir) {
    $SourceDir = Join-Path $repoRoot 'AppOutput\PromptForge'
}

if (-not $OutputDir) {
    $OutputDir = Join-Path $repoRoot 'artifacts\installer'
}

if (-not $MetadataPath) {
    $MetadataPath = Join-Path $repoRoot 'tools\trusted-signing\metadata.json'
}

function Get-InnoSetupCompilerPath {
    $candidates = @(
        (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
        (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe'),
        (Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe')
    ) | Where-Object { $_ -and (Test-Path -LiteralPath $_) }

    if ($candidates.Count -gt 0) {
        return $candidates[0]
    }

    $command = Get-Command 'ISCC.exe' -ErrorAction SilentlyContinue
    if ($command) {
        return $command.Source
    }

    throw 'ISCC.exe was not found. Install Inno Setup 6 or add ISCC.exe to PATH.'
}

function Get-AppVersion {
    [xml]$projectXml = Get-Content -LiteralPath $appProjectPath
    $propertyGroup = $projectXml.Project.PropertyGroup | Where-Object { $_.Version } | Select-Object -First 1
    if (-not $propertyGroup -or [string]::IsNullOrWhiteSpace($propertyGroup.Version)) {
        throw "Could not resolve Version from '$appProjectPath'."
    }

    return $propertyGroup.Version.Trim()
}

function Invoke-Signing {
    param(
        [Parameter(Mandatory = $true)]
        [string]$TargetPath
    )

    if (-not (Test-Path -LiteralPath $TargetPath)) {
        throw "Signing target was not found: '$TargetPath'."
    }

    Write-Host "Signing $TargetPath"
    & powershell -ExecutionPolicy Bypass -File $signScript -TargetPath $TargetPath -MetadataPath $MetadataPath
    if ($LASTEXITCODE -ne 0) {
        throw "Signing failed for '$TargetPath' with exit code $LASTEXITCODE."
    }
}

$appVersion = Get-AppVersion
$setupBaseName = "PromptForge-$appVersion-Setup"
$binaryTargets = @(
    (Join-Path $SourceDir 'PromptForge.App.exe'),
    (Join-Path $SourceDir 'PromptForge.App.dll'),
    (Join-Path $SourceDir 'PromptForge.Core.dll')
)

if (-not $SkipBuild) {
    Write-Host "Building solution ($Configuration)..."
    & dotnet build $solutionPath -c $Configuration
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet build failed with exit code $LASTEXITCODE."
    }
}

if (-not (Test-Path -LiteralPath $SourceDir)) {
    throw "SourceDir was not found: '$SourceDir'."
}

if (-not $SkipBinarySigning) {
    foreach ($target in $binaryTargets) {
        Invoke-Signing -TargetPath $target
    }
}

New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
$isccPath = Get-InnoSetupCompilerPath

Write-Host 'Building installer...'
& $isccPath "/DSourceDir=$SourceDir" "/DOutputDir=$OutputDir" "/DAppVersion=$appVersion" "/DSetupBaseName=$setupBaseName" $issPath
if ($LASTEXITCODE -ne 0) {
    throw "Installer build failed with exit code $LASTEXITCODE."
}

$installerPath = Join-Path $OutputDir "$setupBaseName.exe"
if (-not $SkipInstallerSigning) {
    Invoke-Signing -TargetPath $installerPath
}

Write-Host "Signed release ready: $installerPath"
