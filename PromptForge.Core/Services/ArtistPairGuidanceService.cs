using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed class ArtistPairGuidanceService : IArtistPairGuidanceService
{
    private const string ResourceName = "PromptForge.App.Data.prompt_forge_artist_pair_matrix.json";

    private readonly Dictionary<string, ArtistPairGuidance> _pairs;
    private readonly Dictionary<string, string> _aliasToArtistKey;
    private readonly Dictionary<string, string> _artistKeyToDisplayName;

    public ArtistPairGuidanceService()
    {
        var document = LoadDocument();
        MatrixMetadata = BuildMetadata(document);
        (_pairs, _aliasToArtistKey, _artistKeyToDisplayName) = LoadPairs(document);

        Trace.WriteLine(
            $"[ArtistPairGuidance] Loaded {MatrixMetadata.ResourceName} from {MatrixMetadata.AssemblyPath} | " +
            $"schema_version={MatrixMetadata.SchemaVersion}, source_artist_count={MatrixMetadata.SourceArtistCount}, pair_count={MatrixMetadata.PairCount}");
    }

    public ArtistPairMatrixMetadata MatrixMetadata { get; }

    public ArtistPairLookupResult ResolvePair(string? primaryArtist, string? secondaryArtist)
    {
        var leftInput = primaryArtist ?? string.Empty;
        var rightInput = secondaryArtist ?? string.Empty;
        var leftKey = ResolveArtistKey(leftInput);
        var rightKey = ResolveArtistKey(rightInput);

        ArtistPairGuidance? guidance = null;
        if (!string.IsNullOrWhiteSpace(leftKey) && !string.IsNullOrWhiteSpace(rightKey))
        {
            _pairs.TryGetValue(BuildOrderedPairKey(leftKey, rightKey), out guidance);
        }

        return new ArtistPairLookupResult
        {
            Guidance = guidance,
            LeftInput = leftInput,
            RightInput = rightInput,
            LeftResolvedKey = leftKey,
            RightResolvedKey = rightKey,
            LeftResolvedName = ResolveDisplayName(leftKey, leftInput),
            RightResolvedName = ResolveDisplayName(rightKey, rightInput),
        };
    }

    public ArtistPairGuidance? GetGuidance(string? primaryArtist, string? secondaryArtist)
    {
        return ResolvePair(primaryArtist, secondaryArtist).Guidance;
    }

    private static (Dictionary<string, ArtistPairGuidance> Pairs, Dictionary<string, string> Aliases, Dictionary<string, string> DisplayNames) LoadPairs(ArtistPairMatrixDocument? document)
    {
        var pairs = new Dictionary<string, ArtistPairGuidance>(StringComparer.OrdinalIgnoreCase);
        var displayNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var aliasCandidates = new Dictionary<string, AliasResolution>(StringComparer.OrdinalIgnoreCase);
        if (document?.Pairs is null)
        {
            return (pairs, new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase), displayNames);
        }

        var definitions = document.CategoryDefinitions ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in document.Pairs)
        {
            if (string.IsNullOrWhiteSpace(entry.ArtistA) || string.IsNullOrWhiteSpace(entry.ArtistB))
            {
                continue;
            }

            var guidance = new ArtistPairGuidance
            {
                Category = entry.Category ?? string.Empty,
                EffectOnPromptGeneration = entry.EffectOnPromptGeneration?.Trim() ?? string.Empty,
                WhatModelsStruggleWith = entry.WhatModelsStruggleWith?.Trim() ?? string.Empty,
                SharedTraits = entry.SharedTraits?.Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value.Trim()).ToArray() ?? Array.Empty<string>(),
                ConflictSignals = entry.ConflictSignals?.Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value.Trim()).ToArray() ?? Array.Empty<string>(),
                AffinityScore = entry.AffinityScore,
                DifficultyScore = entry.DifficultyScore,
                CategoryDefinition = !string.IsNullOrWhiteSpace(entry.Category) && definitions.TryGetValue(entry.Category, out var definition)
                    ? definition
                    : null,
            };

            var leftKey = GetCanonicalArtistKey(entry.ArtistAKey, entry.ArtistA);
            var rightKey = GetCanonicalArtistKey(entry.ArtistBKey, entry.ArtistB);
            AddIfMissing(pairs, guidance, leftKey, rightKey);

            RegisterArtist(entry.ArtistA, leftKey, displayNames, aliasCandidates);
            RegisterArtist(entry.ArtistB, rightKey, displayNames, aliasCandidates);
        }

        var aliases = aliasCandidates
            .Where(pair => pair.Value.IsUsable && !string.IsNullOrWhiteSpace(pair.Value.ArtistKey))
            .ToDictionary(pair => pair.Key, pair => pair.Value.ArtistKey!, StringComparer.OrdinalIgnoreCase);

        foreach (var alias in document.ArtistAliases ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase))
        {
            RegisterAlias(aliasCandidates, ArtistNameNormalizer.NormalizeExplicitKey(alias.Key), alias.Value, 500);
            RegisterAlias(aliasCandidates, ArtistNameNormalizer.ToPairKey(alias.Key), alias.Value, 500);
        }

        aliases = aliasCandidates
            .Where(pair => pair.Value.IsUsable && !string.IsNullOrWhiteSpace(pair.Value.ArtistKey))
            .ToDictionary(pair => pair.Key, pair => pair.Value.ArtistKey!, StringComparer.OrdinalIgnoreCase);

        return (pairs, aliases, displayNames);
    }

    private static void AddIfMissing(Dictionary<string, ArtistPairGuidance> pairs, ArtistPairGuidance guidance, string left, string right)
    {
        if (string.IsNullOrWhiteSpace(left) || string.IsNullOrWhiteSpace(right))
        {
            return;
        }

        var key = BuildOrderedPairKey(left, right);
        if (!pairs.ContainsKey(key))
        {
            pairs[key] = guidance;
        }
    }

    private static ArtistPairMatrixDocument? LoadDocument()
    {
        using var stream = typeof(ArtistPairGuidanceService).Assembly.GetManifestResourceStream(ResourceName);
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        return JsonSerializer.Deserialize<ArtistPairMatrixDocument>(reader.ReadToEnd(), options);
    }

    private string? ResolveArtistKey(string? name)
    {
        foreach (var candidate in ArtistNameNormalizer.GetLookupCandidates(name))
        {
            if (_artistKeyToDisplayName.ContainsKey(candidate))
            {
                return candidate;
            }

            if (_aliasToArtistKey.TryGetValue(candidate, out var artistKey))
            {
                return artistKey;
            }
        }

        return null;
    }

    private string ResolveDisplayName(string? artistKey, string input)
    {
        if (!string.IsNullOrWhiteSpace(artistKey) && !string.IsNullOrWhiteSpace(input))
        {
            return ArtistNameNormalizer.CleanDisplayName(input);
        }

        if (!string.IsNullOrWhiteSpace(artistKey) && _artistKeyToDisplayName.TryGetValue(artistKey, out var displayName))
        {
            return displayName;
        }

        return ArtistNameNormalizer.CleanDisplayName(input);
    }

    private static string BuildOrderedPairKey(string left, string right)
    {
        return string.Compare(left, right, StringComparison.OrdinalIgnoreCase) <= 0
            ? $"{left}|{right}"
            : $"{right}|{left}";
    }

    private static string GetCanonicalArtistKey(string? explicitKey, string displayName)
    {
        var normalizedKey = ArtistNameNormalizer.NormalizeExplicitKey(explicitKey ?? string.Empty);
        return string.IsNullOrWhiteSpace(normalizedKey)
            ? ArtistNameNormalizer.ToPairKey(displayName)
            : normalizedKey;
    }

    private static void RegisterArtist(
        string displayName,
        string artistKey,
        Dictionary<string, string> displayNames,
        Dictionary<string, AliasResolution> aliasCandidates)
    {
        if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(artistKey))
        {
            return;
        }

        var cleanedDisplayName = ArtistNameNormalizer.CleanDisplayName(displayName);
        displayNames.TryAdd(artistKey, cleanedDisplayName);

        RegisterAlias(aliasCandidates, artistKey, artistKey, 400);
        RegisterAlias(aliasCandidates, ArtistNameNormalizer.ToPairKey(cleanedDisplayName), artistKey, 300);

        foreach (var candidate in ArtistNameNormalizer.GetLookupCandidates(cleanedDisplayName).Skip(2))
        {
            RegisterAlias(aliasCandidates, candidate, artistKey, candidate.Contains('_', StringComparison.Ordinal) ? 200 : 100);
        }
    }

    private static void RegisterAlias(Dictionary<string, AliasResolution> aliases, string alias, string artistKey, int score)
    {
        if (string.IsNullOrWhiteSpace(alias) || string.IsNullOrWhiteSpace(artistKey))
        {
            return;
        }

        if (!aliases.TryGetValue(alias, out var existing))
        {
            aliases[alias] = new AliasResolution(artistKey, score, true);
            return;
        }

        if (string.Equals(existing.ArtistKey, artistKey, StringComparison.OrdinalIgnoreCase))
        {
            if (score > existing.Score)
            {
                aliases[alias] = new AliasResolution(artistKey, score, true);
            }

            return;
        }

        if (score > existing.Score)
        {
            aliases[alias] = new AliasResolution(artistKey, score, true);
            return;
        }

        if (score == existing.Score)
        {
            aliases[alias] = new AliasResolution(null, score, false);
        }
    }

    private static ArtistPairMatrixMetadata BuildMetadata(ArtistPairMatrixDocument? document)
    {
        var assembly = typeof(ArtistPairGuidanceService).Assembly;
        return new ArtistPairMatrixMetadata
        {
            ResourceName = ResourceName,
            AssemblyPath = assembly.Location,
            SourcePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "PromptForge.Core", "Data", "prompt_forge_artist_pair_matrix.json")),
            SchemaVersion = document?.SchemaVersion ?? string.Empty,
            SourceArtistCount = document?.SourceArtistCount ?? 0,
            PairCount = document?.PairCount ?? 0,
        };
    }

    private sealed class ArtistPairMatrixDocument
    {
        [JsonPropertyName("schema_version")]
        public string? SchemaVersion { get; set; }

        [JsonPropertyName("source_artist_count")]
        public int? SourceArtistCount { get; set; }

        [JsonPropertyName("pair_count")]
        public int? PairCount { get; set; }

        [JsonPropertyName("artist_aliases")]
        public Dictionary<string, string>? ArtistAliases { get; set; }

        [JsonPropertyName("category_definitions")]
        public Dictionary<string, string>? CategoryDefinitions { get; set; }

        [JsonPropertyName("pairs")]
        public List<ArtistPairMatrixEntry>? Pairs { get; set; }
    }

    private sealed class ArtistPairMatrixEntry
    {
        [JsonPropertyName("artist_a")]
        public string? ArtistA { get; set; }

        [JsonPropertyName("artist_b")]
        public string? ArtistB { get; set; }

        [JsonPropertyName("artist_a_key")]
        public string? ArtistAKey { get; set; }

        [JsonPropertyName("artist_b_key")]
        public string? ArtistBKey { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("affinity_score")]
        public int? AffinityScore { get; set; }

        [JsonPropertyName("difficulty_score")]
        public int? DifficultyScore { get; set; }

        [JsonPropertyName("shared_traits")]
        public List<string>? SharedTraits { get; set; }

        [JsonPropertyName("conflict_signals")]
        public List<string>? ConflictSignals { get; set; }

        [JsonPropertyName("effect_on_prompt_generation")]
        public string? EffectOnPromptGeneration { get; set; }

        [JsonPropertyName("what_models_struggle_with")]
        public string? WhatModelsStruggleWith { get; set; }
    }

    private sealed record AliasResolution(string? ArtistKey, int Score, bool IsUsable);
}
