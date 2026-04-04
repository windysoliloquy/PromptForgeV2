using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class IntentModeCatalog
{
    public const string ExperimentalName = "Experimental";
    public const string VintageBendName = "Vintage Bend";
    public const string AnimeName = "Anime";
    public const string WatercolorName = "Watercolor";
    public const string ChildrensBookName = "Children's Book";
    public const string ComicBookName = "Comic Book";
    public const string CinematicName = "Cinematic";
    public const string PhotographyName = "Photography";
    public const string PhotographicName = "Photographic";
    public const string ProductPhotographyName = "Product Photography";
    public const string FoodPhotographyName = "Food Photography";
    public const string ArchitectureArchvizName = "Architecture / Archviz";
    public const string ThreeDRenderName = "3D Render";
    public const string ConceptArtName = "Concept Art";
    public const string PixelArtName = "Pixel Art";

    private static readonly IReadOnlyDictionary<string, IntentModeDefinition> Definitions =
        BuildDefinitions();

    private static IReadOnlyDictionary<string, IntentModeDefinition> BuildDefinitions()
    {
        var photography = new IntentModeDefinition(
            PhotographyName,
            Whimsy: 14,
            Tension: 32,
            Awe: 42,
            Chaos: 12,
            MotionEnergy: 30,
            AtmosphericDepth: 40,
            NarrativeDensity: 54,
            Symbolism: 28,
            Saturation: 50,
            Contrast: 56,
            Lighting: "Soft daylight",
            Summary: "observational, editorial, and documentary photographic language with lens-mediated presence and compact process-aware phrasing");

        var productPhotography = new IntentModeDefinition(
            ProductPhotographyName,
            Whimsy: 4,
            Tension: 8,
            Awe: 18,
            Chaos: 8,
            MotionEnergy: 6,
            AtmosphericDepth: 26,
            NarrativeDensity: 16,
            Symbolism: 8,
            Saturation: 54,
            Contrast: 58,
            Lighting: "Dramatic studio light",
            Summary: "commercial merchandise imagery with controlled studio presentation, material fidelity, and clean sellable staging");

        var foodPhotography = new IntentModeDefinition(
            FoodPhotographyName,
            Whimsy: 2,
            Tension: 2,
            Awe: 18,
            Chaos: 10,
            MotionEnergy: 6,
            AtmosphericDepth: 24,
            NarrativeDensity: 18,
            Symbolism: 2,
            Saturation: 58,
            Contrast: 52,
            Lighting: "Soft daylight",
            Summary: "commercial food imagery with appetizing presentation, menu-ready clarity, hospitality polish, and styled service-driven visuals");

        var architectureArchviz = new IntentModeDefinition(
            ArchitectureArchvizName,
            Whimsy: 2,
            Tension: 4,
            Awe: 24,
            Chaos: 8,
            MotionEnergy: 4,
            AtmosphericDepth: 48,
            NarrativeDensity: 14,
            Symbolism: 4,
            Saturation: 48,
            Contrast: 54,
            Lighting: "Soft daylight",
            Summary: "built-space visualization with spatial clarity, material realism, development polish, and market-ready architectural imagery");

        return new Dictionary<string, IntentModeDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            [AnimeName] = new(
                AnimeName,
                Whimsy: 28,
                Tension: 40,
                Awe: 56,
                Chaos: 24,
                MotionEnergy: 54,
                AtmosphericDepth: 44,
                NarrativeDensity: 52,
                Symbolism: 34,
                Saturation: 62,
                Contrast: 58,
                Lighting: "Soft glow",
                Summary: "stylized anime illustration language with polished character-led staging and clean visual readability"),
            [WatercolorName] = new(
                WatercolorName,
                Whimsy: 24,
                Tension: 26,
                Awe: 58,
                Chaos: 10,
                MotionEnergy: 18,
                AtmosphericDepth: 52,
                NarrativeDensity: 44,
                Symbolism: 30,
                Saturation: 50,
                Contrast: 42,
                Lighting: "Soft daylight",
                Summary: "watercolor illustration language with luminous washes, paper-backed softness, and hand-painted clarity"),
            [ChildrensBookName] = new(
                ChildrensBookName,
                Whimsy: 42,
                Tension: 18,
                Awe: 50,
                Chaos: 12,
                MotionEnergy: 24,
                AtmosphericDepth: 40,
                NarrativeDensity: 58,
                Symbolism: 36,
                Saturation: 56,
                Contrast: 44,
                Lighting: "Soft daylight",
                Summary: "gentle illustrated scenes with readable storytelling, warm character presence, and decorative clarity"),
            [ComicBookName] = new(
                ComicBookName,
                Whimsy: 18,
                Tension: 52,
                Awe: 44,
                Chaos: 24,
                MotionEnergy: 56,
                AtmosphericDepth: 34,
                NarrativeDensity: 60,
                Symbolism: 28,
                Saturation: 58,
                Contrast: 72,
                Lighting: "Dramatic studio light",
                Summary: "graphic storytelling with sharp ink contrast, readable panel energy, and clear narrative beats"),
            [CinematicName] = new(
                CinematicName,
                Whimsy: 18,
                Tension: 44,
                Awe: 46,
                Chaos: 16,
                MotionEnergy: 42,
                AtmosphericDepth: 54,
                NarrativeDensity: 60,
                Symbolism: 28,
                Saturation: 56,
                Contrast: 62,
                Lighting: "Soft cinematic daylight",
                Summary: "cinematic film still framing with screen-native atmosphere, disciplined tension, and clear scene presence"),
            [PhotographyName] = photography,
            [PhotographicName] = photography,
            [ProductPhotographyName] = productPhotography,
            [FoodPhotographyName] = foodPhotography,
            [ArchitectureArchvizName] = architectureArchviz,
            [ThreeDRenderName] = new(
                ThreeDRenderName,
                Whimsy: 14,
                Tension: 32,
                Awe: 40,
                Chaos: 18,
                MotionEnergy: 34,
                AtmosphericDepth: 44,
                NarrativeDensity: 52,
                Symbolism: 22,
                Saturation: 54,
                Contrast: 60,
                Lighting: "Soft rendered daylight",
                Summary: "clean 3D render presentation with material clarity, controlled lighting, and polished scene readability"),
            [ConceptArtName] = new(
                ConceptArtName,
                Whimsy: 18,
                Tension: 34,
                Awe: 48,
                Chaos: 14,
                MotionEnergy: 22,
                AtmosphericDepth: 48,
                NarrativeDensity: 58,
                Symbolism: 34,
                Saturation: 54,
                Contrast: 52,
                Lighting: "Soft daylight",
                Summary: "concept art presentation with production-facing clarity, readable design intent, and disciplined worldbuilding"),
            [PixelArtName] = new(
                PixelArtName,
                Whimsy: 16,
                Tension: 20,
                Awe: 32,
                Chaos: 12,
                MotionEnergy: 18,
                AtmosphericDepth: 24,
                NarrativeDensity: 42,
                Symbolism: 26,
                Saturation: 54,
                Contrast: 60,
                Lighting: "Soft daylight",
                Summary: "pixel art presentation with palette discipline, sprite-readable structure, and crisp game-oriented clarity"),
        };
    }

    public static IReadOnlyList<string> Names { get; } = new[]
    {
        "Custom",
        AnimeName,
        WatercolorName,
        ChildrensBookName,
        ComicBookName,
        CinematicName,
        PhotographyName,
        ProductPhotographyName,
        FoodPhotographyName,
        ArchitectureArchvizName,
        ThreeDRenderName,
        ConceptArtName,
        PixelArtName,
        VintageBendName,
        ExperimentalName,
    };

    public static bool IsExperimental(string? intentMode)
    {
        return string.Equals(intentMode, ExperimentalName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsVintageBend(string? intentMode)
    {
        return string.Equals(intentMode, VintageBendName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsAnime(string? intentMode)
    {
        return string.Equals(intentMode, AnimeName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsWatercolor(string? intentMode)
    {
        return string.Equals(intentMode, WatercolorName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsChildrensBook(string? intentMode)
    {
        return string.Equals(intentMode, ChildrensBookName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsComicBook(string? intentMode)
    {
        return string.Equals(intentMode, ComicBookName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsCinematic(string? intentMode)
    {
        return string.Equals(intentMode, CinematicName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPhotography(string? intentMode)
    {
        return string.Equals(intentMode, PhotographyName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(intentMode, PhotographicName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsProductPhotography(string? intentMode)
    {
        return string.Equals(intentMode, ProductPhotographyName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsFoodPhotography(string? intentMode)
    {
        return string.Equals(intentMode, FoodPhotographyName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsArchitectureArchviz(string? intentMode)
    {
        return string.Equals(intentMode, ArchitectureArchvizName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsThreeDRender(string? intentMode)
    {
        return string.Equals(intentMode, ThreeDRenderName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsConceptArt(string? intentMode)
    {
        return string.Equals(intentMode, ConceptArtName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPixelArt(string? intentMode)
    {
        return string.Equals(intentMode, PixelArtName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool TryGet(string? intentMode, out IntentModeDefinition definition)
    {
        if (string.IsNullOrWhiteSpace(intentMode)
            || string.Equals(intentMode, "Custom", StringComparison.OrdinalIgnoreCase)
            || IsExperimental(intentMode))
        {
            definition = null!;
            return false;
        }

        if (Definitions.TryGetValue(intentMode, out definition!))
        {
            return true;
        }

        if (IsProductPhotography(intentMode))
        {
            return Definitions.TryGetValue(ProductPhotographyName, out definition!);
        }

        if (IsFoodPhotography(intentMode))
        {
            return Definitions.TryGetValue(FoodPhotographyName, out definition!);
        }

        if (IsPhotography(intentMode))
        {
            return Definitions.TryGetValue(PhotographyName, out definition!);
        }

        return false;
    }
}
