using PromptForge.App.Models;
using PromptForge.App.Services;
using PromptForge.App.ViewModels;

var childrensBookLane = LaneRegistry.GetByIntentName(IntentModeCatalog.ChildrensBookName)
    ?? throw new InvalidOperationException("Children's Book lane definition was not found.");
var cinematicLane = LaneRegistry.GetByIntentName(IntentModeCatalog.CinematicName)
    ?? throw new InvalidOperationException("Cinematic lane definition was not found.");
var photographyLane = LaneRegistry.GetByIntentName(IntentModeCatalog.PhotographyName)
    ?? throw new InvalidOperationException("Photography lane definition was not found.");
var productPhotographyLane = LaneRegistry.GetByIntentName(IntentModeCatalog.ProductPhotographyName)
    ?? throw new InvalidOperationException("Product Photography lane definition was not found.");
var foodPhotographyLane = LaneRegistry.GetByIntentName(IntentModeCatalog.FoodPhotographyName)
    ?? throw new InvalidOperationException("Food Photography lane definition was not found.");
var architectureArchvizLane = LaneRegistry.GetByIntentName(IntentModeCatalog.ArchitectureArchvizName)
    ?? throw new InvalidOperationException("Architecture / Archviz lane definition was not found.");
var watercolorLane = LaneRegistry.GetByIntentName(IntentModeCatalog.WatercolorName)
    ?? throw new InvalidOperationException("Watercolor lane definition was not found.");

var failures = new List<string>();

failures.AddRange(LaneRegistryValidator.Validate(LaneRegistry.All, typeof(PromptConfiguration)));
failures.AddRange(StandardLaneBindingValidator.Validate(typeof(MainWindowViewModel), StandardLaneBindingValidator.GetSharedStandardLaneDefinitions()));
failures.AddRange(LaneRegressionHarness.Run(new PromptBuilderService(new ArtistProfileService())));

VerifyValidatorDetection(failures);
VerifyBindingValidatorDetection(failures);
VerifyRegressionDetectorCoverage(failures);
VerifyStandardLaneStateCompatibility(failures, childrensBookLane);
VerifyStandardLaneStateCompatibility(failures, cinematicLane);
VerifyStandardLaneStateCompatibility(failures, photographyLane);
VerifyStandardLaneStateCompatibility(failures, productPhotographyLane);
VerifyStandardLaneStateCompatibility(failures, foodPhotographyLane);
VerifyStandardLaneStateCompatibility(failures, architectureArchvizLane);
VerifyStandardLaneStateCompatibility(failures, watercolorLane);
VerifySharedLanePanelBridge(failures);
VerifySharedLanePresetRoundTrip(failures);
VerifySliderPromptExclusionPilot(failures);

if (failures.Count > 0)
{
    Console.Error.WriteLine("Prompt Forge diagnostics failed:");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"- {failure}");
    }

    Environment.ExitCode = 1;
    return;
}

Console.WriteLine("Prompt Forge diagnostics passed.");

static void VerifyValidatorDetection(ICollection<string> failures)
{
    var invalidLane = CreateMalformedLane();
    var errors = LaneRegistryValidator.Validate([invalidLane], typeof(PromptConfiguration));
    var expectedSignals = new[]
    {
        "exactly one default option",
        "duplicate modifier key",
        "invalid modifier cap",
        "missing or non-string config property",
        "missing or non-bool config property",
        "conditionally hidden but defaults to enabled",
        "must declare a descriptor hint or be an explicit no-op",
    };

    foreach (var signal in expectedSignals)
    {
        if (!errors.Any(error => error.Contains(signal, StringComparison.OrdinalIgnoreCase)))
        {
            failures.Add($"Lane registry validator did not report expected malformed metadata signal '{signal}'.");
        }
    }
}

static void VerifyBindingValidatorDetection(ICollection<string> failures)
{
    var invalidBindingLane = CreateInvalidBindingLane();
    var errors = StandardLaneBindingValidator.Validate(typeof(MainWindowViewModel), [invalidBindingLane]);
    if (!errors.Any())
    {
        failures.Add("Standard lane binding validator did not detect missing view-model properties.");
    }
}

static void VerifyRegressionDetectorCoverage(ICollection<string> failures)
{
    if (!LaneRegressionHarness.DetectRepeatedAnchor(["cinematic film still", "cinematic film still"], "cinematic film still"))
    {
        failures.Add("Regression detector did not flag repeated anchors.");
    }

    if (!LaneRegressionHarness.DetectMissingSubtypeInfluence("pixel art, arcade-style gameplay clarity", "pixel art, arcade-style gameplay clarity"))
    {
        failures.Add("Regression detector did not flag missing subtype influence.");
    }

    if (!LaneRegressionHarness.DetectModifierCapBreakage(2, 7, 4))
    {
        failures.Add("Regression detector did not flag modifier-cap breakage.");
    }

    if (!LaneRegressionHarness.DetectDuplicateFragments("3D render, studio backdrop, 3D render"))
    {
        failures.Add("Regression detector did not flag duplicated prompt fragments.");
    }
}

static void VerifyStandardLaneStateCompatibility(ICollection<string> failures, LaneDefinition lane)
{
    var selectorFixtures = lane.Id switch
    {
        "childrens-book" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["style"] = "whimsical-fantasy",
        },
        "cinematic" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["style"] = "noir-neo-noir",
        },
        "photography" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["type"] = "documentary-street",
            ["era"] = "nineteenth-century-process",
        },
        "product-photography" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["shot-type"] = "hero-studio",
        },
        "food-photography" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["shot-mode"] = "tabletop-spread",
        },
        "architecture-archviz" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["view-mode"] = "interior",
        },
        "watercolor" => new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["style"] = "landscape-watercolor",
        },
        _ => throw new InvalidOperationException($"Compatibility verification does not define a fixture for lane '{lane.Id}'."),
    };

        var sampleModifierKey = string.Equals(lane.Id, "cinematic", StringComparison.Ordinal)
        ? "film-grain"
        : string.Equals(lane.Id, "childrens-book", StringComparison.Ordinal)
            ? "soft-color-palette"
        : string.Equals(lane.Id, "photography", StringComparison.Ordinal)
            ? "candid-capture"
        : string.Equals(lane.Id, "product-photography", StringComparison.Ordinal)
            ? "with-packaging"
        : string.Equals(lane.Id, "food-photography", StringComparison.Ordinal)
            ? "visible-steam"
        : string.Equals(lane.Id, "architecture-archviz", StringComparison.Ordinal)
            ? "human-scale-cues"
        : string.Equals(lane.Id, "watercolor", StringComparison.Ordinal)
            ? "transparent-washes"
            : throw new InvalidOperationException($"Compatibility verification does not define a fixture for lane '{lane.Id}'.");
    var sampleModifier = lane.Modifiers.FirstOrDefault(modifier => string.Equals(modifier.Key, sampleModifierKey, StringComparison.Ordinal))
        ?? throw new InvalidOperationException($"Lane '{lane.Id}' did not declare expected modifier '{sampleModifierKey}' for compatibility verification.");

    var hydrated = new PromptConfiguration
    {
        IntentMode = lane.PrimaryIntentName,
    };

    foreach (var selector in lane.SubtypeSelectors)
    {
        if (!selectorFixtures.TryGetValue(selector.Key, out var selectedValue))
        {
            continue;
        }

        typeof(PromptConfiguration)
            .GetProperty(selector.SelectedValuePropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
            ?.SetValue(hydrated, selectedValue);
    }

    typeof(PromptConfiguration)
        .GetProperty(sampleModifier.StatePropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
        ?.SetValue(hydrated, true);

    StandardLaneStateAdapter.HydrateConfiguration(hydrated);
    if (!hydrated.StandardLaneStates.TryGetLane(lane.Id, out var hydratedLane))
    {
        failures.Add($"Standard lane hydration did not create a '{lane.Id}' lane state.");
        return;
    }

    foreach (var selector in lane.SubtypeSelectors)
    {
        if (!selectorFixtures.TryGetValue(selector.Key, out var expectedSelectorValue))
        {
            continue;
        }

        if (!string.Equals(hydratedLane.GetSelector(selector.Key), expectedSelectorValue, StringComparison.Ordinal))
        {
            failures.Add($"Standard lane hydration did not preserve the '{lane.Id}' selector '{selector.Key}' from legacy fields.");
        }
    }

    if (!hydratedLane.GetModifier(sampleModifier.Key))
    {
        failures.Add($"Standard lane hydration did not preserve modifier '{sampleModifier.Key}' from legacy fields for lane '{lane.Id}'.");
    }

    var applied = new PromptConfiguration
    {
        StandardLaneStates = StandardLaneStateAdapter.CreateDefaultCollection(),
    };

    var appliedLane = applied.StandardLaneStates.GetOrAddLane(lane.Id);
    foreach (var selector in lane.SubtypeSelectors)
    {
        var appliedSelectorValue = selector.Options.Skip(1).FirstOrDefault()?.Key
            ?? selector.Options.FirstOrDefault()?.Key
            ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(appliedSelectorValue))
        {
            appliedLane.SetSelector(selector.Key, appliedSelectorValue);
        }
    }
    appliedLane.SetModifier(sampleModifier.Key, true);
    StandardLaneStateAdapter.ApplyToConfiguration(applied, applied.StandardLaneStates);

    foreach (var selector in lane.SubtypeSelectors)
    {
        var expectedAppliedSelector = selector.Options.Skip(1).FirstOrDefault()?.Key
            ?? selector.Options.FirstOrDefault()?.Key
            ?? string.Empty;
        var appliedCompatibilitySelector = selector.SelectedValuePropertyName switch
        {
            nameof(PromptConfiguration.ChildrensBookStyle) => applied.ChildrensBookStyle,
            nameof(PromptConfiguration.CinematicSubtype) => applied.CinematicSubtype,
            nameof(PromptConfiguration.PhotographyType) => applied.PhotographyType,
            nameof(PromptConfiguration.PhotographyEra) => applied.PhotographyEra,
            nameof(PromptConfiguration.ProductPhotographyShotType) => applied.ProductPhotographyShotType,
            nameof(PromptConfiguration.FoodPhotographyShotMode) => applied.FoodPhotographyShotMode,
            nameof(PromptConfiguration.ArchitectureArchvizViewMode) => applied.ArchitectureArchvizViewMode,
            nameof(PromptConfiguration.WatercolorStyle) => applied.WatercolorStyle,
            _ => string.Empty,
        };

        if (!string.IsNullOrWhiteSpace(expectedAppliedSelector)
            && !string.Equals(appliedCompatibilitySelector, expectedAppliedSelector, StringComparison.Ordinal))
        {
            failures.Add($"Standard lane adapter did not apply the '{lane.Id}' selector '{selector.Key}' from container state back to legacy fields.");
        }
    }

        var appliedCompatibilityModifier = string.Equals(lane.Id, "childrens-book", StringComparison.Ordinal)
        ? applied.ChildrensBookSoftColorPalette
        : string.Equals(lane.Id, "cinematic", StringComparison.Ordinal)
            ? applied.CinematicFilmGrain
        : string.Equals(lane.Id, "photography", StringComparison.Ordinal)
            ? applied.PhotographyCandidCapture
        : string.Equals(lane.Id, "product-photography", StringComparison.Ordinal)
            ? applied.ProductPhotographyWithPackaging
        : string.Equals(lane.Id, "food-photography", StringComparison.Ordinal)
            ? applied.FoodPhotographyVisibleSteam
        : string.Equals(lane.Id, "architecture-archviz", StringComparison.Ordinal)
            ? applied.ArchitectureArchvizHumanScaleCues
        : applied.WatercolorTransparentWashes;
    if (!appliedCompatibilityModifier)
    {
        failures.Add($"Standard lane adapter did not apply modifier '{sampleModifier.Key}' from container state back to legacy fields for lane '{lane.Id}'.");
    }
}

static void VerifySharedLanePanelBridge(ICollection<string> failures)
{
    var viewModel = new MainWindowViewModel(
        new PromptBuilderService(new ArtistProfileService()),
        new InMemoryPresetStorageService(),
        new ClipboardService(),
        new ArtistProfileService(),
        new ArtistPairGuidanceService(),
        new ThemeService(),
        new DemoStateService(),
        new LicenseService());

    var captureMethod = typeof(MainWindowViewModel).GetMethod("CaptureConfiguration", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("CaptureConfiguration method was not found.");
    var sharedIntentNames = new[]
    {
        IntentModeCatalog.ChildrensBookName,
        IntentModeCatalog.CinematicName,
        IntentModeCatalog.PhotographyName,
        IntentModeCatalog.ProductPhotographyName,
        IntentModeCatalog.FoodPhotographyName,
        IntentModeCatalog.ArchitectureArchvizName,
        IntentModeCatalog.ThreeDRenderName,
        IntentModeCatalog.ConceptArtName,
        IntentModeCatalog.PixelArtName,
        IntentModeCatalog.WatercolorName,
    };

    foreach (var intentName in sharedIntentNames)
    {
        var definition = LaneRegistry.GetByIntentName(intentName)
            ?? throw new InvalidOperationException($"Shared lane definition '{intentName}' was not found.");

        viewModel.IntentMode = intentName;
        var panel = viewModel.ActiveStandardLanePanel;
        if (panel is null)
        {
            failures.Add($"Shared lane bridge did not expose an active standard lane panel for '{intentName}'.");
            continue;
        }

        if (!string.Equals(panel.LaneId, definition.Id, StringComparison.OrdinalIgnoreCase))
        {
            failures.Add($"Shared lane bridge exposed panel '{panel.LaneId}' while '{definition.Id}' was expected for intent '{intentName}'.");
        }

        if (panel.SubtypeSelectors.Count != definition.SubtypeSelectors.Count)
        {
            failures.Add($"Shared lane bridge subtype selector count drifted for '{definition.Id}'.");
        }

        if (panel.Modifiers.Count != definition.Modifiers.Count)
        {
            failures.Add($"Shared lane bridge modifier count drifted for '{definition.Id}'.");
        }

        foreach (var selectorVm in panel.SubtypeSelectors)
        {
            var alternateSubtype = selectorVm.Options.Skip(1).FirstOrDefault()?.Key;
            if (!string.IsNullOrWhiteSpace(alternateSubtype))
            {
                selectorVm.SelectedValue = alternateSubtype;
            }
        }

        if (panel.Modifiers.Count > 0)
        {
            panel.Modifiers[0].IsChecked = true;
        }

        var captured = (PromptConfiguration?)captureMethod.Invoke(viewModel, null)
            ?? throw new InvalidOperationException("CaptureConfiguration returned null.");

        if (!captured.StandardLaneStates.TryGetLane(definition.Id, out var capturedLane))
        {
            failures.Add($"Shared lane bridge did not preserve '{definition.Id}' state in the ordinary lane container during capture.");
            continue;
        }

        foreach (var selector in definition.SubtypeSelectors)
        {
            var selectedValue = panel.SubtypeSelectors
                .FirstOrDefault(item => string.Equals(item.Label, selector.Label, StringComparison.Ordinal))
                ?.SelectedValue;

            if (!string.IsNullOrWhiteSpace(selectedValue)
                && !string.Equals(capturedLane.GetSelector(selector.Key), selectedValue, StringComparison.Ordinal))
            {
                failures.Add($"Shared lane bridge did not preserve selector '{selector.Key}' for '{definition.Id}' during capture.");
            }
        }

        if (definition.Modifiers.Count > 0)
        {
            var firstModifier = definition.Modifiers[0];
            if (!capturedLane.GetModifier(firstModifier.Key))
            {
                failures.Add($"Shared lane bridge did not preserve modifier '{firstModifier.Key}' for '{definition.Id}' during capture.");
            }
        }
    }

    viewModel.IntentMode = IntentModeCatalog.ChildrensBookName;
    viewModel.ResetCommand.Execute(null);
    var resetPanel = viewModel.ActiveStandardLanePanel;
    if (resetPanel is not null && resetPanel.SubtypeSelectors.Count > 0)
    {
        var defaultSubtype = resetPanel.SubtypeSelectors[0].Options.FirstOrDefault()?.Key;
        if (!string.IsNullOrWhiteSpace(defaultSubtype) && !string.Equals(resetPanel.SubtypeSelectors[0].SelectedValue, defaultSubtype, StringComparison.Ordinal))
        {
            failures.Add("Shared lane reset did not restore the default Children's Book subtype in the panel view model.");
        }
    }
}

static void VerifySharedLanePresetRoundTrip(ICollection<string> failures)
{
    var presetStorage = new InMemoryPresetStorageService();
    var viewModel = new MainWindowViewModel(
        new PromptBuilderService(new ArtistProfileService()),
        presetStorage,
        new ClipboardService(),
        new ArtistProfileService(),
        new ArtistPairGuidanceService(),
        new ThemeService(),
        new DemoStateService(),
        new LicenseService());

    viewModel.IntentMode = IntentModeCatalog.ChildrensBookName;
    var panel = viewModel.ActiveStandardLanePanel;
    if (panel is null || panel.SubtypeSelectors.Count == 0 || panel.Modifiers.Count == 0)
    {
        failures.Add("Shared lane preset round-trip could not access the Children's Book standard lane panel.");
        return;
    }

    foreach (var selectorVm in panel.SubtypeSelectors)
    {
        var alternateSubtype = selectorVm.Options.Skip(1).FirstOrDefault()?.Key;
        if (!string.IsNullOrWhiteSpace(alternateSubtype))
        {
            selectorVm.SelectedValue = alternateSubtype;
        }
    }

    panel.Modifiers[0].IsChecked = true;
    viewModel.PresetName = "Shared Lane Preset";

    var savePresetMethod = typeof(MainWindowViewModel).GetMethod("SavePreset", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("SavePreset method was not found.");
    var loadPresetMethod = typeof(MainWindowViewModel).GetMethod("LoadPreset", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("LoadPreset method was not found.");

    savePresetMethod.Invoke(viewModel, null);

    viewModel.ResetCommand.Execute(null);
    viewModel.SelectedPresetName = "Shared Lane Preset";
    loadPresetMethod.Invoke(viewModel, null);

    if (!string.Equals(viewModel.IntentMode, IntentModeCatalog.ChildrensBookName, StringComparison.Ordinal))
    {
        failures.Add("Shared lane preset round-trip did not restore the saved intent mode.");
    }

    panel = viewModel.ActiveStandardLanePanel;
    if (panel is null)
    {
        failures.Add("Shared lane preset round-trip did not restore the active standard lane panel.");
        return;
    }

    foreach (var selectorVm in panel.SubtypeSelectors)
    {
        var expected = selectorVm.Options.Skip(1).FirstOrDefault()?.Key
            ?? selectorVm.Options.FirstOrDefault()?.Key;
        if (!string.IsNullOrWhiteSpace(expected)
            && !string.Equals(selectorVm.SelectedValue, expected, StringComparison.Ordinal))
        {
            failures.Add("Shared lane preset round-trip did not restore a shared lane subtype through the standard lane panel.");
            break;
        }
    }

    if (!panel.Modifiers[0].IsChecked)
    {
        failures.Add("Shared lane preset round-trip did not restore the Children's Book modifier through the standard lane panel.");
    }

    viewModel.IntentMode = IntentModeCatalog.ProductPhotographyName;
    panel = viewModel.ActiveStandardLanePanel;
    if (panel is null || panel.SubtypeSelectors.Count == 0 || panel.Modifiers.Count == 0)
    {
        failures.Add("Shared lane preset round-trip could not access the Product Photography standard lane panel.");
        return;
    }

    var shotTypeAlternate = panel.SubtypeSelectors[0].Options.Skip(1).FirstOrDefault()?.Key;
    if (!string.IsNullOrWhiteSpace(shotTypeAlternate))
    {
        panel.SubtypeSelectors[0].SelectedValue = shotTypeAlternate;
    }

    panel.Modifiers[0].IsChecked = true;
    viewModel.PresetName = "Product Photography Preset";
    savePresetMethod.Invoke(viewModel, null);

    viewModel.ResetCommand.Execute(null);
    viewModel.SelectedPresetName = "Product Photography Preset";
    loadPresetMethod.Invoke(viewModel, null);

    if (!string.Equals(viewModel.IntentMode, IntentModeCatalog.ProductPhotographyName, StringComparison.Ordinal))
    {
        failures.Add("Shared lane preset round-trip did not restore the saved Product Photography intent mode.");
    }

    panel = viewModel.ActiveStandardLanePanel;
    if (panel is null)
    {
        failures.Add("Shared lane preset round-trip did not restore the Product Photography standard lane panel.");
        return;
    }

    if (!string.Equals(panel.SubtypeSelectors[0].SelectedValue, "hero-studio", StringComparison.Ordinal))
    {
        failures.Add("Shared lane preset round-trip did not restore the Product Photography shot type through the standard lane panel.");
    }

    if (!panel.Modifiers[0].IsChecked)
    {
        failures.Add("Shared lane preset round-trip did not restore the Product Photography modifier through the standard lane panel.");
    }
}

static void VerifySliderPromptExclusionPilot(ICollection<string> failures)
{
    var builder = new PromptBuilderService(new ArtistProfileService());
    var viewModel = new MainWindowViewModel(
        builder,
        new InMemoryPresetStorageService(),
        new ClipboardService(),
        new ArtistProfileService(),
        new ArtistPairGuidanceService(),
        new ThemeService(),
        new DemoStateService(),
        new LicenseService());

    var captureMethod = typeof(MainWindowViewModel).GetMethod("CaptureConfiguration", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("CaptureConfiguration method was not found.");
    var savePresetMethod = typeof(MainWindowViewModel).GetMethod("SavePreset", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("SavePreset method was not found.");
    var loadPresetMethod = typeof(MainWindowViewModel).GetMethod("LoadPreset", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)
        ?? throw new InvalidOperationException("LoadPreset method was not found.");

    viewModel.IntentMode = "Custom";
    viewModel.Subject = "test subject";
    viewModel.Temperature = 50;
    viewModel.LightingIntensity = 50;
    viewModel.Stylization = 50;
    viewModel.Realism = 50;
    viewModel.TextureDepth = 50;
    viewModel.Framing = 50;
    viewModel.CameraDistance = 50;
    viewModel.CameraAngle = 50;
    viewModel.BackgroundComplexity = 50;
    viewModel.MotionEnergy = 50;
    viewModel.NarrativeDensity = 50;
    viewModel.Symbolism = 50;
    viewModel.AtmosphericDepth = 50;
    viewModel.SurfaceAge = 50;
    viewModel.Chaos = 50;
    viewModel.FocusDepth = 50;
    viewModel.ImageCleanliness = 50;
    viewModel.DetailDensity = 50;
    viewModel.Whimsy = 50;
    viewModel.Tension = 50;
    viewModel.Awe = 50;
    viewModel.Saturation = 50;
    viewModel.Contrast = 50;

    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeTemperatureFromPrompt),
        nameof(PromptConfiguration.Temperature),
        SliderLanguageCatalog.Temperature);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeLightingIntensityFromPrompt),
        nameof(PromptConfiguration.LightingIntensity),
        SliderLanguageCatalog.LightingIntensity);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeStylizationFromPrompt),
        nameof(PromptConfiguration.Stylization),
        SliderLanguageCatalog.Stylization);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeRealismFromPrompt),
        nameof(PromptConfiguration.Realism),
        SliderLanguageCatalog.Realism);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeTextureDepthFromPrompt),
        nameof(PromptConfiguration.TextureDepth),
        SliderLanguageCatalog.TextureDepth);

    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeFramingFromPrompt),
        nameof(PromptConfiguration.Framing),
        SliderLanguageCatalog.Framing);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeCameraDistanceFromPrompt),
        nameof(PromptConfiguration.CameraDistance),
        SliderLanguageCatalog.CameraDistance);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeCameraAngleFromPrompt),
        nameof(PromptConfiguration.CameraAngle),
        SliderLanguageCatalog.CameraAngle);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeBackgroundComplexityFromPrompt),
        nameof(PromptConfiguration.BackgroundComplexity),
        SliderLanguageCatalog.BackgroundComplexity);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeMotionEnergyFromPrompt),
        nameof(PromptConfiguration.MotionEnergy),
        SliderLanguageCatalog.MotionEnergy);

    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeNarrativeDensityFromPrompt),
        nameof(PromptConfiguration.NarrativeDensity),
        SliderLanguageCatalog.NarrativeDensity);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeSymbolismFromPrompt),
        nameof(PromptConfiguration.Symbolism),
        SliderLanguageCatalog.Symbolism);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeAtmosphericDepthFromPrompt),
        nameof(PromptConfiguration.AtmosphericDepth),
        SliderLanguageCatalog.AtmosphericDepth);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeSurfaceAgeFromPrompt),
        nameof(PromptConfiguration.SurfaceAge),
        SliderLanguageCatalog.SurfaceAge);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeChaosFromPrompt),
        nameof(PromptConfiguration.Chaos),
        SliderLanguageCatalog.Chaos);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeFocusDepthFromPrompt),
        nameof(PromptConfiguration.FocusDepth),
        SliderLanguageCatalog.FocusDepth);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeImageCleanlinessFromPrompt),
        nameof(PromptConfiguration.ImageCleanliness),
        SliderLanguageCatalog.ImageCleanliness);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeDetailDensityFromPrompt),
        nameof(PromptConfiguration.DetailDensity),
        SliderLanguageCatalog.DetailDensity);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeWhimsyFromPrompt),
        nameof(PromptConfiguration.Whimsy),
        SliderLanguageCatalog.Whimsy);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeTensionFromPrompt),
        nameof(PromptConfiguration.Tension),
        SliderLanguageCatalog.Tension);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeAweFromPrompt),
        nameof(PromptConfiguration.Awe),
        SliderLanguageCatalog.Awe);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeSaturationFromPrompt),
        nameof(PromptConfiguration.Saturation),
        SliderLanguageCatalog.Saturation);
    AssertSliderPromptGate(
        failures,
        builder,
        viewModel,
        captureMethod,
        nameof(PromptConfiguration.ExcludeContrastFromPrompt),
        nameof(PromptConfiguration.Contrast),
        SliderLanguageCatalog.Contrast);

    viewModel.ExcludeTemperatureFromPrompt = true;
    viewModel.ExcludeLightingIntensityFromPrompt = true;
    viewModel.ExcludeStylizationFromPrompt = true;
    viewModel.ExcludeRealismFromPrompt = true;
    viewModel.ExcludeTextureDepthFromPrompt = true;
    viewModel.ExcludeFramingFromPrompt = true;
    viewModel.ExcludeCameraDistanceFromPrompt = true;
    viewModel.ExcludeCameraAngleFromPrompt = true;
    viewModel.ExcludeBackgroundComplexityFromPrompt = true;
    viewModel.ExcludeMotionEnergyFromPrompt = true;
    viewModel.ExcludeNarrativeDensityFromPrompt = true;
    viewModel.ExcludeSymbolismFromPrompt = true;
    viewModel.ExcludeAtmosphericDepthFromPrompt = true;
    viewModel.ExcludeSurfaceAgeFromPrompt = true;
    viewModel.ExcludeChaosFromPrompt = true;
    viewModel.ExcludeFocusDepthFromPrompt = true;
    viewModel.ExcludeImageCleanlinessFromPrompt = true;
    viewModel.ExcludeDetailDensityFromPrompt = true;
    viewModel.ExcludeWhimsyFromPrompt = true;
    viewModel.ExcludeTensionFromPrompt = true;
    viewModel.ExcludeAweFromPrompt = true;
    viewModel.ExcludeSaturationFromPrompt = true;
    viewModel.ExcludeContrastFromPrompt = true;
    viewModel.NarrativeDensity = 77;
    if (viewModel.NarrativeDensity != 77)
    {
        failures.Add("Pilot slider exclusion prevented NarrativeDensity from remaining editable.");
    }

    viewModel.PresetName = "Slider Exclusion Pilot";
    savePresetMethod.Invoke(viewModel, null);
    viewModel.ResetCommand.Execute(null);

    if (viewModel.ExcludeTemperatureFromPrompt
        || viewModel.ExcludeLightingIntensityFromPrompt
        || viewModel.ExcludeStylizationFromPrompt
        || viewModel.ExcludeRealismFromPrompt
        || viewModel.ExcludeTextureDepthFromPrompt
        || viewModel.ExcludeFramingFromPrompt
        || viewModel.ExcludeCameraDistanceFromPrompt
        || viewModel.ExcludeCameraAngleFromPrompt
        || viewModel.ExcludeBackgroundComplexityFromPrompt
        || viewModel.ExcludeMotionEnergyFromPrompt
        || viewModel.ExcludeNarrativeDensityFromPrompt
        || viewModel.ExcludeSymbolismFromPrompt
        || viewModel.ExcludeAtmosphericDepthFromPrompt
        || viewModel.ExcludeSurfaceAgeFromPrompt
        || viewModel.ExcludeChaosFromPrompt
        || viewModel.ExcludeFocusDepthFromPrompt
        || viewModel.ExcludeImageCleanlinessFromPrompt
        || viewModel.ExcludeDetailDensityFromPrompt
        || viewModel.ExcludeWhimsyFromPrompt
        || viewModel.ExcludeTensionFromPrompt
        || viewModel.ExcludeAweFromPrompt
        || viewModel.ExcludeSaturationFromPrompt
        || viewModel.ExcludeContrastFromPrompt)
    {
        failures.Add("Reset did not restore slider prompt exclusions to false.");
    }

    viewModel.SelectedPresetName = "Slider Exclusion Pilot";
    loadPresetMethod.Invoke(viewModel, null);
    if (!viewModel.ExcludeTemperatureFromPrompt
        || !viewModel.ExcludeLightingIntensityFromPrompt
        || !viewModel.ExcludeStylizationFromPrompt
        || !viewModel.ExcludeRealismFromPrompt
        || !viewModel.ExcludeTextureDepthFromPrompt
        || !viewModel.ExcludeFramingFromPrompt
        || !viewModel.ExcludeCameraDistanceFromPrompt
        || !viewModel.ExcludeCameraAngleFromPrompt
        || !viewModel.ExcludeBackgroundComplexityFromPrompt
        || !viewModel.ExcludeMotionEnergyFromPrompt
        || !viewModel.ExcludeNarrativeDensityFromPrompt
        || !viewModel.ExcludeSymbolismFromPrompt
        || !viewModel.ExcludeAtmosphericDepthFromPrompt
        || !viewModel.ExcludeSurfaceAgeFromPrompt
        || !viewModel.ExcludeChaosFromPrompt
        || !viewModel.ExcludeFocusDepthFromPrompt
        || !viewModel.ExcludeImageCleanlinessFromPrompt
        || !viewModel.ExcludeDetailDensityFromPrompt
        || !viewModel.ExcludeWhimsyFromPrompt
        || !viewModel.ExcludeTensionFromPrompt
        || !viewModel.ExcludeAweFromPrompt
        || !viewModel.ExcludeSaturationFromPrompt
        || !viewModel.ExcludeContrastFromPrompt)
    {
        failures.Add("Preset round-trip did not restore the slider prompt exclusions.");
    }

    viewModel.IntentMode = IntentModeCatalog.CinematicName;
    if (viewModel.ExcludeTemperatureFromPrompt
        || viewModel.ExcludeLightingIntensityFromPrompt
        || viewModel.ExcludeStylizationFromPrompt
        || viewModel.ExcludeRealismFromPrompt
        || viewModel.ExcludeTextureDepthFromPrompt
        || viewModel.ExcludeFramingFromPrompt
        || viewModel.ExcludeCameraDistanceFromPrompt
        || viewModel.ExcludeCameraAngleFromPrompt
        || viewModel.ExcludeBackgroundComplexityFromPrompt
        || viewModel.ExcludeMotionEnergyFromPrompt
        || viewModel.ExcludeNarrativeDensityFromPrompt
        || viewModel.ExcludeSymbolismFromPrompt
        || viewModel.ExcludeAtmosphericDepthFromPrompt
        || viewModel.ExcludeSurfaceAgeFromPrompt
        || viewModel.ExcludeChaosFromPrompt
        || viewModel.ExcludeFocusDepthFromPrompt
        || viewModel.ExcludeImageCleanlinessFromPrompt
        || viewModel.ExcludeDetailDensityFromPrompt
        || viewModel.ExcludeWhimsyFromPrompt
        || viewModel.ExcludeTensionFromPrompt
        || viewModel.ExcludeAweFromPrompt
        || viewModel.ExcludeSaturationFromPrompt
        || viewModel.ExcludeContrastFromPrompt)
    {
        failures.Add("Intent switching did not clear the slider prompt exclusions.");
    }
}

static void AssertSliderPromptGate(
    ICollection<string> failures,
    PromptBuilderService builder,
    MainWindowViewModel viewModel,
    System.Reflection.MethodInfo captureMethod,
    string exclusionPropertyName,
    string valuePropertyName,
    string sliderKey)
{
    typeof(MainWindowViewModel)
        .GetProperty(exclusionPropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
        ?.SetValue(viewModel, false);

    var includedConfiguration = (PromptConfiguration?)captureMethod.Invoke(viewModel, null)
        ?? throw new InvalidOperationException("CaptureConfiguration returned null.");
    var expectedPhrase = SliderLanguageCatalog.ResolvePhrase(sliderKey, (int)(typeof(PromptConfiguration)
        .GetProperty(valuePropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
        ?.GetValue(includedConfiguration) ?? 0), includedConfiguration);
    var includedPrompt = builder.Build(includedConfiguration).PositivePrompt;

    if (string.IsNullOrWhiteSpace(expectedPhrase) || !includedPrompt.Contains(expectedPhrase, StringComparison.Ordinal))
    {
        failures.Add($"Included prompt did not contain the expected phrase for pilot slider '{sliderKey}'.");
    }

    typeof(MainWindowViewModel)
        .GetProperty(exclusionPropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
        ?.SetValue(viewModel, true);

    var excludedConfiguration = (PromptConfiguration?)captureMethod.Invoke(viewModel, null)
        ?? throw new InvalidOperationException("CaptureConfiguration returned null.");
    var excludedPrompt = builder.Build(excludedConfiguration).PositivePrompt;
    if (excludedPrompt.Contains(expectedPhrase, StringComparison.Ordinal))
    {
        failures.Add($"Excluded prompt still contained the phrase for pilot slider '{sliderKey}'.");
    }

    typeof(MainWindowViewModel)
        .GetProperty(exclusionPropertyName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public)
        ?.SetValue(viewModel, false);

    var restoredConfiguration = (PromptConfiguration?)captureMethod.Invoke(viewModel, null)
        ?? throw new InvalidOperationException("CaptureConfiguration returned null.");
    var restoredPrompt = builder.Build(restoredConfiguration).PositivePrompt;
    if (!restoredPrompt.Contains(expectedPhrase, StringComparison.Ordinal))
    {
        failures.Add($"Restoring pilot slider '{sliderKey}' did not bring its phrase back into the prompt.");
    }
}

static LaneDefinition CreateMalformedLane()
{
    return new LaneDefinition(
        Id: "broken-lane",
        DisplayTitle: "Broken Lane",
        IntentNames: ["Broken Lane"],
        Summary: "Broken lane summary",
        AnchorLabel: "broken lane anchor",
        Panel: new LanePanelDefinition("Broken", "Broken", "Broken", "Broken", "Broken", LanePanelLayout.SingleColumn),
        SubtypeSelectors:
        [
            new LaneSubtypeSelectorDefinition(
                "style",
                "Broken Style",
                "MissingStringProperty",
                [
                    new LaneSubtypeOptionDefinition("broken-a", "Broken A"),
                    new LaneSubtypeOptionDefinition("broken-b", "Broken B"),
                ])
        ],
        Modifiers:
        [
            new LaneModifierDefinition("broken-modifier", "Broken Modifier", "MissingBoolProperty", LaneControlType.Checkbox, false, null, "broken-group"),
            new LaneModifierDefinition("broken-modifier", "Hidden Broken Modifier", nameof(PromptConfiguration.CinematicFilmGrain), LaneControlType.Checkbox, true, null, "missing-group", 1, false, "OnlyWhenVisible", null),
        ],
        WeightGroups:
        [
            new LaneWeightGroupDefinition("broken-group", 2, 1, ["ghost-modifier"]),
        ],
        Defaults: new LanePromptDefaults(),
        ModifierCap: -1);
}

static LaneDefinition CreateInvalidBindingLane()
{
    return new LaneDefinition(
        Id: "invalid-binding-lane",
        DisplayTitle: "Invalid Binding Lane",
        IntentNames: ["Invalid Binding Lane"],
        Summary: "Invalid binding lane summary",
        AnchorLabel: "invalid binding anchor",
        Panel: new LanePanelDefinition("Invalid", "Invalid", "Invalid", "Invalid", "Invalid", LanePanelLayout.SingleColumn),
        SubtypeSelectors:
        [
            new LaneSubtypeSelectorDefinition(
                "style",
                "Invalid Style",
                "MissingViewModelSubtype",
                [
                    new LaneSubtypeOptionDefinition("valid-default", "Valid Default", true),
                ])
        ],
        Modifiers:
        [
            new LaneModifierDefinition("invalid-binding-modifier", "Invalid Binding Modifier", "MissingViewModelModifier", LaneControlType.Checkbox, false, "binding descriptor", "binding-group"),
        ],
        WeightGroups:
        [
            new LaneWeightGroupDefinition("binding-group", 1, 1, ["invalid-binding-modifier"]),
        ],
        Defaults: new LanePromptDefaults(),
        ModifierCap: 1);
}

sealed class InMemoryPresetStorageService : IPresetStorageService
{
    private readonly Dictionary<string, PromptConfiguration> _records = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<string> GetPresetNames()
    {
        return _records.Keys.OrderBy(static name => name, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    public void Save(string name, PromptConfiguration configuration)
    {
        _records[name] = configuration.Clone();
    }

    public PromptConfiguration Load(string name)
    {
        return _records.TryGetValue(name, out var configuration)
            ? configuration.Clone()
            : throw new FileNotFoundException("Preset not found.", name);
    }

    public void Rename(string currentName, string newName)
    {
        var configuration = Load(currentName);
        Delete(currentName);
        Save(newName, configuration);
    }

    public void Delete(string name)
    {
        _records.Remove(name);
    }
}
