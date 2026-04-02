namespace PromptForge.App.Models;

public sealed class ArtistPairLookupResult
{
    public ArtistPairGuidance? Guidance { get; init; }
    public string LeftInput { get; init; } = string.Empty;
    public string RightInput { get; init; } = string.Empty;
    public string? LeftResolvedKey { get; init; }
    public string? RightResolvedKey { get; init; }
    public string? LeftResolvedName { get; init; }
    public string? RightResolvedName { get; init; }
    public bool LeftArtistRecognized => !string.IsNullOrWhiteSpace(LeftResolvedKey);
    public bool RightArtistRecognized => !string.IsNullOrWhiteSpace(RightResolvedKey);
    public bool PairFound => Guidance is not null;
}
