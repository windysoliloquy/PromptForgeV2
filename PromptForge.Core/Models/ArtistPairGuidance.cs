namespace PromptForge.App.Models;

public sealed class ArtistPairGuidance
{
    public string Category { get; set; } = string.Empty;
    public string EffectOnPromptGeneration { get; set; } = string.Empty;
    public string WhatModelsStruggleWith { get; set; } = string.Empty;
    public IReadOnlyList<string> SharedTraits { get; set; } = Array.Empty<string>();
    public IReadOnlyList<string> ConflictSignals { get; set; } = Array.Empty<string>();
    public int? AffinityScore { get; set; }
    public int? DifficultyScore { get; set; }
    public string? CategoryDefinition { get; set; }
}
