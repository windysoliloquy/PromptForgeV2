using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveCinematicPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded scene treatment",
                "light directorial influence",
                "director-led staging",
                "strong visual direction",
                "high-authorial screen language"),
            Realism => MapBand(value,
                string.Empty,
                "loosely observed rendering",
                "moderately realistic depiction",
                "high visual realism",
                "capture-grade realism"),
            TextureDepth => MapBand(value,
                "minimal surface texture",
                "light material indication",
                "clear surface tactility",
                "rich material presence",
                "deep tactile detail"),
            NarrativeDensity => MapBand(value,
                "single-shot visual beat",
                "light scene implication",
                "scene-driven cues",
                "layered narrative suggestion",
                "world-rich narrative charge"),
            Symbolism => MapBand(value,
                "literal framing",
                "subtle symbolic cues",
                "suggestive motif use",
                "pronounced thematic elements",
                "mythic symbolic charge"),
            SurfaceAge => MapBand(value,
                "fresh finish",
                "slight surface patina",
                "gentle wear character",
                "noticeable analog aging",
                "time-softened finish"),
            Framing => MapBand(value,
                "intimate framing",
                "tight staging",
                "balanced composition",
                "expansive staging",
                "large-format framing"),
            BackgroundComplexity => MapBand(value,
                "minimal set presence",
                "restrained set detail",
                "supporting environment",
                "rich staging detail",
                "densely layered environment"),
            MotionEnergy => MapBand(value,
                "held frame",
                "gentle motion",
                "active cinematic energy",
                "dynamic motion",
                "high kinetic momentum"),
            FocusDepth => MapBand(value,
                "deep focus clarity",
                "moderate depth separation",
                "selective focus",
                "shallow depth of field",
                "extreme focal isolation"),
            ImageCleanliness => MapBand(value,
                "raw analog finish",
                "light refinement",
                "clean production finish",
                "polished theatrical finish",
                "ultra-clean presentation"),
            DetailDensity => MapBand(value,
                "sparse scene detail",
                "light descriptive detail",
                "balanced scene detail",
                "rich layered detail",
                "dense information load"),
            AtmosphericDepth => MapBand(value,
                "slight depth falloff",
                "light spatial recession",
                "air-filled depth",
                "luminous atmosphere",
                "deep layered atmosphere"),
            Chaos => MapBand(value,
                "controlled composition",
                "restless tension",
                "active instability",
                "dynamic turbulence",
                "orchestrated chaos"),
            Whimsy => MapBand(value,
                "serious tone",
                "light emotional lift",
                "subtle warmth",
                "gentle tonal softness",
                "playful dramatic tone"),
            Tension => MapBand(value,
                "low dramatic tension",
                "light dramatic charge",
                "scene-level tension",
                "strong dramatic pressure",
                "high-stakes intensity"),
            Awe => MapBand(value,
                "grounded scale",
                "slight wonder",
                "sense of spectacle",
                "strong scale presence",
                "overwhelming grandeur"),
            Temperature => MapBand(value,
                "cool balance",
                "slightly cool tone",
                "neutral balance",
                "warm tone",
                "heated warmth"),
            LightingIntensity => MapBand(value,
                "dim light",
                "soft daylight",
                "balanced illumination",
                "bright scene light",
                "radiant lighting"),
            Saturation => MapBand(value,
                "muted color",
                "restrained color",
                "balanced color",
                "rich color",
                "luminous color"),
            Contrast => MapBand(value,
                "low contrast",
                "gentle separation",
                "balanced contrast",
                "crisp separation",
                "striking contrast"),
            CameraDistance => MapBand(value,
                "close view",
                "near scene view",
                "mid-distance view",
                "wide scene view",
                "far-set view"),
            CameraAngle => MapBand(value,
                "eye-level view",
                "slightly lowered view",
                "balanced angle",
                "low angle",
                "elevated view"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyCinematicGuardrails(sliderKey, value, configuration, ApplyCinematicPhraseEconomy(phrase));
    }

    public static string ResolveCinematicGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded scene treatment", "light directorial influence", "director-led staging", "strong visual direction", "high-authorial screen language" },
            Realism => new[] { "omit explicit realism", "loosely observed rendering", "moderately realistic depiction", "high visual realism", "capture-grade realism" },
            TextureDepth => new[] { "minimal surface texture", "light material indication", "clear surface tactility", "rich material presence", "deep tactile detail" },
            NarrativeDensity => new[] { "single-shot visual beat", "light scene implication", "scene-driven cues", "layered narrative suggestion", "world-rich narrative charge" },
            Symbolism => new[] { "literal framing", "subtle symbolic cues", "suggestive motif use", "pronounced thematic elements", "mythic symbolic charge" },
            SurfaceAge => new[] { "fresh finish", "slight surface patina", "gentle wear character", "noticeable analog aging", "time-softened finish" },
            Framing => new[] { "intimate framing", "tight staging", "balanced composition", "expansive staging", "large-format framing" },
            BackgroundComplexity => new[] { "minimal set presence", "restrained set detail", "supporting environment", "rich staging detail", "densely layered environment" },
            MotionEnergy => new[] { "held frame", "gentle motion", "active scene energy", "dynamic motion", "high kinetic momentum" },
            FocusDepth => new[] { "deep focus clarity", "moderate depth separation", "selective focus", "shallow depth of field", "extreme focal isolation" },
            ImageCleanliness => new[] { "raw analog finish", "light refinement", "clean production finish", "polished theatrical finish", "ultra-clean presentation" },
            DetailDensity => new[] { "sparse scene detail", "light descriptive detail", "balanced scene detail", "rich layered detail", "dense information load" },
            AtmosphericDepth => new[] { "slight depth falloff", "light spatial recession", "air-filled depth", "luminous atmosphere", "deep layered atmosphere" },
            Chaos => new[] { "controlled composition", "restless tension", "active instability", "dynamic turbulence", "orchestrated chaos" },
            Whimsy => new[] { "serious tone", "light emotional lift", "subtle warmth", "gentle tonal softness", "playful dramatic tone" },
            Tension => new[] { "low dramatic tension", "light dramatic charge", "scene-level tension", "strong dramatic pressure", "high-stakes intensity" },
            Awe => new[] { "grounded scale", "slight wonder", "sense of spectacle", "strong scale presence", "overwhelming grandeur" },
            Temperature => new[] { "cool balance", "slightly cool tone", "neutral balance", "warm tone", "heated warmth" },
            LightingIntensity => new[] { "dim light", "soft daylight", "balanced illumination", "bright scene light", "radiant lighting" },
            Saturation => new[] { "muted color", "restrained color", "balanced color", "rich color", "luminous color" },
            Contrast => new[] { "low contrast", "gentle separation", "balanced contrast", "crisp separation", "striking contrast" },
            CameraDistance => new[] { "close view", "near scene view", "mid-distance view", "wide scene view", "far-set view" },
            CameraAngle => new[] { "eye-level view", "slightly lowered view", "balanced angle", "low angle", "elevated view" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveCinematicDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddCinematicDescriptor(phrases, seen, "cinematic film still");

        var subtypeDescriptor = ResolveCinematicSubtypeDescriptor(configuration.CinematicSubtype);
        if (!string.IsNullOrWhiteSpace(subtypeDescriptor))
        {
            AddCinematicDescriptor(phrases, seen, subtypeDescriptor);
        }

        foreach (var phrase in ResolveCinematicModifierDescriptors(configuration))
        {
            AddCinematicDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveCinematicLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "soft daylight",
            "Golden hour" => "golden-hour light",
            "Dramatic studio light" => "dramatic stage lighting",
            "Overcast" => "muted overcast light",
            "Moonlit" => "moody moonlight",
            "Soft glow" => "soft glow",
            "Dusk haze" => "dusk haze light",
            "Warm directional light" => "warm directional light",
            "Volumetric cinematic light" => "volumetric light",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveCinematicSubtypeDescriptor(string cinematicSubtype)
    {
        return cinematicSubtype switch
        {
            "general-film-still" => string.Empty,
            "prestige-drama" => "dramatic restraint",
            "thriller-suspense" => "suspense-driven scene tension",
            "noir-neo-noir" => "shadow-heavy noir framing",
            "epic-blockbuster" => "large-scale spectacle",
            "intimate-indie" => "intimate observational staging",
            "sci-fi-cinema" => "futurist staging",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveCinematicModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetCinematicModifierPriority(configuration.CinematicSubtype);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Letterboxed Framing" when configuration.CinematicLetterboxedFraming => "letterboxed framing",
                "Shallow Depth of Field" when configuration.CinematicShallowDepthOfField => "shallow depth of field",
                "Practical Lighting" when configuration.CinematicPracticalLighting => "practical-light motivation",
                "Atmospheric Haze" when configuration.CinematicAtmosphericHaze => "atmospheric haze layering",
                "Film Grain" when configuration.CinematicFilmGrain => "subtle film grain",
                "Anamorphic Flares" when configuration.CinematicAnamorphicFlares => "anamorphic flare accents",
                "Dramatic Backlight" when configuration.CinematicDramaticBacklight => "backlight separation",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetCinematicModifierPriority(string cinematicSubtype)
    {
        return cinematicSubtype switch
        {
            "prestige-drama" => ["Practical Lighting", "Shallow Depth of Field", "Film Grain", "Letterboxed Framing", "Dramatic Backlight", "Atmospheric Haze", "Anamorphic Flares"],
            "thriller-suspense" => ["Practical Lighting", "Atmospheric Haze", "Dramatic Backlight", "Shallow Depth of Field", "Film Grain", "Letterboxed Framing", "Anamorphic Flares"],
            "noir-neo-noir" => ["Dramatic Backlight", "Practical Lighting", "Film Grain", "Atmospheric Haze", "Shallow Depth of Field", "Letterboxed Framing", "Anamorphic Flares"],
            "epic-blockbuster" => ["Letterboxed Framing", "Atmospheric Haze", "Anamorphic Flares", "Dramatic Backlight", "Practical Lighting", "Shallow Depth of Field", "Film Grain"],
            "intimate-indie" => ["Shallow Depth of Field", "Practical Lighting", "Film Grain", "Letterboxed Framing", "Atmospheric Haze", "Dramatic Backlight", "Anamorphic Flares"],
            "sci-fi-cinema" => ["Anamorphic Flares", "Atmospheric Haze", "Practical Lighting", "Letterboxed Framing", "Shallow Depth of Field", "Film Grain", "Dramatic Backlight"],
            _ => ["Shallow Depth of Field", "Practical Lighting", "Film Grain", "Atmospheric Haze", "Dramatic Backlight", "Anamorphic Flares", "Letterboxed Framing"],
        };
    }

    private static void AddCinematicDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyCinematicGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "luminous color";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "deep layered atmosphere";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return "densely layered environment";
        }

        return phrase;
    }

    private static string ApplyCinematicPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("cinematic ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("cinematic-", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }
}
