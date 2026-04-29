using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetAnimeSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsAnime(configuration.IntentMode))
        {
            yield break;
        }

        var stylizationRealismPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
        {
            (0, 0) => "grounded cel-graphic simplicity",
            (0, 1) => "grounded cel structure",
            (0, 2) => "grounded cel construction",
            (0, 3) => "grounded cel figure fidelity",
            (0, 4) => "grounded cel production fidelity",

            (1, 0) => "lightly lifted graphic logic",
            (1, 1) => "lightly stylized structure",
            (1, 2) => "lightly stylized construction",
            (1, 3) => "lightly stylized figure fidelity",
            (1, 4) => "lightly stylized production fidelity",

            (2, 0) => "clear stylized graphic form",
            (2, 1) => "clear stylized structure",
            (2, 2) => "clear stylized construction",
            (2, 3) => "clear stylized figure fidelity",
            (2, 4) => "clear stylized production fidelity",

            (3, 0) => "assertive graphic shape logic",
            (3, 1) => "assertive structured stylization",
            (3, 2) => "assertive constructed stylization",
            (3, 3) => "assertive figure fidelity",
            (3, 4) => "assertive production fidelity",

            (4, 0) => "high-authorial graphic abstraction",
            (4, 1) => "high-authorial structured abstraction",
            (4, 2) => "high-authorial constructed abstraction",
            (4, 3) => "high-authorial figure fidelity",
            (4, 4) => "high-authorial production fidelity",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Stylization,
            configuration.Stylization,
            Realism,
            configuration.Realism,
            stylizationRealismPhrase,
            out var stylizationRealismCollapse))
        {
            yield return stylizationRealismCollapse;
        }

        var lightingContrastPhrase = (GetBandIndex(configuration.LightingIntensity), GetBandIndex(configuration.Contrast)) switch
        {
            (0, 0) => "muted diffuse lighting",
            (0, 1) => "muted contour light",
            (0, 2) => "muted clean value read",
            (0, 3) => "muted crisp edge read",
            (0, 4) => "muted hard-shadow cut",

            (1, 0) => "soft blooming light",
            (1, 1) => "soft shaped glow",
            (1, 2) => "soft clear definition",
            (1, 3) => "soft highlight snap",
            (1, 4) => "soft sharp-cut lighting",

            (2, 0) => "even low-contrast light",
            (2, 1) => "even tonal shaping",
            (2, 2) => "even clean light structure",
            (2, 3) => "even crisp separation",
            (2, 4) => "even striking tonal cut",

            (3, 0) => "bright wash-lit scene",
            (3, 1) => "bright contoured light",
            (3, 2) => "bright value clarity",
            (3, 3) => "bright impact snap",
            (3, 4) => "bright hard-strike contrast",

            (4, 0) => "radiant bloom-heavy light",
            (4, 1) => "radiant sculpted glow",
            (4, 2) => "radiant light clarity",
            (4, 3) => "radiant crisp flare",
            (4, 4) => "radiant hard-cut illumination",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            LightingIntensity,
            configuration.LightingIntensity,
            Contrast,
            configuration.Contrast,
            lightingContrastPhrase,
            out var lightingContrastCollapse))
        {
            yield return lightingContrastCollapse;
        }

        foreach (var collapse in BuildAnimeFocusAtmosphereCollapses(configuration))
        {
            yield return collapse;
        }

        foreach (var collapse in BuildAnimeMotionTensionCollapses(configuration))
        {
            yield return collapse;
        }

        foreach (var collapse in BuildAnimeTextureCleanlinessCollapses(configuration))
        {
            yield return collapse;
        }
    }

    private static IEnumerable<PromptSemanticPairCollapse> BuildAnimeFocusAtmosphereCollapses(PromptConfiguration configuration)
    {
        var style = NormalizeAnimeStyleKey(configuration.AnimeStyle);
        if (string.IsNullOrWhiteSpace(style) || string.Equals(style, "general-anime", StringComparison.OrdinalIgnoreCase))
        {
            yield break;
        }

        var fusedPhrase = style switch
        {
            "shonen-action" => (GetBandIndex(configuration.FocusDepth), GetBandIndex(configuration.AtmosphericDepth)) switch
            {
                (0, 0) => "flat clash readability",
                (0, 1) => "broad charge recession",
                (0, 2) => "broad air-cut depth",
                (0, 3) => "broad impact layering",
                (0, 4) => "broad deep-combat read",

                (1, 0) => "flat falloff read",
                (1, 1) => "receding falloff",
                (1, 2) => "air-cut falloff",
                (1, 3) => "layered falloff",
                (1, 4) => "deep-scene falloff",

                (2, 0) => "flat hero emphasis",
                (2, 1) => "recessed hero emphasis",
                (2, 2) => "air-cut hero focus",
                (2, 3) => "layered hero focus",
                (2, 4) => "deep-combat hero pull",

                (3, 0) => "selective clash isolation",
                (3, 1) => "recessed strike isolation",
                (3, 2) => "air-cut strike pull",
                (3, 3) => "layered strike isolation",
                (3, 4) => "deep-combat strike pull",

                (4, 0) => "hard clash isolation",
                (4, 1) => "hard recessed isolation",
                (4, 2) => "hard depth-cut strike isolation",
                (4, 3) => "hard layered strike isolation",
                (4, 4) => "hard deep-combat isolation",
                _ => string.Empty,
            },
            "shojo-romance" => (GetBandIndex(configuration.FocusDepth), GetBandIndex(configuration.AtmosphericDepth)) switch
            {
                (0, 0) => "flat relational clarity",
                (0, 1) => "broad glow recession",
                (0, 2) => "broad airy intimacy",
                (0, 3) => "broad romantic layering",
                (0, 4) => "broad dreamy recession",

                (1, 0) => "soft flat-space focus",
                (1, 1) => "soft glowing recession",
                (1, 2) => "soft airy focus",
                (1, 3) => "soft romantic layering",
                (1, 4) => "soft dreamy pull",

                (2, 0) => "flat face emphasis",
                (2, 1) => "glow-recessed face focus",
                (2, 2) => "airy face focus",
                (2, 3) => "layered romantic focus",
                (2, 4) => "deep dreamy face pull",

                (3, 0) => "selective relational isolation",
                (3, 1) => "selective glow isolation",
                (3, 2) => "selective airy pull",
                (3, 3) => "selective romantic isolation",
                (3, 4) => "selective dreamy pull",

                (4, 0) => "dreamy flat-space intimacy",
                (4, 1) => "dreamy glow-pull recession",
                (4, 2) => "dreamy airy intimacy",
                (4, 3) => "dreamy romantic pull",
                (4, 4) => "deep dreamy intimacy",
                _ => string.Empty,
            },
            "seinen-dark" => (GetBandIndex(configuration.FocusDepth), GetBandIndex(configuration.AtmosphericDepth)) switch
            {
                (0, 0) => "flat oppressive clarity",
                (0, 1) => "broad murk recession",
                (0, 2) => "broad weighted depth",
                (0, 3) => "broad shadow-loaded recession",
                (0, 4) => "broad ominous recession",

                (1, 0) => "shadowed flat-space focus",
                (1, 1) => "shadowed murk recession",
                (1, 2) => "shadowed weighted depth",
                (1, 3) => "shadowed loaded recession",
                (1, 4) => "shadowed ominous pull",

                (2, 0) => "flat pressure focus",
                (2, 1) => "murk-recessed pressure focus",
                (2, 2) => "weighted pressure focus",
                (2, 3) => "shadow-loaded pressure pull",
                (2, 4) => "deep ominous pressure pull",

                (3, 0) => "selective oppressive pull",
                (3, 1) => "selective murk pull",
                (3, 2) => "selective weighted pull",
                (3, 3) => "selective shadow-loaded pull",
                (3, 4) => "selective ominous pull",

                (4, 0) => "hard oppressive isolation",
                (4, 1) => "hard murk isolation",
                (4, 2) => "hard weighted isolation",
                (4, 3) => "hard shadow-loaded isolation",
                (4, 4) => "hard ominous isolation",
                _ => string.Empty,
            },
            "fantasy-anime" => (GetBandIndex(configuration.FocusDepth), GetBandIndex(configuration.AtmosphericDepth)) switch
            {
                (0, 0) => "flat storybook clarity",
                (0, 1) => "broad enchanted recession",
                (0, 2) => "broad wonder depth",
                (0, 3) => "broad mythic layering",
                (0, 4) => "broad legendary depth",

                (1, 0) => "enchanted flat-space focus",
                (1, 1) => "enchanted recessed focus",
                (1, 2) => "enchanted wonder depth",
                (1, 3) => "enchanted mythic layering",
                (1, 4) => "enchanted legendary pull",

                (2, 0) => "flat wonder focus",
                (2, 1) => "recessed wonder focus",
                (2, 2) => "air-filled wonder focus",
                (2, 3) => "layered mythic focus",
                (2, 4) => "deep legendary pull",

                (3, 0) => "selective storybook pull",
                (3, 1) => "selective enchanted pull",
                (3, 2) => "selective wonder pull",
                (3, 3) => "selective mythic pull",
                (3, 4) => "selective legendary pull",

                (4, 0) => "spellbound flat-space isolation",
                (4, 1) => "spellbound recessed isolation",
                (4, 2) => "spellbound wonder isolation",
                (4, 3) => "spellbound mythic isolation",
                (4, 4) => "spellbound legendary isolation",
                _ => string.Empty,
            },
            "mecha-sci-fi-anime" => (GetBandIndex(configuration.FocusDepth), GetBandIndex(configuration.AtmosphericDepth)) switch
            {
                (0, 0) => "flat systems clarity",
                (0, 1) => "broad structural recession",
                (0, 2) => "broad technical depth",
                (0, 3) => "broad structural layering",
                (0, 4) => "broad industrial recession",

                (1, 0) => "clean flat-space focus",
                (1, 1) => "clean structural recession",
                (1, 2) => "clean technical depth",
                (1, 3) => "clean layered recession",
                (1, 4) => "clean industrial pull",

                (2, 0) => "flat machine emphasis",
                (2, 1) => "structurally recessed machine focus",
                (2, 2) => "technical machine focus",
                (2, 3) => "layered machine focus",
                (2, 4) => "deep industrial machine pull",

                (3, 0) => "selective engineered pull",
                (3, 1) => "selective structural pull",
                (3, 2) => "selective technical pull",
                (3, 3) => "selective layered pull",
                (3, 4) => "selective industrial pull",

                (4, 0) => "hard engineered isolation",
                (4, 1) => "hard structural isolation",
                (4, 2) => "hard technical isolation",
                (4, 3) => "hard layered isolation",
                (4, 4) => "hard industrial isolation",
                _ => string.Empty,
            },
            "slice-of-life" => (GetBandIndex(configuration.FocusDepth), GetBandIndex(configuration.AtmosphericDepth)) switch
            {
                (0, 0) => "flat everyday clarity",
                (0, 1) => "broad lived-in recession",
                (0, 2) => "broad everyday depth",
                (0, 3) => "broad ambient layering",
                (0, 4) => "broad deep-lived-in read",

                (1, 0) => "gentle flat-space focus",
                (1, 1) => "gentle lived-in recession",
                (1, 2) => "gentle everyday depth",
                (1, 3) => "gentle ambient layering",
                (1, 4) => "gentle deep-scene pull",

                (2, 0) => "flat situational emphasis",
                (2, 1) => "lived-in situational focus",
                (2, 2) => "breathable situational focus",
                (2, 3) => "layered situational focus",
                (2, 4) => "deep-lived-in situational pull",

                (3, 0) => "selective domestic pull",
                (3, 1) => "selective lived-in pull",
                (3, 2) => "selective everyday pull",
                (3, 3) => "selective ambient pull",
                (3, 4) => "selective deep-lived-in pull",

                (4, 0) => "intimate flat-space isolation",
                (4, 1) => "intimate lived-in isolation",
                (4, 2) => "intimate everyday isolation",
                (4, 3) => "intimate ambient isolation",
                (4, 4) => "intimate deep-lived-in isolation",
                _ => string.Empty,
            },
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            FocusDepth,
            configuration.FocusDepth,
            AtmosphericDepth,
            configuration.AtmosphericDepth,
            fusedPhrase,
            out var collapse))
        {
            yield return collapse;
        }
    }

    private static IEnumerable<PromptSemanticPairCollapse> BuildAnimeMotionTensionCollapses(PromptConfiguration configuration)
    {
        var style = NormalizeAnimeStyleKey(configuration.AnimeStyle);
        if (string.IsNullOrWhiteSpace(style) || string.Equals(style, "general-anime", StringComparison.OrdinalIgnoreCase))
        {
            yield break;
        }

        var fusedPhrase = style switch
        {
            "shonen-action" => (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Tension)) switch
            {
                (0, 0) => "held rivalry posture",
                (0, 1) => "held contest friction",
                (0, 2) => "held battle-ready strain",
                (0, 3) => "held showdown strain",
                (0, 4) => "held life-or-death tension",

                (1, 0) => "rivalry-charged momentum",
                (1, 1) => "contest-charged momentum",
                (1, 2) => "battle-ready momentum",
                (1, 3) => "showdown momentum",
                (1, 4) => "life-or-death momentum",

                (2, 0) => "active rivalry movement",
                (2, 1) => "active contest movement",
                (2, 2) => "active battle pressure",
                (2, 3) => "active showdown force",
                (2, 4) => "active lethal pressure",

                (3, 0) => "explosive rivalry flow",
                (3, 1) => "explosive contest flow",
                (3, 2) => "explosive battle force",
                (3, 3) => "explosive showdown force",
                (3, 4) => "explosive lethal force",

                (4, 0) => "breakneck rivalry surge",
                (4, 1) => "breakneck contest surge",
                (4, 2) => "breakneck battle surge",
                (4, 3) => "breakneck showdown surge",
                (4, 4) => "breakneck lethal surge",
                _ => string.Empty,
            },
            "shojo-romance" => (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Tension)) switch
            {
                (0, 0) => "held romantic pause",
                (0, 1) => "held heart-friction pause",
                (0, 2) => "held emotional pressure",
                (0, 3) => "held relational ache",
                (0, 4) => "held heart-crisis tension",

                (1, 0) => "drifting romantic gesture",
                (1, 1) => "drifting heart-friction",
                (1, 2) => "drifting emotional pressure",
                (1, 3) => "drifting relational ache",
                (1, 4) => "drifting heart-crisis pull",

                (2, 0) => "active romantic movement",
                (2, 1) => "active heart-friction movement",
                (2, 2) => "active emotional pressure",
                (2, 3) => "active relational ache",
                (2, 4) => "active heart-crisis motion",

                (3, 0) => "sweeping romantic flow",
                (3, 1) => "sweeping heart-friction",
                (3, 2) => "sweeping emotional pressure",
                (3, 3) => "sweeping relational ache",
                (3, 4) => "sweeping heart-crisis flow",

                (4, 0) => "emotion-rush romance",
                (4, 1) => "emotion-rush heart-friction",
                (4, 2) => "emotion-rush pressure",
                (4, 3) => "emotion-rush ache",
                (4, 4) => "emotion-rush heart crisis",
                _ => string.Empty,
            },
            "seinen-dark" => (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Tension)) switch
            {
                (0, 0) => "uneasy dread stillness",
                (0, 1) => "uneasy social strain",
                (0, 2) => "uneasy grim pressure",
                (0, 3) => "uneasy moral strain",
                (0, 4) => "uneasy existential tension",

                (1, 0) => "restrained dread leak",
                (1, 1) => "restrained social friction",
                (1, 2) => "restrained grim pressure",
                (1, 3) => "restrained moral strain",
                (1, 4) => "restrained existential pull",

                (2, 0) => "active dread movement",
                (2, 1) => "active social pressure",
                (2, 2) => "active grim pressure",
                (2, 3) => "active moral strain",
                (2, 4) => "active existential pressure",

                (3, 0) => "violent dread flow",
                (3, 1) => "violent social rupture",
                (3, 2) => "violent grim force",
                (3, 3) => "violent moral rupture",
                (3, 4) => "violent existential rupture",

                (4, 0) => "brutal dread rupture",
                (4, 1) => "brutal social rupture",
                (4, 2) => "brutal grim rupture",
                (4, 3) => "brutal moral rupture",
                (4, 4) => "brutal existential rupture",
                _ => string.Empty,
            },
            "fantasy-anime" => (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Tension)) switch
            {
                (0, 0) => "held quest pause",
                (0, 1) => "held peril friction",
                (0, 2) => "held adventure pressure",
                (0, 3) => "held quest peril",
                (0, 4) => "held legendary peril",

                (1, 0) => "spell-stir quest motion",
                (1, 1) => "spell-stir peril motion",
                (1, 2) => "spell-stir adventure pressure",
                (1, 3) => "spell-stir quest peril",
                (1, 4) => "spell-stir legendary peril",

                (2, 0) => "active quest motion",
                (2, 1) => "active peril motion",
                (2, 2) => "active adventure pressure",
                (2, 3) => "active quest peril",
                (2, 4) => "active legendary peril",

                (3, 0) => "sweeping quest flow",
                (3, 1) => "sweeping peril flow",
                (3, 2) => "sweeping adventure force",
                (3, 3) => "sweeping quest peril",
                (3, 4) => "sweeping legendary peril",

                (4, 0) => "surging quest motion",
                (4, 1) => "surging peril motion",
                (4, 2) => "surging adventure force",
                (4, 3) => "surging quest peril",
                (4, 4) => "surging legendary peril",
                _ => string.Empty,
            },
            "mecha-sci-fi-anime" => (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Tension)) switch
            {
                (0, 0) => "locked systems stance",
                (0, 1) => "locked mission friction",
                (0, 2) => "locked operational pressure",
                (0, 3) => "locked combat urgency",
                (0, 4) => "locked catastrophic pressure",

                (1, 0) => "actuator-stir systems motion",
                (1, 1) => "actuator-stir mission friction",
                (1, 2) => "actuator-stir operational pressure",
                (1, 3) => "actuator-stir combat urgency",
                (1, 4) => "actuator-stir catastrophic pressure",

                (2, 0) => "active systems motion",
                (2, 1) => "active mission pressure",
                (2, 2) => "active operational pressure",
                (2, 3) => "active combat urgency",
                (2, 4) => "active catastrophic pressure",

                (3, 0) => "thrust-driven systems flow",
                (3, 1) => "thrust-driven mission force",
                (3, 2) => "thrust-driven operational force",
                (3, 3) => "thrust-driven combat urgency",
                (3, 4) => "thrust-driven catastrophic force",

                (4, 0) => "high-output systems surge",
                (4, 1) => "high-output mission surge",
                (4, 2) => "high-output operational surge",
                (4, 3) => "high-output combat surge",
                (4, 4) => "high-output catastrophic surge",
                _ => string.Empty,
            },
            "slice-of-life" => (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Tension)) switch
            {
                (0, 0) => "held daily-life pose",
                (0, 1) => "held interpersonal friction",
                (0, 2) => "held awkward pressure",
                (0, 3) => "held domestic tension",
                (0, 4) => "held turning-point strain",

                (1, 0) => "casual daily-life motion",
                (1, 1) => "casual interpersonal friction",
                (1, 2) => "casual awkward pressure",
                (1, 3) => "casual domestic tension",
                (1, 4) => "casual turning-point pressure",

                (2, 0) => "active daily-life motion",
                (2, 1) => "active interpersonal pressure",
                (2, 2) => "active awkward pressure",
                (2, 3) => "active domestic tension",
                (2, 4) => "active turning-point pressure",

                (3, 0) => "gentle daily-life flow",
                (3, 1) => "gentle interpersonal friction",
                (3, 2) => "gentle awkward pressure",
                (3, 3) => "gentle domestic tension",
                (3, 4) => "gentle turning-point pressure",

                (4, 0) => "busy daily-life flow",
                (4, 1) => "busy interpersonal strain",
                (4, 2) => "busy awkward pressure",
                (4, 3) => "busy domestic tension",
                (4, 4) => "busy turning-point pressure",
                _ => string.Empty,
            },
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            MotionEnergy,
            configuration.MotionEnergy,
            Tension,
            configuration.Tension,
            fusedPhrase,
            out var collapse))
        {
            yield return collapse;
        }
    }

    private static IEnumerable<PromptSemanticPairCollapse> BuildAnimeTextureCleanlinessCollapses(PromptConfiguration configuration)
    {
        var era = NormalizeAnimeEraKey(configuration.AnimeEra);
        if (string.IsNullOrWhiteSpace(era) || string.Equals(era, "modern-default", StringComparison.OrdinalIgnoreCase))
        {
            yield break;
        }

        var fusedPhrase = era switch
        {
            "classic-anime" => (GetBandIndex(configuration.TextureDepth), GetBandIndex(configuration.ImageCleanliness)) switch
            {
                (0, 0) => "raw painted-cel finish",
                (0, 1) => "grit-touched painted-cel finish",
                (0, 2) => "controlled painted-cel finish",
                (0, 3) => "cleaned painted-cel finish",
                (0, 4) => "polished painted-cel finish",

                (1, 0) => "raw analog surface",
                (1, 1) => "grit-touched analog surface",
                (1, 2) => "controlled analog surface",
                (1, 3) => "cleaned analog surface",
                (1, 4) => "polished analog surface",

                (2, 0) => "raw hand-painted texture",
                (2, 1) => "grit-touched hand-painted texture",
                (2, 2) => "controlled hand-painted texture",
                (2, 3) => "cleaned hand-painted texture",
                (2, 4) => "polished hand-painted texture",

                (3, 0) => "raw cel-background texture",
                (3, 1) => "grit-touched cel-background texture",
                (3, 2) => "controlled cel-background texture",
                (3, 3) => "cleaned cel-background texture",
                (3, 4) => "polished cel-background texture",

                (4, 0) => "raw vintage cel finish",
                (4, 1) => "grit-touched vintage cel finish",
                (4, 2) => "controlled vintage cel finish",
                (4, 3) => "cleaned vintage cel finish",
                (4, 4) => "archival vintage cel finish",
                _ => string.Empty,
            },
            "cel-era" => (GetBandIndex(configuration.TextureDepth), GetBandIndex(configuration.ImageCleanliness)) switch
            {
                (0, 0) => "raw acetate finish",
                (0, 1) => "speck-touched acetate finish",
                (0, 2) => "controlled acetate finish",
                (0, 3) => "cleaned acetate finish",
                (0, 4) => "polished acetate finish",

                (1, 0) => "raw cel paint grain",
                (1, 1) => "speck-touched cel grain",
                (1, 2) => "controlled cel grain",
                (1, 3) => "cleaned cel grain",
                (1, 4) => "polished cel grain",

                (2, 0) => "raw cel material surface",
                (2, 1) => "speck-touched cel material",
                (2, 2) => "controlled cel material",
                (2, 3) => "cleaned cel material",
                (2, 4) => "polished cel material",

                (3, 0) => "raw cel-backdrop texture",
                (3, 1) => "speck-touched cel-backdrop texture",
                (3, 2) => "controlled cel-backdrop texture",
                (3, 3) => "cleaned cel-backdrop texture",
                (3, 4) => "polished cel-backdrop texture",

                (4, 0) => "raw cel-era finish",
                (4, 1) => "speck-touched cel-era finish",
                (4, 2) => "controlled cel-era finish",
                (4, 3) => "cleaned cel-era finish",
                (4, 4) => "premium cel-era finish",
                _ => string.Empty,
            },
            "broadcast-anime" => (GetBandIndex(configuration.TextureDepth), GetBandIndex(configuration.ImageCleanliness)) switch
            {
                (0, 0) => "raw TV-cel finish",
                (0, 1) => "grit-touched TV-cel finish",
                (0, 2) => "controlled TV-cel finish",
                (0, 3) => "cleaned TV-cel finish",
                (0, 4) => "polished TV-cel finish",

                (1, 0) => "raw broadcast-era surface",
                (1, 1) => "grit-touched broadcast surface",
                (1, 2) => "controlled broadcast surface",
                (1, 3) => "cleaned broadcast surface",
                (1, 4) => "polished broadcast surface",

                (2, 0) => "raw cel material definition",
                (2, 1) => "grit-touched cel material",
                (2, 2) => "controlled cel material",
                (2, 3) => "cleaned cel material",
                (2, 4) => "polished cel material",

                (3, 0) => "raw paint-and-ink texture",
                (3, 1) => "grit-touched paint-and-ink texture",
                (3, 2) => "controlled paint-and-ink texture",
                (3, 3) => "cleaned paint-and-ink texture",
                (3, 4) => "polished paint-and-ink texture",

                (4, 0) => "raw broadcast-cel finish",
                (4, 1) => "grit-touched broadcast-cel finish",
                (4, 2) => "controlled broadcast-cel finish",
                (4, 3) => "cleaned broadcast-cel finish",
                (4, 4) => "polished broadcast finish",
                _ => string.Empty,
            },
            "early-digital" => (GetBandIndex(configuration.TextureDepth), GetBandIndex(configuration.ImageCleanliness)) switch
            {
                (0, 0) => "raw hybrid finish",
                (0, 1) => "grit-touched hybrid finish",
                (0, 2) => "controlled hybrid finish",
                (0, 3) => "cleaned hybrid finish",
                (0, 4) => "polished hybrid finish",

                (1, 0) => "raw composited surface",
                (1, 1) => "grit-touched composited surface",
                (1, 2) => "controlled composited surface",
                (1, 3) => "cleaned composited surface",
                (1, 4) => "polished composited surface",

                (2, 0) => "raw digital-cel material",
                (2, 1) => "grit-touched digital-cel material",
                (2, 2) => "controlled digital-cel material",
                (2, 3) => "cleaned digital-cel material",
                (2, 4) => "polished digital-cel material",

                (3, 0) => "raw hybrid texture layering",
                (3, 1) => "grit-touched hybrid texture",
                (3, 2) => "controlled hybrid texture",
                (3, 3) => "cleaned hybrid texture",
                (3, 4) => "polished hybrid texture",

                (4, 0) => "raw early-digital finish",
                (4, 1) => "grit-touched early-digital finish",
                (4, 2) => "controlled early-digital finish",
                (4, 3) => "cleaned early-digital finish",
                (4, 4) => "polished early-digital finish",
                _ => string.Empty,
            },
            "modern-anime" => (GetBandIndex(configuration.TextureDepth), GetBandIndex(configuration.ImageCleanliness)) switch
            {
                (0, 0) => "raw polished finish",
                (0, 1) => "grit-touched polished finish",
                (0, 2) => "controlled polished finish",
                (0, 3) => "cleaned polished finish",
                (0, 4) => "ultra-polished finish",

                (1, 0) => "raw premium surface",
                (1, 1) => "grit-touched premium surface",
                (1, 2) => "controlled premium surface",
                (1, 3) => "cleaned premium surface",
                (1, 4) => "ultra-polished premium surface",

                (2, 0) => "raw digital material",
                (2, 1) => "grit-touched digital material",
                (2, 2) => "controlled digital material",
                (2, 3) => "cleaned digital material",
                (2, 4) => "ultra-polished digital material",

                (3, 0) => "raw seasonal-finish texture",
                (3, 1) => "grit-touched seasonal texture",
                (3, 2) => "controlled seasonal texture",
                (3, 3) => "cleaned seasonal texture",
                (3, 4) => "ultra-polished seasonal texture",

                (4, 0) => "raw modern-production finish",
                (4, 1) => "grit-touched modern-production finish",
                (4, 2) => "controlled modern-production finish",
                (4, 3) => "cleaned modern-production finish",
                (4, 4) => "ultra-polished modern-production finish",
                _ => string.Empty,
            },
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            TextureDepth,
            configuration.TextureDepth,
            ImageCleanliness,
            configuration.ImageCleanliness,
            fusedPhrase,
            out var collapse))
        {
            yield return collapse;
        }
    }
}
