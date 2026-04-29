using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetCinematicSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsCinematic(configuration.IntentMode))
        {
            yield break;
        }

        var fusedPhrase = (GetBandIndex(configuration.NarrativeDensity), GetBandIndex(configuration.BackgroundComplexity)) switch
        {
            (0, 0) => "self-contained screen moment",
            (0, 1) => "single-shot beat with restrained set detail",
            (0, 2) => "single-shot beat anchored in its environment",
            (0, 3) => "single-shot beat in richly staged surroundings",
            (0, 4) => "single-shot beat within a dense cinematic world",

            (1, 0) => "lightly implied scene beat",
            (1, 1) => "lightly implied beat with restrained staging",
            (1, 2) => "lightly implied scene with supporting environment",
            (1, 3) => "lightly implied scene in rich staging",
            (1, 4) => "lightly implied scene within a layered world",

            (2, 0) => "scene-driven beat on a spare set",
            (2, 1) => "scene-driven beat with restrained set support",
            (2, 2) => "scene-driven environment cues",
            (2, 3) => "scene-driven staging with rich environmental support",
            (2, 4) => "scene-driven staging across a layered cinematic world",

            (3, 0) => "layered narrative suggestion in a stripped scene",
            (3, 1) => "layered narrative suggestion with restrained staging",
            (3, 2) => "layered narrative suggestion with supporting environment",
            (3, 3) => "layered narrative suggestion through rich staging",
            (3, 4) => "layered narrative suggestion through a dense cinematic world",

            (4, 0) => "world-charged narrative on a spare stage",
            (4, 1) => "world-charged narrative with restrained staging",
            (4, 2) => "world-rich narrative with supporting environment",
            (4, 3) => "world-rich narrative through expansive staging",
            (4, 4) => "world-rich narrative charge through a densely layered cinematic world",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            NarrativeDensity,
            configuration.NarrativeDensity,
            BackgroundComplexity,
            configuration.BackgroundComplexity,
            fusedPhrase,
            out var collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Chaos)) switch
        {
            (0, 0) => "held frame with controlled composition",
            (0, 1) => "held frame under restless tension",
            (0, 2) => "held frame with active instability",
            (0, 3) => "held frame inside dynamic turbulence",
            (0, 4) => "held frame against orchestrated chaos",

            (1, 0) => "gentle motion within controlled composition",
            (1, 1) => "gentle motion with restless tension",
            (1, 2) => "gentle motion through active instability",
            (1, 3) => "gentle motion through dynamic turbulence",
            (1, 4) => "gentle motion inside orchestrated chaos",

            (2, 0) => "active cinematic energy with controlled composition",
            (2, 1) => "active cinematic energy under restless tension",
            (2, 2) => "active cinematic energy through instability",
            (2, 3) => "active cinematic energy through dynamic turbulence",
            (2, 4) => "active cinematic energy against orchestrated chaos",

            (3, 0) => "dynamic motion with compositional control",
            (3, 1) => "dynamic motion with restless tension",
            (3, 2) => "dynamic motion through active instability",
            (3, 3) => "dynamic motion inside turbulence",
            (3, 4) => "dynamic motion across orchestrated chaos",

            (4, 0) => "high kinetic momentum with disciplined control",
            (4, 1) => "high kinetic momentum under restless pressure",
            (4, 2) => "high kinetic momentum through instability",
            (4, 3) => "high kinetic momentum through turbulence",
            (4, 4) => "high kinetic momentum across orchestrated chaos",
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

        fusedPhrase = (GetBandIndex(configuration.TextureDepth), GetBandIndex(configuration.ImageCleanliness)) switch
        {
            (0, 0) => "bare surfaces in a raw analog finish",
            (0, 1) => "bare surfaces with light refinement",
            (0, 2) => "restrained surfaces with clean production finish",
            (0, 3) => "restrained surfaces in polished theatrical finish",
            (0, 4) => "restrained surfaces in ultra-clean presentation",

            (1, 0) => "light material indication in raw analog finish",
            (1, 1) => "light material indication with gentle refinement",
            (1, 2) => "light material indication with clean production finish",
            (1, 3) => "light material indication in polished theatrical finish",
            (1, 4) => "light material indication in ultra-clean presentation",

            (2, 0) => "clear surface tactility in raw analog finish",
            (2, 1) => "clear surface tactility with light refinement",
            (2, 2) => "clear surface tactility with clean production finish",
            (2, 3) => "clear surface tactility in polished theatrical finish",
            (2, 4) => "clear surface tactility in ultra-clean presentation",

            (3, 0) => "rich material presence in raw analog finish",
            (3, 1) => "rich material presence with light refinement",
            (3, 2) => "rich material presence with clean production finish",
            (3, 3) => "rich material presence in polished theatrical finish",
            (3, 4) => "rich material presence in ultra-clean presentation",

            (4, 0) => "deep tactile detail in raw analog finish",
            (4, 1) => "deep tactile detail with light refinement",
            (4, 2) => "deep tactile detail with clean production finish",
            (4, 3) => "deep tactile detail in polished theatrical finish",
            (4, 4) => "deep tactile detail in ultra-clean presentation",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            TextureDepth,
            configuration.TextureDepth,
            ImageCleanliness,
            configuration.ImageCleanliness,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.Awe), GetBandIndex(configuration.AtmosphericDepth)) switch
        {
            (0, 0) => "grounded scale with slight depth falloff",
            (0, 1) => "grounded scale with light spatial recession",
            (0, 2) => "grounded scale in air-filled depth",
            (0, 3) => "grounded scale within luminous atmosphere",
            (0, 4) => "grounded scale inside deep layered atmosphere",

            (1, 0) => "slight wonder at grounded scale",
            (1, 1) => "slight wonder with light spatial recession",
            (1, 2) => "slight wonder in air-filled depth",
            (1, 3) => "slight wonder within luminous atmosphere",
            (1, 4) => "slight wonder inside deep layered atmosphere",

            (2, 0) => "sense of spectacle at grounded depth",
            (2, 1) => "sense of spectacle with opening recession",
            (2, 2) => "sense of spectacle in air-filled depth",
            (2, 3) => "sense of spectacle through luminous atmosphere",
            (2, 4) => "sense of spectacle inside deep layered atmosphere",

            (3, 0) => "strong scale presence in near space",
            (3, 1) => "strong scale presence with spatial opening",
            (3, 2) => "strong scale presence through air-filled depth",
            (3, 3) => "strong scale presence within luminous atmosphere",
            (3, 4) => "strong scale presence inside deep layered atmosphere",

            (4, 0) => "overwhelming grandeur at close depth",
            (4, 1) => "overwhelming grandeur with widening recession",
            (4, 2) => "overwhelming grandeur through air-filled depth",
            (4, 3) => "overwhelming grandeur within luminous atmosphere",
            (4, 4) => "overwhelming grandeur inside deep layered atmosphere",
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
    }
}
