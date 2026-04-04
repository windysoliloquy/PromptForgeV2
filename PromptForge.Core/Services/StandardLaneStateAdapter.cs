using System.Reflection;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class StandardLaneStateAdapter
{
    private static readonly HashSet<string> SpecialLaneIds = new(StringComparer.OrdinalIgnoreCase)
    {
        "comic-book",
        "vintage-bend",
    };

    public static StandardLaneStateCollection CreateDefaultCollection()
    {
        var collection = new StandardLaneStateCollection();
        foreach (var lane in GetOrdinaryLaneDefinitions())
        {
            collection.SetLane(CreateDefaultLaneState(lane));
        }

        return collection;
    }

    public static StandardLaneStateCollection CaptureFromConfiguration(PromptConfiguration configuration)
    {
        var collection = CreateDefaultCollection();

        foreach (var lane in GetOrdinaryLaneDefinitions())
        {
            var laneState = collection.GetOrAddLane(lane.Id);

            foreach (var selector in lane.SubtypeSelectors)
            {
                laneState.SetSelector(selector.Key, NormalizeSelectorValue(selector, GetStringProperty(configuration, selector.SelectedValuePropertyName)));
            }

            foreach (var modifier in lane.Modifiers)
            {
                laneState.SetModifier(modifier.Key, GetBoolProperty(configuration, modifier.StatePropertyName));
            }
        }

        return collection;
    }

    public static StandardLaneStateCollection HydrateConfiguration(PromptConfiguration configuration)
    {
        var hydrated = CreateDefaultCollection();
        var source = configuration.StandardLaneStates;

        if (source is not null && source.Count > 0)
        {
            MergeInto(hydrated, source);
            ApplyToConfiguration(configuration, hydrated);
        }
        else
        {
            hydrated = CaptureFromConfiguration(configuration);
        }

        configuration.StandardLaneStates = hydrated;
        return hydrated;
    }

    public static void ApplyToConfiguration(PromptConfiguration configuration, StandardLaneStateCollection? states = null)
    {
        var source = states ?? configuration.StandardLaneStates;
        if (source is null)
        {
            return;
        }

        foreach (var lane in GetOrdinaryLaneDefinitions())
        {
            if (!source.TryGetLane(lane.Id, out var laneState))
            {
                continue;
            }

            foreach (var selector in lane.SubtypeSelectors)
            {
                var selectorValue = NormalizeSelectorValue(selector, laneState.GetSelector(selector.Key, GetDefaultSelectorValue(selector)));
                SetStringProperty(configuration, selector.SelectedValuePropertyName, selectorValue);
            }

            foreach (var modifier in lane.Modifiers)
            {
                SetBoolProperty(configuration, modifier.StatePropertyName, laneState.GetModifier(modifier.Key, modifier.DefaultState));
            }
        }
    }

    private static void MergeInto(StandardLaneStateCollection target, StandardLaneStateCollection source)
    {
        foreach (var lane in source.Lanes.Values)
        {
            if (string.IsNullOrWhiteSpace(lane.LaneId))
            {
                continue;
            }

            var targetLane = target.GetOrAddLane(lane.LaneId);

            var definition = LaneRegistry.All.FirstOrDefault(item => string.Equals(item.Id, lane.LaneId, StringComparison.OrdinalIgnoreCase));
            foreach (var selector in lane.Selectors)
            {
                var selectorDefinition = definition?.SubtypeSelectors.FirstOrDefault(item => string.Equals(item.Key, selector.Key, StringComparison.OrdinalIgnoreCase));
                targetLane.SetSelector(selector.Key, selectorDefinition is null ? selector.Value : NormalizeSelectorValue(selectorDefinition, selector.Value));
            }

            foreach (var modifier in lane.Modifiers)
            {
                targetLane.SetModifier(modifier.Key, modifier.Value);
            }
        }
    }

    private static IReadOnlyList<LaneDefinition> GetOrdinaryLaneDefinitions()
    {
        return LaneRegistry.All
            .Where(lane => !SpecialLaneIds.Contains(lane.Id))
            .ToArray();
    }

    private static StandardLaneState CreateDefaultLaneState(LaneDefinition lane)
    {
        var laneState = new StandardLaneState
        {
            LaneId = lane.Id,
        };

        foreach (var selector in lane.SubtypeSelectors)
        {
            laneState.SetSelector(selector.Key, GetDefaultSelectorValue(selector));
        }

        foreach (var modifier in lane.Modifiers)
        {
            laneState.SetModifier(modifier.Key, modifier.DefaultState);
        }

        return laneState;
    }

    private static string GetDefaultSelectorValue(LaneSubtypeSelectorDefinition selector)
    {
        return selector.Options.FirstOrDefault(static option => option.IsDefault)?.Key
            ?? selector.Options.FirstOrDefault()?.Key
            ?? string.Empty;
    }

    private static string NormalizeSelectorValue(LaneSubtypeSelectorDefinition selector, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return GetDefaultSelectorValue(selector);
        }

        var match = selector.Options.FirstOrDefault(option =>
            string.Equals(option.Key, value, StringComparison.OrdinalIgnoreCase)
            || string.Equals(option.Label, value, StringComparison.OrdinalIgnoreCase));

        return match?.Key ?? value;
    }

    private static string GetStringProperty(PromptConfiguration configuration, string propertyName)
    {
        var property = typeof(PromptConfiguration).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Prompt configuration property '{propertyName}' was not found.");
        return property.GetValue(configuration) as string ?? string.Empty;
    }

    private static bool GetBoolProperty(PromptConfiguration configuration, string propertyName)
    {
        var property = typeof(PromptConfiguration).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?? throw new InvalidOperationException($"Prompt configuration property '{propertyName}' was not found.");
        return property.GetValue(configuration) is bool value && value;
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
}
