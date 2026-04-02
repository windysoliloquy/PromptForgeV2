namespace PromptForge.App.Models;

public sealed class ArtistPhraseParts
{
    public string Prefix { get; init; } = string.Empty;
    public string ArtistName { get; init; } = string.Empty;
    public string Suffix { get; init; } = string.Empty;
    public bool UsedExactMatch { get; init; }
    public string SourcePhrase { get; init; } = string.Empty;
}
