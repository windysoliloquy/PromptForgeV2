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
3. Confirm Azure CLI is installed and logged in with the signing identity before signing.
4. Confirm Inno Setup 6 is installed. Check Program Files first, then LocalAppData\Programs\Inno Setup 6 before assuming it is missing.
5. Run this script from the repo root or by full path.

Release order used here:
1. Publish self-contained Release output for win-x64.
2. Validate the staged publish folder is self-contained.
3. Sign shipped app binaries in artifacts\publish\PromptForge-win-x64.
4. Build the installer from tools\installer\PromptForge.iss.
5. Sign the generated installer exe.

Typical command:
powershell -ExecutionPolicy Bypass -File ".\tools\installer\Build-SignedRelease.ps1"
#>

$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$appProjectPath = Join-Path $repoRoot 'PromptForge.App\PromptForge.App.csproj'
$issPath = Join-Path $PSScriptRoot 'PromptForge.iss'
$signScript = Join-Path $repoRoot 'tools\trusted-signing\Sign-Artifact.ps1'

if (-not $SourceDir) {
    $SourceDir = Join-Path $repoRoot 'artifacts\publish\PromptForge-win-x64'
}

if (-not $OutputDir) {
    $OutputDir = Join-Path $repoRoot 'artifacts\installer'
}

if (-not $MetadataPath) {
    $MetadataPath = Join-Path $repoRoot 'tools\trusted-signing\metadata.json'
}

function Get-PowerShellExePath {
    $candidates = @(
        (Join-Path $PSHOME 'powershell.exe'),
        (Join-Path $env:WINDIR 'System32\WindowsPowerShell\v1.0\powershell.exe')
    ) | Where-Object { $_ -and (Test-Path -LiteralPath $_) }

    if ($candidates.Count -gt 0) {
        return $candidates[0]
    }

    throw 'powershell.exe was not found.'
}

function Get-DotNetExePath {
    $command = Get-Command 'dotnet' -ErrorAction SilentlyContinue
    if ($command) {
        return $command.Source
    }

    throw 'dotnet was not found in PATH.'
}

function Invoke-NativeProcess {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [string[]]$ArgumentList = @(),

        [string]$FailureMessage
    )

    $resolvedFilePath = (Resolve-Path -LiteralPath $FilePath).Path
    & $resolvedFilePath @ArgumentList
    if ($LASTEXITCODE -ne 0) {
        if ([string]::IsNullOrWhiteSpace($FailureMessage)) {
            throw "Process '$resolvedFilePath' failed with exit code $LASTEXITCODE."
        }

        throw ($FailureMessage -f $LASTEXITCODE)
    }
}

function Get-InnoSetupCompilerPath {
    $actualLocalAppData = [Environment]::GetFolderPath([Environment+SpecialFolder]::LocalApplicationData)
    $candidates = @(@(
        (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe'),
        (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe'),
        (Join-Path $actualLocalAppData 'Programs\Inno Setup 6\ISCC.exe'),
        (Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe')
    ) | Where-Object { $_ -and (Test-Path -LiteralPath $_) })

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
    $powerShellExePath = Get-PowerShellExePath
    Invoke-NativeProcess -FilePath $powerShellExePath -ArgumentList @(
        '-ExecutionPolicy', 'Bypass',
        '-File', $signScript,
        '-TargetPath', $TargetPath,
        '-MetadataPath', $MetadataPath
    ) -FailureMessage "Signing failed for '$TargetPath' with exit code {0}."
}

function Assert-SelfContainedPublishDirectory {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PublishDirectory
    )

    $resolvedPublishDirectory = (Resolve-Path -LiteralPath $PublishDirectory).Path
    $runtimeConfigPath = Join-Path $resolvedPublishDirectory 'PromptForge.App.runtimeconfig.json'
    $exePath = Join-Path $resolvedPublishDirectory 'PromptForge.App.exe'

    if (-not (Test-Path -LiteralPath $exePath)) {
        throw "Publish output is missing PromptForge.App.exe: '$exePath'."
    }

    if (-not (Test-Path -LiteralPath $runtimeConfigPath)) {
        throw "Publish output is missing PromptForge.App.runtimeconfig.json: '$runtimeConfigPath'."
    }

    $runtimeConfig = Get-Content -LiteralPath $runtimeConfigPath -Raw | ConvertFrom-Json
    $runtimeOptions = $runtimeConfig.runtimeOptions

    if (-not $runtimeOptions) {
        throw "Runtime config was missing runtimeOptions: '$runtimeConfigPath'."
    }

    if ($runtimeOptions.framework -or (($runtimeOptions.frameworks | Measure-Object).Count -gt 0)) {
        throw "Release output is framework-dependent. Re-run publish as self-contained before packaging."
    }

    if ((($runtimeOptions.includedFrameworks | Measure-Object).Count -eq 0)) {
        throw "Release output is not a valid self-contained publish. runtimeconfig.json did not include bundled frameworks."
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
    Write-Host "Publishing Prompt Forge ($Configuration, win-x64, self-contained)..."
    $dotNetExePath = Get-DotNetExePath
    Invoke-NativeProcess -FilePath $dotNetExePath -ArgumentList @(
        'publish',
        $appProjectPath,
        '-c', $Configuration,
        '-r', 'win-x64',
        '--self-contained', 'true',
        '-p:PublishSingleFile=false',
        '-o', $SourceDir
    ) -FailureMessage 'dotnet publish failed with exit code {0}.'
}

if (-not (Test-Path -LiteralPath $SourceDir)) {
    throw "SourceDir was not found: '$SourceDir'."
}

Assert-SelfContainedPublishDirectory -PublishDirectory $SourceDir

if (-not $SkipBinarySigning) {
    foreach ($target in $binaryTargets) {
        Invoke-Signing -TargetPath $target
    }
}

New-Item -ItemType Directory -Path $OutputDir -Force | Out-Null
$isccPath = Get-InnoSetupCompilerPath

Write-Host 'Building installer...'
$isccArguments = @(
    "/DSourceDir=$SourceDir",
    "/DOutputDir=$OutputDir",
    "/DAppVersion=$appVersion",
    "/DSetupBaseName=$setupBaseName",
    $issPath
)
$quotedIsccArguments = ($isccArguments | ForEach-Object {
    if ($_ -match '\s') { '"{0}"' -f $_ } else { $_ }
}) -join ' '
$cmdExePath = Join-Path $env:WINDIR 'System32\cmd.exe'
$tempBatchPath = Join-Path ([System.IO.Path]::GetTempPath()) ("prompt-forge-iscc-{0}.cmd" -f [guid]::NewGuid().ToString('N'))
@(
    '@echo off',
    ('"{0}" {1}' -f $isccPath, $quotedIsccArguments)
) | Set-Content -LiteralPath $tempBatchPath -Encoding ASCII

try {
    & $cmdExePath /c $tempBatchPath
    if ($LASTEXITCODE -ne 0) {
        throw "Installer build failed with exit code $LASTEXITCODE."
    }
}
finally {
    Remove-Item -LiteralPath $tempBatchPath -Force -ErrorAction SilentlyContinue
}

$installerPath = Join-Path $OutputDir "$setupBaseName.exe"
if (-not $SkipInstallerSigning) {
    Invoke-Signing -TargetPath $installerPath
}

Write-Host "Signed release ready: $installerPath"
