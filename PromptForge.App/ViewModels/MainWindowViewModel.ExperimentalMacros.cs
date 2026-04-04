using System.Collections.ObjectModel;
using System.Diagnostics;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel
{
    private const double ViewConstructionDistanceLeak = 0.5d;
    private const double ViewConstructionDistanceBiasWeight = 0.85d;
    private const double LightAtmosphereSaturationHorizontalGain = 0.45d;
    private const double LightAtmosphereSaturationVerticalGain = 0.22d;
    private const double LightAtmosphereContrastGain = 0.6d;
    private const double MaterialFinishSurfaceAgeGain = 0.75d;
    private const double MaterialFinishTextureDepthGain = 0.8d;
    private const double SceneRichnessBackgroundGain = 0.75d;
    private const double SceneRichnessAtmosphericGain = 0.75d;
    private const double SceneRichnessNarrativeTrigger = 10d;
    private const double SceneRichnessNarrativeGain = 0.55d;
    private const double SceneRichnessNarrativeToneGain = 0.18d;
    private const double EnergyInstabilityMotionGain = 0.7d;
    private const double EnergyInstabilityChaosGain = 0.75d;
    private const double EnergyInstabilityTensionHorizontalGain = 0.28d;
    private const double EnergyInstabilityTensionVerticalGain = 0.38d;
    private const double EnergyInstabilityTensionBonusThreshold = 40d;
    private const double EnergyInstabilityTensionBonusGain = 0.24d;
    private const double EnergyInstabilitySyncPrimaryWeight = 0.82d;
    private const double EnergyInstabilitySyncTensionWeight = 0.18d;
    private const double ToneWhimsyGain = 0.75d;
    private const double ToneAweGain = 0.68d;
    private const double ToneSymbolismGain = 0.63d;

    private bool _isApplyingExperimentalMacroState;
    private bool _isSyncingExperimentalMacros;
#if DEBUG
    private static bool _hasLoggedExperimentalMacroSamples;
#endif
    private string _experimentalControlMode = "Macro / Guided";
    private int _viewConstructionHorizontal = 50;
    private int _viewConstructionVertical = 50;
    private int _viewConstructionDistanceBias = 50;
    private int _lightAtmosphereHorizontal = 50;
    private int _lightAtmosphereVertical = 50;
    private int _materialFinishHorizontal = 50;
    private int _materialFinishVertical = 50;
    private int _sceneRichness = 45;
    private int _energyInstabilityHorizontal = 35;
    private int _energyInstabilityVertical = 30;
    private int _toneHorizontal = 35;
    private int _toneVertical = 40;

    public ObservableCollection<string> ExperimentalControlModes { get; } = new(["Macro / Guided", "Manual / Advanced"]);

    public bool IsExperimentalIntent => IntentModeCatalog.IsExperimental(IntentMode);
    public bool ShowExperimentalMacroControls => IsExperimentalIntent;
    public bool IsExperimentalMacroGuidedMode => IsExperimentalIntent && string.Equals(ExperimentalControlMode, "Macro / Guided", StringComparison.OrdinalIgnoreCase);
    public bool IsExperimentalManualAdvancedMode => IsExperimentalIntent && string.Equals(ExperimentalControlMode, "Manual / Advanced", StringComparison.OrdinalIgnoreCase);

    public string ExperimentalControlMode
    {
        get => _experimentalControlMode;
        set
        {
            if (SetProperty(ref _experimentalControlMode, NormalizeExperimentalControlMode(value)))
            {
                OnPropertyChanged(nameof(IsExperimentalMacroGuidedMode));
                OnPropertyChanged(nameof(IsExperimentalManualAdvancedMode));
                OnPropertyChanged(nameof(ShowManualIntentControls));
                OnPropertyChanged(nameof(ShowLegacyManualCompositionCard));
                OnPropertyChanged(nameof(ShowEmbeddedLaneCompositionCard));
                OnPropertyChanged(nameof(ShowManualNegativeConstraints));
            }
        }
    }

    public int ViewConstructionHorizontal
    {
        get => _viewConstructionHorizontal;
        set => SetMacroAndApply(ref _viewConstructionHorizontal, value, nameof(ViewConstructionHorizontal));
    }

    public int ViewConstructionVertical
    {
        get => _viewConstructionVertical;
        set => SetMacroAndApply(ref _viewConstructionVertical, value, nameof(ViewConstructionVertical));
    }

    public int ViewConstructionDistanceBias
    {
        get => _viewConstructionDistanceBias;
        set => SetMacroAndApply(ref _viewConstructionDistanceBias, value, nameof(ViewConstructionDistanceBias));
    }

    public int LightAtmosphereHorizontal
    {
        get => _lightAtmosphereHorizontal;
        set => SetMacroAndApply(ref _lightAtmosphereHorizontal, value, nameof(LightAtmosphereHorizontal));
    }

    public int LightAtmosphereVertical
    {
        get => _lightAtmosphereVertical;
        set => SetMacroAndApply(ref _lightAtmosphereVertical, value, nameof(LightAtmosphereVertical));
    }

    public int MaterialFinishHorizontal
    {
        get => _materialFinishHorizontal;
        set => SetMacroAndApply(ref _materialFinishHorizontal, value, nameof(MaterialFinishHorizontal));
    }

    public int MaterialFinishVertical
    {
        get => _materialFinishVertical;
        set => SetMacroAndApply(ref _materialFinishVertical, value, nameof(MaterialFinishVertical));
    }

    public int SceneRichness
    {
        get => _sceneRichness;
        set => SetMacroAndApply(ref _sceneRichness, value, nameof(SceneRichness));
    }

    public int EnergyInstabilityHorizontal
    {
        get => _energyInstabilityHorizontal;
        set => SetMacroAndApply(ref _energyInstabilityHorizontal, value, nameof(EnergyInstabilityHorizontal));
    }

    public int EnergyInstabilityVertical
    {
        get => _energyInstabilityVertical;
        set => SetMacroAndApply(ref _energyInstabilityVertical, value, nameof(EnergyInstabilityVertical));
    }

    public int ToneHorizontal
    {
        get => _toneHorizontal;
        set => SetMacroAndApply(ref _toneHorizontal, value, nameof(ToneHorizontal));
    }

    public int ToneVertical
    {
        get => _toneVertical;
        set => SetMacroAndApply(ref _toneVertical, value, nameof(ToneVertical));
    }

    public string ViewConstructionMacroSummary => $"{FramingValueText}, {CameraDistanceValueText.ToLowerInvariant()}, {CameraAngleValueText.ToLowerInvariant()}";
    public string LightAtmosphereMacroSummary => $"{Lighting}, {TemperatureValueText.ToLowerInvariant()}, {LightingIntensityValueText.ToLowerInvariant()}";
    public string MaterialFinishMacroSummary => $"{ImageCleanlinessValueText}, {DetailDensityValueText.ToLowerInvariant()}, {SurfaceAgeValueText.ToLowerInvariant()}";
    public string SceneRichnessMacroSummary => $"{BackgroundComplexityValueText}, {AtmosphericDepthValueText.ToLowerInvariant()}, {NarrativeDensityValueText.ToLowerInvariant()}";
    public string EnergyInstabilityMacroSummary => $"{MotionEnergyValueText}, {ChaosValueText.ToLowerInvariant()}, {TensionValueText.ToLowerInvariant()}";
    public string ToneMacroSummary => $"{WhimsyValueText}, {AweValueText.ToLowerInvariant()}, {SymbolismValueText.ToLowerInvariant()}";

    private bool SetMacroAndApply(ref int field, int value, string propertyName)
    {
        var changed = SetProperty(ref field, Math.Clamp(value, 0, 100), propertyName);
        if (changed && !_isSyncingExperimentalMacros && !_isApplyingConfiguration)
        {
            ApplyExperimentalMacrosToRaw();
        }

        return changed;
    }

    private void SyncExperimentalMacrosFromRaw()
    {
        _isSyncingExperimentalMacros = true;

        SetProperty(ref _viewConstructionHorizontal, Framing, nameof(ViewConstructionHorizontal));
        SetProperty(ref _viewConstructionVertical, CameraAngle, nameof(ViewConstructionVertical));
        SetProperty(ref _viewConstructionDistanceBias, ClampToSlider(CameraDistance - (int)Math.Round((Framing - 50) * ViewConstructionDistanceLeak)), nameof(ViewConstructionDistanceBias));

        SetProperty(ref _lightAtmosphereHorizontal, Temperature, nameof(LightAtmosphereHorizontal));
        SetProperty(ref _lightAtmosphereVertical, LightingIntensity, nameof(LightAtmosphereVertical));

        SetProperty(ref _materialFinishHorizontal, ImageCleanliness, nameof(MaterialFinishHorizontal));
        SetProperty(ref _materialFinishVertical, DetailDensity, nameof(MaterialFinishVertical));

        SetProperty(ref _sceneRichness, ClampToSlider((int)Math.Round((BackgroundComplexity * 0.42d) + (AtmosphericDepth * 0.33d) + (NarrativeDensity * 0.25d))), nameof(SceneRichness));

        var tensionMacro = ClampToSlider((int)Math.Round((Tension - 15) / 0.6d));
        SetProperty(ref _energyInstabilityHorizontal, ClampToSlider((int)Math.Round((((MotionEnergy - 5) / EnergyInstabilityMotionGain) * EnergyInstabilitySyncPrimaryWeight) + (tensionMacro * EnergyInstabilitySyncTensionWeight))), nameof(EnergyInstabilityHorizontal));
        SetProperty(ref _energyInstabilityVertical, ClampToSlider((int)Math.Round((((Chaos - 10) / EnergyInstabilityChaosGain) * EnergyInstabilitySyncPrimaryWeight) + (tensionMacro * EnergyInstabilitySyncTensionWeight))), nameof(EnergyInstabilityVertical));

        SetProperty(ref _toneHorizontal, ClampToSlider((int)Math.Round((Whimsy - 5) / ToneWhimsyGain)), nameof(ToneHorizontal));
        SetProperty(ref _toneVertical, ClampToSlider((int)Math.Round(((Awe - 20) / ToneAweGain + (Symbolism - 10) / ToneSymbolismGain) / 2d)), nameof(ToneVertical));

        _isSyncingExperimentalMacros = false;
        RaiseExperimentalMacroChanged();
    }

    private void ApplyExperimentalMacrosToRaw()
    {
        _isApplyingExperimentalMacroState = true;

        SetRawSlider(ref _framing, ViewConstructionHorizontal, nameof(Framing));
        SetRawSlider(ref _cameraAngle, ViewConstructionVertical, nameof(CameraAngle));
        SetRawSlider(ref _cameraDistance, ClampToSlider((int)Math.Round(50 + ((ViewConstructionHorizontal - 50) * ViewConstructionDistanceLeak) + ((ViewConstructionDistanceBias - 50) * ViewConstructionDistanceBiasWeight))), nameof(CameraDistance));

        SetRawSlider(ref _temperature, LightAtmosphereHorizontal, nameof(Temperature));
        SetRawSlider(ref _lightingIntensity, LightAtmosphereVertical, nameof(LightingIntensity));
        SetRawSlider(ref _saturation, ClampToSlider((int)Math.Round(50 + ((LightAtmosphereHorizontal - 50) * LightAtmosphereSaturationHorizontalGain) + (Math.Abs(LightAtmosphereVertical - 50) * LightAtmosphereSaturationVerticalGain))), nameof(Saturation));
        SetRawSlider(ref _contrast, ClampToSlider((int)Math.Round(50 + ((LightAtmosphereVertical - 50) * LightAtmosphereContrastGain))), nameof(Contrast));

        SetRawSlider(ref _imageCleanliness, MaterialFinishHorizontal, nameof(ImageCleanliness));
        SetRawSlider(ref _surfaceAge, ClampToSlider((int)Math.Round(50 - ((MaterialFinishHorizontal - 50) * MaterialFinishSurfaceAgeGain))), nameof(SurfaceAge));
        SetRawSlider(ref _detailDensity, MaterialFinishVertical, nameof(DetailDensity));
        SetRawSlider(ref _textureDepth, ClampToSlider((int)Math.Round(20 + (MaterialFinishVertical * MaterialFinishTextureDepthGain))), nameof(TextureDepth));

        SetRawSlider(ref _backgroundComplexity, ClampToSlider((int)Math.Round(20 + (SceneRichness * SceneRichnessBackgroundGain))), nameof(BackgroundComplexity));
        SetRawSlider(ref _atmosphericDepth, ClampToSlider((int)Math.Round(20 + (SceneRichness * SceneRichnessAtmosphericGain))), nameof(AtmosphericDepth));
        SetRawSlider(ref _narrativeDensity, ClampToSlider((int)Math.Round(10 + (Math.Max(0, SceneRichness - SceneRichnessNarrativeTrigger) * SceneRichnessNarrativeGain) + (Math.Max(0, ToneVertical - 60) * SceneRichnessNarrativeToneGain))), nameof(NarrativeDensity));

        SetRawSlider(ref _motionEnergy, ClampToSlider((int)Math.Round(5 + (EnergyInstabilityHorizontal * EnergyInstabilityMotionGain))), nameof(MotionEnergy));
        SetRawSlider(ref _chaos, ClampToSlider((int)Math.Round(10 + (EnergyInstabilityVertical * EnergyInstabilityChaosGain))), nameof(Chaos));
        SetRawSlider(ref _tension, ClampToSlider((int)Math.Round(15 + (EnergyInstabilityHorizontal * EnergyInstabilityTensionHorizontalGain) + (EnergyInstabilityVertical * EnergyInstabilityTensionVerticalGain) + (Math.Max(0, ((EnergyInstabilityHorizontal + EnergyInstabilityVertical) / 2d) - EnergyInstabilityTensionBonusThreshold) * EnergyInstabilityTensionBonusGain))), nameof(Tension));

        SetRawSlider(ref _whimsy, ClampToSlider((int)Math.Round(5 + (ToneHorizontal * ToneWhimsyGain))), nameof(Whimsy));
        SetRawSlider(ref _awe, ClampToSlider((int)Math.Round(20 + (ToneVertical * ToneAweGain))), nameof(Awe));
        SetRawSlider(ref _symbolism, ClampToSlider((int)Math.Round(10 + (ToneVertical * ToneSymbolismGain))), nameof(Symbolism));

        _isApplyingExperimentalMacroState = false;
#if DEBUG
        LogExperimentalMacroMappingSamples();
#endif
        ScheduleExperimentalMacroRefresh();
    }

    private void SetRawSlider(ref int field, int value, string propertyName)
    {
        SetProperty(ref field, Math.Clamp(value, 0, 100), propertyName);
    }

    private void RaiseExperimentalMacroChanged()
    {
        OnPropertyChanged(nameof(ViewConstructionMacroSummary));
        OnPropertyChanged(nameof(LightAtmosphereMacroSummary));
        OnPropertyChanged(nameof(MaterialFinishMacroSummary));
        OnPropertyChanged(nameof(SceneRichnessMacroSummary));
        OnPropertyChanged(nameof(EnergyInstabilityMacroSummary));
        OnPropertyChanged(nameof(ToneMacroSummary));
    }

    private static int ClampToSlider(int value)
    {
        return Math.Clamp(value, 0, 100);
    }

    private static string NormalizeExperimentalControlMode(string? value)
    {
        return string.Equals(value, "Manual / Advanced", StringComparison.OrdinalIgnoreCase)
            ? "Manual / Advanced"
            : "Macro / Guided";
    }

#if DEBUG
    private static void LogExperimentalMacroMappingSamples()
    {
        if (_hasLoggedExperimentalMacroSamples)
        {
            return;
        }

        _hasLoggedExperimentalMacroSamples = true;

        LogMacroPoint("View center", 50, 50, (x, y) => $"framing={x}, angle={y}, distance={MapViewConstructionDistance(x, 50)}");
        LogMacroPoint("View left", 0, 50, (x, y) => $"framing={x}, angle={y}, distance={MapViewConstructionDistance(x, 50)}");
        LogMacroPoint("View right", 100, 50, (x, y) => $"framing={x}, angle={y}, distance={MapViewConstructionDistance(x, 50)}");

        LogMacroPoint("Light center", 50, 50, (x, y) => $"temp={x}, intensity={y}, sat={MapLightAtmosphereSaturation(x, y)}, contrast={MapLightAtmosphereContrast(y)}");
        LogMacroPoint("Light top", 50, 100, (x, y) => $"temp={x}, intensity={y}, sat={MapLightAtmosphereSaturation(x, y)}, contrast={MapLightAtmosphereContrast(y)}");
        LogMacroPoint("Light bottom", 50, 0, (x, y) => $"temp={x}, intensity={y}, sat={MapLightAtmosphereSaturation(x, y)}, contrast={MapLightAtmosphereContrast(y)}");
        LogMacroPoint("Light left", 0, 50, (x, y) => $"temp={x}, intensity={y}, sat={MapLightAtmosphereSaturation(x, y)}, contrast={MapLightAtmosphereContrast(y)}");
        LogMacroPoint("Light right", 100, 50, (x, y) => $"temp={x}, intensity={y}, sat={MapLightAtmosphereSaturation(x, y)}, contrast={MapLightAtmosphereContrast(y)}");

        Debug.WriteLine($"[MacroMap] Scene 0 -> bg={MapSceneRichnessBackground(0)}, atm={MapSceneRichnessAtmosphere(0)}, narr={MapSceneRichnessNarrative(0, 40)}");
        Debug.WriteLine($"[MacroMap] Scene 50 -> bg={MapSceneRichnessBackground(50)}, atm={MapSceneRichnessAtmosphere(50)}, narr={MapSceneRichnessNarrative(50, 40)}");
        Debug.WriteLine($"[MacroMap] Scene 100 -> bg={MapSceneRichnessBackground(100)}, atm={MapSceneRichnessAtmosphere(100)}, narr={MapSceneRichnessNarrative(100, 100)}");

        LogMacroPoint("Energy center", 50, 50, (x, y) => $"motion={MapEnergyMotion(x)}, chaos={MapEnergyChaos(y)}, tension={MapEnergyTension(x, y)}");
        LogMacroPoint("Energy top-right", 100, 100, (x, y) => $"motion={MapEnergyMotion(x)}, chaos={MapEnergyChaos(y)}, tension={MapEnergyTension(x, y)}");
        LogMacroPoint("Energy bottom-left", 0, 0, (x, y) => $"motion={MapEnergyMotion(x)}, chaos={MapEnergyChaos(y)}, tension={MapEnergyTension(x, y)}");

        LogMacroPoint("Tone center", 50, 50, (x, y) => $"whimsy={MapToneWhimsy(x)}, awe={MapToneAwe(y)}, symbolism={MapToneSymbolism(y)}");
        LogMacroPoint("Tone top-right", 100, 100, (x, y) => $"whimsy={MapToneWhimsy(x)}, awe={MapToneAwe(y)}, symbolism={MapToneSymbolism(y)}");
        LogMacroPoint("Tone bottom-left", 0, 0, (x, y) => $"whimsy={MapToneWhimsy(x)}, awe={MapToneAwe(y)}, symbolism={MapToneSymbolism(y)}");
    }

    private static void LogMacroPoint(string label, int x, int y, Func<int, int, string> formatter)
    {
        Debug.WriteLine($"[MacroMap] {label} -> {formatter(x, y)}");
    }

    private static int MapViewConstructionDistance(int horizontal, int distanceBias)
        => ClampToSlider((int)Math.Round(50 + ((horizontal - 50) * ViewConstructionDistanceLeak) + ((distanceBias - 50) * ViewConstructionDistanceBiasWeight)));

    private static int MapLightAtmosphereSaturation(int horizontal, int vertical)
        => ClampToSlider((int)Math.Round(50 + ((horizontal - 50) * LightAtmosphereSaturationHorizontalGain) + (Math.Abs(vertical - 50) * LightAtmosphereSaturationVerticalGain)));

    private static int MapLightAtmosphereContrast(int vertical)
        => ClampToSlider((int)Math.Round(50 + ((vertical - 50) * LightAtmosphereContrastGain)));

    private static int MapSceneRichnessBackground(int richness)
        => ClampToSlider((int)Math.Round(20 + (richness * SceneRichnessBackgroundGain)));

    private static int MapSceneRichnessAtmosphere(int richness)
        => ClampToSlider((int)Math.Round(20 + (richness * SceneRichnessAtmosphericGain)));

    private static int MapSceneRichnessNarrative(int richness, int toneVertical)
        => ClampToSlider((int)Math.Round(10 + (Math.Max(0, richness - SceneRichnessNarrativeTrigger) * SceneRichnessNarrativeGain) + (Math.Max(0, toneVertical - 60) * SceneRichnessNarrativeToneGain)));

    private static int MapEnergyMotion(int horizontal)
        => ClampToSlider((int)Math.Round(5 + (horizontal * EnergyInstabilityMotionGain)));

    private static int MapEnergyChaos(int vertical)
        => ClampToSlider((int)Math.Round(10 + (vertical * EnergyInstabilityChaosGain)));

    private static int MapEnergyTension(int horizontal, int vertical)
        => ClampToSlider((int)Math.Round(15 + (horizontal * EnergyInstabilityTensionHorizontalGain) + (vertical * EnergyInstabilityTensionVerticalGain) + (Math.Max(0, ((horizontal + vertical) / 2d) - EnergyInstabilityTensionBonusThreshold) * EnergyInstabilityTensionBonusGain)));

    private static int MapToneWhimsy(int horizontal)
        => ClampToSlider((int)Math.Round(5 + (horizontal * ToneWhimsyGain)));

    private static int MapToneAwe(int vertical)
        => ClampToSlider((int)Math.Round(20 + (vertical * ToneAweGain)));

    private static int MapToneSymbolism(int vertical)
        => ClampToSlider((int)Math.Round(10 + (vertical * ToneSymbolismGain)));
#endif
}
