using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using PromptForge.App.Commands;
using PromptForge.App.Models;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel : ViewModelBase
{
    private readonly IPromptBuilderService _promptBuilderService;
    private readonly IPresetStorageService _presetStorageService;
    private readonly IClipboardService _clipboardService;
    private readonly IArtistProfileService _artistProfileService;
    private readonly IArtistPairGuidanceService _artistPairGuidanceService;
    private readonly IThemeService _themeService;
    private readonly IDemoStateService _demoStateService;
    private readonly ILicenseService _licenseService;
    private readonly DispatcherTimer _experimentalMacroRefreshTimer;
    private bool _isApplyingConfiguration;
    private bool _suspendArtistOverrideReset;

    private string _intentMode = "Custom";
    private string _subject = string.Empty;
    private string _action = string.Empty;
    private string _relationship = string.Empty;
    private int _temperature = 50;
    private int _lightingIntensity = 50;
    private int _stylization = 50;
    private int _realism = 50;
    private int _textureDepth = 35;
    private int _narrativeDensity = 35;
    private int _symbolism = 25;
    private int _atmosphericDepth = 40;
    private int _surfaceAge = 20;
    private int _chaos = 20;
    private int _framing = 50;
    private string _material = "None";
    private string _artStyle = "None";
    private string _artistInfluencePrimary = "None";
    private int _influenceStrengthPrimary = 45;
    private ArtistPhraseOverride _primaryArtistPhraseOverride = new();
    private string _artistInfluenceSecondary = "None";
    private int _influenceStrengthSecondary = 30;
    private ArtistPhraseOverride _secondaryArtistPhraseOverride = new();
    private int _cameraDistance = 50;
    private int _cameraAngle = 50;
    private int _backgroundComplexity = 40;
    private int _motionEnergy = 20;
    private int _focusDepth = 50;
    private int _imageCleanliness = 55;
    private int _detailDensity = 50;
    private int _whimsy = 20;
    private int _tension = 20;
    private int _awe = 40;
    private string _lighting = "Soft daylight";
    private int _saturation = 55;
    private int _contrast = 55;
    private string _aspectRatio = "1:1";
    private bool _printReady;
    private bool _transparentBackground;
    private bool _useNegativePrompt = true;
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

        IntentModes = new ObservableCollection<string>(IntentModeCatalog.Names);
        Materials = new ObservableCollection<string>(new[] { "None", "Yarn", "Paint", "Glass", "Ink", "Stone", "Metal" });
        ArtStyles = new ObservableCollection<string>(new[] { "None", "Cinematic", "Painterly", "Yarn Relief", "Stained Glass", "Surreal Symbolic", "Concept Art" });
        ArtistInfluences = new ObservableCollection<string>(artistProfileService.GetArtistNames());
        Lightings = new ObservableCollection<string>(new[] { "Soft daylight", "Golden hour", "Dramatic studio light", "Overcast", "Moonlit", "Soft glow", "Dusk haze", "Warm directional light", "Volumetric cinematic light" });
        AspectRatios = new ObservableCollection<string>(new[] { "1:1", "4:5", "16:9", "9:16" });
        PresetNames = new ObservableCollection<string>();
        Themes = new ObservableCollection<string>(themeService.AvailableThemeNames);

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
            if (SetAndRefresh(ref _intentMode, NormalizeIntentMode(value)))
            {
                OnPropertyChanged(nameof(IntentModeSelectedIndex));
                OnPropertyChanged(nameof(IsExperimentalIntent));
                OnPropertyChanged(nameof(ShowExperimentalMacroControls));
                OnPropertyChanged(nameof(IsExperimentalMacroGuidedMode));
                OnPropertyChanged(nameof(IsExperimentalManualAdvancedMode));
                OnPropertyChanged(nameof(ShowCustomRandomizeControls));
                OnPropertyChanged(nameof(ShowManualIntentControls));
                OnPropertyChanged(nameof(IsVintageBendIntent));
                OnPropertyChanged(nameof(ShowVintageBendModifierPanel));
                OnPropertyChanged(nameof(IntentModeSummary));
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
    public int LightingIntensity { get => _lightingIntensity; set => SetAndRefresh(ref _lightingIntensity, value); }
    public int Stylization { get => _stylization; set => SetAndRefresh(ref _stylization, value); }
    public int Realism { get => _realism; set => SetAndRefresh(ref _realism, value); }
    public int TextureDepth { get => _textureDepth; set => SetAndRefresh(ref _textureDepth, value); }
    public int NarrativeDensity { get => _narrativeDensity; set => SetAndRefresh(ref _narrativeDensity, value); }
    public int Symbolism { get => _symbolism; set => SetAndRefresh(ref _symbolism, value); }
    public int AtmosphericDepth { get => _atmosphericDepth; set => SetAndRefresh(ref _atmosphericDepth, value); }
    public int SurfaceAge { get => _surfaceAge; set => SetAndRefresh(ref _surfaceAge, value); }
    public int Chaos { get => _chaos; set => SetAndRefresh(ref _chaos, value); }
    public int Framing { get => _framing; set => SetAndRefresh(ref _framing, value); }
    public string Material { get => _material; set => SetAndRefresh(ref _material, value); }
    public string ArtStyle { get => _artStyle; set => SetAndRefresh(ref _artStyle, value); }
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
    public int CameraAngle { get => _cameraAngle; set => SetAndRefresh(ref _cameraAngle, value); }
    public int BackgroundComplexity { get => _backgroundComplexity; set => SetAndRefresh(ref _backgroundComplexity, value); }
    public int MotionEnergy { get => _motionEnergy; set => SetAndRefresh(ref _motionEnergy, value); }
    public int FocusDepth { get => _focusDepth; set => SetAndRefresh(ref _focusDepth, value); }
    public int ImageCleanliness { get => _imageCleanliness; set => SetAndRefresh(ref _imageCleanliness, value); }
    public int DetailDensity { get => _detailDensity; set => SetAndRefresh(ref _detailDensity, value); }
    public int Whimsy { get => _whimsy; set => SetAndRefresh(ref _whimsy, value); }
    public int Tension { get => _tension; set => SetAndRefresh(ref _tension, value); }
    public int Awe { get => _awe; set => SetAndRefresh(ref _awe, value); }
    public string Lighting { get => _lighting; set => SetAndRefresh(ref _lighting, value); }
    public int Saturation { get => _saturation; set => SetAndRefresh(ref _saturation, value); }
    public int Contrast { get => _contrast; set => SetAndRefresh(ref _contrast, value); }
    public string AspectRatio { get => _aspectRatio; set => SetAndRefresh(ref _aspectRatio, value); }
    public bool PrintReady { get => _printReady; set => SetAndRefresh(ref _printReady, value); }
    public bool TransparentBackground { get => _transparentBackground; set => SetAndRefresh(ref _transparentBackground, value); }
    public bool UseNegativePrompt { get => _useNegativePrompt; set { if (SetAndRefresh(ref _useNegativePrompt, value)) OnPropertyChanged(nameof(ShowNegativePrompt)); } }
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
    public bool ShowCustomRandomizeControls => string.Equals(IntentMode, "Custom", StringComparison.OrdinalIgnoreCase) || IntentModeCatalog.IsVintageBend(IntentMode);
    public bool ExcludeArtistSlidersFromRandomize { get => _excludeArtistSlidersFromRandomize; set => SetProperty(ref _excludeArtistSlidersFromRandomize, value); }
    public bool ShowManualIntentControls => string.Equals(IntentMode, "Custom", StringComparison.OrdinalIgnoreCase) || IntentModeCatalog.IsVintageBend(IntentMode) || IsExperimentalManualAdvancedMode;
    public bool IsVintageBendIntent => IntentModeCatalog.IsVintageBend(IntentMode);
    public bool ShowVintageBendModifierPanel => IsVintageBendIntent;
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

    private bool SetAndRefresh<T>(ref T field, T value)
    {
        var changed = SetProperty(ref field, value);
        if (changed && !_isApplyingConfiguration)
        {
            if (!_isApplyingExperimentalMacroState)
            {
                SyncExperimentalMacrosFromRaw();
            }

            RegeneratePrompt();
        }

        return changed;
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

    private void RegeneratePrompt()
    {
        var result = _promptBuilderService.Build(CaptureConfiguration());
        PromptPreview = result.PositivePrompt;
        NegativePromptPreview = result.NegativePrompt;
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

    private PromptConfiguration CaptureConfiguration() => new()
    {
        IntentMode = NormalizeIntentMode(IntentMode),
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
        PrimaryArtistPhraseOverride = _primaryArtistPhraseOverride.Clone(),
        ArtistInfluenceSecondary = ArtistInfluenceSecondary,
        InfluenceStrengthSecondary = InfluenceStrengthSecondary,
        SecondaryArtistPhraseOverride = _secondaryArtistPhraseOverride.Clone(),
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
        VintageBendEasternBlocGdr = VintageBendEasternBlocGdr,
        VintageBendThrillerUndertone = VintageBendThrillerUndertone,
        VintageBendInstitutionalAusterity = VintageBendInstitutionalAusterity,
        VintageBendSurveillanceStateAtmosphere = VintageBendSurveillanceStateAtmosphere,
        VintageBendPeriodArtifacts = VintageBendPeriodArtifacts,
    };

    private void ApplyConfiguration(PromptConfiguration configuration)
    {
        _isApplyingConfiguration = true;
        IntentMode = NormalizeIntentMode(configuration.IntentMode);
        Subject = configuration.Subject;
        Action = configuration.Action;
        Relationship = configuration.Relationship;
        Temperature = configuration.Temperature;
        LightingIntensity = configuration.LightingIntensity;
        Stylization = configuration.Stylization;
        Realism = configuration.Realism;
        TextureDepth = configuration.TextureDepth;
        NarrativeDensity = configuration.NarrativeDensity;
        Symbolism = configuration.Symbolism;
        AtmosphericDepth = configuration.AtmosphericDepth;
        SurfaceAge = configuration.SurfaceAge;
        Chaos = configuration.Chaos;
        Framing = configuration.Framing;
        Material = configuration.Material;
        ArtStyle = configuration.ArtStyle;
        ArtistInfluencePrimary = configuration.ArtistInfluencePrimary;
        InfluenceStrengthPrimary = configuration.InfluenceStrengthPrimary;
        _primaryArtistPhraseOverride = configuration.PrimaryArtistPhraseOverride?.Clone() ?? new ArtistPhraseOverride();
        ArtistInfluenceSecondary = configuration.ArtistInfluenceSecondary;
        InfluenceStrengthSecondary = configuration.InfluenceStrengthSecondary;
        _secondaryArtistPhraseOverride = configuration.SecondaryArtistPhraseOverride?.Clone() ?? new ArtistPhraseOverride();
        CameraDistance = configuration.CameraDistance;
        CameraAngle = configuration.CameraAngle;
        BackgroundComplexity = configuration.BackgroundComplexity;
        MotionEnergy = configuration.MotionEnergy;
        FocusDepth = configuration.FocusDepth;
        ImageCleanliness = configuration.ImageCleanliness;
        DetailDensity = configuration.DetailDensity;
        Whimsy = configuration.Whimsy;
        Tension = configuration.Tension;
        Awe = configuration.Awe;
        Lighting = configuration.Lighting;
        Saturation = configuration.Saturation;
        Contrast = configuration.Contrast;
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
        ApplyConfiguration(new PromptConfiguration
        {
            IntentMode = "Custom",
            Temperature = 50,
            LightingIntensity = 50,
            Stylization = 50,
            Realism = 50,
            TextureDepth = 35,
            NarrativeDensity = 35,
            Symbolism = 25,
            AtmosphericDepth = 40,
            SurfaceAge = 20,
            Chaos = 20,
            Framing = 50,
            Material = "None",
            ArtStyle = "None",
            ArtistInfluencePrimary = "None",
            InfluenceStrengthPrimary = 45,
            ArtistInfluenceSecondary = "None",
            InfluenceStrengthSecondary = 30,
            CameraDistance = 50,
            CameraAngle = 50,
            BackgroundComplexity = 40,
            MotionEnergy = 20,
            FocusDepth = 50,
            ImageCleanliness = 55,
            DetailDensity = 50,
            Whimsy = 20,
            Tension = 20,
            Awe = 40,
            Lighting = "Soft daylight",
            Saturation = 55,
            Contrast = 55,
            AspectRatio = "1:1",
            UseNegativePrompt = true,
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
        });
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
        if (IntentModeCatalog.IsVintageBend(IntentMode))
        {
            return "Vintage Bend language pack is active: candid documentary realism, period-correct color restraint, and disciplined analog texture are now steering prompt language.";
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

        var artPrefix = ArtStyle switch
        {
            "Painterly" => "Painterly: ",
            "Yarn Relief" => "Textile: ",
            "Stained Glass" => "Glasswork: ",
            "Surreal Symbolic" => "Surreal: ",
            "Concept Art" => "Concept: ",
            "Cinematic" => "Cinematic: ",
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
            : TryGetBandMetadata(key, out var metadata)
                ? metadata.GuideText
                : "0  |  100";
    }

    private string GetSliderBandLabel(string key, int value)
    {
        return IsVintageBendIntent
            ? SliderLanguageCatalog.ResolveVintageBendPhrase(key, value, CaptureConfiguration())
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




