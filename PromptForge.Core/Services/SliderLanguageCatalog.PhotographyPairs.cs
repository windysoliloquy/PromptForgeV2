using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetPhotographySemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsPhotography(configuration.IntentMode))
        {
            yield break;
        }
    }
}
