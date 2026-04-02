using PromptForge.App.Models;

namespace PromptForge.App.Services;

public interface IArtistPairGuidanceService
{
    ArtistPairMatrixMetadata MatrixMetadata { get; }
    ArtistPairLookupResult ResolvePair(string? primaryArtist, string? secondaryArtist);
    ArtistPairGuidance? GetGuidance(string? primaryArtist, string? secondaryArtist);
}
