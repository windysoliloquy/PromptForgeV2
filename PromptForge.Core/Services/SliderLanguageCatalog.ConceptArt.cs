using PromptForge.App.Models;
using System.Text.RegularExpressions;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveConceptArtPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetConceptArtBandLabels(sliderKey, configuration.ConceptArtSubtype);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyConceptArtGuardrails(sliderKey, value, configuration, ApplyConceptArtPhraseEconomy(phrase));
    }

    public static string ResolveConceptArtGuideText(string sliderKey)
    {
        var labels = GetConceptArtBandLabels(sliderKey, "keyframe-concept");

        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static string ResolveConceptArtGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetConceptArtBandLabels(sliderKey, configuration.ConceptArtSubtype);

        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveConceptArtDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddConceptArtDescriptor(phrases, seen, "concept art");

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
            "Soft daylight" => "soft reference light",
            "Golden hour" => "golden-hour light",
            "Dramatic studio light" => "studio lighting",
            "Overcast" => "neutral overcast light",
            "Moonlit" => "moody atmospheric light",
            "Soft glow" => "subtle glow",
            "Dusk haze" => "late-day haze",
            "Warm directional light" => "warm directional light",
            "Volumetric cinematic light" => "atmospheric volumetric light",
            _ => configuration.Lighting.Trim(' ', ',', '.'),
        };
    }

    private static string ResolveConceptArtSubtypeDescriptor(string conceptArtSubtype)
    {
        return conceptArtSubtype switch
        {
            "keyframe-concept" => "keyframe scene staging",
            "environment-concept" => "environment development",
            "character-concept" => "character development",
            "creature-concept" => "creature development",
            "costume-concept" => "costume development",
            "prop-concept" => "prop development",
            "vehicle-concept" => "vehicle development",
            _ => string.Empty,
        };
    }

    private static string[] GetConceptArtBandLabels(string sliderKey, string conceptArtSubtype)
    {
        if (string.Equals(conceptArtSubtype, "keyframe-concept", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-beat scene read", "light story-event cues", "clear scene storytelling", "layered narrative staging", "keyframe-led narrative charge"],
                Framing => ["tight cinematic crop", "contained scene framing", "balanced keyframe framing", "broad cinematic staging", "showcase scene staging"],
                BackgroundComplexity => ["minimal scene support", "restrained world support", "clear environmental staging", "rich scene buildout", "densely layered world support"],
                CameraDistance => ["intimate scene distance", "close dramatic distance", "readable scene distance", "wide event distance", "far-set cinematic distance"],
                AtmosphericDepth => ["limited cinematic depth", "slight spatial recession", "clear scene depth", "luminous atmospheric layering", "deep cinematic atmosphere"],
                MotionEnergy => ["held dramatic beat", "light motion cue", "active scene force", "dynamic event momentum", "high-impact cinematic motion"],
                Chaos => ["controlled dramatic order", "light scene unrest", "active compositional tension", "volatile scene disruption", "orchestrated visual upheaval"],
                Awe => ["grounded dramatic scale", "slight cinematic wonder", "atmosphere of significance", "strong cinematic grandeur", "overwhelming set-piece scale"],
                FocusDepth => ["deep scene clarity", "broad spatial clarity", "balanced subject separation", "selective dramatic focus", "hero-subject isolation"],
                _ => GetConceptArtBaselineBandLabels(sliderKey),
            };
        }

        if (string.Equals(conceptArtSubtype, "costume-concept", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-costume read", "light role cues", "clear wardrobe identity", "layered design logic", "costume-led narrative charge"],
                TextureDepth => ["simplified fabric read", "light textile articulation", "clear material-and-trim detail", "rich layered material detail", "dense finish-and-wear complexity"],
                Framing => ["close garment crop", "tight costume staging", "balanced costume framing", "full-outfit presentation", "showcase costume presentation"],
                CameraDistance => ["intimate garment inspection", "close outfit study", "readable full-outfit distance", "display-distance costume read", "stand-off showcase distance"],
                FocusDepth => ["full-outfit clarity", "broad garment clarity", "balanced detail separation", "selective trim focus", "hero-detail isolation"],
                MotionEnergy => ["static wardrobe presentation", "light fabric shift", "active drape movement", "strong motion-led flow", "high-energy fabric sweep"],
                ImageCleanliness => ["raw design-board finish", "light study refinement", "clean development finish", "polished presentation finish", "hero-costume presentation finish"],
                DetailDensity => ["sparse garment definition", "light construction support", "clear seam-and-trim detail", "dense wardrobe detailing", "high-density ornament articulation"],
                Awe => ["grounded wardrobe presence", "striking costume appeal", "clear signature presence", "commanding silhouette presence", "standout iconic costume presence"],
                _ => GetConceptArtBaselineBandLabels(sliderKey),
            };
        }

        if (string.Equals(conceptArtSubtype, "vehicle-concept", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-vehicle read", "light function cues", "clear operational role", "layered engineering logic", "vehicle-led narrative charge"],
                TextureDepth => ["simplified material read", "light panel-and-surface articulation", "clear material-and-join detail", "rich structural surface detail", "dense finish-and-wear complexity"],
                Framing => ["cropped vehicle study", "tight vehicle staging", "balanced vehicle framing", "full-vehicle presentation", "showcase vehicle presentation"],
                CameraDistance => ["close structural inspection", "near-form study distance", "readable full-vehicle distance", "display-distance vehicle read", "stand-off showcase distance"],
                FocusDepth => ["full-vehicle clarity", "broad structural clarity", "balanced form separation", "selective feature focus", "hero-feature isolation"],
                MotionEnergy => ["static machine presentation", "light readiness cue", "active operational motion", "strong propulsion force", "high-velocity machine energy"],
                ImageCleanliness => ["raw design-board finish", "light study refinement", "clean development finish", "polished presentation finish", "hero-vehicle presentation finish"],
                DetailDensity => ["sparse structural definition", "light engineering support", "clear construction detail", "dense mechanical detailing", "high-density component articulation"],
                Awe => ["grounded machine presence", "striking vehicle appeal", "clear signature presence", "commanding machine presence", "standout iconic vehicle presence"],
                _ => GetConceptArtBaselineBandLabels(sliderKey),
            };
        }

        if (string.Equals(conceptArtSubtype, "prop-concept", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-object read", "light functional cues", "clear use-case read", "layered design logic", "prop-led narrative charge"],
                TextureDepth => ["simplified material read", "light material articulation", "clear surface-and-edge detail", "rich material specificity", "dense finish-and-wear detail"],
                Framing => ["isolated object crop", "tight object staging", "balanced prop framing", "full-object presentation", "showcase prop presentation"],
                CameraDistance => ["intimate surface inspection", "close object study", "readable prop distance", "display-distance presentation", "stand-off showcase distance"],
                FocusDepth => ["full-object clarity", "broad object clarity", "balanced detail separation", "selective feature focus", "hero-feature isolation"],
                MotionEnergy => ["static object presentation", "light implied handling", "active function cue", "operational force suggestion", "high-energy deployment cue"],
                ImageCleanliness => ["raw design-board finish", "light study refinement", "clean development finish", "polished presentation finish", "hero-object presentation finish"],
                DetailDensity => ["sparse shape definition", "light functional support", "clear construction detail", "dense design detailing", "high-density component articulation"],
                Awe => ["grounded object presence", "striking object appeal", "clear signature presence", "commanding hero-object presence", "standout iconic object presence"],
                _ => GetConceptArtBaselineBandLabels(sliderKey),
            };
        }

        if (string.Equals(conceptArtSubtype, "creature-concept", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                TextureDepth => ["simplified surface read", "light hide articulation", "clear anatomical surface detail", "rich biological texture", "dense creature-surface complexity"],
                NarrativeDensity => ["single-creature read", "light behavioral cues", "clear ecological role", "layered survival cues", "creature-led narrative charge"],
                Framing => ["head-focused study framing", "tight form staging", "balanced full-creature framing", "extended body staging", "showcase anatomy framing"],
                BackgroundComplexity => ["neutral presentation ground", "faint habitat hint", "readable habitat support", "rich habitat context", "densely layered habitat support"],
                CameraDistance => ["intimate anatomy inspection", "close-form study distance", "readable full-body distance", "display-distance form read", "stand-off specimen distance"],
                MotionEnergy => ["still specimen stance", "alert posture shift", "active prowling motion", "attack-ready force", "explosive feral momentum"],
                DetailDensity => ["sparse shape read", "light anatomical support", "clear structural anatomy", "dense form-and-surface detail", "high-density creature articulation"],
                Tension => ["calm living presence", "alert restraint", "noticeable threat signal", "strong feral pressure", "immediate danger presence"],
                Awe => ["grounded physical presence", "striking animal presence", "commanding apex presence", "imposing primal grandeur", "overwhelming primeval presence"],
                _ => GetConceptArtBaselineBandLabels(sliderKey),
            };
        }

        if (string.Equals(conceptArtSubtype, "environment-concept", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-location read", "light world cues", "clear environmental storytelling", "layered worldbuilding context", "environment-led narrative charge"],
                Framing => ["cropped location study", "contained scene framing", "balanced environmental framing", "wide spatial staging", "panorama-scale staging"],
                BackgroundComplexity => ["minimal setting scaffold", "restrained location structure", "clear environmental read", "rich spatial buildout", "densely layered world buildout"],
                CameraDistance => ["close-in location study", "near-scene distance", "readable environmental distance", "broad establishing distance", "far-set survey distance"],
                AtmosphericDepth => ["limited spatial recession", "slight aerial recession", "clear environmental depth", "luminous distance layering", "deep atmospheric perspective"],
                FocusDepth => ["survey-wide clarity", "broad spatial clarity", "balanced depth separation", "selective area focus", "foreground-isolated focus"],
                MotionEnergy => ["still world state", "light environmental drift", "active weather or flow", "dynamic force movement", "high kinetic environmental force"],
                Chaos => ["controlled world layout", "light compositional unrest", "active structural tension", "unstable environmental layering", "large-scale visual upheaval"],
                Awe => ["grounded location scale", "slight sense of scale", "atmosphere of significance", "strong environmental grandeur", "overwhelming world scale"],
                _ => GetConceptArtBaselineBandLabels(sliderKey),
            };
        }

        if (string.Equals(conceptArtSubtype, "character-concept", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-character read", "light role cues", "clear personality read", "layered identity cues", "character-led narrative charge"],
                MotionEnergy => ["static presentation pose", "gentle stance shift", "active gesture read", "dynamic pose force", "explosive pose energy"],
                Framing => ["portrait crop", "tight figure staging", "balanced figure framing", "full-figure framing", "showcase presentation framing"],
                CameraDistance => ["intimate inspection distance", "close presentation distance", "readable figure distance", "display distance", "stand-off presentation distance"],
                AtmosphericDepth => ["flat presentation plane", "slight subject separation", "clean figure separation", "soft depth falloff", "pronounced depth falloff"],
                Chaos => ["controlled design balance", "light asymmetry", "noticeable design tension", "bold asymmetrical push", "aggressive design disruption"],
                Awe => ["grounded presence", "light visual appeal", "clear iconic presence", "commanding presence", "standout signature presence"],
                _ => GetConceptArtBaselineBandLabels(sliderKey),
            };
        }

        return GetConceptArtBaselineBandLabels(sliderKey);
    }

    private static string[] GetConceptArtBaselineBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["grounded production treatment", "light stylization", "stylized development rendering", "strong art-direction stylization", "highly stylized portfolio finish"],
            Realism => [string.Empty, "loosely realistic rendering", "moderately realistic depiction", "high visual realism", "portfolio-grade realism"],
            TextureDepth => ["minimal material texture", "light surface articulation", "clear material texture", "rich tactile material detail", "deep material articulation"],
            NarrativeDensity => ["single-read visual idea", "light story-world cues", "layered world context", "densely implied story world", "world-rich narrative charge"],
            Symbolism => ["mostly literal visual language", "subtle motif cues", "suggestive symbolic cues", "pronounced allegorical motifs", "mythic symbolic charge"],
            SurfaceAge => ["fresh drafting finish", "slight development patina", "gentle workboard wear", "noticeable board character", "time-worn reference character"],
            Framing => ["intimate framing", "tight staging", "balanced framing", "expansive staging", "showcase-scale framing"],
            BackgroundComplexity => ["minimal backdrop support", "restrained scene support", "supporting environment", "rich world support", "densely layered environment"],
            MotionEnergy => ["still moment", "gentle motion", "active scene energy", "dynamic action energy", "high kinetic momentum"],
            FocusDepth => ["deep focus clarity", "moderate depth separation", "selective focus", "shallow focus", "very shallow focus"],
            ImageCleanliness => ["raw board finish", "light refinement", "clean development finish", "polished portfolio finish", "highly polished portfolio finish"],
            DetailDensity => ["sparse detail load", "light supporting detail", "clear descriptive detail", "dense layered detail", "high-density detail load"],
            AtmosphericDepth => ["limited atmospheric depth", "slight spatial recession", "air-filled depth", "luminous depth layering", "deep atmospheric layering"],
            Chaos => ["controlled composition", "light creative restlessness", "active instability", "dynamic turbulence", "orchestrated chaos"],
            Whimsy => ["serious tone", "subtle tonal lift", "mild creative play", "strong expressive energy", "bold imaginative flourish"],
            Tension => ["low tension", "light dramatic tension", "noticeable scene tension", "strong dramatic pressure", "intense pressure"],
            Awe => ["grounded scale", "slight wonder", "atmosphere of significance", "strong sense of awe", "overwhelming grandeur"],
            Saturation => ["muted color", "restrained color", "balanced color", "rich color", "luminous color"],
            Contrast => ["low contrast", "gentle tonal separation", "balanced contrast", "crisp contrast", "striking contrast"],
            Temperature => ["cool balance", "slightly cool tone", "neutral balance", "warm balance", "heated warmth"],
            LightingIntensity => ["dim light", "soft light", "balanced illumination", "bright light", "radiant lighting"],
            CameraDistance => ["extreme close view", "close view", "mid-distance view", "wide view", "far-set view"],
            CameraAngle => ["low angle", "slightly lowered viewpoint", "eye-level angle", "slightly elevated angle", "high vantage"],
            _ => Array.Empty<string>(),
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
                "Design Callouts" when configuration.ConceptArtDesignCallouts => "callout annotations",
                "Turnaround Readability" when configuration.ConceptArtTurnaroundReadability => "turnaround-ready readability",
                "Material Breakdown" when configuration.ConceptArtMaterialBreakdown => "material-breakdown clarity",
                "Scale Reference" when configuration.ConceptArtScaleReference => "clear scale reference",
                "Worldbuilding Accents" when configuration.ConceptArtWorldbuildingAccents => "world support accents",
                "Production Notes Feel" when configuration.ConceptArtProductionNotesFeel => "board-note energy",
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

    private static string ApplyConceptArtGuardrails(string sliderKey, int value, PromptConfiguration configuration, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        if (string.Equals(sliderKey, Stylization, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.ImageCleanliness >= 61)
        {
            return "highly stylized portfolio finish";
        }

        if (string.Equals(sliderKey, BackgroundComplexity, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity >= 61)
        {
            return "densely layered environment";
        }

        if (string.Equals(sliderKey, Symbolism, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.NarrativeDensity <= 40)
        {
            return "mythic symbolic charge";
        }

        if (string.Equals(sliderKey, Contrast, StringComparison.OrdinalIgnoreCase) && value >= 81 && configuration.ImageCleanliness >= 61)
        {
            return "striking contrast";
        }

        return phrase;
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
}
