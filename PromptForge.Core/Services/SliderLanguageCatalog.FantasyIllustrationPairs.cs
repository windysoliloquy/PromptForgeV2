using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetFantasyIllustrationSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsFantasyIllustration(configuration.IntentMode))
        {
            yield break;
        }

        if (configuration.FantasyIllustrationCharacterSketch)
        {
            yield break;
        }

        var register = NormalizeFantasyIllustrationRegister(configuration.FantasyIllustrationRegister);

        if (string.Equals(register, "general-fantasy", StringComparison.Ordinal))
        {
            foreach (var collapse in BuildGeneralFantasySemanticPairCollapses(configuration))
            {
                yield return collapse;
            }
            yield break;
        }

        if (string.Equals(register, "low-magic", StringComparison.Ordinal))
        {
            foreach (var collapse in BuildLowMagicFantasySemanticPairCollapses(configuration))
            {
                yield return collapse;
            }
            yield break;
        }

        if (string.Equals(register, "high-magic", StringComparison.Ordinal))
        {
            foreach (var collapse in BuildHighMagicFantasySemanticPairCollapses(configuration))
            {
                yield return collapse;
            }
            yield break;
        }

        if (string.Equals(register, "magitech", StringComparison.Ordinal))
        {
            foreach (var collapse in BuildMagitechFantasySemanticPairCollapses(configuration))
            {
                yield return collapse;
            }
            yield break;
        }

        if (string.Equals(register, "sword-and-sorcery", StringComparison.Ordinal))
        {
            foreach (var collapse in BuildSwordAndSorceryFantasySemanticPairCollapses(configuration))
            {
                yield return collapse;
            }
        }
    }

    private static IEnumerable<PromptSemanticPairCollapse> BuildGeneralFantasySemanticPairCollapses(PromptConfiguration configuration)
    {
        // Future pad scaffolding note only:
        // X-axis: restrained -> myth-shaped
        // Y-axis: imagined -> convincing
        var stylizationRealismFusedPhrase = (GetBandIndex(configuration.Realism), GetBandIndex(configuration.Stylization)) switch
        {
            (0, 0) => "grounded imagined tale-world",
            (0, 1) => "lightly storied tale-world",
            (0, 2) => "legend-led storyworld",
            (0, 3) => "strongly myth-shaped realm",
            (0, 4) => "myth-burdened legend vision",

            (1, 0) => "grounded imagined storyworld",
            (1, 1) => "storied imagined world",
            (1, 2) => "legend-led imagined realm",
            (1, 3) => "myth-shaped imagined realm",
            (1, 4) => "myth-burdened imagined legend realm",

            (2, 0) => "grounded believable storyworld",
            (2, 1) => "storied but believable realm",
            (2, 2) => "legend-led believable realm",
            (2, 3) => "myth-shaped credible realm",
            (2, 4) => "myth-burdened believable legend realm",

            (3, 0) => "grounded physically convincing storyworld",
            (3, 1) => "storied materially convincing realm",
            (3, 2) => "legend-led materially convincing realm",
            (3, 3) => "myth-shaped materially persuasive realm",
            (3, 4) => "myth-burdened physically convincing legend realm",

            (4, 0) => "grounded lived-in fantasy realism",
            (4, 1) => "storied lived-in world realism",
            (4, 2) => "legend-led lived-in realm realism",
            (4, 3) => "myth-shaped lived-in legend realism",
            (4, 4) => "myth-burdened tangible legend realism",
            _ => string.Empty,
        };

        var firstPhrase = GetGeneralFantasyStylizationPairPhrase(configuration.Stylization);
        var secondPhrase = GetGeneralFantasyRealismPairPhrase(configuration.Realism);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(stylizationRealismFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, stylizationRealismFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: intimate -> sweeping
        // Y-axis: close -> far-set
        var framingDistanceFusedPhrase = (GetBandIndex(configuration.CameraDistance), GetBandIndex(configuration.Framing)) switch
        {
            (0, 0) => "intimate close-held view",
            (0, 1) => "contained close scene view",
            (0, 2) => "balanced close story view",
            (0, 3) => "expansive close-quarters staging",
            (0, 4) => "sweeping close hero view",

            (1, 0) => "intimate near-set view",
            (1, 1) => "contained near-scene framing",
            (1, 2) => "balanced near-scene view",
            (1, 3) => "expansive near-scene staging",
            (1, 4) => "sweeping near-hero view",

            (2, 0) => "intimate mid-scene view",
            (2, 1) => "contained mid-scene framing",
            (2, 2) => "balanced story-view framing",
            (2, 3) => "expansive mid-scene adventure view",
            (2, 4) => "sweeping mid-scene realm view",

            (3, 0) => "intimate world-revealing view",
            (3, 1) => "contained world-revealing framing",
            (3, 2) => "balanced world-revealing story view",
            (3, 3) => "expansive world-revealing adventure framing",
            (3, 4) => "sweeping world-revealing realm view",

            (4, 0) => "intimate far-set witness view",
            (4, 1) => "contained far-set scene view",
            (4, 2) => "balanced far-set story view",
            (4, 3) => "expansive far-set adventure view",
            (4, 4) => "sweeping far-set legend view",
            _ => string.Empty,
        };

        firstPhrase = GetGeneralFantasyFramingPairPhrase(configuration.Framing);
        secondPhrase = GetGeneralFantasyCameraDistancePairPhrase(configuration.CameraDistance);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(framingDistanceFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, framingDistanceFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: solemn -> mischievous
        // Y-axis: ease -> crisis
        var whimsyTensionFusedPhrase = (GetBandIndex(configuration.Tension), GetBandIndex(configuration.Whimsy)) switch
        {
            (0, 0) => "solemn fireside calm",
            (0, 1) => "warm folktale ease",
            (0, 2) => "lively tale ease",
            (0, 3) => "trickster tale ease",
            (0, 4) => "bold fairytale ease",

            (1, 0) => "grave watchfulness",
            (1, 1) => "hearth-warm watchfulness",
            (1, 2) => "playful unease",
            (1, 3) => "sly unease",
            (1, 4) => "mischief-lit unease",

            (2, 0) => "stern frontier danger",
            (2, 1) => "firelit frontier danger",
            (2, 2) => "high-hearted adventure peril",
            (2, 3) => "roguish adventure peril",
            (2, 4) => "mischief-laced peril",

            (3, 0) => "grim duel-near strain",
            (3, 1) => "ember-warm duel-near strain",
            (3, 2) => "spirited clash strain",
            (3, 3) => "taunting brink strain",
            (3, 4) => "mischief-marked brinkmanship",

            (4, 0) => "oath-bound crisis gravity",
            (4, 1) => "warm-blooded crisis resolve",
            (4, 2) => "bright-hearted crisis courage",
            (4, 3) => "wry crisis defiance",
            (4, 4) => "irreverent crisis defiance",
            _ => string.Empty,
        };

        firstPhrase = GetGeneralFantasyWhimsyPairPhrase(configuration.Whimsy);
        secondPhrase = GetGeneralFantasyTensionPairPhrase(configuration.Tension);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(whimsyTensionFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, whimsyTensionFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: grounded -> sublime
        // Y-axis: dim -> radiant
        var aweLightingFusedPhrase = (GetBandIndex(configuration.LightingIntensity), GetBandIndex(configuration.Awe)) switch
        {
            (0, 0) => "grounded ambient hush",
            (0, 1) => "old-world hush",
            (0, 2) => "legend-hushed atmosphere",
            (0, 3) => "shadowed storied grandeur",
            (0, 4) => "myth-hushed sublimity",

            (1, 0) => "grounded soft-lit calm",
            (1, 1) => "softly wonder-lit scene",
            (1, 2) => "legend-softened glow",
            (1, 3) => "soft-lit grandeur",
            (1, 4) => "glow-held sublimity",

            (2, 0) => "grounded full-scene clarity",
            (2, 1) => "clear old-world wonder",
            (2, 2) => "legend-borne clarity",
            (2, 3) => "grandeur in clear light",
            (2, 4) => "sublime clear-lit majesty",

            (3, 0) => "bright grounded clarity",
            (3, 1) => "wonder-brightened scene",
            (3, 2) => "legend-bright scene",
            (3, 3) => "grandeur-charged illumination",
            (3, 4) => "myth-bright majesty",

            (4, 0) => "radiant grounded clarity",
            (4, 1) => "radiant old-world wonder",
            (4, 2) => "legend-scale radiance",
            (4, 3) => "radiant storied grandeur",
            (4, 4) => "overwhelming mythic radiance",
            _ => string.Empty,
        };

        firstPhrase = GetGeneralFantasyAwePairPhrase(configuration.Awe);
        secondPhrase = GetGeneralFantasyLightingIntensityPairPhrase(configuration.LightingIntensity);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(aweLightingFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, aweLightingFusedPhrase);
        }

    }

    private static IEnumerable<PromptSemanticPairCollapse> BuildLowMagicFantasySemanticPairCollapses(PromptConfiguration configuration)
    {
        // Future pad scaffolding note only:
        // X-axis: frontier-grounded -> omen-burdened
        // Y-axis: imagined -> lived-in
        var stylizationRealismFusedPhrase = (GetBandIndex(configuration.Realism), GetBandIndex(configuration.Stylization)) switch
        {
            (0, 0) => "grounded imagined frontier",
            (0, 1) => "lightly storied frontier",
            (0, 2) => "old-world legend treatment",
            (0, 3) => "omen-touched borderland vision",
            (0, 4) => "omen-burdened frontier vision",

            (1, 0) => "grounded imagined frontier",
            (1, 1) => "storied imagined borderland",
            (1, 2) => "old-world imagined legend",
            (1, 3) => "omen-touched imagined realm",
            (1, 4) => "omen-burdened imagined legendworld",

            (2, 0) => "grounded believable frontier",
            (2, 1) => "storied but believable borderland",
            (2, 2) => "old-world believable legend",
            (2, 3) => "omen-touched credible realm",
            (2, 4) => "omen-burdened believable legendworld",

            (3, 0) => "grounded physically convincing frontier",
            (3, 1) => "storied materially convincing borderland",
            (3, 2) => "old-world materially convincing legend",
            (3, 3) => "omen-touched materially persuasive realm",
            (3, 4) => "omen-burdened physically convincing legendworld",

            (4, 0) => "grounded lived-in frontier realism",
            (4, 1) => "storied lived-in borderland realism",
            (4, 2) => "old-world lived-in legend realism",
            (4, 3) => "omen-touched lived-in realm realism",
            (4, 4) => "omen-burdened tangible legend realism",
            _ => string.Empty,
        };

        var firstPhrase = GetLowMagicFantasyStylizationPairPhrase(configuration.Stylization);
        var secondPhrase = GetLowMagicFantasyRealismPairPhrase(configuration.Realism);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(stylizationRealismFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, stylizationRealismFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: fireside -> kingdom-edge
        // Y-axis: close -> realm-edge
        var framingDistanceFusedPhrase = (GetBandIndex(configuration.CameraDistance), GetBandIndex(configuration.Framing)) switch
        {
            (0, 0) => "fireside close view",
            (0, 1) => "path-side close view",
            (0, 2) => "old-road close view",
            (0, 3) => "borderland close view",
            (0, 4) => "kingdom-edge close view",

            (1, 0) => "fireside travel view",
            (1, 1) => "path-side travel view",
            (1, 2) => "old-road travel view",
            (1, 3) => "borderland travel view",
            (1, 4) => "kingdom-edge travel view",

            (2, 0) => "fireside figure-and-relic view",
            (2, 1) => "path-side figure-and-relic view",
            (2, 2) => "old-road figure-and-relic view",
            (2, 3) => "borderland figure-and-relic view",
            (2, 4) => "kingdom-edge figure-and-relic view",

            (3, 0) => "fireside frontier-revealing view",
            (3, 1) => "path-side frontier-revealing view",
            (3, 2) => "old-road frontier-revealing view",
            (3, 3) => "borderland frontier-revealing view",
            (3, 4) => "kingdom-edge frontier-revealing view",

            (4, 0) => "fireside realm-edge view",
            (4, 1) => "path-side realm-edge view",
            (4, 2) => "old-road realm-edge view",
            (4, 3) => "borderland realm-edge view",
            (4, 4) => "kingdom-edge realm-edge view",
            _ => string.Empty,
        };

        firstPhrase = GetLowMagicFantasyFramingPairPhrase(configuration.Framing);
        secondPhrase = GetLowMagicFantasyCameraDistancePairPhrase(configuration.CameraDistance);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(framingDistanceFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, framingDistanceFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: old-world -> kingdom-sublime
        // Y-axis: chapel-dim -> omen-lit
        var aweLightingFusedPhrase = (GetBandIndex(configuration.LightingIntensity), GetBandIndex(configuration.Awe)) switch
        {
            (0, 0) => "old-world hush",
            (0, 1) => "wayside wonder in shadow",
            (0, 2) => "legend-hushed air",
            (0, 3) => "shadow-held ancestral grandeur",
            (0, 4) => "shadow-held kingdom sublimity",

            (1, 0) => "weather-muted old-world wonder",
            (1, 1) => "weather-softened roadside wonder",
            (1, 2) => "weather-held legend glow",
            (1, 3) => "weather-softened ancestral grandeur",
            (1, 4) => "weather-veiled kingdom sublimity",

            (2, 0) => "torch-and-daylight old-world wonder",
            (2, 1) => "torch-and-daylight roadside wonder",
            (2, 2) => "torch-and-daylight legend clarity",
            (2, 3) => "torch-and-daylight ancestral grandeur",
            (2, 4) => "torch-and-daylight kingdom majesty",

            (3, 0) => "stormbreak old-world wonder",
            (3, 1) => "stormbreak roadside wonder",
            (3, 2) => "stormbreak legend presence",
            (3, 3) => "stormbreak ancestral grandeur",
            (3, 4) => "stormbreak kingdom sublimity",

            (4, 0) => "omen-lit old-world wonder",
            (4, 1) => "omen-lit roadside wonder",
            (4, 2) => "omen-lit burdened legend",
            (4, 3) => "omen-lit ancestral grandeur",
            (4, 4) => "omen-lit age-and-kingdom sublimity",
            _ => string.Empty,
        };

        firstPhrase = GetLowMagicFantasyAwePairPhrase(configuration.Awe);
        secondPhrase = GetLowMagicFantasyLightingIntensityPairPhrase(configuration.LightingIntensity);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(aweLightingFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, aweLightingFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: grave -> strange-tale
        // Y-axis: peril -> curse-crisis
        var whimsyTensionFusedPhrase = (GetBandIndex(configuration.Tension), GetBandIndex(configuration.Whimsy)) switch
        {
            (0, 0) => "grave fireside peril",
            (0, 1) => "warm fireside peril",
            (0, 2) => "road-worn peril",
            (0, 3) => "oddity-marked peril",
            (0, 4) => "strange-tale peril",

            (1, 0) => "grave watchfulness",
            (1, 1) => "hearth-warm watchfulness",
            (1, 2) => "road-worn unease",
            (1, 3) => "oddity-marked unease",
            (1, 4) => "strange-tale unease",

            (2, 0) => "stern oath-and-pursuit danger",
            (2, 1) => "ember-warm pursuit danger",
            (2, 2) => "road-worn pursuit danger",
            (2, 3) => "oddity-marked pursuit danger",
            (2, 4) => "strange-tale pursuit danger",

            (3, 0) => "grim feud-near strain",
            (3, 1) => "ember-warm feud strain",
            (3, 2) => "road-worn feud strain",
            (3, 3) => "oddity-marked feud strain",
            (3, 4) => "strange-tale feud strain",

            (4, 0) => "curse-bound blood crisis",
            (4, 1) => "warm-blooded crisis resolve",
            (4, 2) => "road-worn crisis resolve",
            (4, 3) => "oddity-marked crisis defiance",
            (4, 4) => "strange-tale crisis defiance",
            _ => string.Empty,
        };

        firstPhrase = GetLowMagicFantasyWhimsyPairPhrase(configuration.Whimsy);
        secondPhrase = GetLowMagicFantasyTensionPairPhrase(configuration.Tension);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(whimsyTensionFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, whimsyTensionFusedPhrase);
        }
    }

    private static IEnumerable<PromptSemanticPairCollapse> BuildHighMagicFantasySemanticPairCollapses(PromptConfiguration configuration)
    {
        // Future pad scaffolding note only:
        // X-axis: spellworld-grounded -> radiant-mythworld
        // Y-axis: enchanted -> convincing
        var stylizationRealismFusedPhrase = (GetBandIndex(configuration.Realism), GetBandIndex(configuration.Stylization)) switch
        {
            (0, 0) => "grounded enchanted spellworld",
            (0, 1) => "lightly enchanted spellworld",
            (0, 2) => "ritual-shaped spellworld",
            (0, 3) => "luminous mythworld vision",
            (0, 4) => "radiant mythworld vision",

            (1, 0) => "grounded enchanted world",
            (1, 1) => "lightly enchanted realm",
            (1, 2) => "ritual-shaped enchanted realm",
            (1, 3) => "luminous enchanted mythworld",
            (1, 4) => "radiant enchanted mythworld",

            (2, 0) => "grounded believable spellworld",
            (2, 1) => "believable enchanted realm",
            (2, 2) => "ritual-shaped believable realm",
            (2, 3) => "luminous spellwrought realm",
            (2, 4) => "radiant credible mythworld",

            (3, 0) => "grounded materially convincing spellworld",
            (3, 1) => "materially convincing enchanted realm",
            (3, 2) => "ritual-shaped materially convincing realm",
            (3, 3) => "luminous materially persuasive mythworld",
            (3, 4) => "radiant materially persuasive mythworld",

            (4, 0) => "grounded lived-in spellworld realism",
            (4, 1) => "lived-in enchanted realm realism",
            (4, 2) => "ritual-shaped lived-in realm realism",
            (4, 3) => "luminous lived-in mythworld realism",
            (4, 4) => "radiant lived-in mythworld realism",
            _ => string.Empty,
        };

        var firstPhrase = GetHighMagicFantasyStylizationPairPhrase(configuration.Stylization);
        var secondPhrase = GetHighMagicFantasyRealismPairPhrase(configuration.Realism);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(stylizationRealismFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, stylizationRealismFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: ritual -> citadel
        // Y-axis: close-up -> power vista
        var framingDistanceFusedPhrase = (GetBandIndex(configuration.CameraDistance), GetBandIndex(configuration.Framing)) switch
        {
            (0, 0) => "ritual close-up",
            (0, 1) => "sanctum close-up",
            (0, 2) => "spellcourt close-up",
            (0, 3) => "enchanted-realm close-up",
            (0, 4) => "citadel close-up",

            (1, 0) => "ritual scene",
            (1, 1) => "sanctum scene",
            (1, 2) => "spellcourt scene",
            (1, 3) => "enchanted-realm scene",
            (1, 4) => "citadel scene",

            (2, 0) => "ritual chamber view",
            (2, 1) => "sanctum chamber view",
            (2, 2) => "spellcourt chamber view",
            (2, 3) => "enchanted-realm chamber view",
            (2, 4) => "citadel chamber view",

            (3, 0) => "ritual world view",
            (3, 1) => "sanctum world view",
            (3, 2) => "spellcourt world view",
            (3, 3) => "enchanted-realm world view",
            (3, 4) => "citadel world view",

            (4, 0) => "ritual power vista",
            (4, 1) => "sanctum power vista",
            (4, 2) => "spellcourt power vista",
            (4, 3) => "enchanted-realm power vista",
            (4, 4) => "citadel power vista",
            _ => string.Empty,
        };

        firstPhrase = GetHighMagicFantasyFramingPairPhrase(configuration.Framing);
        secondPhrase = GetHighMagicFantasyCameraDistancePairPhrase(configuration.CameraDistance);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(framingDistanceFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, framingDistanceFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: spellbound -> sorcerous
        // Y-axis: sigil-lit -> celestial
        var aweLightingFusedPhrase = (GetBandIndex(configuration.LightingIntensity), GetBandIndex(configuration.Awe)) switch
        {
            (0, 0) => "sigil-lit wonder",
            (0, 1) => "sigil-lit spellbound wonder",
            (0, 2) => "sigil-lit mythic wonder",
            (0, 3) => "sigil-lit sorcerous grandeur",
            (0, 4) => "sigil-lit celestial grandeur",

            (1, 0) => "enchanted wonder",
            (1, 1) => "enchanted spellbound wonder",
            (1, 2) => "enchanted mythic wonder",
            (1, 3) => "enchanted sorcerous grandeur",
            (1, 4) => "enchanted celestial grandeur",

            (2, 0) => "spell-and-sky wonder",
            (2, 1) => "spell-and-sky spellbound wonder",
            (2, 2) => "spell-and-sky mythic wonder",
            (2, 3) => "spell-and-sky sorcerous grandeur",
            (2, 4) => "spell-and-sky celestial grandeur",

            (3, 0) => "ritual wonder",
            (3, 1) => "ritual spellbound wonder",
            (3, 2) => "ritual mythic wonder",
            (3, 3) => "ritual sorcerous grandeur",
            (3, 4) => "ritual celestial grandeur",

            (4, 0) => "celestial wonder",
            (4, 1) => "celestial spellbound wonder",
            (4, 2) => "celestial mythic wonder",
            (4, 3) => "celestial sorcerous grandeur",
            (4, 4) => "celestial radiance",
            _ => string.Empty,
        };

        firstPhrase = GetHighMagicFantasyAwePairPhrase(configuration.Awe);
        secondPhrase = GetHighMagicFantasyLightingIntensityPairPhrase(configuration.LightingIntensity);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(aweLightingFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, aweLightingFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: solemn -> quickened
        // Y-axis: peril -> cataclysm
        var whimsyTensionFusedPhrase = (GetBandIndex(configuration.Tension), GetBandIndex(configuration.Whimsy)) switch
        {
            (0, 0) => "solemn magical peril",
            (0, 1) => "warm magical peril",
            (0, 2) => "buoyant magical peril",
            (0, 3) => "spirited magical peril",
            (0, 4) => "quickened magical peril",

            (1, 0) => "solemn ritual unease",
            (1, 1) => "warm ritual unease",
            (1, 2) => "buoyant ritual unease",
            (1, 3) => "spirited ritual unease",
            (1, 4) => "quickened ritual unease",

            (2, 0) => "stern spell-conflict",
            (2, 1) => "warm spell-conflict",
            (2, 2) => "buoyant spell-conflict",
            (2, 3) => "spirited spell-conflict",
            (2, 4) => "quickened spell-conflict",

            (3, 0) => "grim ward-break strain",
            (3, 1) => "warm ward-break strain",
            (3, 2) => "buoyant ward-break strain",
            (3, 3) => "spirited ward-break strain",
            (3, 4) => "quickened ward-break strain",

            (4, 0) => "cataclysm-near gravity",
            (4, 1) => "warm-blooded cataclysm resolve",
            (4, 2) => "buoyant cataclysm resolve",
            (4, 3) => "spirited cataclysm defiance",
            (4, 4) => "quickened cataclysm defiance",
            _ => string.Empty,
        };

        firstPhrase = GetHighMagicFantasyWhimsyPairPhrase(configuration.Whimsy);
        secondPhrase = GetHighMagicFantasyTensionPairPhrase(configuration.Tension);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(whimsyTensionFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, whimsyTensionFusedPhrase);
        }
    }

    private static IEnumerable<PromptSemanticPairCollapse> BuildSwordAndSorceryFantasySemanticPairCollapses(PromptConfiguration configuration)
    {
        // Future pad scaffolding note only:
        // X-axis: steel-first -> blood-warm pulp
        // Y-axis: pulp vision -> ruin-world realism
        var stylizationRealismFusedPhrase = (GetBandIndex(configuration.Realism), GetBandIndex(configuration.Stylization)) switch
        {
            (0, 0) => "steel-first pulp vision",
            (0, 1) => "hard-traveled pulp vision",
            (0, 2) => "pulp-forged vision",
            (0, 3) => "ruin-charged vision",
            (0, 4) => "blood-warm pulp vision",

            (1, 0) => "steel-first survival world",
            (1, 1) => "hard-traveled survival world",
            (1, 2) => "pulp-forged survival world",
            (1, 3) => "ruin-charged survival world",
            (1, 4) => "blood-warm survival world",

            (2, 0) => "steel-first believable survival world",
            (2, 1) => "hard-traveled believable survival world",
            (2, 2) => "pulp-forged survival realism",
            (2, 3) => "ruin-charged believable survival world",
            (2, 4) => "blood-warm survival realism",

            (3, 0) => "steel-first brutal realism",
            (3, 1) => "hard-traveled brutal realism",
            (3, 2) => "pulp-forged brutal realism",
            (3, 3) => "ruin-charged brutal realism",
            (3, 4) => "blood-warm brutal realism",

            (4, 0) => "steel-first lived-in ruin-world realism",
            (4, 1) => "hard-traveled lived-in ruin-world realism",
            (4, 2) => "pulp-forged lived-in ruin-world realism",
            (4, 3) => "ruin-charged lived-in ruin-world realism",
            (4, 4) => "blood-warm lived-in ruin-world realism",
            _ => string.Empty,
        };

        var firstPhrase = GetSwordAndSorceryFantasyStylizationPairPhrase(configuration.Stylization);
        var secondPhrase = GetSwordAndSorceryFantasyRealismPairPhrase(configuration.Realism);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(stylizationRealismFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, stylizationRealismFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: blade-near -> throne-wasteland
        // Y-axis: close-up -> stronghold vista
        var framingDistanceFusedPhrase = (GetBandIndex(configuration.CameraDistance), GetBandIndex(configuration.Framing)) switch
        {
            (0, 0) => "blade-near close-up",
            (0, 1) => "chamber-side close-up",
            (0, 2) => "peril-world close-up",
            (0, 3) => "arena close-up",
            (0, 4) => "throne-side close-up",

            (1, 0) => "blade-near scene",
            (1, 1) => "chamber-side scene",
            (1, 2) => "peril-world scene",
            (1, 3) => "arena scene",
            (1, 4) => "throne-wasteland scene",

            (2, 0) => "blade-near figure view",
            (2, 1) => "chamber-side figure view",
            (2, 2) => "peril-world figure view",
            (2, 3) => "arena figure view",
            (2, 4) => "throne-wasteland figure view",

            (3, 0) => "blade-near ruin view",
            (3, 1) => "chamber-side ruin view",
            (3, 2) => "peril-world ruin view",
            (3, 3) => "arena ruin view",
            (3, 4) => "throne-wasteland ruin view",

            (4, 0) => "blade-foreground vista",
            (4, 1) => "chamber-side stronghold vista",
            (4, 2) => "peril-world stronghold vista",
            (4, 3) => "arena stronghold vista",
            (4, 4) => "throne-wasteland stronghold vista",
            _ => string.Empty,
        };

        firstPhrase = GetSwordAndSorceryFantasyFramingPairPhrase(configuration.Framing);
        secondPhrase = GetSwordAndSorceryFantasyCameraDistancePairPhrase(configuration.CameraDistance);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(framingDistanceFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, framingDistanceFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: predatory -> empire-idol
        // Y-axis: torchlit -> gold-and-fire
        var aweLightingFusedPhrase = (GetBandIndex(configuration.LightingIntensity), GetBandIndex(configuration.Awe)) switch
        {
            (0, 0) => "torchlit predatory wonder",
            (0, 1) => "torchlit brutal wonder",
            (0, 2) => "torchlit ruin wonder",
            (0, 3) => "torchlit idol grandeur",
            (0, 4) => "torchlit empire sublimity",

            (1, 0) => "heat-muted predatory wonder",
            (1, 1) => "heat-muted brutal grandeur",
            (1, 2) => "heat-muted ruin wonder",
            (1, 3) => "heat-muted idol grandeur",
            (1, 4) => "heat-muted empire sublimity",

            (2, 0) => "fire-and-daylight predatory wonder",
            (2, 1) => "fire-and-daylight brutal wonder",
            (2, 2) => "fire-and-daylight ruin wonder",
            (2, 3) => "fire-and-daylight idol grandeur",
            (2, 4) => "fire-and-daylight empire sublimity",

            (3, 0) => "furnace-and-sun predatory wonder",
            (3, 1) => "furnace-and-sun brutal wonder",
            (3, 2) => "furnace-and-sun ruin grandeur",
            (3, 3) => "furnace-and-sun idol grandeur",
            (3, 4) => "furnace-and-sun empire sublimity",

            (4, 0) => "gold-and-fire predatory wonder",
            (4, 1) => "gold-and-fire brutal wonder",
            (4, 2) => "gold-and-fire ruin wonder",
            (4, 3) => "gold-and-fire idol grandeur",
            (4, 4) => "gold-and-fire empire sublimity",
            _ => string.Empty,
        };

        firstPhrase = GetSwordAndSorceryFantasyAwePairPhrase(configuration.Awe);
        secondPhrase = GetSwordAndSorceryFantasyLightingIntensityPairPhrase(configuration.LightingIntensity);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(aweLightingFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, aweLightingFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: grave -> pulp-charged
        // Y-axis: peril -> blood-crisis
        var whimsyTensionFusedPhrase = (GetBandIndex(configuration.Tension), GetBandIndex(configuration.Whimsy)) switch
        {
            (0, 0) => "grave peril",
            (0, 1) => "warm peril",
            (0, 2) => "swaggering peril",
            (0, 3) => "pulp-odd peril",
            (0, 4) => "savage-odd peril",

            (1, 0) => "grave ambush",
            (1, 1) => "warm ambush",
            (1, 2) => "swaggering ambush",
            (1, 3) => "pulp-charged ambush",
            (1, 4) => "savage-odd ambush",

            (2, 0) => "grim blade-and-pursuit danger",
            (2, 1) => "warm-blooded pursuit danger",
            (2, 2) => "swaggering pursuit danger",
            (2, 3) => "pulp-odd pursuit danger",
            (2, 4) => "savage-odd pursuit danger",

            (3, 0) => "grim cult-and-usurpation strain",
            (3, 1) => "warm-blooded usurpation strain",
            (3, 2) => "swaggering usurpation strain",
            (3, 3) => "pulp-odd usurpation strain",
            (3, 4) => "savage usurpation strain",

            (4, 0) => "throne-and-blood crisis",
            (4, 1) => "warm-blooded crisis resolve",
            (4, 2) => "swaggering crisis resolve",
            (4, 3) => "pulp-odd crisis defiance",
            (4, 4) => "savage-odd crisis defiance",
            _ => string.Empty,
        };

        firstPhrase = GetSwordAndSorceryFantasyWhimsyPairPhrase(configuration.Whimsy);
        secondPhrase = GetSwordAndSorceryFantasyTensionPairPhrase(configuration.Tension);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(whimsyTensionFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, whimsyTensionFusedPhrase);
        }
    }

    private static IEnumerable<PromptSemanticPairCollapse> BuildMagitechFantasySemanticPairCollapses(PromptConfiguration configuration)
    {
        // Future pad scaffolding note only:
        // X-axis: engineered -> built-wonder
        // Y-axis: artificer -> infrastructure-real
        var stylizationRealismFusedPhrase = (GetBandIndex(configuration.Realism), GetBandIndex(configuration.Stylization)) switch
        {
            (0, 0) => "engineered arcana vision",
            (0, 1) => "built-arcana vision",
            (0, 2) => "guild-forged arcana vision",
            (0, 3) => "infrastructure-charged vision",
            (0, 4) => "built-wonder vision",

            (1, 0) => "engineered artificer world",
            (1, 1) => "built-arcana artificer world",
            (1, 2) => "guild-forged artificer world",
            (1, 3) => "infrastructure-charged artificer world",
            (1, 4) => "built-wonder artificer world",

            (2, 0) => "engineered believable arcana",
            (2, 1) => "built-arcana credible world",
            (2, 2) => "guild-forged believable world",
            (2, 3) => "infrastructure-charged believable world",
            (2, 4) => "built-wonder believable world",

            (3, 0) => "engineered operational realism",
            (3, 1) => "built-arcana operational realism",
            (3, 2) => "guild-forged operational realism",
            (3, 3) => "infrastructure-charged operational realism",
            (3, 4) => "built-wonder operational realism",

            (4, 0) => "engineered infrastructure realism",
            (4, 1) => "built-arcana infrastructure realism",
            (4, 2) => "guild-forged infrastructure realism",
            (4, 3) => "infrastructure-charged world realism",
            (4, 4) => "built-wonder infrastructure realism",
            _ => string.Empty,
        };

        var firstPhrase = GetMagitechFantasyStylizationPairPhrase(configuration.Stylization);
        var secondPhrase = GetMagitechFantasyRealismPairPhrase(configuration.Realism);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(stylizationRealismFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, stylizationRealismFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: workshop -> transit-tower
        // Y-axis: component-close -> civic-grid
        var framingDistanceFusedPhrase = (GetBandIndex(configuration.CameraDistance), GetBandIndex(configuration.Framing)) switch
        {
            (0, 0) => "workshop close-up",
            (0, 1) => "station close-up",
            (0, 2) => "apparatus close-up",
            (0, 3) => "infrastructure close-up",
            (0, 4) => "tower close-up",

            (1, 0) => "workshop scene",
            (1, 1) => "station scene",
            (1, 2) => "apparatus scene",
            (1, 3) => "infrastructure scene",
            (1, 4) => "tower scene",

            (2, 0) => "workshop apparatus view",
            (2, 1) => "station apparatus view",
            (2, 2) => "apparatus-world view",
            (2, 3) => "infrastructure apparatus view",
            (2, 4) => "tower machine view",

            (3, 0) => "workshop system view",
            (3, 1) => "station system view",
            (3, 2) => "apparatus system view",
            (3, 3) => "infrastructure system view",
            (3, 4) => "tower system view",

            (4, 0) => "workshop network vista",
            (4, 1) => "station grid vista",
            (4, 2) => "apparatus-grid vista",
            (4, 3) => "infrastructure grid vista",
            (4, 4) => "transit-tower grid vista",
            _ => string.Empty,
        };

        firstPhrase = GetMagitechFantasyFramingPairPhrase(configuration.Framing);
        secondPhrase = GetMagitechFantasyCameraDistancePairPhrase(configuration.CameraDistance);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(framingDistanceFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, framingDistanceFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: engineered -> built-power
        // Y-axis: chamber-lit -> installation-scale
        var aweLightingFusedPhrase = (GetBandIndex(configuration.LightingIntensity), GetBandIndex(configuration.Awe)) switch
        {
            (0, 0) => "chamber wonder",
            (0, 1) => "chamber engineered wonder",
            (0, 2) => "chamber built wonder",
            (0, 3) => "chamber power grandeur",
            (0, 4) => "chamber built-power sublimity",

            (1, 0) => "regulated wonder",
            (1, 1) => "regulated engineered wonder",
            (1, 2) => "regulated constructed wonder",
            (1, 3) => "regulated power grandeur",
            (1, 4) => "regulated built-power sublimity",

            (2, 0) => "lamp-and-conduit wonder",
            (2, 1) => "lamp-and-conduit engineered wonder",
            (2, 2) => "lamp-and-conduit built wonder",
            (2, 3) => "lamp-and-conduit power grandeur",
            (2, 4) => "lamp-and-conduit built-power sublimity",

            (3, 0) => "grid-fed wonder",
            (3, 1) => "grid-fed engineered wonder",
            (3, 2) => "grid-fed built wonder",
            (3, 3) => "grid-fed power grandeur",
            (3, 4) => "grid-fed built-power sublimity",

            (4, 0) => "installation-scale wonder",
            (4, 1) => "installation-scale engineered wonder",
            (4, 2) => "installation-scale constructed wonder",
            (4, 3) => "installation-scale power grandeur",
            (4, 4) => "installation-scale built-power sublimity",
            _ => string.Empty,
        };

        firstPhrase = GetMagitechFantasyAwePairPhrase(configuration.Awe);
        secondPhrase = GetMagitechFantasyLightingIntensityPairPhrase(configuration.LightingIntensity);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(aweLightingFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, aweLightingFusedPhrase);
        }

        // Future pad scaffolding note only:
        // X-axis: austere -> quickened
        // Y-axis: strain -> containment-failure
        var whimsyTensionFusedPhrase = (GetBandIndex(configuration.Tension), GetBandIndex(configuration.Whimsy)) switch
        {
            (0, 0) => "austere operational strain",
            (0, 1) => "warm operational strain",
            (0, 2) => "lively operational strain",
            (0, 3) => "spirited operational strain",
            (0, 4) => "quickened operational strain",

            (1, 0) => "austere warded instability",
            (1, 1) => "warm warded instability",
            (1, 2) => "charged warded instability",
            (1, 3) => "spirited warded instability",
            (1, 4) => "quickened warded instability",

            (2, 0) => "stern routing tension",
            (2, 1) => "warm routing tension",
            (2, 2) => "lively routing tension",
            (2, 3) => "spirited routing tension",
            (2, 4) => "quickened routing tension",

            (3, 0) => "grim breach-and-overload strain",
            (3, 1) => "warm-blooded overload strain",
            (3, 2) => "lively overload strain",
            (3, 3) => "spirited overload strain",
            (3, 4) => "quickened overload strain",

            (4, 0) => "containment-failure gravity",
            (4, 1) => "warm-blooded containment resolve",
            (4, 2) => "steady containment resolve",
            (4, 3) => "spirited containment defiance",
            (4, 4) => "quickened containment defiance",
            _ => string.Empty,
        };

        firstPhrase = GetMagitechFantasyWhimsyPairPhrase(configuration.Whimsy);
        secondPhrase = GetMagitechFantasyTensionPairPhrase(configuration.Tension);

        if (!string.IsNullOrWhiteSpace(firstPhrase)
            && !string.IsNullOrWhiteSpace(secondPhrase)
            && !string.IsNullOrWhiteSpace(whimsyTensionFusedPhrase))
        {
            yield return new PromptSemanticPairCollapse(firstPhrase, secondPhrase, whimsyTensionFusedPhrase);
        }
    }

    private static string GetGeneralFantasyStylizationPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded tale-world treatment",
            1 => "lightly storied image shaping",
            2 => "legend-led illustration treatment",
            3 => "strongly legend-shaped rendering",
            4 => "myth-burdened visual",
            _ => string.Empty,
        };
    }

    private static string GetGeneralFantasyRealismPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "minimal realism emphasis",
            1 => "loosely believable storyworld realism",
            2 => "moderately convincing imaginative realism",
            3 => "high physical believability in a legend-touched world",
            4 => "strongly convincing lived-in fantasy realism",
            _ => string.Empty,
        };
    }

    private static string GetGeneralFantasyFramingPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "intimate story framing",
            1 => "contained narrative framing",
            2 => "balanced tale-world framing",
            3 => "expansive adventure framing",
            4 => "sweeping legend-scale framing",
            _ => string.Empty,
        };
    }

    private static string GetGeneralFantasyCameraDistancePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "close character-centric view",
            1 => "near scene view",
            2 => "balanced story-view distance",
            3 => "wider world-revealing distance",
            4 => "far-set realm-scale distance",
            _ => string.Empty,
        };
    }

    private static string GetGeneralFantasyWhimsyPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "serious tale tone",
            1 => "light folktale warmth",
            2 => "playful storybook energy",
            3 => "trickster play",
            4 => "bold fairytale mischief",
            _ => string.Empty,
        };
    }

    private static string GetGeneralFantasyTensionPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "low peril",
            1 => "watchful unease",
            2 => "frontier danger",
            3 => "duel-near strain",
            4 => "oath-and-blood crisis tension",
            _ => string.Empty,
        };
    }

    private static string GetGeneralFantasyAwePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded scale",
            1 => "slight old-world wonder",
            2 => "atmosphere of legend",
            3 => "strong sense of storied grandeur",
            4 => "overwhelming mythic sublimity",
            _ => string.Empty,
        };
    }

    private static string GetGeneralFantasyLightingIntensityPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "dim ambient illumination",
            1 => "soft story lighting",
            2 => "balanced illustrative illumination",
            3 => "vivid realm-light emphasis",
            4 => "radiant legend-scale illumination",
            _ => string.Empty,
        };
    }

    private static string GetLowMagicFantasyStylizationPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded frontier treatment",
            1 => "lightly folkloric image shaping",
            2 => "old-world illustration treatment",
            3 => "severe legend-shaped rendering",
            4 => "omen-burdened visual",
            _ => string.Empty,
        };
    }

    private static string GetLowMagicFantasyRealismPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "minimal realism emphasis",
            1 => "loosely grounded frontier realism",
            2 => "moderately convincing old-world realism",
            3 => "high physical believability in a hardship-marked world",
            4 => "strongly convincing lived-in legend realism",
            _ => string.Empty,
        };
    }

    private static string GetLowMagicFantasyFramingPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "intimate fireside framing",
            1 => "contained path-side framing",
            2 => "balanced old-road framing",
            3 => "expansive borderland framing",
            4 => "sweeping kingdom-edge framing",
            _ => string.Empty,
        };
    }

    private static string GetLowMagicFantasyCameraDistancePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "close face-and-relic view",
            1 => "near travel-scene view",
            2 => "balanced road-and-figure distance",
            3 => "wider frontier-revealing distance",
            4 => "far-set realm-edge distance",
            _ => string.Empty,
        };
    }

    private static string GetLowMagicFantasyAwePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded scale",
            1 => "slight old-world wonder",
            2 => "atmosphere of burdened legend",
            3 => "strong sense of ancestral grandeur",
            4 => "overwhelming age-and-kingdom sublimity",
            _ => string.Empty,
        };
    }

    private static string GetLowMagicFantasyLightingIntensityPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "dim chapel-like illumination",
            1 => "soft weather-muted lighting",
            2 => "balanced torch-and-daylight illumination",
            3 => "vivid stormbreak illumination",
            4 => "radiant omen-lit illumination",
            _ => string.Empty,
        };
    }

    private static string GetLowMagicFantasyWhimsyPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "serious fireside tone",
            1 => "light folktale warmth",
            2 => "rustic storybook play",
            3 => "superstition-tinged levity",
            4 => "bold fairytale oddity",
            _ => string.Empty,
        };
    }

    private static string GetLowMagicFantasyTensionPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "low peril",
            1 => "watchful unease",
            2 => "oath-and-pursuit danger",
            3 => "feud-near strain",
            4 => "curse-and-blood crisis tension",
            _ => string.Empty,
        };
    }

    private static string GetHighMagicFantasyStylizationPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded spellworld treatment",
            1 => "lightly enchanted image shaping",
            2 => "sorcery-led illustration treatment",
            3 => "strongly spellwrought rendering",
            4 => "radiant mythworld visual",
            _ => string.Empty,
        };
    }

    private static string GetHighMagicFantasyRealismPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "minimal realism emphasis",
            1 => "loosely believable spellworld realism",
            2 => "moderately convincing enchanted-world realism",
            3 => "high believability in overtly magical conditions",
            4 => "strongly convincing grand-magic realism",
            _ => string.Empty,
        };
    }

    private static string GetHighMagicFantasyFramingPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "intimate ritual framing",
            1 => "contained sanctum framing",
            2 => "balanced spellcourt framing",
            3 => "expansive enchanted-realm framing",
            4 => "sweeping mythic-citadel framing",
            _ => string.Empty,
        };
    }

    private static string GetHighMagicFantasyCameraDistancePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "close sigil-and-face view",
            1 => "near ritual-scene view",
            2 => "balanced figure-and-chamber distance",
            3 => "wider spellworld-revealing distance",
            4 => "far-set realm-of-power distance",
            _ => string.Empty,
        };
    }

    private static string GetHighMagicFantasyAwePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded scale",
            1 => "slight spellbound wonder",
            2 => "atmosphere of enchantment",
            3 => "strong sense of mythic majesty",
            4 => "overwhelming sorcerous grandeur",
            _ => string.Empty,
        };
    }

    private static string GetHighMagicFantasyLightingIntensityPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "dim sigil-lit illumination",
            1 => "soft enchanted lighting",
            2 => "balanced spell-and-sky illumination",
            3 => "vivid ritual illumination",
            4 => "radiant celestial illumination",
            _ => string.Empty,
        };
    }

    private static string GetHighMagicFantasyWhimsyPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "serious mythic tone",
            1 => "light enchanted play",
            2 => "playful enchantment",
            3 => "fey-charged playfulness",
            4 => "exuberant magical mischief",
            _ => string.Empty,
        };
    }

    private static string GetHighMagicFantasyTensionPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "low magical peril",
            1 => "ritual unease",
            2 => "charged spell-conflict tension",
            3 => "ward-break strain",
            4 => "cataclysm-near sorcerous tension",
            _ => string.Empty,
        };
    }

    private static string GetMagitechFantasyStylizationPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded engineered-arcana treatment",
            1 => "lightly fabricated image shaping",
            2 => "artificer-led illustration treatment",
            3 => "strongly infrastructure-shaped rendering",
            4 => "built-wonder visual language",
            _ => string.Empty,
        };
    }

    private static string GetMagitechFantasyRealismPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "omit explicit realism",
            1 => "loosely believable artificer realism",
            2 => "moderately convincing constructed-enchantment realism",
            3 => "high believability in rune-powered infrastructure",
            4 => "strongly convincing magitech world realism",
            _ => string.Empty,
        };
    }

    private static string GetMagitechFantasyFramingPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "intimate workshop framing",
            1 => "contained station framing",
            2 => "balanced apparatus-world framing",
            3 => "expansive infrastructure framing",
            4 => "sweeping transit-and-tower framing",
            _ => string.Empty,
        };
    }

    private static string GetMagitechFantasyCameraDistancePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "close component-and-hand view",
            1 => "near chamber-scene view",
            2 => "balanced figure-and-apparatus distance",
            3 => "wider infrastructure-revealing distance",
            4 => "far-set civic-grid distance",
            _ => string.Empty,
        };
    }

    private static string GetMagitechFantasyAwePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded scale",
            1 => "slight engineered wonder",
            2 => "atmosphere of constructed marvel",
            3 => "strong sense of infrastructure grandeur",
            4 => "overwhelming built-power sublimity",
            _ => string.Empty,
        };
    }

    private static string GetMagitechFantasyLightingIntensityPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "dim chamber illumination",
            1 => "soft regulated lighting",
            2 => "balanced lamp-and-conduit illumination",
            3 => "vivid grid-fed illumination",
            4 => "radiant installation-scale illumination",
            _ => string.Empty,
        };
    }

    private static string GetMagitechFantasyWhimsyPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "serious engineered tone",
            1 => "light workshop play",
            2 => "playful tinkering",
            3 => "artificer-bent playfulness",
            4 => "exuberant gadget-lore mischief",
            _ => string.Empty,
        };
    }

    private static string GetMagitechFantasyTensionPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "low operational strain",
            1 => "warded instability",
            2 => "charged routing tension",
            3 => "breach-and-overload strain",
            4 => "containment-failure crisis tension",
            _ => string.Empty,
        };
    }

    private static string GetSwordAndSorceryFantasyStylizationPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded steel-first treatment",
            1 => "lightly pulp-shaped image styling",
            2 => "hard-edged adventure illustration treatment",
            3 => "strongly ruin-charged rendering",
            4 => "blood-warm pulp visual",
            _ => string.Empty,
        };
    }

    private static string GetSwordAndSorceryFantasyRealismPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "minimal realism emphasis",
            1 => "loosely believable survival-world realism",
            2 => "moderately convincing hard-traveled realism",
            3 => "high bodily believability in a brutal ruin-world",
            4 => "strongly convincing steel-and-survival realism",
            _ => string.Empty,
        };
    }

    private static string GetSwordAndSorceryFantasyFramingPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "intimate blade-near framing",
            1 => "contained chamber-side framing",
            2 => "balanced peril-world framing",
            3 => "expansive ruin-and-arena framing",
            4 => "sweeping throne-and-wasteland framing",
            _ => string.Empty,
        };
    }

    private static string GetSwordAndSorceryFantasyCameraDistancePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "close face-and-weapon view",
            1 => "near chamber-scene view",
            2 => "balanced figure-and-peril distance",
            3 => "wider ruin-revealing distance",
            4 => "far-set wasteland-and-stronghold distance",
            _ => string.Empty,
        };
    }

    private static string GetSwordAndSorceryFantasyAwePairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "grounded scale",
            1 => "slight predatory wonder",
            2 => "atmosphere of ruin-charged legend",
            3 => "strong sense of decadent grandeur",
            4 => "overwhelming idol-and-empire sublimity",
            _ => string.Empty,
        };
    }

    private static string GetSwordAndSorceryFantasyLightingIntensityPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "dim torchlit illumination",
            1 => "soft heat-muted lighting",
            2 => "balanced fire-and-daylight illumination",
            3 => "vivid furnace-and-sun illumination",
            4 => "radiant gold-and-fire illumination",
            _ => string.Empty,
        };
    }

    private static string GetSwordAndSorceryFantasyWhimsyPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "serious danger-charged tone",
            1 => "light rogueish edge",
            2 => "rough adventure play",
            3 => "sly plunder-bent playfulness",
            4 => "bold pulp oddity",
            _ => string.Empty,
        };
    }

    private static string GetSwordAndSorceryFantasyTensionPairPhrase(int value)
    {
        return GetBandIndex(value) switch
        {
            0 => "low immediate peril",
            1 => "watchful ambush unease",
            2 => "blade-and-pursuit danger",
            3 => "cult-and-usurpation strain",
            4 => "throne-and-blood crisis tension",
            _ => string.Empty,
        };
    }
}
