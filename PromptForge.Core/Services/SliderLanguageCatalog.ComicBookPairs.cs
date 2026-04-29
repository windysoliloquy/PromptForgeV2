using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetComicBookSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsComicBook(configuration.IntentMode))
        {
            yield break;
        }

        var fusedPhrase = (GetBandIndex(configuration.TextureDepth), GetBandIndex(configuration.ImageCleanliness)) switch
        {
            (0, 0) => "raw flat finish",
            (0, 1) => "loose flat surface",
            (0, 2) => "clean flat finish",
            (0, 3) => "smooth polished surface",
            (0, 4) => "immaculate smooth finish",

            (1, 0) => "dry grain and grit",
            (1, 1) => "light grain breakup",
            (1, 2) => "fine-grain finish",
            (1, 3) => "refined grain control",
            (1, 4) => "immaculate grain finish",

            (2, 0) => "roughened line texture",
            (2, 1) => "lived-in surface texture",
            (2, 2) => "clear tactile definition",
            (2, 3) => "polished tactile finish",
            (2, 4) => "pristine tactile definition",

            (3, 0) => "dense worked texture",
            (3, 1) => "rugged tactile handling",
            (3, 2) => "controlled tactile richness",
            (3, 3) => "richly resolved surface",
            (3, 4) => "immaculate tactile richness",

            (4, 0) => "heavily worked grain",
            (4, 1) => "forceful worked surface",
            (4, 2) => "resolved heavy texture",
            (4, 3) => "polished heavy texture",
            (4, 4) => "immaculate high-relief surface",
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

        fusedPhrase = (GetBandIndex(configuration.Awe), GetBandIndex(configuration.AtmosphericDepth)) switch
        {
            (0, 0) => "grounded frontal scale",
            (0, 1) => "grounded depth hint",
            (0, 2) => "grounded layered space",
            (0, 3) => "grounded luminous recession",
            (0, 4) => "grounded deep-distance reach",

            (1, 0) => "lifted frontal presence",
            (1, 1) => "lifted depth cue",
            (1, 2) => "lifted spatial layering",
            (1, 3) => "lifted luminous recession",
            (1, 4) => "lifted distant reach",

            (2, 0) => "broadened visual scope",
            (2, 1) => "broadened spatial pull",
            (2, 2) => "clear layered breadth",
            (2, 3) => "luminous sense of scale",
            (2, 4) => "far-reaching visual breadth",

            (3, 0) => "forceful scale impact",
            (3, 1) => "forceful depth pull",
            (3, 2) => "strong layered spectacle",
            (3, 3) => "luminous impact build",
            (3, 4) => "sweeping monumental reach",

            (4, 0) => "overwhelming frontal mass",
            (4, 1) => "overwhelming depth pressure",
            (4, 2) => "colossal layered scale",
            (4, 3) => "towering luminous breadth",
            (4, 4) => "immense horizon-spanning depth",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Awe,
            configuration.Awe,
            AtmosphericDepth,
            configuration.AtmosphericDepth,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Chaos)) switch
        {
            (0, 0) => "held stillness",
            (0, 1) => "braced stillness",
            (0, 2) => "poised disruption",
            (0, 3) => "stillness under churn",
            (0, 4) => "locked under overload",

            (1, 0) => "slight directional drift",
            (1, 1) => "restless drift",
            (1, 2) => "uneven movement spread",
            (1, 3) => "pressured movement sweep",
            (1, 4) => "unstable surge",

            (2, 0) => "active directional flow",
            (2, 1) => "driven directional flow",
            (2, 2) => "scattered movement burst",
            (2, 3) => "driving turbulent motion",
            (2, 4) => "forceful visual surge",

            (3, 0) => "explosive directional burst",
            (3, 1) => "explosive pressure burst",
            (3, 2) => "explosive motion spread",
            (3, 3) => "violent turbulent surge",
            (3, 4) => "explosive overload",

            (4, 0) => "relentless kinetic drive",
            (4, 1) => "relentless pressure drive",
            (4, 2) => "relentless movement spread",
            (4, 3) => "relentless turbulence",
            (4, 4) => "total kinetic overload",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            MotionEnergy,
            configuration.MotionEnergy,
            Chaos,
            configuration.Chaos,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.NarrativeDensity), GetBandIndex(configuration.BackgroundComplexity)) switch
        {
            (0, 0) => "isolated focal beat",
            (0, 1) => "spare contextual beat",
            (0, 2) => "clear situated beat",
            (0, 3) => "anchored scene beat",
            (0, 4) => "fully situated beat",

            (1, 0) => "hinted situational cue",
            (1, 1) => "light situational cue",
            (1, 2) => "readable situational cue",
            (1, 3) => "developed situational cue",
            (1, 4) => "dense situational surround",

            (2, 0) => "focused scene turn",
            (2, 1) => "clear scene turn",
            (2, 2) => "readable scene event",
            (2, 3) => "developed scene event",
            (2, 4) => "dense scene event",

            (3, 0) => "layered situational turn",
            (3, 1) => "layered contextual turn",
            (3, 2) => "layered scene build",
            (3, 3) => "developed layered scene",
            (3, 4) => "dense layered setting",

            (4, 0) => "compressed event pressure",
            (4, 1) => "compressed cue pressure",
            (4, 2) => "dense scene pressure",
            (4, 3) => "developed event build",
            (4, 4) => "fully loaded scene pressure",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            NarrativeDensity,
            configuration.NarrativeDensity,
            BackgroundComplexity,
            configuration.BackgroundComplexity,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        if (string.Equals(configuration.ComicBookStyle, "Superhero Comic", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
            {
                (0, 0) => "grounded heroic figurework",
                (0, 1) => "grounded heroic anatomy",
                (0, 2) => "grounded superhero clarity",
                (0, 3) => "grounded idealized action",
                (0, 4) => "grounded controlled hero anatomy",

                (1, 0) => "shaped heroic forms",
                (1, 1) => "shaped heroic anatomy",
                (1, 2) => "shaped superhero figure clarity",
                (1, 3) => "shaped action realism",
                (1, 4) => "shaped idealized hero anatomy",

                (2, 0) => "bold superhero forms",
                (2, 1) => "bold heroic anatomy",
                (2, 2) => "bold superhero clarity",
                (2, 3) => "bold idealized action realism",
                (2, 4) => "bold controlled hero anatomy",

                (3, 0) => "explosive heroic forms",
                (3, 1) => "explosive heroic anatomy",
                (3, 2) => "explosive superhero figure clarity",
                (3, 3) => "explosive idealized action realism",
                (3, 4) => "explosive controlled hero anatomy",

                (4, 0) => "larger-than-life heroic forms",
                (4, 1) => "larger-than-life heroic anatomy",
                (4, 2) => "iconic superhero figure clarity",
                (4, 3) => "larger-than-life action realism",
                (4, 4) => "larger-than-life idealized hero anatomy",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                Stylization,
                configuration.Stylization,
                Realism,
                configuration.Realism,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }

        if (string.Equals(configuration.ComicBookStyle, "Noir Comic", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
            {
                (0, 0) => "grounded noir figurework",
                (0, 1) => "grounded observed anatomy",
                (0, 2) => "grounded dramatic figuration",
                (0, 3) => "grounded observed realism",
                (0, 4) => "grounded noir realism",

                (1, 0) => "shadow-led noir figuration",
                (1, 1) => "shadow-led observed anatomy",
                (1, 2) => "shadow-led dramatic figuration",
                (1, 3) => "shadow-led restrained realism",
                (1, 4) => "shadow-led noir realism",

                (2, 0) => "hard-boiled noir forms",
                (2, 1) => "hard-boiled anatomy",
                (2, 2) => "hard-boiled dramatic figuration",
                (2, 3) => "hard-boiled restrained realism",
                (2, 4) => "hard-boiled noir realism",

                (3, 0) => "shadow-heavy noir forms",
                (3, 1) => "shadow-heavy anatomy",
                (3, 2) => "shadow-heavy dramatic figuration",
                (3, 3) => "shadow-heavy restrained realism",
                (3, 4) => "shadow-heavy noir realism",

                (4, 0) => "severe noir forms",
                (4, 1) => "severe anatomy",
                (4, 2) => "severe dramatic figuration",
                (4, 3) => "severe restrained realism",
                (4, 4) => "severe noir realism",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                Stylization,
                configuration.Stylization,
                Realism,
                configuration.Realism,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }

        if (string.Equals(configuration.ComicBookStyle, "Graphic Novel", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
            {
                (0, 0) => "grounded observed figurework",
                (0, 1) => "grounded observed anatomy",
                (0, 2) => "grounded human figuration",
                (0, 3) => "grounded representational realism",
                (0, 4) => "grounded narrative realism",

                (1, 0) => "restrained graphic forms",
                (1, 1) => "restrained grounded anatomy",
                (1, 2) => "restrained human figuration",
                (1, 3) => "restrained representational realism",
                (1, 4) => "restrained narrative realism",

                (2, 0) => "mature narrative forms",
                (2, 1) => "mature grounded anatomy",
                (2, 2) => "mature human figuration",
                (2, 3) => "mature representational realism",
                (2, 4) => "mature controlled realism",

                (3, 0) => "controlled narrative forms",
                (3, 1) => "controlled narrative anatomy",
                (3, 2) => "controlled human figuration",
                (3, 3) => "controlled representational realism",
                (3, 4) => "controlled narrative realism",

                (4, 0) => "forceful graphic-novel forms",
                (4, 1) => "forceful grounded anatomy",
                (4, 2) => "forceful human figuration",
                (4, 3) => "forceful representational realism",
                (4, 4) => "forceful graphic-novel realism",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                Stylization,
                configuration.Stylization,
                Realism,
                configuration.Realism,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }

            fusedPhrase = (GetBandIndex(configuration.Whimsy), GetBandIndex(configuration.Tension)) switch
            {
                (0, 0) => "sober calm",
                (0, 1) => "quiet dramatic weight",
                (0, 2) => "active dramatic strain",
                (0, 3) => "conflict-laden gravity",
                (0, 4) => "severe emotional weight",

                (1, 0) => "restrained warmth",
                (1, 1) => "light human tension",
                (1, 2) => "gentle dramatic friction",
                (1, 3) => "rising emotional strain",
                (1, 4) => "brittle emotional strain",

                (2, 0) => "lively dramatic lift",
                (2, 1) => "playful dramatic friction",
                (2, 2) => "active human tension",
                (2, 3) => "conflicted dramatic energy",
                (2, 4) => "pressured emotional clash",

                (3, 0) => "expressive human warmth",
                (3, 1) => "animated emotional tension",
                (3, 2) => "charged dramatic friction",
                (3, 3) => "unstable emotional intensity",
                (3, 4) => "strained emotional volatility",

                (4, 0) => "heightened human warmth",
                (4, 1) => "heightened emotional tension",
                (4, 2) => "forceful dramatic friction",
                (4, 3) => "severe emotional instability",
                (4, 4) => "overwhelming emotional conflict",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                Whimsy,
                configuration.Whimsy,
                Tension,
                configuration.Tension,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }

        if (string.Equals(configuration.ComicBookStyle, "Vintage Comic", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
            {
                (0, 0) => "grounded retro figurework",
                (0, 1) => "grounded vintage anatomy",
                (0, 2) => "grounded classic figure clarity",
                (0, 3) => "grounded vintage figuration",
                (0, 4) => "grounded old-school anatomy",

                (1, 0) => "retro graphic forms",
                (1, 1) => "retro grounded anatomy",
                (1, 2) => "retro figure clarity",
                (1, 3) => "retro vintage figuration",
                (1, 4) => "retro old-school anatomy",

                (2, 0) => "classic comic forms",
                (2, 1) => "classic grounded anatomy",
                (2, 2) => "classic figure clarity",
                (2, 3) => "classic vintage figuration",
                (2, 4) => "classic old-school anatomy",

                (3, 0) => "halftone-era forms",
                (3, 1) => "halftone-era grounded anatomy",
                (3, 2) => "halftone-era figure clarity",
                (3, 3) => "halftone-era vintage figuration",
                (3, 4) => "halftone-era old-school anatomy",

                (4, 0) => "old-print forms",
                (4, 1) => "old-print anatomy",
                (4, 2) => "old-print figure clarity",
                (4, 3) => "old-print vintage figuration",
                (4, 4) => "old-school print anatomy",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                Stylization,
                configuration.Stylization,
                Realism,
                configuration.Realism,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }

        if (string.Equals(configuration.ComicBookStyle, "Modern Comic", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
            {
                (0, 0) => "grounded clean figurework",
                (0, 1) => "grounded contemporary anatomy",
                (0, 2) => "grounded precise figure clarity",
                (0, 3) => "grounded modern realism",
                (0, 4) => "grounded controlled contemporary anatomy",

                (1, 0) => "sleek clean forms",
                (1, 1) => "sleek grounded anatomy",
                (1, 2) => "sleek precise figure clarity",
                (1, 3) => "sleek refined modern realism",
                (1, 4) => "sleek controlled contemporary anatomy",

                (2, 0) => "clean modern forms",
                (2, 1) => "clean contemporary anatomy",
                (2, 2) => "clean precise figure clarity",
                (2, 3) => "clean refined modern realism",
                (2, 4) => "clean controlled contemporary anatomy",

                (3, 0) => "sharp modern forms",
                (3, 1) => "sharp contemporary anatomy",
                (3, 2) => "sharp precise figure clarity",
                (3, 3) => "sharp modern realism",
                (3, 4) => "sharp controlled contemporary anatomy",

                (4, 0) => "high-clarity graphic forms",
                (4, 1) => "high-clarity anatomy",
                (4, 2) => "high-clarity figure clarity",
                (4, 3) => "high-clarity modern realism",
                (4, 4) => "high-clarity controlled anatomy",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                Stylization,
                configuration.Stylization,
                Realism,
                configuration.Realism,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }

        if (!string.Equals(configuration.ComicBookStyle, "General Comic", StringComparison.Ordinal))
        {
            yield break;
        }

        fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
        {
            (0, 0) => "grounded drawn figurework",
            (0, 1) => "grounded observed anatomy",
            (0, 2) => "grounded figure clarity",
            (0, 3) => "grounded illustrative realism",
            (0, 4) => "grounded controlled anatomy",

            (1, 0) => "light graphic forms",
            (1, 1) => "light grounded anatomy",
            (1, 2) => "light graphic figure clarity",
            (1, 3) => "light graphic realism",
            (1, 4) => "light controlled anatomy",

            (2, 0) => "clear inked forms",
            (2, 1) => "clear grounded anatomy",
            (2, 2) => "clear inked figure clarity",
            (2, 3) => "clear illustrative realism",
            (2, 4) => "clear controlled anatomy",

            (3, 0) => "expressive panel forms",
            (3, 1) => "expressive grounded anatomy",
            (3, 2) => "expressive figure clarity",
            (3, 3) => "expressive illustrative realism",
            (3, 4) => "expressive controlled anatomy",

            (4, 0) => "bold graphic forms",
            (4, 1) => "bold graphic anatomy",
            (4, 2) => "bold figure clarity",
            (4, 3) => "bold illustrative realism",
            (4, 4) => "bold anatomical rendering",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Stylization,
            configuration.Stylization,
            Realism,
            configuration.Realism,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }
    }
}
