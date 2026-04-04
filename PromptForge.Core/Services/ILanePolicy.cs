using PromptForge.App.Models;

namespace PromptForge.App.Services;

public interface ILanePolicy
{
    PromptConfiguration ApplyDefaults(PromptConfiguration configuration, LaneDefinition lane);
}
