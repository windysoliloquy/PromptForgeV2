using System.Windows.Controls;
using PromptForge.App.Services;

namespace PromptForge.App.Views.LaneReplacements.Anime;

public partial class AnimeCompactManualStack : UserControl
{
    private const string LaneId = "anime";
    private const string StyleMoodSectionKey = "style-mood";
    private const string LightingImageFinishSectionKey = "lighting-image-finish";
    private const string SceneCompositionSectionKey = "scene-composition";

    private readonly CompactSectionUiStateService _sectionStateService = new();
    private bool _isStyleMoodExpanded;
    private bool _isLightingImageFinishExpanded;
    private bool _isSceneCompositionExpanded;

    public AnimeCompactManualStack()
    {
        InitializeComponent();
        RestoreSectionStates();
    }

    private void RestoreSectionStates()
    {
        SetStyleMoodExpanded(_sectionStateService.GetIsExpanded(LaneId, StyleMoodSectionKey), persist: false);
        SetLightingImageFinishExpanded(_sectionStateService.GetIsExpanded(LaneId, LightingImageFinishSectionKey), persist: false);
        SetSceneCompositionExpanded(_sectionStateService.GetIsExpanded(LaneId, SceneCompositionSectionKey), persist: false);
    }

    private void OnStyleMoodGateClick(object sender, System.Windows.RoutedEventArgs e)
    {
        SetStyleMoodExpanded(!_isStyleMoodExpanded);
    }

    private void OnLightingGateClick(object sender, System.Windows.RoutedEventArgs e)
    {
        SetLightingImageFinishExpanded(!_isLightingImageFinishExpanded);
    }

    private void OnImageFinishGateClick(object sender, System.Windows.RoutedEventArgs e)
    {
        SetLightingImageFinishExpanded(!_isLightingImageFinishExpanded);
    }

    private void OnLightingImageFinishGateClick(object sender, System.Windows.RoutedEventArgs e)
    {
        SetLightingImageFinishExpanded(!_isLightingImageFinishExpanded);
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
        MoodGateButton.Content = GetGateText(isExpanded);
        StyleMoodCollapsedGateButton.Content = GetGateText(isExpanded);

        if (persist)
        {
            _sectionStateService.SetIsExpanded(LaneId, StyleMoodSectionKey, isExpanded);
        }
    }

    private void SetLightingImageFinishExpanded(bool isExpanded, bool persist = true)
    {
        _isLightingImageFinishExpanded = isExpanded;
        ControlLightingCard.Visibility = ToVisibility(isExpanded);
        ImageFinishCard.Visibility = ToVisibility(isExpanded);
        ControlLightingImageFinishCollapsedCard.Visibility = ToVisibility(!isExpanded);
        LightingGateButton.Content = GetGateText(isExpanded);
        ImageFinishGateButton.Content = GetGateText(isExpanded);
        ControlLightingImageFinishCollapsedGateButton.Content = GetGateText(isExpanded);

        if (persist)
        {
            _sectionStateService.SetIsExpanded(LaneId, LightingImageFinishSectionKey, isExpanded);
        }
    }

    private void SetSceneCompositionExpanded(bool isExpanded, bool persist = true)
    {
        _isSceneCompositionExpanded = isExpanded;
        SceneCompositionSectionContent.Visibility = ToVisibility(isExpanded);
        SceneCompositionGateButton.Content = GetGateText(isExpanded);

        if (persist)
        {
            _sectionStateService.SetIsExpanded(LaneId, SceneCompositionSectionKey, isExpanded);
        }
    }

    private static System.Windows.Visibility ToVisibility(bool isExpanded) =>
        isExpanded ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

    private static string GetGateText(bool isExpanded) => isExpanded ? "Hide" : "Show";
}
