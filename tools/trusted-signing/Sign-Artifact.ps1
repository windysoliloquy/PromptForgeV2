[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$TargetPath,

    [string]$MetadataPath,

    [int]$MaxAttemptsPerTimestamp = 3,

    [int]$RetryDelaySeconds = 5,

    [string[]]$TimestampUrls = @(
        'http://timestamp.acs.microsoft.com',
        'http://timestamp.digicert.com',
        'http://timestamp.sectigo.com'
    )
)

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$signToolPath = Join-Path $scriptRoot 'packages\Microsoft.Windows.SDK.BuildTools\bin\10.0.26100.0\x64\signtool.exe'
$dlibPath = Join-Path $scriptRoot 'packages\Microsoft.ArtifactSigning.Client\bin\x64\Azure.CodeSigning.Dlib.dll'
$signToolPackageDirectory = Split-Path -Parent $signToolPath
$dlibPackageDirectory = Split-Path -Parent $dlibPath

if (-not $MetadataPath) {
    $MetadataPath = Join-Path $scriptRoot 'metadata.json'
}

$resolvedTargetPath = (Resolve-Path -LiteralPath $TargetPath).Path

function Get-AzureCliPath {
    $candidates = @(
        (Join-Path $env:ProgramFiles 'Microsoft SDKs\Azure\CLI2\wbin\az.cmd'),
        (Join-Path ${env:ProgramFiles(x86)} 'Microsoft SDKs\Azure\CLI2\wbin\az.cmd')
    ) | Where-Object { $_ -and (Test-Path -LiteralPath $_) }

    if ($candidates.Count -gt 0) {
        return $candidates[0]
    }

    $command = Get-Command 'az' -ErrorAction SilentlyContinue
    if ($command) {
        return $command.Source
    }

    return $null
}

function Restore-TrustedSigningRuntime {
    $nugetRoot = Join-Path $env:USERPROFILE '.nuget\packages'
    $signToolSourceDirectory = Join-Path $nugetRoot 'microsoft.windows.sdk.buildtools\10.0.26100.7705\bin\10.0.26100.0\x64'
    $dlibSourceDirectory = Join-Path $nugetRoot 'microsoft.artifactsigning.client\1.0.115\bin\x64'

    if ((-not (Test-Path -LiteralPath $signToolPath)) -and (Test-Path -LiteralPath $signToolSourceDirectory)) {
        New-Item -ItemType Directory -Path $signToolPackageDirectory -Force | Out-Null
        Copy-Item -Path (Join-Path $signToolSourceDirectory '*') -Destination $signToolPackageDirectory -Recurse -Force
    }

    if ((-not (Test-Path -LiteralPath $dlibPath)) -and (Test-Path -LiteralPath $dlibSourceDirectory)) {
        New-Item -ItemType Directory -Path $dlibPackageDirectory -Force | Out-Null
        Copy-Item -Path (Join-Path $dlibSourceDirectory '*') -Destination $dlibPackageDirectory -Recurse -Force
    }
}

function Assert-AzureCliLogin {
    $azureCliPath = Get-AzureCliPath
    if (-not $azureCliPath) {
        throw "Azure CLI was not found. Install Azure CLI and run 'az login' with the signing identity before calling Sign-Artifact.ps1."
    }

    az account show --only-show-errors | Out-Null
    if ($LASTEXITCODE -ne 0) {
        throw "Azure CLI is not logged in for Trusted Signing. Run 'az login' with the signing identity before calling Sign-Artifact.ps1."
    }
}

Restore-TrustedSigningRuntime

if (-not (Test-Path -LiteralPath $signToolPath)) {
    throw "SignTool not found at '$signToolPath'."
}

if (-not (Test-Path -LiteralPath $dlibPath)) {
    throw "Artifact Signing dlib not found at '$dlibPath'."
}

if (-not (Test-Path -LiteralPath $MetadataPath)) {
    throw "Trusted Signing metadata file not found at '$MetadataPath'. Copy metadata.template.json to metadata.json and fill in your Azure values first."
}

Assert-AzureCliLogin

$errors = [System.Collections.Generic.List[string]]::new()

foreach ($timestampUrl in $TimestampUrls) {
    for ($attempt = 1; $attempt -le $MaxAttemptsPerTimestamp; $attempt++) {
        Write-Host "Signing attempt $attempt of $MaxAttemptsPerTimestamp using timestamp server '$timestampUrl'."
        & $signToolPath sign /v /debug /fd SHA256 /tr $timestampUrl /td SHA256 /dlib $dlibPath /dmdf $MetadataPath $resolvedTargetPath
        $exitCode = $LASTEXITCODE

        if ($exitCode -eq 0) {
            Write-Host "Signed successfully using timestamp server '$timestampUrl'."
            return
        }

        $errors.Add("Timestamp server '$timestampUrl' failed with exit code $exitCode on attempt $attempt.")

        if ($attempt -lt $MaxAttemptsPerTimestamp) {
            Start-Sleep -Seconds $RetryDelaySeconds
        }
    }
}

throw ("SignTool failed for '{0}' after trying all configured timestamp servers.{1}{2}" -f $resolvedTargetPath, [Environment]::NewLine, ($errors -join [Environment]::NewLine))
