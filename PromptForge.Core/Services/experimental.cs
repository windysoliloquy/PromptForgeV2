using System.Diagnostics;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

internal static class ExperimentalPromptGovernanceService
{
    private const string LightingPresetKey = "LightingPreset";

    private const int AnchorDirectThreshold = 58;
    private const int SupportDirectThreshold = 52;
    private const int ModifierDirectThreshold = 60;
    private const int FuseThreshold = 30;
    private const int TintThreshold = 18;

    private static readonly IReadOnlyList<string> GroupOrder = ["style", "composition", "lighting_color", "image_finish", "mood"];

    private static readonly IReadOnlyDictionary<int, double> BandMultipliers = new Dictionary<int, double>
    {
        [1] = 0.90,
        [2] = 0.55,
        [3] = 0.20,
        [4] = 0.75,
        [5] = 1.00,
    };

    private static readonly AssemblyGroup[] AssemblyGroupSequence =
    [
        AssemblyGroup.RenderingIdentity,
        AssemblyGroup.ViewConstruction,
        AssemblyGroup.CompositionEnvironment,
        AssemblyGroup.MotionInstability,
        AssemblyGroup.LightingColorAtmosphere,
        AssemblyGroup.MoodSymbolicTone,
        AssemblyGroup.TrailingRefiners,
    ];

    private static readonly IReadOnlyDictionary<AssemblyGroup, int> GroupCaps = new Dictionary<AssemblyGroup, int>
    {
        [AssemblyGroup.RenderingIdentity] = 2,
        [AssemblyGroup.ViewConstruction] = 2,
        [AssemblyGroup.CompositionEnvironment] = 2,
        [AssemblyGroup.MotionInstability] = 2,
        [AssemblyGroup.LightingColorAtmosphere] = 2,
        [AssemblyGroup.MoodSymbolicTone] = 2,
        [AssemblyGroup.TrailingRefiners] = 1,
    };

    private static readonly IReadOnlyDictionary<string, GroupDefinition> Groups =
        new Dictionary<string, GroupDefinition>(StringComparer.OrdinalIgnoreCase)
        {
            ["style"] = new(
                DirectCap: 2,
                FuseLeadIn: "style cues carrying",
                TintLeadIn: "stylistic tint toward",
                Precedence:
                [
                    SliderLanguageCatalog.Stylization,
                    SliderLanguageCatalog.Realism,
                    SliderLanguageCatalog.TextureDepth,
                    SliderLanguageCatalog.NarrativeDensity,
                    SliderLanguageCatalog.SurfaceAge,
                    SliderLanguageCatalog.Symbolism,
                ]),
            ["composition"] = new(
                DirectCap: 2,
                FuseLeadIn: "composition accents carrying",
                TintLeadIn: "compositional tint toward",
                Precedence:
                [
                    SliderLanguageCatalog.Framing,
                    SliderLanguageCatalog.CameraDistance,
                    SliderLanguageCatalog.MotionEnergy,
                    SliderLanguageCatalog.CameraAngle,
                    SliderLanguageCatalog.AtmosphericDepth,
                    SliderLanguageCatalog.BackgroundComplexity,
                    SliderLanguageCatalog.Chaos,
                ]),
            ["lighting_color"] = new(
                DirectCap: 2,
                FuseLeadIn: "lighting and color shaping carrying",
                TintLeadIn: "palette tint toward",
                Precedence:
                [
                    LightingPresetKey,
                    SliderLanguageCatalog.LightingIntensity,
                    SliderLanguageCatalog.Temperature,
                    SliderLanguageCatalog.Contrast,
                    SliderLanguageCatalog.Saturation,
                ]),
            ["image_finish"] = new(
                DirectCap: 2,
                FuseLeadIn: "finish cues carrying",
                TintLeadIn: "finish tint toward",
                Precedence:
                [
                    SliderLanguageCatalog.FocusDepth,
                    SliderLanguageCatalog.DetailDensity,
                    SliderLanguageCatalog.ImageCleanliness,
                ]),
            ["mood"] = new(
                DirectCap: 2,
                FuseLeadIn: "mood undertones carrying",
                TintLeadIn: "mood tint toward",
                Precedence:
                [
                    SliderLanguageCatalog.Tension,
                    SliderLanguageCatalog.Awe,
                    SliderLanguageCatalog.Whimsy,
                ]),
        };

    private static readonly IReadOnlyDictionary<string, SliderRule> Rules =
        new Dictionary<string, SliderRule>(StringComparer.OrdinalIgnoreCase)
        {
            [SliderLanguageCatalog.Stylization] = new("Stylization", SliderLanguageCatalog.Stylization, "style", SliderClass.Anchor, AuthorityTier.Top, 95, 70, 100, true, true, true, SuppressionLevel.Low, TriggerBehavior.DirectExceptNearNeutral, "rendering_identity", "rendering_identity"),
            [SliderLanguageCatalog.Realism] = new("Realism", SliderLanguageCatalog.Realism, "style", SliderClass.Anchor, AuthorityTier.Top, 95, 70, 100, true, true, true, SuppressionLevel.Low, TriggerBehavior.DirectExceptNearNeutralOrOff, "rendering_identity", "rendering_identity"),
            [SliderLanguageCatalog.TextureDepth] = new("Texture Depth", SliderLanguageCatalog.TextureDepth, "style", SliderClass.Support, AuthorityTier.Mid, 66, 35, 82, true, true, false, SuppressionLevel.Medium, TriggerBehavior.DirectAtOuterExtremesElseFuse, "material_richness", "finish_detail"),
            [SliderLanguageCatalog.NarrativeDensity] = new("Narrative Density", SliderLanguageCatalog.NarrativeDensity, "style", SliderClass.Support, AuthorityTier.Low, 50, 20, 72, true, true, true, SuppressionLevel.High, TriggerBehavior.DirectAboveNeutralElseFuseTint, "story_density", "style_modifier"),
            [SliderLanguageCatalog.Symbolism] = new("Symbolism", SliderLanguageCatalog.Symbolism, "style", SliderClass.Modifier, AuthorityTier.Low, 28, 5, 55, true, true, true, SuppressionLevel.High, TriggerBehavior.DirectOnlyUpperBands, "symbolic_read", "style_modifier"),
            [SliderLanguageCatalog.SurfaceAge] = new("Surface Age", SliderLanguageCatalog.SurfaceAge, "style", SliderClass.Modifier, AuthorityTier.Low, 34, 10, 60, true, true, false, SuppressionLevel.Medium, TriggerBehavior.DirectAtOuterExtremesElseFuse, "material_age", "finish_detail"),
            [SliderLanguageCatalog.Framing] = new("Framing", SliderLanguageCatalog.Framing, "composition", SliderClass.Anchor, AuthorityTier.Top, 90, 65, 100, true, true, false, SuppressionLevel.Low, TriggerBehavior.DirectExceptNearNeutral, "camera_clause", "camera_clause"),
            [SliderLanguageCatalog.CameraDistance] = new("Camera Distance", SliderLanguageCatalog.CameraDistance, "composition", SliderClass.Anchor, AuthorityTier.Top, 88, 65, 98, true, true, false, SuppressionLevel.Low, TriggerBehavior.DirectExceptNearNeutral, "camera_clause", "camera_clause"),
            [SliderLanguageCatalog.CameraAngle] = new("Camera Angle", SliderLanguageCatalog.CameraAngle, "composition", SliderClass.Anchor, AuthorityTier.Mid, 84, 60, 95, true, true, true, SuppressionLevel.Low, TriggerBehavior.DirectExceptNearNeutral, "camera_clause", "camera_clause"),
            [SliderLanguageCatalog.BackgroundComplexity] = new("Background Complexity", SliderLanguageCatalog.BackgroundComplexity, "composition", SliderClass.Support, AuthorityTier.Mid, 62, 35, 80, true, true, false, SuppressionLevel.Medium, TriggerBehavior.DirectAtOuterExtremesElseFuse, "scene_density", "composition_modifier"),
            [SliderLanguageCatalog.MotionEnergy] = new("Motion Energy", SliderLanguageCatalog.MotionEnergy, "composition", SliderClass.Anchor, AuthorityTier.Top, 86, 60, 97, true, true, true, SuppressionLevel.Low, TriggerBehavior.DirectExceptNearNeutral, "action_language", "action_language"),
            [SliderLanguageCatalog.AtmosphericDepth] = new("Atmospheric Depth", SliderLanguageCatalog.AtmosphericDepth, "composition", SliderClass.Support, AuthorityTier.Mid, 64, 35, 82, true, true, true, SuppressionLevel.Medium, TriggerBehavior.DirectAtUpperBandsElseFuseTint, "space_depth", "lighting_finish_bridge"),
            [SliderLanguageCatalog.Chaos] = new("Chaos", SliderLanguageCatalog.Chaos, "composition", SliderClass.Support, AuthorityTier.Low, 46, 15, 72, true, true, true, SuppressionLevel.High, TriggerBehavior.DirectOnlyUpperBands, "instability", "composition_modifier"),
            [SliderLanguageCatalog.Whimsy] = new("Whimsy", SliderLanguageCatalog.Whimsy, "mood", SliderClass.Modifier, AuthorityTier.Low, 24, 5, 50, true, true, true, SuppressionLevel.High, TriggerBehavior.DirectOnlyUpperBands, "playful_tone", "mood_modifier"),
            [SliderLanguageCatalog.Tension] = new("Tension", SliderLanguageCatalog.Tension, "mood", SliderClass.SupportModifier, AuthorityTier.Mid, 52, 20, 76, true, true, true, SuppressionLevel.Medium, TriggerBehavior.DirectWhenNoticeableOrAbove, "dramatic_pressure", "dramatic_pressure"),
            [SliderLanguageCatalog.Awe] = new("Awe", SliderLanguageCatalog.Awe, "mood", SliderClass.SupportModifier, AuthorityTier.Low, 48, 15, 74, true, true, true, SuppressionLevel.MediumHigh, TriggerBehavior.DirectAtUpperBandsElseFuseTint, "grand_wonder", "mood_modifier"),
            [LightingPresetKey] = new("Lighting Preset", LightingPresetKey, "lighting_color", SliderClass.AnchorState, AuthorityTier.Top, 98, 80, 100, true, true, true, SuppressionLevel.Low, TriggerBehavior.DirectWhenActive, "lighting_regime", "lighting_regime"),
            [SliderLanguageCatalog.Temperature] = new("Temperature", SliderLanguageCatalog.Temperature, "lighting_color", SliderClass.Anchor, AuthorityTier.Top, 88, 65, 98, true, true, true, SuppressionLevel.Low, TriggerBehavior.DirectExceptNearNeutral, "color_atmosphere", "lighting_regime"),
            [SliderLanguageCatalog.LightingIntensity] = new("Lighting Intensity", SliderLanguageCatalog.LightingIntensity, "lighting_color", SliderClass.Anchor, AuthorityTier.Top, 90, 65, 98, true, true, true, SuppressionLevel.Low, TriggerBehavior.DirectExceptNearNeutral, "illumination_force", "lighting_regime"),
            [SliderLanguageCatalog.Saturation] = new("Saturation", SliderLanguageCatalog.Saturation, "lighting_color", SliderClass.Support, AuthorityTier.Low, 54, 20, 76, true, true, true, SuppressionLevel.MediumHigh, TriggerBehavior.DirectAtOuterExtremesElseFuseTint, "color_intensity", "lighting_regime"),
            [SliderLanguageCatalog.Contrast] = new("Contrast", SliderLanguageCatalog.Contrast, "lighting_color", SliderClass.Support, AuthorityTier.Mid, 58, 25, 80, true, true, true, SuppressionLevel.Medium, TriggerBehavior.DirectAtOuterExtremesElseFuseTint, "value_separation", "lighting_finish_bridge"),
            [SliderLanguageCatalog.FocusDepth] = new("Focus / DOF", SliderLanguageCatalog.FocusDepth, "image_finish", SliderClass.AnchorSupport, AuthorityTier.Mid, 82, 55, 94, true, true, false, SuppressionLevel.Low, TriggerBehavior.DirectExceptNearBalancedAndCrowded, "reading_hierarchy", "reading_hierarchy"),
            [SliderLanguageCatalog.ImageCleanliness] = new("Image Cleanliness", SliderLanguageCatalog.ImageCleanliness, "image_finish", SliderClass.Support, AuthorityTier.Low, 56, 25, 78, true, true, true, SuppressionLevel.Medium, TriggerBehavior.DirectAtOuterExtremesElseFuseTint, "finish_polish", "finish_detail"),
            [SliderLanguageCatalog.DetailDensity] = new("Detail Density", SliderLanguageCatalog.DetailDensity, "image_finish", SliderClass.Support, AuthorityTier.Mid, 60, 30, 82, true, true, false, SuppressionLevel.Medium, TriggerBehavior.DirectAtOuterExtremesElseFuse, "information_load", "finish_detail"),
        };

    public static IReadOnlyList<string> BuildGovernedFragments(PromptConfiguration configuration)
    {
        var resolved = ResolveStates(configuration);
        return BuildGroupFragments(resolved);
    }

    private static IReadOnlyList<ResolvedSliderState> ResolveStates(PromptConfiguration configuration)
    {
        var candidates = BuildCandidates(configuration);
        var candidatesByKey = candidates.ToDictionary(candidate => candidate.Rule.SliderKey, StringComparer.OrdinalIgnoreCase);
        var resolved = new List<ResolvedSliderState>();
        var acceptedDirects = new List<ResolvedSliderState>();

        foreach (var groupKey in GroupOrder)
        {
            var groupCandidates = candidates
                .Where(candidate => string.Equals(candidate.Rule.Group, groupKey, StringComparison.OrdinalIgnoreCase))
                .OrderBy(candidate => candidate.Precedence)
                .ToArray();

            if (groupCandidates.Length == 0 || !Groups.TryGetValue(groupKey, out var group))
            {
                continue;
            }

            var groupResolved = new List<ResolvedSliderState>();

            foreach (var candidate in groupCandidates)
            {
                var resolvedState = ResolveCandidate(candidate, configuration, candidatesByKey, groupResolved, acceptedDirects, group.DirectCap);
                groupResolved.Add(resolvedState);
                resolved.Add(resolvedState);

                if (resolvedState.Mode == EmissionMode.Direct)
                {
                    acceptedDirects.Add(resolvedState);
                }
            }
        }

        return resolved;
    }

    private static List<SliderCandidate> BuildCandidates(PromptConfiguration configuration)
    {
        var candidates = new List<SliderCandidate>();

        foreach (var groupKey in GroupOrder)
        {
            if (!Groups.TryGetValue(groupKey, out var group))
            {
                continue;
            }

            for (var precedence = 0; precedence < group.Precedence.Length; precedence++)
            {
                var sliderKey = group.Precedence[precedence];
                if (!Rules.TryGetValue(sliderKey, out var rule))
                {
                    continue;
                }

                var band = ResolveBand(rule.SliderKey, configuration);
                var phrase = ResolvePhrase(rule.SliderKey, configuration);
                if (string.IsNullOrWhiteSpace(phrase))
                {
                    continue;
                }

                var multiplier = BandMultipliers[band];
                var rawWeight = (int)Math.Round(rule.BaseWeight * multiplier, MidpointRounding.AwayFromZero);
                var adjustedWeight = Math.Clamp(rawWeight, rule.MinWeight, rule.MaxWeight);

                candidates.Add(new SliderCandidate(
                    Rule: rule,
                    Band: band,
                    BaseWeight: rule.BaseWeight,
                    WeightBeforeConflicts: adjustedWeight,
                    Phrase: phrase,
                    Precedence: precedence));
            }
        }

        return candidates;
    }

    private static ResolvedSliderState ResolveCandidate(
        SliderCandidate candidate,
        PromptConfiguration configuration,
        IReadOnlyDictionary<string, SliderCandidate> candidatesByKey,
        IReadOnlyList<ResolvedSliderState> groupResolved,
        IReadOnlyList<ResolvedSliderState> acceptedDirects,
        int groupDirectCap)
    {
        var allowances = ResolveAllowances(candidate.Rule, candidate.Band, groupResolved.Count(state => state.Mode == EmissionMode.Direct));
        var score = candidate.WeightBeforeConflicts;
        var reason = ReasonCode.SurvivedAsOwner;

        var groupDirectCount = groupResolved.Count(state => state.Mode == EmissionMode.Direct);
        score += GetCrowdingPenalty(groupDirectCount, acceptedDirects.Count);
        score += GetSuppressionPenalty(candidate.Rule.Suppression, groupDirectCount);

        var ownership = EvaluateOwnership(candidate, candidatesByKey, groupResolved);
        score += ownership.Penalty;
        if (ownership.Reason is not null)
        {
            reason = ownership.Reason.Value;
        }

        var conflict = EvaluateConflicts(candidate, configuration, candidatesByKey, groupResolved, acceptedDirects);
        score += conflict.Penalty;
        if (conflict.Reason is not null)
        {
            reason = conflict.Reason.Value;
        }

        var directThreshold = GetDirectThreshold(candidate.Rule.Class);
        var effectiveMode = DetermineMode(candidate, allowances, score, directThreshold, groupDirectCap, groupResolved, candidatesByKey, ownership, conflict);

        if (candidate.Band == 3
            && effectiveMode != EmissionMode.Silent
            && !IsPreserveFirstSlider(candidate.Rule.SliderKey))
        {
            effectiveMode = allowances.Tint && score >= TintThreshold ? EmissionMode.Tint : EmissionMode.Silent;
            reason = effectiveMode == EmissionMode.Silent ? ReasonCode.NeutralQuieted : ReasonCode.DowngradedToTint;
        }

        if (effectiveMode == EmissionMode.Direct)
        {
            reason = candidate.Band == 5 || ownership.IsOwner ? ReasonCode.SurvivedAsExtreme : ReasonCode.SurvivedAsOwner;
        }
        else if (effectiveMode == EmissionMode.Fuse && reason is not ReasonCode.DowngradedToFuse)
        {
            reason = conflict.Penalty < 0 || ownership.Penalty < 0 || groupDirectCount >= groupDirectCap
                ? ReasonCode.DowngradedToFuse
                : reason;
        }
        else if (effectiveMode == EmissionMode.Tint && reason is not ReasonCode.DowngradedToTint)
        {
            reason = conflict.Penalty < 0 || ownership.Penalty < 0 || groupDirectCount >= groupDirectCap
                ? ReasonCode.DowngradedToTint
                : reason;
        }
        else if (effectiveMode == EmissionMode.Silent && reason is ReasonCode.SurvivedAsOwner)
        {
            reason = score < TintThreshold ? ReasonCode.NeutralQuieted : ReasonCode.CrowdedOut;
        }

        return new ResolvedSliderState(candidate, Math.Clamp(score, 0, candidate.Rule.MaxWeight), effectiveMode, reason, ownership.OwnerKey);
    }

    private static ModeAllowance ResolveAllowances(SliderRule rule, int band, int sameGroupDirectCount)
    {
        var direct = rule.AllowDirect;
        var fuse = rule.AllowFuse;
        var tint = rule.AllowTint;

        switch (rule.Trigger)
        {
            case TriggerBehavior.DirectExceptNearNeutral:
                direct &= band != 3;
                break;
            case TriggerBehavior.DirectExceptNearNeutralOrOff:
                direct &= band != 1 && band != 3;
                break;
            case TriggerBehavior.DirectAtOuterExtremesElseFuse:
                direct &= band is 1 or 5;
                tint = false;
                break;
            case TriggerBehavior.DirectAtOuterExtremesElseFuseTint:
                direct &= band is 1 or 5;
                break;
            case TriggerBehavior.DirectAboveNeutralElseFuseTint:
                direct &= band >= 4;
                break;
            case TriggerBehavior.DirectOnlyUpperBands:
                direct &= band >= 4;
                break;
            case TriggerBehavior.DirectAtUpperBandsElseFuseTint:
                direct &= band >= 4;
                break;
            case TriggerBehavior.DirectWhenNoticeableOrAbove:
                direct &= band >= 3;
                break;
            case TriggerBehavior.DirectWhenActive:
                direct &= band == 5;
                break;
            case TriggerBehavior.DirectExceptNearBalancedAndCrowded:
                direct &= band != 3 && (sameGroupDirectCount == 0 || band is 1 or 5);
                tint = false;
                break;
        }

        return new ModeAllowance(direct, fuse, tint);
    }

    private static OwnershipResolution EvaluateOwnership(
        SliderCandidate candidate,
        IReadOnlyDictionary<string, SliderCandidate> candidatesByKey,
        IReadOnlyList<ResolvedSliderState> groupResolved)
    {
        string? ownerKey = null;
        var penalty = 0;
        ReasonCode? reason = null;

        if (candidate.Rule.Group == "style")
        {
            ownerKey = ResolveOwnerKey(candidatesByKey, SliderLanguageCatalog.Stylization, SliderLanguageCatalog.Realism);
        }
        else if (candidate.Rule.Group == "composition")
        {
            ownerKey = ResolveOwnerKey(candidatesByKey, SliderLanguageCatalog.Framing, SliderLanguageCatalog.CameraDistance, SliderLanguageCatalog.CameraAngle, SliderLanguageCatalog.MotionEnergy);
        }
        else if (candidate.Rule.Group == "lighting_color")
        {
            ownerKey = ResolveOwnerKey(candidatesByKey, LightingPresetKey, SliderLanguageCatalog.LightingIntensity, SliderLanguageCatalog.Temperature);
        }
        else if (candidate.Rule.Group == "image_finish")
        {
            ownerKey = ResolveOwnerKey(candidatesByKey, SliderLanguageCatalog.FocusDepth);
        }
        else if (candidate.Rule.Group == "mood")
        {
            ownerKey = ResolveOwnerKey(candidatesByKey, SliderLanguageCatalog.Tension, SliderLanguageCatalog.Awe);
        }

        var earlierDirectOwner = groupResolved.FirstOrDefault(state =>
            state.Mode == EmissionMode.Direct
            && string.Equals(state.Candidate.Rule.OwnerDomain, candidate.Rule.OwnerDomain, StringComparison.OrdinalIgnoreCase));

        if (earlierDirectOwner is not null && !string.Equals(earlierDirectOwner.Candidate.Rule.SliderKey, candidate.Rule.SliderKey, StringComparison.OrdinalIgnoreCase))
        {
            penalty -= candidate.Rule.Authority switch
            {
                AuthorityTier.Top => 6,
                AuthorityTier.Mid => 12,
                _ => 18,
            };
            reason = ReasonCode.SuppressedByOwner;
        }

        var candidateIsOwner = string.Equals(ownerKey, candidate.Rule.SliderKey, StringComparison.OrdinalIgnoreCase)
            || string.Equals(candidate.Rule.Group, "style", StringComparison.OrdinalIgnoreCase)
                && (candidate.Rule.SliderKey == SliderLanguageCatalog.Stylization || candidate.Rule.SliderKey == SliderLanguageCatalog.Realism);

        if (!candidateIsOwner && ownerKey is not null)
        {
            penalty -= candidate.Rule.Authority switch
            {
                AuthorityTier.Top => 5,
                AuthorityTier.Mid => 10,
                _ => 14,
            };

            if (reason is null)
            {
                reason = ReasonCode.SuppressedByOwner;
            }
        }

        if (ownerKey is not null
            && IsDistinctPadCompanion(candidate.Rule.SliderKey, ownerKey)
            && IsPreserveFirstSlider(candidate.Rule.SliderKey))
        {
            penalty = Math.Max(penalty, -4);
            reason = penalty < 0 ? ReasonCode.DowngradedToFuse : reason;
        }

        return new OwnershipResolution(candidateIsOwner, ownerKey, penalty, reason);
    }

    private static ConflictResolution EvaluateConflicts(
        SliderCandidate candidate,
        PromptConfiguration configuration,
        IReadOnlyDictionary<string, SliderCandidate> candidatesByKey,
        IReadOnlyList<ResolvedSliderState> groupResolved,
        IReadOnlyList<ResolvedSliderState> acceptedDirects)
    {
        var penalty = 0;
        ReasonCode? reason = null;
        var directCount = groupResolved.Count(state => state.Mode == EmissionMode.Direct);

        if (candidate.Rule.Authority == AuthorityTier.Low && directCount >= 1)
        {
            penalty -= 8;
            reason ??= ReasonCode.CrowdedOut;
        }
        else if (candidate.Rule.Authority == AuthorityTier.Mid && directCount >= 2)
        {
            penalty -= 6;
            reason ??= ReasonCode.CrowdedOut;
        }

        var styleRealism = GetCandidate(candidatesByKey, SliderLanguageCatalog.Realism);
        var styleStylization = GetCandidate(candidatesByKey, SliderLanguageCatalog.Stylization);
        var whimsy = GetCandidate(candidatesByKey, SliderLanguageCatalog.Whimsy);
        var symbolism = GetCandidate(candidatesByKey, SliderLanguageCatalog.Symbolism);
        var chaos = GetCandidate(candidatesByKey, SliderLanguageCatalog.Chaos);
        var narrative = GetCandidate(candidatesByKey, SliderLanguageCatalog.NarrativeDensity);
        var awe = GetCandidate(candidatesByKey, SliderLanguageCatalog.Awe);
        var motion = GetCandidate(candidatesByKey, SliderLanguageCatalog.MotionEnergy);
        var tension = GetCandidate(candidatesByKey, SliderLanguageCatalog.Tension);
        var cameraDistance = GetCandidate(candidatesByKey, SliderLanguageCatalog.CameraDistance);
        var framing = GetCandidate(candidatesByKey, SliderLanguageCatalog.Framing);
        var lightingPreset = GetCandidate(candidatesByKey, LightingPresetKey);
        var temperature = GetCandidate(candidatesByKey, SliderLanguageCatalog.Temperature);
        var intensity = GetCandidate(candidatesByKey, SliderLanguageCatalog.LightingIntensity);
        var atmosphere = GetCandidate(candidatesByKey, SliderLanguageCatalog.AtmosphericDepth);
        var focus = GetCandidate(candidatesByKey, SliderLanguageCatalog.FocusDepth);
        var detail = GetCandidate(candidatesByKey, SliderLanguageCatalog.DetailDensity);
        var cleanliness = GetCandidate(candidatesByKey, SliderLanguageCatalog.ImageCleanliness);
        var surfaceAge = GetCandidate(candidatesByKey, SliderLanguageCatalog.SurfaceAge);
        var texture = GetCandidate(candidatesByKey, SliderLanguageCatalog.TextureDepth);

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Whimsy && styleRealism is not null && styleRealism.Band >= 4)
        {
            if (candidate.Band == 4)
            {
                penalty -= 20;
                reason ??= ReasonCode.DowngradedToTint;
            }
            else if (candidate.Band == 5 && (styleStylization is null || styleStylization.Band < 4))
            {
                penalty -= 36;
                reason ??= ReasonCode.SuppressedByConflict;
            }
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Symbolism && styleRealism is not null && styleRealism.Band >= 4)
        {
            if (candidate.Band == 4)
            {
                penalty -= 18;
                reason ??= ReasonCode.DowngradedToFuse;
            }
            else if (candidate.Band == 5 && (narrative is null || narrative.Band < 4) && (awe is null || awe.Band < 4))
            {
                penalty -= 14;
                reason ??= ReasonCode.DowngradedToTint;
            }
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Chaos && styleRealism is not null && styleRealism.Band >= 4)
        {
            if ((motion is null || motion.Band < 4) && (tension is null || tension.Band < 4))
            {
                penalty -= 20;
                reason ??= ReasonCode.SuppressedByConflict;
            }
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.BackgroundComplexity && cameraDistance is not null && cameraDistance.Band <= 2 && candidate.Band < 5)
        {
            penalty -= 10;
            reason ??= ReasonCode.DowngradedToFuse;
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Chaos)
        {
            var calmComposition = (framing is not null && framing.Band <= 2)
                || (cameraDistance is not null && cameraDistance.Band <= 2)
                || (motion is not null && motion.Band <= 2);

            if (candidate.Band == 4 && calmComposition)
            {
                penalty -= 12;
                reason ??= ReasonCode.DowngradedToFuse;
            }

            if (motion is not null && motion.Band >= 4)
            {
                penalty -= 12;
                reason ??= ReasonCode.DowngradedToFuse;
            }
            else if (candidate.Band == 5 && (motion is null || motion.Band <= 2) && (tension is null || tension.Band < 4))
            {
                penalty -= 18;
                reason ??= ReasonCode.SuppressedByConflict;
            }
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Temperature && lightingPreset is not null && !IsClearlyComplementaryToLightingPreset(SliderLanguageCatalog.Temperature, candidate.Band, configuration.Lighting))
        {
            penalty -= 18;
            reason ??= ReasonCode.SuppressedByConflict;
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.LightingIntensity && lightingPreset is not null && !IsClearlyComplementaryToLightingPreset(SliderLanguageCatalog.LightingIntensity, candidate.Band, configuration.Lighting))
        {
            penalty -= 15;
            reason ??= ReasonCode.SuppressedByConflict;
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Saturation && temperature is not null && temperature.Band >= 4 && candidate.Band < 5)
        {
            penalty -= 10;
            reason ??= ReasonCode.DowngradedToFuse;
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Contrast && atmosphere is not null && atmosphere.Band >= 4 && intensity is not null && intensity.Band <= 2)
        {
            penalty -= 10;
            reason ??= ReasonCode.DowngradedToFuse;
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.DetailDensity && focus is not null && focus.Band == 5)
        {
            penalty -= 12;
            reason ??= ReasonCode.DowngradedToFuse;
        }

        if ((candidate.Rule.SliderKey == SliderLanguageCatalog.ImageCleanliness || candidate.Rule.SliderKey == SliderLanguageCatalog.SurfaceAge)
            && cleanliness is not null
            && surfaceAge is not null
            && cleanliness.Band >= 4
            && surfaceAge.Band >= 4)
        {
            var other = candidate.Rule.SliderKey == SliderLanguageCatalog.ImageCleanliness ? surfaceAge : cleanliness;
            if (other is not null && ComparePriority(candidate, other) < 0)
            {
                penalty -= 18;
                reason ??= ReasonCode.SuppressedByConflict;
            }
            else
            {
                penalty -= 8;
                reason ??= ReasonCode.DowngradedToFuse;
            }
        }

        if ((candidate.Rule.SliderKey == SliderLanguageCatalog.TextureDepth || candidate.Rule.SliderKey == SliderLanguageCatalog.DetailDensity)
            && texture is not null
            && detail is not null)
        {
            var other = candidate.Rule.SliderKey == SliderLanguageCatalog.TextureDepth ? detail : texture;
            if (other is not null && ComparePriority(candidate, other) < 0)
            {
                penalty -= 12;
                reason ??= ReasonCode.DowngradedToFuse;
            }
            else
            {
                penalty -= 6;
                reason ??= ReasonCode.DowngradedToFuse;
            }
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Whimsy && tension is not null && tension.Band >= 4)
        {
            if (candidate.Band == 4)
            {
                penalty -= 16;
                reason ??= ReasonCode.DowngradedToTint;
            }
            else if (candidate.Band == 5 && (styleStylization is null || styleStylization.Band < 4))
            {
                penalty -= 28;
                reason ??= ReasonCode.SuppressedByConflict;
            }
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Whimsy && awe is not null && awe.Band >= 4 && candidate.Band < 5)
        {
            penalty -= 10;
            reason ??= ReasonCode.DowngradedToTint;
        }

        if (candidate.Rule.SliderKey == SliderLanguageCatalog.Awe && tension is not null && tension.Band >= 4)
        {
            var supportCue = (atmosphere is not null && atmosphere.Band >= 4)
                || (lightingPreset is not null && lightingPreset.Band >= 5)
                || (intensity is not null && intensity.Band >= 4)
                || (framing is not null && framing.Band >= 4)
                || (cameraDistance is not null && cameraDistance.Band >= 4);

            if (!supportCue)
            {
                penalty -= 14;
                reason ??= ReasonCode.DowngradedToFuse;
            }
        }

        if (acceptedDirects.Count >= 6)
        {
            penalty -= candidate.Rule.Authority switch
            {
                AuthorityTier.Top => 2,
                AuthorityTier.Mid => 6,
                _ => 10,
            };
            reason ??= ReasonCode.CrowdedOut;
        }

        if (IsPreserveFirstSlider(candidate.Rule.SliderKey))
        {
            penalty = Math.Max(penalty, -8);

            if (penalty < 0 && reason is ReasonCode.SuppressedByConflict or ReasonCode.CrowdedOut)
            {
                reason = ReasonCode.DowngradedToFuse;
            }
        }

        return new ConflictResolution(penalty, reason);
    }

    private static EmissionMode DetermineMode(
        SliderCandidate candidate,
        ModeAllowance allowances,
        int score,
        int directThreshold,
        int groupDirectCap,
        IReadOnlyList<ResolvedSliderState> groupResolved,
        IReadOnlyDictionary<string, SliderCandidate> candidatesByKey,
        OwnershipResolution ownership,
        ConflictResolution conflict)
    {
        var groupDirectCount = groupResolved.Count(state => state.Mode == EmissionMode.Direct);
        var preferredMode = EmissionMode.Silent;

        if (allowances.Direct && score >= directThreshold)
        {
            preferredMode = EmissionMode.Direct;
        }
        else if (allowances.Fuse && score >= FuseThreshold)
        {
            preferredMode = EmissionMode.Fuse;
        }
        else if (allowances.Tint && score >= TintThreshold)
        {
            preferredMode = EmissionMode.Tint;
        }

        if (preferredMode == EmissionMode.Direct && groupDirectCount >= groupDirectCap)
        {
            preferredMode = allowances.Fuse && score >= FuseThreshold
                ? EmissionMode.Fuse
                : allowances.Tint && score >= TintThreshold
                    ? EmissionMode.Tint
                    : EmissionMode.Silent;
        }

        if (preferredMode == EmissionMode.Direct && ownership.Penalty <= -14)
        {
            preferredMode = allowances.Fuse && score >= FuseThreshold
                ? EmissionMode.Fuse
                : allowances.Tint && score >= TintThreshold
                    ? EmissionMode.Tint
                    : EmissionMode.Silent;
        }

        if (preferredMode == EmissionMode.Direct && conflict.Penalty <= -18)
        {
            preferredMode = allowances.Fuse && score >= FuseThreshold
                ? EmissionMode.Fuse
                : allowances.Tint && score >= TintThreshold
                    ? EmissionMode.Tint
                    : EmissionMode.Silent;
        }

        if (preferredMode == EmissionMode.Direct && HasRedundantDirectOwner(candidate, groupResolved))
        {
            preferredMode = allowances.Fuse && score >= FuseThreshold
                ? EmissionMode.Fuse
                : allowances.Tint && score >= TintThreshold
                    ? EmissionMode.Tint
                    : EmissionMode.Silent;
        }

        if ((candidate.Rule.SliderKey == SliderLanguageCatalog.Whimsy || candidate.Rule.SliderKey == SliderLanguageCatalog.Symbolism)
            && preferredMode == EmissionMode.Direct)
        {
            var realism = GetCandidate(candidatesByKey, SliderLanguageCatalog.Realism);
            if (realism is not null && realism.Band >= 4)
            {
                preferredMode = allowances.Fuse && score >= FuseThreshold
                    ? EmissionMode.Fuse
                    : allowances.Tint && score >= TintThreshold
                        ? EmissionMode.Tint
                        : EmissionMode.Silent;
            }
        }

        return preferredMode;
    }

    private static bool HasRedundantDirectOwner(SliderCandidate candidate, IReadOnlyList<ResolvedSliderState> groupResolved)
    {
        return groupResolved.Any(state =>
            state.Mode == EmissionMode.Direct
            && !string.Equals(state.Candidate.Rule.SliderKey, candidate.Rule.SliderKey, StringComparison.OrdinalIgnoreCase)
            && !IsDistinctPadCompanion(state.Candidate.Rule.SliderKey, candidate.Rule.SliderKey)
            && string.Equals(state.Candidate.Rule.RedundancyDomain, candidate.Rule.RedundancyDomain, StringComparison.OrdinalIgnoreCase));
    }

    private static IReadOnlyList<string> BuildGroupFragments(IReadOnlyList<ResolvedSliderState> resolved)
    {
        var clauses = BuildOrderedClauses(resolved);
        return clauses.Select(clause => clause.Text).ToArray();
    }

    private static IReadOnlyList<ClauseUnit> BuildOrderedClauses(IReadOnlyList<ResolvedSliderState> resolved)
    {
        var survivors = resolved
            .Where(state => state.Mode != EmissionMode.Silent)
            .OrderBy(state => GetAssemblyGroupOrder(state.Candidate.Rule.SliderKey))
            .ThenBy(state => GetAssemblySliderOrder(state.Candidate.Rule.SliderKey))
            .ToArray();
        var richnessDemand = BuildRichnessDemandProfile(survivors);
        var padPlans = BuildPadSurvivorPlans(resolved);
        var padTrace = InitializePadTrace(resolved, padPlans);

        var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var clauses = new List<ClauseUnit>();

        AddRenderingIdentityClauses(survivors, usedKeys, clauses);
        AddViewConstructionClauses(survivors, usedKeys, clauses);
        AddLightingAtmosphereClauses(survivors, usedKeys, clauses);
        AddMaterialFinishClauses(survivors, usedKeys, clauses);
        AddStandardClauses(survivors, usedKeys, clauses, AssemblyGroup.CompositionEnvironment);
        AddMotionPressureClauses(survivors, usedKeys, clauses);
        AddStandardClauses(survivors, usedKeys, clauses, AssemblyGroup.MoodSymbolicTone);
        CapturePadTraceStage(padTrace, clauses, PadLossStage.Fusion);

        clauses = CompressSupportClauses(clauses).ToList();
        clauses = ApplyMoodSymbolismDiction(clauses);
        clauses = ApplyModifierQuieting(clauses);
        clauses = ApplyRhythmCleanup(clauses);
        clauses = ApplyMicroCompression(clauses);
        clauses = ApplyCarrierSelectionRefinement(clauses);
        clauses = ApplyAttachmentAdjacencyRules(clauses);
        clauses = ApplyTailOrderingDiscipline(clauses);
        var richnessSnapshot = clauses.ToList();
        clauses = ApplyAbstractPhysicalBalancing(clauses);
        clauses = ApplyCarrierScoringRefinement(clauses);
        clauses = ApplyQualifierPhrasingRefinement(clauses);
        clauses = ApplyDuplicateSemanticEchoCleanup(clauses);
        CapturePadTraceStage(padTrace, clauses, PadLossStage.Cleanup);
        clauses = ApplyPadSurvivorFloors(clauses, resolved, padPlans);
        clauses = ApplyCoverageRichnessRecovery(clauses, richnessSnapshot, richnessDemand);
        clauses = ApplyMinimumVisualRichnessSafeguards(clauses, richnessSnapshot, richnessDemand);
        clauses = ApplyFinalCrowdedStatePolish(clauses, richnessSnapshot, richnessDemand);
        clauses = ApplyFinalThinPromptCheck(clauses, richnessSnapshot, richnessDemand);
        CapturePadTraceStage(padTrace, clauses, PadLossStage.Recovery);

        var finalClauses = EnforceClauseCaps(clauses, richnessDemand, padPlans);
        CapturePadTraceStage(padTrace, finalClauses, PadLossStage.CapEnforcement);
        LogPadSurvivorship(padTrace, padPlans, finalClauses);
        return finalClauses;
    }

    private static void AddRenderingIdentityClauses(
        IReadOnlyList<ResolvedSliderState> survivors,
        ISet<string> usedKeys,
        ICollection<ClauseUnit> clauses)
    {
        var stylization = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.Stylization);
        var realism = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.Realism);

        if (TryBuildRenderingIdentityFusion(stylization, realism, out var fusedClause))
        {
            clauses.Add(fusedClause);
            usedKeys.Add(SliderLanguageCatalog.Stylization);
            usedKeys.Add(SliderLanguageCatalog.Realism);
        }

        var texture = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.TextureDepth);
        var detail = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.DetailDensity);

        if (TryBuildMaterialOwnerFusion(texture, detail, out fusedClause, out var consumedMaterialKeys))
        {
            clauses.Add(fusedClause);
            foreach (var key in consumedMaterialKeys)
            {
                usedKeys.Add(key);
            }
        }

        AddStandardClauses(survivors, usedKeys, clauses, AssemblyGroup.RenderingIdentity);
    }

    private static void AddViewConstructionClauses(
        IReadOnlyList<ResolvedSliderState> survivors,
        ISet<string> usedKeys,
        ICollection<ClauseUnit> clauses)
    {
        var framing = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.Framing);
        var cameraDistance = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.CameraDistance);
        var cameraAngle = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.CameraAngle);

        if (TryBuildCameraClauseFusion(framing, cameraDistance, cameraAngle, out var fusedClause, out var consumedKeys))
        {
            clauses.Add(fusedClause);
            foreach (var key in consumedKeys)
            {
                usedKeys.Add(key);
            }
        }

        AddStandardClauses(survivors, usedKeys, clauses, AssemblyGroup.ViewConstruction);
    }

    private static void AddLightingAtmosphereClauses(
        IReadOnlyList<ResolvedSliderState> survivors,
        ISet<string> usedKeys,
        ICollection<ClauseUnit> clauses)
    {
        var preset = GetUnusedSurvivor(survivors, usedKeys, LightingPresetKey);
        var temperature = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.Temperature);
        var intensity = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.LightingIntensity);

        if (TryBuildLightingAtmosphereFusion(preset, intensity, temperature, out var fusedClause, out var consumedKeys))
        {
            clauses.Add(fusedClause);
            foreach (var key in consumedKeys)
            {
                usedKeys.Add(key);
            }
        }

        AddStandardClauses(survivors, usedKeys, clauses, AssemblyGroup.LightingColorAtmosphere);
    }

    private static void AddMaterialFinishClauses(
        IReadOnlyList<ResolvedSliderState> survivors,
        ISet<string> usedKeys,
        ICollection<ClauseUnit> clauses)
    {
        var materialClause = clauses.FirstOrDefault(clause =>
            clause.Group == AssemblyGroup.RenderingIdentity
            && clause.IsFused
            && clause.SliderKeys.Any(static key =>
                string.Equals(key, SliderLanguageCatalog.TextureDepth, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.DetailDensity, StringComparison.OrdinalIgnoreCase)));

        var cleanliness = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.ImageCleanliness);
        var surfaceAge = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.SurfaceAge);

        if (materialClause is not null
            && TryApplyFinishTintToMaterialClause(materialClause, cleanliness, surfaceAge, out var updatedMaterialClause, out var consumedFinishKeys))
        {
            clauses.Remove(materialClause);
            clauses.Add(updatedMaterialClause);

            foreach (var key in consumedFinishKeys)
            {
                usedKeys.Add(key);
            }
        }

        var texture = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.TextureDepth);
        var detail = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.DetailDensity);
        cleanliness = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.ImageCleanliness);
        surfaceAge = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.SurfaceAge);

        if (TryBuildFinishTintClause(cleanliness, surfaceAge, out var finishClause, out var finishKeys))
        {
            clauses.Add(finishClause);
            foreach (var key in finishKeys)
            {
                usedKeys.Add(key);
            }

            return;
        }

        if (TryBuildMaterialFinishFallback(texture, detail, cleanliness, surfaceAge, out var fallbackClause, out var fallbackKeys))
        {
            clauses.Add(fallbackClause);
            foreach (var key in fallbackKeys)
            {
                usedKeys.Add(key);
            }
        }
    }

    private static void AddMotionPressureClauses(
        IReadOnlyList<ResolvedSliderState> survivors,
        ISet<string> usedKeys,
        ICollection<ClauseUnit> clauses)
    {
        var motion = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.MotionEnergy);
        var chaos = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.Chaos);
        var tension = GetUnusedSurvivor(survivors, usedKeys, SliderLanguageCatalog.Tension);

        if (TryBuildMotionPressureFusion(motion, chaos, tension, out var fusedClause, out var consumedKeys))
        {
            clauses.Add(fusedClause);
            foreach (var key in consumedKeys)
            {
                usedKeys.Add(key);
            }
        }

        AddStandardClauses(survivors, usedKeys, clauses, AssemblyGroup.MotionInstability);
    }

    private static void AddStandardClauses(
        IReadOnlyList<ResolvedSliderState> survivors,
        ISet<string> usedKeys,
        ICollection<ClauseUnit> clauses,
        AssemblyGroup group)
    {
        foreach (var state in survivors)
        {
            var sliderKey = state.Candidate.Rule.SliderKey;
            if (usedKeys.Contains(sliderKey) || GetAssemblyGroup(sliderKey) != group)
            {
                continue;
            }

            clauses.Add(CreateClauseUnit(state));
            usedKeys.Add(sliderKey);
        }
    }

    private static ClauseUnit CreateClauseUnit(ResolvedSliderState state)
    {
        var sliderKey = state.Candidate.Rule.SliderKey;
        var group = GetAssemblyGroup(sliderKey);
        var clauseClass = GetBaseClauseClass(sliderKey);
        var isOwner = string.Equals(state.OwnerKey, sliderKey, StringComparison.OrdinalIgnoreCase)
            || state.Reason is ReasonCode.SurvivedAsOwner or ReasonCode.SurvivedAsExtreme;

        return new ClauseUnit(
            Text: state.Candidate.Phrase,
            Group: group,
            ClauseClass: clauseClass,
            Score: GetClauseScore(state, clauseClass, isOwner),
            Mode: state.Mode,
            IsOwner: isOwner,
            IsFused: false,
            SliderKeys: [sliderKey]);
    }

    private static bool TryBuildRenderingIdentityFusion(
        ResolvedSliderState? stylization,
        ResolvedSliderState? realism,
        out ClauseUnit clause)
    {
        clause = null!;

        if (stylization is null
            || realism is null
            || (stylization.Mode is EmissionMode.Tint && realism.Mode is EmissionMode.Tint))
        {
            return false;
        }

        var owner = SelectFusionOwner(stylization, realism, preferFirstOnClose: true);
        var qualifier = ReferenceEquals(owner, stylization) ? realism : stylization;
        var text = owner == stylization
            ? $"{stylization.Candidate.Phrase} with {LowerClause(realism.Candidate.Phrase)}"
            : $"{realism.Candidate.Phrase} with {LowerClause(stylization.Candidate.Phrase)}";

        clause = new ClauseUnit(
            Text: text,
            Group: AssemblyGroup.RenderingIdentity,
            ClauseClass: ClauseClass.Primary,
            Score: Math.Max(stylization.FinalWeight, realism.FinalWeight) + 8,
            Mode: EmissionMode.Direct,
            IsOwner: true,
            IsFused: true,
            SliderKeys: [stylization.Candidate.Rule.SliderKey, realism.Candidate.Rule.SliderKey]);
        return true;
    }

    private static bool TryBuildCameraClauseFusion(
        ResolvedSliderState? framing,
        ResolvedSliderState? cameraDistance,
        ResolvedSliderState? cameraAngle,
        out ClauseUnit clause,
        out string[] consumedKeys)
    {
        clause = null!;
        consumedKeys = [];

        var eligible = new[] { framing, cameraDistance, cameraAngle }
            .Where(static state => state is not null && state.Mode is EmissionMode.Direct or EmissionMode.Fuse)
            .Cast<ResolvedSliderState>()
            .ToArray();

        if (eligible.Length < 2)
        {
            return false;
        }

        string text;
        if (framing is not null && cameraDistance is not null)
        {
            text = $"{framing.Candidate.Phrase} with {LowerClause(NormalizeCameraDistance(cameraDistance.Candidate.Phrase))}";
            consumedKeys = [framing.Candidate.Rule.SliderKey, cameraDistance.Candidate.Rule.SliderKey];

        }
        else if (framing is not null && cameraAngle is not null)
        {
            text = $"{framing.Candidate.Phrase} from a {NormalizeCameraAngle(cameraAngle.Candidate.Phrase)}";
            consumedKeys = [framing.Candidate.Rule.SliderKey, cameraAngle.Candidate.Rule.SliderKey];
        }
        else if (cameraDistance is not null && cameraAngle is not null)
        {
            text = $"{NormalizeCameraDistance(cameraDistance.Candidate.Phrase)} from a {NormalizeCameraAngle(cameraAngle.Candidate.Phrase)}";
            consumedKeys = [cameraDistance.Candidate.Rule.SliderKey, cameraAngle.Candidate.Rule.SliderKey];
        }
        else
        {
            return false;
        }

        clause = new ClauseUnit(
            Text: text,
            Group: AssemblyGroup.ViewConstruction,
            ClauseClass: ClauseClass.Primary,
            Score: eligible.Max(state => state.FinalWeight) + 8,
            Mode: EmissionMode.Direct,
            IsOwner: true,
            IsFused: true,
            SliderKeys: consumedKeys);
        return true;
    }

    private static bool TryBuildLightingAtmosphereFusion(
        ResolvedSliderState? preset,
        ResolvedSliderState? intensity,
        ResolvedSliderState? temperature,
        out ClauseUnit clause,
        out string[] consumedKeys)
    {
        clause = null!;
        consumedKeys = [];

        var eligible = new[] { preset, intensity, temperature }
            .Where(static state => state is not null && state.Mode is EmissionMode.Direct or EmissionMode.Fuse)
            .Cast<ResolvedSliderState>()
            .ToArray();

        if (eligible.Length < 2)
        {
            return false;
        }

        var owner = SelectLightingOwner(preset, intensity, temperature);
        if (owner is null)
        {
            return false;
        }

        string text;
        if (ReferenceEquals(owner, preset) && preset is not null)
        {
            var lead = NormalizeLightingPresetClause(preset.Candidate.Phrase);
            var qualifiers = new List<string>();
            var strongestCompanion = SelectBestState(temperature, intensity);

            if (temperature is not null && ReferenceEquals(temperature, strongestCompanion))
            {
                qualifiers.Add(NormalizeTemperatureQualifier(temperature.Candidate.Phrase));
            }

            if (intensity is not null && ReferenceEquals(intensity, strongestCompanion))
            {
                qualifiers.Add(NormalizeLightingIntensityQualifier(intensity.Candidate.Phrase));
            }

            text = qualifiers.Count == 0
                ? lead
                : $"{lead} with {JoinPhrases(qualifiers)}";

            consumedKeys = [preset.Candidate.Rule.SliderKey];
            if (strongestCompanion is not null)
            {
                consumedKeys = [.. consumedKeys, strongestCompanion.Candidate.Rule.SliderKey];
            }
        }
        else if (ReferenceEquals(owner, intensity) && intensity is not null)
        {
            text = NormalizeLightingIntensityOwner(intensity.Candidate.Phrase, preset?.Candidate.Phrase);
            consumedKeys = [intensity.Candidate.Rule.SliderKey];

            if (preset is not null)
            {
                consumedKeys = [.. consumedKeys, preset.Candidate.Rule.SliderKey];
            }
            else if (temperature is not null && !ReferenceEquals(temperature, owner))
            {
                text = $"{text} with {NormalizeTemperatureQualifier(temperature.Candidate.Phrase)}";
                consumedKeys = [.. consumedKeys, temperature.Candidate.Rule.SliderKey];
            }
        }
        else if (temperature is not null)
        {
            text = NormalizeTemperatureOwner(temperature.Candidate.Phrase, intensity?.Candidate.Phrase, preset?.Candidate.Phrase);
            consumedKeys = [temperature.Candidate.Rule.SliderKey];

            if (preset is not null)
            {
                consumedKeys = [.. consumedKeys, preset.Candidate.Rule.SliderKey];
            }
            else if (intensity is not null)
            {
                consumedKeys = [.. consumedKeys, intensity.Candidate.Rule.SliderKey];
            }
        }
        else
        {
            return false;
        }

        clause = new ClauseUnit(
            Text: text,
            Group: AssemblyGroup.LightingColorAtmosphere,
            ClauseClass: ClauseClass.Primary,
            Score: eligible.Max(state => state.FinalWeight) + 10,
            Mode: EmissionMode.Direct,
            IsOwner: true,
            IsFused: true,
            SliderKeys: consumedKeys);
        return true;
    }

    private static bool TryBuildMaterialOwnerFusion(
        ResolvedSliderState? texture,
        ResolvedSliderState? detail,
        out ClauseUnit clause,
        out string[] consumedKeys)
    {
        clause = null!;
        consumedKeys = [];

        if (texture is null
            || detail is null
            || texture.Mode is not (EmissionMode.Direct or EmissionMode.Fuse)
            || detail.Mode is not (EmissionMode.Direct or EmissionMode.Fuse))
        {
            return false;
        }

        var owner = SelectFusionOwner(texture, detail, preferFirstOnClose: true);
        var text = ReferenceEquals(owner, texture)
            ? $"{texture.Candidate.Phrase} with {NormalizeDetailDensityQualifier(detail.Candidate.Phrase)}"
            : $"{NormalizeDetailDensityOwner(detail.Candidate.Phrase)} with {NormalizeTextureDepthQualifier(texture.Candidate.Phrase)}";

        consumedKeys = [texture.Candidate.Rule.SliderKey, detail.Candidate.Rule.SliderKey];
        clause = new ClauseUnit(
            Text: text,
            Group: AssemblyGroup.RenderingIdentity,
            ClauseClass: ClauseClass.Primary,
            Score: Math.Max(texture.FinalWeight, detail.FinalWeight) + 9,
            Mode: EmissionMode.Direct,
            IsOwner: true,
            IsFused: true,
            SliderKeys: consumedKeys);
        return true;
    }

    private static bool TryBuildFinishTintClause(
        ResolvedSliderState? cleanliness,
        ResolvedSliderState? surfaceAge,
        out ClauseUnit clause,
        out string[] consumedKeys)
    {
        clause = null!;
        consumedKeys = [];

        var eligible = new[] { cleanliness, surfaceAge }
            .Where(static state => state is not null && state.Mode is EmissionMode.Direct or EmissionMode.Fuse)
            .Cast<ResolvedSliderState>()
            .ToArray();

        if (eligible.Length < 2 || cleanliness is null || surfaceAge is null)
        {
            return false;
        }

        var owner = SelectFusionOwner(cleanliness, surfaceAge, preferFirstOnClose: true);
        var text = ReferenceEquals(owner, cleanliness)
            ? $"{NormalizeImageCleanlinessOwner(cleanliness.Candidate.Phrase)} with {NormalizeSurfaceAgeQualifier(surfaceAge.Candidate.Phrase)}"
            : $"{surfaceAge.Candidate.Phrase} with {NormalizeImageCleanlinessQualifier(cleanliness.Candidate.Phrase)}";

        consumedKeys = eligible.Select(state => state.Candidate.Rule.SliderKey).ToArray();
        clause = new ClauseUnit(
            Text: text,
            Group: AssemblyGroup.RenderingIdentity,
            ClauseClass: ClauseClass.Support,
            Score: eligible.Max(state => state.FinalWeight) + 4,
            Mode: EmissionMode.Fuse,
            IsOwner: ReferenceEquals(owner, cleanliness),
            IsFused: true,
            SliderKeys: consumedKeys);
        return true;
    }

    private static bool TryApplyFinishTintToMaterialClause(
        ClauseUnit materialClause,
        ResolvedSliderState? cleanliness,
        ResolvedSliderState? surfaceAge,
        out ClauseUnit updatedClause,
        out string[] consumedKeys)
    {
        updatedClause = materialClause;
        consumedKeys = [];

        var qualifiers = new List<string>();
        var keys = new List<string>();

        if (cleanliness is not null)
        {
            qualifiers.Add(NormalizeImageCleanlinessQualifier(cleanliness.Candidate.Phrase));
            keys.Add(cleanliness.Candidate.Rule.SliderKey);
        }

        if (surfaceAge is not null && keys.Count == 0)
        {
            qualifiers.Add(NormalizeSurfaceAgeQualifier(surfaceAge.Candidate.Phrase));
            keys.Add(surfaceAge.Candidate.Rule.SliderKey);
        }

        if (qualifiers.Count == 0)
        {
            return false;
        }

        updatedClause = materialClause with
        {
            Text = $"{materialClause.Text} with {JoinPhrases(qualifiers)}",
            Score = materialClause.Score + Math.Min(6, qualifiers.Count * 3),
            SliderKeys = [.. materialClause.SliderKeys, .. keys.Distinct(StringComparer.OrdinalIgnoreCase)],
        };

        consumedKeys = keys.ToArray();
        return true;
    }

    private static bool TryBuildMaterialFinishFallback(
        ResolvedSliderState? texture,
        ResolvedSliderState? detail,
        ResolvedSliderState? cleanliness,
        ResolvedSliderState? surfaceAge,
        out ClauseUnit clause,
        out string[] consumedKeys)
    {
        clause = null!;
        consumedKeys = [];

        var finishOwner = SelectBestState(cleanliness, surfaceAge);
        if (finishOwner is null)
        {
            return false;
        }

        var materialOwner = SelectBestState(texture, detail);
        if (materialOwner is not null && materialOwner.FinalWeight >= finishOwner.FinalWeight)
        {
            return false;
        }

        var text = ReferenceEquals(finishOwner, cleanliness)
            ? NormalizeImageCleanlinessOwner(cleanliness!.Candidate.Phrase)
            : surfaceAge!.Candidate.Phrase;

        if (texture is not null && texture.Mode != EmissionMode.Silent)
        {
            text = $"{NormalizeTextureDepthQualifier(texture.Candidate.Phrase)}, {LowerClause(text)}";
            consumedKeys = [finishOwner.Candidate.Rule.SliderKey, texture.Candidate.Rule.SliderKey];
        }
        else if (detail is not null && detail.Mode != EmissionMode.Silent)
        {
            text = $"{NormalizeDetailDensityQualifier(detail.Candidate.Phrase)}, {LowerClause(text)}";
            consumedKeys = [finishOwner.Candidate.Rule.SliderKey, detail.Candidate.Rule.SliderKey];
        }
        else
        {
            consumedKeys = [finishOwner.Candidate.Rule.SliderKey];
        }

        clause = new ClauseUnit(
            Text: text,
            Group: AssemblyGroup.RenderingIdentity,
            ClauseClass: ClauseClass.Support,
            Score: finishOwner.FinalWeight + 3,
            Mode: finishOwner.Mode,
            IsOwner: true,
            IsFused: consumedKeys.Length > 1,
            SliderKeys: consumedKeys);
        return true;
    }

    private static bool TryBuildMotionPressureFusion(
        ResolvedSliderState? motion,
        ResolvedSliderState? chaos,
        ResolvedSliderState? tension,
        out ClauseUnit clause,
        out string[] consumedKeys)
    {
        clause = null!;
        consumedKeys = [];

        var eligible = new[] { motion, chaos, tension }
            .Where(static state => state is not null && state.Mode is EmissionMode.Direct or EmissionMode.Fuse)
            .Cast<ResolvedSliderState>()
            .ToArray();

        if (eligible.Length < 2)
        {
            return false;
        }

        string text;
        if (motion is not null)
        {
            var qualifiers = new List<string>();
            consumedKeys = [motion.Candidate.Rule.SliderKey];
            var strongestCompanion = SelectBestState(chaos, tension);

            if (chaos is not null && ReferenceEquals(chaos, strongestCompanion))
            {
                qualifiers.Add(NormalizeChaosQualifier(chaos.Candidate.Phrase));
                consumedKeys = [.. consumedKeys, chaos.Candidate.Rule.SliderKey];
            }

            if (tension is not null && ReferenceEquals(tension, strongestCompanion))
            {
                qualifiers.Add(NormalizeTensionQualifier(tension.Candidate.Phrase));
                consumedKeys = [.. consumedKeys, tension.Candidate.Rule.SliderKey];
            }

            text = qualifiers.Count == 0
                ? motion.Candidate.Phrase
                : $"{motion.Candidate.Phrase} with {JoinPhrases(qualifiers)}";
        }
        else if (chaos is not null && tension is not null)
        {
            text = $"{chaos.Candidate.Phrase} with {NormalizeTensionQualifier(tension.Candidate.Phrase)}";
        }
        else
        {
            return false;
        }

        clause = new ClauseUnit(
            Text: text,
            Group: AssemblyGroup.MotionInstability,
            ClauseClass: ClauseClass.Primary,
            Score: eligible.Max(state => state.FinalWeight) + 8,
            Mode: EmissionMode.Direct,
            IsOwner: true,
            IsFused: true,
            SliderKeys: consumedKeys);
        return true;
    }

    private static IReadOnlyList<ClauseUnit> CompressSupportClauses(IReadOnlyList<ClauseUnit> initialClauses)
    {
        var clauses = initialClauses.ToList();

        TryCompressClausePair(clauses, SliderLanguageCatalog.BackgroundComplexity, SliderLanguageCatalog.AtmosphericDepth, CombineEnvironmentDepthClauses);
        TryCompressClausePair(clauses, SliderLanguageCatalog.FocusDepth, SliderLanguageCatalog.BackgroundComplexity, CombineFocusEnvironmentClauses);
        TryCompressClausePair(clauses, SliderLanguageCatalog.TextureDepth, SliderLanguageCatalog.DetailDensity, CombineDenseTactileClauses);
        TryCompressWeakNarrativeEnvironment(clauses);

        return clauses;
    }

    private static void TryCompressClausePair(
        List<ClauseUnit> clauses,
        string firstKey,
        string secondKey,
        Func<ClauseUnit, ClauseUnit, ClauseUnit?> combiner)
    {
        var first = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(firstKey, StringComparer.OrdinalIgnoreCase));
        var second = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(secondKey, StringComparer.OrdinalIgnoreCase));

        if (first is null || second is null || ReferenceEquals(first, second))
        {
            return;
        }

        if (IsDistinctPadCompanion(firstKey, secondKey)
            && first.Score >= 58
            && second.Score >= 58)
        {
            return;
        }

        var combined = combiner(first, second);
        if (combined is null)
        {
            return;
        }

        clauses.Remove(first);
        clauses.Remove(second);
        clauses.Add(combined);
    }

    private static void TryCompressWeakNarrativeEnvironment(List<ClauseUnit> clauses)
    {
        var narrative = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(SliderLanguageCatalog.NarrativeDensity, StringComparer.OrdinalIgnoreCase));
        var background = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(SliderLanguageCatalog.BackgroundComplexity, StringComparer.OrdinalIgnoreCase));

        if (narrative is null
            || background is null
            || narrative.Score > 58
            || background.Score > 62
            || ReferenceEquals(narrative, background))
        {
            return;
        }

        var combined = new ClauseUnit(
            Text: $"{background.Text} with restrained {NormalizeNarrativeDensityQualifier(narrative.Text)}",
            Group: background.Group,
            ClauseClass: ClauseClass.Support,
            Score: Math.Max(background.Score, narrative.Score) + 3,
            Mode: background.Mode,
            IsOwner: background.IsOwner,
            IsFused: true,
            SliderKeys: [.. background.SliderKeys, .. narrative.SliderKeys]);

        clauses.Remove(background);
        clauses.Remove(narrative);
        clauses.Add(combined);
    }

    private static List<ClauseUnit> ApplyMoodSymbolismDiction(List<ClauseUnit> clauses)
    {
        TryTintWeakClause(
            clauses,
            SliderLanguageCatalog.Whimsy,
            NormalizeWhimsyTone,
            static clause => clause.Group is not AssemblyGroup.TrailingRefiners);

        TryTintWeakClause(
            clauses,
            SliderLanguageCatalog.Symbolism,
            NormalizeSymbolismTone,
            static clause => clause.Group is not AssemblyGroup.TrailingRefiners);

        TryTintWeakClause(
            clauses,
            SliderLanguageCatalog.Awe,
            NormalizeAweTone,
            static clause => clause.Group is AssemblyGroup.CompositionEnvironment
                or AssemblyGroup.LightingColorAtmosphere
                or AssemblyGroup.MotionInstability
                or AssemblyGroup.RenderingIdentity);

        TryTintWeakClause(
            clauses,
            SliderLanguageCatalog.Tension,
            NormalizeTensionTone,
            static clause => clause.Group is AssemblyGroup.MotionInstability
                or AssemblyGroup.LightingColorAtmosphere
                or AssemblyGroup.CompositionEnvironment);

        TryTintWeakClause(
            clauses,
            SliderLanguageCatalog.NarrativeDensity,
            NormalizeNarrativeTone,
            static clause => clause.Group == AssemblyGroup.CompositionEnvironment);

        return clauses;
    }

    private static List<ClauseUnit> ApplyModifierQuieting(List<ClauseUnit> clauses)
    {
        if (CountPadSemanticSurvivors(clauses, MacroPad.MaterialFinish) > 2)
        {
            TryQuietWeakModifier(
                clauses,
                SliderLanguageCatalog.SurfaceAge,
                NormalizeSurfaceAgeTail,
                static clause => clause.Group == AssemblyGroup.RenderingIdentity);

            TryQuietWeakModifier(
                clauses,
                SliderLanguageCatalog.ImageCleanliness,
                NormalizeImageCleanlinessTail,
                static clause => clause.Group == AssemblyGroup.RenderingIdentity);
        }

        if (CountPadSemanticSurvivors(clauses, MacroPad.LightAtmosphere) > 2)
        {
            TryQuietWeakModifier(
                clauses,
                SliderLanguageCatalog.Saturation,
                NormalizeSaturationTail,
                static clause => clause.Group == AssemblyGroup.LightingColorAtmosphere);

            TryQuietWeakModifier(
                clauses,
                SliderLanguageCatalog.Contrast,
                NormalizeContrastTail,
                static clause => clause.Group == AssemblyGroup.LightingColorAtmosphere);
        }

        if (CountExplicitToneSurvivors(clauses) > 2)
        {
            SuppressWeakDuplicateMoodTail(clauses, SliderLanguageCatalog.Whimsy, SliderLanguageCatalog.Symbolism);
            SuppressWeakDuplicateMoodTail(clauses, SliderLanguageCatalog.Awe, SliderLanguageCatalog.Tension);
        }

        return clauses;
    }

    private static List<ClauseUnit> ApplyRhythmCleanup(List<ClauseUnit> clauses)
    {
        for (var index = 0; index < clauses.Count; index++)
        {
            var clause = clauses[index];
            var cleaned = clause.Text
                .Replace(" with a a ", " with a ", StringComparison.OrdinalIgnoreCase)
                .Replace(" with with ", " with ", StringComparison.OrdinalIgnoreCase)
                .Replace(" with slight slight ", " with slight ", StringComparison.OrdinalIgnoreCase);

            var withCount = CountOccurrences(cleaned, " with ");
            if (withCount > 1)
            {
                cleaned = ReplaceLast(cleaned, " with ", ", ");
            }

            clauses[index] = clause with { Text = cleaned };
        }

        if (clauses.Count >= 2)
        {
            var last = clauses[^1];
            var previous = clauses[^2];

            if (IsAbstractTail(last) && IsAbstractTail(previous))
            {
                if (last.Score <= previous.Score)
                {
                    clauses.RemoveAt(clauses.Count - 1);
                }
                else
                {
                    clauses.RemoveAt(clauses.Count - 2);
                }
            }
        }

        return clauses;
    }

    private static List<ClauseUnit> ApplyMicroCompression(List<ClauseUnit> clauses)
    {
        TryCompressWeakTailPair(
            clauses,
            SliderLanguageCatalog.Awe,
            SliderLanguageCatalog.Symbolism,
            "lightly mythic wonder");

        TryCompressWeakTailPair(
            clauses,
            SliderLanguageCatalog.Tension,
            SliderLanguageCatalog.Chaos,
            "tense, slightly chaotic energy");

        TryCompressWeakTailPair(
            clauses,
            SliderLanguageCatalog.ImageCleanliness,
            SliderLanguageCatalog.SurfaceAge,
            "clean, slightly time-worn finish");

        TryCompressWeakTailPair(
            clauses,
            SliderLanguageCatalog.Saturation,
            SliderLanguageCatalog.Contrast,
            "rich but controlled color contrast");

        TryCompressWeakTailPair(
            clauses,
            SliderLanguageCatalog.NarrativeDensity,
            SliderLanguageCatalog.BackgroundComplexity,
            "readable environment with implied story presence");

        return clauses;
    }

    private static List<ClauseUnit> ApplyCarrierSelectionRefinement(List<ClauseUnit> clauses)
    {
        RefineTintCarrier(clauses, SliderLanguageCatalog.Whimsy);
        RefineTintCarrier(clauses, SliderLanguageCatalog.Symbolism);
        RefineTintCarrier(clauses, SliderLanguageCatalog.Awe);
        RefineTintCarrier(clauses, SliderLanguageCatalog.Tension);
        RefineTintCarrier(clauses, SliderLanguageCatalog.SurfaceAge);
        RefineTintCarrier(clauses, SliderLanguageCatalog.ImageCleanliness);
        RefineTintCarrier(clauses, SliderLanguageCatalog.Saturation);
        RefineTintCarrier(clauses, SliderLanguageCatalog.Contrast);
        RefineTintCarrier(clauses, SliderLanguageCatalog.NarrativeDensity);
        RefineTintCarrier(clauses, SliderLanguageCatalog.Chaos);

        return clauses;
    }

    private static List<ClauseUnit> ApplyAttachmentAdjacencyRules(List<ClauseUnit> clauses)
    {
        for (var index = clauses.Count - 1; index >= 0; index--)
        {
            var clause = clauses[index];
            var abstractCount = GetAttachedAbstractTintCount(clause);
            var physicalCount = GetAttachedPhysicalTintCount(clause);

            if (abstractCount > 1 && physicalCount > 1 && clause.Score < 86)
            {
                clauses[index] = clause with
                {
                    Text = SimplifyTailAttachment(clause.Text),
                    Score = clause.Score - 1,
                };
            }
            else if (abstractCount > 2 && clause.Score < 82)
            {
                clauses[index] = clause with
                {
                    Text = SimplifyTailAttachment(clause.Text),
                    Score = clause.Score - 1,
                };
            }
        }

        return clauses;
    }

    private static List<ClauseUnit> ApplyTailOrderingDiscipline(List<ClauseUnit> clauses)
    {
        var anchored = clauses
            .Where(clause => !IsTinyTailClause(clause))
            .OrderBy(clause => Array.IndexOf(AssemblyGroupSequence, clause.Group))
            .ThenBy(GetClauseOrder)
            .ToList();

        var tails = clauses
            .Where(IsTinyTailClause)
            .OrderBy(GetTailOrder)
            .ThenBy(GetClauseOrder)
            .ToList();

        anchored.AddRange(tails);
        return anchored;
    }

    private static List<ClauseUnit> ApplyAbstractPhysicalBalancing(List<ClauseUnit> clauses)
    {
        var abstractResidue = clauses.Where(IsAbstractResidue).OrderBy(clause => clause.Score).ToList();
        var physicalResidue = clauses.Count(IsPhysicalResidue);

        while (abstractResidue.Count > 0 && abstractResidue.Count > physicalResidue + 1)
        {
            var removable = abstractResidue[0];
            if (GetMacroPad(removable) == MacroPad.Tone && CountExplicitToneSurvivors(clauses) <= 1)
            {
                break;
            }

            clauses.Remove(removable);
            abstractResidue.RemoveAt(0);
        }

        return clauses;
    }

    private static List<ClauseUnit> ApplyCarrierScoringRefinement(List<ClauseUnit> clauses)
    {
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.Whimsy);
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.Symbolism);
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.Awe);
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.Tension);
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.SurfaceAge);
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.ImageCleanliness);
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.Saturation);
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.Contrast);
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.NarrativeDensity);
        RefineCarrierWithScoring(clauses, SliderLanguageCatalog.Chaos);

        return clauses;
    }

    private static List<ClauseUnit> ApplyQualifierPhrasingRefinement(List<ClauseUnit> clauses)
    {
        for (var index = 0; index < clauses.Count; index++)
        {
            var clause = clauses[index];
            var polished = clause.Text
                .Replace(" with a playful edge", ", playful at the edges", StringComparison.OrdinalIgnoreCase)
                .Replace(" with a symbolic undertone", " with symbolic undertone", StringComparison.OrdinalIgnoreCase)
                .Replace(" with a lightly mythic undertone", " with lightly mythic undertone", StringComparison.OrdinalIgnoreCase)
                .Replace(" with a sense of wonder", " with wonder-tinged scale", StringComparison.OrdinalIgnoreCase)
                .Replace(" with a clean finish", " with clean finish", StringComparison.OrdinalIgnoreCase)
                .Replace(" with a polished finish", " with polished finish", StringComparison.OrdinalIgnoreCase)
                .Replace(" with a balanced finish", " with balanced finish", StringComparison.OrdinalIgnoreCase)
                .Replace(" with slight grit", ", slight grit", StringComparison.OrdinalIgnoreCase)
                .Replace(" with controlled color", " with controlled color", StringComparison.OrdinalIgnoreCase)
                .Replace(" with controlled contrast", " with controlled contrast", StringComparison.OrdinalIgnoreCase)
                .Replace(" slightly tense slightly chaotic", " tense, slightly chaotic", StringComparison.OrdinalIgnoreCase);

            polished = CleanupQualifierRepeats(polished);
            clauses[index] = clause with { Text = polished };
        }

        return clauses;
    }

    private static List<ClauseUnit> ApplyDuplicateSemanticEchoCleanup(List<ClauseUnit> clauses)
    {
        CleanupSemanticEcho(clauses, SliderLanguageCatalog.TextureDepth, SliderLanguageCatalog.DetailDensity);
        CleanupSemanticEcho(clauses, SliderLanguageCatalog.LightingIntensity, SliderLanguageCatalog.Contrast);
        CleanupSemanticEcho(clauses, SliderLanguageCatalog.Awe, SliderLanguageCatalog.AtmosphericDepth);
        CleanupSemanticEcho(clauses, SliderLanguageCatalog.Symbolism, SliderLanguageCatalog.NarrativeDensity);
        CleanupSemanticEcho(clauses, SliderLanguageCatalog.ImageCleanliness, SliderLanguageCatalog.SurfaceAge);
        CleanupSemanticEcho(clauses, SliderLanguageCatalog.Tension, SliderLanguageCatalog.Chaos);

        return clauses;
    }

    private static List<ClauseUnit> ApplyCoverageRichnessRecovery(
        List<ClauseUnit> clauses,
        IReadOnlyList<ClauseUnit> richnessSnapshot,
        RichnessDemandProfile richnessDemand)
    {
        if (!NeedsRichnessRecovery(clauses, richnessSnapshot, richnessDemand))
        {
            return clauses;
        }

        while (CountMeaningfulRichnessClauses(clauses) < GetTargetRichnessCount(richnessDemand))
        {
            var recovered = SelectRecoveryClause(richnessSnapshot, clauses, richnessDemand, preferSupplementalCategory: true);
            if (recovered is null)
            {
                break;
            }

            clauses.Add(recovered with { Score = recovered.Score + 6 });
        }

        return clauses;
    }

    private static List<ClauseUnit> ApplyMinimumVisualRichnessSafeguards(
        List<ClauseUnit> clauses,
        IReadOnlyList<ClauseUnit> richnessSnapshot,
        RichnessDemandProfile richnessDemand)
    {
        if (!HasCoreStructuralCoverage(clauses))
        {
            return clauses;
        }

        if (richnessDemand.WantsMaterialFinish && !HasRichnessCategory(clauses, RichnessCategory.MaterialFinish))
        {
            var materialClause = SelectRecoveryClause(richnessSnapshot, clauses, RichnessCategory.MaterialFinish);
            if (materialClause is not null)
            {
                clauses.Add(materialClause with { Score = materialClause.Score + 5 });
            }
        }

        if (richnessDemand.WantsSpatialDepth && !HasRichnessCategory(clauses, RichnessCategory.SpatialDepth))
        {
            var spatialClause = SelectRecoveryClause(richnessSnapshot, clauses, RichnessCategory.SpatialDepth);
            if (spatialClause is not null)
            {
                clauses.Add(spatialClause with { Score = spatialClause.Score + 4 });
            }
        }

        if (richnessDemand.WantsMotionEnergy && !HasRichnessCategory(clauses, RichnessCategory.MotionEnergy))
        {
            var motionClause = SelectRecoveryClause(richnessSnapshot, clauses, RichnessCategory.MotionEnergy);
            if (motionClause is not null)
            {
                clauses.Add(motionClause with { Score = motionClause.Score + 4 });
            }
        }

        return clauses;
    }

    private static List<ClauseUnit> ApplyFinalCrowdedStatePolish(
        List<ClauseUnit> clauses,
        IReadOnlyList<ClauseUnit> richnessSnapshot,
        RichnessDemandProfile richnessDemand)
    {
        if (clauses.Count <= 9)
        {
            return clauses;
        }

        var heavyMiddle = clauses.Count(clause => clause.Score >= 72);
        if (heavyMiddle < 5 && CountMeaningfulRichnessClauses(clauses) < GetTargetRichnessCount(richnessDemand) + 2)
        {
            return clauses;
        }

        var removable = clauses
            .Where(clause => clause.Score <= 66)
            .Where(clause => !clause.IsOwner)
            .Where(clause => clause.Group is not AssemblyGroup.ViewConstruction)
            .Where(clause => !IsProtectedRichnessClause(clause, clauses, richnessSnapshot))
            .OrderBy(GetSuppressionPriority)
            .ThenBy(clause => clause.IsFused ? 1 : 0)
            .ThenBy(clause => clause.Score)
            .FirstOrDefault();

        if (removable is not null
            && clauses.Any(clause => clause.Score >= 84)
            && (GetSuppressionPriority(removable) < 4 || CountMeaningfulRichnessClauses(clauses) > 1)
            && !WouldDropPadBelowSemanticFloor(removable, clauses))
        {
            clauses.Remove(removable);
        }

        return clauses;
    }

    private static List<ClauseUnit> ApplyFinalThinPromptCheck(
        List<ClauseUnit> clauses,
        IReadOnlyList<ClauseUnit> richnessSnapshot,
        RichnessDemandProfile richnessDemand)
    {
        if (!WouldReadThin(clauses, richnessDemand))
        {
            return clauses;
        }

        while (CountMeaningfulRichnessClauses(clauses) < GetTargetRichnessCount(richnessDemand))
        {
            var recovered = SelectRecoveryClause(richnessSnapshot, clauses, richnessDemand, preferSupplementalCategory: false);
            if (recovered is null)
            {
                break;
            }

            clauses.Add(recovered with { Score = recovered.Score + 7 });
        }

        return clauses;
    }

    private static IReadOnlyList<ClauseUnit> EnforceClauseCaps(
        IReadOnlyList<ClauseUnit> initialClauses,
        RichnessDemandProfile richnessDemand,
        IReadOnlyDictionary<MacroPad, PadSurvivorPlan> padPlans)
    {
        var kept = new List<ClauseUnit>();
        var overflow = new List<ClauseUnit>();

        foreach (var group in AssemblyGroupSequence)
        {
            var groupClauses = initialClauses.Where(clause => clause.Group == group).ToArray();
            if (groupClauses.Length == 0)
            {
                continue;
            }

            var cap = GroupCaps[group];
            if (groupClauses.Length <= cap)
            {
                kept.AddRange(groupClauses);
                continue;
            }

            var keepSet = groupClauses
                .OrderByDescending(clause => GetCapRetentionScore(clause, groupClauses, padPlans))
                .Take(cap)
                .ToHashSet();

            foreach (var clause in groupClauses)
            {
                if (keepSet.Contains(clause))
                {
                    kept.Add(clause);
                }
                else
                {
                    overflow.Add(clause with { Group = AssemblyGroup.TrailingRefiners });
                }
            }
        }

        kept = EnforceClassAndTotalCaps(kept, overflow, richnessDemand, padPlans);

        if (overflow.Count > 0 && kept.Count(clause => clause.Group == AssemblyGroup.TrailingRefiners) < GroupCaps[AssemblyGroup.TrailingRefiners])
        {
            var trailing = SelectTrailingRefiner(overflow);

            if (trailing is not null)
            {
                kept.Add(trailing with { Group = AssemblyGroup.TrailingRefiners });
            }
        }

        return kept
            .OrderBy(clause => Array.IndexOf(AssemblyGroupSequence, clause.Group))
            .ThenBy(clause => GetClauseOrder(clause))
            .ToArray();
    }

    private static List<ClauseUnit> EnforceClassAndTotalCaps(
        List<ClauseUnit> kept,
        List<ClauseUnit> overflow,
        RichnessDemandProfile richnessDemand,
        IReadOnlyDictionary<MacroPad, PadSurvivorPlan> padPlans)
    {
        TrimByClass(kept, overflow, ClauseClass.Primary, 5);
        TrimByClass(kept, overflow, ClauseClass.Support, richnessDemand.WantsMaterialFinish && richnessDemand.WantsSpatialDepth ? 5 : 4);
        TrimByClass(kept, overflow, ClauseClass.Modifier, 1);

        while (kept.Count > GetTotalClauseCap(richnessDemand))
        {
            var removable = kept
                .Where(clause => !WouldDropPadBelowPlannedFloor(clause, kept, padPlans))
                .DefaultIfEmpty(kept
                    .OrderBy(clause => IsProtectedRichnessClause(clause, kept, kept) ? 1 : 0)
                    .ThenBy(GetSuppressionPriority)
                    .ThenBy(clause => clause.IsOwner ? 1 : 0)
                    .ThenBy(clause => clause.IsFused ? 1 : 0)
                    .ThenBy(clause => clause.ClauseClass switch
                    {
                        ClauseClass.Modifier => 0,
                        ClauseClass.Support => 1,
                        _ => 2,
                    })
                    .ThenBy(clause => clause.Score)
                    .First())
                .OrderBy(clause => IsProtectedRichnessClause(clause, kept, kept) ? 1 : 0)
                .ThenBy(GetSuppressionPriority)
                .ThenBy(clause => clause.IsOwner ? 1 : 0)
                .ThenBy(clause => clause.IsFused ? 1 : 0)
                .ThenBy(clause => clause.ClauseClass switch
                {
                    ClauseClass.Modifier => 0,
                    ClauseClass.Support => 1,
                    _ => 2,
                })
                .ThenBy(clause => clause.Score)
                .First();

            kept.Remove(removable);
            overflow.Add(removable with { Group = AssemblyGroup.TrailingRefiners });
        }

        return kept;
    }

    private static ClauseUnit? SelectTrailingRefiner(IReadOnlyList<ClauseUnit> overflow)
    {
        return overflow
            .Where(clause => !IsWeakModifierOverflow(clause))
            .OrderByDescending(clause => IsRichnessBearingClause(clause) ? 1 : 0)
            .ThenByDescending(clause => clause.IsFused ? 1 : 0)
            .ThenByDescending(GetRetentionScore)
            .FirstOrDefault();
    }

    private static bool IsWeakModifierOverflow(ClauseUnit clause)
    {
        return clause.ClauseClass == ClauseClass.Modifier
            && !clause.IsFused
            && !clause.IsOwner
            && clause.Score < 58;
    }

    private static void TryTintWeakClause(
        List<ClauseUnit> clauses,
        string sliderKey,
        Func<string, string> toneSelector,
        Func<ClauseUnit, bool> targetFilter)
    {
        var source = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(sliderKey, StringComparer.OrdinalIgnoreCase));
        if (source is null || !IsWeakToneClause(source))
        {
            return;
        }

        var target = FindTintTarget(clauses, source, targetFilter);
        if (target is null)
        {
            return;
        }

        var qualifier = toneSelector(source.Text);
        if (string.IsNullOrWhiteSpace(qualifier))
        {
            return;
        }

        ReplaceClause(
            clauses,
            target,
            target with
            {
                Text = AppendCompactQualifier(target.Text, qualifier),
                Score = target.Score + 2,
                SliderKeys = [.. target.SliderKeys, sliderKey],
            });

        clauses.Remove(source);
    }

    private static void TryQuietWeakModifier(
        List<ClauseUnit> clauses,
        string sliderKey,
        Func<string, string> qualifierSelector,
        Func<ClauseUnit, bool> targetFilter)
    {
        var source = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(sliderKey, StringComparer.OrdinalIgnoreCase));
        if (source is null || !IsWeakModifierClause(source))
        {
            return;
        }

        var target = FindTintTarget(clauses, source, targetFilter);
        if (target is null)
        {
            return;
        }

        ReplaceClause(
            clauses,
            target,
            target with
            {
                Text = AppendCompactQualifier(target.Text, qualifierSelector(source.Text)),
                Score = target.Score + 1,
                SliderKeys = [.. target.SliderKeys, sliderKey],
            });

        clauses.Remove(source);
    }

    private static void SuppressWeakDuplicateMoodTail(List<ClauseUnit> clauses, string firstKey, string secondKey)
    {
        var first = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(firstKey, StringComparer.OrdinalIgnoreCase));
        var second = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(secondKey, StringComparer.OrdinalIgnoreCase));

        if (first is null || second is null || !IsWeakToneClause(first) || !IsWeakToneClause(second))
        {
            return;
        }

        if (CountExplicitToneSurvivors(clauses) <= 1)
        {
            return;
        }

        if (first.Score <= second.Score)
        {
            clauses.Remove(first);
        }
        else
        {
            clauses.Remove(second);
        }
    }

    private static void TryCompressWeakTailPair(List<ClauseUnit> clauses, string firstKey, string secondKey, string mergedText)
    {
        var first = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(firstKey, StringComparer.OrdinalIgnoreCase));
        var second = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(secondKey, StringComparer.OrdinalIgnoreCase));

        if (first is null
            || second is null
            || ReferenceEquals(first, second)
            || !IsWeakTailClause(first)
            || !IsWeakTailClause(second))
        {
            return;
        }

        if (IsDistinctPadCompanion(firstKey, secondKey)
            && CountPadSemanticSurvivors(clauses, GetMacroPad(firstKey)) <= 2)
        {
            return;
        }

        var merged = new ClauseUnit(
            Text: mergedText,
            Group: first.Group == AssemblyGroup.TrailingRefiners ? second.Group : first.Group,
            ClauseClass: ClauseClass.Modifier,
            Score: Math.Max(first.Score, second.Score) + 2,
            Mode: first.Mode,
            IsOwner: false,
            IsFused: true,
            SliderKeys: [.. first.SliderKeys, .. second.SliderKeys]);

        clauses.Remove(first);
        clauses.Remove(second);
        clauses.Add(merged);
    }

    private static void RefineTintCarrier(List<ClauseUnit> clauses, string sliderKey)
    {
        var source = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(sliderKey, StringComparer.OrdinalIgnoreCase));
        if (source is null || !IsCarrierRefinementCandidate(source, sliderKey))
        {
            return;
        }

        var carrier = SelectPreferredCarrier(clauses, source, sliderKey);
        if (carrier is null)
        {
            return;
        }

        var qualifier = GetCarrierQualifier(sliderKey, source.Text);
        if (string.IsNullOrWhiteSpace(qualifier))
        {
            return;
        }

        ReplaceClause(
            clauses,
            carrier,
            carrier with
            {
                Text = AppendCarrierQualifier(carrier, qualifier, sliderKey),
                Score = carrier.Score + 1,
                SliderKeys = [.. carrier.SliderKeys, sliderKey],
            });

        clauses.Remove(source);
    }

    private static void RefineCarrierWithScoring(List<ClauseUnit> clauses, string sliderKey)
    {
        var source = clauses.FirstOrDefault(clause => clause.SliderKeys.Length == 1 && clause.SliderKeys.Contains(sliderKey, StringComparer.OrdinalIgnoreCase));
        if (source is null || !IsCarrierRefinementCandidate(source, sliderKey))
        {
            return;
        }

        var carrier = clauses
            .Where(clause => !ReferenceEquals(clause, source))
            .Where(clause => IsAllowedCarrierForSlider(sliderKey, clause))
            .Where(clause => !WouldOverloadCarrier(clause, sliderKey))
            .Select(clause => new { Clause = clause, Score = GetRefinedCarrierScore(clauses, source, clause, sliderKey) })
            .Where(candidate => candidate.Score >= 70)
            .OrderByDescending(candidate => candidate.Score)
            .FirstOrDefault()
            ?.Clause;

        if (carrier is null)
        {
            return;
        }

        var qualifier = GetCarrierQualifier(sliderKey, source.Text);
        if (string.IsNullOrWhiteSpace(qualifier))
        {
            return;
        }

        ReplaceClause(
            clauses,
            carrier,
            carrier with
            {
                Text = AppendCarrierQualifier(carrier, qualifier, sliderKey),
                Score = carrier.Score + 1,
                SliderKeys = [.. carrier.SliderKeys, sliderKey],
            });

        clauses.Remove(source);
    }

    private static int GetRefinedCarrierScore(IReadOnlyList<ClauseUnit> clauses, ClauseUnit source, ClauseUnit carrier, string sliderKey)
    {
        var score = GetCarrierPreferenceScore(sliderKey, carrier);

        if (carrier.Score >= source.Score + 6)
        {
            score += 8;
        }

        if (carrier.IsOwner)
        {
            score += 6;
        }

        if (carrier.IsFused)
        {
            score += 5;
        }

        var abstractCount = GetAttachedAbstractTintCount(carrier);
        var physicalCount = GetAttachedPhysicalTintCount(carrier);

        if (abstractCount == 0 && IsAbstractTint(sliderKey))
        {
            score += 5;
        }

        if (physicalCount == 0 && IsPhysicalTint(sliderKey))
        {
            score += 5;
        }

        if (abstractCount > 0 && physicalCount > 0)
        {
            score -= 8;
        }

        if (CarrierAlreadyEchoesMeaning(carrier, sliderKey))
        {
            score -= 14;
        }

        var sourceIndex = IndexOfClause(clauses, source);
        var carrierIndex = IndexOfClause(clauses, carrier);
        if (sourceIndex >= 0 && carrierIndex >= 0 && Math.Abs(sourceIndex - carrierIndex) <= 1)
        {
            score += 2;
        }

        if (WouldBecomeAwkwardlyStacked(carrier, sliderKey))
        {
            score -= 10;
        }

        return score;
    }

    private static ClauseUnit? FindTintTarget(
        IReadOnlyList<ClauseUnit> clauses,
        ClauseUnit source,
        Func<ClauseUnit, bool> targetFilter)
    {
        var sourceIndex = IndexOfClause(clauses, source);

        return clauses
            .Where(clause => !ReferenceEquals(clause, source))
            .Where(targetFilter)
            .Where(clause => clause.Score >= source.Score + 4 || clause.IsFused || clause.IsOwner)
            .OrderBy(clause => Math.Abs(IndexOfClause(clauses, clause) - sourceIndex))
            .ThenByDescending(clause => clause.IsFused ? 1 : 0)
            .ThenByDescending(clause => clause.Score)
            .FirstOrDefault();
    }

    private static void ReplaceClause(List<ClauseUnit> clauses, ClauseUnit existing, ClauseUnit updated)
    {
        var index = clauses.IndexOf(existing);
        if (index >= 0)
        {
            clauses[index] = updated;
        }
    }

    private static int IndexOfClause(IReadOnlyList<ClauseUnit> clauses, ClauseUnit target)
    {
        for (var index = 0; index < clauses.Count; index++)
        {
            if (EqualityComparer<ClauseUnit>.Default.Equals(clauses[index], target))
            {
                return index;
            }
        }

        return -1;
    }

    private static ClauseUnit? SelectPreferredCarrier(IReadOnlyList<ClauseUnit> clauses, ClauseUnit source, string sliderKey)
    {
        return clauses
            .Where(clause => !ReferenceEquals(clause, source))
            .Where(clause => clause.Score >= source.Score || clause.IsOwner || clause.IsFused)
            .Where(clause => IsAllowedCarrierForSlider(sliderKey, clause))
            .Where(clause => !WouldOverloadCarrier(clause, sliderKey))
            .OrderByDescending(clause => GetCarrierPreferenceScore(sliderKey, clause))
            .ThenByDescending(clause => clause.Score)
            .ThenBy(clause => Math.Abs(IndexOfClause(clauses, clause) - IndexOfClause(clauses, source)))
            .FirstOrDefault();
    }

    private static bool IsAllowedCarrierForSlider(string sliderKey, ClauseUnit clause)
    {
        var group = clause.Group;
        return sliderKey switch
        {
            var key when string.Equals(key, SliderLanguageCatalog.Whimsy, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.RenderingIdentity or AssemblyGroup.CompositionEnvironment or AssemblyGroup.MoodSymbolicTone,
            var key when string.Equals(key, SliderLanguageCatalog.Symbolism, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.CompositionEnvironment or AssemblyGroup.RenderingIdentity or AssemblyGroup.LightingColorAtmosphere or AssemblyGroup.MoodSymbolicTone,
            var key when string.Equals(key, SliderLanguageCatalog.Awe, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.LightingColorAtmosphere or AssemblyGroup.ViewConstruction or AssemblyGroup.CompositionEnvironment,
            var key when string.Equals(key, SliderLanguageCatalog.Tension, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.MotionInstability or AssemblyGroup.ViewConstruction or AssemblyGroup.LightingColorAtmosphere,
            var key when string.Equals(key, SliderLanguageCatalog.SurfaceAge, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.RenderingIdentity or AssemblyGroup.CompositionEnvironment,
            var key when string.Equals(key, SliderLanguageCatalog.ImageCleanliness, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.RenderingIdentity or AssemblyGroup.LightingColorAtmosphere,
            var key when string.Equals(key, SliderLanguageCatalog.Saturation, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.LightingColorAtmosphere or AssemblyGroup.TrailingRefiners,
            var key when string.Equals(key, SliderLanguageCatalog.Contrast, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.LightingColorAtmosphere or AssemblyGroup.MotionInstability or AssemblyGroup.TrailingRefiners,
            var key when string.Equals(key, SliderLanguageCatalog.NarrativeDensity, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.CompositionEnvironment or AssemblyGroup.MoodSymbolicTone,
            var key when string.Equals(key, SliderLanguageCatalog.Chaos, StringComparison.OrdinalIgnoreCase)
                => group is AssemblyGroup.MotionInstability or AssemblyGroup.CompositionEnvironment,
            _ => group is not AssemblyGroup.TrailingRefiners,
        };
    }

    private static int GetCarrierPreferenceScore(string sliderKey, ClauseUnit clause)
    {
        var groupScore = sliderKey switch
        {
            var key when string.Equals(key, SliderLanguageCatalog.Whimsy, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.RenderingIdentity => 100,
                AssemblyGroup.CompositionEnvironment => 80,
                AssemblyGroup.MoodSymbolicTone => 60,
                _ => 0,
            },
            var key when string.Equals(key, SliderLanguageCatalog.Symbolism, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.CompositionEnvironment => 100,
                AssemblyGroup.RenderingIdentity => 88,
                AssemblyGroup.LightingColorAtmosphere => 72,
                AssemblyGroup.MoodSymbolicTone => 66,
                _ => 0,
            },
            var key when string.Equals(key, SliderLanguageCatalog.Awe, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.LightingColorAtmosphere => 100,
                AssemblyGroup.ViewConstruction => 92,
                AssemblyGroup.CompositionEnvironment => 84,
                _ => 0,
            },
            var key when string.Equals(key, SliderLanguageCatalog.Tension, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.MotionInstability => 100,
                AssemblyGroup.ViewConstruction => 82,
                AssemblyGroup.LightingColorAtmosphere => 72,
                _ => 0,
            },
            var key when string.Equals(key, SliderLanguageCatalog.SurfaceAge, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.RenderingIdentity => 100,
                AssemblyGroup.CompositionEnvironment => 50,
                _ => 0,
            },
            var key when string.Equals(key, SliderLanguageCatalog.ImageCleanliness, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.RenderingIdentity => 100,
                AssemblyGroup.LightingColorAtmosphere => 58,
                _ => 0,
            },
            var key when string.Equals(key, SliderLanguageCatalog.Saturation, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.LightingColorAtmosphere => 100,
                AssemblyGroup.TrailingRefiners => 40,
                _ => 0,
            },
            var key when string.Equals(key, SliderLanguageCatalog.Contrast, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.LightingColorAtmosphere => 100,
                AssemblyGroup.MotionInstability => 62,
                AssemblyGroup.TrailingRefiners => 48,
                _ => 0,
            },
            var key when string.Equals(key, SliderLanguageCatalog.NarrativeDensity, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.CompositionEnvironment => 100,
                AssemblyGroup.MoodSymbolicTone => 70,
                _ => 0,
            },
            var key when string.Equals(key, SliderLanguageCatalog.Chaos, StringComparison.OrdinalIgnoreCase) => clause.Group switch
            {
                AssemblyGroup.MotionInstability => 100,
                AssemblyGroup.CompositionEnvironment => 68,
                _ => 0,
            },
            _ => 0,
        };

        return groupScore
            + (clause.IsFused ? 8 : 0)
            + (clause.IsOwner ? 6 : 0)
            + (IsPhysicalResidue(clause) ? 4 : 0);
    }

    private static bool WouldOverloadCarrier(ClauseUnit clause, string sliderKey)
    {
        var abstractCount = GetAttachedAbstractTintCount(clause);
        var physicalCount = GetAttachedPhysicalTintCount(clause);
        var incomingAbstract = IsAbstractTint(sliderKey);
        var incomingPhysical = IsPhysicalTint(sliderKey);

        if (incomingAbstract && abstractCount >= 1 && physicalCount >= 1)
        {
            return true;
        }

        if (incomingAbstract && abstractCount >= 2)
        {
            return true;
        }

        if (incomingPhysical && physicalCount >= 1 && abstractCount >= 1)
        {
            return true;
        }

        return false;
    }

    private static bool WouldBecomeAwkwardlyStacked(ClauseUnit carrier, string sliderKey)
    {
        var text = carrier.Text;
        var withCount = CountOccurrences(text, " with ");
        var commaCount = CountOccurrences(text, ", ");

        if (withCount >= 2 || commaCount >= 3)
        {
            return true;
        }

        if (IsAbstractTint(sliderKey) && text.Contains("undertone", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (IsPhysicalTint(sliderKey) && text.Contains("finish", StringComparison.OrdinalIgnoreCase) && text.Contains("surface", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private static bool CarrierAlreadyEchoesMeaning(ClauseUnit carrier, string sliderKey)
    {
        return sliderKey switch
        {
            var key when string.Equals(key, SliderLanguageCatalog.TextureDepth, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.DetailDensity, StringComparison.OrdinalIgnoreCase)
                    => carrier.Text.Contains("tactile", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("detail", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("texture", StringComparison.OrdinalIgnoreCase),
            var key when string.Equals(key, SliderLanguageCatalog.LightingIntensity, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.Contrast, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.Saturation, StringComparison.OrdinalIgnoreCase)
                    => carrier.Text.Contains("lighting", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("illumination", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("contrast", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("color", StringComparison.OrdinalIgnoreCase),
            var key when string.Equals(key, SliderLanguageCatalog.Awe, StringComparison.OrdinalIgnoreCase)
                    => carrier.Text.Contains("wonder", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("scale", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("atmosphere", StringComparison.OrdinalIgnoreCase),
            var key when string.Equals(key, SliderLanguageCatalog.Symbolism, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.NarrativeDensity, StringComparison.OrdinalIgnoreCase)
                    => carrier.Text.Contains("symbolic", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("story", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("narrative", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("undertone", StringComparison.OrdinalIgnoreCase),
            var key when string.Equals(key, SliderLanguageCatalog.ImageCleanliness, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.SurfaceAge, StringComparison.OrdinalIgnoreCase)
                    => carrier.Text.Contains("finish", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("polished", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("time-worn", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("weathered", StringComparison.OrdinalIgnoreCase),
            var key when string.Equals(key, SliderLanguageCatalog.Tension, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.Chaos, StringComparison.OrdinalIgnoreCase)
                    => carrier.Text.Contains("tension", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("pressure", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("energy", StringComparison.OrdinalIgnoreCase)
                        || carrier.Text.Contains("chaotic", StringComparison.OrdinalIgnoreCase),
            _ => false,
        };
    }

    private static bool IsCarrierRefinementCandidate(ClauseUnit clause, string sliderKey)
    {
        if (!clause.SliderKeys.Contains(sliderKey, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        return sliderKey switch
        {
            var key when string.Equals(key, SliderLanguageCatalog.NarrativeDensity, StringComparison.OrdinalIgnoreCase)
                => IsWeakTailClause(clause),
            var key when string.Equals(key, SliderLanguageCatalog.Chaos, StringComparison.OrdinalIgnoreCase)
                => IsWeakTailClause(clause) || IsWeakToneClause(clause),
            _ => IsWeakToneClause(clause) || IsWeakModifierClause(clause) || IsWeakTailClause(clause),
        };
    }

    private static string GetCarrierQualifier(string sliderKey, string phrase)
    {
        return sliderKey switch
        {
            var key when string.Equals(key, SliderLanguageCatalog.Whimsy, StringComparison.OrdinalIgnoreCase) => NormalizeWhimsyTone(phrase),
            var key when string.Equals(key, SliderLanguageCatalog.Symbolism, StringComparison.OrdinalIgnoreCase) => NormalizeSymbolismTone(phrase),
            var key when string.Equals(key, SliderLanguageCatalog.Awe, StringComparison.OrdinalIgnoreCase) => NormalizeAweTone(phrase),
            var key when string.Equals(key, SliderLanguageCatalog.Tension, StringComparison.OrdinalIgnoreCase) => NormalizeTensionTone(phrase),
            var key when string.Equals(key, SliderLanguageCatalog.SurfaceAge, StringComparison.OrdinalIgnoreCase) => NormalizeSurfaceAgeTail(phrase),
            var key when string.Equals(key, SliderLanguageCatalog.ImageCleanliness, StringComparison.OrdinalIgnoreCase) => NormalizeImageCleanlinessTail(phrase),
            var key when string.Equals(key, SliderLanguageCatalog.Saturation, StringComparison.OrdinalIgnoreCase) => NormalizeSaturationTail(phrase),
            var key when string.Equals(key, SliderLanguageCatalog.Contrast, StringComparison.OrdinalIgnoreCase) => NormalizeContrastTail(phrase),
            var key when string.Equals(key, SliderLanguageCatalog.NarrativeDensity, StringComparison.OrdinalIgnoreCase) => NormalizeNarrativeTone(phrase),
            var key when string.Equals(key, SliderLanguageCatalog.Chaos, StringComparison.OrdinalIgnoreCase) => NormalizeChaosQualifier(phrase),
            _ => LowerClause(phrase),
        };
    }

    private static string AppendCarrierQualifier(ClauseUnit carrier, string qualifier, string sliderKey)
    {
        if (string.IsNullOrWhiteSpace(qualifier) || carrier.Text.Contains(qualifier, StringComparison.OrdinalIgnoreCase))
        {
            return carrier.Text;
        }

        if (IsPhysicalTint(sliderKey) && carrier.Group == AssemblyGroup.RenderingIdentity)
        {
            return carrier.Text.Contains(" with ", StringComparison.OrdinalIgnoreCase)
                ? $"{carrier.Text}, {qualifier}"
                : $"{carrier.Text} with {qualifier}";
        }

        if (IsAbstractTint(sliderKey) && CountOccurrences(carrier.Text, " with ") == 0)
        {
            return $"{carrier.Text} with {qualifier}";
        }

        return $"{carrier.Text}, {qualifier}";
    }

    private static void TrimByClass(List<ClauseUnit> kept, List<ClauseUnit> overflow, ClauseClass clauseClass, int maxCount)
    {
        while (kept.Count(clause => clause.ClauseClass == clauseClass) > maxCount)
        {
            var removable = kept
                .Where(clause => clause.ClauseClass == clauseClass)
                .OrderBy(clause => IsProtectedRichnessClause(clause, kept, kept) ? 1 : 0)
                .ThenBy(GetSuppressionPriority)
                .ThenBy(clause => clause.IsOwner ? 1 : 0)
                .ThenBy(clause => clause.IsFused ? 1 : 0)
                .ThenBy(clause => clause.Score)
                .First();

            kept.Remove(removable);
            overflow.Add(removable with { Group = AssemblyGroup.TrailingRefiners });
        }
    }

    private static int GetRetentionScore(ClauseUnit clause)
    {
        return clause.Score
            + (clause.IsOwner ? 10 : 0)
            + (clause.IsFused ? 6 : 0)
            + (IsRichnessBearingClause(clause) ? 10 : 0)
            + (IsAbstractResidue(clause) ? -4 : 0)
            + clause.ClauseClass switch
            {
                ClauseClass.Primary => 12,
                ClauseClass.Support => 6,
                _ => 0,
            };
    }

    private static int GetClauseScore(ResolvedSliderState state, ClauseClass clauseClass, bool isOwner)
    {
        return state.FinalWeight
            + (state.Mode == EmissionMode.Direct ? 6 : state.Mode == EmissionMode.Fuse ? 3 : 0)
            + (isOwner ? 4 : 0)
            + clauseClass switch
            {
                ClauseClass.Primary => 8,
                ClauseClass.Support => 4,
                _ => 0,
            };
    }

    private static int GetClauseOrder(ClauseUnit clause)
    {
        var firstSliderKey = clause.SliderKeys.FirstOrDefault() ?? string.Empty;
        return GetAssemblySliderOrder(firstSliderKey);
    }

    private static ResolvedSliderState? GetUnusedSurvivor(IReadOnlyList<ResolvedSliderState> survivors, ISet<string> usedKeys, string sliderKey)
    {
        return survivors.FirstOrDefault(state =>
            !usedKeys.Contains(state.Candidate.Rule.SliderKey)
            && string.Equals(state.Candidate.Rule.SliderKey, sliderKey, StringComparison.OrdinalIgnoreCase));
    }

    private static ResolvedSliderState SelectFusionOwner(ResolvedSliderState first, ResolvedSliderState second, bool preferFirstOnClose)
    {
        var delta = Math.Abs(first.FinalWeight - second.FinalWeight);
        if (delta <= 6)
        {
            return preferFirstOnClose ? first : second;
        }

        return first.FinalWeight >= second.FinalWeight ? first : second;
    }

    private static string LowerClause(string phrase)
    {
        if (string.IsNullOrWhiteSpace(phrase))
        {
            return string.Empty;
        }

        return char.ToLowerInvariant(phrase[0]) + phrase[1..];
    }

    private static string NormalizeCameraDistance(string phrase)
    {
        return phrase.EndsWith(" view", StringComparison.OrdinalIgnoreCase)
            ? phrase[..^5]
            : phrase;
    }

    private static string NormalizeCameraAngle(string phrase)
    {
        return phrase
            .Replace(" angle view", " angle", StringComparison.OrdinalIgnoreCase)
            .Replace(" view", string.Empty, StringComparison.OrdinalIgnoreCase);
    }

    private static ResolvedSliderState? SelectLightingOwner(
        ResolvedSliderState? preset,
        ResolvedSliderState? intensity,
        ResolvedSliderState? temperature)
    {
        if (preset is not null && preset.Mode is EmissionMode.Direct or EmissionMode.Fuse)
        {
            var presetText = preset.Candidate.Phrase;
            var presetIsGeneric = string.IsNullOrWhiteSpace(presetText)
                || presetText.Contains("daylight", StringComparison.OrdinalIgnoreCase)
                || presetText.Contains("soft", StringComparison.OrdinalIgnoreCase)
                || presetText.Contains("light", StringComparison.OrdinalIgnoreCase) && !presetText.Contains("studio", StringComparison.OrdinalIgnoreCase);

            if (!presetIsGeneric || preset.FinalWeight >= 82)
            {
                return preset;
            }
        }

        if (intensity is not null
            && intensity.Mode is EmissionMode.Direct or EmissionMode.Fuse
            && (preset is null || intensity.FinalWeight >= preset.FinalWeight - 6))
        {
            return intensity;
        }

        if (temperature is not null && temperature.Mode is EmissionMode.Direct or EmissionMode.Fuse)
        {
            return temperature;
        }

        return preset ?? intensity ?? temperature;
    }

    private static ResolvedSliderState? SelectBestState(ResolvedSliderState? first, ResolvedSliderState? second)
    {
        if (first is null)
        {
            return second;
        }

        if (second is null)
        {
            return first;
        }

        return first.FinalWeight >= second.FinalWeight ? first : second;
    }

    private static ClauseUnit? CombineEnvironmentDepthClauses(ClauseUnit background, ClauseUnit atmosphere)
    {
        if (background.Group != AssemblyGroup.CompositionEnvironment || atmosphere.Group != AssemblyGroup.CompositionEnvironment)
        {
            return null;
        }

        return new ClauseUnit(
            Text: $"{background.Text} with {LowerClause(atmosphere.Text)}",
            Group: AssemblyGroup.CompositionEnvironment,
            ClauseClass: ClauseClass.Support,
            Score: Math.Max(background.Score, atmosphere.Score) + 4,
            Mode: background.Mode,
            IsOwner: background.IsOwner || atmosphere.IsOwner,
            IsFused: true,
            SliderKeys: [.. background.SliderKeys, .. atmosphere.SliderKeys]);
    }

    private static ClauseUnit? CombineFocusEnvironmentClauses(ClauseUnit focus, ClauseUnit background)
    {
        var owner = focus.Score >= background.Score ? focus : background;
        var text = ReferenceEquals(owner, focus)
            ? $"{focus.Text} with {LowerClause(background.Text)}"
            : $"{background.Text} with {LowerClause(focus.Text)}";

        return new ClauseUnit(
            Text: text,
            Group: owner.Group,
            ClauseClass: ClauseClass.Support,
            Score: Math.Max(focus.Score, background.Score) + 4,
            Mode: owner.Mode,
            IsOwner: owner.IsOwner,
            IsFused: true,
            SliderKeys: [.. focus.SliderKeys, .. background.SliderKeys]);
    }

    private static ClauseUnit? CombineDenseTactileClauses(ClauseUnit texture, ClauseUnit detail)
    {
        if (ReferenceEquals(texture, detail))
        {
            return null;
        }

        return new ClauseUnit(
            Text: $"{texture.Text} with {NormalizeDetailDensityQualifier(detail.Text)}",
            Group: AssemblyGroup.RenderingIdentity,
            ClauseClass: ClauseClass.Primary,
            Score: Math.Max(texture.Score, detail.Score) + 4,
            Mode: texture.Mode,
            IsOwner: texture.IsOwner || detail.IsOwner,
            IsFused: true,
            SliderKeys: [.. texture.SliderKeys, .. detail.SliderKeys]);
    }

    private static string NormalizeLightingPresetClause(string phrase)
    {
        if (phrase.Contains("lighting", StringComparison.OrdinalIgnoreCase)
            || phrase.Contains("light", StringComparison.OrdinalIgnoreCase) && !phrase.Contains("daylight", StringComparison.OrdinalIgnoreCase))
        {
            return phrase;
        }

        return phrase.Contains("daylight", StringComparison.OrdinalIgnoreCase)
            ? phrase
            : $"{phrase} lighting";
    }

    private static string NormalizeLightingIntensityOwner(string phrase, string? presetPhrase)
    {
        if (!string.IsNullOrWhiteSpace(presetPhrase) && presetPhrase.Contains("studio", StringComparison.OrdinalIgnoreCase))
        {
            return $"{NormalizeLightingIntensityQualifier(phrase)} studio light";
        }

        return phrase
            .Replace(" scene lighting", " illumination", StringComparison.OrdinalIgnoreCase)
            .Replace(" lighting", " illumination", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeLightingIntensityQualifier(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("balanced lighting", StringComparison.OrdinalIgnoreCase) => "balanced lighting",
            var value when value.Contains("bright scene lighting", StringComparison.OrdinalIgnoreCase) => "bright scene illumination",
            var value when value.Contains("soft lighting", StringComparison.OrdinalIgnoreCase) => "soft illumination",
            var value when value.Contains("dim lighting", StringComparison.OrdinalIgnoreCase) => "dim illumination",
            var value when value.Contains("radiant luminous lighting", StringComparison.OrdinalIgnoreCase) => "radiant illumination",
            _ => phrase.Replace(" lighting", " illumination", StringComparison.OrdinalIgnoreCase),
        };
    }

    private static string NormalizeTemperatureQualifier(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("cool", StringComparison.OrdinalIgnoreCase) => "cool atmosphere",
            var value when value.Contains("neutral", StringComparison.OrdinalIgnoreCase) => "neutral temperature",
            var value when value.Contains("heated", StringComparison.OrdinalIgnoreCase) => "heated atmosphere",
            var value when value.Contains("warm", StringComparison.OrdinalIgnoreCase) => "warm temperature",
            _ => phrase,
        };
    }

    private static string NormalizeTemperatureOwner(string phrase, string? intensityPhrase, string? presetPhrase)
    {
        var lead = NormalizeTemperatureQualifier(phrase);

        if (!string.IsNullOrWhiteSpace(presetPhrase))
        {
            var normalizedPreset = NormalizeLightingPresetClause(presetPhrase);
            if (normalizedPreset.Contains("daylight", StringComparison.OrdinalIgnoreCase))
            {
                return $"{lead} {normalizedPreset}";
            }
        }

        if (!string.IsNullOrWhiteSpace(intensityPhrase))
        {
            return $"{NormalizeLightingIntensityQualifier(intensityPhrase)} with {lead}";
        }

        return $"{lead} lighting";
    }

    private static string NormalizeDetailDensityOwner(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("dense detail layering", StringComparison.OrdinalIgnoreCase) => "dense worked surface",
            var value when value.Contains("rich fine detail", StringComparison.OrdinalIgnoreCase) => "rich worked surface detail",
            var value when value.Contains("moderate detail density", StringComparison.OrdinalIgnoreCase) => "moderately worked surface",
            var value when value.Contains("light detail presence", StringComparison.OrdinalIgnoreCase) => "lightly worked surface",
            _ => phrase,
        };
    }

    private static string NormalizeDetailDensityQualifier(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("dense detail layering", StringComparison.OrdinalIgnoreCase) => "dense tactile detail",
            var value when value.Contains("rich fine detail", StringComparison.OrdinalIgnoreCase) => "rich fine detail",
            var value when value.Contains("moderate detail density", StringComparison.OrdinalIgnoreCase) => "moderate detail density",
            var value when value.Contains("light detail presence", StringComparison.OrdinalIgnoreCase) => "light detail presence",
            var value when value.Contains("sparse detail treatment", StringComparison.OrdinalIgnoreCase) => "sparse detail treatment",
            _ => LowerClause(phrase),
        };
    }

    private static string NormalizeTextureDepthQualifier(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("deeply worked tactile relief", StringComparison.OrdinalIgnoreCase) => "deep tactile character",
            var value when value.Contains("rich tactile surface detail", StringComparison.OrdinalIgnoreCase) => "rich tactile character",
            var value when value.Contains("clear material texture", StringComparison.OrdinalIgnoreCase) => "clear material character",
            var value when value.Contains("light surface texture", StringComparison.OrdinalIgnoreCase) => "light texture",
            var value when value.Contains("minimal added texture", StringComparison.OrdinalIgnoreCase) => "minimal texture",
            _ => LowerClause(phrase),
        };
    }

    private static string NormalizeImageCleanlinessOwner(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("polished visual finish", StringComparison.OrdinalIgnoreCase) => "polished finish",
            var value when value.Contains("clean visual finish", StringComparison.OrdinalIgnoreCase) => "clean finish",
            var value when value.Contains("balanced finish", StringComparison.OrdinalIgnoreCase) => "balanced finish",
            var value when value.Contains("slight visual grit", StringComparison.OrdinalIgnoreCase) => "slight grit",
            var value when value.Contains("raw visual finish", StringComparison.OrdinalIgnoreCase) => "raw finish",
            _ => phrase,
        };
    }

    private static string NormalizeImageCleanlinessQualifier(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("polished visual finish", StringComparison.OrdinalIgnoreCase) => "a polished finish",
            var value when value.Contains("clean visual finish", StringComparison.OrdinalIgnoreCase) => "a clean finish",
            var value when value.Contains("balanced finish", StringComparison.OrdinalIgnoreCase) => "a balanced finish",
            var value when value.Contains("slight visual grit", StringComparison.OrdinalIgnoreCase) => "slight grit",
            var value when value.Contains("raw visual finish", StringComparison.OrdinalIgnoreCase) => "a raw finish",
            _ => LowerClause(phrase),
        };
    }

    private static string NormalizeSurfaceAgeQualifier(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("time-worn", StringComparison.OrdinalIgnoreCase) => "time-worn material character",
            var value when value.Contains("weathered", StringComparison.OrdinalIgnoreCase) => "weathered material character",
            var value when value.Contains("freshly", StringComparison.OrdinalIgnoreCase) || value.Contains("newly", StringComparison.OrdinalIgnoreCase) => "fresh material character",
            var value when value.Contains("subtle", StringComparison.OrdinalIgnoreCase) => "subtle patina",
            _ => LowerClause(phrase),
        };
    }

    private static string NormalizeChaosQualifier(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("high visual instability", StringComparison.OrdinalIgnoreCase) => "high instability",
            var value when value.Contains("orchestrated", StringComparison.OrdinalIgnoreCase) || value.Contains("controlled", StringComparison.OrdinalIgnoreCase) => "controlled turbulence",
            var value when value.Contains("volatile", StringComparison.OrdinalIgnoreCase) => "volatile energy",
            var value when value.Contains("restless", StringComparison.OrdinalIgnoreCase) => "restless pressure",
            _ => LowerClause(phrase),
        };
    }

    private static string NormalizeTensionQualifier(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("pressure", StringComparison.OrdinalIgnoreCase) => LowerClause(phrase),
            var value when value.Contains("strain", StringComparison.OrdinalIgnoreCase) => LowerClause(phrase),
            var value when value.Contains("tension", StringComparison.OrdinalIgnoreCase) => LowerClause(phrase),
            _ => $"dramatic pressure from {LowerClause(phrase)}",
        };
    }

    private static string NormalizeNarrativeDensityQualifier(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("light narrative layering", StringComparison.OrdinalIgnoreCase) => "light narrative presence",
            var value when value.Contains("layered storytelling cues", StringComparison.OrdinalIgnoreCase) => "layered narrative presence",
            var value when value.Contains("dense implied story", StringComparison.OrdinalIgnoreCase) => "dense story presence",
            _ => LowerClause(phrase),
        };
    }

    private static string NormalizeWhimsyTone(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("subtle whimsy", StringComparison.OrdinalIgnoreCase) => "a playful edge",
            var value when value.Contains("playful tone", StringComparison.OrdinalIgnoreCase) => "a playful tone",
            var value when value.Contains("whimsical", StringComparison.OrdinalIgnoreCase) => "a whimsical edge",
            _ => "a playful edge",
        };
    }

    private static string NormalizeSymbolismTone(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("mythic", StringComparison.OrdinalIgnoreCase) => "a lightly mythic undertone",
            var value when value.Contains("symbolic", StringComparison.OrdinalIgnoreCase) => "a symbolic undertone",
            var value when value.Contains("allegorical", StringComparison.OrdinalIgnoreCase) => "an allegorical undertone",
            _ => "a symbolic undertone",
        };
    }

    private static string NormalizeAweTone(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("grandeur", StringComparison.OrdinalIgnoreCase) => "wonder-tinged scale",
            var value when value.Contains("wonder", StringComparison.OrdinalIgnoreCase) => "a wonder-tinged edge",
            var value when value.Contains("awe", StringComparison.OrdinalIgnoreCase) => "a sense of wonder",
            _ => "a wonder-tinged edge",
        };
    }

    private static string NormalizeTensionTone(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("pressure", StringComparison.OrdinalIgnoreCase) => "tense visual energy",
            var value when value.Contains("strain", StringComparison.OrdinalIgnoreCase) => "dramatic strain",
            var value when value.Contains("tension", StringComparison.OrdinalIgnoreCase) => "tense visual energy",
            _ => "dramatic pressure",
        };
    }

    private static string NormalizeNarrativeTone(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("story", StringComparison.OrdinalIgnoreCase) => "implied story presence",
            var value when value.Contains("narrative", StringComparison.OrdinalIgnoreCase) => "light narrative presence",
            _ => "implied story presence",
        };
    }

    private static string NormalizeSurfaceAgeTail(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("time-worn", StringComparison.OrdinalIgnoreCase) => "slightly time-worn",
            var value when value.Contains("weathered", StringComparison.OrdinalIgnoreCase) => "slightly weathered",
            var value when value.Contains("fresh", StringComparison.OrdinalIgnoreCase) => "freshly finished",
            _ => "subtle patina",
        };
    }

    private static string NormalizeImageCleanlinessTail(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("polished", StringComparison.OrdinalIgnoreCase) => "a polished finish",
            var value when value.Contains("clean", StringComparison.OrdinalIgnoreCase) => "a clean finish",
            var value when value.Contains("grit", StringComparison.OrdinalIgnoreCase) => "slight grit",
            _ => "a balanced finish",
        };
    }

    private static string NormalizeSaturationTail(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("vivid", StringComparison.OrdinalIgnoreCase) => "rich color lift",
            var value when value.Contains("rich", StringComparison.OrdinalIgnoreCase) => "rich color",
            var value when value.Contains("muted", StringComparison.OrdinalIgnoreCase) => "controlled color",
            _ => "controlled color",
        };
    }

    private static string NormalizeContrastTail(string phrase)
    {
        return phrase switch
        {
            var value when value.Contains("striking", StringComparison.OrdinalIgnoreCase) => "controlled contrast",
            var value when value.Contains("crisp", StringComparison.OrdinalIgnoreCase) => "crisp contrast",
            var value when value.Contains("gentle", StringComparison.OrdinalIgnoreCase) => "gentle contrast",
            _ => "controlled contrast",
        };
    }

    private static bool IsWeakToneClause(ClauseUnit clause)
    {
        return clause.Score <= 68 && (!clause.IsOwner || clause.ClauseClass != ClauseClass.Primary);
    }

    private static bool IsWeakModifierClause(ClauseUnit clause)
    {
        return clause.Score <= 66 && clause.ClauseClass != ClauseClass.Primary;
    }

    private static bool IsWeakTailClause(ClauseUnit clause)
    {
        return clause.Score <= 64 && clause.Group != AssemblyGroup.ViewConstruction && clause.Group != AssemblyGroup.RenderingIdentity;
    }

    private static bool IsTinyTailClause(ClauseUnit clause)
    {
        return clause.Score <= 66
            && clause.Group is not AssemblyGroup.ViewConstruction
            && clause.Group is not AssemblyGroup.RenderingIdentity
            && clause.Group is not AssemblyGroup.LightingColorAtmosphere;
    }

    private static bool IsAbstractTail(ClauseUnit clause)
    {
        return clause.Text.Contains("undertone", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("presence", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("wonder", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("symbolic", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("tension", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAbstractResidue(ClauseUnit clause)
    {
        return clause.SliderKeys.Any(IsAbstractTint)
            || clause.Group == AssemblyGroup.MoodSymbolicTone
            || IsAbstractTail(clause);
    }

    private static bool IsPhysicalResidue(ClauseUnit clause)
    {
        return clause.Group is AssemblyGroup.RenderingIdentity
            or AssemblyGroup.ViewConstruction
            or AssemblyGroup.CompositionEnvironment
            or AssemblyGroup.LightingColorAtmosphere
            || clause.SliderKeys.Any(IsPhysicalTint);
    }

    private static List<ClauseUnit> ApplyPadSurvivorFloors(
        List<ClauseUnit> clauses,
        IReadOnlyList<ResolvedSliderState> resolved,
        IReadOnlyDictionary<MacroPad, PadSurvivorPlan> padPlans)
    {
        foreach (var plan in padPlans.Values)
        {
            while (CountPadSemanticSurvivors(clauses, plan.Pad) < plan.SemanticFloor)
            {
                var candidate = SelectPadFloorCandidate(resolved, clauses, plan);
                if (candidate is null)
                {
                    break;
                }

                clauses.Add(candidate);
            }

            while (plan.ExplicitFloor > 0 && CountExplicitPadSurvivors(clauses, plan.Pad) < plan.ExplicitFloor)
            {
                var explicitCandidate = SelectPadFloorCandidate(resolved, clauses, plan, requireExplicit: true);
                if (explicitCandidate is null)
                {
                    break;
                }

                clauses.Add(explicitCandidate);
            }
        }

        return clauses;
    }

    private static IReadOnlyDictionary<MacroPad, PadSurvivorPlan> BuildPadSurvivorPlans(IReadOnlyList<ResolvedSliderState> resolved)
    {
        var plans = new Dictionary<MacroPad, PadSurvivorPlan>();

        foreach (var pad in Enum.GetValues<MacroPad>())
        {
            var activeKeys = resolved
                .Where(state => GetMacroPad(state.Candidate.Rule.SliderKey) == pad)
                .Where(IsMeaningfullyActiveState)
                .Select(state => state.Candidate.Rule.SliderKey)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            var activeCount = activeKeys.Length;
            var semanticFloor = pad switch
            {
                MacroPad.ViewConstruction => activeCount >= 2 ? 2 : 0,
                MacroPad.LightAtmosphere => activeCount >= 2 ? 2 : 0,
                MacroPad.MaterialFinish => activeCount >= 2 ? 2 : 0,
                MacroPad.SceneRichness => activeCount >= 2 ? 2 : 0,
                MacroPad.EnergyInstability => activeCount >= 2 ? 2 : 0,
                MacroPad.Tone => activeCount >= 2 ? 2 : activeCount >= 1 ? 1 : 0,
                _ => 0,
            };

            var explicitFloor = pad switch
            {
                MacroPad.Tone => activeCount >= 2 ? 2 : activeCount >= 1 ? 1 : 0,
                _ => 0,
            };

            plans[pad] = new PadSurvivorPlan(pad, activeCount, semanticFloor, explicitFloor);
        }

        return plans;
    }

    private static bool IsMeaningfullyActiveState(ResolvedSliderState state)
    {
        return state.Candidate.Band != 0
            && !string.IsNullOrWhiteSpace(state.Candidate.Phrase)
            && (state.FinalWeight >= 35 || state.Candidate.Band is 1 or 5);
    }

    private static ClauseUnit? SelectPadFloorCandidate(
        IReadOnlyList<ResolvedSliderState> resolved,
        IReadOnlyList<ClauseUnit> clauses,
        PadSurvivorPlan plan,
        bool requireExplicit = false)
    {
        var existingLanes = GetPresentSemanticLanes(clauses, plan.Pad);

        return resolved
            .Where(state => GetMacroPad(state.Candidate.Rule.SliderKey) == plan.Pad)
            .Where(IsMeaningfullyActiveState)
            .Where(state => !existingLanes.Contains(GetSemanticLane(state.Candidate.Rule.SliderKey)))
            .OrderByDescending(state => state.Mode == EmissionMode.Direct ? 2 : state.Mode == EmissionMode.Fuse ? 1 : 0)
            .ThenByDescending(state => state.FinalWeight)
            .Select(state => CreatePadFloorClause(state, requireExplicit))
            .FirstOrDefault(clause => clause is not null);
    }

    private static ClauseUnit? CreatePadFloorClause(ResolvedSliderState state, bool requireExplicit)
    {
        var clause = CreateClauseUnit(state);
        var pad = GetMacroPad(state.Candidate.Rule.SliderKey);

        if (pad == MacroPad.Tone || requireExplicit)
        {
            return clause with
            {
                Group = AssemblyGroup.MoodSymbolicTone,
                ClauseClass = ClauseClass.Modifier,
                Score = clause.Score + 4,
            };
        }

        return clause with { Score = clause.Score + 3 };
    }

    private static int CountPadSemanticSurvivors(IReadOnlyList<ClauseUnit> clauses, MacroPad pad)
    {
        return GetPresentSemanticLanes(clauses, pad).Count;
    }

    private static HashSet<string> GetPresentSemanticLanes(IReadOnlyList<ClauseUnit> clauses, MacroPad pad)
    {
        return clauses
            .SelectMany(clause => clause.SliderKeys)
            .Where(key => GetMacroPad(key) == pad)
            .Select(GetSemanticLane)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static int CountExplicitPadSurvivors(IReadOnlyList<ClauseUnit> clauses, MacroPad pad)
    {
        return clauses.Count(clause =>
            GetMacroPad(clause) == pad
            && (pad != MacroPad.Tone || clause.Group == AssemblyGroup.MoodSymbolicTone));
    }

    private static int CountExplicitToneSurvivors(IReadOnlyList<ClauseUnit> clauses)
    {
        return CountExplicitPadSurvivors(clauses, MacroPad.Tone);
    }

    private static bool WouldDropPadBelowSemanticFloor(ClauseUnit removable, IReadOnlyList<ClauseUnit> clauses)
    {
        var pad = GetMacroPad(removable);
        if (pad == MacroPad.None)
        {
            return false;
        }

        var lanes = GetPresentSemanticLanes(clauses, pad);
        var removableLanes = removable.SliderKeys
            .Where(key => GetMacroPad(key) == pad)
            .Select(GetSemanticLane)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var lane in removableLanes)
        {
            lanes.Remove(lane);
        }

        return lanes.Count < GetCurrentPadSemanticFloor(clauses, pad);
    }

    private static int GetCurrentPadSemanticFloor(IReadOnlyList<ClauseUnit> clauses, MacroPad pad)
    {
        var active = CountPadSemanticSurvivors(clauses, pad);
        return pad switch
        {
            MacroPad.Tone => active >= 2 ? 1 : 0,
            MacroPad.None => 0,
            _ => active >= 2 ? 2 : 0,
        };
    }

    private static bool WouldDropPadBelowPlannedFloor(
        ClauseUnit removable,
        IReadOnlyList<ClauseUnit> clauses,
        IReadOnlyDictionary<MacroPad, PadSurvivorPlan> padPlans)
    {
        var pad = GetMacroPad(removable);
        if (pad == MacroPad.None || !padPlans.TryGetValue(pad, out var plan) || plan.SemanticFloor == 0)
        {
            return false;
        }

        var lanes = GetPresentSemanticLanes(clauses, pad);
        var removableLanes = removable.SliderKeys
            .Where(key => GetMacroPad(key) == pad)
            .Select(GetSemanticLane)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var lane in removableLanes)
        {
            if (!clauses.Where(clause => !ReferenceEquals(clause, removable))
                .Any(clause => clause.SliderKeys.Any(key => GetMacroPad(key) == pad && string.Equals(GetSemanticLane(key), lane, StringComparison.OrdinalIgnoreCase))))
            {
                lanes.Remove(lane);
            }
        }

        return lanes.Count < plan.SemanticFloor;
    }

    private static int GetCapRetentionScore(
        ClauseUnit clause,
        IReadOnlyList<ClauseUnit> groupClauses,
        IReadOnlyDictionary<MacroPad, PadSurvivorPlan> padPlans)
    {
        return GetRetentionScore(clause)
            + (WouldDropPadBelowPlannedFloor(clause, groupClauses, padPlans) ? 100 : 0)
            + (GetMacroPad(clause) == MacroPad.Tone && clause.Group == AssemblyGroup.MoodSymbolicTone ? 12 : 0);
    }

    private static bool IsPreserveFirstSlider(string sliderKey)
    {
        return sliderKey switch
        {
            SliderLanguageCatalog.Framing
            or SliderLanguageCatalog.CameraDistance
            or SliderLanguageCatalog.CameraAngle
            or SliderLanguageCatalog.LightingIntensity
            or SliderLanguageCatalog.Temperature
            or SliderLanguageCatalog.Saturation
            or SliderLanguageCatalog.Contrast
            or SliderLanguageCatalog.TextureDepth
            or SliderLanguageCatalog.DetailDensity
            or SliderLanguageCatalog.ImageCleanliness
            or SliderLanguageCatalog.SurfaceAge
            or SliderLanguageCatalog.BackgroundComplexity
            or SliderLanguageCatalog.AtmosphericDepth
            or SliderLanguageCatalog.NarrativeDensity
            or SliderLanguageCatalog.MotionEnergy
            or SliderLanguageCatalog.Tension
            or SliderLanguageCatalog.Awe
            or SliderLanguageCatalog.Symbolism => true,
            _ => false,
        };
    }

    private static bool IsDistinctPadCompanion(string firstKey, string secondKey)
    {
        var firstPad = GetMacroPad(firstKey);
        var secondPad = GetMacroPad(secondKey);
        return firstPad != MacroPad.None
            && firstPad == secondPad
            && !string.Equals(GetSemanticLane(firstKey), GetSemanticLane(secondKey), StringComparison.OrdinalIgnoreCase);
    }

    private static MacroPad GetMacroPad(ClauseUnit clause)
    {
        return clause.SliderKeys.Select(GetMacroPad).FirstOrDefault(pad => pad != MacroPad.None);
    }

    private static MacroPad GetMacroPad(string sliderKey) => sliderKey switch
    {
        SliderLanguageCatalog.Framing or SliderLanguageCatalog.CameraDistance or SliderLanguageCatalog.CameraAngle => MacroPad.ViewConstruction,
        LightingPresetKey or SliderLanguageCatalog.Temperature or SliderLanguageCatalog.LightingIntensity or SliderLanguageCatalog.Saturation or SliderLanguageCatalog.Contrast => MacroPad.LightAtmosphere,
        SliderLanguageCatalog.TextureDepth or SliderLanguageCatalog.DetailDensity or SliderLanguageCatalog.ImageCleanliness or SliderLanguageCatalog.SurfaceAge => MacroPad.MaterialFinish,
        SliderLanguageCatalog.BackgroundComplexity or SliderLanguageCatalog.AtmosphericDepth or SliderLanguageCatalog.NarrativeDensity => MacroPad.SceneRichness,
        SliderLanguageCatalog.MotionEnergy or SliderLanguageCatalog.Chaos or SliderLanguageCatalog.Tension => MacroPad.EnergyInstability,
        SliderLanguageCatalog.Whimsy or SliderLanguageCatalog.Awe or SliderLanguageCatalog.Symbolism => MacroPad.Tone,
        _ => MacroPad.None,
    };

    private static string GetSemanticLane(string sliderKey) => sliderKey switch
    {
        LightingPresetKey => "lighting_preset",
        _ => sliderKey,
    };

    [Conditional("DEBUG")]
    private static void LogPadSurvivorship(
        IReadOnlyDictionary<MacroPad, PadTrace> trace,
        IReadOnlyDictionary<MacroPad, PadSurvivorPlan> plans,
        IReadOnlyList<ClauseUnit> finalClauses)
    {
        foreach (var pad in Enum.GetValues<MacroPad>().Where(static pad => pad != MacroPad.None))
        {
            if (!plans.TryGetValue(pad, out var plan))
            {
                continue;
            }

            var finalSurvivors = CountPadSemanticSurvivors(finalClauses, pad);
            var explicitFinal = CountExplicitPadSurvivors(finalClauses, pad);
            var lossStage = "ok";

            if (trace.TryGetValue(pad, out var padTrace)
                && finalSurvivors < plan.SemanticFloor)
            {
                lossStage = padTrace.FirstLoss?.ToString().ToLowerInvariant() ?? "recovery unavailable";
            }

            Debug.WriteLine($"{pad}: active={plan.ActiveCount} survivors={finalSurvivors} explicit={explicitFinal} loss={lossStage}");
        }
    }

    private static Dictionary<MacroPad, PadTrace> InitializePadTrace(
        IReadOnlyList<ResolvedSliderState> resolved,
        IReadOnlyDictionary<MacroPad, PadSurvivorPlan> plans)
    {
        var trace = new Dictionary<MacroPad, PadTrace>();

        foreach (var pad in Enum.GetValues<MacroPad>().Where(static pad => pad != MacroPad.None))
        {
            var resolvedCount = resolved
                .Where(state => state.Mode != EmissionMode.Silent && GetMacroPad(state.Candidate.Rule.SliderKey) == pad)
                .Select(state => GetSemanticLane(state.Candidate.Rule.SliderKey))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();

            var floor = plans.TryGetValue(pad, out var plan) ? plan.SemanticFloor : 0;
            trace[pad] = new PadTrace(resolvedCount, floor, resolvedCount < floor ? PadLossStage.Resolution : null);
        }

        return trace;
    }

    private static void CapturePadTraceStage(
        IDictionary<MacroPad, PadTrace> trace,
        IReadOnlyList<ClauseUnit> clauses,
        PadLossStage stage)
    {
        foreach (var key in trace.Keys.ToArray())
        {
            var count = CountPadSemanticSurvivors(clauses, key);
            var current = trace[key];
            trace[key] = current with
            {
                FirstLoss = current.FirstLoss ?? (count < current.Floor ? stage : null),
            };
        }
    }

    private static bool HasCoreStructuralCoverage(IReadOnlyList<ClauseUnit> clauses)
    {
        return clauses.Any(clause => clause.Group == AssemblyGroup.RenderingIdentity)
            && clauses.Any(clause => clause.Group == AssemblyGroup.ViewConstruction)
            && clauses.Any(clause => clause.Group == AssemblyGroup.LightingColorAtmosphere);
    }

    private static bool NeedsRichnessRecovery(
        IReadOnlyList<ClauseUnit> clauses,
        IReadOnlyList<ClauseUnit> richnessSnapshot,
        RichnessDemandProfile richnessDemand)
    {
        return HasCoreStructuralCoverage(clauses)
            && richnessSnapshot.Any(IsRecoverableRichnessClause)
            && CountMeaningfulRichnessClauses(clauses) < GetTargetRichnessCount(richnessDemand);
    }

    private static bool WouldReadThin(IReadOnlyList<ClauseUnit> clauses, RichnessDemandProfile richnessDemand)
    {
        return HasCoreStructuralCoverage(clauses)
            && clauses.Count <= 7
            && CountMeaningfulRichnessClauses(clauses) < GetTargetRichnessCount(richnessDemand);
    }

    private static bool IsProtectedRichnessClause(
        ClauseUnit clause,
        IReadOnlyList<ClauseUnit> clauses,
        IReadOnlyList<ClauseUnit> richnessSnapshot)
    {
        if (!IsRichnessBearingClause(clause))
        {
            return false;
        }

        var category = GetRichnessCategory(clause);
        if (category == RichnessCategory.None)
        {
            return false;
        }

        return clauses.Count(candidate => GetRichnessCategory(candidate) == category && IsMeaningfulRichnessClause(candidate)) <= 1
            && richnessSnapshot.Any(candidate => GetRichnessCategory(candidate) == category && IsRecoverableRichnessClause(candidate));
    }

    private static ClauseUnit? SelectRecoveryClause(
        IReadOnlyList<ClauseUnit> richnessSnapshot,
        IReadOnlyList<ClauseUnit> currentClauses,
        RichnessCategory category)
    {
        return SelectRecoveryClause(
            richnessSnapshot,
            currentClauses,
            new RichnessDemandProfile(
                WantsMaterialFinish: category == RichnessCategory.MaterialFinish,
                WantsSpatialDepth: category == RichnessCategory.SpatialDepth,
                WantsMotionEnergy: category == RichnessCategory.MotionEnergy),
            preferSupplementalCategory: false,
            requiredCategory: category);
    }

    private static ClauseUnit? SelectRecoveryClause(
        IReadOnlyList<ClauseUnit> richnessSnapshot,
        IReadOnlyList<ClauseUnit> currentClauses,
        RichnessDemandProfile richnessDemand,
        bool preferSupplementalCategory)
    {
        return SelectRecoveryClause(richnessSnapshot, currentClauses, richnessDemand, preferSupplementalCategory, RichnessCategory.None);
    }

    private static ClauseUnit? SelectRecoveryClause(
        IReadOnlyList<ClauseUnit> richnessSnapshot,
        IReadOnlyList<ClauseUnit> currentClauses,
        RichnessDemandProfile richnessDemand,
        bool preferSupplementalCategory,
        RichnessCategory requiredCategory)
    {
        return richnessSnapshot
            .Where(IsRecoverableRichnessClause)
            .Where(clause => requiredCategory == RichnessCategory.None || GetRichnessCategory(clause) == requiredCategory)
            .Where(clause => !currentClauses.Any(existing => string.Equals(existing.Text, clause.Text, StringComparison.OrdinalIgnoreCase)))
            .Where(clause => requiredCategory != RichnessCategory.None || IsEligibleForDemandRecovery(clause, currentClauses, richnessDemand))
            .OrderByDescending(clause => preferSupplementalCategory && IsSupplementalRichnessCategory(clause) ? 1 : 0)
            .ThenByDescending(clause => IsDemandedButMissingCategory(clause, currentClauses, richnessDemand) ? 1 : 0)
            .ThenByDescending(GetRecoveryScore)
            .FirstOrDefault();
    }

    private static int CountMeaningfulRichnessClauses(IReadOnlyList<ClauseUnit> clauses)
    {
        return clauses.Count(IsMeaningfulRichnessClause);
    }

    private static bool HasRichnessCategory(IReadOnlyList<ClauseUnit> clauses, RichnessCategory category)
    {
        return clauses.Any(clause => GetRichnessCategory(clause) == category && IsMeaningfulRichnessClause(clause));
    }

    private static bool IsMeaningfulRichnessClause(ClauseUnit clause)
    {
        return IsRichnessBearingClause(clause)
            && (clause.Score >= 60 || clause.IsOwner || clause.IsFused);
    }

    private static bool IsRecoverableRichnessClause(ClauseUnit clause)
    {
        return IsRichnessBearingClause(clause)
            && clause.Score >= 58
            && !IsAbstractResidue(clause);
    }

    private static bool IsRichnessBearingClause(ClauseUnit clause)
    {
        return GetRichnessCategory(clause) != RichnessCategory.None;
    }

    private static RichnessCategory GetRichnessCategory(ClauseUnit clause)
    {
        if (clause.SliderKeys.Any(key =>
                string.Equals(key, SliderLanguageCatalog.TextureDepth, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.DetailDensity, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.ImageCleanliness, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.SurfaceAge, StringComparison.OrdinalIgnoreCase))
            || clause.Text.Contains("texture", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("detail", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("finish", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("patina", StringComparison.OrdinalIgnoreCase))
        {
            return RichnessCategory.MaterialFinish;
        }

        if (clause.SliderKeys.Any(key =>
                string.Equals(key, SliderLanguageCatalog.BackgroundComplexity, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.AtmosphericDepth, StringComparison.OrdinalIgnoreCase))
            || clause.Text.Contains("background", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("atmosphere", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("depth", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("environment", StringComparison.OrdinalIgnoreCase))
        {
            return RichnessCategory.SpatialDepth;
        }

        if (clause.SliderKeys.Any(key =>
                string.Equals(key, SliderLanguageCatalog.MotionEnergy, StringComparison.OrdinalIgnoreCase)
                || string.Equals(key, SliderLanguageCatalog.Chaos, StringComparison.OrdinalIgnoreCase))
            || clause.Text.Contains("energy", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("motion", StringComparison.OrdinalIgnoreCase))
        {
            return RichnessCategory.MotionEnergy;
        }

        return RichnessCategory.None;
    }

    private static bool IsSupplementalRichnessCategory(ClauseUnit clause)
    {
        var category = GetRichnessCategory(clause);
        return category is RichnessCategory.MaterialFinish or RichnessCategory.SpatialDepth;
    }

    private static int GetRecoveryScore(ClauseUnit clause)
    {
        return GetRetentionScore(clause)
            + (GetRichnessCategory(clause) == RichnessCategory.MaterialFinish ? 6 : 0)
            + (GetRichnessCategory(clause) == RichnessCategory.SpatialDepth ? 4 : 0);
    }

    private static RichnessDemandProfile BuildRichnessDemandProfile(IReadOnlyList<ResolvedSliderState> survivors)
    {
        return new RichnessDemandProfile(
            WantsMaterialFinish: survivors.Any(state =>
                state.Mode is EmissionMode.Direct or EmissionMode.Fuse
                && state.Candidate.Rule.SliderKey is SliderLanguageCatalog.TextureDepth or SliderLanguageCatalog.DetailDensity or SliderLanguageCatalog.ImageCleanliness or SliderLanguageCatalog.SurfaceAge
                && state.FinalWeight >= 56),
            WantsSpatialDepth: survivors.Any(state =>
                state.Mode is EmissionMode.Direct or EmissionMode.Fuse
                && state.Candidate.Rule.SliderKey is SliderLanguageCatalog.BackgroundComplexity or SliderLanguageCatalog.AtmosphericDepth or SliderLanguageCatalog.NarrativeDensity
                && state.FinalWeight >= 56),
            WantsMotionEnergy: survivors.Any(state =>
                state.Mode is EmissionMode.Direct or EmissionMode.Fuse
                && state.Candidate.Rule.SliderKey is SliderLanguageCatalog.MotionEnergy or SliderLanguageCatalog.Chaos or SliderLanguageCatalog.Tension
                && state.FinalWeight >= 60));
    }

    private static int GetTargetRichnessCount(RichnessDemandProfile richnessDemand)
    {
        var target = 1;
        if (richnessDemand.WantsMaterialFinish)
        {
            target++;
        }

        if (richnessDemand.WantsSpatialDepth)
        {
            target++;
        }

        if (richnessDemand.WantsMotionEnergy)
        {
            target++;
        }

        return Math.Min(target, 4);
    }

    private static int GetTotalClauseCap(RichnessDemandProfile richnessDemand)
    {
        var cap = 10;
        if (richnessDemand.WantsMaterialFinish && richnessDemand.WantsSpatialDepth)
        {
            cap++;
        }

        if (richnessDemand.WantsMotionEnergy)
        {
            cap++;
        }

        return Math.Min(cap, 12);
    }

    private static bool IsEligibleForDemandRecovery(
        ClauseUnit clause,
        IReadOnlyList<ClauseUnit> currentClauses,
        RichnessDemandProfile richnessDemand)
    {
        var category = GetRichnessCategory(clause);
        return category == RichnessCategory.None || IsDemandedButMissingCategory(clause, currentClauses, richnessDemand);
    }

    private static bool IsDemandedButMissingCategory(
        ClauseUnit clause,
        IReadOnlyList<ClauseUnit> currentClauses,
        RichnessDemandProfile richnessDemand)
    {
        var category = GetRichnessCategory(clause);
        return category switch
        {
            RichnessCategory.MaterialFinish => richnessDemand.WantsMaterialFinish && !HasRichnessCategory(currentClauses, RichnessCategory.MaterialFinish),
            RichnessCategory.SpatialDepth => richnessDemand.WantsSpatialDepth && !HasRichnessCategory(currentClauses, RichnessCategory.SpatialDepth),
            RichnessCategory.MotionEnergy => richnessDemand.WantsMotionEnergy && !HasRichnessCategory(currentClauses, RichnessCategory.MotionEnergy),
            _ => CountMeaningfulRichnessClauses(currentClauses) == 0,
        };
    }

    private static int GetSuppressionPriority(ClauseUnit clause)
    {
        if (IsAbstractResidue(clause) && clause.Score <= 64)
        {
            return 0;
        }

        if (clause.ClauseClass == ClauseClass.Modifier && clause.Score <= 66)
        {
            return 1;
        }

        if (HasSemanticEchoPartner(clause))
        {
            return 2;
        }

        if (!IsRichnessBearingClause(clause) && clause.Score <= 68)
        {
            return 3;
        }

        return 4;
    }

    private static bool HasSemanticEchoPartner(ClauseUnit clause)
    {
        return clause.SliderKeys.Length > 1
            || clause.Text.Contains("undertone", StringComparison.OrdinalIgnoreCase)
            || clause.Text.Contains("presence", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAbstractTint(string sliderKey)
    {
        return string.Equals(sliderKey, SliderLanguageCatalog.Whimsy, StringComparison.OrdinalIgnoreCase)
            || string.Equals(sliderKey, SliderLanguageCatalog.Symbolism, StringComparison.OrdinalIgnoreCase)
            || string.Equals(sliderKey, SliderLanguageCatalog.Awe, StringComparison.OrdinalIgnoreCase)
            || string.Equals(sliderKey, SliderLanguageCatalog.Tension, StringComparison.OrdinalIgnoreCase)
            || string.Equals(sliderKey, SliderLanguageCatalog.NarrativeDensity, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPhysicalTint(string sliderKey)
    {
        return string.Equals(sliderKey, SliderLanguageCatalog.SurfaceAge, StringComparison.OrdinalIgnoreCase)
            || string.Equals(sliderKey, SliderLanguageCatalog.ImageCleanliness, StringComparison.OrdinalIgnoreCase)
            || string.Equals(sliderKey, SliderLanguageCatalog.Saturation, StringComparison.OrdinalIgnoreCase)
            || string.Equals(sliderKey, SliderLanguageCatalog.Contrast, StringComparison.OrdinalIgnoreCase)
            || string.Equals(sliderKey, SliderLanguageCatalog.Chaos, StringComparison.OrdinalIgnoreCase);
    }

    private static int GetAttachedAbstractTintCount(ClauseUnit clause)
    {
        return clause.SliderKeys.Count(IsAbstractTint);
    }

    private static int GetAttachedPhysicalTintCount(ClauseUnit clause)
    {
        return clause.SliderKeys.Count(IsPhysicalTint);
    }

    private static int GetTailOrder(ClauseUnit clause)
    {
        if (clause.SliderKeys.Any(key =>
            string.Equals(key, SliderLanguageCatalog.ImageCleanliness, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.SurfaceAge, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.TextureDepth, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.DetailDensity, StringComparison.OrdinalIgnoreCase)))
        {
            return 1;
        }

        if (clause.SliderKeys.Any(key =>
            string.Equals(key, SliderLanguageCatalog.Saturation, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.Contrast, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.Temperature, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.LightingIntensity, StringComparison.OrdinalIgnoreCase)))
        {
            return 2;
        }

        if (clause.SliderKeys.Any(key =>
            string.Equals(key, SliderLanguageCatalog.Tension, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.Chaos, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.MotionEnergy, StringComparison.OrdinalIgnoreCase)))
        {
            return 3;
        }

        if (clause.SliderKeys.Any(key =>
            string.Equals(key, SliderLanguageCatalog.Whimsy, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.Symbolism, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, SliderLanguageCatalog.Awe, StringComparison.OrdinalIgnoreCase)))
        {
            return 4;
        }

        if (clause.SliderKeys.Any(key => string.Equals(key, SliderLanguageCatalog.NarrativeDensity, StringComparison.OrdinalIgnoreCase)))
        {
            return 5;
        }

        return 6;
    }

    private static string AppendCompactQualifier(string text, string qualifier)
    {
        if (string.IsNullOrWhiteSpace(qualifier) || text.Contains(qualifier, StringComparison.OrdinalIgnoreCase))
        {
            return text;
        }

        return CountOccurrences(text, " with ") >= 1
            ? $"{text}, {qualifier}"
            : $"{text} with {qualifier}";
    }

    private static string SimplifyTailAttachment(string text)
    {
        if (CountOccurrences(text, ", ") > 1)
        {
            return ReplaceLast(text, ", ", " with ");
        }

        if (CountOccurrences(text, " with ") > 1)
        {
            return ReplaceLast(text, " with ", ", ");
        }

        return text;
    }

    private static string CleanupQualifierRepeats(string text)
    {
        return text
            .Replace("slightly slightly", "slightly", StringComparison.OrdinalIgnoreCase)
            .Replace("controlled controlled", "controlled", StringComparison.OrdinalIgnoreCase)
            .Replace("finish finish", "finish", StringComparison.OrdinalIgnoreCase)
            .Replace("color contrast color", "color contrast", StringComparison.OrdinalIgnoreCase)
            .Replace("symbolic undertone undertone", "symbolic undertone", StringComparison.OrdinalIgnoreCase);
    }

    private static void CleanupSemanticEcho(List<ClauseUnit> clauses, string firstKey, string secondKey)
    {
        var first = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(firstKey, StringComparer.OrdinalIgnoreCase));
        var second = clauses.FirstOrDefault(clause => clause.SliderKeys.Contains(secondKey, StringComparer.OrdinalIgnoreCase));

        if (first is null || second is null || ReferenceEquals(first, second))
        {
            return;
        }

        if (!HasStrongSemanticEcho(first, second))
        {
            return;
        }

        if (CanCompressEcho(first, second, out var merged))
        {
            if (!WouldDropPadBelowSemanticFloor(first, clauses)
                && !WouldDropPadBelowSemanticFloor(second, clauses))
            {
                clauses.Remove(first);
                clauses.Remove(second);
                clauses.Add(merged);
            }
            return;
        }

        if (GetEchoKeepScore(first) >= GetEchoKeepScore(second))
        {
            if (!WouldDropPadBelowSemanticFloor(second, clauses))
            {
                clauses.Remove(second);
            }
        }
        else
        {
            if (!WouldDropPadBelowSemanticFloor(first, clauses))
            {
                clauses.Remove(first);
            }
        }
    }

    private static bool HasStrongSemanticEcho(ClauseUnit first, ClauseUnit second)
    {
        if (first.Text.Contains("tactile", StringComparison.OrdinalIgnoreCase) && second.Text.Contains("detail", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if ((first.Text.Contains("lighting", StringComparison.OrdinalIgnoreCase) || first.Text.Contains("illumination", StringComparison.OrdinalIgnoreCase))
            && second.Text.Contains("contrast", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (first.Text.Contains("wonder", StringComparison.OrdinalIgnoreCase) && second.Text.Contains("atmosphere", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (first.Text.Contains("symbolic", StringComparison.OrdinalIgnoreCase) && second.Text.Contains("story", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (first.Text.Contains("finish", StringComparison.OrdinalIgnoreCase)
            && (second.Text.Contains("polished", StringComparison.OrdinalIgnoreCase) || second.Text.Contains("time-worn", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (first.Text.Contains("tension", StringComparison.OrdinalIgnoreCase) && second.Text.Contains("chaotic", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private static bool CanCompressEcho(ClauseUnit first, ClauseUnit second, out ClauseUnit merged)
    {
        merged = null!;

        if (first.Score > 70 || second.Score > 70)
        {
            return false;
        }

        var mergedText = first.Text.Contains("finish", StringComparison.OrdinalIgnoreCase) && second.Text.Contains("time-worn", StringComparison.OrdinalIgnoreCase)
            ? "clean, slightly time-worn finish"
            : first.Text.Contains("contrast", StringComparison.OrdinalIgnoreCase) || second.Text.Contains("contrast", StringComparison.OrdinalIgnoreCase)
                ? "rich but controlled color contrast"
                : first.Text.Contains("symbolic", StringComparison.OrdinalIgnoreCase) || second.Text.Contains("symbolic", StringComparison.OrdinalIgnoreCase)
                    ? "light symbolic story residue"
                    : first.Text.Contains("tension", StringComparison.OrdinalIgnoreCase) || second.Text.Contains("chaotic", StringComparison.OrdinalIgnoreCase)
                        ? "tense, slightly chaotic energy"
                        : string.Empty;

        if (string.IsNullOrWhiteSpace(mergedText))
        {
            return false;
        }

        merged = new ClauseUnit(
            Text: mergedText,
            Group: GetTailOrder(first) <= GetTailOrder(second) ? first.Group : second.Group,
            ClauseClass: ClauseClass.Modifier,
            Score: Math.Max(first.Score, second.Score) + 1,
            Mode: first.Mode,
            IsOwner: false,
            IsFused: true,
            SliderKeys: [.. first.SliderKeys, .. second.SliderKeys]);
        return true;
    }

    private static int GetEchoKeepScore(ClauseUnit clause)
    {
        return clause.Score
            + (clause.IsOwner ? 8 : 0)
            + (clause.IsFused ? 4 : 0)
            + (IsPhysicalResidue(clause) ? 6 : 0)
            - (IsAbstractResidue(clause) ? 2 : 0);
    }

    private static int CountOccurrences(string text, string value)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(value))
        {
            return 0;
        }

        var count = 0;
        var index = 0;

        while ((index = text.IndexOf(value, index, StringComparison.OrdinalIgnoreCase)) >= 0)
        {
            count++;
            index += value.Length;
        }

        return count;
    }

    private static string ReplaceLast(string text, string oldValue, string newValue)
    {
        var index = text.LastIndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
        if (index < 0)
        {
            return text;
        }

        return string.Concat(text.AsSpan(0, index), newValue, text.AsSpan(index + oldValue.Length));
    }

    private static AssemblyGroup GetAssemblyGroup(string sliderKey) => sliderKey switch
    {
        SliderLanguageCatalog.Stylization or SliderLanguageCatalog.Realism or SliderLanguageCatalog.TextureDepth or SliderLanguageCatalog.DetailDensity or SliderLanguageCatalog.ImageCleanliness or SliderLanguageCatalog.SurfaceAge
            => AssemblyGroup.RenderingIdentity,
        SliderLanguageCatalog.Framing or SliderLanguageCatalog.CameraDistance or SliderLanguageCatalog.CameraAngle or SliderLanguageCatalog.FocusDepth
            => AssemblyGroup.ViewConstruction,
        SliderLanguageCatalog.BackgroundComplexity or SliderLanguageCatalog.AtmosphericDepth or SliderLanguageCatalog.NarrativeDensity
            => AssemblyGroup.CompositionEnvironment,
        SliderLanguageCatalog.MotionEnergy or SliderLanguageCatalog.Chaos or SliderLanguageCatalog.Tension
            => AssemblyGroup.MotionInstability,
        LightingPresetKey or SliderLanguageCatalog.Temperature or SliderLanguageCatalog.LightingIntensity or SliderLanguageCatalog.Saturation or SliderLanguageCatalog.Contrast
            => AssemblyGroup.LightingColorAtmosphere,
        SliderLanguageCatalog.Whimsy or SliderLanguageCatalog.Awe or SliderLanguageCatalog.Symbolism
            => AssemblyGroup.MoodSymbolicTone,
        _ => AssemblyGroup.TrailingRefiners,
    };

    private static int GetAssemblyGroupOrder(string sliderKey)
    {
        return Array.IndexOf(AssemblyGroupSequence, GetAssemblyGroup(sliderKey));
    }

    private static int GetAssemblySliderOrder(string sliderKey) => sliderKey switch
    {
        SliderLanguageCatalog.Stylization => 1,
        SliderLanguageCatalog.Realism => 2,
        SliderLanguageCatalog.TextureDepth => 3,
        SliderLanguageCatalog.DetailDensity => 4,
        SliderLanguageCatalog.ImageCleanliness => 5,
        SliderLanguageCatalog.SurfaceAge => 6,
        SliderLanguageCatalog.Framing => 7,
        SliderLanguageCatalog.CameraDistance => 8,
        SliderLanguageCatalog.CameraAngle => 9,
        SliderLanguageCatalog.FocusDepth => 10,
        SliderLanguageCatalog.BackgroundComplexity => 11,
        SliderLanguageCatalog.AtmosphericDepth => 12,
        SliderLanguageCatalog.NarrativeDensity => 13,
        SliderLanguageCatalog.MotionEnergy => 14,
        SliderLanguageCatalog.Chaos => 15,
        SliderLanguageCatalog.Tension => 16,
        LightingPresetKey => 17,
        SliderLanguageCatalog.Temperature => 18,
        SliderLanguageCatalog.LightingIntensity => 19,
        SliderLanguageCatalog.Saturation => 20,
        SliderLanguageCatalog.Contrast => 21,
        SliderLanguageCatalog.Whimsy => 22,
        SliderLanguageCatalog.Awe => 23,
        SliderLanguageCatalog.Symbolism => 24,
        _ => 100,
    };

    private static ClauseClass GetBaseClauseClass(string sliderKey) => sliderKey switch
    {
        SliderLanguageCatalog.TextureDepth or SliderLanguageCatalog.DetailDensity or SliderLanguageCatalog.FocusDepth or SliderLanguageCatalog.BackgroundComplexity or SliderLanguageCatalog.AtmosphericDepth or SliderLanguageCatalog.NarrativeDensity
            => ClauseClass.Support,
        SliderLanguageCatalog.Whimsy or SliderLanguageCatalog.Symbolism or SliderLanguageCatalog.Saturation or SliderLanguageCatalog.Contrast or SliderLanguageCatalog.SurfaceAge or SliderLanguageCatalog.ImageCleanliness or SliderLanguageCatalog.Awe
            => ClauseClass.Modifier,
        _ => ClauseClass.Primary,
    };

    private static string ResolvePhrase(string sliderKey, PromptConfiguration configuration)
    {
        if (string.Equals(sliderKey, LightingPresetKey, StringComparison.Ordinal))
        {
            return LowerLightingPreset(configuration.Lighting);
        }

        return SliderLanguageCatalog.ResolvePhrase(sliderKey, GetSliderValue(sliderKey, configuration), configuration);
    }

    private static int ResolveBand(string sliderKey, PromptConfiguration configuration)
    {
        if (string.Equals(sliderKey, LightingPresetKey, StringComparison.Ordinal))
        {
            return string.IsNullOrWhiteSpace(LowerLightingPreset(configuration.Lighting)) ? 3 : 5;
        }

        var value = GetSliderValue(sliderKey, configuration);
        if (value <= 20) return 1;
        if (value <= 40) return 2;
        if (value <= 60) return 3;
        if (value <= 80) return 4;
        return 5;
    }

    private static int GetSliderValue(string sliderKey, PromptConfiguration configuration) => sliderKey switch
    {
        SliderLanguageCatalog.Stylization => configuration.Stylization,
        SliderLanguageCatalog.Realism => configuration.Realism,
        SliderLanguageCatalog.TextureDepth => configuration.TextureDepth,
        SliderLanguageCatalog.NarrativeDensity => configuration.NarrativeDensity,
        SliderLanguageCatalog.Symbolism => configuration.Symbolism,
        SliderLanguageCatalog.SurfaceAge => configuration.SurfaceAge,
        SliderLanguageCatalog.Framing => configuration.Framing,
        SliderLanguageCatalog.CameraDistance => configuration.CameraDistance,
        SliderLanguageCatalog.CameraAngle => configuration.CameraAngle,
        SliderLanguageCatalog.BackgroundComplexity => configuration.BackgroundComplexity,
        SliderLanguageCatalog.MotionEnergy => configuration.MotionEnergy,
        SliderLanguageCatalog.AtmosphericDepth => configuration.AtmosphericDepth,
        SliderLanguageCatalog.Chaos => configuration.Chaos,
        SliderLanguageCatalog.Whimsy => configuration.Whimsy,
        SliderLanguageCatalog.Tension => configuration.Tension,
        SliderLanguageCatalog.Awe => configuration.Awe,
        SliderLanguageCatalog.Temperature => configuration.Temperature,
        SliderLanguageCatalog.LightingIntensity => configuration.LightingIntensity,
        SliderLanguageCatalog.Saturation => configuration.Saturation,
        SliderLanguageCatalog.Contrast => configuration.Contrast,
        SliderLanguageCatalog.FocusDepth => configuration.FocusDepth,
        SliderLanguageCatalog.ImageCleanliness => configuration.ImageCleanliness,
        SliderLanguageCatalog.DetailDensity => configuration.DetailDensity,
        _ => 50,
    };

    private static int GetCrowdingPenalty(int sameGroupDirectCount, int globalDirectCount)
    {
        var penalty = 0;

        if (sameGroupDirectCount >= 2)
        {
            penalty -= 25;
        }
        else if (sameGroupDirectCount >= 1)
        {
            penalty -= 12;
        }

        if (globalDirectCount >= 8)
        {
            penalty -= 20;
        }
        else if (globalDirectCount >= 6)
        {
            penalty -= 10;
        }

        return penalty;
    }

    private static int GetSuppressionPenalty(SuppressionLevel suppression, int sameGroupDirectCount)
    {
        if (sameGroupDirectCount == 0)
        {
            return 0;
        }

        var perSlotPenalty = suppression switch
        {
            SuppressionLevel.Low => -1,
            SuppressionLevel.Medium => -4,
            SuppressionLevel.MediumHigh => -6,
            SuppressionLevel.High => -8,
            _ => 0,
        };

        return perSlotPenalty * sameGroupDirectCount;
    }

    private static int GetDirectThreshold(SliderClass sliderClass) => sliderClass switch
    {
        SliderClass.Anchor => AnchorDirectThreshold,
        SliderClass.AnchorState => AnchorDirectThreshold,
        SliderClass.AnchorSupport => AnchorDirectThreshold,
        SliderClass.Support => SupportDirectThreshold,
        SliderClass.SupportModifier => SupportDirectThreshold,
        SliderClass.Modifier => ModifierDirectThreshold,
        _ => AnchorDirectThreshold,
    };

    private static string? ResolveOwnerKey(IReadOnlyDictionary<string, SliderCandidate> candidatesByKey, params string[] sliderKeys)
    {
        SliderCandidate? best = null;

        foreach (var sliderKey in sliderKeys)
        {
            var candidate = GetCandidate(candidatesByKey, sliderKey);
            if (candidate is null)
            {
                continue;
            }

            if (best is null || ComparePriority(candidate, best) > 0)
            {
                best = candidate;
            }
        }

        return best?.Rule.SliderKey;
    }

    private static int ComparePriority(SliderCandidate left, SliderCandidate right)
    {
        var classComparison = GetClassRank(left.Rule.Class).CompareTo(GetClassRank(right.Rule.Class));
        if (classComparison != 0)
        {
            return classComparison;
        }

        var weightComparison = left.WeightBeforeConflicts.CompareTo(right.WeightBeforeConflicts);
        if (weightComparison != 0)
        {
            return weightComparison;
        }

        var authorityComparison = GetAuthorityRank(left.Rule.Authority).CompareTo(GetAuthorityRank(right.Rule.Authority));
        if (authorityComparison != 0)
        {
            return authorityComparison;
        }

        var precedenceComparison = right.Precedence.CompareTo(left.Precedence);
        if (precedenceComparison != 0)
        {
            return precedenceComparison;
        }

        return GetStandaloneClutterRisk(right.Rule).CompareTo(GetStandaloneClutterRisk(left.Rule));
    }

    private static int GetClassRank(SliderClass sliderClass) => sliderClass switch
    {
        SliderClass.AnchorState => 6,
        SliderClass.Anchor => 5,
        SliderClass.AnchorSupport => 4,
        SliderClass.SupportModifier => 3,
        SliderClass.Support => 2,
        SliderClass.Modifier => 1,
        _ => 0,
    };

    private static int GetAuthorityRank(AuthorityTier authority) => authority switch
    {
        AuthorityTier.Top => 3,
        AuthorityTier.Mid => 2,
        _ => 1,
    };

    private static int GetStandaloneClutterRisk(SliderRule rule) => rule.Class switch
    {
        SliderClass.Modifier => 3,
        SliderClass.Support => 2,
        SliderClass.SupportModifier => 2,
        SliderClass.AnchorSupport => 1,
        _ => 0,
    };

    private static SliderCandidate? GetCandidate(IReadOnlyDictionary<string, SliderCandidate> candidatesByKey, string sliderKey)
    {
        return candidatesByKey.TryGetValue(sliderKey, out var candidate) ? candidate : null;
    }

    private static bool IsClearlyComplementaryToLightingPreset(string sliderKey, int band, string lightingPreset)
    {
        if (string.IsNullOrWhiteSpace(lightingPreset) || string.Equals(lightingPreset, "None", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return sliderKey switch
        {
            SliderLanguageCatalog.Temperature => band switch
            {
                1 or 2 => lightingPreset is "Moonlit" or "Overcast",
                4 or 5 => lightingPreset is "Golden hour" or "Warm directional light" or "Soft glow" or "Dusk haze",
                _ => true,
            },
            SliderLanguageCatalog.LightingIntensity => band switch
            {
                1 or 2 => lightingPreset is "Moonlit" or "Overcast" or "Dusk haze",
                4 or 5 => lightingPreset is "Golden hour" or "Dramatic studio light" or "Soft glow" or "Volumetric cinematic light",
                _ => true,
            },
            SliderLanguageCatalog.Contrast => band >= 4
                ? lightingPreset is "Dramatic studio light" or "Moonlit" or "Volumetric cinematic light"
                : band <= 2
                    ? lightingPreset is "Soft daylight" or "Overcast" or "Soft glow"
                    : true,
            _ => true,
        };
    }

    private static string JoinPhrases(IReadOnlyList<string> phrases)
    {
        return phrases.Count switch
        {
            0 => string.Empty,
            1 => phrases[0],
            2 => $"{phrases[0]} and {phrases[1]}",
            _ => $"{string.Join(", ", phrases.Take(phrases.Count - 1))}, and {phrases[^1]}",
        };
    }

    private static string LowerLightingPreset(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "None", StringComparison.OrdinalIgnoreCase))
        {
            return string.Empty;
        }

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    private sealed record GroupDefinition(int DirectCap, string FuseLeadIn, string TintLeadIn, string[] Precedence);

    private sealed record SliderRule(
        string DisplayName,
        string SliderKey,
        string Group,
        SliderClass Class,
        AuthorityTier Authority,
        int BaseWeight,
        int MinWeight,
        int MaxWeight,
        bool AllowDirect,
        bool AllowFuse,
        bool AllowTint,
        SuppressionLevel Suppression,
        TriggerBehavior Trigger,
        string RedundancyDomain,
        string OwnerDomain);

    private sealed record SliderCandidate(
        SliderRule Rule,
        int Band,
        int BaseWeight,
        int WeightBeforeConflicts,
        string Phrase,
        int Precedence);

    private sealed record ResolvedSliderState(
        SliderCandidate Candidate,
        int FinalWeight,
        EmissionMode Mode,
        ReasonCode Reason,
        string? OwnerKey);

    private sealed record OwnershipResolution(bool IsOwner, string? OwnerKey, int Penalty, ReasonCode? Reason);

    private sealed record ConflictResolution(int Penalty, ReasonCode? Reason);

    private sealed record ModeAllowance(bool Direct, bool Fuse, bool Tint);

    private enum SliderClass
    {
        Anchor,
        Support,
        Modifier,
        SupportModifier,
        AnchorState,
        AnchorSupport,
    }

    private enum AuthorityTier
    {
        Low,
        Mid,
        Top,
    }

    private enum TriggerBehavior
    {
        DirectExceptNearNeutral,
        DirectExceptNearNeutralOrOff,
        DirectAtOuterExtremesElseFuse,
        DirectAboveNeutralElseFuseTint,
        DirectOnlyUpperBands,
        DirectAtUpperBandsElseFuseTint,
        DirectWhenNoticeableOrAbove,
        DirectWhenActive,
        DirectAtOuterExtremesElseFuseTint,
        DirectExceptNearBalancedAndCrowded,
    }

    private enum SuppressionLevel
    {
        Low,
        Medium,
        MediumHigh,
        High,
    }

    private enum EmissionMode
    {
        Silent,
        Direct,
        Fuse,
        Tint,
    }

    private enum AssemblyGroup
    {
        RenderingIdentity,
        ViewConstruction,
        CompositionEnvironment,
        MotionInstability,
        LightingColorAtmosphere,
        MoodSymbolicTone,
        TrailingRefiners,
    }

    private enum ClauseClass
    {
        Primary,
        Support,
        Modifier,
    }

    private enum RichnessCategory
    {
        None,
        MaterialFinish,
        SpatialDepth,
        MotionEnergy,
    }

    private enum MacroPad
    {
        None,
        ViewConstruction,
        LightAtmosphere,
        MaterialFinish,
        SceneRichness,
        EnergyInstability,
        Tone,
    }

    private enum PadLossStage
    {
        Resolution,
        Fusion,
        Cleanup,
        Recovery,
        CapEnforcement,
    }

    private enum ReasonCode
    {
        SuppressedByOwner,
        SuppressedByConflict,
        DowngradedToFuse,
        DowngradedToTint,
        CrowdedOut,
        NeutralQuieted,
        SurvivedAsOwner,
        SurvivedAsExtreme,
    }

    private sealed record ClauseUnit(
        string Text,
        AssemblyGroup Group,
        ClauseClass ClauseClass,
        int Score,
        EmissionMode Mode,
        bool IsOwner,
        bool IsFused,
        string[] SliderKeys);

    private sealed record RichnessDemandProfile(
        bool WantsMaterialFinish,
        bool WantsSpatialDepth,
        bool WantsMotionEnergy);

    private sealed record PadSurvivorPlan(
        MacroPad Pad,
        int ActiveCount,
        int SemanticFloor,
        int ExplicitFloor);

    private sealed record PadTrace(
        int ResolvedCount,
        int Floor,
        PadLossStage? FirstLoss);
}
