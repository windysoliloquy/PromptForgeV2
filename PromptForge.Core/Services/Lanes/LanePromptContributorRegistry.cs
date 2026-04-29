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
    private static readonly IReadOnlyDictionary<string, ILanePromptContributor> ContributorsByIntentName =
        Contributors.ToDictionary(static contributor => contributor.IntentName, StringComparer.OrdinalIgnoreCase);

    public static bool TryGet(string? intentName, out ILanePromptContributor contributor)
    {
        if (!string.IsNullOrWhiteSpace(intentName) && ContributorsByIntentName.TryGetValue(intentName, out contributor!))
        {
            return true;
        }

        contributor = null!;
        return false;
    }

}
