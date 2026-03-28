using System.IO;
using System.Text.Json;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed class ArtistProfileService : IArtistProfileService
{
    private readonly Dictionary<string, ArtistProfile> _profiles;
    private readonly List<string> _artistNames;

    public ArtistProfileService()
    {
        _profiles = LoadProfiles();
        _artistNames = BuildArtistNameList(_profiles);
    }

    public IReadOnlyList<string> GetArtistNames() => _artistNames;

    public ArtistProfile? GetProfile(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || string.Equals(name, "None", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (_profiles.TryGetValue(name, out var profile))
        {
            return profile;
        }

        var normalizedTarget = NormalizeKey(name);
        var normalizedMatch = _profiles.FirstOrDefault(pair => NormalizeKey(pair.Key) == normalizedTarget).Value;
        if (normalizedMatch is not null)
        {
            return normalizedMatch;
        }

        var surname = ExtractSurname(normalizedTarget);
        if (!string.IsNullOrWhiteSpace(surname))
        {
            var surnameMatch = _profiles.FirstOrDefault(pair => NormalizeKey(pair.Key).Contains(surname, StringComparison.OrdinalIgnoreCase)).Value;
            if (surnameMatch is not null)
            {
                return surnameMatch;
            }
        }

        return null;
    }

    private static Dictionary<string, ArtistProfile> LoadProfiles()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "artist_profiles.json");
        if (!File.Exists(path))
        {
            return new Dictionary<string, ArtistProfile>(StringComparer.OrdinalIgnoreCase);
        }

        var profiles = JsonSerializer.Deserialize<List<ArtistProfile>>(File.ReadAllText(path)) ?? [];
        return profiles.ToDictionary(profile => profile.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static List<string> BuildArtistNameList(Dictionary<string, ArtistProfile> profiles)
    {
        var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "None" };
        foreach (var name in profiles.Keys)
        {
            names.Add(name);
        }

        foreach (var name in LoadCachedArtistNames())
        {
            names.Add(name);
        }

        return names.OrderBy(name => name == "None" ? string.Empty : name, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static IEnumerable<string> LoadCachedArtistNames()
    {
        var manifestPath = FindCacheManifestPath();
        if (manifestPath is null)
        {
            yield break;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(manifestPath));
        if (!document.RootElement.TryGetProperty("artists", out var artists))
        {
            yield break;
        }

        foreach (var artist in artists.EnumerateArray())
        {
            if (artist.TryGetProperty("artist_name", out var nameElement))
            {
                var name = nameElement.GetString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    yield return name;
                }
            }
        }
    }

    private static string? FindCacheManifestPath()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "cache", "masterapollon", "artists_manifest.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            current = current.Parent;
        }

        return null;
    }

    private static string NormalizeKey(string value)
    {
        var chars = value.Normalize(System.Text.NormalizationForm.FormKD)
            .Where(ch => ch < 128)
            .Select(ch => char.ToLowerInvariant(ch))
            .Where(ch => char.IsLetterOrDigit(ch) || char.IsWhiteSpace(ch))
            .ToArray();

        return string.Join(' ', new string(chars).Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    private static string ExtractSurname(string normalizedName)
    {
        var parts = normalizedName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 0 ? string.Empty : parts[^1];
    }
}
