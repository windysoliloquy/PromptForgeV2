using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetPixelArtSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsPixelArt(configuration.IntentMode))
        {
            yield break;
        }
    }
}
