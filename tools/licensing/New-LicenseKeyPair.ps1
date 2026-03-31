[CmdletBinding()]
param(
    [string]$PrivateKeyPath,
    [string]$PublicKeyPath,
    [switch]$Force
)

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path (Split-Path $scriptRoot -Parent) -Parent

if (-not $PrivateKeyPath) {
    $PrivateKeyPath = Join-Path $repoRoot 'tools\licensing\private\license-private-key.pem'
}

if (-not $PublicKeyPath) {
    $PublicKeyPath = Join-Path $repoRoot 'PromptForge.Core\Data\promptforge-license-public.pem'
}

if (((Test-Path -LiteralPath $PrivateKeyPath) -or (Test-Path -LiteralPath $PublicKeyPath)) -and -not $Force) {
    throw "Key output already exists. Re-run with -Force only if you are intentionally rotating Prompt Forge seller keys."
}

$privateKeyDirectory = Split-Path -Parent $PrivateKeyPath
$publicKeyDirectory = Split-Path -Parent $PublicKeyPath

if ($privateKeyDirectory) {
    [System.IO.Directory]::CreateDirectory($privateKeyDirectory) | Out-Null
}

if ($publicKeyDirectory) {
    [System.IO.Directory]::CreateDirectory($publicKeyDirectory) | Out-Null
}

$licenseToolProject = Join-Path $repoRoot 'PromptForge.LicenseTool\PromptForge.LicenseTool.csproj'
$arguments = @(
    'run',
    '--project', $licenseToolProject,
    '--configuration', 'Release',
    '--',
    '--generate-keypair',
    '--private-key-output', $PrivateKeyPath,
    '--public-key-output', $PublicKeyPath
)

$output = & dotnet @arguments 2>&1
$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    throw ($output | Out-String)
}

Write-Host ($output | Out-String)
