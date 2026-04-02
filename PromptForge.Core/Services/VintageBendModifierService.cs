using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class VintageBendModifierService
{
    public static IReadOnlyList<string> Apply(IReadOnlyList<string> phrases, PromptConfiguration configuration)
    {
        if (!IntentModeCatalog.IsVintageBend(configuration.IntentMode))
        {
            return phrases;
        }

        if (!configuration.VintageBendEasternBlocGdr
            && !configuration.VintageBendThrillerUndertone
            && !configuration.VintageBendInstitutionalAusterity
            && !configuration.VintageBendSurveillanceStateAtmosphere
            && !configuration.VintageBendPeriodArtifacts)
        {
            return phrases;
        }

        var filtered = phrases
            .Where(phrase => !ShouldSuppress(phrase, configuration))
            .ToList();

        var seen = new HashSet<string>(filtered, StringComparer.OrdinalIgnoreCase);

        if (configuration.VintageBendEasternBlocGdr)
        {
            AddUnique(filtered, seen, "East German institutional realism");
            AddUnique(filtered, seen, "socialist administrative interiors");
            AddUnique(filtered, seen, "restrained Eastern Bloc palette");
            AddUnique(filtered, seen, "utilitarian military-civic presentation");
        }

        if (configuration.VintageBendThrillerUndertone)
        {
            AddUnique(filtered, seen, "quiet political tension");
            AddUnique(filtered, seen, "understated paranoia");
            AddUnique(filtered, seen, "restrained thriller mood");
            AddUnique(filtered, seen, "human unease under institutional control");
        }

        if (configuration.VintageBendInstitutionalAusterity)
        {
            AddUnique(filtered, seen, "severe interior sparseness");
            AddUnique(filtered, seen, "functional bureaucracy");
            AddUnique(filtered, seen, "plain fluorescent practicality");
            AddUnique(filtered, seen, "modest state-office realism");
        }

        if (configuration.VintageBendSurveillanceStateAtmosphere)
        {
            AddUnique(filtered, seen, "watchful stillness");
            AddUnique(filtered, seen, "administrative secrecy");
            AddUnique(filtered, seen, "controlled social tension");
            AddUnique(filtered, seen, "subdued intelligence-service mood");
        }

        if (configuration.VintageBendPeriodArtifacts)
        {
            AddUnique(filtered, seen, "appropriate era artifacts supporting the subject, period-authentic objects and environmental details");
        }

        return filtered;
    }

    private static bool ShouldSuppress(string phrase, PromptConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return false;
        }

        if (configuration.VintageBendEasternBlocGdr
            && MatchesAny(
                phrase,
                "muted olive-and-amber palette"))
        {
            return true;
        }

        if ((configuration.VintageBendThrillerUndertone || configuration.VintageBendSurveillanceStateAtmosphere)
            && MatchesAny(
                phrase,
                "slight social lightness",
                "casual human playfulness",
                "rowdy period playfulness",
                "everyday social energy"))
        {
            return true;
        }

        if (configuration.VintageBendInstitutionalAusterity
            && MatchesAny(
                phrase,
                "balanced period warmth",
                "mild warm print bias",
                "strong tobacco-amber warmth",
                "rich film-era color",
                "vivid 1980s color intensity",
                "quiet human tension"))
        {
            return true;
        }

        if (configuration.VintageBendSurveillanceStateAtmosphere
            && MatchesAny(
                phrase,
                "single candid moment",
                "light social context",
                "casual human playfulness"))
        {
            return true;
        }

        return false;
    }

    private static bool MatchesAny(string phrase, params string[] candidates)
    {
        return candidates.Any(candidate => string.Equals(phrase, candidate, StringComparison.OrdinalIgnoreCase));
    }

    private static void AddUnique(ICollection<string> phrases, ISet<string> seen, string phrase)
    {
        if (!string.IsNullOrWhiteSpace(phrase) && seen.Add(phrase))
        {
            phrases.Add(phrase);
        }
    }
}
