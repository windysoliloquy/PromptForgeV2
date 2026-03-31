namespace PromptForge.App.Models;

public sealed class PromptResult
{
    public string PositivePrompt { get; init; } = string.Empty;
    public string NegativePrompt { get; init; } = string.Empty;
}
