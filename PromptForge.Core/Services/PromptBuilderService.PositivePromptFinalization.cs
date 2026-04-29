using System.Collections.Generic;
using System.Linq;
using System.Text;

using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed partial class PromptBuilderService
{
    private static string FinalizePositivePrompt(
        List<PromptFragment> phrases,
        HashSet<string> seen,
        PromptConfiguration configuration,
        bool useVintageBend)
    {
        if (useVintageBend)
        {
            phrases =
            [
                .. VintageBendModifierService.Apply(phrases.Select(fragment => fragment.Text).ToList(), configuration)
                    .Select(text => new PromptFragment(text)),
            ];
            seen = new HashSet<string>(phrases.Select(fragment => fragment.Text), StringComparer.OrdinalIgnoreCase);
        }

        if (PromptCompressionService.ShouldCompress(configuration))
        {
            phrases = PromptCompressionService.Apply(phrases, configuration).ToList();
        }

        phrases = CleanPromptOutputFragments(phrases).ToList();
        return string.Join(", ", phrases.Select(fragment => fragment.Text));
    }

    private static bool TryRewritePromptFragment(string phrase, out string rewritten)
    {
        rewritten = phrase;

        switch (phrase)
        {
            case "supporting environment":
            case "supporting scene detail":
                rewritten = "gentle background detail";
                return true;
            case "layered storytelling cues":
                rewritten = "subtle narrative detail";
                return true;
            case "slight recession":
                rewritten = "soft depth";
                return true;
            case "balanced tonal contrast":
                rewritten = "tonal contrast";
                return true;
        }

        return false;
    }

    private static bool IsExactLowSignalFragment(string phrase)
    {
        return phrase is "balanced framing"
            or "controlled composition"
            or "balanced focus depth"
            or "balanced cleanliness"
            or "balanced detail";
    }

    private static bool IsCompressionScaffold(string phrase)
    {
        if (IsExactLowSignalFragment(phrase))
        {
            return true;
        }

        return phrase.StartsWith("balanced ", StringComparison.OrdinalIgnoreCase)
            || phrase.StartsWith("controlled ", StringComparison.OrdinalIgnoreCase)
            || phrase.StartsWith("supporting ", StringComparison.OrdinalIgnoreCase)
            || string.Equals(phrase, "layered storytelling cues", StringComparison.OrdinalIgnoreCase)
            || string.Equals(phrase, "slight recession", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsHighSignalFragment(string phrase)
    {
        return phrase.Contains("paper texture", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("ink linework", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("speech bubble", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("cel-shaded", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("transparent washes", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("soft glow", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("vivid color", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("gouache", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("stylized hair", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("expressive characters", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("atmospheric effects", StringComparison.OrdinalIgnoreCase);
    }

    private static string CleanPromptOutputFragment(string? value)
    {
        var cleaned = Clean(value);
        if (string.IsNullOrWhiteSpace(cleaned))
        {
            return string.Empty;
        }

        cleaned = HyphenatedLanguageTokenRegex.Replace(cleaned, string.Empty);
        cleaned = LanguageTokenRegex.Replace(cleaned, string.Empty);
        return ExtraWhitespaceRegex.Replace(cleaned, " ").Trim().Trim(',');
    }

    private static IEnumerable<PromptFragment> CleanPromptOutputFragments(IEnumerable<PromptFragment> fragments)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var fragment in fragments)
        {
            var cleaned = CleanPromptOutputFragment(fragment.Text);
            if (string.IsNullOrWhiteSpace(cleaned) || !seen.Add(cleaned))
            {
                continue;
            }

            yield return new PromptFragment(cleaned, fragment.PreserveFromCompression);
        }
    }

    private static string Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Trim());
        builder.Replace("  ", " ");
        return builder.ToString().Trim().Trim(',');
    }
}
