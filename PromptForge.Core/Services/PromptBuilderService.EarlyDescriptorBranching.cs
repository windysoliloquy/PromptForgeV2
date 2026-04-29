using System.Collections.Generic;

using PromptForge.App.Models;
using PromptForge.App.Services.Lanes;

namespace PromptForge.App.Services;

public sealed partial class PromptBuilderService
{
    private static void ExecuteEarlyDescriptorBranching(
        List<PromptFragment> phrases,
        HashSet<string> seen,
        PromptConfiguration configuration,
        bool useAnimeLane,
        bool useWatercolorLane,
        bool useChildrensBookLane,
        bool useLaneContributor,
        ILanePromptContributor? laneContributor,
        bool useCinematicLane,
        bool useThreeDRenderLane,
        bool useConceptArtLane,
        bool useInfographicDataVisualizationLane,
        bool usePixelArtLane,
        bool useProductPhotographyLane,
        bool useFoodPhotographyLane,
        bool useLifestyleAdvertisingPhotographyLane,
        bool useArchitectureArchvizLane,
        bool usePhotographyLane,
        bool useVintageBend)
    {
        if (useAnimeLane)
        {
            foreach (var phrase in BuildAnimeSection(configuration))
            {
                AddUnique(phrases, seen, phrase, preserveFromCompression: true);
            }
        }
        else if (useWatercolorLane)
        {
            foreach (var phrase in BuildWatercolorSection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
        else if (useChildrensBookLane)
        {
            foreach (var phrase in BuildChildrensBookSection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
        else if (useLaneContributor)
        {
            foreach (var phrase in laneContributor!.BuildEarlyDescriptors(configuration))
            {
                AddUnique(phrases, seen, phrase.Text, phrase.PreserveFromCompression);
            }
        }
        else if (useCinematicLane)
        {
            foreach (var phrase in BuildCinematicSection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
        else if (useThreeDRenderLane)
        {
            foreach (var phrase in Build3DRenderSection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
        else if (useConceptArtLane)
        {
            foreach (var phrase in BuildConceptArtSection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
        else if (useInfographicDataVisualizationLane)
        {
            foreach (var phrase in BuildInfographicDataVisualizationSection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
        else if (usePixelArtLane)
        {
            foreach (var phrase in BuildPixelArtSection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
        else if (useProductPhotographyLane)
        {
            foreach (var phrase in BuildProductPhotographySection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
        else if (useFoodPhotographyLane)
        {
            foreach (var phrase in BuildFoodPhotographySection(configuration))
            {
                AddUnique(phrases, seen, phrase.Text, phrase.PreserveFromCompression);
            }
        }
        else if (useLifestyleAdvertisingPhotographyLane)
        {
            foreach (var phrase in BuildLifestyleAdvertisingPhotographySection(configuration))
            {
                AddUnique(phrases, seen, phrase.Text, phrase.PreserveFromCompression);
            }
        }
        else if (useArchitectureArchvizLane)
        {
            foreach (var phrase in BuildArchitectureArchvizSection(configuration))
            {
                AddUnique(phrases, seen, phrase.Text, phrase.PreserveFromCompression);
            }
        }
        else if (usePhotographyLane)
        {
            foreach (var phrase in BuildPhotographySection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
        else if (useVintageBend)
        {
            foreach (var phrase in BuildVintageBendStyleSection(configuration))
            {
                AddUnique(phrases, seen, phrase);
            }
        }
    }
}
