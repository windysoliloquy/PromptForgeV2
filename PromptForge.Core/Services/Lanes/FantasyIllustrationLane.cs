using PromptForge.App.Models;

namespace PromptForge.App.Services.Lanes;

public sealed class FantasyIllustrationLane : ILanePromptContributor, ILaneSliderSuppressionProvider
{
    public static FantasyIllustrationLane Instance { get; } = new();

    public string IntentName => IntentModeCatalog.FantasyIllustrationName;

    private FantasyIllustrationLane()
    {
    }

    public IEnumerable<PromptFragment> BuildEarlyDescriptors(PromptConfiguration configuration)
    {
        return SliderLanguageCatalog.ResolveFantasyIllustrationDescriptors(configuration)
            .Select(static phrase => new PromptFragment(phrase));
    }

    public IReadOnlySet<string> GetSuppressedSliders(PromptConfiguration configuration)
    {
        var suppressions = new HashSet<string>(StringComparer.Ordinal);
        if (!IntentModeCatalog.IsFantasyIllustration(configuration.IntentMode))
        {
            return suppressions;
        }

        if (configuration.FantasyIllustrationCharacterSketch)
        {
            suppressions.Add(SliderLanguageCatalog.BackgroundComplexity);
            suppressions.Add(SliderLanguageCatalog.AtmosphericDepth);
            suppressions.Add(SliderLanguageCatalog.NarrativeDensity);
        }

        if (configuration.FantasyIllustrationCharacterCentric)
        {
            suppressions.Add(SliderLanguageCatalog.BackgroundComplexity);
        }

        if (configuration.FantasyIllustrationCleanBackground)
        {
            suppressions.Add(SliderLanguageCatalog.BackgroundComplexity);
            suppressions.Add(SliderLanguageCatalog.AtmosphericDepth);
            suppressions.Add(SliderLanguageCatalog.NarrativeDensity);
        }

        if (configuration.FantasyIllustrationSilhouetteReadability)
        {
            suppressions.Add(SliderLanguageCatalog.BackgroundComplexity);
            suppressions.Add(SliderLanguageCatalog.DetailDensity);
            suppressions.Add(SliderLanguageCatalog.Chaos);
        }

        if (configuration.FantasyIllustrationPropArtifactFocus)
        {
            suppressions.Add(SliderLanguageCatalog.MotionEnergy);
            suppressions.Add(SliderLanguageCatalog.NarrativeDensity);
        }

        if (configuration.FantasyIllustrationCreatureDesign)
        {
            suppressions.Add(SliderLanguageCatalog.BackgroundComplexity);
            suppressions.Add(SliderLanguageCatalog.NarrativeDensity);
            suppressions.Add(SliderLanguageCatalog.AtmosphericDepth);
        }

        return suppressions;
    }

    public IEnumerable<string> GetSuppressibleSliderKeys()
    {
        yield return SliderLanguageCatalog.BackgroundComplexity;
        yield return SliderLanguageCatalog.AtmosphericDepth;
        yield return SliderLanguageCatalog.NarrativeDensity;
        yield return SliderLanguageCatalog.DetailDensity;
        yield return SliderLanguageCatalog.Chaos;
        yield return SliderLanguageCatalog.MotionEnergy;
    }
}
