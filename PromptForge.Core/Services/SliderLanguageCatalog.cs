using PromptForge.App.Models;

namespace PromptForge.App.Services;

internal static class SliderLanguageCatalog
{
    public const string ArtistInfluenceStrength = "ArtistInfluenceStrength";
    public const string Stylization = "Stylization";
    public const string Realism = "Realism";
    public const string TextureDepth = "TextureDepth";
    public const string NarrativeDensity = "NarrativeDensity";
    public const string Symbolism = "Symbolism";
    public const string SurfaceAge = "SurfaceAge";
    public const string BackgroundComplexity = "BackgroundComplexity";
    public const string MotionEnergy = "MotionEnergy";
    public const string AtmosphericDepth = "AtmosphericDepth";
    public const string Chaos = "Chaos";
    public const string Whimsy = "Whimsy";
    public const string Tension = "Tension";
    public const string Awe = "Awe";
    public const string Saturation = "Saturation";
    public const string Contrast = "Contrast";

    private static readonly IReadOnlyDictionary<string, BundlePhrasePreference> BundlePhrasePreferences =
        new Dictionary<string, BundlePhrasePreference>(StringComparer.OrdinalIgnoreCase)
        {
            ["Playful Chaos"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["lightly interpreted presentation", "restrained authored treatment", "subtle stylistic shaping", "softly stylized rendering"],
                    [Realism] = ["convincingly observed", "convincing live-action realism", "clear realism within surreal logic"],
                    [TextureDepth] = ["light tactile grain", "soft material texture", "soft set-level texture", "soft yarn texture"],
                    [SurfaceAge] = ["fresh-built surface character", "recently completed surface character", "recently finished oneiric surfaces"],
                    [BackgroundComplexity] = ["light environmental support", "restrained scene support", "light symbolic environment"],
                    [Whimsy] = ["lively playful tone", "mischievous visual energy", "bold playful character"],
                    [MotionEnergy] = ["strong directional movement", "high-energy scene motion"],
                    [Chaos] = ["strong compositional instability", "controlled visual turbulence"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["grounded visual treatment"],
                    [BackgroundComplexity] = ["controlled backdrop detail"],
                    [TextureDepth] = ["subtle surface grain"],
                }),
            ["Sacred Stillness"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["restrained image treatment", "minimally stylized presentation"],
                    [Realism] = ["convincingly observed", "clear realistic rendering"],
                    [TextureDepth] = ["soft material texture", "subtle material grain"],
                    [BackgroundComplexity] = ["light environmental support", "restrained scene support"],
                    [MotionEnergy] = ["quietly held composition", "settled image energy"],
                    [Whimsy] = ["solemn tone", "quiet serious mood"],
                    [Tension] = ["quiet dramatic footing", "little dramatic strain"],
                    [Awe] = ["clear sense of wonder", "gentle reverent lift", "overwhelming grandeur"],
                    [Symbolism] = ["clearly symbolic framing", "suggestive symbolic motifs"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Whimsy] = ["straight-faced tone"],
                    [Tension] = ["calm dramatic footing"],
                }),
            ["Dramatic Fracture"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["restrained authored treatment"],
                    [Realism] = ["convincingly observed", "clear realistic rendering", "solid realistic grounding"],
                    [TextureDepth] = ["subtle material grain", "light tactile grain", "soft material texture"],
                    [SurfaceAge] = ["newly finished surfaces", "fresh-built surface character"],
                    [BackgroundComplexity] = ["restrained scene support", "readable surrounding detail"],
                    [Whimsy] = ["unsmiling tone", "severe tone"],
                    [Tension] = ["severe emotional strain", "rising scene tension", "clear dramatic pressure"],
                    [Awe] = ["clear sense of wonder", "strong sense of awe"],
                    [Chaos] = ["controlled visual turbulence", "strong compositional instability"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Whimsy] = ["straight-faced tone"],
                    [Awe] = ["noticeable reverent atmosphere"],
                    [BackgroundComplexity] = ["controlled backdrop detail"],
                }),
            ["Dreamlike Drift"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["lightly interpreted presentation", "subtle stylistic shaping", "soft dreamlike stylization"],
                    [TextureDepth] = ["soft material texture", "soft uncanny texture"],
                    [SurfaceAge] = ["recently completed surface character", "recently finished oneiric surfaces"],
                    [BackgroundComplexity] = ["light environmental support", "light symbolic environment", "restrained surreal backdrop"],
                    [MotionEnergy] = ["soft scene motion", "light oneiric drift"],
                    [AtmosphericDepth] = ["deepened atmospheric layering", "pronounced spatial atmosphere", "vast air-filled perspective"],
                    [Awe] = ["hushed atmosphere of wonder", "clear sense of wonder"],
                    [Symbolism] = ["suggestive symbolic motifs", "suggestive dream motifs"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [BackgroundComplexity] = ["controlled backdrop detail"],
                    [TextureDepth] = ["subtle surface grain"],
                }),
            ["Quiet Wonder"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["restrained image treatment", "lightly interpreted presentation"],
                    [Realism] = ["convincingly observed", "solid realistic grounding"],
                    [BackgroundComplexity] = ["light environmental support", "restrained scene support"],
                    [MotionEnergy] = ["soft scene motion", "settled image energy"],
                    [AtmosphericDepth] = ["deepened atmospheric layering", "pronounced spatial atmosphere"],
                    [Whimsy] = ["light playful touch", "subtle whimsy"],
                    [Awe] = ["clear sense of wonder", "hushed atmosphere of wonder"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Chaos] = ["strong compositional instability"],
                }),
            ["Ominous Calm"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["restrained authored treatment", "grounded image treatment"],
                    [BackgroundComplexity] = ["restrained scene support", "readable surrounding detail"],
                    [MotionEnergy] = ["quietly held composition", "still composition"],
                    [AtmosphericDepth] = ["air-filled spatial depth", "deepened atmospheric layering"],
                    [Whimsy] = ["severe tone", "solemn tone"],
                    [Tension] = ["charged emotional tension", "high dramatic strain", "scene-level tension"],
                    [Saturation] = ["controlled color intensity", "lightly muted palette"],
                    [Contrast] = ["strong tonal separation", "crisp contrast"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Whimsy] = ["playful tone"],
                }),
            ["Mythic Radiance"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["restrained image treatment", "lightly interpreted presentation"],
                    [MotionEnergy] = ["light directional movement", "gentle motion"],
                    [AtmosphericDepth] = ["pronounced spatial atmosphere", "vast air-filled perspective"],
                    [Whimsy] = ["solemn tone", "quiet serious mood"],
                    [Awe] = ["overwhelming grandeur", "strong sense of awe"],
                    [Symbolism] = ["clearly symbolic framing", "suggestive symbolic motifs"],
                    [Saturation] = ["deepened color intensity", "rich color saturation"],
                    [Contrast] = ["strong tonal separation", "crisp contrast"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Whimsy] = ["playful tone"],
                }),
            ["Playful Reverie"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["lightly interpreted presentation", "subtle stylistic shaping", "soft dreamlike stylization"],
                    [BackgroundComplexity] = ["light environmental support", "light symbolic environment"],
                    [MotionEnergy] = ["soft scene motion", "clear movement through the scene"],
                    [Whimsy] = ["lively playful tone", "playful tone"],
                    [Awe] = ["clear sense of wonder", "atmosphere of wonder"],
                    [Saturation] = ["balanced saturation", "measured color intensity"],
                    [Contrast] = ["soft tonal separation", "gentle contrast"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Tension] = ["charged emotional tension"],
                }),
            ["Haunted Stillness"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["restrained authored treatment", "restrained image treatment"],
                    [BackgroundComplexity] = ["restrained scene support", "light environmental support"],
                    [MotionEnergy] = ["still composition", "quietly held composition"],
                    [AtmosphericDepth] = ["pronounced spatial atmosphere", "deepened atmospheric layering"],
                    [Whimsy] = ["severe tone", "unsmiling tone"],
                    [Tension] = ["charged emotional tension", "high dramatic strain", "scene-level tension"],
                    [Awe] = ["clear sense of wonder", "atmosphere of wonder"],
                    [Saturation] = ["subdued color treatment", "muted saturation"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [MotionEnergy] = ["dynamic motion"],
                }),
            ["Charged Spectacle"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["restrained authored treatment", "lightly interpreted presentation"],
                    [BackgroundComplexity] = ["clear environmental support", "readable surrounding detail"],
                    [MotionEnergy] = ["high-energy scene motion", "strong directional movement"],
                    [AtmosphericDepth] = ["pronounced spatial atmosphere", "deepened atmospheric layering"],
                    [Chaos] = ["controlled visual turbulence", "strong compositional instability"],
                    [Whimsy] = ["severe tone", "solemn tone"],
                    [Tension] = ["charged emotional tension", "high dramatic strain"],
                    [Awe] = ["overwhelming grandeur", "strong sense of awe"],
                    [Saturation] = ["deepened color intensity", "rich color saturation"],
                    [Contrast] = ["bold value structure", "strong tonal separation"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [MotionEnergy] = ["soft scene motion"],
                }),
            ["Melancholic Drift"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["restrained image treatment", "grounded image treatment"],
                    [BackgroundComplexity] = ["light environmental support", "restrained scene support"],
                    [MotionEnergy] = ["soft scene motion", "settled image energy"],
                    [AtmosphericDepth] = ["pronounced spatial atmosphere", "deepened atmospheric layering"],
                    [Whimsy] = ["quiet serious mood", "solemn tone"],
                    [Tension] = ["subtle dramatic strain", "gentle scene tension"],
                    [Saturation] = ["lightly muted palette", "controlled color intensity"],
                    [Contrast] = ["gentle contrast", "soft tonal separation"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Whimsy] = ["playful tone"],
                }),
            ["Storybook Tension"] = new(
                PreferredBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Stylization] = ["lightly interpreted presentation", "subtle stylistic shaping"],
                    [BackgroundComplexity] = ["light environmental support", "clear environmental support"],
                    [MotionEnergy] = ["light directional movement", "gentle motion"],
                    [Whimsy] = ["lively playful tone", "playful tone"],
                    [Tension] = ["clear dramatic pressure", "rising scene tension"],
                    [Awe] = ["clear sense of wonder", "atmosphere of wonder"],
                    [NarrativeDensity] = ["noticeable narrative layering", "clear implied story context"],
                    [Symbolism] = ["suggestive symbolic motifs", "noticeable allegorical cues"],
                },
                AvoidedBySlider: new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
                {
                    [Whimsy] = ["severe tone"],
                }),
        };

    private static readonly IReadOnlyDictionary<string, SliderLanguageDefinition> Definitions =
        new Dictionary<string, SliderLanguageDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            [ArtistInfluenceStrength] = new(
                "Controls how explicitly artist influence is named in the prompt language.",
                [
                    Band("Off"),
                    Band("subtle influence", "subtle influence from {artist}", "gentle cues drawn from {artist}", "light stylistic influence from {artist}"),
                    Band("artist-influenced sensibility", "artist-influenced sensibility drawn from {artist}", "stylistic cues shaped by {artist}", "a {artist}-informed sensibility"),
                    Band("strongly shaped", "strongly shaped by {artist}", "clear influence from {artist}", "distinctly guided by {artist}"),
                    Band("deeply informed", "deeply informed by {artist}", "profoundly shaped by {artist}", "steeped in the visual sensibility of {artist}"),
                ],
                [],
                [],
                [],
                ["descriptive phrase", "finishing clause"],
                "Keep the descriptor singular and let the artist profile phrases carry most of the specificity."),
            [Stylization] = new(
                "Controls how grounded or interpretive the overall visual language feels.",
                [
                    Band("grounded visual treatment", "grounded image treatment", "restrained image treatment", "lightly interpreted presentation", "restrained authored treatment"),
                    Band("light stylization", "light stylization", "softly stylized rendering", "subtle stylistic shaping"),
                    Band("stylized rendering", "stylized rendering", "clear stylistic treatment", "noticeably stylized image language"),
                    Band("strong stylization", "strong stylization", "boldly stylized rendering", "strong authored visual language"),
                    Band("highly stylized visual language", "highly stylized visual language", "fully authored stylized treatment", "highly interpretive visual design"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] =
                    [
                        Band("grounded cinematic framing", "grounded cinematic framing", "restrained directorial framing", "naturalistic cinematic treatment"),
                        Band("light directorial stylization", "light directorial stylization", "soft filmic shaping", "lightly stylized cinematic language"),
                        Band("filmic stylization", "filmic stylization", "clear directorial image treatment", "stylized screen language"),
                        Band("strong cinematic stylization", "strong cinematic stylization", "bold directorial framing", "pronounced filmic authorship"),
                        Band("highly stylized cinematic language", "highly stylized cinematic language", "maximal directorial image shaping", "strongly authored filmic language"),
                    ],
                    ["Painterly"] =
                    [
                        Band("grounded painterly treatment", "grounded painterly treatment", "restrained painterly shaping", "naturalistic brush-led treatment"),
                        Band("light painterly stylization", "light painterly stylization", "soft brush-led stylization", "lightly interpreted painterly rendering"),
                        Band("stylized brush-led rendering", "stylized brush-led rendering", "clear painterly stylization", "expressive painted image treatment"),
                        Band("strong painterly stylization", "strong painterly stylization", "bold brush-led authorship", "assertive painterly framing"),
                        Band("highly painterly visual language", "highly painterly visual language", "fully authored painterly stylization", "pronounced painterly design language"),
                    ],
                    ["Yarn Relief"] =
                    [
                        Band("grounded textile shaping", "grounded textile shaping", "restrained relief construction", "naturalistic yarn-built treatment"),
                        Band("light textile stylization", "light textile stylization", "soft relief stylization", "lightly interpreted textile construction"),
                        Band("constructed yarn relief rendering", "constructed yarn relief rendering", "clear textile stylization", "stylized fiber-built relief"),
                        Band("strong yarn relief stylization", "strong yarn relief stylization", "bold textile authorship", "assertive relief-driven shaping"),
                        Band("highly stylized textile relief language", "highly stylized textile relief language", "fully authored yarn relief design", "pronounced textile iconography"),
                    ],
                    ["Stained Glass"] =
                    [
                        Band("grounded leaded-glass structure", "grounded leaded-glass structure", "restrained stained-glass shaping", "simple leaded-glass organization"),
                        Band("light stained-glass stylization", "light stained-glass stylization", "soft ornamental glass stylization", "lightly interpreted lead-line design"),
                        Band("stained-glass image treatment", "stained-glass image treatment", "clear ornamental glass stylization", "stylized leaded-glass rendering"),
                        Band("strong stained-glass stylization", "strong stained-glass stylization", "ornate lead-line authorship", "bold iconographic glass language"),
                        Band("highly stylized stained-glass iconography", "highly stylized stained-glass iconography", "fully authored ornamental glass design", "pronounced leaded-glass symbolism"),
                    ],
                    ["Surreal Symbolic"] =
                    [
                        Band("grounded dreamlike treatment", "grounded dreamlike treatment", "restrained surreal shaping", "minimally stylized oneiric treatment"),
                        Band("light surreal stylization", "light surreal stylization", "soft dreamlike stylization", "lightly interpreted surreal framing"),
                        Band("stylized surreal rendering", "stylized surreal rendering", "clear oneiric stylization", "confident dream-led image treatment"),
                        Band("strong surreal stylization", "strong surreal stylization", "bold dream logic shaping", "assertive surreal authorship"),
                        Band("highly symbolic surreal language", "highly symbolic surreal language", "fully authored oneiric design", "pronounced surreal image language"),
                    ],
                    ["Concept Art"] =
                    [
                        Band("grounded production design treatment", "grounded production design treatment", "restrained concept presentation", "clear production-minded framing"),
                        Band("light concept stylization", "light concept stylization", "soft production stylization", "lightly interpreted design rendering"),
                        Band("stylized concept rendering", "stylized concept rendering", "clear concept-art treatment", "confident design-language rendering"),
                        Band("strong concept-art stylization", "strong concept-art stylization", "bold production design language", "assertive concept presentation"),
                        Band("highly stylized concept-art language", "highly stylized concept-art language", "fully authored production design", "pronounced concept-art framing"),
                    ],
                },
                [],
                [],
                ["descriptive phrase", "finishing clause"],
                "Stylization is high-impact language, so keep it to one selected phrase."),
            [Realism] = new(
                "Controls how explicitly the prompt asks for observed, convincing, or strongly realistic rendering.",
                [
                    Band("omit explicit realism"),
                    Band("loosely realistic", "loosely realistic", "suggestively realistic", "lightly anchored in realism"),
                    Band("moderately realistic", "moderately realistic", "convincingly observed", "solid realistic grounding", "clear realistic rendering"),
                    Band("high visual realism", "high visual realism", "strong visual realism", "convincing realistic rendering"),
                    Band("strongly realistic rendering", "strongly realistic rendering", "deeply observed realism", "highly convincing realistic treatment"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "loosely realistic cinematic rendering", "lightly naturalistic screen realism", "suggestively realistic film framing"),
                        Band("moderately realistic", "moderately realistic cinematic realism", "convincing live-action realism", "clear filmic realism"),
                        Band("high visual realism", "high visual realism in cinematic rendering", "strong live-action realism", "highly convincing cinematic realism"),
                        Band("strongly realistic rendering", "strongly realistic cinematic rendering", "deeply observed screen realism", "feature-film realism"),
                    ],
                    ["Painterly"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "loosely observed realism", "suggestively observed realism", "light painterly realism"),
                        Band("moderately realistic", "painterly realism", "moderately realistic painterly rendering", "convincingly observed painted realism"),
                        Band("high visual realism", "convincing atelier realism", "high visual realism in paint", "strong representational realism"),
                        Band("strongly realistic rendering", "museum-grade representational realism", "deeply observed painterly realism", "highly convincing painted realism"),
                    ],
                    ["Yarn Relief"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "suggestively realistic textile forms", "loosely realistic fiber sculpture", "lightly observed yarn-built form"),
                        Band("moderately realistic", "recognizably realistic fiber-built forms", "convincingly observed yarn-built anatomy", "moderately realistic textile figuration"),
                        Band("high visual realism", "highly convincing textile realism", "strong realistic fiber-built rendering", "clear realistic yarn relief"),
                        Band("strongly realistic rendering", "museum-caliber yarn relief realism", "strikingly realistic yarn-built rendering", "deeply observed textile realism"),
                    ],
                    ["Stained Glass"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "loosely realistic sacred figuration", "lightly naturalistic leaded-glass figuration", "suggestively realistic iconographic form"),
                        Band("moderately realistic", "moderately realistic leaded-glass figuration", "clear stained-glass realism", "convincing figurative glass rendering"),
                        Band("high visual realism", "highly legible luminous figuration", "strong figurative realism in glass", "high visual realism in stained glass"),
                        Band("strongly realistic rendering", "cathedral-grade stained-glass realism", "strongly realistic stained-glass figuration", "deeply observed luminous figuration"),
                    ],
                    ["Surreal Symbolic"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "loosely realistic dream imagery", "lightly anchored surreal realism", "suggestively realistic uncanny forms"),
                        Band("moderately realistic", "moderately realistic surreal rendering", "clear realism within surreal logic", "convincing dreamlike realism"),
                        Band("high visual realism", "high visual realism within surreal logic", "strong uncanny realism", "highly convincing surreal realism"),
                        Band("strongly realistic rendering", "hyper-real surreal rendering", "deeply observed visionary realism", "strongly realistic oneiric rendering"),
                    ],
                    ["Concept Art"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "loosely realistic design rendering", "lightly naturalistic production design", "suggestively realistic concept rendering"),
                        Band("moderately realistic", "moderately realistic design rendering", "clear production realism", "convincing concept-art realism"),
                        Band("high visual realism", "high visual realism for production art", "strong realistic concept rendering", "highly convincing design realism"),
                        Band("strongly realistic rendering", "portfolio-grade concept realism", "strongly realistic concept rendering", "deeply observed production realism"),
                    ],
                },
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Yarn"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "suggestively realistic textile forms", "loosely realistic yarn-built forms", "lightly observed fiber structure"),
                        Band("moderately realistic", "recognizably realistic fiber-built forms", "moderately realistic yarn rendering", "clear textile realism"),
                        Band("high visual realism", "highly convincing textile realism", "strong realistic fiber rendering", "high visual realism in yarn"),
                        Band("strongly realistic rendering", "strikingly realistic yarn-built rendering", "deeply observed textile realism", "highly convincing fiber realism"),
                    ],
                    ["Glass"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "loosely realistic translucent forms", "lightly observed glass realism", "suggestively realistic glass-formed shapes"),
                        Band("moderately realistic", "moderately realistic glass-formed rendering", "clear translucent realism", "convincing luminous realism"),
                        Band("high visual realism", "highly legible glass realism", "strong realistic translucence", "high visual realism in glass"),
                        Band("strongly realistic rendering", "strikingly realistic luminous glass rendering", "deeply observed glass realism", "strongly realistic translucent rendering"),
                    ],
                    ["Paint"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "loosely observed realism", "light painterly realism", "suggestively observed realism"),
                        Band("moderately realistic", "painterly realism", "moderately realistic painted rendering", "convincing representational paint handling"),
                        Band("high visual realism", "convincing painterly realism", "strong representational realism", "high visual realism in paint"),
                        Band("strongly realistic rendering", "museum-grade representational realism", "deeply observed painted realism", "highly convincing painterly realism"),
                    ],
                    ["Metal"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "loosely realistic metallic forms", "lightly naturalistic metal rendering", "suggestively realistic metal surfaces"),
                        Band("moderately realistic", "moderately realistic metallic rendering", "clear metal realism", "convincing reflective realism"),
                        Band("high visual realism", "high visual realism in reflective metal", "strong realistic metallic rendering", "highly convincing metal realism"),
                        Band("strongly realistic rendering", "strongly realistic metallic surface rendering", "deeply observed metal realism", "highly convincing reflective realism"),
                    ],
                    ["Ink"] =
                    [
                        Band("omit explicit realism"),
                        Band("loosely realistic", "loosely realistic ink-drawn forms", "lightly observed ink realism", "suggestively realistic line-defined form"),
                        Band("moderately realistic", "moderately realistic ink rendering", "clear ink-defined realism", "convincing line-led realism"),
                        Band("high visual realism", "highly legible ink realism", "strong ink-defined realism", "high visual realism in ink"),
                        Band("strongly realistic rendering", "strongly realistic ink-defined rendering", "deeply observed ink realism", "highly convincing line-based realism"),
                    ],
                },
                [],
                ["descriptive phrase"],
                "Keep realism language sparse. The low band intentionally emits nothing to preserve manual and stylized cases."),
            [TextureDepth] = new(
                "Controls how much tactile surface information and material relief the prompt calls for.",
                [
                    Band("minimal added texture", "minimal added texture", "clean surface treatment", "smooth surface read"),
                    Band("light surface texture", "light surface texture", "light tactile grain", "soft material texture", "subtle material grain"),
                    Band("clear material texture", "clear material texture", "readable material grain", "visible tactile texture"),
                    Band("rich tactile surface detail", "rich tactile surface detail", "deepened material texture", "pronounced tactile build-up"),
                    Band("deeply worked tactile relief", "deeply worked tactile relief", "heavily worked surface relief", "deep tactile material articulation"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] = [Band("minimal added texture", "minimal added texture", "clean cinematic surface treatment", "smooth production finish"), Band("light surface texture", "light cinematic surface grain", "subtle production texture", "soft set-level texture"), Band("clear material texture", "readable production texture", "clear material read", "visible cinematic surface detail"), Band("rich tactile surface detail", "rich cinematic surface detail", "deepened production texture", "pronounced tactile scene texture"), Band("deeply worked tactile relief", "hyper-detailed cinematic surface finish", "deeply resolved production texture", "heavily worked cinematic surface articulation")],
                    ["Painterly"] = [Band("minimal added texture", "minimal added texture", "clean painted surface treatment", "smooth paint handling"), Band("light surface texture", "light brush grain", "subtle paint tooth", "soft painterly texture"), Band("clear material texture", "visible paint handling", "clear painterly texture", "readable brush-led surface"), Band("rich tactile surface detail", "rich impasto and brush build-up", "deepened painterly texture", "pronounced brush-built relief"), Band("deeply worked tactile relief", "deeply worked painterly surface relief", "heavily built impasto", "museum-surface tactile relief")],
                    ["Yarn Relief"] = [Band("minimal added texture", "minimal added texture", "clean textile surface treatment", "smooth fiber read"), Band("light surface texture", "light thread texture", "subtle fiber grain", "soft textile surface texture"), Band("clear material texture", "clearly layered yarn texture", "readable fiber texture", "visible textile structure"), Band("rich tactile surface detail", "rich tactile fiber build-up", "deepened textile texture", "pronounced yarn relief"), Band("deeply worked tactile relief", "deep textile relief and knotted depth", "heavily worked yarn relief", "deep tactile fiber articulation")],
                    ["Stained Glass"] = [Band("minimal added texture", "minimal added texture", "clean glass surface treatment", "smooth leaded-glass finish"), Band("light surface texture", "light glass texture", "subtle lead-line texture", "soft faceted surface read"), Band("clear material texture", "leaded-glass surface definition", "clear faceted glass texture", "readable ornamental glass texture"), Band("rich tactile surface detail", "rich faceted glass texture", "deepened glass surface detail", "pronounced leaded-glass texture"), Band("deeply worked tactile relief", "deeply worked glass-and-leading relief", "heavily faceted stained-glass texture", "deep tactile glass articulation")],
                    ["Surreal Symbolic"] = [Band("minimal added texture", "minimal added texture", "clean oneiric surface treatment", "smooth surreal surface read"), Band("light surface texture", "light illusionistic surface grain", "subtle dream-texture", "soft uncanny texture"), Band("clear material texture", "finely worked surreal texture", "clear uncanny surface texture", "readable dreamlike material grain"), Band("rich tactile surface detail", "rich uncanny surface detail", "deepened surreal texture", "pronounced oneiric surface build-up"), Band("deeply worked tactile relief", "hyper-finished surreal surface articulation", "heavily worked uncanny relief", "deep tactile dream-surface detail")],
                    ["Concept Art"] = [Band("minimal added texture", "minimal added texture", "clean design-surface treatment", "smooth production finish"), Band("light surface texture", "light design texture", "subtle material breakup", "soft production texture"), Band("clear material texture", "clear material read", "readable production-surface texture", "visible design-surface detail"), Band("rich tactile surface detail", "rich production-surface detail", "deepened hard-material texture", "pronounced material breakup"), Band("deeply worked tactile relief", "deeply resolved material finish", "heavily worked production texture", "deep tactile design-surface articulation")],
                },
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Yarn"] = [Band("minimal added texture", "minimal added texture", "clean textile surface treatment", "smooth yarn read"), Band("light surface texture", "light thread texture", "subtle fiber grain", "soft yarn texture"), Band("clear material texture", "clearly layered yarn texture", "visible fiber structure", "readable textile texture"), Band("rich tactile surface detail", "rich tactile fiber build-up", "deepened yarn texture", "pronounced textile relief"), Band("deeply worked tactile relief", "deep textile relief and knotted depth", "heavily worked yarn texture", "deep tactile fiber relief")],
                    ["Glass"] = [Band("minimal added texture", "minimal added texture", "clean glass surface treatment", "smooth translucent finish"), Band("light surface texture", "light glass texture", "subtle faceted texture", "soft translucent surface grain"), Band("clear material texture", "clear faceted glass texture", "readable glass surface definition", "visible leaded texture"), Band("rich tactile surface detail", "rich faceted glass texture", "deepened glass surface detail", "pronounced translucent texture"), Band("deeply worked tactile relief", "deeply worked glass relief", "heavily faceted glass texture", "deep tactile glass articulation")],
                    ["Paint"] = [Band("minimal added texture", "minimal added texture", "clean painted surface treatment", "smooth pigment finish"), Band("light surface texture", "light brush grain", "subtle paint tooth", "soft pigment texture"), Band("clear material texture", "visible paint handling", "readable brushwork", "clear painterly texture"), Band("rich tactile surface detail", "rich impasto and brush build-up", "deepened pigment texture", "pronounced paint relief"), Band("deeply worked tactile relief", "deeply worked painterly surface relief", "heavily built impasto", "deep tactile pigment relief")],
                    ["Metal"] = [Band("minimal added texture", "minimal added texture", "clean metallic finish", "smooth metal read"), Band("light surface texture", "light metallic grain", "subtle machined texture", "soft reflective surface texture"), Band("clear material texture", "clearly machined metal texture", "readable forged surface detail", "visible metallic grain"), Band("rich tactile surface detail", "rich forged surface detail", "deepened metallic texture", "pronounced industrial surface build-up"), Band("deeply worked tactile relief", "deeply worked metallic relief", "heavily resolved metal texture", "deep tactile hard-surface articulation")],
                    ["Ink"] = [Band("minimal added texture", "minimal added texture", "clean ink surface treatment", "smooth line-led finish"), Band("light surface texture", "light ink grain", "subtle paper-and-ink texture", "soft ink texture"), Band("clear material texture", "clear inked line texture", "readable ink layering", "visible line-built texture"), Band("rich tactile surface detail", "rich ink layering", "deepened ink surface build-up", "pronounced line-and-pool texture"), Band("deeply worked tactile relief", "deeply saturated ink surface build-up", "heavily worked ink relief", "deep tactile ink articulation")],
                },
                [],
                ["descriptive phrase", "finishing clause"],
                "Texture language can stack with material phrases quickly, so avoid selecting more than one texture phrase per slider."),
            [NarrativeDensity] = new(
                "Controls how much implied story, context, and multi-read narrative information appears in the prompt.",
                [
                    Band("simple single-read image", "simple single-read image", "single-read visual idea", "clear single-moment image"),
                    Band("light narrative layering", "light narrative layering", "subtle story cues", "gentle narrative suggestion"),
                    Band("layered storytelling cues", "layered storytelling cues", "clear implied story context", "noticeable narrative layering"),
                    Band("dense implied story", "dense implied story", "rich storytelling cues", "strong implied narrative context"),
                    Band("world-rich narrative density", "world-rich narrative density", "deep story-world layering", "densely implied narrative world"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] = [Band("simple single-read image", "single-shot visual beat", "clear cinematic moment", "simple scene beat"), Band("light narrative layering", "light narrative layering", "subtle scene context", "gentle cinematic storytelling cues"), Band("layered storytelling cues", "scene-driven storytelling cues", "clear narrative scene context", "noticeable cinematic story layering"), Band("dense implied story", "densely implied cinematic narrative", "rich scene-level storytelling", "strong narrative scene pressure"), Band("world-rich narrative density", "world-rich cinematic narrative density", "deep film-world implication", "densely layered story-world cinema")],
                    ["Painterly"] = [Band("simple single-read image", "single-read visual idea", "clear tableau moment", "simple painterly image read"), Band("light narrative layering", "light narrative suggestion", "subtle tableau storytelling", "gentle painterly story cues"), Band("layered storytelling cues", "layered painterly storytelling", "clear narrative tableau cues", "noticeable story layering"), Band("dense implied story", "densely implied narrative tableau", "rich tableau storytelling", "strong painted story context"), Band("world-rich narrative density", "world-rich tableau narrative density", "deeply storied painterly world", "densely layered narrative tableau")],
                    ["Yarn Relief"] = [Band("simple single-read image", "single-read textile motif", "clear fiber-built image read", "simple textile scene read"), Band("light narrative layering", "light narrative stitching", "subtle textile story cues", "gentle fiber-built narrative suggestion"), Band("layered storytelling cues", "layered textile storytelling", "clear narrative fiber cues", "noticeable textile story layering"), Band("dense implied story", "densely implied fiber-built story", "rich textile narrative cues", "strong stitched story context"), Band("world-rich narrative density", "world-rich textile narrative density", "deeply storied fiber-built world", "densely layered yarn-built storytelling")],
                    ["Stained Glass"] = [Band("simple single-read image", "single iconographic read", "clear devotional image read", "simple emblematic scene read"), Band("light narrative layering", "light devotional narrative", "subtle iconographic story cues", "gentle stained-glass narrative suggestion"), Band("layered storytelling cues", "layered symbolic storytelling", "clear devotional scene context", "noticeable iconographic story layering"), Band("dense implied story", "densely storied stained-glass tableau", "rich devotional storytelling", "strong iconographic narrative context"), Band("world-rich narrative density", "world-rich sacred narrative density", "deeply storied stained-glass world", "densely layered iconographic storytelling")],
                    ["Surreal Symbolic"] = [Band("simple single-read image", "single dream image", "clear oneiric image read", "simple surreal image read"), Band("light narrative layering", "light symbolic narrative", "subtle dream-story cues", "gentle oneiric narrative suggestion"), Band("layered storytelling cues", "layered oneiric storytelling", "clear surreal story context", "noticeable dream-logic layering"), Band("dense implied story", "densely implied surreal narrative world", "rich dream-story cues", "strong oneiric story pressure"), Band("world-rich narrative density", "world-rich surreal narrative density", "deeply storied dream world", "densely layered symbolic narrative")],
                    ["Concept Art"] = [Band("simple single-read image", "single-read design idea", "clear production moment", "simple worldbuilding image read"), Band("light narrative layering", "light worldbuilding cues", "subtle story-world suggestion", "gentle production narrative context"), Band("layered storytelling cues", "layered story-world context", "clear worldbuilding narrative cues", "noticeable production story layering"), Band("dense implied story", "densely implied narrative worldbuilding", "rich story-world detail", "strong production narrative context"), Band("world-rich narrative density", "world-rich worldbuilding density", "deeply layered production lore", "densely implied story-world design")],
                },
                [],
                [],
                ["scene clause", "descriptive phrase"],
                "Narrative Density and Symbolism can collide. Favor concrete story cues here and leave allegorical weight to Symbolism."),
            [Symbolism] = new(
                "Controls how literal or allegorical the prompt language becomes.",
                [
                    Band("mostly literal", "mostly literal", "primarily literal image language", "direct visual read"),
                    Band("subtle symbolic cues", "subtle symbolic cues", "light symbolic undertones", "quiet allegorical hints"),
                    Band("suggestive symbolic motifs", "suggestive symbolic motifs", "clear symbolic motifs", "noticeable allegorical cues"),
                    Band("pronounced allegory", "pronounced allegory", "strong allegorical charge", "clearly symbolic framing"),
                    Band("mythic symbolic charge", "mythic symbolic charge", "heavy allegorical weight", "myth-laden symbolic framing"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Stained Glass"] = [Band("mostly literal", "mostly literal", "direct iconographic read", "plain devotional read"), Band("subtle symbolic cues", "subtle iconographic cues", "light devotional symbolism", "quiet sacred motifs"), Band("suggestive symbolic motifs", "suggestive devotional motifs", "clear iconographic motifs", "noticeable sacred symbolism"), Band("pronounced allegory", "pronounced allegorical iconography", "strong devotional allegory", "clearly symbolic sacred framing"), Band("mythic symbolic charge", "mythic sacred symbolism", "heavy iconographic charge", "cathedral-scale allegorical weight")],
                    ["Surreal Symbolic"] = [Band("mostly literal", "mostly literal", "direct surreal read", "plain oneiric read"), Band("subtle symbolic cues", "subtle surreal symbols", "light dream-symbol cues", "quiet uncanny symbolism"), Band("suggestive symbolic motifs", "suggestive dream motifs", "clear symbolic dream logic", "noticeable oneiric symbolism"), Band("pronounced allegory", "pronounced allegorical symbolism", "strong surreal allegory", "clearly symbolic dream framing"), Band("mythic symbolic charge", "mythic symbolic charge", "heavy visionary allegory", "myth-laden oneiric symbolism")],
                    ["Concept Art"] = [Band("mostly literal", "mostly literal", "direct worldbuilding read", "plain design read"), Band("subtle symbolic cues", "subtle worldbuilding motifs", "light symbolic design cues", "quiet allegorical design hints"), Band("suggestive symbolic motifs", "suggestive symbolic design motifs", "clear emblematic design cues", "noticeable allegorical worldbuilding"), Band("pronounced allegory", "pronounced allegorical design motifs", "strong symbolic worldbuilding", "clearly allegorical design framing"), Band("mythic symbolic charge", "mythic symbolic worldbuilding", "heavy emblematic charge", "myth-laden design symbolism")],
                },
                [],
                [],
                ["mood clause", "descriptive phrase"],
                "Symbolism can dominate quickly. Keep the selected phrase concise so it supports rather than replaces the subject."),
            [SurfaceAge] = new(
                "Controls how newly finished or time-worn the described surfaces feel.",
                [
                    Band("freshly finished surfaces", "newly finished surfaces", "fresh-built surface character", "recently completed surface character"),
                    Band("subtle patina", "subtle patina", "light surface wear", "gentle signs of age"),
                    Band("gentle weathering", "gentle weathering", "softly weathered surfaces", "noticeable time-softened wear"),
                    Band("aged surface character", "aged surface character", "time-worn surface detail", "pronounced patina and wear"),
                    Band("time-worn patina", "time-worn patina", "deeply aged surface character", "heavily weathered patina"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Painterly"] = [Band("freshly finished surfaces", "freshly laid paint surfaces", "clean gallery-fresh paint", "newly finished painted surfaces"), Band("subtle patina", "subtle aged varnish character", "light gallery patina", "gentle paint-surface wear"), Band("gentle weathering", "weathered painted patina", "softly aged paint surfaces", "noticeable varnish-softened wear"), Band("aged surface character", "time-softened gallery-surface patina", "aged painterly surface character", "pronounced painted wear"), Band("time-worn patina", "time-softened museum-surface patina", "deeply aged painted patina", "heavily weathered gallery-surface character")],
                    ["Yarn Relief"] = [Band("freshly finished surfaces", "fresh textile surfaces", "clean newly assembled fibers", "recently finished textile surfaces"), Band("subtle patina", "soft handled-fiber wear", "light textile patina", "gentle heirloom wear"), Band("gentle weathering", "weathered fiber character", "softly aged textile surfaces", "noticeable handled-fiber wear"), Band("aged surface character", "time-worn textile patina", "aged heirloom textile character", "pronounced fiber wear"), Band("time-worn patina", "time-worn heirloom textile patina", "deeply aged textile character", "heavily weathered fiber patina")],
                    ["Stained Glass"] = [Band("freshly finished surfaces", "freshly set glass surfaces", "clean newly leaded glass", "recently assembled glass panels"), Band("subtle patina", "subtle devotional patina", "light chapel wear", "gentle lead-and-glass aging"), Band("gentle weathering", "weathered glass-and-lead character", "softly aged stained glass", "noticeable devotional wear"), Band("aged surface character", "aged cathedral-glass patina", "strong old-glass character", "pronounced sacred patina"), Band("time-worn patina", "deeply aged cathedral-glass patina", "time-worn sacred glass character", "heavily weathered lead-and-glass patina")],
                    ["Surreal Symbolic"] = [Band("freshly finished surfaces", "fresh illusionistic surfaces", "clean newly formed dream surfaces", "recently finished oneiric surfaces"), Band("subtle patina", "slight antique dream patina", "light oneiric wear", "gentle archival dream aging"), Band("gentle weathering", "weathered oneiric surface character", "softly aged dream surfaces", "noticeable antique surreal wear"), Band("aged surface character", "time-worn dreamlike patina", "aged visionary surface character", "pronounced dream-surface wear"), Band("time-worn patina", "deeply time-worn dreamlike patina", "heavily aged oneiric surface character", "antique visionary patina")],
                },
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Yarn"] = [Band("freshly finished surfaces", "fresh textile surfaces", "clean newly assembled fibers", "recently finished yarn surfaces"), Band("subtle patina", "soft handled-fiber wear", "light textile patina", "gentle yarn wear"), Band("gentle weathering", "weathered fiber character", "softly aged yarn surfaces", "noticeable textile wear"), Band("aged surface character", "time-worn textile patina", "aged fiber character", "pronounced yarn wear"), Band("time-worn patina", "deeply aged textile patina", "heavily weathered fiber character", "heirloom yarn patina")],
                    ["Glass"] = [Band("freshly finished surfaces", "freshly formed glass surfaces", "clean newly finished glass", "recently finished translucent surfaces"), Band("subtle patina", "subtle glass patina", "light devotional wear", "gentle lead-and-glass aging"), Band("gentle weathering", "weathered glass character", "softly aged translucent surfaces", "noticeable glass wear"), Band("aged surface character", "aged glass patina", "time-worn translucent character", "pronounced glass weathering"), Band("time-worn patina", "deeply aged glass patina", "heavily weathered translucent character", "ancient glass wear")],
                    ["Paint"] = [Band("freshly finished surfaces", "freshly laid paint surfaces", "clean newly finished paint", "recently completed painted surfaces"), Band("subtle patina", "subtle aged varnish character", "light painterly patina", "gentle paint wear"), Band("gentle weathering", "weathered painted patina", "softly aged paint surfaces", "noticeable varnish-softened wear"), Band("aged surface character", "aged painted surface character", "time-worn paint finish", "pronounced painted patina"), Band("time-worn patina", "time-softened museum-surface patina", "deeply aged paint character", "heavily weathered painted patina")],
                    ["Metal"] = [Band("freshly finished surfaces", "freshly forged metal", "clean newly fabricated metal", "recently finished metallic surfaces"), Band("subtle patina", "subtle metallic wear", "light alloy patina", "gentle industrial wear"), Band("gentle weathering", "weathered metal patina", "softly aged metallic surfaces", "noticeable alloy wear"), Band("aged surface character", "time-worn metallic character", "aged metal patina", "pronounced industrial wear"), Band("time-worn patina", "oxidized time-worn metal character", "deeply aged metallic patina", "heavily weathered alloy surfaces")],
                    ["Ink"] = [Band("freshly finished surfaces", "fresh ink surfaces", "clean newly inked surfaces", "recently finished ink work"), Band("subtle patina", "subtle archival wear", "light paper-and-ink aging", "gentle archival patina"), Band("gentle weathering", "weathered ink character", "softly aged ink surfaces", "noticeable archival wear"), Band("aged surface character", "aged archival ink character", "time-worn ink patina", "pronounced paper-and-ink wear"), Band("time-worn patina", "aged archival ink patina", "deeply weathered ink character", "heavily time-worn ink surfaces")],
                },
                [],
                ["descriptive phrase", "finishing clause"],
                "Surface Age works best as a single modifier. It can overpower saturation and contrast if phrased too vividly."),
            [BackgroundComplexity] = new(
                "Controls how spare or layered the environment and backdrop become.",
                [
                    Band("minimal background", "minimal background", "spare background", "clean backdrop"),
                    Band("restrained background", "restrained background", "light environmental support", "restrained scene support"),
                    Band("supporting environment", "supporting environment", "clear environmental support", "readable surrounding detail"),
                    Band("rich environmental detail", "rich environmental detail", "layered environmental support", "densely worked backdrop detail"),
                    Band("densely layered environment", "densely layered environment", "deeply built surrounding detail", "heavily layered environmental context"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] = [Band("minimal background", "minimal background", "spare set dressing", "clean cinematic backdrop"), Band("restrained background", "restrained set dressing", "light production detail", "controlled cinematic environment"), Band("supporting environment", "supporting production detail", "clear environmental staging", "readable scene backdrop"), Band("rich environmental detail", "rich environmental staging", "layered set detail", "densely worked scene environment"), Band("densely layered environment", "densely layered cinematic environment", "deeply built production environment", "heavily layered scene world")],
                    ["Painterly"] = [Band("minimal background", "minimal background", "spare painted backdrop", "clean painterly backdrop"), Band("restrained background", "restrained painted backdrop", "light surrounding brushwork", "controlled environmental painting"), Band("supporting environment", "supporting painted environment", "clear painterly setting", "readable backdrop detail"), Band("rich environmental detail", "richly worked painterly setting", "layered painted environment", "densely worked tableau backdrop"), Band("densely layered environment", "densely layered painted environment", "deeply built painterly setting", "heavily layered tableau environment")],
                    ["Yarn Relief"] = [Band("minimal background", "minimal textile background", "spare fiber-built backdrop", "clean textile backdrop"), Band("restrained background", "restrained textile backdrop", "light stitched environment", "controlled fiber-built setting"), Band("supporting environment", "supporting fiber-built environment", "clear textile setting", "readable stitched backdrop"), Band("rich environmental detail", "rich layered textile environment", "layered fiber-built setting", "densely worked textile backdrop"), Band("densely layered environment", "densely layered yarn-built environment", "deeply built textile setting", "heavily layered fiber environment")],
                    ["Stained Glass"] = [Band("minimal background", "minimal glass backdrop", "spare ornamental backdrop", "clean iconographic background"), Band("restrained background", "restrained ornamental backdrop", "light devotional setting", "controlled leaded-glass environment"), Band("supporting environment", "supporting iconographic setting", "clear ornamental environment", "readable sacred backdrop"), Band("rich environmental detail", "rich ornamental environment", "layered stained-glass setting", "densely worked iconographic backdrop"), Band("densely layered environment", "densely layered stained-glass environment", "deeply built devotional setting", "heavily layered ornamental world")],
                    ["Surreal Symbolic"] = [Band("minimal background", "minimal dream background", "spare oneiric backdrop", "clean surreal backdrop"), Band("restrained background", "restrained surreal backdrop", "light symbolic environment", "controlled dream setting"), Band("supporting environment", "supporting symbolic environment", "clear oneiric setting", "readable surreal backdrop"), Band("rich environmental detail", "rich oneiric environment", "layered dream-world detail", "densely worked surreal backdrop"), Band("densely layered environment", "densely layered surreal environment", "deeply built oneiric setting", "heavily layered dream world")],
                    ["Concept Art"] = [Band("minimal background", "minimal background", "spare worldbuilding backdrop", "clean design backdrop"), Band("restrained background", "restrained worldbuilding cues", "light environmental design", "controlled production backdrop"), Band("supporting environment", "supporting environmental design", "clear worldbuilding support", "readable production setting"), Band("rich environmental detail", "rich worldbuilding detail", "layered environmental design", "densely worked production setting"), Band("densely layered environment", "densely layered production environment", "deeply built worldbuilding context", "heavily layered design environment")],
                },
                [],
                [],
                ["compositional clause", "scene clause"],
                "Background Complexity should stay concrete. Avoid symbolic or mood-heavy wording here."),
            [MotionEnergy] = new(
                "Controls how still or kinetic the scene language feels.",
                [
                    Band("still composition", "still composition", "settled image energy", "quietly held composition"),
                    Band("gentle motion", "gentle motion", "light directional movement", "soft scene motion"),
                    Band("active scene energy", "active scene energy", "clear movement through the scene", "noticeable kinetic flow"),
                    Band("dynamic motion", "dynamic motion", "strong directional movement", "high-energy scene motion"),
                    Band("high kinetic energy", "high kinetic energy", "surging motion energy", "full kinetic momentum"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] = [Band("still composition", "still cinematic frame", "held cinematic frame", "quiet screen presence"), Band("gentle motion", "gentle screen motion", "light cinematic movement", "soft directorial motion"), Band("active scene energy", "active cinematic energy", "clear scene momentum", "noticeable filmic movement"), Band("dynamic motion", "dynamic action energy", "strong cinematic motion", "high-energy screen movement"), Band("high kinetic energy", "high kinetic cinematic motion", "surging action momentum", "full-throttle cinematic movement")],
                    ["Painterly"] = [Band("still composition", "still composition", "held painterly balance", "quiet tableau energy"), Band("gentle motion", "gentle gestural motion", "light painterly movement", "soft brush-led motion"), Band("active scene energy", "active painterly movement", "clear gestural energy", "noticeable brush-led motion"), Band("dynamic motion", "dynamic brush-led motion", "strong painterly movement", "high-energy gestural sweep"), Band("high kinetic energy", "high-velocity painterly motion", "surging gestural energy", "full kinetic brush momentum")],
                    ["Yarn Relief"] = [Band("still composition", "still textile composition", "held fiber-built balance", "quiet textile energy"), Band("gentle motion", "gentle textile drift", "light fiber motion", "soft stitched movement"), Band("active scene energy", "active woven movement", "clear textile momentum", "noticeable fiber-driven motion"), Band("dynamic motion", "dynamic fiber motion", "strong textile movement", "high-energy woven sweep"), Band("high kinetic energy", "high kinetic textile energy", "surging fiber momentum", "full kinetic textile motion")],
                    ["Stained Glass"] = [Band("still composition", "still iconographic composition", "held ornamental balance", "quiet devotional energy"), Band("gentle motion", "gentle directional sweep", "light ornamental motion", "soft lead-line movement"), Band("active scene energy", "active glass-led movement", "clear iconographic momentum", "noticeable lead-line flow"), Band("dynamic motion", "dynamic lead-line motion", "strong ornamental movement", "high-energy stained-glass sweep"), Band("high kinetic energy", "high kinetic stained-glass motion", "surging lead-line momentum", "full kinetic ornamental flow")],
                    ["Surreal Symbolic"] = [Band("still composition", "still oneiric composition", "held dream balance", "quiet surreal energy"), Band("gentle motion", "gentle dream motion", "light oneiric drift", "soft surreal movement"), Band("active scene energy", "active surreal energy", "clear dream momentum", "noticeable oneiric motion"), Band("dynamic motion", "dynamic dreamlike motion", "strong surreal movement", "high-energy dream sweep"), Band("high kinetic energy", "high kinetic dream energy", "surging oneiric momentum", "full kinetic dream motion")],
                },
                [],
                [],
                ["scene clause", "compositional clause"],
                "Motion Energy and Chaos both affect dynamism. Keep Motion Energy focused on movement, not instability."),
            [AtmosphericDepth] = new(
                "Controls how much air, recession, and layered spatial atmosphere the prompt calls for.",
                [
                    Band("limited atmospheric depth", "limited atmospheric depth", "shallow spatial recession", "minimal air perspective"),
                    Band("slight recession", "slight atmospheric recession", "light depth falloff", "soft spatial recession"),
                    Band("air-filled spatial depth", "air-filled spatial depth", "clear atmospheric recession", "noticeable spatial layering"),
                    Band("luminous depth layering", "luminous depth layering", "deepened atmospheric layering", "pronounced spatial atmosphere"),
                    Band("deep atmospheric perspective", "deep atmospheric perspective", "heavily layered atmospheric depth", "vast air-filled perspective"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] = [Band("limited atmospheric depth", "limited atmospheric depth", "shallow cinematic recession", "minimal filmic depth haze"), Band("slight recession", "slight atmospheric falloff", "light cinematic recession", "soft screen-depth layering"), Band("air-filled spatial depth", "air-filled cinematic depth", "clear filmic atmospheric depth", "noticeable theatrical recession"), Band("luminous depth layering", "luminous theatrical depth", "deepened cinematic atmosphere", "pronounced layered screen depth"), Band("deep atmospheric perspective", "deeply layered cinematic atmosphere", "vast cinematic depth", "heavily tiered filmic atmosphere")],
                    ["Painterly"] = [Band("limited atmospheric depth", "limited atmospheric depth", "shallow aerial perspective", "minimal painted recession"), Band("slight recession", "slight aerial recession", "light painterly falloff", "soft atmospheric brush depth"), Band("air-filled spatial depth", "painted atmospheric depth", "clear aerial perspective", "noticeable painterly recession"), Band("luminous depth layering", "luminous painterly depth", "deepened atmospheric layering in paint", "pronounced painted atmosphere"), Band("deep atmospheric perspective", "deeply layered atmospheric perspective", "vast painterly recession", "heavily tiered painted depth")],
                    ["Yarn Relief"] = [Band("limited atmospheric depth", "limited atmospheric depth", "shallow textile recession", "minimal relief-space depth"), Band("slight recession", "slight textile depth recession", "light fiber-space falloff", "soft relief recession"), Band("air-filled spatial depth", "layered fiber depth", "clear textile spatial depth", "noticeable relief-space layering"), Band("luminous depth layering", "luminous textile spatial depth", "deepened relief atmosphere", "pronounced fiber-depth layering"), Band("deep atmospheric perspective", "deeply layered relief-space perspective", "vast textile recession", "heavily tiered fiber depth")],
                    ["Stained Glass"] = [Band("limited atmospheric depth", "limited atmospheric depth", "shallow glass-space recession", "minimal luminous layering"), Band("slight recession", "slight glass-depth separation", "light leaded-glass recession", "soft luminous falloff"), Band("air-filled spatial depth", "layered leaded-glass depth", "clear luminous spatial layering", "noticeable iconographic recession"), Band("luminous depth layering", "luminous glass depth", "deepened stained-glass atmosphere", "pronounced tiered luminosity"), Band("deep atmospheric perspective", "deeply tiered stained-glass spatial layering", "vast luminous recession", "heavily layered sacred depth")],
                    ["Surreal Symbolic"] = [Band("limited atmospheric depth", "limited atmospheric depth", "shallow dream recession", "minimal oneiric falloff"), Band("slight recession", "slight dreamlike recession", "light surreal depth falloff", "soft oneiric recession"), Band("air-filled spatial depth", "air-filled oneiric depth", "clear surreal atmospheric depth", "noticeable dream-space layering"), Band("luminous depth layering", "luminous surreal depth", "deepened oneiric atmosphere", "pronounced visionary depth"), Band("deep atmospheric perspective", "deeply layered oneiric perspective", "vast surreal recession", "heavily tiered dream atmosphere")],
                },
                [],
                [],
                ["scene clause", "mood clause"],
                "Atmospheric Depth should read as space and air, not as fog for its own sake."),
            [Chaos] = new(
                "Controls how orderly or unstable the composition language feels.",
                [
                    Band("controlled composition", "controlled composition", "stable visual order", "disciplined compositional balance"),
                    Band("restless tension", "restless tension", "light compositional unrest", "subtle visual instability"),
                    Band("volatile energy", "volatile energy", "clear compositional volatility", "noticeable unstable energy"),
                    Band("orchestrated chaos", "orchestrated chaos", "controlled visual turbulence", "strong compositional instability"),
                    Band("high visual instability", "high visual instability", "near-chaotic image energy", "maximal compositional turbulence"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] = [Band("controlled composition", "controlled frame tension", "stable cinematic order", "disciplined scene balance"), Band("restless tension", "restless cinematic tension", "light screen instability", "subtle directorial unrest"), Band("volatile energy", "volatile scene energy", "clear cinematic instability", "noticeable action-framed unrest"), Band("orchestrated chaos", "orchestrated cinematic chaos", "controlled scene turbulence", "strong screen instability"), Band("high visual instability", "high visual instability", "near-chaotic cinematic energy", "maximal scene turbulence")],
                    ["Painterly"] = [Band("controlled composition", "controlled asymmetry", "stable painterly order", "disciplined tableau balance"), Band("restless tension", "restless painterly tension", "light pictorial unrest", "subtle compositional instability"), Band("volatile energy", "volatile painterly energy", "clear pictorial instability", "noticeable tableau unrest"), Band("orchestrated chaos", "orchestrated pictorial chaos", "controlled painterly turbulence", "strong pictorial instability"), Band("high visual instability", "high visual instability", "near-chaotic pictorial energy", "maximal painterly turbulence")],
                    ["Yarn Relief"] = [Band("controlled composition", "controlled textile asymmetry", "stable fiber-built order", "disciplined textile balance"), Band("restless tension", "restless fiber tension", "light stitched unrest", "subtle textile instability"), Band("volatile energy", "volatile textile energy", "clear fiber-built instability", "noticeable woven unrest"), Band("orchestrated chaos", "orchestrated woven chaos", "controlled textile turbulence", "strong fiber instability"), Band("high visual instability", "high visual instability", "near-chaotic textile energy", "maximal woven turbulence")],
                    ["Stained Glass"] = [Band("controlled composition", "controlled ornamental asymmetry", "stable iconographic order", "disciplined ornamental balance"), Band("restless tension", "restless ornamental tension", "light lead-line unrest", "subtle devotional instability"), Band("volatile energy", "volatile iconographic energy", "clear ornamental instability", "noticeable sacred unrest"), Band("orchestrated chaos", "orchestrated ornamental chaos", "controlled iconographic turbulence", "strong ornamental instability"), Band("high visual instability", "high visual instability", "near-chaotic ornamental energy", "maximal iconographic turbulence")],
                    ["Surreal Symbolic"] = [Band("controlled composition", "controlled dream tension", "stable oneiric order", "disciplined surreal balance"), Band("restless tension", "restless surreal tension", "light dream unrest", "subtle uncanny instability"), Band("volatile energy", "volatile dream energy", "clear surreal instability", "noticeable oneiric unrest"), Band("orchestrated chaos", "orchestrated surreal chaos", "controlled dream turbulence", "strong visionary instability"), Band("high visual instability", "high visual instability", "near-chaotic surreal energy", "maximal dream turbulence")],
                },
                [],
                [],
                ["compositional clause", "mood clause"],
                "Chaos should describe instability, not clutter. Let Background Complexity handle how full the frame is."),
            [Whimsy] = new(
                "Controls how serious, playful, or outright comedic the prompt tone feels.",
                [
                    Band("serious tone", "serious tone", "quiet serious mood", "solemn tone", "unsmiling tone", "severe tone"),
                    Band("subtle whimsy", "subtle whimsy", "light playful touch", "gentle whimsical tone"),
                    Band("playful tone", "playful tone", "clear sense of play", "lively playful tone"),
                    Band("strong whimsical energy", "strong whimsical energy", "bold playful character", "mischievous visual energy"),
                    Band("bold comedic whimsy", "bold comedic whimsy", "overtly playful comedy", "full comic whimsy"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Stained Glass"] = [Band("serious tone", "serious tone", "plain devotional seriousness", "straight-faced sacred tone"), Band("subtle whimsy", "subtle folkloric whimsy", "light storybook playfulness", "gentle ornamental charm"), Band("playful tone", "playful iconographic tone", "clear folkloric play", "noticeably storybook mood"), Band("strong whimsical energy", "strong ornamental whimsy", "bold folkloric charm", "pronounced storybook playfulness"), Band("bold comedic whimsy", "bold storybook whimsy", "overtly playful sacred-pageantry", "full folkloric comic charm")],
                    ["Surreal Symbolic"] = [Band("serious tone", "serious tone", "plain surreal seriousness", "straight-faced oneiric tone"), Band("subtle whimsy", "subtle surreal playfulness", "light dreamlike play", "gentle uncanny whimsy"), Band("playful tone", "playful dream logic", "clear surreal playfulness", "noticeably whimsical strangeness"), Band("strong whimsical energy", "strong whimsical strangeness", "bold absurdist play", "pronounced dreamlike whimsy"), Band("bold comedic whimsy", "bold absurdist whimsy", "overt surreal comedy", "full dreamlike comic energy")],
                },
                [],
                [],
                ["mood clause"],
                "Whimsy affects tone quickly. Keep it clean so the special whimsy+tension interaction can still stand out."),
            [Tension] = new(
                "Controls how much dramatic, interpersonal, or psychological strain the prompt carries.",
                [
                    Band("low tension", "low tension", "quiet dramatic footing", "little dramatic strain"),
                    Band("light dramatic tension", "light dramatic tension", "subtle dramatic strain", "gentle scene tension"),
                    Band("noticeable tension", "noticeable tension", "clear dramatic pressure", "rising scene tension"),
                    Band("strong interpersonal tension", "strong interpersonal tension", "high dramatic strain", "charged emotional tension"),
                    Band("intense dramatic tension", "intense dramatic tension", "high-stakes dramatic pressure", "severe emotional strain"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] = [Band("low tension", "low tension", "calm scene pressure", "little cinematic strain"), Band("light dramatic tension", "light dramatic tension", "subtle cinematic strain", "gentle screen tension"), Band("noticeable tension", "scene-level tension", "clear cinematic pressure", "rising screen tension"), Band("strong interpersonal tension", "strong cinematic tension", "charged scene tension", "high dramatic screen strain"), Band("intense dramatic tension", "high-stakes cinematic tension", "severe scene pressure", "intense screen-level strain")],
                    ["Surreal Symbolic"] = [Band("low tension", "low tension", "calm uncanny footing", "little oneiric strain"), Band("light dramatic tension", "light uncanny tension", "subtle dreamlike strain", "gentle surreal unease"), Band("noticeable tension", "noticeable oneiric tension", "clear uncanny pressure", "rising surreal unease"), Band("strong interpersonal tension", "strong psychological tension", "charged uncanny strain", "high dreamlike pressure"), Band("intense dramatic tension", "intense dreamlike dread", "severe psychological strain", "full uncanny pressure")],
                },
                [],
                [],
                ["mood clause", "scene clause"],
                "Tension should stay readable and direct. Avoid letting it drift into symbolism vocabulary."),
            [Awe] = new(
                "Controls how much wonder, reverence, or grandeur the prompt language evokes.",
                [
                    Band("grounded scale", "grounded scale", "human-scale presence", "contained sense of scale"),
                    Band("slight wonder", "slight wonder", "light sense of wonder", "gentle reverent lift"),
                    Band("atmosphere of wonder", "atmosphere of wonder", "clear sense of wonder", "hushed atmosphere of wonder"),
                    Band("strong sense of awe", "strong sense of awe", "powerful wonder", "pronounced reverent grandeur"),
                    Band("overwhelming grandeur", "overwhelming grandeur", "vast sublime presence", "monumental sense of wonder"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] = [Band("grounded scale", "grounded scale", "human-scale cinematic presence", "contained screen grandeur"), Band("slight wonder", "slight cinematic wonder", "light screen-scale wonder", "gentle spectacle"), Band("atmosphere of wonder", "atmosphere of spectacle", "clear cinematic wonder", "noticeable screen reverence"), Band("strong sense of awe", "strong cinematic awe", "powerful filmic grandeur", "pronounced spectacle"), Band("overwhelming grandeur", "overwhelming big-screen grandeur", "vast cinematic awe", "monumental filmic presence")],
                    ["Painterly"] = [Band("grounded scale", "grounded scale", "human-scale tableau presence", "contained painterly grandeur"), Band("slight wonder", "slight sense of wonder", "light painterly reverence", "gentle sublime lift"), Band("atmosphere of wonder", "atmosphere of reverence", "clear painterly wonder", "noticeable sublime atmosphere"), Band("strong sense of awe", "strong painterly awe", "powerful tableau grandeur", "pronounced reverent scale"), Band("overwhelming grandeur", "overwhelming sublime grandeur", "vast painterly awe", "monumental tableau presence")],
                    ["Stained Glass"] = [Band("grounded scale", "grounded scale", "human-scale sacred presence", "contained devotional grandeur"), Band("slight wonder", "slight devotional wonder", "light sacred uplift", "gentle cathedral wonder"), Band("atmosphere of wonder", "atmosphere of reverence", "clear sacred wonder", "noticeable devotional grandeur"), Band("strong sense of awe", "strong sacred awe", "powerful devotional grandeur", "pronounced cathedral reverence"), Band("overwhelming grandeur", "overwhelming cathedral-scale grandeur", "vast sacred awe", "monumental devotional presence")],
                    ["Surreal Symbolic"] = [Band("grounded scale", "grounded scale", "human-scale oneiric presence", "contained visionary scale"), Band("slight wonder", "slight oneiric wonder", "light visionary wonder", "gentle uncanny reverence"), Band("atmosphere of wonder", "dreamlike atmosphere of wonder", "clear visionary wonder", "noticeable oneiric grandeur"), Band("strong sense of awe", "strong uncanny awe", "powerful visionary grandeur", "pronounced dreamlike wonder"), Band("overwhelming grandeur", "overwhelming visionary grandeur", "vast oneiric awe", "monumental uncanny presence")],
                },
                [],
                [],
                ["mood clause", "scene clause"],
                "Awe is broad and atmospheric. Keep the selected phrase short enough that subject and composition still lead."),
            [Saturation] = new(
                "Controls how muted, balanced, or vivid the color treatment becomes.",
                [
                    Band("muted saturation", "muted saturation", "low color intensity", "subdued color treatment"),
                    Band("restrained color", "restrained color", "controlled color intensity", "lightly muted palette"),
                    Band("balanced saturation", "balanced saturation", "measured color intensity", "even color treatment"),
                    Band("rich color saturation", "rich color saturation", "full color presence", "deepened color intensity"),
                    Band("vivid color saturation", "vivid color saturation", "high chroma color", "strongly vivid palette"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Painterly"] = [Band("muted saturation", "muted pigment saturation", "subdued pigment color", "low-intensity painted palette"), Band("restrained color", "restrained atelier color", "controlled pigment intensity", "lightly muted paint color"), Band("balanced saturation", "balanced pigment saturation", "measured painted color", "even pigment intensity"), Band("rich color saturation", "rich painterly color", "full-bodied pigment color", "deepened painted saturation"), Band("vivid color saturation", "luminous painted saturation", "high-chroma pigment color", "strongly vivid paint palette")],
                    ["Stained Glass"] = [Band("muted saturation", "muted glass color", "subdued chapel glass", "low-intensity jewel color"), Band("restrained color", "restrained jewel tones", "controlled stained-glass color", "lightly muted glass palette"), Band("balanced saturation", "balanced stained-glass color", "measured jewel-tone color", "even luminous color"), Band("rich color saturation", "rich cathedral jewel tones", "full stained-glass color", "deepened jewel-tone saturation"), Band("vivid color saturation", "radiant jewel-tone saturation", "high-chroma cathedral color", "strongly vivid glass palette")],
                    ["Surreal Symbolic"] = [Band("muted saturation", "muted dream color", "subdued visionary palette", "low-intensity surreal color"), Band("restrained color", "restrained surreal color", "controlled uncanny color", "lightly muted dream palette"), Band("balanced saturation", "balanced surreal saturation", "measured visionary color", "even oneiric palette"), Band("rich color saturation", "rich surreal saturation", "full dream-color presence", "deepened uncanny color"), Band("vivid color saturation", "vivid oneiric color saturation", "high-chroma dream palette", "strongly vivid visionary color")],
                    ["Concept Art"] = [Band("muted saturation", "muted production color", "subdued design palette", "low-intensity concept color"), Band("restrained color", "restrained production color", "controlled design color", "lightly muted worldbuilding palette"), Band("balanced saturation", "balanced production color", "measured concept-art color", "even design palette"), Band("rich color saturation", "rich production color", "full design-color presence", "deepened concept palette"), Band("vivid color saturation", "vivid concept-art color", "high-chroma production palette", "strongly vivid design color")],
                },
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Glass"] = [Band("muted saturation", "muted glass color", "subdued translucent color", "low-intensity jewel glass"), Band("restrained color", "restrained jewel tones", "controlled translucent color", "lightly muted glass palette"), Band("balanced saturation", "balanced stained-glass color", "measured jewel-tone saturation", "even luminous color"), Band("rich color saturation", "rich jewel-toned saturation", "full glass color presence", "deepened jewel-tone intensity"), Band("vivid color saturation", "radiant jewel-tone saturation", "high-chroma glass color", "strongly vivid translucent palette")],
                    ["Paint"] = [Band("muted saturation", "muted pigment saturation", "subdued paint color", "low-intensity pigment palette"), Band("restrained color", "restrained pigment intensity", "controlled paint color", "lightly muted painted palette"), Band("balanced saturation", "balanced pigment saturation", "measured paint intensity", "even pigment color"), Band("rich color saturation", "rich pigment saturation", "full paint color presence", "deepened pigment intensity"), Band("vivid color saturation", "luminous paint saturation", "high-chroma pigment color", "strongly vivid painted palette")],
                    ["Metal"] = [Band("muted saturation", "muted metallic color", "subdued alloy tones", "low-intensity industrial color"), Band("restrained color", "restrained alloy tones", "controlled metallic accents", "lightly muted hard-surface color"), Band("balanced saturation", "balanced metallic saturation", "measured alloy color", "even industrial palette"), Band("rich color saturation", "rich metallic color accents", "full alloy color presence", "deepened hard-surface color"), Band("vivid color saturation", "luminous alloy saturation", "high-chroma metallic color", "strongly vivid industrial palette")],
                    ["Ink"] = [Band("muted saturation", "muted ink value range", "subdued ink density", "low-intensity ink coloration"), Band("restrained color", "restrained ink saturation", "controlled ink density", "lightly muted ink palette"), Band("balanced saturation", "balanced ink density", "measured ink saturation", "even line-and-wash color"), Band("rich color saturation", "rich ink saturation", "full-bodied ink density", "deepened ink color"), Band("vivid color saturation", "deeply saturated ink intensity", "high-chroma ink density", "strongly vivid ink palette")],
                },
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Stained Glass|Glass"] = [Band("muted saturation", "muted chapel glass", "subdued chapel jewel tones", "low-intensity sacred glass"), Band("restrained color", "restrained jewel tones", "controlled cathedral glass color", "lightly muted devotional palette"), Band("balanced saturation", "balanced stained-glass color", "measured cathedral jewel tones", "even luminous glass color"), Band("rich color saturation", "rich cathedral jewel tones", "full sacred glass color", "deepened cathedral saturation"), Band("vivid color saturation", "radiant jewel-tone luminosity", "high-chroma cathedral glass", "strongly vivid sacred jewel palette")],
                    ["Painterly|Paint"] = [Band("muted saturation", "muted pigment saturation", "subdued atelier color", "low-intensity gallery palette"), Band("restrained color", "restrained atelier color", "controlled painted color", "lightly muted studio palette"), Band("balanced saturation", "balanced pigment saturation", "measured atelier color", "even gallery palette"), Band("rich color saturation", "rich painterly color", "full-bodied atelier color", "deepened painted saturation"), Band("vivid color saturation", "luminous painted saturation", "high-chroma atelier palette", "strongly vivid gallery color")],
                    ["Surreal Symbolic|Ink"] = [Band("muted saturation", "muted ink value range", "subdued symbolic ink density", "low-intensity visionary ink"), Band("restrained color", "restrained symbolic ink density", "controlled dream-journal ink", "lightly muted symbolic palette"), Band("balanced saturation", "balanced ink saturation", "measured symbolic ink density", "even visionary ink color"), Band("rich color saturation", "rich surreal ink density", "full dream-ink color presence", "deepened symbolic ink saturation"), Band("vivid color saturation", "deeply saturated visionary ink intensity", "high-chroma visionary ink", "strongly vivid dream-ink palette")],
                    ["Surreal Symbolic|Glass"] = [Band("muted saturation", "muted dream-glass color", "subdued visionary jewel tones", "low-intensity uncanny glass"), Band("restrained color", "restrained surreal jewel tones", "controlled dream-glass color", "lightly muted uncanny palette"), Band("balanced saturation", "balanced uncanny translucence", "measured visionary glass color", "even dream-glass palette"), Band("rich color saturation", "rich surreal jewel saturation", "full uncanny glass color", "deepened dream-glass intensity"), Band("vivid color saturation", "vivid oneiric glass saturation", "high-chroma visionary glass", "strongly vivid surreal jewel palette")],
                    ["Concept Art|Metal"] = [Band("muted saturation", "muted alloy color", "subdued industrial color", "low-intensity hard-surface palette"), Band("restrained color", "restrained industrial accents", "controlled alloy color", "lightly muted sci-fi palette"), Band("balanced saturation", "balanced production color", "measured industrial color", "even hard-surface palette"), Band("rich color saturation", "rich hard-surface accents", "full alloy color presence", "deepened industrial saturation"), Band("vivid color saturation", "luminous sci-fi alloy color", "high-chroma hard-surface palette", "strongly vivid industrial color")],
                },
                ["finishing clause", "descriptive phrase"],
                "Saturation should stay tight and readable. Avoid ornate wording that duplicates Lighting or Contrast."),
            [Contrast] = new(
                "Controls how gently separated or sharply defined the image values and edges feel.",
                [
                    Band("low contrast", "low contrast", "soft value separation", "muted tonal spread"),
                    Band("gentle contrast", "gentle contrast", "soft tonal separation", "light edge definition"),
                    Band("balanced contrast", "balanced contrast", "measured tonal separation", "even value structure"),
                    Band("crisp contrast", "crisp contrast", "strong tonal separation", "clear edge definition"),
                    Band("striking contrast", "striking contrast", "high tonal separation", "bold value structure"),
                ],
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Cinematic"] = [Band("low contrast", "low contrast", "soft cinematic separation", "muted screen values"), Band("gentle contrast", "gentle tonal separation", "soft cinematic contrast", "light screen-edge definition"), Band("balanced contrast", "balanced cinematic contrast", "measured filmic separation", "even screen value structure"), Band("crisp contrast", "crisp cinematic contrast", "strong screen separation", "clear filmic edge definition"), Band("striking contrast", "striking theatrical contrast", "high filmic separation", "bold cinematic value structure")],
                    ["Painterly"] = [Band("low contrast", "low contrast", "soft painterly separation", "muted pictorial values"), Band("gentle contrast", "gentle painterly contrast", "soft tonal brush separation", "light pictorial edge definition"), Band("balanced contrast", "balanced pictorial contrast", "measured painterly separation", "even painted value structure"), Band("crisp contrast", "crisp tonal contrast", "strong painterly separation", "clear painted edge definition"), Band("striking contrast", "striking painterly contrast", "high pictorial separation", "bold painted value structure")],
                    ["Stained Glass"] = [Band("low contrast", "low contrast", "soft glass separation", "muted lead-line spread"), Band("gentle contrast", "gentle lead-line contrast", "soft stained-glass separation", "light ornamental definition"), Band("balanced contrast", "balanced glass contrast", "measured leaded-glass separation", "even luminous structure"), Band("crisp contrast", "crisp leaded contrast", "strong lead-line separation", "clear stained-glass definition"), Band("striking contrast", "striking jewel-glass contrast", "high lead-line separation", "bold luminous structure")],
                },
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Glass"] = [Band("low contrast", "low contrast", "soft glass separation", "muted translucent spread"), Band("gentle contrast", "gentle lead-line contrast", "soft glass-value separation", "light translucent definition"), Band("balanced contrast", "balanced glass contrast", "measured translucent separation", "even luminous structure"), Band("crisp contrast", "crisp leaded contrast", "strong glass-value separation", "clear translucent definition"), Band("striking contrast", "striking jewel-glass contrast", "high translucent separation", "bold luminous structure")],
                    ["Paint"] = [Band("low contrast", "low contrast", "soft painterly separation", "muted paint-value spread"), Band("gentle contrast", "gentle painterly contrast", "soft tonal paint separation", "light brush-edge definition"), Band("balanced contrast", "balanced pictorial contrast", "measured paint-value separation", "even painted structure"), Band("crisp contrast", "crisp tonal contrast", "strong painted separation", "clear paint-edge definition"), Band("striking contrast", "striking painterly contrast", "high pictorial separation", "bold paint-value structure")],
                    ["Metal"] = [Band("low contrast", "low contrast", "soft reflective separation", "muted metallic value spread"), Band("gentle contrast", "gentle reflective separation", "soft alloy-value contrast", "light hard-surface definition"), Band("balanced contrast", "balanced metallic contrast", "measured reflective separation", "even industrial value structure"), Band("crisp contrast", "crisp reflective contrast", "strong metallic separation", "clear hard-surface definition"), Band("striking contrast", "striking polished-metal contrast", "high reflective separation", "bold industrial value structure")],
                    ["Stone"] = [Band("low contrast", "low contrast", "soft carved separation", "muted mineral value spread"), Band("gentle contrast", "gentle carved contrast", "soft stone-value separation", "light chiseled definition"), Band("balanced contrast", "balanced mineral contrast", "measured stone separation", "even carved value structure"), Band("crisp contrast", "crisp carved contrast", "strong stone separation", "clear chiseled definition"), Band("striking contrast", "striking chiseled stone contrast", "high carved separation", "bold mineral value structure")],
                    ["Ink"] = [Band("low contrast", "low contrast", "soft ink separation", "muted line-and-wash spread"), Band("gentle contrast", "gentle ink separation", "soft value contrast in ink", "light line definition"), Band("balanced contrast", "balanced ink contrast", "measured ink separation", "even line-and-wash structure"), Band("crisp contrast", "crisp ink contrast", "strong ink separation", "clear line definition"), Band("striking contrast", "striking black-ink contrast", "high ink separation", "bold line-and-value structure")],
                },
                new Dictionary<string, SliderBandDefinition[]>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Stained Glass|Glass"] = [Band("low contrast", "low contrast", "soft devotional glass separation", "muted chapel lead-line spread"), Band("gentle contrast", "gentle lead-line contrast", "soft sacred glass separation", "light cathedral definition"), Band("balanced contrast", "balanced devotional glass contrast", "measured cathedral separation", "even sacred luminous structure"), Band("crisp contrast", "crisp cathedral leaded contrast", "strong devotional separation", "clear sacred glass definition"), Band("striking contrast", "striking jewel-glass contrast", "high cathedral separation", "bold sacred luminous structure")],
                    ["Painterly|Paint"] = [Band("low contrast", "low contrast", "soft gallery-value separation", "muted atelier tonal spread"), Band("gentle contrast", "gentle painterly separation", "soft atelier contrast", "light gallery edge definition"), Band("balanced contrast", "balanced atelier contrast", "measured gallery separation", "even painted tonal structure"), Band("crisp contrast", "crisp tonal contrast", "strong atelier separation", "clear gallery edge definition"), Band("striking contrast", "striking gallery-grade contrast", "high atelier separation", "bold painted tonal structure")],
                    ["Surreal Symbolic|Ink"] = [Band("low contrast", "low contrast", "soft visionary ink separation", "muted dream-journal spread"), Band("gentle contrast", "gentle ink separation", "soft symbolic ink contrast", "light dream-line definition"), Band("balanced contrast", "balanced symbolic ink contrast", "measured visionary ink separation", "even surreal ink structure"), Band("crisp contrast", "crisp surreal ink contrast", "strong dream-ink separation", "clear symbolic line definition"), Band("striking contrast", "striking dream-journal black-ink contrast", "high visionary ink separation", "bold surreal ink structure")],
                    ["Surreal Symbolic|Glass"] = [Band("low contrast", "low contrast", "soft uncanny glass separation", "muted visionary translucence"), Band("gentle contrast", "gentle translucent separation", "soft surreal glass contrast", "light uncanny edge definition"), Band("balanced contrast", "balanced uncanny glass contrast", "measured visionary translucence", "even surreal luminous structure"), Band("crisp contrast", "crisp surreal glass contrast", "strong uncanny separation", "clear visionary glass definition"), Band("striking contrast", "striking visionary jewel-glass contrast", "high surreal translucence", "bold uncanny luminous structure")],
                    ["Concept Art|Metal"] = [Band("low contrast", "low contrast", "soft industrial separation", "muted hard-surface spread"), Band("gentle contrast", "gentle hard-surface separation", "soft alloy contrast", "light production edge definition"), Band("balanced contrast", "balanced industrial contrast", "measured hard-surface separation", "even production value structure"), Band("crisp contrast", "crisp reflective hard-surface contrast", "strong industrial separation", "clear production edge definition"), Band("striking contrast", "striking production-grade metal contrast", "high hard-surface separation", "bold industrial value structure")],
                },
                ["finishing clause", "descriptive phrase"],
                "Contrast should remain concise and technical-feeling. It benefits from cleaner language than Symbolism or Narrative Density."),
        };

    public static string ResolvePhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        if (!Definitions.TryGetValue(sliderKey, out var definition))
        {
            return string.Empty;
        }

        var bandIndex = GetBandIndex(value);
        var candidates = new List<string>();

        AddCandidates(candidates, GetVariant(definition.StyleMaterialVariants, $"{configuration.ArtStyle}|{configuration.Material}"), bandIndex);
        AddCandidates(candidates, GetVariant(definition.MaterialVariants, configuration.Material), bandIndex);
        AddCandidates(candidates, GetVariant(definition.StyleVariants, configuration.ArtStyle), bandIndex);
        AddCandidates(candidates, definition.Bands, bandIndex);

        var deduped = candidates
            .Where(static phrase => !string.IsNullOrWhiteSpace(phrase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (deduped.Length == 0)
        {
            return string.Empty;
        }

        var seed = string.Join("|",
            sliderKey,
            bandIndex,
            configuration.Subject,
            configuration.Action,
            configuration.Relationship,
            configuration.ArtStyle,
            configuration.Material,
            configuration.Lighting,
            configuration.CameraDistance,
            configuration.CameraAngle,
            configuration.ArtistInfluencePrimary,
            configuration.ArtistInfluenceSecondary);

        var resolvedPool = ApplyBundlePreference(configuration.IntentMode, sliderKey, deduped);
        return resolvedPool[Math.Abs(GetStableHash(seed)) % resolvedPool.Length];
    }

    public static string ResolveArtistInfluenceDescriptor(int strength, string artistName)
    {
        if (string.IsNullOrWhiteSpace(artistName) || !Definitions.TryGetValue(ArtistInfluenceStrength, out var definition))
        {
            return string.Empty;
        }

        var phrases = definition.Bands[GetBandIndex(strength)].Phrases;
        if (phrases.Length == 0)
        {
            return string.Empty;
        }

        return phrases[Math.Abs(GetStableHash($"{artistName}|{strength}")) % phrases.Length]
            .Replace("{artist}", artistName, StringComparison.Ordinal);
    }

    private static SliderBandDefinition[] GetVariant(Dictionary<string, SliderBandDefinition[]> variants, string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return [];
        }

        return variants.TryGetValue(key, out var bands) ? bands : [];
    }

    private static void AddCandidates(ICollection<string> phrases, SliderBandDefinition[] bands, int bandIndex)
    {
        if (bands.Length != 5)
        {
            return;
        }

        foreach (var phrase in bands[bandIndex].Phrases)
        {
            if (!string.IsNullOrWhiteSpace(phrase))
            {
                phrases.Add(phrase);
            }
        }
    }

    private static string[] ApplyBundlePreference(string? intentMode, string sliderKey, string[] candidates)
    {
        if (candidates.Length == 0
            || string.IsNullOrWhiteSpace(intentMode)
            || !IntentModeCatalog.TryGet(intentMode, out _)
            || !BundlePhrasePreferences.TryGetValue(intentMode, out var preference))
        {
            return candidates;
        }

        var preferred = TryFilter(candidates, preference.PreferredBySlider, sliderKey);
        if (preferred.Length > 0)
        {
            return preferred;
        }

        var avoided = GetPhraseSet(preference.AvoidedBySlider, sliderKey);
        if (avoided.Count == 0)
        {
            return candidates;
        }

        var filtered = candidates
            .Where(candidate => !avoided.Contains(candidate))
            .ToArray();

        return filtered.Length > 0 ? filtered : candidates;
    }

    private static string[] TryFilter(string[] candidates, IReadOnlyDictionary<string, string[]> phraseLookup, string sliderKey)
    {
        var allowed = GetPhraseSet(phraseLookup, sliderKey);
        if (allowed.Count == 0)
        {
            return [];
        }

        return candidates
            .Where(candidate => allowed.Contains(candidate))
            .ToArray();
    }

    private static HashSet<string> GetPhraseSet(IReadOnlyDictionary<string, string[]> phraseLookup, string sliderKey)
    {
        if (!phraseLookup.TryGetValue(sliderKey, out var phrases) || phrases.Length == 0)
        {
            return [];
        }

        return new HashSet<string>(
            phrases.Where(static phrase => !string.IsNullOrWhiteSpace(phrase)),
            StringComparer.OrdinalIgnoreCase);
    }

    private static int GetBandIndex(int value)
    {
        if (value <= 20) return 0;
        if (value <= 40) return 1;
        if (value <= 60) return 2;
        if (value <= 80) return 3;
        return 4;
    }

    private static int GetStableHash(string value)
    {
        unchecked
        {
            var hash = 23;
            foreach (var character in value)
            {
                hash = (hash * 31) + character;
            }

            return hash;
        }
    }

    private static SliderBandDefinition Band(string interpretation, params string[] phrases) => new(interpretation, phrases);

    private sealed record SliderLanguageDefinition(
        string CoreMeaning,
        SliderBandDefinition[] Bands,
        Dictionary<string, SliderBandDefinition[]> StyleVariants,
        Dictionary<string, SliderBandDefinition[]> MaterialVariants,
        Dictionary<string, SliderBandDefinition[]> StyleMaterialVariants,
        string[] PromptPlacementNotes,
        string RepetitionControlNotes);

    private sealed record SliderBandDefinition(string Interpretation, string[] Phrases);

    private sealed record BundlePhrasePreference(
        IReadOnlyDictionary<string, string[]> PreferredBySlider,
        IReadOnlyDictionary<string, string[]> AvoidedBySlider);
}
