using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveThreeDRenderPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded 3D treatment",
                "light 3D stylization",
                "stylized CGI rendering",
                "strong 3D stylization",
                "highly stylized CGI presentation"),
            Realism => MapBand(value,
                string.Empty,
                "loosely realistic 3D rendering",
                "moderately realistic CGI rendering",
                "high visual realism in 3D",
                "photoreal 3D rendering"),
            TextureDepth => MapBand(value,
                "minimal surface texture",
                "light rendered texture",
                "clear material texture",
                "rich rendered surface detail",
                "deeply worked material definition"),
            NarrativeDensity => MapBand(value,
                "single-read render concept",
                "light story suggestion",
                "scene-supporting story cues",
                "layered rendered storytelling",
                "world-rich CGI narrative"),
            Symbolism => MapBand(value,
                "literal presentation",
                "subtle symbolic cue",
                "suggestive design motif",
                "pronounced symbolic intent",
                "mythic symbolic charge"),
            SurfaceAge => MapBand(value,
                "fresh surface finish",
                "slight production wear",
                "gentle material patina",
                "noticeable render wear",
                "time-softened surface finish"),
            Framing => MapBand(value,
                "intimate render framing",
                "tight presentation framing",
                "balanced render framing",
                "expansive scene framing",
                "showcase-scale framing"),
            BackgroundComplexity => MapBand(value,
                "minimal render background",
                "restrained scene support",
                "supporting rendered environment",
                "rich CGI environment",
                "densely layered rendered environment"),
            MotionEnergy => MapBand(value,
                "still render composition",
                "gentle scene motion",
                "active rendered energy",
                "dynamic presentation energy",
                "high kinetic presentation motion"),
            FocusDepth => MapBand(value,
                "deep focus clarity",
                "moderate focus separation",
                "selective render focus",
                "shallow depth of field",
                "very shallow presentation focus"),
            ImageCleanliness => MapBand(value,
                "raw render finish",
                "lightly refined render finish",
                "clean CGI finish",
                "polished render finish",
                "ultra-clean commercial finish"),
            DetailDensity => MapBand(value,
                "sparse digital detail",
                "light supporting detail",
                "clear modeled detail",
                "dense rendered detail layering",
                "high-density production detail"),
            AtmosphericDepth => MapBand(value,
                "limited atmospheric depth",
                "slight rendered recession",
                "air-filled scene depth",
                "luminous rendered depth",
                "deep CGI atmospheric layering"),
            Chaos => MapBand(value,
                "controlled presentation",
                "restless scene tension",
                "active render energy",
                "dynamic visual turbulence",
                "orchestrated digital chaos"),
            Whimsy => MapBand(value,
                "serious tone",
                "light stylistic charm",
                "balanced visual playfulness",
                "strong stylized energy",
                "bold presentation energy"),
            Tension => MapBand(value,
                "low dramatic tension",
                "light dramatic charge",
                "scene-level tension",
                "strong render tension",
                "intense dramatic pressure"),
            Awe => MapBand(value,
                "grounded scale",
                "slight visual wonder",
                "atmosphere of spectacle",
                "strong presentation awe",
                "overwhelming render grandeur"),
            Temperature => MapBand(value,
                "cool render balance",
                "slightly cool digital balance",
                "neutral render temperature",
                "warm presentation balance",
                "heated screen color"),
            LightingIntensity => MapBand(value,
                "soft rendered daylight",
                "studio render lighting",
                "balanced render lighting",
                "bright CGI lighting",
                "radiant presentation lighting"),
            Saturation => MapBand(value,
                "muted render color",
                "restrained digital color",
                "balanced render color",
                "rich CGI color",
                "vivid rendered color"),
            Contrast => MapBand(value,
                "low rendered contrast",
                "gentle tonal separation",
                "balanced render contrast",
                "crisp CGI contrast",
                "striking render contrast"),
            CameraDistance => MapBand(value,
                "extreme close render view",
                "close product view",
                "mid-distance render view",
                "wide presentation view",
                "far-set scene view"),
            CameraAngle => MapBand(value,
                "eye-level view",
                "slightly lowered viewpoint",
                "balanced presentation angle",
                "dramatic low angle",
                "showcase elevated angle"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyThreeDRenderGuardrails(sliderKey, value, configuration, ApplyThreeDRenderPhraseEconomy(phrase));
    }

    public static string ResolveThreeDRenderGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded 3D treatment", "light 3D stylization", "stylized CGI rendering", "strong 3D stylization", "highly stylized CGI presentation" },
            Realism => new[] { "omit explicit realism", "loosely realistic 3D rendering", "moderately realistic CGI rendering", "high visual realism in 3D", "photoreal 3D rendering" },
            TextureDepth => new[] { "minimal surface texture", "light rendered texture", "clear material texture", "rich rendered surface detail", "deeply worked material definition" },
            NarrativeDensity => new[] { "single-read render concept", "light story suggestion", "scene-supporting story cues", "layered rendered storytelling", "world-rich CGI narrative" },
            Symbolism => new[] { "literal presentation", "subtle symbolic cue", "suggestive design motif", "pronounced symbolic intent", "mythic symbolic charge" },
            SurfaceAge => new[] { "fresh surface finish", "slight production wear", "gentle material patina", "noticeable render wear", "time-softened surface finish" },
            Framing => new[] { "intimate render framing", "tight presentation framing", "balanced render framing", "expansive scene framing", "showcase-scale framing" },
            BackgroundComplexity => new[] { "minimal render background", "restrained scene support", "supporting rendered environment", "rich CGI environment", "densely layered rendered environment" },
            MotionEnergy => new[] { "still render composition", "gentle scene motion", "active rendered energy", "dynamic presentation energy", "high kinetic presentation motion" },
            FocusDepth => new[] { "deep focus clarity", "moderate focus separation", "selective render focus", "shallow depth of field", "very shallow presentation focus" },
            ImageCleanliness => new[] { "raw render finish", "lightly refined render finish", "clean CGI finish", "polished render finish", "ultra-clean commercial finish" },
            DetailDensity => new[] { "sparse digital detail", "light supporting detail", "clear modeled detail", "dense rendered detail layering", "high-density production detail" },
            AtmosphericDepth => new[] { "limited atmospheric depth", "slight rendered recession", "air-filled scene depth", "luminous rendered depth", "deep CGI atmospheric layering" },
            Chaos => new[] { "controlled presentation", "restless scene tension", "active render energy", "dynamic visual turbulence", "orchestrated digital chaos" },
            Whimsy => new[] { "serious tone", "light stylistic charm", "balanced visual playfulness", "strong stylized energy", "bold presentation energy" },
            Tension => new[] { "low dramatic tension", "light dramatic charge", "scene-level tension", "strong render tension", "intense dramatic pressure" },
            Awe => new[] { "grounded scale", "slight visual wonder", "atmosphere of spectacle", "strong presentation awe", "overwhelming render grandeur" },
            Temperature => new[] { "cool render balance", "slightly cool digital balance", "neutral render temperature", "warm presentation balance", "heated screen color" },
            LightingIntensity => new[] { "soft rendered daylight", "studio render lighting", "balanced render lighting", "bright CGI lighting", "radiant presentation lighting" },
            Saturation => new[] { "muted render color", "restrained digital color", "balanced render color", "rich CGI color", "vivid rendered color" },
            Contrast => new[] { "low rendered contrast", "gentle tonal separation", "balanced render contrast", "crisp CGI contrast", "striking render contrast" },
            CameraDistance => new[] { "extreme close render view", "close product view", "mid-distance render view", "wide presentation view", "far-set scene view" },
            CameraAngle => new[] { "eye-level view", "slightly lowered viewpoint", "balanced presentation angle", "dramatic low angle", "showcase elevated angle" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveThreeDRenderDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddThreeDRenderDescriptor(phrases, seen, "3D render");

        var subtypeDescriptor = ResolveThreeDRenderSubtypeDescriptor(configuration.ThreeDRenderSubtype);
        if (!string.IsNullOrWhiteSpace(subtypeDescriptor))
        {
            AddThreeDRenderDescriptor(phrases, seen, subtypeDescriptor);
        }

        foreach (var phrase in ResolveThreeDRenderModifierDescriptors(configuration))
        {
            AddThreeDRenderDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveThreeDRenderLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "soft rendered daylight",
            "Golden hour" => "commercial golden-hour render light",
            "Dramatic studio light" => "studio render lighting",
            "Overcast" => "diffuse studio daylight",
            "Moonlit" => "cool atmospheric render light",
            "Soft glow" => "soft rendered glow",
            "Dusk haze" => "neon-lit render atmosphere",
            "Warm directional light" => "commercial product lighting",
            "Volumetric cinematic light" => "volumetric render lighting",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveThreeDRenderSubtypeDescriptor(string threeDRenderSubtype)
    {
        return threeDRenderSubtype switch
        {
            "general-cgi" => "clean CGI presentation",
            "stylized-3d" => "designed digital shaping",
            "photoreal-3d" => "photoreal material realism",
            "game-asset" => "game-ready presentation clarity",
            "animated-feature" => "feature-animation polish",
            "product-visualization" => "studio-grade product presentation",
            "sci-fi-hard-surface" => "engineered hard-surface precision",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveThreeDRenderModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetThreeDRenderModifierPriority(configuration.ThreeDRenderSubtype);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Global Illumination" when configuration.ThreeDRenderGlobalIllumination => "global-illumination light bounce",
                "Volumetric Lighting" when configuration.ThreeDRenderVolumetricLighting => "volumetric light shafts",
                "Ray-Traced Reflections" when configuration.ThreeDRenderRayTracedReflections => "ray-traced reflection fidelity",
                "Depth of Field" when configuration.ThreeDRenderDepthOfField => "cinematic depth of field",
                "Subsurface Scattering" when configuration.ThreeDRenderSubsurfaceScattering => "subsurface light transmission",
                "Hard-Surface Precision" when configuration.ThreeDRenderHardSurfacePrecision => "hard-surface edge precision",
                "Studio Backdrop" when configuration.ThreeDRenderStudioBackdrop => "studio-backdrop presentation",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetThreeDRenderModifierPriority(string threeDRenderSubtype)
    {
        return threeDRenderSubtype switch
        {
            "stylized-3d" => ["Global Illumination", "Depth of Field", "Studio Backdrop", "Volumetric Lighting", "Ray-Traced Reflections", "Subsurface Scattering", "Hard-Surface Precision"],
            "photoreal-3d" => ["Ray-Traced Reflections", "Global Illumination", "Subsurface Scattering", "Depth of Field", "Volumetric Lighting", "Hard-Surface Precision", "Studio Backdrop"],
            "game-asset" => ["Hard-Surface Precision", "Studio Backdrop", "Depth of Field", "Global Illumination", "Ray-Traced Reflections", "Subsurface Scattering", "Volumetric Lighting"],
            "animated-feature" => ["Subsurface Scattering", "Global Illumination", "Depth of Field", "Studio Backdrop", "Volumetric Lighting", "Ray-Traced Reflections", "Hard-Surface Precision"],
            "product-visualization" => ["Studio Backdrop", "Ray-Traced Reflections", "Global Illumination", "Depth of Field", "Subsurface Scattering", "Volumetric Lighting", "Hard-Surface Precision"],
            "sci-fi-hard-surface" => ["Hard-Surface Precision", "Ray-Traced Reflections", "Volumetric Lighting", "Global Illumination", "Depth of Field", "Subsurface Scattering", "Studio Backdrop"],
            _ => ["Global Illumination", "Depth of Field", "Volumetric Lighting", "Ray-Traced Reflections", "Subsurface Scattering", "Hard-Surface Precision", "Studio Backdrop"],
        };
    }

    private static void AddThreeDRenderDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyThreeDRenderGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Realism >= 60)
        {
            return "striking render contrast";
        }

        if (string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.DetailDensity >= 60)
        {
            return "ultra-clean commercial finish";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.DetailDensity <= 40)
        {
            return "densely layered rendered environment";
        }

        if (string.Equals(sliderKey, DetailDensity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "high-density production detail";
        }

        return phrase;
    }

    private static string ApplyThreeDRenderPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("3D render ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("3D-render ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("3D ", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }
}
