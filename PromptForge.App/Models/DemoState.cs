using PromptForge.App.Services;

namespace PromptForge.App.Models;

public sealed class DemoState
{
    public int MaxCopies { get; set; } = DemoModeOptions.MaxDemoCopies;
    public int RemainingCopies { get; set; } = DemoModeOptions.MaxDemoCopies;
}
