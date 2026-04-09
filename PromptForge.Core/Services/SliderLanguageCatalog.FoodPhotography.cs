using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveFoodPhotographyPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetFoodPhotographyBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyFoodPhotographyGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveFoodPhotographyGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetFoodPhotographyBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveFoodPhotographyDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddFoodPhotographyDescriptor(phrases, seen, ResolveFoodPhotographyCommercialAnchor(configuration.FoodPhotographyShotMode));

        var selectorDescriptor = ResolveFoodPhotographyShotModeDescriptor(configuration.FoodPhotographyShotMode);
        if (!string.IsNullOrWhiteSpace(selectorDescriptor))
        {
            AddFoodPhotographyDescriptor(phrases, seen, selectorDescriptor);
        }

        foreach (var phrase in ResolveFoodPhotographyModifierDescriptors(configuration))
        {
            AddFoodPhotographyDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveFoodPhotographyLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "clean window light",
            "Golden hour" => "amber dining glow",
            "Dramatic studio light" => "sculpted studio shaping",
            "Overcast" => "soft diffuse illumination",
            "Moonlit" => "cool evening hush",
            "Soft glow" => "intimate ambient glow",
            "Dusk haze" => "late-service haze",
            "Warm directional light" => "raking warm illumination",
            "Volumetric cinematic light" => "layered room glow",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveFoodPhotographyCommercialAnchor(string shotMode)
    {
        return shotMode switch
        {
            "tabletop-spread" => "shared-table food photography",
            "macro-detail" => "close food study photography",
            "beverage-service" => "beverage photography",
            "hospitality-campaign" => "hospitality dining photography",
            _ => "plated food photography",
        };
    }

    private static string ResolveFoodPhotographyShotModeDescriptor(string shotMode)
    {
        return shotMode switch
        {
            "tabletop-spread" => "multi-item abundance and shared-setting readability",
            "macro-detail" => "ingredient intimacy and material appetite emphasis",
            "beverage-service" => "drink presentation clarity and refreshment cues",
            "hospitality-campaign" => "dining atmosphere and guest-facing polish",
            _ => "single-dish priority and appetite-led focal order",
        };
    }

    private static IEnumerable<string> ResolveFoodPhotographyModifierDescriptors(PromptConfiguration configuration)
    {
        var ordered = new (string Group, bool Enabled, string Phrase)[]
        {
            ("freshness-accents", configuration.FoodPhotographyVisibleSteam, "rising heat trace"),
            ("plating-accents", configuration.FoodPhotographyGarnishEmphasis, "finishing garnish priority"),
            ("service-context", configuration.FoodPhotographyUtensilContext, "cutlery or servingware context"),
            ("service-context", configuration.FoodPhotographyHandServiceCue, "active hand-led serving cue"),
            ("plating-accents", configuration.FoodPhotographyIngredientScatter, "styled ingredient scatter"),
            ("freshness-accents", configuration.FoodPhotographyCondensationEmphasis, "chilled condensation presence"),
        };

        var groupCaps = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["freshness-accents"] = 1,
            ["plating-accents"] = 2,
            ["service-context"] = 1,
        };
        var groupUsage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var selected = new List<string>();

        foreach (var entry in GetFoodPhotographyModifierPriority(configuration.FoodPhotographyShotMode)
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

    private static IReadOnlyList<string> GetFoodPhotographyModifierPriority(string shotMode)
    {
        return shotMode switch
        {
            "tabletop-spread" => ["styled ingredient scatter", "cutlery or servingware context", "finishing garnish priority", "active hand-led serving cue", "rising heat trace", "chilled condensation presence"],
            "macro-detail" => ["finishing garnish priority", "styled ingredient scatter", "rising heat trace", "chilled condensation presence", "cutlery or servingware context", "active hand-led serving cue"],
            "beverage-service" => ["chilled condensation presence", "active hand-led serving cue", "cutlery or servingware context", "rising heat trace", "finishing garnish priority", "styled ingredient scatter"],
            "hospitality-campaign" => ["active hand-led serving cue", "cutlery or servingware context", "finishing garnish priority", "rising heat trace", "styled ingredient scatter", "chilled condensation presence"],
            _ => ["finishing garnish priority", "rising heat trace", "cutlery or servingware context", "styled ingredient scatter", "active hand-led serving cue", "chilled condensation presence"],
        };
    }

    private static void AddFoodPhotographyDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyFoodPhotographyGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Realism, StringComparison.OrdinalIgnoreCase) && value >= 61)
        {
            return "high-fidelity appetizing realism";
        }

        if (string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "guest-facing polish";
        }

        if (string.Equals(sliderKey, DetailDensity, StringComparison.OrdinalIgnoreCase)
            && value >= 61
            && string.Equals(configuration.FoodPhotographyShotMode, "macro-detail", StringComparison.OrdinalIgnoreCase))
        {
            return value >= 81 ? "dense sensory detail load" : "rich material detail load";
        }

        if (string.Equals(configuration.FoodPhotographyShotMode, "beverage-service", StringComparison.OrdinalIgnoreCase)
            && string.Equals(sliderKey, LightingIntensity, StringComparison.OrdinalIgnoreCase)
            && value >= 61)
        {
            return value >= 81 ? "radiant chilled highlights" : "rim-shaped brightness";
        }

        if ((string.IsNullOrWhiteSpace(configuration.FoodPhotographyShotMode) || string.Equals(configuration.FoodPhotographyShotMode, "plated-hero", StringComparison.OrdinalIgnoreCase))
            && string.Equals(sliderKey, FocusDepth, StringComparison.OrdinalIgnoreCase)
            && value >= 61)
        {
            return value >= 81 ? "razor-held hero isolation" : "selective plate isolation";
        }

        return phrase;
    }

    private static string[] GetFoodPhotographyBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var shotMode = configuration.FoodPhotographyShotMode;
        return sliderKey switch
        {
            Stylization => shotMode switch
            {
                "tabletop-spread" => ["restrained table presentation", "lightly styled sharing setup", "gathering-aware table styling", "editorial spread direction", "fully art-directed banquet staging"],
                "macro-detail" => ["restrained close study", "lightly styled ingredient focus", "texture-led close styling", "editorial material direction", "fully art-directed sensory detail"],
                "beverage-service" => ["restrained drink presentation", "lightly styled pour setup", "bar-aware drink styling", "editorial beverage direction", "fully art-directed signature serve"],
                "hospitality-campaign" => ["restrained dining presentation", "lightly styled room service", "guest-aware editorial styling", "polished hospitality direction", "fully art-directed dining campaign"],
                _ => ["restrained dish presentation", "lightly styled plating", "menu-minded plate styling", "editorial hero direction", "fully art-directed signature plating"],
            },
            Realism => ["clear edible read", "lightly interpreted edible surfaces", "believable culinary surfaces", "high-fidelity appetizing realism", "deeply convincing edible realism"],
            TextureDepth => shotMode switch
            {
                "tabletop-spread" => ["clean tabletop surfaces", "light bread-and-linen grain", "serving-surface definition", "rich platter-and-sauce texture", "deeply layered table texture"],
                "macro-detail" => ["smooth ingredient surfaces", "light fiber presence", "pore-and-crumb definition", "rich glaze and crystal articulation", "deeply worked close material texture"],
                "beverage-service" => ["smooth glass surfaces", "light chill trace", "froth and rim definition", "rich condensation and specular texture", "deeply worked chilled-surface texture"],
                "hospitality-campaign" => ["polished dining surfaces", "light fabric and ceramic grain", "tabletop material definition", "rich service-material articulation", "deeply layered room-facing texture"],
                _ => ["smooth plated surfaces", "light crust presence", "clear sauce-and-crumb definition", "rich sear and glaze articulation", "deeply worked plated texture"],
            },
            NarrativeDensity => shotMode switch
            {
                "tabletop-spread" => ["single-table read", "subtle gathering implication", "shared-course presentation", "layered hosting cues", "richly implied communal meal"],
                "macro-detail" => ["isolated ingredient read", "subtle preparation implication", "close culinary presentation", "layered making cues", "richly implied ingredient story"],
                "beverage-service" => ["isolated drink read", "subtle pour implication", "served refreshment presentation", "layered bar-service cues", "richly implied hospitality ritual"],
                "hospitality-campaign" => ["isolated dining read", "subtle guest implication", "service-minded presentation", "layered venue cues", "richly implied restaurant story"],
                _ => ["single-dish isolation", "subtle plating intention", "chef-finished presentation", "layered course cues", "richly implied meal progression"],
            },
            Symbolism => ["minimal symbolic load", "restrained styling cue", "suggestive hospitality motif", "pronounced editorial symbolism", "dining allegory"],
            SurfaceAge => shotMode switch
            {
                "tabletop-spread" => ["freshly set table surfaces", "faint hosting trace", "light shared-use handling", "settled gathering character", "softly time-touched table presence"],
                "macro-detail" => ["fresh ingredient surfaces", "faint prep trace", "light handling wear", "mature cooked character", "softly time-touched kitchen presence"],
                "beverage-service" => ["freshly poured surfaces", "faint glass handling trace", "light bar-use handling", "settled service character", "softly time-touched barware presence"],
                "hospitality-campaign" => ["freshly set dining surfaces", "faint room-use trace", "light service handling", "settled venue character", "softly time-touched hospitality presence"],
                _ => ["freshly finished plate surfaces", "faint serving trace", "light pass-of-service handling", "settled course character", "softly time-touched plating"],
            },
            Framing => shotMode switch
            {
                "tabletop-spread" => ["tight place-setting crop", "close shared-table framing", "balanced spread framing", "broader hosting view", "expansive feast staging"],
                "macro-detail" => ["extreme appetite crop", "close ingredient framing", "texture-first framing", "broader material view", "expansive detail field"],
                "beverage-service" => ["tight glass crop", "close drink framing", "serve-led framing", "broader pour setting", "expansive beverage staging"],
                "hospitality-campaign" => ["tight dining crop", "close service framing", "guest-aware framing", "broader venue view", "expansive hospitality staging"],
                _ => ["tight plate crop", "clean hero crop", "centered entrée framing", "broader plate setting", "expansive course staging"],
            },
            BackgroundComplexity => shotMode switch
            {
                "tabletop-spread" => ["minimal table surround", "restrained gathering field", "supporting hosting context", "richer shared-setting support", "densely layered feast surround"],
                "macro-detail" => ["minimal close surround", "restrained backdrop field", "supporting ingredient context", "richer material backdrop", "densely layered detail surround"],
                "beverage-service" => ["minimal drink surround", "restrained bar field", "supporting service context", "richer pour backdrop", "densely layered lounge surround"],
                "hospitality-campaign" => ["minimal dining surround", "restrained venue field", "supporting room context", "richer hospitality backdrop", "densely layered restaurant surround"],
                _ => ["minimal plate surround", "restrained tabletop field", "supporting course context", "richer setting support", "densely layered dining surround"],
            },
            MotionEnergy => shotMode switch
            {
                "tabletop-spread" => ["still gathering moment", "slight reach trace", "active serving energy", "passing-and-sharing momentum", "split-second table motion"],
                "macro-detail" => ["still ingredient moment", "slight prep trace", "active finishing energy", "cutting-and-drizzling momentum", "split-second sensory motion"],
                "beverage-service" => ["still served moment", "slight pour trace", "active service energy", "lift-and-pour momentum", "split-second drink motion"],
                "hospitality-campaign" => ["still dining moment", "slight guest trace", "active service energy", "room-flow momentum", "split-second editorial motion"],
                _ => ["still plated moment", "slight finishing trace", "active plating energy", "pass-out momentum", "split-second hero motion"],
            },
            FocusDepth => shotMode switch
            {
                "tabletop-spread" => ["broad table clarity", "light spread falloff", "measured shared-table focus", "selective course emphasis", "tight gathering isolation"],
                "macro-detail" => ["broad detail clarity", "shallow close-focus control", "measured texture emphasis", "tight macro isolation", "razor-thin ingredient isolation"],
                "beverage-service" => ["broad drink clarity", "light glass falloff", "measured serve focus", "selective rim-and-pour isolation", "razor-held chilled isolation"],
                "hospitality-campaign" => ["broad dining clarity", "light room falloff", "measured service focus", "selective hospitality isolation", "razor-held venue isolation"],
                _ => ["broad plate clarity", "light falloff control", "measured entrée focus", "selective plate isolation", "razor-held hero isolation"],
            },
            ImageCleanliness => ["raw kitchen trace", "slight surface grit", "styling cleanliness", "menu polish", "guest-facing polish"],
            DetailDensity => shotMode switch
            {
                "tabletop-spread" => ["sparse visible detail", "light setting detail", "clear spread detail load", "rich hosting detail load", "dense banquet detail load"],
                "macro-detail" => ["sparse visible detail", "light micro detail", "clear ingredient detail load", "rich material detail load", "dense sensory detail load"],
                "beverage-service" => ["sparse visible detail", "light glass detail", "clear garnish detail load", "rich drink-service detail load", "dense chilled detail load"],
                "hospitality-campaign" => ["sparse visible detail", "light room detail", "clear service detail load", "rich dining detail load", "dense campaign detail load"],
                _ => ["sparse visible detail", "light ingredient detail", "clear plated detail load", "rich entrée detail load", "dense hero detail load"],
            },
            AtmosphericDepth => shotMode switch
            {
                "tabletop-spread" => ["flat table space", "shallow gathering recession", "gentle shared-space separation", "luminous table-to-room depth", "deep banquet recession"],
                "macro-detail" => ["flat detail space", "shallow close recession", "gentle material separation", "luminous micro depth", "deep ingredient recession"],
                "beverage-service" => ["flat drink space", "shallow bar recession", "gentle pour-space separation", "luminous glass-to-room depth", "deep lounge recession"],
                "hospitality-campaign" => ["flat dining space", "shallow room recession", "gentle venue separation", "luminous hospitality depth", "deep restaurant recession"],
                _ => ["flat plate space", "shallow tabletop recession", "gentle course separation", "luminous plate-to-setting depth", "deep dining-room recession"],
            },
            Chaos => ["styling control", "quiet restlessness", "light plating disorder", "loose service clutter", "orchestrated table disorder"],
            Whimsy => ["serious dining tone", "subtle warmth", "casual social lift", "warm gathering play", "gentle celebratory playfulness"],
            Tension => ["calm service tone", "faint rush trace", "light pass-of-service tension", "noticeable kitchen-floor pressure", "intense service pressure"],
            Awe => shotMode switch
            {
                "tabletop-spread" => ["human-scale grounding", "slight gathering pull", "quiet feast allure", "strong shared-table allure", "expansive abundance allure"],
                "macro-detail" => ["human-scale grounding", "slight sensory pull", "quiet ingredient allure", "strong material allure", "expansive appetite allure"],
                "beverage-service" => ["human-scale grounding", "slight refreshment pull", "quiet pour allure", "strong served-drink allure", "expansive chilled allure"],
                "hospitality-campaign" => ["human-scale grounding", "slight indulgence pull", "quiet dining allure", "strong hospitality allure", "expansive venue allure"],
                _ => ["human-scale grounding", "slight appetite pull", "quiet entrée allure", "strong signature-dish allure", "expansive culinary allure"],
            },
            Temperature => shotMode switch
            {
                "tabletop-spread" => ["cool table balance", "lightly cool neutrality", "neutral gathering balance", "warm hosting balance", "heated feast warmth"],
                "macro-detail" => ["cool ingredient balance", "lightly cool neutrality", "neutral material balance", "warm sensory balance", "heated close-up warmth"],
                "beverage-service" => ["cool chilled balance", "lightly cool neutrality", "neutral pour balance", "warm lounge balance", "heated hospitality warmth"],
                "hospitality-campaign" => ["cool room balance", "lightly cool neutrality", "neutral venue balance", "warm dining balance", "heated restaurant warmth"],
                _ => ["cool plate balance", "lightly cool neutrality", "neutral culinary balance", "warm appetite balance", "heated dining warmth"],
            },
            LightingIntensity => shotMode switch
            {
                "tabletop-spread" => ["dim table illumination", "soft gathering illumination", "hosting brightness", "bright course illumination", "radiant feast highlights"],
                "macro-detail" => ["dim detail illumination", "soft close illumination", "texture-shaped brightness", "highlight-shaped micro brightness", "radiant ingredient highlights"],
                "beverage-service" => ["dim service illumination", "soft pour illumination", "glass-safe brightness", "rim-shaped brightness", "radiant chilled highlights"],
                "hospitality-campaign" => ["dim room illumination", "soft venue illumination", "service-shaped brightness", "bright hospitality illumination", "radiant dining highlights"],
                _ => ["dim plate illumination", "soft entrée illumination", "appetizing brightness", "highlight-shaped brightness", "radiant plated highlights"],
            },
            Saturation => ["muted color", "restrained commercial color", "natural edible color", "rich menu color", "vivid plated color"],
            Contrast => shotMode switch
            {
                "tabletop-spread" => ["low spread contrast", "gentle setting separation", "clear serving-edge definition", "crisp course-to-linen separation", "striking feast contrast"],
                "macro-detail" => ["low detail contrast", "gentle material separation", "clear surface definition", "crisp crumb-and-fiber separation", "striking ingredient contrast"],
                "beverage-service" => ["low drink contrast", "gentle rim separation", "clear glass-and-garnish definition", "crisp condensation separation", "striking chilled contrast"],
                "hospitality-campaign" => ["low dining contrast", "gentle room separation", "clear service-edge definition", "crisp guest-facing separation", "striking venue contrast"],
                _ => ["low plated contrast", "gentle course separation", "clear plate-edge definition", "crisp sauce-and-form separation", "striking crust-and-glaze contrast"],
            },
            CameraDistance => shotMode switch
            {
                "tabletop-spread" => ["close setting crop", "close spread read", "mid-distance table read", "wider feast read", "far-set gathering overview"],
                "macro-detail" => ["extreme macro intimacy", "close ingredient study", "tight texture read", "mid-distance detail read", "far-set material overview"],
                "beverage-service" => ["close glass study", "close served-drink read", "mid-distance pour read", "wider bar-setting read", "far-set beverage overview"],
                "hospitality-campaign" => ["close dining crop", "close service read", "mid-distance venue read", "wider room setting", "far-set hospitality overview"],
                _ => ["hero close-up", "close plated read", "mid-distance entrée read", "wider place-setting read", "far-set course overview"],
            },
            CameraAngle => shotMode switch
            {
                "tabletop-spread" => ["slight dining angle", "level table view", "shared-table bias", "elevated hosting vantage", "near-overhead spread vantage"],
                "macro-detail" => ["level detail read", "slightly lowered ingredient angle", "close material angle", "slightly elevated macro vantage", "high micro vantage"],
                "beverage-service" => ["level glass read", "slightly lowered pour angle", "service-level angle", "slightly elevated bar vantage", "high beverage vantage"],
                "hospitality-campaign" => ["level dining read", "slightly lowered guest angle", "balanced venue angle", "slightly elevated service vantage", "high hospitality vantage"],
                _ => ["level plated read", "slightly lowered entrée angle", "balanced dish angle", "slightly elevated service angle", "high plated vantage"],
            },
            _ => Array.Empty<string>(),
        };
    }
}
