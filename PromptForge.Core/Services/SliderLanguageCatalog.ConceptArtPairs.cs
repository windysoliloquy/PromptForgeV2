using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static IEnumerable<PromptSemanticPairCollapse> GetConceptArtSemanticPairCollapses(PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsConceptArt(configuration.IntentMode))
        {
            yield break;
        }

        var fusedPhrase = (GetBandIndex(configuration.Temperature), GetBandIndex(configuration.LightingIntensity)) switch
        {
            (0, 0) => "cold low light",
            (0, 1) => "soft cool light",
            (0, 2) => "clear cool illumination",
            (0, 3) => "bright cool light",
            (0, 4) => "icy radiant light",

            (1, 0) => "dim blue-cast light",
            (1, 1) => "cool ambient light",
            (1, 2) => "lightly cool illumination",
            (1, 3) => "clean cool light",
            (1, 4) => "cool luminous radiance",

            (2, 0) => "dim neutral light",
            (2, 1) => "soft neutral light",
            (2, 2) => "balanced neutral illumination",
            (2, 3) => "bright neutral light",
            (2, 4) => "neutral radiant light",

            (3, 0) => "low warm light",
            (3, 1) => "soft warm light",
            (3, 2) => "warm balanced illumination",
            (3, 3) => "bright warm light",
            (3, 4) => "warm radiant light",

            (4, 0) => "ember-dim glow",
            (4, 1) => "heated soft glow",
            (4, 2) => "intense warm illumination",
            (4, 3) => "blazing warm light",
            (4, 4) => "searing radiant glow",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Temperature,
            configuration.Temperature,
            LightingIntensity,
            configuration.LightingIntensity,
            fusedPhrase,
            out var collapse))
        {
            yield return collapse;
        }

        fusedPhrase = (GetBandIndex(configuration.Saturation), GetBandIndex(configuration.Contrast)) switch
        {
            (0, 0) => "ash-soft tonal field",
            (0, 1) => "muted gentle-separation palette",
            (0, 2) => "muted balanced chroma",
            (0, 3) => "muted crisp tonal snap",
            (0, 4) => "muted hard-edged separation",

            (1, 0) => "restrained soft palette",
            (1, 1) => "restrained tonal separation",
            (1, 2) => "restrained balanced color",
            (1, 3) => "restrained clean tonal snap",
            (1, 4) => "restrained striking separation",

            (2, 0) => "soft balanced palette",
            (2, 1) => "balanced tonal separation",
            (2, 2) => "balanced chroma field",
            (2, 3) => "balanced crisp color snap",
            (2, 4) => "balanced striking separation",

            (3, 0) => "rich soft-edged color",
            (3, 1) => "rich tonal separation",
            (3, 2) => "rich balanced color",
            (3, 3) => "rich crisp snap",
            (3, 4) => "rich dramatic contrast",

            (4, 0) => "luminous bloom-soft color",
            (4, 1) => "luminous tonal separation",
            (4, 2) => "luminous chroma balance",
            (4, 3) => "luminous crisp separation",
            (4, 4) => "luminous color punch",
            _ => string.Empty,
        };

        if (TryBuildSemanticPairCollapse(
            configuration,
            Saturation,
            configuration.Saturation,
            Contrast,
            configuration.Contrast,
            fusedPhrase,
            out collapse))
        {
            yield return collapse;
        }

        if (string.Equals(configuration.ConceptArtSubtype, "keyframe-concept", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Framing), GetBandIndex(configuration.CameraDistance)) switch
            {
                (0, 0) => "claustrophobic close beat",
                (0, 1) => "tight dramatic close-up",
                (0, 2) => "compressed scene read",
                (0, 3) => "cropped event glimpse",
                (0, 4) => "telephoto scene compression",

                (1, 0) => "contained intimate staging",
                (1, 1) => "contained dramatic shot",
                (1, 2) => "contained scene read",
                (1, 3) => "contained wide event shot",
                (1, 4) => "contained long-view staging",

                (2, 0) => "balanced close scene shot",
                (2, 1) => "balanced dramatic framing",
                (2, 2) => "balanced cinematic read",
                (2, 3) => "balanced event staging",
                (2, 4) => "balanced long-view scene",

                (3, 0) => "broad close-quarters staging",
                (3, 1) => "broad dramatic staging",
                (3, 2) => "broad scene coverage",
                (3, 3) => "wide event staging",
                (3, 4) => "epic long-view staging",

                (4, 0) => "showcase close-quarters set-piece",
                (4, 1) => "showcase dramatic tableau",
                (4, 2) => "showcase cinematic tableau",
                (4, 3) => "showcase event panorama",
                (4, 4) => "showcase epic vista",
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

            fusedPhrase = (GetBandIndex(configuration.AtmosphericDepth), GetBandIndex(configuration.FocusDepth)) switch
            {
                (0, 0) => "flat fully resolved scene",
                (0, 1) => "flat broad-focus staging",
                (0, 2) => "flat subject-plane separation",
                (0, 3) => "flat dramatic focus",
                (0, 4) => "flat hero isolation",

                (1, 0) => "light-recession deep clarity",
                (1, 1) => "light-recession spatial read",
                (1, 2) => "light-recession subject separation",
                (1, 3) => "light-recession focal pull",
                (1, 4) => "light-recession hero isolation",

                (2, 0) => "clear deep-scene resolution",
                (2, 1) => "clear staged depth read",
                (2, 2) => "clear subject-plane separation",
                (2, 3) => "clear dramatic focal falloff",
                (2, 4) => "clear hero-depth isolation",

                (3, 0) => "luminous layered scene clarity",
                (3, 1) => "luminous layered spatial read",
                (3, 2) => "luminous subject separation",
                (3, 3) => "luminous dramatic focus falloff",
                (3, 4) => "luminous hero isolation",

                (4, 0) => "deep atmospheric scene clarity",
                (4, 1) => "deep atmospheric spatial read",
                (4, 2) => "deep atmospheric subject separation",
                (4, 3) => "deep atmospheric focal pull",
                (4, 4) => "deep cinematic hero isolation",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                AtmosphericDepth,
                configuration.AtmosphericDepth,
                FocusDepth,
                configuration.FocusDepth,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }
        else if (string.Equals(configuration.ConceptArtSubtype, "environment-concept", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Framing), GetBandIndex(configuration.CameraDistance)) switch
            {
                (0, 0) => "tight site close-study",
                (0, 1) => "cropped near-site read",
                (0, 2) => "cropped location read",
                (0, 3) => "cropped establishing slice",
                (0, 4) => "compressed survey crop",

                (1, 0) => "contained close-range location",
                (1, 1) => "contained site framing",
                (1, 2) => "contained environmental read",
                (1, 3) => "contained establishing view",
                (1, 4) => "contained survey view",

                (2, 0) => "balanced close location study",
                (2, 1) => "balanced near-scene framing",
                (2, 2) => "balanced environment read",
                (2, 3) => "balanced establishing view",
                (2, 4) => "balanced survey staging",

                (3, 0) => "wide close-quarters environment",
                (3, 1) => "wide near-site staging",
                (3, 2) => "wide environmental read",
                (3, 3) => "broad establishing staging",
                (3, 4) => "wide survey staging",

                (4, 0) => "panoramic close-range sweep",
                (4, 1) => "panoramic near-site sweep",
                (4, 2) => "panoramic environment read",
                (4, 3) => "panoramic establishing vista",
                (4, 4) => "panoramic survey vista",
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

            fusedPhrase = (GetBandIndex(configuration.AtmosphericDepth), GetBandIndex(configuration.FocusDepth)) switch
            {
                (0, 0) => "flat survey clarity",
                (0, 1) => "flat broad spatial read",
                (0, 2) => "flat depth-band separation",
                (0, 3) => "flat selective area emphasis",
                (0, 4) => "flat foreground isolation",

                (1, 0) => "light aerial survey read",
                (1, 1) => "light aerial spatial clarity",
                (1, 2) => "light aerial depth separation",
                (1, 3) => "light aerial focus taper",
                (1, 4) => "light aerial foreground isolation",

                (2, 0) => "clear deep-environment clarity",
                (2, 1) => "clear environmental spatial read",
                (2, 2) => "clear environmental layering",
                (2, 3) => "clear area-focused depth",
                (2, 4) => "clear foreground isolation",

                (3, 0) => "luminous layered survey read",
                (3, 1) => "luminous layered spatial clarity",
                (3, 2) => "luminous distance separation",
                (3, 3) => "luminous area-focus falloff",
                (3, 4) => "luminous foreground isolation",

                (4, 0) => "deep atmospheric survey read",
                (4, 1) => "deep atmospheric spatial clarity",
                (4, 2) => "deep perspective separation",
                (4, 3) => "deep atmospheric focus falloff",
                (4, 4) => "deep-perspective foreground isolation",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                AtmosphericDepth,
                configuration.AtmosphericDepth,
                FocusDepth,
                configuration.FocusDepth,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }
        else if (string.Equals(configuration.ConceptArtSubtype, "character-concept", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Framing), GetBandIndex(configuration.CameraDistance)) switch
            {
                (0, 0) => "intimate portrait inspection",
                (0, 1) => "tight portrait read",
                (0, 2) => "cropped figure presentation",
                (0, 3) => "compressed full-figure read",
                (0, 4) => "telephoto presentation crop",

                (1, 0) => "tight close-figure staging",
                (1, 1) => "tight presentation framing",
                (1, 2) => "tight figure read",
                (1, 3) => "tight display staging",
                (1, 4) => "tight stand-off presentation",

                (2, 0) => "balanced close figure study",
                (2, 1) => "balanced presentation framing",
                (2, 2) => "balanced figure presentation",
                (2, 3) => "balanced display read",
                (2, 4) => "balanced stand-off presentation",

                (3, 0) => "full-figure close study",
                (3, 1) => "full-figure presentation",
                (3, 2) => "full-figure readable staging",
                (3, 3) => "full-figure display framing",
                (3, 4) => "full-figure stand-off read",

                (4, 0) => "showcase close figure study",
                (4, 1) => "showcase presentation framing",
                (4, 2) => "showcase full-figure read",
                (4, 3) => "showcase display staging",
                (4, 4) => "showcase stand-off presentation",
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

            fusedPhrase = (GetBandIndex(configuration.AtmosphericDepth), GetBandIndex(configuration.FocusDepth)) switch
            {
                (0, 0) => "flat fully resolved presentation",
                (0, 1) => "flat broad subject read",
                (0, 2) => "flat selective subject emphasis",
                (0, 3) => "flat shallow subject focus",
                (0, 4) => "flat isolated hero read",

                (1, 0) => "light-separation full clarity",
                (1, 1) => "light-separation broad subject read",
                (1, 2) => "light-separation selective emphasis",
                (1, 3) => "light-separation shallow focus",
                (1, 4) => "light-separation hero isolation",

                (2, 0) => "clean subject-plane clarity",
                (2, 1) => "clean broad figure read",
                (2, 2) => "clean selective subject separation",
                (2, 3) => "clean shallow figure focus",
                (2, 4) => "clean hero isolation",

                (3, 0) => "soft-falloff full subject clarity",
                (3, 1) => "soft-falloff broad figure read",
                (3, 2) => "soft-falloff selective subject emphasis",
                (3, 3) => "soft-falloff dramatic figure focus",
                (3, 4) => "soft-falloff hero isolation",

                (4, 0) => "deep-falloff full subject clarity",
                (4, 1) => "deep-falloff broad figure read",
                (4, 2) => "deep-falloff selective subject separation",
                (4, 3) => "deep-falloff dramatic figure focus",
                (4, 4) => "deep-falloff hero isolation",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                AtmosphericDepth,
                configuration.AtmosphericDepth,
                FocusDepth,
                configuration.FocusDepth,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }
        else if (string.Equals(configuration.ConceptArtSubtype, "costume-concept", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Framing), GetBandIndex(configuration.CameraDistance)) switch
            {
                (0, 0) => "close garment inspection",
                (0, 1) => "tight garment crop",
                (0, 2) => "cropped outfit read",
                (0, 3) => "compressed costume display",
                (0, 4) => "telephoto outfit crop",

                (1, 0) => "tight close-outfit staging",
                (1, 1) => "tight costume framing",
                (1, 2) => "tight outfit read",
                (1, 3) => "tight display-costume read",
                (1, 4) => "tight stand-off costume read",

                (2, 0) => "balanced close outfit study",
                (2, 1) => "balanced costume framing",
                (2, 2) => "balanced outfit presentation",
                (2, 3) => "balanced costume display",
                (2, 4) => "balanced stand-off costume read",

                (3, 0) => "full-outfit close study",
                (3, 1) => "full-outfit presentation",
                (3, 2) => "full-outfit readable staging",
                (3, 3) => "full-outfit display framing",
                (3, 4) => "full-outfit stand-off read",

                (4, 0) => "showcase close outfit study",
                (4, 1) => "showcase costume framing",
                (4, 2) => "showcase full-outfit read",
                (4, 3) => "showcase costume display",
                (4, 4) => "showcase stand-off costume presentation",
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

            fusedPhrase = (GetBandIndex(configuration.AtmosphericDepth), GetBandIndex(configuration.FocusDepth)) switch
            {
                (0, 0) => "flat fully resolved outfit read",
                (0, 1) => "flat broad garment clarity",
                (0, 2) => "flat balanced trim separation",
                (0, 3) => "flat selective trim focus",
                (0, 4) => "flat hero-detail isolation",

                (1, 0) => "light-separation full outfit clarity",
                (1, 1) => "light-separation broad garment read",
                (1, 2) => "light-separation balanced trim separation",
                (1, 3) => "light-separation selective trim focus",
                (1, 4) => "light-separation hero-detail isolation",

                (2, 0) => "clean outfit-plane clarity",
                (2, 1) => "clean broad garment clarity",
                (2, 2) => "clean balanced detail separation",
                (2, 3) => "clean selective trim focus",
                (2, 4) => "clean hero-detail isolation",

                (3, 0) => "soft-falloff full outfit clarity",
                (3, 1) => "soft-falloff broad garment read",
                (3, 2) => "soft-falloff balanced trim separation",
                (3, 3) => "soft-falloff selective trim focus",
                (3, 4) => "soft-falloff hero-detail isolation",

                (4, 0) => "deep-falloff full outfit clarity",
                (4, 1) => "deep-falloff broad garment read",
                (4, 2) => "deep-falloff balanced trim separation",
                (4, 3) => "deep-falloff selective trim focus",
                (4, 4) => "deep-falloff hero-detail isolation",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                AtmosphericDepth,
                configuration.AtmosphericDepth,
                FocusDepth,
                configuration.FocusDepth,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }
        else if (string.Equals(configuration.ConceptArtSubtype, "prop-concept", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Framing), GetBandIndex(configuration.CameraDistance)) switch
            {
                (0, 0) => "intimate object inspection",
                (0, 1) => "tight object crop",
                (0, 2) => "cropped prop read",
                (0, 3) => "compressed display view",
                (0, 4) => "telephoto object crop",

                (1, 0) => "tight close-object staging",
                (1, 1) => "tight prop framing",
                (1, 2) => "tight object read",
                (1, 3) => "tight display-object read",
                (1, 4) => "tight stand-off object read",

                (2, 0) => "balanced close prop study",
                (2, 1) => "balanced object framing",
                (2, 2) => "balanced prop presentation",
                (2, 3) => "balanced display read",
                (2, 4) => "balanced stand-off presentation",

                (3, 0) => "full-object close study",
                (3, 1) => "full-object presentation",
                (3, 2) => "full-object readable staging",
                (3, 3) => "full-object display framing",
                (3, 4) => "full-object stand-off read",

                (4, 0) => "showcase close prop study",
                (4, 1) => "showcase prop framing",
                (4, 2) => "showcase full-object read",
                (4, 3) => "showcase display staging",
                (4, 4) => "showcase stand-off presentation",
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

            fusedPhrase = (GetBandIndex(configuration.AtmosphericDepth), GetBandIndex(configuration.FocusDepth)) switch
            {
                (0, 0) => "flat fully resolved object read",
                (0, 1) => "flat broad object clarity",
                (0, 2) => "flat balanced feature separation",
                (0, 3) => "flat selective feature focus",
                (0, 4) => "flat hero-feature isolation",

                (1, 0) => "light-separation full object clarity",
                (1, 1) => "light-separation broad object read",
                (1, 2) => "light-separation balanced feature separation",
                (1, 3) => "light-separation selective feature focus",
                (1, 4) => "light-separation hero-feature isolation",

                (2, 0) => "clean object-plane clarity",
                (2, 1) => "clean broad object read",
                (2, 2) => "clean balanced detail separation",
                (2, 3) => "clean feature-led focus",
                (2, 4) => "clean hero-feature isolation",

                (3, 0) => "soft-falloff full object clarity",
                (3, 1) => "soft-falloff broad object read",
                (3, 2) => "soft-falloff balanced feature separation",
                (3, 3) => "soft-falloff selective feature focus",
                (3, 4) => "soft-falloff hero-feature isolation",

                (4, 0) => "deep-falloff full object clarity",
                (4, 1) => "deep-falloff broad object read",
                (4, 2) => "deep-falloff balanced feature separation",
                (4, 3) => "deep-falloff selective feature focus",
                (4, 4) => "deep-falloff hero-feature isolation",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                AtmosphericDepth,
                configuration.AtmosphericDepth,
                FocusDepth,
                configuration.FocusDepth,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }
        else if (string.Equals(configuration.ConceptArtSubtype, "vehicle-concept", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Framing), GetBandIndex(configuration.CameraDistance)) switch
            {
                (0, 0) => "close structural inspection",
                (0, 1) => "tight vehicle crop",
                (0, 2) => "cropped vehicle read",
                (0, 3) => "compressed display-machine view",
                (0, 4) => "telephoto vehicle crop",

                (1, 0) => "tight near-form staging",
                (1, 1) => "tight vehicle framing",
                (1, 2) => "tight machine read",
                (1, 3) => "tight display-vehicle read",
                (1, 4) => "tight stand-off vehicle read",

                (2, 0) => "balanced close vehicle study",
                (2, 1) => "balanced machine framing",
                (2, 2) => "balanced vehicle presentation",
                (2, 3) => "balanced display-vehicle read",
                (2, 4) => "balanced stand-off vehicle presentation",

                (3, 0) => "full-vehicle close study",
                (3, 1) => "full-vehicle presentation",
                (3, 2) => "full-vehicle readable staging",
                (3, 3) => "full-vehicle display framing",
                (3, 4) => "full-vehicle stand-off read",

                (4, 0) => "showcase close vehicle study",
                (4, 1) => "showcase vehicle framing",
                (4, 2) => "showcase full-vehicle read",
                (4, 3) => "showcase vehicle display",
                (4, 4) => "showcase stand-off vehicle presentation",
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

            fusedPhrase = (GetBandIndex(configuration.AtmosphericDepth), GetBandIndex(configuration.FocusDepth)) switch
            {
                (0, 0) => "flat fully resolved machine read",
                (0, 1) => "flat broad structural clarity",
                (0, 2) => "flat balanced form separation",
                (0, 3) => "flat selective feature focus",
                (0, 4) => "flat hero-feature isolation",

                (1, 0) => "light-separation full vehicle clarity",
                (1, 1) => "light-separation broad structural read",
                (1, 2) => "light-separation balanced form separation",
                (1, 3) => "light-separation selective feature focus",
                (1, 4) => "light-separation hero-feature isolation",

                (2, 0) => "clean vehicle-plane clarity",
                (2, 1) => "clean broad machine read",
                (2, 2) => "clean balanced form separation",
                (2, 3) => "clean engineering-focus pull",
                (2, 4) => "clean hero-feature isolation",

                (3, 0) => "soft-falloff full vehicle clarity",
                (3, 1) => "soft-falloff broad structural read",
                (3, 2) => "soft-falloff balanced form separation",
                (3, 3) => "soft-falloff selective feature focus",
                (3, 4) => "soft-falloff hero-feature isolation",

                (4, 0) => "deep-falloff full vehicle clarity",
                (4, 1) => "deep-falloff broad structural read",
                (4, 2) => "deep-falloff balanced form separation",
                (4, 3) => "deep-falloff selective feature focus",
                (4, 4) => "deep-falloff hero-feature isolation",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                AtmosphericDepth,
                configuration.AtmosphericDepth,
                FocusDepth,
                configuration.FocusDepth,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }
        else if (string.Equals(configuration.ConceptArtSubtype, "creature-concept", StringComparison.Ordinal))
        {
            fusedPhrase = (GetBandIndex(configuration.Framing), GetBandIndex(configuration.CameraDistance)) switch
            {
                (0, 0) => "intimate head-study inspection",
                (0, 1) => "tight head-study crop",
                (0, 2) => "cropped creature read",
                (0, 3) => "compressed anatomy glimpse",
                (0, 4) => "telephoto specimen crop",

                (1, 0) => "tight close-form staging",
                (1, 1) => "tight creature framing",
                (1, 2) => "tight form read",
                (1, 3) => "tight display-form read",
                (1, 4) => "tight stand-off specimen read",

                (2, 0) => "balanced close anatomy study",
                (2, 1) => "balanced creature framing",
                (2, 2) => "balanced full-creature read",
                (2, 3) => "balanced display-form read",
                (2, 4) => "balanced stand-off specimen read",

                (3, 0) => "extended-body close study",
                (3, 1) => "extended-body presentation",
                (3, 2) => "extended-body readable staging",
                (3, 3) => "extended-body display framing",
                (3, 4) => "extended-body stand-off read",

                (4, 0) => "showcase close anatomy study",
                (4, 1) => "showcase creature framing",
                (4, 2) => "showcase full-creature read",
                (4, 3) => "showcase specimen display",
                (4, 4) => "showcase stand-off specimen presentation",
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

            fusedPhrase = (GetBandIndex(configuration.Tension), GetBandIndex(configuration.Awe)) switch
            {
                (0, 0) => "calm grounded presence",
                (0, 1) => "calm striking presence",
                (0, 2) => "calm apex presence",
                (0, 3) => "calm imposing grandeur",
                (0, 4) => "calm primeval grandeur",

                (1, 0) => "alert grounded restraint",
                (1, 1) => "alert striking presence",
                (1, 2) => "alert apex presence",
                (1, 3) => "alert imposing grandeur",
                (1, 4) => "alert primeval grandeur",

                (2, 0) => "grounded threat signal",
                (2, 1) => "striking threat presence",
                (2, 2) => "apex threat presence",
                (2, 3) => "imposing feral grandeur",
                (2, 4) => "primeval danger grandeur",

                (3, 0) => "grounded feral pressure",
                (3, 1) => "striking feral presence",
                (3, 2) => "commanding feral presence",
                (3, 3) => "imposing predatory grandeur",
                (3, 4) => "overwhelming feral grandeur",

                (4, 0) => "grounded immediate danger",
                (4, 1) => "striking danger presence",
                (4, 2) => "commanding apex danger",
                (4, 3) => "imposing lethal grandeur",
                (4, 4) => "overwhelming primeval terror",
                _ => string.Empty,
            };

            if (TryBuildSemanticPairCollapse(
                configuration,
                Tension,
                configuration.Tension,
                Awe,
                configuration.Awe,
                fusedPhrase,
                out collapse))
            {
                yield return collapse;
            }
        }
    }
}
