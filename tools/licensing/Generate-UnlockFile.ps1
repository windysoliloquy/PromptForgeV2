[CmdletBinding()]
param()

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$repoRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
$toolPath = Join-Path $repoRoot 'PromptForge.LicenseTool\bin\Release\net8.0\PromptForge.LicenseTool.exe'
$outputDirectory = Join-Path $env:USERPROFILE 'OneDrive\Desktop\unlocks'

if (-not (Test-Path -LiteralPath $toolPath)) {
    [System.Windows.Forms.MessageBox]::Show(
        "PromptForge.LicenseTool.exe was not found at:`r`n$toolPath`r`n`r`nBuild the Release version first.",
        "Prompt Forge Unlock Generator",
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Error) | Out-Null
    exit 1
}

$form = New-Object System.Windows.Forms.Form
$form.Text = 'Prompt Forge Unlock Generator'
$form.StartPosition = 'CenterScreen'
$form.ClientSize = New-Object System.Drawing.Size(430, 155)
$form.FormBorderStyle = 'FixedDialog'
$form.MaximizeBox = $false
$form.MinimizeBox = $false
$form.TopMost = $true

$label = New-Object System.Windows.Forms.Label
$label.Text = 'Purchaser email'
$label.AutoSize = $true
$label.Location = New-Object System.Drawing.Point(18, 20)
$form.Controls.Add($label)

$textBox = New-Object System.Windows.Forms.TextBox
$textBox.Location = New-Object System.Drawing.Point(18, 48)
$textBox.Size = New-Object System.Drawing.Size(390, 23)
$form.Controls.Add($textBox)

$okButton = New-Object System.Windows.Forms.Button
$okButton.Text = 'Generate Unlock'
$okButton.Location = New-Object System.Drawing.Point(218, 100)
$okButton.Size = New-Object System.Drawing.Size(110, 28)
$okButton.DialogResult = [System.Windows.Forms.DialogResult]::OK
$form.Controls.Add($okButton)

$cancelButton = New-Object System.Windows.Forms.Button
$cancelButton.Text = 'Cancel'
$cancelButton.Location = New-Object System.Drawing.Point(338, 100)
$cancelButton.Size = New-Object System.Drawing.Size(70, 28)
$cancelButton.DialogResult = [System.Windows.Forms.DialogResult]::Cancel
$form.Controls.Add($cancelButton)

$form.AcceptButton = $okButton
$form.CancelButton = $cancelButton

if ($form.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) {
    exit 0
}

$email = $textBox.Text.Trim()
if ([string]::IsNullOrWhiteSpace($email)) {
    [System.Windows.Forms.MessageBox]::Show(
        'Please enter an email address.',
        'Prompt Forge Unlock Generator',
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Warning) | Out-Null
    exit 1
}

[System.IO.Directory]::CreateDirectory($outputDirectory) | Out-Null

$timestamp = [DateTime]::UtcNow.ToString('yyyy-MM-ddTHH:mm:ssZ')
$arguments = @(
    '--email', $email,
    '--issued-utc', $timestamp
)

Push-Location $outputDirectory
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
        'Prompt Forge Unlock Generator',
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Error) | Out-Null
    exit $exitCode
}

[System.Windows.Forms.MessageBox]::Show(
    ($output | Out-String),
    'Prompt Forge Unlock Generator',
    [System.Windows.Forms.MessageBoxButtons]::OK,
    [System.Windows.Forms.MessageBoxIcon]::Information) | Out-Null
