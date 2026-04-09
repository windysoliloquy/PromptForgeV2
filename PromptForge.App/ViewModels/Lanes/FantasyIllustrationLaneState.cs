namespace PromptForge.App.ViewModels.Lanes;

internal sealed class FantasyIllustrationLaneState
{
    public const string CharacterSketchKey = "character-sketch";
    public const string CharacterCentricKey = "character-centric";
    public const string EnvironmentConceptKey = "environment-concept";
    public const string KeyArtKey = "key-art";
    public const string CleanBackgroundKey = "clean-background";
    public const string SilhouetteReadabilityKey = "silhouette-readability";
    public const string PhotorealisticKey = "photorealistic";
    public const string CartoonArtKey = "cartoon-art";
    public const string PropArtifactFocusKey = "prop-artifact-focus";
    public const string CreatureDesignKey = "creature-design";

    internal string Register = "general-fantasy";
    internal bool CharacterSketch;
    internal bool CharacterCentric;
    internal bool EnvironmentConcept;
    internal bool KeyArt;
    internal bool CleanBackground;
    internal bool SilhouetteReadability;
    internal bool Photorealistic;
    internal bool CartoonArt;
    internal bool PropArtifactFocus;
    internal bool CreatureDesignEnabled;

    public static IReadOnlyList<string> GetMutuallyExclusiveModifierKeys(string enabledModifierKey)
    {
        return enabledModifierKey switch
        {
            PhotorealisticKey => [CartoonArtKey],
            CartoonArtKey => [PhotorealisticKey],
            CharacterCentricKey => [EnvironmentConceptKey],
            EnvironmentConceptKey => [CharacterCentricKey, CharacterSketchKey],
            CharacterSketchKey => [EnvironmentConceptKey, PropArtifactFocusKey, CreatureDesignKey],
            PropArtifactFocusKey => [CharacterSketchKey],
            CreatureDesignKey => [CharacterSketchKey],
            _ => [],
        };
    }
}
