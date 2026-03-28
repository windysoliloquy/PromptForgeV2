using System.Collections.ObjectModel;
using PromptForge.App.Commands;
using PromptForge.App.Models;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly IPromptBuilderService _promptBuilderService;
    private readonly IPresetStorageService _presetStorageService;
    private readonly IClipboardService _clipboardService;
    private readonly IArtistProfileService _artistProfileService;
    private bool _isApplyingConfiguration;

    private string _subject = string.Empty;
    private string _action = string.Empty;
    private string _relationship = string.Empty;
    private int _stylization = 50;
    private int _realism = 50;
    private int _textureDepth = 35;
    private int _narrativeDensity = 35;
    private int _symbolism = 25;
    private int _atmosphericDepth = 40;
    private int _surfaceAge = 20;
    private int _chaos = 20;
    private string _material = "None";
    private string _artStyle = "None";
    private string _artistInfluencePrimary = "None";
    private int _influenceStrengthPrimary = 45;
    private string _artistInfluenceSecondary = "None";
    private int _influenceStrengthSecondary = 30;
    private string _cameraDistance = "Medium";
    private string _cameraAngle = "Eye level";
    private int _backgroundComplexity = 40;
    private int _motionEnergy = 20;
    private int _whimsy = 20;
    private int _tension = 20;
    private int _awe = 40;
    private string _lighting = "Soft daylight";
    private int _saturation = 55;
    private int _contrast = 55;
    private string _aspectRatio = "1:1";
    private bool _printReady;
    private bool _transparentBackground;
    private bool _useNegativePrompt;
    private string _promptPreview = string.Empty;
    private string _negativePromptPreview = string.Empty;
    private string _presetName = string.Empty;
    private string? _selectedPresetName;
    private string _statusMessage = "Ready.";

    public MainWindowViewModel(IPromptBuilderService promptBuilderService, IPresetStorageService presetStorageService, IClipboardService clipboardService, IArtistProfileService artistProfileService)
    {
        _promptBuilderService = promptBuilderService;
        _presetStorageService = presetStorageService;
        _clipboardService = clipboardService;
        _artistProfileService = artistProfileService;

        Materials = new ObservableCollection<string>(new[] { "None", "Yarn", "Paint", "Glass", "Ink", "Stone", "Metal" });
        ArtStyles = new ObservableCollection<string>(new[] { "None", "Cinematic", "Painterly", "Yarn Relief", "Stained Glass", "Surreal Symbolic", "Concept Art" });
        ArtistInfluences = new ObservableCollection<string>(artistProfileService.GetArtistNames());
        CameraDistances = new ObservableCollection<string>(new[] { "Close-up", "Medium", "Medium-wide", "Wide", "Epic wide" });
        CameraAngles = new ObservableCollection<string>(new[] { "Eye level", "Low angle", "High angle", "Overhead", "Cinematic tilt" });
        Lightings = new ObservableCollection<string>(new[] { "Soft daylight", "Golden hour", "Dramatic studio light", "Overcast", "Moonlit", "Volumetric cinematic light" });
        AspectRatios = new ObservableCollection<string>(new[] { "1:1", "4:5", "16:9", "9:16" });
        PresetNames = new ObservableCollection<string>();

        CopyPromptCommand = new RelayCommand(CopyPrompt);
        CopyNegativePromptCommand = new RelayCommand(CopyNegativePrompt);
        SavePresetCommand = new RelayCommand(SavePreset);
        LoadPresetCommand = new RelayCommand(LoadPreset);
        RenamePresetCommand = new RelayCommand(RenamePreset);
        DeletePresetCommand = new RelayCommand(DeletePreset);
        ResetCommand = new RelayCommand(Reset);

        RefreshPresetNames();
        RegeneratePrompt();
    }

    public ObservableCollection<string> Materials { get; }
    public ObservableCollection<string> ArtStyles { get; }
    public ObservableCollection<string> ArtistInfluences { get; }
    public ObservableCollection<string> CameraDistances { get; }
    public ObservableCollection<string> CameraAngles { get; }
    public ObservableCollection<string> Lightings { get; }
    public ObservableCollection<string> AspectRatios { get; }
    public ObservableCollection<string> PresetNames { get; }

    public RelayCommand CopyPromptCommand { get; }
    public RelayCommand CopyNegativePromptCommand { get; }
    public RelayCommand SavePresetCommand { get; }
    public RelayCommand LoadPresetCommand { get; }
    public RelayCommand RenamePresetCommand { get; }
    public RelayCommand DeletePresetCommand { get; }
    public RelayCommand ResetCommand { get; }

    public string Subject { get => _subject; set => SetAndRefresh(ref _subject, value); }
    public string Action { get => _action; set => SetAndRefresh(ref _action, value); }
    public string Relationship { get => _relationship; set => SetAndRefresh(ref _relationship, value); }
    public int Stylization { get => _stylization; set => SetAndRefresh(ref _stylization, value); }
    public int Realism { get => _realism; set => SetAndRefresh(ref _realism, value); }
    public int TextureDepth { get => _textureDepth; set => SetAndRefresh(ref _textureDepth, value); }
    public int NarrativeDensity { get => _narrativeDensity; set => SetAndRefresh(ref _narrativeDensity, value); }
    public int Symbolism { get => _symbolism; set => SetAndRefresh(ref _symbolism, value); }
    public int AtmosphericDepth { get => _atmosphericDepth; set => SetAndRefresh(ref _atmosphericDepth, value); }
    public int SurfaceAge { get => _surfaceAge; set => SetAndRefresh(ref _surfaceAge, value); }
    public int Chaos { get => _chaos; set => SetAndRefresh(ref _chaos, value); }
    public string Material { get => _material; set => SetAndRefresh(ref _material, value); }
    public string ArtStyle { get => _artStyle; set => SetAndRefresh(ref _artStyle, value); }
    public string ArtistInfluencePrimary { get => _artistInfluencePrimary; set => SetAndRefresh(ref _artistInfluencePrimary, value); }
    public int InfluenceStrengthPrimary { get => _influenceStrengthPrimary; set => SetAndRefresh(ref _influenceStrengthPrimary, value); }
    public string ArtistInfluenceSecondary { get => _artistInfluenceSecondary; set => SetAndRefresh(ref _artistInfluenceSecondary, value); }
    public int InfluenceStrengthSecondary { get => _influenceStrengthSecondary; set => SetAndRefresh(ref _influenceStrengthSecondary, value); }
    public string CameraDistance { get => _cameraDistance; set => SetAndRefresh(ref _cameraDistance, value); }
    public string CameraAngle { get => _cameraAngle; set => SetAndRefresh(ref _cameraAngle, value); }
    public int BackgroundComplexity { get => _backgroundComplexity; set => SetAndRefresh(ref _backgroundComplexity, value); }
    public int MotionEnergy { get => _motionEnergy; set => SetAndRefresh(ref _motionEnergy, value); }
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
    public bool ShowNegativePrompt => UseNegativePrompt;
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

    private bool SetAndRefresh<T>(ref T field, T value)
    {
        var changed = SetProperty(ref field, value);
        if (changed && !_isApplyingConfiguration)
        {
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
    }

    private PromptConfiguration CaptureConfiguration() => new()
    {
        Subject = Subject,
        Action = Action,
        Relationship = Relationship,
        Stylization = Stylization,
        Realism = Realism,
        TextureDepth = TextureDepth,
        NarrativeDensity = NarrativeDensity,
        Symbolism = Symbolism,
        AtmosphericDepth = AtmosphericDepth,
        SurfaceAge = SurfaceAge,
        Chaos = Chaos,
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
    };

    private void ApplyConfiguration(PromptConfiguration configuration)
    {
        _isApplyingConfiguration = true;
        Subject = configuration.Subject;
        Action = configuration.Action;
        Relationship = configuration.Relationship;
        Stylization = configuration.Stylization;
        Realism = configuration.Realism;
        TextureDepth = configuration.TextureDepth;
        NarrativeDensity = configuration.NarrativeDensity;
        Symbolism = configuration.Symbolism;
        AtmosphericDepth = configuration.AtmosphericDepth;
        SurfaceAge = configuration.SurfaceAge;
        Chaos = configuration.Chaos;
        Material = configuration.Material;
        ArtStyle = configuration.ArtStyle;
        ArtistInfluencePrimary = configuration.ArtistInfluencePrimary;
        InfluenceStrengthPrimary = configuration.InfluenceStrengthPrimary;
        ArtistInfluenceSecondary = configuration.ArtistInfluenceSecondary;
        InfluenceStrengthSecondary = configuration.InfluenceStrengthSecondary;
        CameraDistance = configuration.CameraDistance;
        CameraAngle = configuration.CameraAngle;
        BackgroundComplexity = configuration.BackgroundComplexity;
        MotionEnergy = configuration.MotionEnergy;
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
        _isApplyingConfiguration = false;
        RegeneratePrompt();
    }

    private void CopyPrompt() { if (string.IsNullOrWhiteSpace(PromptPreview)) { StatusMessage = "Nothing to copy yet."; return; } _clipboardService.SetText(PromptPreview); StatusMessage = "Main prompt copied."; }
    private void CopyNegativePrompt() { if (!UseNegativePrompt || string.IsNullOrWhiteSpace(NegativePromptPreview)) { StatusMessage = "Negative prompt is disabled."; return; } _clipboardService.SetText(NegativePromptPreview); StatusMessage = "Negative prompt copied."; }
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
            Stylization = 50,
            Realism = 50,
            TextureDepth = 35,
            NarrativeDensity = 35,
            Symbolism = 25,
            AtmosphericDepth = 40,
            SurfaceAge = 20,
            Chaos = 20,
            Material = "None",
            ArtStyle = "None",
            ArtistInfluencePrimary = "None",
            InfluenceStrengthPrimary = 45,
            ArtistInfluenceSecondary = "None",
            InfluenceStrengthSecondary = 30,
            CameraDistance = "Medium",
            CameraAngle = "Eye level",
            BackgroundComplexity = 40,
            MotionEnergy = 20,
            Whimsy = 20,
            Tension = 20,
            Awe = 40,
            Lighting = "Soft daylight",
            Saturation = 55,
            Contrast = 55,
            AspectRatio = "1:1",
        });
        StatusMessage = "Controls reset to defaults.";
    }

    private void RefreshPresetNames(string? selected = null)
    {
        PresetNames.Clear();
        foreach (var name in _presetStorageService.GetPresetNames()) PresetNames.Add(name);
        SelectedPresetName = selected ?? PresetNames.FirstOrDefault();
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

    private static bool HasActiveArtist(string? name, int strength) => strength > 20 && !string.IsNullOrWhiteSpace(name) && !string.Equals(name, "None", StringComparison.OrdinalIgnoreCase);

    private enum ContributionArea
    {
        Composition,
        Palette,
        Surface,
        Mood,
    }

    private sealed record ArtistState(string Name, int Strength);
}
