using System.Linq;
using System.Text;

namespace PromptForge.App.Services;

internal static class ArtistNameNormalizer
{
    public static string CleanDisplayName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var cleaned = value.Trim();
        for (var i = 0; i < 3; i++)
        {
            if (!ContainsMojibake(cleaned))
            {
                break;
            }

            var repaired = RepairMojibake(cleaned);
            if (repaired == cleaned)
            {
                break;
            }

            cleaned = repaired;
        }

        return cleaned;
    }

    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var cleaned = CleanDisplayName(value);

        var builder = new StringBuilder();
        foreach (var ch in cleaned.Normalize(NormalizationForm.FormKD))
        {
            if (ch >= 128)
            {
                continue;
            }

            var lower = char.ToLowerInvariant(ch);
            if (char.IsLetterOrDigit(lower))
            {
                builder.Append(lower);
            }
            else if (char.IsWhiteSpace(lower) || char.IsPunctuation(lower) || char.IsSymbol(lower))
            {
                builder.Append(' ');
            }
        }

        return string.Join(' ', builder.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    public static string ToPairKey(string value)
    {
        var parts = Normalize(value).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join('_', parts);
    }

    public static IReadOnlyList<string> GetLookupCandidates(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Array.Empty<string>();
        }

        var candidates = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddCandidate(candidates, seen, NormalizeExplicitKey(value));
        AddCandidate(candidates, seen, ToPairKey(value));

        var parts = Normalize(value).Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 2)
        {
            var surname = parts[^1];
            var initials = parts
                .Take(parts.Length - 1)
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .Select(part => part[0].ToString())
                .ToArray();

            if (initials.Length > 0)
            {
                AddCandidate(candidates, seen, string.Join('_', initials.Append(surname)));
            }

            AddCandidate(candidates, seen, surname);
        }

        return candidates;
    }

    public static bool ContainsMojibake(string value)
    {
        return value.Any(IsMojibakeChar);
    }

    public static bool IsMojibakeChar(char ch)
    {
        return ch is '\u00C3' or '\u00C2' or '\u00D0' or '\u00D1' or '\uFFFD';
    }

    public static string NormalizeExplicitKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().ToLowerInvariant();
        normalized = string.Join('_', normalized
            .Split(new[] { ' ', '-', '.' }, StringSplitOptions.RemoveEmptyEntries));
        return normalized;
    }

    public static string RepairMojibake(string value)
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

    private static void AddCandidate(List<string> candidates, HashSet<string> seen, string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate) || !seen.Add(candidate))
        {
            return;
        }

        candidates.Add(candidate);
    }
}
