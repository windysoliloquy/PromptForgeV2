using PromptForge.App.Models;

namespace PromptForge.App.Services;

public interface ILicenseService
{
    bool IsUnlocked { get; }
    UnlockState CurrentState { get; }
    bool HasAllowedLane(string? intentMode);
    string PurchasePrice { get; }
    string PurchaseEmail { get; }
    string GetActivationRequestCode();
    string BuildPurchaseMailtoUri();
    UnlockImportResult ImportUnlockFile(string filePath);
    void Refresh();
}
