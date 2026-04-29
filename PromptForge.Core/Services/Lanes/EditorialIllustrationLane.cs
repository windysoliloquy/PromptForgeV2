using PromptForge.App.Models;

namespace PromptForge.App.Services.Lanes;

public sealed class EditorialIllustrationLane : ILanePromptContributor, ILanePresentationOverlayProvider, ILaneSliderSuppressionProvider
{
    private static readonly IReadOnlySet<string> NoSuppressedSliders = new HashSet<string>(StringComparer.Ordinal);
    private static readonly IReadOnlySet<string> DefaultSuppressedSliders = new HashSet<string>(StringComparer.Ordinal)
    {
        SliderLanguageCatalog.Stylization,
        SliderLanguageCatalog.Chaos,
        SliderLanguageCatalog.BackgroundComplexity,
    };

    public static EditorialIllustrationLane Instance { get; } = new();

    public string IntentName => IntentModeCatalog.EditorialIllustrationName;

    private EditorialIllustrationLane()
    {
    }

    public IEnumerable<PromptFragment> BuildEarlyDescriptors(PromptConfiguration configuration)
    {
        foreach (var phrase in SliderLanguageCatalog.ResolveEditorialIllustrationDescriptors(configuration))
        {
            yield return new PromptFragment(phrase);
        }

        foreach (var fragment in BuildPresentationOverlayDescriptors(configuration))
        {
            yield return fragment;
        }
    }

    public IEnumerable<PromptFragment> BuildPresentationOverlayDescriptors(PromptConfiguration configuration)
    {
        return SliderLanguageCatalog.ResolveEditorialIllustrationOverlayDescriptors(configuration)
            .Select(static phrase => new PromptFragment(phrase));
    }

    public IReadOnlySet<string> GetSuppressedSliders(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsEditorialIllustration(configuration.IntentMode))
        {
            return NoSuppressedSliders;
        }

        if (configuration.EditorialIllustrationBlackAndWhiteMonochrome)
        {
            return new HashSet<string>(StringComparer.Ordinal)
            {
                SliderLanguageCatalog.Stylization,
                SliderLanguageCatalog.Chaos,
                SliderLanguageCatalog.BackgroundComplexity,
                SliderLanguageCatalog.Saturation,
                SliderLanguageCatalog.Temperature,
            };
        }

        return DefaultSuppressedSliders;
    }

    public IEnumerable<string> GetSuppressibleSliderKeys()
    {
        yield return SliderLanguageCatalog.Stylization;
        yield return SliderLanguageCatalog.Chaos;
        yield return SliderLanguageCatalog.BackgroundComplexity;
        yield return SliderLanguageCatalog.Saturation;
        yield return SliderLanguageCatalog.Temperature;
    }
}
