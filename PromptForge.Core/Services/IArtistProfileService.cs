using PromptForge.App.Models;

namespace PromptForge.App.Services;

public interface IArtistProfileService
{
    IReadOnlyList<string> GetArtistNames();
    ArtistProfile? GetProfile(string name);
}
