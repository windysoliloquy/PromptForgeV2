using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetProductPhotographySemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsProductPhotography(configuration.IntentMode))
        {
            yield break;
        }
    }
}
