using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveEditorialIllustrationPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetEditorialIllustrationBandLabels(sliderKey);
        return labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);
    }

    public static string ResolveEditorialIllustrationGuideText(string sliderKey)
    {
        var labels = GetEditorialIllustrationBandLabels(sliderKey);
        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveEditorialIllustrationDescriptors(PromptConfiguration configuration)
    {
        yield return "editorial illustration";
    }

    public static IEnumerable<string> ResolveEditorialIllustrationOverlayDescriptors(PromptConfiguration configuration)
    {
        if (configuration.EditorialIllustrationBlackAndWhiteMonochrome)
        {
            yield return "black-and-white monochrome treatment";
        }
    }

    private static string[] GetEditorialIllustrationBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["plainly observed illustration treatment", "light illustrative shaping", "editorial illustration treatment", "strong stylized shaping", "highly stylized concept-forward illustration"],
            Realism => ["omit explicit realism", "loosely observed illustration logic", "moderately realistic illustrative treatment", "high visual illustrative realism", "strongly realistic rendered finish"],
            TextureDepth => ["minimal surface articulation", "light printed-surface texture", "clear illustrative surface definition", "rich tactile illustration detail", "deeply worked publication-surface richness"],
            NarrativeDensity => ["single-read visual idea", "light article-story suggestion", "layered storytelling cues", "dense feature-story implication", "world-aware longform narrative"],
            Symbolism => ["mostly literal read", "subtle metaphor cues", "clear symbolic framing", "pronounced allegorical construction", "high-concept symbolic charge"],
            Framing => ["intimate crop logic", "focused subject framing", "balanced article-illustration framing", "broad feature-spread framing", "expansive cover-or-spread framing"],
            CameraDistance => ["extreme close conceptual crop", "close subject emphasis", "mid-distance idea staging", "wider scene read", "far-set cover-scene distance"],
            CameraAngle => ["low-angle emphasis", "slight low-angle pressure", "eye-level conceptual read", "slightly high analytic vantage", "high overview vantage"],
            BackgroundComplexity => ["minimal background field", "restrained supporting backdrop", "supporting contextual environment", "rich contextual setting", "densely layered article-world context"],
            MotionEnergy => ["still conceptual staging", "gentle implied movement", "active idea-driven energy", "dynamic visual momentum", "high-velocity visual argument"],
            AtmosphericDepth => ["flat poster-like depth", "slight spatial recession", "air-shaped illustration depth", "layered feature-scene depth", "deep atmospheric staging"],
            Chaos => ["controlled visual order", "light conceptual tension", "active compositional friction", "orchestrated visual unrest", "high conceptual instability"],
            Whimsy => ["serious tone", "dry wit", "playful editorial wit", "strong whimsical commentary", "bold satirical play"],
            Tension => ["low tension", "light argumentative tension", "noticeable conceptual tension", "strong editorial pressure", "intense polemical tension"],
            Awe => ["grounded scale", "slight wonder", "atmosphere of significance", "strong sense of consequence", "overwhelming thematic grandeur"],
            LightingIntensity => ["subdued light presence", "soft restrained illumination", "balanced illustration lighting", "assertive feature-light emphasis", "bright attention-commanding illumination"],
            Saturation => ["muted print-safe color", "restrained palette control", "balanced editorial color", "rich magazine-color saturation", "vivid cover-ready color"],
            Contrast => ["soft tonal separation", "restrained tonal contrast", "balanced print contrast", "crisp graphic contrast", "bold cover-grade contrast"],
            FocusDepth => ["broad clarity across the frame", "mostly deep focus clarity", "balanced focus hierarchy", "selective subject emphasis", "strong focal isolation"],
            ImageCleanliness => ["raw mark-making character", "lightly refined finish", "balanced publication finish", "polished editorial finish", "ultra-clean cover finish"],
            DetailDensity => ["sparse conceptual detail", "restrained descriptive detail", "balanced illustrative detail", "rich article-detail layering", "dense publication-grade detail"],
            _ => [],
        };
    }
}
