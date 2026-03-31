using PromptForge.App.Models;

namespace PromptForge.App.Services;

public interface IDemoStateService
{
    DemoState CurrentState { get; }
    bool TryConsumeCopy(out DemoState state);
}
