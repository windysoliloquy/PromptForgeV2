using PromptForge.App.Models;

namespace PromptForge.App.Services.Lanes;

internal sealed class ComicBookLane : ILanePromptContributor
{
    public static ComicBookLane Instance { get; } = new();

    public string IntentName => IntentModeCatalog.ComicBookName;

    private ComicBookLane()
    {
    }

    public IEnumerable<PromptFragment> BuildEarlyDescriptors(PromptConfiguration configuration)
    {
        return SliderLanguageCatalog.ResolveComicBookDescriptors(configuration);
    }
}
