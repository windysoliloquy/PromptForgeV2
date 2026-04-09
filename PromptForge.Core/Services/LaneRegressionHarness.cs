using System.Reflection;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class LaneRegressionHarness
{
    private static readonly string[] SharedLaneIntents =
    [
        IntentModeCatalog.CinematicName,
        IntentModeCatalog.PhotographyName,
        IntentModeCatalog.ProductPhotographyName,
        IntentModeCatalog.FoodPhotographyName,
        IntentModeCatalog.LifestyleAdvertisingPhotographyName,
        IntentModeCatalog.ArchitectureArchvizName,
        IntentModeCatalog.ThreeDRenderName,
        IntentModeCatalog.ConceptArtName,
        IntentModeCatalog.PixelArtName,
        IntentModeCatalog.FantasyIllustrationName,
        IntentModeCatalog.EditorialIllustrationName,
        IntentModeCatalog.GraphicDesignName,
        IntentModeCatalog.TattooArtName,
    ];

    public static IReadOnlyList<string> Run(IPromptBuilderService promptBuilderService)
    {
        var failures = new List<string>();

        foreach (var intentName in SharedLaneIntents)
        {
            var lane = LaneRegistry.GetByIntentName(intentName)
                ?? throw new InvalidOperationException($"Lane '{intentName}' was not found.");

            var baseline = CreateHarnessConfiguration(lane);
            var baselineDescriptors = ResolveLaneDescriptors(lane.PrimaryIntentName, baseline).ToList();
            if (baselineDescriptors.Count == 0)
            {
                failures.Add($"Lane '{lane.Id}' did not produce any baseline descriptors.");
                continue;
            }

            var subtypeSelector = lane.SubtypeSelectors.FirstOrDefault();
            if (subtypeSelector is not null && subtypeSelector.Options.Count > 1)
            {
                var alternate = baseline.Clone();
                SetStringProperty(alternate, subtypeSelector.SelectedValuePropertyName, subtypeSelector.Options[1].Key);

                var baselinePrompt = promptBuilderService.Build(baseline).PositivePrompt;
                var alternatePrompt = promptBuilderService.Build(alternate).PositivePrompt;
                if (DetectMissingSubtypeInfluence(baselinePrompt, alternatePrompt))
                {
                    failures.Add($"Lane '{lane.Id}' subtype variation did not change the assembled prompt.");
                }
            }

            var allModifiers = baseline.Clone();
            foreach (var modifier in lane.Modifiers)
            {
                SetBoolProperty(allModifiers, modifier.StatePropertyName, true);
            }

            var allModifierDescriptors = ResolveLaneDescriptors(lane.PrimaryIntentName, allModifiers).ToList();
            if (DetectRepeatedAnchor(allModifierDescriptors, baselineDescriptors[0]))
            {
                failures.Add($"Lane '{lane.Id}' repeated its anchor descriptor '{baselineDescriptors[0]}'.");
            }

            var modifierDescriptorCount = Math.Max(0, allModifierDescriptors.Count - baselineDescriptors.Count);
            if (DetectModifierCapBreakage(baselineDescriptors.Count, allModifierDescriptors.Count, lane.ModifierCap))
            {
                failures.Add($"Lane '{lane.Id}' exceeded modifier cap '{lane.ModifierCap}' with '{modifierDescriptorCount}' modifier descriptors.");
            }

            var assembledPrompt = promptBuilderService.Build(allModifiers).PositivePrompt;
            if (DetectDuplicateFragments(assembledPrompt))
            {
                failures.Add($"Lane '{lane.Id}' produced duplicate prompt fragments in the assembled prompt.");
            }

            if (string.Equals(lane.Id, "photography", StringComparison.OrdinalIgnoreCase))
            {
                if (assembledPrompt.Contains("photographic image language", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Photography compression did not remove the weak meta phrase 'photographic image language'.");
                }

                if (assembledPrompt.Contains("photographic framing", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Photography compression left behind repeated photographic framing language.");
                }
            }

            if (string.Equals(lane.Id, "product-photography", StringComparison.OrdinalIgnoreCase))
            {
                if (assembledPrompt.Contains("product image language", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("commercial image language", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Product Photography compression did not remove weak meta language.");
                }

                if (assembledPrompt.Contains("product photography framing", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("product photography focus", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Product Photography compression left behind repeated product-photography root language.");
                }
            }

            if (string.Equals(lane.Id, "food-photography", StringComparison.OrdinalIgnoreCase))
            {
                if (assembledPrompt.Contains("food image language", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("restaurant image language", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("culinary atmosphere", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Food Photography compression did not remove weak meta language.");
                }

                if (assembledPrompt.Contains("food framing", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("food focus", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("food detail", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Food Photography compression left behind repeated food-photography root language.");
                }
            }

            if (string.Equals(lane.Id, "lifestyle-advertising-photography", StringComparison.OrdinalIgnoreCase))
            {
                if (assembledPrompt.Contains("lifestyle image language", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("advertising image language", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("campaign image language", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Lifestyle / Advertising Photography compression did not remove weak meta language.");
                }

                if (assembledPrompt.Contains("lifestyle photography framing", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("lifestyle photography focus", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Lifestyle / Advertising Photography compression left behind repeated lifestyle-photography root language.");
                }
            }

            if (string.Equals(lane.Id, "architecture-archviz", StringComparison.OrdinalIgnoreCase))
            {
                if (assembledPrompt.Contains("archviz image language", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("architectural atmosphere", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Architecture / Archviz compression did not remove weak meta language.");
                }

                if (assembledPrompt.Contains("architectural framing", StringComparison.OrdinalIgnoreCase)
                    || assembledPrompt.Contains("architectural focus", StringComparison.OrdinalIgnoreCase))
                {
                    failures.Add("Architecture / Archviz compression left behind repeated architectural root language.");
                }
            }
        }

        return failures;
    }

    public static bool DetectRepeatedAnchor(IEnumerable<string> descriptors, string anchorDescriptor)
    {
        return descriptors.Count(descriptor => string.Equals(descriptor, anchorDescriptor, StringComparison.OrdinalIgnoreCase)) > 1;
    }

    public static bool DetectMissingSubtypeInfluence(string baselinePrompt, string alternatePrompt)
    {
        return string.Equals(Normalize(baselinePrompt), Normalize(alternatePrompt), StringComparison.OrdinalIgnoreCase);
    }

    public static bool DetectModifierCapBreakage(int baselineDescriptorCount, int allModifierDescriptorCount, int modifierCap)
    {
        return Math.Max(0, allModifierDescriptorCount - baselineDescriptorCount) > modifierCap;
    }

    public static bool DetectDuplicateFragments(string prompt)
    {
        var fragments = SplitFragments(prompt);
        return fragments.Count != fragments.Distinct(StringComparer.OrdinalIgnoreCase).Count();
    }

    private static PromptConfiguration CreateHarnessConfiguration(LaneDefinition lane)
    {
        return new PromptConfiguration
        {
            IntentMode = lane.PrimaryIntentName,
            Subject = "weathered courier drone",
            Action = "hovering beside a rain-slick market stall",
            Relationship = "dense city signage in the background",
            AspectRatio = "16:9",
            UseNegativePrompt = false,
            CompressPromptSemantics = true,
            ReduceRepeatedLaneWords = true,
            TrimRepeatedLongWords = false,
        };
    }

    private static IEnumerable<string> ResolveLaneDescriptors(string intentName, PromptConfiguration configuration)
    {
        return intentName switch
        {
            var mode when string.Equals(mode, IntentModeCatalog.CinematicName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveCinematicDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.PhotographyName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolvePhotographyDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.ProductPhotographyName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveProductPhotographyDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.FoodPhotographyName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveFoodPhotographyDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.LifestyleAdvertisingPhotographyName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveLifestyleAdvertisingPhotographyDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.ArchitectureArchvizName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveArchitectureArchvizDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.ThreeDRenderName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveThreeDRenderDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.ConceptArtName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveConceptArtDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.PixelArtName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolvePixelArtDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.FantasyIllustrationName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveFantasyIllustrationDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.EditorialIllustrationName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveEditorialIllustrationDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.GraphicDesignName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveGraphicDesignDescriptors(configuration),
            var mode when string.Equals(mode, IntentModeCatalog.TattooArtName, StringComparison.OrdinalIgnoreCase) => SliderLanguageCatalog.ResolveTattooArtDescriptors(configuration),
            _ => Array.Empty<string>(),
        };
    }

    private static void SetStringProperty(PromptConfiguration configuration, string propertyName, string value)
    {
        var property = typeof(PromptConfiguration).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Prompt configuration property '{propertyName}' was not found.");
        property.SetValue(configuration, value);
    }

    private static void SetBoolProperty(PromptConfiguration configuration, string propertyName, bool value)
    {
        var property = typeof(PromptConfiguration).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Prompt configuration property '{propertyName}' was not found.");
        property.SetValue(configuration, value);
    }

    private static IReadOnlyList<string> SplitFragments(string prompt)
    {
        return prompt
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(Normalize)
            .Where(static fragment => !string.IsNullOrWhiteSpace(fragment))
            .ToArray();
    }

    private static string Normalize(string value)
    {
        return value.Trim().Trim(',', '.', ' ');
    }
}
