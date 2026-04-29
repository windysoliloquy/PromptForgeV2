using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetTattooArtSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsTattooArt(configuration.IntentMode))
        {
            yield break;
        }
    }
}
