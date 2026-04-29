using System.IO;

namespace PromptForge.App.Services;

public sealed class SavestateFolderSelectionService : ISavestateFolderSelectionService
{
    private readonly string _settingsPath;

    public SavestateFolderSelectionService()
    {
        var settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PromptForge");
        Directory.CreateDirectory(settingsDirectory);
        _settingsPath = Path.Combine(settingsDirectory, "savestate-folder.txt");
    }

    public string? LoadSelectedFolderKey()
    {
        try
        {
            return File.Exists(_settingsPath)
                ? File.ReadAllText(_settingsPath).Trim()
                : null;
        }
        catch
        {
            return null;
        }
    }

    public void SaveSelectedFolderKey(string folderKey)
    {
        try
        {
            File.WriteAllText(_settingsPath, folderKey);
        }
        catch
        {
        }
    }
}
