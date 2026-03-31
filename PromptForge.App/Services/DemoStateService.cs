using System.IO;
using System.Text.Json;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed class DemoStateService : IDemoStateService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _statePath;
    private DemoState _currentState;

    public DemoStateService()
    {
        var appDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PromptForgeDemo");

        Directory.CreateDirectory(appDataDirectory);
        _statePath = Path.Combine(appDataDirectory, "demo-state.json");
        _currentState = LoadState();
    }

    public DemoState CurrentState => new()
    {
        MaxCopies = _currentState.MaxCopies,
        RemainingCopies = _currentState.RemainingCopies,
    };

    public bool TryConsumeCopy(out DemoState state)
    {
        if (!DemoModeOptions.IsDemoMode)
        {
            state = CurrentState;
            return true;
        }

        if (_currentState.RemainingCopies <= 0)
        {
            state = CurrentState;
            return false;
        }

        var updatedState = new DemoState
        {
            MaxCopies = DemoModeOptions.MaxDemoCopies,
            RemainingCopies = Math.Max(0, _currentState.RemainingCopies - 1),
        };

        try
        {
            SaveState(updatedState);
            _currentState = updatedState;
            state = CurrentState;
            return true;
        }
        catch
        {
            state = CurrentState;
            return false;
        }
    }

    private DemoState LoadState()
    {
        if (!DemoModeOptions.IsDemoMode)
        {
            return CreateDefaultState();
        }

        try
        {
            if (File.Exists(_statePath))
            {
                var json = File.ReadAllText(_statePath);
                var state = JsonSerializer.Deserialize<DemoState>(json, JsonOptions);
                if (state is not null)
                {
                    return NormalizeState(state);
                }
            }
        }
        catch
        {
        }

        var defaultState = CreateDefaultState();

        try
        {
            SaveState(defaultState);
        }
        catch
        {
        }

        return defaultState;
    }

    private void SaveState(DemoState state)
    {
        File.WriteAllText(_statePath, JsonSerializer.Serialize(state, JsonOptions));
    }

    private static DemoState CreateDefaultState()
    {
        return new DemoState
        {
            MaxCopies = DemoModeOptions.MaxDemoCopies,
            RemainingCopies = DemoModeOptions.MaxDemoCopies,
        };
    }

    private static DemoState NormalizeState(DemoState state)
    {
        var maxCopies = DemoModeOptions.MaxDemoCopies;
        return new DemoState
        {
            MaxCopies = maxCopies,
            RemainingCopies = Math.Clamp(state.RemainingCopies, 0, maxCopies),
        };
    }
}
