using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveGraphicDesignPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetGraphicDesignBandLabels(sliderKey, configuration);
        return labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);
    }

    public static string ResolveGraphicDesignGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetGraphicDesignBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveGraphicDesignDescriptors(PromptConfiguration configuration)
    {
        yield return "graphic design composition";
        yield return "clear visual hierarchy";

        if (configuration.GraphicDesignMinimalLayout)
        {
            yield return "minimal layout discipline";
        }

        if (configuration.GraphicDesignBoldHierarchy)
        {
            yield return "bold visual hierarchy";
        }

        if (string.Equals(NormalizeGraphicDesignType(configuration.GraphicDesignType), "poster", StringComparison.Ordinal))
        {
            yield return "poster design composition";
        }
        else if (string.Equals(NormalizeGraphicDesignType(configuration.GraphicDesignType), "social-media", StringComparison.Ordinal))
        {
            yield return "social media graphic composition";
        }
        else if (string.Equals(NormalizeGraphicDesignType(configuration.GraphicDesignType), "cover-design", StringComparison.Ordinal))
        {
            yield return "cover design composition";
        }
        else if (string.Equals(NormalizeGraphicDesignType(configuration.GraphicDesignType), "flyer-handout", StringComparison.Ordinal))
        {
            yield return "flyer handout composition";
        }
        else if (string.Equals(NormalizeGraphicDesignType(configuration.GraphicDesignType), "brand-identity", StringComparison.Ordinal))
        {
            yield return "brand identity composition";
        }
    }

    private static string[] GetGraphicDesignBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        if (configuration.GraphicDesignBoldHierarchy)
        {
            var boldHierarchyLabels = GetBoldHierarchyGraphicDesignBandLabels(sliderKey);
            if (boldHierarchyLabels.Length != 0)
            {
                return boldHierarchyLabels;
            }
        }

        var designType = NormalizeGraphicDesignType(configuration.GraphicDesignType);

        if (string.Equals(designType, "poster", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-read headline message", "light poster messaging", "layered poster communication", "dense campaign messaging", "multi-layered poster communication"],
                Symbolism => ["mostly literal poster framing", "subtle campaign cues", "suggestive promotional motifs", "pronounced conceptual poster framing", "high-concept poster symbolism"],
                Framing => ["tight poster crop", "focused poster framing", "bold poster layout", "broad poster spread", "expansive full-poster presentation"],
                CameraDistance => ["close poster detail", "near poster read", "mid-distance poster view", "broad poster read", "far-set full-poster view"],
                BackgroundComplexity => ["minimal poster field", "restrained supporting field", "supporting poster context", "rich campaign support", "densely layered poster support"],
                MotionEnergy => ["still poster composition", "gentle directional pull", "active poster movement", "dynamic campaign force", "high-impact visual momentum"],
                AtmosphericDepth => ["flat poster depth", "slight poster recession", "controlled display layering", "luminous poster separation", "deep showcase layering"],
                Chaos => ["controlled poster discipline", "light visual friction", "restless campaign pressure", "orchestrated poster collision", "high-impact visual instability"],
                FocusDepth => ["broad poster clarity", "mild focal preference", "controlled poster hierarchy", "strong headline isolation", "razor-thin focal emphasis"],
                ImageCleanliness => ["raw poster finish", "clean poster finish", "polished poster presentation", "studio-clean campaign finish", "immaculate print-ready finish"],
                DetailDensity => ["sparse poster information", "selective support detail", "balanced poster density", "rich campaign detail", "dense promotional packing"],
                Tension => ["low poster tension", "light campaign tension", "noticeable promotional pressure", "strong headline urgency", "intense attention pressure"],
                Awe => ["grounded poster scale", "slight spectacle", "poster sense of wonder", "strong display grandeur", "overwhelming marquee presence"],
                LightingIntensity => ["subdued display light", "soft poster light", "balanced showcase light", "bright attention light", "high-impact reveal light"],
                Saturation => ["muted poster color", "restrained campaign color", "balanced poster color", "rich promotional saturation", "vivid marquee color"],
                Contrast => ["soft poster separation", "restrained campaign contrast", "balanced poster snap", "bold headline contrast", "high-impact poster contrast"],
                _ => GetGeneralGraphicDesignBandLabels(sliderKey),
            };
        }

        if (string.Equals(designType, "social-media", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-read social message", "light social messaging", "layered social communication", "dense promotional messaging", "multi-layered social communication"],
                Symbolism => ["mostly literal social framing", "subtle platform-aware cues", "suggestive promotional motifs", "pronounced campaign framing", "high-concept social symbolism"],
                Framing => ["tight social crop", "focused social framing", "bold social layout", "broad social spread", "expansive full-frame presentation"],
                CameraDistance => ["close social detail", "near social read", "mid-distance social view", "broad social read", "far-set full-frame view"],
                BackgroundComplexity => ["minimal social field", "restrained supporting field", "supporting social context", "rich campaign support", "densely layered social support"],
                MotionEnergy => ["still social composition", "gentle directional pull", "active social movement", "dynamic promotional force", "high-impact scroll momentum"],
                AtmosphericDepth => ["flat social depth", "slight display recession", "controlled display layering", "luminous separation", "deep showcase layering"],
                Chaos => ["controlled social discipline", "light visual friction", "restless promotional pressure", "orchestrated visual collision", "high-impact visual instability"],
                FocusDepth => ["broad social clarity", "mild focal preference", "controlled feed hierarchy", "strong focal isolation", "razor-thin focal emphasis"],
                ImageCleanliness => ["raw social finish", "clean social finish", "polished social presentation", "studio-clean campaign finish", "immaculate platform-ready finish"],
                DetailDensity => ["sparse social information", "selective support detail", "balanced social density", "rich campaign detail", "dense promotional packing"],
                Tension => ["low social tension", "light promotional tension", "noticeable attention pressure", "strong urgency cue", "intense scroll-stopping pressure"],
                Awe => ["grounded social scale", "slight visual lift", "social sense of wonder", "strong display presence", "overwhelming feed dominance"],
                LightingIntensity => ["subdued display light", "soft social light", "balanced showcase light", "bright attention light", "high-impact reveal light"],
                Saturation => ["muted social color", "restrained campaign color", "balanced social color", "rich promotional saturation", "vivid feed color"],
                Contrast => ["soft social separation", "restrained campaign contrast", "balanced social snap", "bold feed contrast", "high-impact social contrast"],
                _ => GetGeneralGraphicDesignBandLabels(sliderKey),
            };
        }

        if (string.Equals(designType, "cover-design", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-cover concept", "light cover messaging", "layered cover communication", "dense thematic cover messaging", "multi-layered cover concept"],
                Symbolism => ["mostly literal cover framing", "subtle thematic cues", "suggestive symbolic motifs", "pronounced conceptual cover framing", "high symbolic cover charge"],
                Framing => ["tight cover crop", "focused cover framing", "balanced cover layout", "broad cover spread", "expansive full-cover presentation"],
                CameraDistance => ["close cover detail", "near cover read", "mid-distance cover view", "broad cover read", "far-set full-cover view"],
                BackgroundComplexity => ["minimal cover field", "restrained supporting field", "supporting cover context", "rich thematic support", "densely layered cover support"],
                MotionEnergy => ["still cover composition", "gentle directional flow", "active visual movement", "dynamic thematic force", "high dramatic movement"],
                AtmosphericDepth => ["flat cover depth", "slight cover recession", "controlled spatial layering", "luminous thematic separation", "deep atmospheric layering"],
                Chaos => ["controlled cover discipline", "light visual friction", "restless thematic pressure", "orchestrated visual collision", "high dramatic instability"],
                FocusDepth => ["broad cover clarity", "mild focal preference", "controlled cover hierarchy", "strong focal isolation", "razor-thin focal emphasis"],
                ImageCleanliness => ["raw cover finish", "clean cover finish", "polished cover presentation", "studio-clean cover finish", "immaculate publication finish"],
                DetailDensity => ["sparse cover information", "selective supporting detail", "balanced cover density", "rich thematic detail", "dense cover detail packing"],
                Tension => ["low cover tension", "light thematic tension", "noticeable dramatic pressure", "strong emotional tension", "intense cover drama"],
                Awe => ["grounded cover scale", "slight wonder", "cover sense of wonder", "strong dramatic grandeur", "overwhelming cover presence"],
                LightingIntensity => ["subdued cover light", "soft cover light", "balanced presentation light", "bright dramatic light", "high-impact showcase light"],
                Saturation => ["muted cover color", "restrained thematic color", "balanced cover color", "rich dramatic saturation", "vivid cover color"],
                Contrast => ["soft cover separation", "restrained thematic contrast", "balanced cover snap", "bold focal contrast", "high-impact cover contrast"],
                _ => GetGeneralGraphicDesignBandLabels(sliderKey),
            };
        }

        if (string.Equals(designType, "flyer-handout", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single-read handout message", "light flyer messaging", "layered flyer communication", "dense informational messaging", "multi-layered handout communication"],
                Symbolism => ["mostly literal flyer framing", "subtle informational cues", "suggestive supporting motifs", "pronounced conceptual flyer framing", "high-concept informational symbolism"],
                Framing => ["tight flyer crop", "focused flyer framing", "balanced flyer layout", "broad handout spread", "expansive full-page handout presentation"],
                CameraDistance => ["close flyer detail", "near handout read", "mid-distance layout view", "broad handout read", "far-set full-page view"],
                BackgroundComplexity => ["minimal flyer field", "restrained supporting field", "supporting flyer context", "rich informational support", "densely layered handout support"],
                MotionEnergy => ["still flyer composition", "gentle directional flow", "active informational movement", "dynamic layout guidance", "high visual movement"],
                AtmosphericDepth => ["flat flyer depth", "slight page recession", "controlled layout layering", "luminous content separation", "deep presentation layering"],
                Chaos => ["controlled flyer discipline", "light layout friction", "restless informational pressure", "orchestrated visual collision", "high layout instability"],
                FocusDepth => ["broad handout clarity", "mild focal preference", "controlled section hierarchy", "strong focal grouping", "razor-thin focal emphasis"],
                ImageCleanliness => ["raw flyer finish", "clean flyer finish", "polished handout presentation", "studio-clean print finish", "immaculate publication finish"],
                DetailDensity => ["sparse flyer information", "selective support detail", "balanced informational density", "rich layered information", "dense handout detail packing"],
                Tension => ["low flyer tension", "light informational tension", "noticeable communication pressure", "strong urgency cue", "intense informational pressure"],
                Awe => ["grounded flyer scale", "slight visual lift", "informative sense of presence", "strong display presence", "overwhelming page presence"],
                LightingIntensity => ["subdued page light", "soft print light", "balanced presentation light", "bright attention light", "high-impact display light"],
                Saturation => ["muted flyer color", "restrained print color", "balanced flyer color", "rich promotional saturation", "vivid handout color"],
                Contrast => ["soft flyer separation", "restrained informational contrast", "balanced layout snap", "bold section contrast", "high-impact page contrast"],
                _ => GetGeneralGraphicDesignBandLabels(sliderKey),
            };
        }

        if (string.Equals(designType, "brand-identity", StringComparison.Ordinal))
        {
            return sliderKey switch
            {
                NarrativeDensity => ["single identity message", "light brand messaging", "layered identity communication", "dense brand-system messaging", "multi-layered identity system"],
                Symbolism => ["mostly literal identity framing", "subtle brand cues", "suggestive identity motifs", "pronounced conceptual identity framing", "high symbolic brand charge"],
                Framing => ["tight identity crop", "focused brand framing", "balanced identity layout", "broad brand spread", "expansive full-identity presentation"],
                CameraDistance => ["close identity detail", "near brand read", "mid-distance identity view", "broad brand read", "far-set full-identity view"],
                BackgroundComplexity => ["minimal identity field", "restrained supporting field", "supporting identity context", "rich brand support", "densely layered identity support"],
                MotionEnergy => ["still identity composition", "gentle directional flow", "active brand movement", "dynamic identity force", "high visual momentum"],
                AtmosphericDepth => ["flat identity depth", "slight display recession", "controlled brand layering", "luminous identity separation", "deep presentation layering"],
                Chaos => ["controlled identity discipline", "light visual tension", "restless brand pressure", "orchestrated identity collision", "high visual instability"],
                FocusDepth => ["broad identity clarity", "mild focal preference", "controlled brand hierarchy", "strong identity isolation", "razor-thin focal emphasis"],
                ImageCleanliness => ["raw identity finish", "clean brand finish", "polished identity presentation", "studio-clean brand finish", "immaculate system finish"],
                DetailDensity => ["sparse identity information", "selective support detail", "balanced identity density", "rich brand detail", "dense system detail packing"],
                Tension => ["low identity tension", "light brand tension", "noticeable positioning pressure", "strong market-facing tension", "intense identity pressure"],
                Awe => ["grounded identity scale", "slight visual lift", "quiet brand presence", "strong identity presence", "overwhelming flagship presence"],
                LightingIntensity => ["subdued identity light", "soft brand light", "balanced presentation light", "bright reveal light", "high-impact showcase light"],
                Saturation => ["muted identity color", "restrained brand color", "balanced identity color", "rich brand saturation", "vivid flagship color"],
                Contrast => ["soft identity separation", "restrained brand contrast", "balanced identity snap", "bold brand contrast", "high-impact identity contrast"],
                _ => GetGeneralGraphicDesignBandLabels(sliderKey),
            };
        }

        return GetGeneralGraphicDesignBandLabels(sliderKey);
    }

    private static string[] GetBoldHierarchyGraphicDesignBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            NarrativeDensity => ["single dominant message", "clear primary-secondary read", "structured message grouping", "stacked communication hierarchy", "commanding information hierarchy"],
            DetailDensity => ["sparse focal information", "selective priority detail", "tiered detail grouping", "dense focal support", "tightly packed information tiers"],
            BackgroundComplexity => ["blank support field", "subordinate backdrop support", "ordered support structure", "tiered backdrop support", "dense but subordinate backdrop"],
            FocusDepth => ["clear focal lock", "dominant focal preference", "tiered focal grouping", "strong primary-secondary separation", "razor-sharp focal command"],
            Framing => ["tight dominant crop", "assertive focal framing", "clear dominant-subordinate staging", "commanding layout spread", "full-frame hierarchy staging"],
            _ => [],
        };
    }

    private static string[] GetGeneralGraphicDesignBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["grounded design treatment", "light graphic stylization", "stylized design rendering", "strong graphic stylization", "highly stylized design finish"],
            Realism => ["omit explicit realism", "lightly grounded visual logic", "moderately realistic design finish", "high visual realism in designed surfaces", "strongly convincing polished design realism"],
            TextureDepth => ["minimal surface texture", "light print-surface character", "clear surface tactility", "rich graphic surface detail", "deeply worked tactile finish"],
            NarrativeDensity => ["single-read visual message", "light message layering", "layered communication cues", "dense conceptual messaging", "multi-layered communication system"],
            Symbolism => ["mostly literal message framing", "subtle conceptual cues", "suggestive symbolic motifs", "pronounced conceptual framing", "high symbolic charge"],
            SurfaceAge => ["freshly finished surface", "subtle printed wear", "gentle handled character", "aged design patina", "time-worn printed patina"],
            Framing => ["tight message crop", "focused design framing", "balanced layout framing", "broad composition spread", "expansive full-layout presentation"],
            CameraDistance => ["close design detail", "near composition read", "mid-distance layout view", "broad composition read", "far-set full-layout view"],
            CameraAngle => ["flat front-on presentation", "light presentation tilt", "balanced straight-on view", "structured display angle", "dramatic layout vantage"],
            BackgroundComplexity => ["minimal background support", "restrained supporting field", "supporting layout context", "rich design support", "densely layered support system"],
            MotionEnergy => ["still composition", "gentle directional flow", "active visual movement", "dynamic compositional force", "high kinetic visual energy"],
            AtmosphericDepth => ["flat graphic depth", "slight depth recession", "controlled spatial layering", "luminous depth separation", "deep presentation layering"],
            Chaos => ["controlled layout discipline", "light visual tension", "restless compositional pressure", "orchestrated visual collision", "high visual instability"],
            FocusDepth => ["broad compositional clarity", "mild focal preference", "controlled hierarchy focus", "strong focal isolation", "razor-thin focal emphasis"],
            ImageCleanliness => ["raw working finish", "clean design finish", "polished presentation finish", "studio-clean graphic finish", "immaculate production finish"],
            DetailDensity => ["sparse information load", "selective supporting detail", "balanced information density", "rich layered detail", "dense information packing"],
            Whimsy => ["serious tone", "subtle playfulness", "playful design tone", "strong playful energy", "bold graphic play"],
            Tension => ["low visual tension", "light design tension", "noticeable compositional tension", "strong message pressure", "intense visual tension"],
            Awe => ["grounded scale", "slight wonder", "designed sense of wonder", "strong visual grandeur", "overwhelming graphic presence"],
            Temperature => ["cool neutral cast", "tempered cool-warm balance", "balanced warmth", "heated warm cast", "hot promotional warmth"],
            LightingIntensity => ["subdued presentation light", "soft display light", "balanced design light", "bright reveal light", "high-impact showcase light"],
            Saturation => ["muted color range", "restrained color balance", "balanced color charge", "rich graphic saturation", "vivid promotional color"],
            Contrast => ["soft tonal separation", "restrained contrast structure", "balanced tonal snap", "bold hierarchy contrast", "high-impact graphic contrast"],
            _ => [],
        };
    }

    private static string NormalizeGraphicDesignType(string? value)
    {
        return value switch
        {
            null or "" => "general",
            var current when string.Equals(current, "poster", StringComparison.OrdinalIgnoreCase) => "poster",
            var current when string.Equals(current, "Poster", StringComparison.OrdinalIgnoreCase) => "poster",
            var current when string.Equals(current, "social-media", StringComparison.OrdinalIgnoreCase) => "social-media",
            var current when string.Equals(current, "Social Media Graphic", StringComparison.OrdinalIgnoreCase) => "social-media",
            var current when string.Equals(current, "cover-design", StringComparison.OrdinalIgnoreCase) => "cover-design",
            var current when string.Equals(current, "Cover Design", StringComparison.OrdinalIgnoreCase) => "cover-design",
            var current when string.Equals(current, "flyer-handout", StringComparison.OrdinalIgnoreCase) => "flyer-handout",
            var current when string.Equals(current, "Flyer / Handout", StringComparison.OrdinalIgnoreCase) => "flyer-handout",
            var current when string.Equals(current, "brand-identity", StringComparison.OrdinalIgnoreCase) => "brand-identity",
            var current when string.Equals(current, "Brand / Identity", StringComparison.OrdinalIgnoreCase) => "brand-identity",
            var current when string.Equals(current, "General Graphic Design", StringComparison.OrdinalIgnoreCase) => "general",
            _ => "general",
        };
    }
}
