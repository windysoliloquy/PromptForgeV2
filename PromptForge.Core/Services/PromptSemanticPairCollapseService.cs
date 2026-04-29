using PromptForge.App.Models;

namespace PromptForge.App.Services;

public readonly record struct PromptSemanticPairCollapse(string FirstPhrase, string SecondPhrase, string FusedPhrase);

public static class PromptSemanticPairCollapseService
{
    public static string Apply(string prompt, PromptConfiguration configuration)
    {
        if (!configuration.SemanticPairInteractions || string.IsNullOrWhiteSpace(prompt))
        {
            return prompt;
        }

        var collapsed = prompt;

        foreach (var collapse in GetApplicableCollapses(configuration))
        {
            collapsed = ApplyExactPairCollapse(collapsed, collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
        }

        return collapsed;
    }

    private static IEnumerable<PairCollapse> GetApplicableCollapses(PromptConfiguration configuration)
    {
        if (IntentModeCatalog.IsAnime(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetAnimeSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsChildrensBook(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetChildrensBookSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsWatercolor(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetWatercolorSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsComicBook(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetComicBookSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsCinematic(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetCinematicSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsPhotography(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetPhotographySemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsProductPhotography(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetProductPhotographySemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsFoodPhotography(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetFoodPhotographySemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsLifestyleAdvertisingPhotography(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetLifestyleAdvertisingPhotographySemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsArchitectureArchviz(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetArchitectureArchvizSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsThreeDRender(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetThreeDRenderSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsConceptArt(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetConceptArtSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsPixelArt(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetPixelArtSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsFantasyIllustration(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetFantasyIllustrationSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsEditorialIllustration(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetEditorialIllustrationSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsGraphicDesign(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetGraphicDesignSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsInfographicDataVisualization(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetInfographicDataVisualizationSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
        else if (IntentModeCatalog.IsTattooArt(configuration.IntentMode))
        {
            foreach (var collapse in SliderLanguageCatalog.GetTattooArtSemanticPairCollapses(configuration))
            {
                yield return new PairCollapse(collapse.FirstPhrase, collapse.SecondPhrase, collapse.FusedPhrase);
            }
        }
    }

    private static string ApplyExactPairCollapse(string prompt, string firstPhrase, string secondPhrase, string fusedPhrase)
    {
        var fragments = prompt
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(static fragment => fragment.Trim())
            .ToList();

        var firstIndex = fragments.FindIndex(fragment => string.Equals(fragment, firstPhrase, StringComparison.Ordinal));
        if (firstIndex < 0)
        {
            return prompt;
        }

        var secondIndex = -1;
        for (var i = 0; i < fragments.Count; i++)
        {
            if (i == firstIndex)
            {
                continue;
            }

            if (string.Equals(fragments[i], secondPhrase, StringComparison.Ordinal))
            {
                secondIndex = i;
                break;
            }
        }

        if (secondIndex < 0)
        {
            return prompt;
        }

        fragments[firstIndex] = fusedPhrase;
        fragments.RemoveAt(secondIndex);

        return string.Join(", ", fragments);
    }

    private readonly record struct PairCollapse(string FirstPhrase, string SecondPhrase, string FusedPhrase);
}
