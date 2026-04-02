using System;
using System.Collections.Generic;
using System.Linq;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

internal static class ArtistPairTooltipFormatter
{
    private static readonly IReadOnlyDictionary<string, string> CategoryGuidance = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["near-overlap"] = "These artists mostly reinforce the same visual lane. Expect a tighter result rather than a broad hybrid.",
        ["complementary"] = "These artists usually blend cleanly and add useful variation without fighting for control.",
        ["tense-but-usable"] = "This pair can work, but it usually benefits from role separation. Let one artist dominate a specific domain.",
        ["high-conflict"] = "These artists pull the image in competing directions. The model may flatten, average out, or drop one influence unless the prompt is carefully steered.",
    };

    public static string FormatTooltip(ArtistPairLookupResult lookup, ArtistPairGuidance guidance)
    {
        var lines = new List<string>
        {
            $"{lookup.LeftResolvedName ?? lookup.LeftInput} + {lookup.RightResolvedName ?? lookup.RightInput}",
            $"Category: {guidance.Category}",
            $"General guidance: {GetCategoryGuidance(guidance)}",
        };

        var domainHint = InferDomainHint(guidance);
        if (!string.IsNullOrWhiteSpace(domainHint))
        {
            lines.Add($"Best split: {domainHint}");
        }

        lines.Add($"Effect on prompt generation: {CleanEffectText(guidance)}");
        lines.Add($"Likely model struggle: {CleanStruggleText(guidance)}");

        if (guidance.AffinityScore.HasValue || guidance.DifficultyScore.HasValue)
        {
            lines.Add($"Affinity: {guidance.AffinityScore?.ToString() ?? "N/A"} | Difficulty: {guidance.DifficultyScore?.ToString() ?? "N/A"}");
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static string GetCategoryGuidance(ArtistPairGuidance guidance)
    {
        return CategoryGuidance.TryGetValue(guidance.Category, out var text)
            ? text
            : "This blend may need a clear hierarchy so the two influences do not blur together.";
    }

    private static string CleanEffectText(ArtistPairGuidance guidance)
    {
        var text = FirstNonEmpty(guidance.EffectOnPromptGeneration, guidance.CategoryDefinition, "Pair guidance is not available for this selection.");
        text = ReplaceInsensitive(text, "Prompt can distribute ownership cleanly across adjacent traits. The result is usually additive rather than contradictory.", "Treat this as a flexible blend. You can usually combine both influences without a hard split.");
        text = ReplaceInsensitive(text, "This usually creates a readable hybrid instead of two competing instructions.", "This usually reads as a coherent hybrid instead of a fight between two styles.");
        text = ReplaceInsensitive(text, "Prompt should assign ownership explicitly, usually letting one artist drive ", "Give each artist a clearer job. Let one artist drive ");
        text = ReplaceInsensitive(text, "Without that split, the blend can wobble.", "Without that split, the blend can lose clarity.");
        text = ReplaceInsensitive(text, "Prompt needs hard domain separation because it is asking for ", "Use a strong split because the pair is asking for ");
        return NormalizeSentence(text);
    }

    private static string CleanStruggleText(ArtistPairGuidance guidance)
    {
        var text = FirstNonEmpty(guidance.WhatModelsStruggleWith, "The model may average or discard one artist without a strong dominance cue.");
        text = ReplaceInsensitive(text, "Main risk is generic averaging: ", string.Empty);
        text = ReplaceInsensitive(text, "Main risk is over-compression: ", string.Empty);
        text = ReplaceInsensitive(text, "Model will tend to average or drop one side", "The model may average out or drop one side");
        text = ReplaceInsensitive(text, "Model will usually flatten or ignore one artist", "The model may flatten or ignore one artist");
        text = ReplaceInsensitive(text, "the model may collapse the blend into a safer middle and lose whichever artist is weaker.", "The model may collapse the blend into a safer middle and mute the weaker influence.");
        text = ReplaceInsensitive(text, "the model may collapse the blend into one artist and flatten the distinction.", "The model may collapse the blend into one artist and erase the distinction.");
        return NormalizeSentence(text);
    }

    private static string? InferDomainHint(ArtistPairGuidance guidance)
    {
        var conflictText = string.Join(" | ", guidance.ConflictSignals);
        var effectText = guidance.EffectOnPromptGeneration ?? string.Empty;
        var sharedTraits = guidance.SharedTraits;

        if (ContainsAny(conflictText, "realism", "ornamental") || ContainsAny(effectText, "ornamental decorative design"))
        {
            return "realism vs ornament";
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

    private static bool ContainsAny(string text, params string[] needles)
    {
        return needles.Any(needle => text.Contains(needle, StringComparison.OrdinalIgnoreCase));
    }

    private static string FirstNonEmpty(params string?[] values)
    {
        return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value))?.Trim() ?? string.Empty;
    }

    private static string ReplaceInsensitive(string value, string oldValue, string newValue)
    {
        var index = value.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
        return index >= 0
            ? value.Remove(index, oldValue.Length).Insert(index, newValue)
            : value;
    }

    private static string NormalizeSentence(string value)
    {
        var cleaned = string.Join(" ", value.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries)).Trim();
        return cleaned.Length == 0 ? string.Empty : char.ToUpperInvariant(cleaned[0]) + cleaned[1..];
    }
}
