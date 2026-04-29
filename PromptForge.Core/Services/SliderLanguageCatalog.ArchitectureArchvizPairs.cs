using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetArchitectureArchvizSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsArchitectureArchviz(configuration.IntentMode))
        {
            yield break;
        }
    }
}
