namespace PromptForge.App.Services;

public interface ISavestateFolderSelectionService
{
    string? LoadSelectedFolderKey();
    void SaveSelectedFolderKey(string folderKey);
}
