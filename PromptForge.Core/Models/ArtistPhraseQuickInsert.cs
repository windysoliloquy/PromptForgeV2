namespace PromptForge.App.Models;

public enum ArtistPhraseInsertTarget
{
    Prefix,
    Suffix,
}

public sealed class ArtistPhraseQuickInsert
{
    public string Label { get; init; } = string.Empty;
    public string Fragment { get; init; } = string.Empty;
    public ArtistPhraseInsertTarget Target { get; init; }
    public bool IsPairAware { get; init; }
    public string? RoleStem { get; init; }
    public string? DomainLabel { get; init; }
}

public sealed class ArtistPhraseQuickInsertGroup
{
    public string Title { get; init; } = string.Empty;
    public IReadOnlyList<ArtistPhraseQuickInsert> Inserts { get; init; } = Array.Empty<ArtistPhraseQuickInsert>();
}

public sealed class ArtistPhraseSuffixRoleGroup
{
    public string RoleStem { get; init; } = string.Empty;
    public IReadOnlyList<string> Domains { get; init; } = Array.Empty<string>();
}
