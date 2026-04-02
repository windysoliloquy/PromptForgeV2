using System.Text.RegularExpressions;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class ArtistPhraseComposer
{
    public static string BuildGeneratedPhrase(string artistName, int strength, bool hasProfile, string? intentMode = null)
    {
        var cleanedArtistName = Clean(artistName);
        if (strength <= 20 || string.IsNullOrWhiteSpace(cleanedArtistName) || string.Equals(cleanedArtistName, "None", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return SliderLanguageCatalog.ResolveArtistInfluenceDescriptor(strength, cleanedArtistName, intentMode);
    }

    public static string BuildFinalPhrase(string artistName, int strength, bool hasProfile, ArtistPhraseOverride? phraseOverride, string? intentMode = null)
    {
        var cleanedArtistName = Clean(artistName);
        if (string.IsNullOrWhiteSpace(cleanedArtistName))
        {
            return string.Empty;
        }

        if (phraseOverride?.IsEnabled == true
            && !string.IsNullOrWhiteSpace(phraseOverride.ArtistName)
            && string.Equals(Clean(phraseOverride.ArtistName), cleanedArtistName, StringComparison.OrdinalIgnoreCase))
        {
            return Combine(phraseOverride.Prefix, cleanedArtistName, phraseOverride.Suffix);
        }

        return BuildGeneratedPhrase(cleanedArtistName, strength, hasProfile, intentMode);
    }

    public static ArtistPhraseParts SplitPhrase(string phrase, string artistName)
    {
        var sourcePhrase = phrase?.Trim() ?? string.Empty;
        var cleanedArtistName = Clean(artistName);
        if (string.IsNullOrWhiteSpace(cleanedArtistName))
        {
            return new ArtistPhraseParts { SourcePhrase = sourcePhrase };
        }

        var index = sourcePhrase.IndexOf(cleanedArtistName, StringComparison.OrdinalIgnoreCase);
        if (index >= 0)
        {
            return new ArtistPhraseParts
            {
                Prefix = sourcePhrase[..index],
                ArtistName = sourcePhrase.Substring(index, cleanedArtistName.Length),
                Suffix = sourcePhrase[(index + cleanedArtistName.Length)..],
                UsedExactMatch = true,
                SourcePhrase = sourcePhrase,
            };
        }

        return new ArtistPhraseParts
        {
            Prefix = string.Empty,
            ArtistName = cleanedArtistName,
            Suffix = string.Empty,
            UsedExactMatch = false,
            SourcePhrase = sourcePhrase,
        };
    }

    public static string Combine(string? prefix, string artistName, string? suffix)
    {
        var cleanPrefix = NormalizeWhitespace(prefix ?? string.Empty);
        var cleanArtistName = Clean(artistName);
        var cleanSuffix = NormalizeWhitespace(suffix ?? string.Empty).Trim().TrimStart(',', ';', '-', '—');

        if (string.IsNullOrWhiteSpace(cleanArtistName))
        {
            return NormalizeWhitespace($"{cleanPrefix} {cleanSuffix}");
        }

        if (string.IsNullOrWhiteSpace(cleanSuffix))
        {
            return NormalizeWhitespace($"{cleanPrefix} {cleanArtistName}".Trim());
        }

        var lead = NormalizeWhitespace($"{cleanPrefix} {cleanArtistName}".Trim());
        return string.IsNullOrWhiteSpace(lead)
            ? cleanSuffix
            : $"{lead}, {cleanSuffix}";
    }

    public static string AppendFragment(string? existing, string fragment, ArtistPhraseInsertTarget target)
    {
        var cleanExisting = existing?.Trim() ?? string.Empty;
        var cleanFragment = NormalizeWhitespace(fragment).Trim().Trim(',', ';');
        if (string.IsNullOrWhiteSpace(cleanFragment))
        {
            return cleanExisting;
        }

        if (cleanExisting.Contains(cleanFragment, StringComparison.OrdinalIgnoreCase))
        {
            return target == ArtistPhraseInsertTarget.Prefix && !cleanExisting.EndsWith(' ')
                ? $"{cleanExisting} "
                : cleanExisting;
        }

        if (target == ArtistPhraseInsertTarget.Prefix)
        {
            var combined = string.IsNullOrWhiteSpace(cleanExisting)
                ? cleanFragment
                : $"{cleanExisting} {cleanFragment}";
            return $"{NormalizeWhitespace(combined)} ";
        }

        if (string.IsNullOrWhiteSpace(cleanExisting))
        {
            return cleanFragment;
        }

        var separator = cleanExisting.EndsWith(',') || cleanExisting.EndsWith(';') ? " " : ", ";
        return NormalizeWhitespace($"{cleanExisting}{separator}{cleanFragment}");
    }

    public static string RenderStructuredSuffix(IReadOnlyList<ArtistPhraseSuffixRoleGroup> roleGroups, string? trailingText)
    {
        var clauses = roleGroups
            .Where(group => !string.IsNullOrWhiteSpace(group.RoleStem) && group.Domains.Count > 0)
            .Select(group => $"{group.RoleStem} {FormatList(group.Domains)}")
            .ToList();

        var cleanTrailingText = NormalizeWhitespace(trailingText ?? string.Empty).Trim().TrimStart(',', ';');
        if (!string.IsNullOrWhiteSpace(cleanTrailingText))
        {
            clauses.Add(cleanTrailingText);
        }

        if (clauses.Count == 0)
        {
            return string.Empty;
        }

        if (clauses.Count == 1)
        {
            return clauses[0];
        }

        return $"{clauses[0]}; {string.Join("; ", clauses.Skip(1))}";
    }

    public static IReadOnlyList<ArtistPhraseSuffixRoleGroup> AddStructuredSuffixInsert(
        IReadOnlyList<ArtistPhraseSuffixRoleGroup> existingGroups,
        string roleStem,
        string domainLabel)
    {
        var cleanRoleStem = NormalizeWhitespace(roleStem ?? string.Empty);
        var cleanDomain = NormalizeWhitespace(domainLabel ?? string.Empty);
        if (string.IsNullOrWhiteSpace(cleanRoleStem) || string.IsNullOrWhiteSpace(cleanDomain))
        {
            return existingGroups;
        }

        var groups = existingGroups
            .Select(group => new ArtistPhraseSuffixRoleGroup
            {
                RoleStem = group.RoleStem,
                Domains = group.Domains.ToList(),
            })
            .ToList();

        var matchingGroup = groups.FirstOrDefault(group => string.Equals(group.RoleStem, cleanRoleStem, StringComparison.OrdinalIgnoreCase));
        if (matchingGroup is null)
        {
            groups.Add(new ArtistPhraseSuffixRoleGroup
            {
                RoleStem = cleanRoleStem,
                Domains = new[] { cleanDomain },
            });

            return groups;
        }

        if (matchingGroup.Domains.Contains(cleanDomain, StringComparer.OrdinalIgnoreCase))
        {
            return groups;
        }

        var updatedDomains = matchingGroup.Domains.ToList();
        updatedDomains.Add(cleanDomain);

        var index = groups.IndexOf(matchingGroup);
        groups[index] = new ArtistPhraseSuffixRoleGroup
        {
            RoleStem = matchingGroup.RoleStem,
            Domains = updatedDomains,
        };

        return groups;
    }

    public static string MergeStructuredSuffixWithManualText(
        IReadOnlyList<ArtistPhraseSuffixRoleGroup> roleGroups,
        string previousRenderedSuffix,
        string currentSuffixText)
    {
        var normalizedCurrent = NormalizeWhitespace(currentSuffixText ?? string.Empty);
        var normalizedPreviousRendered = NormalizeWhitespace(previousRenderedSuffix ?? string.Empty);

        if (!string.Equals(normalizedCurrent, normalizedPreviousRendered, StringComparison.Ordinal))
        {
            return normalizedCurrent;
        }

        return RenderStructuredSuffix(roleGroups, string.Empty);
    }

    private static string Clean(string? value)
    {
        return NormalizeWhitespace(value?.Trim() ?? string.Empty);
    }

    private static string FormatList(IReadOnlyList<string> values)
    {
        var cleaned = values
            .Select(value => NormalizeWhitespace(value))
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        return cleaned.Count switch
        {
            0 => string.Empty,
            1 => cleaned[0],
            2 => $"{cleaned[0]} and {cleaned[1]}",
            _ => $"{string.Join(", ", cleaned.Take(cleaned.Count - 1))}, and {cleaned[^1]}",
        };
    }

    private static string NormalizeWhitespace(string value)
    {
        return Regex.Replace(value, "\\s+", " ").Trim();
    }
}
