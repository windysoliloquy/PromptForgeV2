using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using PromptForge.App.Services;
using PromptForge.App.ViewModels;

namespace PromptForge.App.Views.LaneReplacements.Shared;

public partial class CompactArtistInfluenceCard : UserControl
{
    private const string SharedUiLaneId = "shared";
    private const string ArtistInfluenceSectionKey = "artist-influence";
    private readonly Thickness _expandedCardMargin;
    private readonly Thickness _expandedCardPadding;

    private readonly CompactSectionUiStateService _compactSectionStateService = new();
    private bool _isArtistInfluenceExpanded;
    private bool _isObservingSectionState;

    public event EventHandler? CollapseRequested;

    public CompactArtistInfluenceCard()
    {
        InitializeComponent();
        _expandedCardMargin = ArtistInfluenceCard.Margin;
        _expandedCardPadding = ArtistInfluenceCard.Padding;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public bool UseHostCollapseRouting { get; set; }

    public void SetExpandedForHost(bool isExpanded)
    {
        SetArtistInfluenceExpanded(isExpanded, persist: false);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (!_isObservingSectionState)
        {
            CompactSectionUiStateService.SectionStateChanged += OnSectionStateChanged;
            _isObservingSectionState = true;
        }

        SetArtistInfluenceExpanded(
            _compactSectionStateService.GetIsExpanded(SharedUiLaneId, ArtistInfluenceSectionKey),
            persist: false);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (!_isObservingSectionState)
        {
            return;
        }

        CompactSectionUiStateService.SectionStateChanged -= OnSectionStateChanged;
        _isObservingSectionState = false;
    }

    private void OnSectionStateChanged(object? sender, CompactSectionUiStateChangedEventArgs e)
    {
        if (!string.Equals(e.LaneId, SharedUiLaneId, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(e.SectionKey, ArtistInfluenceSectionKey, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        Dispatcher.BeginInvoke(() => SetArtistInfluenceExpanded(e.IsExpanded, persist: false));
    }

    private void OnArtistInfluenceGateClick(object sender, RoutedEventArgs e)
    {
        var nextIsExpanded = !_isArtistInfluenceExpanded;

        if (UseHostCollapseRouting && !nextIsExpanded)
        {
            SetArtistInfluenceExpanded(false, persist: false);
            CollapseRequested?.Invoke(this, EventArgs.Empty);
            return;
        }

        SetArtistInfluenceExpanded(nextIsExpanded);
    }

    private void OnArtistInfluenceTitleClick(object sender, RoutedEventArgs e)
    {
        OnArtistInfluenceGateClick(sender, e);
    }

    private void OnArtistInfluenceResetClick(object sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        viewModel.ArtistInfluencePrimary = "None";
        viewModel.ArtistInfluenceSecondary = "None";

        Dispatcher.BeginInvoke(RefreshArtistInfluenceResetTargets, DispatcherPriority.Background);
    }

    private void RefreshArtistInfluenceResetTargets()
    {
        RefreshArtistInfluenceResetTarget(PrimaryArtistInfluenceComboBox);
        RefreshArtistInfluenceResetTarget(SecondaryArtistInfluenceComboBox);
    }

    private static void RefreshArtistInfluenceResetTarget(ComboBox comboBox)
    {
        comboBox.GetBindingExpression(Selector.SelectedItemProperty)?.UpdateTarget();
        comboBox.GetBindingExpression(ComboBox.TextProperty)?.UpdateTarget();
    }

    private void SetArtistInfluenceExpanded(bool isExpanded, bool persist = true)
    {
        _isArtistInfluenceExpanded = isExpanded;
        ArtistInfluenceBody.Visibility = isExpanded ? Visibility.Visible : Visibility.Collapsed;
        ArtistInfluenceResetButton.Visibility = isExpanded ? Visibility.Visible : Visibility.Collapsed;
        ArtistInfluenceCard.Margin = isExpanded
            ? _expandedCardMargin
            : (Thickness)FindResource("CompactArtistInfluenceCollapsedMargin");
        ArtistInfluenceCard.Padding = isExpanded
            ? _expandedCardPadding
            : (Thickness)FindResource("CompactArtistInfluenceCollapsedPadding");

        if (persist)
        {
            _compactSectionStateService.SetIsExpanded(SharedUiLaneId, ArtistInfluenceSectionKey, isExpanded);
        }
    }
}
