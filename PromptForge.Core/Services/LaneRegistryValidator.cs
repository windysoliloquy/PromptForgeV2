using System.Reflection;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class LaneRegistryValidator
{
    public static IReadOnlyList<string> Validate(IEnumerable<LaneDefinition> definitions, Type configurationType)
    {
        var errors = new List<string>();
        var seenLaneIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var seenIntentNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var definition in definitions)
        {
            if (!seenLaneIds.Add(definition.Id))
            {
                errors.Add($"Duplicate lane id '{definition.Id}'.");
            }

            if (definition.IntentNames.Count == 0)
            {
                errors.Add($"Lane '{definition.Id}' must claim at least one intent name.");
            }

            if (definition.ModifierCap < 0)
            {
                errors.Add($"Lane '{definition.Id}' has invalid modifier cap '{definition.ModifierCap}'.");
            }

            foreach (var intentName in definition.IntentNames)
            {
                if (!seenIntentNames.Add(intentName))
                {
                    errors.Add($"Intent name '{intentName}' is claimed by more than one lane.");
                }
            }

            ValidateSelectors(definition, configurationType, errors);
            ValidateModifiers(definition, configurationType, errors);
            ValidateWeightGroups(definition, errors);
        }

        return errors;
    }

    public static void ThrowIfInvalid(IEnumerable<LaneDefinition> definitions, Type configurationType)
    {
        var errors = Validate(definitions, configurationType);
        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException($"Lane registry validation failed:{Environment.NewLine}- {string.Join($"{Environment.NewLine}- ", errors)}");
    }

    private static void ValidateSelectors(LaneDefinition definition, Type configurationType, ICollection<string> errors)
    {
        var seenSelectorKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var selector in definition.SubtypeSelectors)
        {
            if (!seenSelectorKeys.Add(selector.Key))
            {
                errors.Add($"Lane '{definition.Id}' has duplicate selector key '{selector.Key}'.");
            }

            if (selector.Options.Count == 0)
            {
                errors.Add($"Lane '{definition.Id}' selector '{selector.Key}' must declare at least one option.");
            }

            if (selector.Options.Count(static option => option.IsDefault) != 1)
            {
                errors.Add($"Lane '{definition.Id}' selector '{selector.Key}' must declare exactly one default option.");
            }

            var property = configurationType.GetProperty(selector.SelectedValuePropertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property is null || property.PropertyType != typeof(string))
            {
                errors.Add($"Lane '{definition.Id}' selector '{selector.Key}' references missing or non-string config property '{selector.SelectedValuePropertyName}'.");
            }
        }
    }

    private static void ValidateModifiers(LaneDefinition definition, Type configurationType, ICollection<string> errors)
    {
        var seenModifierKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var weightGroups = new HashSet<string>(definition.WeightGroups.Select(static group => group.Key), StringComparer.OrdinalIgnoreCase);

        foreach (var modifier in definition.Modifiers)
        {
            if (!seenModifierKeys.Add(modifier.Key))
            {
                errors.Add($"Lane '{definition.Id}' has duplicate modifier key '{modifier.Key}'.");
            }

            if (modifier.CapContribution < 0)
            {
                errors.Add($"Lane '{definition.Id}' modifier '{modifier.Key}' has invalid cap contribution '{modifier.CapContribution}'.");
            }

            if (!weightGroups.Contains(modifier.WeightGroup))
            {
                errors.Add($"Lane '{definition.Id}' modifier '{modifier.Key}' references unknown weight group '{modifier.WeightGroup}'.");
            }

            var property = configurationType.GetProperty(modifier.StatePropertyName, BindingFlags.Instance | BindingFlags.Public);
            if (property is null || property.PropertyType != typeof(bool))
            {
                errors.Add($"Lane '{definition.Id}' modifier '{modifier.Key}' references missing or non-bool config property '{modifier.StatePropertyName}'.");
            }

            if (!string.IsNullOrWhiteSpace(modifier.VisibilityPredicate) && modifier.DefaultState)
            {
                errors.Add($"Lane '{definition.Id}' modifier '{modifier.Key}' is conditionally hidden but defaults to enabled.");
            }

            if (string.IsNullOrWhiteSpace(modifier.DescriptorHint) && modifier.CapContribution > 0)
            {
                errors.Add($"Lane '{definition.Id}' modifier '{modifier.Key}' must declare a descriptor hint or be an explicit no-op.");
            }
        }
    }

    private static void ValidateWeightGroups(LaneDefinition definition, ICollection<string> errors)
    {
        var seenWeightGroupKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var modifierKeys = new HashSet<string>(definition.Modifiers.Select(static modifier => modifier.Key), StringComparer.OrdinalIgnoreCase);

        foreach (var group in definition.WeightGroups)
        {
            if (!seenWeightGroupKeys.Add(group.Key))
            {
                errors.Add($"Lane '{definition.Id}' has duplicate weight group '{group.Key}'.");
            }

            if (group.SoftCap < 0 || group.HardCap < group.SoftCap)
            {
                errors.Add($"Lane '{definition.Id}' weight group '{group.Key}' has invalid caps.");
            }

            foreach (var key in group.PriorityOrder)
            {
                if (!modifierKeys.Contains(key))
                {
                    errors.Add($"Lane '{definition.Id}' weight group '{group.Key}' references unknown modifier '{key}'.");
                }
            }
        }
    }
}
