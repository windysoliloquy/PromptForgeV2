using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveComicBookPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetComicBookBandLabels(sliderKey, configuration.ComicBookStyle);
        var phrase = labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);

        return ApplyComicBookGuardrails(sliderKey, value, configuration, phrase);
    }

    public static string ResolveComicBookGuideText(string sliderKey)
    {
        var labels = GetComicBookBandLabels(sliderKey, "General Comic");

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    public static string ResolveComicBookGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetComicBookBandLabels(sliderKey, configuration.ComicBookStyle);

        return labels.Length == 0 ? string.Empty : string.Join("  |  ", labels);
    }

    private static string[] GetComicBookBandLabels(string sliderKey, string comicBookStyle)
    {
        return sliderKey switch
        {
            Stylization => comicBookStyle switch
            {
                "Superhero Comic" => ["grounded heroic illustration", "light heroic graphic shaping", "bold superhero rendering", "explosive panel stylization", "larger-than-life graphic language"],
                "Noir Comic" => ["grounded noir illustration", "restrained shadow-led shaping", "hard-boiled panel rendering", "shadow-heavy stylization", "severe noir graphic language"],
                "Graphic Novel" => ["grounded narrative illustration", "restrained graphic shaping", "mature story rendering", "controlled narrative stylization", "forceful graphic-novel language"],
                "Vintage Comic" => ["grounded print-era illustration", "light retro graphic shaping", "classic comic rendering", "halftone-era stylization", "old-print graphic language"],
                "Modern Comic" => ["grounded contemporary illustration", "sleek graphic shaping", "clean modern rendering", "sharp panel stylization", "high-clarity graphic language"],
                _ => ["grounded illustrated treatment", "light graphic shaping", "clear inked rendering", "expressive panel stylization", "bold graphic image language"],
            },
            Realism => comicBookStyle switch
            {
                "Superhero Comic" => ["simplified heroic forms", "lightly grounded heroic anatomy", "strong figure clarity", "idealized action realism", "highly controlled heroic anatomy"],
                "Noir Comic" => ["simplified noir forms", "grounded human anatomy", "structured dramatic figuration", "restrained realistic figuration", "tightly controlled noir realism"],
                "Graphic Novel" => ["simplified observed forms", "grounded anatomy", "convincing human figuration", "refined representational realism", "tightly controlled narrative realism"],
                "Vintage Comic" => ["simplified retro forms", "lightly grounded anatomy", "classic figure clarity", "polished vintage figuration", "tightly controlled old-school anatomy"],
                "Modern Comic" => ["simplified clean forms", "grounded contemporary anatomy", "precise figure clarity", "refined modern realism", "tightly controlled contemporary anatomy"],
                _ => ["simplified drawn forms", "lightly grounded anatomy", "structured figure clarity", "refined illustrative realism", "tightly controlled anatomical rendering"],
            },
            TextureDepth => comicBookStyle switch
            {
                "Superhero Comic" => ["flat print surface", "light ink grain", "visible inked surface texture", "tactile impact texture", "heavily worked action-print texture"],
                "Noir Comic" => ["matte print surface", "dry ink grain", "visible ink-and-shadow texture", "tactile shadow-rich texture", "heavily worked noir-print texture"],
                "Graphic Novel" => ["flat print surface", "light page grain", "visible ink-on-paper texture", "tactile narrative texture", "heavily worked graphic-novel texture"],
                "Vintage Comic" => ["aged print surface", "light halftone grain", "visible halftone print texture", "tactile retro print texture", "heavily worked old-print texture"],
                "Modern Comic" => ["clean print surface", "light graphic grain", "visible crisp ink texture", "tactile polished texture", "heavily worked high-clarity texture"],
                _ => ["flat print surface", "light ink grain", "visible ink-on-paper texture", "tactile ink richness", "heavily worked print texture"],
            },
            NarrativeDensity => comicBookStyle switch
            {
                "Superhero Comic" => ["single heroic beat", "light action cue", "defined conflict moment", "layered hero-versus-threat implication", "event-dense heroic escalation"],
                "Noir Comic" => ["single tense beat", "light suspicion cue", "defined investigative moment", "layered criminal implication", "tension-dense noir progression"],
                "Graphic Novel" => ["single narrative beat", "light character cue", "defined story moment", "layered narrative implication", "story-dense dramatic progression"],
                "Vintage Comic" => ["single-panel beat", "light retro story cue", "defined pulp moment", "layered issue-era implication", "story-dense serial progression"],
                "Modern Comic" => ["single sharp beat", "light story cue", "defined dramatic moment", "layered cinematic implication", "story-dense modern progression"],
                _ => ["single-panel read", "light story cue", "defined scene moment", "layered narrative implication", "story-dense panel progression"],
            },
            Symbolism => comicBookStyle switch
            {
                "Superhero Comic" => ["literal framing", "subtle heroic cue", "suggestive power motif", "pronounced mythic signal", "iconic heroic charge"],
                "Noir Comic" => ["literal framing", "subtle moral cue", "suggestive doom motif", "pronounced fatalistic signal", "heavy noir symbolism"],
                "Graphic Novel" => ["literal framing", "subtle thematic cue", "suggestive narrative motif", "pronounced symbolic signal", "heavy thematic charge"],
                "Vintage Comic" => ["literal framing", "subtle pulp cue", "suggestive emblematic motif", "pronounced retro signal", "iconic pulp-symbolic charge"],
                "Modern Comic" => ["literal framing", "subtle graphic cue", "suggestive design motif", "pronounced thematic signal", "iconic modern symbolic charge"],
                _ => ["literal framing", "subtle symbolic cue", "suggestive visual motif", "pronounced thematic signal", "heavy symbolic charge"],
            },
            SurfaceAge => ["fresh print", "faint print wear", "gentle page aging", "worn print character", "time-softened print surface"],
            Framing => ["tight panel framing", "light panel spacing", "balanced panel composition", "wide scene staging", "full-spread composition"],
            BackgroundComplexity => comicBookStyle switch
            {
                "Superhero Comic" => ["minimal backdrop", "restrained action context", "readable battle environment", "developed spectacle setting", "densely constructed action world"],
                "Noir Comic" => ["minimal backdrop", "restrained urban cues", "readable noir setting", "developed shadowed environment", "densely constructed noir city detail"],
                "Graphic Novel" => ["minimal backdrop", "restrained narrative context", "readable story environment", "developed lived-in setting", "densely constructed narrative world"],
                "Vintage Comic" => ["minimal backdrop", "restrained retro set cues", "readable pulp environment", "developed issue-era setting", "densely constructed retro world detail"],
                "Modern Comic" => ["minimal backdrop", "restrained contemporary cues", "readable modern environment", "developed cinematic setting", "densely constructed modern scene detail"],
                _ => ["minimal backdrop", "restrained scene support", "readable environment", "developed setting detail", "densely constructed setting"],
            },
            MotionEnergy => comicBookStyle switch
            {
                "Superhero Comic" => ["still heroic pose", "charged action cue", "active combat motion", "explosive impact movement", "maximum kinetic hero action"],
                "Noir Comic" => ["still tense pose", "restrained movement cue", "active dramatic motion", "sharp evasive movement", "high-pressure noir motion"],
                "Graphic Novel" => ["still narrative pose", "restrained motion cue", "active dramatic motion", "forceful scene movement", "high-pressure narrative motion"],
                "Vintage Comic" => ["still panel pose", "brisk action cue", "active pulp motion", "punchy retro movement", "high-impact serial motion"],
                "Modern Comic" => ["still graphic pose", "sleek action cue", "active contemporary motion", "sharp cinematic movement", "high-impact modern motion"],
                _ => ["still panel moment", "slight action cue", "active scene motion", "dynamic movement burst", "high-impact kinetic action"],
            },
            FocusDepth => ["broad focus", "light falloff", "balanced focus distribution", "selective subject focus", "sharp subject isolation"],
            ImageCleanliness => ["raw ink finish", "loose ink handling", "clean line finish", "polished print finish", "pristine press-ready finish"],
            DetailDensity => ["sparse line detail", "light descriptive detail", "defined detail layering", "richly built detail", "dense linework intricacy"],
            AtmosphericDepth => ["flat spatial read", "slight depth recession", "clear spatial layering", "luminous depth build", "deep atmospheric perspective"],
            Chaos => ["controlled layout", "mild visual tension", "scattered visual activity", "turbulent panel disorder", "orchestrated visual overload"],
            Whimsy => ["serious tone", "subtle playfulness", "playful exaggeration", "expressive cartoon energy", "bold exaggerated character"],
            Tension => comicBookStyle switch
            {
                "Superhero Comic" => ["low tension", "rising conflict pressure", "active heroic tension", "major confrontation intensity", "world-at-stake conflict"],
                "Noir Comic" => ["low tension", "quiet dramatic pressure", "active moral tension", "severe noir tension", "suffocating fatal pressure"],
                "Graphic Novel" => ["low tension", "quiet dramatic pressure", "active dramatic tension", "high conflict intensity", "severe emotional conflict"],
                "Vintage Comic" => ["low tension", "brisk dramatic pressure", "active pulp tension", "high serial conflict", "peak issue-ending tension"],
                "Modern Comic" => ["low tension", "sharp dramatic pressure", "active dramatic tension", "high-impact conflict", "peak modern conflict"],
                _ => ["low tension", "light dramatic pressure", "active dramatic tension", "high conflict intensity", "peak conflict escalation"],
            },
            Awe => comicBookStyle switch
            {
                "Superhero Comic" => ["grounded scale", "slight heroic lift", "sense of spectacle", "larger-than-life impact", "colossal heroic grandeur"],
                "Noir Comic" => ["grounded scale", "slight ominous lift", "sense of significance", "looming noir impact", "oppressive noir grandeur"],
                "Graphic Novel" => ["grounded scale", "slight dramatic lift", "sense of gravity", "forceful dramatic impact", "overwhelming narrative weight"],
                "Vintage Comic" => ["grounded scale", "slight pulp lift", "sense of adventure", "bold retro spectacle", "towering pulp grandeur"],
                "Modern Comic" => ["grounded scale", "slight cinematic lift", "sense of impact", "strong visual spectacle", "overwhelming modern scale"],
                _ => ["grounded scale", "slight visual lift", "sense of spectacle", "strong visual impact", "overwhelming graphic scale"],
            },
            Temperature => ["cool palette", "slightly cool balance", "neutral temperature", "warm palette", "heated tonal cast"],
            LightingIntensity => ["soft lighting", "gentle illumination", "balanced lighting", "strong illumination", "high-intensity lighting"],
            Saturation => ["muted palette", "restrained color", "balanced saturation", "rich color fill", "vivid color intensity"],
            Contrast => comicBookStyle switch
            {
                "Superhero Comic" => ["soft tonal contrast", "moderate tonal punch", "clear graphic contrast", "bold heroic contrast", "maximum impact contrast"],
                "Noir Comic" => ["soft shadow contrast", "moderate shadow separation", "clear noir contrast", "hard shadow contrast", "crushing black-and-light contrast"],
                "Graphic Novel" => ["soft tonal contrast", "moderate tonal separation", "clear dramatic contrast", "forceful graphic contrast", "severe narrative contrast"],
                "Vintage Comic" => ["soft print contrast", "moderate print separation", "clear retro contrast", "bold halftone contrast", "maximum old-print contrast"],
                "Modern Comic" => ["soft tonal contrast", "moderate clean separation", "clear modern contrast", "sharp graphic contrast", "maximum high-clarity contrast"],
                _ => ["soft tonal contrast", "moderate tonal separation", "clear tonal contrast", "strong ink contrast", "high-impact graphic contrast"],
            },
            CameraDistance => comicBookStyle switch
            {
                "Superhero Comic" => ["tight impact view", "close action view", "mid-action panel view", "wide battle view", "giant splash view"],
                "Noir Comic" => ["tight tension view", "close dramatic view", "mid-panel view", "wide noir scene view", "distant overhead scene view"],
                "Graphic Novel" => ["tight character view", "close narrative view", "mid-panel view", "wider story view", "broad scene view"],
                "Vintage Comic" => ["tight pulp view", "close issue-panel view", "mid-panel view", "broad retro scene view", "wide serial splash view"],
                "Modern Comic" => ["tight cinematic view", "close graphic view", "mid-panel view", "wide modern scene view", "widescreen splash view"],
                _ => ["tight panel view", "close panel view", "mid-panel view", "environmental panel view", "wide splash view"],
            },
            CameraAngle => comicBookStyle switch
            {
                "Superhero Comic" => ["heroic low angle", "slightly lowered action view", "eye-level action view", "elevated combat view", "soaring overhead action view"],
                "Noir Comic" => ["severe low angle", "lowered suspicious view", "eye-level dramatic view", "elevated surveillance view", "detached overhead view"],
                "Graphic Novel" => ["low dramatic angle", "slightly lowered view", "eye-level view", "elevated narrative view", "detached overhead view"],
                "Vintage Comic" => ["bold low angle", "slightly lowered pulp view", "eye-level panel view", "elevated retro view", "overhead splash view"],
                "Modern Comic" => ["sharp low angle", "slightly lowered cinematic view", "eye-level view", "elevated modern view", "overhead cinematic view"],
                _ => ["low dramatic angle", "slightly lowered view", "eye-level view", "elevated vantage", "overhead panel view"],
            },
            _ => Array.Empty<string>(),
        };
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
            "Graphic Novel" => "mature story pacing",
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

        if (string.Equals(configuration.SpeechBubbleMode, "Blank Bubbles for Later Editing", StringComparison.Ordinal))
        {
            var size = string.IsNullOrWhiteSpace(configuration.SpeechBubbleSize)
                ? "medium"
                : configuration.SpeechBubbleSize.Trim().ToLowerInvariant();
            var shape = configuration.StylizedSpeechBubbleShape ? " with stylized bubble shape" : string.Empty;

            return $"empty {size} speech bubbles for later dialogue placement{shape}";
        }

        if (SpeechBubbleDialogueAnalyzer.HasUnclearMultiSubjectDialogue(configuration.Subject, configuration.Action, configuration.Relationship) ||
            !SpeechBubbleDialogueAnalyzer.HasQuotedDialogue(configuration.Subject, configuration.Action, configuration.Relationship))
        {
            return "non-specific dialogue bubble presence";
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
}
