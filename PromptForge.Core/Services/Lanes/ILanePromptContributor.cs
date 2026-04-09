using PromptForge.App.Models;

namespace PromptForge.App.Services.Lanes;

public interface ILanePromptContributor
{
    string IntentName { get; }

    IEnumerable<PromptFragment> BuildEarlyDescriptors(PromptConfiguration configuration);
}
