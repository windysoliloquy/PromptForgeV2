using PromptForge.App.Models;

namespace PromptForge.App.Services.Lanes;

public sealed class GraphicDesignLane : ILanePromptContributor, ILaneSliderSuppressionProvider
{
    private static readonly IReadOnlySet<string> NoSuppressedSliders = new HashSet<string>(StringComparer.Ordinal);
    private static readonly IReadOnlySet<string> DefaultSuppressedSliders = new HashSet<string>(StringComparer.Ordinal)
    {
        SliderLanguageCatalog.SurfaceAge,
        SliderLanguageCatalog.CameraAngle,
        SliderLanguageCatalog.Temperature,
        SliderLanguageCatalog.AtmosphericDepth,
    };

    public static GraphicDesignLane Instance { get; } = new();

    public string IntentName => IntentModeCatalog.GraphicDesignName;

    private GraphicDesignLane()
    {
    }

    public IEnumerable<PromptFragment> BuildEarlyDescriptors(PromptConfiguration configuration)
    {
        foreach (var phrase in SliderLanguageCatalog.ResolveGraphicDesignDescriptors(configuration))
        {
            yield return new PromptFragment(phrase);
        }
    }

    public IReadOnlySet<string> GetSuppressedSliders(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsGraphicDesign(configuration.IntentMode))
        {
            return NoSuppressedSliders;
        }

        var suppressions = new HashSet<string>(DefaultSuppressedSliders, StringComparer.Ordinal);
        if (!configuration.GraphicDesignMinimalLayout)
        {
            return suppressions;
        }

        suppressions.Add(SliderLanguageCatalog.BackgroundComplexity);
        suppressions.Add(SliderLanguageCatalog.DetailDensity);
        suppressions.Add(SliderLanguageCatalog.NarrativeDensity);
        suppressions.Add(SliderLanguageCatalog.Chaos);
        suppressions.Add(SliderLanguageCatalog.Framing);
        suppressions.Add(SliderLanguageCatalog.FocusDepth);
        return suppressions;
    }

    public IEnumerable<string> GetSuppressibleSliderKeys()
    {
        yield return SliderLanguageCatalog.SurfaceAge;
        yield return SliderLanguageCatalog.CameraAngle;
        yield return SliderLanguageCatalog.Temperature;
        yield return SliderLanguageCatalog.AtmosphericDepth;
        yield return SliderLanguageCatalog.BackgroundComplexity;
        yield return SliderLanguageCatalog.DetailDensity;
        yield return SliderLanguageCatalog.NarrativeDensity;
        yield return SliderLanguageCatalog.Chaos;
        yield return SliderLanguageCatalog.Framing;
        yield return SliderLanguageCatalog.FocusDepth;
    }
}
