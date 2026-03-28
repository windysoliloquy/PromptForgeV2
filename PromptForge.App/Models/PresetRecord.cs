namespace PromptForge.App.Models;

public sealed class PresetRecord
{
    public string Name { get; set; } = string.Empty;
    public DateTime SavedAtUtc { get; set; }
    public PromptConfiguration Configuration { get; set; } = new();
}
