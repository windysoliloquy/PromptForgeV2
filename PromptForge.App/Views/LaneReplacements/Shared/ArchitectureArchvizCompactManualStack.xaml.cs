using System.Windows.Controls;
using PromptForge.App.Services;

namespace PromptForge.App.Views.LaneReplacements.Shared;

public partial class ArchitectureArchvizCompactManualStack : UserControl
{
    private const string LaneId = "architecture-archviz";
    private const string StyleMoodSectionKey = "style-mood";
    private const string ControlLightingImageFinishSectionKey = "control-lighting-image-finish";
    private const string SceneCompositionSectionKey = "scene-composition";

    private readonly CompactSectionUiStateService _sectionStateService = new();
    private bool _isStyleMoodExpanded;
    private bool _isControlLightingImageFinishExpanded;
    private bool _isSceneCompositionExpanded;

    public ArchitectureArchvizCompactManualStack()
    {
        InitializeComponent();
        RestoreSectionStates();
    }

    private void RestoreSectionStates()
    {
        SetStyleMoodExpanded(_sectionStateService.GetIsExpanded(LaneId, StyleMoodSectionKey), persist: false);
        SetControlLightingImageFinishExpanded(
            _sectionStateService.GetIsExpanded(LaneId, ControlLightingImageFinishSectionKey),
            persist: false);
        SetSceneCompositionExpanded(
            _sectionStateService.GetIsExpanded(LaneId, SceneCompositionSectionKey),
            persist: false);
    }

    private void OnStyleMoodGateClick(object sender, System.Windows.RoutedEventArgs e)
    {
        SetStyleMoodExpanded(!_isStyleMoodExpanded);
    }

    private void OnControlLightingImageFinishGateClick(object sender, System.Windows.RoutedEventArgs e)
    {
        SetControlLightingImageFinishExpanded(!_isControlLightingImageFinishExpanded);
    }

    private void OnSceneCompositionGateClick(object sender, System.Windows.RoutedEventArgs e)
    {
        SetSceneCompositionExpanded(!_isSceneCompositionExpanded);
    }

    private void SetStyleMoodExpanded(bool isExpanded, bool persist = true)
    {
        _isStyleMoodExpanded = isExpanded;
        StyleControlsCard.Visibility = ToVisibility(isExpanded);
        MoodCard.Visibility = ToVisibility(isExpanded);
        StyleMoodCollapsedCard.Visibility = ToVisibility(!isExpanded);
        StyleMoodGateButton.Content = GetGateText(isExpanded);
        StyleMoodCollapsedGateButton.Content = GetGateText(isExpanded);

        if (persist)
        {
            _sectionStateService.SetIsExpanded(LaneId, StyleMoodSectionKey, isExpanded);
        }
    }

    private void SetControlLightingImageFinishExpanded(bool isExpanded, bool persist = true)
    {
        _isControlLightingImageFinishExpanded = isExpanded;
        ControlLightingCard.Visibility = ToVisibility(isExpanded);
        ImageFinishCard.Visibility = ToVisibility(isExpanded);
        ControlLightingImageFinishCollapsedCard.Visibility = ToVisibility(!isExpanded);
        ImageFinishGateButton.Content = GetGateText(isExpanded);
        ControlLightingImageFinishCollapsedGateButton.Content = GetGateText(isExpanded);

        if (persist)
        {
            _sectionStateService.SetIsExpanded(LaneId, ControlLightingImageFinishSectionKey, isExpanded);
        }
    }

    private void SetSceneCompositionExpanded(bool isExpanded, bool persist = true)
    {
        _isSceneCompositionExpanded = isExpanded;
        SceneCompositionCard.Visibility = ToVisibility(isExpanded);
        SceneCompositionCollapsedCard.Visibility = ToVisibility(!isExpanded);
        SceneCompositionGateButton.Content = GetGateText(isExpanded);
        SceneCompositionCollapsedGateButton.Content = GetGateText(isExpanded);

        if (persist)
        {
            _sectionStateService.SetIsExpanded(LaneId, SceneCompositionSectionKey, isExpanded);
        }
    }

    private static System.Windows.Visibility ToVisibility(bool isExpanded) =>
        isExpanded ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

    private static string GetGateText(bool isExpanded) => isExpanded ? "Hide" : "Show";
}

