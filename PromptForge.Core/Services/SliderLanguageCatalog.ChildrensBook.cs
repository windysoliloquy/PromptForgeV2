using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveChildrensBookPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetChildrensBookBandLabels(sliderKey, configuration.ChildrensBookStyle);
        var phrase = labels.Length == 0 ? string.Empty : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyChildrensBookGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveChildrensBookGuideText(string sliderKey)
    {
        var labels = GetChildrensBookBandLabels(sliderKey, "general-childrens-book");

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static string ResolveChildrensBookGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetChildrensBookBandLabels(sliderKey, configuration.ChildrensBookStyle);

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    private static string[] GetChildrensBookBandLabels(string sliderKey, string childrensBookStyle)
    {
        return sliderKey switch
        {
            Stylization => childrensBookStyle switch
            {
                "storybook-classic" => ["grounded page illustration", "light pen-and-wash shaping", "classic hand-drawn illustration", "richly composed page stylization", "heirloom illustration language"],
                "playful-cartoon" => ["grounded cartoon readability", "light graphic bounce", "playful cartoon rendering", "elastic cartoon stylization", "exuberantly graphic image language"],
                "gentle-bedtime" => ["grounded bedtime illustration", "soft lullaby shaping", "cozy picture-book rendering", "dreamy page stylization", "enveloping bedtime visual language"],
                "educational-illustration" => ["grounded teaching illustration", "lightly simplified clarity", "clear explanatory rendering", "polished educational stylization", "highly legible instructional image language"],
                "whimsical-fantasy" => ["grounded fairy-tale treatment", "light enchanted shaping", "whimsical fantasy rendering", "ornate magical stylization", "enchanted visual language"],
                _ => ["grounded illustrative treatment", "light hand-drawn shaping", "picture-book rendering", "expressive page stylization", "highly authored visual language"],
            },
            Realism => childrensBookStyle switch
            {
                "storybook-classic" => ["loosely observed forms", "lightly grounded anatomy", "convincing lived-in form", "high-believability figuration", "polished representational grace"],
                "playful-cartoon" => ["freely simplified forms", "buoyantly grounded anatomy", "convincing cartoon form logic", "high-legibility caricature logic", "polished elastic figuration"],
                "gentle-bedtime" => ["softly simplified forms", "lightly grounded anatomy", "gentle form logic", "high-believability softness", "polished comforting representation"],
                "educational-illustration" => ["clearly observed forms", "firmly grounded anatomy", "convincing explanatory form", "high-clarity representation", "polished educational fidelity"],
                "whimsical-fantasy" => ["lightly stylized forms", "gently grounded anatomy", "convincing imaginative form", "high-believability fantasy figuration", "polished mythic representation"],
                _ => ["loosely observed forms", "lightly grounded anatomy", "convincing form logic", "high-believability representation", "polished representational fidelity"],
            },
            TextureDepth => childrensBookStyle switch
            {
                "storybook-classic" => ["minimal paper tooth", "soft paper grain", "visible ink-and-paper texture", "tactile page richness", "deeply worked print-like texture"],
                "playful-cartoon" => ["smooth page surface", "light surface grain", "clean graphic texture", "tactile color-block texture", "richly worked cartoon surface"],
                "gentle-bedtime" => ["velvety paper surface", "soft brushed grain", "visible paper-and-pigment softness", "tactile cozy surface", "deeply worked velvety texture"],
                "educational-illustration" => ["clean page surface", "light print grain", "visible page texture", "tactile print richness", "deeply worked reference texture"],
                "whimsical-fantasy" => ["fine paper tooth", "soft pigment grain", "visible paper-and-pigment texture", "tactile luminous surface", "deeply worked magical texture"],
                _ => ["minimal paper tooth", "soft surface grain", "visible paper-and-pigment texture", "tactile surface richness", "deeply worked material texture"],
            },
            NarrativeDensity => childrensBookStyle switch
            {
                "storybook-classic" => ["single-read tableau", "light story cueing", "layered page-turn cues", "scene-rich storytelling", "tale-bearing narrative load"],
                "playful-cartoon" => ["single gag beat", "light comic cueing", "layered story beats", "scene-rich cartoon momentum", "high-velocity narrative bounce"],
                "gentle-bedtime" => ["single restful scene", "light bedtime cueing", "layered winding-down cues", "scene-rich lullaby progression", "dream-bearing narrative drift"],
                "educational-illustration" => ["single-read concept scene", "light explanatory cueing", "layered learning cues", "concept-rich visual explanation", "knowledge-bearing narrative load"],
                "whimsical-fantasy" => ["single enchanted scene", "light quest cueing", "layered tale cues", "scene-rich fantastical progression", "world-bearing quest momentum"],
                _ => ["single-read scene", "light narrative cueing", "layered page-turn cues", "scene-rich storytelling", "world-bearing narrative load"],
            },
            Symbolism => childrensBookStyle switch
            {
                "storybook-classic" => ["mostly literal framing", "quiet emblematic cue", "suggestive fable motif", "heritage-tinted resonance", "mythic allegorical charge"],
                "playful-cartoon" => ["mostly literal framing", "light visual gag cue", "suggestive comic motif", "playful thematic echo", "boldly emblematic exaggeration"],
                "gentle-bedtime" => ["mostly literal framing", "quiet comfort cue", "suggestive dream motif", "lullaby-like resonance", "moonlit allegorical charge"],
                "educational-illustration" => ["mostly literal framing", "quiet teaching cue", "suggestive concept motif", "clear thematic reinforcement", "strongly emblematic lesson framing"],
                "whimsical-fantasy" => ["mostly literal framing", "quiet magical cue", "suggestive mythic motif", "enchanted allegorical resonance", "spellbound symbolic charge"],
                _ => ["mostly literal framing", "quiet symbolic cue", "suggestive thematic motif", "fable-like resonance", "mythic allegorical charge"],
            },
            SurfaceAge => ["fresh paper", "faint handling wear", "gentle page age", "worn surface character", "time-softened patina"],
            Framing => ["close-held framing", "lightly opened spacing", "settled scene framing", "broad spread staging", "expansive page composition"],
            BackgroundComplexity => childrensBookStyle switch
            {
                "storybook-classic" => ["minimal backdrop", "restrained setting cues", "supporting world detail", "richly developed scene setting", "densely layered tale-world detail"],
                "playful-cartoon" => ["minimal backdrop", "readable set cues", "supporting play-space detail", "richly staged cartoon setting", "densely layered play-world detail"],
                "gentle-bedtime" => ["minimal backdrop", "restrained cozy cues", "supporting room detail", "richly developed cozy setting", "densely layered nighttime detail"],
                "educational-illustration" => ["minimal backdrop", "restrained context cues", "supporting reference detail", "richly developed teaching context", "densely layered learning detail"],
                "whimsical-fantasy" => ["minimal backdrop", "restrained enchanted cues", "supporting fantasy detail", "richly developed magical setting", "densely layered enchanted world detail"],
                _ => ["minimal backdrop", "restrained setting cues", "supporting scene detail", "richly developed setting", "densely layered world detail"],
            },
            MotionEnergy => childrensBookStyle switch
            {
                "storybook-classic" => ["still composition", "gentle gestural motion", "active scene motion", "lively page movement", "sweeping narrative motion"],
                "playful-cartoon" => ["still pose humor", "bouncy motion", "active comic motion", "spring-loaded movement", "high-spirited kinetic sweep"],
                "gentle-bedtime" => ["still composition", "soft drifting motion", "gentle scene motion", "dreamy gliding movement", "floating nighttime motion"],
                "educational-illustration" => ["still explanatory pose", "light demonstrative motion", "active teaching motion", "clear step-by-step movement", "lively instructional motion"],
                "whimsical-fantasy" => ["still composition", "light spell-like drift", "active enchanted motion", "lively fantastical movement", "sweeping magical motion"],
                _ => ["still composition", "soft movement", "active scene motion", "lively directional movement", "high-spirited kinetic sweep"],
            },
            FocusDepth => ["broad focus", "shallow falloff", "even focus spread", "selective focal pull", "sharp subject isolation"],
            ImageCleanliness => ["raw working finish", "sketch-loose finish", "readable finish", "polished page finish", "pristine presentation finish"],
            DetailDensity => ["sparse detail load", "light descriptive cues", "descriptive detail layering", "richly packed detail", "dense illustrative intricacy"],
            AtmosphericDepth => ["flat spatial read", "slight recession", "airy spatial depth", "luminous depth layering", "enveloping spatial atmosphere"],
            Chaos => ["controlled composition", "mild visual unrest", "bustling scene disorder", "rollicking page commotion", "orchestrated visual mayhem"],
            Whimsy => ["serious tone", "subtle charm", "playful charm", "buoyant whimsy", "boldly fanciful energy"],
            Tension => ["low tension", "light dramatic pull", "noticeable suspense", "child-safe peril", "high-stakes page tension"],
            Awe => childrensBookStyle switch
            {
                "storybook-classic" => ["grounded scale", "slight sense of marvel", "hushed wonder", "reverent page-lit lift", "heirloom-scale grandeur"],
                "playful-cartoon" => ["grounded scale", "slight delight", "gleeful surprise", "exuberant spectacle", "big-hearted cartoon grandeur"],
                "gentle-bedtime" => ["grounded scale", "slight bedtime wonder", "hushed moonlit wonder", "tender imaginative lift", "enveloping lullaby grandeur"],
                "educational-illustration" => ["grounded scale", "slight curiosity", "sense of discovery", "elevating discovery lift", "world-opening wonder"],
                "whimsical-fantasy" => ["grounded scale", "slight sense of enchantment", "magical wonder", "soaring imaginative lift", "sweeping fantastical grandeur"],
                _ => ["grounded scale", "slight sense of marvel", "hushed wonder", "reverent imaginative lift", "sweeping childlike grandeur"],
            },
            Temperature => ["cool balance", "lightly cooled cast", "temperate color balance", "warm glow", "sun-warmed heat"],
            LightingIntensity => ["soft light", "tender illumination", "steady scene lighting", "bright illumination", "radiant light"],
            Saturation => ["muted color", "restrained palette", "full natural color", "rich chroma", "vivid chroma"],
            Contrast => ["low contrast", "gentle value separation", "stable tonal structure", "crisp tonal snap", "striking value punch"],
            _ => Array.Empty<string>(),
        };
    }

    public static IEnumerable<string> ResolveChildrensBookDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddChildrensBookDescriptor(phrases, seen, "children's book illustration");
        AddChildrensBookDescriptor(phrases, seen, ResolveChildrensBookStyleDescriptor(configuration.ChildrensBookStyle));
        AddChildrensBookDescriptor(phrases, seen, ResolveChildrensBookStoryChargeDescriptor(configuration));
        AddChildrensBookDescriptor(phrases, seen, ResolveChildrensBookAtmosphereDescriptor(configuration));

        foreach (var phrase in ResolveChildrensBookCheckboxDescriptors(configuration))
        {
            AddChildrensBookDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    private static string ResolveChildrensBookStyleDescriptor(string childrensBookStyle)
    {
        return childrensBookStyle switch
        {
            "general-childrens-book" => string.Empty,
            "storybook-classic" => "classic hand-drawn pacing",
            "playful-cartoon" => "playful cartoon timing",
            "gentle-bedtime" => "gentle bedtime mood",
            "educational-illustration" => "clear educational framing",
            "whimsical-fantasy" => "whimsical fantasy staging",
            _ => string.Empty,
        };
    }

    private static string ResolveChildrensBookStoryChargeDescriptor(PromptConfiguration configuration)
    {
        if (configuration.Tension >= 81 && configuration.MotionEnergy >= 61)
        {
            return "high-stakes child-safe peril";
        }

        if (configuration.NarrativeDensity >= 81 && configuration.MotionEnergy >= 61)
        {
            return "adventure-ready story momentum";
        }

        if (configuration.Symbolism >= 81 && configuration.Awe >= 61)
        {
            return "mythic storybook symbolism";
        }

        if (configuration.NarrativeDensity >= 61)
        {
            return "story-rich scene progression";
        }

        return string.Empty;
    }

    private static string ResolveChildrensBookAtmosphereDescriptor(PromptConfiguration configuration)
    {
        if (configuration.AtmosphericDepth >= 81 && configuration.BackgroundComplexity >= 61)
        {
            return "deep story-world atmosphere";
        }

        if (configuration.Awe >= 81 && configuration.AtmosphericDepth >= 61)
        {
            return "big-hearted storybook grandeur";
        }

        if (configuration.BackgroundComplexity >= 81 && configuration.NarrativeDensity >= 61)
        {
            return "densely layered storybook environment";
        }

        return string.Empty;
    }

    private static IEnumerable<string> ResolveChildrensBookCheckboxDescriptors(PromptConfiguration configuration)
    {
        var keys = GetChildrensBookModifierPriority(configuration.ChildrensBookStyle);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Soft Color Palette" when configuration.ChildrensBookSoftColorPalette => "soft color palette",
                "Textured Paper" when configuration.ChildrensBookTexturedPaper => "textured paper",
                "Ink Linework" when configuration.ChildrensBookInkLinework => "ink linework",
                "Expressive Characters" when configuration.ChildrensBookExpressiveCharacters => "expressive characters",
                "Minimal Background" when configuration.ChildrensBookMinimalBackground => "minimal background",
                "Decorative Details" when configuration.ChildrensBookDecorativeDetails => "decorative details",
                "Gentle Lighting" when configuration.ChildrensBookGentleLighting => "gentle lighting",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetChildrensBookModifierPriority(string childrensBookStyle)
    {
        return childrensBookStyle switch
        {
            "storybook-classic" => ["Textured Paper", "Ink Linework", "Decorative Details", "Soft Color Palette", "Gentle Lighting", "Expressive Characters", "Minimal Background"],
            "playful-cartoon" => ["Expressive Characters", "Soft Color Palette", "Decorative Details", "Gentle Lighting", "Minimal Background", "Ink Linework", "Textured Paper"],
            "gentle-bedtime" => ["Gentle Lighting", "Soft Color Palette", "Textured Paper", "Expressive Characters", "Minimal Background", "Decorative Details", "Ink Linework"],
            "educational-illustration" => ["Ink Linework", "Minimal Background", "Textured Paper", "Decorative Details", "Expressive Characters", "Soft Color Palette", "Gentle Lighting"],
            "whimsical-fantasy" => ["Decorative Details", "Expressive Characters", "Soft Color Palette", "Gentle Lighting", "Ink Linework", "Textured Paper", "Minimal Background"],
            _ => ["Soft Color Palette", "Textured Paper", "Ink Linework", "Expressive Characters", "Minimal Background", "Decorative Details", "Gentle Lighting"],
        };
    }

    private static void AddChildrensBookDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyChildrensBookGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        phrase = ApplyChildrensBookPhraseEconomy(phrase);

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "vivid color";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking tonal contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "deep story-world atmosphere";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return "densely layered storybook environment";
        }

        if (string.Equals(sliderKey, NarrativeDensity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.MotionEnergy >= 61)
        {
            return "adventure-ready story momentum";
        }

        if (string.Equals(sliderKey, Symbolism, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Awe >= 61)
        {
            return "mythic storybook symbolism";
        }

        if (string.Equals(sliderKey, Tension, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.MotionEnergy >= 61)
        {
            return "high-stakes child-safe peril";
        }

        if (string.Equals(sliderKey, Awe, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth >= 61)
        {
            return "big-hearted storybook grandeur";
        }

        if (string.Equals(sliderKey, MotionEnergy, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Tension >= 61)
        {
            return "high-spirited storybook motion";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity >= 61)
        {
            return "densely layered storybook environment";
        }

        if (string.Equals(sliderKey, Chaos, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.MotionEnergy >= 61)
        {
            return "orchestrated adventure-chaos";
        }

        return phrase;
    }

    private static string ApplyChildrensBookPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("children's book ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("storybook ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("storybook", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }
}
