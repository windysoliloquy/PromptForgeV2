using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolvePhotographyPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetPhotographyBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyPhotographyGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolvePhotographyGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetPhotographyBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolvePhotographyDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var typeDescriptor = ResolvePhotographyTypeDescriptor(configuration.PhotographyType);
        if (!string.IsNullOrWhiteSpace(typeDescriptor))
        {
            AddPhotographyDescriptor(phrases, seen, typeDescriptor);
        }

        var eraDescriptor = ResolvePhotographyEraDescriptor(configuration.PhotographyEra);
        if (!string.IsNullOrWhiteSpace(eraDescriptor))
        {
            AddPhotographyDescriptor(phrases, seen, eraDescriptor);
        }

        foreach (var phrase in ResolvePhotographyModifierDescriptors(configuration))
        {
            AddPhotographyDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolvePhotographyLightingDescriptor(PromptConfiguration configuration)
    {
        var historical = IsHistoricalPhotography(configuration);

        return configuration.Lighting switch
        {
            "Soft daylight" => historical ? "period daylight" : "soft scene light",
            "Golden hour" => historical ? "late-day natural light" : "warm natural light",
            "Dramatic studio light" => historical ? "staged portrait light" : "controlled studio light",
            "Overcast" => historical ? "diffused historical daylight" : "soft overcast light",
            "Moonlit" => historical ? "low-illumination nocturne light" : "cool low-light capture",
            "Soft glow" => historical ? "chemical plate softness" : "gentle print glow",
            "Dusk haze" => historical ? "late-day atmospheric light" : "evening atmospheric light",
            "Warm directional light" => historical ? "window-directed period light" : "directional key light",
            "Volumetric cinematic light" => historical ? "atmospheric process light" : "layered atmospheric light",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string[] GetPhotographyBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var historical = IsHistoricalPhotography(configuration);
        return sliderKey switch
        {
            Stylization => historical
                ? ["formal process stillness", "lightly staged process treatment", "measured period image treatment", "deliberately styled plate treatment", "fully art-directed historical construction"]
                : ["observational capture", "lightly directed image treatment", "guided editorial treatment", "deliberately shaped visual treatment", "fully art-directed image construction"],
            Realism => historical
                ? ["observed period realism", "lightly arranged historical presence", "plate-grounded subject presence", "high-fidelity archival presence", "deeply convincing period presence"]
                : ["unforced scene observation", "lightly interpreted presence", "lens-grounded subject presence", "high-fidelity scene presence", "deeply convincing camera-true presence"],
            TextureDepth => historical
                ? ["plate-surface softness", "fine plate grain", "visible process texture", "rich archival texture", "deep plate character"]
                : ["clean capture surface", "faint print grain", "visible print texture", "rich surface grain", "deep print body"],
            NarrativeDensity => historical
                ? ["singular historical instant", "light period implication", "implied period narrative", "layered archival context", "dense historical storytelling"]
                : ["single captured instant", "light scene implication", "implied story in frame", "layered social context", "dense visual storytelling"],
            Symbolism => historical
                ? ["literal period document", "restrained emblematic cue", "suggestive process motif", "pronounced ceremonial symbolism", "archival allegory"]
                : ["literal documentary read", "restrained visual cue", "suggestive visual motif", "pronounced framing symbolism", "editorial allegory"],
            SurfaceAge => historical
                ? ["fresh process surface", "slight plate wear", "gentle archival aging", "noticeable collector patina", "time-softened plate character"]
                : ["fresh capture surface", "faint print wear", "gentle print aging", "noticeable print patina", "time-softened surface character"],
            Framing => historical
                ? ["intimate plate border", "close formal crop", "measured period framing", "broader contextual staging", "expansive historical tableau"]
                : ["intimate framing", "close portrait crop", "measured framing", "broader environmental framing", "expansive editorial staging"],
            BackgroundComplexity => historical
                ? ["spare background field", "restrained period setting", "supporting historical environment", "richly situated context", "densely layered period surroundings"]
                : ["minimal backdrop", "restrained setting", "supporting environment", "contextual scene detail", "densely layered surroundings"],
            MotionEnergy => historical
                ? ["formal stillness", "faint exposure drift", "visible long-exposure trace", "dragged motion blur", "sweeping exposure smear"]
                : ["still captured moment", "slight movement trace", "active scene motion", "candid motion energy", "split-second kinetic force"],
            FocusDepth => historical
                ? ["broad plate clarity", "shallow edge softness", "measured process focus", "selective subject isolation", "razor-held sitter clarity"]
                : ["broad image clarity", "light focus falloff", "measured focus depth", "selective focus isolation", "sharp subject separation"],
            ImageCleanliness => historical
                ? ["raw process texture", "slight plate softness", "tidy print clarity", "careful archival refinement", "immaculate plate finish"]
                : ["raw capture texture", "slight surface grit", "measured print finish", "controlled editorial polish", "immaculate print clarity"],
            DetailDensity => historical
                ? ["sparse plate detail", "light documentary detail", "clear archival detail", "rich print detail", "dense process detail"]
                : ["sparse scene detail", "light observed detail", "clear scene detail", "rich visual detail", "dense fine detail"],
            AtmosphericDepth => historical
                ? ["flat process space", "shallow tonal recession", "visible process depth", "luminous plate recession", "deep archival atmosphere"]
                : ["flat recorded space", "slight air separation", "visible atmospheric recession", "luminous scene depth", "deep lens-carried depth"],
            Chaos => historical
                ? ["controlled plate arrangement", "quiet period unrest", "field instability", "loosened process disorder", "orchestrated historical turbulence"]
                : ["controlled composition", "quiet restlessness", "scene instability", "loose street volatility", "orchestrated visual disorder"],
            Whimsy => historical
                ? ["formal social tone", "slight human lightness", "reserved social play", "warm salon looseness", "gentle period playfulness"]
                : ["serious tone", "subtle human lightness", "casual social play", "warm interpersonal looseness", "gentle editorial playfulness"],
            Tension => historical
                ? ["composed witness tone", "faint human unease", "noticeable social tension", "strong ceremonial pressure", "intense historical tension"]
                : ["quiet witness tone", "light human unease", "noticeable documentary tension", "strong interpersonal pressure", "intense scene tension"],
            Awe => historical
                ? ["document-bound scale", "slight archival presence", "quiet historical wonder", "strong historical awe", "expansive archival grandeur"]
                : ["human-scale grounding", "slight sense of presence", "quiet visual wonder", "strong felt scale", "expansive image grandeur"],
            Temperature => historical
                ? ["cool silvered tone", "restrained neutral toning", "balanced period toning", "warm sepia bias", "deep archival warmth"]
                : ["cool daylight balance", "slightly cool neutrality", "neutral daylight balance", "warm natural light", "heated warm cast"],
            LightingIntensity => historical
                ? ["dim available light", "soft window light", "steady period light", "bright process exposure", "radiant plate glow"]
                : ["dim scene light", "soft scene light", "even illumination", "bright natural light", "radiant scene light"],
            Saturation => historical
                ? ["monochrome restraint", "restrained sepia tone", "balanced historical toning", "rich print tonality", "hand-tinted period color"]
                : ["muted color", "restrained color", "natural color balance", "rich color presence", "vivid color charge"],
            Contrast => historical
                ? ["soft print contrast", "gentle plate contrast", "measured process contrast", "crisp historical separation", "striking print contrast"]
                : ["low tonal contrast", "gentle tonal separation", "balanced tonal contrast", "crisp tonal separation", "striking tonal contrast"],
            CameraDistance => historical
                ? ["intimate plate portrait", "close sitter study", "mid-distance period view", "broader historical context", "wide period framing"]
                : ["intimate close portrait", "close human view", "mid-distance scene view", "wider contextual view", "far-set environmental view"],
            CameraAngle => historical
                ? ["eye-level plate view", "slightly lowered human viewpoint", "composed formal angle", "slightly elevated historical vantage", "high detached vantage"]
                : ["eye-level view", "slightly lowered viewpoint", "level camera angle", "slightly elevated vantage", "high documentary vantage"],
            _ => Array.Empty<string>(),
        };
    }

    private static string ResolvePhotographyTypeDescriptor(string photographyType)
    {
        return photographyType switch
        {
            "portrait" => "portrait photography",
            "lifestyle-editorial" => "lifestyle editorial photography",
            "documentary-street" => "street documentary photography",
            "fine-art-photography" => "fine-art photography",
            "commercial-photography" => "commercial photography",
            _ => string.Empty,
        };
    }

    private static string ResolvePhotographyEraDescriptor(string photographyEra)
    {
        return photographyEra switch
        {
            "contemporary" => string.Empty,
            "nineteenth-century-process" => "process print character",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolvePhotographyModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetPhotographyModifierPriority(configuration.PhotographyType, configuration.PhotographyEra);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Candid Capture" when configuration.PhotographyCandidCapture => "candid capture",
                "Posed / Staged Capture" when configuration.PhotographyPosedStagedCapture => "posed studio arrangement",
                "Available Light" when configuration.PhotographyAvailableLight => IsHistoricalPhotography(configuration) ? "window-light capture" : "available-light capture",
                "On-Camera Flash" when configuration.PhotographyOnCameraFlash => IsHistoricalPhotography(configuration) ? "period flash exposure" : "direct flash",
                "Editorial Polish" when configuration.PhotographyEditorialPolish => IsHistoricalPhotography(configuration) ? "formal print refinement" : "editorial finish",
                "Raw Documentary Texture" when configuration.PhotographyRawDocumentaryTexture => "raw documentary texture",
                "Environmental Portrait Context" when configuration.PhotographyEnvironmentalPortraitContext => "environmental portrait context",
                "Film / Analog Character" when configuration.PhotographyFilmAnalogCharacter => IsHistoricalPhotography(configuration) ? "archival surface character" : "film stock character",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetPhotographyModifierPriority(string photographyType, string photographyEra)
    {
        var historical = string.Equals(photographyEra, "nineteenth-century-process", StringComparison.OrdinalIgnoreCase);

        return photographyType switch
        {
            "lifestyle-editorial" => historical
                ? ["Editorial Polish", "Available Light", "Environmental Portrait Context", "Candid Capture", "Raw Documentary Texture", "Posed / Staged Capture", "Film / Analog Character", "On-Camera Flash"]
                : ["Editorial Polish", "Available Light", "Candid Capture", "Environmental Portrait Context", "Posed / Staged Capture", "Raw Documentary Texture", "Film / Analog Character", "On-Camera Flash"],
            "documentary-street" => historical
                ? ["Candid Capture", "Raw Documentary Texture", "Available Light", "Environmental Portrait Context", "Posed / Staged Capture", "Editorial Polish", "Film / Analog Character", "On-Camera Flash"]
                : ["Candid Capture", "Raw Documentary Texture", "Available Light", "Environmental Portrait Context", "Film / Analog Character", "Posed / Staged Capture", "Editorial Polish", "On-Camera Flash"],
            "fine-art-photography" => historical
                ? ["Posed / Staged Capture", "Environmental Portrait Context", "Editorial Polish", "Available Light", "Raw Documentary Texture", "Candid Capture", "Film / Analog Character", "On-Camera Flash"]
                : ["Film / Analog Character", "Posed / Staged Capture", "Editorial Polish", "Available Light", "Raw Documentary Texture", "Environmental Portrait Context", "Candid Capture", "On-Camera Flash"],
            "commercial-photography" => historical
                ? ["Posed / Staged Capture", "Editorial Polish", "Available Light", "Environmental Portrait Context", "Raw Documentary Texture", "Candid Capture", "Film / Analog Character", "On-Camera Flash"]
                : ["Editorial Polish", "Posed / Staged Capture", "Available Light", "Environmental Portrait Context", "Film / Analog Character", "Candid Capture", "Raw Documentary Texture", "On-Camera Flash"],
            _ => historical
                ? ["Candid Capture", "Available Light", "Environmental Portrait Context", "Raw Documentary Texture", "Posed / Staged Capture", "Editorial Polish", "Film / Analog Character", "On-Camera Flash"]
                : ["Candid Capture", "Available Light", "Environmental Portrait Context", "Raw Documentary Texture", "Film / Analog Character", "Posed / Staged Capture", "Editorial Polish", "On-Camera Flash"],
        };
    }

    private static void AddPhotographyDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyPhotographyGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "immaculate plate finish";
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "hand-tinted period color";
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, MotionEnergy, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "sweeping exposure smear";
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, Tension, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "intense historical tension";
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, Awe, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "expansive archival grandeur";
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return IsHistoricalPhotography(configuration) ? "hand-tinted period color" : "vivid color";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return IsHistoricalPhotography(configuration) ? "striking print contrast" : "striking tonal contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return IsHistoricalPhotography(configuration) ? "deep archival atmosphere" : "deep lens-mediated depth";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return IsHistoricalPhotography(configuration) ? "densely layered historical environment" : "dense environmental context";
        }

        return phrase;
    }

    private static bool IsHistoricalPhotography(PromptConfiguration configuration)
    {
        return string.Equals(configuration.PhotographyEra, "nineteenth-century-process", StringComparison.OrdinalIgnoreCase);
    }
}
