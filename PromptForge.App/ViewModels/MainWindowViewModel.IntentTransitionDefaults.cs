using PromptForge.App.Models;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel
{
    private void RunIntentTransitionDefaultingWorkflow(string previousIntentMode, string normalized)
    {
        if (_isApplyingConfiguration)
        {
            return;
        }

        var preservedSliderPositions = OverrideDefaultSliderPositions ? CaptureCurrentSliderPositions() : null;
        LogOverrideSliderDiagnostics(
            preservedSliderPositions is null
                ? $"intent-change-preserve-skipped from='{previousIntentMode}' to='{normalized}' override={OverrideDefaultSliderPositions} {FormatCurrentOverrideSliderState()}"
                : $"intent-change-preserve-captured from='{previousIntentMode}' to='{normalized}' override={OverrideDefaultSliderPositions} {FormatLanePromptDefaults(preservedSliderPositions)}");
        SyncCompressionStateForIntentChange(normalized);
        ResetPromptExclusionFlags();
        if (IntentModeCatalog.IsAnime(normalized))
        {
            ApplyAnimeIntentDefaults();
        }

        if (IntentModeCatalog.IsChildrensBook(normalized))
        {
            ApplyChildrensBookIntentDefaults();
        }

        if (IntentModeCatalog.IsProductPhotography(normalized))
        {
            ApplyProductPhotographyIntentDefaults();
        }

        if (IntentModeCatalog.IsFoodPhotography(normalized))
        {
            ApplyFoodPhotographyIntentDefaults();
        }

        if (IntentModeCatalog.IsLifestyleAdvertisingPhotography(normalized))
        {
            ApplyLifestyleAdvertisingPhotographyIntentDefaults();
        }

        if (IntentModeCatalog.IsVintageBend(normalized))
        {
            ApplyVintageBendIntentDefaults();
        }

        if (IntentModeCatalog.IsCinematic(normalized))
        {
            ApplyCinematicIntentDefaults();
        }

        if (IntentModeCatalog.IsThreeDRender(normalized))
        {
            ApplyThreeDRenderIntentDefaults();
        }

        if (IntentModeCatalog.IsConceptArt(normalized))
        {
            ApplyConceptArtIntentDefaults();
        }

        if (IntentModeCatalog.IsPixelArt(normalized))
        {
            ApplyPixelArtIntentDefaults();
        }

        if (IntentModeCatalog.IsWatercolor(normalized))
        {
            ApplyWatercolorIntentDefaults();
        }

        if (IntentModeCatalog.IsArchitectureArchviz(normalized))
        {
            ApplyArchitectureArchvizIntentDefaults();
        }

        if (IntentModeCatalog.IsPhotography(normalized))
        {
            ApplyPhotographyIntentDefaults();
        }

        if (IntentModeCatalog.IsFantasyIllustration(normalized))
        {
            ApplyFantasyIllustrationIntentDefaults();
        }

        if (IntentModeCatalog.IsEditorialIllustration(normalized))
        {
            ApplyEditorialIllustrationIntentDefaults();
        }

        if (IntentModeCatalog.IsGraphicDesign(normalized))
        {
            ApplyGraphicDesignIntentDefaults();
        }

        if (IntentModeCatalog.IsInfographicDataVisualization(normalized))
        {
            ApplyInfographicDataVisualizationIntentDefaults();
        }

        if (IntentModeCatalog.IsTattooArt(normalized))
        {
            ApplyTattooArtIntentDefaults();
        }

        LogOverrideSliderDiagnostics(
            $"intent-change-after-defaults from='{previousIntentMode}' to='{normalized}' override={OverrideDefaultSliderPositions} {FormatCurrentOverrideSliderState()}");
        if (preservedSliderPositions is not null)
        {
            RestoreCurrentSliderPositions(preservedSliderPositions);
            LogOverrideSliderDiagnostics(
                $"intent-change-after-restore from='{previousIntentMode}' to='{normalized}' override={OverrideDefaultSliderPositions} {FormatCurrentOverrideSliderState()}");
        }
    }

    private void ApplyLanePromptDefaults(LanePromptDefaults defaults)
    {
        if (defaults.Temperature.HasValue) Temperature = defaults.Temperature.Value;
        if (defaults.LightingIntensity.HasValue) LightingIntensity = defaults.LightingIntensity.Value;
        if (defaults.Stylization.HasValue) Stylization = defaults.Stylization.Value;
        if (defaults.Realism.HasValue) Realism = defaults.Realism.Value;
        if (defaults.TextureDepth.HasValue) TextureDepth = defaults.TextureDepth.Value;
        if (defaults.NarrativeDensity.HasValue) NarrativeDensity = defaults.NarrativeDensity.Value;
        if (defaults.Symbolism.HasValue) Symbolism = defaults.Symbolism.Value;
        if (defaults.AtmosphericDepth.HasValue) AtmosphericDepth = defaults.AtmosphericDepth.Value;
        if (defaults.SurfaceAge.HasValue) SurfaceAge = defaults.SurfaceAge.Value;
        if (defaults.Chaos.HasValue) Chaos = defaults.Chaos.Value;
        if (defaults.Framing.HasValue) Framing = defaults.Framing.Value;
        if (defaults.CameraDistance.HasValue) CameraDistance = defaults.CameraDistance.Value;
        if (defaults.CameraAngle.HasValue) CameraAngle = defaults.CameraAngle.Value;
        if (defaults.BackgroundComplexity.HasValue) BackgroundComplexity = defaults.BackgroundComplexity.Value;
        if (defaults.MotionEnergy.HasValue) MotionEnergy = defaults.MotionEnergy.Value;
        if (defaults.FocusDepth.HasValue) FocusDepth = defaults.FocusDepth.Value;
        if (defaults.ImageCleanliness.HasValue) ImageCleanliness = defaults.ImageCleanliness.Value;
        if (defaults.DetailDensity.HasValue) DetailDensity = defaults.DetailDensity.Value;
        if (defaults.Whimsy.HasValue) Whimsy = defaults.Whimsy.Value;
        if (defaults.Tension.HasValue) Tension = defaults.Tension.Value;
        if (defaults.Awe.HasValue) Awe = defaults.Awe.Value;
        if (defaults.Saturation.HasValue) Saturation = defaults.Saturation.Value;
        if (defaults.Contrast.HasValue) Contrast = defaults.Contrast.Value;
        if (!string.IsNullOrWhiteSpace(defaults.Lighting)) Lighting = defaults.Lighting;
        if (!string.IsNullOrWhiteSpace(defaults.ArtStyle)) ArtStyle = defaults.ArtStyle;
    }

    private LanePromptDefaults CaptureCurrentSliderPositions()
    {
        return new LanePromptDefaults
        {
            Temperature = Temperature,
            LightingIntensity = LightingIntensity,
            Stylization = Stylization,
            Realism = Realism,
            TextureDepth = TextureDepth,
            NarrativeDensity = NarrativeDensity,
            Symbolism = Symbolism,
            AtmosphericDepth = AtmosphericDepth,
            SurfaceAge = SurfaceAge,
            Chaos = Chaos,
            Framing = Framing,
            CameraDistance = CameraDistance,
            CameraAngle = CameraAngle,
            BackgroundComplexity = BackgroundComplexity,
            MotionEnergy = MotionEnergy,
            FocusDepth = FocusDepth,
            ImageCleanliness = ImageCleanliness,
            DetailDensity = DetailDensity,
            Whimsy = Whimsy,
            Tension = Tension,
            Awe = Awe,
            Saturation = Saturation,
            Contrast = Contrast,
        };
    }

    private void RestoreCurrentSliderPositions(LanePromptDefaults preservedSliderPositions)
    {
        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            ApplyLanePromptDefaults(preservedSliderPositions);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }
}
