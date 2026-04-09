using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveAnimePhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetAnimeBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyAnimeGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveAnimeGuideText(string sliderKey)
    {
        var labels = GetAnimeSharedBandLabels(sliderKey);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static string ResolveAnimeGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetAnimeBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    private static string[] GetAnimeBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var style = NormalizeAnimeStyleKey(configuration.AnimeStyle);
        var labels = string.IsNullOrWhiteSpace(style) || string.Equals(style, "general-anime", StringComparison.OrdinalIgnoreCase)
            ? GetAnimeSharedBandLabels(sliderKey)
            : sliderKey switch
        {
            Stylization => style switch
            {
                "shonen-action" => ["grounded action treatment", "light action stylization", "battle-ready stylization", "assertive combat design", "high-voltage heroic exaggeration"],
                "shojo-romance" => ["grounded romantic softness", "light ornamental stylization", "expressive romantic stylization", "delicate emotive exaggeration", "ornamental dream-romance styling"],
                "seinen-dark" => ["grounded dramatic severity", "light grim stylization", "shadow-weighted stylization", "oppressive dramatic shaping", "harsh mature visual severity"],
                "fantasy-anime" => ["grounded enchanted treatment", "light enchanted stylization", "spellbound fantasy rendering", "ornate mythic stylization", "high-magic world design language"],
                "mecha-sci-fi-anime" => ["grounded industrial design treatment", "light tech stylization", "machine-led stylization", "hardened futurist shape logic", "high-authorial futurist design"],
                "slice-of-life" => ["grounded everyday sketch", "light domestic stylization", "warm observational stylization", "tender lived-in shaping", "highly charming daily-life stylization"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            NarrativeDensity => style switch
            {
                "shonen-action" => ["single clash beat", "light rivalry cueing", "mission-driven scene cues", "layered conflict escalation", "saga-thick battle stakes"],
                "shojo-romance" => ["single relational beat", "light emotional cueing", "layered affection cues", "dense heartline implication", "world-rich romantic entanglement"],
                "seinen-dark" => ["single grim beat", "light unease cueing", "layered social pressure", "dense moral implication", "world-heavy existential burden"],
                "fantasy-anime" => ["single quest beat", "light lore cueing", "layered mythic cues", "dense world-fabric implication", "legend-rich quest weave"],
                "mecha-sci-fi-anime" => ["single systems beat", "light tech cueing", "layered mission logic", "dense faction implication", "world-built operational stakes"],
                "slice-of-life" => ["single daily beat", "light routine cueing", "layered everyday cues", "dense lived-in implication", "world-rich everyday texture"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            MotionEnergy => style switch
            {
                "shonen-action" => ["held pre-strike posture", "gathering momentum", "active clash movement", "explosive impact flow", "breakneck strike velocity"],
                "shojo-romance" => ["held emotional pause", "drifting gesture", "active relational movement", "sweeping emotional flow", "emotion-rush motion"],
                "seinen-dark" => ["held uneasy stillness", "restrained motion leak", "active pressure movement", "violent disruption flow", "brutal kinetic rupture"],
                "fantasy-anime" => ["held enchanted pause", "spell-stir movement", "active adventure motion", "sweeping adventure flow", "surging mythic motion"],
                "mecha-sci-fi-anime" => ["locked mechanical stance", "actuator stir", "active systems motion", "thrust-driven force flow", "high-output combat surge"],
                "slice-of-life" => ["held ordinary pose", "casual movement hint", "active daily-life motion", "gentle routine flow", "busy daily-life flow"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            FocusDepth => style switch
            {
                "shonen-action" => ["broad action readability", "light falloff", "measured hero emphasis", "selective strike isolation", "hard strike isolation"],
                "shojo-romance" => ["broad relational clarity", "soft falloff", "measured face emphasis", "selective emotional isolation", "dreamy intimacy pull"],
                "seinen-dark" => ["broad scene clarity", "shadowed falloff", "measured pressure focus", "selective psychological pull", "hard unease isolation"],
                "fantasy-anime" => ["broad world clarity", "enchanted falloff", "measured wonder focus", "selective enchanted pull", "spellbound subject isolation"],
                "mecha-sci-fi-anime" => ["broad systems clarity", "clean falloff", "measured machine emphasis", "selective pilot-machine pull", "hard target isolation"],
                "slice-of-life" => ["broad everyday clarity", "gentle falloff", "measured situational emphasis", "selective domestic pull", "intimate daily-life isolation"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            AtmosphericDepth => style switch
            {
                "shonen-action" => ["flat clash space", "slight charge recession", "air-cut action depth", "dramatic impact layering", "deep combat recession"],
                "shojo-romance" => ["flat relational space", "slight glow recession", "airy emotional depth", "luminous romantic layering", "deep dreamy recession"],
                "seinen-dark" => ["flat oppressive space", "slight murk recession", "weighted spatial depth", "shadow-loaded recession", "deep ominous recession"],
                "fantasy-anime" => ["flat storybook space", "slight enchanted recession", "air-filled wonder depth", "luminous mythic layering", "deep legendary atmosphere"],
                "mecha-sci-fi-anime" => ["flat engineered space", "slight structural recession", "technical depth lanes", "luminous structural layering", "deep industrial recession"],
                "slice-of-life" => ["flat domestic space", "slight lived-in recession", "breathable everyday depth", "warm ambient layering", "deep lived-in recession"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            Whimsy => style switch
            {
                "shonen-action" => ["stern heroic tone", "light swagger", "spirited play", "boisterous heroic play", "bold comic swagger"],
                "shojo-romance" => ["sincere romantic tone", "light sweetness", "playful affection", "sparkling emotional play", "bold sparkling charm"],
                "seinen-dark" => ["grave tonal posture", "dry irony", "bitter tonal play", "sharp sardonic play", "bold sardonic bite"],
                "fantasy-anime" => ["earnest wonder tone", "light magic play", "playful enchantment", "spirited enchanted play", "bold enchanted exuberance"],
                "mecha-sci-fi-anime" => ["serious systems tone", "light systems charm", "playful gadget energy", "spirited futurist bounce", "bold gadget exuberance"],
                "slice-of-life" => ["serious everyday tone", "light neighborly charm", "playful warmth", "strong situational play", "bold everyday comedy"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            Tension => style switch
            {
                "shonen-action" => ["low rivalry tension", "light contest friction", "battle-ready pressure", "strong showdown strain", "life-or-death heat"],
                "shojo-romance" => ["low romantic strain", "light heart-friction", "noticeable emotional pressure", "strong relational ache", "overwhelming heart-crisis tension"],
                "seinen-dark" => ["low background dread", "light social friction", "noticeable grim pressure", "strong moral strain", "crushing existential tension"],
                "fantasy-anime" => ["low quest strain", "light peril friction", "noticeable adventure pressure", "strong quest peril", "overwhelming legendary peril"],
                "mecha-sci-fi-anime" => ["low systems strain", "light mission friction", "noticeable operational pressure", "strong combat urgency", "catastrophic engagement pressure"],
                "slice-of-life" => ["low daily strain", "light interpersonal friction", "noticeable awkward pressure", "strong domestic tension", "overwhelming turning-point pressure"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            Awe => style switch
            {
                "shonen-action" => ["grounded heroic scale", "slight hype", "charged spectacle", "towering heroic grandeur", "overwhelming clash spectacle"],
                "shojo-romance" => ["grounded emotional scale", "slight flutter", "charged romantic wonder", "sweeping emotional grandeur", "overwhelming dreamlike rapture"],
                "seinen-dark" => ["grounded grim scale", "slight unease", "charged ominous wonder", "looming dramatic grandeur", "overwhelming oppressive spectacle"],
                "fantasy-anime" => ["grounded quest scale", "slight enchantment", "charged mythic wonder", "towering legendary grandeur", "overwhelming spellbound spectacle"],
                "mecha-sci-fi-anime" => ["grounded industrial scale", "slight systems wonder", "charged mechanized spectacle", "towering machine grandeur", "overwhelming mechanized spectacle"],
                "slice-of-life" => ["grounded everyday scale", "slight gentle wonder", "charged ordinary magic", "strong lived-in wonder", "overwhelming ordinary tenderness"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            LightingIntensity => style switch
            {
                "shonen-action" => ["soft action light", "combat-lit lift", "even battle light", "bright impact light", "blazing clash illumination"],
                "shojo-romance" => ["soft romantic glow", "soft romantic lift", "balanced tender glow", "bright romantic glow", "radiant heart-glow"],
                "seinen-dark" => ["dim weighted light", "low-key lift", "even low-key light", "stark dramatic light", "hard severe illumination"],
                "fantasy-anime" => ["soft enchanted glow", "enchanted lift", "even wonder light", "bright spell-lit glow", "radiant mythic illumination"],
                "mecha-sci-fi-anime" => ["cool systems glow", "measured lift", "even engineered light", "bright systems clarity", "radiant reactor glow"],
                "slice-of-life" => ["soft window light", "gentle household lift", "even daily light", "bright domestic light", "radiant afternoon glow"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            Saturation => style switch
            {
                "shonen-action" => ["restrained primary color", "balanced hero chroma", "vivid action color", "rich contest color", "electric battle color"],
                "shojo-romance" => ["muted blush chroma", "balanced petal color", "rich romantic color", "vivid petal-jewel color", "luminous candy-blush palette"],
                "seinen-dark" => ["muted bruised color", "restrained shadow chroma", "balanced severe color", "rich nocturnal color", "harsh fevered chroma"],
                "fantasy-anime" => ["muted storybook color", "balanced enchanted chroma", "rich quest color", "vivid spell color", "luminous myth-palette"],
                "mecha-sci-fi-anime" => ["muted alloy color", "restrained signal chroma", "rich tech color", "vivid systems color", "luminous neon-alloy palette"],
                "slice-of-life" => ["muted domestic color", "balanced lived-in chroma", "rich everyday color", "vivid seasonal color", "luminous seasonal palette"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            Contrast => style switch
            {
                "shonen-action" => ["soft impact edges", "balanced strike separation", "crisp clash definition", "hard impact snap", "explosive tonal strike"],
                "shojo-romance" => ["soft blush edges", "gentle romantic separation", "clear emotional definition", "crisp sparkle snap", "luminous heart-contrast"],
                "seinen-dark" => ["soft murk edges", "stern shadow separation", "crisp severe definition", "hard noir snap", "crushing tonal severity"],
                "fantasy-anime" => ["soft enchanted edges", "balanced spell separation", "crisp mythic definition", "dramatic rune-lit snap", "blazing legendary contrast"],
                "mecha-sci-fi-anime" => ["soft panel edges", "balanced systems separation", "crisp mechanical definition", "hard industrial snap", "striking reactor contrast"],
                "slice-of-life" => ["soft domestic edges", "gentle household separation", "clear everyday definition", "crisp seasonal snap", "bright lived-in contrast"],
                _ => GetAnimeSharedBandLabels(sliderKey),
            },
            _ => GetAnimeSharedBandLabels(sliderKey),
        };

        var eraLabels = GetAnimeEraOverlayBandLabels(sliderKey, NormalizeAnimeEraKey(configuration.AnimeEra));
        return eraLabels.Length == 0 ? labels : eraLabels;
    }

    private static string[] GetAnimeSharedBandLabels(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded cel treatment", "light graphic lift", "clear stylized rendering", "assertive shape clarity", "high-authorial visual abstraction" },
            Realism => new[] { "openly graphic form logic", "light structural grounding", "credible constructed form", "high-fidelity figure logic", "production-grade visual conviction" },
            TextureDepth => new[] { "flat celsheet finish", "faint surface modulation", "refined material cueing", "tactile surface build", "highly worked finish skin" },
            NarrativeDensity => new[] { "single-read image beat", "light story suggestion", "layered character cues", "dense implied scenario", "world-thick narrative charge" },
            Symbolism => new[] { "mostly literal framing", "quiet motif hints", "suggestive thematic markers", "pronounced allegorical weight", "myth-charged symbolic force" },
            SurfaceAge => new[] { "freshly finished surface", "faint handled wear", "gentle production softening", "noticeable patina trace", "time-softened studio residue" },
            Framing => new[] { "tight subject framing", "slightly opened spacing", "balanced scene crop", "broad set-piece framing", "expansive stage breadth" },
            BackgroundComplexity => new[] { "minimal backdrop support", "restrained setting cues", "readable place context", "rich scene support", "densely layered world detail" },
            MotionEnergy => new[] { "still held pose", "gentle movement hint", "active scene motion", "dynamic impact flow", "high-kinetic strike energy" },
            FocusDepth => new[] { "broad focal spread", "light falloff", "measured focal balance", "selective subject pull", "hard subject isolation" },
            ImageCleanliness => new[] { "raw production edge", "slight grit retention", "even finish control", "clean polish", "high-sheen presentation" },
            DetailDensity => new[] { "sparse information load", "light descriptive detail", "measured information density", "rich descriptive load", "dense visual packing" },
            AtmosphericDepth => new[] { "flat picture-plane space", "slight aerial recession", "air-filled depth", "luminous spatial layering", "deep receding atmosphere" },
            Chaos => new[] { "controlled arrangement", "restless imbalance", "volatile scene pressure", "directed disorder", "orchestrated instability" },
            Whimsy => new[] { "serious tonal posture", "light charm", "expressive play", "strong whimsical bounce", "bold comic exaggeration" },
            Tension => new[] { "low dramatic strain", "light friction", "noticeable pressure", "strong confrontation", "high-stakes dramatic heat" },
            Awe => new[] { "grounded scale", "slight wonder", "charged wonder field", "cinematic grandeur", "overwhelming spectacle" },
            Temperature => new[] { "cool-biased palette", "slightly cool balance", "neutral color balance", "warm-biased palette", "heated chromatic cast" },
            LightingIntensity => new[] { "soft illumination", "gentle lift", "even scene light", "bright light push", "radiant illumination" },
            Saturation => new[] { "muted palette", "restrained chroma", "balanced color charge", "vivid chromatic lift", "radiant palette intensity" },
            Contrast => new[] { "soft tonal spread", "gentle value separation", "balanced tonal snap", "crisp value cut", "striking tonal separation" },
            _ => Array.Empty<string>(),
        };

        return labels;
    }

    private static string[] GetAnimeEraOverlayBandLabels(string sliderKey, string era)
    {
        if (string.IsNullOrWhiteSpace(era) || string.Equals(era, "modern-default", StringComparison.OrdinalIgnoreCase))
        {
            return Array.Empty<string>();
        }

        return sliderKey switch
        {
            TextureDepth => era switch
            {
                "classic-anime" => ["flat painted-cel finish", "faint analog surface cueing", "hand-painted material hints", "rich cel-and-background texture", "heavily worked vintage cel finish"],
                "cel-era" => ["flat acetate finish", "faint cel paint grain", "analog cel material cueing", "rich cel-and-backdrop texture", "heavily worked cel-era finish"],
                "broadcast-anime" => ["flat TV-cel finish", "faint broadcast-era surface cueing", "clean cel material definition", "rich paint-and-ink texture", "heavily worked broadcast-cel finish"],
                "early-digital" => ["flat hybrid finish", "faint composited surface cueing", "digital-cel material definition", "rich hybrid texture layering", "heavily worked early-digital finish"],
                "modern-anime" => ["flat polished finish", "faint premium surface cueing", "refined digital material definition", "rich seasonal-finish texture", "heavily worked modern-production finish"],
                _ => Array.Empty<string>(),
            },
            ImageCleanliness => era switch
            {
                "classic-anime" => ["raw analog edge", "slight print grit", "even studio control", "cleaned vintage presentation", "archival cel polish"],
                "cel-era" => ["raw cel residue", "slight paint-speck retention", "even cel-finish control", "cleaned acetate presentation", "premium cel-era polish"],
                "broadcast-anime" => ["raw broadcast edge", "slight TV-era grit", "even broadcast control", "cleaned TV-cel presentation", "polished broadcast finish"],
                "early-digital" => ["raw hybrid edge", "slight compositing grit", "even digital control", "cleaned early-digital presentation", "polished hybrid finish"],
                "modern-anime" => ["raw digital edge", "slight production grit", "even seasonal control", "cleaned premium presentation", "ultra-polished seasonal finish"],
                _ => Array.Empty<string>(),
            },
            LightingIntensity => era switch
            {
                "classic-anime" => ["modest painted light", "gentle analog lift", "even vintage scene light", "bright hand-painted glow", "radiant classic highlights"],
                "cel-era" => ["soft cel light", "warm analog lift", "even cel-era light", "bright painted glow", "radiant cel warmth"],
                "broadcast-anime" => ["soft TV light", "clean broadcast lift", "even broadcast light", "bright TV-cel glow", "radiant broadcast highlights"],
                "early-digital" => ["soft hybrid light", "composited lift", "even digital light", "bright layered glow", "radiant early-digital illumination"],
                "modern-anime" => ["soft premium light", "polished seasonal lift", "even production light", "bright modern glow", "radiant seasonal illumination"],
                _ => Array.Empty<string>(),
            },
            Contrast => era switch
            {
                "classic-anime" => ["soft poster edges", "gentle print separation", "clear vintage definition", "crisp painted snap", "striking classic contrast"],
                "cel-era" => ["soft cel edges", "balanced ink separation", "clear cel definition", "crisp acetate snap", "striking cel-era contrast"],
                "broadcast-anime" => ["soft TV-cel edges", "clean broadcast separation", "clear line-and-fill definition", "crisp TV-era snap", "striking broadcast contrast"],
                "early-digital" => ["soft hybrid edges", "balanced composited separation", "clear digital-cel definition", "crisp hybrid snap", "striking early-digital contrast"],
                "modern-anime" => ["soft polished edges", "balanced premium separation", "clear seasonal definition", "crisp modern snap", "striking production contrast"],
                _ => Array.Empty<string>(),
            },
            _ => Array.Empty<string>(),
        };
    }

    public static IEnumerable<string> ResolveAnimeDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddAnimeDescriptor(phrases, seen, "stylized anime illustration");
        AddAnimeDescriptor(phrases, seen, "polished key-art finish");
        AddAnimeDescriptor(phrases, seen, "character-led staging");
        AddAnimeDescriptor(phrases, seen, "clear silhouette read");
        AddAnimeDescriptor(phrases, seen, "emotion-first expression");

        AddAnimeDescriptor(phrases, seen, ResolveAnimeStyleDescriptor(configuration.AnimeStyle));
        AddAnimeDescriptor(phrases, seen, ResolveAnimeEraDescriptor(configuration.AnimeEra));

        foreach (var phrase in ResolveAnimeCheckboxDescriptors(configuration))
        {
            AddAnimeDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    private static IEnumerable<string> ResolveAnimeCheckboxDescriptors(PromptConfiguration configuration)
    {
        var keys = GetAnimeModifierPriority(configuration.AnimeStyle);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Cel Shading" when configuration.AnimeCelShading => "crisp cel bands",
                "Clean Line Art" when configuration.AnimeCleanLineArt => "precise contour ink",
                "Expressive Eyes" when configuration.AnimeExpressiveEyes => "iris-led expression",
                "Dynamic Action" when configuration.AnimeDynamicAction => "impact-first staging",
                "Cinematic Lighting" when configuration.AnimeCinematicLighting => "filmic light shaping",
                "Stylized Hair" when configuration.AnimeStylizedHair => "graphic hair masses",
                "Atmospheric Effects" when configuration.AnimeAtmosphericEffects => "suspended particulate atmosphere",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetAnimeModifierPriority(string animeStyle)
    {
        return NormalizeAnimeStyleKey(animeStyle) switch
        {
            "shonen-action" => ["Dynamic Action", "Cinematic Lighting", "Cel Shading", "Clean Line Art", "Stylized Hair", "Atmospheric Effects", "Expressive Eyes"],
            "shojo-romance" => ["Expressive Eyes", "Clean Line Art", "Stylized Hair", "Cel Shading", "Atmospheric Effects", "Cinematic Lighting", "Dynamic Action"],
            "seinen-dark" => ["Cinematic Lighting", "Clean Line Art", "Atmospheric Effects", "Cel Shading", "Dynamic Action", "Stylized Hair", "Expressive Eyes"],
            "fantasy-anime" => ["Atmospheric Effects", "Cinematic Lighting", "Clean Line Art", "Cel Shading", "Stylized Hair", "Expressive Eyes", "Dynamic Action"],
            "mecha-sci-fi-anime" => ["Clean Line Art", "Cinematic Lighting", "Dynamic Action", "Cel Shading", "Atmospheric Effects", "Stylized Hair", "Expressive Eyes"],
            "slice-of-life" => ["Clean Line Art", "Expressive Eyes", "Atmospheric Effects", "Cel Shading", "Stylized Hair", "Cinematic Lighting", "Dynamic Action"],
            _ => ["Clean Line Art", "Cel Shading", "Expressive Eyes", "Cinematic Lighting", "Stylized Hair", "Atmospheric Effects", "Dynamic Action"],
        };
    }

    private static string ResolveAnimeStyleDescriptor(string animeStyle)
    {
        return NormalizeAnimeStyleKey(animeStyle) switch
        {
            "general-anime" => string.Empty,
            "shonen-action" => "forward-drive combat momentum",
            "shojo-romance" => "soft relational delicacy",
            "seinen-dark" => "heavier dramatic gravity",
            "fantasy-anime" => "myth-laced world fabric",
            "mecha-sci-fi-anime" => "industrial futurist design logic",
            "slice-of-life" => "everyday closeness",
            _ => string.Empty,
        };
    }

    private static string ResolveAnimeEraDescriptor(string animeEra)
    {
        return NormalizeAnimeEraKey(animeEra) switch
        {
            "modern-default" => string.Empty,
            "classic-anime" => "simplified forms and flatter graphic logic",
            "cel-era" => "analog cel warmth with painted depth",
            "broadcast-anime" => "TV-cel cleanliness",
            "early-digital" => "transitional digital compositing",
            "modern-anime" => "seasonal-finish polish",
            _ => string.Empty,
        };
    }

    private static string NormalizeAnimeStyleKey(string animeStyle)
    {
        return animeStyle switch
        {
            "General Anime" or "general-anime" => "general-anime",
            "Shonen Action" or "shonen-action" => "shonen-action",
            "Shojo Romance" or "shojo-romance" => "shojo-romance",
            "Seinen Dark" or "seinen-dark" => "seinen-dark",
            "Fantasy Anime" or "fantasy-anime" => "fantasy-anime",
            "Mecha / Sci-fi Anime" or "mecha-sci-fi-anime" => "mecha-sci-fi-anime",
            "Slice of Life" or "slice-of-life" => "slice-of-life",
            _ => animeStyle,
        };
    }

    private static string NormalizeAnimeEraKey(string animeEra)
    {
        return animeEra switch
        {
            "Default / Modern" or "modern-default" => "modern-default",
            "Classic Anime (1960s–1970s)" or "Classic Anime (1960sâ€“1970s)" or "classic-anime" => "classic-anime",
            "Cel-Era Anime (1980s)" or "cel-era" => "cel-era",
            "Broadcast Anime (1990s)" or "broadcast-anime" => "broadcast-anime",
            "Early Digital Anime (2000s)" or "early-digital" => "early-digital",
            "Modern Anime (2010s+)" or "modern-anime" => "modern-anime",
            _ => animeEra,
        };
    }

    private static void AddAnimeDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyAnimeGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast >= 60)
        {
            return "radiant palette intensity";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation >= 60)
        {
            return "striking tonal separation";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity >= 60)
        {
            return "deep receding atmosphere";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth >= 60)
        {
            return "densely layered world detail";
        }

        return phrase;
    }
}
