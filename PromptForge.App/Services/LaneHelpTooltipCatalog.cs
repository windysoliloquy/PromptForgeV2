using System;
using System.Collections.Generic;

namespace PromptForge.App.Services;

public sealed record LaneHelpTooltipContent(
    string Principle,
    string Weak,
    string Stronger);

public static class LaneHelpTooltipCatalog
{
    public const string FooterContactPrefixText = "Need lane-specific help? Contact:";
    public const string ContactEmail = "WindySoliloquy@gmail.com";

    private static readonly IReadOnlyDictionary<string, LaneHelpTooltipContent> ByIntentName =
        new Dictionary<string, LaneHelpTooltipContent>(StringComparer.OrdinalIgnoreCase)
        {
            [IntentModeCatalog.EditorialIllustrationName] = new(
                "Editorial ideas must be visual, not abstract. If the concept cannot be physically shown, the model will guess or literalize it.",
                "A tide shaped like debt",
                "A rising tide built from stacked financial documents and collapsing currency forms"),
            [IntentModeCatalog.PhotographyName] = new(
                "Photography works best when the subject feels physically capturable through lens, light, and focus.",
                "A beautiful emotional scene",
                "A quiet portrait lit by soft window light with shallow depth of field and a restrained background"),
            [IntentModeCatalog.CinematicName] = new(
                "Cinematic prompts need a moment in progress, not just a nice-looking scene.",
                "A dramatic alley at night",
                "A lone figure pausing under a flickering streetlight as distant headlights cut through the rain"),
            [IntentModeCatalog.ConceptArtName] = new(
                "Concept Art gets stronger when the design has purpose, structure, and use-case.",
                "A cool futuristic vehicle",
                "A heavy reconnaissance vehicle built for arctic terrain with elevated suspension, sealed sensor housing, and modular cargo plating"),
            [IntentModeCatalog.AnimeName] = new(
                "Anime works best when emotion, silhouette, and pose are immediately readable.",
                "A cute anime girl",
                "A shy girl clutching her school bag with a turned-in posture and a hesitant sideways glance"),
            [IntentModeCatalog.ChildrensBookName] = new(
                "Children's Book imagery should be instantly readable, warm, and emotionally clear.",
                "A magical forest adventure",
                "A small fox holding a lantern on a friendly forest path lined with oversized glowing mushrooms"),
            [IntentModeCatalog.ComicBookName] = new(
                "Comic Book prompts need a decisive moment with strong action readability.",
                "A superhero fight",
                "A masked hero landing on a cracked rooftop as an energy blast tears past in the background"),
            [IntentModeCatalog.VintageBendName] = new(
                "Vintage Bend is strongest when the scene feels ordinary, grounded, and quietly wrong.",
                "A creepy old office",
                "A fluorescent-lit municipal office with dated furniture, pale paperwork stacks, and a receptionist watching too calmly"),
            [IntentModeCatalog.ProductPhotographyName] = new(
                "Product images improve when the object's shape, material, and selling surface are clearly staged.",
                "A nice watch on a table",
                "A brushed steel watch angled on a clean dark surface with controlled rim light catching the crystal edge"),
            [IntentModeCatalog.FoodPhotographyName] = new(
                "Food works when texture, freshness, and appetite cues are visible.",
                "A delicious burger",
                "A freshly assembled burger with glossy bun sheen, crisp lettuce edges, and melting cheese under soft directional light"),
            [IntentModeCatalog.LifestyleAdvertisingPhotographyName] = new(
                "Lifestyle imagery should sell a feeling through people, environment, and mood.",
                "Friends having fun outside",
                "A relaxed rooftop gathering at golden hour with casual movement, natural laughter, and warm city light"),
            [IntentModeCatalog.ArchitectureArchvizName] = new(
                "Architecture prompts work best when they express space, scale, and material experience.",
                "A modern house interior",
                "A double-height living space with poured concrete walls, oak flooring, and morning light tracking across the room"),
            [IntentModeCatalog.ThreeDRenderName] = new(
                "3D renders get stronger when form, material response, and lighting are explicit.",
                "A futuristic chair render",
                "A sculptural polymer chair with satin finish, beveled edges, and studio light revealing subtle surface curvature"),
            [IntentModeCatalog.PixelArtName] = new(
                "Pixel Art depends on strong shape decisions, not fine-detail language.",
                "A detailed fantasy castle",
                "A compact stone castle silhouette with bright window clusters, a readable tower shape, and limited-color dusk lighting"),
            [IntentModeCatalog.FantasyIllustrationName] = new(
                "Fantasy Illustration gets stronger when the world feels specific, lived-in, and materially believable.",
                "A magical fantasy kingdom",
                "A wind-beaten cliffside kingdom with banner towers, rope bridges, weathered stone keeps, and lantern-lit market paths"),
            [IntentModeCatalog.InfographicDataVisualizationName] = new(
                "Infographic and Data Visualization prompts work best when the message is structured, prioritized, and instantly scannable.",
                "An infographic about climate change",
                "A clean comparison graphic showing rising temperatures, emissions, and regional effects with labeled sections, strong hierarchy, and restrained icon support"),
            [IntentModeCatalog.WatercolorName] = new(
                "Watercolor improves when flow, softness, and pigment behavior are implied naturally.",
                "A pretty flower painting",
                "A loose flower study with soft wet-on-wet petal edges and pigment settling into textured paper"),
            [IntentModeCatalog.GraphicDesignName] = new(
                "Graphic Design should communicate clearly before it decorates.",
                "A stylish poster",
                "A bold event poster with one dominant headline, strong visual hierarchy, and a clear supporting information block"),
            [IntentModeCatalog.TattooArtName] = new(
                "Tattoo designs must stay readable when reduced to strong line, shape, and contrast.",
                "An intricate wolf tattoo",
                "A wolf head built from bold contour shapes, clean fur direction, and simplified shadow masses that hold at a distance"),
        };

    public static bool TryGet(string? intentName, out LaneHelpTooltipContent content)
    {
        if (!string.IsNullOrWhiteSpace(intentName) &&
            ByIntentName.TryGetValue(intentName.Trim(), out content!))
        {
            return true;
        }

        content = null!;
        return false;
    }
}
