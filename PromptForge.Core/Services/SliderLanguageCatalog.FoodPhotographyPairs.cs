using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetFoodPhotographySemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsFoodPhotography(configuration.IntentMode))
        {
            yield break;
        }
    }
}
