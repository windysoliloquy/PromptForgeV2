using PromptForge.App.Models;

namespace PromptForge.App.Services.Lanes;

public interface ILanePresentationOverlayProvider
{
    IEnumerable<PromptFragment> BuildPresentationOverlayDescriptors(PromptConfiguration configuration);
}
