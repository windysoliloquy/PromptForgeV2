using PromptForge.App.Models;

namespace PromptForge.App.Services;

public interface IPromptBuilderService
{
    PromptResult Build(PromptConfiguration configuration);
}
