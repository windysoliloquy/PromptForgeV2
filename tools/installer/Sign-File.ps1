[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$FilePath
)

$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$signScript = Join-Path $repoRoot 'tools\trusted-signing\Sign-Artifact.ps1'
$metadataPath = Join-Path $repoRoot 'tools\trusted-signing\metadata.json'

if (-not (Test-Path -LiteralPath $signScript)) {
    throw "Trusted Signing wrapper script not found at '$signScript'."
}

& $signScript -TargetPath $FilePath -MetadataPath $metadataPath
$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    throw "Installer signing hook failed with exit code $exitCode for '$FilePath'."
}
