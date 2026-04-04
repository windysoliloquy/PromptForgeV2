namespace PromptForge.App.Models;

[Flags]
public enum LaneBehaviorFlags
{
    None = 0,
    ShowManualControls = 1 << 0,
    ShowModifierPanel = 1 << 1,
    ShowSidecar = 1 << 2,
    RequiresPolicyHook = 1 << 3,
}

public enum LaneControlType
{
    Checkbox,
    Dropdown,
}

public enum LanePanelLayout
{
    SingleColumn,
    SplitColumns,
}

public sealed record LaneDefinition(
    string Id,
    string DisplayTitle,
    IReadOnlyList<string> IntentNames,
    string Summary,
    string AnchorLabel,
    LanePanelDefinition Panel,
    IReadOnlyList<LaneSubtypeSelectorDefinition> SubtypeSelectors,
    IReadOnlyList<LaneModifierDefinition> Modifiers,
    IReadOnlyList<LaneWeightGroupDefinition> WeightGroups,
    LanePromptDefaults Defaults,
    int ModifierCap,
    LaneBehaviorFlags BehaviorFlags = LaneBehaviorFlags.None,
    string? PolicyKey = null)
{
    public string PrimaryIntentName => IntentNames[0];
    public bool RequiresPolicyHook => BehaviorFlags.HasFlag(LaneBehaviorFlags.RequiresPolicyHook) || !string.IsNullOrWhiteSpace(PolicyKey);
}

public sealed record LanePanelDefinition(
    string Title,
    string HelpText,
    string ModifierTitle,
    string ModifierDescription,
    string? AccentSectionTitle,
    LanePanelLayout Layout);

public sealed record LaneSubtypeSelectorDefinition(
    string Key,
    string Label,
    string SelectedValuePropertyName,
    IReadOnlyList<LaneSubtypeOptionDefinition> Options,
    bool PreserveFromCompression = false);

public sealed record LaneSubtypeOptionDefinition(
    string Key,
    string Label,
    bool IsDefault = false,
    string? SupportDescriptorHint = null,
    LanePromptDefaults? DefaultNudges = null,
    IReadOnlyList<string>? ModifierPriorityBias = null);

public sealed record LaneModifierDefinition(
    string Key,
    string Label,
    string StatePropertyName,
    LaneControlType ControlType,
    bool DefaultState,
    string? DescriptorHint,
    string WeightGroup,
    int CapContribution = 1,
    bool PreserveFromCompression = false,
    string? VisibilityPredicate = null,
    string? TriggerRequirement = null);

public sealed record LaneWeightGroupDefinition(
    string Key,
    int SoftCap,
    int HardCap,
    IReadOnlyList<string> PriorityOrder,
    string? SubtypeBiasHint = null);

public sealed record LanePromptDefaults
{
    public int? Temperature { get; init; }
    public int? LightingIntensity { get; init; }
    public int? Stylization { get; init; }
    public int? Realism { get; init; }
    public int? TextureDepth { get; init; }
    public int? NarrativeDensity { get; init; }
    public int? Symbolism { get; init; }
    public int? AtmosphericDepth { get; init; }
    public int? SurfaceAge { get; init; }
    public int? Chaos { get; init; }
    public int? Framing { get; init; }
    public int? CameraDistance { get; init; }
    public int? CameraAngle { get; init; }
    public int? BackgroundComplexity { get; init; }
    public int? MotionEnergy { get; init; }
    public int? FocusDepth { get; init; }
    public int? ImageCleanliness { get; init; }
    public int? DetailDensity { get; init; }
    public int? Whimsy { get; init; }
    public int? Tension { get; init; }
    public int? Awe { get; init; }
    public int? Saturation { get; init; }
    public int? Contrast { get; init; }
    public string? Lighting { get; init; }
    public string? ArtStyle { get; init; }

    public void ApplyTo(PromptConfiguration configuration)
    {
        if (Temperature.HasValue) configuration.Temperature = Temperature.Value;
        if (LightingIntensity.HasValue) configuration.LightingIntensity = LightingIntensity.Value;
        if (Stylization.HasValue) configuration.Stylization = Stylization.Value;
        if (Realism.HasValue) configuration.Realism = Realism.Value;
        if (TextureDepth.HasValue) configuration.TextureDepth = TextureDepth.Value;
        if (NarrativeDensity.HasValue) configuration.NarrativeDensity = NarrativeDensity.Value;
        if (Symbolism.HasValue) configuration.Symbolism = Symbolism.Value;
        if (AtmosphericDepth.HasValue) configuration.AtmosphericDepth = AtmosphericDepth.Value;
        if (SurfaceAge.HasValue) configuration.SurfaceAge = SurfaceAge.Value;
        if (Chaos.HasValue) configuration.Chaos = Chaos.Value;
        if (Framing.HasValue) configuration.Framing = Framing.Value;
        if (CameraDistance.HasValue) configuration.CameraDistance = CameraDistance.Value;
        if (CameraAngle.HasValue) configuration.CameraAngle = CameraAngle.Value;
        if (BackgroundComplexity.HasValue) configuration.BackgroundComplexity = BackgroundComplexity.Value;
        if (MotionEnergy.HasValue) configuration.MotionEnergy = MotionEnergy.Value;
        if (FocusDepth.HasValue) configuration.FocusDepth = FocusDepth.Value;
        if (ImageCleanliness.HasValue) configuration.ImageCleanliness = ImageCleanliness.Value;
        if (DetailDensity.HasValue) configuration.DetailDensity = DetailDensity.Value;
        if (Whimsy.HasValue) configuration.Whimsy = Whimsy.Value;
        if (Tension.HasValue) configuration.Tension = Tension.Value;
        if (Awe.HasValue) configuration.Awe = Awe.Value;
        if (Saturation.HasValue) configuration.Saturation = Saturation.Value;
        if (Contrast.HasValue) configuration.Contrast = Contrast.Value;
        if (!string.IsNullOrWhiteSpace(Lighting)) configuration.Lighting = Lighting;
        if (!string.IsNullOrWhiteSpace(ArtStyle)) configuration.ArtStyle = ArtStyle;
    }
}
