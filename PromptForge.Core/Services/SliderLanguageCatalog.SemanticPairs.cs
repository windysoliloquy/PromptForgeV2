using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    private static readonly IReadOnlySet<string> InfographicDataVisualizationGenericPairSliderKeys =
        PairSliderSet(
            Stylization,
            Realism,
            Framing,
            CameraDistance,
            FocusDepth,
            DetailDensity);

    private static readonly IReadOnlySet<string> InfographicDataVisualizationDataVizPairSliderKeys =
        PairSliderSet(
            Stylization,
            Realism,
            Framing,
            CameraDistance,
            FocusDepth,
            DetailDensity,
            Tension,
            Contrast,
            MotionEnergy,
            Awe);

    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> InstalledSemanticPairSliderKeysByLane =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            [IntentModeCatalog.AnimeName] = PairSliderSet(
                Stylization,
                Realism,
                LightingIntensity,
                Contrast,
                FocusDepth,
                AtmosphericDepth,
                MotionEnergy,
                Tension,
                TextureDepth,
                ImageCleanliness),
            [IntentModeCatalog.ArchitectureArchvizName] = PairSliderSet(),
            [IntentModeCatalog.ChildrensBookName] = PairSliderSet(
                Stylization,
                Realism,
                NarrativeDensity,
                BackgroundComplexity,
                TextureDepth,
                ImageCleanliness,
                MotionEnergy,
                Chaos,
                Whimsy,
                Tension,
                Awe,
                AtmosphericDepth),
            [IntentModeCatalog.CinematicName] = PairSliderSet(
                NarrativeDensity,
                BackgroundComplexity,
                MotionEnergy,
                Chaos,
                TextureDepth,
                ImageCleanliness,
                Awe,
                AtmosphericDepth),
            [IntentModeCatalog.ComicBookName] = PairSliderSet(
                TextureDepth,
                ImageCleanliness,
                Awe,
                AtmosphericDepth,
                MotionEnergy,
                Chaos,
                NarrativeDensity,
                BackgroundComplexity,
                Stylization,
                Realism,
                Whimsy,
                Tension),
            [IntentModeCatalog.ConceptArtName] = PairSliderSet(
                Temperature,
                LightingIntensity,
                Saturation,
                Contrast,
                Framing,
                CameraDistance,
                AtmosphericDepth,
                FocusDepth,
                Tension,
                Awe),
            [IntentModeCatalog.FantasyIllustrationName] = PairSliderSet(
                Stylization,
                Realism,
                CameraDistance,
                Framing,
                Tension,
                Whimsy,
                LightingIntensity,
                Awe),
            [IntentModeCatalog.EditorialIllustrationName] = PairSliderSet(),
            [IntentModeCatalog.FoodPhotographyName] = PairSliderSet(),
            [IntentModeCatalog.GraphicDesignName] = PairSliderSet(
                Stylization,
                Realism,
                Whimsy,
                Tension,
                Awe,
                Contrast),
            [IntentModeCatalog.InfographicDataVisualizationName] = InfographicDataVisualizationGenericPairSliderKeys,
            [IntentModeCatalog.LifestyleAdvertisingPhotographyName] = PairSliderSet(),
            [IntentModeCatalog.PhotographicName] = PairSliderSet(),
            [IntentModeCatalog.PhotographyName] = PairSliderSet(),
            [IntentModeCatalog.PixelArtName] = PairSliderSet(),
            [IntentModeCatalog.ProductPhotographyName] = PairSliderSet(),
            [IntentModeCatalog.TattooArtName] = PairSliderSet(),
            [IntentModeCatalog.ThreeDRenderName] = PairSliderSet(),
            [IntentModeCatalog.VintageBendName] = PairSliderSet(),
            [IntentModeCatalog.WatercolorName] = PairSliderSet(
                Stylization,
                Realism),
        };

    public static bool HasInstalledSemanticPairMap(string? intentMode) =>
        TryGetInstalledSemanticPairSliderKeys(intentMode, out _);

    public static bool IsInstalledSemanticPairSlider(string? intentMode, string sliderKey, PromptConfiguration? configuration = null) =>
        TryGetInstalledSemanticPairSliderKeys(intentMode, configuration, out var sliderKeys)
        && sliderKeys.Contains(sliderKey);

    public static bool TryGetInstalledNeutralBandPairSliderKeys(
        string? intentMode,
        PromptConfiguration? configuration,
        out IReadOnlySet<string> sliderKeys) =>
        TryGetInstalledSemanticPairSliderKeys(intentMode, configuration, out sliderKeys);

    private static bool TryGetInstalledSemanticPairSliderKeys(
        string? intentMode,
        PromptConfiguration? configuration,
        out IReadOnlySet<string> sliderKeys)
    {
        if (IntentModeCatalog.IsInfographicDataVisualization(intentMode))
        {
            sliderKeys = configuration is not null && IsDataVizSubdomain(configuration)
                ? InfographicDataVisualizationDataVizPairSliderKeys
                : InfographicDataVisualizationGenericPairSliderKeys;
            return true;
        }

        if (!string.IsNullOrWhiteSpace(intentMode)
            && InstalledSemanticPairSliderKeysByLane.TryGetValue(intentMode, out sliderKeys!))
        {
            return true;
        }

        sliderKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        return false;
    }

    private static bool TryGetInstalledSemanticPairSliderKeys(
        string? intentMode,
        out IReadOnlySet<string> sliderKeys)
    {
        return TryGetInstalledSemanticPairSliderKeys(intentMode, null, out sliderKeys);
    }

    private static IReadOnlySet<string> PairSliderSet(params string[] sliderKeys) =>
        new HashSet<string>(sliderKeys, StringComparer.OrdinalIgnoreCase);

    private static bool TryBuildSemanticPairCollapse(
        PromptConfiguration configuration,
        string firstSliderKey,
        int firstValue,
        string secondSliderKey,
        int secondValue,
        string fusedPhrase,
        out PromptSemanticPairCollapse collapse)
    {
        collapse = default;

        var firstPhrase = ResolvePromptPhraseOrFallback(firstSliderKey, firstValue, configuration);
        var secondPhrase = ResolvePromptPhraseOrFallback(secondSliderKey, secondValue, configuration);

        if (string.IsNullOrWhiteSpace(firstPhrase)
            || string.IsNullOrWhiteSpace(secondPhrase)
            || string.IsNullOrWhiteSpace(fusedPhrase))
        {
            return false;
        }

        collapse = new PromptSemanticPairCollapse(firstPhrase, secondPhrase, fusedPhrase);
        return true;
    }
}
