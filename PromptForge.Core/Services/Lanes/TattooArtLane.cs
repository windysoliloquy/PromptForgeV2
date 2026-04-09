using PromptForge.App.Models;

namespace PromptForge.App.Services.Lanes;

internal sealed class TattooArtLane : ILanePromptContributor
{
    public static TattooArtLane Instance { get; } = new();

    public string IntentName => IntentModeCatalog.TattooArtName;

    private TattooArtLane()
    {
    }

    public IEnumerable<PromptFragment> BuildEarlyDescriptors(PromptConfiguration configuration)
    {
        foreach (var phrase in SliderLanguageCatalog.ResolveTattooArtDescriptors(configuration))
        {
            yield return new PromptFragment(phrase);
        }
    }
}
