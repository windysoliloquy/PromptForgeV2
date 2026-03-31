[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$TargetPath,

    [string]$MetadataPath,

    [string]$TimestampUrl = 'http://timestamp.acs.microsoft.com'
)

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$signToolPath = Join-Path $scriptRoot 'packages\Microsoft.Windows.SDK.BuildTools\bin\10.0.26100.0\x64\signtool.exe'
$dlibPath = Join-Path $scriptRoot 'packages\Microsoft.ArtifactSigning.Client\bin\x64\Azure.CodeSigning.Dlib.dll'

if (-not $MetadataPath) {
    $MetadataPath = Join-Path $scriptRoot 'metadata.json'
}

$resolvedTargetPath = (Resolve-Path -LiteralPath $TargetPath).Path

if (-not (Test-Path -LiteralPath $signToolPath)) {
    throw "SignTool not found at '$signToolPath'."
}

if (-not (Test-Path -LiteralPath $dlibPath)) {
    throw "Artifact Signing dlib not found at '$dlibPath'."
}

if (-not (Test-Path -LiteralPath $MetadataPath)) {
    throw "Trusted Signing metadata file not found at '$MetadataPath'. Copy metadata.template.json to metadata.json and fill in your Azure values first."
}

& $signToolPath sign /v /debug /fd SHA256 /tr $TimestampUrl /td SHA256 /dlib $dlibPath /dmdf $MetadataPath $resolvedTargetPath
$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    throw "SignTool failed with exit code $exitCode while signing '$resolvedTargetPath'."
}
