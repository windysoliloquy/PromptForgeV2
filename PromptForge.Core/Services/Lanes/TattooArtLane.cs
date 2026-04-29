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
        return SliderLanguageCatalog.ResolveTattooArtDescriptors(configuration)
            .Select(static phrase => new PromptFragment(phrase));
    }
}
