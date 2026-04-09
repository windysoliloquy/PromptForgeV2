using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveLifestyleAdvertisingPhotographyPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetLifestyleAdvertisingPhotographyBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyLifestyleAdvertisingPhotographyGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveLifestyleAdvertisingPhotographyGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetLifestyleAdvertisingPhotographyBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveLifestyleAdvertisingPhotographyDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddLifestyleAdvertisingPhotographyDescriptor(phrases, seen, ResolveLifestyleAdvertisingPhotographyShotModeAnchor(configuration.LifestyleAdvertisingShotMode));

        var selectorDescriptor = ResolveLifestyleAdvertisingPhotographyShotModeDescriptor(configuration.LifestyleAdvertisingShotMode);
        if (!string.IsNullOrWhiteSpace(selectorDescriptor))
        {
            AddLifestyleAdvertisingPhotographyDescriptor(phrases, seen, selectorDescriptor);
        }

        foreach (var phrase in ResolveLifestyleAdvertisingPhotographyModifierDescriptors(configuration))
        {
            AddLifestyleAdvertisingPhotographyDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveLifestyleAdvertisingPhotographyLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "clean natural daylight",
            "Golden hour" => "warm aspirational glow",
            "Dramatic studio light" => "controlled campaign shaping",
            "Overcast" => "soft diffused realism",
            "Moonlit" => "cool evening calm",
            "Soft glow" => "intimate ambient glow",
            "Dusk haze" => "late-day atmospheric haze",
            "Warm directional light" => "warm directional shaping",
            "Volumetric cinematic light" => "layered ambient shafts",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveLifestyleAdvertisingPhotographyShotModeAnchor(string shotMode)
    {
        return shotMode switch
        {
            "premium-brand-campaign" => "premium brand campaign photography",
            "business-lifestyle" => "business lifestyle photography",
            "home-family-life" => "home life photography",
            "wellness-leisure" => "wellness lifestyle photography",
            _ => "everyday lifestyle photography",
        };
    }

    private static string ResolveLifestyleAdvertisingPhotographyShotModeDescriptor(string shotMode)
    {
        return shotMode switch
        {
            "premium-brand-campaign" => "polished aspiration and brand-led desirability",
            "business-lifestyle" => "capable teamwork and modern professional ease",
            "home-family-life" => "domestic warmth and relational familiarity",
            "wellness-leisure" => "restorative calm and self-care ease",
            _ => "believable routine and natural human ease",
        };
    }

    private static IEnumerable<string> ResolveLifestyleAdvertisingPhotographyModifierDescriptors(PromptConfiguration configuration)
    {
        var ordered = new (string Group, bool Enabled, string Phrase)[]
        {
            ("interaction-cues", configuration.LifestyleAdvertisingNaturalInteraction, "natural human interaction"),
            ("brand-cues", configuration.LifestyleAdvertisingProductInUse, "product-in-use cue"),
            ("brand-cues", configuration.LifestyleAdvertisingBrandColorAccent, "restrained brand-color accent"),
            ("environment-cues", configuration.LifestyleAdvertisingPropContext, "lifestyle prop context"),
            ("environment-cues", configuration.LifestyleAdvertisingSunlitOptimism, "sunlit optimism"),
            ("interaction-cues", configuration.LifestyleAdvertisingMotionCandidness, "candid motion trace"),
        };

        var groupCaps = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["interaction-cues"] = 1,
            ["brand-cues"] = 2,
            ["environment-cues"] = 2,
        };
        var groupUsage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var selected = new List<string>();

        foreach (var entry in GetLifestyleAdvertisingPhotographyModifierPriority(configuration.LifestyleAdvertisingShotMode)
                     .Join(ordered, key => key, item => item.Phrase, (_, item) => item))
        {
            if (!entry.Enabled || selected.Count >= 2)
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

    private static IReadOnlyList<string> GetLifestyleAdvertisingPhotographyModifierPriority(string shotMode)
    {
        return shotMode switch
        {
            "premium-brand-campaign" => ["product-in-use cue", "restrained brand-color accent", "sunlit optimism", "natural human interaction", "lifestyle prop context", "candid motion trace"],
            "business-lifestyle" => ["natural human interaction", "product-in-use cue", "lifestyle prop context", "candid motion trace", "restrained brand-color accent", "sunlit optimism"],
            "home-family-life" => ["natural human interaction", "lifestyle prop context", "candid motion trace", "sunlit optimism", "product-in-use cue", "restrained brand-color accent"],
            "wellness-leisure" => ["sunlit optimism", "natural human interaction", "lifestyle prop context", "candid motion trace", "product-in-use cue", "restrained brand-color accent"],
            _ => ["natural human interaction", "candid motion trace", "product-in-use cue", "lifestyle prop context", "sunlit optimism", "restrained brand-color accent"],
        };
    }

    private static void AddLifestyleAdvertisingPhotographyDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyLifestyleAdvertisingPhotographyGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Realism, StringComparison.OrdinalIgnoreCase) && value >= 61)
        {
            return "high-fidelity human realism";
        }

        if (string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "brand-facing polish";
        }

        if (string.Equals(configuration.LifestyleAdvertisingShotMode, "premium-brand-campaign", StringComparison.OrdinalIgnoreCase)
            && string.Equals(sliderKey, LightingIntensity, StringComparison.OrdinalIgnoreCase)
            && value >= 61)
        {
            return value >= 81 ? "radiant brand highlights" : "polished highlight brightness";
        }

        if (string.Equals(configuration.LifestyleAdvertisingShotMode, "business-lifestyle", StringComparison.OrdinalIgnoreCase)
            && string.Equals(sliderKey, NarrativeDensity, StringComparison.OrdinalIgnoreCase)
            && value >= 61)
        {
            return value >= 81 ? "richly implied professional story" : "layered workplace cues";
        }

        if (string.Equals(configuration.LifestyleAdvertisingShotMode, "home-family-life", StringComparison.OrdinalIgnoreCase)
            && string.Equals(sliderKey, Whimsy, StringComparison.OrdinalIgnoreCase)
            && value >= 61)
        {
            return value >= 81 ? "gentle celebratory playfulness" : "bright human play";
        }

        if (string.Equals(configuration.LifestyleAdvertisingShotMode, "wellness-leisure", StringComparison.OrdinalIgnoreCase)
            && string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase)
            && value >= 61)
        {
            return value >= 81 ? "deep tranquil recession" : "luminous wellness depth";
        }

        return phrase;
    }

    private static string[] GetLifestyleAdvertisingPhotographyBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var shotMode = configuration.LifestyleAdvertisingShotMode;
        return sliderKey switch
        {
            Stylization => shotMode switch
            {
                "premium-brand-campaign" => ["restrained premium treatment", "lightly styled aspirational framing", "polished brand styling", "editorial campaign direction", "fully art-directed premium campaign"],
                "business-lifestyle" => ["restrained professional treatment", "lightly styled workplace framing", "polished business styling", "editorial workday direction", "fully art-directed commercial workplace campaign"],
                "home-family-life" => ["restrained domestic treatment", "lightly styled home-life framing", "polished family-life styling", "editorial home-story direction", "fully art-directed domestic campaign"],
                "wellness-leisure" => ["restrained wellness treatment", "lightly styled leisure framing", "polished self-care styling", "editorial wellness direction", "fully art-directed leisure campaign"],
                _ => ["restrained lived-in treatment", "lightly styled daily-life framing", "polished everyday styling", "editorial lifestyle direction", "fully art-directed daily-life campaign"],
            },
            Realism => ["clear human read", "lightly interpreted real-world surfaces", "believable commercial realism", "high-fidelity human realism", "deeply convincing brand-friendly realism"],
            TextureDepth => shotMode switch
            {
                "premium-brand-campaign" => ["smooth premium surfaces", "light finish grain", "refined material definition", "rich fabric and product texture", "deeply worked premium texture"],
                "business-lifestyle" => ["smooth workplace surfaces", "light fabric grain", "desk-and-garment definition", "rich material and device texture", "deeply worked professional texture"],
                "home-family-life" => ["smooth domestic surfaces", "light textile grain", "home-material definition", "rich fabric and furnishing texture", "deeply worked domestic texture"],
                "wellness-leisure" => ["smooth wellness surfaces", "light towel-and-linen grain", "calm material definition", "rich skin and textile texture", "deeply worked restorative texture"],
                _ => ["smooth real-world surfaces", "light fabric grain", "natural surface definition", "rich textile and skin texture", "deeply worked tactile realism"],
            },
            NarrativeDensity => shotMode switch
            {
                "premium-brand-campaign" => ["isolated premium moment", "subtle aspirational implication", "brand-use cues", "layered campaign cues", "richly implied premium story"],
                "business-lifestyle" => ["isolated workday moment", "subtle task implication", "collaboration cues", "layered workplace cues", "richly implied professional story"],
                "home-family-life" => ["isolated home-life moment", "subtle relational implication", "family-use cues", "layered domestic cues", "richly implied home story"],
                "wellness-leisure" => ["isolated restorative moment", "subtle self-care implication", "leisure-use cues", "layered wellness cues", "richly implied restorative story"],
                _ => ["isolated daily-life moment", "subtle routine implication", "everyday-use cues", "layered lived-in cues", "richly implied daily-life story"],
            },
            Symbolism => ["minimal symbolic load", "restrained styling cue", "suggestive lifestyle motif", "pronounced editorial symbolism", "commercial allegory"],
            SurfaceAge => shotMode switch
            {
                "premium-brand-campaign" => ["freshly finished surfaces", "faint use trace", "gentle handled wear", "settled premium character", "softly time-touched luxury wear"],
                "business-lifestyle" => ["freshly finished surfaces", "faint desk-use trace", "gentle workplace wear", "settled professional character", "softly time-touched office wear"],
                "home-family-life" => ["freshly finished surfaces", "faint home-use trace", "gentle domestic wear", "settled family character", "softly time-touched household wear"],
                "wellness-leisure" => ["freshly finished surfaces", "faint handling trace", "gentle spa-like wear", "settled restorative character", "softly time-touched leisure wear"],
                _ => ["freshly finished surfaces", "faint handling trace", "gentle lived-in wear", "settled everyday character", "softly time-touched domestic wear"],
            },
            Framing => shotMode switch
            {
                "premium-brand-campaign" => ["intimate premium crop", "close aspirational framing", "polished campaign framing", "broader premium view", "expansive brand staging"],
                "business-lifestyle" => ["intimate work crop", "close workplace framing", "balanced professional framing", "broader collaboration view", "expansive commercial staging"],
                "home-family-life" => ["intimate family crop", "close domestic framing", "balanced home-life framing", "broader relational view", "expansive household staging"],
                "wellness-leisure" => ["intimate wellness crop", "close restorative framing", "balanced leisure framing", "broader calm-setting view", "expansive retreat staging"],
                _ => ["intimate human crop", "close everyday framing", "balanced lifestyle framing", "broader situational view", "expansive lived-in staging"],
            },
            BackgroundComplexity => shotMode switch
            {
                "premium-brand-campaign" => ["minimal premium surround", "restrained aspirational surround", "supporting brand environment", "richer campaign surroundings", "densely layered premium environment"],
                "business-lifestyle" => ["minimal workplace surround", "restrained professional surround", "supporting work environment", "richer office surroundings", "densely layered commercial environment"],
                "home-family-life" => ["minimal domestic surround", "restrained home surround", "supporting household environment", "richer family surroundings", "densely layered home environment"],
                "wellness-leisure" => ["minimal restorative surround", "restrained calm surround", "supporting wellness environment", "richer leisure surroundings", "densely layered retreat environment"],
                _ => ["minimal situational surround", "restrained lived-in surround", "supporting everyday environment", "richer daily-life surroundings", "densely layered lifestyle environment"],
            },
            MotionEnergy => shotMode switch
            {
                "premium-brand-campaign" => ["still premium moment", "slight polished trace", "active campaign motion", "brand-led momentum", "split-second aspirational energy"],
                "business-lifestyle" => ["still workday moment", "slight task trace", "active workplace motion", "collaborative momentum", "split-second professional energy"],
                "home-family-life" => ["still home-life moment", "slight relational trace", "active family motion", "domestic momentum", "split-second household energy"],
                "wellness-leisure" => ["still restorative moment", "slight body trace", "active leisure motion", "self-care momentum", "split-second wellness energy"],
                _ => ["still daily-life moment", "slight candid trace", "active everyday motion", "lived-in momentum", "split-second candid energy"],
            },
            FocusDepth => shotMode switch
            {
                "premium-brand-campaign" => ["broad premium clarity", "light focus falloff", "measured campaign emphasis", "selective aspirational isolation", "razor-held premium separation"],
                "business-lifestyle" => ["broad workplace clarity", "light focus falloff", "measured professional emphasis", "selective workday isolation", "razor-held commercial separation"],
                "home-family-life" => ["broad domestic clarity", "light focus falloff", "measured relational emphasis", "selective family isolation", "razor-held household separation"],
                "wellness-leisure" => ["broad restorative clarity", "light focus falloff", "measured calm emphasis", "selective wellness isolation", "razor-held tranquil separation"],
                _ => ["broad human clarity", "light focus falloff", "measured subject emphasis", "selective lifestyle isolation", "razor-held human separation"],
            },
            ImageCleanliness => ["raw lived-in trace", "slight surface grit", "commercial polish", "campaign-ready polish", "brand-facing polish"],
            DetailDensity => shotMode switch
            {
                "premium-brand-campaign" => ["sparse visible detail", "light premium detail", "clear campaign detail load", "rich brand detail load", "dense aspirational detail load"],
                "business-lifestyle" => ["sparse visible detail", "light workplace detail", "clear professional detail load", "rich workday detail load", "dense commercial detail load"],
                "home-family-life" => ["sparse visible detail", "light domestic detail", "clear household detail load", "rich family-life detail load", "dense home detail load"],
                "wellness-leisure" => ["sparse visible detail", "light restorative detail", "clear wellness detail load", "rich self-care detail load", "dense leisure detail load"],
                _ => ["sparse visible detail", "light lived-in detail", "clear daily-life detail load", "rich lifestyle detail load", "dense candid detail load"],
            },
            AtmosphericDepth => shotMode switch
            {
                "premium-brand-campaign" => ["flat premium space", "slight ambient recession", "gentle campaign separation", "luminous aspirational depth", "deep polished recession"],
                "business-lifestyle" => ["flat workplace space", "slight environmental recession", "gentle office separation", "luminous professional depth", "deep commercial recession"],
                "home-family-life" => ["flat domestic space", "slight environmental recession", "gentle household separation", "luminous home depth", "deep relational recession"],
                "wellness-leisure" => ["flat restorative space", "slight ambient recession", "gentle calm separation", "luminous wellness depth", "deep tranquil recession"],
                _ => ["flat lived-in space", "slight environmental recession", "gentle room separation", "luminous lifestyle depth", "deep situational recession"],
            },
            Chaos => ["situational control", "quiet restlessness", "light candid disorder", "loose real-world clutter", "orchestrated lifestyle disorder"],
            Whimsy => ["serious commercial tone", "subtle warmth", "casual social lift", "bright human play", "gentle celebratory playfulness"],
            Tension => ["calm human tone", "faint situational urgency", "light interpersonal pressure", "noticeable commercial pressure", "intense campaign pressure"],
            Awe => shotMode switch
            {
                "premium-brand-campaign" => ["human-scale grounding", "slight prestige lift", "quiet premium allure", "strong aspirational allure", "expansive desirability"],
                "business-lifestyle" => ["human-scale grounding", "slight capability lift", "quiet professional allure", "strong competent presence", "expansive commercial confidence"],
                "home-family-life" => ["human-scale grounding", "slight warmth lift", "quiet domestic allure", "strong relational presence", "expansive household warmth"],
                "wellness-leisure" => ["human-scale grounding", "slight calm lift", "quiet restorative allure", "strong serene presence", "expansive wellness allure"],
                _ => ["human-scale grounding", "slight presence lift", "quiet everyday allure", "strong relatable allure", "expansive human warmth"],
            },
            Temperature => shotMode switch
            {
                "premium-brand-campaign" => ["cool premium balance", "lightly cool neutrality", "neutral polished balance", "warm aspirational balance", "heated luxury warmth"],
                "business-lifestyle" => ["cool professional balance", "lightly cool neutrality", "neutral office balance", "warm collaborative balance", "heated workday warmth"],
                "home-family-life" => ["cool domestic balance", "lightly cool neutrality", "neutral home balance", "warm familial balance", "heated household warmth"],
                "wellness-leisure" => ["cool restorative balance", "lightly cool neutrality", "neutral calm balance", "warm self-care balance", "heated retreat warmth"],
                _ => ["cool natural balance", "lightly cool neutrality", "neutral daylight balance", "warm familiar balance", "heated sunlit warmth"],
            },
            LightingIntensity => shotMode switch
            {
                "premium-brand-campaign" => ["dim premium light", "soft aspirational light", "balanced campaign brightness", "polished highlight brightness", "radiant brand highlights"],
                "business-lifestyle" => ["dim workday light", "soft professional light", "balanced office brightness", "task-shaped brightness", "radiant commercial highlights"],
                "home-family-life" => ["dim domestic light", "soft household light", "balanced home brightness", "warm-room brightness", "radiant family highlights"],
                "wellness-leisure" => ["dim restorative light", "soft calming light", "balanced wellness brightness", "glow-shaped brightness", "radiant tranquil highlights"],
                _ => ["dim natural light", "soft natural light", "balanced daylight brightness", "sun-shaped brightness", "radiant lifestyle highlights"],
            },
            Saturation => ["muted real-world color", "restrained commercial color", "balanced natural color", "rich lifestyle color", "vivid campaign color"],
            Contrast => shotMode switch
            {
                "premium-brand-campaign" => ["low premium contrast", "gentle contour separation", "clear polished definition", "crisp aspirational separation", "striking brand contrast"],
                "business-lifestyle" => ["low workplace contrast", "gentle contour separation", "clear professional definition", "crisp office separation", "striking commercial contrast"],
                "home-family-life" => ["low domestic contrast", "gentle contour separation", "clear relational definition", "crisp household separation", "striking home-life contrast"],
                "wellness-leisure" => ["low restorative contrast", "gentle contour separation", "clear calm definition", "crisp wellness separation", "striking serene contrast"],
                _ => ["low edge contrast", "gentle contour separation", "clear human definition", "crisp situational separation", "striking candid contrast"],
            },
            CameraDistance => shotMode switch
            {
                "premium-brand-campaign" => ["close premium read", "intimate aspirational read", "mid-distance campaign read", "broader brand overview", "far-set premium overview"],
                "business-lifestyle" => ["close workday read", "intimate workplace read", "mid-distance professional read", "broader collaboration overview", "far-set office overview"],
                "home-family-life" => ["close family read", "intimate domestic read", "mid-distance household read", "broader home-life overview", "far-set family overview"],
                "wellness-leisure" => ["close restorative read", "intimate calm read", "mid-distance wellness read", "broader leisure overview", "far-set retreat overview"],
                _ => ["close human read", "intimate situational read", "mid-distance lifestyle read", "broader daily-life overview", "far-set lived-in overview"],
            },
            CameraAngle => shotMode switch
            {
                "premium-brand-campaign" => ["eye-level premium view", "slightly lowered aspirational view", "level campaign view", "slightly elevated brand vantage", "high polished vantage"],
                "business-lifestyle" => ["eye-level workplace view", "slightly lowered professional view", "level workday view", "slightly elevated office vantage", "high collaborative vantage"],
                "home-family-life" => ["eye-level domestic view", "slightly lowered familial view", "level household view", "slightly elevated home vantage", "high relational vantage"],
                "wellness-leisure" => ["eye-level calm view", "slightly lowered restorative view", "level wellness view", "slightly elevated leisure vantage", "high tranquil vantage"],
                _ => ["eye-level human view", "slightly lowered candid view", "level situational view", "slightly elevated lifestyle vantage", "high observational vantage"],
            },
            _ => Array.Empty<string>(),
        };
    }
}
