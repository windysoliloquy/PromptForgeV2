using System;
using System.Collections.Generic;
using PromptForge.App.Services;
using PromptForge.App.Services.Lanes;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel
{
    private readonly HashSet<string> _conceptArtAppliedSliderSuppressions = new(StringComparer.Ordinal);
    private readonly HashSet<string> _fantasyIllustrationAppliedSliderSuppressions = new(StringComparer.Ordinal);
    private readonly HashSet<string> _editorialIllustrationAppliedSliderSuppressions = new(StringComparer.Ordinal);
    private readonly HashSet<string> _graphicDesignAppliedSliderSuppressions = new(StringComparer.Ordinal);
    private readonly HashSet<string> _infographicAppliedSliderSuppressions = new(StringComparer.Ordinal);
    private readonly HashSet<string> _dataVizAppliedSliderSuppressions = new(StringComparer.Ordinal);

    private void SyncConceptArtSliderSuppressions()
    {
        var desiredSuppressions = GetConceptArtSubtypeSuppressions(ConceptArtSubtype);

        SyncAppliedSliderSuppressions(_conceptArtAppliedSliderSuppressions, desiredSuppressions, GetConceptArtSuppressibleSliderKeys());
    }

    private void SyncInfographicDataVisualizationSliderSuppressions()
    {
        EnsureInfographicDataVisualizationModeDefaults();

        var infographicDesiredSuppressions = IsInfographicSubdomainActive
            ? GetInfographicModeSuppressions()
            : Array.Empty<string>();
        var dataVizDesiredSuppressions = IsDataVizSubdomainActive
            ? GetDataVizModeSuppressions()
            : Array.Empty<string>();

        SyncAppliedSliderSuppressions(_infographicAppliedSliderSuppressions, infographicDesiredSuppressions, GetInfographicSuppressibleSliderKeys());
        SyncAppliedSliderSuppressions(_dataVizAppliedSliderSuppressions, dataVizDesiredSuppressions, GetDataVizSuppressibleSliderKeys());
    }

    private void SyncAppliedSliderSuppressions(HashSet<string> appliedSuppressions, IReadOnlyCollection<string> desiredSuppressions, IReadOnlyCollection<string> suppressibleSliderKeys)
    {
        foreach (var sliderKey in suppressibleSliderKeys)
        {
            if (desiredSuppressions.Contains(sliderKey))
            {
                if (!GetSliderExclusionFlag(sliderKey))
                {
                    SetSliderExclusionFlag(sliderKey, true);
                    appliedSuppressions.Add(sliderKey);
                }

                continue;
            }

            if (appliedSuppressions.Remove(sliderKey) &&
                GetSliderExclusionFlag(sliderKey))
            {
                SetSliderExclusionFlag(sliderKey, false);
            }
        }
    }

    private string[] GetInfographicModeSuppressions()
    {
        if (InfographicModePublicPoster)
        {
            return
            [
                SliderLanguageCatalog.AtmosphericDepth,
                SliderLanguageCatalog.SurfaceAge,
            ];
        }

        if (InfographicModeReferenceSheet)
        {
            return
            [
                SliderLanguageCatalog.AtmosphericDepth,
                SliderLanguageCatalog.Chaos,
                SliderLanguageCatalog.Awe,
                SliderLanguageCatalog.SurfaceAge,
                SliderLanguageCatalog.Whimsy,
                SliderLanguageCatalog.Tension,
                SliderLanguageCatalog.MotionEnergy,
                SliderLanguageCatalog.Symbolism,
            ];
        }

        return
        [
            SliderLanguageCatalog.AtmosphericDepth,
            SliderLanguageCatalog.Chaos,
            SliderLanguageCatalog.Awe,
            SliderLanguageCatalog.SurfaceAge,
        ];
    }

    private string[] GetDataVizModeSuppressions()
    {
        if (DataVizModeDashboard)
        {
            return
            [
                SliderLanguageCatalog.AtmosphericDepth,
                SliderLanguageCatalog.Awe,
                SliderLanguageCatalog.SurfaceAge,
                SliderLanguageCatalog.Whimsy,
                SliderLanguageCatalog.Symbolism,
            ];
        }

        if (DataVizModeReportGraphic)
        {
            return
            [
                SliderLanguageCatalog.AtmosphericDepth,
                SliderLanguageCatalog.Chaos,
                SliderLanguageCatalog.SurfaceAge,
                SliderLanguageCatalog.Whimsy,
            ];
        }

        return
        [
            SliderLanguageCatalog.AtmosphericDepth,
            SliderLanguageCatalog.Chaos,
            SliderLanguageCatalog.Awe,
            SliderLanguageCatalog.SurfaceAge,
            SliderLanguageCatalog.Whimsy,
            SliderLanguageCatalog.Tension,
            SliderLanguageCatalog.Symbolism,
            SliderLanguageCatalog.MotionEnergy,
        ];
    }

    private static string[] GetInfographicSuppressibleSliderKeys()
    {
        return
        [
            SliderLanguageCatalog.AtmosphericDepth,
            SliderLanguageCatalog.Chaos,
            SliderLanguageCatalog.Awe,
            SliderLanguageCatalog.SurfaceAge,
            SliderLanguageCatalog.Whimsy,
            SliderLanguageCatalog.Tension,
            SliderLanguageCatalog.MotionEnergy,
            SliderLanguageCatalog.Symbolism,
        ];
    }

    private static string[] GetConceptArtSubtypeSuppressions(string conceptArtSubtype)
    {
        return conceptArtSubtype switch
        {
            "keyframe-concept" => [SliderLanguageCatalog.NarrativeDensity],
            "character-concept" => [SliderLanguageCatalog.BackgroundComplexity],
            "environment-concept" => [SliderLanguageCatalog.Chaos],
            "prop-concept" => [SliderLanguageCatalog.BackgroundComplexity],
            "vehicle-concept" => [SliderLanguageCatalog.BackgroundComplexity],
            "costume-concept" => [SliderLanguageCatalog.BackgroundComplexity],
            _ => Array.Empty<string>(),
        };
    }

    private static string[] GetConceptArtSuppressibleSliderKeys()
    {
        return
        [
            SliderLanguageCatalog.NarrativeDensity,
            SliderLanguageCatalog.BackgroundComplexity,
            SliderLanguageCatalog.Chaos,
        ];
    }

    private static string[] GetDataVizSuppressibleSliderKeys()
    {
        return
        [
            SliderLanguageCatalog.AtmosphericDepth,
            SliderLanguageCatalog.Chaos,
            SliderLanguageCatalog.Awe,
            SliderLanguageCatalog.SurfaceAge,
            SliderLanguageCatalog.Whimsy,
            SliderLanguageCatalog.Tension,
            SliderLanguageCatalog.Symbolism,
            SliderLanguageCatalog.MotionEnergy,
        ];
    }

    private void SyncFantasyIllustrationSliderSuppressions()
    {
        var desiredSuppressions = FantasyIllustrationLane.Instance.GetSuppressedSliders(CaptureConfiguration());
        if (IsFantasyIllustrationIntent || FantasyIllustrationCharacterSketch)
        {
            UiEventLog.Write(
                $"fantasy-debug suppression-sync-start intent='{IntentMode}' register='{FantasyIllustrationRegister}' characterSketch={FantasyIllustrationCharacterSketch} desiredNarrative={desiredSuppressions.Contains(SliderLanguageCatalog.NarrativeDensity)} beforeNarrative={ExcludeNarrativeDensityFromPrompt} appliedBefore={_fantasyIllustrationAppliedSliderSuppressions.Contains(SliderLanguageCatalog.NarrativeDensity)}");
        }

        foreach (var sliderKey in FantasyIllustrationLane.Instance.GetSuppressibleSliderKeys())
        {
            if (desiredSuppressions.Contains(sliderKey))
            {
                if (!GetSliderExclusionFlag(sliderKey))
                {
                    SetSliderExclusionFlag(sliderKey, true);
                    _fantasyIllustrationAppliedSliderSuppressions.Add(sliderKey);
                }

                continue;
            }

            if (_fantasyIllustrationAppliedSliderSuppressions.Remove(sliderKey) &&
                GetSliderExclusionFlag(sliderKey))
            {
                SetSliderExclusionFlag(sliderKey, false);
            }
        }

        if (IsFantasyIllustrationIntent || FantasyIllustrationCharacterSketch)
        {
            UiEventLog.Write(
                $"fantasy-debug suppression-sync-complete intent='{IntentMode}' register='{FantasyIllustrationRegister}' characterSketch={FantasyIllustrationCharacterSketch} afterNarrative={ExcludeNarrativeDensityFromPrompt} appliedAfter={_fantasyIllustrationAppliedSliderSuppressions.Contains(SliderLanguageCatalog.NarrativeDensity)}");
        }
    }

    private void SyncEditorialIllustrationSliderSuppressions()
    {
        var desiredSuppressions = EditorialIllustrationLane.Instance.GetSuppressedSliders(CaptureConfiguration());

        SyncAppliedSliderSuppressions(
            _editorialIllustrationAppliedSliderSuppressions,
            desiredSuppressions,
            EditorialIllustrationLane.Instance.GetSuppressibleSliderKeys().ToArray());
    }

    private void SyncGraphicDesignSliderSuppressions()
    {
        var desiredSuppressions = GraphicDesignLane.Instance.GetSuppressedSliders(CaptureConfiguration());

        foreach (var sliderKey in GraphicDesignLane.Instance.GetSuppressibleSliderKeys())
        {
            if (desiredSuppressions.Contains(sliderKey))
            {
                if (!GetSliderExclusionFlag(sliderKey))
                {
                    SetSliderExclusionFlag(sliderKey, true);
                    _graphicDesignAppliedSliderSuppressions.Add(sliderKey);
                }

                continue;
            }

            if (_graphicDesignAppliedSliderSuppressions.Remove(sliderKey) &&
                GetSliderExclusionFlag(sliderKey))
            {
                SetSliderExclusionFlag(sliderKey, false);
            }
        }
    }

    private bool GetSliderExclusionFlag(string sliderKey)
    {
        return sliderKey switch
        {
            SliderLanguageCatalog.Stylization => ExcludeStylizationFromPrompt,
            SliderLanguageCatalog.Temperature => ExcludeTemperatureFromPrompt,
            SliderLanguageCatalog.CameraAngle => ExcludeCameraAngleFromPrompt,
            SliderLanguageCatalog.BackgroundComplexity => ExcludeBackgroundComplexityFromPrompt,
            SliderLanguageCatalog.AtmosphericDepth => ExcludeAtmosphericDepthFromPrompt,
            SliderLanguageCatalog.NarrativeDensity => ExcludeNarrativeDensityFromPrompt,
            SliderLanguageCatalog.DetailDensity => ExcludeDetailDensityFromPrompt,
            SliderLanguageCatalog.Chaos => ExcludeChaosFromPrompt,
            SliderLanguageCatalog.MotionEnergy => ExcludeMotionEnergyFromPrompt,
            SliderLanguageCatalog.Symbolism => ExcludeSymbolismFromPrompt,
            SliderLanguageCatalog.SurfaceAge => ExcludeSurfaceAgeFromPrompt,
            SliderLanguageCatalog.Whimsy => ExcludeWhimsyFromPrompt,
            SliderLanguageCatalog.Tension => ExcludeTensionFromPrompt,
            SliderLanguageCatalog.Awe => ExcludeAweFromPrompt,
            SliderLanguageCatalog.Saturation => ExcludeSaturationFromPrompt,
            _ => false,
        };
    }

    private void SetSliderExclusionFlag(string sliderKey, bool value)
    {
        switch (sliderKey)
        {
            case SliderLanguageCatalog.Stylization:
                SetProperty(ref _excludeStylizationFromPrompt, value, nameof(ExcludeStylizationFromPrompt));
                break;
            case SliderLanguageCatalog.Temperature:
                SetProperty(ref _excludeTemperatureFromPrompt, value, nameof(ExcludeTemperatureFromPrompt));
                break;
            case SliderLanguageCatalog.CameraAngle:
                SetProperty(ref _excludeCameraAngleFromPrompt, value, nameof(ExcludeCameraAngleFromPrompt));
                break;
            case SliderLanguageCatalog.BackgroundComplexity:
                SetProperty(ref _excludeBackgroundComplexityFromPrompt, value, nameof(ExcludeBackgroundComplexityFromPrompt));
                break;
            case SliderLanguageCatalog.AtmosphericDepth:
                SetProperty(ref _excludeAtmosphericDepthFromPrompt, value, nameof(ExcludeAtmosphericDepthFromPrompt));
                break;
            case SliderLanguageCatalog.NarrativeDensity:
                SetProperty(ref _excludeNarrativeDensityFromPrompt, value, nameof(ExcludeNarrativeDensityFromPrompt));
                break;
            case SliderLanguageCatalog.DetailDensity:
                SetProperty(ref _excludeDetailDensityFromPrompt, value, nameof(ExcludeDetailDensityFromPrompt));
                break;
            case SliderLanguageCatalog.Chaos:
                SetProperty(ref _excludeChaosFromPrompt, value, nameof(ExcludeChaosFromPrompt));
                break;
            case SliderLanguageCatalog.MotionEnergy:
                SetProperty(ref _excludeMotionEnergyFromPrompt, value, nameof(ExcludeMotionEnergyFromPrompt));
                break;
            case SliderLanguageCatalog.Symbolism:
                SetProperty(ref _excludeSymbolismFromPrompt, value, nameof(ExcludeSymbolismFromPrompt));
                break;
            case SliderLanguageCatalog.SurfaceAge:
                SetProperty(ref _excludeSurfaceAgeFromPrompt, value, nameof(ExcludeSurfaceAgeFromPrompt));
                break;
            case SliderLanguageCatalog.Whimsy:
                SetProperty(ref _excludeWhimsyFromPrompt, value, nameof(ExcludeWhimsyFromPrompt));
                break;
            case SliderLanguageCatalog.Tension:
                SetProperty(ref _excludeTensionFromPrompt, value, nameof(ExcludeTensionFromPrompt));
                break;
            case SliderLanguageCatalog.Awe:
                SetProperty(ref _excludeAweFromPrompt, value, nameof(ExcludeAweFromPrompt));
                break;
            case SliderLanguageCatalog.Saturation:
                SetProperty(ref _excludeSaturationFromPrompt, value, nameof(ExcludeSaturationFromPrompt));
                break;
        }
    }
}
