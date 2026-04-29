using System.Reflection;
using PromptForge.App.Models;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public static class StandardLaneBindingValidator
{
    private static readonly string[] SharedStandardLaneIntentNames =
    [
        IntentModeCatalog.ChildrensBookName,
        IntentModeCatalog.CinematicName,
        IntentModeCatalog.PhotographyName,
        IntentModeCatalog.ProductPhotographyName,
        IntentModeCatalog.FoodPhotographyName,
        IntentModeCatalog.LifestyleAdvertisingPhotographyName,
        IntentModeCatalog.InfographicDataVisualizationName,
        IntentModeCatalog.ArchitectureArchvizName,
        IntentModeCatalog.ThreeDRenderName,
        IntentModeCatalog.ConceptArtName,
        IntentModeCatalog.PixelArtName,
        IntentModeCatalog.FantasyIllustrationName,
        IntentModeCatalog.EditorialIllustrationName,
        IntentModeCatalog.GraphicDesignName,
        IntentModeCatalog.TattooArtName,
        IntentModeCatalog.WatercolorName,
    ];

    public static IReadOnlyList<string> Validate(Type viewModelType, IEnumerable<LaneDefinition> definitions)
    {
        var errors = new List<string>();

        foreach (var definition in definitions)
        {
            foreach (var selector in definition.SubtypeSelectors)
            {
                var property = viewModelType.GetProperty(selector.SelectedValuePropertyName, BindingFlags.Instance | BindingFlags.Public);
                if (property is null || property.PropertyType != typeof(string))
                {
                    errors.Add($"Shared lane '{definition.Id}' selector '{selector.Key}' references missing or non-string view-model property '{selector.SelectedValuePropertyName}'.");
                }
            }

            foreach (var modifier in definition.Modifiers)
            {
                var property = viewModelType.GetProperty(modifier.StatePropertyName, BindingFlags.Instance | BindingFlags.Public);
                if (property is null || property.PropertyType != typeof(bool))
                {
                    errors.Add($"Shared lane '{definition.Id}' modifier '{modifier.Key}' references missing or non-bool view-model property '{modifier.StatePropertyName}'.");
                }
            }
        }

        return errors;
    }

    public static void ThrowIfInvalid(Type viewModelType, IEnumerable<LaneDefinition> definitions)
    {
        var errors = Validate(viewModelType, definitions);
        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException($"Standard lane binding validation failed:{Environment.NewLine}- {string.Join($"{Environment.NewLine}- ", errors)}");
    }

    public static IReadOnlyList<LaneDefinition> GetSharedStandardLaneDefinitions()
    {
        return SharedStandardLaneIntentNames
            .Select(intentName => LaneRegistry.GetByIntentName(intentName)
                ?? throw new InvalidOperationException($"Shared standard lane '{intentName}' was not found in the lane registry."))
            .ToArray();
    }
}
