using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveVintageBendLightingDescriptor(PromptConfiguration configuration)
    {
        var value = configuration.LightingIntensity;
        return value switch
        {
            <= 20 => "subdued practical room light",
            <= 40 => "practical fluorescent and tungsten mixed light",
            <= 60 => "balanced practical illumination",
            <= 80 => "clear period interior brightness",
            _ => "strong practical-light presence",
        };
    }

    public static IEnumerable<string> ResolveVintageBendDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddVintageDescriptor(phrases, seen, "early-1980s GDR institutional thriller");
        AddVintageDescriptor(phrases, seen, "East German spatial realism");
        AddVintageDescriptor(phrases, seen, "muted analog film color");
        AddVintageDescriptor(phrases, seen, "practical room light");
        AddVintageDescriptor(phrases, seen, "plain social reserve");
        AddVintageDescriptor(phrases, seen, "worn paper-and-wood detail");
        AddVintageDescriptor(phrases, seen, "bureaucratic interior stillness");
        AddVintageDescriptor(phrases, seen, "watchful procedural tension");

        return phrases;
    }

    private static string ResolveVintageBendArtistInfluenceDescriptor(int strength, string artistName)
    {
        if (strength <= 20)
        {
            return string.Empty;
        }

        var band = GetBandIndex(strength);
        var phrase = band switch
        {
            0 => string.Empty,
            1 => "light stylistic cues from {artist}",
            2 => "artist-informed sensibility drawn from {artist}",
            3 => "clearly shaped by {artist}",
            _ => "deeply informed by {artist}",
        };

        return string.IsNullOrWhiteSpace(phrase)
            ? string.Empty
            : phrase.Replace("{artist}", artistName, StringComparison.Ordinal);
    }

    public static string ResolveVintageBendPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        if (configuration.VintageBendUrbanCivilian)
        {
            var urbanCivilianPhrase = ResolveVintageBendUrbanCivilianPhrase(sliderKey, value);
            if (!string.IsNullOrWhiteSpace(urbanCivilianPhrase))
            {
                return urbanCivilianPhrase;
            }
        }

        var bandIndex = GetBandIndex(value);
        var phrase = sliderKey switch
        {
            ArtistInfluenceStrength => string.Empty,
            Stylization => MapBand(value,
                "plain period depiction",
                "restrained dramatic shaping",
                "directed period stylization",
                "severe formal shaping",
                "austere thriller stylization"),
            Realism => MapBand(value,
                "omit explicit realism",
                "plausible period realism",
                "credible institutional realism",
                "highly convincing historical realism",
                "exacting production realism"),
            TextureDepth => MapBand(value,
                "sparse surface description",
                "light wear marks",
                "clear cloth, paper, wood, and glass texture",
                "rich analog material detail",
                "deeply worked tactile wear"),
            NarrativeDensity => MapBand(value,
                "isolated scene fact",
                "light situational context",
                "layered room-and-role cues",
                "dense procedural setting",
                "fully networked social context"),
            Symbolism => MapBand(value,
                "mostly literal",
                "subtle authority cues",
                "suggestive civic motifs",
                "surveillance allegory",
                "historical-state symbolism"),
            SurfaceAge => MapBand(value,
                "well-kept surfaces",
                "light handling marks",
                "ordinary office wear",
                "tired public-room patina",
                "prolonged state-use wear"),
            Framing => MapBand(value,
                "tight document framing",
                "close room framing",
                "settled scene framing",
                "broad spatial framing",
                "detached observational framing"),
            BackgroundComplexity => MapBand(value,
                "bare setting support",
                "limited spatial detail",
                "supporting room or street detail",
                "layered civic or housing-block detail",
                "densely observed institutional environment"),
            MotionEnergy => MapBand(value,
                "still observation",
                "restrained movement",
                "active procedural motion",
                "urgent human motion",
                "rapid movement under control"),
            FocusDepth => MapBand(value,
                "narrow focal isolation",
                "subject-led focus",
                "even room focus",
                "broad interior clarity",
                "front-to-back clarity"),
            ImageCleanliness => MapBand(value,
                "raw handling character",
                "modest roughness",
                "orderly print finish",
                "controlled period finish",
                "exact technical finish"),
            DetailDensity => MapBand(value,
                "sparse factual detail",
                "selective scene detail",
                "descriptive object detail",
                "dense paper-and-equipment detail",
                "exhaustive scene-inventory detail"),
            AtmosphericDepth => MapBand(value,
                "flat enclosed air",
                "slight spatial recession",
                "dim air-filled depth",
                "layered smoke or winter air",
                "deep air-separated recession"),
            Chaos => MapBand(value,
                "ordered placement",
                "unsettled order",
                "procedural strain",
                "frayed public order",
                "failing room order"),
            Whimsy => MapBand(value,
                "sober tone",
                "faint human warmth",
                "mild everyday warmth",
                "ironic social lightness",
                "brittle satirical play"),
            Tension => MapBand(value,
                "low dramatic pressure",
                "quiet suspicion",
                "visible interpersonal strain",
                "surveillance pressure",
                "acute state fear"),
            Awe => MapBand(value,
                "human scale",
                "slight historical weight",
                "civic heaviness",
                "oppressive official scale",
                "overwhelming structural weight"),
            Saturation => MapBand(value,
                "drained color",
                "low-chroma color",
                "restrained period color",
                "dense selective color",
                "severe accent color"),
            Contrast => MapBand(value,
                "soft tonal spread",
                "subdued tonal separation",
                "measured dark-light structure",
                "hard room contrast",
                "severe lamp-shadow contrast"),
            Temperature => MapBand(value,
                "cold gray cast",
                "cool stale air",
                "neutral mixed indoor balance",
                "worn tungsten warmth",
                "heated amber cast"),
            LightingIntensity => MapBand(value,
                "dim practical light",
                "low room light",
                "ordinary interior light",
                "hard desk-lamp emphasis",
                "exposed lamp-and-shadow pressure"),
            CameraDistance => MapBand(value,
                "intimate face distance",
                "near conversational distance",
                "room-scale distance",
                "hall-scale distance",
                "remote observer distance"),
            CameraAngle => MapBand(value,
                "seated low angle",
                "modest low angle",
                "eye-level observation",
                "slight supervisory height",
                "overhead surveillance angle"),
            _ => string.Empty,
        };

        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        return ApplyVintageBendGuardrails(sliderKey, bandIndex, configuration, phrase);
    }

    public static string ResolveVintageBendGuideText(string sliderKey)
    {
        return ResolveVintageBendGuideText(sliderKey, null);
    }

    public static string ResolveVintageBendGuideText(string sliderKey, PromptConfiguration? configuration)
    {
        if (configuration?.VintageBendUrbanCivilian == true)
        {
            var urbanCivilianLabels = GetVintageBendUrbanCivilianGuideLabels(sliderKey);
            if (urbanCivilianLabels.Length != 0)
            {
                return string.Join("  |  ", urbanCivilianLabels);
            }
        }

        var labels = sliderKey switch
        {
            ArtistInfluenceStrength => new[] { "omit artist language", "light stylistic cues from", "artist-informed sensibility drawn from", "clearly shaped by", "deeply informed by" },
            Stylization => new[] { "plain period depiction", "restrained dramatic shaping", "directed period stylization", "severe formal shaping", "austere thriller stylization" },
            Realism => new[] { "omit explicit realism", "plausible period realism", "credible institutional realism", "highly convincing historical realism", "exacting production realism" },
            TextureDepth => new[] { "sparse surface description", "light wear marks", "clear cloth, paper, wood, and glass texture", "rich analog material detail", "deeply worked tactile wear" },
            NarrativeDensity => new[] { "isolated scene fact", "light situational context", "layered room-and-role cues", "dense procedural setting", "fully networked social context" },
            Symbolism => new[] { "mostly literal", "subtle authority cues", "suggestive civic motifs", "surveillance allegory", "historical-state symbolism" },
            SurfaceAge => new[] { "well-kept surfaces", "light handling marks", "ordinary office wear", "tired public-room patina", "prolonged state-use wear" },
            Framing => new[] { "tight document framing", "close room framing", "settled scene framing", "broad spatial framing", "detached observational framing" },
            BackgroundComplexity => new[] { "bare setting support", "limited spatial detail", "supporting room or street detail", "layered civic or housing-block detail", "densely observed institutional environment" },
            MotionEnergy => new[] { "still observation", "restrained movement", "active procedural motion", "urgent human motion", "rapid movement under control" },
            FocusDepth => new[] { "narrow focal isolation", "subject-led focus", "even room focus", "broad interior clarity", "front-to-back clarity" },
            ImageCleanliness => new[] { "raw handling character", "modest roughness", "orderly print finish", "controlled period finish", "exact technical finish" },
            DetailDensity => new[] { "sparse factual detail", "selective scene detail", "descriptive object detail", "dense paper-and-equipment detail", "exhaustive scene-inventory detail" },
            AtmosphericDepth => new[] { "flat enclosed air", "slight spatial recession", "dim air-filled depth", "layered smoke or winter air", "deep air-separated recession" },
            Chaos => new[] { "ordered placement", "unsettled order", "procedural strain", "frayed public order", "failing room order" },
            Whimsy => new[] { "sober tone", "faint human warmth", "mild everyday warmth", "ironic social lightness", "brittle satirical play" },
            Tension => new[] { "low dramatic pressure", "quiet suspicion", "visible interpersonal strain", "surveillance pressure", "acute state fear" },
            Awe => new[] { "human scale", "slight historical weight", "civic heaviness", "oppressive official scale", "overwhelming structural weight" },
            Saturation => new[] { "drained color", "low-chroma color", "restrained period color", "dense selective color", "severe accent color" },
            Contrast => new[] { "soft tonal spread", "subdued tonal separation", "measured dark-light structure", "hard room contrast", "severe lamp-shadow contrast" },
            Temperature => new[] { "cold gray cast", "cool stale air", "neutral mixed indoor balance", "worn tungsten warmth", "heated amber cast" },
            LightingIntensity => new[] { "dim practical light", "low room light", "ordinary interior light", "hard desk-lamp emphasis", "exposed lamp-and-shadow pressure" },
            CameraDistance => new[] { "intimate face distance", "near conversational distance", "room-scale distance", "hall-scale distance", "remote observer distance" },
            CameraAngle => new[] { "seated low angle", "modest low angle", "eye-level observation", "slight supervisory height", "overhead surveillance angle" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    private static string ResolveVintageBendUrbanCivilianPhrase(string sliderKey, int value)
    {
        return sliderKey switch
        {
            NarrativeDensity => MapBand(value, "isolated city fact", "light civilian context", "layered housing-and-street cues", "dense everyday urban context", "fully networked urban social life"),
            CameraDistance => MapBand(value, "intimate pedestrian distance", "near sidewalk distance", "street-scale distance", "courtyard or block-front distance", "detached urban observer distance"),
            BackgroundComplexity => MapBand(value, "bare urban support", "limited civic or housing detail", "supporting street or apartment-block detail", "layered public-and-domestic urban detail", "densely observed urban-civilian environment"),
            AtmosphericDepth => MapBand(value, "flat city air", "slight street recession", "cold air-filled urban depth", "layered winter haze or exhaust air", "deep block-to-street recession"),
            SurfaceAge => MapBand(value, "maintained urban surfaces", "light public wear", "ordinary housing-block wear", "tired civic-weathered surfaces", "prolonged concrete-and-paint fatigue"),
            MotionEnergy => MapBand(value, "still public presence", "restrained pedestrian movement", "active everyday street motion", "busy civilian flow", "compressed crowd-and-transit motion"),
            DetailDensity => MapBand(value, "sparse civic detail", "selective housing detail", "descriptive public-and-domestic object detail", "dense notice-board-and-street-furniture detail", "exhaustive urban scene inventory"),
            Tension => MapBand(value, "low social pressure", "quiet public caution", "visible civic unease", "watchful street-level pressure", "acute public-state pressure"),
            _ => string.Empty,
        };
    }

    private static string[] GetVintageBendUrbanCivilianGuideLabels(string sliderKey)
    {
        return sliderKey switch
        {
            NarrativeDensity => ["isolated city fact", "light civilian context", "layered housing-and-street cues", "dense everyday urban context", "fully networked urban social life"],
            CameraDistance => ["intimate pedestrian distance", "near sidewalk distance", "street-scale distance", "courtyard or block-front distance", "detached urban observer distance"],
            BackgroundComplexity => ["bare urban support", "limited civic or housing detail", "supporting street or apartment-block detail", "layered public-and-domestic urban detail", "densely observed urban-civilian environment"],
            AtmosphericDepth => ["flat city air", "slight street recession", "cold air-filled urban depth", "layered winter haze or exhaust air", "deep block-to-street recession"],
            SurfaceAge => ["maintained urban surfaces", "light public wear", "ordinary housing-block wear", "tired civic-weathered surfaces", "prolonged concrete-and-paint fatigue"],
            MotionEnergy => ["still public presence", "restrained pedestrian movement", "active everyday street motion", "busy civilian flow", "compressed crowd-and-transit motion"],
            DetailDensity => ["sparse civic detail", "selective housing detail", "descriptive public-and-domestic object detail", "dense notice-board-and-street-furniture detail", "exhaustive urban scene inventory"],
            Tension => ["low social pressure", "quiet public caution", "visible civic unease", "watchful street-level pressure", "acute public-state pressure"],
            _ => Array.Empty<string>(),
        };
    }

    private static string ApplyVintageBendGuardrails(string sliderKey, int bandIndex, PromptConfiguration configuration, string phrase)
    {
        if (string.Equals(sliderKey, Temperature, StringComparison.OrdinalIgnoreCase) && bandIndex == 4)
        {
            return configuration.SurfaceAge >= 60 || configuration.TextureDepth >= 60
                ? "worn tungsten warmth"
                : phrase;
        }

        if (string.Equals(sliderKey, SurfaceAge, StringComparison.OrdinalIgnoreCase) && bandIndex == 4)
        {
            return configuration.TextureDepth >= 60 || configuration.Temperature >= 60
                ? "tired public-room patina"
                : phrase;
        }

        if (string.Equals(sliderKey, TextureDepth, StringComparison.OrdinalIgnoreCase) && bandIndex == 4)
        {
            return configuration.SurfaceAge >= 60 || configuration.Temperature >= 60
                ? "rich analog material detail"
                : phrase;
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && bandIndex == 4)
        {
            return configuration.Temperature >= 60 || configuration.SurfaceAge >= 60
                ? "dense selective color"
                : phrase;
        }

        return phrase;
    }

    private static void AddVintageDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }
}
