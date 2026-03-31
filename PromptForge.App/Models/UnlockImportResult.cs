namespace PromptForge.App.Models;

public sealed class UnlockImportResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool CleanupSucceeded { get; set; }
}
