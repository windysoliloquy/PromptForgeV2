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
