using System.Text;
using System.Text.RegularExpressions;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class PromptCompressionService
{
    private static readonly Regex TokenRegex = new(@"[\p{L}\p{N}]+(?:['\-][\p{L}\p{N}]+)*|[^\p{L}\p{N}]+", RegexOptions.Compiled);

    private static readonly HashSet<string> LongWordStopwords = new(StringComparer.OrdinalIgnoreCase)
    {
        "image",
        "scene",
        "style",
        "visual",
        "frame",
        "framing",
        "tone",
        "light",
        "lights",
        "detail",
    };

    private static readonly IReadOnlyDictionary<string, CompressionLaneProfile> Profiles =
        new Dictionary<string, CompressionLaneProfile>(StringComparer.OrdinalIgnoreCase)
        {
            [IntentModeCatalog.VintageBendName] = new(
                1,
                1,
                Roots("vintage"),
                SetOf("vintage documentary image language"),
                SetOf("vintage image language"),
                Rewrite(
                    ("vintage framing", "framing"),
                    ("vintage daylight", "daylight"),
                    ("vintage detail", "detail"),
                    ("vintage focus", "focus"))),
            [IntentModeCatalog.AnimeName] = new(
                1,
                1,
                Roots("anime", "celshaded"),
                SetOf("anime illustration language"),
                SetOf("anime image language"),
                Rewrite(
                    ("anime framing", "framing"),
                    ("anime lighting", "lighting"),
                    ("anime detail", "detail"),
                    ("cel shaded", "cel-shaded"),
                    ("cel shading", "cel-shading"))),
            [IntentModeCatalog.ChildrensBookName] = new(
                1,
                2,
                Roots("childrens", "storybook", "picturebook"),
                SetOf("children's book illustration", "storybook classic", "picturebook illustration"),
                SetOf("children's book image language"),
                Rewrite(
                    ("children's book framing", "framing"),
                    ("storybook framing", "framing"),
                    ("picturebook framing", "framing"),
                    ("children's book detail", "detail"))),
            [IntentModeCatalog.ComicBookName] = new(
                1,
                2,
                Roots("comic", "comics", "panel", "bubble", "speech"),
                SetOf("comic book illustration", "graphic storytelling", "speech bubbles", "panel framing"),
                SetOf("comic image language"),
                Rewrite(
                    ("comic framing", "framing"),
                    ("comic lighting", "lighting"),
                    ("comic detail", "detail"),
                    ("speech bubbles", "speech bubbles"))),
            [IntentModeCatalog.CinematicName] = new(
                1,
                2,
                Roots("cinematic", "filmic"),
                SetOf("cinematic film-still treatment", "cinematic film still", "film-still anchoring"),
                SetOf("cinematic image language"),
                Rewrite(
                    ("cinematic framing", "framing"),
                    ("cinematic lighting", "lighting"),
                    ("cinematic atmosphere", "atmosphere"),
                    ("cinematic daylight", "daylight"),
                    ("cinematic detail", "detail"),
                    ("cinematic focus", "focus"),
                    ("filmic framing", "framing"),
                    ("filmic lighting", "lighting"),
                    ("filmic detail", "detail"))),
            [IntentModeCatalog.PhotographyName] = new(
                1,
                2,
                Roots("photo", "photography", "photographic"),
                SetOf("portrait photography", "lifestyle editorial photography", "documentary street photography", "fine art photography", "commercial photography"),
                SetOf("photographic image language", "photography image language"),
                Rewrite(
                    ("photographic framing", "framing"),
                    ("photographic angle", "camera angle"),
                    ("photographic focus", "focus"),
                    ("photographic daylight", "daylight"),
                    ("photographic detail", "detail"),
                    ("photographic wonder", "wonder"),
                    ("photographic presence", "presence"),
                    ("photographic polish", "polish"),
                    ("available light capture", "available light"),
                    ("available-light capture", "available light"),
                    ("portrait photography", "portrait photography"),
                    ("lifestyle editorial photography", "lifestyle editorial photography"),
                    ("documentary street photography", "documentary street photography"),
                    ("fine art photography", "fine art photography"),
                    ("commercial photography", "commercial photography"))),
            [IntentModeCatalog.ProductPhotographyName] = new(
                1,
                2,
                Roots("product photography", "photography", "photographic"),
                SetOf("product photography", "studio packshot", "editorial still life", "macro product detail", "lifestyle placement"),
                SetOf("product image language", "commercial image language", "product photography image language", "product photography atmosphere", "product photography finish"),
                Rewrite(
                    ("product photography framing", "framing"),
                    ("product photography focus", "focus"),
                    ("product photography detail", "detail"),
                    ("product photography lighting", "lighting"),
                    ("product photography polish", "polish"),
                    ("product photography finish", "finish"),
                    ("commercial product photography", "commercial product presentation"))),
            [IntentModeCatalog.FoodPhotographyName] = new(
                1,
                2,
                Roots("food photography", "culinary photography"),
                SetOf(
                    "commercial-grade plated food photography",
                    "commercial-grade tabletop food photography",
                    "commercial-grade close food detail photography",
                    "commercial-grade beverage service photography",
                    "commercial-grade hospitality food campaign photography"),
                SetOf("food image language", "restaurant image language", "culinary atmosphere", "food photography image language"),
                Rewrite(
                    ("food framing", "framing"),
                    ("food focus", "focus"),
                    ("food detail", "detail"),
                    ("culinary atmosphere", string.Empty),
                    ("food photography framing", "framing"),
                    ("food photography focus", "focus"),
                    ("food photography detail", "detail"))),
            [IntentModeCatalog.LifestyleAdvertisingPhotographyName] = new(
                1,
                2,
                Roots("lifestyle photography", "advertising photography", "campaign photography"),
                SetOf(
                    "everyday lifestyle photography",
                    "premium brand campaign photography",
                    "business lifestyle photography",
                    "home life photography",
                    "wellness lifestyle photography"),
                SetOf("lifestyle image language", "advertising image language", "campaign image language", "lifestyle advertising photography"),
                Rewrite(
                    ("lifestyle photography framing", "framing"),
                    ("lifestyle photography focus", "focus"),
                    ("lifestyle photography detail", "detail"),
                    ("advertising photography framing", "framing"),
                    ("campaign photography framing", "framing"))),
            [IntentModeCatalog.ArchitectureArchvizName] = new(
                1,
                2,
                Roots("architectural visualization", "archviz"),
                SetOf(
                    "commercial-grade architectural exterior visualization",
                    "commercial-grade architectural interior visualization",
                    "commercial-grade architectural streetscape visualization",
                    "commercial-grade aerial masterplan visualization",
                    "commercial-grade twilight architectural marketing render"),
                SetOf("archviz image language", "architectural atmosphere", "architectural visualization atmosphere"),
                Rewrite(
                    ("architectural framing", "framing"),
                    ("architectural focus", "focus"),
                    ("architectural detail", "detail"),
                    ("architectural atmosphere", "spatial recession"),
                    ("archviz image language", string.Empty))),
            [IntentModeCatalog.ThreeDRenderName] = new(
                1,
                1,
                Roots("render", "rendered"),
                SetOf("3d render", "clean cgi presentation"),
                SetOf("render image language"),
                Rewrite(
                    ("render framing", "framing"),
                    ("render lighting", "lighting"),
                    ("render detail", "detail"),
                    ("render depth", "depth"),
                    ("rendered environment", "environment"),
                    ("rendered daylight", "daylight"),
                    ("rendered contrast", "contrast"))),
            [IntentModeCatalog.ConceptArtName] = new(
                1,
                2,
                Roots("concept"),
                SetOf("concept art", "production design treatment", "concept art presentation"),
                SetOf("concept image language"),
                Rewrite(
                    ("concept framing", "framing"),
                    ("concept lighting", "lighting"),
                    ("concept detail", "detail"),
                    ("concept depth", "depth"),
                    ("concept environment", "environment"))),
            [IntentModeCatalog.PixelArtName] = new(
                1,
                1,
                Roots("pixel", "pixelated"),
                SetOf("pixel art", "retro arcade"),
                SetOf("pixel image language"),
                Rewrite(
                    ("pixel framing", "framing"),
                    ("pixel lighting", "lighting"),
                    ("pixel detail", "detail"))),
            [IntentModeCatalog.WatercolorName] = new(
                1,
                1,
                Roots("watercolor", "watercolour"),
                SetOf("watercolor illustration language", "watercolour illustration language"),
                SetOf("watercolor image language", "watercolour image language"),
                Rewrite(
                    ("watercolor framing", "framing"),
                    ("watercolor daylight", "daylight"),
                    ("watercolor detail", "detail"),
                    ("watercolor focus", "focus"),
                    ("watercolour framing", "framing"),
                    ("watercolour daylight", "daylight"))),
        };

    public static bool ShouldCompress(PromptConfiguration configuration)
    {
        return IntentModeCatalog.TryGet(configuration.IntentMode, out _)
            && configuration.CompressPromptSemantics;
    }

    public static IReadOnlyList<PromptFragment> Apply(IReadOnlyList<PromptFragment> fragments, PromptConfiguration configuration)
    {
        if (!ShouldCompress(configuration) || fragments.Count == 0)
        {
            return fragments;
        }

        if (!Profiles.TryGetValue(configuration.IntentMode, out var profile))
        {
            return fragments;
        }

        var tierOne = ApplyTierOne(fragments, profile);
        if (!configuration.ReduceRepeatedLaneWords)
        {
            return tierOne;
        }

        var tierTwo = ApplyTierTwo(tierOne, profile);
        if (!configuration.TrimRepeatedLongWords)
        {
            return tierTwo;
        }

        return ApplyTierThree(tierTwo);
    }

    private static List<PromptFragment> ApplyTierOne(IReadOnlyList<PromptFragment> fragments, CompressionLaneProfile profile)
    {
        var compressed = new List<PromptFragment>(fragments.Count);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var fragment in fragments)
        {
            var cleaned = Clean(fragment.Text);
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                continue;
            }

            if (fragment.PreserveFromCompression)
            {
                AddFragment(compressed, seen, cleaned, true);
                continue;
            }

            var rewritten = ApplyExactRewrite(cleaned, profile);
            if (string.IsNullOrWhiteSpace(rewritten))
            {
                continue;
            }

            var normalized = NormalizeComparisonText(rewritten);
            if (profile.WeakMetaPhrases.Contains(normalized))
            {
                continue;
            }

            AddFragment(compressed, seen, rewritten, false);
        }

        return compressed;
    }

    private static List<PromptFragment> ApplyTierTwo(IReadOnlyList<PromptFragment> fragments, CompressionLaneProfile profile)
    {
        var compressed = new List<PromptFragment>(fragments.Count);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var rootOccurrences = 0;

        foreach (var fragment in fragments)
        {
            var cleaned = Clean(fragment.Text);
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                continue;
            }

            if (fragment.PreserveFromCompression)
            {
                AddFragment(compressed, seen, cleaned, true);
                continue;
            }

            var normalized = NormalizeComparisonText(cleaned);
            if (ContainsRootFamily(normalized, profile.RootFamilies))
            {
                if (rootOccurrences < profile.RootCap)
                {
                    rootOccurrences++;
                    AddFragment(compressed, seen, cleaned, false);
                    continue;
                }

                if (profile.ProtectedAnchors.Contains(normalized) && rootOccurrences < profile.ProtectedCap)
                {
                    rootOccurrences++;
                    AddFragment(compressed, seen, cleaned, false);
                    continue;
                }

                cleaned = StripRootFamilies(cleaned, profile.RootFamilies);
                if (string.IsNullOrWhiteSpace(cleaned))
                {
                    continue;
                }
            }

            AddFragment(compressed, seen, cleaned, false);
        }

        return compressed;
    }

    private static List<PromptFragment> ApplyTierThree(IReadOnlyList<PromptFragment> fragments)
    {
        var compressed = new List<PromptFragment>(fragments.Count);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var wordUsage = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var fragment in fragments)
        {
            var cleaned = Clean(fragment.Text);
            if (string.IsNullOrWhiteSpace(cleaned))
            {
                continue;
            }

            if (fragment.PreserveFromCompression)
            {
                AddFragment(compressed, seen, cleaned, true);
                continue;
            }

            var trimmed = TrimRepeatedLongWords(cleaned, wordUsage);
            trimmed = Clean(trimmed);
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                continue;
            }

            AddFragment(compressed, seen, trimmed, false);
        }

        return compressed;
    }

    private static string ApplyExactRewrite(string phrase, CompressionLaneProfile profile)
    {
        var rewritten = phrase;

        foreach (var entry in profile.ExactRewrites.OrderByDescending(item => item.Key.Length))
        {
            if (!rewritten.Contains(entry.Key, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            rewritten = Regex.Replace(
                rewritten,
                Regex.Escape(entry.Key),
                entry.Value,
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }

        return Clean(rewritten);
    }

    private static bool ContainsRootFamily(string normalizedPhrase, IReadOnlyList<string> rootFamilies)
    {
        return rootFamilies.Any(root => normalizedPhrase.Contains(root, StringComparison.OrdinalIgnoreCase));
    }

    private static string StripRootFamilies(string phrase, IReadOnlyList<string> rootFamilies)
    {
        var builder = new StringBuilder();

        foreach (Match match in TokenRegex.Matches(phrase))
        {
            var token = match.Value;
            if (IsWordToken(token))
            {
                var normalized = NormalizeToken(token);
                if (string.IsNullOrWhiteSpace(normalized) || rootFamilies.Any(root => normalized.Contains(root, StringComparison.OrdinalIgnoreCase)))
                {
                    continue;
                }
            }

            builder.Append(token);
        }

        return Clean(builder.ToString());
    }

    private static string TrimRepeatedLongWords(string phrase, IDictionary<string, int> wordUsage)
    {
        var builder = new StringBuilder();

        foreach (Match match in TokenRegex.Matches(phrase))
        {
            var token = match.Value;
            if (!IsWordToken(token))
            {
                builder.Append(token);
                continue;
            }

            var normalized = NormalizeToken(token);
            if (string.IsNullOrWhiteSpace(normalized)
                || normalized.Length < 5
                || normalized.All(char.IsDigit)
                || LongWordStopwords.Contains(normalized))
            {
                builder.Append(token);
                continue;
            }

            wordUsage.TryGetValue(normalized, out var usage);
            usage++;
            wordUsage[normalized] = usage;

            if (usage <= 2)
            {
                builder.Append(token);
            }
        }

        return builder.ToString();
    }

    private static void AddFragment(ICollection<PromptFragment> fragments, ISet<string> seen, string text, bool preserveFromCompression)
    {
        var cleaned = Clean(text);
        if (string.IsNullOrWhiteSpace(cleaned) || !seen.Add(cleaned))
        {
            return;
        }

        fragments.Add(new PromptFragment(cleaned, preserveFromCompression));
    }

    private static bool IsWordToken(string token)
    {
        return token.Length > 0 && char.IsLetterOrDigit(token[0]);
    }

    private static string NormalizeComparisonText(string value)
    {
        var tokens = TokenRegex.Matches(Clean(value))
            .Select(match => match.Value)
            .Where(IsWordToken)
            .Select(NormalizeToken)
            .Where(token => !string.IsNullOrWhiteSpace(token))
            .ToArray();

        return string.Join(" ", tokens);
    }

    private static string NormalizeToken(string token)
    {
        var builder = new StringBuilder(token.Length);
        foreach (var character in token)
        {
            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
            }
        }

        return builder.ToString();
    }

    private static string Clean(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var cleaned = value
            .Replace('’', '\'')
            .Replace('–', '-')
            .Replace('—', '-')
            .Trim();

        while (cleaned.Contains("  ", StringComparison.Ordinal))
        {
            cleaned = cleaned.Replace("  ", " ", StringComparison.Ordinal);
        }

        return cleaned.Trim(' ', ',', '.');
    }

    private static IReadOnlyList<string> Roots(params string[] values)
    {
        return values.Select(NormalizeToken).Where(value => !string.IsNullOrWhiteSpace(value)).ToArray();
    }

    private static IReadOnlySet<string> SetOf(params string[] values)
    {
        return new HashSet<string>(values.Select(NormalizeComparisonText).Where(value => !string.IsNullOrWhiteSpace(value)), StringComparer.OrdinalIgnoreCase);
    }

    private static IReadOnlyDictionary<string, string> Rewrite(params (string Key, string Value)[] entries)
    {
        var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var (key, value) in entries)
        {
            dictionary[Clean(key)] = value;
        }

        return dictionary;
    }

    private sealed record CompressionLaneProfile(
        int RootCap,
        int ProtectedCap,
        IReadOnlyList<string> RootFamilies,
        IReadOnlySet<string> ProtectedAnchors,
        IReadOnlySet<string> WeakMetaPhrases,
        IReadOnlyDictionary<string, string> ExactRewrites);
}
