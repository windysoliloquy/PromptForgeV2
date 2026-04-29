using System;
using System.Collections.ObjectModel;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel
{
    public string PresetName { get => _presetName; set => SetProperty(ref _presetName, value); }
    public string NewSavestateFolderName
    {
        get => _newSavestateFolderName;
        set
        {
            if (SetProperty(ref _newSavestateFolderName, value))
            {
                DeleteSavestateFolderCommand.RaiseCanExecuteChanged();
            }
        }
    }
    public PresetSavestateFolder? SelectedSavestateFolder
    {
        get => _selectedSavestateFolder;
        set
        {
            if (SetProperty(ref _selectedSavestateFolder, value) && value is not null)
            {
                _presetStorageService.SelectSavestateFolder(value.Key);
                RefreshPresetNames();
                DeleteSavestateFolderCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string? SelectedPresetName
    {
        get => _selectedPresetName;
        set => SetProperty(ref _selectedPresetName, value);
    }

    private void SavePreset()
    {
        var name = PresetName?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            StatusMessage = "Enter a preset name before saving.";
            return;
        }

        if (_presetStorageService.PresetNameExists(name))
        {
            StatusMessage = "A preset with that name already exists.";
            return;
        }

        var requiredUnlockPresetName = GetRequiredUnlockPresetName(IntentMode);
        if (!string.IsNullOrWhiteSpace(requiredUnlockPresetName)
            && !HasLockedLaneAccess
            && SelectedSavestateFolder?.IsDefault != true)
        {
            StatusMessage = "Ready";
            return;
        }

        if (!string.IsNullOrWhiteSpace(requiredUnlockPresetName)
            && !HasLockedLaneAccess
            && !string.Equals(name, requiredUnlockPresetName, StringComparison.Ordinal))
        {
            StatusMessage = "Ready";
            return;
        }

        _presetStorageService.Save(name, CaptureConfiguration());
        var savedToDefaultStore = SelectedSavestateFolder?.IsDefault == true;
        if (!string.IsNullOrWhiteSpace(requiredUnlockPresetName)
            && string.Equals(name, requiredUnlockPresetName, StringComparison.Ordinal)
            && savedToDefaultStore)
        {
            _laneUnlockStateService.Unlock(IntentMode);
        }

        RefreshPresetNames(name);
        PresetName = string.Empty;
        StatusMessage = !string.IsNullOrWhiteSpace(requiredUnlockPresetName) && string.Equals(name, requiredUnlockPresetName, StringComparison.Ordinal) && savedToDefaultStore
            ? $"{IntentMode} unlocked on this machine."
            : $"Preset '{name}' saved.";
    }

    private void LoadPreset()
    {
        var name = SelectedPresetName?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            StatusMessage = "Select a preset to load.";
            return;
        }

        ApplyConfiguration(_presetStorageService.Load(name));
        StatusMessage = $"Preset '{name}' loaded.";
    }

    private void RenamePreset()
    {
        var current = SelectedPresetName?.Trim();
        var target = PresetName?.Trim();
        if (string.IsNullOrWhiteSpace(current))
        {
            StatusMessage = "Select a preset to rename.";
            return;
        }

        if (string.IsNullOrWhiteSpace(target))
        {
            StatusMessage = "Enter the new preset name first.";
            return;
        }

        if (string.Equals(NormalizePresetName(current), NormalizePresetName(target), StringComparison.Ordinal))
        {
            StatusMessage = $"Preset '{current}' unchanged.";
            return;
        }

        if (_presetStorageService.PresetNameExists(target))
        {
            StatusMessage = "A different preset with that name already exists.";
            return;
        }

        _presetStorageService.Rename(current, target);
        RefreshPresetNames(target);
        StatusMessage = $"Preset renamed to '{target}'.";
    }

    private void DeletePreset()
    {
        var name = SelectedPresetName?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            StatusMessage = "Select a preset to delete.";
            return;
        }

        _presetStorageService.Delete(name);
        RefreshPresetNames();
        StatusMessage = $"Preset '{name}' deleted.";
    }

    private void CreateSavestateFolder()
    {
        try
        {
            var folder = _presetStorageService.CreateSavestateFolder(NewSavestateFolderName);
            ClearNewSavestateFolderName();
            RefreshSavestateFolders(folder.Key);
            RefreshPresetNames();
            StatusMessage = $"Savestate folder '{folder.DisplayName}' created.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private void DeleteSelectedSavestateFolder()
    {
        var folder = SelectedSavestateFolder;
        if (folder is null)
        {
            StatusMessage = "Select a savestate folder first.";
            return;
        }

        if (!folder.CanDelete)
        {
            StatusMessage = "Prompt Forge Default cannot be deleted.";
            return;
        }

        if (!CanDeleteSelectedSavestateFolder())
        {
            StatusMessage = "Type the selected savestate folder name exactly before deleting it.";
            return;
        }

        try
        {
            _presetStorageService.DeleteSavestateFolder(folder.Key);
            RefreshSavestateFolders();
            RefreshPresetNames();
            StatusMessage = $"Savestate folder '{folder.DisplayName}' deleted. Using Prompt Forge Default.";
        }
        catch (Exception ex)
        {
            StatusMessage = ex.Message;
        }
    }

    private bool CanDeleteSelectedSavestateFolder()
    {
        var folder = SelectedSavestateFolder;
        return folder?.CanDelete == true
            && string.Equals(NewSavestateFolderName, folder.DisplayName, StringComparison.Ordinal);
    }

    private void ClearNewSavestateFolderName()
    {
        _newSavestateFolderName = string.Empty;
        OnPropertyChanged(nameof(NewSavestateFolderName));
        DeleteSavestateFolderCommand.RaiseCanExecuteChanged();
    }

    private void RefreshSavestateFolders(string? selectedKey = null)
    {
        SavestateFolders.Clear();
        var folders = _presetStorageService.GetSavestateFolders();
        foreach (var folder in folders)
        {
            SavestateFolders.Add(folder);
        }

        var activeFolder = selectedKey is null
            ? _presetStorageService.GetActiveSavestateFolder()
            : folders.FirstOrDefault(folder => string.Equals(folder.Key, selectedKey, StringComparison.OrdinalIgnoreCase))
                ?? _presetStorageService.GetActiveSavestateFolder();

        SelectedSavestateFolder = SavestateFolders.FirstOrDefault(folder => string.Equals(folder.Key, activeFolder.Key, StringComparison.OrdinalIgnoreCase))
            ?? SavestateFolders.FirstOrDefault();
    }

    private void RefreshPresetNames(string? selected = null)
    {
        PresetNames.Clear();
        var presetNames = _presetStorageService.GetPresetNames();
        foreach (var name in presetNames)
        {
            PresetNames.Add(name);
        }

        SelectedPresetName = selected ?? PresetNames.FirstOrDefault();
        UpdateLockedLaneAccessState();
    }

    private static string NormalizePresetName(string? name) => name?.Trim().ToUpperInvariant() ?? string.Empty;
}
