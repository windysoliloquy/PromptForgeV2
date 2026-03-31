namespace PromptForge.App.Models;

public sealed class ArtistProfile
{
    public string Name { get; set; } = string.Empty;
    public string[] Hallmarks { get; set; } = [];
    public string[] Composition { get; set; } = [];
    public string[] Palette { get; set; } = [];
    public string[] Surface { get; set; } = [];
    public string[] Mood { get; set; } = [];
}
