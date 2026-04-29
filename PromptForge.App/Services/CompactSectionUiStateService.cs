using System.IO;
using System.Text.Json;

namespace PromptForge.App.Services;

public sealed class CompactSectionUiStateService
{
    private readonly string _settingsPath;
    private Dictionary<string, bool> _expandedStates = new(StringComparer.OrdinalIgnoreCase);

    public static event EventHandler<CompactSectionUiStateChangedEventArgs>? SectionStateChanged;

    public CompactSectionUiStateService()
    {
        var settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PromptForge");
        Directory.CreateDirectory(settingsDirectory);
        _settingsPath = Path.Combine(settingsDirectory, "compact-section-state.json");
        Load();
    }

    public bool GetIsExpanded(string laneId, string sectionKey, bool defaultValue = true)
    {
        return _expandedStates.TryGetValue(BuildKey(laneId, sectionKey), out var isExpanded)
            ? isExpanded
            : defaultValue;
    }

    public void SetIsExpanded(string laneId, string sectionKey, bool isExpanded)
    {
        _expandedStates[BuildKey(laneId, sectionKey)] = isExpanded;
        Save();
        SectionStateChanged?.Invoke(
            this,
            new CompactSectionUiStateChangedEventArgs(laneId, sectionKey, isExpanded));
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return;
            }

            var json = File.ReadAllText(_settingsPath);
            _expandedStates = JsonSerializer.Deserialize<Dictionary<string, bool>>(json)
                ?? new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            _expandedStates = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_expandedStates, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsPath, json);
        }
        catch
        {
        }
    }

    private static string BuildKey(string laneId, string sectionKey) => $"{laneId}/{sectionKey}";
}

public sealed class CompactSectionUiStateChangedEventArgs : EventArgs
{
    public CompactSectionUiStateChangedEventArgs(string laneId, string sectionKey, bool isExpanded)
    {
        LaneId = laneId;
        SectionKey = sectionKey;
        IsExpanded = isExpanded;
    }

    public string LaneId { get; }
    public string SectionKey { get; }
    public bool IsExpanded { get; }
}
