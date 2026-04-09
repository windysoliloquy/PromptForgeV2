using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static partial class SliderLanguageCatalog
{
    public static string ResolveFantasyIllustrationPhrase(string sliderKey, int value, PromptConfiguration configuration)
    {
        var labels = GetFantasyIllustrationBandLabels(sliderKey, configuration);
        return labels.Length == 0
            ? ResolveStandardPhrase(sliderKey, value, configuration)
            : MapBand(value, labels[0], labels[1], labels[2], labels[3], labels[4]);
    }

    public static string ResolveFantasyIllustrationGuideText(string sliderKey, PromptConfiguration configuration)
    {
        var labels = GetFantasyIllustrationBandLabels(sliderKey, configuration);
        return labels.Length == 0 ? ResolveDefaultGuideText(sliderKey) : string.Join("  |  ", labels);
    }

    public static IEnumerable<string> ResolveFantasyIllustrationDescriptors(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        AddFantasyIllustrationDescriptor(phrases, seen, ResolveFantasyIllustrationAnchor(configuration.FantasyIllustrationRegister));

        var registerDescriptor = ResolveFantasyIllustrationRegisterDescriptor(configuration.FantasyIllustrationRegister);
        if (!string.IsNullOrWhiteSpace(registerDescriptor))
        {
            AddFantasyIllustrationDescriptor(phrases, seen, registerDescriptor);
        }

        foreach (var phrase in ResolveFantasyIllustrationModifierDescriptors(configuration))
        {
            AddFantasyIllustrationDescriptor(phrases, seen, phrase);
        }

        return phrases;
    }

    public static string ResolveFantasyIllustrationLightingDescriptor(PromptConfiguration configuration)
    {
        return configuration.Lighting switch
        {
            "Soft daylight" => "soft daylight",
            "Golden hour" => "warm late-day light",
            "Dramatic studio light" => "controlled studio lighting",
            "Overcast" => "diffuse overcast light",
            "Moonlit" => "moonlit illumination",
            "Soft glow" => "glowing ambient light",
            "Dusk haze" => "cool twilight light",
            "Warm directional light" => "warm directional illumination",
            "Volumetric cinematic light" => "radiant legend-scale illumination",
            _ => string.Empty,
        };
    }

    private static string ResolveFantasyIllustrationAnchor(string fantasyRegister)
    {
        return NormalizeFantasyIllustrationRegister(fantasyRegister) switch
        {
            "high-magic" => "fantasy illustration",
            "magitech" => "fantasy illustration",
            "low-magic" => "fantasy illustration",
            _ => "fantasy illustration",
        };
    }

    private static string ResolveFantasyIllustrationRegisterDescriptor(string fantasyRegister)
    {
        return NormalizeFantasyIllustrationRegister(fantasyRegister) switch
        {
            "general-fantasy" => "restrained sorcery world logic",
            "high-magic" => "overt spellbound world logic",
            "magitech" => "engineered arcana world logic",
            "sword-and-sorcery" => string.Empty,
            "low-magic" => string.Empty,
            _ => "restrained sorcery world logic",
        };
    }

    private static string NormalizeFantasyIllustrationRegister(string fantasyRegister)
    {
        return fantasyRegister switch
        {
            "General Fantasy" => "general-fantasy",
            "High Magic" => "high-magic",
            "Magitech" => "magitech",
            "Sword-and-Sorcery" => "sword-and-sorcery",
            "Low Magic" => "low-magic",
            _ when string.IsNullOrWhiteSpace(fantasyRegister) => "general-fantasy",
            _ => fantasyRegister,
        };
    }

    private static void AddFantasyIllustrationDescriptor(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return;
        }

        var cleaned = phrase.Trim(' ', ',', '.');
        if (cleaned.Length > 0 && seen.Add(cleaned))
        {
            phrases.Add(cleaned);
        }
    }

    private static IEnumerable<string> ResolveFantasyIllustrationModifierDescriptors(PromptConfiguration configuration)
    {
        if (configuration.FantasyIllustrationCharacterSketch)
        {
            yield return "character-design presentation";
            yield return "figure-first staging";
            yield return "clear costume-and-gear readability";
            yield return "sketch-oriented fantasy character focus";
        }

        if (configuration.FantasyIllustrationCharacterCentric)
        {
            yield return "character-first presentation";
            yield return "subject-dominant staging";
            yield return "figure-priority composition";
        }

        if (configuration.FantasyIllustrationEnvironmentConcept)
        {
            yield return "environment concept presentation";
            yield return "world-first staging";
            yield return "location-driven fantasy scene design";
        }

        if (configuration.FantasyIllustrationKeyArt)
        {
            yield return "fantasy key art presentation";
            yield return "hero-image staging";
            yield return "high-impact promotional composition";
        }

        if (configuration.FantasyIllustrationCleanBackground)
        {
            yield return "simplified backdrop treatment";
            yield return "restrained environmental load";
            yield return "subject-forward background control";
        }

        if (configuration.FantasyIllustrationSilhouetteReadability)
        {
            yield return "clean silhouette priority";
            yield return "shape-led readability";
            yield return "strong subject separation";
        }

        if (configuration.FantasyIllustrationPhotorealistic)
        {
            yield return "photorealistic fantasy rendering";
            yield return "highly convincing material realism";
            yield return "real-world visual believability emphasis";
        }

        if (configuration.FantasyIllustrationCartoonArt)
        {
            yield return "cartoon fantasy illustration treatment";
            yield return "graphic shape-led rendering";
            yield return "simplified expressive visual language";
        }

        if (configuration.FantasyIllustrationPropArtifactFocus)
        {
            yield return "prop-first presentation";
            yield return "artifact showcase staging";
            yield return "object-centric fantasy display";
        }

        if (configuration.FantasyIllustrationCreatureDesign)
        {
            yield return "creature-design presentation";
            yield return "anatomy-and-form readability";
            yield return "species-first fantasy subject focus";
        }
    }

    private static string[] GetFantasyIllustrationBandLabels(string sliderKey, PromptConfiguration configuration)
    {
        if (configuration.FantasyIllustrationCharacterSketch)
        {
            var characterSketchLabels = GetCharacterSketchFantasyIllustrationBandLabels(sliderKey);
            if (characterSketchLabels.Length > 0)
            {
                return characterSketchLabels;
            }
        }

        return GetFantasyIllustrationBandLabels(sliderKey, configuration.FantasyIllustrationRegister);
    }

    private static string[] GetFantasyIllustrationBandLabels(string sliderKey, string fantasyRegister)
    {
        return NormalizeFantasyIllustrationRegister(fantasyRegister) switch
        {
            "low-magic" => GetLowMagicFantasyIllustrationBandLabels(sliderKey),
            "high-magic" => GetHighMagicFantasyIllustrationBandLabels(sliderKey),
            "magitech" => GetMagitechFantasyIllustrationBandLabels(sliderKey),
            "sword-and-sorcery" => GetSwordAndSorceryFantasyIllustrationBandLabels(sliderKey),
            _ => GetSharedFantasyIllustrationBandLabels(sliderKey),
        };
    }

    private static string[] GetCharacterSketchFantasyIllustrationBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["plain character sketch treatment", "lightly stylized character study", "character-design illustration treatment", "strongly shaped character concept art", "highly stylized fantasy character sheet language"],
            Realism => ["omit explicit realism", "loose believable character construction", "moderately convincing character material logic", "high physical believability in costume and anatomy", "strongly convincing fantasy character realism"],
            TextureDepth => ["flat sketch surface", "light material notes", "clear costume-and-gear texture notes", "rich fabric, armor, and prop texture", "dense material-callout texture detail"],
            SurfaceAge => ["fresh design pass", "lightly worn costume notes", "seasoned gear and garment wear", "richly weathered character equipment", "heavily aged artifact-and-costume finish"],
            Framing => ["tight portrait sketch crop", "contained bust-and-gear framing", "balanced character-study framing", "expanded full-figure framing", "turnaround-ready full-body presentation"],
            CameraDistance => ["close face-and-costume view", "near bust-and-gear view", "balanced full-character distance", "wider silhouette-and-gear distance", "full design-board distance"],
            CameraAngle => ["neutral eye-level character view", "slightly lowered figure view", "balanced model-sheet vantage", "subtle hero presentation angle", "strong display-sheet character angle"],
            BackgroundComplexity => ["blank character-sheet backdrop", "minimal grounding shape", "simple presentation backdrop", "light contextual prop setting", "controlled design-board environment"],
            MotionEnergy => ["static character pose", "subtle gesture pose", "clear readable stance", "dynamic action-ready pose", "high-energy character pose"],
            NarrativeDensity => ["single design read", "light character premise cues", "clear character-role cues", "dense costume-and-backstory evidence", "lore-heavy character-design read"],
            AtmosphericDepth => ["flat presentation space", "slight subject separation", "clean studio-like depth", "soft depth behind the figure", "dramatic subject-isolating depth"],
            Chaos => ["orderly character sheet", "slight costume asymmetry", "controlled design complexity", "busy but readable gear layout", "highly ornate character-design complexity"],
            FocusDepth => ["flat model-sheet readability", "light subject focus", "balanced figure focus", "strong character isolation", "highly selective facial-and-costume focus"],
            ImageCleanliness => ["rough sketch pass", "lightly cleaned sketch", "clean character concept finish", "refined presentation drawing", "polished character-sheet finish"],
            DetailDensity => ["spare design notation", "light costume detail", "clear costume-and-gear detail", "rich prop and garment detail", "dense character-equipment detail load"],
            Whimsy => ["serious character tone", "light folkloric charm", "playful character accent", "quirky fantasy design energy", "bold fairytale character eccentricity"],
            Tension => ["calm character presentation", "subtle alertness", "clear ready-for-action tension", "strong dramatic character pressure", "intense battle-ready character tension"],
            Awe => ["grounded character presence", "light iconic presence", "clear heroic read", "strong legendary figure presence", "mythic character presence"],
            Temperature => ["cool design-sheet temperature", "slightly cool material read", "neutral character color balance", "warm costume-and-skin warmth", "heated heroic color cast"],
            LightingIntensity => ["soft low presentation light", "gentle character lighting", "balanced character-sheet illumination", "strong display lighting", "radiant hero-character illumination"],
            Saturation => ["muted character palette", "restrained costume color", "balanced character color", "rich costume-and-prop color", "vivid fantasy character palette"],
            Contrast => ["low sketch contrast", "gentle figure separation", "balanced character contrast", "crisp silhouette-and-costume contrast", "striking character-sheet contrast"],
            _ => [],
        };
    }

    private static string[] GetSwordAndSorceryFantasyIllustrationBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["grounded steel-first treatment", "lightly pulp-shaped image styling", "hard-edged adventure illustration treatment", "strongly ruin-charged rendering", "blood-warm pulp visual language"],
            Realism => ["omit explicit realism", "loosely believable survival-world realism", "moderately convincing hard-traveled realism", "high bodily believability in a brutal ruin-world", "strongly convincing steel-and-survival realism"],
            TextureDepth => ["light hard-used surface definition", "worn battle-made material character", "clear scarred-and-weathered texture", "richly roughened tactile evidence", "dense bronze-stone-and-leather surface richness"],
            NarrativeDensity => ["single peril-charged beat", "light plunder and pursuit cues", "layered survival-driven storytelling", "dense relic-and-betrayal story evidence", "world-rich throne-and-ruin narrative density"],
            Symbolism => ["mostly literal", "subtle idol and beast-sign cues", "suggestive altar-and-crown motifs", "pronounced usurpation-and-cult symbolism", "predatory ruin-world emblematic charge"],
            SurfaceAge => ["freshly kept surfaces", "hard-traveled wear", "smoke-and-dust aging", "age-marked ruin-world surfaces", "deep plundered timewear"],
            Framing => ["intimate blade-near framing", "contained chamber-side framing", "balanced peril-world framing", "expansive ruin-and-arena framing", "sweeping throne-and-wasteland framing"],
            CameraDistance => ["close face-and-weapon view", "near chamber-scene view", "balanced figure-and-peril distance", "wider ruin-revealing distance", "far-set wasteland-and-stronghold distance"],
            CameraAngle => ["grounded eye-level vantage", "gently lowered witness vantage", "balanced danger-bearing vantage", "strong combat-low emphasis", "commanding idol-and-throne vantage"],
            BackgroundComplexity => ["minimal ruin backdrop", "restrained chamber and wild surroundings", "supporting peril-world detail", "rich temple-and-arena environment", "densely layered throne-and-ruin environment"],
            MotionEnergy => ["settled predator stillness", "blade-ready motion", "active pursuit and combat movement", "raid-and-arena momentum", "battle-swept survival force"],
            AtmosphericDepth => ["limited ruin depth", "slight dust-haze recession", "air-filled torch-and-stone depth", "smoke-held distance layering", "deep idol-shadowed recession"],
            Chaos => ["controlled peril composition", "restless ambush tension", "volatile survival energy", "raid-torn disorder", "throne-break instability"],
            FocusDepth => ["broad action readability", "lightly guided combat focus", "balanced figure-and-weapon emphasis", "strong focal isolation", "highly selective danger-focus depth"],
            ImageCleanliness => ["rough hard-lived finish", "lightly rugged presentation", "balanced hand-kept finish", "refined brutalist presentation", "polished spoil-and-steel finish"],
            DetailDensity => ["spare survival detail", "light peril-bearing detail", "clear world-supporting danger detail", "rich ruin-and-spoil density", "high throne-and-plunder detail load"],
            Whimsy => ["serious danger-charged tone", "light rogueish edge", "rough adventure play", "sly plunder-bent playfulness", "bold pulp oddity"],
            Tension => ["low immediate peril", "watchful ambush unease", "blade-and-pursuit danger", "cult-and-usurpation strain", "throne-and-blood crisis tension"],
            Awe => ["grounded scale", "slight predatory wonder", "atmosphere of ruin-charged legend", "strong sense of decadent grandeur", "overwhelming idol-and-empire sublimity"],
            LightingIntensity => ["dim torchlit illumination", "soft heat-muted lighting", "balanced fire-and-daylight illumination", "vivid furnace-and-sun illumination", "radiant gold-and-fire illumination"],
            Saturation => ["muted dust-and-leather color", "restrained smoke-softened color", "balanced bronze-and-stone color", "rich gold-and-blood warmth", "vivid pulp-forged color"],
            Contrast => ["low dust-and-shadow contrast", "gentle torch-and-stone separation", "balanced ruin-world contrast", "crisp steel-and-fire contrast", "striking idol-and-shadow contrast"],
            _ => GetSharedFantasyIllustrationBandLabels(sliderKey),
        };
    }

    private static string[] GetMagitechFantasyIllustrationBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["grounded engineered-arcana treatment", "lightly fabricated image shaping", "artificer-led illustration treatment", "strongly infrastructure-shaped rendering", "built-wonder visual language"],
            Realism => ["omit explicit realism", "loosely believable artificer realism", "moderately convincing constructed-enchantment realism", "high believability in rune-powered infrastructure", "strongly convincing magitech world realism"],
            TextureDepth => ["light fabricated surface definition", "plated material character", "clear machined-and-inscribed texture", "richly worked mechanical surface detail", "dense crystal-and-alloy surface richness"],
            NarrativeDensity => ["single engineered wonder beat", "light workshop and transit cues", "layered infrastructure storytelling", "dense overload-and-maintenance story evidence", "world-rich guild-and-grid narrative density"],
            Symbolism => ["mostly literal", "subtle control-mark cues", "suggestive guild-and-regulator motifs", "pronounced authority-and-access symbolism", "sanctioned-power emblematic charge"],
            SurfaceAge => ["freshly assembled surfaces", "service-worn finish", "maintenance-shaped wear", "long-run infrastructure patina", "field-repaired timewear"],
            Framing => ["intimate workshop framing", "contained station framing", "balanced apparatus-world framing", "expansive infrastructure framing", "sweeping transit-and-tower framing"],
            CameraDistance => ["close component-and-hand view", "near chamber-scene view", "balanced figure-and-apparatus distance", "wider infrastructure-revealing distance", "far-set civic-grid distance"],
            CameraAngle => ["grounded eye-level vantage", "gently lowered operator vantage", "balanced constructed-world vantage", "strong tower-and-bridge emphasis", "commanding grid-scale vantage"],
            BackgroundComplexity => ["minimal workshop backdrop", "restrained engineered surroundings", "supporting built-world detail", "rich relay-and-station environment", "densely layered magitech infrastructure"],
            MotionEnergy => ["settled operational stillness", "mechanism-led motion", "active transport and workshop movement", "surge-driven momentum", "full-grid kinetic force"],
            AtmosphericDepth => ["limited chamber depth", "slight conduit-lit recession", "air-filled machine-hall depth", "regulated glow depth layering", "deep power-grid atmosphere"],
            Chaos => ["controlled engineered composition", "restless load tension", "volatile routing energy", "overload-torn disorder", "grid-failure instability"],
            FocusDepth => ["broad operational readability", "lightly guided component focus", "balanced figure-and-device emphasis", "strong focal isolation", "highly selective calibration-focus depth"],
            ImageCleanliness => ["workmanlike fabricated finish", "lightly cleaned presentation", "balanced operational finish", "refined guild-built presentation", "immaculate installation-grade finish"],
            DetailDensity => ["spare structural detail", "light component-bearing detail", "clear function-supporting detail", "rich routed-and-inscribed density", "high infrastructure detail load"],
            Whimsy => ["serious engineered tone", "light workshop play", "tinkered whimsy", "artificer-bent playfulness", "exuberant gadget-lore mischief"],
            Tension => ["low operational strain", "warded instability", "charged routing tension", "breach-and-overload strain", "containment-failure crisis tension"],
            Awe => ["grounded scale", "slight engineered wonder", "atmosphere of constructed marvel", "strong sense of infrastructure grandeur", "overwhelming built-power sublimity"],
            LightingIntensity => ["dim chamber illumination", "soft regulated lighting", "balanced lamp-and-conduit illumination", "vivid grid-fed illumination", "radiant installation-scale illumination"],
            Saturation => ["muted alloy-and-ceramic color", "restrained plated color", "balanced brass-and-crystal color", "rich engineered chroma", "vivid conduit-fed color"],
            Contrast => ["low panel-and-shadow contrast", "gentle lamp-and-metal separation", "balanced constructed contrast", "crisp plated-and-glow contrast", "striking installation contrast"],
            _ => GetSharedFantasyIllustrationBandLabels(sliderKey),
        };
    }

    private static string[] GetHighMagicFantasyIllustrationBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["grounded spellworld treatment", "lightly enchanted image shaping", "sorcery-led illustration treatment", "strongly spellwrought rendering", "radiant mythworld visual language"],
            Realism => ["omit explicit realism", "loosely believable spellworld realism", "moderately convincing enchanted-world realism", "high believability in overtly magical conditions", "strongly convincing grand-magic realism"],
            TextureDepth => ["light ritual surface definition", "polished enchanted material character", "clear sigil-worked texture", "richly ornamented magical surface detail", "jewel-and-inscription surface richness"],
            NarrativeDensity => ["single mythic beat", "light spellworld story cues", "layered enchanted-world storytelling", "dense rite-and-relic story evidence", "world-rich mythcycle narrative density"],
            Symbolism => ["mostly literal", "subtle sigil cues", "suggestive ritual motifs", "pronounced spellbound allegory", "celestial covenant charge"],
            SurfaceAge => ["freshly maintained surfaces", "ceremonial wear", "seasoned spell-use patina", "age-marked enchanted surfaces", "ancient sorcerous patina"],
            Framing => ["intimate ritual framing", "contained sanctum framing", "balanced spellcourt framing", "expansive enchanted-realm framing", "sweeping mythic-citadel framing"],
            CameraDistance => ["close sigil-and-face view", "near ritual-scene view", "balanced figure-and-chamber distance", "wider spellworld-revealing distance", "far-set realm-of-power distance"],
            CameraAngle => ["grounded eye-level vantage", "gently lowered ceremonial vantage", "balanced arcane-stage vantage", "elevated mythic low-view emphasis", "towering celestial vantage"],
            BackgroundComplexity => ["minimal spellworld backdrop", "restrained enchanted surroundings", "supporting magical-world detail", "rich ritual-and-citadel environment", "densely layered sorcerous environment"],
            MotionEnergy => ["settled spellbound stillness", "drifting magical motion", "active ritual movement", "soaring mythic momentum", "spellstorm kinetic force"],
            AtmosphericDepth => ["limited enchanted depth", "slight luminous recession", "air-filled spellworld depth", "radiant magical depth layering", "deep celestial atmosphere"],
            Chaos => ["controlled ritual composition", "restless magical tension", "volatile spellforce energy", "conjuration-torn disorder", "realm-warping instability"],
            FocusDepth => ["broad ritual readability", "lightly guided spell-focus", "balanced figure-and-sigil emphasis", "strong focal isolation", "highly selective conjuration-focus depth"],
            ImageCleanliness => ["clean ceremonial finish", "lightly polished presentation", "balanced enchanted finish", "refined high-magic presentation", "immaculate sanctum-grade finish"],
            DetailDensity => ["spare ritual detail", "light spell-bearing detail", "clear world-supporting magical detail", "rich sigil-and-relic density", "high mythworld detail load"],
            Whimsy => ["serious mythic tone", "light enchanted play", "spellbright whimsy", "fey-charged playfulness", "exuberant magical mischief"],
            Tension => ["low magical peril", "ritual unease", "charged spell-conflict tension", "ward-break strain", "cataclysm-near sorcerous tension"],
            Awe => ["grounded scale", "slight spellbound wonder", "atmosphere of enchantment", "strong sense of mythic majesty", "overwhelming sorcerous grandeur"],
            LightingIntensity => ["dim sigil-lit illumination", "soft enchanted lighting", "balanced spell-and-sky illumination", "vivid ritual illumination", "radiant celestial illumination"],
            Saturation => ["muted spell-lit color", "restrained jewel-light color", "balanced enchanted color", "rich sorcerous color", "vivid mythic chroma"],
            Contrast => ["low sigil-lit contrast", "gentle ritual separation", "balanced enchanted contrast", "crisp rune-lit contrast", "striking sorcerous contrast"],
            _ => GetSharedFantasyIllustrationBandLabels(sliderKey),
        };
    }

    private static string[] GetLowMagicFantasyIllustrationBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["grounded frontier treatment", "lightly folkloric image shaping", "old-world illustration treatment", "severe legend-shaped rendering", "omen-burdened visual language"],
            Realism => ["omit explicit realism", "loosely grounded frontier realism", "moderately convincing old-world realism", "high physical believability in a hardship-marked world", "strongly convincing lived-in legend realism"],
            TextureDepth => ["light field-made texture", "worn material character", "clear hand-worked surface texture", "richly weathered tactile evidence", "heirloom-and-relic surface richness"],
            NarrativeDensity => ["single journey beat", "light borderland story cues", "layered hardship-driven storytelling", "dense relic-and-feud story evidence", "world-rich old-kingdom chronicle density"],
            Symbolism => ["mostly literal", "subtle omen cues", "suggestive saint-and-heraldry motifs", "pronounced prophecy-bearing symbolism", "ancestral covenant charge"],
            SurfaceAge => ["freshly kept surfaces", "road-worn patina", "rain-and-smoke wear", "age-marked frontier surfaces", "deep inheritance timewear"],
            Framing => ["intimate fireside framing", "contained path-side framing", "balanced old-road framing", "expansive borderland framing", "sweeping kingdom-edge framing"],
            CameraDistance => ["close face-and-relic view", "near travel-scene view", "balanced road-and-figure distance", "wider frontier-revealing distance", "far-set realm-edge distance"],
            CameraAngle => ["grounded eye-level vantage", "gently lowered witness vantage", "balanced tale-bearing vantage", "solemn low-view emphasis", "destiny-marked towering vantage"],
            BackgroundComplexity => ["minimal frontier backdrop", "restrained old-road surroundings", "supporting weathered world detail", "rich ruin-and-holding environment", "densely layered border-kingdom environment"],
            MotionEnergy => ["settled watchfire stillness", "foot-travel motion", "active road-and-rider movement", "pursuit or skirmish momentum", "battle-swept borderland force"],
            AtmosphericDepth => ["limited valley depth", "slight mist recession", "air-filled old-world depth", "weather-held distance layering", "deep legend-haunted recession"],
            Chaos => ["controlled frontier composition", "restless border tension", "volatile hardship energy", "raid-torn disorder", "realm-fracture instability"],
            FocusDepth => ["broad tale readability", "lightly guided witness focus", "balanced figure-and-relic emphasis", "strong focal isolation", "highly selective omen-focus depth"],
            ImageCleanliness => ["rough roadworn finish", "lightly rugged presentation", "balanced hand-kept finish", "refined old-world presentation", "polished heirloom-grade finish"],
            DetailDensity => ["spare survival detail", "light hardship-bearing detail", "clear world-supporting detail", "rich road-and-relic density", "high chronicle-world detail load"],
            Whimsy => ["serious fireside tone", "light folktale warmth", "rustic storybook play", "superstition-tinged whimsy", "bold fairytale oddity"],
            Tension => ["low peril", "watchful unease", "oath-and-pursuit danger", "feud-near strain", "curse-and-blood crisis tension"],
            Awe => ["grounded scale", "slight old-world wonder", "atmosphere of burdened legend", "strong sense of ancestral grandeur", "overwhelming age-and-kingdom sublimity"],
            LightingIntensity => ["dim chapel-like illumination", "soft weather-muted lighting", "balanced torch-and-daylight illumination", "vivid stormbreak illumination", "radiant omen-lit illumination"],
            Saturation => ["muted earth-and-wool color", "restrained smoke-softened color", "balanced weathered heraldic color", "rich banner-and-relic color", "vivid legend-dyed color"],
            Contrast => ["low fog-and-stone contrast", "gentle torch-and-cloak separation", "balanced weathered contrast", "crisp steel-and-shadow contrast", "striking chapel-and-fire contrast"],
            _ => GetSharedFantasyIllustrationBandLabels(sliderKey),
        };
    }

    private static string[] GetSharedFantasyIllustrationBandLabels(string sliderKey)
    {
        return sliderKey switch
        {
            Stylization => ["grounded tale-world treatment", "lightly storied image shaping", "legend-led illustration treatment", "strongly legend-shaped rendering", "myth-burdened visual language"],
            Realism => ["omit explicit realism", "loosely believable storyworld realism", "moderately convincing imaginative realism", "high physical believability in a legend-touched world", "strongly convincing lived-in fantasy realism"],
            TextureDepth => ["light material evidence", "worked surface character", "clear hand-crafted texture", "richly weathered tactile detail", "relic-grade physical surface richness"],
            NarrativeDensity => ["single tale beat", "light story cues", "layered journey storytelling", "dense quest-marked story evidence", "world-rich legend chronicle density"],
            Symbolism => ["mostly literal", "subtle omen cues", "suggestive heraldic motifs", "pronounced prophecy-bearing symbolism", "mythic emblematic charge"],
            SurfaceAge => ["freshly kept surfaces", "road-worn patina", "seasoned field wear", "age-marked kingdom surfaces", "heirloom-deep timewear"],
            Framing => ["intimate story framing", "contained narrative framing", "balanced tale-world framing", "expansive adventure framing", "sweeping legend-scale framing"],
            CameraDistance => ["close character-centric view", "near scene view", "balanced story-view distance", "wider world-revealing distance", "far-set realm-scale distance"],
            CameraAngle => ["grounded eye-level vantage", "gently lowered vantage", "balanced illustrative vantage", "heroic low-view emphasis", "towering or destiny-marked vantage"],
            BackgroundComplexity => ["minimal tale-world backdrop", "restrained story surroundings", "supporting world detail", "rich realm-building environment", "densely layered legend-land environment"],
            MotionEnergy => ["settled scene stillness", "travel-borne motion", "active story movement", "skirmish or pursuit momentum", "battle-swept kinetic force"],
            AtmosphericDepth => ["limited depth layering", "slight valley recession", "air-filled storyworld depth", "mist-held distance layering", "deep legendary perspective"],
            Chaos => ["controlled story composition", "restless adventure tension", "volatile conflict energy", "orchestrated battlefield disorder", "high realm-shaking instability"],
            FocusDepth => ["broad story readability", "lightly guided focus", "balanced subject emphasis", "strong focal isolation", "highly selective story-focus depth"],
            ImageCleanliness => ["rough field-made finish", "lightly rugged presentation", "balanced illustrative finish", "refined crafted presentation", "polished guild-grade finish"],
            DetailDensity => ["spare descriptive detail", "light story-bearing detail", "clear world-supporting detail", "rich descriptive density", "high legend-world detail load"],
            Whimsy => ["serious tale tone", "light folktale warmth", "playful storybook energy", "trickster-bent whimsy", "bold fairytale mischief"],
            Tension => ["low peril", "watchful unease", "frontier danger", "duel-near strain", "oath-and-blood crisis tension"],
            Awe => ["grounded scale", "slight old-world wonder", "atmosphere of legend", "strong sense of storied grandeur", "overwhelming mythic sublimity"],
            LightingIntensity => ["dim ambient illumination", "soft story lighting", "balanced illustrative illumination", "vivid realm-light emphasis", "radiant legend-scale illumination"],
            Saturation => ["muted earth-and-cloth color", "restrained heraldic color", "balanced kingdom color", "rich banner-and-relic color", "vivid legendary color"],
            Contrast => ["low weathered contrast", "gentle torch-and-stone separation", "balanced storyworld contrast", "crisp steel-and-cloak contrast", "striking old-world contrast"],
            _ => [],
        };
    }
}
