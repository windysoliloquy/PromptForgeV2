using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using PromptForge.App.Commands;
using PromptForge.App.Models;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
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
    private readonly DispatcherTimer _experimentalMacroRefreshTimer;
    private readonly IReadOnlyDictionary<string, StandardLanePanelViewModel> _sharedLanePanels;
    private StandardLaneStateCollection _ordinaryLaneStates;
    private bool _isApplyingConfiguration;
    private bool _suspendArtistOverrideReset;

    private string _intentMode = IntentModeCatalog.AnimeName;
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
    private bool _watercolorTransparentWashes;
    private bool _watercolorSoftBleeds;
    private bool _watercolorPaperTexture;
    private bool _watercolorInkAndWatercolor;
    private bool _watercolorAtmosphericWash;
    private bool _watercolorGouacheAccents;
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
    private string _architectureArchvizViewMode = "exterior";
    private bool _architectureArchvizHumanScaleCues;
    private bool _architectureArchvizLandscapeEmphasis;
    private bool _architectureArchvizFurnishingEmphasis;
    private bool _architectureArchvizWarmInteriorGlow;
    private bool _architectureArchvizReflectiveSurfaceAccents;
    private bool _architectureArchvizAmenityFocus;
    private string _artistInfluencePrimary = "None";
    private int _influenceStrengthPrimary = 45;
    private ArtistPhraseOverride _primaryArtistPhraseOverride = new();
    private string _artistInfluenceSecondary = "None";
    private int _influenceStrengthSecondary = 30;
    private ArtistPhraseOverride _secondaryArtistPhraseOverride = new();
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
    private bool _excludeArtistSlidersFromRandomize = true;
    private string _selectedThemeName = string.Empty;
    private string _promptPreview = string.Empty;
    private string _negativePromptPreview = string.Empty;
    private string _presetName = string.Empty;
    private string? _selectedPresetName;
    private string _statusMessage = "Ready.";
    private int _remainingDemoCopies;
    private long _copyPromptFeedbackTick;
    private string? _artistPairGuidanceTooltip;
    private bool _isArtistPhraseEditorOpen;
    private bool _isEditingPrimaryArtistPhrase;
    private string _artistPhraseEditorPrefix = string.Empty;
    private string _artistPhraseEditorArtistName = string.Empty;
    private string _artistPhraseEditorSuffix = string.Empty;
    private string _artistPhraseEditorGeneratedPhrase = string.Empty;
    private IReadOnlyList<ArtistPhraseQuickInsertGroup> _artistPhraseQuickInsertGroups = Array.Empty<ArtistPhraseQuickInsertGroup>();
    private ArtistPairLookupResult? _currentArtistPairLookup;
    private IReadOnlyList<ArtistPhraseSuffixRoleGroup> _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
    private string _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
    private string _artistPhraseEditorStructuredSuffixTrailingText = string.Empty;

    static MainWindowViewModel()
    {
        StandardLaneBindingValidator.ThrowIfInvalid(typeof(MainWindowViewModel), StandardLaneBindingValidator.GetSharedStandardLaneDefinitions());
    }

    public MainWindowViewModel(IPromptBuilderService promptBuilderService, IPresetStorageService presetStorageService, IClipboardService clipboardService, IArtistProfileService artistProfileService, IArtistPairGuidanceService artistPairGuidanceService, IThemeService themeService, IDemoStateService demoStateService, ILicenseService licenseService)
    {
        _promptBuilderService = promptBuilderService;
        _presetStorageService = presetStorageService;
        _clipboardService = clipboardService;
        _artistProfileService = artistProfileService;
        _artistPairGuidanceService = artistPairGuidanceService;
        _themeService = themeService;
        _demoStateService = demoStateService;
        _licenseService = licenseService;
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
        ArtistInfluences = new ObservableCollection<string>(artistProfileService.GetArtistNames());
        Lightings = new ObservableCollection<string>(new[] { "Soft daylight", "Golden hour", "Dramatic studio light", "Overcast", "Moonlit", "Soft glow", "Dusk haze", "Warm directional light", "Volumetric cinematic light" });
        AspectRatios = new ObservableCollection<string>(new[] { "1:1", "4:5", "16:9", "9:16" });
        PresetNames = new ObservableCollection<string>();
        Themes = new ObservableCollection<string>(themeService.AvailableThemeNames);
        _sharedLanePanels = BuildSharedLanePanels();

        CopyPromptCommand = new RelayCommand(CopyPrompt, CanCopyPrompt);
        CopyNegativePromptCommand = new RelayCommand(CopyNegativePrompt, CanCopyNegativePrompt);
        SavePresetCommand = new RelayCommand(SavePreset);
        LoadPresetCommand = new RelayCommand(LoadPreset);
        RenamePresetCommand = new RelayCommand(RenamePreset);
        DeletePresetCommand = new RelayCommand(DeletePreset);
        EditPrimaryArtistPhraseCommand = new RelayCommand(OpenPrimaryArtistPhraseEditor, () => CanEditPrimaryArtistPhrase);
        EditSecondaryArtistPhraseCommand = new RelayCommand(OpenSecondaryArtistPhraseEditor, () => CanEditSecondaryArtistPhrase);
        SwapArtistInfluencesCommand = new RelayCommand(SwapArtistInfluences, () => CanSwapArtistInfluences);
        InsertArtistPhraseQuickInsertCommand = new RelayCommand(InsertArtistPhraseQuickInsert, parameter => parameter is ArtistPhraseQuickInsert);
        SaveArtistPhraseEditorCommand = new RelayCommand(SaveArtistPhraseEditor, () => IsArtistPhraseEditorOpen && !string.IsNullOrWhiteSpace(ArtistPhraseEditorArtistName));
        ResetArtistPhraseEditorCommand = new RelayCommand(ResetArtistPhraseEditorToGenerated, () => IsArtistPhraseEditorOpen);
        CancelArtistPhraseEditorCommand = new RelayCommand(CancelArtistPhraseEditor, () => IsArtistPhraseEditorOpen);
        ResetCommand = new RelayCommand(Reset);
        RandomizeSlidersCommand = new RelayCommand(RandomizeSliders);

        RefreshPresetNames();
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
    public ObservableCollection<string> ArtistInfluences { get; }
    public ObservableCollection<string> Lightings { get; }
    public ObservableCollection<string> AspectRatios { get; }
    public ObservableCollection<string> PresetNames { get; }
    public ObservableCollection<string> Themes { get; }

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
    public bool IsArtistPhraseEditorOpen
    {
        get => _isArtistPhraseEditorOpen;
        private set
        {
            if (SetProperty(ref _isArtistPhraseEditorOpen, value))
            {
                SaveArtistPhraseEditorCommand.RaiseCanExecuteChanged();
                ResetArtistPhraseEditorCommand.RaiseCanExecuteChanged();
                CancelArtistPhraseEditorCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string ArtistPhraseEditorTitle => _isEditingPrimaryArtistPhrase
        ? "Edit Primary Artist Phrase"
        : "Edit Secondary Artist Phrase";

    public string ArtistPhraseEditorPrefix
    {
        get => _artistPhraseEditorPrefix;
        set
        {
            if (SetProperty(ref _artistPhraseEditorPrefix, value))
            {
                OnPropertyChanged(nameof(ArtistPhraseEditorPreview));
            }
        }
    }

    public string ArtistPhraseEditorArtistName
    {
        get => _artistPhraseEditorArtistName;
        private set
        {
            if (SetProperty(ref _artistPhraseEditorArtistName, value))
            {
                OnPropertyChanged(nameof(ArtistPhraseEditorPreview));
                SaveArtistPhraseEditorCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string ArtistPhraseEditorSuffix
    {
        get => _artistPhraseEditorSuffix;
        set
        {
            if (SetProperty(ref _artistPhraseEditorSuffix, value))
            {
                if (!string.Equals(value, _artistPhraseEditorStructuredSuffixLastRendered, StringComparison.Ordinal))
                {
                    _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
                    _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
                    _artistPhraseEditorStructuredSuffixTrailingText = value?.Trim() ?? string.Empty;
                }

                OnPropertyChanged(nameof(ArtistPhraseEditorPreview));
            }
        }
    }

    public string ArtistPhraseEditorGeneratedPhrase
    {
        get => _artistPhraseEditorGeneratedPhrase;
        private set => SetProperty(ref _artistPhraseEditorGeneratedPhrase, value);
    }

    public string ArtistPhraseEditorPreview => ArtistPhraseComposer.Combine(ArtistPhraseEditorPrefix, ArtistPhraseEditorArtistName, ArtistPhraseEditorSuffix);
    public IReadOnlyList<ArtistPhraseQuickInsertGroup> ArtistPhraseQuickInsertGroups
    {
        get => _artistPhraseQuickInsertGroups;
        private set => SetProperty(ref _artistPhraseQuickInsertGroups, value);
    }
    public bool CanEditPrimaryArtistPhrase => TryGetArtistPhraseSlotState(isPrimary: true, out _);
    public bool CanEditSecondaryArtistPhrase => TryGetArtistPhraseSlotState(isPrimary: false, out _);
    public bool CanSwapArtistInfluences
        => HasSelectedArtistValue(ArtistInfluencePrimary) || HasSelectedArtistValue(ArtistInfluenceSecondary);

    public RelayCommand CopyPromptCommand { get; }
    public RelayCommand CopyNegativePromptCommand { get; }
    public RelayCommand SavePresetCommand { get; }
    public RelayCommand LoadPresetCommand { get; }
    public RelayCommand RenamePresetCommand { get; }
    public RelayCommand DeletePresetCommand { get; }
    public RelayCommand EditPrimaryArtistPhraseCommand { get; }
    public RelayCommand EditSecondaryArtistPhraseCommand { get; }
    public RelayCommand SwapArtistInfluencesCommand { get; }
    public RelayCommand InsertArtistPhraseQuickInsertCommand { get; }
    public RelayCommand SaveArtistPhraseEditorCommand { get; }
    public RelayCommand ResetArtistPhraseEditorCommand { get; }
    public RelayCommand CancelArtistPhraseEditorCommand { get; }
    public RelayCommand ResetCommand { get; }
    public RelayCommand RandomizeSlidersCommand { get; }

    public string IntentMode
    {
        get => _intentMode;
        set
        {
            var normalized = NormalizeIntentMode(value);
            if (SetProperty(ref _intentMode, normalized))
            {
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
                OnPropertyChanged(nameof(ShowAnimeModifierPanel));
                OnPropertyChanged(nameof(IsChildrensBookIntent));
                OnPropertyChanged(nameof(IsComicBookIntent));
                OnPropertyChanged(nameof(ShowComicBookModifierPanel));
                OnPropertyChanged(nameof(IsCinematicIntent));
                OnPropertyChanged(nameof(IsPhotographyIntent));
                OnPropertyChanged(nameof(IsProductPhotographyIntent));
                OnPropertyChanged(nameof(IsFoodPhotographyIntent));
                OnPropertyChanged(nameof(IsArchitectureArchvizIntent));
                OnPropertyChanged(nameof(IsThreeDRenderIntent));
                OnPropertyChanged(nameof(IsConceptArtIntent));
                OnPropertyChanged(nameof(IsPixelArtIntent));
                OnPropertyChanged(nameof(IsWatercolorIntent));
                OnPropertyChanged(nameof(IsVintageBendIntent));
                OnPropertyChanged(nameof(ShowVintageBendModifierPanel));
                if (!_isApplyingConfiguration)
                {
                    SyncCompressionStateForIntentChange(normalized);
                    ResetPromptExclusionFlags();
                    if (IntentModeCatalog.IsAnime(normalized))
                    {
                        ApplyAnimeIntentDefaults();
                    }
                    if (IntentModeCatalog.IsProductPhotography(normalized))
                    {
                        ApplyProductPhotographyIntentDefaults();
                    }
                    if (IntentModeCatalog.IsFoodPhotography(normalized))
                    {
                        ApplyFoodPhotographyIntentDefaults();
                    }
                    if (IntentModeCatalog.IsArchitectureArchviz(normalized))
                    {
                        ApplyArchitectureArchvizIntentDefaults();
                    }
                    if (IntentModeCatalog.IsPhotography(normalized))
                    {
                        ApplyPhotographyIntentDefaults();
                    }
                }
                OnPropertyChanged(nameof(CanUseCompressionControls));
                OnPropertyChanged(nameof(ShowCompressionTierTwo));
                OnPropertyChanged(nameof(ShowCompressionTierThree));
                OnPropertyChanged(nameof(IntentModeSummary));
                OnPropertyChanged(nameof(ActiveStandardLanePanel));
                OnPropertyChanged(nameof(ShowActiveStandardLanePanel));
                if (!_isApplyingConfiguration && !_isApplyingExperimentalMacroState)
                {
                    SyncExperimentalMacrosFromRaw();
                }
                RegeneratePrompt();
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
    public string Subject { get => _subject; set => SetAndRefresh(ref _subject, value); }
    public string Action { get => _action; set => SetAndRefresh(ref _action, value); }
    public string Relationship { get => _relationship; set => SetAndRefresh(ref _relationship, value); }
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
    public bool ExcludeNarrativeDensityFromPrompt { get => _excludeNarrativeDensityFromPrompt; set => SetAndRefresh(ref _excludeNarrativeDensityFromPrompt, value); }
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
    public string ChildrensBookStyle { get => _childrensBookStyle; set => SetStandardLaneSelectorAndRefresh(ref _childrensBookStyle, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.StyleSelector); }
    public bool ChildrensBookSoftColorPalette { get => _childrensBookSoftColorPalette; set => SetStandardLaneModifierAndRefresh(ref _childrensBookSoftColorPalette, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.SoftColorPalette); }
    public bool ChildrensBookTexturedPaper { get => _childrensBookTexturedPaper; set => SetStandardLaneModifierAndRefresh(ref _childrensBookTexturedPaper, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.TexturedPaper); }
    public bool ChildrensBookInkLinework { get => _childrensBookInkLinework; set => SetStandardLaneModifierAndRefresh(ref _childrensBookInkLinework, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.InkLinework); }
    public bool ChildrensBookExpressiveCharacters { get => _childrensBookExpressiveCharacters; set => SetStandardLaneModifierAndRefresh(ref _childrensBookExpressiveCharacters, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.ExpressiveCharacters); }
    public bool ChildrensBookMinimalBackground { get => _childrensBookMinimalBackground; set => SetStandardLaneModifierAndRefresh(ref _childrensBookMinimalBackground, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.MinimalBackground); }
    public bool ChildrensBookDecorativeDetails { get => _childrensBookDecorativeDetails; set => SetStandardLaneModifierAndRefresh(ref _childrensBookDecorativeDetails, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.DecorativeDetails); }
    public bool ChildrensBookGentleLighting { get => _childrensBookGentleLighting; set => SetStandardLaneModifierAndRefresh(ref _childrensBookGentleLighting, value, SharedLaneKeys.ChildrensBook.LaneId, SharedLaneKeys.ChildrensBook.GentleLighting); }
    public string ComicBookStyle { get => _comicBookStyle; set => SetAndRefresh(ref _comicBookStyle, value); }
    public bool ComicBookBoldInk { get => _comicBookBoldInk; set => SetAndRefresh(ref _comicBookBoldInk, value); }
    public bool ComicBookHalftoneShading { get => _comicBookHalftoneShading; set => SetAndRefresh(ref _comicBookHalftoneShading, value); }
    public bool ComicBookPanelFraming { get => _comicBookPanelFraming; set => SetAndRefresh(ref _comicBookPanelFraming, value); }
    public bool ComicBookDynamicPoses { get => _comicBookDynamicPoses; set => SetAndRefresh(ref _comicBookDynamicPoses, value); }
    public bool ComicBookSpeedLines { get => _comicBookSpeedLines; set => SetAndRefresh(ref _comicBookSpeedLines, value); }
    public bool ComicBookHighContrastLighting { get => _comicBookHighContrastLighting; set => SetAndRefresh(ref _comicBookHighContrastLighting, value); }
    public bool ComicBookSpeechBubbles { get => _comicBookSpeechBubbles; set => SetAndRefresh(ref _comicBookSpeechBubbles, value); }
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
    public string ConceptArtSubtype { get => _conceptArtSubtype; set => SetStandardLaneSelectorAndRefresh(ref _conceptArtSubtype, value, SharedLaneKeys.ConceptArt.LaneId, SharedLaneKeys.StyleSelector); }
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
    public bool CanUseCompressionControls => IntentModeCatalog.TryGet(IntentMode, out _);
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
    public bool ShowCompressionTierTwo => CanUseCompressionControls && CompressPromptSemantics;
    public bool ShowCompressionTierThree => CanUseCompressionControls && CompressPromptSemantics && ReduceRepeatedLaneWords;
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
    public bool IsUnlocked => _licenseService.IsUnlocked;
    public bool IsDemoMode => DemoModeOptions.IsDemoMode && !IsUnlocked;
    public bool ShowDemoModeBanner => IsDemoMode;
    public bool ShowInteractivePromptPreview => !IsDemoMode;
    public bool ShowDemoPromptPreview => IsDemoMode;
    public int MaxDemoCopies => DemoModeOptions.MaxDemoCopies;
    public string VersionButtonText => IsDemoMode ? "Unlock Full Version" : "Version Info";
    public int RemainingDemoCopies
    {
        get => _remainingDemoCopies;
        private set
        {
            if (SetProperty(ref _remainingDemoCopies, value))
            {
                OnPropertyChanged(nameof(DemoModeHeadline));
                OnPropertyChanged(nameof(CopyPromptRemainingText));
                OnPropertyChanged(nameof(DemoModeBody));
                RaiseCopyCommandCanExecuteChanged();
            }
        }
    }
    public string DemoModeHeadline => RemainingDemoCopies > 0
        ? $"Demo mode: {RemainingDemoCopies} of {MaxDemoCopies} exports remaining"
        : "Demo mode: export limit reached";
    public string CopyPromptRemainingText => RemainingDemoCopies > 0
        ? $"{RemainingDemoCopies} of {MaxDemoCopies} left"
        : "No exports left";
    public string DemoModeBody => RemainingDemoCopies > 0
        ? "Preview stays readable, but export is limited to the copy buttons."
        : "Preview stays visible, but copy/export is now locked until the full version is used.";
    public bool ShowNegativePrompt => UseNegativePrompt;
    public bool IsCustomIntent => string.Equals(IntentMode, "Custom", StringComparison.OrdinalIgnoreCase);
    public bool ShowCustomRandomizeControls => string.Equals(IntentMode, "Custom", StringComparison.OrdinalIgnoreCase) || IntentModeCatalog.IsVintageBend(IntentMode);
    public bool ExcludeArtistSlidersFromRandomize { get => _excludeArtistSlidersFromRandomize; set => SetProperty(ref _excludeArtistSlidersFromRandomize, value); }
    public bool ShowManualIntentControls => string.Equals(IntentMode, "Custom", StringComparison.OrdinalIgnoreCase) || LaneRegistry.TryGetByIntentName(IntentMode, out _) || IsExperimentalManualAdvancedMode;
    public bool ShowLegacyManualCompositionCard => IsCustomIntent || IsExperimentalManualAdvancedMode;
    public bool ShowEmbeddedLaneCompositionCard => ShowManualIntentControls && !IsCustomIntent && !IsExperimentalIntent;
    public bool ShowManualNegativeConstraints => ShowManualIntentControls && UseNegativePrompt;
    public bool IsAnimeIntent => IntentModeCatalog.IsAnime(IntentMode);
    public bool ShowAnimeModifierPanel => IsAnimeIntent;
    public bool IsChildrensBookIntent => IntentModeCatalog.IsChildrensBook(IntentMode);
    public bool IsComicBookIntent => IntentModeCatalog.IsComicBook(IntentMode);
    public bool ShowComicBookModifierPanel => IsComicBookIntent;
    public bool IsCinematicIntent => IntentModeCatalog.IsCinematic(IntentMode);
    public bool IsPhotographyIntent => IntentModeCatalog.IsPhotography(IntentMode);
    public bool IsProductPhotographyIntent => IntentModeCatalog.IsProductPhotography(IntentMode);
    public bool IsFoodPhotographyIntent => IntentModeCatalog.IsFoodPhotography(IntentMode);
    public bool IsArchitectureArchvizIntent => IntentModeCatalog.IsArchitectureArchviz(IntentMode);
    public bool IsThreeDRenderIntent => IntentModeCatalog.IsThreeDRender(IntentMode);
    public bool IsConceptArtIntent => IntentModeCatalog.IsConceptArt(IntentMode);
    public bool IsPixelArtIntent => IntentModeCatalog.IsPixelArt(IntentMode);
    public bool IsWatercolorIntent => IntentModeCatalog.IsWatercolor(IntentMode);
    public bool IsVintageBendIntent => IntentModeCatalog.IsVintageBend(IntentMode);
    public bool ShowVintageBendModifierPanel => IsVintageBendIntent;
    public StandardLanePanelViewModel? ActiveStandardLanePanel => _sharedLanePanels.TryGetValue(IntentMode, out var panel) ? panel : null;
    public bool ShowActiveStandardLanePanel => ActiveStandardLanePanel is not null;
    public string IntentModeSummary => BuildIntentModeSummary();
    public bool VintageBendEasternBlocGdr { get => _vintageBendEasternBlocGdr; set => SetAndRefresh(ref _vintageBendEasternBlocGdr, value); }
    public bool VintageBendThrillerUndertone { get => _vintageBendThrillerUndertone; set => SetAndRefresh(ref _vintageBendThrillerUndertone, value); }
    public bool VintageBendInstitutionalAusterity { get => _vintageBendInstitutionalAusterity; set => SetAndRefresh(ref _vintageBendInstitutionalAusterity, value); }
    public bool VintageBendSurveillanceStateAtmosphere { get => _vintageBendSurveillanceStateAtmosphere; set => SetAndRefresh(ref _vintageBendSurveillanceStateAtmosphere, value); }
    public bool VintageBendPeriodArtifacts { get => _vintageBendPeriodArtifacts; set => SetAndRefresh(ref _vintageBendPeriodArtifacts, value); }
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
    public bool ShowArtistBlendSummary => HasActiveArtist(ArtistInfluencePrimary, InfluenceStrengthPrimary) || HasActiveArtist(ArtistInfluenceSecondary, InfluenceStrengthSecondary);
    public string ArtistBlendSummaryTitle => BuildArtistBlendSummaryTitle();
    public string ArtistBlendSummaryBody => BuildArtistBlendSummaryBody();
    public string CompositionDriver => BuildContributionValue(ContributionArea.Composition);
    public string PaletteDriver => BuildContributionValue(ContributionArea.Palette);
    public string SurfaceDriver => BuildContributionValue(ContributionArea.Surface);
    public string MoodDriver => BuildContributionValue(ContributionArea.Mood);
    public string PromptPreview { get => _promptPreview; private set => SetProperty(ref _promptPreview, value); }
    public string NegativePromptPreview { get => _negativePromptPreview; private set => SetProperty(ref _negativePromptPreview, value); }
    public string PresetName { get => _presetName; set => SetProperty(ref _presetName, value); }
    public string? SelectedPresetName { get => _selectedPresetName; set { if (SetProperty(ref _selectedPresetName, value) && !string.IsNullOrWhiteSpace(value)) PresetName = value; } }
    public string StatusMessage { get => _statusMessage; private set => SetProperty(ref _statusMessage, value); }

    public void RefreshLicenseState()
    {
        _licenseService.Refresh();
        OnPropertyChanged(nameof(IsUnlocked));
        OnPropertyChanged(nameof(IsDemoMode));
        OnPropertyChanged(nameof(ShowDemoModeBanner));
        OnPropertyChanged(nameof(ShowInteractivePromptPreview));
        OnPropertyChanged(nameof(ShowDemoPromptPreview));
        OnPropertyChanged(nameof(VersionButtonText));
        OnPropertyChanged(nameof(DemoModeHeadline));
        OnPropertyChanged(nameof(CopyPromptRemainingText));
        OnPropertyChanged(nameof(DemoModeBody));
        RaiseCopyCommandCanExecuteChanged();
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
                propertyName.StartsWith("Exclude", StringComparison.Ordinal))
            {
                UiEventLog.Write(
                    $"prompt-preview property='{propertyName}' intent='{IntentMode}' text='{FormatPromptPreviewForLog(PromptPreview)}'");
            }
        }

        return changed;
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

            Stylization = 34;
            Realism = 72;
            TextureDepth = 46;
            NarrativeDensity = 44;
            Symbolism = 24;
            SurfaceAge = 18;
            Framing = 48;
            CameraDistance = 46;
            CameraAngle = 46;
            BackgroundComplexity = 50;
            MotionEnergy = 32;
            AtmosphericDepth = 47;
            Chaos = 18;
            Whimsy = 18;
            Tension = 28;
            Awe = 28;
            Temperature = 50;
            LightingIntensity = 50;
            Saturation = 48;
            Contrast = 52;
            FocusDepth = 48;
            ImageCleanliness = 47;
            DetailDensity = 46;
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyProductPhotographyIntentDefaults()
    {
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

            Stylization = 28;
            Realism = 84;
            TextureDepth = 58;
            NarrativeDensity = 16;
            Symbolism = 8;
            AtmosphericDepth = 26;
            SurfaceAge = 6;
            Chaos = 8;
            Framing = 48;
            CameraDistance = 42;
            CameraAngle = 46;
            BackgroundComplexity = 18;
            MotionEnergy = 6;
            FocusDepth = 52;
            ImageCleanliness = 88;
            DetailDensity = 74;
            Whimsy = 4;
            Tension = 8;
            Awe = 18;
            LightingIntensity = 72;
            Saturation = 54;
            Contrast = 58;
            Lighting = "Dramatic studio light";
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyFoodPhotographyIntentDefaults()
    {
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

            Stylization = 18;
            Realism = 92;
            TextureDepth = 68;
            NarrativeDensity = 10;
            Symbolism = 0;
            AtmosphericDepth = 14;
            SurfaceAge = 2;
            Chaos = 6;
            Framing = 48;
            CameraDistance = 34;
            CameraAngle = 42;
            BackgroundComplexity = 14;
            MotionEnergy = 2;
            FocusDepth = 42;
            ImageCleanliness = 92;
            DetailDensity = 80;
            Whimsy = 0;
            Tension = 0;
            Awe = 10;
            LightingIntensity = 66;
            Saturation = 56;
            Contrast = 58;
            Lighting = "Soft daylight";
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyArchitectureArchvizIntentDefaults()
    {
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

            Stylization = 26;
            Realism = 88;
            TextureDepth = 56;
            NarrativeDensity = 14;
            Symbolism = 4;
            AtmosphericDepth = 48;
            SurfaceAge = 6;
            Chaos = 8;
            Framing = 56;
            CameraDistance = 62;
            CameraAngle = 46;
            BackgroundComplexity = 42;
            MotionEnergy = 4;
            FocusDepth = 82;
            ImageCleanliness = 90;
            DetailDensity = 76;
            Whimsy = 2;
            Tension = 4;
            Awe = 24;
            LightingIntensity = 68;
            Saturation = 48;
            Contrast = 54;
            Lighting = "Soft daylight";
        }
        finally
        {
            _isApplyingConfiguration = wasApplyingConfiguration;
        }
    }

    private void ApplyAnimeIntentDefaults()
    {
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

    private StandardLanePanelViewModel BuildStandardLanePanel(string intentName)
    {
        var definition = LaneRegistry.GetByIntentName(intentName)
            ?? throw new InvalidOperationException($"Lane definition for intent '{intentName}' was not found.");

        var laneState = _ordinaryLaneStates.GetOrAddLane(definition.Id);
        var laneStateViewModel = new StandardLaneStateViewModel(
            laneState,
            SetCompatibilityStringProperty,
            SetCompatibilityBoolProperty);

        return new StandardLanePanelViewModel(definition, laneStateViewModel);
    }

    private IReadOnlyDictionary<string, StandardLanePanelViewModel> BuildSharedLanePanels()
    {
        return new Dictionary<string, StandardLanePanelViewModel>(StringComparer.OrdinalIgnoreCase)
        {
            [IntentModeCatalog.CinematicName] = BuildStandardLanePanel(IntentModeCatalog.CinematicName),
            [IntentModeCatalog.ChildrensBookName] = BuildStandardLanePanel(IntentModeCatalog.ChildrensBookName),
            [IntentModeCatalog.ConceptArtName] = BuildStandardLanePanel(IntentModeCatalog.ConceptArtName),
            [IntentModeCatalog.ArchitectureArchvizName] = BuildStandardLanePanel(IntentModeCatalog.ArchitectureArchvizName),
            [IntentModeCatalog.FoodPhotographyName] = BuildStandardLanePanel(IntentModeCatalog.FoodPhotographyName),
            [IntentModeCatalog.PhotographyName] = BuildStandardLanePanel(IntentModeCatalog.PhotographyName),
            [IntentModeCatalog.ProductPhotographyName] = BuildStandardLanePanel(IntentModeCatalog.ProductPhotographyName),
            [IntentModeCatalog.PixelArtName] = BuildStandardLanePanel(IntentModeCatalog.PixelArtName),
            [IntentModeCatalog.ThreeDRenderName] = BuildStandardLanePanel(IntentModeCatalog.ThreeDRenderName),
            [IntentModeCatalog.WatercolorName] = BuildStandardLanePanel(IntentModeCatalog.WatercolorName),
        };
    }

    private static ObservableCollection<string> BuildSubtypeCollection(string intentName, string selectorKey = SharedLaneKeys.StyleSelector)
    {
        return new ObservableCollection<string>(LaneRegistry.GetSubtypeLabels(intentName, selectorKey));
    }

    private void SetCompatibilityStringProperty(string propertyName, string value)
    {
        var property = GetType().GetProperty(propertyName)
            ?? throw new InvalidOperationException($"View-model property '{propertyName}' was not found.");

        property.SetValue(this, value);
    }

    private void SetCompatibilityBoolProperty(string propertyName, bool value)
    {
        var property = GetType().GetProperty(propertyName)
            ?? throw new InvalidOperationException($"View-model property '{propertyName}' was not found.");

        property.SetValue(this, value);
    }

    private void SyncStandardLanePanels()
    {
        SyncStandardLanePanelStates();
        foreach (var panel in _sharedLanePanels.Values)
        {
            panel.SyncFromSource();
        }
    }

    private void SyncStandardLanePanelStates()
    {
        foreach (var panel in _sharedLanePanels.Values)
        {
            panel.ReplaceState(_ordinaryLaneStates.GetOrAddLane(panel.LaneId));
        }
    }

    private bool SetStandardLaneSelectorAndRefresh(ref string field, string value, string laneId, string selectorKey)
    {
        var changed = SetAndRefresh(ref field, value);
        if (changed)
        {
            _ordinaryLaneStates.GetOrAddLane(laneId).SetSelector(selectorKey, value);
        }

        return changed;
    }

    private bool SetStandardLaneModifierAndRefresh(ref bool field, bool value, string laneId, string modifierKey)
    {
        var changed = SetAndRefresh(ref field, value);
        if (changed)
        {
            _ordinaryLaneStates.GetOrAddLane(laneId).SetModifier(modifierKey, value);
        }

        return changed;
    }

    private bool SetAnimeSelectorAndRefresh(ref string field, string value, string selectorKey)
    {
        var laneState = _ordinaryLaneStates.GetOrAddLane(SharedLaneKeys.Anime.LaneId);
        laneState.SetSelector(selectorKey, value);
        return SetAndRefresh(ref field, value);
    }

    private bool SetAnimeModifierAndRefresh(ref bool field, bool value, string modifierKey)
    {
        var laneState = _ordinaryLaneStates.GetOrAddLane(SharedLaneKeys.Anime.LaneId);
        laneState.SetModifier(modifierKey, value);
        return SetAndRefresh(ref field, value);
    }

    private void RegeneratePrompt()
    {
        var result = _promptBuilderService.Build(CaptureConfiguration());
        PromptPreview = result.PositivePrompt;
        NegativePromptPreview = result.NegativePrompt;
        SyncStandardLanePanels();
        RaiseArtistBlendSummaryChanged();
        RaiseArtistPhraseEditorAvailabilityChanged();
        RaiseSliderHelperChanged();
        RaiseExperimentalMacroChanged();
        RaiseCopyCommandCanExecuteChanged();
    }

    private void OpenPrimaryArtistPhraseEditor()
    {
        OpenArtistPhraseEditor(isPrimary: true);
    }

    private void OpenSecondaryArtistPhraseEditor()
    {
        OpenArtistPhraseEditor(isPrimary: false);
    }

    private void OpenArtistPhraseEditor(bool isPrimary)
    {
        if (!TryGetArtistPhraseSlotState(isPrimary, out var slotState))
        {
            StatusMessage = "Select an active artist influence before editing its phrase.";
            return;
        }

        var parts = ArtistPhraseComposer.SplitPhrase(slotState.CurrentPhrase, slotState.DisplayName);
        _isEditingPrimaryArtistPhrase = isPrimary;
        OnPropertyChanged(nameof(ArtistPhraseEditorTitle));

        ArtistPhraseEditorPrefix = parts.Prefix;
        ArtistPhraseEditorArtistName = slotState.DisplayName;
        ArtistPhraseEditorSuffix = parts.Suffix;
        ArtistPhraseEditorGeneratedPhrase = slotState.GeneratedPhrase;
        _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
        _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
        _artistPhraseEditorStructuredSuffixTrailingText = parts.Suffix;
        ArtistPhraseQuickInsertGroups = ArtistPhraseQuickInsertService.BuildGroups(isPrimary, _currentArtistPairLookup);
        IsArtistPhraseEditorOpen = true;
    }

    private void InsertArtistPhraseQuickInsert(object? parameter)
    {
        if (parameter is not ArtistPhraseQuickInsert insert)
        {
            return;
        }

        switch (insert.Target)
        {
            case ArtistPhraseInsertTarget.Prefix:
                ArtistPhraseEditorPrefix = ArtistPhraseComposer.AppendFragment(string.Empty, insert.Fragment, ArtistPhraseInsertTarget.Prefix);
                break;
            case ArtistPhraseInsertTarget.Suffix:
                if (!string.IsNullOrWhiteSpace(insert.RoleStem) && !string.IsNullOrWhiteSpace(insert.DomainLabel))
                {
                    _artistPhraseEditorStructuredSuffixGroups = ArtistPhraseComposer.AddStructuredSuffixInsert(
                        _artistPhraseEditorStructuredSuffixGroups,
                        insert.RoleStem,
                        insert.DomainLabel);
                    _artistPhraseEditorStructuredSuffixLastRendered = ArtistPhraseComposer.RenderStructuredSuffix(
                        _artistPhraseEditorStructuredSuffixGroups,
                        _artistPhraseEditorStructuredSuffixTrailingText);
                    ArtistPhraseEditorSuffix = _artistPhraseEditorStructuredSuffixLastRendered;
                }
                else
                {
                    ArtistPhraseEditorSuffix = ArtistPhraseComposer.AppendFragment(ArtistPhraseEditorSuffix, insert.Fragment, ArtistPhraseInsertTarget.Suffix);
                }
                break;
        }
    }

    private void SaveArtistPhraseEditor()
    {
        var normalizedPreview = ArtistPhraseEditorPreview;
        var useGeneratedPhrase = string.Equals(
            normalizedPreview,
            ArtistPhraseEditorGeneratedPhrase,
            StringComparison.OrdinalIgnoreCase);

        if (_isEditingPrimaryArtistPhrase)
        {
            _primaryArtistPhraseOverride = useGeneratedPhrase
                ? new ArtistPhraseOverride()
                : new ArtistPhraseOverride
                {
                    IsEnabled = true,
                    ArtistName = ArtistPhraseEditorArtistName,
                    Prefix = ArtistPhraseEditorPrefix,
                    Suffix = ArtistPhraseEditorSuffix,
                };
            StatusMessage = useGeneratedPhrase
                ? "Primary artist phrase reset to generated."
                : "Primary artist phrase updated.";
        }
        else
        {
            _secondaryArtistPhraseOverride = useGeneratedPhrase
                ? new ArtistPhraseOverride()
                : new ArtistPhraseOverride
                {
                    IsEnabled = true,
                    ArtistName = ArtistPhraseEditorArtistName,
                    Prefix = ArtistPhraseEditorPrefix,
                    Suffix = ArtistPhraseEditorSuffix,
                };
            StatusMessage = useGeneratedPhrase
                ? "Secondary artist phrase reset to generated."
                : "Secondary artist phrase updated.";
        }

        IsArtistPhraseEditorOpen = false;
        RegeneratePrompt();
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

    private void ResetArtistPhraseEditorToGenerated()
    {
        if (!TryGetArtistPhraseSlotState(_isEditingPrimaryArtistPhrase, out var slotState))
        {
            return;
        }

        var parts = ArtistPhraseComposer.SplitPhrase(slotState.GeneratedPhrase, slotState.DisplayName);
        ArtistPhraseEditorPrefix = parts.Prefix;
        ArtistPhraseEditorArtistName = slotState.DisplayName;
        ArtistPhraseEditorSuffix = parts.Suffix;
        ArtistPhraseEditorGeneratedPhrase = slotState.GeneratedPhrase;
        _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
        _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
        _artistPhraseEditorStructuredSuffixTrailingText = parts.Suffix;
    }

    private void CancelArtistPhraseEditor()
    {
        IsArtistPhraseEditorOpen = false;
        ArtistPhraseQuickInsertGroups = Array.Empty<ArtistPhraseQuickInsertGroup>();
        _artistPhraseEditorStructuredSuffixGroups = Array.Empty<ArtistPhraseSuffixRoleGroup>();
        _artistPhraseEditorStructuredSuffixLastRendered = string.Empty;
        _artistPhraseEditorStructuredSuffixTrailingText = string.Empty;
    }

    private bool TryGetArtistPhraseSlotState(bool isPrimary, out ArtistPhraseSlotState slotState)
    {
        var selectedArtist = isPrimary ? ArtistInfluencePrimary : ArtistInfluenceSecondary;
        var strength = isPrimary ? InfluenceStrengthPrimary : InfluenceStrengthSecondary;
        var phraseOverride = isPrimary ? _primaryArtistPhraseOverride : _secondaryArtistPhraseOverride;

        if (!HasActiveArtist(selectedArtist, strength))
        {
            slotState = new ArtistPhraseSlotState(string.Empty, 0, string.Empty, string.Empty);
            return false;
        }

        var profile = _artistProfileService.GetProfile(selectedArtist);
        var displayName = profile?.Name ?? selectedArtist;
        var generatedPhrase = ArtistPhraseComposer.BuildGeneratedPhrase(displayName, strength, profile is not null, IntentMode);
        var currentPhrase = ArtistPhraseComposer.BuildFinalPhrase(displayName, strength, profile is not null, phraseOverride, IntentMode);

        slotState = new ArtistPhraseSlotState(displayName, strength, generatedPhrase, currentPhrase);
        return true;
    }

    private void HandleArtistSelectionChanged(bool isPrimary)
    {
        if (_isApplyingConfiguration || _suspendArtistOverrideReset)
        {
            return;
        }

        if (isPrimary)
        {
            _primaryArtistPhraseOverride = new ArtistPhraseOverride();
        }
        else
        {
            _secondaryArtistPhraseOverride = new ArtistPhraseOverride();
        }

        if (IsArtistPhraseEditorOpen && _isEditingPrimaryArtistPhrase == isPrimary)
        {
            IsArtistPhraseEditorOpen = false;
            ArtistPhraseQuickInsertGroups = Array.Empty<ArtistPhraseQuickInsertGroup>();
        }

        RaiseArtistPhraseEditorAvailabilityChanged();
    }

    private void RaiseArtistPhraseEditorAvailabilityChanged()
    {
        OnPropertyChanged(nameof(CanEditPrimaryArtistPhrase));
        OnPropertyChanged(nameof(CanEditSecondaryArtistPhrase));
        OnPropertyChanged(nameof(CanSwapArtistInfluences));
        EditPrimaryArtistPhraseCommand.RaiseCanExecuteChanged();
        EditSecondaryArtistPhraseCommand.RaiseCanExecuteChanged();
        SwapArtistInfluencesCommand.RaiseCanExecuteChanged();
    }

    private void UpdateArtistPhraseOverrideTargetNames()
    {
        UpdateArtistPhraseOverrideTargetName(_primaryArtistPhraseOverride, ArtistInfluencePrimary);
        UpdateArtistPhraseOverrideTargetName(_secondaryArtistPhraseOverride, ArtistInfluenceSecondary);
    }

    private void UpdateArtistPhraseOverrideTargetName(ArtistPhraseOverride phraseOverride, string selectedArtist)
    {
        if (phraseOverride is null || !phraseOverride.IsEnabled)
        {
            return;
        }

        if (!HasSelectedArtistValue(selectedArtist))
        {
            phraseOverride.IsEnabled = false;
            phraseOverride.ArtistName = string.Empty;
            phraseOverride.Prefix = string.Empty;
            phraseOverride.Suffix = string.Empty;
            return;
        }

        var profile = _artistProfileService.GetProfile(selectedArtist);
        phraseOverride.ArtistName = profile?.Name ?? selectedArtist;
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
        var configuration = new PromptConfiguration
        {
        IntentMode = NormalizeIntentMode(IntentMode),
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
        ArchitectureArchvizViewMode = ArchitectureArchvizViewMode,
        ArchitectureArchvizHumanScaleCues = ArchitectureArchvizHumanScaleCues,
        ArchitectureArchvizLandscapeEmphasis = ArchitectureArchvizLandscapeEmphasis,
        ArchitectureArchvizFurnishingEmphasis = ArchitectureArchvizFurnishingEmphasis,
        ArchitectureArchvizWarmInteriorGlow = ArchitectureArchvizWarmInteriorGlow,
        ArchitectureArchvizReflectiveSurfaceAccents = ArchitectureArchvizReflectiveSurfaceAccents,
        ArchitectureArchvizAmenityFocus = ArchitectureArchvizAmenityFocus,
        ArtistInfluencePrimary = ArtistInfluencePrimary,
        InfluenceStrengthPrimary = InfluenceStrengthPrimary,
        PrimaryArtistPhraseOverride = _primaryArtistPhraseOverride.Clone(),
        ArtistInfluenceSecondary = ArtistInfluenceSecondary,
        InfluenceStrengthSecondary = InfluenceStrengthSecondary,
        SecondaryArtistPhraseOverride = _secondaryArtistPhraseOverride.Clone(),
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
        };

        configuration.StandardLaneStates = _ordinaryLaneStates.Clone();
        StandardLaneStateAdapter.ApplyToConfiguration(configuration, configuration.StandardLaneStates);
        UiEventLog.Write(
            $"capture-configuration intent='{configuration.IntentMode}' narrative={configuration.ExcludeNarrativeDensityFromPrompt} symbolism={configuration.ExcludeSymbolismFromPrompt} atmospheric={configuration.ExcludeAtmosphericDepthFromPrompt} chaos={configuration.ExcludeChaosFromPrompt} framing={configuration.ExcludeFramingFromPrompt} cameraDistance={configuration.ExcludeCameraDistanceFromPrompt} cameraAngle={configuration.ExcludeCameraAngleFromPrompt} background={configuration.ExcludeBackgroundComplexityFromPrompt} motion={configuration.ExcludeMotionEnergyFromPrompt} whimsy={configuration.ExcludeWhimsyFromPrompt} tension={configuration.ExcludeTensionFromPrompt} awe={configuration.ExcludeAweFromPrompt} temperature={configuration.ExcludeTemperatureFromPrompt} lightingIntensity={configuration.ExcludeLightingIntensityFromPrompt} stylization={configuration.ExcludeStylizationFromPrompt} realism={configuration.ExcludeRealismFromPrompt} textureDepth={configuration.ExcludeTextureDepthFromPrompt} surfaceAge={configuration.ExcludeSurfaceAgeFromPrompt} focusDepth={configuration.ExcludeFocusDepthFromPrompt} imageCleanliness={configuration.ExcludeImageCleanlinessFromPrompt} detailDensity={configuration.ExcludeDetailDensityFromPrompt} saturation={configuration.ExcludeSaturationFromPrompt} contrast={configuration.ExcludeContrastFromPrompt}");
        return configuration;
    }

    private void ApplyConfiguration(PromptConfiguration configuration)
    {
        StandardLaneStateAdapter.HydrateConfiguration(configuration);
        _ordinaryLaneStates = configuration.StandardLaneStates.Clone();
        _isApplyingConfiguration = true;
        IntentMode = NormalizeIntentMode(configuration.IntentMode);
        Subject = configuration.Subject;
        Action = configuration.Action;
        Relationship = configuration.Relationship;
        Temperature = configuration.Temperature;
        ExcludeTemperatureFromPrompt = configuration.ExcludeTemperatureFromPrompt;
        LightingIntensity = configuration.LightingIntensity;
        ExcludeLightingIntensityFromPrompt = configuration.ExcludeLightingIntensityFromPrompt;
        Stylization = configuration.Stylization;
        ExcludeStylizationFromPrompt = configuration.ExcludeStylizationFromPrompt;
        Realism = configuration.Realism;
        ExcludeRealismFromPrompt = configuration.ExcludeRealismFromPrompt;
        TextureDepth = configuration.TextureDepth;
        ExcludeTextureDepthFromPrompt = configuration.ExcludeTextureDepthFromPrompt;
        NarrativeDensity = configuration.NarrativeDensity;
        ExcludeNarrativeDensityFromPrompt = configuration.ExcludeNarrativeDensityFromPrompt;
        Symbolism = configuration.Symbolism;
        ExcludeSymbolismFromPrompt = configuration.ExcludeSymbolismFromPrompt;
        AtmosphericDepth = configuration.AtmosphericDepth;
        ExcludeAtmosphericDepthFromPrompt = configuration.ExcludeAtmosphericDepthFromPrompt;
        SurfaceAge = configuration.SurfaceAge;
        ExcludeSurfaceAgeFromPrompt = configuration.ExcludeSurfaceAgeFromPrompt;
        Chaos = configuration.Chaos;
        ExcludeChaosFromPrompt = configuration.ExcludeChaosFromPrompt;
        Framing = configuration.Framing;
        ExcludeFramingFromPrompt = configuration.ExcludeFramingFromPrompt;
        Material = configuration.Material;
        ArtStyle = configuration.ArtStyle;
        AnimeStyle = configuration.AnimeStyle;
        AnimeEra = configuration.AnimeEra;
        AnimeCelShading = configuration.AnimeCelShading;
        AnimeCleanLineArt = configuration.AnimeCleanLineArt;
        AnimeExpressiveEyes = configuration.AnimeExpressiveEyes;
        AnimeDynamicAction = configuration.AnimeDynamicAction;
        AnimeCinematicLighting = configuration.AnimeCinematicLighting;
        AnimeStylizedHair = configuration.AnimeStylizedHair;
        AnimeAtmosphericEffects = configuration.AnimeAtmosphericEffects;
        ChildrensBookStyle = configuration.ChildrensBookStyle;
        ChildrensBookSoftColorPalette = configuration.ChildrensBookSoftColorPalette;
        ChildrensBookTexturedPaper = configuration.ChildrensBookTexturedPaper;
        ChildrensBookInkLinework = configuration.ChildrensBookInkLinework;
        ChildrensBookExpressiveCharacters = configuration.ChildrensBookExpressiveCharacters;
        ChildrensBookMinimalBackground = configuration.ChildrensBookMinimalBackground;
        ChildrensBookDecorativeDetails = configuration.ChildrensBookDecorativeDetails;
        ChildrensBookGentleLighting = configuration.ChildrensBookGentleLighting;
        ComicBookStyle = configuration.ComicBookStyle;
        ComicBookBoldInk = configuration.ComicBookBoldInk;
        ComicBookHalftoneShading = configuration.ComicBookHalftoneShading;
        ComicBookPanelFraming = configuration.ComicBookPanelFraming;
        ComicBookDynamicPoses = configuration.ComicBookDynamicPoses;
        ComicBookSpeedLines = configuration.ComicBookSpeedLines;
        ComicBookHighContrastLighting = configuration.ComicBookHighContrastLighting;
        ComicBookSpeechBubbles = configuration.ComicBookSpeechBubbles;
        CinematicSubtype = configuration.CinematicSubtype;
        CinematicLetterboxedFraming = configuration.CinematicLetterboxedFraming;
        CinematicShallowDepthOfField = configuration.CinematicShallowDepthOfField;
        CinematicPracticalLighting = configuration.CinematicPracticalLighting;
        CinematicAtmosphericHaze = configuration.CinematicAtmosphericHaze;
        CinematicFilmGrain = configuration.CinematicFilmGrain;
        CinematicAnamorphicFlares = configuration.CinematicAnamorphicFlares;
        CinematicDramaticBacklight = configuration.CinematicDramaticBacklight;
        ThreeDRenderSubtype = configuration.ThreeDRenderSubtype;
        ThreeDRenderGlobalIllumination = configuration.ThreeDRenderGlobalIllumination;
        ThreeDRenderVolumetricLighting = configuration.ThreeDRenderVolumetricLighting;
        ThreeDRenderRayTracedReflections = configuration.ThreeDRenderRayTracedReflections;
        ThreeDRenderDepthOfField = configuration.ThreeDRenderDepthOfField;
        ThreeDRenderSubsurfaceScattering = configuration.ThreeDRenderSubsurfaceScattering;
        ThreeDRenderHardSurfacePrecision = configuration.ThreeDRenderHardSurfacePrecision;
        ThreeDRenderStudioBackdrop = configuration.ThreeDRenderStudioBackdrop;
        ConceptArtSubtype = configuration.ConceptArtSubtype;
        ConceptArtDesignCallouts = configuration.ConceptArtDesignCallouts;
        ConceptArtTurnaroundReadability = configuration.ConceptArtTurnaroundReadability;
        ConceptArtMaterialBreakdown = configuration.ConceptArtMaterialBreakdown;
        ConceptArtScaleReference = configuration.ConceptArtScaleReference;
        ConceptArtWorldbuildingAccents = configuration.ConceptArtWorldbuildingAccents;
        ConceptArtProductionNotesFeel = configuration.ConceptArtProductionNotesFeel;
        ConceptArtSilhouetteClarity = configuration.ConceptArtSilhouetteClarity;
        PixelArtSubtype = configuration.PixelArtSubtype;
        PixelArtLimitedPalette = configuration.PixelArtLimitedPalette;
        PixelArtDithering = configuration.PixelArtDithering;
        PixelArtTileableDesign = configuration.PixelArtTileableDesign;
        PixelArtSpriteSheetReadability = configuration.PixelArtSpriteSheetReadability;
        PixelArtCleanOutline = configuration.PixelArtCleanOutline;
        PixelArtSubpixelShading = configuration.PixelArtSubpixelShading;
        PixelArtHudUiFraming = configuration.PixelArtHudUiFraming;
        WatercolorStyle = configuration.WatercolorStyle;
        WatercolorTransparentWashes = configuration.WatercolorTransparentWashes;
        WatercolorSoftBleeds = configuration.WatercolorSoftBleeds;
        WatercolorPaperTexture = configuration.WatercolorPaperTexture;
        WatercolorInkAndWatercolor = configuration.WatercolorInkAndWatercolor;
        WatercolorAtmosphericWash = configuration.WatercolorAtmosphericWash;
        WatercolorGouacheAccents = configuration.WatercolorGouacheAccents;
        PhotographyType = configuration.PhotographyType;
        PhotographyEra = configuration.PhotographyEra;
        PhotographyCandidCapture = configuration.PhotographyCandidCapture;
        PhotographyPosedStagedCapture = configuration.PhotographyPosedStagedCapture;
        PhotographyAvailableLight = configuration.PhotographyAvailableLight;
        PhotographyOnCameraFlash = configuration.PhotographyOnCameraFlash;
        PhotographyEditorialPolish = configuration.PhotographyEditorialPolish;
        PhotographyRawDocumentaryTexture = configuration.PhotographyRawDocumentaryTexture;
        PhotographyEnvironmentalPortraitContext = configuration.PhotographyEnvironmentalPortraitContext;
        PhotographyFilmAnalogCharacter = configuration.PhotographyFilmAnalogCharacter;
        ProductPhotographyShotType = configuration.ProductPhotographyShotType;
        ProductPhotographyWithPackaging = configuration.ProductPhotographyWithPackaging;
        ProductPhotographyPedestalDisplay = configuration.ProductPhotographyPedestalDisplay;
        ProductPhotographyReflectiveSurface = configuration.ProductPhotographyReflectiveSurface;
        ProductPhotographyFloatingPresentation = configuration.ProductPhotographyFloatingPresentation;
        ProductPhotographyScaleCueHand = configuration.ProductPhotographyScaleCueHand;
        ProductPhotographyBrandProps = configuration.ProductPhotographyBrandProps;
        ProductPhotographyGroupedVariants = configuration.ProductPhotographyGroupedVariants;
        FoodPhotographyShotMode = configuration.FoodPhotographyShotMode;
        FoodPhotographyVisibleSteam = configuration.FoodPhotographyVisibleSteam;
        FoodPhotographyGarnishEmphasis = configuration.FoodPhotographyGarnishEmphasis;
        FoodPhotographyUtensilContext = configuration.FoodPhotographyUtensilContext;
        FoodPhotographyHandServiceCue = configuration.FoodPhotographyHandServiceCue;
        FoodPhotographyIngredientScatter = configuration.FoodPhotographyIngredientScatter;
        FoodPhotographyCondensationEmphasis = configuration.FoodPhotographyCondensationEmphasis;
        ArchitectureArchvizViewMode = configuration.ArchitectureArchvizViewMode;
        ArchitectureArchvizHumanScaleCues = configuration.ArchitectureArchvizHumanScaleCues;
        ArchitectureArchvizLandscapeEmphasis = configuration.ArchitectureArchvizLandscapeEmphasis;
        ArchitectureArchvizFurnishingEmphasis = configuration.ArchitectureArchvizFurnishingEmphasis;
        ArchitectureArchvizWarmInteriorGlow = configuration.ArchitectureArchvizWarmInteriorGlow;
        ArchitectureArchvizReflectiveSurfaceAccents = configuration.ArchitectureArchvizReflectiveSurfaceAccents;
        ArchitectureArchvizAmenityFocus = configuration.ArchitectureArchvizAmenityFocus;
        ArtistInfluencePrimary = configuration.ArtistInfluencePrimary;
        InfluenceStrengthPrimary = configuration.InfluenceStrengthPrimary;
        _primaryArtistPhraseOverride = configuration.PrimaryArtistPhraseOverride?.Clone() ?? new ArtistPhraseOverride();
        ArtistInfluenceSecondary = configuration.ArtistInfluenceSecondary;
        InfluenceStrengthSecondary = configuration.InfluenceStrengthSecondary;
        _secondaryArtistPhraseOverride = configuration.SecondaryArtistPhraseOverride?.Clone() ?? new ArtistPhraseOverride();
        CameraDistance = configuration.CameraDistance;
        ExcludeCameraDistanceFromPrompt = configuration.ExcludeCameraDistanceFromPrompt;
        CameraAngle = configuration.CameraAngle;
        ExcludeCameraAngleFromPrompt = configuration.ExcludeCameraAngleFromPrompt;
        BackgroundComplexity = configuration.BackgroundComplexity;
        ExcludeBackgroundComplexityFromPrompt = configuration.ExcludeBackgroundComplexityFromPrompt;
        MotionEnergy = configuration.MotionEnergy;
        ExcludeMotionEnergyFromPrompt = configuration.ExcludeMotionEnergyFromPrompt;
        FocusDepth = configuration.FocusDepth;
        ExcludeFocusDepthFromPrompt = configuration.ExcludeFocusDepthFromPrompt;
        ImageCleanliness = configuration.ImageCleanliness;
        ExcludeImageCleanlinessFromPrompt = configuration.ExcludeImageCleanlinessFromPrompt;
        DetailDensity = configuration.DetailDensity;
        ExcludeDetailDensityFromPrompt = configuration.ExcludeDetailDensityFromPrompt;
        Whimsy = configuration.Whimsy;
        ExcludeWhimsyFromPrompt = configuration.ExcludeWhimsyFromPrompt;
        Tension = configuration.Tension;
        ExcludeTensionFromPrompt = configuration.ExcludeTensionFromPrompt;
        Awe = configuration.Awe;
        ExcludeAweFromPrompt = configuration.ExcludeAweFromPrompt;
        Lighting = configuration.Lighting;
        Saturation = configuration.Saturation;
        ExcludeSaturationFromPrompt = configuration.ExcludeSaturationFromPrompt;
        Contrast = configuration.Contrast;
        ExcludeContrastFromPrompt = configuration.ExcludeContrastFromPrompt;
        AspectRatio = configuration.AspectRatio;
        PrintReady = configuration.PrintReady;
        TransparentBackground = configuration.TransparentBackground;
        UseNegativePrompt = configuration.UseNegativePrompt;
        AvoidClutter = configuration.AvoidClutter;
        AvoidMuddyLighting = configuration.AvoidMuddyLighting;
        AvoidDistortedAnatomy = configuration.AvoidDistortedAnatomy;
        AvoidExtraLimbs = configuration.AvoidExtraLimbs;
        AvoidTextArtifacts = configuration.AvoidTextArtifacts;
        AvoidOversaturation = configuration.AvoidOversaturation;
        AvoidFlatComposition = configuration.AvoidFlatComposition;
        AvoidMessyBackground = configuration.AvoidMessyBackground;
        AvoidWeakMaterialDefinition = configuration.AvoidWeakMaterialDefinition;
        AvoidBlurryDetail = configuration.AvoidBlurryDetail;
        VintageBendEasternBlocGdr = configuration.VintageBendEasternBlocGdr;
        VintageBendThrillerUndertone = configuration.VintageBendThrillerUndertone;
        VintageBendInstitutionalAusterity = configuration.VintageBendInstitutionalAusterity;
        VintageBendSurveillanceStateAtmosphere = configuration.VintageBendSurveillanceStateAtmosphere;
        VintageBendPeriodArtifacts = configuration.VintageBendPeriodArtifacts;
        ApplyCompressionConfiguration(configuration);
        SyncStandardLanePanelStates();
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
    private void SavePreset() { var name = PresetName?.Trim(); if (string.IsNullOrWhiteSpace(name)) { StatusMessage = "Enter a preset name before saving."; return; } _presetStorageService.Save(name, CaptureConfiguration()); RefreshPresetNames(name); StatusMessage = $"Preset '{name}' saved."; }
    private void LoadPreset() { var name = SelectedPresetName?.Trim(); if (string.IsNullOrWhiteSpace(name)) { StatusMessage = "Select a preset to load."; return; } ApplyConfiguration(_presetStorageService.Load(name)); PresetName = name; StatusMessage = $"Preset '{name}' loaded."; }
    private void RenamePreset() { var current = SelectedPresetName?.Trim(); var target = PresetName?.Trim(); if (string.IsNullOrWhiteSpace(current)) { StatusMessage = "Select a preset to rename."; return; } if (string.IsNullOrWhiteSpace(target)) { StatusMessage = "Enter the new preset name first."; return; } _presetStorageService.Rename(current, target); RefreshPresetNames(target); StatusMessage = $"Preset renamed to '{target}'."; }
    private void DeletePreset() { var name = SelectedPresetName?.Trim(); if (string.IsNullOrWhiteSpace(name)) { StatusMessage = "Select a preset to delete."; return; } _presetStorageService.Delete(name); RefreshPresetNames(); StatusMessage = $"Preset '{name}' deleted."; }

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
        return !IsDemoMode || RemainingDemoCopies > 0;
    }

    private bool CanCopyNegativePrompt()
    {
        return true;
    }

    private bool CopyExportText(string text, string label)
    {
        if (IsDemoMode && RemainingDemoCopies <= 0)
        {
            StatusMessage = "Demo export limit reached. Preview remains visible.";
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

    private void RefreshPresetNames(string? selected = null)
    {
        PresetNames.Clear();
        foreach (var name in _presetStorageService.GetPresetNames()) PresetNames.Add(name);
        SelectedPresetName = selected ?? PresetNames.FirstOrDefault();
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
            if (string.IsNullOrWhiteSpace(pixelArtPhrase))
            {
                return string.Empty;
            }

            return $"Pixel Art: {pixelArtPhrase}{BuildArtistHelperTint(key)}".Trim();
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
            "Whimsy" => MapBand(value, "serious tone", "subtle whimsy", "playful tone", "strong whimsical energy", "bold comedic whimsy"),
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
            ? SliderLanguageCatalog.ResolveVintageBendGuideText(key)
            : IsAnimeIntent
                ? SliderLanguageCatalog.ResolveAnimeGuideText(key)
                : IsChildrensBookIntent
                    ? SliderLanguageCatalog.ResolveChildrensBookGuideText(key)
                : IsComicBookIntent
                    ? SliderLanguageCatalog.ResolveComicBookGuideText(key)
                : IsCinematicIntent
                    ? SliderLanguageCatalog.ResolveCinematicGuideText(key)
                : IsThreeDRenderIntent
                    ? SliderLanguageCatalog.ResolveThreeDRenderGuideText(key)
                : IsConceptArtIntent
                    ? SliderLanguageCatalog.ResolveConceptArtGuideText(key)
                : IsPixelArtIntent
                    ? SliderLanguageCatalog.ResolvePixelArtGuideText(key)
                : IsWatercolorIntent
                    ? SliderLanguageCatalog.ResolveWatercolorGuideText(key)
                : IsProductPhotographyIntent
                    ? SliderLanguageCatalog.ResolveProductPhotographyGuideText(key, CaptureConfiguration())
                : IsFoodPhotographyIntent
                    ? SliderLanguageCatalog.ResolveFoodPhotographyGuideText(key, CaptureConfiguration())
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
        return IsVintageBendIntent
            ? SliderLanguageCatalog.ResolveVintageBendPhrase(key, value, CaptureConfiguration())
            : IsAnimeIntent
                ? SliderLanguageCatalog.ResolveAnimePhrase(key, value, CaptureConfiguration())
                : IsChildrensBookIntent
                    ? SliderLanguageCatalog.ResolveChildrensBookPhrase(key, value, CaptureConfiguration())
                : IsComicBookIntent
                    ? SliderLanguageCatalog.ResolveComicBookPhrase(key, value, CaptureConfiguration())
                : IsCinematicIntent
                    ? SliderLanguageCatalog.ResolveCinematicPhrase(key, value, CaptureConfiguration())
                : IsThreeDRenderIntent
                    ? SliderLanguageCatalog.ResolveThreeDRenderPhrase(key, value, CaptureConfiguration())
                : IsConceptArtIntent
                    ? SliderLanguageCatalog.ResolveConceptArtPhrase(key, value, CaptureConfiguration())
                : IsPixelArtIntent
                    ? SliderLanguageCatalog.ResolvePixelArtPhrase(key, value, CaptureConfiguration())
                : IsWatercolorIntent
                    ? SliderLanguageCatalog.ResolveWatercolorPhrase(key, value, CaptureConfiguration())
                : IsProductPhotographyIntent
                    ? SliderLanguageCatalog.ResolveProductPhotographyPhrase(key, value, CaptureConfiguration())
                : IsFoodPhotographyIntent
                    ? SliderLanguageCatalog.ResolveFoodPhotographyPhrase(key, value, CaptureConfiguration())
                : IsArchitectureArchvizIntent
                    ? SliderLanguageCatalog.ResolveArchitectureArchvizPhrase(key, value, CaptureConfiguration())
                : IsPhotographyIntent
                    ? SliderLanguageCatalog.ResolvePhotographyPhrase(key, value, CaptureConfiguration())
            : TryGetBandMetadata(key, out var metadata)
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
            var index = Math.Min(Labels.Length - 1, (int)Math.Floor((normalized / 100d) * Labels.Length));
            return Labels[index];
        }
    }

    private sealed record ArtistPhraseSlotState(string DisplayName, int Strength, string GeneratedPhrase, string CurrentPhrase);
    private sealed record ArtistState(string Name, int Strength);
}




