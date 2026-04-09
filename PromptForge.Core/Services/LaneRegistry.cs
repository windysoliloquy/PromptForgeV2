using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class LaneRegistry
{
    private static readonly IReadOnlyList<LaneDefinition> Definitions =
    [
        CreateVintageBendLane(),
        CreateAnimeLane(),
        CreateChildrensBookLane(),
        CreateComicBookLane(),
        CreateCinematicLane(),
        CreatePhotographyLane(),
        CreateProductPhotographyLane(),
        CreateFoodPhotographyLane(),
        CreateLifestyleAdvertisingPhotographyLane(),
        CreateArchitectureArchvizLane(),
        CreateThreeDRenderLane(),
        CreateConceptArtLane(),
        CreatePixelArtLane(),
        CreateFantasyIllustrationLane(),
        CreateEditorialIllustrationLane(),
        CreateGraphicDesignLane(),
        CreateTattooArtLane(),
        CreateWatercolorLane(),
    ];

    private static readonly IReadOnlyDictionary<string, LaneDefinition> ById = Definitions.ToDictionary(static definition => definition.Id, StringComparer.OrdinalIgnoreCase);
    private static readonly IReadOnlyDictionary<string, LaneDefinition> ByIntentName = BuildByIntentName(Definitions);
    private static readonly IReadOnlyDictionary<string, ILanePolicy> Policies = new Dictionary<string, ILanePolicy>(StringComparer.OrdinalIgnoreCase)
    {
        ["default"] = DefaultLanePolicy.Instance,
        ["comic-book"] = ComicBookLanePolicy.Instance,
        ["vintage-bend"] = VintageBendLanePolicy.Instance,
    };

    static LaneRegistry()
    {
        LaneRegistryValidator.ThrowIfInvalid(Definitions, typeof(PromptConfiguration));
    }

    public static IReadOnlyList<LaneDefinition> All => Definitions;

    public static bool TryGetByIntentName(string? intentName, out LaneDefinition definition)
    {
        if (!string.IsNullOrWhiteSpace(intentName) && ByIntentName.TryGetValue(intentName.Trim(), out definition!))
        {
            return true;
        }

        definition = null!;
        return false;
    }

    public static LaneDefinition? GetByIntentName(string? intentName)
    {
        return TryGetByIntentName(intentName, out var definition) ? definition : null;
    }

    public static ILanePolicy GetPolicy(LaneDefinition definition)
    {
        if (!string.IsNullOrWhiteSpace(definition.PolicyKey) && Policies.TryGetValue(definition.PolicyKey, out var policy))
        {
            return policy;
        }

        return DefaultLanePolicy.Instance;
    }

    public static IReadOnlyList<string> GetSubtypeLabels(string intentName, string selectorKey)
    {
        if (!TryGetByIntentName(intentName, out var lane))
        {
            return Array.Empty<string>();
        }

        var selector = lane.SubtypeSelectors.FirstOrDefault(item => string.Equals(item.Key, selectorKey, StringComparison.OrdinalIgnoreCase));
        return selector is null ? Array.Empty<string>() : selector.Options.Select(static option => option.Label).ToArray();
    }

    public static IReadOnlyList<LaneSubtypeOptionDefinition> GetSubtypeOptions(string intentName, string selectorKey)
    {
        if (!TryGetByIntentName(intentName, out var lane))
        {
            return Array.Empty<LaneSubtypeOptionDefinition>();
        }

        var selector = lane.SubtypeSelectors.FirstOrDefault(item => string.Equals(item.Key, selectorKey, StringComparison.OrdinalIgnoreCase));
        return selector?.Options ?? Array.Empty<LaneSubtypeOptionDefinition>();
    }

    public static string GetDefaultSubtypeLabel(string intentName, string selectorKey, string fallback)
    {
        if (!TryGetByIntentName(intentName, out var lane))
        {
            return fallback;
        }

        var selector = lane.SubtypeSelectors.FirstOrDefault(item => string.Equals(item.Key, selectorKey, StringComparison.OrdinalIgnoreCase));
        return selector?.Options.FirstOrDefault(static option => option.IsDefault)?.Label ?? fallback;
    }

    public static string GetDefaultSubtypeValue(string intentName, string selectorKey, string fallback)
    {
        if (!TryGetByIntentName(intentName, out var lane))
        {
            return fallback;
        }

        var selector = lane.SubtypeSelectors.FirstOrDefault(item => string.Equals(item.Key, selectorKey, StringComparison.OrdinalIgnoreCase));
        return selector?.Options.FirstOrDefault(static option => option.IsDefault)?.Key ?? fallback;
    }

    public static bool TryGetSubtypeDefaultNudges(string intentName, string selectorKey, string selectedValue, out LanePromptDefaults defaults)
    {
        defaults = null!;

        if (!TryGetByIntentName(intentName, out var lane) &&
            !ById.TryGetValue(intentName, out lane!))
        {
            return false;
        }

        var selector = lane.SubtypeSelectors.FirstOrDefault(item => string.Equals(item.Key, selectorKey, StringComparison.OrdinalIgnoreCase));
        var option = selector?.Options.FirstOrDefault(option =>
            string.Equals(option.Key, selectedValue, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(option.Label, selectedValue, StringComparison.OrdinalIgnoreCase));

        if (option?.DefaultNudges is null)
        {
            return false;
        }

        defaults = option.DefaultNudges;
        return true;
    }

    private static IReadOnlyDictionary<string, LaneDefinition> BuildByIntentName(IEnumerable<LaneDefinition> definitions)
    {
        var result = new Dictionary<string, LaneDefinition>(StringComparer.OrdinalIgnoreCase);
        foreach (var definition in definitions)
        {
            foreach (var intentName in definition.IntentNames)
            {
                result.Add(intentName, definition);
            }
        }

        return result;
    }

    private static LaneSubtypeSelectorDefinition Selector(string key, string label, string propertyName, IReadOnlyList<LaneSubtypeOptionDefinition> options)
    {
        return new(key, label, propertyName, options);
    }

    private static LaneSubtypeOptionDefinition Option(string key, string label, bool isDefault = false, LanePromptDefaults? defaultNudges = null, string? supportDescriptorHint = null)
    {
        return new(key, label, isDefault, SupportDescriptorHint: supportDescriptorHint, DefaultNudges: defaultNudges);
    }

    private static LaneModifierDefinition Modifier(string key, string label, string propertyName, string descriptorHint, string weightGroup, int capContribution = 1, bool preserveFromCompression = false, string? triggerRequirement = null)
    {
        return new(key, label, propertyName, LaneControlType.Checkbox, false, descriptorHint, weightGroup, capContribution, preserveFromCompression, null, triggerRequirement);
    }

    private static LaneDefinition CreateVintageBendLane()
    {
        return new(
            Id: "vintage-bend",
            DisplayTitle: "Vintage Bend",
            IntentNames: [IntentModeCatalog.VintageBendName],
            Summary: "Vintage Bend language pack is active: candid documentary realism, period-correct color restraint, and disciplined analog texture are now steering prompt language.",
            AnchorLabel: "vintage documentary image language",
            Panel: new("Vintage Bend Modifiers", "Compact semantic world-state modifiers for the Vintage Bend pack.", "Vintage Bend Modifiers", "Compact semantic world-state modifiers for the Vintage Bend pack.", "World-State Accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors: [],
            Modifiers:
            [
                Modifier("eastern-bloc-gdr", "Eastern Bloc / GDR", nameof(PromptConfiguration.VintageBendEasternBlocGdr), "regional-political context", "world-state"),
                Modifier("thriller-undertone", "Thriller Undertone", nameof(PromptConfiguration.VintageBendThrillerUndertone), "quiet thriller pressure", "world-state"),
                Modifier("institutional-austerity", "Institutional Austerity", nameof(PromptConfiguration.VintageBendInstitutionalAusterity), "austere public-institution mood", "world-state"),
                Modifier("surveillance-state-atmosphere", "Surveillance-State Atmosphere", nameof(PromptConfiguration.VintageBendSurveillanceStateAtmosphere), "state-watchfulness atmosphere", "world-state"),
                Modifier("period-artifacts", "Period Artifacts", nameof(PromptConfiguration.VintageBendPeriodArtifacts), "period-correct documentary artifacts", "world-state"),
                Modifier("urban-civilian", "Urban Civilian", nameof(PromptConfiguration.VintageBendUrbanCivilian), "urban civilian context", "world-state"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("world-state", 3, 4, ["eastern-bloc-gdr", "thriller-undertone", "institutional-austerity", "surveillance-state-atmosphere", "period-artifacts", "urban-civilian"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 34,
                Realism = 62,
                TextureDepth = 42,
                NarrativeDensity = 38,
                Symbolism = 18,
                SurfaceAge = 22,
                Framing = 46,
                CameraDistance = 50,
                CameraAngle = 50,
                BackgroundComplexity = 34,
                MotionEnergy = 26,
                FocusDepth = 44,
                ImageCleanliness = 34,
                DetailDensity = 40,
                AtmosphericDepth = 28,
                Chaos = 20,
                Whimsy = 6,
                Tension = 34,
                Awe = 14,
                Saturation = 30,
                Contrast = 42,
                Temperature = 24,
                LightingIntensity = 42,
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar | LaneBehaviorFlags.RequiresPolicyHook,
            PolicyKey: "vintage-bend");
    }

    private static LaneDefinition CreateAnimeLane()
    {
        return new(
            Id: "anime",
            DisplayTitle: "Anime",
            IntentNames: [IntentModeCatalog.AnimeName],
            Summary: "Anime language pack is active: stylized illustration language, clean silhouette readability, and anime-aware slider phrasing are now steering the prompt.",
            AnchorLabel: "anime illustration language",
            Panel: new("Anime Lane", "Style family and era refine the anime lane without replacing the base intent.", "Anime Modifiers", "Accent injectors only. The lane keeps the strongest few and trims the rest.", "Rendering Accents", LanePanelLayout.SplitColumns),
            SubtypeSelectors:
            [
                Selector("style", "Anime Style", nameof(PromptConfiguration.AnimeStyle),
                [
                    Option("general-anime", "General Anime", isDefault: true),
                    Option("shonen-action", "Shonen Action"),
                    Option("shojo-romance", "Shojo Romance"),
                    Option("seinen-dark", "Seinen Dark"),
                    Option("fantasy-anime", "Fantasy Anime"),
                    Option("mecha-sci-fi-anime", "Mecha / Sci-fi Anime"),
                    Option("slice-of-life", "Slice of Life"),
                ]),
                Selector("era", "Anime Era", nameof(PromptConfiguration.AnimeEra),
                [
                    Option("default-modern", "Default / Modern", isDefault: true),
                    Option("classic-anime", "Classic Anime (1960s–1970s)"),
                    Option("cel-era", "Cel-Era Anime (1980s)"),
                    Option("broadcast-anime", "Broadcast Anime (1990s)"),
                    Option("early-digital", "Early Digital Anime (2000s)"),
                    Option("modern-anime", "Modern Anime (2010s+)"),
                ]),
            ],
            Modifiers:
            [
                Modifier("cel-shading", "Cel Shading", nameof(PromptConfiguration.AnimeCelShading), "cel-shaded finish", "rendering-accents"),
                Modifier("clean-line-art", "Clean Line Art", nameof(PromptConfiguration.AnimeCleanLineArt), "clean line discipline", "rendering-accents"),
                Modifier("expressive-eyes", "Expressive Eyes", nameof(PromptConfiguration.AnimeExpressiveEyes), "expressive eye design", "rendering-accents"),
                Modifier("dynamic-action", "Dynamic Action", nameof(PromptConfiguration.AnimeDynamicAction), "action-forward staging", "rendering-accents"),
                Modifier("cinematic-lighting", "Cinematic Lighting", nameof(PromptConfiguration.AnimeCinematicLighting), "cinematic anime lighting", "rendering-accents"),
                Modifier("stylized-hair", "Stylized Hair", nameof(PromptConfiguration.AnimeStylizedHair), "graphic hair styling", "rendering-accents"),
                Modifier("atmospheric-effects", "Atmospheric Effects", nameof(PromptConfiguration.AnimeAtmosphericEffects), "anime atmosphere effects", "rendering-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("rendering-accents", 3, 4, ["clean-line-art", "cel-shading", "dynamic-action", "cinematic-lighting", "expressive-eyes", "stylized-hair", "atmospheric-effects"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 66,
                Realism = 36,
                TextureDepth = 24,
                NarrativeDensity = 42,
                Symbolism = 22,
                SurfaceAge = 10,
                Framing = 48,
                Chaos = 18,
                MotionEnergy = 48,
                BackgroundComplexity = 38,
                AtmosphericDepth = 40,
                FocusDepth = 50,
                ImageCleanliness = 78,
                DetailDensity = 52,
                Whimsy = 28,
                Tension = 18,
                Awe = 44,
                Temperature = 50,
                LightingIntensity = 58,
                Saturation = 56,
                Contrast = 58,
                Lighting = "Soft glow",
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreatePhotographyLane()
    {
        return new(
            Id: "photography",
            DisplayTitle: "Photography",
            IntentNames: [IntentModeCatalog.PhotographyName, IntentModeCatalog.PhotographicName],
            Summary: "Photography language pack is active: observational, editorial, and documentary photographic phrasing are now steering the prompt.",
            AnchorLabel: "photographic image language",
            Panel: new("Photography Modifiers", "Photography type and era/process refine the pack without replacing the base intent.", "Photography Modifiers", "Photo-native capture and finish accents. The lane keeps the strongest few fragments and trims the rest.", "Capture Accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("type", "Photography Type", nameof(PromptConfiguration.PhotographyType),
                [
                    Option("portrait", "Portrait", isDefault: true),
                    Option("lifestyle-editorial", "Lifestyle / Editorial"),
                    Option("documentary-street", "Documentary / Street"),
                    Option("fine-art-photography", "Fine Art Photography"),
                    Option("commercial-photography", "Commercial Photography"),
                ]),
                Selector("era", "Era / Process", nameof(PromptConfiguration.PhotographyEra),
                [
                    Option("contemporary", "Contemporary (default)", isDefault: true),
                    Option("nineteenth-century-process", "19th-Century Process"),
                ]),
            ],
            Modifiers:
            [
                Modifier("candid-capture", "Candid Capture", nameof(PromptConfiguration.PhotographyCandidCapture), "candid capture", "capture-mode"),
                Modifier("posed-staged-capture", "Posed / Staged Capture", nameof(PromptConfiguration.PhotographyPosedStagedCapture), "posed or staged capture", "capture-mode"),
                Modifier("available-light", "Available Light", nameof(PromptConfiguration.PhotographyAvailableLight), "available-light capture", "capture-mode"),
                Modifier("on-camera-flash", "On-Camera Flash", nameof(PromptConfiguration.PhotographyOnCameraFlash), "direct flash burst", "capture-mode"),
                Modifier("editorial-polish", "Editorial Polish", nameof(PromptConfiguration.PhotographyEditorialPolish), "editorial finish", "finish"),
                Modifier("raw-documentary-texture", "Raw Documentary Texture", nameof(PromptConfiguration.PhotographyRawDocumentaryTexture), "raw documentary texture", "finish"),
                Modifier("environmental-portrait-context", "Environmental Portrait Context", nameof(PromptConfiguration.PhotographyEnvironmentalPortraitContext), "environmental portrait context", "finish"),
                Modifier("film-analog-character", "Film / Analog Character", nameof(PromptConfiguration.PhotographyFilmAnalogCharacter), "film character", "finish"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("capture-mode", 3, 4, ["candid-capture", "posed-staged-capture", "available-light", "on-camera-flash"], "capture approach"),
                new LaneWeightGroupDefinition("finish", 3, 4, ["editorial-polish", "raw-documentary-texture", "environmental-portrait-context", "film-analog-character"], "photographic finish"),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 48,
                Realism = 62,
                TextureDepth = 36,
                NarrativeDensity = 54,
                SurfaceAge = 10,
                Chaos = 12,
                Framing = 54,
                CameraDistance = 52,
                CameraAngle = 50,
                BackgroundComplexity = 46,
                MotionEnergy = 28,
                FocusDepth = 58,
                ImageCleanliness = 58,
                DetailDensity = 52,
                Whimsy = 14,
                Tension = 30,
                Awe = 42,
                AtmosphericDepth = 42,
                Saturation = 50,
                Contrast = 56,
                LightingIntensity = 52,
                Lighting = "Soft daylight",
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateProductPhotographyLane()
    {
        return new(
            Id: "product-photography",
            DisplayTitle: "Product Photography",
            IntentNames: [IntentModeCatalog.ProductPhotographyName],
            Summary: "Commercial merchandise imagery built for clean presentation, sellable staging, material fidelity, and controlled studio-to-editorial product shots.",
            AnchorLabel: "product photography",
            Panel: new("Product Photography", "Use this lane for catalog packshots, premium hero setups, editorial product still lifes, detail-led merchandise shots, and commerce-aware lifestyle placement.", "Presentation Accents", "Optional merchandising accents. Keep these sparse so the prompt stays commercially clean.", "Commercial accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("shot-type", "Shot Type", nameof(PromptConfiguration.ProductPhotographyShotType),
                [
                    Option("packshot", "Packshot", isDefault: true),
                    Option("hero-studio", "Hero Studio"),
                    Option("editorial-still-life", "Editorial Still Life"),
                    Option("macro-detail", "Macro Detail"),
                    Option("lifestyle-placement", "Lifestyle Placement"),
                ]),
            ],
            Modifiers:
            [
                Modifier("with-packaging", "With Packaging", nameof(PromptConfiguration.ProductPhotographyWithPackaging), "packaging in frame", "commerce-context"),
                Modifier("pedestal-display", "Pedestal Display", nameof(PromptConfiguration.ProductPhotographyPedestalDisplay), "pedestal display staging", "display-staging"),
                Modifier("reflective-surface", "Reflective Surface", nameof(PromptConfiguration.ProductPhotographyReflectiveSurface), "reflective display surface", "surface-accents"),
                Modifier("floating-presentation", "Floating Presentation", nameof(PromptConfiguration.ProductPhotographyFloatingPresentation), "floating product presentation", "display-staging"),
                Modifier("scale-cue-hand", "Hand Scale Cue", nameof(PromptConfiguration.ProductPhotographyScaleCueHand), "hand-in-frame scale cue", "commerce-context"),
                Modifier("brand-props", "Brand Props", nameof(PromptConfiguration.ProductPhotographyBrandProps), "brand-matched supporting props", "set-accents"),
                Modifier("grouped-variants", "Grouped Variants", nameof(PromptConfiguration.ProductPhotographyGroupedVariants), "grouped variant arrangement", "commerce-context"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("display-staging", 1, 1, ["pedestal-display", "floating-presentation"], "presentation staging"),
                new LaneWeightGroupDefinition("commerce-context", 1, 2, ["with-packaging", "grouped-variants", "scale-cue-hand"], "commerce context"),
                new LaneWeightGroupDefinition("surface-accents", 1, 1, ["reflective-surface"], "surface accents"),
                new LaneWeightGroupDefinition("set-accents", 1, 1, ["brand-props"], "set accents"),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 28,
                Realism = 84,
                TextureDepth = 58,
                NarrativeDensity = 16,
                Symbolism = 8,
                AtmosphericDepth = 26,
                SurfaceAge = 6,
                Chaos = 8,
                Framing = 48,
                CameraDistance = 42,
                CameraAngle = 46,
                BackgroundComplexity = 18,
                MotionEnergy = 6,
                FocusDepth = 52,
                ImageCleanliness = 88,
                DetailDensity = 74,
                Whimsy = 4,
                Tension = 8,
                Awe = 18,
                LightingIntensity = 72,
                Saturation = 54,
                Contrast = 58,
                Lighting = "Dramatic studio light",
            },
            ModifierCap: 3,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateArchitectureArchvizLane()
    {
        return new(
            Id: "architecture-archviz",
            DisplayTitle: "Architecture / Archviz",
            IntentNames: [IntentModeCatalog.ArchitectureArchvizName],
            Summary: "Built-space visualization for spatial clarity, material realism, development polish, and market-ready architectural imagery.",
            AnchorLabel: "architectural visualization",
            Panel: new("Architecture / Archviz", "Use this lane for exterior renders, interiors, streetscapes, aerial planning views, and polished development-marketing imagery.", "Presentation accents", "Optional spatial and marketing accents. Keep these controlled so the prompt stays architecturally clean.", "Spatial accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("view-mode", "View Mode", nameof(PromptConfiguration.ArchitectureArchvizViewMode),
                [
                    Option("exterior", "Exterior", isDefault: true),
                    Option("interior", "Interior"),
                    Option("streetscape", "Streetscape"),
                    Option("aerial-masterplan", "Aerial Masterplan"),
                    Option("twilight-marketing", "Twilight Marketing"),
                ]),
            ],
            Modifiers:
            [
                Modifier("human-scale-cues", "Human Scale Cues", nameof(PromptConfiguration.ArchitectureArchvizHumanScaleCues), "human scale cues", "occupancy-context"),
                Modifier("landscape-emphasis", "Landscape Emphasis", nameof(PromptConfiguration.ArchitectureArchvizLandscapeEmphasis), "landscape-forward site presentation", "site-accents"),
                Modifier("furnishing-emphasis", "Furnishing Emphasis", nameof(PromptConfiguration.ArchitectureArchvizFurnishingEmphasis), "furnishing-forward interior styling", "interior-accents"),
                Modifier("warm-interior-glow", "Warm Interior Glow", nameof(PromptConfiguration.ArchitectureArchvizWarmInteriorGlow), "warm interior glow", "lighting-accents"),
                Modifier("reflective-surface-accents", "Reflective Surface Accents", nameof(PromptConfiguration.ArchitectureArchvizReflectiveSurfaceAccents), "reflective surface accents", "site-accents"),
                Modifier("amenity-focus", "Amenity Focus", nameof(PromptConfiguration.ArchitectureArchvizAmenityFocus), "amenity-led presentation", "marketing-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("occupancy-context", 1, 1, ["human-scale-cues"]),
                new LaneWeightGroupDefinition("site-accents", 1, 2, ["landscape-emphasis", "reflective-surface-accents"]),
                new LaneWeightGroupDefinition("interior-accents", 1, 1, ["furnishing-emphasis"]),
                new LaneWeightGroupDefinition("lighting-accents", 1, 1, ["warm-interior-glow"]),
                new LaneWeightGroupDefinition("marketing-accents", 1, 1, ["amenity-focus"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 26,
                Realism = 88,
                TextureDepth = 56,
                NarrativeDensity = 14,
                Symbolism = 4,
                AtmosphericDepth = 48,
                SurfaceAge = 6,
                Chaos = 8,
                Framing = 56,
                CameraDistance = 62,
                CameraAngle = 46,
                BackgroundComplexity = 42,
                MotionEnergy = 4,
                FocusDepth = 82,
                ImageCleanliness = 90,
                DetailDensity = 76,
                Whimsy = 2,
                Tension = 4,
                Awe = 24,
                LightingIntensity = 68,
                Saturation = 48,
                Contrast = 54,
                Lighting = "Soft daylight",
            },
            ModifierCap: 3,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateFoodPhotographyLane()
    {
        return new(
            Id: "food-photography",
            DisplayTitle: "Food Photography",
            IntentNames: [IntentModeCatalog.FoodPhotographyName],
            Summary: "Commercial food imagery for appetizing presentation, menu-ready clarity, hospitality polish, and styled service-driven visuals.",
            AnchorLabel: "food photography",
            Panel: new("Food Photography", "Use this lane for plated dishes, styled tabletop spreads, close food detail, beverage service, and menu-ready hospitality imagery.", "Presentation accents", "Optional styling accents. Keep these restrained so the prompt stays appetizing, clear, and commercially clean.", "Food styling accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("shot-mode", "Shot Mode", nameof(PromptConfiguration.FoodPhotographyShotMode),
                [
                    Option("plated-hero", "Plated Hero", isDefault: true),
                    Option("tabletop-spread", "Tabletop Spread"),
                    Option("macro-detail", "Macro Detail"),
                    Option("beverage-service", "Beverage Service"),
                    Option("hospitality-campaign", "Hospitality Campaign"),
                ]),
            ],
            Modifiers:
            [
                Modifier("visible-steam", "Visible Steam", nameof(PromptConfiguration.FoodPhotographyVisibleSteam), "visible steam", "freshness-accents"),
                Modifier("garnish-emphasis", "Garnish Emphasis", nameof(PromptConfiguration.FoodPhotographyGarnishEmphasis), "garnish-forward finishing", "plating-accents"),
                Modifier("utensil-context", "Utensil Context", nameof(PromptConfiguration.FoodPhotographyUtensilContext), "utensil context in frame", "service-context"),
                Modifier("hand-service-cue", "Hand Service Cue", nameof(PromptConfiguration.FoodPhotographyHandServiceCue), "hand-led service cue", "service-context"),
                Modifier("ingredient-scatter", "Ingredient Scatter", nameof(PromptConfiguration.FoodPhotographyIngredientScatter), "ingredient scatter styling", "plating-accents"),
                Modifier("condensation-emphasis", "Condensation Emphasis", nameof(PromptConfiguration.FoodPhotographyCondensationEmphasis), "condensation detail", "freshness-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("freshness-accents", 1, 1, ["visible-steam", "condensation-emphasis"]),
                new LaneWeightGroupDefinition("plating-accents", 1, 2, ["garnish-emphasis", "ingredient-scatter"]),
                new LaneWeightGroupDefinition("service-context", 1, 1, ["hand-service-cue", "utensil-context"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 22,
                Realism = 90,
                TextureDepth = 60,
                NarrativeDensity = 18,
                Symbolism = 2,
                AtmosphericDepth = 24,
                SurfaceAge = 4,
                Chaos = 10,
                Framing = 52,
                CameraDistance = 40,
                CameraAngle = 44,
                BackgroundComplexity = 20,
                MotionEnergy = 6,
                FocusDepth = 46,
                ImageCleanliness = 88,
                DetailDensity = 74,
                Whimsy = 2,
                Tension = 2,
                Awe = 18,
                LightingIntensity = 70,
                Saturation = 58,
                Contrast = 52,
                Lighting = "Soft daylight",
            },
            ModifierCap: 3,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateLifestyleAdvertisingPhotographyLane()
    {
        return new(
            Id: "lifestyle-advertising-photography",
            DisplayTitle: "Lifestyle / Advertising Photography",
            IntentNames: [IntentModeCatalog.LifestyleAdvertisingPhotographyName],
            Summary: "Human-centered commercial photography built for believable aspiration, brand-friendly realism, and campaign-ready social or editorial use.",
            AnchorLabel: "lifestyle advertising photography",
            Panel: new("Lifestyle / Advertising Photography", "Commercial human-centered photography for ads, campaigns, websites, and social brand imagery.", "Campaign Accents", "Adds controlled commercial cues without turning the lane into fashion-editorial sludge.", "Brand / Lifestyle Accents", LanePanelLayout.SplitColumns),
            SubtypeSelectors:
            [
                Selector("shot-mode", "Shot Mode", nameof(PromptConfiguration.LifestyleAdvertisingShotMode),
                [
                    Option("everyday-lifestyle", "Everyday Lifestyle", isDefault: true),
                    Option("premium-brand-campaign", "Premium Brand Campaign"),
                    Option("business-lifestyle", "Business / Work Lifestyle"),
                    Option("home-family-life", "Home / Family Life"),
                    Option("wellness-leisure", "Wellness / Leisure Lifestyle"),
                ]),
            ],
            Modifiers:
            [
                Modifier("natural-interaction", "Natural Interaction", nameof(PromptConfiguration.LifestyleAdvertisingNaturalInteraction), "natural human interaction", "interaction-cues"),
                Modifier("product-in-use", "Product-in-Use Cue", nameof(PromptConfiguration.LifestyleAdvertisingProductInUse), "product-in-use cue", "brand-cues"),
                Modifier("brand-color-accent", "Brand Color Accent", nameof(PromptConfiguration.LifestyleAdvertisingBrandColorAccent), "restrained brand-color accent", "brand-cues"),
                Modifier("prop-context", "Lifestyle Prop Context", nameof(PromptConfiguration.LifestyleAdvertisingPropContext), "lifestyle prop context", "environment-cues"),
                Modifier("sunlit-optimism", "Sunlit Optimism", nameof(PromptConfiguration.LifestyleAdvertisingSunlitOptimism), "sunlit optimism", "environment-cues"),
                Modifier("motion-candidness", "Motion / Candidness", nameof(PromptConfiguration.LifestyleAdvertisingMotionCandidness), "candid motion trace", "interaction-cues"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("interaction-cues", 1, 1, ["natural-interaction", "motion-candidness"]),
                new LaneWeightGroupDefinition("brand-cues", 1, 2, ["product-in-use", "brand-color-accent"]),
                new LaneWeightGroupDefinition("environment-cues", 1, 2, ["prop-context", "sunlit-optimism"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 24,
                Realism = 84,
                TextureDepth = 42,
                NarrativeDensity = 34,
                Symbolism = 6,
                AtmosphericDepth = 18,
                SurfaceAge = 8,
                Chaos = 10,
                Framing = 54,
                CameraDistance = 48,
                CameraAngle = 46,
                BackgroundComplexity = 36,
                MotionEnergy = 26,
                FocusDepth = 28,
                ImageCleanliness = 82,
                DetailDensity = 64,
                Whimsy = 18,
                Tension = 10,
                Awe = 18,
                LightingIntensity = 62,
                Saturation = 54,
                Contrast = 52,
                Lighting = "Soft daylight",
            },
            ModifierCap: 2,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateThreeDRenderLane()
    {
        return new(
            Id: "3d-render",
            DisplayTitle: "3D Render",
            IntentNames: [IntentModeCatalog.ThreeDRenderName],
            Summary: "3D Render language pack is active: clean CGI anchoring, render-native lighting, and material-aware slider phrasing are now steering the prompt.",
            AnchorLabel: "3d render image language",
            Panel: new("3D Render Modifiers", "Render subtype and compact production accents. The lane keeps the strongest few fragments and trims the rest.", "3D Render Modifiers", "Render subtype and compact production accents. The lane keeps the strongest few fragments and trims the rest.", "Rendering Accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("style", "3D Render Subtype", nameof(PromptConfiguration.ThreeDRenderSubtype),
                [
                    Option("general-cgi", "General CGI", isDefault: true),
                    Option("stylized-3d", "Stylized 3D"),
                    Option("photoreal-3d", "Photoreal 3D"),
                    Option("game-asset", "Game Asset"),
                    Option("animated-feature", "Animated Feature"),
                    Option("product-visualization", "Product Visualization"),
                    Option("sci-fi-hard-surface", "Sci-Fi Hard Surface"),
                ]),
            ],
            Modifiers:
            [
                Modifier("global-illumination", "Global Illumination", nameof(PromptConfiguration.ThreeDRenderGlobalIllumination), "global illumination", "rendering-accents"),
                Modifier("volumetric-lighting", "Volumetric Lighting", nameof(PromptConfiguration.ThreeDRenderVolumetricLighting), "volumetric lighting", "rendering-accents"),
                Modifier("ray-traced-reflections", "Ray-Traced Reflections", nameof(PromptConfiguration.ThreeDRenderRayTracedReflections), "ray-traced reflections", "rendering-accents"),
                Modifier("depth-of-field", "Depth of Field", nameof(PromptConfiguration.ThreeDRenderDepthOfField), "rendered depth of field", "rendering-accents"),
                Modifier("subsurface-scattering", "Subsurface Scattering", nameof(PromptConfiguration.ThreeDRenderSubsurfaceScattering), "subsurface scattering", "rendering-accents"),
                Modifier("hard-surface-precision", "Hard-Surface Precision", nameof(PromptConfiguration.ThreeDRenderHardSurfacePrecision), "hard-surface precision", "rendering-accents"),
                Modifier("studio-backdrop", "Studio Backdrop", nameof(PromptConfiguration.ThreeDRenderStudioBackdrop), "studio backdrop", "rendering-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("rendering-accents", 3, 4, ["global-illumination", "volumetric-lighting", "ray-traced-reflections", "depth-of-field", "subsurface-scattering", "hard-surface-precision", "studio-backdrop"]),
            ],
            Defaults: new LanePromptDefaults
            {
                ArtStyle = "3D Render",
                Stylization = 48,
                Realism = 64,
                TextureDepth = 58,
                NarrativeDensity = 50,
                SurfaceAge = 8,
                BackgroundComplexity = 46,
                MotionEnergy = 24,
                FocusDepth = 56,
                ImageCleanliness = 64,
                DetailDensity = 60,
                Whimsy = 12,
                Tension = 24,
                Awe = 36,
                AtmosphericDepth = 46,
                Saturation = 52,
                Contrast = 60,
                LightingIntensity = 58,
                Lighting = "Soft rendered daylight",
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateConceptArtLane()
    {
        return new(
            Id: "concept-art",
            DisplayTitle: "Concept Art",
            IntentNames: [IntentModeCatalog.ConceptArtName],
            Summary: "Concept Art language pack is active: production-design anchoring, concept-aware slider phrasing, and compact modifier support are now steering the prompt.",
            AnchorLabel: "concept art presentation",
            Panel: new("Concept Art Modifiers", "Concept-focused subtype and compact design accents. The lane keeps the strongest few fragments and trims the rest.", "Concept Art Modifiers", "Concept-focused subtype and compact design accents. The lane keeps the strongest few fragments and trims the rest.", "Design Accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("style", "Concept Art Subtype", nameof(PromptConfiguration.ConceptArtSubtype),
                [
                    Option("keyframe-concept", "Keyframe Concept", isDefault: true),
                    Option("environment-concept", "Environment Concept"),
                    Option("character-concept", "Character Concept"),
                    Option("creature-concept", "Creature Concept"),
                    Option("costume-concept", "Costume Concept"),
                    Option("prop-concept", "Prop Concept"),
                    Option("vehicle-concept", "Vehicle Concept"),
                ]),
            ],
            Modifiers:
            [
                Modifier("design-callouts", "Design Callouts", nameof(PromptConfiguration.ConceptArtDesignCallouts), "design callouts", "design-accents"),
                Modifier("turnaround-readability", "Turnaround Readability", nameof(PromptConfiguration.ConceptArtTurnaroundReadability), "turnaround readability", "design-accents"),
                Modifier("material-breakdown", "Material Breakdown", nameof(PromptConfiguration.ConceptArtMaterialBreakdown), "material breakdown", "design-accents"),
                Modifier("scale-reference", "Scale Reference", nameof(PromptConfiguration.ConceptArtScaleReference), "scale reference", "design-accents"),
                Modifier("worldbuilding-accents", "Worldbuilding Accents", nameof(PromptConfiguration.ConceptArtWorldbuildingAccents), "worldbuilding accents", "design-accents"),
                Modifier("production-notes-feel", "Production Notes Feel", nameof(PromptConfiguration.ConceptArtProductionNotesFeel), "production notes feel", "design-accents"),
                Modifier("silhouette-clarity", "Silhouette Clarity", nameof(PromptConfiguration.ConceptArtSilhouetteClarity), "silhouette clarity", "design-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("design-accents", 3, 4, ["design-callouts", "turnaround-readability", "material-breakdown", "scale-reference", "worldbuilding-accents", "production-notes-feel", "silhouette-clarity"]),
            ],
            Defaults: new LanePromptDefaults
            {
                ArtStyle = "Concept Art",
                Stylization = 58,
                Realism = 46,
                TextureDepth = 34,
                NarrativeDensity = 38,
                Symbolism = 20,
                SurfaceAge = 8,
                Framing = 46,
                CameraDistance = 48,
                CameraAngle = 42,
                BackgroundComplexity = 36,
                MotionEnergy = 20,
                FocusDepth = 52,
                ImageCleanliness = 58,
                DetailDensity = 44,
                AtmosphericDepth = 28,
                Chaos = 18,
                Whimsy = 12,
                Tension = 22,
                Awe = 26,
                Temperature = 50,
                LightingIntensity = 48,
                Saturation = 44,
                Contrast = 54,
                Lighting = "Soft daylight",
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreatePixelArtLane()
    {
        return new(
            Id: "pixel-art",
            DisplayTitle: "Pixel Art",
            IntentNames: [IntentModeCatalog.PixelArtName],
            Summary: "Pixel Art language pack is active: sprite-readable anchoring, palette discipline, and pixel-native slider phrasing are now steering the prompt.",
            AnchorLabel: "pixel art image language",
            Panel: new("Pixel Art Modifiers", "Compact subtype and sprite-readable accents. The lane keeps the strongest few fragments and trims the rest.", "Pixel Art Modifiers", "Compact subtype and sprite-readable accents. The lane keeps the strongest few fragments and trims the rest.", "Rendering Accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("style", "Pixel Art Subtype", nameof(PromptConfiguration.PixelArtSubtype),
                [
                    Option("retro-arcade", "Retro Arcade", isDefault: true),
                    Option("console-sprite", "Console Sprite"),
                    Option("isometric-pixel", "Isometric Pixel"),
                    Option("pixel-platformer", "Pixel Platformer"),
                    Option("rpg-tileset", "RPG Tileset"),
                    Option("pixel-portrait", "Pixel Portrait"),
                    Option("pixel-scene", "Pixel Scene"),
                ]),
            ],
            Modifiers:
            [
                Modifier("limited-palette", "Limited Palette", nameof(PromptConfiguration.PixelArtLimitedPalette), "limited palette", "rendering-accents"),
                Modifier("dithering", "Dithering", nameof(PromptConfiguration.PixelArtDithering), "dithering", "rendering-accents"),
                Modifier("tileable-design", "Tileable Design", nameof(PromptConfiguration.PixelArtTileableDesign), "tileable design", "rendering-accents"),
                Modifier("sprite-sheet-readability", "Sprite Sheet Readability", nameof(PromptConfiguration.PixelArtSpriteSheetReadability), "sprite-sheet readability", "rendering-accents"),
                Modifier("clean-outline", "Clean Outline", nameof(PromptConfiguration.PixelArtCleanOutline), "clean outline", "rendering-accents"),
                Modifier("subpixel-shading", "Subpixel Shading", nameof(PromptConfiguration.PixelArtSubpixelShading), "subpixel shading", "rendering-accents"),
                Modifier("hud-ui-framing", "HUD / UI Framing", nameof(PromptConfiguration.PixelArtHudUiFraming), "HUD-style framing", "rendering-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("rendering-accents", 3, 4, ["limited-palette", "dithering", "tileable-design", "sprite-sheet-readability", "clean-outline", "subpixel-shading", "hud-ui-framing"]),
            ],
            Defaults: new LanePromptDefaults
            {
                ArtStyle = "Pixel Art",
                Stylization = 66,
                Realism = 22,
                TextureDepth = 22,
                NarrativeDensity = 30,
                Symbolism = 14,
                SurfaceAge = 6,
                Framing = 44,
                CameraDistance = 44,
                BackgroundComplexity = 30,
                MotionEnergy = 20,
                FocusDepth = 54,
                ImageCleanliness = 72,
                DetailDensity = 42,
                AtmosphericDepth = 16,
                Whimsy = 20,
                Tension = 16,
                Awe = 24,
                Saturation = 48,
                Contrast = 66,
                Lighting = "Soft daylight",
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateWatercolorLane()
    {
        return new(
            Id: "watercolor",
            DisplayTitle: "Watercolor",
            IntentNames: [IntentModeCatalog.WatercolorName],
            Summary: "Watercolor language pack is active: transparent pigment handling, paper-backed softness, and watercolor-aware slider phrasing are now steering the prompt.",
            AnchorLabel: "watercolor illustration",
            Panel: new("Watercolor Modifiers", "Dominant watercolor subtype plus compact rendering accents. The lane keeps the strongest few fragments and trims the rest.", "Watercolor Modifiers", "Dominant watercolor subtype plus compact rendering accents. The lane keeps the strongest few fragments and trims the rest.", "Rendering Accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("style", "Watercolor Style", nameof(PromptConfiguration.WatercolorStyle),
                [
                    Option("general-watercolor", "General Watercolor", isDefault: true),
                    Option("botanical-watercolor", "Botanical Watercolor"),
                    Option("storybook-watercolor", "Storybook Watercolor"),
                    Option("landscape-watercolor", "Landscape Watercolor"),
                    Option("architectural-watercolor", "Architectural Watercolor"),
                ]),
            ],
            Modifiers:
            [
                Modifier("transparent-washes", "Transparent Washes", nameof(PromptConfiguration.WatercolorTransparentWashes), "transparent washes", "rendering-accents"),
                Modifier("soft-bleeds", "Soft Bleeds", nameof(PromptConfiguration.WatercolorSoftBleeds), "soft bleeds", "rendering-accents"),
                Modifier("paper-texture", "Paper Texture", nameof(PromptConfiguration.WatercolorPaperTexture), "paper texture", "rendering-accents"),
                Modifier("ink-and-watercolor", "Ink and Watercolor", nameof(PromptConfiguration.WatercolorInkAndWatercolor), "ink-and-watercolor interplay", "rendering-accents"),
                Modifier("atmospheric-wash", "Atmospheric Wash", nameof(PromptConfiguration.WatercolorAtmosphericWash), "atmospheric wash", "rendering-accents"),
                Modifier("gouache-accents", "Gouache Accents", nameof(PromptConfiguration.WatercolorGouacheAccents), "gouache accents", "rendering-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("rendering-accents", 3, 4, ["transparent-washes", "soft-bleeds", "paper-texture", "ink-and-watercolor", "atmospheric-wash", "gouache-accents"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 58,
                Realism = 39,
                TextureDepth = 60,
                NarrativeDensity = 44,
                SurfaceAge = 12,
                BackgroundComplexity = 48,
                MotionEnergy = 16,
                FocusDepth = 39,
                ImageCleanliness = 54,
                DetailDensity = 46,
                Whimsy = 24,
                Tension = 26,
                Awe = 58,
                AtmosphericDepth = 60,
                Saturation = 36,
                Contrast = 36,
                Lighting = "Soft daylight",
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateFantasyIllustrationLane()
    {
        return new(
            Id: "fantasy-illustration",
            DisplayTitle: "Fantasy Illustration",
            IntentNames: [IntentModeCatalog.FantasyIllustrationName],
            Summary: "Storied worldbuilding, crafted material presence, legend-bearing atmosphere, and emblematic narrative suggestion.",
            AnchorLabel: "fantasy illustration",
            Panel: new("Fantasy Illustration", "Legend-driven illustrative language with a shared baseline semantic map and future subtype expansion.", "Accents", "Select a fantasy register to tune the lane's slider defaults and semantic language.", "Subtype", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("fantasy-register", "Fantasy Register", nameof(PromptConfiguration.FantasyIllustrationRegister),
                [
                    Option("general-fantasy", "General Fantasy", isDefault: true, supportDescriptorHint: "Storied worldbuilding, crafted material presence, legend-bearing atmosphere, and emblematic narrative suggestion."),
                    Option("low-magic", "Low Magic", defaultNudges: new LanePromptDefaults
                    {
                        Stylization = 54,
                        Realism = 60,
                        TextureDepth = 64,
                        NarrativeDensity = 58,
                        Symbolism = 46,
                        SurfaceAge = 62,
                        Framing = 55,
                        CameraDistance = 57,
                        CameraAngle = 47,
                        BackgroundComplexity = 58,
                        MotionEnergy = 42,
                        AtmosphericDepth = 63,
                        Chaos = 32,
                        FocusDepth = 56,
                        ImageCleanliness = 52,
                        DetailDensity = 62,
                        Whimsy = 18,
                        Tension = 40,
                        Awe = 50,
                        LightingIntensity = 47,
                        Saturation = 42,
                        Contrast = 51,
                        Lighting = "Soft daylight",
                    }, supportDescriptorHint: "Grounded, weathered, omen-bearing, materially believable."),
                    Option("high-magic", "High Magic", defaultNudges: new LanePromptDefaults
                    {
                        Stylization = 66,
                        Realism = 46,
                        TextureDepth = 61,
                        NarrativeDensity = 60,
                        Symbolism = 61,
                        SurfaceAge = 50,
                        Framing = 56,
                        CameraDistance = 58,
                        CameraAngle = 48,
                        BackgroundComplexity = 61,
                        MotionEnergy = 48,
                        AtmosphericDepth = 68,
                        Chaos = 36,
                        FocusDepth = 56,
                        ImageCleanliness = 56,
                        DetailDensity = 64,
                        Whimsy = 24,
                        Tension = 40,
                        Awe = 68,
                        LightingIntensity = 61,
                        Saturation = 57,
                        Contrast = 57,
                        Lighting = "Soft glow",
                    }, supportDescriptorHint: "Visible enchantment, ritual order, luminous systems, ceremonial grandeur."),
                    Option("magitech", "Magitech", defaultNudges: new LanePromptDefaults
                    {
                        Stylization = 61,
                        Realism = 55,
                        TextureDepth = 67,
                        NarrativeDensity = 59,
                        Symbolism = 52,
                        SurfaceAge = 56,
                        Framing = 56,
                        CameraDistance = 58,
                        CameraAngle = 48,
                        BackgroundComplexity = 66,
                        MotionEnergy = 49,
                        AtmosphericDepth = 61,
                        Chaos = 35,
                        FocusDepth = 56,
                        ImageCleanliness = 61,
                        DetailDensity = 70,
                        Whimsy = 20,
                        Tension = 44,
                        Awe = 58,
                        LightingIntensity = 56,
                        Saturation = 50,
                        Contrast = 59,
                        Lighting = "Dramatic studio light",
                    }, supportDescriptorHint: "Constructed magical infrastructure, engineered systems, operational stakes."),
                    Option("sword-and-sorcery", "Sword-and-Sorcery", defaultNudges: new LanePromptDefaults
                    {
                        Stylization = 62,
                        Realism = 56,
                        TextureDepth = 66,
                        NarrativeDensity = 60,
                        Symbolism = 50,
                        SurfaceAge = 63,
                        Framing = 58,
                        CameraDistance = 57,
                        CameraAngle = 49,
                        BackgroundComplexity = 60,
                        MotionEnergy = 52,
                        AtmosphericDepth = 58,
                        Chaos = 41,
                        FocusDepth = 55,
                        ImageCleanliness = 49,
                        DetailDensity = 65,
                        Whimsy = 12,
                        Tension = 54,
                        Awe = 56,
                        LightingIntensity = 53,
                        Saturation = 51,
                        Contrast = 61,
                        Lighting = "Warm directional light",
                    }, supportDescriptorHint: "Hard-edged pulp, steel-first danger, cruel decadence, ruin-shadowed brutality."),
                ]),
            ],
            Modifiers:
            [
                Modifier("character-sketch", "Character Sketch", nameof(PromptConfiguration.FantasyIllustrationCharacterSketch), "character-design presentation", "presentation-accents"),
                Modifier("character-centric", "Character-Centric", nameof(PromptConfiguration.FantasyIllustrationCharacterCentric), "character-first presentation", "presentation-accents"),
                Modifier("environment-concept", "Environment Concept", nameof(PromptConfiguration.FantasyIllustrationEnvironmentConcept), "environment concept presentation", "presentation-accents"),
                Modifier("key-art", "Key Art", nameof(PromptConfiguration.FantasyIllustrationKeyArt), "fantasy key art presentation", "presentation-accents"),
                Modifier("clean-background", "Clean Background", nameof(PromptConfiguration.FantasyIllustrationCleanBackground), "simplified backdrop treatment", "presentation-accents"),
                Modifier("silhouette-readability", "Silhouette Readability", nameof(PromptConfiguration.FantasyIllustrationSilhouetteReadability), "clean silhouette priority", "presentation-accents"),
                Modifier("photorealistic", "Photorealistic", nameof(PromptConfiguration.FantasyIllustrationPhotorealistic), "photorealistic fantasy rendering", "presentation-accents"),
                Modifier("cartoon-art", "Cartoon Art", nameof(PromptConfiguration.FantasyIllustrationCartoonArt), "cartoon fantasy illustration treatment", "presentation-accents"),
                Modifier("prop-artifact-focus", "Prop / Artifact Focus", nameof(PromptConfiguration.FantasyIllustrationPropArtifactFocus), "prop-first presentation", "presentation-accents"),
                Modifier("creature-design", "Creature Design", nameof(PromptConfiguration.FantasyIllustrationCreatureDesign), "creature-design presentation", "presentation-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("presentation-accents", 4, 10, ["character-sketch", "character-centric", "environment-concept", "key-art", "clean-background", "silhouette-readability", "photorealistic", "cartoon-art", "prop-artifact-focus", "creature-design"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 58,
                Realism = 52,
                TextureDepth = 61,
                NarrativeDensity = 57,
                Symbolism = 49,
                SurfaceAge = 55,
                Framing = 56,
                CameraDistance = 58,
                CameraAngle = 47,
                BackgroundComplexity = 59,
                MotionEnergy = 41,
                AtmosphericDepth = 60,
                Chaos = 34,
                FocusDepth = 56,
                ImageCleanliness = 54,
                DetailDensity = 63,
                Whimsy = 24,
                Tension = 38,
                Awe = 54,
                LightingIntensity = 51,
                Saturation = 48,
                Contrast = 52,
                Lighting = "Soft daylight",
            },
            ModifierCap: 10,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateEditorialIllustrationLane()
    {
        return new(
            Id: "editorial-illustration",
            DisplayTitle: "Editorial Illustration",
            IntentNames: [IntentModeCatalog.EditorialIllustrationName],
            Summary: "Concept-first illustration language for articles, essays, covers, op-eds, and publication-oriented visual storytelling.",
            AnchorLabel: "editorial illustration",
            Panel: new(
                "Editorial Illustration",
                "A publication-focused illustration lane for articles, essays, covers, and commentary. It emphasizes clear visual metaphor, controlled symbolism, and polished storytelling so ideas read quickly without drifting into spectacle, fantasy excess, or heavy concept-art worldbuilding.",
                "Accents",
                "A publication-focused illustration lane for articles, essays, covers, and commentary. It emphasizes clear visual metaphor, controlled symbolism, and polished storytelling so ideas read quickly without drifting into spectacle, fantasy excess, or heavy concept-art worldbuilding.",
                null,
                LanePanelLayout.SingleColumn),
            SubtypeSelectors: [],
            Modifiers:
            [
                Modifier("black-and-white-monochrome", "Black and White / Monochrome", nameof(PromptConfiguration.EditorialIllustrationBlackAndWhiteMonochrome), "black-and-white monochrome treatment", "presentation-overlays"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("presentation-overlays", 1, 1, ["black-and-white-monochrome"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 52,
                Realism = 46,
                TextureDepth = 42,
                NarrativeDensity = 58,
                Symbolism = 64,
                SurfaceAge = 10,
                Chaos = 22,
                Framing = 50,
                CameraDistance = 48,
                CameraAngle = 50,
                BackgroundComplexity = 30,
                MotionEnergy = 28,
                AtmosphericDepth = 36,
                FocusDepth = 44,
                ImageCleanliness = 72,
                DetailDensity = 50,
                Whimsy = 26,
                Tension = 38,
                Awe = 24,
                LightingIntensity = 54,
                Saturation = 48,
                Contrast = 54,
                Lighting = "Soft daylight",
            },
            ModifierCap: 1,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateTattooArtLane()
    {
        return new(
            Id: "tattoo-art",
            DisplayTitle: "Tattoo Art",
            IntentNames: [IntentModeCatalog.TattooArtName],
            Summary: "Flat, printable tattoo-flash design language for clean concept sheets and transfer-ready art foundations.",
            AnchorLabel: "tattoo flash design",
            Panel: new(
                "Tattoo Art",
                "Flat flash-design language for clean, front-facing tattoo concepts without body-wrap distortion.",
                "Accents",
                "For a simple line-art drawing, specify that directly in the Subject field along with the subject itself. Most professional tattoo artists already use tools that can convert artwork into pure line art, and most image models tend to drift away from basic black-and-white output unless that constraint is stated very directly. This lane is better suited for shaping clean flash-design foundations than for acting as a line-art converter.",
                "Accents",
                LanePanelLayout.SingleColumn),
            SubtypeSelectors: [],
            Modifiers: [],
            WeightGroups: [],
            Defaults: new LanePromptDefaults
            {
                Stylization = 45,
                Realism = 21,
                TextureDepth = 25,
                NarrativeDensity = 0,
                Symbolism = 25,
                SurfaceAge = 10,
                Framing = 10,
                CameraDistance = 21,
                CameraAngle = 5,
                BackgroundComplexity = 5,
                MotionEnergy = 24,
                AtmosphericDepth = 1,
                Chaos = 10,
                FocusDepth = 5,
                ImageCleanliness = 85,
                DetailDensity = 42,
                Whimsy = 10,
                Tension = 17,
                Awe = 15,
                Temperature = 51,
                LightingIntensity = 35,
                Saturation = 43,
                Contrast = 65,
                Lighting = "Soft daylight",
            },
            ModifierCap: 0,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateGraphicDesignLane()
    {
        return new(
            Id: "graphic-design",
            DisplayTitle: "Graphic Design",
            IntentNames: [IntentModeCatalog.GraphicDesignName],
            Summary: "Structured visual-communication language for layout-driven compositions with clear hierarchy, grouping, and polished presentation.",
            AnchorLabel: "graphic design composition",
            Panel: new(
                "Graphic Design",
                "Layout-first visual communication with hierarchy, grouping, and polished presentation discipline.",
                "Accents",
                "Reserved for later pass. No lane modifiers in this baseline install.",
                "Accents",
                LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("design-type", "Design Type", nameof(PromptConfiguration.GraphicDesignType),
                [
                    Option("general", "General Graphic Design", isDefault: true),
                    Option("poster", "Poster"),
                    Option("social-media", "Social Media Graphic"),
                    Option("cover-design", "Cover Design"),
                    Option("flyer-handout", "Flyer / Handout"),
                    Option("brand-identity", "Brand / Identity"),
                ]),
            ],
            Modifiers:
            [
                Modifier("minimal-layout", "Minimal Layout", nameof(PromptConfiguration.GraphicDesignMinimalLayout), "minimal layout discipline", "layout-accents"),
                Modifier("bold-hierarchy", "Bold Hierarchy", nameof(PromptConfiguration.GraphicDesignBoldHierarchy), "bold visual hierarchy", "layout-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("layout-accents", 2, 2, ["minimal-layout", "bold-hierarchy"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 56,
                Realism = 34,
                TextureDepth = 24,
                NarrativeDensity = 34,
                Symbolism = 36,
                SurfaceAge = 10,
                Framing = 46,
                CameraDistance = 42,
                CameraAngle = 10,
                BackgroundComplexity = 28,
                MotionEnergy = 34,
                AtmosphericDepth = 14,
                Chaos = 18,
                FocusDepth = 48,
                ImageCleanliness = 82,
                DetailDensity = 44,
                Whimsy = 18,
                Tension = 28,
                Awe = 26,
                Temperature = 50,
                LightingIntensity = 42,
                Saturation = 50,
                Contrast = 66,
                Lighting = "Soft daylight",
            },
            ModifierCap: 2,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateChildrensBookLane()
    {
        return new(
            Id: "childrens-book",
            DisplayTitle: "Children's Book",
            IntentNames: [IntentModeCatalog.ChildrensBookName],
            Summary: "Children's Book language pack is active: gentle illustrated storytelling, compact anchor language, and story-first slider phrasing are now steering the prompt.",
            AnchorLabel: "children's book illustration",
            Panel: new("Children's Book Modifiers", "Compact story-first subtype and rendering accents. The lane keeps the strongest few fragments and trims the rest.", "Children's Book Modifiers", "Compact story-first subtype and rendering accents. The lane keeps the strongest few fragments and trims the rest.", "Rendering Accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("style", "Children's Book Style", nameof(PromptConfiguration.ChildrensBookStyle),
                [
                    Option("general-childrens-book", "General Children's Book", isDefault: true),
                    Option("storybook-classic", "Storybook Classic"),
                    Option("playful-cartoon", "Playful Cartoon"),
                    Option("gentle-bedtime", "Gentle Bedtime"),
                    Option("educational-illustration", "Educational Illustration"),
                    Option("whimsical-fantasy", "Whimsical Fantasy"),
                ]),
            ],
            Modifiers:
            [
                Modifier("soft-color-palette", "Soft Color Palette", nameof(PromptConfiguration.ChildrensBookSoftColorPalette), "soft color palette", "rendering-accents"),
                Modifier("textured-paper", "Textured Paper", nameof(PromptConfiguration.ChildrensBookTexturedPaper), "textured paper", "rendering-accents"),
                Modifier("ink-linework", "Ink Linework", nameof(PromptConfiguration.ChildrensBookInkLinework), "ink linework", "rendering-accents"),
                Modifier("expressive-characters", "Expressive Characters", nameof(PromptConfiguration.ChildrensBookExpressiveCharacters), "expressive characters", "rendering-accents"),
                Modifier("minimal-background", "Minimal Background", nameof(PromptConfiguration.ChildrensBookMinimalBackground), "minimal background", "rendering-accents"),
                Modifier("decorative-details", "Decorative Details", nameof(PromptConfiguration.ChildrensBookDecorativeDetails), "decorative details", "rendering-accents"),
                Modifier("gentle-lighting", "Gentle Lighting", nameof(PromptConfiguration.ChildrensBookGentleLighting), "gentle lighting", "rendering-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("rendering-accents", 3, 4, ["soft-color-palette", "textured-paper", "ink-linework", "expressive-characters", "minimal-background", "decorative-details", "gentle-lighting"]),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 50,
                Realism = 38,
                TextureDepth = 42,
                NarrativeDensity = 58,
                SurfaceAge = 8,
                BackgroundComplexity = 42,
                MotionEnergy = 22,
                FocusDepth = 52,
                ImageCleanliness = 58,
                DetailDensity = 46,
                Whimsy = 42,
                Tension = 18,
                Awe = 50,
                AtmosphericDepth = 40,
                Saturation = 56,
                Contrast = 44,
                Lighting = "Soft daylight",
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }

    private static LaneDefinition CreateComicBookLane()
    {
        return new(
            Id: "comic-book",
            DisplayTitle: "Comic Book",
            IntentNames: [IntentModeCatalog.ComicBookName],
            Summary: "Comic Book language pack is active: graphic storytelling with sharp ink contrast, readable panel energy, and clear narrative beats.",
            AnchorLabel: "comic book illustration",
            Panel: new("Comic Book Modifiers", "Compact subtype and ink-forward accents. Speech bubbles only trigger when quoted dialogue is present.", "Comic Book Modifiers", "Compact subtype and ink-forward accents. Speech bubbles only trigger when quoted dialogue is present.", "Rendering Accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("style", "Comic Book Style", nameof(PromptConfiguration.ComicBookStyle),
                [
                    Option("general-comic", "General Comic", isDefault: true),
                    Option("superhero-comic", "Superhero Comic", defaultNudges: new LanePromptDefaults
                    {
                        Stylization = 78,
                        Realism = 44,
                        TextureDepth = 40,
                        NarrativeDensity = 58,
                        Symbolism = 28,
                        BackgroundComplexity = 42,
                        MotionEnergy = 74,
                        Tension = 62,
                        Awe = 68,
                        Contrast = 78,
                        CameraDistance = 58,
                        CameraAngle = 34,
                    }),
                    Option("noir-comic", "Noir Comic", defaultNudges: new LanePromptDefaults
                    {
                        Stylization = 70,
                        Realism = 52,
                        TextureDepth = 44,
                        NarrativeDensity = 56,
                        Symbolism = 30,
                        BackgroundComplexity = 38,
                        MotionEnergy = 42,
                        Tension = 64,
                        Awe = 34,
                        Contrast = 86,
                        CameraDistance = 46,
                        CameraAngle = 36,
                    }),
                    Option("graphic-novel", "Graphic Novel", defaultNudges: new LanePromptDefaults
                    {
                        Stylization = 62,
                        Realism = 58,
                        TextureDepth = 42,
                        NarrativeDensity = 62,
                        Symbolism = 26,
                        BackgroundComplexity = 44,
                        MotionEnergy = 46,
                        Tension = 58,
                        Awe = 38,
                        Contrast = 70,
                        CameraDistance = 48,
                        CameraAngle = 42,
                    }),
                    Option("vintage-comic", "Vintage Comic", defaultNudges: new LanePromptDefaults
                    {
                        Stylization = 72,
                        Realism = 40,
                        TextureDepth = 52,
                        NarrativeDensity = 54,
                        Symbolism = 24,
                        BackgroundComplexity = 38,
                        MotionEnergy = 64,
                        Tension = 54,
                        Awe = 52,
                        Contrast = 80,
                        CameraDistance = 52,
                        CameraAngle = 38,
                    }),
                    Option("modern-comic", "Modern Comic", defaultNudges: new LanePromptDefaults
                    {
                        Stylization = 68,
                        Realism = 50,
                        TextureDepth = 36,
                        NarrativeDensity = 56,
                        Symbolism = 22,
                        BackgroundComplexity = 42,
                        MotionEnergy = 62,
                        Tension = 56,
                        Awe = 50,
                        Contrast = 76,
                        CameraDistance = 52,
                        CameraAngle = 40,
                    }),
                ]),
            ],
            Modifiers:
            [
                Modifier("bold-ink", "Bold Ink", nameof(PromptConfiguration.ComicBookBoldInk), "bold ink linework", "rendering-accents"),
                Modifier("halftone-shading", "Halftone Shading", nameof(PromptConfiguration.ComicBookHalftoneShading), "halftone shading", "rendering-accents"),
                Modifier("panel-framing", "Panel Framing", nameof(PromptConfiguration.ComicBookPanelFraming), "panel-driven framing", "rendering-accents"),
                Modifier("dynamic-poses", "Dynamic Poses", nameof(PromptConfiguration.ComicBookDynamicPoses), "dynamic posing", "rendering-accents"),
                Modifier("speed-lines", "Speed Lines", nameof(PromptConfiguration.ComicBookSpeedLines), "speed-line motion", "rendering-accents"),
                Modifier("high-contrast-lighting", "High Contrast Lighting", nameof(PromptConfiguration.ComicBookHighContrastLighting), "high-contrast lighting", "rendering-accents"),
                Modifier("speech-bubbles", "Speech Bubbles", nameof(PromptConfiguration.ComicBookSpeechBubbles), "dialogue rendered in comic speech bubbles", "story-overlay", preserveFromCompression: true, triggerRequirement: "QuotedDialoguePresent"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("rendering-accents", 3, 4, ["bold-ink", "halftone-shading", "panel-framing", "dynamic-poses", "speed-lines", "high-contrast-lighting"]),
                new LaneWeightGroupDefinition("story-overlay", 1, 1, ["speech-bubbles"], "quoted dialogue"),
            ],
            Defaults: new LanePromptDefaults
            {
                Stylization = 66,
                Realism = 39,
                TextureDepth = 38,
                NarrativeDensity = 52,
                Symbolism = 20,
                SurfaceAge = 12,
                Framing = 54,
                CameraDistance = 50,
                CameraAngle = 42,
                BackgroundComplexity = 40,
                MotionEnergy = 60,
                FocusDepth = 52,
                ImageCleanliness = 60,
                DetailDensity = 46,
                AtmosphericDepth = 28,
                Chaos = 22,
                Whimsy = 14,
                Tension = 50,
                Awe = 44,
                Temperature = 50,
                LightingIntensity = 58,
                Saturation = 56,
                Contrast = 72,
                Lighting = "Dramatic studio light",
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar | LaneBehaviorFlags.RequiresPolicyHook,
            PolicyKey: "comic-book");
    }

    private static LaneDefinition CreateCinematicLane()
    {
        return new(
            Id: "cinematic",
            DisplayTitle: "Cinematic",
            IntentNames: [IntentModeCatalog.CinematicName],
            Summary: "Cinematic language pack is active: film-still anchoring, lens-aware framing, and cinematic slider phrasing are now steering the prompt.",
            AnchorLabel: "cinematic film still",
            Panel: new("Cinematic Modifiers", "Film-still subtype and compact lens / lighting accents. The lane keeps the strongest few fragments and trims the rest.", "Cinematic Modifiers", "Film-still subtype and compact lens / lighting accents. The lane keeps the strongest few fragments and trims the rest.", "Rendering Accents", LanePanelLayout.SingleColumn),
            SubtypeSelectors:
            [
                Selector("style", "Cinematic Subtype", nameof(PromptConfiguration.CinematicSubtype),
                [
                    Option("general-film-still", "General Film Still", isDefault: true),
                    Option("prestige-drama", "Prestige Drama"),
                    Option("thriller-suspense", "Thriller / Suspense"),
                    Option("noir-neo-noir", "Noir / Neo-Noir"),
                    Option("epic-blockbuster", "Epic / Blockbuster"),
                    Option("intimate-indie", "Intimate Indie"),
                    Option("sci-fi-cinema", "Sci-Fi Cinema"),
                ]),
            ],
            Modifiers:
            [
                Modifier("letterboxed-framing", "Letterboxed Framing", nameof(PromptConfiguration.CinematicLetterboxedFraming), "letterboxed presentation", "rendering-accents"),
                Modifier("shallow-depth-of-field", "Shallow Depth of Field", nameof(PromptConfiguration.CinematicShallowDepthOfField), "shallow depth of field", "rendering-accents"),
                Modifier("practical-lighting", "Practical Lighting", nameof(PromptConfiguration.CinematicPracticalLighting), "practical light sources", "rendering-accents"),
                Modifier("atmospheric-haze", "Atmospheric Haze", nameof(PromptConfiguration.CinematicAtmosphericHaze), "atmospheric haze", "rendering-accents"),
                Modifier("film-grain", "Film Grain", nameof(PromptConfiguration.CinematicFilmGrain), "fine film grain", "rendering-accents"),
                Modifier("anamorphic-flares", "Anamorphic Flares", nameof(PromptConfiguration.CinematicAnamorphicFlares), "anamorphic flares", "rendering-accents"),
                Modifier("dramatic-backlight", "Dramatic Backlight", nameof(PromptConfiguration.CinematicDramaticBacklight), "dramatic backlight", "rendering-accents"),
            ],
            WeightGroups:
            [
                new LaneWeightGroupDefinition("rendering-accents", 3, 4, ["letterboxed-framing", "shallow-depth-of-field", "practical-lighting", "atmospheric-haze", "film-grain", "anamorphic-flares", "dramatic-backlight"]),
            ],
            Defaults: new LanePromptDefaults
            {
                ArtStyle = "Cinematic",
                Stylization = 58,
                Realism = 54,
                TextureDepth = 30,
                NarrativeDensity = 42,
                Symbolism = 24,
                SurfaceAge = 8,
                Framing = 46,
                CameraDistance = 48,
                CameraAngle = 42,
                BackgroundComplexity = 34,
                MotionEnergy = 24,
                FocusDepth = 52,
                ImageCleanliness = 58,
                DetailDensity = 42,
                AtmosphericDepth = 30,
                Chaos = 18,
                Whimsy = 10,
                Tension = 28,
                Awe = 30,
                Temperature = 50,
                LightingIntensity = 50,
                Saturation = 48,
                Contrast = 58,
                Lighting = "Soft daylight",
            },
            ModifierCap: 4,
            BehaviorFlags: LaneBehaviorFlags.ShowManualControls | LaneBehaviorFlags.ShowModifierPanel | LaneBehaviorFlags.ShowSidecar);
    }
}
