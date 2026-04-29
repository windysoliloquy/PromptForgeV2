using PromptForge.App.Models;

namespace PromptForge.App.Services;

public interface IPresetStorageService
{
    IReadOnlyList<PresetSavestateFolder> GetSavestateFolders();
    PresetSavestateFolder GetActiveSavestateFolder();
    void SelectSavestateFolder(string key);
    PresetSavestateFolder CreateSavestateFolder(string name);
    void DeleteSavestateFolder(string key);
    IReadOnlyList<string> GetPresetNames();
    IReadOnlyList<string> GetDefaultPresetNames();
    bool PresetNameExists(string name);
    void Save(string name, PromptConfiguration configuration);
    PromptConfiguration Load(string name);
    void Rename(string currentName, string newName);
    void Delete(string name);
}

public sealed record PresetSavestateFolder(string Key, string DisplayName, bool IsDefault, bool CanDelete);
