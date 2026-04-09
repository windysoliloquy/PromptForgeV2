using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveConceptArtPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded production treatment",
                "light stylization",
                "stylized development rendering",
                "strong art-direction stylization",
                "highly stylized portfolio finish"),
            Realism => MapBand(value,
                string.Empty,
                "loosely realistic rendering",
                "moderately realistic depiction",
                "high visual realism",
                "portfolio-grade realism"),
            TextureDepth => MapBand(value,
                "minimal material texture",
                "light surface articulation",
                "clear material texture",
                "rich tactile material detail",
                "deep material articulation"),
            NarrativeDensity => MapBand(value,
                "single-read visual idea",
                "light story-world cues",
                "layered world context",
                "densely implied story world",
                "world-rich narrative charge"),
            Symbolism => MapBand(value,
                "mostly literal visual language",
                "subtle motif cues",
                "suggestive symbolic cues",
                "pronounced allegorical motifs",
                "mythic symbolic charge"),
            SurfaceAge => MapBand(value,
                "fresh drafting finish",
                "slight development patina",
                "gentle workboard wear",
                "noticeable board character",
                "time-worn reference character"),
            Framing => MapBand(value,
                "intimate framing",
                "tight staging",
                "balanced framing",
                "expansive staging",
                "showcase-scale framing"),
            BackgroundComplexity => MapBand(value,
                "minimal backdrop support",
                "restrained scene support",
                "supporting environment",
                "rich world support",
                "densely layered environment"),
            MotionEnergy => MapBand(value,
                "still moment",
                "gentle motion",
                "active scene energy",
                "dynamic action energy",
                "high kinetic momentum"),
            FocusDepth => MapBand(value,
                "deep focus clarity",
                "moderate depth separation",
                "selective focus",
                "shallow focus",
                "very shallow focus"),
            ImageCleanliness => MapBand(value,
                "raw board finish",
                "light refinement",
                "clean development finish",
                "polished portfolio finish",
                "highly polished portfolio finish"),
            DetailDensity => MapBand(value,
                "sparse detail load",
                "light supporting detail",
                "clear descriptive detail",
                "dense layered detail",
                "high-density detail load"),
            AtmosphericDepth => MapBand(value,
                "limited atmospheric depth",
                "slight spatial recession",
                "air-filled depth",
                "luminous depth layering",
                "deep atmospheric layering"),
            Chaos => MapBand(value,
                "controlled composition",
                "light creative restlessness",
                "active instability",
                "dynamic turbulence",
                "orchestrated chaos"),
            Whimsy => MapBand(value,
                "serious tone",
                "subtle tonal lift",
                "mild creative play",
                "strong expressive energy",
                "bold imaginative flourish"),
            Tension => MapBand(value,
                "low tension",
                "light dramatic tension",
                "noticeable scene tension",
                "strong dramatic pressure",
                "intense pressure"),
            Awe => MapBand(value,
                "grounded scale",
                "slight wonder",
                "atmosphere of significance",
                "strong sense of awe",
                "overwhelming grandeur"),
            Saturation => MapBand(value,
                "muted color",
                "restrained color",
                "balanced color",
                "rich color",
                "luminous color"),
            Contrast => MapBand(value,
                "low contrast",
                "gentle tonal separation",
                "balanced contrast",
                "crisp contrast",
                "striking contrast"),
            Temperature => MapBand(value,
                "cool balance",
                "slightly cool tone",
                "neutral balance",
                "warm balance",
                "heated warmth"),
            LightingIntensity => MapBand(value,
                "dim light",
                "soft light",
                "balanced illumination",
                "bright light",
                "radiant lighting"),
            CameraDistance => MapBand(value,
                "extreme close view",
                "close view",
                "mid-distance view",
                "wide view",
                "far-set view"),
            CameraAngle => MapBand(value,
                "low angle",
                "slightly lowered viewpoint",
                "eye-level angle",
                "slightly elevated angle",
                "high vantage"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyConceptArtGuardrails(sliderKey, value, configuration, ApplyConceptArtPhraseEconomy(phrase));
    }

    public static string ResolveConceptArtGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded production treatment", "light stylization", "stylized development rendering", "strong art-direction stylization", "highly stylized portfolio finish" },
            Realism => new[] { "omit explicit realism", "loosely realistic rendering", "moderately realistic depiction", "high visual realism", "portfolio-grade realism" },
            TextureDepth => new[] { "minimal material texture", "light surface articulation", "clear material texture", "rich tactile material detail", "deep material articulation" },
            NarrativeDensity => new[] { "single-read visual idea", "light story-world cues", "layered world context", "densely implied story world", "world-rich narrative charge" },
            Symbolism => new[] { "mostly literal visual language", "subtle motif cues", "suggestive symbolic cues", "pronounced allegorical motifs", "mythic symbolic charge" },
            SurfaceAge => new[] { "fresh drafting finish", "slight development patina", "gentle workboard wear", "noticeable board character", "time-worn reference character" },
            Framing => new[] { "intimate framing", "tight staging", "balanced framing", "expansive staging", "showcase-scale framing" },
            BackgroundComplexity => new[] { "minimal backdrop support", "restrained scene support", "supporting environment", "rich world support", "densely layered environment" },
            MotionEnergy => new[] { "still moment", "gentle motion", "active scene energy", "dynamic action energy", "high kinetic momentum" },
            FocusDepth => new[] { "deep focus clarity", "moderate depth separation", "selective focus", "shallow focus", "very shallow focus" },
            ImageCleanliness => new[] { "raw board finish", "light refinement", "clean development finish", "polished portfolio finish", "highly polished portfolio finish" },
            DetailDensity => new[] { "sparse detail load", "light supporting detail", "clear descriptive detail", "dense layered detail", "high-density detail load" },
            AtmosphericDepth => new[] { "limited atmospheric depth", "slight spatial recession", "air-filled depth", "luminous depth layering", "deep atmospheric layering" },
            Chaos => new[] { "controlled composition", "light creative restlessness", "active instability", "dynamic turbulence", "orchestrated chaos" },
            Whimsy => new[] { "serious tone", "subtle tonal lift", "mild creative play", "strong expressive energy", "bold imaginative flourish" },
            Tension => new[] { "low tension", "light dramatic tension", "noticeable scene tension", "strong dramatic pressure", "intense pressure" },
            Awe => new[] { "grounded scale", "slight wonder", "atmosphere of significance", "strong sense of awe", "overwhelming grandeur" },
            Saturation => new[] { "muted color", "restrained color", "balanced color", "rich color", "luminous color" },
            Contrast => new[] { "low contrast", "gentle tonal separation", "balanced contrast", "crisp contrast", "striking contrast" },
            Temperature => new[] { "cool balance", "slightly cool tone", "neutral balance", "warm balance", "heated warmth" },
            LightingIntensity => new[] { "dim light", "soft light", "balanced illumination", "bright light", "radiant lighting" },
            CameraDistance => new[] { "extreme close view", "close view", "mid-distance view", "wide view", "far-set view" },
            CameraAngle => new[] { "low angle", "slightly lowered viewpoint", "eye-level angle", "slightly elevated angle", "high vantage" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveConceptArtDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddConceptArtDescriptor(phrases, seen, "concept art");

        var subtypeDescriptor = ResolveConceptArtSubtypeDescriptor(configuration.ConceptArtSubtype);
        if (!string.IsNullOrWhiteSpace(subtypeDescriptor))
        {
            AddConceptArtDescriptor(phrases, seen, subtypeDescriptor);
        }

        foreach (var phrase in ResolveConceptArtModifierDescriptors(configuration))
        {
            AddConceptArtDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveConceptArtLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "soft reference light",
            "Golden hour" => "golden-hour light",
            "Dramatic studio light" => "studio lighting",
            "Overcast" => "neutral overcast light",
            "Moonlit" => "moody atmospheric light",
            "Soft glow" => "subtle glow",
            "Dusk haze" => "late-day haze",
            "Warm directional light" => "warm directional light",
            "Volumetric cinematic light" => "atmospheric volumetric light",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveConceptArtSubtypeDescriptor(string conceptArtSubtype)
    {
        return conceptArtSubtype switch
        {
            "keyframe-concept" => "keyframe scene staging",
            "environment-concept" => "environment development",
            "character-concept" => "character development",
            "creature-concept" => "creature development",
            "costume-concept" => "costume development",
            "prop-concept" => "prop development",
            "vehicle-concept" => "vehicle development",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveConceptArtModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetConceptArtModifierPriority(configuration.ConceptArtSubtype);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Design Callouts" when configuration.ConceptArtDesignCallouts => "callout annotations",
                "Turnaround Readability" when configuration.ConceptArtTurnaroundReadability => "turnaround-ready readability",
                "Material Breakdown" when configuration.ConceptArtMaterialBreakdown => "material-breakdown clarity",
                "Scale Reference" when configuration.ConceptArtScaleReference => "clear scale reference",
                "Worldbuilding Accents" when configuration.ConceptArtWorldbuildingAccents => "world support accents",
                "Production Notes Feel" when configuration.ConceptArtProductionNotesFeel => "board-note energy",
                "Silhouette Clarity" when configuration.ConceptArtSilhouetteClarity => "strong silhouette clarity",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetConceptArtModifierPriority(string conceptArtSubtype)
    {
        return conceptArtSubtype switch
        {
            "environment-concept" => ["Worldbuilding Accents", "Scale Reference", "Design Callouts", "Production Notes Feel", "Material Breakdown", "Turnaround Readability", "Silhouette Clarity"],
            "character-concept" => ["Silhouette Clarity", "Turnaround Readability", "Material Breakdown", "Design Callouts", "Scale Reference", "Worldbuilding Accents", "Production Notes Feel"],
            "creature-concept" => ["Silhouette Clarity", "Material Breakdown", "Scale Reference", "Design Callouts", "Turnaround Readability", "Worldbuilding Accents", "Production Notes Feel"],
            "costume-concept" => ["Material Breakdown", "Turnaround Readability", "Silhouette Clarity", "Design Callouts", "Scale Reference", "Worldbuilding Accents", "Production Notes Feel"],
            "prop-concept" => ["Design Callouts", "Material Breakdown", "Turnaround Readability", "Scale Reference", "Silhouette Clarity", "Worldbuilding Accents", "Production Notes Feel"],
            "vehicle-concept" => ["Design Callouts", "Scale Reference", "Silhouette Clarity", "Material Breakdown", "Turnaround Readability", "Worldbuilding Accents", "Production Notes Feel"],
            "keyframe-concept" => ["Worldbuilding Accents", "Scale Reference", "Silhouette Clarity", "Design Callouts", "Production Notes Feel", "Material Breakdown", "Turnaround Readability"],
            _ => ["Worldbuilding Accents", "Scale Reference", "Silhouette Clarity", "Design Callouts", "Production Notes Feel", "Material Breakdown", "Turnaround Readability"],
        };
    }

    private static void AddConceptArtDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyConceptArtGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Stylization, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.ImageCleanliness >= 61)
        {
            return "highly stylized portfolio finish";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity >= 61)
        {
            return "densely layered environment";
        }

        if (string.Equals(sliderKey, Symbolism, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity <= 40)
        {
            return "mythic symbolic charge";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.ImageCleanliness >= 61)
        {
            return "striking contrast";
        }

        return phrase;
    }

    private static string ApplyConceptArtPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("concept-art ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("concept art ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("concept-art", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("concept art", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }
}
