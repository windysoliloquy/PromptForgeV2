using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolvePixelArtPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded sprite treatment",
                "light graphic lift",
                "clear stylized rendering",
                "strong pixel-art stylization",
                "authorial tilework"),
            Realism => MapBand(value,
                string.Empty,
                "loosely observed sprite depiction",
                "moderately realistic sprite depiction",
                "high-legibility representational form",
                "advanced representational rendering"),
            DetailDensity => MapBand(value,
                "sparse information load",
                "light sprite detailing",
                "clear descriptive detail",
                "dense layered detail",
                "high-density information packing"),
            BackgroundComplexity => MapBand(value,
                "minimal backdrop support",
                "restrained backdrop detail",
                "supporting environment structure",
                "rich scene support",
                "densely layered environment"),
            Contrast => MapBand(value,
                "low value contrast",
                "gentle value separation",
                "balanced tonal snap",
                "crisp sprite contrast",
                "striking value separation"),
            Saturation => MapBand(value,
                "muted palette",
                "restrained palette color",
                "balanced color charge",
                "rich palette intensity",
                "vivid chromatic punch"),
            ImageCleanliness => MapBand(value,
                "raw sprite finish",
                "light refinement",
                "clean sprite presentation",
                "polished presentation",
                "ultra-clean finish"),
            NarrativeDensity => MapBand(value,
                "single-read image idea",
                "light scene implication",
                "clear gameplay/story cues",
                "layered scenario storytelling",
                "world-rich narrative charge"),
            Framing => MapBand(value,
                "tight sprite framing",
                "close gameplay framing",
                "balanced scene framing",
                "expansive display framing",
                "showcase-scale staging"),
            CameraDistance => MapBand(value,
                "extreme close sprite view",
                "close view",
                "mid-distance scene view",
                "wide gameplay view",
                "far-set scene view"),
            TextureDepth => MapBand(value,
                "minimal texture indication",
                "light texture suggestion",
                "clear material cueing",
                "rich surface work",
                "dense surface articulation"),
            FocusDepth => MapBand(value,
                "uniform scene clarity",
                "light depth separation",
                "controlled scene separation",
                "selective depth emphasis",
                "strong layered depth emphasis"),
            Symbolism => MapBand(value,
                "mostly literal image language",
                "subtle symbolic cues",
                "suggestive motif use",
                "pronounced symbolic motifs",
                "mythic symbolic charge"),
            AtmosphericDepth => MapBand(value,
                "limited atmospheric depth",
                "slight spatial recession",
                "air-filled scene depth",
                "luminous atmosphere",
                "deep layered atmosphere"),
            SurfaceAge => MapBand(value,
                "fresh finish",
                "slight surface wear",
                "gentle aging",
                "noticeable retro wear",
                "time-softened character"),
            Awe => MapBand(value,
                "grounded scale",
                "slight visual wonder",
                "atmosphere of significance",
                "strong scale awe",
                "overwhelming grandeur"),
            Whimsy => MapBand(value,
                "serious tone",
                "light playful charm",
                "balanced visual playfulness",
                "strong playful energy",
                "bold arcade play"),
            Tension => MapBand(value,
                "low tension",
                "light dramatic charge",
                "scene-level pressure",
                "strong gameplay tension",
                "intense arcade pressure"),
            MotionEnergy => MapBand(value,
                "still composition",
                "gentle scene motion",
                "active gameplay energy",
                "dynamic motion",
                "high-kinetic arcade motion"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyPixelArtGuardrails(sliderKey, value, configuration, ApplyPixelArtPhraseEconomy(phrase));
    }

    public static string ResolvePixelArtGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded sprite treatment", "light graphic lift", "clear stylized rendering", "strong pixel-art stylization", "authorial tilework" },
            Realism => new[] { "omit explicit realism", "loosely observed sprite depiction", "moderately realistic sprite depiction", "high-legibility representational form", "advanced representational rendering" },
            DetailDensity => new[] { "sparse information load", "light sprite detailing", "clear descriptive detail", "dense layered detail", "high-density information packing" },
            BackgroundComplexity => new[] { "minimal backdrop support", "restrained backdrop detail", "supporting environment structure", "rich scene support", "densely layered environment" },
            Contrast => new[] { "low value contrast", "gentle value separation", "balanced tonal snap", "crisp sprite contrast", "striking value separation" },
            Saturation => new[] { "muted palette", "restrained palette color", "balanced color charge", "rich palette intensity", "vivid chromatic punch" },
            ImageCleanliness => new[] { "raw sprite finish", "light refinement", "clean sprite presentation", "polished presentation", "ultra-clean finish" },
            NarrativeDensity => new[] { "single-read image idea", "light scene implication", "clear gameplay/story cues", "layered scenario storytelling", "world-rich narrative charge" },
            Framing => new[] { "tight sprite framing", "close gameplay framing", "balanced scene framing", "expansive display framing", "showcase-scale staging" },
            CameraDistance => new[] { "extreme close sprite view", "close view", "mid-distance scene view", "wide gameplay view", "far-set scene view" },
            TextureDepth => new[] { "minimal texture indication", "light texture suggestion", "clear material cueing", "rich surface work", "dense surface articulation" },
            FocusDepth => new[] { "uniform scene clarity", "light depth separation", "controlled scene separation", "selective depth emphasis", "strong layered depth emphasis" },
            Symbolism => new[] { "mostly literal image language", "subtle symbolic cues", "suggestive motif use", "pronounced symbolic motifs", "mythic symbolic charge" },
            AtmosphericDepth => new[] { "limited atmospheric depth", "slight spatial recession", "air-filled scene depth", "luminous atmosphere", "deep layered atmosphere" },
            SurfaceAge => new[] { "fresh finish", "slight surface wear", "gentle aging", "noticeable retro wear", "time-softened character" },
            Awe => new[] { "grounded scale", "slight visual wonder", "atmosphere of significance", "strong scale awe", "overwhelming grandeur" },
            Whimsy => new[] { "serious tone", "light playful charm", "balanced visual playfulness", "strong playful energy", "bold arcade play" },
            Tension => new[] { "low tension", "light dramatic charge", "scene-level pressure", "strong gameplay tension", "intense arcade pressure" },
            MotionEnergy => new[] { "still composition", "gentle scene motion", "active gameplay energy", "dynamic motion", "high-kinetic arcade motion" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolvePixelArtDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddPixelArtDescriptor(phrases, seen, "pixel art");
        AddPixelArtDescriptor(phrases, seen, "sprite-readable structure");

        var subtypeDescriptor = ResolvePixelArtSubtypeDescriptor(configuration.PixelArtSubtype);
        if (!string.IsNullOrWhiteSpace(subtypeDescriptor))
        {
            AddPixelArtDescriptor(phrases, seen, subtypeDescriptor);
        }

        foreach (var phrase in ResolvePixelArtModifierDescriptors(configuration))
        {
            AddPixelArtDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    private static string ResolvePixelArtSubtypeDescriptor(string pixelArtSubtype)
    {
        return pixelArtSubtype switch
        {
            "retro-arcade" => "arcade gameplay clarity",
            "console-sprite" => "console-era sprite presentation",
            "isometric-pixel" => "isometric scene structure",
            "pixel-platformer" => "side-view traversal readability",
            "rpg-tileset" => "modular tileset clarity",
            "pixel-portrait" => "close-read sprite portraiture",
            "pixel-scene" => "story-supporting scene detail",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolvePixelArtModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetPixelArtModifierPriority(configuration.PixelArtSubtype);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Limited Palette" when configuration.PixelArtLimitedPalette => "limited-palette discipline",
                "Dithering" when configuration.PixelArtDithering => "intentional dithering",
                "Tileable Design" when configuration.PixelArtTileableDesign => "tileable layout logic",
                "Sprite Sheet Readability" when configuration.PixelArtSpriteSheetReadability => "sprite-sheet readiness",
                "Clean Outline" when configuration.PixelArtCleanOutline => "clean outline separation",
                "Subpixel Shading" when configuration.PixelArtSubpixelShading => "subpixel shading control",
                "HUD / UI Framing" when configuration.PixelArtHudUiFraming => "HUD-style framing",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetPixelArtModifierPriority(string pixelArtSubtype)
    {
        return pixelArtSubtype switch
        {
            "retro-arcade" => ["Limited Palette", "Clean Outline", "HUD / UI Framing", "Sprite Sheet Readability", "Dithering", "Tileable Design", "Subpixel Shading"],
            "console-sprite" => ["Clean Outline", "Sprite Sheet Readability", "Limited Palette", "Subpixel Shading", "HUD / UI Framing", "Dithering", "Tileable Design"],
            "isometric-pixel" => ["Tileable Design", "Limited Palette", "Subpixel Shading", "Clean Outline", "Sprite Sheet Readability", "HUD / UI Framing", "Dithering"],
            "pixel-platformer" => ["Clean Outline", "Tileable Design", "HUD / UI Framing", "Limited Palette", "Sprite Sheet Readability", "Dithering", "Subpixel Shading"],
            "rpg-tileset" => ["Tileable Design", "Limited Palette", "Sprite Sheet Readability", "Clean Outline", "Dithering", "Subpixel Shading", "HUD / UI Framing"],
            "pixel-portrait" => ["Clean Outline", "Limited Palette", "Subpixel Shading", "Sprite Sheet Readability", "Dithering", "Tileable Design", "HUD / UI Framing"],
            "pixel-scene" => ["Limited Palette", "Dithering", "HUD / UI Framing", "Clean Outline", "Tileable Design", "Sprite Sheet Readability", "Subpixel Shading"],
            _ => ["Limited Palette", "Clean Outline", "Sprite Sheet Readability", "Dithering", "Tileable Design", "Subpixel Shading", "HUD / UI Framing"],
        };
    }

    private static void AddPixelArtDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyPixelArtGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking value separation";
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "vivid chromatic punch";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity <= 40)
        {
            return "densely layered environment";
        }

        return phrase;
    }

    private static string ApplyPixelArtPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("pixel-art ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("pixel art ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("pixel-art", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("pixel art", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }
}
