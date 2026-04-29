using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetThreeDRenderSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsThreeDRender(configuration.IntentMode))
        {
            yield break;
        }
    }
}
