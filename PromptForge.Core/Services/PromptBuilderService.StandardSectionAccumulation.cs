using System.Collections.Generic;

using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed partial class PromptBuilderService
{
    private void AccumulateSharedStandardSections(
        List<PromptFragment> phrases,
        HashSet<string> seen,
        PromptConfiguration configuration,
        bool useVintageBend,
        bool useProductPhotographyLane,
        bool useFoodPhotographyLane,
        bool useLifestyleAdvertisingPhotographyLane,
        bool useArchitectureArchvizLane,
        bool usePhotographyLane,
        bool useCinematicLane,
        bool useThreeDRenderLane,
        bool useConceptArtLane,
        bool useFantasyIllustrationLane,
        NeutralBandEmissionContext neutralBandEmissionContext)
    {
        if (!useVintageBend)
        {
            foreach (var phrase in BuildStyleSection(configuration, neutralBandEmissionContext))
            {
                AddUnique(phrases, seen, phrase);
            }
        }

        foreach (var phrase in BuildCompositionSection(configuration, neutralBandEmissionContext))
        {
            AddUnique(phrases, seen, phrase);
        }

        foreach (var phrase in BuildMoodSection(configuration, neutralBandEmissionContext))
        {
            AddUnique(phrases, seen, phrase);
        }

        foreach (var phrase in BuildLightingAndColorSection(
                     configuration,
                     useVintageBend,
                     useProductPhotographyLane,
                     useFoodPhotographyLane,
                     useLifestyleAdvertisingPhotographyLane,
                     useArchitectureArchvizLane,
                     usePhotographyLane,
                     useCinematicLane,
                     useThreeDRenderLane,
                     useConceptArtLane,
                     useFantasyIllustrationLane,
                     neutralBandEmissionContext))
        {
            AddUnique(phrases, seen, phrase);
        }

        foreach (var phrase in BuildImageFinishSection(configuration, neutralBandEmissionContext))
        {
            AddUnique(phrases, seen, phrase);
        }

        foreach (var phrase in BuildOutputSection(configuration))
        {
            AddUnique(phrases, seen, phrase, preserveFromCompression: true);
        }
    }
}
