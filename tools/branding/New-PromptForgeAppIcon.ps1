[CmdletBinding()]
param(
    [string]$OutputDirectory
)

Add-Type -AssemblyName System.Drawing

$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Split-Path (Split-Path $scriptRoot -Parent) -Parent

if (-not $OutputDirectory) {
    $OutputDirectory = Join-Path $repoRoot 'PromptForge.App\Assets'
}

$outputDirectory = [System.IO.Path]::GetFullPath($OutputDirectory)
[System.IO.Directory]::CreateDirectory($outputDirectory) | Out-Null

$pngPath = Join-Path $outputDirectory 'PromptForgeIcon-256.png'
$icoPath = Join-Path $outputDirectory 'PromptForge.ico'

$size = 256
$bitmap = New-Object System.Drawing.Bitmap $size, $size
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)

try {
    $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
    $graphics.Clear([System.Drawing.Color]::Transparent)

    $backgroundRect = New-Object System.Drawing.RectangleF 12, 12, 232, 232
    $backgroundPath = New-Object System.Drawing.Drawing2D.GraphicsPath
    $radius = 56.0
    $diameter = $radius * 2
    $backgroundPath.AddArc($backgroundRect.X, $backgroundRect.Y, $diameter, $diameter, 180, 90)
    $backgroundPath.AddArc($backgroundRect.Right - $diameter, $backgroundRect.Y, $diameter, $diameter, 270, 90)
    $backgroundPath.AddArc($backgroundRect.Right - $diameter, $backgroundRect.Bottom - $diameter, $diameter, $diameter, 0, 90)
    $backgroundPath.AddArc($backgroundRect.X, $backgroundRect.Bottom - $diameter, $diameter, $diameter, 90, 90)
    $backgroundPath.CloseFigure()

    $bgBrush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
        (New-Object System.Drawing.Point 0, 0),
        (New-Object System.Drawing.Point $size, $size),
        ([System.Drawing.Color]::FromArgb(255, 27, 34, 44)),
        ([System.Drawing.Color]::FromArgb(255, 10, 15, 22)))
    $graphics.FillPath($bgBrush, $backgroundPath)

    $glowBrush = New-Object System.Drawing.Drawing2D.PathGradientBrush($backgroundPath)
    $glowBrush.CenterColor = [System.Drawing.Color]::FromArgb(120, 255, 166, 76)
    $glowBrush.SurroundColors = @([System.Drawing.Color]::FromArgb(0, 255, 166, 76))
    $graphics.FillPath($glowBrush, $backgroundPath)

    $ringPen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(110, 255, 210, 150)), 4
    $graphics.DrawPath($ringPen, $backgroundPath)

    $forgeBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 140, 46))
    $sparkBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 255, 214, 120))
    $steelBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 229, 232, 236))
    $steelDarkBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(255, 126, 137, 151))

    $anvil = New-Object System.Drawing.Drawing2D.GraphicsPath
    $anvil.AddPolygon([System.Drawing.PointF[]]@(
        (New-Object System.Drawing.PointF 70, 150),
        (New-Object System.Drawing.PointF 150, 150),
        (New-Object System.Drawing.PointF 172, 136),
        (New-Object System.Drawing.PointF 198, 136),
        (New-Object System.Drawing.PointF 185, 155),
        (New-Object System.Drawing.PointF 210, 155),
        (New-Object System.Drawing.PointF 196, 176),
        (New-Object System.Drawing.PointF 143, 176),
        (New-Object System.Drawing.PointF 135, 195),
        (New-Object System.Drawing.PointF 92, 195),
        (New-Object System.Drawing.PointF 102, 176),
        (New-Object System.Drawing.PointF 62, 176)
    ))
    $graphics.FillPath($steelBrush, $anvil)
    $graphics.FillRectangle($steelDarkBrush, 97, 194, 35, 16)
    $graphics.FillRectangle($steelDarkBrush, 82, 208, 67, 12)

    $hammerPen = New-Object System.Drawing.Pen ([System.Drawing.Color]::FromArgb(255, 196, 205, 214)), 18
    $hammerPen.StartCap = [System.Drawing.Drawing2D.LineCap]::Round
    $hammerPen.EndCap = [System.Drawing.Drawing2D.LineCap]::Round
    $graphics.DrawLine($hammerPen, 169, 65, 118, 122)

    $hammerHead = New-Object System.Drawing.Drawing2D.GraphicsPath
    $hammerHead.AddPolygon([System.Drawing.PointF[]]@(
        (New-Object System.Drawing.PointF 156, 60),
        (New-Object System.Drawing.PointF 206, 84),
        (New-Object System.Drawing.PointF 193, 111),
        (New-Object System.Drawing.PointF 142, 87)
    ))
    $graphics.FillPath($steelBrush, $hammerHead)
    $graphics.FillRectangle($steelDarkBrush, 167, 69, 18, 34)

    $graphics.FillEllipse($forgeBrush, 118, 106, 38, 38)
    $graphics.FillEllipse($sparkBrush, 128, 84, 10, 10)
    $graphics.FillEllipse($sparkBrush, 144, 71, 8, 8)
    $graphics.FillEllipse($sparkBrush, 159, 90, 6, 6)

    $fontFamily = New-Object System.Drawing.FontFamily 'Segoe UI Semibold'
    $font = New-Object System.Drawing.Font $fontFamily, 68, ([System.Drawing.FontStyle]::Bold), ([System.Drawing.GraphicsUnit]::Pixel)
    $textBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(245, 255, 246, 236))
    $textShadowBrush = New-Object System.Drawing.SolidBrush ([System.Drawing.Color]::FromArgb(90, 0, 0, 0))
    $graphics.DrawString('PF', $font, $textShadowBrush, 48, 33)
    $graphics.DrawString('PF', $font, $textBrush, 44, 29)

    $bitmap.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)

    $pngBytes = [System.IO.File]::ReadAllBytes($pngPath)
    $stream = [System.IO.File]::Create($icoPath)
    try {
        $writer = New-Object System.IO.BinaryWriter $stream
        $writer.Write([UInt16]0)
        $writer.Write([UInt16]1)
        $writer.Write([UInt16]1)
        $writer.Write([byte]0)
        $writer.Write([byte]0)
        $writer.Write([byte]0)
        $writer.Write([byte]0)
        $writer.Write([UInt16]1)
        $writer.Write([UInt16]32)
        $writer.Write([UInt32]$pngBytes.Length)
        $writer.Write([UInt32]22)
        $writer.Write($pngBytes)
        $writer.Flush()
    }
    finally {
        if ($writer) { $writer.Dispose() }
        $stream.Dispose()
    }
}
finally {
    $graphics.Dispose()
    $bitmap.Dispose()
    if ($backgroundPath) { $backgroundPath.Dispose() }
    if ($bgBrush) { $bgBrush.Dispose() }
    if ($glowBrush) { $glowBrush.Dispose() }
    if ($ringPen) { $ringPen.Dispose() }
    if ($forgeBrush) { $forgeBrush.Dispose() }
    if ($sparkBrush) { $sparkBrush.Dispose() }
    if ($steelBrush) { $steelBrush.Dispose() }
    if ($steelDarkBrush) { $steelDarkBrush.Dispose() }
    if ($anvil) { $anvil.Dispose() }
    if ($hammerPen) { $hammerPen.Dispose() }
    if ($hammerHead) { $hammerHead.Dispose() }
    if ($font) { $font.Dispose() }
    if ($fontFamily) { $fontFamily.Dispose() }
    if ($textBrush) { $textBrush.Dispose() }
    if ($textShadowBrush) { $textShadowBrush.Dispose() }
}

Write-Host "Created Prompt Forge icon assets:"
Write-Host "  $pngPath"
Write-Host "  $icoPath"
