using System;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    private static bool IsUsablePromptPhrase(string? phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return false;
        }

        return !IsPlaceholderPromptPhrase(phrase);
    }

    private static bool IsPlaceholderPromptPhrase(string phrase)
    {
        var cleaned = phrase.Trim();
        return cleaned.Equals("off", StringComparison.OrdinalIgnoreCase)
            || cleaned.Equals("omit explicit realism", StringComparison.OrdinalIgnoreCase)
            || cleaned.Equals("omit artist language", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeFallbackInterpretation(string sliderKey, string interpretation)
    {
        if (IsPlaceholderPromptPhrase(interpretation))
        {
            return ResolveNeutralFallbackPhrase(sliderKey, 0);
        }

        return interpretation.Trim();
    }

    private static string ResolveNeutralFallbackPhrase(string sliderKey, int value)
    {
        return sliderKey switch
        {
            Realism => value <= 20 ? "minimal realism emphasis" : string.Empty,
            ArtistInfluenceStrength => value <= 20 ? "no direct artist citation" : string.Empty,
            _ => string.Empty,
        };
    }
}
