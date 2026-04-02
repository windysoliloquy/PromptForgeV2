namespace PromptForge.App.Models;

public sealed class ArtistPhraseOverride
{
    public bool IsEnabled { get; set; }
    public string ArtistName { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;

    public ArtistPhraseOverride Clone()
    {
        return new ArtistPhraseOverride
        {
            IsEnabled = IsEnabled,
            ArtistName = ArtistName,
            Prefix = Prefix,
            Suffix = Suffix,
        };
    }
}
