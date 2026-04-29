using System.IO;
using System.Text.Json;

namespace PromptForge.App.Services;

public sealed class LaneUnlockStateService : ILaneUnlockStateService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _statePath;
    private readonly HashSet<string> _unlockedIntentModes = new(StringComparer.OrdinalIgnoreCase);

    public LaneUnlockStateService()
    {
        var settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PromptForge");
        Directory.CreateDirectory(settingsDirectory);
        _statePath = Path.Combine(settingsDirectory, "lane-unlocks.json");
        LoadState();
    }

    public bool IsUnlocked(string? intentMode)
    {
        return !string.IsNullOrWhiteSpace(intentMode)
            && _unlockedIntentModes.Contains(intentMode.Trim());
    }

    public void Unlock(string? intentMode)
    {
        if (string.IsNullOrWhiteSpace(intentMode))
        {
            return;
        }

        if (_unlockedIntentModes.Add(intentMode.Trim()))
        {
            SaveState();
        }
    }

    public void MigrateFromLegacyPresetMarkers(
        IReadOnlyDictionary<string, string> legacyUnlockPresetNames,
        IReadOnlyCollection<string> defaultPresetNames)
    {
        var changed = false;
        foreach (var pair in legacyUnlockPresetNames)
        {
            if (defaultPresetNames.Any(name => string.Equals(name, pair.Value, StringComparison.Ordinal))
                && _unlockedIntentModes.Add(pair.Key))
            {
                changed = true;
            }
        }

        if (changed)
        {
            SaveState();
        }
    }

    private void LoadState()
    {
        try
        {
            if (!File.Exists(_statePath))
            {
                return;
            }

            var json = File.ReadAllText(_statePath);
            var state = JsonSerializer.Deserialize<LaneUnlockState>(json, JsonOptions);
            if (state?.UnlockedIntentModes is null)
            {
                return;
            }

            foreach (var intentMode in state.UnlockedIntentModes.Where(mode => !string.IsNullOrWhiteSpace(mode)))
            {
                _unlockedIntentModes.Add(intentMode.Trim());
            }
        }
        catch
        {
        }
    }

    private void SaveState()
    {
        try
        {
            var state = new LaneUnlockState
            {
                UnlockedIntentModes = _unlockedIntentModes
                    .OrderBy(mode => mode, StringComparer.OrdinalIgnoreCase)
                    .ToList(),
            };

            File.WriteAllText(_statePath, JsonSerializer.Serialize(state, JsonOptions));
        }
        catch
        {
        }
    }

    private sealed class LaneUnlockState
    {
        public List<string> UnlockedIntentModes { get; set; } = new();
    }
}
