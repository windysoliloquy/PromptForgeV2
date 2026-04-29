[CmdletBinding()]
param()

Add-Type -AssemblyName System.Windows.Forms
Add-Type -AssemblyName System.Drawing

$repoRoot = $env:PROMPTFORGE_REPO_ROOT
if ([string]::IsNullOrWhiteSpace($repoRoot)) {
    $repoRoot = 'C:\Users\windy\OneDrive\Desktop\codex\Prompt Forge'
}

$outputDirectory = 'C:\Users\windy\OneDrive\Desktop\unlocks\v5.1'
$generatedDirectory = Join-Path $outputDirectory 'generated'
$appProjectPath = Join-Path $repoRoot 'PromptForge.App\PromptForge.App.csproj'
$appVersion = 'unknown'

$premiumLanes = @(
    'Anime',
    'Architecture / Archviz',
    "Children's Book",
    'Comic Book',
    'Concept Art',
    'Editorial Illustration',
    'Fantasy Illustration',
    'Food Photography',
    'Infographic / Data Visualization',
    'Lifestyle / Advertising Photography',
    'Product Photography',
    'Tattoo Art',
    '3D Render',
    'Vintage Bend'
)

$licenseTypes = @(
    [PSCustomObject]@{
        Label = 'App Unlock - includes 1 premium lane'
        Profile = 'AppUnlock'
        MinLanes = 1
        MaxLanes = 1
        AutoAllPremium = $false
        Description = 'One-time app unlock with exactly one permanent premium lane entitlement.'
    },
    [PSCustomObject]@{
        Label = 'Monthly Group - includes 1 premium lane'
        Profile = 'Monthly'
        MinLanes = 1
        MaxLanes = 1
        AutoAllPremium = $false
        Description = 'Monthly group access profile with exactly one permanent premium lane entitlement.'
    },
    [PSCustomObject]@{
        Label = 'Quarterly Group - includes 5 premium lanes'
        Profile = 'Quarterly'
        MinLanes = 5
        MaxLanes = 5
        AutoAllPremium = $false
        Description = 'Quarterly group access profile with exactly five permanent premium lane entitlements.'
    },
    [PSCustomObject]@{
        Label = 'Yearly - all premium lanes'
        Profile = 'Yearly'
        MinLanes = 0
        MaxLanes = 0
        AutoAllPremium = $true
        Description = 'Yearly profile with all current premium lanes included.'
    },
    [PSCustomObject]@{
        Label = 'Single Premium Lane'
        Profile = 'SingleLane'
        MinLanes = 1
        MaxLanes = 1
        AutoAllPremium = $false
        Description = 'Standalone permanent entitlement for exactly one premium lane.'
    },
    [PSCustomObject]@{
        Label = 'Lane Pack - custom selection'
        Profile = 'LanePack'
        MinLanes = 1
        MaxLanes = 0
        AutoAllPremium = $false
        Description = 'Permanent entitlement for the selected premium lanes.'
    },
    [PSCustomObject]@{
        Label = 'Standard Full - app only'
        Profile = 'Full'
        MinLanes = 0
        MaxLanes = 0
        AutoAllPremium = $false
        Description = 'Full app unlock profile with no lane entitlements attached.'
    },
    [PSCustomObject]@{
        Label = 'Commercial - app only'
        Profile = 'Commercial'
        MinLanes = 0
        MaxLanes = 0
        AutoAllPremium = $false
        Description = 'Commercial profile with no lane entitlements attached unless lanes are added later.'
    }
)

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

function Get-SelectedLanes {
    param(
        [Parameter(Mandatory = $true)]
        [System.Windows.Forms.CheckedListBox]$LaneList
    )

    $lanes = New-Object System.Collections.Generic.List[string]
    foreach ($item in $LaneList.CheckedItems) {
        $lanes.Add([string]$item)
    }

    return $lanes
}

function Set-AllLaneChecks {
    param(
        [Parameter(Mandatory = $true)]
        [System.Windows.Forms.CheckedListBox]$LaneList,
        [Parameter(Mandatory = $true)]
        [bool]$Checked
    )

    for ($index = 0; $index -lt $LaneList.Items.Count; $index++) {
        $LaneList.SetItemChecked($index, $Checked)
    }
}

function Sync-LicenseTypeState {
    param(
        [Parameter(Mandatory = $true)]
        [System.Windows.Forms.ComboBox]$TypeBox,
        [Parameter(Mandatory = $true)]
        [System.Windows.Forms.CheckedListBox]$LaneList,
        [Parameter(Mandatory = $true)]
        [System.Windows.Forms.Label]$DescriptionLabel
    )

    $selectedType = $TypeBox.SelectedItem
    if ($null -eq $selectedType) {
        return
    }

    $DescriptionLabel.Text = $selectedType.Description

    if ($selectedType.AutoAllPremium) {
        Set-AllLaneChecks -LaneList $LaneList -Checked $true
        $LaneList.Enabled = $false
        return
    }

    if ($selectedType.MinLanes -eq 0 -and $selectedType.MaxLanes -eq 0) {
        Set-AllLaneChecks -LaneList $LaneList -Checked $false
        $LaneList.Enabled = $false
        return
    }

    $LaneList.Enabled = $true
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
        "PromptForge.LicenseTool.exe was not found.`r`n`r`nRepo root:`r`n$repoRoot`r`n`r`nBuild PromptForge.LicenseTool first, or set PROMPTFORGE_REPO_ROOT.",
        "Prompt Forge V5.1 Unlock Generator",
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Error) | Out-Null
    exit 1
}

[System.IO.Directory]::CreateDirectory($outputDirectory) | Out-Null
[System.IO.Directory]::CreateDirectory($generatedDirectory) | Out-Null

$form = New-Object System.Windows.Forms.Form
$form.Text = "Prompt Forge $appVersion V5.1 Unlock Generator"
$form.StartPosition = 'CenterScreen'
$form.ClientSize = New-Object System.Drawing.Size(760, 610)
$form.FormBorderStyle = 'FixedDialog'
$form.MaximizeBox = $false
$form.MinimizeBox = $false
$form.TopMost = $true

$emailLabel = New-Object System.Windows.Forms.Label
$emailLabel.Text = 'Purchaser email'
$emailLabel.AutoSize = $true
$emailLabel.Location = New-Object System.Drawing.Point(18, 18)
$form.Controls.Add($emailLabel)

$emailBox = New-Object System.Windows.Forms.TextBox
$emailBox.Location = New-Object System.Drawing.Point(18, 42)
$emailBox.Size = New-Object System.Drawing.Size(350, 23)
$form.Controls.Add($emailBox)

$modeLabel = New-Object System.Windows.Forms.Label
$modeLabel.Text = 'License mode'
$modeLabel.AutoSize = $true
$modeLabel.Location = New-Object System.Drawing.Point(392, 18)
$form.Controls.Add($modeLabel)

$modeBox = New-Object System.Windows.Forms.ComboBox
$modeBox.Location = New-Object System.Drawing.Point(392, 42)
$modeBox.Size = New-Object System.Drawing.Size(160, 23)
$modeBox.DropDownStyle = 'DropDownList'
[void]$modeBox.Items.Add('Temporary')
[void]$modeBox.Items.Add('MachineBound')
$modeBox.SelectedIndex = 0
$form.Controls.Add($modeBox)

$licenseTypeLabel = New-Object System.Windows.Forms.Label
$licenseTypeLabel.Text = 'License type'
$licenseTypeLabel.AutoSize = $true
$licenseTypeLabel.Location = New-Object System.Drawing.Point(18, 82)
$form.Controls.Add($licenseTypeLabel)

$licenseTypeBox = New-Object System.Windows.Forms.ComboBox
$licenseTypeBox.Location = New-Object System.Drawing.Point(18, 106)
$licenseTypeBox.Size = New-Object System.Drawing.Size(350, 23)
$licenseTypeBox.DropDownStyle = 'DropDownList'
$licenseTypeBox.DisplayMember = 'Label'
foreach ($licenseType in $licenseTypes) {
    [void]$licenseTypeBox.Items.Add($licenseType)
}
$licenseTypeBox.SelectedIndex = 0
$form.Controls.Add($licenseTypeBox)

$requestLabel = New-Object System.Windows.Forms.Label
$requestLabel.Text = 'Product / request code from their computer'
$requestLabel.AutoSize = $true
$requestLabel.Location = New-Object System.Drawing.Point(392, 82)
$form.Controls.Add($requestLabel)

$requestBox = New-Object System.Windows.Forms.TextBox
$requestBox.Location = New-Object System.Drawing.Point(392, 106)
$requestBox.Size = New-Object System.Drawing.Size(340, 23)
$form.Controls.Add($requestBox)

$descriptionLabel = New-Object System.Windows.Forms.Label
$descriptionLabel.AutoSize = $false
$descriptionLabel.Size = New-Object System.Drawing.Size(714, 38)
$descriptionLabel.Location = New-Object System.Drawing.Point(18, 142)
$form.Controls.Add($descriptionLabel)

$laneLabel = New-Object System.Windows.Forms.Label
$laneLabel.Text = 'Premium lane entitlements'
$laneLabel.AutoSize = $true
$laneLabel.Location = New-Object System.Drawing.Point(18, 190)
$form.Controls.Add($laneLabel)

$laneList = New-Object System.Windows.Forms.CheckedListBox
$laneList.Location = New-Object System.Drawing.Point(18, 214)
$laneList.Size = New-Object System.Drawing.Size(350, 300)
$laneList.CheckOnClick = $true
foreach ($lane in $premiumLanes) {
    [void]$laneList.Items.Add($lane)
}
$form.Controls.Add($laneList)

$selectAllButton = New-Object System.Windows.Forms.Button
$selectAllButton.Text = 'Select all'
$selectAllButton.Location = New-Object System.Drawing.Point(18, 524)
$selectAllButton.Size = New-Object System.Drawing.Size(80, 28)
$selectAllButton.Add_Click({ Set-AllLaneChecks -LaneList $laneList -Checked $true })
$form.Controls.Add($selectAllButton)

$clearButton = New-Object System.Windows.Forms.Button
$clearButton.Text = 'Clear'
$clearButton.Location = New-Object System.Drawing.Point(106, 524)
$clearButton.Size = New-Object System.Drawing.Size(80, 28)
$clearButton.Add_Click({ Set-AllLaneChecks -LaneList $laneList -Checked $false })
$form.Controls.Add($clearButton)

$notesBox = New-Object System.Windows.Forms.TextBox
$notesBox.Location = New-Object System.Drawing.Point(392, 214)
$notesBox.Size = New-Object System.Drawing.Size(340, 300)
$notesBox.Multiline = $true
$notesBox.ReadOnly = $true
$notesBox.ScrollBars = 'Vertical'
$notesBox.Text = @"
V5.1 generator notes

- Temporary creates a portable signed unlock.
- MachineBound signs the product/request code pasted from the buyer's machine.
- Lane selections are written as signed AllowedLanes.
- App import can merge repeated valid signed files.
- Yearly automatically includes all current premium lanes.
- This tool does not rotate keys.
"@
$form.Controls.Add($notesBox)

$statusLabel = New-Object System.Windows.Forms.Label
$statusLabel.AutoSize = $false
$statusLabel.Size = New-Object System.Drawing.Size(714, 28)
$statusLabel.Location = New-Object System.Drawing.Point(18, 560)
$statusLabel.Text = "Output folder: $generatedDirectory"
$form.Controls.Add($statusLabel)

$generateButton = New-Object System.Windows.Forms.Button
$generateButton.Text = 'Forge Unlock'
$generateButton.Location = New-Object System.Drawing.Point(552, 524)
$generateButton.Size = New-Object System.Drawing.Size(100, 28)
$form.Controls.Add($generateButton)

$cancelButton = New-Object System.Windows.Forms.Button
$cancelButton.Text = 'Close'
$cancelButton.Location = New-Object System.Drawing.Point(662, 524)
$cancelButton.Size = New-Object System.Drawing.Size(70, 28)
$cancelButton.DialogResult = [System.Windows.Forms.DialogResult]::Cancel
$form.Controls.Add($cancelButton)

$form.CancelButton = $cancelButton

$licenseTypeBox.Add_SelectedIndexChanged({
    Sync-LicenseTypeState -TypeBox $licenseTypeBox -LaneList $laneList -DescriptionLabel $descriptionLabel
})

$generateButton.Add_Click({
    $email = $emailBox.Text.Trim()
    $mode = [string]$modeBox.SelectedItem
    $requestCode = $requestBox.Text.Trim()
    $selectedType = $licenseTypeBox.SelectedItem

    if ([string]::IsNullOrWhiteSpace($email)) {
        [System.Windows.Forms.MessageBox]::Show(
            'Please enter the purchaser email.',
            'Prompt Forge V5.1 Unlock Generator',
            [System.Windows.Forms.MessageBoxButtons]::OK,
            [System.Windows.Forms.MessageBoxIcon]::Warning) | Out-Null
        return
    }

    if ($mode -eq 'MachineBound' -and [string]::IsNullOrWhiteSpace($requestCode)) {
        [System.Windows.Forms.MessageBox]::Show(
            'MachineBound unlocks need the product/request code from the buyer computer.',
            'Prompt Forge V5.1 Unlock Generator',
            [System.Windows.Forms.MessageBoxButtons]::OK,
            [System.Windows.Forms.MessageBoxIcon]::Warning) | Out-Null
        return
    }

    if ($selectedType.AutoAllPremium) {
        Set-AllLaneChecks -LaneList $laneList -Checked $true
    }

    $selectedLanes = Get-SelectedLanes -LaneList $laneList
    if ($selectedType.MinLanes -gt 0 -and $selectedLanes.Count -lt $selectedType.MinLanes) {
        [System.Windows.Forms.MessageBox]::Show(
            "This license type needs at least $($selectedType.MinLanes) selected premium lane(s).",
            'Prompt Forge V5.1 Unlock Generator',
            [System.Windows.Forms.MessageBoxButtons]::OK,
            [System.Windows.Forms.MessageBoxIcon]::Warning) | Out-Null
        return
    }

    if ($selectedType.MaxLanes -gt 0 -and $selectedLanes.Count -gt $selectedType.MaxLanes) {
        [System.Windows.Forms.MessageBox]::Show(
            "This license type allows at most $($selectedType.MaxLanes) selected premium lane(s).",
            'Prompt Forge V5.1 Unlock Generator',
            [System.Windows.Forms.MessageBoxButtons]::OK,
            [System.Windows.Forms.MessageBoxIcon]::Warning) | Out-Null
        return
    }

    $timestamp = [DateTime]::UtcNow.ToString('yyyy-MM-ddTHH-mm-ssZ')
    $safeEmail = ($email -replace '[^a-zA-Z0-9._-]', '_')
    $safeProfile = ($selectedType.Profile -replace '[^a-zA-Z0-9._-]', '_')
    $outputPath = Join-Path $generatedDirectory "PromptForge-$safeProfile-$safeEmail-$timestamp.json"

    $arguments = @(
        '--email', $email,
        '--issued-utc', ([DateTime]::UtcNow.ToString('yyyy-MM-ddTHH:mm:ssZ')),
        '--mode', $mode,
        '--entitlement-profile', $selectedType.Profile,
        '--output', $outputPath
    )

    if ($mode -eq 'MachineBound') {
        $arguments += @('--request-code', $requestCode)
    }

    foreach ($lane in $selectedLanes) {
        $arguments += @('--allowed-lane', $lane)
    }

    $statusLabel.Text = 'Forging signed unlock...'
    $form.Refresh()

    Push-Location $generatedDirectory
    try {
        $output = & $toolPath @arguments 2>&1
        $exitCode = $LASTEXITCODE
    }
    finally {
        Pop-Location
    }

    if ($exitCode -ne 0) {
        $statusLabel.Text = 'Generation failed.'
        [System.Windows.Forms.MessageBox]::Show(
            ($output | Out-String),
            'Prompt Forge V5.1 Unlock Generator',
            [System.Windows.Forms.MessageBoxButtons]::OK,
            [System.Windows.Forms.MessageBoxIcon]::Error) | Out-Null
        return
    }

    $statusLabel.Text = "Generated: $outputPath"
    $message = @(
        'Signed unlock generated.'
        ''
        "Output: $outputPath"
        "Mode: $mode"
        "License type: $($selectedType.Label)"
        "Entitlement profile: $($selectedType.Profile)"
        "Allowed lanes: $(if ($selectedLanes.Count -gt 0) { $selectedLanes -join ', ' } else { 'none' })"
        ''
        ($output | Out-String).Trim()
    ) -join "`r`n"

    [System.Windows.Forms.MessageBox]::Show(
        $message,
        'Prompt Forge V5.1 Unlock Generator',
        [System.Windows.Forms.MessageBoxButtons]::OK,
        [System.Windows.Forms.MessageBoxIcon]::Information) | Out-Null
})

Sync-LicenseTypeState -TypeBox $licenseTypeBox -LaneList $laneList -DescriptionLabel $descriptionLabel
[void]$form.ShowDialog()
