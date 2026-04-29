using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    private static string[] BuildResolvedPhrasePool(SliderLanguageDefinition definition, string sliderKey, int bandIndex, PromptConfiguration configuration)
    {
        var candidates = new List<string>();

        AddCandidates(candidates, GetVariant(definition.StyleMaterialVariants, $"{configuration.ArtStyle}|{configuration.Material}"), bandIndex);
        AddCandidates(candidates, GetVariant(definition.MaterialVariants, configuration.Material), bandIndex);
        AddCandidates(candidates, GetVariant(definition.StyleVariants, configuration.ArtStyle), bandIndex);
        AddCandidates(candidates, definition.Bands, bandIndex);

        var deduped = candidates
            .Where(static phrase => !string.IsNullOrWhiteSpace(phrase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (deduped.Length == 0)
        {
            return [];
        }

        return ApplyBundlePreference(configuration.IntentMode, sliderKey, deduped);
    }

    private static SliderBandDefinition[] GetVariant(Dictionary<string, SliderBandDefinition[]> variants, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return [];
        }

        return variants.TryGetValue(key, out var bands) ? bands : [];
    }

    private static void AddCandidates(ICollection<string> phrases, SliderBandDefinition[] bands, int bandIndex)
    {
        if (bands.Length != 5)
        {
            return;
        }

        foreach (var phrase in bands[bandIndex].Phrases)
        {
            if (!string.IsNullOrWhiteSpace(phrase))
            {
                phrases.Add(phrase);
            }
        }
    }

    private static string[] ApplyBundlePreference(string? intentMode, string sliderKey, string[] candidates)
    {
        if (candidates.Length == 0
            || string.IsNullOrWhiteSpace(intentMode)
            || !IntentModeCatalog.TryGet(intentMode, out _)
            || !BundlePhrasePreferences.TryGetValue(intentMode, out var preference))
        {
            return candidates;
        }

        var preferred = TryFilter(candidates, preference.PreferredBySlider, sliderKey);
        if (preferred.Length > 0)
        {
            return preferred;
        }

        var avoided = GetPhraseSet(preference.AvoidedBySlider, sliderKey);
        if (avoided.Count == 0)
        {
            return candidates;
        }

        var filtered = candidates
            .Where(candidate => !avoided.Contains(candidate))
            .ToArray();

        return filtered.Length > 0 ? filtered : candidates;
    }

    private static string[] TryFilter(string[] candidates, IReadOnlyDictionary<string, string[]> phraseLookup, string sliderKey)
    {
        var allowed = GetPhraseSet(phraseLookup, sliderKey);
        if (allowed.Count == 0)
        {
            return [];
        }

        return candidates
            .Where(candidate => allowed.Contains(candidate))
            .ToArray();
    }

    private static HashSet<string> GetPhraseSet(IReadOnlyDictionary<string, string[]> phraseLookup, string sliderKey)
    {
        if (!phraseLookup.TryGetValue(sliderKey, out var phrases) || phrases.Length == 0)
        {
            return [];
        }

        return new HashSet<string>(
            phrases.Where(static phrase => !string.IsNullOrWhiteSpace(phrase)),
            StringComparer.OrdinalIgnoreCase);
    }
}
