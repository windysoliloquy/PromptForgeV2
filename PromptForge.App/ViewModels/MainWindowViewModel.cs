using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using PromptForge.App.Commands;
using PromptForge.App.Models;
using PromptForge.App.Services;
using PromptForge.App.Services.Lanes;
using PromptForge.App.ViewModels.Lanes;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private const string BlankSpeechBubbleMode = "Blank Bubbles for Later Editing";
    private const string RenderedDialogueSpeechBubbleMode = "Rendered Dialogue";
    private const string MediumSpeechBubbleSize = "Medium";
    private const string VintageBendUnlockPresetName = "VB";
    private const string ProductPhotographyUnlockPresetName = "Product";
    private const string FoodPhotographyUnlockPresetName = "Food";
    private const string LifestyleAdvertisingUnlockPresetName = "Lifestyle";
    private const string StandardPresentationMode = "Standard";
    private const string CompactPresentationMode = "Compact";

    private static readonly IReadOnlyDictionary<string, string> LockedLaneUnlockPresetNames =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [IntentModeCatalog.VintageBendName] = VintageBendUnlockPresetName,
            [IntentModeCatalog.ProductPhotographyName] = ProductPhotographyUnlockPresetName,
            [IntentModeCatalog.FoodPhotographyName] = FoodPhotographyUnlockPresetName,
            [IntentModeCatalog.LifestyleAdvertisingPhotographyName] = LifestyleAdvertisingUnlockPresetName,
        };

    private static readonly IReadOnlySet<string> BaseUnlockedLaneNames =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            IntentModeCatalog.PhotographyName,
            IntentModeCatalog.PhotographicName,
            IntentModeCatalog.CinematicName,
            IntentModeCatalog.WatercolorName,
            IntentModeCatalog.PixelArtName,
            IntentModeCatalog.GraphicDesignName,
        };

    private static class SharedLaneKeys
    {
        public const string StyleSelector = "style";

        public static class Cinematic
        {
            public const string LaneId = "cinematic";
            public const string DefaultSubtypeLabel = "general-film-still";
            public const string LetterboxedFraming = "letterboxed-framing";
            public const string ShallowDepthOfField = "shallow-depth-of-field";
            public const string PracticalLighting = "practical-lighting";
            public const string AtmosphericHaze = "atmospheric-haze";
            public const string FilmGrain = "film-grain";
            public const string AnamorphicFlares = "anamorphic-flares";
            public const string DramaticBacklight = "dramatic-backlight";
        }

        public static class Anime
        {
            public const string LaneId = "anime";
            public const string StyleSelector = "style";
            public const string EraSelector = "era";
            public const string CelShading = "cel-shading";
            public const string CleanLineArt = "clean-line-art";
            public const string ExpressiveEyes = "expressive-eyes";
            public const string DynamicAction = "dynamic-action";
            public const string CinematicLighting = "cinematic-lighting";
            public const string StylizedHair = "stylized-hair";
            public const string AtmosphericEffects = "atmospheric-effects";
            public const string CharacterLedStaging = "character-led-staging";
            public const string ClearSilhouetteRead = "clear-silhouette-read";
            public const string EmotionFirstExpression = "emotion-first-expression";
        }

        public static class ThreeDRender
        {
            public const string LaneId = "3d-render";
            public const string DefaultSubtypeLabel = "general-cgi";
            public const string GlobalIllumination = "global-illumination";
            public const string VolumetricLighting = "volumetric-lighting";
            public const string RayTracedReflections = "ray-traced-reflections";
            public const string DepthOfField = "depth-of-field";
            public const string SubsurfaceScattering = "subsurface-scattering";
            public const string HardSurfacePrecision = "hard-surface-precision";
            public const string StudioBackdrop = "studio-backdrop";
        }

        public static class ConceptArt
        {
            public const string LaneId = "concept-art";
            public const string DefaultSubtypeLabel = "keyframe-concept";
            public const string DesignCallouts = "design-callouts";
            public const string TurnaroundReadability = "turnaround-readability";
            public const string MaterialBreakdown = "material-breakdown";
            public const string ScaleReference = "scale-reference";
            public const string WorldbuildingAccents = "worldbuilding-accents";
            public const string ProductionNotesFeel = "production-notes-feel";
            public const string SilhouetteClarity = "silhouette-clarity";
        }

        public static class PixelArt
        {
            public const string LaneId = "pixel-art";
            public const string DefaultSubtypeLabel = "retro-arcade";
            public const string LimitedPalette = "limited-palette";
            public const string Dithering = "dithering";
            public const string TileableDesign = "tileable-design";
            public const string SpriteSheetReadability = "sprite-sheet-readability";
            public const string CleanOutline = "clean-outline";
            public const string SubpixelShading = "subpixel-shading";
            public const string HudUiFraming = "hud-ui-framing";
        }

        public static class Watercolor
        {
            public const string LaneId = "watercolor";
            public const string DefaultSubtypeLabel = "general-watercolor";
            public const string TransparentWashes = "transparent-washes";
            public const string SoftBleeds = "soft-bleeds";
            public const string PaperTexture = "paper-texture";
            public const string InkAndWatercolor = "ink-and-watercolor";
            public const string AtmosphericWash = "atmospheric-wash";
            public const string GouacheAccents = "gouache-accents";
        }

        public static class FantasyIllustration
        {
            public const string LaneId = "fantasy-illustration";
            public const string RegisterSelector = "fantasy-register";
            public const string DefaultRegisterLabel = "general-fantasy";
            public const string CharacterSketch = "character-sketch";
            public const string CharacterCentric = "character-centric";
            public const string EnvironmentConcept = "environment-concept";
            public const string KeyArt = "key-art";
            public const string CleanBackground = "clean-background";
            public const string SilhouetteReadability = "silhouette-readability";
            public const string Photorealistic = "photorealistic";
            public const string CartoonArt = "cartoon-art";
            public const string PropArtifactFocus = "prop-artifact-focus";
            public const string CreatureDesign = "creature-design";
        }

        public static class EditorialIllustration
        {
            public const string LaneId = "editorial-illustration";
            public const string BlackAndWhiteMonochrome = "black-and-white-monochrome";
        }

        public static class GraphicDesign
        {
            public const string LaneId = "graphic-design";
            public const string TypeSelector = "design-type";
            public const string DefaultTypeLabel = "general";
            public const string MinimalLayout = "minimal-layout";
            public const string BoldHierarchy = "bold-hierarchy";
        }

        public static class InfographicDataVisualization
        {
            public const string LaneId = "infographic-data-visualization";
            public const string SubdomainSelector = "subdomain";
            public const string DefaultSubdomainLabel = "infographic";
            public const string Infographic = "infographic";
            public const string DataViz = "data-viz";
            public const string LeanExplainer = "lean-explainer";
            public const string PublicPoster = "public-poster";
            public const string ReferenceSheet = "reference-sheet";
            public const string ChartPurity = "chart-purity";
            public const string Dashboard = "dashboard";
            public const string ReportGraphic = "report-graphic";
        }

        public static class Photography
        {
            public const string LaneId = "photography";
            public const string TypeSelector = "type";
            public const string EraSelector = "era";
            public const string DefaultTypeLabel = "portrait";
            public const string DefaultEraLabel = "contemporary";
        }

        public static class ProductPhotography
        {
            public const string LaneId = "product-photography";
            public const string ShotTypeSelector = "shot-type";
            public const string DefaultShotTypeLabel = "packshot";
            public const string WithPackaging = "with-packaging";
            public const string PedestalDisplay = "pedestal-display";
            public const string ReflectiveSurface = "reflective-surface";
            public const string FloatingPresentation = "floating-presentation";
            public const string ScaleCueHand = "scale-cue-hand";
            public const string BrandProps = "brand-props";
            public const string GroupedVariants = "grouped-variants";
        }

        public static class FoodPhotography
        {
            public const string LaneId = "food-photography";
            public const string ShotModeSelector = "shot-mode";
            public const string DefaultShotModeLabel = "plated-hero";
            public const string VisibleSteam = "visible-steam";
            public const string GarnishEmphasis = "garnish-emphasis";
            public const string UtensilContext = "utensil-context";
            public const string HandServiceCue = "hand-service-cue";
            public const string IngredientScatter = "ingredient-scatter";
            public const string CondensationEmphasis = "condensation-emphasis";
        }

        public static class LifestyleAdvertising
        {
            public const string LaneId = "lifestyle-advertising-photography";
            public const string ShotModeSelector = "shot-mode";
            public const string DefaultShotModeLabel = "everyday-lifestyle";
            public const string NaturalInteraction = "natural-interaction";
            public const string ProductInUse = "product-in-use";
            public const string BrandColorAccent = "brand-color-accent";
            public const string PropContext = "prop-context";
            public const string SunlitOptimism = "sunlit-optimism";
            public const string MotionCandidness = "motion-candidness";
        }

        public static class ArchitectureArchviz
        {
            public const string LaneId = "architecture-archviz";
            public const string ViewModeSelector = "view-mode";
            public const string DefaultViewModeLabel = "exterior";
            public const string HumanScaleCues = "human-scale-cues";
            public const string LandscapeEmphasis = "landscape-emphasis";
            public const string FurnishingEmphasis = "furnishing-emphasis";
            public const string WarmInteriorGlow = "warm-interior-glow";
            public const string ReflectiveSurfaceAccents = "reflective-surface-accents";
            public const string AmenityFocus = "amenity-focus";
        }

        public static class ChildrensBook
        {
            public const string LaneId = "childrens-book";
            public const string DefaultSubtypeLabel = "general-childrens-book";
            public const string SoftColorPalette = "soft-color-palette";
            public const string TexturedPaper = "textured-paper";
            public const string InkLinework = "ink-linework";
            public const string ExpressiveCharacters = "expressive-characters";
            public const string MinimalBackground = "minimal-background";
            public const string DecorativeDetails = "decorative-details";
            public const string GentleLighting = "gentle-lighting";
        }
    }

    private readonly IPromptBuilderService _promptBuilderService;
    private readonly IPresetStorageService _presetStorageService;
    private readonly IClipboardService _clipboardService;
    private readonly IArtistProfileService _artistProfileService;
    private readonly IArtistPairGuidanceService _artistPairGuidanceService;
    private readonly IThemeService _themeService;
    private readonly IDemoStateService _demoStateService;
    private readonly ILicenseService _licenseService;
    private readonly ILaneUnlockStateService _laneUnlockStateService;
    private readonly DispatcherTimer _experimentalMacroRefreshTimer;
    private readonly IReadOnlyDictionary<string, StandardLanePanelViewModel> _sharedLanePanels;
    private StandardLaneStateCollection _ordinaryLaneStates;
    private bool _isApplyingConfiguration;
    private bool _suspendArtistOverrideReset;
    private bool _overrideDefaultSliderPositions;
    private bool _hasLockedLaneAccess;

    private string _presentationMode = StandardPresentationMode;
    private string _intentMode = IntentModeCatalog.PhotographyName;
    private string _subject = string.Empty;
    private string _action = string.Empty;
    private string _relationship = string.Empty;
    private int _temperature = 50;
    private bool _excludeTemperatureFromPrompt;
    private int _lightingIntensity = 50;
    private bool _excludeLightingIntensityFromPrompt;
    private int _stylization = 50;
    private bool _excludeStylizationFromPrompt;
    private int _realism = 50;
    private bool _excludeRealismFromPrompt;
    private int _textureDepth = 35;
    private bool _excludeTextureDepthFromPrompt;
    private int _narrativeDensity = 35;
    private bool _excludeNarrativeDensityFromPrompt;
    private int _symbolism = 25;
    private bool _excludeSymbolismFromPrompt;
    private int _atmosphericDepth = 40;
    private bool _excludeAtmosphericDepthFromPrompt;
    private int _surfaceAge = 20;
    private bool _excludeSurfaceAgeFromPrompt;
    private int _chaos = 20;
    private bool _excludeChaosFromPrompt;
    private int _framing = 50;
    private bool _excludeFramingFromPrompt;
    private string _material = "None";
    private string _artStyle = "None";
    private string _animeStyle = "General Anime";
    private string _animeEra = "Default / Modern";
    private bool _animeCelShading;
    private bool _animeCleanLineArt;
    private bool _animeExpressiveEyes;
    private bool _animeDynamicAction;
    private bool _animeCinematicLighting;
    private bool _animeStylizedHair;
    private bool _animeAtmosphericEffects;
    private bool _animeCharacterLedStaging;
    private bool _animeClearSilhouetteRead;
    private bool _animeEmotionFirstExpression;
    private bool _animeCompactPanelEnabled;
    private string _childrensBookStyle = "general-childrens-book";
    private bool _childrensBookSoftColorPalette;
    private bool _childrensBookTexturedPaper;
    private bool _childrensBookInkLinework;
    private bool _childrensBookExpressiveCharacters;
    private bool _childrensBookMinimalBackground;
    private bool _childrensBookDecorativeDetails;
    private bool _childrensBookGentleLighting;
    private string _comicBookStyle = "General Comic";
    private bool _comicBookBoldInk;
    private bool _comicBookHalftoneShading;
    private bool _comicBookPanelFraming;
    private bool _comicBookDynamicPoses;
    private bool _comicBookSpeedLines;
    private bool _comicBookHighContrastLighting;
    private bool _comicBookSpeechBubbles;
    private bool _isSpeechBubbleOptionsOpen;
    private string _speechBubbleMode = BlankSpeechBubbleMode;
    private string _speechBubbleSize = MediumSpeechBubbleSize;
    private bool _stylizedSpeechBubbleShape;
    private string _cinematicSubtype = "general-film-still";
    private bool _cinematicLetterboxedFraming;
    private bool _cinematicShallowDepthOfField;
    private bool _cinematicPracticalLighting;
    private bool _cinematicAtmosphericHaze;
    private bool _cinematicFilmGrain;
    private bool _cinematicAnamorphicFlares;
    private bool _cinematicDramaticBacklight;
    private string _threeDRenderSubtype = "general-cgi";
    private bool _threeDRenderGlobalIllumination;
    private bool _threeDRenderVolumetricLighting;
    private bool _threeDRenderRayTracedReflections;
    private bool _threeDRenderDepthOfField;
    private bool _threeDRenderSubsurfaceScattering;
    private bool _threeDRenderHardSurfacePrecision;
    private bool _threeDRenderStudioBackdrop;
    private string _conceptArtSubtype = "keyframe-concept";
    private bool _conceptArtDesignCallouts;
    private bool _conceptArtTurnaroundReadability;
    private bool _conceptArtMaterialBreakdown;
    private bool _conceptArtScaleReference;
    private bool _conceptArtWorldbuildingAccents;
    private bool _conceptArtProductionNotesFeel;
    private bool _conceptArtSilhouetteClarity;
    private string _pixelArtSubtype = "retro-arcade";
    private bool _pixelArtLimitedPalette;
    private bool _pixelArtDithering;
    private bool _pixelArtTileableDesign;
    private bool _pixelArtSpriteSheetReadability;
    private bool _pixelArtCleanOutline;
    private bool _pixelArtSubpixelShading;
    private bool _pixelArtHudUiFraming;
    private string _watercolorStyle = "general-watercolor";
    private string _graphicDesignType = "general";
    private string _infographicDataVisualizationSubdomain = "infographic";
    private bool _infographicModeLeanExplainer = true;
    private bool _infographicModePublicPoster;
    private bool _infographicModeReferenceSheet;
    private bool _graphicDesignMinimalLayout;
    private bool _graphicDesignBoldHierarchy;
    private bool _dataVizModeChartPurity = true;
    private bool _dataVizModeDashboard;
    private bool _dataVizModeReportGraphic;
    private bool _watercolorTransparentWashes;
    private bool _watercolorSoftBleeds;
    private bool _watercolorPaperTexture;
    private bool _watercolorInkAndWatercolor;
    private bool _watercolorAtmosphericWash;
    private bool _watercolorGouacheAccents;
    private readonly FantasyIllustrationLaneState _fantasyIllustrationState = new();
    private bool _editorialIllustrationBlackAndWhiteMonochrome;
    private string _photographyType = "portrait";
    private string _photographyEra = "contemporary";
    private bool _photographyCandidCapture;
    private bool _photographyPosedStagedCapture;
    private bool _photographyAvailableLight;
    private bool _photographyOnCameraFlash;
    private bool _photographyEditorialPolish;
    private bool _photographyRawDocumentaryTexture;
    private bool _photographyEnvironmentalPortraitContext;
    private bool _photographyFilmAnalogCharacter;
    private string _productPhotographyShotType = "packshot";
    private bool _productPhotographyWithPackaging;
    private bool _productPhotographyPedestalDisplay;
    private bool _productPhotographyReflectiveSurface;
    private bool _productPhotographyFloatingPresentation;
    private bool _productPhotographyScaleCueHand;
    private bool _productPhotographyBrandProps;
    private bool _productPhotographyGroupedVariants;
    private string _foodPhotographyShotMode = "plated-hero";
    private bool _foodPhotographyVisibleSteam;
    private bool _foodPhotographyGarnishEmphasis;
    private bool _foodPhotographyUtensilContext;
    private bool _foodPhotographyHandServiceCue;
    private bool _foodPhotographyIngredientScatter;
    private bool _foodPhotographyCondensationEmphasis;
    private string _lifestyleAdvertisingShotMode = "everyday-lifestyle";
    private bool _lifestyleAdvertisingNaturalInteraction;
    private bool _lifestyleAdvertisingProductInUse;
    private bool _lifestyleAdvertisingBrandColorAccent;
    private bool _lifestyleAdvertisingPropContext;
    private bool _lifestyleAdvertisingSunlitOptimism;
    private bool _lifestyleAdvertisingMotionCandidness;
    private string _architectureArchvizViewMode = "exterior";
    private bool _architectureArchvizHumanScaleCues;
    private bool _architectureArchvizLandscapeEmphasis;
    private bool _architectureArchvizFurnishingEmphasis;
    private bool _architectureArchvizWarmInteriorGlow;
    private bool _architectureArchvizReflectiveSurfaceAccents;
    private bool _architectureArchvizAmenityFocus;
    private string _artistInfluencePrimary = "None";
    private int _influenceStrengthPrimary = 45;
    private string _artistInfluenceSecondary = "None";
    private int _influenceStrengthSecondary = 30;
    private int _cameraDistance = 50;
    private bool _excludeCameraDistanceFromPrompt;
    private int _cameraAngle = 50;
    private bool _excludeCameraAngleFromPrompt;
    private int _backgroundComplexity = 40;
    private bool _excludeBackgroundComplexityFromPrompt;
    private int _motionEnergy = 20;
    private bool _excludeMotionEnergyFromPrompt;
    private int _focusDepth = 50;
    private bool _excludeFocusDepthFromPrompt;
    private int _imageCleanliness = 55;
    private bool _excludeImageCleanlinessFromPrompt;
    private int _detailDensity = 50;
    private bool _excludeDetailDensityFromPrompt;
    private int _whimsy = 20;
    private bool _excludeWhimsyFromPrompt;
    private int _tension = 20;
    private bool _excludeTensionFromPrompt;
    private int _awe = 40;
    private bool _excludeAweFromPrompt;
    private string _lighting = "Soft daylight";
    private int _saturation = 55;
    private bool _excludeSaturationFromPrompt;
    private int _contrast = 55;
    private bool _excludeContrastFromPrompt;
    private string _aspectRatio = "1:1";
    private bool _printReady;
    private bool _transparentBackground;
    private bool _useNegativePrompt;
    private bool _compressPromptSemantics;
    private bool _reduceRepeatedLaneWords;
    private bool _trimRepeatedLongWords;
    private bool _semanticPairInteractions = true;
    private bool _avoidClutter = true;
    private bool _avoidMuddyLighting = true;
    private bool _avoidDistortedAnatomy = true;
    private bool _avoidExtraLimbs = true;
    private bool _avoidTextArtifacts = true;
    private bool _avoidOversaturation = true;
    private bool _avoidFlatComposition = true;
    private bool _avoidMessyBackground = true;
    private bool _avoidWeakMaterialDefinition = true;
    private bool _avoidBlurryDetail = true;
    private bool _vintageBendEasternBlocGdr;
    private bool _vintageBendThrillerUndertone;
    private bool _vintageBendInstitutionalAusterity;
    private bool _vintageBendSurveillanceStateAtmosphere;
    private bool _vintageBendPeriodArtifacts;
    private bool _vintageBendUrbanCivilian;
    private bool _excludeArtistSlidersFromRandomize = true;
    private string _selectedThemeName = string.Empty;
    private string _promptPreview = string.Empty;
    private string _negativePromptPreview = string.Empty;
    private string _presetName = string.Empty;
    private string? _selectedPresetName;
    private string _newSavestateFolderName = string.Empty;
    private PresetSavestateFolder? _selectedSavestateFolder;
    private string _statusMessage = "Ready.";
    private int _remainingDemoCopies;
    private long _copyPromptFeedbackTick;
    private string? _artistPairGuidanceTooltip;
    private string _artistPhraseEditorPrefix = string.Empty;
    private string _artistPhraseEditorArtistName = string.Empty;
    private string _artistPhraseEditorSuffix = string.Empty;
    private string _artistPhraseEditorGeneratedPhrase = string.Empty;
    private ArtistPairLookupResult? _currentArtistPairLookup;

    static MainWindowViewModel()
    {
        StandardLaneBindingValidator.ThrowIfInvalid(typeof(MainWindowViewModel), StandardLaneBindingValidator.GetSharedStandardLaneDefinitions());
    }

    public MainWindowViewModel(IPromptBuilderService promptBuilderService, IPresetStorageService presetStorageService, IClipboardService clipboardService, IArtistProfileService artistProfileService, IArtistPairGuidanceService artistPairGuidanceService, IThemeService themeService, IDemoStateService demoStateService, ILicenseService licenseService, ILaneUnlockStateService laneUnlockStateService)
    {
        _promptBuilderService = promptBuilderService;
        _presetStorageService = presetStorageService;
        _clipboardService = clipboardService;
        _artistProfileService = artistProfileService;
        _artistPairGuidanceService = artistPairGuidanceService;
        _themeService = themeService;
        _demoStateService = demoStateService;
        _licenseService = licenseService;
        _laneUnlockStateService = laneUnlockStateService;
        _experimentalMacroRefreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(90),
        };
        _experimentalMacroRefreshTimer.Tick += (_, _) =>
        {
            _experimentalMacroRefreshTimer.Stop();
            RegeneratePrompt();
        };
        _selectedThemeName = themeService.CurrentThemeName;
        _remainingDemoCopies = demoStateService.CurrentState.RemainingCopies;
        _ordinaryLaneStates = StandardLaneStateAdapter.CreateDefaultCollection();

        IntentModes = new ObservableCollection<string>(IntentModeCatalog.Names);
        Materials = new ObservableCollection<string>(new[] { "None", "Yarn", "Paint", "Glass", "Ink", "Stone", "Metal" });
        ArtStyles = new ObservableCollection<string>(new[] { "None", "Cinematic", "Painterly", "Yarn Relief", "Stained Glass", "Surreal Symbolic", "Concept Art" });
        AnimeStyles = BuildSubtypeCollection(IntentModeCatalog.AnimeName);
        AnimeEras = new ObservableCollection<string>(LaneRegistry.GetSubtypeLabels(IntentModeCatalog.AnimeName, "era"));
        ComicBookStyles = BuildSubtypeCollection(IntentModeCatalog.ComicBookName);
        SpeechBubbleModes = new ObservableCollection<string>(new[] { BlankSpeechBubbleMode, RenderedDialogueSpeechBubbleMode });
        SpeechBubbleSizes = new ObservableCollection<string>(new[] { "Small", MediumSpeechBubbleSize, "Large" });
        ArtistInfluences = new ObservableCollection<string>(artistProfileService.GetArtistNames());
        Lightings = new ObservableCollection<string>(new[] { "High-key light", "Midday sun", "Direct flash", "Dramatic studio light", "Warm directional light", "Golden hour", "Backlit", "Side-lit", "Soft daylight", "Window light", "Overcast", "Warm interior light", "Soft glow", "Volumetric cinematic light", "Neon-lit", "Dusk haze", "Blue hour", "Moonlit", "Low-key light" });
        AspectRatios = new ObservableCollection<string>(new[] { "1:1", "4:5", "16:9", "9:16" });
        PresetNames = new ObservableCollection<string>();
        SavestateFolders = new ObservableCollection<PresetSavestateFolder>();
        Themes = new ObservableCollection<string>(themeService.AvailableThemeNames);
        _sharedLanePanels = BuildSharedLanePanels();
        ApplyPhotographyIntentDefaults();
        _laneUnlockStateService.MigrateFromLegacyPresetMarkers(LockedLaneUnlockPresetNames, _presetStorageService.GetDefaultPresetNames());

        CopyPromptCommand = new RelayCommand(CopyPrompt, CanCopyPrompt);
        CopyNegativePromptCommand = new RelayCommand(CopyNegativePrompt, CanCopyNegativePrompt);
        CopyLaneHelpEmailCommand = new RelayCommand(CopyLaneHelpEmail);
        SavePresetCommand = new RelayCommand(SavePreset);
        LoadPresetCommand = new RelayCommand(LoadPreset);
        RenamePresetCommand = new RelayCommand(RenamePreset);
        DeletePresetCommand = new RelayCommand(DeletePreset);
        CreateSavestateFolderCommand = new RelayCommand(CreateSavestateFolder);
        DeleteSavestateFolderCommand = new RelayCommand(DeleteSelectedSavestateFolder, CanDeleteSelectedSavestateFolder);
        EditPrimaryArtistPhraseCommand = new RelayCommand(OpenPrimaryArtistPhraseEditor, () => CanEditPrimaryArtistPhrase);
        EditSecondaryArtistPhraseCommand = new RelayCommand(OpenSecondaryArtistPhraseEditor, () => CanEditSecondaryArtistPhrase);
        SwapArtistInfluencesCommand = new RelayCommand(SwapArtistInfluences, () => CanSwapArtistInfluences);
        InsertArtistPhraseQuickInsertCommand = new RelayCommand(InsertArtistPhraseQuickInsert, parameter => parameter is ArtistPhraseQuickInsert);
        SaveArtistPhraseEditorCommand = new RelayCommand(SaveArtistPhraseEditor, () => IsArtistPhraseEditorOpen && !string.IsNullOrWhiteSpace(ArtistPhraseEditorArtistName));
        ResetArtistPhraseEditorCommand = new RelayCommand(ResetArtistPhraseEditorToGenerated, () => IsArtistPhraseEditorOpen);
        CancelArtistPhraseEditorCommand = new RelayCommand(CancelArtistPhraseEditor, () => IsArtistPhraseEditorOpen);
        ResetCommand = new RelayCommand(Reset);
        RandomizeSlidersCommand = new RelayCommand(RandomizeSliders);

        RefreshSavestateFolders();
        RefreshPresetNames();
        PresetName = string.Empty;
        SyncExperimentalMacrosFromRaw();
        RegeneratePrompt();
        RefreshArtistPairGuidance();
    }

    public ObservableCollection<string> IntentModes { get; }
    public ObservableCollection<string> Materials { get; }
    public ObservableCollection<string> ArtStyles { get; }
    public ObservableCollection<string> AnimeStyles { get; }
    public ObservableCollection<string> AnimeEras { get; }
    public ObservableCollection<string> ComicBookStyles { get; }
    public ObservableCollection<string> SpeechBubbleModes { get; }
    public ObservableCollection<string> SpeechBubbleSizes { get; }
    public ObservableCollection<string> ArtistInfluences { get; }
    public ObservableCollection<string> Lightings { get; }
    public ObservableCollection<string> AspectRatios { get; }
    public ObservableCollection<string> PresetNames { get; }
    public ObservableCollection<PresetSavestateFolder> SavestateFolders { get; }
    public ObservableCollection<string> Themes { get; }
    public ObservableCollection<string> PresentationModes { get; } = new(new[] { StandardPresentationMode, CompactPresentationMode });

    public long CopyPromptFeedbackTick
    {
        get => _copyPromptFeedbackTick;
        private set => SetProperty(ref _copyPromptFeedbackTick, value);
    }

    public string? ArtistPairGuidanceTooltip
    {
        get => _artistPairGuidanceTooltip;
        private set
        {
            if (SetProperty(ref _artistPairGuidanceTooltip, value))
            {
                OnPropertyChanged(nameof(ShowArtistPairGuidanceIcon));
            }
        }
    }

    public bool ShowArtistPairGuidanceIcon => !string.IsNullOrWhiteSpace(ArtistPairGuidanceTooltip);
    public bool CanSwapArtistInfluences
        => HasSelectedArtistValue(ArtistInfluencePrimary) || HasSelectedArtistValue(ArtistInfluenceSecondary);

    public RelayCommand CopyPromptCommand { get; }
    public RelayCommand CopyNegativePromptCommand { get; }
    public RelayCommand CopyLaneHelpEmailCommand { get; }
    public RelayCommand SavePresetCommand { get; }
    public RelayCommand LoadPresetCommand { get; }
    public RelayCommand RenamePresetCommand { get; }
    public RelayCommand DeletePresetCommand { get; }
    public RelayCommand CreateSavestateFolderCommand { get; }
    public RelayCommand DeleteSavestateFolderCommand { get; }
    public RelayCommand SwapArtistInfluencesCommand { get; }
    public RelayCommand ResetCommand { get; }
    public RelayCommand RandomizeSlidersCommand { get; }

    public string PresentationMode
    {
        get => _presentationMode;
        set
        {
            var normalized = NormalizePresentationMode(value);
            if (SetProperty(ref _presentationMode, normalized))
            {
                OnPropertyChanged(nameof(IsCompactPresentationMode));
                RaiseCompactPresentationRoutingChanged();
            }
        }
    }

    public bool IsCompactPresentationMode => string.Equals(PresentationMode, CompactPresentationMode, StringComparison.Ordinal);

    public string IntentMode
    {
        get => _intentMode;
        set
        {
            var previousIntentMode = _intentMode;
            var normalized = NormalizeIntentMode(value);
            if (SetProperty(ref _intentMode, normalized))
            {
                LogOverrideSliderDiagnostics(
                    $"intent-change-start from='{previousIntentMode}' to='{normalized}' override={OverrideDefaultSliderPositions} applyingConfiguration={_isApplyingConfiguration} {FormatCurrentOverrideSliderState()}");
                OnPropertyChanged(nameof(IntentModeSelectedIndex));
                OnPropertyChanged(nameof(IsExperimentalIntent));
                OnPropertyChanged(nameof(ShowExperimentalMacroControls));
                OnPropertyChanged(nameof(IsExperimentalMacroGuidedMode));
                OnPropertyChanged(nameof(IsExperimentalManualAdvancedMode));
                OnPropertyChanged(nameof(IsCustomIntent));
                OnPropertyChanged(nameof(ShowCustomRandomizeControls));
                OnPropertyChanged(nameof(ShowManualIntentControls));
                OnPropertyChanged(nameof(ShowLegacyManualCompositionCard));
                OnPropertyChanged(nameof(ShowEmbeddedLaneCompositionCard));
                OnPropertyChanged(nameof(ShowManualNegativeConstraints));
                OnPropertyChanged(nameof(IsAnimeIntent));
                OnPropertyChanged(nameof(ShowStandardManualStyleControlsCard));
                OnPropertyChanged(nameof(ShowCompactAnimeManualStyleControlsCard));
                OnPropertyChanged(nameof(ShowStandardManualMoodCard));
                OnPropertyChanged(nameof(ShowCompactAnimeManualMoodCard));
                OnPropertyChanged(nameof(ShowStandardManualLightingAndColorCard));
                OnPropertyChanged(nameof(ShowCompactAnimeManualLightingAndColorCard));
                OnPropertyChanged(nameof(ShowStandardManualImageFinishCard));
                OnPropertyChanged(nameof(ShowCompactAnimeManualImageFinishCard));
                OnPropertyChanged(nameof(ShowStandardManualOutputCard));
                OnPropertyChanged(nameof(ShowCompactAnimeManualOutputCard));
                OnPropertyChanged(nameof(ShowCompactComicBookManualStack));
                OnPropertyChanged(nameof(ShowCompactCinematicManualStack));
                OnPropertyChanged(nameof(ShowCompactWatercolorManualStack));
                OnPropertyChanged(nameof(ShowCompactChildrensBookManualStack));
                OnPropertyChanged(nameof(ShowCompactThreeDRenderManualStack));
                OnPropertyChanged(nameof(ShowCompactArchitectureArchvizManualStack));
                OnPropertyChanged(nameof(ShowCompactTattooArtManualStack));
                OnPropertyChanged(nameof(ShowAnimeModifierPanel));
                OnPropertyChanged(nameof(ShowStandardAnimePanel));
                OnPropertyChanged(nameof(ShowCompactAnimePanel));
                OnPropertyChanged(nameof(ShowSubjectContextFields));
                OnPropertyChanged(nameof(IsChildrensBookIntent));
                OnPropertyChanged(nameof(IsComicBookIntent));
                OnPropertyChanged(nameof(ShowComicBookModifierPanel));
                OnPropertyChanged(nameof(IsCinematicIntent));
                OnPropertyChanged(nameof(ShowCompactCinematicPanel));
                OnPropertyChanged(nameof(ShowCompactWatercolorPanel));
                OnPropertyChanged(nameof(ShowCompactChildrensBookPanel));
                OnPropertyChanged(nameof(ShowCompactThreeDRenderPanel));
                OnPropertyChanged(nameof(ShowCompactTattooArtPanel));
                OnPropertyChanged(nameof(IsPhotographyIntent));
                OnPropertyChanged(nameof(IsProductPhotographyIntent));
                OnPropertyChanged(nameof(IsFoodPhotographyIntent));
                OnPropertyChanged(nameof(IsLifestyleAdvertisingPhotographyIntent));
                OnPropertyChanged(nameof(IsArchitectureArchvizIntent));
                OnPropertyChanged(nameof(IsThreeDRenderIntent));
                OnPropertyChanged(nameof(IsConceptArtIntent));
                OnPropertyChanged(nameof(IsPixelArtIntent));
                OnPropertyChanged(nameof(IsFantasyIllustrationIntent));
                OnPropertyChanged(nameof(IsEditorialIllustrationIntent));
                OnPropertyChanged(nameof(IsGraphicDesignIntent));
                OnPropertyChanged(nameof(IsInfographicDataVisualizationIntent));
                OnPropertyChanged(nameof(IsInfographicSubdomainActive));
                OnPropertyChanged(nameof(IsDataVizSubdomainActive));
                OnPropertyChanged(nameof(IsTattooArtIntent));
                OnPropertyChanged(nameof(IsWatercolorIntent));
                OnPropertyChanged(nameof(IsVintageBendIntent));
                UpdateLockedLaneAccessState();
                OnPropertyChanged(nameof(IsLockedLaneActive));
                OnPropertyChanged(nameof(IsVintageBendLocked));
                OnPropertyChanged(nameof(ShowLockedLaneAuthoringSections));
                OnPropertyChanged(nameof(ShowVintageBendAuthoringSections));
                OnPropertyChanged(nameof(ShowLockedLanePane));
                OnPropertyChanged(nameof(ShowVintageBendLockedPane));
                OnPropertyChanged(nameof(ShowVintageBendModifierPanel));
                OnPropertyChanged(nameof(ShowInfographicModeGroup));
                OnPropertyChanged(nameof(ShowDataVizModeGroup));
                OnPropertyChanged(nameof(ShowInteractivePromptPreview));
                OnPropertyChanged(nameof(ShowDemoPromptPreview));
                OnPropertyChanged(nameof(ShowNegativePrompt));
                OnPropertyChanged(nameof(CanUseCompressionControls));
                OnPropertyChanged(nameof(ShowCompressionTierTwo));
                OnPropertyChanged(nameof(ShowCompressionTierThree));
                OnPropertyChanged(nameof(ShowCompactCinematicPanel));
                OnPropertyChanged(nameof(ShowCompactWatercolorPanel));
                OnPropertyChanged(nameof(ShowCompactChildrensBookPanel));
                OnPropertyChanged(nameof(ShowCompactThreeDRenderPanel));
                OnPropertyChanged(nameof(ShowActiveStandardLanePanel));
                OnPropertyChanged(nameof(ShowResolvedStandardLanePanel));
                OnPropertyChanged(nameof(ShowArtistBlendSummary));
                RunIntentTransitionDefaultingWorkflow(previousIntentMode, normalized);
                OnPropertyChanged(nameof(CanUseCompressionControls));
                OnPropertyChanged(nameof(ShowCompressionTierTwo));
                OnPropertyChanged(nameof(ShowCompressionTierThree));
                OnPropertyChanged(nameof(IntentModeSummary));
                OnPropertyChanged(nameof(ShowLaneHelpFooter));
                OnPropertyChanged(nameof(HasLaneHelpTooltip));
                OnPropertyChanged(nameof(LaneHelpFooterPrefix));
                OnPropertyChanged(nameof(LaneHelpFooterEmail));
                OnPropertyChanged(nameof(LaneHelpPrinciple));
                OnPropertyChanged(nameof(LaneHelpWeak));
                OnPropertyChanged(nameof(LaneHelpStronger));
                OnPropertyChanged(nameof(ActiveStandardLanePanel));
                OnPropertyChanged(nameof(ShowCompactCinematicPanel));
                OnPropertyChanged(nameof(ShowCompactWatercolorPanel));
                OnPropertyChanged(nameof(ShowCompactChildrensBookPanel));
                OnPropertyChanged(nameof(ShowCompactThreeDRenderPanel));
                OnPropertyChanged(nameof(ShowActiveStandardLanePanel));
                OnPropertyChanged(nameof(ShowResolvedStandardLanePanel));
                if (!_isApplyingConfiguration && !_isApplyingExperimentalMacroState)
                {
                    SyncExperimentalMacrosFromRaw();
                }
                RegeneratePrompt();
                LogOverrideSliderDiagnostics(
                    $"intent-change-complete from='{previousIntentMode}' to='{normalized}' override={OverrideDefaultSliderPositions} {FormatCurrentOverrideSliderState()} prompt='{FormatPromptPreviewForLog(PromptPreview)}'");
            }
        }
    }
    public int IntentModeSelectedIndex
    {
        get => GetIntentModeSelectedIndex();
        set
        {
            if (value < 0 || value >= IntentModes.Count)
            {
                return;
            }

            var selectedMode = IntentModes[value];
            if (!string.Equals(IntentMode, selectedMode, StringComparison.OrdinalIgnoreCase))
            {
                IntentMode = selectedMode;
            }
        }
    }
    public string Subject { get => _subject; set { if (SetAndRefresh(ref _subject, value)) RefreshSpeechBubbleOptionState(); } }
    public string Action { get => _action; set { if (SetAndRefresh(ref _action, value)) RefreshSpeechBubbleOptionState(); } }
    public string Relationship { get => _relationship; set { if (SetAndRefresh(ref _relationship, value)) RefreshSpeechBubbleOptionState(); } }
    public bool OverrideDefaultSliderPositions
    {
        get => _overrideDefaultSliderPositions;
        set
        {
            var previous = _overrideDefaultSliderPositions;
            if (!SetAndRefresh(ref _overrideDefaultSliderPositions, value))
            {
                return;
            }

            LogOverrideSliderDiagnostics(
                $"override-checkbox-changed old={previous} new={value} intent='{IntentMode}' {FormatCurrentOverrideSliderState()} prompt='{FormatPromptPreviewForLog(PromptPreview)}'");
        }
    }
    public int Temperature { get => _temperature; set => SetAndRefresh(ref _temperature, value); }
    public bool ExcludeTemperatureFromPrompt { get => _excludeTemperatureFromPrompt; set => SetAndRefresh(ref _excludeTemperatureFromPrompt, value); }
    public int LightingIntensity { get => _lightingIntensity; set => SetAndRefresh(ref _lightingIntensity, value); }
    public bool ExcludeLightingIntensityFromPrompt { get => _excludeLightingIntensityFromPrompt; set => SetAndRefresh(ref _excludeLightingIntensityFromPrompt, value); }
    public int Stylization { get => _stylization; set => SetAndRefresh(ref _stylization, value); }
    public bool ExcludeStylizationFromPrompt { get => _excludeStylizationFromPrompt; set => SetAndRefresh(ref _excludeStylizationFromPrompt, value); }
    public int Realism { get => _realism; set => SetAndRefresh(ref _realism, value); }
    public bool ExcludeRealismFromPrompt { get => _excludeRealismFromPrompt; set => SetAndRefresh(ref _excludeRealismFromPrompt, value); }
    public int TextureDepth { get => _textureDepth; set => SetAndRefresh(ref _textureDepth, value); }
    public bool ExcludeTextureDepthFromPrompt { get => _excludeTextureDepthFromPrompt; set => SetAndRefresh(ref _excludeTextureDepthFromPrompt, value); }
    public int NarrativeDensity { get => _narrativeDensity; set => SetAndRefresh(ref _narrativeDensity, value); }
    public bool ExcludeNarrativeDensityFromPrompt
    {
        get => _excludeNarrativeDensityFromPrompt;
        set
        {
            var previous = _excludeNarrativeDensityFromPrompt;
            if (!SetAndRefresh(ref _excludeNarrativeDensityFromPrompt, value))
            {
                return;
            }

            UiEventLog.Write(
                $"fantasy-debug narrative-exclude-set old={previous} new={value} intent='{IntentMode}' register='{FantasyIllustrationRegister}' characterSketch={FantasyIllustrationCharacterSketch} appliedByFantasy={_fantasyIllustrationAppliedSliderSuppressions.Contains(SliderLanguageCatalog.NarrativeDensity)} prompt='{FormatPromptPreviewForLog(PromptPreview)}'");
        }
    }
    public int Symbolism { get => _symbolism; set => SetAndRefresh(ref _symbolism, value); }
    public bool ExcludeSymbolismFromPrompt { get => _excludeSymbolismFromPrompt; set => SetAndRefresh(ref _excludeSymbolismFromPrompt, value); }
    public int AtmosphericDepth { get => _atmosphericDepth; set => SetAndRefresh(ref _atmosphericDepth, value); }
    public bool ExcludeAtmosphericDepthFromPrompt { get => _excludeAtmosphericDepthFromPrompt; set => SetAndRefresh(ref _excludeAtmosphericDepthFromPrompt, value); }
    public int SurfaceAge { get => _surfaceAge; set => SetAndRefresh(ref _surfaceAge, value); }
    public bool ExcludeSurfaceAgeFromPrompt { get => _excludeSurfaceAgeFromPrompt; set => SetAndRefresh(ref _excludeSurfaceAgeFromPrompt, value); }
    public int Chaos { get => _chaos; set => SetAndRefresh(ref _chaos, value); }
    public bool ExcludeChaosFromPrompt { get => _excludeChaosFromPrompt; set => SetAndRefresh(ref _excludeChaosFromPrompt, value); }
    public int Framing { get => _framing; set => SetAndRefresh(ref _framing, value); }
    public bool ExcludeFramingFromPrompt { get => _excludeFramingFromPrompt; set => SetAndRefresh(ref _excludeFramingFromPrompt, value); }
    public string Material { get => _material; set => SetAndRefresh(ref _material, value); }
    public string ArtStyle { get => _artStyle; set => SetAndRefresh(ref _artStyle, value); }
    public string AnimeStyle { get => _animeStyle; set => SetAnimeSelectorAndRefresh(ref _animeStyle, value, SharedLaneKeys.Anime.StyleSelector); }
    public string AnimeEra { get => _animeEra; set => SetAnimeSelectorAndRefresh(ref _animeEra, value, SharedLaneKeys.Anime.EraSelector); }
    public bool AnimeCelShading { get => _animeCelShading; set => SetAnimeModifierAndRefresh(ref _animeCelShading, value, SharedLaneKeys.Anime.CelShading); }
    public bool AnimeCleanLineArt { get => _animeCleanLineArt; set => SetAnimeModifierAndRefresh(ref _animeCleanLineArt, value, SharedLaneKeys.Anime.CleanLineArt); }
    public bool AnimeExpressiveEyes { get => _animeExpressiveEyes; set => SetAnimeModifierAndRefresh(ref _animeExpressiveEyes, value, SharedLaneKeys.Anime.ExpressiveEyes); }
    public bool AnimeDynamicAction { get => _animeDynamicAction; set => SetAnimeModifierAndRefresh(ref _animeDynamicAction, value, SharedLaneKeys.Anime.DynamicAction); }
    public bool AnimeCinematicLighting { get => _animeCinematicLighting; set => SetAnimeModifierAndRefresh(ref _animeCinematicLighting, value, SharedLaneKeys.Anime.CinematicLighting); }
    public bool AnimeStylizedHair { get => _animeStylizedHair; set => SetAnimeModifierAndRefresh(ref _animeStylizedHair, value, SharedLaneKeys.Anime.StylizedHair); }
    public bool AnimeAtmosphericEffects { get => _animeAtmosphericEffects; set => SetAnimeModifierAndRefresh(ref _animeAtmosphericEffects, value, SharedLaneKeys.Anime.AtmosphericEffects); }
    public bool AnimeCharacterLedStaging { get => _animeCharacterLedStaging; set => SetAnimeModifierAndRefresh(ref _animeCharacterLedStaging, value, SharedLaneKeys.Anime.CharacterLedStaging); }
    public bool AnimeClearSilhouetteRead { get => _animeClearSilhouetteRead; set => SetAnimeModifierAndRefresh(ref _animeClearSilhouetteRead, value, SharedLaneKeys.Anime.ClearSilhouetteRead); }
    public bool AnimeEmotionFirstExpression { get => _animeEmotionFirstExpression; set => SetAnimeModifierAndRefresh(ref _animeEmotionFirstExpression, value, SharedLaneKeys.Anime.EmotionFirstExpression); }
    public bool AnimeCompactPanelEnabled
    {
        get => _animeCompactPanelEnabled;
        set
        {
            if (SetProperty(ref _animeCompactPanelEnabled, value))
            {
                RaiseCompactPresentationRoutingChanged();
            }
        }
    }
    public string ChildrensBookStyle { get => _childrensBookStyle; set => SetStandardLaneSelectorAndRefresh(ref _childrensBookStyle, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.StyleSelector); }
    public bool ChildrensBookSoftColorPalette { get => _childrensBookSoftColorPalette; set => SetStandardLaneModifierAndRefresh(ref _childrensBookSoftColorPalette, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.SoftColorPalette); }
    public bool ChildrensBookTexturedPaper { get => _childrensBookTexturedPaper; set => SetStandardLaneModifierAndRefresh(ref _childrensBookTexturedPaper, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.TexturedPaper); }
    public bool ChildrensBookInkLinework { get => _childrensBookInkLinework; set => SetStandardLaneModifierAndRefresh(ref _childrensBookInkLinework, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.InkLinework); }
    public bool ChildrensBookExpressiveCharacters { get => _childrensBookExpressiveCharacters; set => SetStandardLaneModifierAndRefresh(ref _childrensBookExpressiveCharacters, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.ExpressiveCharacters); }
    public bool ChildrensBookMinimalBackground { get => _childrensBookMinimalBackground; set => SetStandardLaneModifierAndRefresh(ref _childrensBookMinimalBackground, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.MinimalBackground); }
    public bool ChildrensBookDecorativeDetails { get => _childrensBookDecorativeDetails; set => SetStandardLaneModifierAndRefresh(ref _childrensBookDecorativeDetails, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.DecorativeDetails); }
    public bool ChildrensBookGentleLighting { get => _childrensBookGentleLighting; set => SetStandardLaneModifierAndRefresh(ref _childrensBookGentleLighting, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.GentleLighting); }
    public string ComicBookStyle { get => _comicBookStyle; set => SetSelectorAndApplyDefaultNudges(ref _comicBookStyle, value, IntentModeCatalog.ComicBookName, SharedLaneKeys.StyleSelector); }
    public bool ComicBookBoldInk { get => _comicBookBoldInk; set => SetAndRefresh(ref _comicBookBoldInk, value); }
    public bool ComicBookHalftoneShading { get => _comicBookHalftoneShading; set => SetAndRefresh(ref _comicBookHalftoneShading, value); }
    public bool ComicBookPanelFraming { get => _comicBookPanelFraming; set => SetAndRefresh(ref _comicBookPanelFraming, value); }
    public bool ComicBookDynamicPoses { get => _comicBookDynamicPoses; set => SetAndRefresh(ref _comicBookDynamicPoses, value); }
    public bool ComicBookSpeedLines { get => _comicBookSpeedLines; set => SetAndRefresh(ref _comicBookSpeedLines, value); }
    public bool ComicBookHighContrastLighting { get => _comicBookHighContrastLighting; set => SetAndRefresh(ref _comicBookHighContrastLighting, value); }
    public bool ComicBookSpeechBubbles
    {
        get => _comicBookSpeechBubbles;
        set
        {
            var wasEnabled = _comicBookSpeechBubbles;
            if (!SetAndRefresh(ref _comicBookSpeechBubbles, value))
            {
                return;
            }

            if (!_isApplyingConfiguration)
            {
                IsSpeechBubbleOptionsOpen = value && !wasEnabled;
            }
            else if (!value)
            {
                IsSpeechBubbleOptionsOpen = false;
            }

            RefreshSpeechBubbleOptionState();
        }
    }
    public bool IsSpeechBubbleOptionsOpen
    {
        get => _isSpeechBubbleOptionsOpen;
        set => SetProperty(ref _isSpeechBubbleOptionsOpen, value && ComicBookSpeechBubbles);
    }
    public string SpeechBubbleMode
    {
        get => _speechBubbleMode;
        set
        {
            if (SetAndRefresh(ref _speechBubbleMode, value))
            {
                RefreshSpeechBubbleOptionState();
            }
        }
    }
    public string SpeechBubbleSize { get => _speechBubbleSize; set => SetAndRefresh(ref _speechBubbleSize, value); }
    public bool StylizedSpeechBubbleShape { get => _stylizedSpeechBubbleShape; set => SetAndRefresh(ref _stylizedSpeechBubbleShape, value); }
    public bool ShowBlankSpeechBubbleOptions => ComicBookSpeechBubbles && string.Equals(SpeechBubbleMode, BlankSpeechBubbleMode, StringComparison.Ordinal);
    public bool ShowSpeechBubbleWarning => ComicBookSpeechBubbles && SpeechBubbleDialogueAnalyzer.HasUnclearMultiSubjectDialogue(Subject, Action, Relationship);
    public string SpeechBubbleHelperText => string.Equals(SpeechBubbleMode, RenderedDialogueSpeechBubbleMode, StringComparison.Ordinal)
        ? "Use subject, action, or relationship fields to make who is speaking explicit. Blank bubbles plus a later image edit is usually more reliable than asking the model to render both image and final text in one pass."
        : "Generate the page layout first, then add final dialogue in a follow-up image edit.";
    public string SpeechBubbleWarningText => "Multiple subjects detected. Make who speaks explicit, or use blank bubbles.";
    public string CinematicSubtype { get => _cinematicSubtype; set => SetStandardLaneSelectorAndRefresh(ref _cinematicSubtype, value, SharedLaneKeys.Cinematic.LaneId, SharedLaneKeys.StyleSelector); }
    public bool CinematicLetterboxedFraming { get => _cinematicLetterboxedFraming; set => SetStandardLaneModifierAndRefresh(ref _cinematicLetterboxedFraming, value, SharedLaneKeys.Cinematic.LaneId, SharedLaneKeys.Cinematic.LetterboxedFraming); }
    public bool CinematicShallowDepthOfField { get => _cinematicShallowDepthOfField; set => SetStandardLaneModifierAndRefresh(ref _cinematicShallowDepthOfField, value, SharedLaneKeys.Cinematic.LaneId, SharedLaneKeys.Cinematic.ShallowDepthOfField); }
    public bool CinematicPracticalLighting { get => _cinematicPracticalLighting; set => SetStandardLaneModifierAndRefresh(ref _cinematicPracticalLighting, value, SharedLaneKeys.Cinematic.LaneId, SharedLaneKeys.Cinematic.PracticalLighting); }
    public bool CinematicAtmosphericHaze { get => _cinematicAtmosphericHaze; set => SetStandardLaneModifierAndRefresh(ref _cinematicAtmosphericHaze, value, SharedLaneKeys.Cinematic.LaneId, SharedLaneKeys.Cinematic.AtmosphericHaze); }
    public bool CinematicFilmGrain { get => _cinematicFilmGrain; set => SetStandardLaneModifierAndRefresh(ref _cinematicFilmGrain, value, SharedLaneKeys.Cinematic.LaneId, SharedLaneKeys.Cinematic.FilmGrain); }
    public bool CinematicAnamorphicFlares { get => _cinematicAnamorphicFlares; set => SetStandardLaneModifierAndRefresh(ref _cinematicAnamorphicFlares, value, SharedLaneKeys.Cinematic.LaneId, SharedLaneKeys.Cinematic.AnamorphicFlares); }
    public bool CinematicDramaticBacklight { get => _cinematicDramaticBacklight; set => SetStandardLaneModifierAndRefresh(ref _cinematicDramaticBacklight, value, SharedLaneKeys.Cinematic.LaneId, SharedLaneKeys.Cinematic.DramaticBacklight); }
    public string ThreeDRenderSubtype { get => _threeDRenderSubtype; set => SetStandardLaneSelectorAndRefresh(ref _threeDRenderSubtype, value, SharedLaneKeys.ThreeDRender.LaneId, SharedLaneKeys.StyleSelector); }
    public bool ThreeDRenderGlobalIllumination { get => _threeDRenderGlobalIllumination; set => SetStandardLaneModifierAndRefresh(ref _threeDRenderGlobalIllumination, value, SharedLaneKeys.ThreeDRender.LaneId, SharedLaneKeys.ThreeDRender.GlobalIllumination); }
    public bool ThreeDRenderVolumetricLighting { get => _threeDRenderVolumetricLighting; set => SetStandardLaneModifierAndRefresh(ref _threeDRenderVolumetricLighting, value, SharedLaneKeys.ThreeDRender.LaneId, SharedLaneKeys.ThreeDRender.VolumetricLighting); }
    public bool ThreeDRenderRayTracedReflections { get => _threeDRenderRayTracedReflections; set => SetStandardLaneModifierAndRefresh(ref _threeDRenderRayTracedReflections, value, SharedLaneKeys.ThreeDRender.LaneId, SharedLaneKeys.ThreeDRender.RayTracedReflections); }
    public bool ThreeDRenderDepthOfField { get => _threeDRenderDepthOfField; set => SetStandardLaneModifierAndRefresh(ref _threeDRenderDepthOfField, value, SharedLaneKeys.ThreeDRender.LaneId, SharedLaneKeys.ThreeDRender.DepthOfField); }
    public bool ThreeDRenderSubsurfaceScattering { get => _threeDRenderSubsurfaceScattering; set => SetStandardLaneModifierAndRefresh(ref _threeDRenderSubsurfaceScattering, value, SharedLaneKeys.ThreeDRender.LaneId, SharedLaneKeys.ThreeDRender.SubsurfaceScattering); }
    public bool ThreeDRenderHardSurfacePrecision { get => _threeDRenderHardSurfacePrecision; set => SetStandardLaneModifierAndRefresh(ref _threeDRenderHardSurfacePrecision, value, SharedLaneKeys.ThreeDRender.LaneId, SharedLaneKeys.ThreeDRender.HardSurfacePrecision); }
    public bool ThreeDRenderStudioBackdrop { get => _threeDRenderStudioBackdrop; set => SetStandardLaneModifierAndRefresh(ref _threeDRenderStudioBackdrop, value, SharedLaneKeys.ThreeDRender.LaneId, SharedLaneKeys.ThreeDRender.StudioBackdrop); }
    public string ConceptArtSubtype
    {
        get => _conceptArtSubtype;
        set
        {
            if (!SetStandardLaneSelectorAndRefresh(ref _conceptArtSubtype, value, SharedLaneKeys.ConceptArt.LaneId, SharedLaneKeys.StyleSelector))
            {
                return;
            }

            SyncConceptArtSliderSuppressions();
        }
    }
    public bool ConceptArtDesignCallouts { get => _conceptArtDesignCallouts; set => SetStandardLaneModifierAndRefresh(ref _conceptArtDesignCallouts, value, SharedLaneKeys.ConceptArt.LaneId, SharedLaneKeys.ConceptArt.DesignCallouts); }
    public bool ConceptArtTurnaroundReadability { get => _conceptArtTurnaroundReadability; set => SetStandardLaneModifierAndRefresh(ref _conceptArtTurnaroundReadability, value, SharedLaneKeys.ConceptArt.LaneId, SharedLaneKeys.ConceptArt.TurnaroundReadability); }
    public bool ConceptArtMaterialBreakdown { get => _conceptArtMaterialBreakdown; set => SetStandardLaneModifierAndRefresh(ref _conceptArtMaterialBreakdown, value, SharedLaneKeys.ConceptArt.LaneId, SharedLaneKeys.ConceptArt.MaterialBreakdown); }
    public bool ConceptArtScaleReference { get => _conceptArtScaleReference; set => SetStandardLaneModifierAndRefresh(ref _conceptArtScaleReference, value, SharedLaneKeys.ConceptArt.LaneId, SharedLaneKeys.ConceptArt.ScaleReference); }
    public bool ConceptArtWorldbuildingAccents { get => _conceptArtWorldbuildingAccents; set => SetStandardLaneModifierAndRefresh(ref _conceptArtWorldbuildingAccents, value, SharedLaneKeys.ConceptArt.LaneId, SharedLaneKeys.ConceptArt.WorldbuildingAccents); }
    public bool ConceptArtProductionNotesFeel { get => _conceptArtProductionNotesFeel; set => SetStandardLaneModifierAndRefresh(ref _conceptArtProductionNotesFeel, value, SharedLaneKeys.ConceptArt.LaneId, SharedLaneKeys.ConceptArt.ProductionNotesFeel); }
    public bool ConceptArtSilhouetteClarity { get => _conceptArtSilhouetteClarity; set => SetStandardLaneModifierAndRefresh(ref _conceptArtSilhouetteClarity, value, SharedLaneKeys.ConceptArt.LaneId, SharedLaneKeys.ConceptArt.SilhouetteClarity); }
    public string PixelArtSubtype { get => _pixelArtSubtype; set => SetStandardLaneSelectorAndRefresh(ref _pixelArtSubtype, value, SharedLaneKeys.PixelArt.LaneId, SharedLaneKeys.StyleSelector); }
    public bool PixelArtLimitedPalette { get => _pixelArtLimitedPalette; set => SetStandardLaneModifierAndRefresh(ref _pixelArtLimitedPalette, value, SharedLaneKeys.PixelArt.LaneId, SharedLaneKeys.PixelArt.LimitedPalette); }
    public bool PixelArtDithering { get => _pixelArtDithering; set => SetStandardLaneModifierAndRefresh(ref _pixelArtDithering, value, SharedLaneKeys.PixelArt.LaneId, SharedLaneKeys.PixelArt.Dithering); }
    public bool PixelArtTileableDesign { get => _pixelArtTileableDesign; set => SetStandardLaneModifierAndRefresh(ref _pixelArtTileableDesign, value, SharedLaneKeys.PixelArt.LaneId, SharedLaneKeys.PixelArt.TileableDesign); }
    public bool PixelArtSpriteSheetReadability { get => _pixelArtSpriteSheetReadability; set => SetStandardLaneModifierAndRefresh(ref _pixelArtSpriteSheetReadability, value, SharedLaneKeys.PixelArt.LaneId, SharedLaneKeys.PixelArt.SpriteSheetReadability); }
    public bool PixelArtCleanOutline { get => _pixelArtCleanOutline; set => SetStandardLaneModifierAndRefresh(ref _pixelArtCleanOutline, value, SharedLaneKeys.PixelArt.LaneId, SharedLaneKeys.PixelArt.CleanOutline); }
    public bool PixelArtSubpixelShading { get => _pixelArtSubpixelShading; set => SetStandardLaneModifierAndRefresh(ref _pixelArtSubpixelShading, value, SharedLaneKeys.PixelArt.LaneId, SharedLaneKeys.PixelArt.SubpixelShading); }
    public bool PixelArtHudUiFraming { get => _pixelArtHudUiFraming; set => SetStandardLaneModifierAndRefresh(ref _pixelArtHudUiFraming, value, SharedLaneKeys.PixelArt.LaneId, SharedLaneKeys.PixelArt.HudUiFraming); }
    public string WatercolorStyle { get => _watercolorStyle; set => SetStandardLaneSelectorAndRefresh(ref _watercolorStyle, value, SharedLaneKeys.Watercolor.LaneId, SharedLaneKeys.StyleSelector); }
    public bool WatercolorTransparentWashes { get => _watercolorTransparentWashes; set => SetStandardLaneModifierAndRefresh(ref _watercolorTransparentWashes, value, SharedLaneKeys.Watercolor.LaneId, SharedLaneKeys.Watercolor.TransparentWashes); }
    public bool WatercolorSoftBleeds { get => _watercolorSoftBleeds; set => SetStandardLaneModifierAndRefresh(ref _watercolorSoftBleeds, value, SharedLaneKeys.Watercolor.LaneId, SharedLaneKeys.Watercolor.SoftBleeds); }
    public bool WatercolorPaperTexture { get => _watercolorPaperTexture; set => SetStandardLaneModifierAndRefresh(ref _watercolorPaperTexture, value, SharedLaneKeys.Watercolor.LaneId, SharedLaneKeys.Watercolor.PaperTexture); }
    public bool WatercolorInkAndWatercolor { get => _watercolorInkAndWatercolor; set => SetStandardLaneModifierAndRefresh(ref _watercolorInkAndWatercolor, value, SharedLaneKeys.Watercolor.LaneId, SharedLaneKeys.Watercolor.InkAndWatercolor); }
    public bool WatercolorAtmosphericWash { get => _watercolorAtmosphericWash; set => SetStandardLaneModifierAndRefresh(ref _watercolorAtmosphericWash, value, SharedLaneKeys.Watercolor.LaneId, SharedLaneKeys.Watercolor.AtmosphericWash); }
    public bool WatercolorGouacheAccents { get => _watercolorGouacheAccents; set => SetStandardLaneModifierAndRefresh(ref _watercolorGouacheAccents, value, SharedLaneKeys.Watercolor.LaneId, SharedLaneKeys.Watercolor.GouacheAccents); }
    public string FantasyIllustrationRegister { get => _fantasyIllustrationState.Register; set => SetStandardLaneSelectorAndRefresh(ref _fantasyIllustrationState.Register, value, SharedLaneKeys.FantasyIllustration.LaneId, SharedLaneKeys.FantasyIllustration.RegisterSelector); }
    public bool FantasyIllustrationCharacterSketch { get => _fantasyIllustrationState.CharacterSketch; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.CharacterSketch, value, SharedLaneKeys.FantasyIllustration.CharacterSketch); }
    public bool FantasyIllustrationCharacterCentric { get => _fantasyIllustrationState.CharacterCentric; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.CharacterCentric, value, SharedLaneKeys.FantasyIllustration.CharacterCentric); }
    public bool FantasyIllustrationEnvironmentConcept { get => _fantasyIllustrationState.EnvironmentConcept; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.EnvironmentConcept, value, SharedLaneKeys.FantasyIllustration.EnvironmentConcept); }
    public bool FantasyIllustrationKeyArt { get => _fantasyIllustrationState.KeyArt; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.KeyArt, value, SharedLaneKeys.FantasyIllustration.KeyArt); }
    public bool FantasyIllustrationCleanBackground { get => _fantasyIllustrationState.CleanBackground; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.CleanBackground, value, SharedLaneKeys.FantasyIllustration.CleanBackground); }
    public bool FantasyIllustrationSilhouetteReadability { get => _fantasyIllustrationState.SilhouetteReadability; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.SilhouetteReadability, value, SharedLaneKeys.FantasyIllustration.SilhouetteReadability); }
    public bool FantasyIllustrationPhotorealistic { get => _fantasyIllustrationState.Photorealistic; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.Photorealistic, value, SharedLaneKeys.FantasyIllustration.Photorealistic); }
    public bool FantasyIllustrationCartoonArt { get => _fantasyIllustrationState.CartoonArt; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.CartoonArt, value, SharedLaneKeys.FantasyIllustration.CartoonArt); }
    public bool FantasyIllustrationPropArtifactFocus { get => _fantasyIllustrationState.PropArtifactFocus; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.PropArtifactFocus, value, SharedLaneKeys.FantasyIllustration.PropArtifactFocus); }
    public bool FantasyIllustrationCreatureDesign { get => _fantasyIllustrationState.CreatureDesignEnabled; set => SetFantasyIllustrationModifierAndRefresh(ref _fantasyIllustrationState.CreatureDesignEnabled, value, SharedLaneKeys.FantasyIllustration.CreatureDesign); }
    public bool EditorialIllustrationBlackAndWhiteMonochrome { get => _editorialIllustrationBlackAndWhiteMonochrome; set => SetEditorialIllustrationModifierAndRefresh(ref _editorialIllustrationBlackAndWhiteMonochrome, value, SharedLaneKeys.EditorialIllustration.BlackAndWhiteMonochrome); }
    public string GraphicDesignType { get => _graphicDesignType; set => SetStandardLaneSelectorAndRefresh(ref _graphicDesignType, value, SharedLaneKeys.GraphicDesign.LaneId, SharedLaneKeys.GraphicDesign.TypeSelector); }
    public bool GraphicDesignMinimalLayout { get => _graphicDesignMinimalLayout; set => SetGraphicDesignModifierAndRefresh(ref _graphicDesignMinimalLayout, value, SharedLaneKeys.GraphicDesign.MinimalLayout); }
    public bool GraphicDesignBoldHierarchy { get => _graphicDesignBoldHierarchy; set => SetGraphicDesignModifierAndRefresh(ref _graphicDesignBoldHierarchy, value, SharedLaneKeys.GraphicDesign.BoldHierarchy); }
    public string InfographicDataVisualizationSubdomain
    {
        get => _infographicDataVisualizationSubdomain;
        set
        {
            if (SetStandardLaneSelectorAndRefresh(ref _infographicDataVisualizationSubdomain, value, SharedLaneKeys.InfographicDataVisualization.LaneId, SharedLaneKeys.InfographicDataVisualization.SubdomainSelector))
            {
                OnPropertyChanged(nameof(IsInfographicSubdomainActive));
                OnPropertyChanged(nameof(IsDataVizSubdomainActive));
                OnPropertyChanged(nameof(ShowInfographicModeGroup));
                OnPropertyChanged(nameof(ShowDataVizModeGroup));
                if (!_isApplyingConfiguration)
                {
                    EnsureInfographicDataVisualizationModeDefaults();
                    SyncInfographicDataVisualizationSliderSuppressions();
                    RegeneratePrompt();
                }
            }
        }
    }
    public bool InfographicModeLeanExplainer { get => _infographicModeLeanExplainer; set => SetInfographicMode(ref _infographicModeLeanExplainer, value, SharedLaneKeys.InfographicDataVisualization.LeanExplainer); }
    public bool InfographicModePublicPoster { get => _infographicModePublicPoster; set => SetInfographicMode(ref _infographicModePublicPoster, value, SharedLaneKeys.InfographicDataVisualization.PublicPoster); }
    public bool InfographicModeReferenceSheet { get => _infographicModeReferenceSheet; set => SetInfographicMode(ref _infographicModeReferenceSheet, value, SharedLaneKeys.InfographicDataVisualization.ReferenceSheet); }
    public bool DataVizModeChartPurity { get => _dataVizModeChartPurity; set => SetDataVizMode(ref _dataVizModeChartPurity, value, SharedLaneKeys.InfographicDataVisualization.ChartPurity); }
    public bool DataVizModeDashboard { get => _dataVizModeDashboard; set => SetDataVizMode(ref _dataVizModeDashboard, value, SharedLaneKeys.InfographicDataVisualization.Dashboard); }
    public bool DataVizModeReportGraphic { get => _dataVizModeReportGraphic; set => SetDataVizMode(ref _dataVizModeReportGraphic, value, SharedLaneKeys.InfographicDataVisualization.ReportGraphic); }
    public string PhotographyType { get => _photographyType; set => SetStandardLaneSelectorAndRefresh(ref _photographyType, value, SharedLaneKeys.Photography.LaneId, SharedLaneKeys.Photography.TypeSelector); }
    public string PhotographyEra { get => _photographyEra; set => SetStandardLaneSelectorAndRefresh(ref _photographyEra, value, SharedLaneKeys.Photography.LaneId, SharedLaneKeys.Photography.EraSelector); }
    public bool PhotographyCandidCapture { get => _photographyCandidCapture; set => SetStandardLaneModifierAndRefresh(ref _photographyCandidCapture, value, SharedLaneKeys.Photography.LaneId, nameof(PromptConfiguration.PhotographyCandidCapture)); }
    public bool PhotographyPosedStagedCapture { get => _photographyPosedStagedCapture; set => SetStandardLaneModifierAndRefresh(ref _photographyPosedStagedCapture, value, SharedLaneKeys.Photography.LaneId, nameof(PromptConfiguration.PhotographyPosedStagedCapture)); }
    public bool PhotographyAvailableLight { get => _photographyAvailableLight; set => SetStandardLaneModifierAndRefresh(ref _photographyAvailableLight, value, SharedLaneKeys.Photography.LaneId, nameof(PromptConfiguration.PhotographyAvailableLight)); }
    public bool PhotographyOnCameraFlash { get => _photographyOnCameraFlash; set => SetStandardLaneModifierAndRefresh(ref _photographyOnCameraFlash, value, SharedLaneKeys.Photography.LaneId, nameof(PromptConfiguration.PhotographyOnCameraFlash)); }
    public bool PhotographyEditorialPolish { get => _photographyEditorialPolish; set => SetStandardLaneModifierAndRefresh(ref _photographyEditorialPolish, value, SharedLaneKeys.Photography.LaneId, nameof(PromptConfiguration.PhotographyEditorialPolish)); }
    public bool PhotographyRawDocumentaryTexture { get => _photographyRawDocumentaryTexture; set => SetStandardLaneModifierAndRefresh(ref _photographyRawDocumentaryTexture, value, SharedLaneKeys.Photography.LaneId, nameof(PromptConfiguration.PhotographyRawDocumentaryTexture)); }
    public bool PhotographyEnvironmentalPortraitContext { get => _photographyEnvironmentalPortraitContext; set => SetStandardLaneModifierAndRefresh(ref _photographyEnvironmentalPortraitContext, value, SharedLaneKeys.Photography.LaneId, nameof(PromptConfiguration.PhotographyEnvironmentalPortraitContext)); }
    public bool PhotographyFilmAnalogCharacter { get => _photographyFilmAnalogCharacter; set => SetStandardLaneModifierAndRefresh(ref _photographyFilmAnalogCharacter, value, SharedLaneKeys.Photography.LaneId, nameof(PromptConfiguration.PhotographyFilmAnalogCharacter)); }
    public string ProductPhotographyShotType { get => _productPhotographyShotType; set => SetStandardLaneSelectorAndRefresh(ref _productPhotographyShotType, value, SharedLaneKeys.ProductPhotography.LaneId, SharedLaneKeys.ProductPhotography.ShotTypeSelector); }
    public bool ProductPhotographyWithPackaging { get => _productPhotographyWithPackaging; set => SetStandardLaneModifierAndRefresh(ref _productPhotographyWithPackaging, value, SharedLaneKeys.ProductPhotography.LaneId, SharedLaneKeys.ProductPhotography.WithPackaging); }
    public bool ProductPhotographyPedestalDisplay { get => _productPhotographyPedestalDisplay; set => SetStandardLaneModifierAndRefresh(ref _productPhotographyPedestalDisplay, value, SharedLaneKeys.ProductPhotography.LaneId, SharedLaneKeys.ProductPhotography.PedestalDisplay); }
    public bool ProductPhotographyReflectiveSurface { get => _productPhotographyReflectiveSurface; set => SetStandardLaneModifierAndRefresh(ref _productPhotographyReflectiveSurface, value, SharedLaneKeys.ProductPhotography.LaneId, SharedLaneKeys.ProductPhotography.ReflectiveSurface); }
    public bool ProductPhotographyFloatingPresentation { get => _productPhotographyFloatingPresentation; set => SetStandardLaneModifierAndRefresh(ref _productPhotographyFloatingPresentation, value, SharedLaneKeys.ProductPhotography.LaneId, SharedLaneKeys.ProductPhotography.FloatingPresentation); }
    public bool ProductPhotographyScaleCueHand { get => _productPhotographyScaleCueHand; set => SetStandardLaneModifierAndRefresh(ref _productPhotographyScaleCueHand, value, SharedLaneKeys.ProductPhotography.LaneId, SharedLaneKeys.ProductPhotography.ScaleCueHand); }
    public bool ProductPhotographyBrandProps { get => _productPhotographyBrandProps; set => SetStandardLaneModifierAndRefresh(ref _productPhotographyBrandProps, value, SharedLaneKeys.ProductPhotography.LaneId, SharedLaneKeys.ProductPhotography.BrandProps); }
    public bool ProductPhotographyGroupedVariants { get => _productPhotographyGroupedVariants; set => SetStandardLaneModifierAndRefresh(ref _productPhotographyGroupedVariants, value, SharedLaneKeys.ProductPhotography.LaneId, SharedLaneKeys.ProductPhotography.GroupedVariants); }
    public string FoodPhotographyShotMode { get => _foodPhotographyShotMode; set => SetStandardLaneSelectorAndRefresh(ref _foodPhotographyShotMode, value, SharedLaneKeys.FoodPhotography.LaneId, SharedLaneKeys.FoodPhotography.ShotModeSelector); }
    public bool FoodPhotographyVisibleSteam { get => _foodPhotographyVisibleSteam; set => SetStandardLaneModifierAndRefresh(ref _foodPhotographyVisibleSteam, value, SharedLaneKeys.FoodPhotography.LaneId, SharedLaneKeys.FoodPhotography.VisibleSteam); }
    public bool FoodPhotographyGarnishEmphasis { get => _foodPhotographyGarnishEmphasis; set => SetStandardLaneModifierAndRefresh(ref _foodPhotographyGarnishEmphasis, value, SharedLaneKeys.FoodPhotography.LaneId, SharedLaneKeys.FoodPhotography.GarnishEmphasis); }
    public bool FoodPhotographyUtensilContext { get => _foodPhotographyUtensilContext; set => SetStandardLaneModifierAndRefresh(ref _foodPhotographyUtensilContext, value, SharedLaneKeys.FoodPhotography.LaneId, SharedLaneKeys.FoodPhotography.UtensilContext); }
    public bool FoodPhotographyHandServiceCue { get => _foodPhotographyHandServiceCue; set => SetStandardLaneModifierAndRefresh(ref _foodPhotographyHandServiceCue, value, SharedLaneKeys.FoodPhotography.LaneId, SharedLaneKeys.FoodPhotography.HandServiceCue); }
    public bool FoodPhotographyIngredientScatter { get => _foodPhotographyIngredientScatter; set => SetStandardLaneModifierAndRefresh(ref _foodPhotographyIngredientScatter, value, SharedLaneKeys.FoodPhotography.LaneId, SharedLaneKeys.FoodPhotography.IngredientScatter); }
    public bool FoodPhotographyCondensationEmphasis { get => _foodPhotographyCondensationEmphasis; set => SetStandardLaneModifierAndRefresh(ref _foodPhotographyCondensationEmphasis, value, SharedLaneKeys.FoodPhotography.LaneId, SharedLaneKeys.FoodPhotography.CondensationEmphasis); }
    public string LifestyleAdvertisingShotMode { get => _lifestyleAdvertisingShotMode; set => SetStandardLaneSelectorAndRefresh(ref _lifestyleAdvertisingShotMode, value, SharedLaneKeys.LifestyleAdvertising.LaneId, SharedLaneKeys.LifestyleAdvertising.ShotModeSelector); }
    public bool LifestyleAdvertisingNaturalInteraction { get => _lifestyleAdvertisingNaturalInteraction; set => SetStandardLaneModifierAndRefresh(ref _lifestyleAdvertisingNaturalInteraction, value, SharedLaneKeys.LifestyleAdvertising.LaneId, SharedLaneKeys.LifestyleAdvertising.NaturalInteraction); }
    public bool LifestyleAdvertisingProductInUse { get => _lifestyleAdvertisingProductInUse; set => SetStandardLaneModifierAndRefresh(ref _lifestyleAdvertisingProductInUse, value, SharedLaneKeys.LifestyleAdvertising.LaneId, SharedLaneKeys.LifestyleAdvertising.ProductInUse); }
    public bool LifestyleAdvertisingBrandColorAccent { get => _lifestyleAdvertisingBrandColorAccent; set => SetStandardLaneModifierAndRefresh(ref _lifestyleAdvertisingBrandColorAccent, value, SharedLaneKeys.LifestyleAdvertising.LaneId, SharedLaneKeys.LifestyleAdvertising.BrandColorAccent); }
    public bool LifestyleAdvertisingPropContext { get => _lifestyleAdvertisingPropContext; set => SetStandardLaneModifierAndRefresh(ref _lifestyleAdvertisingPropContext, value, SharedLaneKeys.LifestyleAdvertising.LaneId, SharedLaneKeys.LifestyleAdvertising.PropContext); }
    public bool LifestyleAdvertisingSunlitOptimism { get => _lifestyleAdvertisingSunlitOptimism; set => SetStandardLaneModifierAndRefresh(ref _lifestyleAdvertisingSunlitOptimism, value, SharedLaneKeys.LifestyleAdvertising.LaneId, SharedLaneKeys.LifestyleAdvertising.SunlitOptimism); }
    public bool LifestyleAdvertisingMotionCandidness { get => _lifestyleAdvertisingMotionCandidness; set => SetStandardLaneModifierAndRefresh(ref _lifestyleAdvertisingMotionCandidness, value, SharedLaneKeys.LifestyleAdvertising.LaneId, SharedLaneKeys.LifestyleAdvertising.MotionCandidness); }
    public string ArchitectureArchvizViewMode { get => _architectureArchvizViewMode; set => SetStandardLaneSelectorAndRefresh(ref _architectureArchvizViewMode, value, SharedLaneKeys.ArchitectureArchviz.LaneId, SharedLaneKeys.ArchitectureArchviz.ViewModeSelector); }
    public bool ArchitectureArchvizHumanScaleCues { get => _architectureArchvizHumanScaleCues; set => SetStandardLaneModifierAndRefresh(ref _architectureArchvizHumanScaleCues, value, SharedLaneKeys.ArchitectureArchviz.LaneId, SharedLaneKeys.ArchitectureArchviz.HumanScaleCues); }
    public bool ArchitectureArchvizLandscapeEmphasis { get => _architectureArchvizLandscapeEmphasis; set => SetStandardLaneModifierAndRefresh(ref _architectureArchvizLandscapeEmphasis, value, SharedLaneKeys.ArchitectureArchviz.LaneId, SharedLaneKeys.ArchitectureArchviz.LandscapeEmphasis); }
    public bool ArchitectureArchvizFurnishingEmphasis { get => _architectureArchvizFurnishingEmphasis; set => SetStandardLaneModifierAndRefresh(ref _architectureArchvizFurnishingEmphasis, value, SharedLaneKeys.ArchitectureArchviz.LaneId, SharedLaneKeys.ArchitectureArchviz.FurnishingEmphasis); }
    public bool ArchitectureArchvizWarmInteriorGlow { get => _architectureArchvizWarmInteriorGlow; set => SetStandardLaneModifierAndRefresh(ref _architectureArchvizWarmInteriorGlow, value, SharedLaneKeys.ArchitectureArchviz.LaneId, SharedLaneKeys.ArchitectureArchviz.WarmInteriorGlow); }
    public bool ArchitectureArchvizReflectiveSurfaceAccents { get => _architectureArchvizReflectiveSurfaceAccents; set => SetStandardLaneModifierAndRefresh(ref _architectureArchvizReflectiveSurfaceAccents, value, SharedLaneKeys.ArchitectureArchviz.LaneId, SharedLaneKeys.ArchitectureArchviz.ReflectiveSurfaceAccents); }
    public bool ArchitectureArchvizAmenityFocus { get => _architectureArchvizAmenityFocus; set => SetStandardLaneModifierAndRefresh(ref _architectureArchvizAmenityFocus, value, SharedLaneKeys.ArchitectureArchviz.LaneId, SharedLaneKeys.ArchitectureArchviz.AmenityFocus); }
    public string ArtistInfluencePrimary
    {
        get => _artistInfluencePrimary;
        set
        {
            if (SetArtistAndRefresh(ref _artistInfluencePrimary, value))
            {
                HandleArtistSelectionChanged(isPrimary: true);
                RefreshArtistPairGuidance();
            }
        }
    }

    public int InfluenceStrengthPrimary
    {
        get => _influenceStrengthPrimary;
        set
        {
            if (SetArtistAndRefresh(ref _influenceStrengthPrimary, value))
            {
                RefreshArtistPairGuidance();
            }
        }
    }

    public string ArtistInfluenceSecondary
    {
        get => _artistInfluenceSecondary;
        set
        {
            if (SetArtistAndRefresh(ref _artistInfluenceSecondary, value))
            {
                HandleArtistSelectionChanged(isPrimary: false);
                RefreshArtistPairGuidance();
            }
        }
    }

    public int InfluenceStrengthSecondary
    {
        get => _influenceStrengthSecondary;
        set
        {
            if (SetArtistAndRefresh(ref _influenceStrengthSecondary, value))
            {
                RefreshArtistPairGuidance();
            }
        }
    }
    public int CameraDistance { get => _cameraDistance; set => SetAndRefresh(ref _cameraDistance, value); }
    public bool ExcludeCameraDistanceFromPrompt { get => _excludeCameraDistanceFromPrompt; set => SetAndRefresh(ref _excludeCameraDistanceFromPrompt, value); }
    public int CameraAngle { get => _cameraAngle; set => SetAndRefresh(ref _cameraAngle, value); }
    public bool ExcludeCameraAngleFromPrompt { get => _excludeCameraAngleFromPrompt; set => SetAndRefresh(ref _excludeCameraAngleFromPrompt, value); }
    public int BackgroundComplexity { get => _backgroundComplexity; set => SetAndRefresh(ref _backgroundComplexity, value); }
    public bool ExcludeBackgroundComplexityFromPrompt { get => _excludeBackgroundComplexityFromPrompt; set => SetAndRefresh(ref _excludeBackgroundComplexityFromPrompt, value); }
    public int MotionEnergy { get => _motionEnergy; set => SetAndRefresh(ref _motionEnergy, value); }
    public bool ExcludeMotionEnergyFromPrompt { get => _excludeMotionEnergyFromPrompt; set => SetAndRefresh(ref _excludeMotionEnergyFromPrompt, value); }
    public int FocusDepth { get => _focusDepth; set => SetAndRefresh(ref _focusDepth, value); }
    public bool ExcludeFocusDepthFromPrompt { get => _excludeFocusDepthFromPrompt; set => SetAndRefresh(ref _excludeFocusDepthFromPrompt, value); }
    public int ImageCleanliness { get => _imageCleanliness; set => SetAndRefresh(ref _imageCleanliness, value); }
    public bool ExcludeImageCleanlinessFromPrompt { get => _excludeImageCleanlinessFromPrompt; set => SetAndRefresh(ref _excludeImageCleanlinessFromPrompt, value); }
    public int DetailDensity { get => _detailDensity; set => SetAndRefresh(ref _detailDensity, value); }
    public bool ExcludeDetailDensityFromPrompt { get => _excludeDetailDensityFromPrompt; set => SetAndRefresh(ref _excludeDetailDensityFromPrompt, value); }
    public int Whimsy { get => _whimsy; set => SetAndRefresh(ref _whimsy, value); }
    public bool ExcludeWhimsyFromPrompt { get => _excludeWhimsyFromPrompt; set => SetAndRefresh(ref _excludeWhimsyFromPrompt, value); }
    public int Tension { get => _tension; set => SetAndRefresh(ref _tension, value); }
    public bool ExcludeTensionFromPrompt { get => _excludeTensionFromPrompt; set => SetAndRefresh(ref _excludeTensionFromPrompt, value); }
    public int Awe { get => _awe; set => SetAndRefresh(ref _awe, value); }
    public bool ExcludeAweFromPrompt { get => _excludeAweFromPrompt; set => SetAndRefresh(ref _excludeAweFromPrompt, value); }
    public string Lighting { get => _lighting; set => SetAndRefresh(ref _lighting, value); }
    public int Saturation { get => _saturation; set => SetAndRefresh(ref _saturation, value); }
    public bool ExcludeSaturationFromPrompt { get => _excludeSaturationFromPrompt; set => SetAndRefresh(ref _excludeSaturationFromPrompt, value); }
    public int Contrast { get => _contrast; set => SetAndRefresh(ref _contrast, value); }
    public bool ExcludeContrastFromPrompt { get => _excludeContrastFromPrompt; set => SetAndRefresh(ref _excludeContrastFromPrompt, value); }
    public string AspectRatio { get => _aspectRatio; set => SetAndRefresh(ref _aspectRatio, value); }
    public bool PrintReady { get => _printReady; set => SetAndRefresh(ref _printReady, value); }
    public bool TransparentBackground { get => _transparentBackground; set => SetAndRefresh(ref _transparentBackground, value); }
    public bool UseNegativePrompt
    {
        get => _useNegativePrompt;
        set
        {
            if (SetAndRefresh(ref _useNegativePrompt, value))
            {
                OnPropertyChanged(nameof(ShowNegativePrompt));
                OnPropertyChanged(nameof(ShowManualNegativeConstraints));
            }
        }
    }
    public bool CanUseCompressionControls => !IsLockedLaneActive && IntentModeCatalog.TryGet(IntentMode, out _);
    public bool CompressPromptSemantics
    {
        get => _compressPromptSemantics;
        set => SetCompressionTier1(value);
    }
    public bool ReduceRepeatedLaneWords
    {
        get => _reduceRepeatedLaneWords;
        set => SetCompressionTier2(value);
    }
    public bool TrimRepeatedLongWords
    {
        get => _trimRepeatedLongWords;
        set => SetCompressionTier3(value);
    }
    public bool SemanticPairInteractions
    {
        get => _semanticPairInteractions;
        set => SetAndRefresh(ref _semanticPairInteractions, value);
    }
    public bool ShowCompressionTierTwo => CanUseCompressionControls && CompressPromptSemantics;
    public bool ShowCompressionTierThree => CanUseCompressionControls && CompressPromptSemantics && ReduceRepeatedLaneWords;
    public bool ShowLaneHelpFooter => LaneHelpTooltipCatalog.TryGet(IntentMode, out _);
    public bool HasLaneHelpTooltip => ShowLaneHelpFooter;
    public string LaneHelpFooterPrefix => ShowLaneHelpFooter ? LaneHelpTooltipCatalog.FooterContactPrefixText : string.Empty;
    public string LaneHelpFooterEmail => ShowLaneHelpFooter ? LaneHelpTooltipCatalog.ContactEmail : string.Empty;
    public string LaneHelpPrinciple => LaneHelpTooltipCatalog.TryGet(IntentMode, out var content) ? content.Principle : string.Empty;
    public string LaneHelpWeak => LaneHelpTooltipCatalog.TryGet(IntentMode, out var content) ? content.Weak : string.Empty;
    public string LaneHelpStronger => LaneHelpTooltipCatalog.TryGet(IntentMode, out var content) ? content.Stronger : string.Empty;
    public bool AvoidClutter { get => _avoidClutter; set => SetAndRefresh(ref _avoidClutter, value); }
    public bool AvoidMuddyLighting { get => _avoidMuddyLighting; set => SetAndRefresh(ref _avoidMuddyLighting, value); }
    public bool AvoidDistortedAnatomy { get => _avoidDistortedAnatomy; set => SetAndRefresh(ref _avoidDistortedAnatomy, value); }
    public bool AvoidExtraLimbs { get => _avoidExtraLimbs; set => SetAndRefresh(ref _avoidExtraLimbs, value); }
    public bool AvoidTextArtifacts { get => _avoidTextArtifacts; set => SetAndRefresh(ref _avoidTextArtifacts, value); }
    public bool AvoidOversaturation { get => _avoidOversaturation; set => SetAndRefresh(ref _avoidOversaturation, value); }
    public bool AvoidFlatComposition { get => _avoidFlatComposition; set => SetAndRefresh(ref _avoidFlatComposition, value); }
    public bool AvoidMessyBackground { get => _avoidMessyBackground; set => SetAndRefresh(ref _avoidMessyBackground, value); }
    public bool AvoidWeakMaterialDefinition { get => _avoidWeakMaterialDefinition; set => SetAndRefresh(ref _avoidWeakMaterialDefinition, value); }
    public bool AvoidBlurryDetail { get => _avoidBlurryDetail; set => SetAndRefresh(ref _avoidBlurryDetail, value); }
    public string SelectedThemeName
    {
        get => _selectedThemeName;
        set
        {
            if (SetProperty(ref _selectedThemeName, value) && !string.IsNullOrWhiteSpace(value))
            {
                _themeService.ApplyTheme(value);
                StatusMessage = $"Skin changed to {value}.";
            }
        }
    }
    public string LockedLaneHeadline => "Locked.";
    public string LockedLaneBody => string.Empty;
    public string VintageBendLockedHeadline => LockedLaneHeadline;
    public string VintageBendLockedBody => LockedLaneBody;
    public int MaxDemoCopies => DemoModeOptions.MaxDemoCopies;
    public int RemainingDemoCopies
    {
        get => _remainingDemoCopies;
        private set
        {
            if (SetProperty(ref _remainingDemoCopies, value))
            {
                OnPropertyChanged(nameof(IsDemoExpired));
                OnPropertyChanged(nameof(IsLockedLaneActive));
                OnPropertyChanged(nameof(IsVintageBendLocked));
                OnPropertyChanged(nameof(DemoModeHeadline));
                OnPropertyChanged(nameof(CopyPromptRemainingText));
                OnPropertyChanged(nameof(DemoModeBody));
                OnPropertyChanged(nameof(ShowInteractivePromptPreview));
                OnPropertyChanged(nameof(ShowDemoPromptPreview));
                OnPropertyChanged(nameof(ShowAuthoringWorkspace));
                OnPropertyChanged(nameof(ShowDemoExpiredLockScreen));
                OnPropertyChanged(nameof(ShowLockedLaneAuthoringSections));
                OnPropertyChanged(nameof(ShowVintageBendAuthoringSections));
                OnPropertyChanged(nameof(ShowLockedLanePane));
                OnPropertyChanged(nameof(ShowVintageBendLockedPane));
                OnPropertyChanged(nameof(ShowCompactCinematicPanel));
                OnPropertyChanged(nameof(ShowActiveStandardLanePanel));
                OnPropertyChanged(nameof(ShowResolvedStandardLanePanel));
                RaiseCopyCommandCanExecuteChanged();
            }
        }
    }
    public bool IsCustomIntent => string.Equals(IntentMode, "Custom", StringComparison.OrdinalIgnoreCase);
    public bool ShowCustomRandomizeControls => string.Equals(IntentMode, "Custom", StringComparison.OrdinalIgnoreCase);
    public bool ExcludeArtistSlidersFromRandomize { get => _excludeArtistSlidersFromRandomize; set => SetProperty(ref _excludeArtistSlidersFromRandomize, value); }
    public bool ShowManualIntentControls => !IsLockedLaneActive && (string.Equals(IntentMode, "Custom", StringComparison.OrdinalIgnoreCase) || LaneRegistry.TryGetByIntentName(IntentMode, out _) || IsExperimentalManualAdvancedMode);
    public bool ShowLegacyManualCompositionCard => !IsLockedLaneActive && (IsCustomIntent || IsExperimentalManualAdvancedMode);
    public bool ShowEmbeddedLaneCompositionCard => ShowManualIntentControls && !IsCustomIntent && !IsExperimentalIntent;
    public bool ShowManualNegativeConstraints => ShowManualIntentControls && UseNegativePrompt;
    public bool IsAnimeIntent => IntentModeCatalog.IsAnime(IntentMode);
    private bool IsAnimeCompactPresentationActive => IsAnimeIntent && (IsCompactPresentationMode || AnimeCompactPanelEnabled);
    private bool IsComicBookCompactPresentationActive => IsComicBookIntent && IsCompactPresentationMode;
    private bool IsCinematicCompactPresentationActive => IsCinematicIntent && IsCompactPresentationMode;
    private bool IsWatercolorCompactPresentationActive => IsWatercolorIntent && IsCompactPresentationMode;
    private bool IsChildrensBookCompactPresentationActive => IsChildrensBookIntent && IsCompactPresentationMode;
    private bool IsThreeDRenderCompactPresentationActive => IsThreeDRenderIntent && IsCompactPresentationMode;
    private bool IsArchitectureArchvizCompactPresentationActive => IsArchitectureArchvizIntent && IsCompactPresentationMode;
    private bool IsTattooArtCompactPresentationActive => IsTattooArtIntent && IsCompactPresentationMode;
    private bool IsCompactManualPresentationActive => IsAnimeCompactPresentationActive || IsComicBookCompactPresentationActive || IsCinematicCompactPresentationActive || IsWatercolorCompactPresentationActive || IsChildrensBookCompactPresentationActive || IsThreeDRenderCompactPresentationActive || IsArchitectureArchvizCompactPresentationActive || IsTattooArtCompactPresentationActive;
    public bool ShowStandardManualStyleControlsCard => ShowManualIntentControls && !IsCompactManualPresentationActive;
    public bool ShowCompactAnimeManualStyleControlsCard => ShowManualIntentControls && IsAnimeCompactPresentationActive;
    public bool ShowCompactComicBookManualStack => ShowManualIntentControls && IsComicBookCompactPresentationActive;
    public bool ShowCompactCinematicManualStack => ShowManualIntentControls && IsCinematicCompactPresentationActive;
    public bool ShowCompactWatercolorManualStack => ShowManualIntentControls && IsWatercolorCompactPresentationActive;
    public bool ShowCompactChildrensBookManualStack => ShowManualIntentControls && IsChildrensBookCompactPresentationActive;
    public bool ShowCompactThreeDRenderManualStack => ShowManualIntentControls && IsThreeDRenderCompactPresentationActive;
    public bool ShowCompactArchitectureArchvizManualStack => ShowManualIntentControls && IsArchitectureArchvizCompactPresentationActive;
    public bool ShowCompactTattooArtManualStack => ShowManualIntentControls && IsTattooArtCompactPresentationActive;
    public bool ShowStandardManualMoodCard => ShowManualIntentControls && !IsCompactManualPresentationActive;
    public bool ShowCompactAnimeManualMoodCard => ShowManualIntentControls && IsAnimeCompactPresentationActive;
    public bool ShowStandardManualLightingAndColorCard => ShowManualIntentControls && !IsCompactManualPresentationActive;
    public bool ShowCompactAnimeManualLightingAndColorCard => ShowManualIntentControls && IsAnimeCompactPresentationActive;
    public bool ShowStandardManualImageFinishCard => ShowManualIntentControls && !IsCompactManualPresentationActive;
    public bool ShowCompactAnimeManualImageFinishCard => ShowManualIntentControls && IsAnimeCompactPresentationActive;
    public bool ShowStandardManualOutputCard => ShowManualIntentControls && !IsCompactManualPresentationActive;
    public bool ShowCompactAnimeManualOutputCard => ShowManualIntentControls && IsAnimeCompactPresentationActive;
    public bool ShowAnimeModifierPanel => IsAnimeIntent;
    public bool ShowStandardAnimePanel => IsAnimeIntent && !IsAnimeCompactPresentationActive;
    public bool ShowCompactAnimePanel => IsAnimeCompactPresentationActive;
    public bool ShowSubjectContextFields => !IsCompactManualPresentationActive;
    public bool IsChildrensBookIntent => IntentModeCatalog.IsChildrensBook(IntentMode);
    public bool IsComicBookIntent => IntentModeCatalog.IsComicBook(IntentMode);
    public bool ShowComicBookModifierPanel => IsComicBookIntent;
    public bool IsCinematicIntent => IntentModeCatalog.IsCinematic(IntentMode);
    public bool ShowCompactCinematicPanel => !IsDemoExpired && !IsLockedLaneActive && IsCinematicCompactPresentationActive && ActiveStandardLanePanel is not null;
    public bool ShowCompactWatercolorPanel => !IsDemoExpired && !IsLockedLaneActive && IsWatercolorCompactPresentationActive && ActiveStandardLanePanel is not null;
    public bool ShowCompactChildrensBookPanel => !IsDemoExpired && !IsLockedLaneActive && IsChildrensBookCompactPresentationActive && ActiveStandardLanePanel is not null;
    public bool ShowCompactThreeDRenderPanel => !IsDemoExpired && !IsLockedLaneActive && IsThreeDRenderCompactPresentationActive && ActiveStandardLanePanel is not null;
    public bool ShowCompactTattooArtPanel => !IsDemoExpired && !IsLockedLaneActive && IsTattooArtCompactPresentationActive && ActiveStandardLanePanel is not null;
    public bool ShowResolvedStandardLanePanel => ShowActiveStandardLanePanel && !IsTattooArtCompactPresentationActive;
    public bool IsPhotographyIntent => IntentModeCatalog.IsPhotography(IntentMode);
    public bool IsProductPhotographyIntent => IntentModeCatalog.IsProductPhotography(IntentMode);
    public bool IsFoodPhotographyIntent => IntentModeCatalog.IsFoodPhotography(IntentMode);
    public bool IsLifestyleAdvertisingPhotographyIntent => IntentModeCatalog.IsLifestyleAdvertisingPhotography(IntentMode);
    public bool IsArchitectureArchvizIntent => IntentModeCatalog.IsArchitectureArchviz(IntentMode);
    public bool IsThreeDRenderIntent => IntentModeCatalog.IsThreeDRender(IntentMode);
    public bool IsConceptArtIntent => IntentModeCatalog.IsConceptArt(IntentMode);
    public bool IsPixelArtIntent => IntentModeCatalog.IsPixelArt(IntentMode);
    public bool IsFantasyIllustrationIntent => IntentModeCatalog.IsFantasyIllustration(IntentMode);
    public bool IsEditorialIllustrationIntent => IntentModeCatalog.IsEditorialIllustration(IntentMode);
    public bool IsGraphicDesignIntent => IntentModeCatalog.IsGraphicDesign(IntentMode);
    public bool IsInfographicDataVisualizationIntent => IntentModeCatalog.IsInfographicDataVisualization(IntentMode);
    public bool IsTattooArtIntent => IntentModeCatalog.IsTattooArt(IntentMode);
    public bool IsWatercolorIntent => IntentModeCatalog.IsWatercolor(IntentMode);
    public bool IsVintageBendIntent => IntentModeCatalog.IsVintageBend(IntentMode);
    public bool ShowVintageBendModifierPanel => IsVintageBendIntent && !IsLockedLaneActive;
    public bool IsInfographicSubdomainActive => IsInfographicDataVisualizationIntent && string.Equals(InfographicDataVisualizationSubdomain, SharedLaneKeys.InfographicDataVisualization.Infographic, StringComparison.Ordinal);
    public bool IsDataVizSubdomainActive => IsInfographicDataVisualizationIntent && string.Equals(InfographicDataVisualizationSubdomain, SharedLaneKeys.InfographicDataVisualization.DataViz, StringComparison.Ordinal);
    public bool ShowInfographicModeGroup => !IsDemoExpired && !IsLockedLaneActive && IsInfographicSubdomainActive;
    public bool ShowDataVizModeGroup => !IsDemoExpired && !IsLockedLaneActive && IsDataVizSubdomainActive;
    public string IntentModeSummary => BuildIntentModeSummary();
    public bool VintageBendEasternBlocGdr { get => _vintageBendEasternBlocGdr; set => SetAndRefresh(ref _vintageBendEasternBlocGdr, value); }
    public bool VintageBendThrillerUndertone { get => _vintageBendThrillerUndertone; set => SetAndRefresh(ref _vintageBendThrillerUndertone, value); }
    public bool VintageBendInstitutionalAusterity { get => _vintageBendInstitutionalAusterity; set => SetAndRefresh(ref _vintageBendInstitutionalAusterity, value); }
    public bool VintageBendSurveillanceStateAtmosphere { get => _vintageBendSurveillanceStateAtmosphere; set => SetAndRefresh(ref _vintageBendSurveillanceStateAtmosphere, value); }
    public bool VintageBendPeriodArtifacts { get => _vintageBendPeriodArtifacts; set => SetAndRefresh(ref _vintageBendPeriodArtifacts, value); }
    public bool VintageBendUrbanCivilian { get => _vintageBendUrbanCivilian; set => SetAndRefresh(ref _vintageBendUrbanCivilian, value); }
    public string InfluenceStrengthPrimaryValueText => GetInfluenceBandLabel(InfluenceStrengthPrimary);
    public string InfluenceStrengthPrimaryGuideText => GetInfluenceBandGuideText();
    public string InfluenceStrengthSecondaryValueText => GetInfluenceBandLabel(InfluenceStrengthSecondary);
    public string InfluenceStrengthSecondaryGuideText => GetInfluenceBandGuideText();
    public string TemperatureHelper => GetSliderHelper("Temperature", Temperature);
    public string TemperatureValueText => GetSliderBandLabel("Temperature", Temperature);
    public string TemperatureGuideText => GetSliderBandGuide("Temperature");
    public string LightingIntensityHelper => GetSliderHelper("LightingIntensity", LightingIntensity);
    public string LightingIntensityValueText => GetSliderBandLabel("LightingIntensity", LightingIntensity);
    public string LightingIntensityGuideText => GetSliderBandGuide("LightingIntensity");
    public string StylizationHelper => GetSliderHelper("Stylization", Stylization);
    public string StylizationValueText => GetSliderBandLabel("Stylization", Stylization);
    public string StylizationGuideText => GetSliderBandGuide("Stylization");
    public string RealismHelper => GetSliderHelper("Realism", Realism);
    public string RealismValueText => GetSliderBandLabel("Realism", Realism);
    public string RealismGuideText => GetSliderBandGuide("Realism");
    public string TextureDepthHelper => GetSliderHelper("TextureDepth", TextureDepth);
    public string TextureDepthValueText => GetSliderBandLabel("TextureDepth", TextureDepth);
    public string TextureDepthGuideText => GetSliderBandGuide("TextureDepth");
    public string NarrativeDensityHelper => GetSliderHelper("NarrativeDensity", NarrativeDensity);
    public string NarrativeDensityValueText => GetSliderBandLabel("NarrativeDensity", NarrativeDensity);
    public string NarrativeDensityGuideText => GetSliderBandGuide("NarrativeDensity");
    public string SymbolismHelper => GetSliderHelper("Symbolism", Symbolism);
    public string SymbolismValueText => GetSliderBandLabel("Symbolism", Symbolism);
    public string SymbolismGuideText => GetSliderBandGuide("Symbolism");
    public string SurfaceAgeHelper => GetSliderHelper("SurfaceAge", SurfaceAge);
    public string SurfaceAgeValueText => GetSliderBandLabel("SurfaceAge", SurfaceAge);
    public string SurfaceAgeGuideText => GetSliderBandGuide("SurfaceAge");
    public string FramingHelper => GetSliderHelper("Framing", Framing);
    public string FramingValueText => GetSliderBandLabel("Framing", Framing);
    public string FramingGuideText => GetSliderBandGuide("Framing");
    public string CameraDistanceHelper => GetSliderHelper("CameraDistance", CameraDistance);
    public string CameraDistanceValueText => GetSliderBandLabel("CameraDistance", CameraDistance);
    public string CameraDistanceGuideText => GetSliderBandGuide("CameraDistance");
    public string CameraAngleHelper => GetSliderHelper("CameraAngle", CameraAngle);
    public string CameraAngleValueText => GetSliderBandLabel("CameraAngle", CameraAngle);
    public string CameraAngleGuideText => GetSliderBandGuide("CameraAngle");
    public string BackgroundComplexityHelper => GetSliderHelper("BackgroundComplexity", BackgroundComplexity);
    public string BackgroundComplexityValueText => GetSliderBandLabel("BackgroundComplexity", BackgroundComplexity);
    public string BackgroundComplexityGuideText => GetSliderBandGuide("BackgroundComplexity");
    public string MotionEnergyHelper => GetSliderHelper("MotionEnergy", MotionEnergy);
    public string MotionEnergyValueText => GetSliderBandLabel("MotionEnergy", MotionEnergy);
    public string MotionEnergyGuideText => GetSliderBandGuide("MotionEnergy");
    public string FocusDepthHelper => GetSliderHelper("FocusDepth", FocusDepth);
    public string FocusDepthValueText => GetSliderBandLabel("FocusDepth", FocusDepth);
    public string FocusDepthGuideText => GetSliderBandGuide("FocusDepth");
    public string ImageCleanlinessHelper => GetSliderHelper("ImageCleanliness", ImageCleanliness);
    public string ImageCleanlinessValueText => GetSliderBandLabel("ImageCleanliness", ImageCleanliness);
    public string ImageCleanlinessGuideText => GetSliderBandGuide("ImageCleanliness");
    public string DetailDensityHelper => GetSliderHelper("DetailDensity", DetailDensity);
    public string DetailDensityValueText => GetSliderBandLabel("DetailDensity", DetailDensity);
    public string DetailDensityGuideText => GetSliderBandGuide("DetailDensity");
    public string AtmosphericDepthHelper => GetSliderHelper("AtmosphericDepth", AtmosphericDepth);
    public string AtmosphericDepthValueText => GetSliderBandLabel("AtmosphericDepth", AtmosphericDepth);
    public string AtmosphericDepthGuideText => GetSliderBandGuide("AtmosphericDepth");
    public string ChaosHelper => GetSliderHelper("Chaos", Chaos);
    public string ChaosValueText => GetSliderBandLabel("Chaos", Chaos);
    public string ChaosGuideText => GetSliderBandGuide("Chaos");
    public string WhimsyHelper => GetSliderHelper("Whimsy", Whimsy);
    public string WhimsyValueText => GetSliderBandLabel("Whimsy", Whimsy);
    public string WhimsyGuideText => GetSliderBandGuide("Whimsy");
    public string TensionHelper => GetSliderHelper("Tension", Tension);
    public string TensionValueText => GetSliderBandLabel("Tension", Tension);
    public string TensionGuideText => GetSliderBandGuide("Tension");
    public string AweHelper => GetSliderHelper("Awe", Awe);
    public string AweValueText => GetSliderBandLabel("Awe", Awe);
    public string AweGuideText => GetSliderBandGuide("Awe");
    public string SaturationHelper => GetSliderHelper("Saturation", Saturation);
    public string SaturationValueText => GetSliderBandLabel("Saturation", Saturation);
    public string SaturationGuideText => GetSliderBandGuide("Saturation");
    public string ContrastHelper => GetSliderHelper("Contrast", Contrast);
    public string ContrastValueText => GetSliderBandLabel("Contrast", Contrast);
    public string ContrastGuideText => GetSliderBandGuide("Contrast");
    public bool ShowArtistBlendSummary => !IsLockedLaneActive && (HasActiveArtist(ArtistInfluencePrimary, InfluenceStrengthPrimary) || HasActiveArtist(ArtistInfluenceSecondary, InfluenceStrengthSecondary));
    public string ArtistBlendSummaryTitle => BuildArtistBlendSummaryTitle();
    public string ArtistBlendSummaryBody => BuildArtistBlendSummaryBody();
    public string CompositionDriver => BuildContributionValue(ContributionArea.Composition);
    public string PaletteDriver => BuildContributionValue(ContributionArea.Palette);
    public string SurfaceDriver => BuildContributionValue(ContributionArea.Surface);
    public string MoodDriver => BuildContributionValue(ContributionArea.Mood);
    public string PromptPreview { get => _promptPreview; private set => SetProperty(ref _promptPreview, value); }
    public string NegativePromptPreview { get => _negativePromptPreview; private set => SetProperty(ref _negativePromptPreview, value); }
    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }

    public void RefreshLicenseState()
    {
        _licenseService.Refresh();
        UpdateLockedLaneAccessState();
        OnPropertyChanged(nameof(IsUnlocked));
        OnPropertyChanged(nameof(IsDemoMode));
        OnPropertyChanged(nameof(IsDemoExpired));
        OnPropertyChanged(nameof(IsLockedLaneActive));
        OnPropertyChanged(nameof(IsVintageBendLocked));
        OnPropertyChanged(nameof(ShowDemoModeBanner));
        OnPropertyChanged(nameof(ShowInteractivePromptPreview));
        OnPropertyChanged(nameof(ShowDemoPromptPreview));
        OnPropertyChanged(nameof(ShowAuthoringWorkspace));
        OnPropertyChanged(nameof(ShowDemoExpiredLockScreen));
        OnPropertyChanged(nameof(ShowLockedLaneAuthoringSections));
        OnPropertyChanged(nameof(ShowVintageBendAuthoringSections));
        OnPropertyChanged(nameof(ShowLockedLanePane));
        OnPropertyChanged(nameof(ShowVintageBendLockedPane));
        OnPropertyChanged(nameof(VersionButtonText));
        OnPropertyChanged(nameof(DemoModeHeadline));
        OnPropertyChanged(nameof(CopyPromptRemainingText));
        OnPropertyChanged(nameof(DemoModeBody));
        OnPropertyChanged(nameof(ShowNegativePrompt));
        OnPropertyChanged(nameof(ShowCompactCinematicPanel));
        OnPropertyChanged(nameof(ShowCompactWatercolorPanel));
        OnPropertyChanged(nameof(ShowCompactChildrensBookPanel));
        OnPropertyChanged(nameof(ShowCompactThreeDRenderPanel));
        OnPropertyChanged(nameof(ShowCompactTattooArtPanel));
        OnPropertyChanged(nameof(ShowActiveStandardLanePanel));
        OnPropertyChanged(nameof(ShowResolvedStandardLanePanel));
        RaiseCopyCommandCanExecuteChanged();
        RegeneratePrompt();
    }

    private bool SetAndRefresh<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        var changed = SetProperty(ref field, value);
        if (changed && !_isApplyingConfiguration)
        {
            if (!_isApplyingExperimentalMacroState)
            {
                SyncExperimentalMacrosFromRaw();
            }

            RegeneratePrompt();

            if (!string.IsNullOrWhiteSpace(propertyName) &&
                (propertyName.StartsWith("Exclude", StringComparison.Ordinal) || IsPromptSliderProperty(propertyName)))
            {
                UiEventLog.Write(
                    $"prompt-preview property='{propertyName}' value='{value}' intent='{IntentMode}' prompt='{FormatPromptPreviewForLog(PromptPreview)}'");
            }
        }

        return changed;
    }

    private static bool IsPromptSliderProperty(string propertyName)
    {
        return propertyName is nameof(Temperature)
            or nameof(LightingIntensity)
            or nameof(Stylization)
            or nameof(Realism)
            or nameof(TextureDepth)
            or nameof(NarrativeDensity)
            or nameof(Symbolism)
            or nameof(AtmosphericDepth)
            or nameof(SurfaceAge)
            or nameof(Chaos)
            or nameof(Framing)
            or nameof(CameraDistance)
            or nameof(CameraAngle)
            or nameof(BackgroundComplexity)
            or nameof(MotionEnergy)
            or nameof(FocusDepth)
            or nameof(ImageCleanliness)
            or nameof(DetailDensity)
            or nameof(Whimsy)
            or nameof(Tension)
            or nameof(Awe)
            or nameof(Saturation)
            or nameof(Contrast);
    }

    private static string FormatPromptPreviewForLog(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var singleLine = value
            .Replace("\r", " ", StringComparison.Ordinal)
            .Replace("\n", " ", StringComparison.Ordinal)
            .Replace("'", "''", StringComparison.Ordinal)
            .Trim();

        return singleLine.Length <= 320
            ? singleLine
            : $"{singleLine[..320]}...";
    }

    private void LogOverrideSliderDiagnostics(string message)
    {
        if (!ShouldLogOverrideSliderDiagnostics(IntentMode))
        {
            return;
        }

        UiEventLog.Write($"override-sliders {message}");
    }

    private static bool ShouldLogOverrideSliderDiagnostics(string? intentMode)
    {
        return IntentModeCatalog.IsFantasyIllustration(intentMode)
            || IntentModeCatalog.IsEditorialIllustration(intentMode)
            || IntentModeCatalog.IsGraphicDesign(intentMode)
            || IntentModeCatalog.IsTattooArt(intentMode)
            || IntentModeCatalog.IsComicBook(intentMode)
            || IntentModeCatalog.IsWatercolor(intentMode);
    }

    private string FormatCurrentOverrideSliderState()
    {
        return $"stylization={Stylization} narrativeDensity={NarrativeDensity} contrast={Contrast}";
    }

    private static string FormatLanePromptDefaults(LanePromptDefaults defaults)
    {
        return $"stylization={FormatNullableInt(defaults.Stylization)} narrativeDensity={FormatNullableInt(defaults.NarrativeDensity)} contrast={FormatNullableInt(defaults.Contrast)}";
    }

    private static string FormatNullableInt(int? value)
    {
        return value.HasValue ? value.Value.ToString() : "null";
    }

    private void RefreshSpeechBubbleOptionState()
    {
        OnPropertyChanged(nameof(ShowBlankSpeechBubbleOptions));
        OnPropertyChanged(nameof(ShowSpeechBubbleWarning));
        OnPropertyChanged(nameof(SpeechBubbleHelperText));
        OnPropertyChanged(nameof(SpeechBubbleWarningText));
    }

    private void SyncCompressionStateForIntentChange(string intentMode)
    {
        var supportsCompression = !string.IsNullOrWhiteSpace(intentMode) && IntentModeCatalog.TryGet(intentMode, out _);
        ApplyCompressionState(supportsCompression, false, false, refresh: false);
    }

    public void ResetCompressionStateForIntentPicker()
    {
        var supportsCompression = CanUseCompressionControls;
        ApplyCompressionState(supportsCompression, false, false);
    }

    private void ResetPromptExclusionFlags()
    {
        UiEventLog.Write(
            $"reset-prompt-exclusions narrative={ExcludeNarrativeDensityFromPrompt} symbolism={ExcludeSymbolismFromPrompt} atmospheric={ExcludeAtmosphericDepthFromPrompt} chaos={ExcludeChaosFromPrompt} framing={ExcludeFramingFromPrompt} cameraDistance={ExcludeCameraDistanceFromPrompt} cameraAngle={ExcludeCameraAngleFromPrompt} background={ExcludeBackgroundComplexityFromPrompt} motion={ExcludeMotionEnergyFromPrompt} whimsy={ExcludeWhimsyFromPrompt} tension={ExcludeTensionFromPrompt} awe={ExcludeAweFromPrompt} temperature={ExcludeTemperatureFromPrompt} lightingIntensity={ExcludeLightingIntensityFromPrompt} stylization={ExcludeStylizationFromPrompt} realism={ExcludeRealismFromPrompt} textureDepth={ExcludeTextureDepthFromPrompt} surfaceAge={ExcludeSurfaceAgeFromPrompt} focusDepth={ExcludeFocusDepthFromPrompt} imageCleanliness={ExcludeImageCleanlinessFromPrompt} detailDensity={ExcludeDetailDensityFromPrompt} saturation={ExcludeSaturationFromPrompt} contrast={ExcludeContrastFromPrompt}");
        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            ExcludeNarrativeDensityFromPrompt = false;
            ExcludeSymbolismFromPrompt = false;
            ExcludeAtmosphericDepthFromPrompt = false;
            ExcludeChaosFromPrompt = false;
            ExcludeFramingFromPrompt = false;
            ExcludeCameraDistanceFromPrompt = false;
            ExcludeCameraAngleFromPrompt = false;
            ExcludeBackgroundComplexityFromPrompt = false;
            ExcludeMotionEnergyFromPrompt = false;
            ExcludeWhimsyFromPrompt = false;
            ExcludeTensionFromPrompt = false;
            ExcludeAweFromPrompt = false;
            ExcludeTemperatureFromPrompt = false;
            ExcludeLightingIntensityFromPrompt = false;
            ExcludeStylizationFromPrompt = false;
            ExcludeRealismFromPrompt = false;
            ExcludeTextureDepthFromPrompt = false;
            ExcludeSurfaceAgeFromPrompt = false;
            ExcludeFocusDepthFromPrompt = false;
            ExcludeImageCleanlinessFromPrompt = false;
            ExcludeDetailDensityFromPrompt = false;
            ExcludeSaturationFromPrompt = false;
            ExcludeContrastFromPrompt = false;
            _fantasyIllustrationAppliedSliderSuppressions.Clear();
            _editorialIllustrationAppliedSliderSuppressions.Clear();
            _graphicDesignAppliedSliderSuppressions.Clear();
            _infographicAppliedSliderSuppressions.Clear();
            _dataVizAppliedSliderSuppressions.Clear();
            _conceptArtAppliedSliderSuppressions.Clear();
            SyncConceptArtSliderSuppressions();
            SyncFantasyIllustrationSliderSuppressions();
            SyncEditorialIllustrationSliderSuppressions();
            SyncGraphicDesignSliderSuppressions();
            SyncInfographicDataVisualizationSliderSuppressions();
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }

        UiEventLog.Write(
            $"reset-prompt-exclusions-complete narrative={ExcludeNarrativeDensityFromPrompt} symbolism={ExcludeSymbolismFromPrompt} atmospheric={ExcludeAtmosphericDepthFromPrompt} chaos={ExcludeChaosFromPrompt} framing={ExcludeFramingFromPrompt} cameraDistance={ExcludeCameraDistanceFromPrompt} cameraAngle={ExcludeCameraAngleFromPrompt} background={ExcludeBackgroundComplexityFromPrompt} motion={ExcludeMotionEnergyFromPrompt} whimsy={ExcludeWhimsyFromPrompt} tension={ExcludeTensionFromPrompt} awe={ExcludeAweFromPrompt} temperature={ExcludeTemperatureFromPrompt} lightingIntensity={ExcludeLightingIntensityFromPrompt} stylization={ExcludeStylizationFromPrompt} realism={ExcludeRealismFromPrompt} textureDepth={ExcludeTextureDepthFromPrompt} surfaceAge={ExcludeSurfaceAgeFromPrompt} focusDepth={ExcludeFocusDepthFromPrompt} imageCleanliness={ExcludeImageCleanlinessFromPrompt} detailDensity={ExcludeDetailDensityFromPrompt} saturation={ExcludeSaturationFromPrompt} contrast={ExcludeContrastFromPrompt}");
    }

    private void ApplyPhotographyIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.PhotographyName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            PhotographyType = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.PhotographyName, SharedLaneKeys.Photography.TypeSelector, SharedLaneKeys.Photography.DefaultTypeLabel);
            PhotographyEra = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.PhotographyName, SharedLaneKeys.Photography.EraSelector, SharedLaneKeys.Photography.DefaultEraLabel);
            PhotographyCandidCapture = false;
            PhotographyPosedStagedCapture = false;
            PhotographyAvailableLight = false;
            PhotographyOnCameraFlash = false;
            PhotographyEditorialPolish = false;
            PhotographyRawDocumentaryTexture = false;
            PhotographyEnvironmentalPortraitContext = false;
            PhotographyFilmAnalogCharacter = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyVintageBendIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.VintageBendName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            VintageBendEasternBlocGdr = false;
            VintageBendThrillerUndertone = false;
            VintageBendInstitutionalAusterity = false;
            VintageBendSurveillanceStateAtmosphere = false;
            VintageBendPeriodArtifacts = false;
            VintageBendUrbanCivilian = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyProductPhotographyIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.ProductPhotographyName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            ProductPhotographyShotType = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.ProductPhotographyName, SharedLaneKeys.ProductPhotography.ShotTypeSelector, SharedLaneKeys.ProductPhotography.DefaultShotTypeLabel);
            ProductPhotographyWithPackaging = false;
            ProductPhotographyPedestalDisplay = false;
            ProductPhotographyReflectiveSurface = false;
            ProductPhotographyFloatingPresentation = false;
            ProductPhotographyScaleCueHand = false;
            ProductPhotographyBrandProps = false;
            ProductPhotographyGroupedVariants = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyFoodPhotographyIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.FoodPhotographyName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            FoodPhotographyShotMode = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.FoodPhotographyName, SharedLaneKeys.FoodPhotography.ShotModeSelector, SharedLaneKeys.FoodPhotography.DefaultShotModeLabel);
            FoodPhotographyVisibleSteam = false;
            FoodPhotographyGarnishEmphasis = false;
            FoodPhotographyUtensilContext = false;
            FoodPhotographyHandServiceCue = false;
            FoodPhotographyIngredientScatter = false;
            FoodPhotographyCondensationEmphasis = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyLifestyleAdvertisingPhotographyIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.LifestyleAdvertisingPhotographyName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            LifestyleAdvertisingShotMode = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.LifestyleAdvertisingPhotographyName, SharedLaneKeys.LifestyleAdvertising.ShotModeSelector, SharedLaneKeys.LifestyleAdvertising.DefaultShotModeLabel);
            LifestyleAdvertisingNaturalInteraction = false;
            LifestyleAdvertisingProductInUse = false;
            LifestyleAdvertisingBrandColorAccent = false;
            LifestyleAdvertisingPropContext = false;
            LifestyleAdvertisingSunlitOptimism = false;
            LifestyleAdvertisingMotionCandidness = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyCinematicIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.CinematicName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            CinematicSubtype = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.CinematicName, SharedLaneKeys.StyleSelector, SharedLaneKeys.Cinematic.DefaultSubtypeLabel);
            CinematicLetterboxedFraming = false;
            CinematicShallowDepthOfField = false;
            CinematicPracticalLighting = false;
            CinematicAtmosphericHaze = false;
            CinematicFilmGrain = false;
            CinematicAnamorphicFlares = false;
            CinematicDramaticBacklight = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyThreeDRenderIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.ThreeDRenderName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            ThreeDRenderSubtype = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.ThreeDRenderName, SharedLaneKeys.StyleSelector, SharedLaneKeys.ThreeDRender.DefaultSubtypeLabel);
            ThreeDRenderGlobalIllumination = false;
            ThreeDRenderVolumetricLighting = false;
            ThreeDRenderRayTracedReflections = false;
            ThreeDRenderDepthOfField = false;
            ThreeDRenderSubsurfaceScattering = false;
            ThreeDRenderHardSurfacePrecision = false;
            ThreeDRenderStudioBackdrop = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyConceptArtIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.ConceptArtName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            ConceptArtSubtype = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.ConceptArtName, SharedLaneKeys.StyleSelector, SharedLaneKeys.ConceptArt.DefaultSubtypeLabel);
            ConceptArtDesignCallouts = false;
            ConceptArtTurnaroundReadability = false;
            ConceptArtMaterialBreakdown = false;
            ConceptArtScaleReference = false;
            ConceptArtWorldbuildingAccents = false;
            ConceptArtProductionNotesFeel = false;
            ConceptArtSilhouetteClarity = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyPixelArtIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.PixelArtName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            PixelArtSubtype = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.PixelArtName, SharedLaneKeys.StyleSelector, SharedLaneKeys.PixelArt.DefaultSubtypeLabel);
            PixelArtLimitedPalette = false;
            PixelArtDithering = false;
            PixelArtTileableDesign = false;
            PixelArtSpriteSheetReadability = false;
            PixelArtCleanOutline = false;
            PixelArtSubpixelShading = false;
            PixelArtHudUiFraming = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyWatercolorIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.WatercolorName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            WatercolorStyle = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.WatercolorName, SharedLaneKeys.StyleSelector, SharedLaneKeys.Watercolor.DefaultSubtypeLabel);
            WatercolorTransparentWashes = false;
            WatercolorSoftBleeds = false;
            WatercolorPaperTexture = false;
            WatercolorInkAndWatercolor = false;
            WatercolorAtmosphericWash = false;
            WatercolorGouacheAccents = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyChildrensBookIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.ChildrensBookName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            ChildrensBookStyle = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.ChildrensBookName, SharedLaneKeys.StyleSelector, SharedLaneKeys.ChildrensBook.DefaultSubtypeLabel);
            ChildrensBookSoftColorPalette = false;
            ChildrensBookTexturedPaper = false;
            ChildrensBookInkLinework = false;
            ChildrensBookExpressiveCharacters = false;
            ChildrensBookMinimalBackground = false;
            ChildrensBookDecorativeDetails = false;
            ChildrensBookGentleLighting = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyArchitectureArchvizIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.ArchitectureArchvizName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            ArchitectureArchvizViewMode = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.ArchitectureArchvizName, SharedLaneKeys.ArchitectureArchviz.ViewModeSelector, SharedLaneKeys.ArchitectureArchviz.DefaultViewModeLabel);
            ArchitectureArchvizHumanScaleCues = false;
            ArchitectureArchvizLandscapeEmphasis = false;
            ArchitectureArchvizFurnishingEmphasis = false;
            ArchitectureArchvizWarmInteriorGlow = false;
            ArchitectureArchvizReflectiveSurfaceAccents = false;
            ArchitectureArchvizAmenityFocus = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyFantasyIllustrationIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.FantasyIllustrationName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            FantasyIllustrationRegister = LaneRegistry.GetDefaultSubtypeValue(
                IntentModeCatalog.FantasyIllustrationName,
                SharedLaneKeys.FantasyIllustration.RegisterSelector,
                SharedLaneKeys.FantasyIllustration.DefaultRegisterLabel);
            FantasyIllustrationCharacterSketch = false;
            FantasyIllustrationCharacterCentric = false;
            FantasyIllustrationEnvironmentConcept = false;
            FantasyIllustrationKeyArt = false;
            FantasyIllustrationCleanBackground = false;
            FantasyIllustrationSilhouetteReadability = false;
            FantasyIllustrationPhotorealistic = false;
            FantasyIllustrationCartoonArt = false;
            FantasyIllustrationPropArtifactFocus = false;
            FantasyIllustrationCreatureDesign = false;
            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyEditorialIllustrationIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.EditorialIllustrationName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            EditorialIllustrationBlackAndWhiteMonochrome = false;
            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyTattooArtIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.TattooArtName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            GraphicDesignType = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.GraphicDesignName, SharedLaneKeys.GraphicDesign.TypeSelector, SharedLaneKeys.GraphicDesign.DefaultTypeLabel);
            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyGraphicDesignIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.GraphicDesignName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyInfographicDataVisualizationIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.InfographicDataVisualizationName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            InfographicDataVisualizationSubdomain = LaneRegistry.GetDefaultSubtypeValue(
                IntentModeCatalog.InfographicDataVisualizationName,
                SharedLaneKeys.InfographicDataVisualization.SubdomainSelector,
                SharedLaneKeys.InfographicDataVisualization.DefaultSubdomainLabel);
            InfographicModeLeanExplainer = true;
            InfographicModePublicPoster = false;
            InfographicModeReferenceSheet = false;
            DataVizModeChartPurity = true;
            DataVizModeDashboard = false;
            DataVizModeReportGraphic = false;
            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyAnimeIntentDefaults()
    {
        var lane = LaneRegistry.GetByIntentName(IntentModeCatalog.AnimeName);
        if (lane is null)
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            AnimeStyle = LaneRegistry.GetDefaultSubtypeLabel(IntentModeCatalog.AnimeName, SharedLaneKeys.Anime.StyleSelector, "General Anime");
            AnimeEra = LaneRegistry.GetDefaultSubtypeLabel(IntentModeCatalog.AnimeName, SharedLaneKeys.Anime.EraSelector, "Default / Modern");
            AnimeCelShading = false;
            AnimeCleanLineArt = false;
            AnimeExpressiveEyes = false;
            AnimeDynamicAction = false;
            AnimeCinematicLighting = false;
            AnimeStylizedHair = false;
            AnimeAtmosphericEffects = false;
            AnimeCharacterLedStaging = false;
            AnimeClearSilhouetteRead = false;
            AnimeEmotionFirstExpression = false;

            ApplyLanePromptDefaults(lane.Defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void SetCompressionTier1(bool value)
    {
        if (!CanUseCompressionControls)
        {
            return;
        }

        if (_compressPromptSemantics == value)
        {
            return;
        }

        if (!value)
        {
            ApplyCompressionState(false, false, false);
            return;
        }

        ApplyCompressionState(true, false, false);
    }

    private void SetCompressionTier2(bool value)
    {
        if (!CanUseCompressionControls || !_compressPromptSemantics)
        {
            return;
        }

        if (_reduceRepeatedLaneWords == value)
        {
            return;
        }

        if (!value)
        {
            ApplyCompressionState(_compressPromptSemantics, false, false);
            return;
        }

        ApplyCompressionState(true, true, false);
    }

    private void SetCompressionTier3(bool value)
    {
        if (!CanUseCompressionControls || !_compressPromptSemantics || !_reduceRepeatedLaneWords)
        {
            return;
        }

        if (_trimRepeatedLongWords == value)
        {
            return;
        }

        ApplyCompressionState(_compressPromptSemantics, _reduceRepeatedLaneWords, value);
    }

    private void ApplyCompressionConfiguration(PromptConfiguration configuration)
    {
        var supportsCompression = IntentModeCatalog.TryGet(configuration.IntentMode, out _);
        var compress = supportsCompression && configuration.CompressPromptSemantics;
        var reduceRepeatedLaneWords = supportsCompression && compress && configuration.ReduceRepeatedLaneWords;
        var trimRepeatedLongWords = supportsCompression && compress && reduceRepeatedLaneWords && configuration.TrimRepeatedLongWords;

        ApplyCompressionState(compress, reduceRepeatedLaneWords, trimRepeatedLongWords, refresh: false);
    }

    private void ApplyCompressionState(bool compressPromptSemantics, bool reduceRepeatedLaneWords, bool trimRepeatedLongWords, bool refresh = true)
    {
        var changed = false;
        changed |= SetProperty(ref _compressPromptSemantics, compressPromptSemantics);
        changed |= SetProperty(ref _reduceRepeatedLaneWords, reduceRepeatedLaneWords);
        changed |= SetProperty(ref _trimRepeatedLongWords, trimRepeatedLongWords);

        OnPropertyChanged(nameof(ShowCompressionTierTwo));
        OnPropertyChanged(nameof(ShowCompressionTierThree));

        if (changed && refresh && !_isApplyingConfiguration)
        {
            RegeneratePrompt();
        }
    }

    private bool SetArtistAndRefresh<T>(ref T field, T value)
    {
        var changed = SetProperty(ref field, value);
        if (changed && !_isApplyingConfiguration)
        {
            ApplyArtistNegativeConstraintDefaults();
            RegeneratePrompt();
        }

        return changed;
    }

    private bool SetStandardLaneSelectorAndRefresh(ref string field, string value, string laneId, string selectorKey)
    {
        var changed = SetAndRefresh(ref field, value);
        if (changed)
        {
            _ordinaryLaneStates.GetOrAddLane(laneId).SetSelector(selectorKey, value);
            LogLaneDebugSelectorChange(laneId, selectorKey, value);
            ApplySelectorDefaultNudges(laneId, selectorKey, value);
        }

        return changed;
    }

    private bool SetSelectorAndApplyDefaultNudges(ref string field, string value, string intentName, string selectorKey)
    {
        var changed = SetAndRefresh(ref field, value);
        if (changed)
        {
            ApplySelectorDefaultNudges(intentName, selectorKey, value);
        }

        return changed;
    }

    private bool SetStandardLaneModifierAndRefresh(ref bool field, bool value, string laneId, string modifierKey)
    {
        var changed = SetAndRefresh(ref field, value);
        if (changed)
        {
            _ordinaryLaneStates.GetOrAddLane(laneId).SetModifier(modifierKey, value);
            LogLaneDebugModifierChange(laneId, modifierKey, value);
        }

        return changed;
    }

    private bool SetInfographicMode(ref bool field, bool value, string modeKey, [CallerMemberName] string? propertyName = null)
    {
        if (!value && GetActiveInfographicModeCount() <= 1 && IsInfographicSubdomainActive)
        {
            return false;
        }

        if (!SetProperty(ref field, value, propertyName))
        {
            return false;
        }

        if (value)
        {
            if (!string.Equals(modeKey, SharedLaneKeys.InfographicDataVisualization.LeanExplainer, StringComparison.Ordinal))
            {
                SetProperty(ref _infographicModeLeanExplainer, false, nameof(InfographicModeLeanExplainer));
            }

            if (!string.Equals(modeKey, SharedLaneKeys.InfographicDataVisualization.PublicPoster, StringComparison.Ordinal))
            {
                SetProperty(ref _infographicModePublicPoster, false, nameof(InfographicModePublicPoster));
            }

            if (!string.Equals(modeKey, SharedLaneKeys.InfographicDataVisualization.ReferenceSheet, StringComparison.Ordinal))
            {
                SetProperty(ref _infographicModeReferenceSheet, false, nameof(InfographicModeReferenceSheet));
            }
        }
        else
        {
            EnsureInfographicModeDefault();
        }

        if (!_isApplyingConfiguration)
        {
            SyncInfographicDataVisualizationSliderSuppressions();
            RegeneratePrompt();
        }
        return true;
    }

    private bool SetDataVizMode(ref bool field, bool value, string modeKey, [CallerMemberName] string? propertyName = null)
    {
        if (!value && GetActiveDataVizModeCount() <= 1 && IsDataVizSubdomainActive)
        {
            return false;
        }

        if (!SetProperty(ref field, value, propertyName))
        {
            return false;
        }

        if (value)
        {
            if (!string.Equals(modeKey, SharedLaneKeys.InfographicDataVisualization.ChartPurity, StringComparison.Ordinal))
            {
                SetProperty(ref _dataVizModeChartPurity, false, nameof(DataVizModeChartPurity));
            }

            if (!string.Equals(modeKey, SharedLaneKeys.InfographicDataVisualization.Dashboard, StringComparison.Ordinal))
            {
                SetProperty(ref _dataVizModeDashboard, false, nameof(DataVizModeDashboard));
            }

            if (!string.Equals(modeKey, SharedLaneKeys.InfographicDataVisualization.ReportGraphic, StringComparison.Ordinal))
            {
                SetProperty(ref _dataVizModeReportGraphic, false, nameof(DataVizModeReportGraphic));
            }
        }
        else
        {
            EnsureDataVizModeDefault();
        }

        if (!_isApplyingConfiguration)
        {
            SyncInfographicDataVisualizationSliderSuppressions();
            RegeneratePrompt();
        }
        return true;
    }

    private int GetActiveInfographicModeCount()
    {
        var count = 0;
        if (InfographicModeLeanExplainer) count++;
        if (InfographicModePublicPoster) count++;
        if (InfographicModeReferenceSheet) count++;
        return count;
    }

    private int GetActiveDataVizModeCount()
    {
        var count = 0;
        if (DataVizModeChartPurity) count++;
        if (DataVizModeDashboard) count++;
        if (DataVizModeReportGraphic) count++;
        return count;
    }

    private void EnsureInfographicModeDefault()
    {
        if (InfographicModeLeanExplainer)
        {
            SetProperty(ref _infographicModePublicPoster, false, nameof(InfographicModePublicPoster));
            SetProperty(ref _infographicModeReferenceSheet, false, nameof(InfographicModeReferenceSheet));
            return;
        }

        if (InfographicModePublicPoster)
        {
            SetProperty(ref _infographicModeReferenceSheet, false, nameof(InfographicModeReferenceSheet));
            return;
        }

        if (InfographicModeReferenceSheet)
        {
            return;
        }

        SetProperty(ref _infographicModeLeanExplainer, true, nameof(InfographicModeLeanExplainer));
        SetProperty(ref _infographicModePublicPoster, false, nameof(InfographicModePublicPoster));
        SetProperty(ref _infographicModeReferenceSheet, false, nameof(InfographicModeReferenceSheet));
    }

    private void EnsureDataVizModeDefault()
    {
        if (DataVizModeChartPurity)
        {
            SetProperty(ref _dataVizModeDashboard, false, nameof(DataVizModeDashboard));
            SetProperty(ref _dataVizModeReportGraphic, false, nameof(DataVizModeReportGraphic));
            return;
        }

        if (DataVizModeDashboard)
        {
            SetProperty(ref _dataVizModeReportGraphic, false, nameof(DataVizModeReportGraphic));
            return;
        }

        if (DataVizModeReportGraphic)
        {
            return;
        }

        SetProperty(ref _dataVizModeChartPurity, true, nameof(DataVizModeChartPurity));
        SetProperty(ref _dataVizModeDashboard, false, nameof(DataVizModeDashboard));
        SetProperty(ref _dataVizModeReportGraphic, false, nameof(DataVizModeReportGraphic));
    }

    private void EnsureInfographicDataVisualizationModeDefaults()
    {
        if (string.Equals(InfographicDataVisualizationSubdomain, SharedLaneKeys.InfographicDataVisualization.DataViz, StringComparison.Ordinal))
        {
            EnsureDataVizModeDefault();
            return;
        }

        EnsureInfographicModeDefault();
    }

    private bool SetFantasyIllustrationModifierAndRefresh(ref bool field, bool value, string modifierKey, [CallerMemberName] string? propertyName = null)
    {
        if (string.Equals(modifierKey, SharedLaneKeys.FantasyIllustration.CharacterSketch, StringComparison.Ordinal))
        {
            UiEventLog.Write(
                $"fantasy-debug character-sketch-request value={value} beforeField={field} narrativeExclude={ExcludeNarrativeDensityFromPrompt} register='{FantasyIllustrationRegister}' prompt='{FormatPromptPreviewForLog(PromptPreview)}'");
        }

        var changed = SetProperty(ref field, value, propertyName);
        if (!changed)
        {
            if (string.Equals(modifierKey, SharedLaneKeys.FantasyIllustration.CharacterSketch, StringComparison.Ordinal))
            {
                UiEventLog.Write(
                    $"fantasy-debug character-sketch-nochange value={value} narrativeExclude={ExcludeNarrativeDensityFromPrompt} register='{FantasyIllustrationRegister}'");
            }

            return false;
        }

        _ordinaryLaneStates.GetOrAddLane(SharedLaneKeys.FantasyIllustration.LaneId).SetModifier(modifierKey, value);
        if (value)
        {
            ApplyFantasyIllustrationMutualExclusions(modifierKey);
        }

        if (!_isApplyingConfiguration)
        {
            SyncFantasyIllustrationSliderSuppressions();
        }

        if (string.Equals(modifierKey, SharedLaneKeys.FantasyIllustration.CharacterSketch, StringComparison.Ordinal))
        {
            UiEventLog.Write(
                $"fantasy-debug character-sketch-applied value={value} narrativeExclude={ExcludeNarrativeDensityFromPrompt} atmosphericExclude={ExcludeAtmosphericDepthFromPrompt} backgroundExclude={ExcludeBackgroundComplexityFromPrompt} appliedByFantasy={_fantasyIllustrationAppliedSliderSuppressions.Contains(SliderLanguageCatalog.NarrativeDensity)} register='{FantasyIllustrationRegister}'");
        }

        if (!_isApplyingConfiguration)
        {
            if (!_isApplyingExperimentalMacroState)
            {
                SyncExperimentalMacrosFromRaw();
            }

            RegeneratePrompt();
        }

        return true;
    }

    private bool SetEditorialIllustrationModifierAndRefresh(ref bool field, bool value, string modifierKey, [CallerMemberName] string? propertyName = null)
    {
        var changed = SetProperty(ref field, value, propertyName);
        if (!changed)
        {
            return false;
        }

        _ordinaryLaneStates.GetOrAddLane(SharedLaneKeys.EditorialIllustration.LaneId).SetModifier(modifierKey, value);

        if (!_isApplyingConfiguration)
        {
            SyncEditorialIllustrationSliderSuppressions();
            if (!_isApplyingExperimentalMacroState)
            {
                SyncExperimentalMacrosFromRaw();
            }

            RegeneratePrompt();
        }

        return true;
    }

    private bool SetGraphicDesignModifierAndRefresh(ref bool field, bool value, string modifierKey, [CallerMemberName] string? propertyName = null)
    {
        var changed = SetProperty(ref field, value, propertyName);
        if (!changed)
        {
            return false;
        }

        _ordinaryLaneStates.GetOrAddLane(SharedLaneKeys.GraphicDesign.LaneId).SetModifier(modifierKey, value);

        if (!_isApplyingConfiguration)
        {
            SyncGraphicDesignSliderSuppressions();
            if (!_isApplyingExperimentalMacroState)
            {
                SyncExperimentalMacrosFromRaw();
            }

            RegeneratePrompt();
        }

        return true;
    }

    private void ApplyFantasyIllustrationMutualExclusions(string enabledModifierKey)
    {
        foreach (var modifierKey in FantasyIllustrationLaneState.GetMutuallyExclusiveModifierKeys(enabledModifierKey))
        {
            ClearFantasyIllustrationModifier(modifierKey);
        }
    }

    private void ClearFantasyIllustrationModifier(string modifierKey)
    {
        switch (modifierKey)
        {
            case SharedLaneKeys.FantasyIllustration.Photorealistic:
                ClearFantasyIllustrationModifier(ref _fantasyIllustrationState.Photorealistic, modifierKey, nameof(FantasyIllustrationPhotorealistic));
                break;
            case SharedLaneKeys.FantasyIllustration.CartoonArt:
                ClearFantasyIllustrationModifier(ref _fantasyIllustrationState.CartoonArt, modifierKey, nameof(FantasyIllustrationCartoonArt));
                break;
            case SharedLaneKeys.FantasyIllustration.CharacterCentric:
                ClearFantasyIllustrationModifier(ref _fantasyIllustrationState.CharacterCentric, modifierKey, nameof(FantasyIllustrationCharacterCentric));
                break;
            case SharedLaneKeys.FantasyIllustration.EnvironmentConcept:
                ClearFantasyIllustrationModifier(ref _fantasyIllustrationState.EnvironmentConcept, modifierKey, nameof(FantasyIllustrationEnvironmentConcept));
                break;
            case SharedLaneKeys.FantasyIllustration.CharacterSketch:
                ClearFantasyIllustrationModifier(ref _fantasyIllustrationState.CharacterSketch, modifierKey, nameof(FantasyIllustrationCharacterSketch));
                break;
            case SharedLaneKeys.FantasyIllustration.PropArtifactFocus:
                ClearFantasyIllustrationModifier(ref _fantasyIllustrationState.PropArtifactFocus, modifierKey, nameof(FantasyIllustrationPropArtifactFocus));
                break;
            case SharedLaneKeys.FantasyIllustration.CreatureDesign:
                ClearFantasyIllustrationModifier(ref _fantasyIllustrationState.CreatureDesignEnabled, modifierKey, nameof(FantasyIllustrationCreatureDesign));
                break;
        }
    }

    private void ClearFantasyIllustrationModifier(ref bool field, string modifierKey, string propertyName)
    {
        if (!SetProperty(ref field, false, propertyName))
        {
            return;
        }

        _ordinaryLaneStates.GetOrAddLane(SharedLaneKeys.FantasyIllustration.LaneId).SetModifier(modifierKey, false);
    }

    private bool SetAnimeSelectorAndRefresh(ref string field, string value, string selectorKey)
    {
        var laneState = _ordinaryLaneStates.GetOrAddLane(SharedLaneKeys.Anime.LaneId);
        laneState.SetSelector(selectorKey, value);
        var changed = SetAndRefresh(ref field, value);
        if (changed)
        {
            LogLaneDebugSelectorChange(SharedLaneKeys.Anime.LaneId, selectorKey, value);
            ApplySelectorDefaultNudges(IntentModeCatalog.AnimeName, selectorKey, value);
        }

        return changed;
    }

    private void ApplySelectorDefaultNudges(string intentName, string selectorKey, string selectedValue)
    {
        if (_isApplyingConfiguration ||
            !LaneRegistry.TryGetSubtypeDefaultNudges(intentName, selectorKey, selectedValue, out var defaults))
        {
            return;
        }

        var wasApplyingConfiguration = _isApplyingConfiguration;
        _isApplyingConfiguration = true;
        try
        {
            ApplyLanePromptDefaults(defaults);
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }

        RegeneratePrompt();
    }


    private bool SetAnimeModifierAndRefresh(ref bool field, bool value, string modifierKey)
    {
        var laneState = _ordinaryLaneStates.GetOrAddLane(SharedLaneKeys.Anime.LaneId);
        laneState.SetModifier(modifierKey, value);
        var changed = SetAndRefresh(ref field, value);
        if (changed)
        {
            LogLaneDebugModifierChange(SharedLaneKeys.Anime.LaneId, modifierKey, value);
        }

        return changed;
    }

    private void LogLaneDebugSelectorChange(string laneId, string selectorKey, string value)
    {
        if (!ShouldLogLaneDebug(laneId))
        {
            return;
        }

        UiEventLog.Write(
            $"lane-selector lane='{laneId}' selector='{selectorKey}' value='{value}' intent='{IntentMode}' prompt='{FormatPromptPreviewForLog(PromptPreview)}'");
    }

    private void LogLaneDebugModifierChange(string laneId, string modifierKey, bool value)
    {
        if (!ShouldLogLaneDebug(laneId))
        {
            return;
        }

        UiEventLog.Write(
            $"lane-modifier lane='{laneId}' modifier='{modifierKey}' value={value} intent='{IntentMode}' prompt='{FormatPromptPreviewForLog(PromptPreview)}'");
    }

    private static bool ShouldLogLaneDebug(string laneId)
    {
        return string.Equals(laneId, SharedLaneKeys.Anime.LaneId, StringComparison.Ordinal)
            || string.Equals(laneId, SharedLaneKeys.PixelArt.LaneId, StringComparison.Ordinal);
    }

    private void RegeneratePrompt()
    {
        if (IsDemoExpired)
        {
            ApplyBlockedPromptPreviewState();
            return;
        }

        if (IsLockedLaneActive)
        {
            ApplyBlockedPromptPreviewState();
            return;
        }

        var configuration = CaptureConfiguration();
        var result = _promptBuilderService.Build(configuration);
        PromptPreview = ApplySemanticPairCollapse(result.PositivePrompt, configuration);
        NegativePromptPreview = result.NegativePrompt;
        LogOverrideSliderDiagnostics(
            $"regenerate-final intent='{IntentMode}' override={OverrideDefaultSliderPositions} {FormatCurrentOverrideSliderState()} prompt='{FormatPromptPreviewForLog(PromptPreview)}'");
        if (IsFantasyIllustrationIntent || FantasyIllustrationCharacterSketch)
        {
            UiEventLog.Write(
                $"fantasy-debug regenerate-final intent='{IntentMode}' register='{FantasyIllustrationRegister}' characterSketch={FantasyIllustrationCharacterSketch} narrativeValue={NarrativeDensity} narrativeExclude={ExcludeNarrativeDensityFromPrompt} prompt='{FormatPromptPreviewForLog(PromptPreview)}'");
        }

        SyncStandardLanePanels();
        RaiseArtistBlendSummaryChanged();
        RaiseArtistPhraseEditorAvailabilityChanged();
        RaiseSliderHelperChanged();
        RaiseExperimentalMacroChanged();
        RaiseCopyCommandCanExecuteChanged();
    }

    private void ApplyBlockedPromptPreviewState()
    {
        PromptPreview = string.Empty;
        NegativePromptPreview = string.Empty;
        SyncStandardLanePanels();
        RaiseArtistBlendSummaryChanged();
        RaiseCopyCommandCanExecuteChanged();
    }

    private void SwapArtistInfluences()
    {
        if (!CanSwapArtistInfluences)
        {
            return;
        }

        var originalPrimary = ArtistInfluencePrimary;
        var originalSecondary = ArtistInfluenceSecondary;

        _suspendArtistOverrideReset = true;
        try
        {
            _artistInfluencePrimary = originalSecondary;
            _artistInfluenceSecondary = originalPrimary;
        }
        finally
        {
            _suspendArtistOverrideReset = false;
        }

        OnPropertyChanged(nameof(ArtistInfluencePrimary));
        OnPropertyChanged(nameof(ArtistInfluenceSecondary));
        ApplyArtistNegativeConstraintDefaults();
        UpdateArtistPhraseOverrideTargetNames();

        if (IsArtistPhraseEditorOpen)
        {
            IsArtistPhraseEditorOpen = false;
            ArtistPhraseQuickInsertGroups = Array.Empty<ArtistPhraseQuickInsertGroup>();
        }

        RegeneratePrompt();
        RefreshArtistPairGuidance();
        StatusMessage = "Primary and secondary artists swapped.";
    }

    private void RefreshArtistPairGuidance()
    {
        var primary = CreateArtistState(ArtistInfluencePrimary, InfluenceStrengthPrimary);
        var secondary = CreateArtistState(ArtistInfluenceSecondary, InfluenceStrengthSecondary);
        if (primary is null || secondary is null)
        {
            _currentArtistPairLookup = null;
            ArtistPairGuidanceTooltip = null;
            return;
        }

        var lookup = _artistPairGuidanceService.ResolvePair(primary.Name, secondary.Name);
        _currentArtistPairLookup = lookup;
        ArtistPairGuidanceTooltip = lookup.PairFound
            ? ArtistPairTooltipFormatter.FormatTooltip(lookup, lookup.Guidance!)
            : BuildMissingArtistPairGuidanceTooltip(lookup);
    }

    private static string BuildMissingArtistPairGuidanceTooltip(ArtistPairLookupResult lookup)
    {
        if (!lookup.LeftArtistRecognized || !lookup.RightArtistRecognized)
        {
            var missingArtists = new List<string>();
            if (!lookup.LeftArtistRecognized)
            {
                missingArtists.Add(lookup.LeftResolvedName ?? lookup.LeftInput);
            }

            if (!lookup.RightArtistRecognized)
            {
                missingArtists.Add(lookup.RightResolvedName ?? lookup.RightInput);
            }

            return string.Join(Environment.NewLine, new[]
            {
                "Pair guidance not available yet for this selection.",
                $"Matrix coverage missing for: {string.Join(", ", missingArtists)}"
            });
        }

        var leftResolved = FormatResolvedArtist(lookup.LeftResolvedName, lookup.LeftResolvedKey, lookup.LeftInput);
        var rightResolved = FormatResolvedArtist(lookup.RightResolvedName, lookup.RightResolvedKey, lookup.RightInput);

        return string.Join(Environment.NewLine, new[]
        {
            "Pair guidance not available yet for this selection.",
            $"Resolved artists: {leftResolved} + {rightResolved}"
        });
    }

    private static string FormatResolvedArtist(string? resolvedName, string? resolvedKey, string input)
    {
        if (!string.IsNullOrWhiteSpace(resolvedName) && !string.IsNullOrWhiteSpace(resolvedKey))
        {
            return $"{resolvedName} [{resolvedKey}]";
        }

        return $"unresolved ({input})";
    }

    private static string BuildCategoryLine(ArtistPairGuidance guidance)
    {
        var categoryLine = $"Category: {guidance.Category}";
        if (!string.IsNullOrWhiteSpace(guidance.CategoryDefinition))
        {
            categoryLine = $"{categoryLine} — {guidance.CategoryDefinition}";
        }

        return categoryLine;
    }

    private static string GetEffectText(ArtistPairGuidance guidance)
    {
        if (!string.IsNullOrWhiteSpace(guidance.EffectOnPromptGeneration))
        {
            return guidance.EffectOnPromptGeneration;
        }

        if (!string.IsNullOrWhiteSpace(guidance.CategoryDefinition))
        {
            return guidance.CategoryDefinition;
        }

        return "Pair guidance is not available for this selection.";
    }

    private static string GetModelStruggleText(ArtistPairGuidance guidance)
    {
        if (!string.IsNullOrWhiteSpace(guidance.WhatModelsStruggleWith))
        {
            return guidance.WhatModelsStruggleWith;
        }

        return "Model may average or discard one artist without a strong dominance cue.";
    }

    private void ScheduleExperimentalMacroRefresh()
    {
        _experimentalMacroRefreshTimer.Stop();
        _experimentalMacroRefreshTimer.Start();
    }

    private PromptConfiguration CaptureConfiguration()
    {
        var configuration = CreateConfigurationFromCurrentState();
        UiEventLog.Write(
            $"capture-configuration intent='{configuration.IntentMode}' stylization={configuration.Stylization} realism={configuration.Realism} textureDepth={configuration.TextureDepth} narrativeDensity={configuration.NarrativeDensity} symbolism={configuration.Symbolism} surfaceAge={configuration.SurfaceAge} framing={configuration.Framing} cameraDistance={configuration.CameraDistance} cameraAngle={configuration.CameraAngle} background={configuration.BackgroundComplexity} motion={configuration.MotionEnergy} atmospheric={configuration.AtmosphericDepth} chaos={configuration.Chaos} focusDepth={configuration.FocusDepth} imageCleanliness={configuration.ImageCleanliness} detailDensity={configuration.DetailDensity} temperature={configuration.Temperature} lightingIntensity={configuration.LightingIntensity} saturation={configuration.Saturation} contrast={configuration.Contrast} excludeNarrative={configuration.ExcludeNarrativeDensityFromPrompt} excludeSymbolism={configuration.ExcludeSymbolismFromPrompt} excludeAtmospheric={configuration.ExcludeAtmosphericDepthFromPrompt} excludeChaos={configuration.ExcludeChaosFromPrompt} excludeFraming={configuration.ExcludeFramingFromPrompt} excludeCameraDistance={configuration.ExcludeCameraDistanceFromPrompt} excludeCameraAngle={configuration.ExcludeCameraAngleFromPrompt} excludeBackground={configuration.ExcludeBackgroundComplexityFromPrompt} excludeMotion={configuration.ExcludeMotionEnergyFromPrompt} excludeWhimsy={configuration.ExcludeWhimsyFromPrompt} excludeTension={configuration.ExcludeTensionFromPrompt} excludeAwe={configuration.ExcludeAweFromPrompt} excludeTemperature={configuration.ExcludeTemperatureFromPrompt} excludeLightingIntensity={configuration.ExcludeLightingIntensityFromPrompt} excludeStylization={configuration.ExcludeStylizationFromPrompt} excludeRealism={configuration.ExcludeRealismFromPrompt} excludeTextureDepth={configuration.ExcludeTextureDepthFromPrompt} excludeSurfaceAge={configuration.ExcludeSurfaceAgeFromPrompt} excludeFocusDepth={configuration.ExcludeFocusDepthFromPrompt} excludeImageCleanliness={configuration.ExcludeImageCleanlinessFromPrompt} excludeDetailDensity={configuration.ExcludeDetailDensityFromPrompt} excludeSaturation={configuration.ExcludeSaturationFromPrompt} excludeContrast={configuration.ExcludeContrastFromPrompt}");
        if (ShouldLogOverrideSliderDiagnostics(configuration.IntentMode))
        {
            UiEventLog.Write(
                $"override-sliders capture-configuration intent='{configuration.IntentMode}' override={OverrideDefaultSliderPositions} stylization={configuration.Stylization} narrativeDensity={configuration.NarrativeDensity} contrast={configuration.Contrast} excludeStylization={configuration.ExcludeStylizationFromPrompt} excludeNarrative={configuration.ExcludeNarrativeDensityFromPrompt} excludeContrast={configuration.ExcludeContrastFromPrompt}");
        }
        if (IntentModeCatalog.IsAnime(configuration.IntentMode))
        {
            UiEventLog.Write(
                $"capture-lane intent='{configuration.IntentMode}' lane='{SharedLaneKeys.Anime.LaneId}' style='{configuration.AnimeStyle}' era='{configuration.AnimeEra}' celShading={configuration.AnimeCelShading} cleanLineArt={configuration.AnimeCleanLineArt} expressiveEyes={configuration.AnimeExpressiveEyes} dynamicAction={configuration.AnimeDynamicAction} cinematicLighting={configuration.AnimeCinematicLighting} stylizedHair={configuration.AnimeStylizedHair} atmosphericEffects={configuration.AnimeAtmosphericEffects}");
        }
        else if (IntentModeCatalog.IsPixelArt(configuration.IntentMode))
        {
            UiEventLog.Write(
                $"capture-lane intent='{configuration.IntentMode}' lane='{SharedLaneKeys.PixelArt.LaneId}' subtype='{configuration.PixelArtSubtype}' limitedPalette={configuration.PixelArtLimitedPalette} dithering={configuration.PixelArtDithering} tileableDesign={configuration.PixelArtTileableDesign} spriteSheetReadability={configuration.PixelArtSpriteSheetReadability} cleanOutline={configuration.PixelArtCleanOutline} subpixelShading={configuration.PixelArtSubpixelShading} hudUiFraming={configuration.PixelArtHudUiFraming}");
        }

        return configuration;
    }

    private void ApplyConfiguration(PromptConfiguration configuration)
    {
        StandardLaneStateAdapter.HydrateConfiguration(configuration);
        _isApplyingConfiguration = true;
        _fantasyIllustrationAppliedSliderSuppressions.Clear();
        _editorialIllustrationAppliedSliderSuppressions.Clear();
        _graphicDesignAppliedSliderSuppressions.Clear();
        _infographicAppliedSliderSuppressions.Clear();
        _dataVizAppliedSliderSuppressions.Clear();
        ApplyConfigurationState(configuration);
        ApplyCompressionConfiguration(configuration);
        SyncStandardLanePanelStates();
        SyncFantasyIllustrationSliderSuppressions();
        SyncEditorialIllustrationSliderSuppressions();
        SyncGraphicDesignSliderSuppressions();
        SyncInfographicDataVisualizationSliderSuppressions();
        SyncConceptArtSliderSuppressions();
        IsArtistPhraseEditorOpen = false;
        ApplyArtistNegativeConstraintDefaults();
        SyncExperimentalMacrosFromRaw();
        _isApplyingConfiguration = false;
        RegeneratePrompt();
    }

    private void CopyPrompt()
    {
        if (string.IsNullOrWhiteSpace(PromptPreview))
        {
            StatusMessage = "Nothing to copy yet.";
            return;
        }

        var wasPlainExperimental = string.Equals(IntentMode, IntentModeCatalog.ExperimentalName, StringComparison.OrdinalIgnoreCase);
        var copied = CopyExportText(PromptPreview, "Prompt");
        if (copied && wasPlainExperimental)
        {
            QueueIntentModeWorkflowUpdate("Custom");
        }

        if (copied)
        {
            CopyPromptFeedbackTick++;
        }
    }

    private void CopyNegativePrompt()
    {
        if (!UseNegativePrompt || string.IsNullOrWhiteSpace(NegativePromptPreview))
        {
            StatusMessage = "Negative prompt is disabled.";
            return;
        }

        try
        {
            _clipboardService.SetText(NegativePromptPreview);
            StatusMessage = "Negative prompt copied.";
        }
        catch
        {
            StatusMessage = "Could not copy the negative prompt.";
        }
    }
    private void Reset()
    {
        PresetName = string.Empty;
        SelectedPresetName = null;
        var configuration = new PromptConfiguration
        {
            IntentMode = "Custom",
            Temperature = 50,
            ExcludeTemperatureFromPrompt = false,
            LightingIntensity = 50,
            ExcludeLightingIntensityFromPrompt = false,
            Stylization = 50,
            ExcludeStylizationFromPrompt = false,
            Realism = 50,
            ExcludeRealismFromPrompt = false,
            TextureDepth = 35,
            ExcludeTextureDepthFromPrompt = false,
            NarrativeDensity = 35,
            ExcludeNarrativeDensityFromPrompt = false,
            Symbolism = 25,
            ExcludeSymbolismFromPrompt = false,
            AtmosphericDepth = 40,
            ExcludeAtmosphericDepthFromPrompt = false,
            SurfaceAge = 20,
            ExcludeSurfaceAgeFromPrompt = false,
            Chaos = 20,
            ExcludeChaosFromPrompt = false,
            Framing = 50,
            ExcludeFramingFromPrompt = false,
            Material = "None",
            ArtStyle = "None",
            AnimeStyle = LaneRegistry.GetDefaultSubtypeLabel(IntentModeCatalog.AnimeName, "style", "General Anime"),
            AnimeEra = LaneRegistry.GetDefaultSubtypeLabel(IntentModeCatalog.AnimeName, "era", "Default / Modern"),
            ChildrensBookStyle = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.ChildrensBookName, SharedLaneKeys.StyleSelector, SharedLaneKeys.ChildrensBook.DefaultSubtypeLabel),
            ChildrensBookSoftColorPalette = false,
            ChildrensBookTexturedPaper = false,
            ChildrensBookInkLinework = false,
            ChildrensBookExpressiveCharacters = false,
            ChildrensBookMinimalBackground = false,
            ChildrensBookDecorativeDetails = false,
            ChildrensBookGentleLighting = false,
            ComicBookStyle = LaneRegistry.GetDefaultSubtypeLabel(IntentModeCatalog.ComicBookName, "style", "General Comic"),
            ComicBookBoldInk = false,
            ComicBookHalftoneShading = false,
            ComicBookPanelFraming = false,
            ComicBookDynamicPoses = false,
            ComicBookSpeedLines = false,
            ComicBookHighContrastLighting = false,
            ComicBookSpeechBubbles = false,
            SpeechBubbleMode = BlankSpeechBubbleMode,
            SpeechBubbleSize = MediumSpeechBubbleSize,
            StylizedSpeechBubbleShape = false,
            CinematicSubtype = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.CinematicName, SharedLaneKeys.StyleSelector, SharedLaneKeys.Cinematic.DefaultSubtypeLabel),
            CinematicLetterboxedFraming = false,
            CinematicShallowDepthOfField = false,
            CinematicPracticalLighting = false,
            CinematicAtmosphericHaze = false,
            CinematicFilmGrain = false,
            CinematicAnamorphicFlares = false,
            CinematicDramaticBacklight = false,
            ThreeDRenderSubtype = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.ThreeDRenderName, SharedLaneKeys.StyleSelector, SharedLaneKeys.ThreeDRender.DefaultSubtypeLabel),
            ThreeDRenderGlobalIllumination = false,
            ThreeDRenderVolumetricLighting = false,
            ThreeDRenderRayTracedReflections = false,
            ThreeDRenderDepthOfField = false,
            ThreeDRenderSubsurfaceScattering = false,
            ThreeDRenderHardSurfacePrecision = false,
            ThreeDRenderStudioBackdrop = false,
            ConceptArtSubtype = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.ConceptArtName, SharedLaneKeys.StyleSelector, SharedLaneKeys.ConceptArt.DefaultSubtypeLabel),
            ConceptArtDesignCallouts = false,
            ConceptArtTurnaroundReadability = false,
            ConceptArtMaterialBreakdown = false,
            ConceptArtScaleReference = false,
            ConceptArtWorldbuildingAccents = false,
            ConceptArtProductionNotesFeel = false,
            ConceptArtSilhouetteClarity = false,
            PixelArtSubtype = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.PixelArtName, SharedLaneKeys.StyleSelector, SharedLaneKeys.PixelArt.DefaultSubtypeLabel),
            PixelArtLimitedPalette = false,
            PixelArtDithering = false,
            PixelArtTileableDesign = false,
            PixelArtSpriteSheetReadability = false,
            PixelArtCleanOutline = false,
            PixelArtSubpixelShading = false,
            PixelArtHudUiFraming = false,
            WatercolorStyle = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.WatercolorName, SharedLaneKeys.StyleSelector, SharedLaneKeys.Watercolor.DefaultSubtypeLabel),
            WatercolorTransparentWashes = false,
            WatercolorSoftBleeds = false,
            WatercolorPaperTexture = false,
            WatercolorInkAndWatercolor = false,
            WatercolorAtmosphericWash = false,
            WatercolorGouacheAccents = false,
            FantasyIllustrationRegister = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.FantasyIllustrationName, SharedLaneKeys.FantasyIllustration.RegisterSelector, SharedLaneKeys.FantasyIllustration.DefaultRegisterLabel),
            FantasyIllustrationCharacterSketch = false,
            FantasyIllustrationCharacterCentric = false,
            FantasyIllustrationEnvironmentConcept = false,
            FantasyIllustrationKeyArt = false,
            FantasyIllustrationCleanBackground = false,
            FantasyIllustrationSilhouetteReadability = false,
            FantasyIllustrationPhotorealistic = false,
            FantasyIllustrationCartoonArt = false,
            FantasyIllustrationPropArtifactFocus = false,
            FantasyIllustrationCreatureDesign = false,
            EditorialIllustrationBlackAndWhiteMonochrome = false,
            GraphicDesignType = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.GraphicDesignName, SharedLaneKeys.GraphicDesign.TypeSelector, SharedLaneKeys.GraphicDesign.DefaultTypeLabel),
            GraphicDesignMinimalLayout = false,
            GraphicDesignBoldHierarchy = false,
            InfographicDataVisualizationSubdomain = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.InfographicDataVisualizationName, SharedLaneKeys.InfographicDataVisualization.SubdomainSelector, SharedLaneKeys.InfographicDataVisualization.DefaultSubdomainLabel),
            InfographicModeLeanExplainer = true,
            InfographicModePublicPoster = false,
            InfographicModeReferenceSheet = false,
            DataVizModeChartPurity = true,
            DataVizModeDashboard = false,
            DataVizModeReportGraphic = false,
            PhotographyType = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.PhotographyName, SharedLaneKeys.Photography.TypeSelector, SharedLaneKeys.Photography.DefaultTypeLabel),
            PhotographyEra = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.PhotographyName, SharedLaneKeys.Photography.EraSelector, SharedLaneKeys.Photography.DefaultEraLabel),
            PhotographyCandidCapture = false,
            PhotographyPosedStagedCapture = false,
            PhotographyAvailableLight = false,
            PhotographyOnCameraFlash = false,
            PhotographyEditorialPolish = false,
            PhotographyRawDocumentaryTexture = false,
            PhotographyEnvironmentalPortraitContext = false,
            PhotographyFilmAnalogCharacter = false,
            FoodPhotographyShotMode = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.FoodPhotographyName, SharedLaneKeys.FoodPhotography.ShotModeSelector, SharedLaneKeys.FoodPhotography.DefaultShotModeLabel),
            FoodPhotographyVisibleSteam = false,
            FoodPhotographyGarnishEmphasis = false,
            FoodPhotographyUtensilContext = false,
            FoodPhotographyHandServiceCue = false,
            FoodPhotographyIngredientScatter = false,
            FoodPhotographyCondensationEmphasis = false,
            LifestyleAdvertisingShotMode = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.LifestyleAdvertisingPhotographyName, SharedLaneKeys.LifestyleAdvertising.ShotModeSelector, SharedLaneKeys.LifestyleAdvertising.DefaultShotModeLabel),
            LifestyleAdvertisingNaturalInteraction = false,
            LifestyleAdvertisingProductInUse = false,
            LifestyleAdvertisingBrandColorAccent = false,
            LifestyleAdvertisingPropContext = false,
            LifestyleAdvertisingSunlitOptimism = false,
            LifestyleAdvertisingMotionCandidness = false,
            ArchitectureArchvizViewMode = LaneRegistry.GetDefaultSubtypeValue(IntentModeCatalog.ArchitectureArchvizName, SharedLaneKeys.ArchitectureArchviz.ViewModeSelector, SharedLaneKeys.ArchitectureArchviz.DefaultViewModeLabel),
            ArchitectureArchvizHumanScaleCues = false,
            ArchitectureArchvizLandscapeEmphasis = false,
            ArchitectureArchvizFurnishingEmphasis = false,
            ArchitectureArchvizWarmInteriorGlow = false,
            ArchitectureArchvizReflectiveSurfaceAccents = false,
            ArchitectureArchvizAmenityFocus = false,
            ArtistInfluencePrimary = "None",
            InfluenceStrengthPrimary = 45,
            ArtistInfluenceSecondary = "None",
            InfluenceStrengthSecondary = 30,
            CameraDistance = 50,
            ExcludeCameraDistanceFromPrompt = false,
            CameraAngle = 50,
            ExcludeCameraAngleFromPrompt = false,
            BackgroundComplexity = 40,
            ExcludeBackgroundComplexityFromPrompt = false,
            MotionEnergy = 20,
            ExcludeMotionEnergyFromPrompt = false,
            FocusDepth = 50,
            ExcludeFocusDepthFromPrompt = false,
            ImageCleanliness = 55,
            ExcludeImageCleanlinessFromPrompt = false,
            DetailDensity = 50,
            ExcludeDetailDensityFromPrompt = false,
            Whimsy = 20,
            ExcludeWhimsyFromPrompt = false,
            Tension = 20,
            ExcludeTensionFromPrompt = false,
            Awe = 40,
            ExcludeAweFromPrompt = false,
            Lighting = "Soft daylight",
            Saturation = 55,
            ExcludeSaturationFromPrompt = false,
            Contrast = 55,
            ExcludeContrastFromPrompt = false,
            AspectRatio = "1:1",
            UseNegativePrompt = false,
            CompressPromptSemantics = false,
            ReduceRepeatedLaneWords = false,
            TrimRepeatedLongWords = false,
            SemanticPairInteractions = true,
            AvoidClutter = true,
            AvoidMuddyLighting = true,
            AvoidDistortedAnatomy = true,
            AvoidExtraLimbs = true,
            AvoidTextArtifacts = true,
            AvoidOversaturation = true,
            AvoidFlatComposition = true,
            AvoidMessyBackground = true,
            AvoidWeakMaterialDefinition = true,
            AvoidBlurryDetail = true,
            VintageBendEasternBlocGdr = false,
            VintageBendThrillerUndertone = false,
            VintageBendInstitutionalAusterity = false,
            VintageBendSurveillanceStateAtmosphere = false,
            VintageBendPeriodArtifacts = false,
            VintageBendUrbanCivilian = false,
        };
        configuration.StandardLaneStates = StandardLaneStateAdapter.CaptureFromConfiguration(configuration);
        ApplyConfiguration(configuration);
        StatusMessage = "Controls reset to defaults.";
    }

    private void RandomizeSliders()
    {
        var configuration = CaptureConfiguration();
        configuration.IntentMode = IntentModeCatalog.ExperimentalName;

        configuration.Temperature = Random.Shared.Next(0, 101);
        configuration.LightingIntensity = Random.Shared.Next(0, 101);
        configuration.Stylization = Random.Shared.Next(0, 101);
        configuration.Realism = Random.Shared.Next(0, 101);
        configuration.TextureDepth = Random.Shared.Next(0, 101);
        configuration.NarrativeDensity = Random.Shared.Next(0, 101);
        configuration.Symbolism = Random.Shared.Next(0, 101);
        configuration.AtmosphericDepth = Random.Shared.Next(0, 101);
        configuration.SurfaceAge = Random.Shared.Next(0, 101);
        configuration.Chaos = Random.Shared.Next(0, 101);
        configuration.Framing = Random.Shared.Next(0, 101);
        if (!ExcludeArtistSlidersFromRandomize)
        {
            configuration.InfluenceStrengthPrimary = Random.Shared.Next(0, 101);
            configuration.InfluenceStrengthSecondary = Random.Shared.Next(0, 101);
        }
        configuration.CameraDistance = Random.Shared.Next(0, 101);
        configuration.CameraAngle = Random.Shared.Next(0, 101);
        configuration.BackgroundComplexity = Random.Shared.Next(0, 101);
        configuration.MotionEnergy = Random.Shared.Next(0, 101);
        configuration.FocusDepth = Random.Shared.Next(0, 101);
        configuration.ImageCleanliness = Random.Shared.Next(0, 101);
        configuration.DetailDensity = Random.Shared.Next(0, 101);
        configuration.Whimsy = Random.Shared.Next(0, 101);
        configuration.Tension = Random.Shared.Next(0, 101);
        configuration.Awe = Random.Shared.Next(0, 101);
        configuration.Saturation = Random.Shared.Next(0, 101);
        configuration.Contrast = Random.Shared.Next(0, 101);

        ApplyConfiguration(configuration);
        QueueIntentModeWorkflowUpdate(IntentModeCatalog.ExperimentalName);
        StatusMessage = "All slider values randomized.";
    }

    private void SetIntentModeFromWorkflow(string intentMode)
    {
        var normalizedIntentMode = ResolveIntentModeSelection(intentMode);
        if (!string.Equals(IntentMode, normalizedIntentMode, StringComparison.OrdinalIgnoreCase))
        {
            IntentMode = normalizedIntentMode;
            return;
        }
        
        OnPropertyChanged(nameof(IntentMode));
        OnPropertyChanged(nameof(IntentModeSelectedIndex));
    }

    private void QueueIntentModeWorkflowUpdate(string intentMode)
    {
        var dispatcher = Application.Current?.Dispatcher;
        if (dispatcher is null)
        {
            SetIntentModeFromWorkflow(intentMode);
            return;
        }

        dispatcher.BeginInvoke(
            DispatcherPriority.Render,
            new Action(() => SetIntentModeFromWorkflow(intentMode)));
    }

    private string ResolveIntentModeSelection(string? intentMode)
    {
        var normalizedIntentMode = NormalizeIntentMode(intentMode);
        return IntentModes.FirstOrDefault(mode => string.Equals(mode, normalizedIntentMode, StringComparison.OrdinalIgnoreCase))
            ?? normalizedIntentMode;
    }

    private int GetIntentModeSelectedIndex()
    {
        var selectedMode = ResolveIntentModeSelection(IntentMode);
        return IntentModes.IndexOf(selectedMode);
    }

    private bool CanCopyPrompt()
    {
        return !IsDemoExpired && !IsLockedLaneActive && (!IsDemoMode || RemainingDemoCopies > 0);
    }

    private bool CanCopyNegativePrompt()
    {
        return !IsDemoExpired && !IsLockedLaneActive;
    }

    private bool CopyExportText(string text, string label)
    {
        if (IsDemoMode && RemainingDemoCopies <= 0)
        {
            StatusMessage = "Demo export limit reached. Unlock the full version to restore prompt output and controls.";
            RaiseCopyCommandCanExecuteChanged();
            return false;
        }

        try
        {
            _clipboardService.SetText(text);
        }
        catch
        {
            StatusMessage = $"Could not copy the {label.ToLowerInvariant()}.";
            return false;
        }

        if (!IsDemoMode)
        {
            StatusMessage = $"{label} copied.";
            return true;
        }

        if (!_demoStateService.TryConsumeCopy(out var state))
        {
            StatusMessage = $"{label} copied, but the demo counter could not be updated locally.";
            return true;
        }

        RemainingDemoCopies = state.RemainingCopies;
        StatusMessage = RemainingDemoCopies > 0
            ? $"{label} copied. {RemainingDemoCopies} demo exports remaining."
            : $"{label} copied. Demo export limit reached.";
        return true;
    }

    private void UpdateLockedLaneAccessState()
    {
        var laneRequiresUnlock = RequiresLaneUnlock(IntentMode);
        var hasAccess = HasLaneAccess(IntentMode);

        if (_hasLockedLaneAccess == hasAccess)
        {
            return;
        }

        _hasLockedLaneAccess = hasAccess;
        OnPropertyChanged(nameof(HasLockedLaneAccess));
        OnPropertyChanged(nameof(IsLockedLaneActive));
        OnPropertyChanged(nameof(IsVintageBendLocked));
        OnPropertyChanged(nameof(ShowInteractivePromptPreview));
        OnPropertyChanged(nameof(ShowDemoPromptPreview));
        OnPropertyChanged(nameof(ShowLockedLaneAuthoringSections));
        OnPropertyChanged(nameof(ShowVintageBendAuthoringSections));
        OnPropertyChanged(nameof(ShowLockedLanePane));
        OnPropertyChanged(nameof(ShowVintageBendLockedPane));
        OnPropertyChanged(nameof(ShowManualIntentControls));
        OnPropertyChanged(nameof(ShowLegacyManualCompositionCard));
        OnPropertyChanged(nameof(ShowEmbeddedLaneCompositionCard));
        OnPropertyChanged(nameof(ShowManualNegativeConstraints));
        OnPropertyChanged(nameof(ShowCompactComicBookManualStack));
        OnPropertyChanged(nameof(ShowCompactCinematicManualStack));
        OnPropertyChanged(nameof(ShowCompactWatercolorManualStack));
        OnPropertyChanged(nameof(ShowCompactChildrensBookManualStack));
        OnPropertyChanged(nameof(ShowCompactThreeDRenderManualStack));
        OnPropertyChanged(nameof(ShowCompactArchitectureArchvizManualStack));
        OnPropertyChanged(nameof(ShowCompactTattooArtManualStack));
        OnPropertyChanged(nameof(ShowVintageBendModifierPanel));
        OnPropertyChanged(nameof(CanUseCompressionControls));
        OnPropertyChanged(nameof(ShowCompressionTierTwo));
        OnPropertyChanged(nameof(ShowCompressionTierThree));
        OnPropertyChanged(nameof(ShowNegativePrompt));
        OnPropertyChanged(nameof(ShowCompactCinematicPanel));
        OnPropertyChanged(nameof(ShowCompactWatercolorPanel));
        OnPropertyChanged(nameof(ShowCompactChildrensBookPanel));
        OnPropertyChanged(nameof(ShowCompactThreeDRenderPanel));
        OnPropertyChanged(nameof(ShowCompactTattooArtPanel));
        OnPropertyChanged(nameof(ShowActiveStandardLanePanel));
        OnPropertyChanged(nameof(ShowResolvedStandardLanePanel));
        OnPropertyChanged(nameof(ShowArtistBlendSummary));
        RaiseCopyCommandCanExecuteChanged();

        if (laneRequiresUnlock && !_isApplyingConfiguration)
        {
            RegeneratePrompt();
        }
    }

    private void CopyLaneHelpEmail()
    {
        CopyExportText(LaneHelpTooltipCatalog.ContactEmail, "Email address");
    }

    private void ApplyArtistNegativeConstraintDefaults()
    {
        var allowsDistortion = IsAnyArtistActive("Pablo Picasso", "Salvador Dali", "Salvador Dalí", "El Greco", "Amedeo Modigliani", "Francis Bacon", "Egon Schiele");
        var allowsFlatComposition = IsAnyArtistActive("Pablo Picasso");

        SetProperty(ref _avoidClutter, true, nameof(AvoidClutter));
        SetProperty(ref _avoidMuddyLighting, true, nameof(AvoidMuddyLighting));
        SetProperty(ref _avoidDistortedAnatomy, !allowsDistortion, nameof(AvoidDistortedAnatomy));
        SetProperty(ref _avoidExtraLimbs, true, nameof(AvoidExtraLimbs));
        SetProperty(ref _avoidTextArtifacts, true, nameof(AvoidTextArtifacts));
        SetProperty(ref _avoidOversaturation, true, nameof(AvoidOversaturation));
        SetProperty(ref _avoidFlatComposition, !allowsFlatComposition, nameof(AvoidFlatComposition));
        SetProperty(ref _avoidMessyBackground, true, nameof(AvoidMessyBackground));
        SetProperty(ref _avoidWeakMaterialDefinition, true, nameof(AvoidWeakMaterialDefinition));
        SetProperty(ref _avoidBlurryDetail, true, nameof(AvoidBlurryDetail));
    }

    private bool IsAnyArtistActive(params string[] artistNames)
    {
        return artistNames.Any(IsArtistActive);
    }

    private bool IsArtistActive(string artistName)
    {
        return (InfluenceStrengthPrimary > 20 && string.Equals(ArtistInfluencePrimary, artistName, StringComparison.OrdinalIgnoreCase))
            || (InfluenceStrengthSecondary > 20 && string.Equals(ArtistInfluenceSecondary, artistName, StringComparison.OrdinalIgnoreCase));
    }

    private void RaiseArtistBlendSummaryChanged()
    {
        OnPropertyChanged(nameof(ShowArtistBlendSummary));
        OnPropertyChanged(nameof(ArtistBlendSummaryTitle));
        OnPropertyChanged(nameof(ArtistBlendSummaryBody));
        OnPropertyChanged(nameof(CompositionDriver));
        OnPropertyChanged(nameof(PaletteDriver));
        OnPropertyChanged(nameof(SurfaceDriver));
        OnPropertyChanged(nameof(MoodDriver));
    }

    private void RaiseSliderHelperChanged()
    {
        OnPropertyChanged(nameof(InfluenceStrengthPrimaryValueText));
        OnPropertyChanged(nameof(InfluenceStrengthPrimaryGuideText));
        OnPropertyChanged(nameof(InfluenceStrengthSecondaryValueText));
        OnPropertyChanged(nameof(InfluenceStrengthSecondaryGuideText));
        OnPropertyChanged(nameof(TemperatureHelper));
        OnPropertyChanged(nameof(TemperatureValueText));
        OnPropertyChanged(nameof(TemperatureGuideText));
        OnPropertyChanged(nameof(LightingIntensityHelper));
        OnPropertyChanged(nameof(LightingIntensityValueText));
        OnPropertyChanged(nameof(LightingIntensityGuideText));
        OnPropertyChanged(nameof(StylizationHelper));
        OnPropertyChanged(nameof(StylizationValueText));
        OnPropertyChanged(nameof(StylizationGuideText));
        OnPropertyChanged(nameof(RealismHelper));
        OnPropertyChanged(nameof(RealismValueText));
        OnPropertyChanged(nameof(RealismGuideText));
        OnPropertyChanged(nameof(TextureDepthHelper));
        OnPropertyChanged(nameof(TextureDepthValueText));
        OnPropertyChanged(nameof(TextureDepthGuideText));
        OnPropertyChanged(nameof(NarrativeDensityHelper));
        OnPropertyChanged(nameof(NarrativeDensityValueText));
        OnPropertyChanged(nameof(NarrativeDensityGuideText));
        OnPropertyChanged(nameof(SymbolismHelper));
        OnPropertyChanged(nameof(SymbolismValueText));
        OnPropertyChanged(nameof(SymbolismGuideText));
        OnPropertyChanged(nameof(SurfaceAgeHelper));
        OnPropertyChanged(nameof(SurfaceAgeValueText));
        OnPropertyChanged(nameof(SurfaceAgeGuideText));
        OnPropertyChanged(nameof(FramingHelper));
        OnPropertyChanged(nameof(FramingValueText));
        OnPropertyChanged(nameof(FramingGuideText));
        OnPropertyChanged(nameof(CameraDistanceHelper));
        OnPropertyChanged(nameof(CameraDistanceValueText));
        OnPropertyChanged(nameof(CameraDistanceGuideText));
        OnPropertyChanged(nameof(CameraAngleHelper));
        OnPropertyChanged(nameof(CameraAngleValueText));
        OnPropertyChanged(nameof(CameraAngleGuideText));
        OnPropertyChanged(nameof(BackgroundComplexityHelper));
        OnPropertyChanged(nameof(BackgroundComplexityValueText));
        OnPropertyChanged(nameof(BackgroundComplexityGuideText));
        OnPropertyChanged(nameof(MotionEnergyHelper));
        OnPropertyChanged(nameof(MotionEnergyValueText));
        OnPropertyChanged(nameof(MotionEnergyGuideText));
        OnPropertyChanged(nameof(FocusDepthHelper));
        OnPropertyChanged(nameof(FocusDepthValueText));
        OnPropertyChanged(nameof(FocusDepthGuideText));
        OnPropertyChanged(nameof(ImageCleanlinessHelper));
        OnPropertyChanged(nameof(ImageCleanlinessValueText));
        OnPropertyChanged(nameof(ImageCleanlinessGuideText));
        OnPropertyChanged(nameof(DetailDensityHelper));
        OnPropertyChanged(nameof(DetailDensityValueText));
        OnPropertyChanged(nameof(DetailDensityGuideText));
        OnPropertyChanged(nameof(AtmosphericDepthHelper));
        OnPropertyChanged(nameof(AtmosphericDepthValueText));
        OnPropertyChanged(nameof(AtmosphericDepthGuideText));
        OnPropertyChanged(nameof(ChaosHelper));
        OnPropertyChanged(nameof(ChaosValueText));
        OnPropertyChanged(nameof(ChaosGuideText));
        OnPropertyChanged(nameof(WhimsyHelper));
        OnPropertyChanged(nameof(WhimsyValueText));
        OnPropertyChanged(nameof(WhimsyGuideText));
        OnPropertyChanged(nameof(TensionHelper));
        OnPropertyChanged(nameof(TensionValueText));
        OnPropertyChanged(nameof(TensionGuideText));
        OnPropertyChanged(nameof(AweHelper));
        OnPropertyChanged(nameof(AweValueText));
        OnPropertyChanged(nameof(AweGuideText));
        OnPropertyChanged(nameof(SaturationHelper));
        OnPropertyChanged(nameof(SaturationValueText));
        OnPropertyChanged(nameof(SaturationGuideText));
        OnPropertyChanged(nameof(ContrastHelper));
        OnPropertyChanged(nameof(ContrastValueText));
        OnPropertyChanged(nameof(ContrastGuideText));
    }

    private void RaiseCopyCommandCanExecuteChanged()
    {
        CopyPromptCommand.RaiseCanExecuteChanged();
        CopyNegativePromptCommand.RaiseCanExecuteChanged();
    }

    private string BuildIntentModeSummary()
    {
        var activeLane = LaneRegistry.GetByIntentName(IntentMode);
        if (activeLane is not null)
        {
            return activeLane.Summary;
        }

        if (IntentModeCatalog.IsVintageBend(IntentMode))
        {
            return "Vintage Bend language pack is active: candid documentary realism, period-correct color restraint, and disciplined analog texture are now steering prompt language.";
        }

        if (IntentModeCatalog.IsAnime(IntentMode))
        {
            return "Anime language pack is active: stylized illustration language, clean silhouette readability, and anime-aware slider phrasing are now steering the prompt.";
        }

        if (IntentModeCatalog.IsChildrensBook(IntentMode))
        {
            return "Children's Book language pack is active: gentle illustrated storytelling, compact anchor language, and story-first slider phrasing are now steering the prompt.";
        }

        if (IntentModeCatalog.IsWatercolor(IntentMode))
        {
            return "Watercolor language pack is active: transparent pigment handling, paper-backed softness, and watercolor-aware slider phrasing are now steering the prompt.";
        }

        if (IntentModeCatalog.IsCinematic(IntentMode))
        {
            return "Cinematic language pack is active: film-still anchoring, lens-aware framing, and cinematic slider phrasing are now steering the prompt.";
        }

        if (IntentModeCatalog.IsThreeDRender(IntentMode))
        {
            return "3D Render language pack is active: clean CGI anchoring, render-native lighting, and material-aware slider phrasing are now steering the prompt.";
        }

        if (IntentModeCatalog.IsConceptArt(IntentMode))
        {
            return "Concept Art language pack is active: production-design anchoring, concept-aware slider phrasing, and compact modifier support are now steering the prompt.";
        }

        if (IntentModeCatalog.IsPixelArt(IntentMode))
        {
            return "Pixel Art language pack is active: sprite-readable anchoring, palette discipline, and pixel-native slider phrasing are now steering the prompt.";
        }

        if (IntentModeCatalog.IsExperimental(IntentMode))
        {
            return "Experimental governance is active: slider phrases are orchestrated through the isolated prototype layer instead of the standard intent path.";
        }

        return IntentModeCatalog.TryGet(IntentMode, out var intentMode)
            ? $"Intent bundle active: {intentMode.Summary}."
            : "Custom intent structure exposes the full manual controls for mood, composition, color, and output shaping.";
    }

    private static string NormalizeIntentMode(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "Custom" : value.Trim();
    }

    private static string NormalizePresentationMode(string? value)
    {
        return string.Equals(value, CompactPresentationMode, StringComparison.OrdinalIgnoreCase)
            ? CompactPresentationMode
            : StandardPresentationMode;
    }

    private void RaiseCompactPresentationRoutingChanged()
    {
        OnPropertyChanged(nameof(ShowStandardManualStyleControlsCard));
        OnPropertyChanged(nameof(ShowCompactAnimeManualStyleControlsCard));
        OnPropertyChanged(nameof(ShowStandardManualMoodCard));
        OnPropertyChanged(nameof(ShowCompactAnimeManualMoodCard));
        OnPropertyChanged(nameof(ShowStandardManualLightingAndColorCard));
        OnPropertyChanged(nameof(ShowCompactAnimeManualLightingAndColorCard));
        OnPropertyChanged(nameof(ShowStandardManualImageFinishCard));
        OnPropertyChanged(nameof(ShowCompactAnimeManualImageFinishCard));
        OnPropertyChanged(nameof(ShowStandardManualOutputCard));
        OnPropertyChanged(nameof(ShowCompactAnimeManualOutputCard));
        OnPropertyChanged(nameof(ShowCompactComicBookManualStack));
        OnPropertyChanged(nameof(ShowCompactCinematicManualStack));
        OnPropertyChanged(nameof(ShowCompactWatercolorManualStack));
        OnPropertyChanged(nameof(ShowCompactChildrensBookManualStack));
        OnPropertyChanged(nameof(ShowCompactThreeDRenderManualStack));
        OnPropertyChanged(nameof(ShowCompactArchitectureArchvizManualStack));
        OnPropertyChanged(nameof(ShowCompactTattooArtManualStack));
        OnPropertyChanged(nameof(ShowStandardAnimePanel));
        OnPropertyChanged(nameof(ShowCompactAnimePanel));
        OnPropertyChanged(nameof(ShowSubjectContextFields));
        OnPropertyChanged(nameof(ShowCompactCinematicPanel));
        OnPropertyChanged(nameof(ShowCompactWatercolorPanel));
        OnPropertyChanged(nameof(ShowCompactChildrensBookPanel));
        OnPropertyChanged(nameof(ShowCompactThreeDRenderPanel));
        OnPropertyChanged(nameof(ShowCompactTattooArtPanel));
        OnPropertyChanged(nameof(ShowActiveStandardLanePanel));
        OnPropertyChanged(nameof(ShowResolvedStandardLanePanel));
    }

    private static readonly Dictionary<string, SliderBandMetadata> SliderBands = new(StringComparer.Ordinal)
    {
        ["Temperature"] = new("Cool", "Mild cool", "Neutral", "Warm", "Hot"),
        ["LightingIntensity"] = new("Dim", "Soft", "Balanced", "Bright", "Radiant"),
        ["Stylization"] = new("Grounded", "Light", "Stylized", "Strong", "Maximal"),
        ["Realism"] = new("Off", "Loose", "Moderate", "High", "Strong"),
        ["TextureDepth"] = new("Minimal", "Light", "Clear", "Rich", "Deep"),
        ["NarrativeDensity"] = new("Simple", "Light", "Layered", "Dense", "World-rich"),
        ["Symbolism"] = new("Literal", "Subtle", "Suggestive", "Allegoric", "Mythic"),
        ["SurfaceAge"] = new("Fresh", "Soft wear", "Weathered", "Aged", "Time-worn"),
        ["Framing"] = new("Intimate", "Tight", "Balanced", "Open", "Expansive"),
        ["CameraDistance"] = new("Extreme close", "Close", "Mid", "Far", "Distant"),
        ["CameraAngle"] = new("Low", "Slight low", "Eye level", "Slight high", "High"),
        ["BackgroundComplexity"] = new("Minimal", "Restrained", "Supporting", "Rich", "Layered"),
        ["MotionEnergy"] = new("Still", "Gentle", "Active", "Dynamic", "Kinetic"),
        ["FocusDepth"] = new("Deep focus", "Mostly deep", "Balanced", "Selective", "Very shallow"),
        ["ImageCleanliness"] = new("Raw", "Slight grit", "Balanced", "Clean", "Polished"),
        ["DetailDensity"] = new("Sparse", "Light", "Moderate", "Rich", "Dense"),
        ["AtmosphericDepth"] = new("Flat", "Light", "Air-filled", "Luminous", "Deep"),
        ["Chaos"] = new("Controlled", "Restless", "Volatile", "Orchestrated", "Unstable"),
        // UI label is "Levity", but the internal key stays "Whimsy" so presets,
        // bindings, and lane metadata remain compatible.
        ["Whimsy"] = new("Serious", "Subtle", "Playful", "Strong", "Bold"),
        ["Tension"] = new("Low", "Light", "Noticeable", "Strong", "Intense"),
        ["Awe"] = new("Grounded", "Slight", "Wonder", "Awe", "Grand"),
        ["Saturation"] = new("Muted", "Restrained", "Balanced", "Rich", "Vivid"),
        ["Contrast"] = new("Low", "Gentle", "Balanced", "Crisp", "Striking"),
    };

    private string BuildArtistBlendSummaryTitle()
    {
        var primary = CreateArtistState(ArtistInfluencePrimary, InfluenceStrengthPrimary);
        var secondary = CreateArtistState(ArtistInfluenceSecondary, InfluenceStrengthSecondary);

        if (primary is null && secondary is null)
        {
            return "Artist blend summary";
        }

        if (primary is not null && secondary is not null)
        {
            if (string.Equals(primary.Name, secondary.Name, StringComparison.OrdinalIgnoreCase))
            {
                return $"Focused through {primary.Name}";
            }

            return Math.Abs(primary.Strength - secondary.Strength) >= 20
                ? $"{(primary.Strength >= secondary.Strength ? primary.Name : secondary.Name)}-led blend"
                : $"Balanced blend of {primary.Name} and {secondary.Name}";
        }

        return $"Single-artist direction: {(primary ?? secondary)!.Name}";
    }

    private string BuildArtistBlendSummaryBody()
    {
        var primary = CreateArtistState(ArtistInfluencePrimary, InfluenceStrengthPrimary);
        var secondary = CreateArtistState(ArtistInfluenceSecondary, InfluenceStrengthSecondary);

        if (primary is null && secondary is null)
        {
            return "Choose one or two artists to shape composition, palette, surface character, and mood.";
        }

        if (primary is not null && secondary is not null)
        {
            if (string.Equals(primary.Name, secondary.Name, StringComparison.OrdinalIgnoreCase))
            {
                return $"Both lanes currently reinforce {primary.Name}, creating a concentrated single-artist read rather than a cross-artist blend.";
            }

            return "The stronger lane now steers structural decisions first, while the lighter lane is shown as an accent source across specific visual dimensions.";
        }

        return "One artist is active, so each visual dimension currently resolves to the same single-source influence.";
    }

    private string BuildContributionValue(ContributionArea area)
    {
        var primary = CreateArtistState(ArtistInfluencePrimary, InfluenceStrengthPrimary);
        var secondary = CreateArtistState(ArtistInfluenceSecondary, InfluenceStrengthSecondary);

        if (primary is null && secondary is null)
        {
            return "No active artist";
        }

        if (primary is not null && secondary is not null && string.Equals(primary.Name, secondary.Name, StringComparison.OrdinalIgnoreCase))
        {
            return primary.Name;
        }

        if (primary is null || secondary is null)
        {
            return (primary ?? secondary)!.Name;
        }

        var stronger = primary.Strength >= secondary.Strength ? primary : secondary;
        var lighter = ReferenceEquals(stronger, primary) ? secondary : primary;
        var difference = stronger.Strength - lighter.Strength;

        return area switch
        {
            ContributionArea.Composition => stronger.Name,
            ContributionArea.Palette => difference >= 35 ? lighter.Name : $"{stronger.Name} + {lighter.Name}",
            ContributionArea.Surface => difference >= 15 ? lighter.Name : $"{stronger.Name} + {lighter.Name}",
            ContributionArea.Mood => difference >= 35 ? lighter.Name : stronger.Name,
            _ => stronger.Name,
        };
    }

    private ArtistState? CreateArtistState(string name, int strength)
    {
        if (!HasActiveArtist(name, strength))
        {
            return null;
        }

        var resolved = _artistProfileService.GetProfile(name)?.Name ?? name;
        return new ArtistState(resolved, strength);
    }

    private string GetSliderHelper(string key, int value)
    {
        if (IsVintageBendIntent)
        {
            var vintagePhrase = SliderLanguageCatalog.ResolveVintageBendPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(vintagePhrase))
            {
                return string.Empty;
            }

            return $"Vintage Bend: {vintagePhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsAnimeIntent)
        {
            var animePhrase = SliderLanguageCatalog.ResolveAnimePhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(animePhrase))
            {
                return string.Empty;
            }

            return $"Anime: {animePhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsChildrensBookIntent)
        {
            var childrensBookPhrase = SliderLanguageCatalog.ResolveChildrensBookPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(childrensBookPhrase))
            {
                return string.Empty;
            }

            return $"Children's Book: {childrensBookPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsComicBookIntent)
        {
            var comicBookPhrase = SliderLanguageCatalog.ResolveComicBookPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(comicBookPhrase))
            {
                return string.Empty;
            }

            return $"Comic Book: {comicBookPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsCinematicIntent)
        {
            var cinematicPhrase = SliderLanguageCatalog.ResolveCinematicPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(cinematicPhrase))
            {
                return string.Empty;
            }

            return $"Cinematic: {cinematicPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsThreeDRenderIntent)
        {
            var threeDRenderPhrase = SliderLanguageCatalog.ResolveThreeDRenderPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(threeDRenderPhrase))
            {
                return string.Empty;
            }

            return $"3D Render: {threeDRenderPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsConceptArtIntent)
        {
            var conceptArtPhrase = SliderLanguageCatalog.ResolveConceptArtPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(conceptArtPhrase))
            {
                return string.Empty;
            }

            return $"Concept Art: {conceptArtPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsPixelArtIntent)
        {
            var pixelArtPhrase = SliderLanguageCatalog.ResolvePixelArtPhrase(key, value, CaptureConfiguration());
            if (string.Equals(key, "Realism", StringComparison.Ordinal) && value <= 20)
            {
                return $"Pixel Art: omit explicit realism{BuildArtistHelperTint(key)}".Trim();
            }

            if (string.IsNullOrWhiteSpace(pixelArtPhrase))
            {
                return string.Empty;
            }

            return $"Pixel Art: {pixelArtPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsFantasyIllustrationIntent)
        {
            var fantasyIllustrationPhrase = SliderLanguageCatalog.ResolveFantasyIllustrationPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(fantasyIllustrationPhrase))
            {
                return string.Empty;
            }

            return $"Fantasy Illustration: {fantasyIllustrationPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsEditorialIllustrationIntent)
        {
            var editorialIllustrationPhrase = SliderLanguageCatalog.ResolveEditorialIllustrationPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(editorialIllustrationPhrase))
            {
                return string.Empty;
            }

            return $"Editorial Illustration: {editorialIllustrationPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsTattooArtIntent)
        {
            var tattooArtPhrase = SliderLanguageCatalog.ResolveTattooArtPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(tattooArtPhrase))
            {
                return string.Empty;
            }

            return $"Tattoo Art: {tattooArtPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsGraphicDesignIntent)
        {
            var graphicDesignPhrase = SliderLanguageCatalog.ResolveGraphicDesignPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(graphicDesignPhrase))
            {
                return string.Empty;
            }

            return $"Graphic Design: {graphicDesignPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsWatercolorIntent)
        {
            var watercolorPhrase = SliderLanguageCatalog.ResolveWatercolorPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(watercolorPhrase))
            {
                return string.Empty;
            }

            return $"Watercolor: {watercolorPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsPhotographyIntent)
        {
            var photographyPhrase = SliderLanguageCatalog.ResolvePhotographyPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(photographyPhrase))
            {
                return string.Empty;
            }

            return $"Photography: {photographyPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsFoodPhotographyIntent)
        {
            var foodPhotographyPhrase = SliderLanguageCatalog.ResolveFoodPhotographyPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(foodPhotographyPhrase))
            {
                return string.Empty;
            }

            return $"Food Photography: {foodPhotographyPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsLifestyleAdvertisingPhotographyIntent)
        {
            var lifestyleAdvertisingPhrase = SliderLanguageCatalog.ResolveLifestyleAdvertisingPhotographyPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(lifestyleAdvertisingPhrase))
            {
                return string.Empty;
            }

            return $"Lifestyle / Advertising Photography: {lifestyleAdvertisingPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        if (IsArchitectureArchvizIntent)
        {
            var architectureArchvizPhrase = SliderLanguageCatalog.ResolveArchitectureArchvizPhrase(key, value, CaptureConfiguration());
            if (string.IsNullOrWhiteSpace(architectureArchvizPhrase))
            {
                return string.Empty;
            }

            return $"Architecture / Archviz: {architectureArchvizPhrase}{BuildArtistHelperTint(key)}".Trim();
        }

        var artPrefix = ArtStyle switch
        {
            "Painterly" => "Painterly: ",
            "Yarn Relief" => "Textile: ",
            "Stained Glass" => "Glasswork: ",
            "Surreal Symbolic" => "Surreal: ",
            "Concept Art" => "Concept: ",
            "Pixel Art" => "Pixel: ",
            "Cinematic" => "Cinematic: ",
            "3D Render" => "3D Render: ",
            _ => string.Empty,
        };

        var materialPrefix = Material switch
        {
            "Yarn" => "Fiber focus. ",
            "Paint" => "Pigment focus. ",
            "Glass" => "Glass focus. ",
            "Ink" => "Ink focus. ",
            "Stone" => "Stone focus. ",
            "Metal" => "Metal focus. ",
            _ => string.Empty,
        };

        string phrase = key switch
        {
            "Temperature" => MapBand(value, "cool color temperature", "slightly cool balance", "neutral temperature balance", "warm color temperature", "heated warm cast"),
            "LightingIntensity" => MapBand(value, "dim lighting", "soft lighting", "balanced lighting", "bright scene lighting", "radiant luminous lighting"),
            "Stylization" => MapBand(value, "grounded visual treatment", "light stylization", "stylized rendering", "strong stylization", "highly stylized visual language"),
            "Realism" => MapBand(value, "omit explicit realism", "loosely realistic", "moderately realistic", "high visual realism", "strongly realistic rendering"),
            "TextureDepth" => MapBand(value, "minimal added texture", "light surface texture", "clear material texture", "rich tactile surface detail", "deeply worked tactile relief"),
            "NarrativeDensity" => MapBand(value, "simple single-read image", "light narrative layering", "layered storytelling cues", "dense implied story", "world-rich narrative density"),
            "Symbolism" => MapBand(value, "mostly literal", "subtle symbolic cues", "suggestive symbolic motifs", "pronounced allegory", "mythic symbolic charge"),
            "SurfaceAge" => MapBand(value, "freshly finished surfaces", "subtle patina", "gentle weathering", "aged surface character", "time-worn patina"),
            "Framing" => MapBand(value, "intimate framing", "tight framing", "balanced framing", "open framing", "expansive framing"),
            "CameraDistance" => MapBand(value, "extreme close view", "close view", "mid-distance view", "wider distant view", "far-set distant view"),
            "CameraAngle" => MapBand(value, "low angle view", "slightly low angle", "eye-level view", "slightly high angle", "high angle view"),
            "BackgroundComplexity" => MapBand(value, "minimal background", "restrained background", "supporting environment", "rich environmental detail", "densely layered environment"),
            "MotionEnergy" => MapBand(value, "still composition", "gentle motion", "active scene energy", "dynamic motion", "high kinetic energy"),
            "FocusDepth" => MapBand(value, "deep focus clarity", "mostly deep focus", "balanced focus depth", "selective focus falloff", "very shallow depth of field"),
            "ImageCleanliness" => MapBand(value, "raw visual finish", "slight visual grit", "balanced finish", "clean visual finish", "polished visual finish"),
            "DetailDensity" => MapBand(value, "sparse detail treatment", "light detail presence", "moderate detail density", "rich fine detail", "dense detail layering"),
            "AtmosphericDepth" => MapBand(value, "limited atmospheric depth", "slight recession", "air-filled spatial depth", "luminous depth layering", "deep atmospheric perspective"),
            "Chaos" => MapBand(value, "controlled composition", "restless tension", "volatile energy", "orchestrated chaos", "high visual instability"),
            "Whimsy" => MapBand(value, "serious tone", "subtle levity", "playful tone", "strong playful energy", "bold comic levity"),
            "Tension" => MapBand(value, "low tension", "light dramatic tension", "noticeable tension", "strong interpersonal tension", "intense dramatic tension"),
            "Awe" => MapBand(value, "grounded scale", "slight wonder", "atmosphere of wonder", "strong sense of awe", "overwhelming grandeur"),
            "Saturation" => MapBand(value, "muted saturation", "restrained color", "balanced saturation", "rich color saturation", "vivid color saturation"),
            "Contrast" => MapBand(value, "low contrast", "gentle contrast", "balanced contrast", "crisp contrast", "striking contrast"),
            _ => string.Empty,
        };

        var artistTint = BuildArtistHelperTint(key);
        return string.IsNullOrWhiteSpace(phrase) ? string.Empty : $"{artPrefix}{materialPrefix}{phrase}{artistTint}".Trim();
    }

    private string BuildArtistHelperTint(string key)
    {
        var area = GetContributionAreaForHelper(key);
        if (area is null)
        {
            return string.Empty;
        }

        var driver = BuildContributionValue(area.Value);
        if (string.IsNullOrWhiteSpace(driver) || string.Equals(driver, "No active artist", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        var label = area.Value switch
        {
            ContributionArea.Composition => "composition",
            ContributionArea.Palette => "palette",
            ContributionArea.Surface => "surface character",
            ContributionArea.Mood => "mood",
            _ => "direction",
        };

        var verb = driver.Contains(" + ", StringComparison.Ordinal) ? "drive" : "drives";
        return $" Artist tint: {driver} {verb} {label}.";
    }

    private static ContributionArea? GetContributionAreaForHelper(string key) => key switch
    {
        "Stylization" => ContributionArea.Composition,
        "NarrativeDensity" => ContributionArea.Composition,
        "BackgroundComplexity" => ContributionArea.Composition,
        "MotionEnergy" => ContributionArea.Composition,
        "Chaos" => ContributionArea.Composition,
        "Framing" => ContributionArea.Composition,
        "CameraDistance" => ContributionArea.Composition,
        "CameraAngle" => ContributionArea.Composition,
        "FocusDepth" => ContributionArea.Composition,
        "Realism" => ContributionArea.Surface,
        "TextureDepth" => ContributionArea.Surface,
        "SurfaceAge" => ContributionArea.Surface,
        "ImageCleanliness" => ContributionArea.Surface,
        "DetailDensity" => ContributionArea.Surface,
        "Temperature" => ContributionArea.Palette,
        "LightingIntensity" => ContributionArea.Palette,
        "Saturation" => ContributionArea.Palette,
        "Contrast" => ContributionArea.Palette,
        "Symbolism" => ContributionArea.Mood,
        "AtmosphericDepth" => ContributionArea.Mood,
        "Whimsy" => ContributionArea.Mood,
        "Tension" => ContributionArea.Mood,
        "Awe" => ContributionArea.Mood,
        _ => null,
    };

    private string GetInfluenceBandLabel(int value)
    {
        if (IsVintageBendIntent)
        {
            if (value <= 20) return "omit artist language";
            if (value <= 40) return "light stylistic cues from";
            if (value <= 60) return "artist-informed sensibility drawn from";
            if (value <= 80) return "clearly shaped by";
            return "deeply informed by";
        }

        if (value <= 20) return "Off";
        if (value <= 40) return "subtle influence";
        if (value <= 60) return "artist-influenced sensibility";
        if (value <= 80) return "strongly shaped";
        return "deeply informed";
    }

    private string GetSliderBandGuide(string key)
    {
        return IsVintageBendIntent
            ? SliderLanguageCatalog.ResolveVintageBendGuideText(key, CaptureConfiguration())
            : IsAnimeIntent
                ? SliderLanguageCatalog.ResolveAnimeGuideText(key, CaptureConfiguration())
                : IsChildrensBookIntent
                    ? SliderLanguageCatalog.ResolveChildrensBookGuideText(key, CaptureConfiguration())
                : IsComicBookIntent
                    ? SliderLanguageCatalog.ResolveComicBookGuideText(key, CaptureConfiguration())
                : IsCinematicIntent
                    ? SliderLanguageCatalog.ResolveCinematicGuideText(key)
                : IsThreeDRenderIntent
                    ? SliderLanguageCatalog.ResolveThreeDRenderGuideText(key)
                : IsConceptArtIntent
                    ? SliderLanguageCatalog.ResolveConceptArtGuideText(key, CaptureConfiguration())
                : IsPixelArtIntent
                    ? SliderLanguageCatalog.ResolvePixelArtGuideText(key)
                : IsFantasyIllustrationIntent
                    ? SliderLanguageCatalog.ResolveFantasyIllustrationGuideText(key, CaptureConfiguration())
                : IsEditorialIllustrationIntent
                    ? SliderLanguageCatalog.ResolveEditorialIllustrationGuideText(key)
                : IsGraphicDesignIntent
                    ? SliderLanguageCatalog.ResolveGraphicDesignGuideText(key, CaptureConfiguration())
                : IntentModeCatalog.IsInfographicDataVisualization(IntentMode)
                    ? SliderLanguageCatalog.ResolveInfographicDataVisualizationGuideText(key, CaptureConfiguration())
                : IsTattooArtIntent
                    ? SliderLanguageCatalog.ResolveTattooArtGuideText(key)
                : IsWatercolorIntent
                    ? SliderLanguageCatalog.ResolveWatercolorGuideText(key, CaptureConfiguration())
                : IsProductPhotographyIntent
                    ? SliderLanguageCatalog.ResolveProductPhotographyGuideText(key, CaptureConfiguration())
                : IsFoodPhotographyIntent
                    ? SliderLanguageCatalog.ResolveFoodPhotographyGuideText(key, CaptureConfiguration())
                : IsLifestyleAdvertisingPhotographyIntent
                    ? SliderLanguageCatalog.ResolveLifestyleAdvertisingPhotographyGuideText(key, CaptureConfiguration())
                : IsArchitectureArchvizIntent
                    ? SliderLanguageCatalog.ResolveArchitectureArchvizGuideText(key, CaptureConfiguration())
                : IsPhotographyIntent
                    ? SliderLanguageCatalog.ResolvePhotographyGuideText(key, CaptureConfiguration())
                : TryGetBandMetadata(key, out var metadata)
                ? metadata.GuideText
                : "0  |  100";
    }

    private string GetSliderBandLabel(string key, int value)
    {
        var configuration = CaptureConfiguration();
        var label = SliderLanguageCatalog.ResolvePromptPhraseOrFallback(key, value, configuration);
        if (!string.IsNullOrWhiteSpace(label))
        {
            return label;
        }

        return TryGetBandMetadata(key, out var metadata)
            ? metadata.GetBandLabel(value)
            : value.ToString();
    }

    private string GetInfluenceBandGuideText()
    {
        return IsVintageBendIntent
            ? "0-20  |  omit artist language  |  21-40  |  light stylistic cues from  |  41-60  |  artist-informed sensibility drawn from  |  61-80  |  clearly shaped by  |  81-100  |  deeply informed by"
            : "Off  |  subtle influence  |  artist-influenced sensibility  |  strongly shaped  |  deeply informed";
    }

    private static bool TryGetBandMetadata(string key, out SliderBandMetadata metadata)
    {
        return SliderBands.TryGetValue(key, out metadata!);
    }

    private static string MapBand(int value, string low, string lowMid, string mid, string high, string veryHigh)
    {
        if (value <= 20) return low;
        if (value <= 40) return lowMid;
        if (value <= 60) return mid;
        if (value <= 80) return high;
        return veryHigh;
    }

    private static bool HasActiveArtist(string? name, int strength) => strength > 20 && !string.IsNullOrWhiteSpace(name) && !string.Equals(name, "None", StringComparison.OrdinalIgnoreCase);
    private static bool HasSelectedArtistValue(string? name) => !string.IsNullOrWhiteSpace(name) && !string.Equals(name, "None", StringComparison.OrdinalIgnoreCase);

    private enum ContributionArea
    {
        Composition,
        Palette,
        Surface,
        Mood,
    }

    private sealed record SliderBandMetadata(params string[] Labels)
    {
        public string GuideText => string.Join("  |  ", Labels);

        public string GetBandLabel(int value)
        {
            if (Labels.Length == 0)
            {
                return value.ToString();
            }

            var normalized = Math.Clamp(value, 0, 100);
            var index = normalized switch
            {
                <= 20 => 0,
                <= 40 => 1,
                <= 60 => 2,
                <= 80 => 3,
                _ => 4,
            };
            return Labels[index];
        }
    }

    private sealed record ArtistState(string Name, int Strength);
}




