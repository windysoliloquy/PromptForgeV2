using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetInfographicDataVisualizationSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsInfographicDataVisualization(configuration.IntentMode))
        {
            yield break;
        }

        var fusedPhrase = (GetBandIndex(configuration.Stylization), GetBandIndex(configuration.Realism)) switch
        {
            (0, 0) => "utilitarian abstract information display",
            (0, 1) => "utilitarian explanatory display with light referential grounding",
            (0, 2) => "utilitarian explanatory display with clear referential grounding",
            (0, 3) => "utilitarian explanatory display with highly convincing referential grounding",
            (0, 4) => "utilitarian explanatory display with strongly observed referential grounding",

            (1, 0) => "lightly designed abstract explainer layout",
            (1, 1) => "lightly designed explainer layout with light referential grounding",
            (1, 2) => "lightly designed explainer layout with clear referential grounding",
            (1, 3) => "lightly designed explainer layout with highly convincing referential grounding",
            (1, 4) => "lightly designed explainer layout with strongly observed referential grounding",

            (2, 0) => "polished abstract information-design treatment",
            (2, 1) => "polished explanatory design with light real-world grounding",
            (2, 2) => "polished explanatory design with clear real-world grounding",
            (2, 3) => "polished explanatory design with highly convincing real-world grounding",
            (2, 4) => "polished explanatory design with strongly observed real-world grounding",

            (3, 0) => "strongly art-directed abstract explanatory display",
            (3, 1) => "strongly art-directed explanatory display with light real-world grounding",
            (3, 2) => "strongly art-directed explanatory display with clear real-world grounding",
            (3, 3) => "strongly art-directed explanatory display with highly convincing real-world grounding",
            (3, 4) => "strongly art-directed explanatory display with strongly observed real-world grounding",

            (4, 0) => "highly stylized abstract information graphic",
            (4, 1) => "highly stylized explanatory graphic with light real-world grounding",
            (4, 2) => "highly stylized explanatory graphic with clear real-world grounding",
            (4, 3) => "highly stylized explanatory graphic with highly convincing real-world grounding",
            (4, 4) => "highly stylized explanatory graphic with strongly observed real-world grounding",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Stylization,
            configuration.Stylization,
            Realism,
            configuration.Realism,
            fusedPhrase,
            out var collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.Framing), GetBandIndex(configuration.CameraDistance)) switch
        {
            (0, 0) => "isolated close-read panel",
            (0, 1) => "isolated near-read panel",
            (0, 2) => "isolated page-read panel",
            (0, 3) => "isolated board-read panel",
            (0, 4) => "isolated distant-read panel",

            (1, 0) => "focused close-read layout",
            (1, 1) => "focused near-read layout",
            (1, 2) => "focused page-read layout",
            (1, 3) => "focused board-read layout",
            (1, 4) => "focused distant-read layout",

            (2, 0) => "balanced close-read page",
            (2, 1) => "balanced near-read page",
            (2, 2) => "balanced page-scale framing",
            (2, 3) => "balanced board-read framing",
            (2, 4) => "balanced distant-read framing",

            (3, 0) => "broad close-read spread",
            (3, 1) => "broad near-read spread",
            (3, 2) => "broad page-read spread",
            (3, 3) => "broad board-read spread",
            (3, 4) => "broad distant-read spread",

            (4, 0) => "expansive close-read display",
            (4, 1) => "expansive near-read display",
            (4, 2) => "expansive page-read display",
            (4, 3) => "expansive board-read display",
            (4, 4) => "expansive wall-scale display",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Framing,
            configuration.Framing,
            CameraDistance,
            configuration.CameraDistance,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.FocusDepth), GetBandIndex(configuration.DetailDensity)) switch
        {
            (0, 0) => "broad sparse-read clarity",
            (0, 1) => "broad selective-detail clarity",
            (0, 2) => "broad rich-detail clarity",
            (0, 3) => "broad dense-detail clarity",
            (0, 4) => "broad exhaustive-detail clarity",

            (1, 0) => "lightly guided sparse-read layout",
            (1, 1) => "lightly guided selective-detail layout",
            (1, 2) => "lightly guided rich-detail layout",
            (1, 3) => "lightly guided dense-detail layout",
            (1, 4) => "lightly guided exhaustive-detail layout",

            (2, 0) => "balanced sparse-read hierarchy",
            (2, 1) => "balanced selective-detail hierarchy",
            (2, 2) => "balanced rich-detail hierarchy",
            (2, 3) => "balanced dense-detail hierarchy",
            (2, 4) => "balanced exhaustive-detail hierarchy",

            (3, 0) => "strong callout sparse-read structure",
            (3, 1) => "strong callout selective-detail structure",
            (3, 2) => "strong callout rich-detail structure",
            (3, 3) => "strong callout dense-detail structure",
            (3, 4) => "strong callout exhaustive-detail structure",

            (4, 0) => "decisive sparse-read hierarchy",
            (4, 1) => "decisive selective-detail hierarchy",
            (4, 2) => "decisive rich-detail hierarchy",
            (4, 3) => "decisive dense-detail hierarchy",
            (4, 4) => "decisive exhaustive-detail hierarchy",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            FocusDepth,
            configuration.FocusDepth,
            DetailDensity,
            configuration.DetailDensity,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        if (!IsDataVizSubdomain(configuration))
        {
            yield break;
        }

        fusedPhrase = (GetBandIndex(configuration.Tension), GetBandIndex(configuration.Contrast)) switch
        {
            (0, 0) => "calm soft-grid reporting",
            (0, 1) => "calm low-contrast reporting",
            (0, 2) => "calm balanced-contrast reporting",
            (0, 3) => "calm crisp-callout reporting",
            (0, 4) => "calm high-visibility reporting",

            (1, 0) => "watchful soft-grid dashboard",
            (1, 1) => "watchful low-contrast dashboard",
            (1, 2) => "watchful balanced-contrast dashboard",
            (1, 3) => "watchful crisp-callout dashboard",
            (1, 4) => "watchful high-visibility dashboard",

            (2, 0) => "risk-weighted soft-grid dashboard",
            (2, 1) => "risk-weighted low-contrast dashboard",
            (2, 2) => "risk-weighted balanced-contrast dashboard",
            (2, 3) => "risk-weighted crisp-callout dashboard",
            (2, 4) => "risk-weighted alert-grade dashboard",

            (3, 0) => "alert-state soft-grid dashboard",
            (3, 1) => "alert-state low-contrast dashboard",
            (3, 2) => "alert-state balanced-contrast dashboard",
            (3, 3) => "alert-state crisp-callout dashboard",
            (3, 4) => "alert-state alert-grade dashboard",

            (4, 0) => "crisis soft-grid dashboard",
            (4, 1) => "crisis low-contrast dashboard",
            (4, 2) => "crisis balanced-contrast dashboard",
            (4, 3) => "crisis crisp-callout dashboard",
            (4, 4) => "crisis alert-grade dashboard",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Tension,
            configuration.Tension,
            Contrast,
            configuration.Contrast,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Tension)) switch
        {
            (0, 0) => "calm static-read dashboard",
            (0, 1) => "watchful static-read dashboard",
            (0, 2) => "risk-weighted static-read dashboard",
            (0, 3) => "alert-state static-read dashboard",
            (0, 4) => "crisis static-read dashboard",

            (1, 0) => "calm trend-flow dashboard",
            (1, 1) => "watchful trend-flow dashboard",
            (1, 2) => "risk-weighted trend-flow dashboard",
            (1, 3) => "alert-state trend-flow dashboard",
            (1, 4) => "crisis trend-flow dashboard",

            (2, 0) => "calm active-comparison dashboard",
            (2, 1) => "watchful active-comparison dashboard",
            (2, 2) => "risk-weighted active-comparison dashboard",
            (2, 3) => "alert-state active-comparison dashboard",
            (2, 4) => "crisis active-comparison dashboard",

            (3, 0) => "calm change-over-time dashboard",
            (3, 1) => "watchful change-over-time dashboard",
            (3, 2) => "risk-weighted change-over-time dashboard",
            (3, 3) => "alert-state change-over-time dashboard",
            (3, 4) => "crisis change-over-time dashboard",

            (4, 0) => "calm high-velocity signal dashboard",
            (4, 1) => "watchful high-velocity signal dashboard",
            (4, 2) => "risk-weighted high-velocity signal dashboard",
            (4, 3) => "alert-state high-velocity signal dashboard",
            (4, 4) => "crisis high-velocity signal dashboard",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            MotionEnergy,
            configuration.MotionEnergy,
            Tension,
            configuration.Tension,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.MotionEnergy), GetBandIndex(configuration.Awe)) switch
        {
            (0, 0) => "grounded static-read dashboard",
            (0, 1) => "scoped static-read dashboard",
            (0, 2) => "large-pattern static-read dashboard",
            (0, 3) => "civilization-scale static-read dashboard",
            (0, 4) => "overwhelming systems dashboard",

            (1, 0) => "grounded trend-flow dashboard",
            (1, 1) => "scoped trend-flow dashboard",
            (1, 2) => "large-pattern trend-flow dashboard",
            (1, 3) => "civilization-scale trend-flow dashboard",
            (1, 4) => "overwhelming systems-flow dashboard",

            (2, 0) => "grounded active-comparison field",
            (2, 1) => "scoped active-comparison field",
            (2, 2) => "large-pattern active-comparison field",
            (2, 3) => "civilization-scale active-comparison field",
            (2, 4) => "overwhelming systems-comparison field",

            (3, 0) => "grounded change-over-time field",
            (3, 1) => "scoped change-over-time field",
            (3, 2) => "large-pattern change-over-time field",
            (3, 3) => "civilization-scale change-over-time field",
            (3, 4) => "overwhelming systems-timeflow field",

            (4, 0) => "grounded high-velocity signal field",
            (4, 1) => "scoped high-velocity signal field",
            (4, 2) => "large-pattern high-velocity signal field",
            (4, 3) => "civilization-scale high-velocity signal field",
            (4, 4) => "overwhelming systems-signal field",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            MotionEnergy,
            configuration.MotionEnergy,
            Awe,
            configuration.Awe,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }
    }
}
