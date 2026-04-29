using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using PromptForge.App.Services;
using PromptForge.App.ViewModels;

namespace PromptForge.App.Views.CompactWorkstation;

public partial class HoverDeckExperimentalCompressedBody : UserControl
{
    public IReadOnlyList<string> HoverDeckIntentModes { get; } = new[]
    {
        IntentModeCatalog.PhotographyName,
        IntentModeCatalog.CinematicName,
        IntentModeCatalog.WatercolorName,
        IntentModeCatalog.PixelArtName,
        IntentModeCatalog.GraphicDesignName,
        IntentModeCatalog.AnimeName,
        IntentModeCatalog.ArchitectureArchvizName,
        IntentModeCatalog.ChildrensBookName,
        IntentModeCatalog.ComicBookName,
        IntentModeCatalog.ConceptArtName,
        IntentModeCatalog.EditorialIllustrationName,
        IntentModeCatalog.FantasyIllustrationName,
        IntentModeCatalog.FoodPhotographyName,
        IntentModeCatalog.InfographicDataVisualizationName,
        IntentModeCatalog.LifestyleAdvertisingPhotographyName,
        IntentModeCatalog.ProductPhotographyName,
        IntentModeCatalog.TattooArtName,
        IntentModeCatalog.ThreeDRenderName,
        IntentModeCatalog.VintageBendName,
    };

    private bool _isStyleMoodOpen;
    private bool _isControlLightingImageFinishOpen;
    private bool _isSceneCompositionOpen;
    private bool _isArtistInfluenceOpen;
    private bool _isHoverDeckCopyPromptFeedbackPending;
    private bool _isStyleMoodRestoreMainWindowPressActive;
    private bool _isStyleMoodRestoreMainWindowLongPressTriggered;
    private MainWindowViewModel? _subscribedViewModel;
    private readonly DispatcherTimer _styleMoodRestoreMainWindowTimer = new()
    {
        Interval = TimeSpan.FromSeconds(5),
    };
    private readonly HashSet<ComboBoxItem> _hoverDeckIntentItems = new();

    public HoverDeckExperimentalCompressedBody()
    {
        InitializeComponent();
        _styleMoodRestoreMainWindowTimer.Tick += OnStyleMoodRestoreMainWindowTimerTick;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        DataContextChanged += OnHoverDeckExperimentalCompressedBodyDataContextChanged;
        UpdateStyleMoodProjection();
        UpdateControlLightingImageFinishProjection();
        UpdateSceneCompositionProjection();
        UpdateArtistInfluenceProjection();
        UpdateHoverDeckCopyPromptStateText();
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        UpdateHoverDeckCopyPromptStateText();
        RefreshHoverDeckIntentItemAccessVisuals();
        LogHoverDeckIntentState("loaded");
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_subscribedViewModel is not null)
        {
            _subscribedViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _subscribedViewModel = null;
        }

        StopStyleMoodRestoreMainWindowTimer(clearLongPressTrigger: true);
        _isHoverDeckCopyPromptFeedbackPending = false;
    }

    private void OnHoverDeckExperimentalCompressedBodyDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_subscribedViewModel is not null)
        {
            _subscribedViewModel.PropertyChanged -= OnViewModelPropertyChanged;
            _subscribedViewModel = null;
        }

        if (e.NewValue is MainWindowViewModel viewModel)
        {
            _subscribedViewModel = viewModel;
            _subscribedViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        UpdateHoverDeckCopyPromptStateText();
        RefreshHoverDeckIntentItemAccessVisuals();
        LogHoverDeckIntentState(
            $"data-context-changed old='{e.OldValue?.GetType().Name ?? "none"}' new='{e.NewValue?.GetType().Name ?? "none"}'");
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(MainWindowViewModel.CopyPromptRemainingText), StringComparison.Ordinal) ||
            string.Equals(e.PropertyName, nameof(MainWindowViewModel.IntentMode), StringComparison.Ordinal) ||
            string.Equals(e.PropertyName, nameof(MainWindowViewModel.HasLockedLaneAccess), StringComparison.Ordinal) ||
            string.Equals(e.PropertyName, nameof(MainWindowViewModel.IsUnlocked), StringComparison.Ordinal) ||
            string.Equals(e.PropertyName, nameof(MainWindowViewModel.IsLockedLaneActive), StringComparison.Ordinal))
        {
            UpdateHoverDeckCopyPromptStateText();
            RefreshHoverDeckIntentItemAccessVisuals();
        }

        if (!_isHoverDeckCopyPromptFeedbackPending ||
            !string.Equals(e.PropertyName, nameof(MainWindowViewModel.CopyPromptFeedbackTick), StringComparison.Ordinal))
        {
            return;
        }

        _isHoverDeckCopyPromptFeedbackPending = false;
        StartHoverDeckCopyPromptFeedbackAnimation();
    }

    private void OnHoverDeckCopyPromptClick(object sender, RoutedEventArgs e)
    {
        _isHoverDeckCopyPromptFeedbackPending = true;
    }

    private void OnHoverDeckClearSubjectClick(object sender, RoutedEventArgs e)
    {
        HoverDeckSubjectTextBox.Text = string.Empty;
        HoverDeckSubjectTextBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.Subject = string.Empty;
        }

        e.Handled = true;
    }

    private void OnHoverDeckSubjectSelectAllClick(object sender, RoutedEventArgs e)
    {
        HoverDeckSubjectTextBox.Focus();
        HoverDeckSubjectTextBox.SelectAll();
    }

    private void StartHoverDeckCopyPromptFeedbackAnimation()
    {
        var storyboardKey = SystemParameters.ClientAreaAnimation
            ? "HoverDeckCopyPromptFeedbackStoryboard"
            : "HoverDeckCopyPromptFeedbackReducedMotionStoryboard";

        if (FindResource(storyboardKey) is not Storyboard storyboard)
        {
            return;
        }

        storyboard.Stop(this);
        storyboard.Begin(this, true);
    }

    private void UpdateHoverDeckCopyPromptStateText()
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            HoverDeckCopyPromptStateText.Text = string.Empty;
            return;
        }

        HoverDeckCopyPromptStateText.Text =
            viewModel.IsLockedLaneActive
                ? "Contact Windy Soliloquy to Unlock This Lane."
                : viewModel.CopyPromptRemainingText;
    }

    private void OnStyleMoodGateClick(object sender, RoutedEventArgs e)
    {
        if (_isStyleMoodRestoreMainWindowLongPressTriggered)
        {
            _isStyleMoodRestoreMainWindowLongPressTriggered = false;
            e.Handled = true;
            return;
        }

        _isStyleMoodOpen = !_isStyleMoodOpen;
        UpdateStyleMoodProjection();
    }

    private void OnStyleMoodGatePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isStyleMoodRestoreMainWindowPressActive = true;
        _isStyleMoodRestoreMainWindowLongPressTriggered = false;
        _styleMoodRestoreMainWindowTimer.Stop();
        _styleMoodRestoreMainWindowTimer.Start();
        UiEventLog.Write("hoverdeck-style-mood-long-press event='started'");
    }

    private void OnStyleMoodGatePreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        StopStyleMoodRestoreMainWindowTimer(clearLongPressTrigger: false);

        if (_isStyleMoodRestoreMainWindowLongPressTriggered)
        {
            e.Handled = true;
        }
    }

    private void OnStyleMoodGateMouseLeave(object sender, MouseEventArgs e)
    {
        if (!_isStyleMoodRestoreMainWindowLongPressTriggered)
        {
            StopStyleMoodRestoreMainWindowTimer(clearLongPressTrigger: false);
        }
    }

    private void OnStyleMoodRestoreMainWindowTimerTick(object? sender, EventArgs e)
    {
        _styleMoodRestoreMainWindowTimer.Stop();
        if (!_isStyleMoodRestoreMainWindowPressActive)
        {
            return;
        }

        _isStyleMoodRestoreMainWindowPressActive = false;
        _isStyleMoodRestoreMainWindowLongPressTriggered = true;
        UiEventLog.Write("hoverdeck-style-mood-long-press event='restore-main'");

        if (Application.Current.MainWindow is PromptForge.App.MainWindow mainWindow)
        {
            mainWindow.RestoreMainWindowFromHoverDeckBackdoor();
        }
    }

    private void StopStyleMoodRestoreMainWindowTimer(bool clearLongPressTrigger)
    {
        _styleMoodRestoreMainWindowTimer.Stop();
        _isStyleMoodRestoreMainWindowPressActive = false;

        if (clearLongPressTrigger)
        {
            _isStyleMoodRestoreMainWindowLongPressTriggered = false;
        }
    }

    private void OnControlLightingImageFinishGateClick(object sender, RoutedEventArgs e)
    {
        _isControlLightingImageFinishOpen = !_isControlLightingImageFinishOpen;
        UpdateControlLightingImageFinishProjection();
    }

    private void OnSceneCompositionGateClick(object sender, RoutedEventArgs e)
    {
        _isSceneCompositionOpen = !_isSceneCompositionOpen;
        UpdateSceneCompositionProjection();
    }

    private void OnArtistInfluenceGateClick(object sender, RoutedEventArgs e)
    {
        _isArtistInfluenceOpen = !_isArtistInfluenceOpen;
        UpdateArtistInfluenceProjection();
    }

    private void OnArtistInfluenceProjectionCollapseRequested(object? sender, EventArgs e)
    {
        _isArtistInfluenceOpen = false;
        UpdateArtistInfluenceProjection();
    }

    private void OnHoverDeckIntentModePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        LogHoverDeckIntentState(
            $"preview-mouse-left-button-down handled={e.Handled} original='{e.OriginalSource?.GetType().Name ?? "none"}'");
    }

    private void OnHoverDeckIntentModeGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        LogHoverDeckIntentState(
            $"got-keyboard-focus old='{e.OldFocus?.GetType().Name ?? "none"}' new='{e.NewFocus?.GetType().Name ?? "none"}'");
    }

    private void OnHoverDeckIntentModeLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        LogHoverDeckIntentState(
            $"lost-keyboard-focus old='{e.OldFocus?.GetType().Name ?? "none"}' new='{e.NewFocus?.GetType().Name ?? "none"}'");
    }

    private void OnHoverDeckIntentModeDropDownOpened(object sender, EventArgs e)
    {
        AttachHoverDeckIntentItemDiagnostics();
        RefreshHoverDeckIntentItemAccessVisuals();
        Dispatcher.BeginInvoke(RefreshHoverDeckIntentItemAccessVisuals, DispatcherPriority.Loaded);
        LogHoverDeckIntentState("dropdown-opened");
    }

    private void OnHoverDeckIntentModeDropDownClosed(object sender, EventArgs e)
    {
        LogHoverDeckIntentState("dropdown-closed");
        DetachHoverDeckIntentItemDiagnostics();
    }

    private void OnHoverDeckIntentModeSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedIntentMode = HoverDeckIntentModeComboBox.SelectedItem as string;
        if (DataContext is not MainWindowViewModel viewModel ||
            string.IsNullOrWhiteSpace(selectedIntentMode))
        {
            LogHoverDeckIntentState(
                $"selection-ignored added={e.AddedItems.Count} removed={e.RemovedItems.Count} hasViewModel={DataContext is MainWindowViewModel}");
            return;
        }

        LogHoverDeckIntentState(
            $"selection-bridge added={FormatSelectionChangedItems(e.AddedItems)} removed={FormatSelectionChangedItems(e.RemovedItems)} before='{viewModel.IntentMode}'");
        viewModel.IntentMode = selectedIntentMode;
        LogHoverDeckIntentState(
            $"selection-bridge-complete after='{viewModel.IntentMode}'");
    }

    private void OnHoverDeckIntentModeItemPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        LogHoverDeckIntentItemState("item-preview-mouse-left-button-down", sender, e);
    }

    private void OnHoverDeckIntentModeItemMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        LogHoverDeckIntentItemState("item-mouse-left-button-up", sender, e);
    }

    private void AttachHoverDeckIntentItemDiagnostics()
    {
        DetachHoverDeckIntentItemDiagnostics();

        foreach (var item in HoverDeckIntentModeComboBox.Items)
        {
            if (HoverDeckIntentModeComboBox.ItemContainerGenerator.ContainerFromItem(item) is not ComboBoxItem comboBoxItem)
            {
                continue;
            }

            comboBoxItem.PreviewMouseLeftButtonDown += OnHoverDeckIntentModeItemPreviewMouseLeftButtonDown;
            comboBoxItem.MouseLeftButtonUp += OnHoverDeckIntentModeItemMouseLeftButtonUp;
            ApplyHoverDeckIntentItemAccessVisual(comboBoxItem);
            _hoverDeckIntentItems.Add(comboBoxItem);
        }

        LogHoverDeckIntentState($"item-diagnostics-attached count={_hoverDeckIntentItems.Count}");
    }

    private void RefreshHoverDeckIntentItemAccessVisuals()
    {
        if (HoverDeckIntentModeComboBox is null)
        {
            return;
        }

        foreach (var item in HoverDeckIntentModeComboBox.Items)
        {
            if (HoverDeckIntentModeComboBox.ItemContainerGenerator.ContainerFromItem(item) is ComboBoxItem comboBoxItem)
            {
                ApplyHoverDeckIntentItemAccessVisual(comboBoxItem);
            }
        }
    }

    private void ApplyHoverDeckIntentItemAccessVisual(ComboBoxItem comboBoxItem)
    {
        var intentMode = comboBoxItem.Content?.ToString();
        var hasAccess = DataContext is MainWindowViewModel viewModel &&
            viewModel.HasLaneAccess(intentMode);

        if (hasAccess)
        {
            comboBoxItem.ClearValue(OpacityProperty);
            comboBoxItem.ClearValue(Control.ForegroundProperty);
            return;
        }

        comboBoxItem.Opacity = 0.46;
        comboBoxItem.SetResourceReference(Control.ForegroundProperty, "TextMutedBrush");
    }

    private void DetachHoverDeckIntentItemDiagnostics()
    {
        foreach (var comboBoxItem in _hoverDeckIntentItems)
        {
            comboBoxItem.PreviewMouseLeftButtonDown -= OnHoverDeckIntentModeItemPreviewMouseLeftButtonDown;
            comboBoxItem.MouseLeftButtonUp -= OnHoverDeckIntentModeItemMouseLeftButtonUp;
        }

        if (_hoverDeckIntentItems.Count > 0)
        {
            LogHoverDeckIntentState($"item-diagnostics-detached count={_hoverDeckIntentItems.Count}");
        }

        _hoverDeckIntentItems.Clear();
    }

    private void LogHoverDeckIntentState(string eventName)
    {
        var ownerWindow = Window.GetWindow(this);
        var selectedItem = HoverDeckIntentModeComboBox?.SelectedItem?.ToString() ?? "none";
        var itemsCount = HoverDeckIntentModeComboBox?.Items.Count ?? -1;
        var bindingExpression = HoverDeckIntentModeComboBox?.GetBindingExpression(Selector.SelectedIndexProperty);
        var viewModel = DataContext as MainWindowViewModel;
        UiEventLog.Write(
            $"hoverdeck-intent event='{eventName}' selectedIndex={HoverDeckIntentModeComboBox?.SelectedIndex ?? -999} selectedItem='{selectedItem}' items={itemsCount} isDropDownOpen={HoverDeckIntentModeComboBox?.IsDropDownOpen.ToString() ?? "none"} hasKeyboardFocus={HoverDeckIntentModeComboBox?.IsKeyboardFocusWithin.ToString() ?? "none"} vmIntent='{viewModel?.IntentMode ?? "none"}' vmIndex={viewModel?.IntentModeSelectedIndex.ToString() ?? "none"} bindingStatus='{bindingExpression?.Status.ToString() ?? "none"}' bindingHasError={bindingExpression?.HasError.ToString() ?? "none"} owner='{ownerWindow?.GetType().Name ?? "none"}' ownerState='{ownerWindow?.WindowState.ToString() ?? "none"}' ownerVisible={ownerWindow?.IsVisible.ToString() ?? "none"} ownerActive={ownerWindow?.IsActive.ToString() ?? "none"}");
    }

    private void LogHoverDeckIntentItemState(string eventName, object sender, MouseButtonEventArgs e)
    {
        var comboBoxItem = sender as ComboBoxItem;
        var resolvedIndex = comboBoxItem is null
            ? -1
            : HoverDeckIntentModeComboBox.ItemContainerGenerator.IndexFromContainer(comboBoxItem);
        var itemText = comboBoxItem?.Content?.ToString() ?? "none";
        LogHoverDeckIntentState(
            $"{eventName} itemIndex={resolvedIndex} item='{itemText}' handled={e.Handled} original='{e.OriginalSource?.GetType().Name ?? "none"}'");
    }

    private static string FormatSelectionChangedItems(System.Collections.IList items)
    {
        if (items.Count == 0)
        {
            return "none";
        }

        var values = new string[items.Count];
        for (var i = 0; i < items.Count; i++)
        {
            values[i] = items[i]?.ToString() ?? "null";
        }

        return string.Join("|", values);
    }

    private void UpdateStyleMoodProjection()
    {
        StyleMoodCollapsedCard.Visibility = _isStyleMoodOpen ? Visibility.Collapsed : Visibility.Visible;
        StyleMoodProjectedContent.Visibility = _isStyleMoodOpen ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateControlLightingImageFinishProjection()
    {
        ControlLightingImageFinishCollapsedCard.Visibility = _isControlLightingImageFinishOpen
            ? Visibility.Collapsed
            : Visibility.Visible;
        ControlLightingImageFinishProjectedContent.Visibility = _isControlLightingImageFinishOpen
            ? Visibility.Visible
            : Visibility.Collapsed;
    }

    private void UpdateSceneCompositionProjection()
    {
        SceneCompositionCollapsedCard.Visibility = _isSceneCompositionOpen ? Visibility.Collapsed : Visibility.Visible;
        SceneCompositionProjectedContent.Visibility = _isSceneCompositionOpen ? Visibility.Visible : Visibility.Collapsed;
    }

    private void UpdateArtistInfluenceProjection()
    {
        ArtistInfluenceCollapsedCard.Visibility = _isArtistInfluenceOpen ? Visibility.Collapsed : Visibility.Visible;
        ArtistInfluenceProjectedContent.Visibility = _isArtistInfluenceOpen ? Visibility.Visible : Visibility.Collapsed;

        if (_isArtistInfluenceOpen)
        {
            ArtistInfluenceProjectedContent.SetExpandedForHost(true);
        }
    }
}
