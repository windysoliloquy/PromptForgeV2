using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveProductPhotographyPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetProductPhotographyBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyProductPhotographyGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveProductPhotographyGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetProductPhotographyBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveProductPhotographyDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddProductPhotographyDescriptor(phrases, seen, "product photography");

        var shotTypeDescriptor = ResolveProductPhotographyShotTypeDescriptor(configuration.ProductPhotographyShotType);
        if (!string.IsNullOrWhiteSpace(shotTypeDescriptor))
        {
            AddProductPhotographyDescriptor(phrases, seen, shotTypeDescriptor);
        }

        foreach (var phrase in ResolveProductPhotographyModifierDescriptors(configuration))
        {
            AddProductPhotographyDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveProductPhotographyLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "clean studio daylight",
            "Golden hour" => "warm merchandising light",
            "Dramatic studio light" => "controlled studio light",
            "Overcast" => "diffused studio light",
            "Moonlit" => "cool showroom light",
            "Soft glow" => "soft display glow",
            "Dusk haze" => "late-day showcase light",
            "Warm directional light" => "directional product light",
            "Volumetric cinematic light" => "layered display light",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveProductPhotographyShotTypeDescriptor(string shotType)
    {
        return shotType switch
        {
            "packshot" => "studio packshot",
            "hero-studio" => "premium hero studio",
            "editorial-still-life" => "editorial still life",
            "macro-detail" => "macro product detail",
            "lifestyle-placement" => "lifestyle placement",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveProductPhotographyModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetProductPhotographyModifierPriority(configuration.ProductPhotographyShotType);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 3)
            {
                break;
            }

            var phrase = key switch
            {
                "With Packaging" when configuration.ProductPhotographyWithPackaging => "packaging in frame",
                "Pedestal Display" when configuration.ProductPhotographyPedestalDisplay => "pedestal display staging",
                "Reflective Surface" when configuration.ProductPhotographyReflectiveSurface => "reflective display surface",
                "Floating Presentation" when configuration.ProductPhotographyFloatingPresentation => "floating product presentation",
                "Hand Scale Cue" when configuration.ProductPhotographyScaleCueHand => "hand-in-frame scale cue",
                "Brand Props" when configuration.ProductPhotographyBrandProps => "brand-matched supporting props",
                "Grouped Variants" when configuration.ProductPhotographyGroupedVariants => "grouped variant arrangement",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetProductPhotographyModifierPriority(string shotType)
    {
        return shotType switch
        {
            "hero-studio" => ["Pedestal Display", "Reflective Surface", "With Packaging", "Floating Presentation", "Hand Scale Cue", "Brand Props", "Grouped Variants"],
            "editorial-still-life" => ["Brand Props", "Pedestal Display", "With Packaging", "Reflective Surface", "Grouped Variants", "Hand Scale Cue", "Floating Presentation"],
            "macro-detail" => ["Reflective Surface", "Hand Scale Cue", "With Packaging", "Pedestal Display", "Brand Props", "Grouped Variants", "Floating Presentation"],
            "lifestyle-placement" => ["Hand Scale Cue", "With Packaging", "Grouped Variants", "Brand Props", "Pedestal Display", "Reflective Surface", "Floating Presentation"],
            _ => ["With Packaging", "Pedestal Display", "Reflective Surface", "Floating Presentation", "Hand Scale Cue", "Brand Props", "Grouped Variants"],
        };
    }

    private static void AddProductPhotographyDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyProductPhotographyGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "immaculate product clarity";
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "vivid merchandise color";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking showcase contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "deep product depth";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return "densely layered retail surroundings";
        }

        if (string.Equals(sliderKey, MotionEnergy, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Framing <= 40)
        {
            return "dynamic placement energy";
        }

        return phrase;
    }

    private static string[] GetProductPhotographyBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var shotType = configuration.ProductPhotographyShotType;
        return sliderKey switch
        {
            Stylization => MapProductPhotographyStylization(shotType),
            Realism => ["clean merchandise read", "lightly interpreted product presence", "material fidelity", "high-fidelity merchandise realism", "deeply convincing sellable realism"],
            TextureDepth => ["smooth surface read", "faint tactile grain", "surface tactility", "rich material texture", "deep material body"],
            NarrativeDensity => shotType switch
            {
                "editorial-still-life" => ["low-story commercial simplicity", "light styling implication", "editorial prop styling", "layered editorial context", "dense editorial storytelling"],
                "lifestyle-placement" => ["low-story commercial simplicity", "light situational story", "contextual product-use placement", "layered use-case context", "dense commerce storytelling"],
                "macro-detail" => ["low-story commercial simplicity", "light material implication", "implied material story", "layered detail context", "dense material storytelling"],
                "hero-studio" => ["low-story commercial simplicity", "light display implication", "implied showcase story", "layered merchandising context", "dense premium storytelling"],
                _ => ["low-story commercial simplicity", "light product implication", "implied product story", "layered merchandising context", "dense commerce storytelling"],
            },
            Symbolism => shotType switch
            {
                "macro-detail" => ["minimal symbolic load", "restrained material cue", "suggestive texture motif", "pronounced material symbolism", "editorial material allegory"],
                "lifestyle-placement" => ["minimal symbolic load", "restrained lifestyle cue", "suggestive use-case motif", "pronounced merchandising symbolism", "editorial brand allegory"],
                _ => ["minimal symbolic load", "restrained brand cue", "suggestive merchandising motif", "pronounced brand symbolism", "editorial brand allegory"],
            },
            SurfaceAge => shotType switch
            {
                "hero-studio" => ["fresh hero surface", "faint handling wear", "gentle showroom patina", "noticeable display wear", "time-softened hero character"],
                "editorial-still-life" => ["fresh display surface", "faint shelf wear", "gentle handling patina", "noticeable retail patina", "time-softened still-life character"],
                "macro-detail" => ["fresh material surface", "faint edge wear", "gentle handling patina", "noticeable material wear", "time-softened detail character"],
                "lifestyle-placement" => ["fresh use surface", "faint handling wear", "gentle lived-in patina", "noticeable placement wear", "time-softened use character"],
                _ => ["fresh stock surface", "faint shelf wear", "gentle handling patina", "noticeable retail patina", "time-softened product character"],
            },
            Framing => shotType switch
            {
                "packshot" => ["centered product read", "clean product presentation", "copy-space discipline", "broader merchandising framing", "expansive catalog staging"],
                "hero-studio" => ["centered hero framing", "clean premium presentation", "premium display staging", "broader hero staging", "expansive hero staging"],
                "editorial-still-life" => ["centered object framing", "clean still-life crop", "measured editorial staging", "broader environmental framing", "expansive editorial staging"],
                "macro-detail" => ["tight detail crop", "close material presentation", "measured detail framing", "broader material framing", "expansive detail staging"],
                "lifestyle-placement" => ["centered product placement", "clean placement framing", "measured contextual placement", "broader selling context", "expansive placement staging"],
                _ => ["centered product read", "clean product presentation", "copy-space discipline", "broader merchandising framing", "expansive catalog staging"],
            },
            BackgroundComplexity => shotType switch
            {
                "packshot" => ["catalog isolation", "restrained backdrop", "supporting environment", "contextual scene detail", "densely layered retail surroundings"],
                "hero-studio" => ["catalog isolation", "restrained stage", "supporting display environment", "contextual merchandising detail", "densely layered showroom surroundings"],
                "editorial-still-life" => ["catalog isolation", "restrained prop field", "styled supporting environment", "contextual styling detail", "densely layered still-life surroundings"],
                "macro-detail" => ["catalog isolation", "restrained surface field", "supporting material environment", "contextual detail texture", "densely layered detail surroundings"],
                "lifestyle-placement" => ["catalog isolation", "restrained use setting", "supporting environment in service of selling the item", "contextual merchandising detail", "densely layered retail surroundings"],
                _ => ["catalog isolation", "restrained backdrop", "supporting environment", "contextual merchandising detail", "densely layered retail surroundings"],
            },
            MotionEnergy => shotType switch
            {
                "hero-studio" => ["still hero presentation", "slight handling trace", "active display energy", "candid merchandising motion", "split-second commercial energy"],
                "editorial-still-life" => ["still styled presentation", "slight placement trace", "active composition energy", "candid editorial motion", "split-second still-life energy"],
                "macro-detail" => ["still detail presentation", "slight handling trace", "active material energy", "candid detail motion", "split-second close-up energy"],
                "lifestyle-placement" => ["still use presentation", "slight handling trace", "active placement energy", "candid lifestyle motion", "split-second commercial energy"],
                _ => ["still product presentation", "slight handling trace", "active placement energy", "candid merchandising motion", "split-second commercial energy"],
            },
            FocusDepth => shotType switch
            {
                "packshot" => ["broad product clarity", "light focus falloff", "disciplined full-product clarity", "selective product isolation", "sharp subject separation"],
                "macro-detail" => ["broad detail clarity", "light focus falloff", "selective detail emphasis", "tight material isolation", "razor-held detail separation"],
                _ => ["broad product clarity", "light focus falloff", "measured focus depth", "selective product isolation", "sharp subject separation"],
            },
            ImageCleanliness => shotType switch
            {
                "packshot" => ["raw studio texture", "slight surface grit", "commerce polish", "controlled commercial polish", "immaculate product clarity"],
                _ => ["raw studio texture", "slight surface grit", "measured print finish", "commerce polish", "immaculate product clarity"],
            },
            DetailDensity => shotType switch
            {
                "macro-detail" => ["sparse product detail", "light observed detail", "dense visible material information", "rich visual detail", "dense fine detail"],
                _ => ["sparse product detail", "light observed detail", "clear merchandise detail load", "rich visual detail", "dense fine detail"],
            },
            AtmosphericDepth => shotType switch
            {
                "editorial-still-life" => ["flat display space", "slight air separation", "richer spatial air", "luminous still-life depth", "deep styling depth"],
                "lifestyle-placement" => ["flat use space", "slight air separation", "natural spatial recession", "luminous placement depth", "deep use-context depth"],
                "hero-studio" => ["flat showcase space", "slight air separation", "visible display recession", "luminous product depth", "deep showroom depth"],
                "macro-detail" => ["flat detail space", "slight air separation", "visible material recession", "luminous material depth", "deep detail depth"],
                _ => ["flat studio space", "slight air separation", "visible display recession", "luminous product depth", "deep lens-carried depth"],
            },
            Chaos => ["low-chaos presentation control", "quiet restlessness", "scene instability", "loose merchandising volatility", "orchestrated visual disorder"],
            Whimsy => shotType switch
            {
                "hero-studio" => ["serious tone", "subtle brand lift", "elevated merchandising play", "warm retail looseness", "gentle showcase playfulness"],
                "editorial-still-life" => ["serious tone", "subtle brand lift", "casual styling play", "warm editorial looseness", "gentle still-life playfulness"],
                "macro-detail" => ["serious tone", "subtle material lift", "casual detail play", "warm tactile looseness", "gentle detail playfulness"],
                "lifestyle-placement" => ["serious tone", "subtle brand lift", "casual lifestyle play", "warm retail looseness", "gentle editorial playfulness"],
                _ => ["serious tone", "subtle brand lift", "casual merchandising play", "warm retail looseness", "gentle editorial playfulness"],
            },
            Tension => shotType switch
            {
                "hero-studio" => ["quiet commercial focus", "light buyer anticipation", "noticeable shelf tension", "strong merchandising pressure", "intense commercial tension"],
                "editorial-still-life" => ["quiet editorial focus", "light viewer anticipation", "noticeable display tension", "strong styling pressure", "intense editorial tension"],
                "macro-detail" => ["quiet material focus", "light viewer anticipation", "noticeable material tension", "strong detail pressure", "intense material tension"],
                "lifestyle-placement" => ["quiet commerce focus", "light buyer anticipation", "noticeable use tension", "strong merchandising pressure", "intense commercial tension"],
                _ => ["quiet commercial focus", "light buyer anticipation", "noticeable shelf tension", "strong merchandising pressure", "intense commercial tension"],
            },
            Awe => shotType switch
            {
                "hero-studio" => ["human-scale grounding", "slight sense of value", "slightly elevated grandeur", "strong showcase scale", "expansive hero grandeur"],
                "editorial-still-life" => ["human-scale grounding", "slight sense of value", "quiet editorial presence", "strong styled scale", "expansive composition grandeur"],
                "macro-detail" => ["human-scale grounding", "slight sense of value", "quiet material presence", "strong detail scale", "expansive material grandeur"],
                "lifestyle-placement" => ["human-scale grounding", "slight sense of value", "quiet use presence", "strong placement scale", "expansive merchandising grandeur"],
                _ => ["human-scale grounding", "slight sense of value", "quiet premium presence", "strong showcase scale", "expansive hero grandeur"],
            },
            Temperature => ["cool studio balance", "slightly cool neutrality", "neutral studio balance", "warm retail balance", "heated showcase warmth"],
            LightingIntensity => shotType switch
            {
                "hero-studio" => ["dim studio light", "soft studio light", "controlled studio brightness", "stronger studio drama", "radiant showcase light"],
                _ => ["dim studio light", "soft studio light", "controlled studio brightness", "bright showroom light", "radiant showcase light"],
            },
            Saturation => ["muted color", "restrained commercial color", "natural color balance", "rich product color", "vivid merchandise color"],
            Contrast => shotType switch
            {
                "hero-studio" => ["low studio contrast", "gentle tonal separation", "contour separation", "crisp showcase contrast", "striking commercial contrast"],
                _ => ["low studio contrast", "gentle tonal separation", "contour separation", "crisp showcase contrast", "striking commercial contrast"],
            },
            CameraDistance => shotType switch
            {
                "macro-detail" => ["isolated close detail", "close material study", "tight material crop", "mid-distance merchandising view", "far-set display view"],
                "hero-studio" => ["isolated close packshot", "close product view", "mid-distance hero view", "wide showcase view", "far-set display view"],
                "lifestyle-placement" => ["isolated close placement", "close product view", "mid-distance merchandising view", "wide context view", "far-set display view"],
                _ => ["isolated close packshot", "close product view", "mid-distance merchandising view", "wide showcase view", "far-set display view"],
            },
            CameraAngle => ["eye-level view", "slightly lowered viewpoint", "balanced product angle", "slightly elevated showcase angle", "high merchandising vantage"],
            _ => Array.Empty<string>(),
        };
    }

    private static string[] MapProductPhotographyStylization(string shotType)
    {
        return shotType switch
        {
            "editorial-still-life" => ["unembellished object presentation", "lightly art-directed arrangement", "slightly more art direction", "premium editorial polish", "fully art-directed still-life presentation"],
            "macro-detail" => ["unembellished detail presentation", "lightly art-directed material study", "controlled commercial styling", "premium material polish", "fully art-directed detail presentation"],
            "lifestyle-placement" => ["unembellished use presentation", "lightly art-directed placement", "controlled lifestyle styling", "premium merchandising polish", "fully art-directed use-case presentation"],
            "hero-studio" => ["unembellished product presentation", "lightly art-directed merchandising", "controlled commercial styling", "premium merchandising polish", "fully art-directed showcase presentation"],
            _ => ["unembellished catalog presentation", "lightly art-directed merchandising", "controlled commercial styling", "premium merchandising polish", "fully art-directed showcase presentation"],
        };
    }
}
