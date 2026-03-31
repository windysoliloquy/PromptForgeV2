using System.Text;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed class PromptBuilderService : IPromptBuilderService
{
    private const string DefaultNegativePrompt = "no blurry detail, no muddy lighting, no distorted anatomy, no extra limbs, no text artifacts, no oversaturated color, no flat composition, no messy background, no poorly defined material texture";
    private readonly IArtistProfileService _artistProfileService;

    public PromptBuilderService(IArtistProfileService artistProfileService)
    {
        _artistProfileService = artistProfileService;
    }

    public PromptResult Build(PromptConfiguration configuration)
    {
        var effectiveConfiguration = ApplyIntentMode(configuration);
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddUnique(phrases, seen, BuildSubjectSection(effectiveConfiguration));
        AddUnique(phrases, seen, BuildRelationshipSection(effectiveConfiguration));
        foreach (var phrase in BuildStyleSection(effectiveConfiguration)) AddUnique(phrases, seen, phrase);
        foreach (var phrase in BuildCompositionSection(effectiveConfiguration)) AddUnique(phrases, seen, phrase);
        foreach (var phrase in BuildMoodSection(effectiveConfiguration)) AddUnique(phrases, seen, phrase);
        foreach (var phrase in BuildLightingAndColorSection(effectiveConfiguration)) AddUnique(phrases, seen, phrase);
        foreach (var phrase in BuildOutputSection(effectiveConfiguration)) AddUnique(phrases, seen, phrase);

        return new PromptResult
        {
            PositivePrompt = string.Join(", ", phrases),
            NegativePrompt = effectiveConfiguration.UseNegativePrompt ? BuildNegativePrompt(effectiveConfiguration) : string.Empty,
        };
    }

    private static PromptConfiguration ApplyIntentMode(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.TryGet(configuration.IntentMode, out var intentMode))
        {
            return configuration;
        }

        var effective = configuration.Clone();
        effective.Whimsy = intentMode.Whimsy;
        effective.Tension = intentMode.Tension;
        effective.Awe = intentMode.Awe;
        effective.Chaos = intentMode.Chaos;
        effective.MotionEnergy = intentMode.MotionEnergy;
        effective.AtmosphericDepth = intentMode.AtmosphericDepth;
        effective.NarrativeDensity = intentMode.NarrativeDensity;
        effective.Symbolism = intentMode.Symbolism;
        effective.Saturation = intentMode.Saturation;
        effective.Contrast = intentMode.Contrast;
        effective.Lighting = intentMode.Lighting;
        return effective;
    }
    private static string BuildSubjectSection(PromptConfiguration configuration)
    {
        var subject = Clean(configuration.Subject);
        var action = Clean(configuration.Action);
        if (!string.IsNullOrWhiteSpace(subject) && !string.IsNullOrWhiteSpace(action)) return $"{subject}, {action}";
        return !string.IsNullOrWhiteSpace(subject) ? subject : action;
    }

    private static string BuildRelationshipSection(PromptConfiguration configuration) => Clean(configuration.Relationship);

    private static string BuildNegativePrompt(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (configuration.AvoidBlurryDetail)
        {
            AddUnique(phrases, seen, "no blurry detail");
            AddUnique(phrases, seen, "no low detail");
        }

        if (configuration.AvoidClutter)
        {
            AddUnique(phrases, seen, "no cluttered composition");
        }

        if (configuration.AvoidMuddyLighting)
        {
            AddUnique(phrases, seen, "no muddy lighting");
        }

        if (configuration.AvoidDistortedAnatomy && !ShouldAllowDistortedAnatomy(configuration))
        {
            AddUnique(phrases, seen, "no distorted anatomy");
        }

        if (configuration.AvoidExtraLimbs)
        {
            AddUnique(phrases, seen, "no extra limbs");
            AddUnique(phrases, seen, "no extra fingers");
        }

        if (configuration.AvoidTextArtifacts)
        {
            AddUnique(phrases, seen, "no text artifacts");
            AddUnique(phrases, seen, "no watermark");
        }

        if (configuration.AvoidOversaturation)
        {
            AddUnique(phrases, seen, "no oversaturated color");
        }

        if (configuration.AvoidFlatComposition && !ShouldAllowFlatComposition(configuration))
        {
            AddUnique(phrases, seen, "no flat composition");
        }

        if (configuration.AvoidMessyBackground)
        {
            AddUnique(phrases, seen, "no messy background");
        }

        if (configuration.AvoidWeakMaterialDefinition)
        {
            AddUnique(phrases, seen, "no poorly defined material texture");
        }

        return phrases.Count == 0 ? DefaultNegativePrompt : string.Join(", ", phrases);
    }

    private static bool ShouldAllowDistortedAnatomy(PromptConfiguration configuration)
    {
        return HasAnyActiveArtist(configuration, "Pablo Picasso", "Salvador Dali", "Salvador Dalí", "El Greco", "Amedeo Modigliani", "Francis Bacon", "Egon Schiele");
    }

    private static bool ShouldAllowFlatComposition(PromptConfiguration configuration)
    {
        return HasAnyActiveArtist(configuration, "Pablo Picasso");
    }

    private static bool HasAnyActiveArtist(PromptConfiguration configuration, params string[] artists)
    {
        return artists.Any(artist => HasActiveArtist(configuration.ArtistInfluencePrimary, configuration.InfluenceStrengthPrimary, artist)
            || HasActiveArtist(configuration.ArtistInfluenceSecondary, configuration.InfluenceStrengthSecondary, artist));
    }

    private static bool HasActiveArtist(string? name, int strength, string expectedArtist)
    {
        return strength > 20
            && !string.IsNullOrWhiteSpace(name)
            && string.Equals(name, expectedArtist, StringComparison.OrdinalIgnoreCase);
    }

    private IEnumerable<string> BuildStyleSection(PromptConfiguration configuration)
    {
        yield return configuration.ArtStyle switch
        {
            "Cinematic" => "cinematic visual language",
            "Painterly" => "painterly image treatment",
            "Yarn Relief" => "rendered as a complete yarn relief composition",
            "Stained Glass" => "stained glass-inspired design language",
            "Surreal Symbolic" => "surreal symbolic imagery",
            "Concept Art" => "concept art presentation",
            _ => string.Empty,
        };

        yield return configuration.Material switch
        {
            "Yarn" => "yarn-built material presence",
            "Paint" => "paint-rich surface treatment",
            "Glass" => "glass-crafted surfaces",
            "Ink" => "ink-defined mark making",
            "Stone" => "stone-hewn form language",
            "Metal" => "metallic surface character",
            _ => string.Empty,
        };

        yield return MapStylization(configuration.Stylization, configuration);
        yield return MapRealism(configuration.Realism, configuration);
        yield return MapTextureDepth(configuration.TextureDepth, configuration);
        yield return MapSurfaceAge(configuration.SurfaceAge, configuration);

        foreach (var phrase in BuildArtistBlend(configuration))
        {
            yield return phrase;
        }

        if (configuration.Material == "Yarn" && configuration.TextureDepth >= 70)
        {
            yield return "visible fiber structure";
            yield return "layered textile depth";
        }
    }

    private IEnumerable<string> BuildArtistBlend(PromptConfiguration configuration)
    {
        var primary = CreateInfluence(configuration.ArtistInfluencePrimary, configuration.InfluenceStrengthPrimary);
        var secondary = CreateInfluence(configuration.ArtistInfluenceSecondary, configuration.InfluenceStrengthSecondary);

        if (primary is null && secondary is null)
        {
            yield break;
        }

        if (primary is not null && secondary is not null && string.Equals(primary.DisplayName, secondary.DisplayName, StringComparison.OrdinalIgnoreCase))
        {
            secondary = null;
        }

        if (primary is not null && secondary is not null)
        {
            foreach (var phrase in BuildDualArtistBlend(primary, secondary))
            {
                yield return phrase;
            }

            yield break;
        }

        var single = primary ?? secondary;
        if (single is null)
        {
            yield break;
        }

        foreach (var phrase in BuildSingleArtistInfluence(single))
        {
            yield return phrase;
        }
    }

    private IEnumerable<string> BuildDualArtistBlend(ArtistInfluence primary, ArtistInfluence secondary)
    {
        var stronger = primary.Strength >= secondary.Strength ? primary : secondary;
        var lighter = ReferenceEquals(stronger, primary) ? secondary : primary;

        yield return DescribeBlend(stronger, lighter);

        var strongerCategories = (stronger.Strength - lighter.Strength) switch
        {
            >= 35 => new[] { ArtistPhraseCategory.Hallmarks, ArtistPhraseCategory.Composition, ArtistPhraseCategory.Palette, ArtistPhraseCategory.Mood },
            >= 15 => new[] { ArtistPhraseCategory.Hallmarks, ArtistPhraseCategory.Composition, ArtistPhraseCategory.Surface },
            _ => new[] { ArtistPhraseCategory.Hallmarks, ArtistPhraseCategory.Composition },
        };

        var lighterCategories = (stronger.Strength - lighter.Strength) switch
        {
            >= 35 => new[] { ArtistPhraseCategory.Palette, ArtistPhraseCategory.Surface },
            >= 15 => new[] { ArtistPhraseCategory.Palette, ArtistPhraseCategory.Mood },
            _ => new[] { ArtistPhraseCategory.Palette, ArtistPhraseCategory.Surface, ArtistPhraseCategory.Mood },
        };

        foreach (var phrase in SelectCategoryPhrases(stronger, strongerCategories, DeterminePhraseBudget(stronger.Strength, true)))
        {
            yield return phrase;
        }

        foreach (var phrase in SelectCategoryPhrases(lighter, lighterCategories, DeterminePhraseBudget(lighter.Strength, false)))
        {
            yield return phrase;
        }
    }

    private IEnumerable<string> BuildSingleArtistInfluence(ArtistInfluence influence)
    {
        if (influence.Profile is null)
        {
            yield return SliderLanguageCatalog.ResolveArtistInfluenceDescriptor(influence.Strength, influence.DisplayName);
            yield break;
        }

        yield return SliderLanguageCatalog.ResolveArtistInfluenceDescriptor(influence.Strength, influence.DisplayName);

        var categories = new[]
        {
            ArtistPhraseCategory.Hallmarks,
            ArtistPhraseCategory.Composition,
            ArtistPhraseCategory.Palette,
            ArtistPhraseCategory.Surface,
            ArtistPhraseCategory.Mood,
        };

        foreach (var phrase in SelectCategoryPhrases(influence, categories, DeterminePhraseBudget(influence.Strength, true)))
        {
            yield return phrase;
        }
    }

    private static string DescribeBlend(ArtistInfluence stronger, ArtistInfluence lighter)
    {
        if (stronger.Profile is null || lighter.Profile is null)
        {
            return MapBand(stronger.Strength, string.Empty,
                $"light blend of {stronger.DisplayName} and {lighter.DisplayName}",
                $"blended influence from {stronger.DisplayName} and {lighter.DisplayName}",
                $"prominent blend of {stronger.DisplayName} with {lighter.DisplayName}",
                $"deeply fused influence from {stronger.DisplayName} and {lighter.DisplayName}");
        }

        if (stronger.Strength - lighter.Strength >= 25)
        {
            return $"{stronger.DisplayName}-led blend with {lighter.DisplayName} accents";
        }

        return $"balanced blend of {stronger.DisplayName} and {lighter.DisplayName}";
    }

    private ArtistInfluence? CreateInfluence(string artistName, int strength)
    {
        if (strength <= 20 || string.IsNullOrWhiteSpace(artistName) || string.Equals(artistName, "None", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var profile = _artistProfileService.GetProfile(artistName);
        return new ArtistInfluence(profile?.Name ?? artistName, strength, profile);
    }

    private static IEnumerable<string> SelectCategoryPhrases(ArtistInfluence influence, ArtistPhraseCategory[] categories, int budget)
    {
        if (influence.Profile is null || budget <= 0)
        {
            yield break;
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var yielded = 0;

        foreach (var category in categories)
        {
            foreach (var phrase in GetPhrases(influence.Profile, category))
            {
                if (string.IsNullOrWhiteSpace(phrase) || !seen.Add(phrase))
                {
                    continue;
                }

                yield return phrase;
                yielded++;
                break;
            }

            if (yielded >= budget)
            {
                yield break;
            }
        }

        if (yielded >= budget)
        {
            yield break;
        }

        foreach (var phrase in categories.SelectMany(category => GetPhrases(influence.Profile, category)))
        {
            if (string.IsNullOrWhiteSpace(phrase) || !seen.Add(phrase))
            {
                continue;
            }

            yield return phrase;
            yielded++;
            if (yielded >= budget)
            {
                yield break;
            }
        }
    }

    private static IEnumerable<string> GetPhrases(ArtistProfile profile, ArtistPhraseCategory category) => category switch
    {
        ArtistPhraseCategory.Hallmarks => profile.Hallmarks,
        ArtistPhraseCategory.Composition => profile.Composition,
        ArtistPhraseCategory.Palette => profile.Palette,
        ArtistPhraseCategory.Surface => profile.Surface,
        ArtistPhraseCategory.Mood => profile.Mood,
        _ => [],
    };

    private static int DeterminePhraseBudget(int strength, bool primaryWeight)
    {
        if (strength <= 40) return primaryWeight ? 2 : 1;
        if (strength <= 60) return primaryWeight ? 3 : 2;
        if (strength <= 80) return primaryWeight ? 4 : 2;
        return primaryWeight ? 5 : 3;
    }

    private static IEnumerable<string> BuildCompositionSection(PromptConfiguration configuration)
    {
        yield return Lower(configuration.CameraDistance);
        yield return Lower(configuration.CameraAngle);
        yield return MapBackgroundComplexity(configuration.BackgroundComplexity, configuration);
        yield return MapMotionEnergy(configuration.MotionEnergy, configuration);
        yield return MapNarrativeDensity(configuration.NarrativeDensity, configuration);
        yield return MapAtmosphericDepth(configuration.AtmosphericDepth, configuration);
        yield return MapChaos(configuration.Chaos, configuration);
    }

    private static IEnumerable<string> BuildMoodSection(PromptConfiguration configuration)
    {
        yield return MapWhimsy(configuration.Whimsy, configuration);
        yield return MapTension(configuration.Tension, configuration);
        yield return MapAwe(configuration.Awe, configuration);
        yield return MapSymbolism(configuration.Symbolism, configuration);
        if (configuration.Whimsy >= 70 && configuration.Tension >= 50) yield return "comedic interpersonal tension";
    }

    private static IEnumerable<string> BuildLightingAndColorSection(PromptConfiguration configuration)
    {
        yield return Lower(configuration.Lighting);
        yield return MapSaturation(configuration.Saturation, configuration);
        yield return MapContrast(configuration.Contrast, configuration);
    }

    private static IEnumerable<string> BuildOutputSection(PromptConfiguration configuration)
    {
        yield return $"aspect ratio {configuration.AspectRatio}";
        if (configuration.PrintReady) { yield return "high detail clarity"; yield return "clean edge definition"; }
        if (configuration.TransparentBackground) { yield return "isolated subject"; yield return "clean background separation"; }
    }

    private static string MapStylization(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.Stylization, value, configuration);

    private static string MapRealism(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.Realism, value, configuration);

    private static string MapTextureDepth(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.TextureDepth, value, configuration);

    private static string MapSurfaceAge(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.SurfaceAge, value, configuration);

    private static string MapBackgroundComplexity(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.BackgroundComplexity, value, configuration);

    private static string MapMotionEnergy(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.MotionEnergy, value, configuration);

    private static string MapNarrativeDensity(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.NarrativeDensity, value, configuration);

    private static string MapAtmosphericDepth(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.AtmosphericDepth, value, configuration);

    private static string MapChaos(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.Chaos, value, configuration);

    private static string MapWhimsy(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.Whimsy, value, configuration);

    private static string MapTension(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.Tension, value, configuration);

    private static string MapAwe(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.Awe, value, configuration);

    private static string MapSymbolism(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.Symbolism, value, configuration);

    private static string MapSaturation(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.Saturation, value, configuration);

    private static string MapContrast(int value, PromptConfiguration configuration) => SliderLanguageCatalog.ResolvePhrase(SliderLanguageCatalog.Contrast, value, configuration);

    private static string MapStylization(int value, string artStyle) => artStyle switch
    {
        "Cinematic" => MapBand(value, "grounded cinematic framing", "light directorial stylization", "filmic stylization", "strong cinematic stylization", "highly stylized cinematic language"),
        "Painterly" => MapBand(value, "grounded painterly treatment", "light painterly stylization", "stylized brush-led rendering", "expressive painterly stylization", "highly painterly visual language"),
        "Yarn Relief" => MapBand(value, "grounded textile shaping", "light textile stylization", "constructed yarn relief rendering", "strong yarn relief stylization", "highly stylized textile relief language"),
        "Stained Glass" => MapBand(value, "grounded leaded-glass structure", "light stained-glass stylization", "stained-glass image treatment", "ornate stained-glass stylization", "highly stylized stained-glass iconography"),
        "Surreal Symbolic" => MapBand(value, "grounded dreamlike treatment", "light surreal stylization", "stylized surreal rendering", "strong surreal stylization", "highly symbolic surreal language"),
        "Concept Art" => MapBand(value, "grounded production design treatment", "light concept stylization", "stylized concept rendering", "strong concept-art stylization", "highly stylized concept-art language"),
        _ => MapBand(value, "grounded visual treatment", "light stylization", "stylized rendering", "strong stylization", "highly stylized visual language"),
    };

    private static string MapRealism(int value, string artStyle, string material)
    {
        if (Matches(artStyle, material, "Yarn Relief", "Yarn"))
        {
            return MapBand(value, string.Empty, "suggestively realistic fiber sculpture", "convincingly observed yarn-built anatomy", "highly convincing textile realism", "museum-caliber yarn relief realism");
        }

        if (Matches(artStyle, material, "Stained Glass", "Glass"))
        {
            return MapBand(value, string.Empty, "loosely realistic sacred figuration", "moderately realistic leaded-glass figuration", "highly legible luminous figuration", "cathedral-grade stained-glass realism");
        }

        if (Matches(artStyle, material, "Painterly", "Paint"))
        {
            return MapBand(value, string.Empty, "loosely observed realism", "painterly realism", "convincing atelier realism", "museum-grade representational realism");
        }

        if (Matches(artStyle, material, "Concept Art", "Metal"))
        {
            return MapBand(value, string.Empty, "loosely realistic hard-surface design", "moderately realistic industrial rendering", "high visual realism in hard-surface production art", "portfolio-grade metallic concept realism");
        }

        if (Matches(artStyle, material, "Concept Art", "Stone"))
        {
            return MapBand(value, string.Empty, "loosely realistic monumental forms", "moderately realistic stone worldbuilding", "high visual realism in carved environmental design", "portfolio-grade stone concept realism");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Ink"))
        {
            return MapBand(value, string.Empty, "loosely realistic symbolic linework", "moderately realistic ink-dream rendering", "high visual realism within inked surreal logic", "hyper-defined surreal ink realism");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Glass"))
        {
            return MapBand(value, string.Empty, "loosely realistic translucent dream forms", "moderately realistic glasslike surreal rendering", "high visual realism in uncanny translucent forms", "hyper-real visionary glass realism");
        }

        if (string.Equals(material, "Yarn", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "suggestively realistic textile forms", "recognizably realistic fiber-built forms", "highly convincing textile realism", "strikingly realistic yarn-built rendering");
        }

        if (string.Equals(material, "Glass", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "loosely realistic translucent forms", "moderately realistic glass-formed rendering", "highly legible glass realism", "strikingly realistic luminous glass rendering");
        }

        if (string.Equals(material, "Stone", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "loosely realistic carved massing", "moderately realistic stone form", "high visual realism in carved surfaces", "strongly realistic hewn stone rendering");
        }

        if (string.Equals(material, "Metal", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "loosely realistic metallic forms", "moderately realistic metallic rendering", "high visual realism in reflective metal", "strongly realistic metallic surface rendering");
        }

        if (string.Equals(material, "Ink", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "loosely realistic ink-drawn forms", "moderately realistic ink rendering", "highly legible ink realism", "strongly realistic ink-defined rendering");
        }

        if (string.Equals(material, "Paint", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "loosely observed realism", "painterly realism", "convincing painterly realism", "museum-grade representational realism");
        }

        return artStyle switch
        {
            "Painterly" => MapBand(value, string.Empty, "loosely observed realism", "painterly realism", "convincing painterly realism", "museum-grade representational realism"),
            "Yarn Relief" => MapBand(value, string.Empty, "suggestively realistic textile forms", "recognizably realistic fiber-built forms", "highly convincing textile realism", "strikingly realistic yarn-built rendering"),
            "Stained Glass" => MapBand(value, string.Empty, "loosely realistic iconographic form", "moderately realistic glass-figural rendering", "highly legible figurative realism", "strongly realistic stained-glass figuration"),
            "Surreal Symbolic" => MapBand(value, string.Empty, "loosely realistic dream imagery", "moderately realistic surreal rendering", "high visual realism within surreal logic", "hyper-real surreal rendering"),
            "Concept Art" => MapBand(value, string.Empty, "loosely realistic production sketching", "moderately realistic design rendering", "high visual realism for production art", "strongly realistic concept rendering"),
            _ => MapBand(value, string.Empty, "loosely realistic", "moderately realistic", "high visual realism", "strongly realistic rendering"),
        };
    }

    private static string MapTextureDepth(int value, string artStyle, string material)
    {
        if (Matches(artStyle, material, "Yarn Relief", "Yarn"))
        {
            return MapBand(value, string.Empty, "light thread texture", "clearly layered yarn relief texture", "rich knotted fiber build-up", "deep textile relief and sculpted yarn depth");
        }

        if (Matches(artStyle, material, "Stained Glass", "Glass"))
        {
            return MapBand(value, string.Empty, "light glass texture", "clear leaded-glass definition", "rich faceted jewel-glass texture", "deeply worked stained-glass relief");
        }

        if (Matches(artStyle, material, "Painterly", "Paint"))
        {
            return MapBand(value, string.Empty, "light brush grain", "visible atelier brushwork", "rich impasto and painterly build-up", "deeply worked museum-surface relief");
        }

        if (Matches(artStyle, material, "Concept Art", "Metal"))
        {
            return MapBand(value, string.Empty, "light hard-surface grain", "clear machined-panel texture", "rich industrial surface detail", "deeply resolved hard-surface finish");
        }

        if (Matches(artStyle, material, "Concept Art", "Stone"))
        {
            return MapBand(value, string.Empty, "light carved grain", "clear hewn architectural texture", "rich monumental stone detail", "deeply resolved carved-environment relief");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Ink"))
        {
            return MapBand(value, string.Empty, "light ink grain", "clear symbolic line texture", "rich ink pooling and line build-up", "deeply saturated surreal ink relief");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Glass"))
        {
            return MapBand(value, string.Empty, "light translucent texture", "clear dream-glass definition", "rich uncanny faceted texture", "deeply worked visionary glass relief");
        }

        if (string.Equals(material, "Yarn", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "light thread texture", "clearly layered yarn texture", "rich tactile fiber build-up", "deep textile relief and knotted depth");
        }

        if (string.Equals(material, "Glass", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "light glass texture", "leaded glass surface definition", "rich faceted glass texture", "deeply worked glass-and-leading relief");
        }

        if (string.Equals(material, "Stone", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "light stone grain", "clearly hewn stone texture", "rich carved mineral texture", "deeply chiseled stone relief");
        }

        if (string.Equals(material, "Metal", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "light metallic grain", "clearly machined metal texture", "rich forged surface detail", "deeply worked metallic relief");
        }

        if (string.Equals(material, "Ink", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "light ink grain", "clear inked line texture", "rich ink layering", "deeply saturated ink surface build-up");
        }

        if (string.Equals(material, "Paint", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "light brush grain", "visible paint handling", "rich impasto and brush build-up", "deeply worked painterly surface relief");
        }

        return artStyle switch
        {
            "Cinematic" => MapBand(value, string.Empty, "light surface grain", "readable production texture", "rich cinematic surface detail", "hyper-detailed cinematic surface finish"),
            "Painterly" => MapBand(value, string.Empty, "light brush grain", "visible paint handling", "rich impasto and brush build-up", "deeply worked painterly surface relief"),
            "Yarn Relief" => MapBand(value, string.Empty, "light thread texture", "clearly layered yarn texture", "rich tactile fiber build-up", "deep textile relief and knotted depth"),
            "Stained Glass" => MapBand(value, string.Empty, "light glass texture", "leaded glass surface definition", "rich faceted glass texture", "deeply worked glass-and-leading relief"),
            "Surreal Symbolic" => MapBand(value, string.Empty, "light illusionistic surface grain", "finely worked surreal texture", "rich uncanny surface detail", "hyper-finished surreal surface articulation"),
            "Concept Art" => MapBand(value, string.Empty, "light design texture", "clear material read", "rich production-surface detail", "deeply resolved material finish"),
            _ => MapBand(value, string.Empty, "light surface grain", "finely worked material texture", "rich tactile surface build-up", "deeply hewn tactile surface relief"),
        };
    }

    private static string MapSurfaceAge(int value, string artStyle, string material)
    {
        if (Matches(artStyle, material, "Yarn Relief", "Yarn"))
        {
            return MapBand(value, string.Empty, "fresh textile surfaces", "soft handled-fiber wear", "weathered tapestry-like fiber character", "time-worn heirloom textile patina");
        }

        if (Matches(artStyle, material, "Stained Glass", "Glass"))
        {
            return MapBand(value, string.Empty, "freshly set glass panels", "subtle devotional patina", "weathered chapel-glass character", "aged cathedral-glass patina");
        }

        if (Matches(artStyle, material, "Painterly", "Paint"))
        {
            return MapBand(value, string.Empty, "freshly laid paint surfaces", "subtle aged varnish character", "weathered painted patina", "time-softened gallery-surface patina");
        }

        if (Matches(artStyle, material, "Concept Art", "Metal"))
        {
            return MapBand(value, string.Empty, "freshly fabricated metal", "subtle industrial wear", "weathered production-metal patina", "battle-worn metallic aging");
        }

        if (Matches(artStyle, material, "Concept Art", "Stone"))
        {
            return MapBand(value, string.Empty, "freshly cut monumental stone", "subtle environmental wear", "weathered stone worldbuilding patina", "ancient ruin-like erosion and patina");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Ink"))
        {
            return MapBand(value, string.Empty, "fresh ink surfaces", "subtle archival wear", "weathered symbolic ink character", "aged dream-journal ink patina");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Glass"))
        {
            return MapBand(value, string.Empty, "freshly formed glass surfaces", "subtle antique dream patina", "weathered uncanny glass character", "time-worn visionary glass patina");
        }

        if (string.Equals(material, "Yarn", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "fresh textile surfaces", "soft handled-fiber wear", "weathered fiber character", "time-worn textile patina");
        }

        if (string.Equals(material, "Glass", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "freshly formed glass surfaces", "subtle devotional patina", "weathered glass character", "aged cathedral-glass patina");
        }

        if (string.Equals(material, "Stone", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "freshly cut stone surfaces", "subtle mineral wear", "weathered carved stone", "ancient stone patina and erosion");
        }

        if (string.Equals(material, "Metal", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "freshly forged metal", "subtle metallic wear", "weathered metal patina", "oxidized time-worn metal character");
        }

        if (string.Equals(material, "Ink", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "fresh ink surfaces", "subtle archival wear", "weathered ink character", "aged archival ink patina");
        }

        if (string.Equals(material, "Paint", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, string.Empty, "freshly laid paint surfaces", "subtle aged varnish character", "weathered painted patina", "time-softened museum-surface patina");
        }

        return artStyle switch
        {
            "Painterly" => MapBand(value, string.Empty, "freshly laid paint surfaces", "subtle aged varnish character", "weathered painted patina", "time-softened museum-surface patina"),
            "Yarn Relief" => MapBand(value, string.Empty, "fresh textile surfaces", "soft handled-fiber wear", "weathered fiber character", "time-worn textile patina"),
            "Stained Glass" => MapBand(value, string.Empty, "freshly set glass surfaces", "subtle devotional patina", "weathered glass-and-lead character", "aged cathedral-glass patina"),
            "Surreal Symbolic" => MapBand(value, string.Empty, "fresh illusionistic surfaces", "slight antique dream patina", "weathered oneiric surface character", "time-worn dreamlike patina"),
            _ => MapBand(value, string.Empty, "freshly finished surfaces", "subtle patina", "weathered surface character", "time-worn patina and age marks"),
        };
    }

    private static string MapBackgroundComplexity(int value, string artStyle) => artStyle switch
    {
        "Cinematic" => MapBand(value, "minimal background", "restrained set dressing", "supporting production detail", "rich environmental staging", "densely layered cinematic environment"),
        "Painterly" => MapBand(value, "minimal background", "restrained painted backdrop", "supporting painted environment", "richly worked painterly setting", "densely layered painted environment"),
        "Yarn Relief" => MapBand(value, "minimal textile background", "restrained textile backdrop", "supporting fiber-built environment", "rich layered textile environment", "densely layered yarn-built environment"),
        "Stained Glass" => MapBand(value, "minimal glass backdrop", "restrained ornamental backdrop", "supporting iconographic setting", "rich ornamental environment", "densely layered stained-glass environment"),
        "Surreal Symbolic" => MapBand(value, "minimal dream background", "restrained surreal backdrop", "supporting symbolic environment", "rich oneiric environment", "densely layered surreal environment"),
        "Concept Art" => MapBand(value, "minimal background", "restrained worldbuilding cues", "supporting environmental design", "rich worldbuilding detail", "densely layered production environment"),
        _ => MapBand(value, "minimal background", "restrained background", "supporting environmental detail", "rich environmental detail", "densely layered environment"),
    };

    private static string MapMotionEnergy(int value, string artStyle) => artStyle switch
    {
        "Cinematic" => MapBand(value, "still cinematic frame", "gentle screen motion", "active cinematic energy", "dynamic action energy", "high kinetic cinematic motion"),
        "Painterly" => MapBand(value, "still composition", "gentle gestural motion", "active painterly movement", "dynamic brush-led motion", "high-velocity painterly motion"),
        "Yarn Relief" => MapBand(value, "still textile composition", "gentle textile drift", "active woven movement", "dynamic fiber motion", "high kinetic textile energy"),
        "Stained Glass" => MapBand(value, "still iconographic composition", "gentle directional sweep", "active glass-led movement", "dynamic lead-line motion", "high kinetic stained-glass motion"),
        "Surreal Symbolic" => MapBand(value, "still oneiric composition", "gentle dream motion", "active surreal energy", "dynamic dreamlike motion", "high kinetic dream energy"),
        _ => MapBand(value, "still composition", "gentle motion", "active scene energy", "dynamic motion", "high kinetic energy"),
    };

    private static string MapNarrativeDensity(int value, string artStyle) => artStyle switch
    {
        "Cinematic" => MapBand(value, string.Empty, "single-shot visual beat", "light narrative layering", "scene-driven storytelling cues", "densely implied cinematic narrative"),
        "Painterly" => MapBand(value, string.Empty, "single-read visual idea", "light narrative suggestion", "layered painterly storytelling", "densely implied narrative tableau"),
        "Yarn Relief" => MapBand(value, string.Empty, "single-read textile motif", "light narrative stitching", "layered textile storytelling", "densely implied fiber-built story world"),
        "Stained Glass" => MapBand(value, string.Empty, "single iconographic read", "light devotional narrative", "layered symbolic storytelling", "densely storied stained-glass tableau"),
        "Surreal Symbolic" => MapBand(value, string.Empty, "single dream image", "light symbolic narrative", "layered oneiric storytelling", "densely implied surreal narrative world"),
        "Concept Art" => MapBand(value, string.Empty, "single-read design idea", "light worldbuilding cues", "layered story-world context", "densely implied narrative worldbuilding"),
        _ => MapBand(value, string.Empty, "single-read visual idea", "light narrative layering", "layered storytelling cues", "densely implied narrative world"),
    };

    private static string MapAtmosphericDepth(int value, string artStyle) => artStyle switch
    {
        "Cinematic" => MapBand(value, string.Empty, "slight atmospheric falloff", "air-filled cinematic depth", "luminous theatrical depth", "deeply layered cinematic atmosphere"),
        "Painterly" => MapBand(value, string.Empty, "slight aerial recession", "painted atmospheric depth", "luminous painterly depth", "deeply layered atmospheric perspective"),
        "Yarn Relief" => MapBand(value, string.Empty, "slight textile depth recession", "layered fiber depth", "luminous textile spatial depth", "deeply layered relief-space perspective"),
        "Stained Glass" => MapBand(value, string.Empty, "slight glass-depth separation", "layered leaded-glass depth", "luminous glass depth", "deeply tiered stained-glass spatial layering"),
        "Surreal Symbolic" => MapBand(value, string.Empty, "slight dreamlike recession", "air-filled oneiric depth", "luminous surreal depth", "deeply layered oneiric perspective"),
        _ => MapBand(value, string.Empty, "slight atmospheric recession", "air-filled spatial depth", "luminous atmospheric depth", "deeply layered atmospheric perspective"),
    };

    private static string MapChaos(int value, string artStyle) => artStyle switch
    {
        "Cinematic" => MapBand(value, string.Empty, "controlled frame tension", "restless cinematic tension", "volatile scene energy", "orchestrated cinematic chaos"),
        "Painterly" => MapBand(value, string.Empty, "controlled asymmetry", "restless painterly tension", "volatile painterly energy", "orchestrated pictorial chaos"),
        "Yarn Relief" => MapBand(value, string.Empty, "controlled textile asymmetry", "restless fiber tension", "volatile textile energy", "orchestrated woven chaos"),
        "Stained Glass" => MapBand(value, string.Empty, "controlled ornamental asymmetry", "restless ornamental tension", "volatile iconographic energy", "orchestrated ornamental chaos"),
        "Surreal Symbolic" => MapBand(value, string.Empty, "controlled dream tension", "restless surreal tension", "volatile dream energy", "orchestrated surreal chaos"),
        _ => MapBand(value, string.Empty, "controlled asymmetry", "restless visual tension", "volatile compositional energy", "orchestrated visual chaos"),
    };

    private static string MapWhimsy(int value, string artStyle) => artStyle switch
    {
        "Surreal Symbolic" => MapBand(value, string.Empty, "subtle surreal playfulness", "playful dream logic", "strong whimsical strangeness", "bold absurdist whimsy"),
        "Stained Glass" => MapBand(value, string.Empty, "subtle folkloric whimsy", "playful iconographic tone", "strong ornamental whimsy", "bold storybook whimsy"),
        _ => MapBand(value, string.Empty, "subtle whimsy", "playful tone", "strong whimsical energy", "bold comedic whimsy"),
    };

    private static string MapTension(int value, string artStyle) => artStyle switch
    {
        "Cinematic" => MapBand(value, string.Empty, "light dramatic tension", "scene-level tension", "strong cinematic tension", "high-stakes cinematic tension"),
        "Surreal Symbolic" => MapBand(value, string.Empty, "light uncanny tension", "noticeable oneiric tension", "strong psychological tension", "intense dreamlike dread"),
        _ => MapBand(value, string.Empty, "light dramatic tension", "noticeable tension", "strong interpersonal tension", "intense dramatic tension"),
    };

    private static string MapAwe(int value, string artStyle) => artStyle switch
    {
        "Cinematic" => MapBand(value, string.Empty, "slight cinematic wonder", "atmosphere of spectacle", "strong cinematic awe", "overwhelming big-screen grandeur"),
        "Painterly" => MapBand(value, string.Empty, "slight sense of wonder", "atmosphere of reverence", "strong painterly awe", "overwhelming sublime grandeur"),
        "Stained Glass" => MapBand(value, string.Empty, "slight devotional wonder", "atmosphere of reverence", "strong sacred awe", "overwhelming cathedral-scale grandeur"),
        "Surreal Symbolic" => MapBand(value, string.Empty, "slight oneiric wonder", "dreamlike atmosphere of wonder", "strong uncanny awe", "overwhelming visionary grandeur"),
        _ => MapBand(value, string.Empty, "slight sense of wonder", "atmosphere of wonder", "strong sense of awe", "overwhelming visual grandeur"),
    };

    private static string MapSymbolism(int value, string artStyle) => artStyle switch
    {
        "Stained Glass" => MapBand(value, string.Empty, "subtle iconographic cues", "suggestive devotional motifs", "pronounced allegorical iconography", "mythic sacred symbolism"),
        "Surreal Symbolic" => MapBand(value, string.Empty, "subtle surreal symbols", "suggestive dream motifs", "pronounced allegorical symbolism", "mythic symbolic charge"),
        "Concept Art" => MapBand(value, string.Empty, "subtle worldbuilding motifs", "suggestive symbolic design cues", "pronounced allegorical design motifs", "mythic symbolic worldbuilding"),
        _ => MapBand(value, string.Empty, "subtle symbolic cues", "suggestive symbolic motifs", "pronounced allegorical symbolism", "mythic symbolic charge"),
    };

    private static string MapSaturation(int value, string artStyle, string material)
    {
        if (Matches(artStyle, material, "Stained Glass", "Glass"))
        {
            return MapBand(value, "muted chapel glass", "restrained jewel tones", "balanced stained-glass color", "rich cathedral jewel tones", "radiant jewel-tone luminosity");
        }

        if (Matches(artStyle, material, "Painterly", "Paint"))
        {
            return MapBand(value, "muted pigment saturation", "restrained atelier color", "balanced pigment saturation", "rich painterly color", "luminous painted saturation");
        }

        if (Matches(artStyle, material, "Concept Art", "Metal"))
        {
            return MapBand(value, "muted alloy color", "restrained industrial accents", "balanced production color", "rich hard-surface accents", "luminous sci-fi alloy color");
        }

        if (Matches(artStyle, material, "Concept Art", "Stone"))
        {
            return MapBand(value, "muted mineral color", "restrained earth saturation", "balanced monumental coloration", "rich environmental color", "vivid mineral and ruin coloration");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Ink"))
        {
            return MapBand(value, "muted ink value range", "restrained symbolic ink density", "balanced ink saturation", "rich surreal ink density", "deeply saturated visionary ink intensity");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Glass"))
        {
            return MapBand(value, "muted dream-glass color", "restrained surreal jewel tones", "balanced uncanny translucence", "rich surreal jewel saturation", "vivid oneiric glass saturation");
        }

        if (string.Equals(material, "Glass", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "muted glass color", "restrained jewel tones", "balanced stained-glass color", "rich jewel-toned saturation", "radiant jewel-tone saturation");
        }

        if (string.Equals(material, "Metal", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "muted metallic color", "restrained alloy tones", "balanced metallic saturation", "rich metallic color accents", "luminous alloy saturation");
        }

        if (string.Equals(material, "Stone", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "muted mineral color", "restrained earth saturation", "balanced mineral saturation", "rich stone coloration", "vivid mineral veining and color");
        }

        if (string.Equals(material, "Ink", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "muted ink value range", "restrained ink saturation", "balanced ink density", "rich ink saturation", "deeply saturated ink intensity");
        }

        if (string.Equals(material, "Paint", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "muted pigment saturation", "restrained pigment intensity", "balanced pigment saturation", "rich pigment saturation", "luminous paint saturation");
        }

        return artStyle switch
        {
            "Painterly" => MapBand(value, "muted pigment saturation", "restrained pigment intensity", "balanced pigment saturation", "rich pigment saturation", "luminous paint saturation"),
            "Stained Glass" => MapBand(value, "muted glass color", "restrained jewel tones", "balanced stained-glass color", "rich jewel-toned saturation", "radiant jewel-tone saturation"),
            "Surreal Symbolic" => MapBand(value, "muted dream color", "restrained surreal color", "balanced surreal saturation", "rich surreal saturation", "vivid oneiric color saturation"),
            _ => MapBand(value, "muted color saturation", "restrained saturation", "balanced saturation", "rich color saturation", "vivid color saturation"),
        };
    }

    private static string MapContrast(int value, string artStyle, string material)
    {
        if (Matches(artStyle, material, "Stained Glass", "Glass"))
        {
            return MapBand(value, "low contrast", "gentle lead-line contrast", "balanced devotional glass contrast", "crisp cathedral leaded contrast", "striking jewel-glass contrast");
        }

        if (Matches(artStyle, material, "Painterly", "Paint"))
        {
            return MapBand(value, "low contrast", "gentle painterly separation", "balanced atelier contrast", "crisp tonal contrast", "striking gallery-grade contrast");
        }

        if (Matches(artStyle, material, "Concept Art", "Metal"))
        {
            return MapBand(value, "low contrast", "gentle hard-surface separation", "balanced industrial contrast", "crisp reflective hard-surface contrast", "striking production-grade metal contrast");
        }

        if (Matches(artStyle, material, "Concept Art", "Stone"))
        {
            return MapBand(value, "low contrast", "gentle carved-structure contrast", "balanced monumental contrast", "crisp carved stone contrast", "striking ruin-scale stone contrast");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Ink"))
        {
            return MapBand(value, "low contrast", "gentle ink separation", "balanced symbolic ink contrast", "crisp surreal ink contrast", "striking dream-journal black-ink contrast");
        }

        if (Matches(artStyle, material, "Surreal Symbolic", "Glass"))
        {
            return MapBand(value, "low contrast", "gentle translucent separation", "balanced uncanny glass contrast", "crisp surreal glass contrast", "striking visionary jewel-glass contrast");
        }

        if (string.Equals(material, "Glass", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "low contrast", "gentle lead-line contrast", "balanced glass contrast", "crisp leaded contrast", "striking jewel-glass contrast");
        }

        if (string.Equals(material, "Metal", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "low contrast", "gentle reflective separation", "balanced metallic contrast", "crisp reflective contrast", "striking polished-metal contrast");
        }

        if (string.Equals(material, "Stone", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "low contrast", "gentle carved contrast", "balanced mineral contrast", "crisp carved contrast", "striking chiseled stone contrast");
        }

        if (string.Equals(material, "Ink", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "low contrast", "gentle ink separation", "balanced ink contrast", "crisp ink contrast", "striking black-ink contrast");
        }

        if (string.Equals(material, "Paint", StringComparison.OrdinalIgnoreCase))
        {
            return MapBand(value, "low contrast", "gentle painterly contrast", "balanced pictorial contrast", "crisp tonal contrast", "striking painterly contrast");
        }

        return artStyle switch
        {
            "Cinematic" => MapBand(value, "low contrast", "gentle tonal separation", "balanced cinematic contrast", "crisp cinematic contrast", "striking theatrical contrast"),
            "Painterly" => MapBand(value, "low contrast", "gentle painterly contrast", "balanced pictorial contrast", "crisp tonal contrast", "striking painterly contrast"),
            "Stained Glass" => MapBand(value, "low contrast", "gentle lead-line contrast", "balanced glass contrast", "crisp leaded contrast", "striking jewel-glass contrast"),
            _ => MapBand(value, "low contrast", "gentle contrast", "balanced contrast", "crisp contrast", "striking contrast"),
        };
    }

    private static bool Matches(string artStyle, string material, string expectedArtStyle, string expectedMaterial)
    {
        return string.Equals(artStyle, expectedArtStyle, StringComparison.OrdinalIgnoreCase)
            && string.Equals(material, expectedMaterial, StringComparison.OrdinalIgnoreCase);
    }

    private static string MapBand(int value, string low, string lowMid, string mid, string high, string veryHigh)
    {
        if (value <= 20) return low;
        if (value <= 40) return lowMid;
        if (value <= 60) return mid;
        if (value <= 80) return high;
        return veryHigh;
    }

    private static void AddUnique(ICollection<string> phrases, ISet<string> seen, string value)
    {
        var cleaned = Clean(value);
        if (string.IsNullOrWhiteSpace(cleaned) || !seen.Add(cleaned)) return;
        phrases.Add(cleaned);
    }

    private static string Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return string.Empty;
        var builder = new StringBuilder(value.Trim());
        builder.Replace("  ", " ");
        return builder.ToString().Trim().Trim(',');
    }

    private static string Lower(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "None", StringComparison.OrdinalIgnoreCase)) return string.Empty;
        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    private enum ArtistPhraseCategory
    {
        Hallmarks,
        Composition,
        Palette,
        Surface,
        Mood,
    }

    private sealed record ArtistInfluence(string DisplayName, int Strength, ArtistProfile? Profile);
}

