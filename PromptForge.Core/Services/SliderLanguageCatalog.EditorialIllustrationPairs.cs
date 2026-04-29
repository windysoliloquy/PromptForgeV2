using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetEditorialIllustrationSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsEditorialIllustration(configuration.IntentMode))
        {
            yield break;
        }
    }
}
