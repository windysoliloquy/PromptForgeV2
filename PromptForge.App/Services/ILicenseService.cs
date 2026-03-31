using PromptForge.App.Models;

namespace PromptForge.App.Services;

public interface ILicenseService
{
    bool IsUnlocked { get; }
    UnlockState CurrentState { get; }
    string PurchasePrice { get; }
    string PurchaseEmail { get; }
    string BuildPurchaseMailtoUri();
    UnlockImportResult ImportUnlockFile(string filePath);
    void Refresh();
}
