using System.Text;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed class PromptBuilderService : IPromptBuilderService
{
    private const string DefaultNegativePrompt = "blurry, low detail, distorted anatomy, messy composition, poorly defined material texture";
    private readonly IArtistProfileService _artistProfileService;

    public PromptBuilderService(IArtistProfileService artistProfileService)
    {
        _artistProfileService = artistProfileService;
    }

    public PromptResult Build(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddUnique(phrases, seen, BuildSubjectSection(configuration));
        AddUnique(phrases, seen, BuildRelationshipSection(configuration));
        foreach (var phrase in BuildStyleSection(configuration)) AddUnique(phrases, seen, phrase);
        foreach (var phrase in BuildCompositionSection(configuration)) AddUnique(phrases, seen, phrase);
        foreach (var phrase in BuildMoodSection(configuration)) AddUnique(phrases, seen, phrase);
        foreach (var phrase in BuildLightingAndColorSection(configuration)) AddUnique(phrases, seen, phrase);
        foreach (var phrase in BuildOutputSection(configuration)) AddUnique(phrases, seen, phrase);

        return new PromptResult
        {
            PositivePrompt = string.Join(", ", phrases),
            NegativePrompt = configuration.UseNegativePrompt ? DefaultNegativePrompt : string.Empty,
        };
    }

    private static string BuildSubjectSection(PromptConfiguration configuration)
    {
        var subject = Clean(configuration.Subject);
        var action = Clean(configuration.Action);
        if (!string.IsNullOrWhiteSpace(subject) && !string.IsNullOrWhiteSpace(action)) return $"{subject}, {action}";
        return !string.IsNullOrWhiteSpace(subject) ? subject : action;
    }

    private static string BuildRelationshipSection(PromptConfiguration configuration) => Clean(configuration.Relationship);

    private IEnumerable<string> BuildStyleSection(PromptConfiguration configuration)
    {
        yield return configuration.ArtStyle switch
        {
            "Cinematic" => "cinematic visual language",
            "Painterly" => "painterly image treatment",
            "Yarn Relief" => "rendered as a complete yarn relief composition",
            "Stained Glass" => "stained glass-inspired design language",
            "Surreal Symbolic" => "surreal symbolic imagery",
            "Concept Art" => "concept art presentation",
            _ => string.Empty,
        };

        yield return configuration.Material switch
        {
            "Yarn" => "yarn-built material presence",
            "Paint" => "paint-rich surface treatment",
            "Glass" => "glass-crafted surfaces",
            "Ink" => "ink-defined mark making",
            "Stone" => "stone-hewn form language",
            "Metal" => "metallic surface character",
            _ => string.Empty,
        };

        yield return MapBand(configuration.Stylization, "grounded visual treatment", "light stylization", "stylized rendering", "strong stylization", "highly stylized visual language");
        yield return MapBand(configuration.Realism, string.Empty, "loosely realistic", "moderately realistic", "high visual realism", "strongly realistic rendering");
        yield return MapBand(configuration.TextureDepth, string.Empty, "light surface grain", "finely worked material texture", "rich tactile surface build-up", "deeply hewn tactile surface relief");
        yield return MapBand(configuration.SurfaceAge, string.Empty, "freshly finished surfaces", "subtle patina", "weathered surface character", "time-worn patina and age marks");

        foreach (var phrase in BuildArtistBlend(configuration))
        {
            yield return phrase;
        }

        if (configuration.Material == "Yarn" && configuration.TextureDepth >= 70)
        {
            yield return "visible fiber structure";
            yield return "layered textile depth";
        }
    }

    private IEnumerable<string> BuildArtistBlend(PromptConfiguration configuration)
    {
        var primary = CreateInfluence(configuration.ArtistInfluencePrimary, configuration.InfluenceStrengthPrimary);
        var secondary = CreateInfluence(configuration.ArtistInfluenceSecondary, configuration.InfluenceStrengthSecondary);

        if (primary is null && secondary is null)
        {
            yield break;
        }

        if (primary is not null && secondary is not null && string.Equals(primary.DisplayName, secondary.DisplayName, StringComparison.OrdinalIgnoreCase))
        {
            secondary = null;
        }

        if (primary is not null && secondary is not null)
        {
            foreach (var phrase in BuildDualArtistBlend(primary, secondary))
            {
                yield return phrase;
            }

            yield break;
        }

        var single = primary ?? secondary;
        if (single is null)
        {
            yield break;
        }

        foreach (var phrase in BuildSingleArtistInfluence(single))
        {
            yield return phrase;
        }
    }

    private IEnumerable<string> BuildDualArtistBlend(ArtistInfluence primary, ArtistInfluence secondary)
    {
        var stronger = primary.Strength >= secondary.Strength ? primary : secondary;
        var lighter = ReferenceEquals(stronger, primary) ? secondary : primary;

        yield return DescribeBlend(stronger, lighter);

        var strongerCategories = (stronger.Strength - lighter.Strength) switch
        {
            >= 35 => new[] { ArtistPhraseCategory.Hallmarks, ArtistPhraseCategory.Composition, ArtistPhraseCategory.Palette, ArtistPhraseCategory.Mood },
            >= 15 => new[] { ArtistPhraseCategory.Hallmarks, ArtistPhraseCategory.Composition, ArtistPhraseCategory.Surface },
            _ => new[] { ArtistPhraseCategory.Hallmarks, ArtistPhraseCategory.Composition },
        };

        var lighterCategories = (stronger.Strength - lighter.Strength) switch
        {
            >= 35 => new[] { ArtistPhraseCategory.Palette, ArtistPhraseCategory.Surface },
            >= 15 => new[] { ArtistPhraseCategory.Palette, ArtistPhraseCategory.Mood },
            _ => new[] { ArtistPhraseCategory.Palette, ArtistPhraseCategory.Surface, ArtistPhraseCategory.Mood },
        };

        foreach (var phrase in SelectCategoryPhrases(stronger, strongerCategories, DeterminePhraseBudget(stronger.Strength, true)))
        {
            yield return phrase;
        }

        foreach (var phrase in SelectCategoryPhrases(lighter, lighterCategories, DeterminePhraseBudget(lighter.Strength, false)))
        {
            yield return phrase;
        }
    }

    private IEnumerable<string> BuildSingleArtistInfluence(ArtistInfluence influence)
    {
        if (influence.Profile is null)
        {
            yield return MapBand(influence.Strength, string.Empty, $"subtle influence from {influence.DisplayName}", $"stylistic cues drawn from {influence.DisplayName}", $"strong influence from {influence.DisplayName}", $"deeply informed by {influence.DisplayName}");
            yield break;
        }

        yield return MapBand(influence.Strength, string.Empty, $"subtle influence from {influence.DisplayName}", $"artist-influenced sensibility drawn from {influence.DisplayName}", $"strongly shaped by {influence.DisplayName}", $"deeply informed by {influence.DisplayName}");

        var categories = new[]
        {
            ArtistPhraseCategory.Hallmarks,
            ArtistPhraseCategory.Composition,
            ArtistPhraseCategory.Palette,
            ArtistPhraseCategory.Surface,
            ArtistPhraseCategory.Mood,
        };

        foreach (var phrase in SelectCategoryPhrases(influence, categories, DeterminePhraseBudget(influence.Strength, true)))
        {
            yield return phrase;
        }
    }

    private static string DescribeBlend(ArtistInfluence stronger, ArtistInfluence lighter)
    {
        if (stronger.Profile is null || lighter.Profile is null)
        {
            return MapBand(stronger.Strength, string.Empty,
                $"light blend of {stronger.DisplayName} and {lighter.DisplayName}",
                $"blended influence from {stronger.DisplayName} and {lighter.DisplayName}",
                $"prominent blend of {stronger.DisplayName} with {lighter.DisplayName}",
                $"deeply fused influence from {stronger.DisplayName} and {lighter.DisplayName}");
        }

        if (stronger.Strength - lighter.Strength >= 25)
        {
            return $"{stronger.DisplayName}-led blend with {lighter.DisplayName} accents";
        }

        return $"balanced blend of {stronger.DisplayName} and {lighter.DisplayName}";
    }

    private ArtistInfluence? CreateInfluence(string artistName, int strength)
    {
        if (strength <= 20 || string.IsNullOrWhiteSpace(artistName) || string.Equals(artistName, "None", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var profile = _artistProfileService.GetProfile(artistName);
        return new ArtistInfluence(profile?.Name ?? artistName, strength, profile);
    }

    private static IEnumerable<string> SelectCategoryPhrases(ArtistInfluence influence, ArtistPhraseCategory[] categories, int budget)
    {
        if (influence.Profile is null || budget <= 0)
        {
            yield break;
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var yielded = 0;

        foreach (var category in categories)
        {
            foreach (var phrase in GetPhrases(influence.Profile, category))
            {
                if (string.IsNullOrWhiteSpace(phrase) || !seen.Add(phrase))
                {
                    continue;
                }

                yield return phrase;
                yielded++;
                break;
            }

            if (yielded >= budget)
            {
                yield break;
            }
        }

        if (yielded >= budget)
        {
            yield break;
        }

        foreach (var phrase in categories.SelectMany(category => GetPhrases(influence.Profile, category)))
        {
            if (string.IsNullOrWhiteSpace(phrase) || !seen.Add(phrase))
            {
                continue;
            }

            yield return phrase;
            yielded++;
            if (yielded >= budget)
            {
                yield break;
            }
        }
    }

    private static IEnumerable<string> GetPhrases(ArtistProfile profile, ArtistPhraseCategory category) => category switch
    {
        ArtistPhraseCategory.Hallmarks => profile.Hallmarks,
        ArtistPhraseCategory.Composition => profile.Composition,
        ArtistPhraseCategory.Palette => profile.Palette,
        ArtistPhraseCategory.Surface => profile.Surface,
        ArtistPhraseCategory.Mood => profile.Mood,
        _ => [],
    };

    private static int DeterminePhraseBudget(int strength, bool primaryWeight)
    {
        if (strength <= 40) return primaryWeight ? 2 : 1;
        if (strength <= 60) return primaryWeight ? 3 : 2;
        if (strength <= 80) return primaryWeight ? 4 : 2;
        return primaryWeight ? 5 : 3;
    }

    private static IEnumerable<string> BuildCompositionSection(PromptConfiguration configuration)
    {
        yield return Lower(configuration.CameraDistance);
        yield return Lower(configuration.CameraAngle);
        yield return MapBand(configuration.BackgroundComplexity, "minimal background", "restrained background", "supporting environmental detail", "rich environmental detail", "densely layered environment");
        yield return MapBand(configuration.MotionEnergy, "still composition", "gentle motion", "active scene energy", "dynamic motion", "high kinetic energy");
        yield return MapBand(configuration.NarrativeDensity, string.Empty, "single-read visual idea", "light narrative layering", "layered storytelling cues", "densely implied narrative world");
        yield return MapBand(configuration.AtmosphericDepth, string.Empty, "slight atmospheric recession", "air-filled spatial depth", "luminous atmospheric depth", "deeply layered atmospheric perspective");
        yield return MapBand(configuration.Chaos, string.Empty, "controlled asymmetry", "restless visual tension", "volatile compositional energy", "orchestrated visual chaos");
    }

    private static IEnumerable<string> BuildMoodSection(PromptConfiguration configuration)
    {
        yield return MapBand(configuration.Whimsy, string.Empty, "subtle whimsy", "playful tone", "strong whimsical energy", "bold comedic whimsy");
        yield return MapBand(configuration.Tension, string.Empty, "light dramatic tension", "noticeable tension", "strong interpersonal tension", "intense dramatic tension");
        yield return MapBand(configuration.Awe, string.Empty, "slight sense of wonder", "atmosphere of wonder", "strong sense of awe", "overwhelming visual grandeur");
        yield return MapBand(configuration.Symbolism, string.Empty, "subtle symbolic cues", "suggestive symbolic motifs", "pronounced allegorical symbolism", "mythic symbolic charge");
        if (configuration.Whimsy >= 70 && configuration.Tension >= 50) yield return "comedic interpersonal tension";
    }

    private static IEnumerable<string> BuildLightingAndColorSection(PromptConfiguration configuration)
    {
        yield return Lower(configuration.Lighting);
        yield return MapBand(configuration.Saturation, "muted color saturation", "restrained saturation", "balanced saturation", "rich color saturation", "vivid color saturation");
        yield return MapBand(configuration.Contrast, "low contrast", "gentle contrast", "balanced contrast", "crisp contrast", "striking contrast");
    }

    private static IEnumerable<string> BuildOutputSection(PromptConfiguration configuration)
    {
        yield return $"aspect ratio {configuration.AspectRatio}";
        if (configuration.PrintReady) { yield return "high detail clarity"; yield return "clean edge definition"; }
        if (configuration.TransparentBackground) { yield return "isolated subject"; yield return "clean background separation"; }
    }

    private static string MapBand(int value, string low, string lowMid, string mid, string high, string veryHigh)
    {
        if (value <= 20) return low;
        if (value <= 40) return lowMid;
        if (value <= 60) return mid;
        if (value <= 80) return high;
        return veryHigh;
    }

    private static void AddUnique(ICollection<string> phrases, ISet<string> seen, string value)
    {
        var cleaned = Clean(value);
        if (string.IsNullOrWhiteSpace(cleaned) || !seen.Add(cleaned)) return;
        phrases.Add(cleaned);
    }

    private static string Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var builder = new StringBuilder(value.Trim());
        builder.Replace("  ", " ");
        return builder.ToString().Trim().Trim(',');
    }

    private static string Lower(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "None", StringComparison.OrdinalIgnoreCase)) return string.Empty;
        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    private enum ArtistPhraseCategory
    {
        Hallmarks,
        Composition,
        Palette,
        Surface,
        Mood,
    }

    private sealed record ArtistInfluence(string DisplayName, int Strength, ArtistProfile? Profile);
}
