using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetLifestyleAdvertisingPhotographySemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsLifestyleAdvertisingPhotography(configuration.IntentMode))
        {
            yield break;
        }
    }
}
