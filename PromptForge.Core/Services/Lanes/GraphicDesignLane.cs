using PromptForge.App.Models;

namespace PromptForge.App.Services.Lanes;

public sealed class GraphicDesignLane : ILanePromptContributor, ILaneSliderSuppressionProvider
{
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
        var suppressions = new HashSet<string>(StringComparer.Ordinal);
        if (!IntentModeCatalog.IsGraphicDesign(configuration.IntentMode) ||
            !configuration.GraphicDesignMinimalLayout)
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
        yield return SliderLanguageCatalog.BackgroundComplexity;
        yield return SliderLanguageCatalog.DetailDensity;
        yield return SliderLanguageCatalog.NarrativeDensity;
        yield return SliderLanguageCatalog.Chaos;
        yield return SliderLanguageCatalog.Framing;
        yield return SliderLanguageCatalog.FocusDepth;
    }
}
