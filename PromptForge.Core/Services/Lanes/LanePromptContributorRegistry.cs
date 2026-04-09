namespace PromptForge.App.Services.Lanes;

internal static class LanePromptContributorRegistry
{
    private static readonly IReadOnlyList<ILanePromptContributor> Contributors =
    [
        ComicBookLane.Instance,
        FantasyIllustrationLane.Instance,
        EditorialIllustrationLane.Instance,
        GraphicDesignLane.Instance,
        TattooArtLane.Instance,
    ];

    public static bool TryGet(string? intentName, out ILanePromptContributor contributor)
    {
        contributor = Contributors.FirstOrDefault(item => string.Equals(item.IntentName, intentName, StringComparison.OrdinalIgnoreCase))!;
        return contributor is not null;
    }

}
