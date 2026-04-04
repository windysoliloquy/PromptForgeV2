namespace PromptForge.App.Models;

public sealed class StandardLaneState
{
    public string LaneId { get; set; } = string.Empty;
    public Dictionary<string, string> Selectors { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, bool> Modifiers { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public string GetSelector(string selectorKey, string fallback = "")
    {
        return Selectors.TryGetValue(selectorKey, out var value) ? value : fallback;
    }

    public void SetSelector(string selectorKey, string value)
    {
        Selectors[selectorKey] = value;
    }

    public bool GetModifier(string modifierKey, bool fallback = false)
    {
        return Modifiers.TryGetValue(modifierKey, out var value) ? value : fallback;
    }

    public void SetModifier(string modifierKey, bool value)
    {
        Modifiers[modifierKey] = value;
    }

    public StandardLaneState Clone()
    {
        return new StandardLaneState
        {
            LaneId = LaneId,
            Selectors = new Dictionary<string, string>(Selectors, StringComparer.OrdinalIgnoreCase),
            Modifiers = new Dictionary<string, bool>(Modifiers, StringComparer.OrdinalIgnoreCase),
        };
    }
}

public sealed class StandardLaneStateCollection
{
    public Dictionary<string, StandardLaneState> Lanes { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public int Count => Lanes.Count;

    public bool TryGetLane(string laneId, out StandardLaneState laneState)
    {
        return Lanes.TryGetValue(laneId, out laneState!);
    }

    public StandardLaneState GetOrAddLane(string laneId)
    {
        if (!Lanes.TryGetValue(laneId, out var laneState))
        {
            laneState = new StandardLaneState { LaneId = laneId };
            Lanes[laneId] = laneState;
        }

        return laneState;
    }

    public void SetLane(StandardLaneState laneState)
    {
        if (string.IsNullOrWhiteSpace(laneState.LaneId))
        {
            throw new InvalidOperationException("Standard lane state must declare a lane id before being added to the collection.");
        }

        Lanes[laneState.LaneId] = laneState;
    }

    public StandardLaneStateCollection Clone()
    {
        var clone = new StandardLaneStateCollection();
        foreach (var laneState in Lanes.Values)
        {
            clone.SetLane(laneState.Clone());
        }

        return clone;
    }
}
