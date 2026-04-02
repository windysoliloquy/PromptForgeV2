namespace PromptForge.App.Models;

public sealed class ArtistPairMatrixMetadata
{
    public string ResourceName { get; init; } = string.Empty;
    public string AssemblyPath { get; init; } = string.Empty;
    public string SourcePath { get; init; } = string.Empty;
    public string SchemaVersion { get; init; } = string.Empty;
    public int SourceArtistCount { get; init; }
    public int PairCount { get; init; }
}
