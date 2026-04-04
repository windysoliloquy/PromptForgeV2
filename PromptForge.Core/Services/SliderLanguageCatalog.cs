using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static class SliderLanguageCatalog
{
    public const string ArtistInfluenceStrength = "ArtistInfluenceStrength";
    public const string Temperature = "Temperature";
    public const string LightingIntensity = "LightingIntensity";
    public const string Stylization = "Stylization";
    public const string Realism = "Realism";
    public const string TextureDepth = "TextureDepth";
    public const string NarrativeDensity = "NarrativeDensity";
    public const string Symbolism = "Symbolism";
    public const string SurfaceAge = "SurfaceAge";
    public const string Framing = "Framing";
    public const string BackgroundComplexity = "BackgroundComplexity";
    public const string MotionEnergy = "MotionEnergy";
    public const string FocusDepth = "FocusDepth";
    public const string ImageCleanliness = "ImageCleanliness";
    public const string DetailDensity = "DetailDensity";
    public const string AtmosphericDepth = "AtmosphericDepth";
    public const string Chaos = "Chaos";
    public const string Whimsy = "Whimsy";
    public const string Tension = "Tension";
    public const string Awe = "Awe";
    public const string Saturation = "Saturation";
    public const string Contrast = "Contrast";
    public const string CameraDistance = "CameraDistance";
    public const string CameraAngle = "CameraAngle";

    private static readonly IReadOnlyDictionary<string, BundlePhrasePreference> BundlePhrasePreferences =
        new Dictionary<string, BundlePhrasePreference>(StringComparer.OrdinalIgnoreCase);

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
            [Temperature] = new(
                "Controls overall color temperature / perceived warmth versus coolness.",
                [
                    Band("Cool", "cool color temperature"),
                    Band("Mild cool", "slightly cool balance"),
                    Band("Neutral", "neutral temperature balance"),
                    Band("Warm", "warm color temperature"),
                    Band("Hot", "heated warm cast"),
                ],
                [],
                [],
                [],
                ["descriptive phrase"],
                "Use once in palette or lighting clause."),
            [LightingIntensity] = new(
                "Controls how forceful or subdued the scene lighting feels.",
                [
                    Band("Dim", "dim lighting"),
                    Band("Soft", "soft lighting"),
                    Band("Balanced", "balanced lighting"),
                    Band("Bright", "bright scene lighting"),
                    Band("Radiant", "radiant luminous lighting"),
                ],
                [],
                [],
                [],
                ["descriptive phrase"],
                "Keep single mention to avoid redundancy with named lighting."),
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
            [Framing] = new(
                "Controls how tight or expansive the composition framing feels.",
                [
                    Band("intimate framing", "intimate framing"),
                    Band("tight framing", "tight framing"),
                    Band("balanced framing", "balanced framing"),
                    Band("open framing", "open framing"),
                    Band("expansive framing", "expansive framing"),
                ],
                [],
                [],
                [],
                ["compositional clause"],
                "Use once; complements camera distance."),
            [CameraDistance] = new(
                "Controls subject distance from the viewer.",
                [
                    Band("extreme close view", "extreme close view"),
                    Band("close view", "close view"),
                    Band("mid-distance view", "mid-distance view"),
                    Band("wider distant view", "wider distant view"),
                    Band("far-set distant view", "far-set distant view"),
                ],
                [],
                [],
                [],
                ["compositional clause"],
                "Pair with framing; avoid repeating distance elsewhere."),
            [CameraAngle] = new(
                "Controls camera vantage / elevation bias.",
                [
                    Band("low angle view", "low angle view"),
                    Band("slightly low angle", "slightly low angle"),
                    Band("eye-level view", "eye-level view"),
                    Band("slightly high angle", "slightly high angle"),
                    Band("high angle view", "high angle view"),
                ],
                [],
                [],
                [],
                ["compositional clause"],
                "Single mention; avoid stacking with subject wording."),
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
            [FocusDepth] = new(
                "Controls whether the image feels broadly resolved or selectively focused.",
                [
                    Band("deep focus clarity", "deep focus clarity"),
                    Band("mostly deep focus", "mostly deep focus"),
                    Band("balanced focus depth", "balanced focus depth"),
                    Band("selective focus falloff", "selective focus falloff"),
                    Band("very shallow depth of field", "very shallow depth of field"),
                ],
                [],
                [],
                [],
                ["focus clause", "surface clarity phrase"],
                "Mention once; pairs with composition phrases."),
            [ImageCleanliness] = new(
                "Controls how clean/polished vs gritty/imperfect the image feels.",
                [
                    Band("raw visual finish", "raw visual finish"),
                    Band("slight visual grit", "slight visual grit"),
                    Band("balanced finish", "balanced finish"),
                    Band("clean visual finish", "clean visual finish"),
                    Band("polished visual finish", "polished visual finish"),
                ],
                [],
                [],
                [],
                ["surface treatment clause"],
                "Avoid duplicating with texture depth phrasing."),
            [DetailDensity] = new(
                "Controls how much fine visual information is packed into the image overall.",
                [
                    Band("sparse detail treatment", "sparse detail treatment"),
                    Band("light detail presence", "light detail presence"),
                    Band("moderate detail density", "moderate detail density"),
                    Band("rich fine detail", "rich fine detail"),
                    Band("dense detail layering", "dense detail layering"),
                ],
                [],
                [],
                [],
                ["surface treatment clause"],
                "Keep single mention; avoid overlap with background complexity."),
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

        if (IntentModeCatalog.IsVintageBend(configuration.IntentMode))
        {
            return ResolveVintageBendPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsWatercolor(configuration.IntentMode))
        {
            return ResolveWatercolorPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsChildrensBook(configuration.IntentMode))
        {
            return ResolveChildrensBookPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsComicBook(configuration.IntentMode))
        {
            return ResolveComicBookPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsThreeDRender(configuration.IntentMode))
        {
            return ResolveThreeDRenderPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsConceptArt(configuration.IntentMode))
        {
            return ResolveConceptArtPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsPixelArt(configuration.IntentMode))
        {
            return ResolvePixelArtPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsProductPhotography(configuration.IntentMode))
        {
            return ResolveProductPhotographyPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsFoodPhotography(configuration.IntentMode))
        {
            return ResolveFoodPhotographyPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsArchitectureArchviz(configuration.IntentMode))
        {
            return ResolveArchitectureArchvizPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsPhotography(configuration.IntentMode))
        {
            return ResolvePhotographyPhrase(sliderKey, value, configuration);
        }

        if (IntentModeCatalog.IsAnime(configuration.IntentMode))
        {
            return ResolveAnimePhrase(sliderKey, value, configuration);
        }

        return ResolveStandardPhrase(sliderKey, value, configuration);
    }

    public static string ResolveAnimePhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded anime treatment",
                "light anime stylization",
                "clear anime rendering",
                "strong anime stylization",
                "highly stylized anime visual language"),
            Realism => MapBand(value,
                "openly stylized anime rendering",
                "lightly grounded anime form logic",
                "convincingly structured anime rendering",
                "high-fidelity anime illustration realism",
                "premium anime production realism"),
            TextureDepth => MapBand(value,
                "flat anime surface finish",
                "light anime surface texture",
                "refined anime surface detail",
                "rich anime surface texture",
                "highly polished anime surface finish"),
            NarrativeDensity => MapBand(value,
                "single-read anime image",
                "light character-story cues",
                "layered anime storytelling cues",
                "dense implied character narrative",
                "world-rich anime narrative density"),
            Symbolism => MapBand(value,
                "literal anime framing",
                "subtle anime symbolism",
                "suggestive anime motifs",
                "pronounced anime symbolism",
                "mythic anime symbolic charge"),
            SurfaceAge => MapBand(value,
                "fresh anime finish",
                "slight production wear",
                "gentle cel-era wear",
                "noticeable production patina",
                "time-softened anime production finish"),
            Framing => MapBand(value,
                "focused anime framing",
                "light anime scene spacing",
                "balanced anime framing",
                "broad anime key framing",
                "expansive anime visual staging"),
            BackgroundComplexity => MapBand(value,
                "minimal anime backdrop",
                "restrained anime setting detail",
                "readable anime environment",
                "rich anime scene staging",
                "densely layered anime world detail"),
            MotionEnergy => MapBand(value,
                "still anime composition",
                "gentle anime motion",
                "active anime scene energy",
                "dynamic anime action",
                "high-kinetic anime impact"),
            FocusDepth => MapBand(value,
                "broad anime focus",
                "light focus falloff",
                "balanced anime focus depth",
                "selective anime focus",
                "sharp subject-isolating anime focus"),
            ImageCleanliness => MapBand(value,
                "raw anime finish",
                "light production grit",
                "balanced anime cleanliness",
                "clean anime finish",
                "polished anime finish"),
            DetailDensity => MapBand(value,
                "sparse anime detail",
                "light anime detail",
                "balanced anime detail",
                "rich anime detail",
                "dense anime detail"),
            AtmosphericDepth => MapBand(value,
                "flat anime space",
                "slight anime atmospheric recession",
                "air-filled anime depth",
                "luminous anime atmosphere",
                "deeply layered anime spatial atmosphere"),
            Chaos => MapBand(value,
                "controlled anime composition",
                "restless anime tension",
                "active anime scene energy",
                "dynamic anime disorder",
                "orchestrated anime chaos"),
            Whimsy => MapBand(value,
                "serious anime tone",
                "light playful anime charm",
                "expressive anime playfulness",
                "strong whimsical anime energy",
                "bold comedic anime exaggeration"),
            Tension => MapBand(value,
                "low anime tension",
                "light dramatic anime tension",
                "noticeable character tension",
                "strong anime confrontation",
                "intense high-stakes anime tension"),
            Awe => MapBand(value,
                "grounded anime scale",
                "slight sense of anime wonder",
                "atmosphere of anime wonder",
                "strong cinematic anime awe",
                "overwhelming anime spectacle"),
            Temperature => MapBand(value,
                "cool anime palette",
                "slightly cool anime balance",
                "balanced anime color temperature",
                "warm anime palette",
                "heated anime color cast"),
            LightingIntensity => MapBand(value,
                "soft anime lighting",
                "gentle anime lighting",
                "balanced anime lighting",
                "bright anime lighting",
                "radiant anime lighting"),
            Saturation => MapBand(value,
                "muted anime palette",
                "restrained anime color",
                "balanced anime saturation",
                "vivid anime color energy",
                "radiant anime palette"),
            Contrast => MapBand(value,
                "soft anime contrast",
                "gentle anime tonal separation",
                "balanced anime contrast",
                "crisp anime contrast",
                "striking anime value separation"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyAnimeGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveAnimeGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded anime treatment", "light anime stylization", "clear anime rendering", "strong anime stylization", "highly stylized anime visual language" },
            Realism => new[] { "openly stylized anime rendering", "lightly grounded anime form logic", "convincingly structured anime rendering", "high-fidelity anime illustration realism", "premium anime production realism" },
            TextureDepth => new[] { "flat anime surface finish", "light anime surface texture", "refined anime surface detail", "rich anime surface texture", "highly polished anime surface finish" },
            NarrativeDensity => new[] { "single-read anime image", "light character-story cues", "layered anime storytelling cues", "dense implied character narrative", "world-rich anime narrative density" },
            Symbolism => new[] { "literal anime framing", "subtle anime symbolism", "suggestive anime motifs", "pronounced anime symbolism", "mythic anime symbolic charge" },
            SurfaceAge => new[] { "fresh anime finish", "slight production wear", "gentle cel-era wear", "noticeable production patina", "time-softened anime production finish" },
            Framing => new[] { "focused anime framing", "light anime scene spacing", "balanced anime framing", "broad anime key framing", "expansive anime visual staging" },
            BackgroundComplexity => new[] { "minimal anime backdrop", "restrained anime setting detail", "readable anime environment", "rich anime scene staging", "densely layered anime world detail" },
            MotionEnergy => new[] { "still anime composition", "gentle anime motion", "active anime scene energy", "dynamic anime action", "high-kinetic anime impact" },
            FocusDepth => new[] { "broad anime focus", "light focus falloff", "balanced anime focus depth", "selective anime focus", "sharp subject-isolating anime focus" },
            ImageCleanliness => new[] { "raw anime finish", "light production grit", "balanced anime cleanliness", "clean anime finish", "polished anime finish" },
            DetailDensity => new[] { "sparse anime detail", "light anime detail", "balanced anime detail", "rich anime detail", "dense anime detail" },
            AtmosphericDepth => new[] { "flat anime space", "slight anime atmospheric recession", "air-filled anime depth", "luminous anime atmosphere", "deeply layered anime spatial atmosphere" },
            Chaos => new[] { "controlled anime composition", "restless anime tension", "active anime scene energy", "dynamic anime disorder", "orchestrated anime chaos" },
            Whimsy => new[] { "serious anime tone", "light playful anime charm", "expressive anime playfulness", "strong whimsical anime energy", "bold comedic anime exaggeration" },
            Tension => new[] { "low anime tension", "light dramatic anime tension", "noticeable character tension", "strong anime confrontation", "intense high-stakes anime tension" },
            Awe => new[] { "grounded anime scale", "slight sense of anime wonder", "atmosphere of anime wonder", "strong cinematic anime awe", "overwhelming anime spectacle" },
            Temperature => new[] { "cool anime palette", "slightly cool anime balance", "balanced anime color temperature", "warm anime palette", "heated anime color cast" },
            LightingIntensity => new[] { "soft anime lighting", "gentle anime lighting", "balanced anime lighting", "bright anime lighting", "radiant anime lighting" },
            Saturation => new[] { "muted anime palette", "restrained anime color", "balanced anime saturation", "vivid anime color energy", "radiant anime palette" },
            Contrast => new[] { "soft anime contrast", "gentle anime tonal separation", "balanced anime contrast", "crisp anime contrast", "striking anime value separation" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static string ResolveWatercolorPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded treatment",
                "light stylization",
                "clear rendering",
                "expressive stylization",
                "highly stylized language"),
            Realism => MapBand(value,
                "loosely observed rendering",
                "lightly grounded form",
                "convincingly observed watercolor realism",
                "high-fidelity representational realism",
                "exhibition-grade realism"),
            TextureDepth => MapBand(value,
                "minimal paper texture",
                "light surface texture",
                "clear paper-and-pigment texture",
                "rich surface character",
                "deeply worked texture"),
            NarrativeDensity => MapBand(value,
                "single-read image",
                "light narrative suggestion",
                "layered storytelling cues",
                "dense implied narrative",
                "world-rich storytelling"),
            Symbolism => MapBand(value,
                "literal framing",
                "subtle symbolism",
                "suggestive motifs",
                "pronounced symbolism",
                "mythic symbolic charge"),
            SurfaceAge => MapBand(value,
                "fresh paper",
                "slight working wear",
                "gentle paper age",
                "noticeable patina",
                "time-softened paper character"),
            Framing => MapBand(value,
                "focused framing",
                "light scene spacing",
                "balanced framing",
                "broad staging",
                "expansive composition"),
            BackgroundComplexity => MapBand(value,
                "minimal wash background",
                "restrained backdrop",
                "supporting painted environment",
                "rich environment",
                "densely layered setting"),
            MotionEnergy => MapBand(value,
                "still composition",
                "gentle movement",
                "active scene energy",
                "fluid motion",
                "dynamic impact"),
            FocusDepth => MapBand(value,
                "broad focus",
                "light focus falloff",
                "balanced focus depth",
                "selective focus",
                "sharp subject-isolating focus"),
            ImageCleanliness => MapBand(value,
                "raw finish",
                "light working looseness",
                "balanced cleanliness",
                "clean finish",
                "polished finish"),
            DetailDensity => MapBand(value,
                "sparse detail",
                "light detail",
                "balanced detail",
                "rich detail",
                "dense detail"),
            AtmosphericDepth => MapBand(value,
                "limited wash depth",
                "slight recession",
                "airy wash depth",
                "luminous wash layering",
                "deep atmospheric perspective"),
            Chaos => MapBand(value,
                "controlled composition",
                "restless tension",
                "active energy",
                "dynamic looseness",
                "orchestrated chaos"),
            Whimsy => MapBand(value,
                "serious tone",
                "subtle whimsy",
                "playful charm",
                "strong whimsical energy",
                "bold storybook whimsy"),
            Tension => MapBand(value,
                "low tension",
                "light dramatic tension",
                "noticeable tension",
                "strong dramatic tension",
                "intense drama"),
            Awe => MapBand(value,
                "grounded scale",
                "slight wonder",
                "atmosphere of reverence",
                "strong awe",
                "overwhelming grandeur"),
            Temperature => MapBand(value,
                "cool balance",
                "slightly cool wash",
                "neutral temperature",
                "warm glow",
                "heated warmth"),
            LightingIntensity => MapBand(value,
                "soft light",
                "gentle illumination",
                "balanced lighting",
                "bright illumination",
                "radiant light"),
            Saturation => MapBand(value,
                "muted pigment",
                "restrained color",
                "balanced pigment saturation",
                "rich pigment",
                "vivid luminous saturation"),
            Contrast => MapBand(value,
                "low contrast",
                "gentle tonal separation",
                "balanced tonal contrast",
                "crisp contrast",
                "striking tonal contrast"),
            _ => string.Empty,
        };

        return ApplyWatercolorGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveWatercolorGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded treatment", "light stylization", "clear rendering", "expressive stylization", "highly stylized language" },
            Realism => new[] { "loosely observed rendering", "lightly grounded form", "convincingly observed watercolor realism", "high-fidelity representational realism", "exhibition-grade realism" },
            TextureDepth => new[] { "minimal paper texture", "light surface texture", "clear paper-and-pigment texture", "rich surface character", "deeply worked texture" },
            NarrativeDensity => new[] { "single-read image", "light narrative suggestion", "layered storytelling cues", "dense implied narrative", "world-rich storytelling" },
            Symbolism => new[] { "literal framing", "subtle symbolism", "suggestive motifs", "pronounced symbolism", "mythic symbolic charge" },
            SurfaceAge => new[] { "fresh paper", "slight working wear", "gentle paper age", "noticeable patina", "time-softened paper character" },
            Framing => new[] { "focused framing", "light scene spacing", "balanced framing", "broad staging", "expansive composition" },
            BackgroundComplexity => new[] { "minimal wash background", "restrained backdrop", "supporting painted environment", "rich environment", "densely layered setting" },
            MotionEnergy => new[] { "still composition", "gentle movement", "active scene energy", "fluid motion", "dynamic impact" },
            FocusDepth => new[] { "broad focus", "light focus falloff", "balanced focus depth", "selective focus", "sharp subject-isolating focus" },
            ImageCleanliness => new[] { "raw finish", "light working looseness", "balanced cleanliness", "clean finish", "polished finish" },
            DetailDensity => new[] { "sparse detail", "light detail", "balanced detail", "rich detail", "dense detail" },
            AtmosphericDepth => new[] { "limited wash depth", "slight recession", "airy wash depth", "luminous wash layering", "deep atmospheric perspective" },
            Chaos => new[] { "controlled composition", "restless tension", "active energy", "dynamic looseness", "orchestrated chaos" },
            Whimsy => new[] { "serious tone", "subtle whimsy", "playful charm", "strong whimsical energy", "bold storybook whimsy" },
            Tension => new[] { "low tension", "light dramatic tension", "noticeable tension", "strong dramatic tension", "intense drama" },
            Awe => new[] { "grounded scale", "slight wonder", "atmosphere of reverence", "strong awe", "overwhelming grandeur" },
            Temperature => new[] { "cool balance", "slightly cool wash", "neutral temperature", "warm glow", "heated warmth" },
            LightingIntensity => new[] { "soft light", "gentle illumination", "balanced lighting", "bright illumination", "radiant light" },
            Saturation => new[] { "muted pigment", "restrained color", "balanced pigment saturation", "rich pigment", "vivid luminous saturation" },
            Contrast => new[] { "low contrast", "gentle tonal separation", "balanced tonal contrast", "crisp contrast", "striking tonal contrast" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static string ResolveChildrensBookPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded treatment",
                "light illustrative stylization",
                "clear illustration",
                "expressive illustration",
                "highly stylized image treatment"),
            Realism => MapBand(value,
                "loosely observed rendering",
                "lightly grounded form",
                "convincingly observed realism",
                "high-fidelity representational rendering",
                "polished representational realism"),
            TextureDepth => MapBand(value,
                "minimal paper texture",
                "light paper grain",
                "clear paper-and-pigment texture",
                "rich surface character",
                "deeply worked texture"),
            NarrativeDensity => MapBand(value,
                "single-read scene",
                "light narrative cue",
                "layered storytelling cues",
                "story-rich scene progression",
                "adventure-ready story momentum"),
            Symbolism => MapBand(value,
                "literal framing",
                "subtle symbolic cue",
                "suggestive motif",
                "fairy-tale symbolic resonance",
                "mythic storybook symbolism"),
            SurfaceAge => MapBand(value,
                "fresh paper",
                "slight working wear",
                "gentle paper age",
                "noticeable patina",
                "time-softened paper character"),
            Framing => MapBand(value,
                "focused framing",
                "light scene spacing",
                "balanced framing",
                "broad staging",
                "expansive composition"),
            BackgroundComplexity => MapBand(value,
                "minimal backdrop",
                "restrained backdrop",
                "supporting scene detail",
                "rich story-world setting",
                "densely layered storybook environment"),
            MotionEnergy => MapBand(value,
                "still composition",
                "gentle movement",
                "active scene energy",
                "adventurous movement",
                "high-spirited storybook motion"),
            FocusDepth => MapBand(value,
                "broad focus",
                "light focus falloff",
                "balanced focus depth",
                "selective focus",
                "sharp subject-isolating focus"),
            ImageCleanliness => MapBand(value,
                "raw finish",
                "light working looseness",
                "balanced cleanliness",
                "clean finish",
                "polished finish"),
            DetailDensity => MapBand(value,
                "sparse detail",
                "light detail",
                "balanced detail",
                "rich detail",
                "dense detail"),
            AtmosphericDepth => MapBand(value,
                "limited depth",
                "slight recession",
                "airy depth",
                "enchanted depth layering",
                "deep story-world atmosphere"),
            Chaos => MapBand(value,
                "controlled composition",
                "restless energy",
                "active energy",
                "rollicking storybook commotion",
                "orchestrated adventure-chaos"),
            Whimsy => MapBand(value,
                "serious tone",
                "subtle charm",
                "playful charm",
                "strong whimsical energy",
                "bold fanciful energy"),
            Tension => MapBand(value,
                "low tension",
                "light dramatic tension",
                "noticeable tension",
                "storybook suspense",
                "high-stakes child-safe peril"),
            Awe => MapBand(value,
                "grounded scale",
                "slight wonder",
                "atmosphere of wonder",
                "enchanted sense of wonder",
                "big-hearted storybook grandeur"),
            Temperature => MapBand(value,
                "cool balance",
                "slightly cool wash",
                "neutral temperature",
                "warm glow",
                "heated warmth"),
            LightingIntensity => MapBand(value,
                "soft light",
                "gentle illumination",
                "balanced lighting",
                "bright illumination",
                "radiant light"),
            Saturation => MapBand(value,
                "muted color",
                "restrained color",
                "balanced color",
                "rich color",
                "vivid color"),
            Contrast => MapBand(value,
                "low contrast",
                "gentle tonal separation",
                "balanced tonal contrast",
                "crisp contrast",
                "striking tonal contrast"),
            _ => string.Empty,
        };

        return ApplyChildrensBookGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveChildrensBookGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded treatment", "light illustrative stylization", "clear illustration", "expressive illustration", "highly stylized image treatment" },
            Realism => new[] { "loosely observed rendering", "lightly grounded form", "convincingly observed realism", "high-fidelity representational rendering", "polished representational realism" },
            TextureDepth => new[] { "minimal paper texture", "light paper grain", "clear paper-and-pigment texture", "rich surface character", "deeply worked texture" },
            NarrativeDensity => new[] { "single-read scene", "light narrative cue", "layered storytelling cues", "story-rich scene progression", "adventure-ready story momentum" },
            Symbolism => new[] { "literal framing", "subtle symbolic cue", "suggestive motif", "fairy-tale symbolic resonance", "mythic storybook symbolism" },
            SurfaceAge => new[] { "fresh paper", "slight working wear", "gentle paper age", "noticeable patina", "time-softened paper character" },
            Framing => new[] { "focused framing", "light scene spacing", "balanced framing", "broad staging", "expansive composition" },
            BackgroundComplexity => new[] { "minimal backdrop", "restrained backdrop", "supporting scene detail", "rich story-world setting", "densely layered storybook environment" },
            MotionEnergy => new[] { "still composition", "gentle movement", "active scene energy", "adventurous movement", "high-spirited storybook motion" },
            FocusDepth => new[] { "broad focus", "light focus falloff", "balanced focus depth", "selective focus", "sharp subject-isolating focus" },
            ImageCleanliness => new[] { "raw finish", "light working looseness", "balanced cleanliness", "clean finish", "polished finish" },
            DetailDensity => new[] { "sparse detail", "light detail", "balanced detail", "rich detail", "dense detail" },
            AtmosphericDepth => new[] { "limited depth", "slight recession", "airy depth", "enchanted depth layering", "deep story-world atmosphere" },
            Chaos => new[] { "controlled composition", "restless energy", "active energy", "rollicking storybook commotion", "orchestrated adventure-chaos" },
            Whimsy => new[] { "serious tone", "subtle charm", "playful charm", "strong whimsical energy", "bold fanciful energy" },
            Tension => new[] { "low tension", "light dramatic tension", "noticeable tension", "storybook suspense", "high-stakes child-safe peril" },
            Awe => new[] { "grounded scale", "slight wonder", "atmosphere of wonder", "enchanted sense of wonder", "big-hearted storybook grandeur" },
            Temperature => new[] { "cool balance", "slightly cool wash", "neutral temperature", "warm glow", "heated warmth" },
            LightingIntensity => new[] { "soft light", "gentle illumination", "balanced lighting", "bright illumination", "radiant light" },
            Saturation => new[] { "muted color", "restrained color", "balanced color", "rich color", "vivid color" },
            Contrast => new[] { "low contrast", "gentle tonal separation", "balanced tonal contrast", "crisp contrast", "striking tonal contrast" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveChildrensBookDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddChildrensBookDescriptor(phrases, seen, "children's book illustration");
        AddChildrensBookDescriptor(phrases, seen, ResolveChildrensBookStyleDescriptor(configuration.ChildrensBookStyle));
        AddChildrensBookDescriptor(phrases, seen, ResolveChildrensBookStoryChargeDescriptor(configuration));
        AddChildrensBookDescriptor(phrases, seen, ResolveChildrensBookAtmosphereDescriptor(configuration));

        foreach (var phrase in ResolveChildrensBookCheckboxDescriptors(configuration))
        {
            AddChildrensBookDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    private static string ResolveChildrensBookStyleDescriptor(string childrensBookStyle)
    {
        return childrensBookStyle switch
        {
            "general-childrens-book" => string.Empty,
            "storybook-classic" => "classic hand-drawn pacing",
            "playful-cartoon" => "playful cartoon timing",
            "gentle-bedtime" => "gentle bedtime mood",
            "educational-illustration" => "clear educational framing",
            "whimsical-fantasy" => "whimsical fantasy staging",
            _ => string.Empty,
        };
    }

    private static string ResolveChildrensBookStoryChargeDescriptor(PromptConfiguration configuration)
    {
        if (configuration.Tension >= 81 && configuration.MotionEnergy >= 61)
        {
            return "high-stakes child-safe peril";
        }

        if (configuration.NarrativeDensity >= 81 && configuration.MotionEnergy >= 61)
        {
            return "adventure-ready story momentum";
        }

        if (configuration.Symbolism >= 81 && configuration.Awe >= 61)
        {
            return "mythic storybook symbolism";
        }

        if (configuration.NarrativeDensity >= 61)
        {
            return "story-rich scene progression";
        }

        return string.Empty;
    }

    private static string ResolveChildrensBookAtmosphereDescriptor(PromptConfiguration configuration)
    {
        if (configuration.AtmosphericDepth >= 81 && configuration.BackgroundComplexity >= 61)
        {
            return "deep story-world atmosphere";
        }

        if (configuration.Awe >= 81 && configuration.AtmosphericDepth >= 61)
        {
            return "big-hearted storybook grandeur";
        }

        if (configuration.BackgroundComplexity >= 81 && configuration.NarrativeDensity >= 61)
        {
            return "densely layered storybook environment";
        }

        return string.Empty;
    }

    private static IEnumerable<string> ResolveChildrensBookCheckboxDescriptors(PromptConfiguration configuration)
    {
        var keys = GetChildrensBookModifierPriority(configuration.ChildrensBookStyle);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Soft Color Palette" when configuration.ChildrensBookSoftColorPalette => "soft color palette",
                "Textured Paper" when configuration.ChildrensBookTexturedPaper => "textured paper",
                "Ink Linework" when configuration.ChildrensBookInkLinework => "ink linework",
                "Expressive Characters" when configuration.ChildrensBookExpressiveCharacters => "expressive characters",
                "Minimal Background" when configuration.ChildrensBookMinimalBackground => "minimal background",
                "Decorative Details" when configuration.ChildrensBookDecorativeDetails => "decorative details",
                "Gentle Lighting" when configuration.ChildrensBookGentleLighting => "gentle lighting",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetChildrensBookModifierPriority(string childrensBookStyle)
    {
        return childrensBookStyle switch
        {
            "storybook-classic" => ["Textured Paper", "Ink Linework", "Decorative Details", "Soft Color Palette", "Gentle Lighting", "Expressive Characters", "Minimal Background"],
            "playful-cartoon" => ["Expressive Characters", "Soft Color Palette", "Decorative Details", "Gentle Lighting", "Minimal Background", "Ink Linework", "Textured Paper"],
            "gentle-bedtime" => ["Gentle Lighting", "Soft Color Palette", "Textured Paper", "Expressive Characters", "Minimal Background", "Decorative Details", "Ink Linework"],
            "educational-illustration" => ["Ink Linework", "Minimal Background", "Textured Paper", "Decorative Details", "Expressive Characters", "Soft Color Palette", "Gentle Lighting"],
            "whimsical-fantasy" => ["Decorative Details", "Expressive Characters", "Soft Color Palette", "Gentle Lighting", "Ink Linework", "Textured Paper", "Minimal Background"],
            _ => ["Soft Color Palette", "Textured Paper", "Ink Linework", "Expressive Characters", "Minimal Background", "Decorative Details", "Gentle Lighting"],
        };
    }

    private static void AddChildrensBookDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyChildrensBookGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        phrase = ApplyChildrensBookPhraseEconomy(phrase);

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "vivid color";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking tonal contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "deep story-world atmosphere";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return "densely layered storybook environment";
        }

        if (string.Equals(sliderKey, NarrativeDensity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.MotionEnergy >= 61)
        {
            return "adventure-ready story momentum";
        }

        if (string.Equals(sliderKey, Symbolism, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Awe >= 61)
        {
            return "mythic storybook symbolism";
        }

        if (string.Equals(sliderKey, Tension, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.MotionEnergy >= 61)
        {
            return "high-stakes child-safe peril";
        }

        if (string.Equals(sliderKey, Awe, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth >= 61)
        {
            return "big-hearted storybook grandeur";
        }

        if (string.Equals(sliderKey, MotionEnergy, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Tension >= 61)
        {
            return "high-spirited storybook motion";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity >= 61)
        {
            return "densely layered storybook environment";
        }

        if (string.Equals(sliderKey, Chaos, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.MotionEnergy >= 61)
        {
            return "orchestrated adventure-chaos";
        }

        return phrase;
    }

    private static string ApplyChildrensBookPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("children's book ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("storybook ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("storybook", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }

    public static string ResolveComicBookPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded graphic treatment",
                "light stylization",
                "clear graphic rendering",
                "expressive stylization",
                "highly stylized presentation"),
            Realism => MapBand(value,
                "simplified graphic form",
                "lightly grounded illustrated form",
                "clear structured anatomy",
                "refined illustrative realism",
                "highly controlled graphic realism"),
            TextureDepth => MapBand(value,
                "flat printed surfaces",
                "light ink texture",
                "clear ink-surface interplay",
                "rich ink treatment",
                "heavily worked graphic texture"),
            NarrativeDensity => MapBand(value,
                "single-panel read",
                "light story cue",
                "clear story moment",
                "dense implied narrative",
                "layered story-world density"),
            Symbolism => MapBand(value,
                "literal framing",
                "subtle symbolic cue",
                "suggestive motif",
                "pronounced symbolism",
                "mythic symbolic charge"),
            SurfaceAge => MapBand(value,
                "fresh print",
                "slight print wear",
                "gentle inked-page age",
                "noticeable print patina",
                "time-softened print character"),
            Framing => MapBand(value,
                "tight panel framing",
                "light panel spacing",
                "balanced panel framing",
                "broad panel staging",
                "expansive splash composition"),
            BackgroundComplexity => MapBand(value,
                "minimal backdrop",
                "restrained scene support",
                "readable environment",
                "rich setting detail",
                "densely layered setting"),
            MotionEnergy => MapBand(value,
                "still panel moment",
                "slight action energy",
                "active scene motion",
                "dynamic action burst",
                "high-impact kinetic motion"),
            FocusDepth => MapBand(value,
                "broad focus",
                "light focus falloff",
                "balanced focus depth",
                "selective focus",
                "sharp subject-isolating focus"),
            ImageCleanliness => MapBand(value,
                "raw print finish",
                "light ink looseness",
                "balanced cleanliness",
                "clean print finish",
                "polished print finish"),
            DetailDensity => MapBand(value,
                "sparse detail",
                "light detail",
                "balanced detail",
                "rich detail",
                "dense detail"),
            AtmosphericDepth => MapBand(value,
                "flat page depth",
                "slight page recession",
                "airy page depth",
                "luminous page layering",
                "deep atmospheric perspective"),
            Chaos => MapBand(value,
                "controlled composition",
                "restless energy",
                "active energy",
                "dynamic looseness",
                "orchestrated chaos"),
            Whimsy => MapBand(value,
                "serious tone",
                "subtle playfulness",
                "playful charm",
                "expressive comic energy",
                "bold exaggerated energy"),
            Tension => MapBand(value,
                "low tension",
                "light dramatic tension",
                "noticeable tension",
                "strong dramatic tension",
                "intense conflict"),
            Awe => MapBand(value,
                "grounded scale",
                "slight sense of wonder",
                "atmosphere of significance",
                "strong spectacle",
                "overwhelming grandeur"),
            Temperature => MapBand(value,
                "cool palette",
                "slightly cool balance",
                "neutral temperature",
                "warm palette",
                "heated tone"),
            LightingIntensity => MapBand(value,
                "soft lighting",
                "gentle illumination",
                "balanced lighting",
                "bright lighting",
                "radiant lighting"),
            Saturation => MapBand(value,
                "muted palette",
                "restrained color",
                "balanced saturation",
                "rich color",
                "vivid color intensity"),
            Contrast => MapBand(value,
                "soft graphic contrast",
                "moderate tonal separation",
                "clear tonal contrast",
                "strong ink contrast",
                "high-impact graphic contrast"),
            CameraDistance => MapBand(value,
                "tight panel framing",
                "close panel framing",
                "mid-panel view",
                "broader environmental panel",
                "wide splash framing"),
            CameraAngle => MapBand(value,
                "low dramatic angle",
                "slightly lowered viewpoint",
                "eye-level panel view",
                "slightly elevated vantage",
                "detached overhead vantage"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyComicBookGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveComicBookGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded graphic treatment", "light stylization", "clear graphic rendering", "expressive stylization", "highly stylized presentation" },
            Realism => new[] { "simplified graphic form", "lightly grounded illustrated form", "clear structured anatomy", "refined illustrative realism", "highly controlled graphic realism" },
            TextureDepth => new[] { "flat printed surfaces", "light ink texture", "clear ink-surface interplay", "rich ink treatment", "heavily worked graphic texture" },
            NarrativeDensity => new[] { "single-panel read", "light story cue", "clear story moment", "dense implied narrative", "layered story-world density" },
            Symbolism => new[] { "literal framing", "subtle symbolic cue", "suggestive motif", "pronounced symbolism", "mythic symbolic charge" },
            SurfaceAge => new[] { "fresh print", "slight print wear", "gentle inked-page age", "noticeable print patina", "time-softened print character" },
            Framing => new[] { "tight panel framing", "light panel spacing", "balanced panel framing", "broad panel staging", "expansive splash composition" },
            BackgroundComplexity => new[] { "minimal backdrop", "restrained scene support", "readable environment", "rich setting detail", "densely layered setting" },
            MotionEnergy => new[] { "still panel moment", "slight action energy", "active scene motion", "dynamic action burst", "high-impact kinetic motion" },
            FocusDepth => new[] { "broad focus", "light focus falloff", "balanced focus depth", "selective focus", "sharp subject-isolating focus" },
            ImageCleanliness => new[] { "raw print finish", "light ink looseness", "balanced cleanliness", "clean print finish", "polished print finish" },
            DetailDensity => new[] { "sparse detail", "light detail", "balanced detail", "rich detail", "dense detail" },
            AtmosphericDepth => new[] { "flat page depth", "slight page recession", "airy page depth", "luminous page layering", "deep atmospheric perspective" },
            Chaos => new[] { "controlled composition", "restless energy", "active energy", "dynamic looseness", "orchestrated chaos" },
            Whimsy => new[] { "serious tone", "subtle playfulness", "playful charm", "expressive comic energy", "bold exaggerated energy" },
            Tension => new[] { "low tension", "light dramatic tension", "noticeable tension", "strong dramatic tension", "intense conflict" },
            Awe => new[] { "grounded scale", "slight sense of wonder", "atmosphere of significance", "strong spectacle", "overwhelming grandeur" },
            Temperature => new[] { "cool palette", "slightly cool balance", "neutral temperature", "warm palette", "heated tone" },
            LightingIntensity => new[] { "soft lighting", "gentle illumination", "balanced lighting", "bright lighting", "radiant lighting" },
            Saturation => new[] { "muted palette", "restrained color", "balanced saturation", "rich color", "vivid color intensity" },
            Contrast => new[] { "soft graphic contrast", "moderate tonal separation", "clear tonal contrast", "strong ink contrast", "high-impact graphic contrast" },
            CameraDistance => new[] { "tight panel framing", "close panel framing", "mid-panel view", "broader environmental panel", "wide splash framing" },
            CameraAngle => new[] { "low dramatic angle", "slightly lowered viewpoint", "eye-level panel view", "slightly elevated vantage", "detached overhead vantage" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<PromptFragment> ResolveComicBookDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<PromptFragment>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddComicBookDescriptor(phrases, seen, ResolveComicBookAnchorDescriptor(configuration.ComicBookStyle));

        var styleDescriptor = ResolveComicBookStyleDescriptor(configuration.ComicBookStyle);
        if (!string.IsNullOrWhiteSpace(styleDescriptor))
        {
            AddComicBookDescriptor(phrases, seen, styleDescriptor);
        }

        var speechBubbleDescriptor = ResolveComicBookSpeechBubbleDescriptor(configuration);
        if (!string.IsNullOrWhiteSpace(speechBubbleDescriptor))
        {
            AddComicBookDescriptor(phrases, seen, speechBubbleDescriptor, preserveFromCompression: true);
        }

        foreach (var phrase in ResolveComicBookModifierDescriptors(configuration))
        {
            AddComicBookDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    private static string ResolveComicBookAnchorDescriptor(string comicBookStyle)
    {
        return comicBookStyle switch
        {
            "Graphic Novel" => "graphic novel illustration",
            "Noir Comic" => "comic panel artwork",
            "Vintage Comic" => "comic panel artwork",
            _ => "comic book illustration",
        };
    }

    private static string ResolveComicBookStyleDescriptor(string comicBookStyle)
    {
        return comicBookStyle switch
        {
            "Superhero Comic" => "bold heroic pacing",
            "Noir Comic" => "shadow-heavy atmosphere",
            "Graphic Novel" => "mature page pacing",
            "Vintage Comic" => "halftone print character",
            "Modern Comic" => "clean contemporary finish",
            _ => string.Empty,
        };
    }

    private static string ResolveComicBookSpeechBubbleDescriptor(PromptConfiguration configuration)
    {
        if (!configuration.ComicBookSpeechBubbles)
        {
            return string.Empty;
        }

        var userText = string.Join(" ", new[] { configuration.Subject, configuration.Action, configuration.Relationship }.Where(static value => !string.IsNullOrWhiteSpace(value)));
        if (string.IsNullOrWhiteSpace(userText) || !Regex.IsMatch(userText, "\"[^\"]+\""))
        {
            return string.Empty;
        }

        return "dialogue rendered in comic speech bubbles";
    }

    private static IEnumerable<string> ResolveComicBookModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetComicBookModifierPriority(configuration.ComicBookStyle);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Bold Ink" when configuration.ComicBookBoldInk => "bold ink linework",
                "Halftone Shading" when configuration.ComicBookHalftoneShading => "halftone shading",
                "Panel Framing" when configuration.ComicBookPanelFraming => "panel-driven framing",
                "Dynamic Poses" when configuration.ComicBookDynamicPoses => "dynamic posing",
                "Speed Lines" when configuration.ComicBookSpeedLines => "speed-line motion",
                "High Contrast Lighting" when configuration.ComicBookHighContrastLighting => "high-contrast lighting",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetComicBookModifierPriority(string comicBookStyle)
    {
        return comicBookStyle switch
        {
            "Superhero Comic" => ["Dynamic Poses", "High Contrast Lighting", "Speed Lines", "Bold Ink", "Panel Framing", "Halftone Shading"],
            "Noir Comic" => ["High Contrast Lighting", "Bold Ink", "Halftone Shading", "Panel Framing", "Dynamic Poses", "Speed Lines"],
            "Graphic Novel" => ["Panel Framing", "Bold Ink", "Halftone Shading", "High Contrast Lighting", "Dynamic Poses", "Speed Lines"],
            "Vintage Comic" => ["Halftone Shading", "Bold Ink", "Panel Framing", "High Contrast Lighting", "Speed Lines", "Dynamic Poses"],
            "Modern Comic" => ["Bold Ink", "High Contrast Lighting", "Dynamic Poses", "Panel Framing", "Speed Lines", "Halftone Shading"],
            _ => ["Bold Ink", "Halftone Shading", "Dynamic Poses", "High Contrast Lighting", "Panel Framing", "Speed Lines"],
        };
    }

    private static void AddComicBookDescriptor(ICollection<PromptFragment> phrases, ISet<string> seen, string phrase, bool preserveFromCompression = false)
    {
        var cleaned = CleanComicBookPhrase(phrase);
        if (!string.IsNullOrWhiteSpace(cleaned) && seen.Add(cleaned))
        {
            phrases.Add(new PromptFragment(cleaned, preserveFromCompression));
        }
    }

    private static string ApplyComicBookGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        phrase = ApplyComicBookPhraseEconomy(phrase);

        if (IntentModeCatalog.IsComicBook(configuration.IntentMode))
        {
            if (IntentModeCatalog.IsComicBook(configuration.IntentMode) && configuration.ComicBookStyle == "Noir Comic" && string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value <= 40)
            {
                return "muted noir palette";
            }

            if (configuration.ComicBookStyle == "Noir Comic" && string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 61)
            {
                return "shadow-heavy contrast";
            }

            if (configuration.ComicBookStyle == "Superhero Comic" && string.Equals(sliderKey, MotionEnergy, StringComparison.OrdinalIgnoreCase) && value >= 61)
            {
                return "high-impact kinetic motion";
            }

            if (configuration.ComicBookStyle == "Superhero Comic" && string.Equals(sliderKey, Awe, StringComparison.OrdinalIgnoreCase) && value >= 61)
            {
                return "larger-than-life spectacle";
            }

            if (configuration.ComicBookStyle == "Graphic Novel" && string.Equals(sliderKey, NarrativeDensity, StringComparison.OrdinalIgnoreCase) && value >= 61)
            {
                return "layered story-world density";
            }

            if (configuration.ComicBookStyle == "Graphic Novel" && string.Equals(sliderKey, Whimsy, StringComparison.OrdinalIgnoreCase) && value >= 61)
            {
                return "mature tone";
            }

            if (configuration.ComicBookStyle == "Vintage Comic" && string.Equals(sliderKey, TextureDepth, StringComparison.OrdinalIgnoreCase) && value >= 41)
            {
                return "halftone texture";
            }

            if (configuration.ComicBookStyle == "Modern Comic" && string.Equals(sliderKey, Stylization, StringComparison.OrdinalIgnoreCase) && value >= 61)
            {
                return "clean modern graphic rendering";
            }
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value <= 20)
        {
            return "muted print color";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "high-impact graphic contrast";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "densely layered setting";
        }

        if (string.Equals(sliderKey, NarrativeDensity, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "layered story-world density";
        }

        return phrase;
    }

    private static string ApplyComicBookPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("comic book ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("graphic novel ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("comic ", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }

    private static string CleanComicBookPhrase(string? phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        var cleaned = phrase.Trim();
        while (cleaned.Contains("  ", StringComparison.Ordinal))
        {
            cleaned = cleaned.Replace("  ", " ", StringComparison.Ordinal);
        }

        return cleaned.Trim(' ', ',', '.');
    }

    public static string ResolveCinematicPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded cinematic treatment",
                "light directorial stylization",
                "director-led screen stylization",
                "strong cinematic stylization",
                "highly stylized screen presentation"),
            Realism => MapBand(value,
                string.Empty,
                "loosely realistic screen rendering",
                "moderately realistic cinematic rendering",
                "high visual realism",
                "strongly realistic screen-grade rendering"),
            TextureDepth => MapBand(value,
                "minimal surface texture",
                "light cinematic texture",
                "production-surface tactility",
                "rich production texture",
                "deeply worked tactile detail"),
            NarrativeDensity => MapBand(value,
                "single-shot visual beat",
                "light scene implication",
                "scene-driven storytelling cues",
                "densely implied cinematic narrative",
                "world-rich cinematic narrative"),
            Symbolism => MapBand(value,
                "literal framing",
                "subtle symbolic cue",
                "suggestive symbolic motif",
                "pronounced thematic symbolism",
                "mythic symbolic charge"),
            SurfaceAge => MapBand(value,
                "fresh finish",
                "slight production patina",
                "gentle film wear",
                "noticeable analog character",
                "time-softened screen finish"),
            Framing => MapBand(value,
                "intimate framing",
                "tight cinematic framing",
                "balanced screen framing",
                "expansive cinematic framing",
                "large-format cinematic framing"),
            BackgroundComplexity => MapBand(value,
                "minimal set background",
                "restrained set dressing",
                "supporting production detail",
                "rich environmental staging",
                "densely layered cinematic environment"),
            MotionEnergy => MapBand(value,
                "held cinematic frame",
                "gentle screen motion",
                "active cinematic energy",
                "dynamic action energy",
                "high kinetic cinematic motion"),
            FocusDepth => MapBand(value,
                "deep focus clarity",
                "moderate depth separation",
                "selective cinematic focus",
                "shallow depth of field",
                "very shallow cinematic focus"),
            ImageCleanliness => MapBand(value,
                "raw filmic finish",
                "lightly refined screen finish",
                "clean cinematic finish",
                "polished theatrical finish",
                "highly polished screen finish"),
            DetailDensity => MapBand(value,
                "sparse scene detail",
                "light scene detail",
                "balanced production detail",
                "rich screen detail",
                "dense cinematic detail"),
            AtmosphericDepth => MapBand(value,
                "slight atmospheric falloff",
                "light spatial recession",
                "air-filled cinematic depth",
                "luminous theatrical depth",
                "deeply layered cinematic atmosphere"),
            Chaos => MapBand(value,
                "controlled composition",
                "restless screen tension",
                "active scene energy",
                "dynamic scene turbulence",
                "orchestrated cinematic chaos"),
            Whimsy => MapBand(value,
                "serious tone",
                "light emotional lift",
                "subtle human warmth",
                "strong tonal softness",
                "gentle dramatic playfulness"),
            Tension => MapBand(value,
                "low dramatic tension",
                "light dramatic charge",
                "scene-level tension",
                "strong cinematic tension",
                "high-stakes dramatic pressure"),
            Awe => MapBand(value,
                "grounded scale",
                "slight cinematic wonder",
                "atmosphere of spectacle",
                "strong cinematic awe",
                "overwhelming big-screen grandeur"),
            Temperature => MapBand(value,
                "cool screen balance",
                "slightly cool film balance",
                "neutral film color",
                "warm cinematic balance",
                "heated theatrical warmth"),
            LightingIntensity => MapBand(value,
                "dim screen light",
                "soft cinematic daylight",
                "balanced set illumination",
                "bright film-set light",
                "radiant screen lighting"),
            Saturation => MapBand(value,
                "muted film color",
                "restrained cinematic color",
                "balanced production color",
                "rich cinematic color",
                "luminous screen color"),
            Contrast => MapBand(value,
                "low cinematic contrast",
                "gentle tonal separation",
                "balanced cinematic contrast",
                "crisp screen contrast",
                "striking theatrical contrast"),
            CameraDistance => MapBand(value,
                "close dramatic view",
                "close screen view",
                "mid-distance cinematic view",
                "wide scene view",
                "far-set cinematic distance"),
            CameraAngle => MapBand(value,
                "eye-level view",
                "slightly lowered viewpoint",
                "balanced observational angle",
                "dramatic low angle",
                "theatrical elevated view"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyCinematicGuardrails(sliderKey, value, configuration, ApplyCinematicPhraseEconomy(phrase));
    }

    public static string ResolveCinematicGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded cinematic treatment", "light directorial stylization", "director-led screen stylization", "strong cinematic stylization", "highly stylized screen presentation" },
            Realism => new[] { "omit explicit realism", "loosely realistic screen rendering", "moderately realistic cinematic rendering", "high visual realism", "strongly realistic screen-grade rendering" },
            TextureDepth => new[] { "minimal surface texture", "light cinematic texture", "production-surface tactility", "rich production texture", "deeply worked tactile detail" },
            NarrativeDensity => new[] { "single-shot visual beat", "light scene implication", "scene-driven storytelling cues", "densely implied cinematic narrative", "world-rich cinematic narrative" },
            Symbolism => new[] { "literal framing", "subtle symbolic cue", "suggestive symbolic motif", "pronounced thematic symbolism", "mythic symbolic charge" },
            SurfaceAge => new[] { "fresh finish", "slight production patina", "gentle film wear", "noticeable analog character", "time-softened screen finish" },
            Framing => new[] { "intimate framing", "tight cinematic framing", "balanced screen framing", "expansive cinematic framing", "large-format cinematic framing" },
            BackgroundComplexity => new[] { "minimal set background", "restrained set dressing", "supporting production detail", "rich environmental staging", "densely layered cinematic environment" },
            MotionEnergy => new[] { "held cinematic frame", "gentle screen motion", "active cinematic energy", "dynamic action energy", "high kinetic cinematic motion" },
            FocusDepth => new[] { "deep focus clarity", "moderate depth separation", "selective cinematic focus", "shallow depth of field", "very shallow cinematic focus" },
            ImageCleanliness => new[] { "raw filmic finish", "lightly refined screen finish", "clean cinematic finish", "polished theatrical finish", "highly polished screen finish" },
            DetailDensity => new[] { "sparse scene detail", "light scene detail", "balanced production detail", "rich screen detail", "dense cinematic detail" },
            AtmosphericDepth => new[] { "slight atmospheric falloff", "light spatial recession", "air-filled cinematic depth", "luminous theatrical depth", "deeply layered cinematic atmosphere" },
            Chaos => new[] { "controlled composition", "restless screen tension", "active scene energy", "dynamic scene turbulence", "orchestrated cinematic chaos" },
            Whimsy => new[] { "serious tone", "light emotional lift", "subtle human warmth", "strong tonal softness", "gentle dramatic playfulness" },
            Tension => new[] { "low dramatic tension", "light dramatic charge", "scene-level tension", "strong cinematic tension", "high-stakes dramatic pressure" },
            Awe => new[] { "grounded scale", "slight cinematic wonder", "atmosphere of spectacle", "strong cinematic awe", "overwhelming big-screen grandeur" },
            Temperature => new[] { "cool screen balance", "slightly cool film balance", "neutral film color", "warm cinematic balance", "heated theatrical warmth" },
            LightingIntensity => new[] { "dim screen light", "soft cinematic daylight", "balanced set illumination", "bright film-set light", "radiant screen lighting" },
            Saturation => new[] { "muted film color", "restrained cinematic color", "balanced production color", "rich cinematic color", "luminous screen color" },
            Contrast => new[] { "low cinematic contrast", "gentle tonal separation", "balanced cinematic contrast", "crisp screen contrast", "striking theatrical contrast" },
            CameraDistance => new[] { "close dramatic view", "close screen view", "mid-distance cinematic view", "wide scene view", "far-set cinematic distance" },
            CameraAngle => new[] { "eye-level view", "slightly lowered viewpoint", "balanced observational angle", "dramatic low angle", "theatrical elevated view" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveCinematicDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddCinematicDescriptor(phrases, seen, "cinematic film still");

        var subtypeDescriptor = ResolveCinematicSubtypeDescriptor(configuration.CinematicSubtype);
        if (!string.IsNullOrWhiteSpace(subtypeDescriptor))
        {
            AddCinematicDescriptor(phrases, seen, subtypeDescriptor);
        }

        foreach (var phrase in ResolveCinematicModifierDescriptors(configuration))
        {
            AddCinematicDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveCinematicLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "soft cinematic daylight",
            "Golden hour" => "golden-hour screen light",
            "Dramatic studio light" => "dramatic film-set lighting",
            "Overcast" => "muted overcast screen light",
            "Moonlit" => "moody moonlit screen light",
            "Soft glow" => "soft cinematic glow",
            "Dusk haze" => "dusk-haze screen light",
            "Warm directional light" => "warm directional film light",
            "Volumetric cinematic light" => "volumetric cinematic light",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    public static string ResolvePhotographyPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetPhotographyBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyPhotographyGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolvePhotographyGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetPhotographyBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolvePhotographyDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var typeDescriptor = ResolvePhotographyTypeDescriptor(configuration.PhotographyType);
        if (!string.IsNullOrWhiteSpace(typeDescriptor))
        {
            AddPhotographyDescriptor(phrases, seen, typeDescriptor);
        }

        var eraDescriptor = ResolvePhotographyEraDescriptor(configuration.PhotographyEra);
        if (!string.IsNullOrWhiteSpace(eraDescriptor))
        {
            AddPhotographyDescriptor(phrases, seen, eraDescriptor);
        }

        foreach (var phrase in ResolvePhotographyModifierDescriptors(configuration))
        {
            AddPhotographyDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolvePhotographyLightingDescriptor(PromptConfiguration configuration)
    {
        var historical = IsHistoricalPhotography(configuration);

        return configuration.Lighting switch
        {
            "Soft daylight" => historical ? "period daylight" : "soft scene light",
            "Golden hour" => historical ? "late-day natural light" : "warm natural light",
            "Dramatic studio light" => historical ? "staged portrait light" : "controlled studio light",
            "Overcast" => historical ? "diffused historical daylight" : "soft overcast light",
            "Moonlit" => historical ? "low-illumination nocturne light" : "cool low-light capture",
            "Soft glow" => historical ? "chemical plate softness" : "gentle print glow",
            "Dusk haze" => historical ? "late-day atmospheric light" : "evening atmospheric light",
            "Warm directional light" => historical ? "window-directed period light" : "directional key light",
            "Volumetric cinematic light" => historical ? "atmospheric process light" : "layered atmospheric light",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveCinematicSubtypeDescriptor(string cinematicSubtype)
    {
        return cinematicSubtype switch
        {
            "general-film-still" => string.Empty,
            "prestige-drama" => "prestige-drama restraint",
            "thriller-suspense" => "suspense-driven scene tension",
            "noir-neo-noir" => "shadow-heavy noir framing",
            "epic-blockbuster" => "large-format cinematic spectacle",
            "intimate-indie" => "intimate observational framing",
            "sci-fi-cinema" => "futurist cinematic staging",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveCinematicModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetCinematicModifierPriority(configuration.CinematicSubtype);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Letterboxed Framing" when configuration.CinematicLetterboxedFraming => "letterboxed cinematic framing",
                "Shallow Depth of Field" when configuration.CinematicShallowDepthOfField => "shallow depth of field",
                "Practical Lighting" when configuration.CinematicPracticalLighting => "practical-light motivated illumination",
                "Atmospheric Haze" when configuration.CinematicAtmosphericHaze => "atmospheric haze layering",
                "Film Grain" when configuration.CinematicFilmGrain => "subtle film-grain texture",
                "Anamorphic Flares" when configuration.CinematicAnamorphicFlares => "anamorphic flare accents",
                "Dramatic Backlight" when configuration.CinematicDramaticBacklight => "dramatic backlight separation",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetCinematicModifierPriority(string cinematicSubtype)
    {
        return cinematicSubtype switch
        {
            "prestige-drama" => ["Practical Lighting", "Shallow Depth of Field", "Film Grain", "Letterboxed Framing", "Dramatic Backlight", "Atmospheric Haze", "Anamorphic Flares"],
            "thriller-suspense" => ["Practical Lighting", "Atmospheric Haze", "Dramatic Backlight", "Shallow Depth of Field", "Film Grain", "Letterboxed Framing", "Anamorphic Flares"],
            "noir-neo-noir" => ["Dramatic Backlight", "Practical Lighting", "Film Grain", "Atmospheric Haze", "Shallow Depth of Field", "Letterboxed Framing", "Anamorphic Flares"],
            "epic-blockbuster" => ["Letterboxed Framing", "Atmospheric Haze", "Anamorphic Flares", "Dramatic Backlight", "Practical Lighting", "Shallow Depth of Field", "Film Grain"],
            "intimate-indie" => ["Shallow Depth of Field", "Practical Lighting", "Film Grain", "Letterboxed Framing", "Atmospheric Haze", "Dramatic Backlight", "Anamorphic Flares"],
            "sci-fi-cinema" => ["Anamorphic Flares", "Atmospheric Haze", "Practical Lighting", "Letterboxed Framing", "Shallow Depth of Field", "Film Grain", "Dramatic Backlight"],
            _ => ["Shallow Depth of Field", "Practical Lighting", "Film Grain", "Atmospheric Haze", "Dramatic Backlight", "Anamorphic Flares", "Letterboxed Framing"],
        };
    }

    private static void AddCinematicDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyCinematicGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "luminous screen color";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking theatrical contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "deeply layered cinematic atmosphere";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return "densely layered cinematic environment";
        }

        return phrase;
    }

    private static string ApplyCinematicPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("cinematic ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("cinematic-", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }

    public static string ResolveThreeDRenderPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded 3D treatment",
                "light 3D stylization",
                "stylized CGI rendering",
                "strong 3D stylization",
                "highly stylized CGI presentation"),
            Realism => MapBand(value,
                string.Empty,
                "loosely realistic 3D rendering",
                "moderately realistic CGI rendering",
                "high visual realism in 3D",
                "photoreal 3D rendering"),
            TextureDepth => MapBand(value,
                "minimal surface texture",
                "light rendered texture",
                "clear material texture",
                "rich rendered surface detail",
                "deeply worked material definition"),
            NarrativeDensity => MapBand(value,
                "single-read render concept",
                "light story suggestion",
                "scene-supporting story cues",
                "layered rendered storytelling",
                "world-rich CGI narrative"),
            Symbolism => MapBand(value,
                "literal presentation",
                "subtle symbolic cue",
                "suggestive design motif",
                "pronounced symbolic intent",
                "mythic symbolic charge"),
            SurfaceAge => MapBand(value,
                "fresh surface finish",
                "slight production wear",
                "gentle material patina",
                "noticeable render wear",
                "time-softened surface finish"),
            Framing => MapBand(value,
                "intimate render framing",
                "tight presentation framing",
                "balanced render framing",
                "expansive scene framing",
                "showcase-scale framing"),
            BackgroundComplexity => MapBand(value,
                "minimal render background",
                "restrained scene support",
                "supporting rendered environment",
                "rich CGI environment",
                "densely layered rendered environment"),
            MotionEnergy => MapBand(value,
                "still render composition",
                "gentle scene motion",
                "active rendered energy",
                "dynamic presentation energy",
                "high kinetic presentation motion"),
            FocusDepth => MapBand(value,
                "deep focus clarity",
                "moderate focus separation",
                "selective render focus",
                "shallow depth of field",
                "very shallow presentation focus"),
            ImageCleanliness => MapBand(value,
                "raw render finish",
                "lightly refined render finish",
                "clean CGI finish",
                "polished render finish",
                "ultra-clean commercial finish"),
            DetailDensity => MapBand(value,
                "sparse digital detail",
                "light supporting detail",
                "clear modeled detail",
                "dense rendered detail layering",
                "high-density production detail"),
            AtmosphericDepth => MapBand(value,
                "limited atmospheric depth",
                "slight rendered recession",
                "air-filled scene depth",
                "luminous rendered depth",
                "deep CGI atmospheric layering"),
            Chaos => MapBand(value,
                "controlled presentation",
                "restless scene tension",
                "active render energy",
                "dynamic visual turbulence",
                "orchestrated digital chaos"),
            Whimsy => MapBand(value,
                "serious tone",
                "light stylistic charm",
                "balanced visual playfulness",
                "strong stylized energy",
                "bold presentation energy"),
            Tension => MapBand(value,
                "low dramatic tension",
                "light dramatic charge",
                "scene-level tension",
                "strong render tension",
                "intense dramatic pressure"),
            Awe => MapBand(value,
                "grounded scale",
                "slight visual wonder",
                "atmosphere of spectacle",
                "strong presentation awe",
                "overwhelming render grandeur"),
            Temperature => MapBand(value,
                "cool render balance",
                "slightly cool digital balance",
                "neutral render temperature",
                "warm presentation balance",
                "heated screen color"),
            LightingIntensity => MapBand(value,
                "soft rendered daylight",
                "studio render lighting",
                "balanced render lighting",
                "bright CGI lighting",
                "radiant presentation lighting"),
            Saturation => MapBand(value,
                "muted render color",
                "restrained digital color",
                "balanced render color",
                "rich CGI color",
                "vivid rendered color"),
            Contrast => MapBand(value,
                "low rendered contrast",
                "gentle tonal separation",
                "balanced render contrast",
                "crisp CGI contrast",
                "striking render contrast"),
            CameraDistance => MapBand(value,
                "extreme close render view",
                "close product view",
                "mid-distance render view",
                "wide presentation view",
                "far-set scene view"),
            CameraAngle => MapBand(value,
                "eye-level view",
                "slightly lowered viewpoint",
                "balanced presentation angle",
                "dramatic low angle",
                "showcase elevated angle"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyThreeDRenderGuardrails(sliderKey, value, configuration, ApplyThreeDRenderPhraseEconomy(phrase));
    }

    public static string ResolveConceptArtPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded production design treatment",
                "light concept stylization",
                "stylized production rendering",
                "strong production-design stylization",
                "highly stylized portfolio presentation"),
            Realism => MapBand(value,
                string.Empty,
                "loosely realistic design rendering",
                "moderately realistic concept rendering",
                "high visual realism in production art",
                "portfolio-grade concept realism"),
            TextureDepth => MapBand(value,
                "minimal material texture",
                "light surface articulation",
                "clear material texture",
                "rich tactile design texture",
                "deeply worked material articulation"),
            NarrativeDensity => MapBand(value,
                "single-read design idea",
                "light worldbuilding cues",
                "layered story-world context",
                "densely implied narrative worldbuilding",
                "world-rich concept narrative"),
            Symbolism => MapBand(value,
                "mostly literal design language",
                "subtle worldbuilding motifs",
                "suggestive design cues",
                "pronounced allegorical design motifs",
                "mythic symbolic worldbuilding"),
            SurfaceAge => MapBand(value,
                "freshly drafted surfaces",
                "slight development patina",
                "gentle work-in-progress wear",
                "noticeable concept-board character",
                "time-worn reference-board character"),
            Framing => MapBand(value,
                "intimate design framing",
                "tight presentation framing",
                "balanced concept framing",
                "expansive presentation framing",
                "showcase-scale framing"),
            BackgroundComplexity => MapBand(value,
                "minimal presentation background",
                "restrained scene support",
                "supporting design environment",
                "rich worldbuilding support",
                "densely layered concept environment"),
            MotionEnergy => MapBand(value,
                "still design moment",
                "gentle scene motion",
                "active concept energy",
                "dynamic presentation energy",
                "high kinetic design motion"),
            FocusDepth => MapBand(value,
                "deep focus clarity",
                "moderate depth separation",
                "selective presentation focus",
                "shallow presentation focus",
                "very shallow concept focus"),
            ImageCleanliness => MapBand(value,
                "raw development-board finish",
                "lightly refined concept finish",
                "clean design presentation",
                "polished concept presentation",
                "highly polished portfolio presentation"),
            DetailDensity => MapBand(value,
                "sparse design detail",
                "light supporting detail",
                "clear design detailing",
                "dense production detail layering",
                "high-density concept detail"),
            AtmosphericDepth => MapBand(value,
                "limited atmospheric depth",
                "slight spatial recession",
                "air-filled design depth",
                "luminous presentation depth",
                "deep worldbuilding atmosphere"),
            Chaos => MapBand(value,
                "controlled design turbulence",
                "light creative restlessness",
                "active concept energy",
                "dynamic design turbulence",
                "orchestrated creative chaos"),
            Whimsy => MapBand(value,
                "serious tone",
                "subtle tonal lift",
                "mild creative play",
                "strong expressive energy",
                "bold imaginative flourish"),
            Tension => MapBand(value,
                "low design tension",
                "light dramatic tension",
                "noticeable scene tension",
                "strong dramatic pressure",
                "intense creative tension"),
            Awe => MapBand(value,
                "grounded scale",
                "slight sense of wonder",
                "atmosphere of significance",
                "strong production-scale awe",
                "overwhelming concept-scale grandeur"),
            Saturation => MapBand(value,
                "muted design color",
                "restrained production color",
                "balanced concept color",
                "rich presentation color",
                "luminous design color"),
            Contrast => MapBand(value,
                "low design contrast",
                "gentle tonal separation",
                "balanced concept contrast",
                "crisp presentation contrast",
                "striking design contrast"),
            Temperature => MapBand(value,
                "cool concept balance",
                "slightly cool design balance",
                "balanced concept color temperature",
                "warm design balance",
                "heated presentation warmth"),
            LightingIntensity => MapBand(value,
                "dim design light",
                "soft presentation light",
                "balanced production illumination",
                "bright concept light",
                "radiant presentation lighting"),
            CameraDistance => MapBand(value,
                "extreme close design view",
                "close presentation view",
                "mid-distance concept view",
                "wide design view",
                "far-set world view"),
            CameraAngle => MapBand(value,
                "low design angle",
                "slightly lowered design viewpoint",
                "eye-level presentation angle",
                "slightly elevated concept angle",
                "high design vantage"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyConceptArtGuardrails(sliderKey, value, configuration, ApplyConceptArtPhraseEconomy(phrase));
    }

    public static string ResolveThreeDRenderGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded 3D treatment", "light 3D stylization", "stylized CGI rendering", "strong 3D stylization", "highly stylized CGI presentation" },
            Realism => new[] { "omit explicit realism", "loosely realistic 3D rendering", "moderately realistic CGI rendering", "high visual realism in 3D", "photoreal 3D rendering" },
            TextureDepth => new[] { "minimal surface texture", "light rendered texture", "clear material texture", "rich rendered surface detail", "deeply worked material definition" },
            NarrativeDensity => new[] { "single-read render concept", "light story suggestion", "scene-supporting story cues", "layered rendered storytelling", "world-rich CGI narrative" },
            Symbolism => new[] { "literal presentation", "subtle symbolic cue", "suggestive design motif", "pronounced symbolic intent", "mythic symbolic charge" },
            SurfaceAge => new[] { "fresh surface finish", "slight production wear", "gentle material patina", "noticeable render wear", "time-softened surface finish" },
            Framing => new[] { "intimate render framing", "tight presentation framing", "balanced render framing", "expansive scene framing", "showcase-scale framing" },
            BackgroundComplexity => new[] { "minimal render background", "restrained scene support", "supporting rendered environment", "rich CGI environment", "densely layered rendered environment" },
            MotionEnergy => new[] { "still render composition", "gentle scene motion", "active rendered energy", "dynamic presentation energy", "high kinetic presentation motion" },
            FocusDepth => new[] { "deep focus clarity", "moderate focus separation", "selective render focus", "shallow depth of field", "very shallow presentation focus" },
            ImageCleanliness => new[] { "raw render finish", "lightly refined render finish", "clean CGI finish", "polished render finish", "ultra-clean commercial finish" },
            DetailDensity => new[] { "sparse digital detail", "light supporting detail", "clear modeled detail", "dense rendered detail layering", "high-density production detail" },
            AtmosphericDepth => new[] { "limited atmospheric depth", "slight rendered recession", "air-filled scene depth", "luminous rendered depth", "deep CGI atmospheric layering" },
            Chaos => new[] { "controlled presentation", "restless scene tension", "active render energy", "dynamic visual turbulence", "orchestrated digital chaos" },
            Whimsy => new[] { "serious tone", "light stylistic charm", "balanced visual playfulness", "strong stylized energy", "bold presentation energy" },
            Tension => new[] { "low dramatic tension", "light dramatic charge", "scene-level tension", "strong render tension", "intense dramatic pressure" },
            Awe => new[] { "grounded scale", "slight visual wonder", "atmosphere of spectacle", "strong presentation awe", "overwhelming render grandeur" },
            Temperature => new[] { "cool render balance", "slightly cool digital balance", "neutral render temperature", "warm presentation balance", "heated screen color" },
            LightingIntensity => new[] { "soft rendered daylight", "studio render lighting", "balanced render lighting", "bright CGI lighting", "radiant presentation lighting" },
            Saturation => new[] { "muted render color", "restrained digital color", "balanced render color", "rich CGI color", "vivid rendered color" },
            Contrast => new[] { "low rendered contrast", "gentle tonal separation", "balanced render contrast", "crisp CGI contrast", "striking render contrast" },
            CameraDistance => new[] { "extreme close render view", "close product view", "mid-distance render view", "wide presentation view", "far-set scene view" },
            CameraAngle => new[] { "eye-level view", "slightly lowered viewpoint", "balanced presentation angle", "dramatic low angle", "showcase elevated angle" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static string ResolveConceptArtGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded production design treatment", "light concept stylization", "stylized production rendering", "strong production-design stylization", "highly stylized portfolio presentation" },
            Realism => new[] { "omit explicit realism", "loosely realistic design rendering", "moderately realistic concept rendering", "high visual realism in production art", "portfolio-grade concept realism" },
            TextureDepth => new[] { "minimal material texture", "light surface articulation", "clear material texture", "rich tactile design texture", "deeply worked material articulation" },
            NarrativeDensity => new[] { "single-read design idea", "light worldbuilding cues", "layered story-world context", "densely implied narrative worldbuilding", "world-rich concept narrative" },
            Symbolism => new[] { "mostly literal design language", "subtle worldbuilding motifs", "suggestive design cues", "pronounced allegorical design motifs", "mythic symbolic worldbuilding" },
            SurfaceAge => new[] { "freshly drafted surfaces", "slight development patina", "gentle work-in-progress wear", "noticeable concept-board character", "time-worn reference-board character" },
            Framing => new[] { "intimate design framing", "tight presentation framing", "balanced concept framing", "expansive presentation framing", "showcase-scale framing" },
            BackgroundComplexity => new[] { "minimal presentation background", "restrained scene support", "supporting design environment", "rich worldbuilding support", "densely layered concept environment" },
            MotionEnergy => new[] { "still design moment", "gentle scene motion", "active concept energy", "dynamic presentation energy", "high kinetic design motion" },
            FocusDepth => new[] { "deep focus clarity", "moderate depth separation", "selective presentation focus", "shallow presentation focus", "very shallow concept focus" },
            ImageCleanliness => new[] { "raw development-board finish", "lightly refined concept finish", "clean design presentation", "polished concept presentation", "highly polished portfolio presentation" },
            DetailDensity => new[] { "sparse design detail", "light supporting detail", "clear design detailing", "dense production detail layering", "high-density concept detail" },
            AtmosphericDepth => new[] { "limited atmospheric depth", "slight spatial recession", "air-filled design depth", "luminous presentation depth", "deep worldbuilding atmosphere" },
            Chaos => new[] { "controlled design turbulence", "light creative restlessness", "active concept energy", "dynamic design turbulence", "orchestrated creative chaos" },
            Whimsy => new[] { "serious tone", "subtle tonal lift", "mild creative play", "strong expressive energy", "bold imaginative flourish" },
            Tension => new[] { "low design tension", "light dramatic tension", "noticeable scene tension", "strong dramatic pressure", "intense creative tension" },
            Awe => new[] { "grounded scale", "slight sense of wonder", "atmosphere of significance", "strong production-scale awe", "overwhelming concept-scale grandeur" },
            Saturation => new[] { "muted design color", "restrained production color", "balanced concept color", "rich presentation color", "luminous design color" },
            Contrast => new[] { "low design contrast", "gentle tonal separation", "balanced concept contrast", "crisp presentation contrast", "striking design contrast" },
            Temperature => new[] { "cool concept balance", "slightly cool design balance", "balanced concept color temperature", "warm design balance", "heated presentation warmth" },
            LightingIntensity => new[] { "dim design light", "soft presentation light", "balanced production illumination", "bright concept light", "radiant presentation lighting" },
            CameraDistance => new[] { "extreme close design view", "close presentation view", "mid-distance concept view", "wide design view", "far-set world view" },
            CameraAngle => new[] { "low design angle", "slightly lowered design viewpoint", "eye-level presentation angle", "slightly elevated concept angle", "high design vantage" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    private static string[] GetPhotographyBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var historical = IsHistoricalPhotography(configuration);
        return sliderKey switch
        {
            Stylization => historical
                ? ["formal process stillness", "lightly staged process treatment", "measured period image treatment", "deliberately styled plate treatment", "fully art-directed historical construction"]
                : ["observational capture", "lightly directed image treatment", "guided editorial treatment", "deliberately shaped visual treatment", "fully art-directed image construction"],
            Realism => historical
                ? ["observed period realism", "lightly arranged historical presence", "plate-grounded subject presence", "high-fidelity archival presence", "deeply convincing period presence"]
                : ["unforced scene observation", "lightly interpreted presence", "lens-grounded subject presence", "high-fidelity scene presence", "deeply convincing camera-true presence"],
            TextureDepth => historical
                ? ["plate-surface softness", "fine plate grain", "visible process texture", "rich archival texture", "deep plate character"]
                : ["clean capture surface", "faint print grain", "visible print texture", "rich surface grain", "deep print body"],
            NarrativeDensity => historical
                ? ["singular historical instant", "light period implication", "implied period narrative", "layered archival context", "dense historical storytelling"]
                : ["single captured instant", "light scene implication", "implied story in frame", "layered social context", "dense visual storytelling"],
            Symbolism => historical
                ? ["literal period document", "restrained emblematic cue", "suggestive process motif", "pronounced ceremonial symbolism", "archival allegory"]
                : ["literal documentary read", "restrained visual cue", "suggestive visual motif", "pronounced framing symbolism", "editorial allegory"],
            SurfaceAge => historical
                ? ["fresh process surface", "slight plate wear", "gentle archival aging", "noticeable collector patina", "time-softened plate character"]
                : ["fresh capture surface", "faint print wear", "gentle print aging", "noticeable print patina", "time-softened surface character"],
            Framing => historical
                ? ["intimate plate border", "close formal crop", "measured period framing", "broader contextual staging", "expansive historical tableau"]
                : ["intimate framing", "close portrait crop", "measured framing", "broader environmental framing", "expansive editorial staging"],
            BackgroundComplexity => historical
                ? ["spare background field", "restrained period setting", "supporting historical environment", "richly situated context", "densely layered period surroundings"]
                : ["minimal backdrop", "restrained setting", "supporting environment", "contextual scene detail", "densely layered surroundings"],
            MotionEnergy => historical
                ? ["formal stillness", "faint exposure drift", "visible long-exposure trace", "dragged motion blur", "sweeping exposure smear"]
                : ["still captured moment", "slight movement trace", "active scene motion", "candid motion energy", "split-second kinetic force"],
            FocusDepth => historical
                ? ["broad plate clarity", "shallow edge softness", "measured process focus", "selective subject isolation", "razor-held sitter clarity"]
                : ["broad image clarity", "light focus falloff", "measured focus depth", "selective focus isolation", "sharp subject separation"],
            ImageCleanliness => historical
                ? ["raw process texture", "slight plate softness", "tidy print clarity", "careful archival refinement", "immaculate plate finish"]
                : ["raw capture texture", "slight surface grit", "measured print finish", "controlled editorial polish", "immaculate print clarity"],
            DetailDensity => historical
                ? ["sparse plate detail", "light documentary detail", "clear archival detail", "rich print detail", "dense process detail"]
                : ["sparse scene detail", "light observed detail", "clear scene detail", "rich visual detail", "dense fine detail"],
            AtmosphericDepth => historical
                ? ["flat process space", "shallow tonal recession", "visible process depth", "luminous plate recession", "deep archival atmosphere"]
                : ["flat recorded space", "slight air separation", "visible atmospheric recession", "luminous scene depth", "deep lens-carried depth"],
            Chaos => historical
                ? ["controlled plate arrangement", "quiet period unrest", "field instability", "loosened process disorder", "orchestrated historical turbulence"]
                : ["controlled composition", "quiet restlessness", "scene instability", "loose street volatility", "orchestrated visual disorder"],
            Whimsy => historical
                ? ["formal social tone", "slight human lightness", "reserved social play", "warm salon looseness", "gentle period playfulness"]
                : ["serious tone", "subtle human lightness", "casual social play", "warm interpersonal looseness", "gentle editorial playfulness"],
            Tension => historical
                ? ["composed witness tone", "faint human unease", "noticeable social tension", "strong ceremonial pressure", "intense historical tension"]
                : ["quiet witness tone", "light human unease", "noticeable documentary tension", "strong interpersonal pressure", "intense scene tension"],
            Awe => historical
                ? ["document-bound scale", "slight archival presence", "quiet historical wonder", "strong historical awe", "expansive archival grandeur"]
                : ["human-scale grounding", "slight sense of presence", "quiet visual wonder", "strong felt scale", "expansive image grandeur"],
            Temperature => historical
                ? ["cool silvered tone", "restrained neutral toning", "balanced period toning", "warm sepia bias", "deep archival warmth"]
                : ["cool daylight balance", "slightly cool neutrality", "neutral daylight balance", "warm natural light", "heated warm cast"],
            LightingIntensity => historical
                ? ["dim available light", "soft window light", "steady period light", "bright process exposure", "radiant plate glow"]
                : ["dim scene light", "soft scene light", "even illumination", "bright natural light", "radiant scene light"],
            Saturation => historical
                ? ["monochrome restraint", "restrained sepia tone", "balanced historical toning", "rich print tonality", "hand-tinted period color"]
                : ["muted color", "restrained color", "natural color balance", "rich color presence", "vivid color charge"],
            Contrast => historical
                ? ["soft print contrast", "gentle plate contrast", "measured process contrast", "crisp historical separation", "striking print contrast"]
                : ["low tonal contrast", "gentle tonal separation", "balanced tonal contrast", "crisp tonal separation", "striking tonal contrast"],
            CameraDistance => historical
                ? ["intimate plate portrait", "close sitter study", "mid-distance period view", "broader historical context", "wide period framing"]
                : ["intimate close portrait", "close human view", "mid-distance scene view", "wider contextual view", "far-set environmental view"],
            CameraAngle => historical
                ? ["eye-level plate view", "slightly lowered human viewpoint", "composed formal angle", "slightly elevated historical vantage", "high detached vantage"]
                : ["eye-level view", "slightly lowered viewpoint", "level camera angle", "slightly elevated vantage", "high documentary vantage"],
            _ => Array.Empty<string>(),
        };
    }

    private static string ResolvePhotographyTypeDescriptor(string photographyType)
    {
        return photographyType switch
        {
            "portrait" => "portrait photography",
            "lifestyle-editorial" => "lifestyle editorial photography",
            "documentary-street" => "street documentary photography",
            "fine-art-photography" => "fine-art photography",
            "commercial-photography" => "commercial photography",
            _ => string.Empty,
        };
    }

    private static string ResolvePhotographyEraDescriptor(string photographyEra)
    {
        return photographyEra switch
        {
            "contemporary" => string.Empty,
            "nineteenth-century-process" => "process print character",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolvePhotographyModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetPhotographyModifierPriority(configuration.PhotographyType, configuration.PhotographyEra);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Candid Capture" when configuration.PhotographyCandidCapture => "candid capture",
                "Posed / Staged Capture" when configuration.PhotographyPosedStagedCapture => "posed studio arrangement",
                "Available Light" when configuration.PhotographyAvailableLight => IsHistoricalPhotography(configuration) ? "window-light capture" : "available-light capture",
                "On-Camera Flash" when configuration.PhotographyOnCameraFlash => IsHistoricalPhotography(configuration) ? "period flash exposure" : "direct flash",
                "Editorial Polish" when configuration.PhotographyEditorialPolish => IsHistoricalPhotography(configuration) ? "formal print refinement" : "editorial finish",
                "Raw Documentary Texture" when configuration.PhotographyRawDocumentaryTexture => "raw documentary texture",
                "Environmental Portrait Context" when configuration.PhotographyEnvironmentalPortraitContext => "environmental portrait context",
                "Film / Analog Character" when configuration.PhotographyFilmAnalogCharacter => IsHistoricalPhotography(configuration) ? "archival surface character" : "film stock character",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetPhotographyModifierPriority(string photographyType, string photographyEra)
    {
        var historical = string.Equals(photographyEra, "nineteenth-century-process", StringComparison.OrdinalIgnoreCase);

        return photographyType switch
        {
            "lifestyle-editorial" => historical
                ? ["Editorial Polish", "Available Light", "Environmental Portrait Context", "Candid Capture", "Raw Documentary Texture", "Posed / Staged Capture", "Film / Analog Character", "On-Camera Flash"]
                : ["Editorial Polish", "Available Light", "Candid Capture", "Environmental Portrait Context", "Posed / Staged Capture", "Raw Documentary Texture", "Film / Analog Character", "On-Camera Flash"],
            "documentary-street" => historical
                ? ["Candid Capture", "Raw Documentary Texture", "Available Light", "Environmental Portrait Context", "Posed / Staged Capture", "Editorial Polish", "Film / Analog Character", "On-Camera Flash"]
                : ["Candid Capture", "Raw Documentary Texture", "Available Light", "Environmental Portrait Context", "Film / Analog Character", "Posed / Staged Capture", "Editorial Polish", "On-Camera Flash"],
            "fine-art-photography" => historical
                ? ["Posed / Staged Capture", "Environmental Portrait Context", "Editorial Polish", "Available Light", "Raw Documentary Texture", "Candid Capture", "Film / Analog Character", "On-Camera Flash"]
                : ["Film / Analog Character", "Posed / Staged Capture", "Editorial Polish", "Available Light", "Raw Documentary Texture", "Environmental Portrait Context", "Candid Capture", "On-Camera Flash"],
            "commercial-photography" => historical
                ? ["Posed / Staged Capture", "Editorial Polish", "Available Light", "Environmental Portrait Context", "Raw Documentary Texture", "Candid Capture", "Film / Analog Character", "On-Camera Flash"]
                : ["Editorial Polish", "Posed / Staged Capture", "Available Light", "Environmental Portrait Context", "Film / Analog Character", "Candid Capture", "Raw Documentary Texture", "On-Camera Flash"],
            _ => historical
                ? ["Candid Capture", "Available Light", "Environmental Portrait Context", "Raw Documentary Texture", "Posed / Staged Capture", "Editorial Polish", "Film / Analog Character", "On-Camera Flash"]
                : ["Candid Capture", "Available Light", "Environmental Portrait Context", "Raw Documentary Texture", "Film / Analog Character", "Posed / Staged Capture", "Editorial Polish", "On-Camera Flash"],
        };
    }

    private static void AddPhotographyDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyPhotographyGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "immaculate plate finish";
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "hand-tinted period color";
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, MotionEnergy, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "sweeping exposure smear";
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, Tension, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "intense historical tension";
        }

        if (IsHistoricalPhotography(configuration) && string.Equals(sliderKey, Awe, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "expansive archival grandeur";
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return IsHistoricalPhotography(configuration) ? "hand-tinted period color" : "vivid color";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return IsHistoricalPhotography(configuration) ? "striking print contrast" : "striking tonal contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return IsHistoricalPhotography(configuration) ? "deep archival atmosphere" : "deep lens-mediated depth";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return IsHistoricalPhotography(configuration) ? "densely layered historical environment" : "dense environmental context";
        }

        return phrase;
    }

    private static bool IsHistoricalPhotography(PromptConfiguration configuration)
    {
        return string.Equals(configuration.PhotographyEra, "nineteenth-century-process", StringComparison.OrdinalIgnoreCase);
    }

    public static string ResolveProductPhotographyPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetProductPhotographyBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyProductPhotographyGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveProductPhotographyGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetProductPhotographyBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveProductPhotographyDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddProductPhotographyDescriptor(phrases, seen, "product photography");

        var shotTypeDescriptor = ResolveProductPhotographyShotTypeDescriptor(configuration.ProductPhotographyShotType);
        if (!string.IsNullOrWhiteSpace(shotTypeDescriptor))
        {
            AddProductPhotographyDescriptor(phrases, seen, shotTypeDescriptor);
        }

        foreach (var phrase in ResolveProductPhotographyModifierDescriptors(configuration))
        {
            AddProductPhotographyDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveProductPhotographyLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "clean studio daylight",
            "Golden hour" => "warm merchandising light",
            "Dramatic studio light" => "controlled studio light",
            "Overcast" => "diffused studio light",
            "Moonlit" => "cool showroom light",
            "Soft glow" => "soft display glow",
            "Dusk haze" => "late-day showcase light",
            "Warm directional light" => "directional product light",
            "Volumetric cinematic light" => "layered display light",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveProductPhotographyShotTypeDescriptor(string shotType)
    {
        return shotType switch
        {
            "packshot" => "studio packshot",
            "hero-studio" => "premium hero studio",
            "editorial-still-life" => "editorial still life",
            "macro-detail" => "macro product detail",
            "lifestyle-placement" => "lifestyle placement",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveProductPhotographyModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetProductPhotographyModifierPriority(configuration.ProductPhotographyShotType);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 3)
            {
                break;
            }

            var phrase = key switch
            {
                "With Packaging" when configuration.ProductPhotographyWithPackaging => "packaging in frame",
                "Pedestal Display" when configuration.ProductPhotographyPedestalDisplay => "pedestal display staging",
                "Reflective Surface" when configuration.ProductPhotographyReflectiveSurface => "reflective display surface",
                "Floating Presentation" when configuration.ProductPhotographyFloatingPresentation => "floating product presentation",
                "Hand Scale Cue" when configuration.ProductPhotographyScaleCueHand => "hand-in-frame scale cue",
                "Brand Props" when configuration.ProductPhotographyBrandProps => "brand-matched supporting props",
                "Grouped Variants" when configuration.ProductPhotographyGroupedVariants => "grouped variant arrangement",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetProductPhotographyModifierPriority(string shotType)
    {
        return shotType switch
        {
            "hero-studio" => ["Pedestal Display", "Reflective Surface", "With Packaging", "Floating Presentation", "Hand Scale Cue", "Brand Props", "Grouped Variants"],
            "editorial-still-life" => ["Brand Props", "Pedestal Display", "With Packaging", "Reflective Surface", "Grouped Variants", "Hand Scale Cue", "Floating Presentation"],
            "macro-detail" => ["Reflective Surface", "Hand Scale Cue", "With Packaging", "Pedestal Display", "Brand Props", "Grouped Variants", "Floating Presentation"],
            "lifestyle-placement" => ["Hand Scale Cue", "With Packaging", "Grouped Variants", "Brand Props", "Pedestal Display", "Reflective Surface", "Floating Presentation"],
            _ => ["With Packaging", "Pedestal Display", "Reflective Surface", "Floating Presentation", "Hand Scale Cue", "Brand Props", "Grouped Variants"],
        };
    }

    private static void AddProductPhotographyDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyProductPhotographyGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "immaculate product clarity";
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "vivid merchandise color";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking showcase contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "deep product depth";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return "densely layered retail surroundings";
        }

        if (string.Equals(sliderKey, MotionEnergy, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Framing <= 40)
        {
            return "dynamic placement energy";
        }

        return phrase;
    }

    private static string[] GetProductPhotographyBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var shotType = configuration.ProductPhotographyShotType;
        return sliderKey switch
        {
            Stylization => MapProductPhotographyStylization(shotType),
            Realism => ["clean merchandise read", "lightly interpreted product presence", "material fidelity", "high-fidelity merchandise realism", "deeply convincing sellable realism"],
            TextureDepth => ["smooth surface read", "faint tactile grain", "surface tactility", "rich material texture", "deep material body"],
            NarrativeDensity => shotType switch
            {
                "editorial-still-life" => ["low-story commercial simplicity", "light styling implication", "editorial prop styling", "layered editorial context", "dense editorial storytelling"],
                "lifestyle-placement" => ["low-story commercial simplicity", "light situational story", "contextual product-use placement", "layered use-case context", "dense commerce storytelling"],
                "macro-detail" => ["low-story commercial simplicity", "light material implication", "implied material story", "layered detail context", "dense material storytelling"],
                "hero-studio" => ["low-story commercial simplicity", "light display implication", "implied showcase story", "layered merchandising context", "dense premium storytelling"],
                _ => ["low-story commercial simplicity", "light product implication", "implied product story", "layered merchandising context", "dense commerce storytelling"],
            },
            Symbolism => shotType switch
            {
                "macro-detail" => ["minimal symbolic load", "restrained material cue", "suggestive texture motif", "pronounced material symbolism", "editorial material allegory"],
                "lifestyle-placement" => ["minimal symbolic load", "restrained lifestyle cue", "suggestive use-case motif", "pronounced merchandising symbolism", "editorial brand allegory"],
                _ => ["minimal symbolic load", "restrained brand cue", "suggestive merchandising motif", "pronounced brand symbolism", "editorial brand allegory"],
            },
            SurfaceAge => shotType switch
            {
                "hero-studio" => ["fresh hero surface", "faint handling wear", "gentle showroom patina", "noticeable display wear", "time-softened hero character"],
                "editorial-still-life" => ["fresh display surface", "faint shelf wear", "gentle handling patina", "noticeable retail patina", "time-softened still-life character"],
                "macro-detail" => ["fresh material surface", "faint edge wear", "gentle handling patina", "noticeable material wear", "time-softened detail character"],
                "lifestyle-placement" => ["fresh use surface", "faint handling wear", "gentle lived-in patina", "noticeable placement wear", "time-softened use character"],
                _ => ["fresh stock surface", "faint shelf wear", "gentle handling patina", "noticeable retail patina", "time-softened product character"],
            },
            Framing => shotType switch
            {
                "packshot" => ["centered product read", "clean product presentation", "copy-space discipline", "broader merchandising framing", "expansive catalog staging"],
                "hero-studio" => ["centered hero framing", "clean premium presentation", "premium display staging", "broader hero staging", "expansive hero staging"],
                "editorial-still-life" => ["centered object framing", "clean still-life crop", "measured editorial staging", "broader environmental framing", "expansive editorial staging"],
                "macro-detail" => ["tight detail crop", "close material presentation", "measured detail framing", "broader material framing", "expansive detail staging"],
                "lifestyle-placement" => ["centered product placement", "clean placement framing", "measured contextual placement", "broader selling context", "expansive placement staging"],
                _ => ["centered product read", "clean product presentation", "copy-space discipline", "broader merchandising framing", "expansive catalog staging"],
            },
            BackgroundComplexity => shotType switch
            {
                "packshot" => ["catalog isolation", "restrained backdrop", "supporting environment", "contextual scene detail", "densely layered retail surroundings"],
                "hero-studio" => ["catalog isolation", "restrained stage", "supporting display environment", "contextual merchandising detail", "densely layered showroom surroundings"],
                "editorial-still-life" => ["catalog isolation", "restrained prop field", "styled supporting environment", "contextual styling detail", "densely layered still-life surroundings"],
                "macro-detail" => ["catalog isolation", "restrained surface field", "supporting material environment", "contextual detail texture", "densely layered detail surroundings"],
                "lifestyle-placement" => ["catalog isolation", "restrained use setting", "supporting environment in service of selling the item", "contextual merchandising detail", "densely layered retail surroundings"],
                _ => ["catalog isolation", "restrained backdrop", "supporting environment", "contextual merchandising detail", "densely layered retail surroundings"],
            },
            MotionEnergy => shotType switch
            {
                "hero-studio" => ["still hero presentation", "slight handling trace", "active display energy", "candid merchandising motion", "split-second commercial energy"],
                "editorial-still-life" => ["still styled presentation", "slight placement trace", "active composition energy", "candid editorial motion", "split-second still-life energy"],
                "macro-detail" => ["still detail presentation", "slight handling trace", "active material energy", "candid detail motion", "split-second close-up energy"],
                "lifestyle-placement" => ["still use presentation", "slight handling trace", "active placement energy", "candid lifestyle motion", "split-second commercial energy"],
                _ => ["still product presentation", "slight handling trace", "active placement energy", "candid merchandising motion", "split-second commercial energy"],
            },
            FocusDepth => shotType switch
            {
                "packshot" => ["broad product clarity", "light focus falloff", "disciplined full-product clarity", "selective product isolation", "sharp subject separation"],
                "macro-detail" => ["broad detail clarity", "light focus falloff", "selective detail emphasis", "tight material isolation", "razor-held detail separation"],
                _ => ["broad product clarity", "light focus falloff", "measured focus depth", "selective product isolation", "sharp subject separation"],
            },
            ImageCleanliness => shotType switch
            {
                "packshot" => ["raw studio texture", "slight surface grit", "commerce polish", "controlled commercial polish", "immaculate product clarity"],
                _ => ["raw studio texture", "slight surface grit", "measured print finish", "commerce polish", "immaculate product clarity"],
            },
            DetailDensity => shotType switch
            {
                "macro-detail" => ["sparse product detail", "light observed detail", "dense visible material information", "rich visual detail", "dense fine detail"],
                _ => ["sparse product detail", "light observed detail", "clear merchandise detail load", "rich visual detail", "dense fine detail"],
            },
            AtmosphericDepth => shotType switch
            {
                "editorial-still-life" => ["flat display space", "slight air separation", "richer spatial air", "luminous still-life depth", "deep styling depth"],
                "lifestyle-placement" => ["flat use space", "slight air separation", "natural spatial recession", "luminous placement depth", "deep use-context depth"],
                "hero-studio" => ["flat showcase space", "slight air separation", "visible display recession", "luminous product depth", "deep showroom depth"],
                "macro-detail" => ["flat detail space", "slight air separation", "visible material recession", "luminous material depth", "deep detail depth"],
                _ => ["flat studio space", "slight air separation", "visible display recession", "luminous product depth", "deep lens-carried depth"],
            },
            Chaos => ["low-chaos presentation control", "quiet restlessness", "scene instability", "loose merchandising volatility", "orchestrated visual disorder"],
            Whimsy => shotType switch
            {
                "hero-studio" => ["serious tone", "subtle brand lift", "elevated merchandising play", "warm retail looseness", "gentle showcase playfulness"],
                "editorial-still-life" => ["serious tone", "subtle brand lift", "casual styling play", "warm editorial looseness", "gentle still-life playfulness"],
                "macro-detail" => ["serious tone", "subtle material lift", "casual detail play", "warm tactile looseness", "gentle detail playfulness"],
                "lifestyle-placement" => ["serious tone", "subtle brand lift", "casual lifestyle play", "warm retail looseness", "gentle editorial playfulness"],
                _ => ["serious tone", "subtle brand lift", "casual merchandising play", "warm retail looseness", "gentle editorial playfulness"],
            },
            Tension => shotType switch
            {
                "hero-studio" => ["quiet commercial focus", "light buyer anticipation", "noticeable shelf tension", "strong merchandising pressure", "intense commercial tension"],
                "editorial-still-life" => ["quiet editorial focus", "light viewer anticipation", "noticeable display tension", "strong styling pressure", "intense editorial tension"],
                "macro-detail" => ["quiet material focus", "light viewer anticipation", "noticeable material tension", "strong detail pressure", "intense material tension"],
                "lifestyle-placement" => ["quiet commerce focus", "light buyer anticipation", "noticeable use tension", "strong merchandising pressure", "intense commercial tension"],
                _ => ["quiet commercial focus", "light buyer anticipation", "noticeable shelf tension", "strong merchandising pressure", "intense commercial tension"],
            },
            Awe => shotType switch
            {
                "hero-studio" => ["human-scale grounding", "slight sense of value", "slightly elevated grandeur", "strong showcase scale", "expansive hero grandeur"],
                "editorial-still-life" => ["human-scale grounding", "slight sense of value", "quiet editorial presence", "strong styled scale", "expansive composition grandeur"],
                "macro-detail" => ["human-scale grounding", "slight sense of value", "quiet material presence", "strong detail scale", "expansive material grandeur"],
                "lifestyle-placement" => ["human-scale grounding", "slight sense of value", "quiet use presence", "strong placement scale", "expansive merchandising grandeur"],
                _ => ["human-scale grounding", "slight sense of value", "quiet premium presence", "strong showcase scale", "expansive hero grandeur"],
            },
            Temperature => ["cool studio balance", "slightly cool neutrality", "neutral studio balance", "warm retail balance", "heated showcase warmth"],
            LightingIntensity => shotType switch
            {
                "hero-studio" => ["dim studio light", "soft studio light", "controlled studio brightness", "stronger studio drama", "radiant showcase light"],
                _ => ["dim studio light", "soft studio light", "controlled studio brightness", "bright showroom light", "radiant showcase light"],
            },
            Saturation => ["muted color", "restrained commercial color", "natural color balance", "rich product color", "vivid merchandise color"],
            Contrast => shotType switch
            {
                "hero-studio" => ["low studio contrast", "gentle tonal separation", "contour separation", "crisp showcase contrast", "striking commercial contrast"],
                _ => ["low studio contrast", "gentle tonal separation", "contour separation", "crisp showcase contrast", "striking commercial contrast"],
            },
            CameraDistance => shotType switch
            {
                "macro-detail" => ["isolated close detail", "close material study", "tight material crop", "mid-distance merchandising view", "far-set display view"],
                "hero-studio" => ["isolated close packshot", "close product view", "mid-distance hero view", "wide showcase view", "far-set display view"],
                "lifestyle-placement" => ["isolated close placement", "close product view", "mid-distance merchandising view", "wide context view", "far-set display view"],
                _ => ["isolated close packshot", "close product view", "mid-distance merchandising view", "wide showcase view", "far-set display view"],
            },
            CameraAngle => ["eye-level view", "slightly lowered viewpoint", "balanced product angle", "slightly elevated showcase angle", "high merchandising vantage"],
            _ => Array.Empty<string>(),
        };
    }

    private static string[] MapProductPhotographyStylization(string shotType)
    {
        return shotType switch
        {
            "editorial-still-life" => ["unembellished object presentation", "lightly art-directed arrangement", "slightly more art direction", "premium editorial polish", "fully art-directed still-life presentation"],
            "macro-detail" => ["unembellished detail presentation", "lightly art-directed material study", "controlled commercial styling", "premium material polish", "fully art-directed detail presentation"],
            "lifestyle-placement" => ["unembellished use presentation", "lightly art-directed placement", "controlled lifestyle styling", "premium merchandising polish", "fully art-directed use-case presentation"],
            "hero-studio" => ["unembellished product presentation", "lightly art-directed merchandising", "controlled commercial styling", "premium merchandising polish", "fully art-directed showcase presentation"],
            _ => ["unembellished catalog presentation", "lightly art-directed merchandising", "controlled commercial styling", "premium merchandising polish", "fully art-directed showcase presentation"],
        };
    }

    public static string ResolveFoodPhotographyPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetFoodPhotographyBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyFoodPhotographyGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveFoodPhotographyGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetFoodPhotographyBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveFoodPhotographyDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddFoodPhotographyDescriptor(phrases, seen, ResolveFoodPhotographyCommercialAnchor(configuration.FoodPhotographyShotMode));

        var selectorDescriptor = ResolveFoodPhotographyShotModeDescriptor(configuration.FoodPhotographyShotMode);
        if (!string.IsNullOrWhiteSpace(selectorDescriptor))
        {
            AddFoodPhotographyDescriptor(phrases, seen, selectorDescriptor);
        }

        foreach (var phrase in ResolveFoodPhotographyModifierDescriptors(configuration))
        {
            AddFoodPhotographyDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveFoodPhotographyLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "soft appetizing daylight",
            "Golden hour" => "warm dining-hour light",
            "Dramatic studio light" => "controlled studio food light",
            "Overcast" => "diffused dining light",
            "Moonlit" => "cool evening service light",
            "Soft glow" => "soft hospitality glow",
            "Dusk haze" => "late-service ambient light",
            "Warm directional light" => "warm service light",
            "Volumetric cinematic light" => "layered restaurant glow",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveFoodPhotographyCommercialAnchor(string shotMode)
    {
        return shotMode switch
        {
            "tabletop-spread" => "commercial-grade tabletop food photography",
            "macro-detail" => "commercial-grade close food detail photography",
            "beverage-service" => "commercial-grade beverage service photography",
            "hospitality-campaign" => "commercial-grade hospitality food campaign photography",
            _ => "commercial-grade plated food photography",
        };
    }

    private static string ResolveFoodPhotographyShotModeDescriptor(string shotMode)
    {
        return shotMode switch
        {
            "tabletop-spread" => "multi-item table composition and supporting serving context",
            "macro-detail" => "close ingredient study and surface appetite detail",
            "beverage-service" => "glassware clarity and freshness cues",
            "hospitality-campaign" => "elevated service mood and richer hospitality context",
            _ => "hero dish emphasis and appetizing focal hierarchy",
        };
    }

    private static IEnumerable<string> ResolveFoodPhotographyModifierDescriptors(PromptConfiguration configuration)
    {
        var ordered = new (string Group, bool Enabled, string Phrase)[]
        {
            ("freshness-accents", configuration.FoodPhotographyVisibleSteam, "visible steam"),
            ("plating-accents", configuration.FoodPhotographyGarnishEmphasis, "garnish-forward finishing"),
            ("service-context", configuration.FoodPhotographyUtensilContext, "utensil context in frame"),
            ("service-context", configuration.FoodPhotographyHandServiceCue, "hand-led service cue"),
            ("plating-accents", configuration.FoodPhotographyIngredientScatter, "ingredient scatter styling"),
            ("freshness-accents", configuration.FoodPhotographyCondensationEmphasis, "condensation detail"),
        };

        var groupCaps = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["freshness-accents"] = 1,
            ["plating-accents"] = 2,
            ["service-context"] = 1,
        };
        var groupUsage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var selected = new List<string>();

        foreach (var entry in GetFoodPhotographyModifierPriority(configuration.FoodPhotographyShotMode)
                     .Join(ordered, key => key, item => item.Phrase, (_, item) => item))
        {
            if (!entry.Enabled || selected.Count >= 2)
            {
                continue;
            }

            groupUsage.TryGetValue(entry.Group, out var used);
            if (used >= groupCaps[entry.Group])
            {
                continue;
            }

            groupUsage[entry.Group] = used + 1;
            selected.Add(entry.Phrase);
        }

        return selected;
    }

    private static IReadOnlyList<string> GetFoodPhotographyModifierPriority(string shotMode)
    {
        return shotMode switch
        {
            "tabletop-spread" => ["ingredient scatter styling", "utensil context in frame", "garnish-forward finishing", "hand-led service cue", "visible steam", "condensation detail"],
            "macro-detail" => ["garnish-forward finishing", "ingredient scatter styling", "visible steam", "condensation detail", "utensil context in frame", "hand-led service cue"],
            "beverage-service" => ["condensation detail", "hand-led service cue", "utensil context in frame", "visible steam", "garnish-forward finishing", "ingredient scatter styling"],
            "hospitality-campaign" => ["hand-led service cue", "utensil context in frame", "garnish-forward finishing", "visible steam", "ingredient scatter styling", "condensation detail"],
            _ => ["garnish-forward finishing", "visible steam", "utensil context in frame", "ingredient scatter styling", "hand-led service cue", "condensation detail"],
        };
    }

    private static void AddFoodPhotographyDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyFoodPhotographyGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Realism, StringComparison.OrdinalIgnoreCase) && value >= 61)
        {
            return "appetizing realism";
        }

        if (string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "hospitality polish";
        }

        if (string.Equals(sliderKey, DetailDensity, StringComparison.OrdinalIgnoreCase)
            && value >= 61
            && string.Equals(configuration.FoodPhotographyShotMode, "macro-detail", StringComparison.OrdinalIgnoreCase))
        {
            return "dense ingredient definition";
        }

        if (string.Equals(configuration.FoodPhotographyShotMode, "beverage-service", StringComparison.OrdinalIgnoreCase)
            && string.Equals(sliderKey, LightingIntensity, StringComparison.OrdinalIgnoreCase)
            && value >= 61)
        {
            return "glass-safe service brightness";
        }

        return phrase;
    }

    private static string[] GetFoodPhotographyBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var shotMode = configuration.FoodPhotographyShotMode;
        return sliderKey switch
        {
            Stylization => shotMode switch
            {
                "tabletop-spread" => ["restrained menu presentation", "lightly styled tabletop treatment", "measured service styling", "polished hospitality styling", "fully art-directed tabletop campaign"],
                "macro-detail" => ["restrained detail presentation", "lightly styled ingredient study", "measured close-up styling", "polished material styling", "fully art-directed detail campaign"],
                "beverage-service" => ["restrained service presentation", "lightly styled beverage service", "measured bar-service styling", "polished hospitality styling", "fully art-directed beverage campaign"],
                "hospitality-campaign" => ["restrained menu presentation", "lightly styled restaurant presentation", "measured campaign styling", "polished hospitality styling", "fully art-directed restaurant campaign"],
                _ => ["restrained hero plating", "lightly styled dish presentation", "measured plating treatment", "polished menu styling", "fully art-directed hero plating"],
            },
            Realism => ["clean edible read", "lightly interpreted edible surfaces", "believable edible surfaces", "high-fidelity appetizing realism", "deeply convincing edible realism"],
            TextureDepth => shotMode switch
            {
                "beverage-service" => ["smooth glass read", "faint chilled texture", "froth and glass texture", "rich chilled surface texture", "deep specular drink texture"],
                "macro-detail" => ["smooth ingredient read", "faint tactile grain", "surface appetite detail", "rich crumb-and-glaze texture", "deep close-up texture body"],
                _ => ["smooth plated read", "faint tactile grain", "ingredient definition", "rich crust-and-sauce texture", "deep plated texture body"],
            },
            NarrativeDensity => shotMode switch
            {
                "tabletop-spread" => ["light service context only", "subtle table activity", "supporting serving context", "layered tabletop service cues", "dense hospitality storytelling"],
                "beverage-service" => ["light service context only", "subtle pour implication", "served drink presentation", "layered bar-service context", "dense hospitality storytelling"],
                "hospitality-campaign" => ["light service context only", "subtle restaurant activity", "elevated service mood", "layered hospitality context", "dense campaign storytelling"],
                _ => ["light service context only", "subtle plating implication", "menu-ready presentation", "layered service cues", "dense hospitality storytelling"],
            },
            Symbolism => ["minimal symbolic load", "restrained styling cue", "suggestive service motif", "pronounced presentation symbolism", "editorial hospitality allegory"],
            SurfaceAge => shotMode switch
            {
                "beverage-service" => ["fresh chilled surface", "faint service wear", "gentle glass handling trace", "noticeable service patina", "time-softened barware character"],
                _ => ["fresh plated surface", "faint service wear", "gentle table handling trace", "noticeable service patina", "time-softened hospitality character"],
            },
            Framing => shotMode switch
            {
                "tabletop-spread" => ["tight tabletop crop", "wider tabletop read", "service-aware composition", "broader shared-table framing", "expansive tabletop staging"],
                "macro-detail" => ["tight appetite crop", "close detail crop", "surface-first composition", "broader ingredient framing", "expansive detail staging"],
                "beverage-service" => ["tight drink crop", "close service framing", "glassware-led composition", "broader service framing", "expansive beverage staging"],
                "hospitality-campaign" => ["tight hero crop", "clean campaign framing", "service-aware composition", "broader hospitality framing", "expansive campaign staging"],
                _ => ["tight plate crop", "clean hero plating", "appetizing plating clarity", "broader menu framing", "expansive plated staging"],
            },
            BackgroundComplexity => shotMode switch
            {
                "tabletop-spread" => ["table context restraint", "restrained service setting", "supporting serving context", "richer tabletop environment", "densely layered dining surroundings"],
                "macro-detail" => ["table context restraint", "restrained surface field", "supporting ingredient context", "richer close-up backdrop load", "densely layered detail surroundings"],
                "beverage-service" => ["table context restraint", "restrained bar setting", "service environment support", "richer beverage backdrop load", "densely layered hospitality surroundings"],
                "hospitality-campaign" => ["table context restraint", "restrained restaurant setting", "service environment support", "campaign backdrop load", "densely layered restaurant surroundings"],
                _ => ["table context restraint", "restrained dining field", "supporting table context", "richer plated backdrop load", "densely layered dining surroundings"],
            },
            MotionEnergy => shotMode switch
            {
                "beverage-service" => ["still served moment", "slight pour trace", "active service motion", "hand-led service motion", "split-second service energy"],
                "tabletop-spread" => ["still shared-table moment", "slight serving trace", "active tabletop motion", "candid service motion", "split-second dining energy"],
                _ => ["still plated moment", "slight service trace", "active plating motion", "candid kitchen-to-table motion", "split-second appetizing motion"],
            },
            FocusDepth => shotMode switch
            {
                "macro-detail" => ["broad detail clarity", "shallow focus control", "selective detail emphasis", "tight macro isolation", "razor-held ingredient separation"],
                "tabletop-spread" => ["broad table clarity", "light focus falloff", "measured table focus", "selective spread emphasis", "tight service isolation"],
                _ => ["broad hero clarity", "light focus falloff", "measured plate focus", "selective plating isolation", "sharp hero separation"],
            },
            ImageCleanliness => ["raw kitchen texture", "slight surface grit", "styling cleanliness", "menu polish", "hospitality polish"],
            DetailDensity => shotMode switch
            {
                "macro-detail" => ["sparse visible detail", "light ingredient detail", "dense ingredient definition", "rich material detail", "dense fine appetite detail"],
                "beverage-service" => ["sparse visible detail", "light glass detail", "visible ingredient detail load", "rich service detail", "dense chilled detail load"],
                _ => ["sparse visible detail", "light ingredient detail", "visible ingredient detail load", "rich plated surface detail", "dense menu detail load"],
            },
            AtmosphericDepth => shotMode switch
            {
                "tabletop-spread" => ["flat tabletop space", "shallow tabletop recession", "soft restaurant air", "luminous table separation", "deep dining-room recession"],
                "beverage-service" => ["flat service space", "shallow service recession", "service-space separation", "luminous bar depth", "deep hospitality recession"],
                "hospitality-campaign" => ["flat campaign space", "slight room recession", "soft restaurant air", "luminous hospitality depth", "deep campaign recession"],
                _ => ["flat plated space", "shallow tabletop recession", "soft service-space separation", "luminous plate depth", "deep dining-room recession"],
            },
            Chaos => ["styling control", "quiet restlessness", "light plating disorder", "loose service clutter", "orchestrated table disorder"],
            Whimsy => shotMode switch
            {
                "plated-hero" => [string.Empty, "subtle warmth", "casual hospitality lift", "warm social play", "gentle restaurant playfulness"],
                _ => ["serious service tone", "subtle warmth", "casual hospitality lift", "warm social play", "gentle restaurant playfulness"],
            },
            Tension => ["calm service tone", "faint service urgency", "light kitchen-to-table tension", "noticeable rush-hour pressure", "intense service pressure"],
            Awe => shotMode switch
            {
                "plated-hero" => [string.Empty, "slight sense of appetite", "quiet plated appeal", "strong menu allure", "expansive hospitality allure"],
                "hospitality-campaign" => ["human-scale grounding", "slight sense of indulgence", "elevated dining presence", "strong hospitality allure", "expansive campaign allure"],
                "beverage-service" => ["human-scale grounding", "slight sense of refreshment", "quiet service appeal", "strong poured allure", "expansive beverage allure"],
                _ => ["human-scale grounding", "slight sense of appetite", "quiet plated appeal", "strong menu allure", "expansive hospitality allure"],
            },
            Temperature => shotMode switch
            {
                "plated-hero" => ["cool plate balance", "slightly cool neutrality", "neutral daylight balance", "warm appetizing balance", "heated dining warmth"],
                _ => ["cool plate balance", "slightly cool neutrality", "neutral service balance", "warm appetizing light", "heated dining warmth"],
            },
            LightingIntensity => shotMode switch
            {
                "beverage-service" => ["dim service light", "soft service light", "highlight-controlled brightness", "glass-safe service brightness", "radiant chilled highlights"],
                "plated-hero" => ["dim plating light", "soft plating light", "appetizing brightness", "controlled highlight brightness", "radiant plated highlights"],
                _ => ["dim plating light", "soft plating light", "appetizing brightness", "bright menu light", "radiant hospitality light"],
            },
            Saturation => ["muted color", "restrained commercial color", "natural edible color", "rich menu color", "vivid plated color"],
            Contrast => shotMode switch
            {
                "beverage-service" => ["low service contrast", "gentle edge separation", "glass and garnish definition", "crisp specular separation", "striking chilled contrast"],
                _ => ["low plating contrast", "gentle edge separation", "plating edge clarity", "crisp contour separation", "striking sauce-and-crust contrast"],
            },
            CameraDistance => shotMode switch
            {
                "tabletop-spread" => ["close place-setting crop", "shared-table read", "mid-distance tabletop read", "wide dining spread", "far-set service overview"],
                "macro-detail" => ["extreme macro intimacy", "close ingredient study", "tight texture read", "mid-distance detail read", "far-set garnish overview"],
                "beverage-service" => ["close glass study", "served-drink read", "mid-distance service read", "wide bar-table read", "far-set beverage overview"],
                _ => ["hero close-up", "close plated read", "mid-distance plated read", "wide table setting", "far-set dining overview"],
            },
            CameraAngle => shotMode switch
            {
                "tabletop-spread" => ["slight dining angle", "level table view", "overhead-adjacent spread bias", "higher tabletop vantage", "near-overhead table overview"],
                "macro-detail" => ["level detail read", "slightly lowered detail angle", "close material angle", "slightly elevated macro vantage", "high garnish vantage"],
                "beverage-service" => ["level glass read", "slightly lowered bar angle", "service-level angle", "slightly elevated service vantage", "high beverage vantage"],
                _ => ["level plated read", "slightly lowered dining angle", "balanced plate angle", "slightly elevated service angle", "high plated vantage"],
            },
            _ => Array.Empty<string>(),
        };
    }

    public static string ResolveArchitectureArchvizPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetArchitectureArchvizBandLabels(sliderKey, configuration);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyArchitectureArchvizGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveArchitectureArchvizGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetArchitectureArchvizBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveArchitectureArchvizDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddArchitectureArchvizDescriptor(phrases, seen, ResolveArchitectureArchvizCommercialAnchor(configuration.ArchitectureArchvizViewMode));

        var selectorDescriptor = ResolveArchitectureArchvizViewModeDescriptor(configuration.ArchitectureArchvizViewMode);
        if (!string.IsNullOrWhiteSpace(selectorDescriptor))
        {
            AddArchitectureArchvizDescriptor(phrases, seen, selectorDescriptor);
        }

        foreach (var phrase in ResolveArchitectureArchvizModifierDescriptors(configuration))
        {
            AddArchitectureArchvizDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveArchitectureArchvizLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "clean daylighted presentation",
            "Golden hour" => "warm facade light",
            "Dramatic studio light" => "controlled marketing light",
            "Overcast" => "neutral overcast daylight",
            "Moonlit" => "cool evening site light",
            "Soft glow" => "soft lobby glow",
            "Dusk haze" => "twilight ambient haze",
            "Warm directional light" => "warm directional glazing light",
            "Volumetric cinematic light" => "layered daylight shafts",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveArchitectureArchvizCommercialAnchor(string viewMode)
    {
        return viewMode switch
        {
            "interior" => "commercial-grade architectural interior visualization",
            "streetscape" => "commercial-grade architectural streetscape visualization",
            "aerial-masterplan" => "commercial-grade aerial masterplan visualization",
            "twilight-marketing" => "commercial-grade twilight architectural marketing render",
            _ => "commercial-grade architectural exterior visualization",
        };
    }

    private static string ResolveArchitectureArchvizViewModeDescriptor(string viewMode)
    {
        return viewMode switch
        {
            "interior" => "room proportion and circulation clarity",
            "streetscape" => "frontage rhythm and sidewalk edge clarity",
            "aerial-masterplan" => "massing hierarchy and district context",
            "twilight-marketing" => "warm interior glow and premium ambient contrast",
            _ => "facade articulation and skyline recession",
        };
    }

    private static IEnumerable<string> ResolveArchitectureArchvizModifierDescriptors(PromptConfiguration configuration)
    {
        var ordered = new (string Group, bool Enabled, string Phrase)[]
        {
            ("occupancy-context", configuration.ArchitectureArchvizHumanScaleCues, "human scale cues"),
            ("site-accents", configuration.ArchitectureArchvizLandscapeEmphasis, "landscape-forward site presentation"),
            ("interior-accents", configuration.ArchitectureArchvizFurnishingEmphasis, "furnishing-forward interior styling"),
            ("lighting-accents", configuration.ArchitectureArchvizWarmInteriorGlow, "warm interior glow"),
            ("site-accents", configuration.ArchitectureArchvizReflectiveSurfaceAccents, "reflective surface accents"),
            ("marketing-accents", configuration.ArchitectureArchvizAmenityFocus, "amenity-led presentation"),
        };

        var groupCaps = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["occupancy-context"] = 1,
            ["site-accents"] = 2,
            ["interior-accents"] = 1,
            ["lighting-accents"] = 1,
            ["marketing-accents"] = 1,
        };
        var groupUsage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        var selected = new List<string>();

        foreach (var entry in GetArchitectureArchvizModifierPriority(configuration.ArchitectureArchvizViewMode)
                     .Join(ordered, key => key, item => item.Phrase, (_, item) => item))
        {
            if (selected.Count >= 2 || !entry.Enabled)
            {
                continue;
            }

            groupUsage.TryGetValue(entry.Group, out var used);
            if (used >= groupCaps[entry.Group])
            {
                continue;
            }

            groupUsage[entry.Group] = used + 1;
            selected.Add(entry.Phrase);
        }

        return selected;
    }

    private static IReadOnlyList<string> GetArchitectureArchvizModifierPriority(string viewMode)
    {
        return viewMode switch
        {
            "interior" => ["furnishing-forward interior styling", "warm interior glow", "human scale cues", "amenity-led presentation", "reflective surface accents", "landscape-forward site presentation"],
            "streetscape" => ["human scale cues", "landscape-forward site presentation", "amenity-led presentation", "reflective surface accents", "warm interior glow", "furnishing-forward interior styling"],
            "aerial-masterplan" => ["amenity-led presentation", "landscape-forward site presentation", "human scale cues", "reflective surface accents", "warm interior glow", "furnishing-forward interior styling"],
            "twilight-marketing" => ["warm interior glow", "reflective surface accents", "amenity-led presentation", "human scale cues", "landscape-forward site presentation", "furnishing-forward interior styling"],
            _ => ["landscape-forward site presentation", "human scale cues", "reflective surface accents", "amenity-led presentation", "warm interior glow", "furnishing-forward interior styling"],
        };
    }

    private static void AddArchitectureArchvizDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyArchitectureArchvizGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(configuration.ArchitectureArchvizViewMode, "twilight-marketing", StringComparison.OrdinalIgnoreCase)
            && string.Equals(sliderKey, LightingIntensity, StringComparison.OrdinalIgnoreCase)
            && value >= 61)
        {
            return "evening glow strength";
        }

        if (string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81)
        {
            return "clean sales-render finish";
        }

        if (string.Equals(sliderKey, Realism, StringComparison.OrdinalIgnoreCase) && value >= 61)
        {
            return "material realism";
        }

        return phrase;
    }

    private static string[] GetArchitectureArchvizBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        var viewMode = configuration.ArchitectureArchvizViewMode;
        return sliderKey switch
        {
            Stylization => viewMode switch
            {
                "interior" => ["restrained design treatment", "lightly styled room presentation", "measured design authorship", "polished development styling", "fully art-directed sales rendering"],
                "streetscape" => ["restrained frontage treatment", "lightly styled urban presentation", "measured marketing authorship", "polished district styling", "fully art-directed development rendering"],
                "aerial-masterplan" => ["restrained planning treatment", "lightly styled site presentation", "measured masterplan authorship", "polished district styling", "fully art-directed planning render"],
                "twilight-marketing" => ["restrained dusk treatment", "lightly styled evening presentation", "measured marketing authorship", "polished twilight styling", "fully art-directed premium marketing render"],
                _ => ["restrained facade treatment", "lightly styled exterior presentation", "measured development authorship", "polished marketing styling", "fully art-directed exterior rendering"],
            },
            Realism => ["clean built-form read", "lightly interpreted built presence", "material realism", "high-fidelity built-space realism", "deeply convincing development realism"],
            TextureDepth => viewMode switch
            {
                "interior" => ["smooth finish read", "faint finish texture", "finish continuity", "rich surface articulation", "deep junction clarity"],
                _ => ["smooth facade read", "faint material texture", "surface articulation", "rich finish texture", "deep junction clarity"],
            },
            NarrativeDensity => viewMode switch
            {
                "interior" => ["low-story occupancy restraint", "light use implication", "light occupancy cues", "layered lived-in context", "dense hospitality storytelling"],
                "streetscape" => ["low-story urban restraint", "light pedestrian implication", "light public-use cues", "layered frontage context", "dense district storytelling"],
                "aerial-masterplan" => ["low-story planning restraint", "light district implication", "light program cues", "layered planning context", "dense development storytelling"],
                "twilight-marketing" => ["low-story marketing restraint", "light arrival implication", "light hospitality cues", "layered evening context", "dense premium storytelling"],
                _ => ["low-story presentation restraint", "light occupancy implication", "light use cues", "layered sales context", "dense development storytelling"],
            },
            Symbolism => ["minimal symbolic load", "restrained brand cue", "suggestive spatial motif", "pronounced ceremonial symbolism", "premium sales allegory"],
            SurfaceAge => viewMode switch
            {
                "interior" => ["fresh fit-out finish", "faint lived-in wear", "gentle finish aging", "noticeable patina", "time-softened interior character"],
                _ => ["fresh facade finish", "faint weathering", "gentle material aging", "noticeable patina", "time-softened built character"],
            },
            Framing => viewMode switch
            {
                "interior" => ["room-focused crop", "clean room composition", "circulation clarity", "broader room staging", "expansive showroom composition"],
                "streetscape" => ["tight frontage crop", "clean street elevation", "frontage rhythm", "broader pedestrian frontage", "expansive boulevard staging"],
                "aerial-masterplan" => ["tight site crop", "clean site overview", "massing hierarchy", "broader district framing", "expansive planning tableau"],
                "twilight-marketing" => ["premium dusk crop", "clean arrival framing", "presentation crop", "broader premium staging", "expansive marketing tableau"],
                _ => ["facade-focused crop", "clean elevation framing", "spatial legibility", "broader approach framing", "expansive exterior staging"],
            },
            BackgroundComplexity => viewMode switch
            {
                "interior" => ["minimal furnishing load", "restrained furnishing support", "finish continuity", "richly staged support", "densely layered room context"],
                "streetscape" => ["minimal street load", "restrained sidewalk context", "pedestrian context", "rich district context", "densely layered street context"],
                "aerial-masterplan" => ["minimal site load", "restrained district context", "site context", "rich planning context", "densely layered district context"],
                "twilight-marketing" => ["minimal arrival load", "restrained site context", "site context", "rich hospitality context", "densely layered premium context"],
                _ => ["minimal site load", "restrained planted approach", "site context", "richly situated context", "densely layered skyline context"],
            },
            MotionEnergy => ["still presentation control", "faint occupancy trace", "light movement cue", "loose pedestrian activity", "busy circulation flow"],
            FocusDepth => viewMode switch
            {
                "interior" => ["broad room clarity", "light focus falloff", "finish continuity", "selective material focus", "razor-held room clarity"],
                "aerial-masterplan" => ["broad site clarity", "light focus falloff", "district legibility", "selective site emphasis", "razor-held masterplan clarity"],
                _ => ["broad facade clarity", "light focus falloff", "spatial legibility", "selective material emphasis", "razor-held frontage clarity"],
            },
            ImageCleanliness => ["raw site texture", "slight presentation grit", "development polish", "controlled marketing polish", "clean sales-render finish"],
            DetailDensity => viewMode switch
            {
                "interior" => ["sparse room detail", "light finish detail", "clear interior detail load", "rich finish detail load", "dense interior detail load"],
                "aerial-masterplan" => ["sparse site detail", "light planning detail", "clear district detail load", "rich site detail load", "dense masterplan detail load"],
                _ => ["sparse facade detail", "light material detail", "clear facade detail load", "rich material detail load", "dense facade detail load"],
            },
            AtmosphericDepth => viewMode switch
            {
                "interior" => ["flat room space", "slight tonal recession", "daylighted volume", "luminous room recession", "deep interior recession"],
                "streetscape" => ["flat street space", "slight tonal recession", "pedestrian-depth layering", "luminous block recession", "deep skyline separation"],
                "aerial-masterplan" => ["flat site field", "slight atmospheric lift", "district recession", "luminous site hierarchy", "deep aerial separation"],
                "twilight-marketing" => ["flat dusk space", "slight ambient recession", "premium ambient contrast", "luminous evening recession", "deep twilight atmosphere"],
                _ => ["flat exterior space", "slight atmospheric recession", "skyline recession", "daylighted volume", "deep skyline separation"],
            },
            Chaos => ["low-chaos presentation control", "quiet asymmetry", "light layout instability", "loose staging disorder", "orchestrated spatial turbulence"],
            Whimsy => ["serious presentation tone", "subtle warmth", "light design play", "warm hospitality looseness", "gentle showcase playfulness"],
            Tension => ["quiet witness tone", "faint market tension", "light presentation pressure", "strong premium pressure", "intense marketing tension"],
            Awe => viewMode switch
            {
                "aerial-masterplan" => ["site-scale grounding", "slight district lift", "quiet planning awe", "strong territorial scale", "expansive masterplan grandeur"],
                "twilight-marketing" => ["human-scale grounding", "slight presence lift", "quiet market grandeur", "strong premium scale", "expansive development grandeur"],
                _ => ["human-scale grounding", "slight presence lift", "quiet spatial wonder", "strong felt scale", "expansive development grandeur"],
            },
            Temperature => viewMode switch
            {
                "twilight-marketing" => ["cool dusk balance", "slightly cool evening neutrality", "balanced evening toning", "warm interior bias", "deep hospitality warmth"],
                _ => ["cool daylight balance", "slightly cool neutrality", "neutral daylight balance", "warm natural light", "heated warm cast"],
            },
            LightingIntensity => viewMode switch
            {
                "twilight-marketing" => ["dim evening light", "soft dusk light", "balanced evening glow", "strong evening glow strength", "radiant hospitality glow"],
                _ => ["dim daylight", "soft daylight", "daylight balance", "controlled studio brightness", "radiant daylight volume"],
            },
            Saturation => ["muted material color", "restrained commercial color", "balanced material color", "rich finish color", "vivid amenity color"],
            Contrast => ["low edge contrast", "gentle contour separation", "contour separation", "crisp glazing definition", "striking edge clarity"],
            CameraDistance => viewMode switch
            {
                "interior" => ["close room read", "intimate room proportion", "room proportion", "broad room overview", "far-set room overview"],
                "streetscape" => ["close frontage read", "pedestrian eye-line read", "block-length read", "broad streetscape overview", "far-set district overview"],
                "aerial-masterplan" => ["close site read", "elevated site overview", "aerial overview distance", "broad district overview", "far-set territorial overview"],
                _ => ["close facade read", "approach-distance read", "massing read", "broad exterior overview", "far-set site overview"],
            },
            CameraAngle => viewMode switch
            {
                "interior" => ["eye-level room view", "slightly lowered human viewpoint", "level room view", "slightly elevated interior vantage", "high overview vantage"],
                "streetscape" => ["pedestrian eye line", "slightly lowered street viewpoint", "level frontage", "slightly elevated block vantage", "high streetscape vantage"],
                "aerial-masterplan" => ["shallow oblique overview", "elevated site view", "elevated plan-view bias", "high site overview", "high detached plan view"],
                _ => ["level frontage", "slightly lowered approach view", "eye-level facade view", "slightly elevated site vantage", "high overview vantage"],
            },
            _ => Array.Empty<string>(),
        };
    }

    public static IEnumerable<string> ResolveThreeDRenderDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddThreeDRenderDescriptor(phrases, seen, "3D render");

        var subtypeDescriptor = ResolveThreeDRenderSubtypeDescriptor(configuration.ThreeDRenderSubtype);
        if (!string.IsNullOrWhiteSpace(subtypeDescriptor))
        {
            AddThreeDRenderDescriptor(phrases, seen, subtypeDescriptor);
        }

        foreach (var phrase in ResolveThreeDRenderModifierDescriptors(configuration))
        {
            AddThreeDRenderDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveThreeDRenderLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "soft rendered daylight",
            "Golden hour" => "commercial golden-hour render light",
            "Dramatic studio light" => "studio render lighting",
            "Overcast" => "diffuse studio daylight",
            "Moonlit" => "cool atmospheric render light",
            "Soft glow" => "soft rendered glow",
            "Dusk haze" => "neon-lit render atmosphere",
            "Warm directional light" => "commercial product lighting",
            "Volumetric cinematic light" => "volumetric render lighting",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveThreeDRenderSubtypeDescriptor(string threeDRenderSubtype)
    {
        return threeDRenderSubtype switch
        {
            "general-cgi" => "clean CGI presentation",
            "stylized-3d" => "designed digital shaping",
            "photoreal-3d" => "photoreal material realism",
            "game-asset" => "game-ready presentation clarity",
            "animated-feature" => "feature-animation polish",
            "product-visualization" => "studio-grade product presentation",
            "sci-fi-hard-surface" => "engineered hard-surface precision",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveThreeDRenderModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetThreeDRenderModifierPriority(configuration.ThreeDRenderSubtype);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Global Illumination" when configuration.ThreeDRenderGlobalIllumination => "global-illumination light bounce",
                "Volumetric Lighting" when configuration.ThreeDRenderVolumetricLighting => "volumetric light shafts",
                "Ray-Traced Reflections" when configuration.ThreeDRenderRayTracedReflections => "ray-traced reflection fidelity",
                "Depth of Field" when configuration.ThreeDRenderDepthOfField => "cinematic depth of field",
                "Subsurface Scattering" when configuration.ThreeDRenderSubsurfaceScattering => "subsurface light transmission",
                "Hard-Surface Precision" when configuration.ThreeDRenderHardSurfacePrecision => "hard-surface edge precision",
                "Studio Backdrop" when configuration.ThreeDRenderStudioBackdrop => "studio-backdrop presentation",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetThreeDRenderModifierPriority(string threeDRenderSubtype)
    {
        return threeDRenderSubtype switch
        {
            "stylized-3d" => ["Global Illumination", "Depth of Field", "Studio Backdrop", "Volumetric Lighting", "Ray-Traced Reflections", "Subsurface Scattering", "Hard-Surface Precision"],
            "photoreal-3d" => ["Ray-Traced Reflections", "Global Illumination", "Subsurface Scattering", "Depth of Field", "Volumetric Lighting", "Hard-Surface Precision", "Studio Backdrop"],
            "game-asset" => ["Hard-Surface Precision", "Studio Backdrop", "Depth of Field", "Global Illumination", "Ray-Traced Reflections", "Subsurface Scattering", "Volumetric Lighting"],
            "animated-feature" => ["Subsurface Scattering", "Global Illumination", "Depth of Field", "Studio Backdrop", "Volumetric Lighting", "Ray-Traced Reflections", "Hard-Surface Precision"],
            "product-visualization" => ["Studio Backdrop", "Ray-Traced Reflections", "Global Illumination", "Depth of Field", "Subsurface Scattering", "Volumetric Lighting", "Hard-Surface Precision"],
            "sci-fi-hard-surface" => ["Hard-Surface Precision", "Ray-Traced Reflections", "Volumetric Lighting", "Global Illumination", "Depth of Field", "Subsurface Scattering", "Studio Backdrop"],
            _ => ["Global Illumination", "Depth of Field", "Volumetric Lighting", "Ray-Traced Reflections", "Subsurface Scattering", "Hard-Surface Precision", "Studio Backdrop"],
        };
    }

    private static void AddThreeDRenderDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    public static IEnumerable<string> ResolveConceptArtDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddConceptArtDescriptor(phrases, seen, "concept art presentation");

        var subtypeDescriptor = ResolveConceptArtSubtypeDescriptor(configuration.ConceptArtSubtype);
        if (!string.IsNullOrWhiteSpace(subtypeDescriptor))
        {
            AddConceptArtDescriptor(phrases, seen, subtypeDescriptor);
        }

        foreach (var phrase in ResolveConceptArtModifierDescriptors(configuration))
        {
            AddConceptArtDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveConceptArtLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "soft concept reference light",
            "Golden hour" => "golden-hour presentation light",
            "Dramatic studio light" => "studio concept lighting",
            "Overcast" => "soft neutral reference light",
            "Moonlit" => "moody atmospheric design light",
            "Soft glow" => "subtle presentation glow",
            "Dusk haze" => "late-day atmospheric reference light",
            "Warm directional light" => "warm directional presentation light",
            "Volumetric cinematic light" => "atmospheric production lighting",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveConceptArtSubtypeDescriptor(string conceptArtSubtype)
    {
        return conceptArtSubtype switch
        {
            "keyframe-concept" => "keyframe-style scene presentation",
            "environment-concept" => "environment-design presentation",
            "character-concept" => "character-design presentation",
            "creature-concept" => "creature-design presentation",
            "costume-concept" => "costume-design presentation",
            "prop-concept" => "prop-design presentation",
            "vehicle-concept" => "vehicle-design presentation",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveConceptArtModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetConceptArtModifierPriority(configuration.ConceptArtSubtype);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Design Callouts" when configuration.ConceptArtDesignCallouts => "design-callout presentation",
                "Turnaround Readability" when configuration.ConceptArtTurnaroundReadability => "turnaround-ready readability",
                "Material Breakdown" when configuration.ConceptArtMaterialBreakdown => "material-breakdown clarity",
                "Scale Reference" when configuration.ConceptArtScaleReference => "clear scale reference",
                "Worldbuilding Accents" when configuration.ConceptArtWorldbuildingAccents => "worldbuilding support accents",
                "Production Notes Feel" when configuration.ConceptArtProductionNotesFeel => "production-board note energy",
                "Silhouette Clarity" when configuration.ConceptArtSilhouetteClarity => "strong silhouette clarity",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetConceptArtModifierPriority(string conceptArtSubtype)
    {
        return conceptArtSubtype switch
        {
            "environment-concept" => ["Worldbuilding Accents", "Scale Reference", "Design Callouts", "Production Notes Feel", "Material Breakdown", "Turnaround Readability", "Silhouette Clarity"],
            "character-concept" => ["Silhouette Clarity", "Turnaround Readability", "Material Breakdown", "Design Callouts", "Scale Reference", "Worldbuilding Accents", "Production Notes Feel"],
            "creature-concept" => ["Silhouette Clarity", "Material Breakdown", "Scale Reference", "Design Callouts", "Turnaround Readability", "Worldbuilding Accents", "Production Notes Feel"],
            "costume-concept" => ["Material Breakdown", "Turnaround Readability", "Silhouette Clarity", "Design Callouts", "Scale Reference", "Worldbuilding Accents", "Production Notes Feel"],
            "prop-concept" => ["Design Callouts", "Material Breakdown", "Turnaround Readability", "Scale Reference", "Silhouette Clarity", "Worldbuilding Accents", "Production Notes Feel"],
            "vehicle-concept" => ["Design Callouts", "Scale Reference", "Silhouette Clarity", "Material Breakdown", "Turnaround Readability", "Worldbuilding Accents", "Production Notes Feel"],
            "keyframe-concept" => ["Worldbuilding Accents", "Scale Reference", "Silhouette Clarity", "Design Callouts", "Production Notes Feel", "Material Breakdown", "Turnaround Readability"],
            _ => ["Worldbuilding Accents", "Scale Reference", "Silhouette Clarity", "Design Callouts", "Production Notes Feel", "Material Breakdown", "Turnaround Readability"],
        };
    }

    private static void AddConceptArtDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyThreeDRenderGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Realism >= 60)
        {
            return "striking render contrast";
        }

        if (string.Equals(sliderKey, ImageCleanliness, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.DetailDensity >= 60)
        {
            return "ultra-clean commercial finish";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.DetailDensity <= 40)
        {
            return "densely layered rendered environment";
        }

        if (string.Equals(sliderKey, DetailDensity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "high-density production detail";
        }

        return phrase;
    }

    private static string ApplyConceptArtGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Stylization, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.ImageCleanliness >= 61)
        {
            return "highly stylized portfolio presentation";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity >= 61)
        {
            return "densely layered concept environment";
        }

        if (string.Equals(sliderKey, Symbolism, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity <= 40)
        {
            return "mythic symbolic worldbuilding";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.ImageCleanliness >= 61)
        {
            return "striking design contrast";
        }

        return phrase;
    }

    private static string ApplyThreeDRenderPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("3D render ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("3D-render ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("3D ", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }

    private static string ApplyConceptArtPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("concept-art ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("concept art ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("concept-art", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("concept art", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }

    public static string ResolvePixelArtPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var phrase = sliderKey switch
        {
            Stylization => MapBand(value,
                "grounded pixel treatment",
                "light pixel stylization",
                "stylized pixel rendering",
                "strong pixel-art stylization",
                "highly stylized pixel-art language"),
            Realism => MapBand(value,
                string.Empty,
                "loosely observed pixel rendering",
                "moderately realistic pixel depiction",
                "highly legible pixel realism",
                "advanced representational pixel rendering"),
            DetailDensity => MapBand(value,
                "sparse pixel detail",
                "light sprite detail",
                "clear pixel detailing",
                "dense pixel detail layering",
                "high-density pixel detail"),
            BackgroundComplexity => MapBand(value,
                "minimal pixel background",
                "restrained backdrop detail",
                "supporting pixel environment",
                "rich pixel scene detail",
                "densely layered pixel environment"),
            Contrast => MapBand(value,
                "low pixel contrast",
                "gentle value separation",
                "balanced pixel contrast",
                "crisp sprite contrast",
                "striking pixel contrast"),
            Saturation => MapBand(value,
                "muted pixel color",
                "restrained palette color",
                "balanced pixel color",
                "rich pixel palette",
                "vivid pixel color"),
            ImageCleanliness => MapBand(value,
                "raw pixel finish",
                "lightly refined pixel finish",
                "clean sprite presentation",
                "polished pixel presentation",
                "ultra-clean pixel finish"),
            NarrativeDensity => MapBand(value,
                "single-read pixel idea",
                "light scene implication",
                "clear gameplay/story cues",
                "layered pixel storytelling",
                "world-rich pixel narrative"),
            Framing => MapBand(value,
                "tight sprite framing",
                "close gameplay framing",
                "balanced scene framing",
                "expansive pixel framing",
                "showcase-scale pixel framing"),
            CameraDistance => MapBand(value,
                "extreme close sprite view",
                "close pixel view",
                "mid-distance scene view",
                "wide gameplay view",
                "far-set pixel scene view"),
            TextureDepth => MapBand(value,
                "minimal texture indication",
                "light pixel texture suggestion",
                "clear material indication",
                "rich pixel texture work",
                "dense pixel-surface articulation"),
            FocusDepth => MapBand(value,
                "uniform scene clarity",
                "light depth separation",
                "controlled scene separation",
                "selective pixel depth emphasis",
                "strong layered depth emphasis"),
            Symbolism => MapBand(value,
                "mostly literal pixel language",
                "subtle symbolic cues",
                "suggestive motif use",
                "pronounced symbolic pixel motifs",
                "mythic pixel symbolism"),
            AtmosphericDepth => MapBand(value,
                "limited atmospheric depth",
                "slight spatial recession",
                "air-filled pixel scene depth",
                "luminous pixel atmosphere",
                "deep layered pixel atmosphere"),
            SurfaceAge => MapBand(value,
                "fresh pixel finish",
                "slight surface wear",
                "gentle surface aging",
                "noticeable retro wear",
                "time-softened pixel character"),
            Awe => MapBand(value,
                "grounded scale",
                "slight visual wonder",
                "atmosphere of significance",
                "strong pixel-scale awe",
                "overwhelming pixel grandeur"),
            Whimsy => MapBand(value,
                "serious tone",
                "light playful charm",
                "balanced visual playfulness",
                "strong whimsical energy",
                "bold arcade whimsy"),
            Tension => MapBand(value,
                "low tension",
                "light dramatic charge",
                "scene-level tension",
                "strong gameplay tension",
                "intense arcade pressure"),
            MotionEnergy => MapBand(value,
                "still pixel composition",
                "gentle scene motion",
                "active gameplay energy",
                "dynamic pixel motion",
                "high-kinetic arcade motion"),
            _ => ResolveStandardPhrase(sliderKey, value, configuration),
        };

        return ApplyPixelArtGuardrails(sliderKey, value, configuration, ApplyPixelArtPhraseEconomy(phrase));
    }

    public static string ResolvePixelArtGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            Stylization => new[] { "grounded pixel treatment", "light pixel stylization", "stylized pixel rendering", "strong pixel-art stylization", "highly stylized pixel-art language" },
            Realism => new[] { "omit explicit realism", "loosely observed pixel rendering", "moderately realistic pixel depiction", "highly legible pixel realism", "advanced representational pixel rendering" },
            DetailDensity => new[] { "sparse pixel detail", "light sprite detail", "clear pixel detailing", "dense pixel detail layering", "high-density pixel detail" },
            BackgroundComplexity => new[] { "minimal pixel background", "restrained backdrop detail", "supporting pixel environment", "rich pixel scene detail", "densely layered pixel environment" },
            Contrast => new[] { "low pixel contrast", "gentle value separation", "balanced pixel contrast", "crisp sprite contrast", "striking pixel contrast" },
            Saturation => new[] { "muted pixel color", "restrained palette color", "balanced pixel color", "rich pixel palette", "vivid pixel color" },
            ImageCleanliness => new[] { "raw pixel finish", "lightly refined pixel finish", "clean sprite presentation", "polished pixel presentation", "ultra-clean pixel finish" },
            NarrativeDensity => new[] { "single-read pixel idea", "light scene implication", "clear gameplay/story cues", "layered pixel storytelling", "world-rich pixel narrative" },
            Framing => new[] { "tight sprite framing", "close gameplay framing", "balanced scene framing", "expansive pixel framing", "showcase-scale pixel framing" },
            CameraDistance => new[] { "extreme close sprite view", "close pixel view", "mid-distance scene view", "wide gameplay view", "far-set pixel scene view" },
            TextureDepth => new[] { "minimal texture indication", "light pixel texture suggestion", "clear material indication", "rich pixel texture work", "dense pixel-surface articulation" },
            FocusDepth => new[] { "uniform scene clarity", "light depth separation", "controlled scene separation", "selective pixel depth emphasis", "strong layered depth emphasis" },
            Symbolism => new[] { "mostly literal pixel language", "subtle symbolic cues", "suggestive motif use", "pronounced symbolic pixel motifs", "mythic pixel symbolism" },
            AtmosphericDepth => new[] { "limited atmospheric depth", "slight spatial recession", "air-filled pixel scene depth", "luminous pixel atmosphere", "deep layered pixel atmosphere" },
            SurfaceAge => new[] { "fresh pixel finish", "slight surface wear", "gentle surface aging", "noticeable retro wear", "time-softened pixel character" },
            Awe => new[] { "grounded scale", "slight visual wonder", "atmosphere of significance", "strong pixel-scale awe", "overwhelming pixel grandeur" },
            Whimsy => new[] { "serious tone", "light playful charm", "balanced visual playfulness", "strong whimsical energy", "bold arcade whimsy" },
            Tension => new[] { "low tension", "light dramatic charge", "scene-level tension", "strong gameplay tension", "intense arcade pressure" },
            MotionEnergy => new[] { "still pixel composition", "gentle scene motion", "active gameplay energy", "dynamic pixel motion", "high-kinetic arcade motion" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolvePixelArtDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddPixelArtDescriptor(phrases, seen, "pixel art");
        AddPixelArtDescriptor(phrases, seen, "sprite-readable structure");

        var subtypeDescriptor = ResolvePixelArtSubtypeDescriptor(configuration.PixelArtSubtype);
        if (!string.IsNullOrWhiteSpace(subtypeDescriptor))
        {
            AddPixelArtDescriptor(phrases, seen, subtypeDescriptor);
        }

        foreach (var phrase in ResolvePixelArtModifierDescriptors(configuration))
        {
            AddPixelArtDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    private static string ResolvePixelArtSubtypeDescriptor(string pixelArtSubtype)
    {
        return pixelArtSubtype switch
        {
            "retro-arcade" => "arcade-style gameplay clarity",
            "console-sprite" => "console-sprite presentation",
            "isometric-pixel" => "isometric scene structure",
            "pixel-platformer" => "side-view gameplay readability",
            "rpg-tileset" => "modular tile-set clarity",
            "pixel-portrait" => "close-read sprite portraiture",
            "pixel-scene" => "story-supporting scene detail",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolvePixelArtModifierDescriptors(PromptConfiguration configuration)
    {
        var keys = GetPixelArtModifierPriority(configuration.PixelArtSubtype);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Limited Palette" when configuration.PixelArtLimitedPalette => "limited-palette discipline",
                "Dithering" when configuration.PixelArtDithering => "intentional dithering texture",
                "Tileable Design" when configuration.PixelArtTileableDesign => "tileable environment design",
                "Sprite Sheet Readability" when configuration.PixelArtSpriteSheetReadability => "sprite-sheet-ready readability",
                "Clean Outline" when configuration.PixelArtCleanOutline => "clean outline definition",
                "Subpixel Shading" when configuration.PixelArtSubpixelShading => "subpixel shading control",
                "HUD / UI Framing" when configuration.PixelArtHudUiFraming => "HUD-style game framing",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetPixelArtModifierPriority(string pixelArtSubtype)
    {
        return pixelArtSubtype switch
        {
            "retro-arcade" => ["Limited Palette", "Clean Outline", "HUD / UI Framing", "Sprite Sheet Readability", "Dithering", "Tileable Design", "Subpixel Shading"],
            "console-sprite" => ["Clean Outline", "Sprite Sheet Readability", "Limited Palette", "Subpixel Shading", "HUD / UI Framing", "Dithering", "Tileable Design"],
            "isometric-pixel" => ["Tileable Design", "Limited Palette", "Subpixel Shading", "Clean Outline", "Sprite Sheet Readability", "HUD / UI Framing", "Dithering"],
            "pixel-platformer" => ["Clean Outline", "Tileable Design", "HUD / UI Framing", "Limited Palette", "Sprite Sheet Readability", "Dithering", "Subpixel Shading"],
            "rpg-tileset" => ["Tileable Design", "Limited Palette", "Sprite Sheet Readability", "Clean Outline", "Dithering", "Subpixel Shading", "HUD / UI Framing"],
            "pixel-portrait" => ["Clean Outline", "Limited Palette", "Subpixel Shading", "Sprite Sheet Readability", "Dithering", "Tileable Design", "HUD / UI Framing"],
            "pixel-scene" => ["Limited Palette", "Dithering", "HUD / UI Framing", "Clean Outline", "Tileable Design", "Sprite Sheet Readability", "Subpixel Shading"],
            _ => ["Limited Palette", "Clean Outline", "Sprite Sheet Readability", "Dithering", "Tileable Design", "Subpixel Shading", "HUD / UI Framing"],
        };
    }

    private static void AddPixelArtDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyPixelArtGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking pixel contrast";
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "vivid pixel color";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity <= 40)
        {
            return "densely layered pixel environment";
        }

        return phrase;
    }

    private static string ApplyPixelArtPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase
            .Replace("pixel-art ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("pixel art ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("pixel-art", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("pixel art", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }

    public static IEnumerable<string> ResolveWatercolorDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddWatercolorDescriptor(phrases, seen, "watercolor illustration");

        var styleDescriptor = ResolveWatercolorStyleDescriptor(configuration.WatercolorStyle);
        if (!string.IsNullOrWhiteSpace(styleDescriptor))
        {
            AddWatercolorDescriptor(phrases, seen, styleDescriptor);
        }

        foreach (var phrase in ResolveWatercolorCheckboxDescriptors(configuration))
        {
            AddWatercolorDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static IEnumerable<string> ResolveAnimeDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddAnimeDescriptor(phrases, seen, "stylized anime illustration");
        AddAnimeDescriptor(phrases, seen, "polished anime key-art sensibility");
        AddAnimeDescriptor(phrases, seen, "expressive character-led staging");
        AddAnimeDescriptor(phrases, seen, "clean silhouette readability");
        AddAnimeDescriptor(phrases, seen, "emotionally legible anime rendering");

        AddAnimeDescriptor(phrases, seen, ResolveAnimeStyleDescriptor(configuration.AnimeStyle));
        AddAnimeDescriptor(phrases, seen, ResolveAnimeEraDescriptor(configuration.AnimeEra));

        foreach (var phrase in ResolveAnimeCheckboxDescriptors(configuration))
        {
            AddAnimeDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    private static string ResolveWatercolorStyleDescriptor(string watercolorStyle)
    {
        return watercolorStyle switch
        {
            "general-watercolor" => string.Empty,
            "botanical-watercolor" => "delicate botanical handling",
            "storybook-watercolor" => "gentle storybook presentation",
            "landscape-watercolor" => "airy landscape staging",
            "architectural-watercolor" => "structured architectural clarity",
            _ => string.Empty,
        };
    }

    private static IEnumerable<string> ResolveWatercolorCheckboxDescriptors(PromptConfiguration configuration)
    {
        var keys = GetWatercolorModifierPriority(configuration.WatercolorStyle);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Transparent Washes" when configuration.WatercolorTransparentWashes => "transparent washes",
                "Soft Bleeds" when configuration.WatercolorSoftBleeds => "soft wet-into-wet diffusion",
                "Paper Texture" when configuration.WatercolorPaperTexture => "visible cold-press paper texture",
                "Ink and Watercolor" when configuration.WatercolorInkAndWatercolor => "ink-and-wash interplay",
                "Atmospheric Wash" when configuration.WatercolorAtmosphericWash => "airy wash atmosphere",
                "Gouache Accents" when configuration.WatercolorGouacheAccents => "selective opaque gouache highlights",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetWatercolorModifierPriority(string watercolorStyle)
    {
        return watercolorStyle switch
        {
            "botanical-watercolor" => ["Paper Texture", "Transparent Washes", "Soft Bleeds", "Atmospheric Wash", "Ink and Watercolor", "Gouache Accents"],
            "storybook-watercolor" => ["Soft Bleeds", "Atmospheric Wash", "Transparent Washes", "Gouache Accents", "Paper Texture", "Ink and Watercolor"],
            "landscape-watercolor" => ["Atmospheric Wash", "Transparent Washes", "Soft Bleeds", "Paper Texture", "Gouache Accents", "Ink and Watercolor"],
            "architectural-watercolor" => ["Ink and Watercolor", "Paper Texture", "Transparent Washes", "Gouache Accents", "Soft Bleeds", "Atmospheric Wash"],
            _ => ["Transparent Washes", "Soft Bleeds", "Paper Texture", "Atmospheric Wash", "Ink and Watercolor", "Gouache Accents"],
        };
    }

    private static void AddWatercolorDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyWatercolorGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        phrase = ApplyWatercolorPhraseEconomy(phrase);

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast <= 40)
        {
            return "vivid luminous saturation";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation <= 40)
        {
            return "striking tonal contrast";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity <= 40)
        {
            return "deep atmospheric perspective";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth <= 40)
        {
            return "densely layered setting";
        }

        return phrase;
    }

    private static string ApplyWatercolorPhraseEconomy(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return phrase;
        }

        var economical = phrase.Replace("watercolor ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("watercolor", string.Empty, StringComparison.OrdinalIgnoreCase);

        while (economical.Contains("  ", StringComparison.Ordinal))
        {
            economical = economical.Replace("  ", " ", StringComparison.Ordinal);
        }

        return economical.Trim(' ', ',', '.');
    }

    private static string ResolveStandardPhrase(string sliderKey, int value, PromptConfiguration configuration)
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

    public static string ResolveVintageBendLightingDescriptor(PromptConfiguration configuration)
    {
        var value = configuration.LightingIntensity;
        return value switch
        {
            <= 20 => "subdued practical room light",
            <= 40 => "practical fluorescent and tungsten mixed light",
            <= 60 => "balanced practical illumination",
            <= 80 => "clear period interior brightness",
            _ => "strong practical-light presence",
        };
    }

    public static IEnumerable<string> ResolveVintageBendDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddVintageDescriptor(phrases, seen, "candid documentary realism");
        AddVintageDescriptor(phrases, seen, "early-1980s color photography");
        AddVintageDescriptor(phrases, seen, "period-correct color restraint");
        AddVintageDescriptor(phrases, seen, "non-glossy finish");

        if (configuration.Realism >= 61)
        {
            AddVintageDescriptor(phrases, seen, "lived-in interior realism");
        }

        if (configuration.TextureDepth >= 41)
        {
            AddVintageDescriptor(phrases, seen, "subtle analog grain");
        }

        if (configuration.Saturation >= 41)
        {
            AddVintageDescriptor(phrases, seen, "muted olive-and-amber palette");
        }

        if (configuration.BackgroundComplexity >= 41)
        {
            AddVintageDescriptor(phrases, seen, "restrained observational framing");
        }

        if (configuration.MotionEnergy >= 21)
        {
            AddVintageDescriptor(phrases, seen, "everyday social energy");
        }

        if (configuration.Tension >= 21)
        {
            AddVintageDescriptor(phrases, seen, "quiet human tension");
        }

        if (configuration.AtmosphericDepth >= 21)
        {
            AddVintageDescriptor(phrases, seen, "slight atmospheric recession");
        }

        if (configuration.FocusDepth <= 60)
        {
            AddVintageDescriptor(phrases, seen, "readable facial structure");
        }

        return phrases;
    }

    private static IEnumerable<string> ResolveAnimeCheckboxDescriptors(PromptConfiguration configuration)
    {
        var keys = GetAnimeModifierPriority(configuration.AnimeStyle);
        var selected = new List<string>();

        foreach (var key in keys)
        {
            if (selected.Count >= 4)
            {
                break;
            }

            var phrase = key switch
            {
                "Cel Shading" when configuration.AnimeCelShading => "clean cel-shaded rendering",
                "Clean Line Art" when configuration.AnimeCleanLineArt => "clean anime linework",
                "Expressive Eyes" when configuration.AnimeExpressiveEyes => "expressive eye design",
                "Dynamic Action" when configuration.AnimeDynamicAction => "dynamic anime action framing",
                "Cinematic Lighting" when configuration.AnimeCinematicLighting => "cinematic anime lighting",
                "Stylized Hair" when configuration.AnimeStylizedHair => "stylized hair-shape design",
                "Atmospheric Effects" when configuration.AnimeAtmosphericEffects => "atmospheric anime effects",
                _ => string.Empty,
            };

            if (!string.IsNullOrWhiteSpace(phrase))
            {
                selected.Add(phrase);
            }
        }

        return selected;
    }

    private static IReadOnlyList<string> GetAnimeModifierPriority(string animeStyle)
    {
        return animeStyle switch
        {
            "Shonen Action" => ["Dynamic Action", "Cinematic Lighting", "Cel Shading", "Clean Line Art", "Stylized Hair", "Atmospheric Effects", "Expressive Eyes"],
            "Shojo Romance" => ["Expressive Eyes", "Clean Line Art", "Stylized Hair", "Cel Shading", "Atmospheric Effects", "Cinematic Lighting", "Dynamic Action"],
            "Seinen Dark" => ["Cinematic Lighting", "Clean Line Art", "Atmospheric Effects", "Cel Shading", "Dynamic Action", "Stylized Hair", "Expressive Eyes"],
            "Fantasy Anime" => ["Atmospheric Effects", "Cinematic Lighting", "Clean Line Art", "Cel Shading", "Stylized Hair", "Expressive Eyes", "Dynamic Action"],
            "Mecha / Sci-fi Anime" => ["Clean Line Art", "Cinematic Lighting", "Dynamic Action", "Cel Shading", "Atmospheric Effects", "Stylized Hair", "Expressive Eyes"],
            "Slice of Life" => ["Clean Line Art", "Expressive Eyes", "Atmospheric Effects", "Cel Shading", "Stylized Hair", "Cinematic Lighting", "Dynamic Action"],
            _ => ["Clean Line Art", "Cel Shading", "Expressive Eyes", "Cinematic Lighting", "Stylized Hair", "Atmospheric Effects", "Dynamic Action"],
        };
    }

    private static string ResolveAnimeStyleDescriptor(string animeStyle)
    {
        return animeStyle switch
        {
            "General Anime" => string.Empty,
            "Shonen Action" => "propulsive shonen action energy",
            "Shojo Romance" => "soft romantic anime framing",
            "Seinen Dark" => "weightier dark-anime tone",
            "Fantasy Anime" => "mythic anime worldbuilding",
            "Mecha / Sci-fi Anime" => "mechanical sci-fi anime design",
            "Slice of Life" => "quiet slice-of-life anime intimacy",
            _ => string.Empty,
        };
    }

    private static string ResolveAnimeEraDescriptor(string animeEra)
    {
        return animeEra switch
        {
            "Default / Modern" => string.Empty,
            "Classic Anime (1960s–1970s)" => "simpler forms and flatter graphic logic",
            "Cel-Era Anime (1980s)" => "analog cel warmth and painted-background depth",
            "Broadcast Anime (1990s)" => "clean cel-TV production feel",
            "Early Digital Anime (2000s)" => "transitional digital compositing",
            "Modern Anime (2010s+)" => "polished seasonal-anime finish",
            _ => string.Empty,
        };
    }

    private static void AddAnimeDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }

    private static string ApplyAnimeGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Contrast >= 60)
        {
            return "radiant anime palette";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.Saturation >= 60)
        {
            return "striking anime value separation";
        }

        if (string.Equals(sliderKey, AtmosphericDepth, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.BackgroundComplexity >= 60)
        {
            return "deeply layered anime spatial atmosphere";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.AtmosphericDepth >= 60)
        {
            return "densely layered anime world detail";
        }

        return phrase;
    }

    public static string ResolveArtistInfluenceDescriptor(int strength, string artistName, string? intentMode = null)
    {
        if (string.IsNullOrWhiteSpace(artistName) || !Definitions.TryGetValue(ArtistInfluenceStrength, out var definition))
        {
            return string.Empty;
        }

        if (IntentModeCatalog.IsVintageBend(intentMode))
        {
            return ResolveVintageBendArtistInfluenceDescriptor(strength, artistName);
        }

        var phrases = definition.Bands[GetBandIndex(strength)].Phrases;
        if (phrases.Length == 0)
        {
            return string.Empty;
        }

        return phrases[Math.Abs(GetStableHash($"{artistName}|{strength}")) % phrases.Length]
            .Replace("{artist}", artistName, StringComparison.Ordinal);
    }

    private static string ResolveVintageBendArtistInfluenceDescriptor(int strength, string artistName)
    {
        if (strength <= 20)
        {
            return string.Empty;
        }

        var band = GetBandIndex(strength);
        var phrase = band switch
        {
            0 => string.Empty,
            1 => "light stylistic cues from {artist}",
            2 => "artist-informed sensibility drawn from {artist}",
            3 => "clearly shaped by {artist}",
            _ => "deeply informed by {artist}",
        };

        return string.IsNullOrWhiteSpace(phrase)
            ? string.Empty
            : phrase.Replace("{artist}", artistName, StringComparison.Ordinal);
    }

    public static string ResolveVintageBendPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var bandIndex = GetBandIndex(value);
        var phrase = sliderKey switch
        {
            ArtistInfluenceStrength => string.Empty,
            Stylization => MapBand(value,
                "candid documentary treatment",
                "lightly stylized documentary realism",
                "period documentary image language",
                "strongly stylized analog documentary treatment",
                "highly stylized period-documentary visual language"),
            Realism => MapBand(value,
                "omit explicit realism",
                "loosely realistic documentary image",
                "moderately realistic period photography",
                "high documentary realism",
                "strongly realistic analog documentary rendering"),
            TextureDepth => MapBand(value,
                "minimal film texture",
                "light analog grain",
                "clear film-era texture",
                "rich analog surface texture",
                "deeply worked film-grain texture"),
            NarrativeDensity => MapBand(value,
                "single candid moment",
                "light social context",
                "layered observational storytelling",
                "dense implied human context",
                "world-rich documentary narrative density"),
            Symbolism => MapBand(value,
                "mostly literal documentary framing",
                "subtle social cues",
                "suggestive cultural undertones",
                "pronounced social-symbolic resonance",
                "mythic social charge"),
            SurfaceAge => MapBand(value,
                "clean period print character",
                "slight print wear",
                "gentle analog age",
                "noticeable period print character",
                "strongly aged print look"),
            Framing => MapBand(value,
                "restrained observational framing",
                "close observational framing",
                "balanced documentary framing",
                "broader environmental documentary framing",
                "wide social-context framing"),
            BackgroundComplexity => MapBand(value,
                "minimal documentary background",
                "restrained period background",
                "supporting lived-in environment",
                "rich period environment detail",
                "densely layered documentary setting"),
            MotionEnergy => MapBand(value,
                "still candid frame",
                "gentle lived-in motion",
                "active social energy",
                "dynamic candid movement",
                "high kinetic documentary energy"),
            FocusDepth => MapBand(value,
                "broad soft-focus realism",
                "gentle focus falloff",
                "balanced documentary focus",
                "selective observational focus",
                "strong subject-isolating focus"),
            ImageCleanliness => MapBand(value,
                "rough analog messiness",
                "lightly imperfect period image",
                "balanced documentary cleanliness",
                "clean analog print character",
                "unusually clean period image"),
            DetailDensity => MapBand(value,
                "sparse readable detail",
                "restrained documentary detail",
                "balanced observational detail",
                "rich period detail",
                "dense documentary detail load"),
            AtmosphericDepth => MapBand(value,
                "limited air depth",
                "slight spatial recession",
                "air-filled period depth",
                "layered analog atmosphere",
                "deep lived-in atmospheric perspective"),
            Chaos => MapBand(value,
                "controlled candid composition",
                "mild social restlessness",
                "busy documentary energy",
                "crowded observational disorder",
                "high visual disorder"),
            Whimsy => MapBand(value,
                "serious observational tone",
                "slight social lightness",
                "casual human playfulness",
                "strong social looseness",
                "rowdy period playfulness"),
            Tension => MapBand(value,
                "low social tension",
                "light documentary tension",
                "noticeable human unease",
                "strong interpersonal pressure",
                "intense social or political tension"),
            Awe => MapBand(value,
                "grounded human scale",
                "slight sense of presence",
                "quiet atmosphere of significance",
                "strong observational gravity",
                "overwhelming social or historical weight"),
            Saturation => MapBand(value,
                "muted period color",
                "restrained analog color",
                "balanced period saturation",
                "rich film-era color",
                "vivid 1980s color intensity"),
            Contrast => MapBand(value,
                "low contrast print softness",
                "gentle tonal separation",
                "balanced documentary contrast",
                "crisp analog contrast",
                "striking print contrast"),
            Temperature => MapBand(value,
                "cool institutional cast",
                "restrained neutral-cool balance",
                "balanced period warmth",
                "mild warm print bias",
                "strong tobacco-amber warmth"),
            LightingIntensity => MapBand(value,
                "dim available light",
                "subdued room light",
                "balanced practical illumination",
                "clear period interior brightness",
                "strong practical-light presence"),
            CameraDistance => MapBand(value,
                "intimate close documentary framing",
                "close observational framing",
                "mid-distance candid view",
                "broader environmental view",
                "wide social-context framing"),
            CameraAngle => MapBand(value,
                "low observational angle",
                "slightly lowered human viewpoint",
                "eye-level documentary view",
                "slightly elevated social overview",
                "detached observational vantage"),
            _ => string.Empty,
        };

        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        return ApplyVintageBendGuardrails(sliderKey, bandIndex, configuration, phrase);
    }

    public static string ResolveVintageBendGuideText(string sliderKey)
    {
        var labels = sliderKey switch
        {
            ArtistInfluenceStrength => new[] { "omit artist language", "light stylistic cues from", "artist-informed sensibility drawn from", "clearly shaped by", "deeply informed by" },
            Stylization => new[] { "candid documentary treatment", "lightly stylized documentary realism", "period documentary image language", "strongly stylized analog documentary treatment", "highly stylized period-documentary visual language" },
            Realism => new[] { "omit explicit realism", "loosely realistic documentary image", "moderately realistic period photography", "high documentary realism", "strongly realistic analog documentary rendering" },
            TextureDepth => new[] { "minimal film texture", "light analog grain", "clear film-era texture", "rich analog surface texture", "deeply worked film-grain texture" },
            NarrativeDensity => new[] { "single candid moment", "light social context", "layered observational storytelling", "dense implied human context", "world-rich documentary narrative density" },
            Symbolism => new[] { "mostly literal documentary framing", "subtle social cues", "suggestive cultural undertones", "pronounced social-symbolic resonance", "mythic social charge" },
            SurfaceAge => new[] { "clean period print character", "slight print wear", "gentle analog age", "noticeable period print character", "strongly aged print look" },
            Framing => new[] { "restrained observational framing", "close observational framing", "balanced documentary framing", "broader environmental documentary framing", "wide social-context framing" },
            BackgroundComplexity => new[] { "minimal documentary background", "restrained period background", "supporting lived-in environment", "rich period environment detail", "densely layered documentary setting" },
            MotionEnergy => new[] { "still candid frame", "gentle lived-in motion", "active social energy", "dynamic candid movement", "high kinetic documentary energy" },
            FocusDepth => new[] { "broad soft-focus realism", "gentle focus falloff", "balanced documentary focus", "selective observational focus", "strong subject-isolating focus" },
            ImageCleanliness => new[] { "rough analog messiness", "lightly imperfect period image", "balanced documentary cleanliness", "clean analog print character", "unusually clean period image" },
            DetailDensity => new[] { "sparse readable detail", "restrained documentary detail", "balanced observational detail", "rich period detail", "dense documentary detail load" },
            AtmosphericDepth => new[] { "limited air depth", "slight spatial recession", "air-filled period depth", "layered analog atmosphere", "deep lived-in atmospheric perspective" },
            Chaos => new[] { "controlled candid composition", "mild social restlessness", "busy documentary energy", "crowded observational disorder", "high visual disorder" },
            Whimsy => new[] { "serious observational tone", "slight social lightness", "casual human playfulness", "strong social looseness", "rowdy period playfulness" },
            Tension => new[] { "low social tension", "light documentary tension", "noticeable human unease", "strong interpersonal pressure", "intense social or political tension" },
            Awe => new[] { "grounded human scale", "slight sense of presence", "quiet atmosphere of significance", "strong observational gravity", "overwhelming social or historical weight" },
            Saturation => new[] { "muted period color", "restrained analog color", "balanced period saturation", "rich film-era color", "vivid 1980s color intensity" },
            Contrast => new[] { "low contrast print softness", "gentle tonal separation", "balanced documentary contrast", "crisp analog contrast", "striking print contrast" },
            Temperature => new[] { "cool institutional cast", "restrained neutral-cool balance", "balanced period warmth", "mild warm print bias", "strong tobacco-amber warmth" },
            LightingIntensity => new[] { "dim available light", "subdued room light", "balanced practical illumination", "clear period interior brightness", "strong practical-light presence" },
            CameraDistance => new[] { "intimate close documentary framing", "close observational framing", "mid-distance candid view", "broader environmental view", "wide social-context framing" },
            CameraAngle => new[] { "low observational angle", "slightly lowered human viewpoint", "eye-level documentary view", "slightly elevated social overview", "detached observational vantage" },
            _ => Array.Empty<string>(),
        };

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    private static string ResolveDefaultGuideText(string sliderKey)
    {
        return Definitions.TryGetValue(sliderKey, out var definition)
            ? string.Join("  |  ", definition.Bands.Select(band => band.Interpretation))
            : string.Empty;
    }

    private static string ApplyVintageBendGuardrails(string sliderKey, int bandIndex, PromptConfiguration configuration, string phrase)
    {
        if (string.Equals(sliderKey, Temperature, StringComparison.OrdinalIgnoreCase) && bandIndex == 4)
        {
            return configuration.SurfaceAge >= 60 || configuration.TextureDepth >= 60
                ? "mild warm print bias"
                : phrase;
        }

        if (string.Equals(sliderKey, SurfaceAge, StringComparison.OrdinalIgnoreCase) && bandIndex == 4)
        {
            return configuration.TextureDepth >= 60 || configuration.Temperature >= 60
                ? "noticeable period print character"
                : phrase;
        }

        if (string.Equals(sliderKey, TextureDepth, StringComparison.OrdinalIgnoreCase) && bandIndex == 4)
        {
            return configuration.SurfaceAge >= 60 || configuration.Temperature >= 60
                ? "rich analog surface texture"
                : phrase;
        }

        if (string.Equals(sliderKey, Saturation, StringComparison.OrdinalIgnoreCase) && bandIndex == 4)
        {
            return configuration.Temperature >= 60 || configuration.SurfaceAge >= 60
                ? "rich film-era color"
                : phrase;
        }

        return phrase;
    }

    private static void AddVintageDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
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

    private static string MapBand(int value, string low, string lowMid, string mid, string high, string veryHigh)
    {
        if (value <= 20) return low;
        if (value <= 40) return lowMid;
        if (value <= 60) return mid;
        if (value <= 80) return high;
        return veryHigh;
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

