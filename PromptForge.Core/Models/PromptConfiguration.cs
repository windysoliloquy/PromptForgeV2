namespace PromptForge.App.Models;

public sealed class PromptConfiguration
{
    public string IntentMode { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public int Temperature { get; set; } = 50;
    public bool ExcludeTemperatureFromPrompt { get; set; }
    public int LightingIntensity { get; set; } = 50;
    public bool ExcludeLightingIntensityFromPrompt { get; set; }
    public int Stylization { get; set; }
    public bool ExcludeStylizationFromPrompt { get; set; }
    public int Realism { get; set; }
    public bool ExcludeRealismFromPrompt { get; set; }
    public int TextureDepth { get; set; }
    public bool ExcludeTextureDepthFromPrompt { get; set; }
    public int NarrativeDensity { get; set; }
    public bool ExcludeNarrativeDensityFromPrompt { get; set; }
    public int Symbolism { get; set; }
    public bool ExcludeSymbolismFromPrompt { get; set; }
    public int AtmosphericDepth { get; set; }
    public bool ExcludeAtmosphericDepthFromPrompt { get; set; }
    public int SurfaceAge { get; set; }
    public bool ExcludeSurfaceAgeFromPrompt { get; set; }
    public int Chaos { get; set; }
    public bool ExcludeChaosFromPrompt { get; set; }
    public int Framing { get; set; } = 50;
    public bool ExcludeFramingFromPrompt { get; set; }
    public string Material { get; set; } = "None";
    public string ArtStyle { get; set; } = "None";
    public string AnimeStyle { get; set; } = "General Anime";
    public string AnimeEra { get; set; } = "Default / Modern";
    public bool AnimeCelShading { get; set; }
    public bool AnimeCleanLineArt { get; set; }
    public bool AnimeExpressiveEyes { get; set; }
    public bool AnimeDynamicAction { get; set; }
    public bool AnimeCinematicLighting { get; set; }
    public bool AnimeStylizedHair { get; set; }
    public bool AnimeAtmosphericEffects { get; set; }
    public bool AnimeCharacterLedStaging { get; set; }
    public bool AnimeClearSilhouetteRead { get; set; }
    public bool AnimeEmotionFirstExpression { get; set; }
    public string ChildrensBookStyle { get; set; } = "general-childrens-book";
    public bool ChildrensBookSoftColorPalette { get; set; }
    public bool ChildrensBookTexturedPaper { get; set; }
    public bool ChildrensBookInkLinework { get; set; }
    public bool ChildrensBookExpressiveCharacters { get; set; }
    public bool ChildrensBookMinimalBackground { get; set; }
    public bool ChildrensBookDecorativeDetails { get; set; }
    public bool ChildrensBookGentleLighting { get; set; }
    public string ComicBookStyle { get; set; } = "General Comic";
    public bool ComicBookBoldInk { get; set; }
    public bool ComicBookHalftoneShading { get; set; }
    public bool ComicBookPanelFraming { get; set; }
    public bool ComicBookDynamicPoses { get; set; }
    public bool ComicBookSpeedLines { get; set; }
    public bool ComicBookHighContrastLighting { get; set; }
    public bool ComicBookSpeechBubbles { get; set; }
    public string SpeechBubbleMode { get; set; } = "Blank Bubbles for Later Editing";
    public string SpeechBubbleSize { get; set; } = "Medium";
    public bool StylizedSpeechBubbleShape { get; set; }
    public string CinematicSubtype { get; set; } = "general-film-still";
    public bool CinematicLetterboxedFraming { get; set; }
    public bool CinematicShallowDepthOfField { get; set; }
    public bool CinematicPracticalLighting { get; set; }
    public bool CinematicAtmosphericHaze { get; set; }
    public bool CinematicFilmGrain { get; set; }
    public bool CinematicAnamorphicFlares { get; set; }
    public bool CinematicDramaticBacklight { get; set; }
    public string ThreeDRenderSubtype { get; set; } = "general-cgi";
    public bool ThreeDRenderGlobalIllumination { get; set; }
    public bool ThreeDRenderVolumetricLighting { get; set; }
    public bool ThreeDRenderRayTracedReflections { get; set; }
    public bool ThreeDRenderDepthOfField { get; set; }
    public bool ThreeDRenderSubsurfaceScattering { get; set; }
    public bool ThreeDRenderHardSurfacePrecision { get; set; }
    public bool ThreeDRenderStudioBackdrop { get; set; }
    public string ConceptArtSubtype { get; set; } = "keyframe-concept";
    public bool ConceptArtDesignCallouts { get; set; }
    public bool ConceptArtTurnaroundReadability { get; set; }
    public bool ConceptArtMaterialBreakdown { get; set; }
    public bool ConceptArtScaleReference { get; set; }
    public bool ConceptArtWorldbuildingAccents { get; set; }
    public bool ConceptArtProductionNotesFeel { get; set; }
    public bool ConceptArtSilhouetteClarity { get; set; }
    public string PixelArtSubtype { get; set; } = "retro-arcade";
    public bool PixelArtLimitedPalette { get; set; }
    public bool PixelArtDithering { get; set; }
    public bool PixelArtTileableDesign { get; set; }
    public bool PixelArtSpriteSheetReadability { get; set; }
    public bool PixelArtCleanOutline { get; set; }
    public bool PixelArtSubpixelShading { get; set; }
    public bool PixelArtHudUiFraming { get; set; }
    public string WatercolorStyle { get; set; } = "general-watercolor";
    public bool WatercolorTransparentWashes { get; set; }
    public bool WatercolorSoftBleeds { get; set; }
    public bool WatercolorPaperTexture { get; set; }
    public bool WatercolorInkAndWatercolor { get; set; }
    public bool WatercolorAtmosphericWash { get; set; }
    public bool WatercolorGouacheAccents { get; set; }
    public string FantasyIllustrationRegister { get; set; } = "general-fantasy";
    public bool FantasyIllustrationCharacterSketch { get; set; }
    public bool FantasyIllustrationCharacterCentric { get; set; }
    public bool FantasyIllustrationEnvironmentConcept { get; set; }
    public bool FantasyIllustrationKeyArt { get; set; }
    public bool FantasyIllustrationCleanBackground { get; set; }
    public bool FantasyIllustrationSilhouetteReadability { get; set; }
    public bool FantasyIllustrationPhotorealistic { get; set; }
    public bool FantasyIllustrationCartoonArt { get; set; }
    public bool FantasyIllustrationPropArtifactFocus { get; set; }
    public bool FantasyIllustrationCreatureDesign { get; set; }
    public bool EditorialIllustrationBlackAndWhiteMonochrome { get; set; }
    public string GraphicDesignType { get; set; } = "general";
    public bool GraphicDesignMinimalLayout { get; set; }
    public bool GraphicDesignBoldHierarchy { get; set; }
    public string InfographicDataVisualizationSubdomain { get; set; } = "infographic";
    public bool InfographicModeLeanExplainer { get; set; } = true;
    public bool InfographicModePublicPoster { get; set; }
    public bool InfographicModeReferenceSheet { get; set; }
    public bool DataVizModeChartPurity { get; set; } = true;
    public bool DataVizModeDashboard { get; set; }
    public bool DataVizModeReportGraphic { get; set; }
    public string PhotographyType { get; set; } = "portrait";
    public string PhotographyEra { get; set; } = "contemporary";
    public bool PhotographyCandidCapture { get; set; }
    public bool PhotographyPosedStagedCapture { get; set; }
    public bool PhotographyAvailableLight { get; set; }
    public bool PhotographyOnCameraFlash { get; set; }
    public bool PhotographyEditorialPolish { get; set; }
    public bool PhotographyRawDocumentaryTexture { get; set; }
    public bool PhotographyEnvironmentalPortraitContext { get; set; }
    public bool PhotographyFilmAnalogCharacter { get; set; }
    public string ProductPhotographyShotType { get; set; } = "packshot";
    public bool ProductPhotographyWithPackaging { get; set; }
    public bool ProductPhotographyPedestalDisplay { get; set; }
    public bool ProductPhotographyReflectiveSurface { get; set; }
    public bool ProductPhotographyFloatingPresentation { get; set; }
    public bool ProductPhotographyScaleCueHand { get; set; }
    public bool ProductPhotographyBrandProps { get; set; }
    public bool ProductPhotographyGroupedVariants { get; set; }
    public string FoodPhotographyShotMode { get; set; } = "plated-hero";
    public bool FoodPhotographyVisibleSteam { get; set; }
    public bool FoodPhotographyGarnishEmphasis { get; set; }
    public bool FoodPhotographyUtensilContext { get; set; }
    public bool FoodPhotographyHandServiceCue { get; set; }
    public bool FoodPhotographyIngredientScatter { get; set; }
    public bool FoodPhotographyCondensationEmphasis { get; set; }
    public string LifestyleAdvertisingShotMode { get; set; } = "everyday-lifestyle";
    public bool LifestyleAdvertisingNaturalInteraction { get; set; }
    public bool LifestyleAdvertisingProductInUse { get; set; }
    public bool LifestyleAdvertisingBrandColorAccent { get; set; }
    public bool LifestyleAdvertisingPropContext { get; set; }
    public bool LifestyleAdvertisingSunlitOptimism { get; set; }
    public bool LifestyleAdvertisingMotionCandidness { get; set; }
    public string ArchitectureArchvizViewMode { get; set; } = "exterior";
    public bool ArchitectureArchvizHumanScaleCues { get; set; }
    public bool ArchitectureArchvizLandscapeEmphasis { get; set; }
    public bool ArchitectureArchvizFurnishingEmphasis { get; set; }
    public bool ArchitectureArchvizWarmInteriorGlow { get; set; }
    public bool ArchitectureArchvizReflectiveSurfaceAccents { get; set; }
    public bool ArchitectureArchvizAmenityFocus { get; set; }
    public string ArtistInfluencePrimary { get; set; } = "None";
    public int InfluenceStrengthPrimary { get; set; }
    public ArtistPhraseOverride PrimaryArtistPhraseOverride { get; set; } = new();
    public string ArtistInfluenceSecondary { get; set; } = "None";
    public int InfluenceStrengthSecondary { get; set; }
    public ArtistPhraseOverride SecondaryArtistPhraseOverride { get; set; } = new();
    public int CameraDistance { get; set; } = 50;
    public bool ExcludeCameraDistanceFromPrompt { get; set; }
    public int CameraAngle { get; set; } = 50;
    public bool ExcludeCameraAngleFromPrompt { get; set; }
    public int BackgroundComplexity { get; set; }
    public bool ExcludeBackgroundComplexityFromPrompt { get; set; }
    public int MotionEnergy { get; set; }
    public bool ExcludeMotionEnergyFromPrompt { get; set; }
    public int FocusDepth { get; set; } = 50;
    public bool ExcludeFocusDepthFromPrompt { get; set; }
    public int ImageCleanliness { get; set; } = 55;
    public bool ExcludeImageCleanlinessFromPrompt { get; set; }
    public int DetailDensity { get; set; } = 50;
    public bool ExcludeDetailDensityFromPrompt { get; set; }
    public int Whimsy { get; set; }
    public bool ExcludeWhimsyFromPrompt { get; set; }
    public int Tension { get; set; }
    public bool ExcludeTensionFromPrompt { get; set; }
    public int Awe { get; set; }
    public bool ExcludeAweFromPrompt { get; set; }
    public string Lighting { get; set; } = "Soft daylight";
    public int Saturation { get; set; }
    public bool ExcludeSaturationFromPrompt { get; set; }
    public int Contrast { get; set; }
    public bool ExcludeContrastFromPrompt { get; set; }
    public string AspectRatio { get; set; } = "1:1";
    public bool PrintReady { get; set; }
    public bool TransparentBackground { get; set; }
    public bool UseNegativePrompt { get; set; }
    public bool CompressPromptSemantics { get; set; } = true;
    public bool ReduceRepeatedLaneWords { get; set; } = true;
    public bool TrimRepeatedLongWords { get; set; }
    public bool SemanticPairInteractions { get; set; } = true;
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
    public bool VintageBendEasternBlocGdr { get; set; }
    public bool VintageBendThrillerUndertone { get; set; }
    public bool VintageBendInstitutionalAusterity { get; set; }
    public bool VintageBendSurveillanceStateAtmosphere { get; set; }
    public bool VintageBendPeriodArtifacts { get; set; }
    public bool VintageBendUrbanCivilian { get; set; }
    public StandardLaneStateCollection StandardLaneStates { get; set; } = new();

    public PromptConfiguration Clone()
    {
        return new PromptConfiguration
        {
        IntentMode = IntentMode,
        Subject = Subject,
        Action = Action,
        Relationship = Relationship,
        Temperature = Temperature,
        ExcludeTemperatureFromPrompt = ExcludeTemperatureFromPrompt,
        LightingIntensity = LightingIntensity,
        ExcludeLightingIntensityFromPrompt = ExcludeLightingIntensityFromPrompt,
        Stylization = Stylization,
        ExcludeStylizationFromPrompt = ExcludeStylizationFromPrompt,
        Realism = Realism,
        ExcludeRealismFromPrompt = ExcludeRealismFromPrompt,
        TextureDepth = TextureDepth,
        ExcludeTextureDepthFromPrompt = ExcludeTextureDepthFromPrompt,
        NarrativeDensity = NarrativeDensity,
        ExcludeNarrativeDensityFromPrompt = ExcludeNarrativeDensityFromPrompt,
        Symbolism = Symbolism,
        ExcludeSymbolismFromPrompt = ExcludeSymbolismFromPrompt,
        AtmosphericDepth = AtmosphericDepth,
        ExcludeAtmosphericDepthFromPrompt = ExcludeAtmosphericDepthFromPrompt,
        SurfaceAge = SurfaceAge,
        ExcludeSurfaceAgeFromPrompt = ExcludeSurfaceAgeFromPrompt,
        Chaos = Chaos,
        ExcludeChaosFromPrompt = ExcludeChaosFromPrompt,
        Framing = Framing,
        ExcludeFramingFromPrompt = ExcludeFramingFromPrompt,
        Material = Material,
        ArtStyle = ArtStyle,
        AnimeStyle = AnimeStyle,
        AnimeEra = AnimeEra,
        AnimeCelShading = AnimeCelShading,
        AnimeCleanLineArt = AnimeCleanLineArt,
        AnimeExpressiveEyes = AnimeExpressiveEyes,
        AnimeDynamicAction = AnimeDynamicAction,
        AnimeCinematicLighting = AnimeCinematicLighting,
        AnimeStylizedHair = AnimeStylizedHair,
        AnimeAtmosphericEffects = AnimeAtmosphericEffects,
        AnimeCharacterLedStaging = AnimeCharacterLedStaging,
        AnimeClearSilhouetteRead = AnimeClearSilhouetteRead,
        AnimeEmotionFirstExpression = AnimeEmotionFirstExpression,
        ChildrensBookStyle = ChildrensBookStyle,
        ChildrensBookSoftColorPalette = ChildrensBookSoftColorPalette,
        ChildrensBookTexturedPaper = ChildrensBookTexturedPaper,
        ChildrensBookInkLinework = ChildrensBookInkLinework,
        ChildrensBookExpressiveCharacters = ChildrensBookExpressiveCharacters,
        ChildrensBookMinimalBackground = ChildrensBookMinimalBackground,
        ChildrensBookDecorativeDetails = ChildrensBookDecorativeDetails,
        ChildrensBookGentleLighting = ChildrensBookGentleLighting,
        ComicBookStyle = ComicBookStyle,
        ComicBookBoldInk = ComicBookBoldInk,
        ComicBookHalftoneShading = ComicBookHalftoneShading,
        ComicBookPanelFraming = ComicBookPanelFraming,
        ComicBookDynamicPoses = ComicBookDynamicPoses,
        ComicBookSpeedLines = ComicBookSpeedLines,
        ComicBookHighContrastLighting = ComicBookHighContrastLighting,
        ComicBookSpeechBubbles = ComicBookSpeechBubbles,
        SpeechBubbleMode = SpeechBubbleMode,
        SpeechBubbleSize = SpeechBubbleSize,
        StylizedSpeechBubbleShape = StylizedSpeechBubbleShape,
        CinematicSubtype = CinematicSubtype,
        CinematicLetterboxedFraming = CinematicLetterboxedFraming,
        CinematicShallowDepthOfField = CinematicShallowDepthOfField,
        CinematicPracticalLighting = CinematicPracticalLighting,
        CinematicAtmosphericHaze = CinematicAtmosphericHaze,
        CinematicFilmGrain = CinematicFilmGrain,
        CinematicAnamorphicFlares = CinematicAnamorphicFlares,
        CinematicDramaticBacklight = CinematicDramaticBacklight,
        ThreeDRenderSubtype = ThreeDRenderSubtype,
        ThreeDRenderGlobalIllumination = ThreeDRenderGlobalIllumination,
        ThreeDRenderVolumetricLighting = ThreeDRenderVolumetricLighting,
        ThreeDRenderRayTracedReflections = ThreeDRenderRayTracedReflections,
        ThreeDRenderDepthOfField = ThreeDRenderDepthOfField,
        ThreeDRenderSubsurfaceScattering = ThreeDRenderSubsurfaceScattering,
        ThreeDRenderHardSurfacePrecision = ThreeDRenderHardSurfacePrecision,
        ThreeDRenderStudioBackdrop = ThreeDRenderStudioBackdrop,
        ConceptArtSubtype = ConceptArtSubtype,
        ConceptArtDesignCallouts = ConceptArtDesignCallouts,
        ConceptArtTurnaroundReadability = ConceptArtTurnaroundReadability,
        ConceptArtMaterialBreakdown = ConceptArtMaterialBreakdown,
        ConceptArtScaleReference = ConceptArtScaleReference,
        ConceptArtWorldbuildingAccents = ConceptArtWorldbuildingAccents,
        ConceptArtProductionNotesFeel = ConceptArtProductionNotesFeel,
        ConceptArtSilhouetteClarity = ConceptArtSilhouetteClarity,
        PixelArtSubtype = PixelArtSubtype,
        PixelArtLimitedPalette = PixelArtLimitedPalette,
        PixelArtDithering = PixelArtDithering,
        PixelArtTileableDesign = PixelArtTileableDesign,
        PixelArtSpriteSheetReadability = PixelArtSpriteSheetReadability,
        PixelArtCleanOutline = PixelArtCleanOutline,
        PixelArtSubpixelShading = PixelArtSubpixelShading,
        PixelArtHudUiFraming = PixelArtHudUiFraming,
        WatercolorStyle = WatercolorStyle,
        WatercolorTransparentWashes = WatercolorTransparentWashes,
        WatercolorSoftBleeds = WatercolorSoftBleeds,
        WatercolorPaperTexture = WatercolorPaperTexture,
        WatercolorInkAndWatercolor = WatercolorInkAndWatercolor,
        WatercolorAtmosphericWash = WatercolorAtmosphericWash,
        WatercolorGouacheAccents = WatercolorGouacheAccents,
        FantasyIllustrationRegister = FantasyIllustrationRegister,
        FantasyIllustrationCharacterSketch = FantasyIllustrationCharacterSketch,
        FantasyIllustrationCharacterCentric = FantasyIllustrationCharacterCentric,
        FantasyIllustrationEnvironmentConcept = FantasyIllustrationEnvironmentConcept,
        FantasyIllustrationKeyArt = FantasyIllustrationKeyArt,
        FantasyIllustrationCleanBackground = FantasyIllustrationCleanBackground,
        FantasyIllustrationSilhouetteReadability = FantasyIllustrationSilhouetteReadability,
        FantasyIllustrationPhotorealistic = FantasyIllustrationPhotorealistic,
        FantasyIllustrationCartoonArt = FantasyIllustrationCartoonArt,
        FantasyIllustrationPropArtifactFocus = FantasyIllustrationPropArtifactFocus,
        FantasyIllustrationCreatureDesign = FantasyIllustrationCreatureDesign,
        EditorialIllustrationBlackAndWhiteMonochrome = EditorialIllustrationBlackAndWhiteMonochrome,
        GraphicDesignType = GraphicDesignType,
        GraphicDesignMinimalLayout = GraphicDesignMinimalLayout,
        GraphicDesignBoldHierarchy = GraphicDesignBoldHierarchy,
        InfographicDataVisualizationSubdomain = InfographicDataVisualizationSubdomain,
        InfographicModeLeanExplainer = InfographicModeLeanExplainer,
        InfographicModePublicPoster = InfographicModePublicPoster,
        InfographicModeReferenceSheet = InfographicModeReferenceSheet,
        DataVizModeChartPurity = DataVizModeChartPurity,
        DataVizModeDashboard = DataVizModeDashboard,
        DataVizModeReportGraphic = DataVizModeReportGraphic,
        PhotographyType = PhotographyType,
        PhotographyEra = PhotographyEra,
        PhotographyCandidCapture = PhotographyCandidCapture,
        PhotographyPosedStagedCapture = PhotographyPosedStagedCapture,
        PhotographyAvailableLight = PhotographyAvailableLight,
        PhotographyOnCameraFlash = PhotographyOnCameraFlash,
        PhotographyEditorialPolish = PhotographyEditorialPolish,
        PhotographyRawDocumentaryTexture = PhotographyRawDocumentaryTexture,
        PhotographyEnvironmentalPortraitContext = PhotographyEnvironmentalPortraitContext,
        PhotographyFilmAnalogCharacter = PhotographyFilmAnalogCharacter,
        ProductPhotographyShotType = ProductPhotographyShotType,
        ProductPhotographyWithPackaging = ProductPhotographyWithPackaging,
        ProductPhotographyPedestalDisplay = ProductPhotographyPedestalDisplay,
        ProductPhotographyReflectiveSurface = ProductPhotographyReflectiveSurface,
        ProductPhotographyFloatingPresentation = ProductPhotographyFloatingPresentation,
        ProductPhotographyScaleCueHand = ProductPhotographyScaleCueHand,
        ProductPhotographyBrandProps = ProductPhotographyBrandProps,
        ProductPhotographyGroupedVariants = ProductPhotographyGroupedVariants,
        FoodPhotographyShotMode = FoodPhotographyShotMode,
        FoodPhotographyVisibleSteam = FoodPhotographyVisibleSteam,
        FoodPhotographyGarnishEmphasis = FoodPhotographyGarnishEmphasis,
        FoodPhotographyUtensilContext = FoodPhotographyUtensilContext,
        FoodPhotographyHandServiceCue = FoodPhotographyHandServiceCue,
        FoodPhotographyIngredientScatter = FoodPhotographyIngredientScatter,
        FoodPhotographyCondensationEmphasis = FoodPhotographyCondensationEmphasis,
        LifestyleAdvertisingShotMode = LifestyleAdvertisingShotMode,
        LifestyleAdvertisingNaturalInteraction = LifestyleAdvertisingNaturalInteraction,
        LifestyleAdvertisingProductInUse = LifestyleAdvertisingProductInUse,
        LifestyleAdvertisingBrandColorAccent = LifestyleAdvertisingBrandColorAccent,
        LifestyleAdvertisingPropContext = LifestyleAdvertisingPropContext,
        LifestyleAdvertisingSunlitOptimism = LifestyleAdvertisingSunlitOptimism,
        LifestyleAdvertisingMotionCandidness = LifestyleAdvertisingMotionCandidness,
        ArchitectureArchvizViewMode = ArchitectureArchvizViewMode,
        ArchitectureArchvizHumanScaleCues = ArchitectureArchvizHumanScaleCues,
        ArchitectureArchvizLandscapeEmphasis = ArchitectureArchvizLandscapeEmphasis,
        ArchitectureArchvizFurnishingEmphasis = ArchitectureArchvizFurnishingEmphasis,
        ArchitectureArchvizWarmInteriorGlow = ArchitectureArchvizWarmInteriorGlow,
        ArchitectureArchvizReflectiveSurfaceAccents = ArchitectureArchvizReflectiveSurfaceAccents,
        ArchitectureArchvizAmenityFocus = ArchitectureArchvizAmenityFocus,
        ArtistInfluencePrimary = ArtistInfluencePrimary,
        InfluenceStrengthPrimary = InfluenceStrengthPrimary,
        PrimaryArtistPhraseOverride = PrimaryArtistPhraseOverride.Clone(),
        ArtistInfluenceSecondary = ArtistInfluenceSecondary,
        InfluenceStrengthSecondary = InfluenceStrengthSecondary,
        SecondaryArtistPhraseOverride = SecondaryArtistPhraseOverride.Clone(),
        CameraDistance = CameraDistance,
        ExcludeCameraDistanceFromPrompt = ExcludeCameraDistanceFromPrompt,
        CameraAngle = CameraAngle,
        ExcludeCameraAngleFromPrompt = ExcludeCameraAngleFromPrompt,
        BackgroundComplexity = BackgroundComplexity,
        ExcludeBackgroundComplexityFromPrompt = ExcludeBackgroundComplexityFromPrompt,
        MotionEnergy = MotionEnergy,
        ExcludeMotionEnergyFromPrompt = ExcludeMotionEnergyFromPrompt,
        FocusDepth = FocusDepth,
        ExcludeFocusDepthFromPrompt = ExcludeFocusDepthFromPrompt,
        ImageCleanliness = ImageCleanliness,
        ExcludeImageCleanlinessFromPrompt = ExcludeImageCleanlinessFromPrompt,
        DetailDensity = DetailDensity,
        ExcludeDetailDensityFromPrompt = ExcludeDetailDensityFromPrompt,
        Whimsy = Whimsy,
        ExcludeWhimsyFromPrompt = ExcludeWhimsyFromPrompt,
        Tension = Tension,
        ExcludeTensionFromPrompt = ExcludeTensionFromPrompt,
        Awe = Awe,
        ExcludeAweFromPrompt = ExcludeAweFromPrompt,
            Lighting = Lighting,
            Saturation = Saturation,
            ExcludeSaturationFromPrompt = ExcludeSaturationFromPrompt,
            Contrast = Contrast,
            ExcludeContrastFromPrompt = ExcludeContrastFromPrompt,
            AspectRatio = AspectRatio,
            PrintReady = PrintReady,
            TransparentBackground = TransparentBackground,
            UseNegativePrompt = UseNegativePrompt,
        CompressPromptSemantics = CompressPromptSemantics,
        ReduceRepeatedLaneWords = ReduceRepeatedLaneWords,
        TrimRepeatedLongWords = TrimRepeatedLongWords,
        SemanticPairInteractions = SemanticPairInteractions,
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
            VintageBendEasternBlocGdr = VintageBendEasternBlocGdr,
            VintageBendThrillerUndertone = VintageBendThrillerUndertone,
            VintageBendInstitutionalAusterity = VintageBendInstitutionalAusterity,
            VintageBendSurveillanceStateAtmosphere = VintageBendSurveillanceStateAtmosphere,
            VintageBendPeriodArtifacts = VintageBendPeriodArtifacts,
            VintageBendUrbanCivilian = VintageBendUrbanCivilian,
            StandardLaneStates = StandardLaneStates.Clone(),
        };
    }
}
