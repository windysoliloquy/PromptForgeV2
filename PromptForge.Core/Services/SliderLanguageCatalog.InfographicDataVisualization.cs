using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    private const string InfographicSubdomainDefault = "infographic";
    private const string InfographicSubdomainDataViz = "data-viz";

    public static string ResolveInfographicDataVisualizationPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetInfographicDataVisualizationBandLabels(sliderKey, configuration);
        return labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);
    }

    public static string ResolveInfographicDataVisualizationGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetInfographicDataVisualizationBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveInfographicDataVisualizationDescriptors(PromptConfiguration configuration)
    {
        if (IsDataVizSubdomain(configuration))
        {
            yield return "data visualization";
            yield return "structured analytical layout";
            yield break;
        }

        yield return "information graphic";
        yield return "structured explanatory layout";
    }

    private static string[] GetInfographicDataVisualizationBandLabels(string sliderKey, PromptConfiguration? configuration = null)
    {
        if (configuration is not null && IsDataVizSubdomain(configuration))
        {
            return sliderKey switch
            {
                Stylization => ["utilitarian chart grammar", "lightly designed chart presentation", "polished analytical design treatment", "strongly art-directed data display", "highly stylized quantitative display"],
                Realism => ["bare quantitative marks", "light reference context", "clear chart-linked context", "highly convincing real-world context", "strongly observed data-in-context rendering"],
                TextureDepth => ["flat plotting surfaces", "light display-surface character", "clear grid-and-mark definition", "rich screen-and-print detail", "deeply worked analytic surface structure"],
                NarrativeDensity => ["single-metric read", "light comparative read", "layered multi-series comparison", "dense analytical story structure", "system-scale explanatory synthesis"],
                Symbolism => ["literal quantitative cues", "subtle interpretive cues", "suggestive analytic metaphors", "pronounced conceptual framing", "mythic systems allegory"],
                SurfaceAge => ["fresh analytical finish", "light handling wear", "settled publication wear", "legacy monitoring wear", "archival systems-room patina"],
                Framing => ["isolated chart frame", "focused figure-and-legend framing", "balanced report-page framing", "multi-panel dashboard framing", "wall-scale analytic layout"],
                CameraDistance => ["single-chart close read", "near axis-and-legend read", "comfortable report-page read", "dashboard-overview read", "room-scale operations-wall read"],
                CameraAngle => ["orthographic flat-on read", "slight display tilt", "gentle report-board angle", "clear oblique dashboard view", "steep operations-board vantage"],
                BackgroundComplexity => ["isolated plotting field", "restrained axes-and-grid support", "supporting legends and reference bands", "rich comparative panels and annotations", "densely layered analytic backdrop"],
                MotionEnergy => ["static quantitative read", "gentle trend flow", "active comparison flow", "dynamic change-over-time flow", "high-velocity signal movement"],
                AtmosphericDepth => ["flat analytic plane", "slight layer separation", "clear dashboard-layer separation", "strong stacked-panel depth", "deep sectional analytic depth"],
                Chaos => ["strictly ordered analysis", "mild signal tension", "lively multi-series density", "controlled analytic clutter", "high information overload"],
                Whimsy => ["sober analytic tone", "lightly approachable tone", "accessible public-facing clarity", "lively explainer friendliness", "bold playful data storytelling"],
                Tension => ["calm reporting tone", "mild urgency", "noticeable risk emphasis", "strong alert-state emphasis", "crisis-dashboard pressure"],
                Awe => ["grounded quantitative scale", "slight sense of scope", "impressive pattern scale", "strong civilization-scale magnitude", "overwhelming systems magnitude"],
                LightingIntensity => ["flat neutral display light", "soft report light", "balanced presentation light", "bright monitor-grade light", "high-luminance control-room light"],
                Saturation => ["muted analytic palette", "restrained category coding", "balanced quantitative color", "rich category differentiation", "vivid high-separation coding"],
                Contrast => ["soft grid separation", "gentle mark-to-ground separation", "balanced axis-and-mark contrast", "crisp plot-and-label separation", "striking alert-grade separation"],
                FocusDepth => ["whole-dashboard clarity", "light focal weighting", "balanced analytical hierarchy", "strong focal callout emphasis", "decisive insight-first hierarchy"],
                ImageCleanliness => ["rough workshop plotting", "lightly polished report finish", "clean publication-grade finish", "refined presentation polish", "immaculate analytical finish"],
                DetailDensity => ["sparse topline metrics", "selective supporting metrics", "rich annotated comparison", "dense multi-variable detail", "exhaustive annotated data load"],
                _ => [],
            };
        }

        return sliderKey switch
        {
            Stylization => ["utilitarian information layout", "lightly designed explainer styling", "polished information-design treatment", "strongly art-directed explainer design", "highly stylized information-graphic treatment"],
            Realism => ["abstract shape-and-icon simplification", "light icon-and-object realism", "clear explanatory realism", "highly convincing instructional realism", "strongly observed explanatory rendering"],
            TextureDepth => ["flat schematic surfaces", "light print-surface character", "clear panel-and-material definition", "rich panel-and-illustration surface detail", "deeply worked explanatory surface relief"],
            NarrativeDensity => ["single-point visual explanation", "light contextual explanation", "layered comparative explanation", "dense multi-part explanatory structure", "system-rich explanatory synthesis"],
            Symbolism => ["literal information cues", "subtle conceptual cues", "suggestive visual metaphors", "pronounced allegorical framing", "mythic conceptual charge"],
            SurfaceAge => ["freshly published finish", "slight print wear", "gentle archival handling", "aged publication character", "time-worn reference-sheet patina"],
            Framing => ["isolated panel crop", "focused information framing", "balanced page-scale framing", "full-layout spread framing", "poster-scale information layout"],
            CameraDistance => ["icon-close viewing distance", "near panel distance", "page-reading distance", "board-reading distance", "wall-display distance"],
            CameraAngle => ["flat-on presentation", "slight desk-view angle", "straightforward viewing angle", "elevated layout view", "steep top-down board view"],
            BackgroundComplexity => ["isolated field", "restrained support grid", "supporting labels-and-panels", "rich auxiliary panels and callouts", "densely layered explanatory backdrop"],
            MotionEnergy => ["static reference layout", "gentle directional flow", "active reading flow", "dynamic process flow", "high kinetic information flow"],
            AtmosphericDepth => ["flat spatial read", "slight layer separation", "clear panel-depth separation", "strong stacked-layer depth", "deep sectional depth"],
            Chaos => ["strictly ordered layout", "mild asymmetrical tension", "lively information density", "controlled visual bustle", "high information overload"],
            Whimsy => ["sober institutional tone", "lightly friendly tone", "approachable educational play", "lively public-facing charm", "bold playful explainer energy"],
            Tension => ["calm explanatory tone", "mild urgency", "noticeable decision pressure", "strong cautionary emphasis", "high-stakes warning structure"],
            Awe => ["grounded scale read", "slight sense of scope", "impressive system scale", "strong sense of magnitude", "overwhelming complexity and scale"],
            LightingIntensity => ["flat even illumination", "soft presentation light", "clear display clarity", "bright exhibit-style illumination", "radiant backlit presentation"],
            Saturation => ["muted print-safe color", "restrained category palette", "balanced informational color", "rich category separation", "vivid high-separation palette"],
            Contrast => ["soft tonal separation", "gentle panel contrast", "balanced hierarchy contrast", "crisp heading-and-callout separation", "striking poster-grade separation"],
            FocusDepth => ["full-layout clarity", "light focal guidance", "balanced hierarchy focus", "strong callout isolation", "decisive focal hierarchy"],
            ImageCleanliness => ["rough sketch-board finish", "lightly polished layout", "clean publication finish", "refined presentation polish", "immaculate presentation finish"],
            DetailDensity => ["sparse key facts", "selective supporting detail", "rich explanatory detail", "dense labeled detail", "exhaustive information load"],
            _ => [],
        };
    }

    private static bool IsDataVizSubdomain(PromptConfiguration configuration)
    {
        return string.Equals(
            NormalizeInfographicDataVisualizationSubdomain(configuration.InfographicDataVisualizationSubdomain),
            InfographicSubdomainDataViz,
            StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeInfographicDataVisualizationSubdomain(string? subdomain)
    {
        if (string.IsNullOrWhiteSpace(subdomain))
        {
            return InfographicSubdomainDefault;
        }

        return string.Equals(subdomain.Trim(), "Data Viz", StringComparison.OrdinalIgnoreCase)
            ? InfographicSubdomainDataViz
            : subdomain.Trim();
    }
}
