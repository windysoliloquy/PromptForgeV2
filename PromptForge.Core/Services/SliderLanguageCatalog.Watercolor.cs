using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveWatercolorPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetWatercolorBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0 ? string.Empty : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyWatercolorGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveWatercolorGuideText(string sliderKey)
    {
        var labels = GetWatercolorBandLabels(sliderKey, new PromptConfiguration { WatercolorStyle = "general-watercolor" });

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static string ResolveWatercolorGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetWatercolorBandLabels(sliderKey, configuration);

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    private static string[] GetWatercolorBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var style = configuration.WatercolorStyle;

        return sliderKey switch
        {
            Stylization => style switch
            {
                "botanical-watercolor" => ["restrained specimen handling", "light herbarium stylization", "delicate botanical rendering", "refined specimen styling", "ornamental botanical language"],
                "storybook-watercolor" => ["grounded picture-book handling", "light storybook stylization", "gentle illustrative rendering", "charming narrative styling", "whimsical picture-book language"],
                "landscape-watercolor" => ["grounded plein-air handling", "light atmospheric stylization", "airy landscape rendering", "luminous wash staging", "lyrical landscape language"],
                "architectural-watercolor" => ["grounded drafted handling", "light structural stylization", "measured architectural rendering", "refined facade shaping", "articulate architectural language"],
                _ => ["grounded wash handling", "light wash stylization", "expressive wash handling", "painterly wash shaping", "lyrical wash language"],
            },
            Realism => style switch
            {
                "botanical-watercolor" => ["suggestively observed specimen form", "lightly anchored petal-and-stem form", "convincing botanical observation", "high-fidelity specimen realism", "herbarium-grade observed realism"],
                "storybook-watercolor" => ["suggestively observed figure logic", "lightly anchored illustrative form", "believable storybook observation", "high-fidelity illustrative realism", "exhibition-grade storybook realism"],
                "landscape-watercolor" => ["suggestively observed landform logic", "lightly anchored atmospheric form", "convincing plein-air observation", "high-fidelity landscape realism", "exhibition-grade landscape realism"],
                "architectural-watercolor" => ["suggestively observed structural form", "lightly anchored drafted form", "convincing built-form observation", "high-fidelity architectural realism", "exhibition-grade structural realism"],
                _ => ["suggestively observed form", "lightly anchored form logic", "convincingly observed wash realism", "high-fidelity representational realism", "exhibition-grade observed realism"],
            },
            TextureDepth => style switch
            {
                "botanical-watercolor" => ["minimal pressed-sheet tooth", "light leaf-edge blooming", "clear petal-and-pigment texture", "rich botanical wash bloom", "deeply worked specimen surface"],
                "storybook-watercolor" => ["minimal storybook grain", "light nursery-page texture", "clear ink-and-wash tooth", "rich illustrative surface character", "deeply worked picture-book surface"],
                "landscape-watercolor" => ["minimal sky-wash grain", "light earth-and-water bloom", "clear granulating terrain texture", "rich atmospheric wash character", "deeply worked plein-air surface"],
                "architectural-watercolor" => ["minimal drafted-paper tooth", "light masonry wash grain", "clear line-and-pigment texture", "rich facade-surface character", "deeply worked structural wash finish"],
                _ => ["minimal paper tooth", "light pigment settling", "clear paper-and-pigment tooth", "rich bloom-and-granulation character", "deeply worked wash surface"],
            },
            NarrativeDensity => style switch
            {
                "botanical-watercolor" => ["isolated specimen study", "light herbal context", "layered growth cues", "dense seasonal implication", "herbarium-rich natural narrative"],
                "storybook-watercolor" => ["single story beat", "light tale suggestion", "layered character cues", "dense implied scene-play", "world-rich storybook narrative"],
                "landscape-watercolor" => ["single scenic read", "light place suggestion", "layered weather cues", "dense sense-of-place", "world-rich landscape storytelling"],
                "architectural-watercolor" => ["single structural read", "light site suggestion", "layered built cues", "dense civic implication", "world-rich architectural narrative"],
                _ => ["single-read wash image", "light scene suggestion", "layered painted cues", "dense implied moment", "world-rich wash storytelling"],
            },
            Symbolism => style switch
            {
                "botanical-watercolor" => ["literal specimen framing", "subtle floral omen hints", "suggestive growth motifs", "pronounced herbal symbolism", "mythic natural charge"],
                "storybook-watercolor" => ["literal tale framing", "subtle fairytale hints", "suggestive narrative emblems", "pronounced folkloric symbolism", "mythic story charge"],
                "landscape-watercolor" => ["literal scenic framing", "subtle place-memory hints", "suggestive land motifs", "pronounced sublime symbolism", "mythic landscape charge"],
                "architectural-watercolor" => ["literal structural framing", "subtle civic hints", "suggestive built motifs", "pronounced monument symbolism", "mythic civic charge"],
                _ => ["literal wash framing", "subtle emblem hints", "suggestive symbolic motifs", "pronounced allegorical weight", "myth-charged symbolic force"],
            },
            BackgroundComplexity => style switch
            {
                "botanical-watercolor" => ["minimal page margin", "restrained foliate surround", "supporting specimen surround", "rich botanical surround", "densely layered garden setting"],
                "storybook-watercolor" => ["minimal picture-book backdrop", "restrained tale surround", "supporting story setting", "rich narrative backdrop", "densely layered story-world"],
                "landscape-watercolor" => ["minimal horizon wash", "restrained landform backdrop", "supporting scenic setting", "rich atmospheric surround", "densely layered landscape field"],
                "architectural-watercolor" => ["minimal site wash", "restrained built surround", "supporting structural setting", "rich civic surround", "densely layered urban setting"],
                _ => ["minimal wash field", "restrained painted backdrop", "supporting wash setting", "rich painted environment", "densely layered wash environment"],
            },
            AtmosphericDepth => style switch
            {
                "botanical-watercolor" => ["flat specimen space", "slight petal recession", "airy botanical depth", "luminous greenhouse haze", "deep garden atmosphere"],
                "storybook-watercolor" => ["flat picture-plane space", "slight fairytale recession", "airy narrative depth", "luminous storybook layering", "deep tale-atmosphere"],
                "landscape-watercolor" => ["flat horizon space", "slight aerial recession", "airy landscape depth", "luminous weather layering", "deep plein-air atmosphere"],
                "architectural-watercolor" => ["flat drafted space", "slight facade recession", "airy structural depth", "luminous civic layering", "deep urban atmosphere"],
                _ => ["flat wash space", "slight pigment recession", "airy wash depth", "luminous wet-paper layering", "deep atmospheric perspective"],
            },
            ImageCleanliness => style switch
            {
                "botanical-watercolor" => ["raw specimen finish", "light hand-work looseness", "balanced herbarium control", "clean botanical finish", "polished specimen-sheet finish"],
                "storybook-watercolor" => ["raw page finish", "light story-hand looseness", "balanced illustrative control", "clean picture-book finish", "polished nursery-page finish"],
                "landscape-watercolor" => ["raw plein-air finish", "light field looseness", "balanced open-air control", "clean landscape finish", "polished scenic-sheet finish"],
                "architectural-watercolor" => ["raw drafted finish", "light site-sketch looseness", "balanced structural control", "clean architectural finish", "polished presentation-sheet finish"],
                _ => ["raw studio finish", "light working looseness", "balanced wash control", "clean paper finish", "polished exhibition finish"],
            },
            LightingIntensity => style switch
            {
                "botanical-watercolor" => ["soft conservatory light", "gentle leaf-lit glow", "balanced specimen illumination", "bright petal-lit bloom", "radiant greenhouse lift"],
                "storybook-watercolor" => ["soft nursery light", "gentle page-lit glow", "balanced tale illumination", "bright picture-book bloom", "radiant story-glow"],
                "landscape-watercolor" => ["soft open-air light", "gentle sky-wash glow", "balanced plein-air illumination", "bright horizon bloom", "radiant weather-lit glow"],
                "architectural-watercolor" => ["soft site light", "gentle facade glow", "balanced structural illumination", "bright sun-struck bloom", "radiant civic light"],
                _ => ["soft daylight wash", "gentle paper-lit glow", "balanced wash illumination", "bright reserved-light glow", "radiant paper-white lift"],
            },
            Saturation => style switch
            {
                "botanical-watercolor" => ["muted herbarium color", "restrained petal chroma", "balanced leaf-and-blossom color", "rich floral bloom", "vivid garden saturation"],
                "storybook-watercolor" => ["muted nursery color", "restrained tale chroma", "balanced picture-book color", "rich storybook bloom", "vivid candy-ink saturation"],
                "landscape-watercolor" => ["muted earth-and-sky color", "restrained atmospheric chroma", "balanced plein-air color", "rich weathered bloom", "vivid horizon saturation"],
                "architectural-watercolor" => ["muted stone-and-plaster color", "restrained civic chroma", "balanced built-surface color", "rich facade bloom", "vivid urban-wash saturation"],
                _ => ["muted pigment restraint", "restrained wash chroma", "balanced pigment bloom", "rich pooled color", "vivid luminous saturation"],
            },
            Contrast => style switch
            {
                "botanical-watercolor" => ["low specimen contrast", "gentle petal separation", "balanced stem-and-leaf contrast", "crisp cutout-edge contrast", "striking botanical separation"],
                "storybook-watercolor" => ["low page contrast", "gentle character separation", "balanced story contrast", "crisp ink-and-wash snap", "striking picture-book separation"],
                "landscape-watercolor" => ["low horizon contrast", "gentle land-sky separation", "balanced atmospheric contrast", "crisp weather-cut contrast", "striking scenic separation"],
                "architectural-watercolor" => ["low facade contrast", "gentle line-and-plane separation", "balanced structural contrast", "crisp drafted-edge snap", "striking civic separation"],
                _ => ["low wash contrast", "gentle edge separation", "balanced tonal contrast", "crisp preserved-edge contrast", "striking tonal separation"],
            },
            _ => GetWatercolorSharedBandLabels(sliderKey),
        };
    }

    private static string[] GetWatercolorSharedBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            SurfaceAge => ["fresh paper", "slight working wear", "gentle paper age", "noticeable patina", "time-softened paper character"],
            Framing => ["focused framing", "light scene spacing", "balanced framing", "broad staging", "expansive composition"],
            MotionEnergy => ["still composition", "gentle movement", "active scene energy", "fluid motion", "dynamic impact"],
            FocusDepth => ["broad focus", "light focus falloff", "balanced focus depth", "selective focus", "sharp subject-isolating focus"],
            DetailDensity => ["sparse detail", "light detail", "balanced detail", "rich detail", "dense detail"],
            Chaos => ["controlled composition", "restless tension", "active energy", "dynamic looseness", "orchestrated chaos"],
            Whimsy => ["serious tone", "subtle levity", "playful charm", "strong playful energy", "bold storybook play"],
            Tension => ["low tension", "light dramatic tension", "noticeable tension", "strong dramatic tension", "intense drama"],
            Awe => ["grounded scale", "slight wonder", "atmosphere of reverence", "strong awe", "overwhelming grandeur"],
            Temperature => ["cool balance", "slightly cool wash", "neutral temperature", "warm glow", "heated warmth"],
            _ => Array.Empty<string>(),
        };
    }

    public static IEnumerable<string> ResolveWatercolorDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddWatercolorDescriptor(phrases, seen, "watercolor illustration");

        var styleDescriptor = ResolveWatercolorStyleDescriptor(configuration.WatercolorStyle);
        if (!string.IsNullOrWhiteSpace(styleDescriptor))
        {
            AddWatercolorDescriptor(phrases, seen, styleDescriptor);
        }

        foreach (var phrase in ResolveWatercolorCheckboxDescriptors(configuration))
        {
            AddWatercolorDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    private static string ResolveWatercolorStyleDescriptor(string watercolorStyle)
    {
        return watercolorStyle switch
        {
            "general-watercolor" => string.Empty,
            "botanical-watercolor" => "delicate botanical handling",
            "storybook-watercolor" => "gentle storybook presentation",
            "landscape-watercolor" => "airy landscape staging",
            "architectural-watercolor" => "structured architectural clarity",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveWatercolorCheckboxDescriptors(PromptConfiguration configuration)
    {
        var keys = GetWatercolorModifierPriority(configuration.WatercolorStyle);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Transparent Washes" when configuration.WatercolorTransparentWashes => "transparent washes",
                "Soft Bleeds" when configuration.WatercolorSoftBleeds => "soft wet-into-wet diffusion",
                "Paper Texture" when configuration.WatercolorPaperTexture => "visible cold-press paper texture",
                "Ink and Watercolor" when configuration.WatercolorInkAndWatercolor => "ink-and-wash interplay",
                "Atmospheric Wash" when configuration.WatercolorAtmosphericWash => "airy wash atmosphere",
                "Gouache Accents" when configuration.WatercolorGouacheAccents => "selective opaque gouache highlights",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetWatercolorModifierPriority(string watercolorStyle)
    {
        return watercolorStyle switch
        {
            "botanical-watercolor" => ["Paper Texture", "Transparent Washes", "Soft Bleeds", "Atmospheric Wash", "Ink and Watercolor", "Gouache Accents"],
            "storybook-watercolor" => ["Soft Bleeds", "Atmospheric Wash", "Transparent Washes", "Gouache Accents", "Paper Texture", "Ink and Watercolor"],
            "landscape-watercolor" => ["Atmospheric Wash", "Transparent Washes", "Soft Bleeds", "Paper Texture", "Gouache Accents", "Ink and Watercolor"],
            "architectural-watercolor" => ["Ink and Watercolor", "Paper Texture", "Transparent Washes", "Gouache Accents", "Soft Bleeds", "Atmospheric Wash"],
            _ => ["Transparent Washes", "Soft Bleeds", "Paper Texture", "Atmospheric Wash", "Ink and Watercolor", "Gouache Accents"],
        };
    }

    private static void AddWatercolorDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyWatercolorGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        phrase = ApplyWatercolorPhraseEconomy(phrase);

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "vivid luminous saturation";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking tonal contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "deep atmospheric perspective";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return "densely layered setting";
        }

        return phrase;
    }

    private static string ApplyWatercolorPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase.Replace("watercolor ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("watercolor", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }
}
