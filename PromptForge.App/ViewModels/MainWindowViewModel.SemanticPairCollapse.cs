using PromptForge.App.Models;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel
{
    private static string ApplySemanticPairCollapse(string prompt, PromptConfiguration configuration)
    {
        return PromptSemanticPairCollapseService.Apply(prompt, configuration);
    }
}
