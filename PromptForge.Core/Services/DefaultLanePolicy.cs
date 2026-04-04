using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed class DefaultLanePolicy : ILanePolicy
{
    public static DefaultLanePolicy Instance { get; } = new();

    private DefaultLanePolicy()
    {
    }

    public PromptConfiguration ApplyDefaults(PromptConfiguration configuration, LaneDefinition lane)
    {
        lane.Defaults.ApplyTo(configuration);
        return configuration;
    }
}

public sealed class ComicBookLanePolicy : ILanePolicy
{
    public static ComicBookLanePolicy Instance { get; } = new();

    private ComicBookLanePolicy()
    {
    }

    public PromptConfiguration ApplyDefaults(PromptConfiguration configuration, LaneDefinition lane)
    {
        lane.Defaults.ApplyTo(configuration);
        return configuration;
    }
}

public sealed class VintageBendLanePolicy : ILanePolicy
{
    public static VintageBendLanePolicy Instance { get; } = new();

    private VintageBendLanePolicy()
    {
    }

    public PromptConfiguration ApplyDefaults(PromptConfiguration configuration, LaneDefinition lane)
    {
        lane.Defaults.ApplyTo(configuration);
        return configuration;
    }
}
