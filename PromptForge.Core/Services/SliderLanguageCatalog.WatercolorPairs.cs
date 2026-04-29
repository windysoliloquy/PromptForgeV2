using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetWatercolorSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsWatercolor(configuration.IntentMode))
        {
            yield break;
        }

        var fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
        {
            (0, 0) => "grounded loose washwork",
            (0, 1) => "grounded representational watercolor",
            (0, 2) => "grounded observed watercolor",
            (0, 3) => "grounded finely observed watercolor",
            (0, 4) => "grounded highly resolved watercolor",

            (1, 0) => "lightly stylized loose washwork",
            (1, 1) => "lightly stylized representational watercolor",
            (1, 2) => "lightly stylized observed watercolor",
            (1, 3) => "lightly stylized finely observed watercolor",
            (1, 4) => "lightly stylized highly resolved watercolor",

            (2, 0) => "expressive loose washwork",
            (2, 1) => "expressive representational watercolor",
            (2, 2) => "expressive observed watercolor",
            (2, 3) => "expressive finely observed watercolor",
            (2, 4) => "expressive highly resolved watercolor",

            (3, 0) => "painterly wash shaping",
            (3, 1) => "painterly representational watercolor",
            (3, 2) => "painterly observed watercolor",
            (3, 3) => "painterly finely observed watercolor",
            (3, 4) => "painterly highly resolved watercolor",

            (4, 0) => "lyrical wash abstraction",
            (4, 1) => "lyrical figurative watercolor",
            (4, 2) => "lyrical observed watercolor",
            (4, 3) => "lyrical finely observed watercolor",
            (4, 4) => "lyrical highly resolved watercolor",
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
    }
}
