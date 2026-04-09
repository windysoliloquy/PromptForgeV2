using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveTattooArtPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetTattooArtBandLabels(sliderKey);
        return labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);
    }

    public static string ResolveTattooArtGuideText(string sliderKey)
    {
        var labels = GetTattooArtBandLabels(sliderKey);
        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveTattooArtDescriptors(PromptConfiguration configuration)
    {
        yield return "tattoo flash design";
        yield return "flat printable design presentation";
    }

    private static string[] GetTattooArtBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["grounded flash-sheet treatment", "light emblematic stylization", "stylized line-and-fill design", "strong iconographic stylization", "highly stylized flash-sheet iconography"],
            Realism => ["omit explicit realism", "loosely plausible draft clarity", "moderately realistic mark construction", "high visual realism in inked design finish", "strongly convincing print-ready ink design"],
            TextureDepth => ["minimal surface buildup", "light needle-clean texture", "clear line-and-fill tactility", "rich packed-pigment texture", "deeply worked etched-and-packed detail"],
            NarrativeDensity => ["single emblem read", "light motif interplay", "layered storytelling cues", "dense symbolic layering", "world-rich motif mythology"],
            Symbolism => ["mostly literal iconography", "subtle emblem cues", "suggestive symbolic motifs", "pronounced allegorical imagery", "mythic emblem charge"],
            SurfaceAge => ["fresh ink finish", "slight settled wear", "gently aged print character", "aged flash-sheet patina", "time-softened heirloom patina"],
            Framing => ["isolated flash motif", "centered design presentation", "balanced flat-layout arrangement", "expanded flash grouping", "full composition layout"],
            CameraDistance => ["extreme close mark detail", "near design read", "mid-distance layout view", "broad composition read", "full composition view"],
            CameraAngle => ["flat front-on presentation", "nearly flat display view", "clean straight-on layout", "formal design vantage", "orthographic-feel presentation"],
            BackgroundComplexity => ["backgroundless isolation", "restrained empty field", "minimal presentation context", "light display support", "layered presentation support"],
            MotionEnergy => ["still held mark", "gentle curve energy", "active flow-driven movement", "dynamic slash-and-whip motion", "high kinetic attack rhythm"],
            AtmosphericDepth => ["flat reference depth", "minimal recession", "controlled shallow separation", "light display separation", "restrained showcase depth"],
            Chaos => ["disciplined stencil control", "slight ornamental drift", "restless collage tension", "orchestrated visual collision", "high compositional volatility"],
            FocusDepth => ["sheet-wide clarity", "mild focal preference", "controlled subject emphasis", "strong display isolation", "razor-thin presentation focus"],
            ImageCleanliness => ["raw transfer feel", "clean stencil finish", "polished line-and-fill finish", "production-clean presentation", "immaculate portfolio finish"],
            DetailDensity => ["sparse mark economy", "selective accent detailing", "articulated linework density", "richly packed filigree detail", "obsessive micro-pattern packing"],
            Whimsy => ["severe tone", "sly charm", "playful mischief", "strong irreverent wit", "bold cheeky punch"],
            Tension => ["low visual tension", "slight edge", "charged design pressure", "strong confrontational menace", "intense dramatic threat"],
            Awe => ["intimate scale", "slight reverence", "emblematic grandeur", "strong iconic weight", "overwhelming mythic majesty"],
            Temperature => ["cool neutral cast", "tempered cool-warm balance", "balanced warmth", "heated warm cast", "furnace-hot glow"],
            LightingIntensity => ["subdued studio light", "soft inspection light", "balanced presentation light", "bright reveal light", "hard showcase blaze"],
            Saturation => ["muted pigment range", "restrained color load", "balanced pigment saturation", "rich chroma packing", "full-spectrum pigment punch"],
            Contrast => ["soft tonal separation", "restrained line-to-fill snap", "balanced light-dark structure", "bold silhouette carve", "high-impact graphic contrast"],
            _ => [],
        };
    }
}
