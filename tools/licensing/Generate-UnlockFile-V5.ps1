[CmdletBinding()]
param()

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$outputDirectory = Join-Path $env:USERPROFILE 'OneDrive\Desktop\unlocks\v5'
$requestDirectory = Join-Path $outputDirectory 'requests'
$generatedDirectory = Join-Path $outputDirectory 'generated'
$appProjectPath = Join-Path $repoRoot 'PromptForge.App\PromptForge.App.csproj'
$appVersion = 'unknown'

function Resolve-LicenseToolPath {
    param(
        [Parameter(Mandatory = $true)]
        [string]$RepoRoot
    )

    $candidates = @(
        (Join-Path $RepoRoot 'PromptForge.LicenseTool\bin\Release\net8.0\PromptForge.LicenseTool.exe'),
        (Join-Path $RepoRoot 'PromptForge.LicenseTool\bin\Debug\net8.0\PromptForge.LicenseTool.exe')
    ) | Where-Object { Test-Path -LiteralPath $_ }

    if ($candidates.Count -eq 0) {
        return $null
    }

    return $candidates |
        Sort-Object { (Get-Item -LiteralPath $_).LastWriteTimeUtc } -Descending |
        Select-Object -First 1
}

$toolPath = Resolve-LicenseToolPath -RepoRoot $repoRoot

if (Test-Path -LiteralPath $appProjectPath) {
    try {
        [xml]$projectXml = Get-Content -LiteralPath $appProjectPath
        $propertyGroup = $projectXml.Project.PropertyGroup | Where-Object { $_.Version } | Select-Object -First 1
        if ($propertyGroup -and -not [string]::IsNullOrWhiteSpace($propertyGroup.Version)) {
            $appVersion = $propertyGroup.Version.Trim()
        }
    }
    catch {
    }
}

if (-not (Test-Path -LiteralPath $toolPath)) {
    [System.Windows.Forms.MessageBox]::Show(
        "PromptForge.LicenseTool.exe was not found at:`r`n$toolPath`r`n`r`nBuild the Release version first.",
        "Prompt Forge V5 Unlock Generator",
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Error) | Out-Null
    exit 1
}

[System.IO.Directory]::CreateDirectory($outputDirectory) | Out-Null
[System.IO.Directory]::CreateDirectory($requestDirectory) | Out-Null
[System.IO.Directory]::CreateDirectory($generatedDirectory) | Out-Null

$form = New-Object System.Windows.Forms.Form
$form.Text = "Prompt Forge $appVersion V5 Unlock Generator"
$form.StartPosition = 'CenterScreen'
$form.ClientSize = New-Object System.Drawing.Size(520, 290)
$form.FormBorderStyle = 'FixedDialog'
$form.MaximizeBox = $false
$form.MinimizeBox = $false
$form.TopMost = $true

$emailLabel = New-Object System.Windows.Forms.Label
$emailLabel.Text = 'Purchaser email'
$emailLabel.AutoSize = $true
$emailLabel.Location = New-Object System.Drawing.Point(18, 20)
$form.Controls.Add($emailLabel)

$emailBox = New-Object System.Windows.Forms.TextBox
$emailBox.Location = New-Object System.Drawing.Point(18, 44)
$emailBox.Size = New-Object System.Drawing.Size(478, 23)
$form.Controls.Add($emailBox)

$modeLabel = New-Object System.Windows.Forms.Label
$modeLabel.Text = 'License mode'
$modeLabel.AutoSize = $true
$modeLabel.Location = New-Object System.Drawing.Point(18, 82)
$form.Controls.Add($modeLabel)

$modeBox = New-Object System.Windows.Forms.ComboBox
$modeBox.Location = New-Object System.Drawing.Point(18, 106)
$modeBox.Size = New-Object System.Drawing.Size(220, 23)
$modeBox.DropDownStyle = 'DropDownList'
[void]$modeBox.Items.Add('Temporary')
[void]$modeBox.Items.Add('MachineBound')
$modeBox.SelectedIndex = 0
$form.Controls.Add($modeBox)

$entitlementLabel = New-Object System.Windows.Forms.Label
$entitlementLabel.Text = 'Entitlement profile'
$entitlementLabel.AutoSize = $true
$entitlementLabel.Location = New-Object System.Drawing.Point(276, 82)
$form.Controls.Add($entitlementLabel)

$entitlementBox = New-Object System.Windows.Forms.ComboBox
$entitlementBox.Location = New-Object System.Drawing.Point(276, 106)
$entitlementBox.Size = New-Object System.Drawing.Size(220, 23)
$entitlementBox.DropDownStyle = 'DropDownList'
[void]$entitlementBox.Items.Add('Full')
[void]$entitlementBox.Items.Add('Commercial')
[void]$entitlementBox.Items.Add('Standard')
$entitlementBox.SelectedIndex = 0
$form.Controls.Add($entitlementBox)

$requestLabel = New-Object System.Windows.Forms.Label
$requestLabel.Text = 'Request code (for MachineBound unlocks)'
$requestLabel.AutoSize = $true
$requestLabel.Location = New-Object System.Drawing.Point(18, 144)
$form.Controls.Add($requestLabel)

$requestBox = New-Object System.Windows.Forms.TextBox
$requestBox.Location = New-Object System.Drawing.Point(18, 168)
$requestBox.Size = New-Object System.Drawing.Size(478, 23)
$form.Controls.Add($requestBox)

$infoLabel = New-Object System.Windows.Forms.Label
$infoLabel.Text = 'Temporary issues a portable trusted-user unlock. MachineBound signs the request code for this specific machine. Entitlement profile is included in the signed payload.'
$infoLabel.AutoSize = $false
$infoLabel.Size = New-Object System.Drawing.Size(478, 38)
$infoLabel.Location = New-Object System.Drawing.Point(18, 202)
$form.Controls.Add($infoLabel)

$okButton = New-Object System.Windows.Forms.Button
$okButton.Text = 'Continue'
$okButton.Location = New-Object System.Drawing.Point(336, 245)
$okButton.Size = New-Object System.Drawing.Size(80, 28)
$okButton.DialogResult = [System.Windows.Forms.DialogResult]::OK
$form.Controls.Add($okButton)

$cancelButton = New-Object System.Windows.Forms.Button
$cancelButton.Text = 'Cancel'
$cancelButton.Location = New-Object System.Drawing.Point(426, 245)
$cancelButton.Size = New-Object System.Drawing.Size(70, 28)
$cancelButton.DialogResult = [System.Windows.Forms.DialogResult]::Cancel
$form.Controls.Add($cancelButton)

$form.AcceptButton = $okButton
$form.CancelButton = $cancelButton

if ($form.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) {
    exit 0
}

$email = $emailBox.Text.Trim()
$mode = [string]$modeBox.SelectedItem
$entitlement = [string]$entitlementBox.SelectedItem
$requestCode = $requestBox.Text.Trim()

if ([string]::IsNullOrWhiteSpace($email)) {
    [System.Windows.Forms.MessageBox]::Show(
        'Please enter an email address.',
        'Prompt Forge V5 Unlock Generator',
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Warning) | Out-Null
    exit 1
}

$timestamp = [DateTime]::UtcNow.ToString('yyyy-MM-ddTHH:mm:ssZ')
$arguments = @(
    '--email', $email,
    '--issued-utc', $timestamp,
    '--mode', $mode,
    '--entitlement-profile', $entitlement
)

if ($mode -eq 'MachineBound') {
    if ([string]::IsNullOrWhiteSpace($requestCode)) {
        [System.Windows.Forms.MessageBox]::Show(
            'MachineBound mode needs a request code. Paste the request code and try again.',
            'Prompt Forge V5 Unlock Generator',
            [System.Windows.Forms.MessageBoxButtons]::OK,
            [System.Windows.Forms.MessageBoxIcon]::Warning) | Out-Null
        exit 1
    }

    $arguments += @('--request-code', $requestCode)
}

Push-Location $generatedDirectory
try {
    $output = & $toolPath @arguments 2>&1
    $exitCode = $LASTEXITCODE
}
finally {
    Pop-Location
}

if ($exitCode -ne 0) {
    [System.Windows.Forms.MessageBox]::Show(
        ($output | Out-String),
        'Prompt Forge V5 Unlock Generator',
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Error) | Out-Null
    exit $exitCode
}

$message = @(
    "Generated a V5 unlock in:"
    $generatedDirectory
    ''
    "Mode: $mode"
    "Entitlement profile: $entitlement"
    ''
    ($output | Out-String).Trim()
) -join "`r`n"

[System.Windows.Forms.MessageBox]::Show(
    $message,
    'Prompt Forge V5 Unlock Generator',
    [System.Windows.Forms.MessageBoxButtons]::OK,
    [System.Windows.Forms.MessageBoxIcon]::Information) | Out-Null
