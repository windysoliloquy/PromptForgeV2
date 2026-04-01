namespace PromptForge.App.Models;

public sealed class PromptConfiguration
{
    public string IntentMode { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public int Temperature { get; set; } = 50;
    public int LightingIntensity { get; set; } = 50;
    public int Stylization { get; set; }
    public int Realism { get; set; }
    public int TextureDepth { get; set; }
    public int NarrativeDensity { get; set; }
    public int Symbolism { get; set; }
    public int AtmosphericDepth { get; set; }
    public int SurfaceAge { get; set; }
    public int Chaos { get; set; }
    public int Framing { get; set; } = 50;
    public string Material { get; set; } = "None";
    public string ArtStyle { get; set; } = "None";
    public string ArtistInfluencePrimary { get; set; } = "None";
    public int InfluenceStrengthPrimary { get; set; }
    public string ArtistInfluenceSecondary { get; set; } = "None";
    public int InfluenceStrengthSecondary { get; set; }
    public int CameraDistance { get; set; } = 50;
    public int CameraAngle { get; set; } = 50;
    public int BackgroundComplexity { get; set; }
    public int MotionEnergy { get; set; }
    public int FocusDepth { get; set; } = 50;
    public int ImageCleanliness { get; set; } = 55;
    public int DetailDensity { get; set; } = 50;
    public int Whimsy { get; set; }
    public int Tension { get; set; }
    public int Awe { get; set; }
    public string Lighting { get; set; } = "Soft daylight";
    public int Saturation { get; set; }
    public int Contrast { get; set; }
    public string AspectRatio { get; set; } = "1:1";
    public bool PrintReady { get; set; }
    public bool TransparentBackground { get; set; }
    public bool UseNegativePrompt { get; set; } = true;
    public bool AvoidClutter { get; set; } = true;
    public bool AvoidMuddyLighting { get; set; } = true;
    public bool AvoidDistortedAnatomy { get; set; } = true;
    public bool AvoidExtraLimbs { get; set; } = true;
    public bool AvoidTextArtifacts { get; set; } = true;
    public bool AvoidOversaturation { get; set; } = true;
    public bool AvoidFlatComposition { get; set; } = true;
    public bool AvoidMessyBackground { get; set; } = true;
    public bool AvoidWeakMaterialDefinition { get; set; } = true;
    public bool AvoidBlurryDetail { get; set; } = true;

    public PromptConfiguration Clone()
    {
        return new PromptConfiguration
        {
        IntentMode = IntentMode,
        Subject = Subject,
        Action = Action,
        Relationship = Relationship,
        Temperature = Temperature,
        LightingIntensity = LightingIntensity,
        Stylization = Stylization,
        Realism = Realism,
        TextureDepth = TextureDepth,
        NarrativeDensity = NarrativeDensity,
        Symbolism = Symbolism,
        AtmosphericDepth = AtmosphericDepth,
        SurfaceAge = SurfaceAge,
        Chaos = Chaos,
        Framing = Framing,
        Material = Material,
        ArtStyle = ArtStyle,
        ArtistInfluencePrimary = ArtistInfluencePrimary,
        InfluenceStrengthPrimary = InfluenceStrengthPrimary,
        ArtistInfluenceSecondary = ArtistInfluenceSecondary,
        InfluenceStrengthSecondary = InfluenceStrengthSecondary,
        CameraDistance = CameraDistance,
        CameraAngle = CameraAngle,
        BackgroundComplexity = BackgroundComplexity,
        MotionEnergy = MotionEnergy,
        FocusDepth = FocusDepth,
        ImageCleanliness = ImageCleanliness,
        DetailDensity = DetailDensity,
        Whimsy = Whimsy,
        Tension = Tension,
        Awe = Awe,
            Lighting = Lighting,
            Saturation = Saturation,
            Contrast = Contrast,
            AspectRatio = AspectRatio,
            PrintReady = PrintReady,
            TransparentBackground = TransparentBackground,
            UseNegativePrompt = UseNegativePrompt,
            AvoidClutter = AvoidClutter,
            AvoidMuddyLighting = AvoidMuddyLighting,
            AvoidDistortedAnatomy = AvoidDistortedAnatomy,
            AvoidExtraLimbs = AvoidExtraLimbs,
            AvoidTextArtifacts = AvoidTextArtifacts,
            AvoidOversaturation = AvoidOversaturation,
            AvoidFlatComposition = AvoidFlatComposition,
            AvoidMessyBackground = AvoidMessyBackground,
            AvoidWeakMaterialDefinition = AvoidWeakMaterialDefinition,
            AvoidBlurryDetail = AvoidBlurryDetail,
        };
    }
}
