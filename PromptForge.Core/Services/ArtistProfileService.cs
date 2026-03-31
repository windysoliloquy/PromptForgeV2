using System.IO;
using System.Reflection;
using System.Text;
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
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("PromptForge.App.Data.artist_profiles.json");
        if (stream is null)
        {
            return new Dictionary<string, ArtistProfile>(StringComparer.OrdinalIgnoreCase);
        }

        using var reader = new StreamReader(stream);
        var profiles = JsonSerializer.Deserialize<List<ArtistProfile>>(reader.ReadToEnd()) ?? [];
        return profiles
            .Select(CleanProfile)
            .GroupBy(profile => NormalizeKey(profile.Name), StringComparer.OrdinalIgnoreCase)
            .Select(group => group
                .OrderByDescending(profile => GetNameQualityScore(profile.Name))
                .ThenBy(profile => profile.Name, StringComparer.OrdinalIgnoreCase)
                .First())
            .ToDictionary(profile => profile.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static List<string> BuildArtistNameList(Dictionary<string, ArtistProfile> profiles)
    {
        var names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["none"] = "None"
        };

        foreach (var name in profiles.Keys)
        {
            names[NormalizeKey(name)] = name;
        }

        foreach (var name in LoadCachedArtistNames())
        {
            var cleanedName = CleanArtistName(name);
            var normalized = NormalizeKey(cleanedName);
            if (!names.ContainsKey(normalized))
            {
                names[normalized] = cleanedName;
            }
        }

        return names.Values
            .OrderBy(name => name == "None" ? string.Empty : NormalizeKey(name), StringComparer.OrdinalIgnoreCase)
            .ToList();
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

    private static ArtistProfile CleanProfile(ArtistProfile profile)
    {
        return new ArtistProfile
        {
            Name = CleanArtistName(profile.Name),
            Hallmarks = profile.Hallmarks,
            Composition = profile.Composition,
            Palette = profile.Palette,
            Surface = profile.Surface,
            Mood = profile.Mood
        };
    }

    private static string CleanArtistName(string value)
    {
        var cleaned = value.Trim();
        for (var i = 0; i < 3; i++)
        {
            if (!LooksMojibake(cleaned))
            {
                break;
            }

            var repaired = TryRepairMojibake(cleaned);
            if (repaired == cleaned)
            {
                break;
            }

            cleaned = repaired;
        }

        return cleaned;
    }

    private static string TryRepairMojibake(string value)
    {
        try
        {
            return Encoding.UTF8.GetString(Encoding.GetEncoding("ISO-8859-1").GetBytes(value));
        }
        catch (ArgumentException)
        {
            return value;
        }
    }

    private static bool LooksMojibake(string value)
    {
        return value.Contains('\u00C3')
            || value.Contains('\u00C2')
            || value.Contains('\u00D0')
            || value.Contains('\u00D1')
            || value.Contains('\uFFFD');
    }

    private static int GetNameQualityScore(string value)
    {
        var suspiciousPenalty = value.Count(ch => ch is '\u00C3' or '\u00C2' or '\u00D0' or '\u00D1' or '\uFFFD' or '?') * 10;
        var diacriticBonus = value.Count(ch => ch > 127 && !LooksMojibake(ch.ToString()));
        return diacriticBonus - suspiciousPenalty;
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
