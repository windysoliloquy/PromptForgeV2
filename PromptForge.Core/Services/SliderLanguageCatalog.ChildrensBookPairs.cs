using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetChildrensBookSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsChildrensBook(configuration.IntentMode))
        {
            yield break;
        }

        var fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
        {
            (0, 0) => "grounded picture-book simplicity",
            (0, 1) => "grounded page drawing",
            (0, 2) => "grounded convincing page illustration",
            (0, 3) => "grounded believable page illustration",
            (0, 4) => "grounded refined page illustration",

            (1, 0) => "lightly stylized picture-book simplicity",
            (1, 1) => "lightly stylized grounded page drawing",
            (1, 2) => "lightly stylized convincing page illustration",
            (1, 3) => "lightly stylized believable page illustration",
            (1, 4) => "lightly stylized refined page illustration",

            (2, 0) => "expressive picture-book simplification",
            (2, 1) => "expressive grounded page drawing",
            (2, 2) => "expressive convincing page illustration",
            (2, 3) => "expressive believable page illustration",
            (2, 4) => "expressive refined page illustration",

            (3, 0) => "richly authored picture-book simplification",
            (3, 1) => "richly authored grounded page drawing",
            (3, 2) => "richly authored convincing page illustration",
            (3, 3) => "richly authored believable page illustration",
            (3, 4) => "richly authored refined page illustration",

            (4, 0) => "boldly stylized picture-book simplification",
            (4, 1) => "boldly stylized grounded page drawing",
            (4, 2) => "boldly stylized convincing page illustration",
            (4, 3) => "boldly stylized believable page illustration",
            (4, 4) => "boldly stylized refined page illustration",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Stylization,
            configuration.Stylization,
            Realism,
            configuration.Realism,
            fusedPhrase,
            out var collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.NarrativeDensity), GetBandIndex(configuration.BackgroundComplexity)) switch
        {
            (0, 0) => "self-contained picture-book tableau",
            (0, 1) => "lightly situated page tableau",
            (0, 2) => "anchored tableau with supporting world cues",
            (0, 3) => "fully situated story tableau",
            (0, 4) => "world-set tableau with dense surroundings",

            (1, 0) => "gentle narrative beat on an open page",
            (1, 1) => "lightly staged narrative beat",
            (1, 2) => "narrative beat with supporting setting detail",
            (1, 3) => "clearly staged beat in a developed setting",
            (1, 4) => "narrative beat nested in a dense story world",

            (2, 0) => "layered page-turn cueing on a spare page",
            (2, 1) => "layered cueing with light setting support",
            (2, 2) => "layered page-turn staging",
            (2, 3) => "richly staged narrative progression",
            (2, 4) => "layered progression through a dense story world",

            (3, 0) => "story-laden scene on an open page",
            (3, 1) => "story-laden scene with guided setting cues",
            (3, 2) => "story-laden scene with supporting world detail",
            (3, 3) => "richly situated storytelling",
            (3, 4) => "expansive storytelling across a dense story world",

            (4, 0) => "tale-bearing image with open surroundings",
            (4, 1) => "tale-bearing scene with light world cues",
            (4, 2) => "world-bearing narrative scene",
            (4, 3) => "fully realized story-world staging",
            (4, 4) => "densely layered story-world sweep",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            NarrativeDensity,
            configuration.NarrativeDensity,
            BackgroundComplexity,
            configuration.BackgroundComplexity,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.TextureDepth), GetBandIndex(configuration.ImageCleanliness)) switch
        {
            (0, 0) => "bare paper in a rough working state",
            (0, 1) => "bare page with loose sketch finish",
            (0, 2) => "clean page with faint paper presence",
            (0, 3) => "polished page with restrained material trace",
            (0, 4) => "pristine page with nearly smoothed texture",

            (1, 0) => "soft grain in a rough working state",
            (1, 1) => "softly grained sketch finish",
            (1, 2) => "soft grain with readable page finish",
            (1, 3) => "polished page with soft grain held intact",
            (1, 4) => "pristine finish over gentle paper grain",

            (2, 0) => "visible texture in a rough working state",
            (2, 1) => "visible paper texture with loose finish",
            (2, 2) => "readable paper-and-pigment texture",
            (2, 3) => "polished page with visible material texture",
            (2, 4) => "pristine finish with visible paper texture",

            (3, 0) => "tactile page richness in rough form",
            (3, 1) => "tactile richness with sketch-loose handling",
            (3, 2) => "tactile page richness with readable finish",
            (3, 3) => "polished page with tactile richness",
            (3, 4) => "refined finish over tactile page richness",

            (4, 0) => "deeply worked texture in raw form",
            (4, 1) => "deeply worked texture with loose handling",
            (4, 2) => "deeply worked texture with clear page finish",
            (4, 3) => "fully polished richly worked page",
            (4, 4) => "heirloom-clean finish over deeply worked texture",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            TextureDepth,
            configuration.TextureDepth,
            ImageCleanliness,
            configuration.ImageCleanliness,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Chaos)) switch
        {
            (0, 0) => "still and carefully ordered page staging",
            (0, 1) => "still scene with a faint unsettled stir",
            (0, 2) => "still scene with busy visual shuffle",
            (0, 3) => "still scene amid playful commotion",
            (0, 4) => "still focal moment inside visual mayhem",

            (1, 0) => "gentle motion in an orderly scene",
            (1, 1) => "soft movement with light visual unrest",
            (1, 2) => "soft movement through busy scene activity",
            (1, 3) => "gentle motion through lively commotion",
            (1, 4) => "gentle movement inside playful mayhem",

            (2, 0) => "active scene motion with clear visual order",
            (2, 1) => "active motion with mild scene unrest",
            (2, 2) => "active motion through bustling scene disorder",
            (2, 3) => "active motion through rollicking commotion",
            (2, 4) => "active motion driving visual mayhem",

            (3, 0) => "lively movement with controlled staging",
            (3, 1) => "lively movement with visible unruliness",
            (3, 2) => "lively movement through busy disorder",
            (3, 3) => "lively movement in full page commotion",
            (3, 4) => "lively movement across orchestrated mayhem",

            (4, 0) => "sweeping motion with disciplined page control",
            (4, 1) => "sweeping motion with spirited visual unrest",
            (4, 2) => "sweeping motion through bustling disorder",
            (4, 3) => "sweeping motion through exuberant commotion",
            (4, 4) => "sweeping motion across storybook mayhem",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            MotionEnergy,
            configuration.MotionEnergy,
            Chaos,
            configuration.Chaos,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.Whimsy), GetBandIndex(configuration.Tension)) switch
        {
            (0, 0) => "plainspoken page calm",
            (0, 1) => "serious tone with a slight dramatic pull",
            (0, 2) => "serious tone under clear suspense",
            (0, 3) => "measured gravity in child-safe peril",
            (0, 4) => "solemn page peril at full stakes",

            (1, 0) => "quiet charm at rest",
            (1, 1) => "quiet charm with a light dramatic pull",
            (1, 2) => "quiet charm under gentle suspense",
            (1, 3) => "quiet charm meeting child-safe peril",
            (1, 4) => "quiet charm against looming story stakes",

            (2, 0) => "playful ease",
            (2, 1) => "playful charm with light story friction",
            (2, 2) => "playful charm under lively suspense",
            (2, 3) => "playful courage in child-safe peril",
            (2, 4) => "playful spirit facing high story stakes",

            (3, 0) => "buoyant whimsical ease",
            (3, 1) => "buoyant whimsy with teasing dramatic pull",
            (3, 2) => "buoyant whimsy under bright suspense",
            (3, 3) => "buoyant whimsy brushing against peril",
            (3, 4) => "buoyant whimsy beneath towering stakes",

            (4, 0) => "bold fanciful exuberance",
            (4, 1) => "bold fanciful energy with dramatic strain",
            (4, 2) => "bold fanciful energy under gathering suspense",
            (4, 3) => "fanciful bravado in child-safe peril",
            (4, 4) => "boldly fanciful force at page-scale stakes",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Whimsy,
            configuration.Whimsy,
            Tension,
            configuration.Tension,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.Awe), GetBandIndex(configuration.AtmosphericDepth)) switch
        {
            (0, 0) => "grounded page presence",
            (0, 1) => "grounded scene with light recession",
            (0, 2) => "grounded scene in airy space",
            (0, 3) => "grounded scene with luminous depth",
            (0, 4) => "grounded scene held in enveloping atmosphere",

            (1, 0) => "quiet marvel on a near page",
            (1, 1) => "quiet marvel with gentle recession",
            (1, 2) => "quiet marvel in airy depth",
            (1, 3) => "quiet marvel through luminous depth",
            (1, 4) => "quiet marvel held in enveloping atmosphere",

            (2, 0) => "hushed wonder in close page space",
            (2, 1) => "hushed wonder with soft recession",
            (2, 2) => "hushed wonder in airy depth",
            (2, 3) => "hushed wonder through luminous depth",
            (2, 4) => "hushed wonder inside enveloping atmosphere",

            (3, 0) => "reverent imaginative lift on a near page",
            (3, 1) => "reverent lift with opening recession",
            (3, 2) => "reverent lift through airy depth",
            (3, 3) => "reverent lift through luminous layering",
            (3, 4) => "reverent lift within enveloping atmosphere",

            (4, 0) => "sweeping childlike grandeur at close range",
            (4, 1) => "sweeping childlike grandeur with opening depth",
            (4, 2) => "sweeping childlike grandeur through airy distance",
            (4, 3) => "sweeping childlike grandeur through luminous space",
            (4, 4) => "sweeping childlike grandeur in enveloping atmosphere",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Awe,
            configuration.Awe,
            AtmosphericDepth,
            configuration.AtmosphericDepth,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }
    }
}
