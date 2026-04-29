using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetGraphicDesignSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsGraphicDesign(configuration.IntentMode))
        {
            yield break;
        }

        var fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
        {
            (0, 0) => "grounded flat graphic treatment",
            (0, 1) => "grounded lightly observed rendering",
            (0, 2) => "grounded believable rendering",
            (0, 3) => "grounded polished realism",
            (0, 4) => "grounded high-fidelity rendering",

            (1, 0) => "lightly stylized flat treatment",
            (1, 1) => "lightly stylized grounded rendering",
            (1, 2) => "lightly stylized believable rendering",
            (1, 3) => "lightly stylized polished realism",
            (1, 4) => "lightly stylized high-fidelity realism",

            (2, 0) => "stylized graphic rendering",
            (2, 1) => "stylized with grounded logic",
            (2, 2) => "stylized believable rendering",
            (2, 3) => "stylized polished realism",
            (2, 4) => "stylized high-fidelity realism",

            (3, 0) => "bold graphic abstraction",
            (3, 1) => "bold grounded stylization",
            (3, 2) => "bold believable stylization",
            (3, 3) => "bold polished stylization",
            (3, 4) => "bold high-fidelity stylization",

            (4, 0) => "high-authorial abstraction",
            (4, 1) => "high-authorial grounded abstraction",
            (4, 2) => "high-authorial believable stylization",
            (4, 3) => "high-authorial polished stylization",
            (4, 4) => "high-authorial realism illusion",
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

        fusedPhrase = (GetBandIndex(configuration.Whimsy), GetBandIndex(configuration.Tension)) switch
        {
            (0, 0) => "austere visual calm",
            (0, 1) => "austere visual restraint",
            (0, 2) => "serious design pressure",
            (0, 3) => "severe urgency cue",
            (0, 4) => "severe attention pressure",

            (1, 0) => "light visual ease",
            (1, 1) => "light tonal restraint",
            (1, 2) => "light promotional pressure",
            (1, 3) => "lively urgency cue",
            (1, 4) => "brisk attention pull",

            (2, 0) => "playful visual ease",
            (2, 1) => "playful visual friction",
            (2, 2) => "spirited design pressure",
            (2, 3) => "spirited urgency cue",
            (2, 4) => "high-energy attention pull",

            (3, 0) => "bold graphic play",
            (3, 1) => "bold visual friction",
            (3, 2) => "charged graphic pressure",
            (3, 3) => "charged urgency cue",
            (3, 4) => "aggressive attention pull",

            (4, 0) => "exuberant graphic play",
            (4, 1) => "exuberant visual friction",
            (4, 2) => "explosive design pressure",
            (4, 3) => "explosive urgency cue",
            (4, 4) => "explosive attention pressure",
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

        fusedPhrase = (GetBandIndex(configuration.Awe), GetBandIndex(configuration.Contrast)) switch
        {
            (0, 0) => "grounded quiet presence",
            (0, 1) => "grounded tonal lift",
            (0, 2) => "grounded graphic presence",
            (0, 3) => "grounded visual snap",
            (0, 4) => "grounded high-contrast impact",

            (1, 0) => "lifted quiet presence",
            (1, 1) => "lifted tonal spark",
            (1, 2) => "lifted graphic presence",
            (1, 3) => "lifted visual snap",
            (1, 4) => "lifted contrast punch",

            (2, 0) => "wonder-marked presence",
            (2, 1) => "vivid tonal lift",
            (2, 2) => "vivid graphic presence",
            (2, 3) => "striking visual punch",
            (2, 4) => "striking high-contrast impact",

            (3, 0) => "grand display presence",
            (3, 1) => "grand tonal spark",
            (3, 2) => "commanding graphic presence",
            (3, 3) => "commanding visual punch",
            (3, 4) => "commanding high-contrast impact",

            (4, 0) => "overwhelming display presence",
            (4, 1) => "overwhelming tonal spark",
            (4, 2) => "overwhelming graphic presence",
            (4, 3) => "overwhelming visual punch",
            (4, 4) => "overwhelming high-contrast impact",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Awe,
            configuration.Awe,
            Contrast,
            configuration.Contrast,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }
    }
}
