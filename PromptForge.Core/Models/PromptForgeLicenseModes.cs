namespace PromptForge.App.Models;

public static class PromptForgeLicenseModes
{
    public const string Temporary = "Temporary";
    public const string MachineBound = "MachineBound";

    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var trimmed = value.Trim();
        if (string.Equals(trimmed, Temporary, StringComparison.OrdinalIgnoreCase))
        {
            return Temporary;
        }

        if (string.Equals(trimmed, MachineBound, StringComparison.OrdinalIgnoreCase))
        {
            return MachineBound;
        }

        return trimmed;
    }
}
