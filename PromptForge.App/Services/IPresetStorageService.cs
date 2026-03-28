using PromptForge.App.Models;

namespace PromptForge.App.Services;

public interface IPresetStorageService
{
    IReadOnlyList<string> GetPresetNames();
    void Save(string name, PromptConfiguration configuration);
    PromptConfiguration Load(string name);
    void Rename(string currentName, string newName);
    void Delete(string name);
}
