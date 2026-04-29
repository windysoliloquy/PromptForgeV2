using System.Linq;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed partial class PromptBuilderService
{
    private const string DefaultNegativePrompt = "no blurry detail, no muddy lighting, no distorted anatomy, no extra limbs, no text artifacts, no oversaturated color, no flat composition, no messy background, no poorly defined material texture";

    private static string BuildNegativePrompt(PromptConfiguration configuration)
    {
        var phrases = new List<string>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        if (IntentModeCatalog.IsVintageBend(configuration.IntentMode))
        {
            AddUnique(phrases, seen, "no hand deformity");
            AddUnique(phrases, seen, "no unstable finger anatomy");
            AddUnique(phrases, seen, "no warped object handling");
            AddUnique(phrases, seen, "no melted facial features");
            AddUnique(phrases, seen, "no unreadable facial structure");
            AddUnique(phrases, seen, "no warped limb proportions");
            AddUnique(phrases, seen, "no excessive softness obscuring structure");
            AddUnique(phrases, seen, "no muddy detail");
            AddUnique(phrases, seen, "no exaggerated sepia wash");
            AddUnique(phrases, seen, "no fake antique damage");
            AddUnique(phrases, seen, "no overprocessed vintage effects");
        }

        if (configuration.AvoidBlurryDetail)
        {
            AddUnique(phrases, seen, "no blurry detail");
            AddUnique(phrases, seen, "no low detail");
        }

        if (configuration.AvoidClutter)
        {
            AddUnique(phrases, seen, "no cluttered composition");
        }

        if (configuration.AvoidMuddyLighting)
        {
            AddUnique(phrases, seen, "no muddy lighting");
        }

        if (configuration.AvoidDistortedAnatomy && !ShouldAllowDistortedAnatomy(configuration))
        {
            AddUnique(phrases, seen, "no distorted anatomy");
        }

        if (configuration.AvoidExtraLimbs)
        {
            AddUnique(phrases, seen, "no extra limbs");
            AddUnique(phrases, seen, "no extra fingers");
        }

        if (configuration.AvoidTextArtifacts)
        {
            AddUnique(phrases, seen, "no text artifacts");
            AddUnique(phrases, seen, "no watermark");
        }

        if (configuration.AvoidOversaturation)
        {
            AddUnique(phrases, seen, "no oversaturated color");
        }

        if (configuration.AvoidFlatComposition && !ShouldAllowFlatComposition(configuration))
        {
            AddUnique(phrases, seen, "no flat composition");
        }

        if (configuration.AvoidMessyBackground)
        {
            AddUnique(phrases, seen, "no messy background");
        }

        if (configuration.AvoidWeakMaterialDefinition)
        {
            AddUnique(phrases, seen, "no poorly defined material texture");
        }

        return phrases.Count == 0 ? DefaultNegativePrompt : string.Join(", ", phrases);
    }

    private static bool ShouldAllowDistortedAnatomy(PromptConfiguration configuration)
    {
        return HasAnyActiveArtist(configuration, "Pablo Picasso", "Salvador Dali", "Salvador DalÃ­", "El Greco", "Amedeo Modigliani", "Francis Bacon", "Egon Schiele");
    }

    private static bool ShouldAllowFlatComposition(PromptConfiguration configuration)
    {
        return HasAnyActiveArtist(configuration, "Pablo Picasso");
    }

    private static bool HasAnyActiveArtist(PromptConfiguration configuration, params string[] artists)
    {
        return artists.Any(artist => HasActiveArtist(configuration.ArtistInfluencePrimary, configuration.InfluenceStrengthPrimary, artist)
            || HasActiveArtist(configuration.ArtistInfluenceSecondary, configuration.InfluenceStrengthSecondary, artist));
    }

    private static bool HasActiveArtist(string? name, int strength, string expectedArtist)
    {
        return strength > 20
            && !string.IsNullOrWhiteSpace(name)
            && string.Equals(name, expectedArtist, StringComparison.OrdinalIgnoreCase);
    }
}
