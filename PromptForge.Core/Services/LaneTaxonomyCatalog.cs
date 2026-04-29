using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class LaneTaxonomyCatalog
{
    private static readonly IReadOnlyDictionary<string, LaneTaxonomyMetadata> ByLaneId =
        new Dictionary<string, LaneTaxonomyMetadata>(StringComparer.OrdinalIgnoreCase)
        {
            ["vintage-bend"] = new("vintage-bend", "Photography", "Locked lane-local lane"),
            ["anime"] = new("anime", "Illustration", "Lane-local resolver lane"),
            ["childrens-book"] = new("childrens-book", "Illustration", "Lane-local resolver lane"),
            ["comic-book"] = new("comic-book", "Illustration", "Contributor-owned lane"),
            ["cinematic"] = new("cinematic", "Digital Render", "Shared-standard lane"),
            ["photography"] = new("photography", "Photography", "Lane-local resolver lane"),
            ["product-photography"] = new("product-photography", "Photography", "Locked lane-local lane"),
            ["food-photography"] = new("food-photography", "Photography", "Locked lane-local lane"),
            ["lifestyle-advertising-photography"] = new("lifestyle-advertising-photography", "Photography", "Locked lane-local lane"),
            ["architecture-archviz"] = new("architecture-archviz", "Photography", "Lane-local resolver lane"),
            ["3d-render"] = new("3d-render", "Digital Render", "Lane-local resolver lane"),
            ["concept-art"] = new("concept-art", "Digital Render", "Lane-local resolver lane"),
            ["pixel-art"] = new("pixel-art", "Digital Render", "Lane-local resolver lane"),
            ["fantasy-illustration"] = new("fantasy-illustration", "Illustration", "Contributor-owned lane"),
            ["editorial-illustration"] = new("editorial-illustration", "Illustration", "Contributor-owned lane"),
            ["graphic-design"] = new("graphic-design", "Design", "Contributor-owned lane"),
            ["infographic-data-visualization"] = new("infographic-data-visualization", "Design", "Lane-local resolver lane"),
            ["tattoo-art"] = new("tattoo-art", "Illustration", "Contributor-owned lane"),
            ["watercolor"] = new("watercolor", "Illustration", "Lane-local resolver lane"),
        };

    public static bool TryGetByLaneId(string? laneId, out LaneTaxonomyMetadata metadata)
    {
        if (!string.IsNullOrWhiteSpace(laneId) && ByLaneId.TryGetValue(laneId.Trim(), out metadata!))
        {
            return true;
        }

        metadata = null!;
        return false;
    }

    public static bool TryGetByIntentName(string? intentName, out LaneTaxonomyMetadata metadata)
    {
        metadata = null!;

        var lane = LaneRegistry.GetByIntentName(intentName);
        return lane is not null && TryGetByLaneId(lane.Id, out metadata);
    }
}

public sealed record LaneTaxonomyMetadata(
    string LaneId,
    string Category,
    string BehaviorProfile);
