using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static class SpeechBubbleDialogueAnalyzer
{
    public static bool HasQuotedDialogue(string subject, string action, string relationship)
    {
        return Regex.IsMatch(Combine(subject, action, relationship), "\"[^\"]+\"");
    }

    public static bool HasMultipleSubjects(string subject)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            return false;
        }

        return Regex.IsMatch(subject, @"\b(two|three|four|several|multiple|group|crowd|pair|duo|couple|friends|siblings|team)\b", RegexOptions.IgnoreCase) ||
            Regex.IsMatch(subject, @"\s(&|\+)\s", RegexOptions.IgnoreCase) ||
            Regex.IsMatch(subject, @"\b\w+\s+and\s+\w+\b", RegexOptions.IgnoreCase) ||
            subject.Contains(',', StringComparison.Ordinal);
    }

    public static bool HasClearSpeakerAttribution(string subject, string action, string relationship)
    {
        var text = Combine(subject, action, relationship);
        if (!HasQuotedDialogue(subject, action, relationship))
        {
            return false;
        }

        return Regex.IsMatch(text, @"\b(says|said|asks|asked|replies|replied|shouts|shouted|whispers|whispered|speaks|speaking|tells|told)\b", RegexOptions.IgnoreCase) ||
            Regex.IsMatch(text, @"\b[A-Z][A-Za-z0-9 _'-]{1,32}\s*:\s*""[^""]+""");
    }

    public static bool HasUnclearMultiSubjectDialogue(string subject, string action, string relationship)
    {
        return HasMultipleSubjects(subject) && !HasClearSpeakerAttribution(subject, action, relationship);
    }

    private static string Combine(string subject, string action, string relationship)
    {
        return string.Join(" ", new[] { subject, action, relationship }.Where(static value => !string.IsNullOrWhiteSpace(value)));
    }
}
