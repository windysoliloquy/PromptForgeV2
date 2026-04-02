using System;
using System.Collections.Generic;
using System.Linq;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

internal static class ArtistPhraseQuickInsertService
{
    private static readonly string[] PrimaryPrefixFragments =
    {
        "deeply informed by",
        "shaped by",
        "guided by",
        "anchored in",
    };

    private static readonly string[] SecondaryPrefixFragments =
    {
        "accented by",
        "supported by",
        "enriched by",
        "with subtle cues from",
    };

    private static readonly string[] PrimaryFallbackDomains =
    {
        "anchoring composition",
        "driving grounded realism",
        "defining palette",
        "steering atmosphere",
    };

    private static readonly string[] SecondaryFallbackDomains =
    {
        "supporting atmosphere",
        "enriching palette",
        "adding subtle narrative cues",
        "softening finish",
    };

    public static IReadOnlyList<ArtistPhraseQuickInsertGroup> BuildGroups(bool isPrimarySlot, ArtistPairLookupResult? pairLookup)
    {
        var groups = new List<ArtistPhraseQuickInsertGroup>();
        var guidance = pairLookup?.Guidance;
        var derivedDomains = guidance is null ? new List<string>() : DeriveDomains(guidance);
        var splitTerms = guidance is null ? Array.Empty<string>() : DeriveSplitTerms(guidance);

        if (derivedDomains.Count > 0)
        {
            groups.Add(new ArtistPhraseQuickInsertGroup
            {
                Title = "Suggested for this pair",
                Inserts = BuildDomainInserts(derivedDomains.Take(4), isPrimarySlot, true),
            });
        }

        if (splitTerms.Length > 0)
        {
            groups.Add(new ArtistPhraseQuickInsertGroup
            {
                Title = "Best split helpers",
                Inserts = BuildSplitInserts(splitTerms, isPrimarySlot),
            });
        }

        groups.Add(new ArtistPhraseQuickInsertGroup
        {
            Title = "Accent helpers",
            Inserts = BuildAccentInserts(derivedDomains, isPrimarySlot),
        });

        groups.Add(new ArtistPhraseQuickInsertGroup
        {
            Title = "Prefix framing helpers",
            Inserts = BuildPrefixInserts(isPrimarySlot),
        });

        return groups.Where(group => group.Inserts.Count > 0).ToArray();
    }

    private static IReadOnlyList<ArtistPhraseQuickInsert> BuildPrefixInserts(bool isPrimarySlot)
    {
        return (isPrimarySlot ? PrimaryPrefixFragments : SecondaryPrefixFragments)
            .Select(fragment => new ArtistPhraseQuickInsert
            {
                Label = fragment,
                Fragment = fragment,
                Target = ArtistPhraseInsertTarget.Prefix,
            })
            .ToArray();
    }

    private static IReadOnlyList<ArtistPhraseQuickInsert> BuildAccentInserts(IReadOnlyList<string> derivedDomains, bool isPrimarySlot)
    {
        var phrases = new List<string>();
        if (derivedDomains.Count > 0)
        {
            phrases.AddRange(BuildRolePhrases(derivedDomains.Take(3), isPrimarySlot));
        }

        phrases.AddRange(isPrimarySlot ? PrimaryFallbackDomains : SecondaryFallbackDomains);

        return phrases
            .Select(fragment => new ArtistPhraseQuickInsert
            {
                Label = fragment,
                Fragment = fragment,
                Target = ArtistPhraseInsertTarget.Suffix,
                IsPairAware = derivedDomains.Count > 0 && derivedDomains.Any(domain => fragment.Contains(domain, StringComparison.OrdinalIgnoreCase)),
                RoleStem = TryExtractRoleStem(fragment),
                DomainLabel = TryExtractDomainLabel(fragment),
            })
            .GroupBy(insert => insert.Label, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Take(4)
            .ToArray();
    }

    private static IReadOnlyList<ArtistPhraseQuickInsert> BuildSplitInserts(IReadOnlyList<string> splitTerms, bool isPrimarySlot)
    {
        var preferred = splitTerms.ElementAtOrDefault(isPrimarySlot ? 0 : 1) ?? splitTerms.FirstOrDefault();
        var secondary = splitTerms.FirstOrDefault(term => !string.Equals(term, preferred, StringComparison.OrdinalIgnoreCase));
        var phrases = new List<string>();

        if (!string.IsNullOrWhiteSpace(preferred))
        {
            phrases.AddRange(BuildRolePhrases(new[] { preferred }, isPrimarySlot));
        }

        if (!string.IsNullOrWhiteSpace(secondary))
        {
            phrases.AddRange(BuildRolePhrases(new[] { secondary }, isPrimarySlot));
        }

        return phrases
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(fragment => new ArtistPhraseQuickInsert
            {
                Label = fragment,
                Fragment = fragment,
                Target = ArtistPhraseInsertTarget.Suffix,
                IsPairAware = true,
                RoleStem = TryExtractRoleStem(fragment),
                DomainLabel = TryExtractDomainLabel(fragment),
            })
            .Take(4)
            .ToArray();
    }

    private static IReadOnlyList<ArtistPhraseQuickInsert> BuildDomainInserts(IEnumerable<string> domains, bool isPrimarySlot, bool isPairAware)
    {
        return BuildRolePhrases(domains, isPrimarySlot)
            .Select(fragment => new ArtistPhraseQuickInsert
            {
                Label = fragment,
                Fragment = fragment,
                Target = ArtistPhraseInsertTarget.Suffix,
                IsPairAware = isPairAware,
                RoleStem = TryExtractRoleStem(fragment),
                DomainLabel = TryExtractDomainLabel(fragment),
            })
            .GroupBy(insert => insert.Label, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Take(5)
            .ToArray();
    }

    private static IEnumerable<string> BuildRolePhrases(IEnumerable<string> domains, bool isPrimarySlot)
    {
        foreach (var domain in domains.Where(domain => !string.IsNullOrWhiteSpace(domain)).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var cleaned = domain.Trim();
            if (isPrimarySlot)
            {
                if (cleaned.Contains("realism", StringComparison.OrdinalIgnoreCase))
                {
                    yield return "driving grounded realism";
                    continue;
                }

                if (cleaned.Contains("palette", StringComparison.OrdinalIgnoreCase))
                {
                    yield return "defining palette";
                    continue;
                }

                if (cleaned.Contains("atmosphere", StringComparison.OrdinalIgnoreCase))
                {
                    yield return "steering atmosphere";
                    continue;
                }

                if (cleaned.Contains("composition", StringComparison.OrdinalIgnoreCase) || cleaned.Contains("staging", StringComparison.OrdinalIgnoreCase))
                {
                    yield return $"anchoring {cleaned}";
                    continue;
                }

                yield return $"defining {cleaned}";
            }
            else
            {
                if (cleaned.Contains("narrative", StringComparison.OrdinalIgnoreCase) || cleaned.Contains("story", StringComparison.OrdinalIgnoreCase))
                {
                    yield return "adding subtle narrative cues";
                    continue;
                }

                if (cleaned.Contains("atmosphere", StringComparison.OrdinalIgnoreCase))
                {
                    yield return "supporting atmosphere";
                    continue;
                }

                if (cleaned.Contains("finish", StringComparison.OrdinalIgnoreCase) || cleaned.Contains("surface", StringComparison.OrdinalIgnoreCase))
                {
                    yield return $"softening {cleaned}";
                    continue;
                }

                yield return $"supporting {cleaned}";
            }
        }
    }

    private static List<string> DeriveDomains(ArtistPairGuidance guidance)
    {
        var domains = new List<string>();
        var text = string.Join(
            " | ",
            new[]
            {
                guidance.EffectOnPromptGeneration,
                guidance.WhatModelsStruggleWith,
                string.Join(" | ", guidance.SharedTraits),
                string.Join(" | ", guidance.ConflictSignals),
            });

        AddDomain(domains, text, "realism", "grounded realism");
        AddDomain(domains, text, "symbol", "symbolic or dream logic");
        AddDomain(domains, text, "dream", "symbolic or dream logic");
        AddDomain(domains, text, "narrative", "illustrative storytelling");
        AddDomain(domains, text, "story", "illustrative storytelling");
        AddDomain(domains, text, "line", "line-led drawing control");
        AddDomain(domains, text, "drawing", "line-led drawing control");
        AddDomain(domains, text, "atmos", "soft atmospheric transitions");
        AddDomain(domains, text, "ornament", "ornament");
        AddDomain(domains, text, "decorative", "decorative design");
        AddDomain(domains, text, "composition", "composition");
        AddDomain(domains, text, "staging", "staging");
        AddDomain(domains, text, "palette", "palette");
        AddDomain(domains, text, "color", "palette");
        AddDomain(domains, text, "finish", "finish");
        AddDomain(domains, text, "surface", "finish");
        AddDomain(domains, text, "graphic", "graphic/print discipline");
        AddDomain(domains, text, "print", "graphic/print discipline");

        foreach (var splitTerm in DeriveSplitTerms(guidance))
        {
            if (!domains.Contains(splitTerm, StringComparer.OrdinalIgnoreCase))
            {
                domains.Add(splitTerm);
            }
        }

        return domains;
    }

    private static string[] DeriveSplitTerms(ArtistPairGuidance guidance)
    {
        var split = InferDomainHint(guidance);
        return string.IsNullOrWhiteSpace(split)
            ? Array.Empty<string>()
            : split.Split(" vs ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private static string? InferDomainHint(ArtistPairGuidance guidance)
    {
        var conflictText = string.Join(" | ", guidance.ConflictSignals);
        var effectText = guidance.EffectOnPromptGeneration ?? string.Empty;
        var sharedTraits = guidance.SharedTraits;

        if (ContainsAny(conflictText, "realism", "ornamental") || ContainsAny(effectText, "ornamental decorative design"))
        {
            return "grounded realism vs ornament";
        }

        if (ContainsAny(conflictText, "line", "atmospheric") || (ContainsAny(effectText, "line-led drawing control") && ContainsAny(effectText, "soft atmospheric transitions")))
        {
            return "line discipline vs atmosphere";
        }

        if (ContainsAny(conflictText, "panoramic density", "minimal staging") || (ContainsAny(effectText, "portrait-first staging") && ContainsAny(effectText, "smooth polished finish", "rough tactile paint")))
        {
            return "staging vs finish";
        }

        if ((sharedTraits.Any(trait => ContainsAny(trait, "palette", "color")) && ContainsAny(effectText, "composition", "staging"))
            || (ContainsAny(conflictText, "staging") && sharedTraits.Any(trait => ContainsAny(trait, "color", "palette"))))
        {
            return "composition vs palette";
        }

        return null;
    }

    private static void AddDomain(ICollection<string> domains, string text, string keyword, string domain)
    {
        if (text.Contains(keyword, StringComparison.OrdinalIgnoreCase) && !domains.Contains(domain, StringComparer.OrdinalIgnoreCase))
        {
            domains.Add(domain);
        }
    }

    private static bool ContainsAny(string text, params string[] needles)
    {
        return needles.Any(needle => text.Contains(needle, StringComparison.OrdinalIgnoreCase));
    }

    private static string? TryExtractRoleStem(string fragment)
    {
        var separators = new[]
        {
            " ",
        };

        foreach (var separator in separators)
        {
            var index = fragment.IndexOf(separator, StringComparison.Ordinal);
            if (index > 0)
            {
                return fragment[..index].Trim();
            }
        }

        return null;
    }

    private static string? TryExtractDomainLabel(string fragment)
    {
        var index = fragment.IndexOf(' ');
        return index > 0 && index < fragment.Length - 1
            ? fragment[(index + 1)..].Trim()
            : null;
    }
}
