using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveArchitectureArchvizPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetArchitectureArchvizBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyArchitectureArchvizGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveArchitectureArchvizGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetArchitectureArchvizBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveArchitectureArchvizDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddArchitectureArchvizDescriptor(phrases, seen, ResolveArchitectureArchvizCommercialAnchor(configuration.ArchitectureArchvizViewMode));

        var selectorDescriptor = ResolveArchitectureArchvizViewModeDescriptor(configuration.ArchitectureArchvizViewMode);
        if (!string.IsNullOrWhiteSpace(selectorDescriptor))
        {
            AddArchitectureArchvizDescriptor(phrases, seen, selectorDescriptor);
        }

        foreach (var phrase in ResolveArchitectureArchvizModifierDescriptors(configuration))
        {
            AddArchitectureArchvizDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveArchitectureArchvizLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "clean daylight rendering",
            "Golden hour" => "warm facade light",
            "Dramatic studio light" => "controlled showcase illumination",
            "Overcast" => "neutral overcast daylight",
            "Moonlit" => "cool evening illumination",
            "Soft glow" => "soft lobby glow",
            "Dusk haze" => "twilight ambient haze",
            "Warm directional light" => "warm directional glazing glow",
            "Volumetric cinematic light" => "layered daylight shafts",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveArchitectureArchvizCommercialAnchor(string viewMode)
    {
        return viewMode switch
        {
            "interior" => "commercial-grade architectural interior visualization",
            "streetscape" => "commercial-grade architectural streetscape visualization",
            "aerial-masterplan" => "commercial-grade aerial masterplan visualization",
            "twilight-marketing" => "commercial-grade twilight architectural marketing render",
            _ => "commercial-grade architectural exterior visualization",
        };
    }

    private static string ResolveArchitectureArchvizViewModeDescriptor(string viewMode)
    {
        return viewMode switch
        {
            "interior" => "room proportion and circulation clarity",
            "streetscape" => "frontage rhythm and sidewalk edge clarity",
            "aerial-masterplan" => "massing hierarchy and district legibility",
            "twilight-marketing" => "warm window glow and premium dusk contrast",
            _ => "facade articulation and skyline recession",
        };
    }

    private static IEnumerable<string> ResolveArchitectureArchvizModifierDescriptors(PromptConfiguration configuration)
    {
        var ordered = new (string Group, bool Enabled, string Phrase)[]
        {
            ("occupancy-context", configuration.ArchitectureArchvizHumanScaleCues, "human scale cues"),
            ("site-accents", configuration.ArchitectureArchvizLandscapeEmphasis, "landscape-led ground plane"),
            ("interior-accents", configuration.ArchitectureArchvizFurnishingEmphasis, "furnishing-led room styling"),
            ("lighting-accents", configuration.ArchitectureArchvizWarmInteriorGlow, "warm interior glow"),
            ("site-accents", configuration.ArchitectureArchvizReflectiveSurfaceAccents, "reflective surface accents"),
            ("marketing-accents", configuration.ArchitectureArchvizAmenityFocus, "amenity-led emphasis"),
        };

        var groupCaps = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["occupancy-context"] = 1,
            ["site-accents"] = 2,
            ["interior-accents"] = 1,
            ["lighting-accents"] = 1,
            ["marketing-accents"] = 1,
        };
        var groupUsage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var selected = new List<string>();

        foreach (var entry in GetArchitectureArchvizModifierPriority(configuration.ArchitectureArchvizViewMode)
                     .Join(ordered, key => key, item => item.Phrase, (_, item) => item))
        {
            if (selected.Count >= 2 || !entry.Enabled)
            {
                continue;
            }

            groupUsage.TryGetValue(entry.Group, out var used);
            if (used >= groupCaps[entry.Group])
            {
                continue;
            }

            groupUsage[entry.Group] = used + 1;
            selected.Add(entry.Phrase);
        }

        return selected;
    }

    private static IReadOnlyList<string> GetArchitectureArchvizModifierPriority(string viewMode)
    {
        return viewMode switch
        {
            "interior" => ["furnishing-led room styling", "warm interior glow", "human scale cues", "amenity-led emphasis", "reflective surface accents", "landscape-led ground plane"],
            "streetscape" => ["human scale cues", "landscape-led ground plane", "amenity-led emphasis", "reflective surface accents", "warm interior glow", "furnishing-led room styling"],
            "aerial-masterplan" => ["amenity-led emphasis", "landscape-led ground plane", "human scale cues", "reflective surface accents", "warm interior glow", "furnishing-led room styling"],
            "twilight-marketing" => ["warm interior glow", "reflective surface accents", "amenity-led emphasis", "human scale cues", "landscape-led ground plane", "furnishing-led room styling"],
            _ => ["landscape-led ground plane", "human scale cues", "reflective surface accents", "amenity-led emphasis", "warm interior glow", "furnishing-led room styling"],
        };
    }

    private static void AddArchitectureArchvizDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyArchitectureArchvizGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(configuration.ArchitectureArchvizViewMode, "twilight-marketing", StringComparison.OrdinalIgnoreCase)
            && string.Equals(sliderKey, LightingIntensity, StringComparison.OrdinalIgnoreCase)
            && value >= 61)
        {
            return "evening glow strength";
        }

        if (string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "clean sales-render finish";
        }

        if (string.Equals(sliderKey, Realism, StringComparison.OrdinalIgnoreCase) && value >= 61)
        {
            return "material realism";
        }

        return phrase;
    }

    private static string[] GetArchitectureArchvizBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var viewMode = configuration.ArchitectureArchvizViewMode;
        return sliderKey switch
        {
            Stylization => viewMode switch
            {
                "interior" => ["restrained design treatment", "lightly styled room rendering", "measured design authorship", "polished development styling", "fully art-directed sales rendering"],
                "streetscape" => ["restrained frontage treatment", "lightly styled urban presentation", "measured marketing authorship", "polished district styling", "fully art-directed development rendering"],
                "aerial-masterplan" => ["restrained planning treatment", "lightly styled masterplan rendering", "measured masterplan authorship", "polished district styling", "fully art-directed planning render"],
                "twilight-marketing" => ["restrained dusk treatment", "lightly styled evening presentation", "measured marketing authorship", "polished twilight styling", "fully art-directed premium marketing render"],
                _ => ["restrained facade treatment", "lightly styled exterior rendering", "measured development authorship", "polished marketing styling", "fully art-directed exterior rendering"],
            },
            Realism => ["clean built-form read", "lightly interpreted built presence", "material realism", "high-fidelity built-space realism", "deeply convincing development realism"],
            TextureDepth => viewMode switch
            {
                "interior" => ["smooth finish read", "faint finish texture", "finish continuity", "rich surface articulation", "deep junction clarity"],
                _ => ["smooth facade read", "faint material texture", "surface articulation", "rich finish texture", "deep junction clarity"],
            },
            NarrativeDensity => viewMode switch
            {
                "interior" => ["low-story occupancy restraint", "light use implication", "light occupancy cues", "layered lived-in context", "dense hospitality storytelling"],
                "streetscape" => ["low-story urban restraint", "light pedestrian implication", "light public-use cues", "layered frontage context", "dense district storytelling"],
                "aerial-masterplan" => ["low-story planning restraint", "light district implication", "light program cues", "layered planning context", "dense development storytelling"],
                "twilight-marketing" => ["low-story marketing restraint", "light arrival implication", "light hospitality cues", "layered evening context", "dense premium storytelling"],
                _ => ["low-story restraint", "light occupancy implication", "light use cues", "layered sales context", "dense development storytelling"],
            },
            Symbolism => ["minimal symbolic load", "restrained brand cue", "suggestive spatial motif", "pronounced ceremonial symbolism", "premium sales allegory"],
            SurfaceAge => viewMode switch
            {
                "interior" => ["fresh fit-out finish", "faint lived-in wear", "gentle finish aging", "noticeable patina", "time-softened interior character"],
                _ => ["fresh facade finish", "faint weathering", "gentle material aging", "noticeable patina", "time-softened built character"],
            },
            Framing => viewMode switch
            {
                "interior" => ["room-focused crop", "clean room composition", "circulation clarity", "broader room staging", "expansive showroom composition"],
                "streetscape" => ["tight frontage crop", "clean street elevation", "frontage rhythm", "broader pedestrian frontage", "expansive boulevard staging"],
                "aerial-masterplan" => ["tight site crop", "clean site overview", "massing hierarchy", "broader district framing", "expansive planning tableau"],
                "twilight-marketing" => ["premium dusk crop", "clean arrival framing", "premium spatial legibility", "broader premium staging", "expansive marketing tableau"],
                _ => ["facade-focused crop", "clean elevation framing", "spatial legibility", "broader approach framing", "expansive exterior staging"],
            },
            BackgroundComplexity => viewMode switch
            {
                "interior" => ["minimal furnishing load", "restrained furnishing support", "finish continuity", "richly staged support", "densely layered room context"],
                "streetscape" => ["minimal street load", "restrained sidewalk context", "pedestrian context", "rich district context", "densely layered street context"],
                "aerial-masterplan" => ["minimal site load", "restrained district context", "supporting territorial surroundings", "rich planning context", "densely layered district context"],
                "twilight-marketing" => ["minimal arrival load", "restrained forecourt surround", "site context", "rich hospitality context", "densely layered premium context"],
                _ => ["minimal site load", "restrained planted approach", "supporting surroundings", "richly situated context", "densely layered skyline context"],
            },
            MotionEnergy => ["still presentation control", "faint occupancy trace", "light movement cue", "loose pedestrian activity", "busy circulation flow"],
            FocusDepth => viewMode switch
            {
                "interior" => ["broad room clarity", "light focus falloff", "finish continuity", "selective material focus", "razor-held room clarity"],
                "aerial-masterplan" => ["broad site clarity", "light focus falloff", "district legibility", "selective site emphasis", "razor-held masterplan clarity"],
                _ => ["broad facade clarity", "light focus falloff", "spatial legibility", "selective material emphasis", "razor-held frontage clarity"],
            },
            ImageCleanliness => ["raw ground texture", "slight presentation grit", "development polish", "controlled marketing polish", "clean sales-render finish"],
            DetailDensity => viewMode switch
            {
                "interior" => ["sparse room detail", "light finish detail", "clear interior detail load", "rich finish detail load", "dense interior detail load"],
                "aerial-masterplan" => ["sparse site detail", "light planning detail", "clear district detail load", "rich site detail load", "dense masterplan detail load"],
                _ => ["sparse facade detail", "light material detail", "clear facade detail load", "rich material detail load", "dense facade detail load"],
            },
            AtmosphericDepth => viewMode switch
            {
                "interior" => ["flat room space", "slight tonal recession", "daylighted volume", "luminous room recession", "deep interior recession"],
                "streetscape" => ["flat street space", "slight tonal recession", "pedestrian-depth layering", "luminous block recession", "deep skyline separation"],
                "aerial-masterplan" => ["flat site field", "slight atmospheric lift", "district recession", "luminous site hierarchy", "deep aerial separation"],
                "twilight-marketing" => ["flat dusk space", "slight ambient recession", "premium ambient contrast", "luminous evening recession", "deep twilight atmosphere"],
                _ => ["flat exterior space", "slight atmospheric recession", "skyline recession", "daylighted volume", "deep skyline separation"],
            },
            Chaos => ["low-chaos presentation control", "quiet asymmetry", "light layout instability", "loose staging disorder", "orchestrated spatial turbulence"],
            Whimsy => ["serious presentation tone", "subtle warmth", "light design play", "warm hospitality looseness", "gentle showcase playfulness"],
            Tension => ["quiet witness tone", "faint market tension", "light presentation pressure", "strong premium pressure", "intense marketing tension"],
            Awe => viewMode switch
            {
                "aerial-masterplan" => ["site-scale grounding", "slight district lift", "quiet planning awe", "strong territorial scale", "expansive masterplan grandeur"],
                "twilight-marketing" => ["human-scale grounding", "slight presence lift", "quiet market grandeur", "strong premium scale", "expansive development grandeur"],
                _ => ["human-scale grounding", "slight presence lift", "quiet spatial wonder", "strong felt scale", "expansive development grandeur"],
            },
            Temperature => viewMode switch
            {
                "twilight-marketing" => ["cool dusk balance", "slightly cool evening neutrality", "balanced evening toning", "warm interior bias", "deep hospitality warmth"],
                _ => ["cool daylight balance", "slightly cool neutrality", "neutral daylight balance", "warm natural light", "heated warm cast"],
            },
            LightingIntensity => viewMode switch
            {
                "twilight-marketing" => ["dim evening light", "soft dusk light", "balanced evening glow", "strong evening glow strength", "radiant hospitality glow"],
                _ => ["dim daylight", "soft daylight", "daylight balance", "controlled studio brightness", "radiant daylight volume"],
            },
            Saturation => ["muted material color", "restrained commercial color", "balanced material color", "rich finish color", "vivid amenity color"],
            Contrast => ["low edge contrast", "gentle contour separation", "contour separation", "crisp glazing definition", "striking edge clarity"],
            CameraDistance => viewMode switch
            {
                "interior" => ["close room read", "intimate room proportion", "room proportion", "broad room overview", "far-set room overview"],
                "streetscape" => ["close frontage read", "pedestrian eye-line read", "block-length read", "broad streetscape overview", "far-set district overview"],
                "aerial-masterplan" => ["close site read", "elevated site overview", "aerial overview distance", "broad district overview", "far-set territorial overview"],
                _ => ["close facade read", "approach-distance read", "massing read", "broad exterior overview", "far-set site overview"],
            },
            CameraAngle => viewMode switch
            {
                "interior" => ["eye-level room view", "slightly lowered human viewpoint", "level room view", "slightly elevated interior vantage", "high overview vantage"],
                "streetscape" => ["pedestrian eye line", "slightly lowered street viewpoint", "level frontage", "slightly elevated block vantage", "high streetscape vantage"],
                "aerial-masterplan" => ["shallow oblique overview", "elevated site view", "elevated plan-view bias", "high site overview", "high detached plan view"],
                _ => ["level frontage", "slightly lowered approach view", "eye-level facade view", "slightly elevated site vantage", "high overview vantage"],
            },
            _ => Array.Empty<string>(),
        };
    }
}
