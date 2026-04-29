using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using PromptForge.App.Services;
using PromptForge.App.ViewModels;

namespace PromptForge.App;

public partial class MainWindow : Window
{
    public static readonly DependencyProperty IsArtistPhraseEditorMainHostOpenProperty =
        DependencyProperty.Register(
            nameof(IsArtistPhraseEditorMainHostOpen),
            typeof(bool),
            typeof(MainWindow),
            new PropertyMetadata(false));

    private const double HoverDeckPreferredLeftInset = 23d;
    private const double HoverDeckPreferredTopInset = 32d;

    private readonly ILicenseService _licenseService;
    private MainWindowViewModel? _observedViewModel;
    private Storyboard? _activeButtonMorphStoryboard;
    private bool _isDraggingArtistPhraseEditor;
    private Point _artistPhraseEditorDragStart;
    private double _artistPhraseEditorStartHorizontalOffset;
    private double _artistPhraseEditorStartVerticalOffset;
    private ComboBoxItem? _photographyIntentComboBoxItem;
    private bool _keepPhotographyIntentPopupOpen;
    private HoverDeckCardWindow? _hoverDeckCardWindow;
    private bool _keepMainWindowMinimizedForHoverDeck;
    private bool _suspendHoverDeckMinimizeEnforcement;
    private bool _mainWindowShowInTaskbarBeforeHoverDeck = true;
    private bool _isShuttingDownFromHoverDeck;

    public bool IsArtistPhraseEditorMainHostOpen
    {
        get => (bool)GetValue(IsArtistPhraseEditorMainHostOpenProperty);
        set => SetValue(IsArtistPhraseEditorMainHostOpenProperty, value);
    }

    public MainWindow()
    {
        _licenseService = new LicenseService();
        InitializeComponent();
        UiEventLog.Reset();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnWindowLoaded;
        SizeChanged += OnWindowSizeChanged;
        StateChanged += OnWindowStateChanged;
        Activated += OnWindowActivated;
    }

    public MainWindow(ILicenseService licenseService)
    {
        _licenseService = licenseService;
        InitializeComponent();
        UiEventLog.Reset();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnWindowLoaded;
        SizeChanged += OnWindowSizeChanged;
        StateChanged += OnWindowStateChanged;
        Activated += OnWindowActivated;
    }

    private void OnVersionInfoClick(object sender, RoutedEventArgs e)
    {
        ShowVersionInfoDialog();
    }

    private void OnBrandPromptForgeClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        ImageGalleryVisitPromptWindow.ShowFor(this);
        e.Handled = true;
    }

    public void ShowVersionInfoDialog()
    {
        ShowVersionInfoDialog(IsVisible ? this : _hoverDeckCardWindow);
    }

    public void ShowVersionInfoDialog(Window? owner)
    {
        var unlockWindow = new UnlockWindow(_licenseService, HandleLicenseStateChanged)
        {
        };

        if (owner?.IsVisible == true)
        {
            unlockWindow.Owner = owner;
        }

        unlockWindow.ShowDialog();
    }

    private void HandleLicenseStateChanged()
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.RefreshLicenseState();
        }
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_observedViewModel is not null)
        {
            _observedViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }

        _observedViewModel = e.NewValue as MainWindowViewModel;

        if (_observedViewModel is not null)
        {
            _observedViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        if (_hoverDeckCardWindow is not null)
        {
            _hoverDeckCardWindow.DataContext = _observedViewModel;
        }

        UpdateArtistPhraseEditorMainHostOpen();
    }

    private void OnOpenHoverDeckCardClick(object sender, RoutedEventArgs e)
    {
        LogHoverDeckWindowState("launcher-click");
        OpenHoverDeckCard(minimizeMainWindow: true);
    }

    public void OpenHoverDeckCard(bool minimizeMainWindow = false)
    {
        LogHoverDeckWindowState($"open-request minimizeMainWindow={minimizeMainWindow}");
        if (_hoverDeckCardWindow is not null)
        {
            LogHoverDeckWindowState("open-existing-window");
            if (_hoverDeckCardWindow.WindowState == WindowState.Minimized)
            {
                _hoverDeckCardWindow.WindowState = WindowState.Normal;
                LogHoverDeckWindowState("existing-window-restored-from-minimized");
            }

            _hoverDeckCardWindow.Activate();
            LogHoverDeckWindowState("existing-window-activated");
            if (minimizeMainWindow)
            {
                MinimizeMainWindowForHoverDeck();
            }

            return;
        }

        var hoverDeckSpawnBounds = GetHoverDeckSpawnBounds();
        LogHoverDeckWindowState(
            $"open-create-window left={hoverDeckSpawnBounds.Left:0.##} top={hoverDeckSpawnBounds.Top:0.##} width={hoverDeckSpawnBounds.Width:0.##} height={hoverDeckSpawnBounds.Height:0.##}");
        _hoverDeckCardWindow = new HoverDeckCardWindow
        {
            DataContext = DataContext,
            Left = hoverDeckSpawnBounds.Left,
            Top = hoverDeckSpawnBounds.Top,
            Width = hoverDeckSpawnBounds.Width,
            Height = hoverDeckSpawnBounds.Height,
        };

        _hoverDeckCardWindow.Closed += (_, _) =>
        {
            LogHoverDeckWindowState("hoverdeck-window-closed");
            _hoverDeckCardWindow = null;
            if (_isShuttingDownFromHoverDeck || Dispatcher.HasShutdownStarted)
            {
                LogHoverDeckWindowState("hoverdeck-window-closed-skip-restore reason='shutdown'");
                return;
            }

            RestoreMainWindowFromHoverDeckClose();
        };
        _hoverDeckCardWindow.Show();
        LogHoverDeckWindowState("hoverdeck-window-shown");
        _hoverDeckCardWindow.Activate();
        LogHoverDeckWindowState("hoverdeck-window-activated");

        if (minimizeMainWindow)
        {
            MinimizeMainWindowForHoverDeck();
        }
    }

    private void MinimizeMainWindowForHoverDeck()
    {
        LogHoverDeckWindowState("minimize-main-requested");
        if (_suspendHoverDeckMinimizeEnforcement)
        {
            LogHoverDeckWindowState("minimize-main-skipped reason='suspended'");
            return;
        }

        if (!_keepMainWindowMinimizedForHoverDeck)
        {
            _mainWindowShowInTaskbarBeforeHoverDeck = ShowInTaskbar;
        }

        _keepMainWindowMinimizedForHoverDeck = true;
        UpdateArtistPhraseEditorMainHostOpen();
        ShowInTaskbar = false;
        WindowState = WindowState.Minimized;
        Hide();
        LogHoverDeckWindowState("minimize-main-applied");
    }

    private void RestoreMainWindowFromHoverDeckClose()
    {
        LogHoverDeckWindowState("restore-main-requested");
        RestoreMainWindowFromHoverDeck(showHoverDeckClosedLog: false);
    }

    public void RestoreMainWindowFromHoverDeckBackdoor()
    {
        LogHoverDeckWindowState("restore-main-long-press-backdoor");
        RestoreMainWindowFromHoverDeck(showHoverDeckClosedLog: false);
    }

    public void ShutdownFromHoverDeck()
    {
        LogHoverDeckWindowState("shutdown-from-hoverdeck-requested");
        _isShuttingDownFromHoverDeck = true;
        Application.Current.Shutdown();
    }

    private void RestoreMainWindowFromHoverDeck(bool showHoverDeckClosedLog)
    {
        _keepMainWindowMinimizedForHoverDeck = false;
        UpdateArtistPhraseEditorMainHostOpen();
        _suspendHoverDeckMinimizeEnforcement = true;

        ShowInTaskbar = _mainWindowShowInTaskbarBeforeHoverDeck;
        Show();
        WindowState = WindowState.Maximized;
        Activate();

        _suspendHoverDeckMinimizeEnforcement = false;
        LogHoverDeckWindowState("restore-main-complete");
    }

    private void OnWindowStateChanged(object? sender, EventArgs e)
    {
        LogHoverDeckWindowState("main-state-changed");
        EnforceMainWindowMinimizedForHoverDeck();
    }

    private void OnWindowActivated(object? sender, EventArgs e)
    {
        LogHoverDeckWindowState("main-activated");
        EnforceMainWindowMinimizedForHoverDeck();
    }

    private void EnforceMainWindowMinimizedForHoverDeck()
    {
        LogHoverDeckWindowState("enforce-minimized-check");
        if (!_keepMainWindowMinimizedForHoverDeck ||
            _suspendHoverDeckMinimizeEnforcement)
        {
            LogHoverDeckWindowState("enforce-minimized-skipped");
            return;
        }

        Dispatcher.BeginInvoke(() =>
        {
            LogHoverDeckWindowState("enforce-minimized-dispatch");
            if (_keepMainWindowMinimizedForHoverDeck && !_suspendHoverDeckMinimizeEnforcement)
            {
                ShowInTaskbar = false;
                WindowState = WindowState.Minimized;
                Hide();
                LogHoverDeckWindowState("enforce-minimized-applied");
            }
        });
    }

    private static Rect GetHoverDeckSpawnBounds()
    {
        var workArea = SystemParameters.WorkArea;
        var width = Math.Min(HoverDeckCardWindow.PreferredWindowWidth, Math.Max(280d, workArea.Width - (HoverDeckPreferredLeftInset * 2d)));
        var height = Math.Min(HoverDeckCardWindow.PreferredWindowHeight, Math.Max(180d, workArea.Height - (HoverDeckPreferredTopInset * 2d)));
        var left = Clamp(workArea.Left + HoverDeckPreferredLeftInset, workArea.Left, workArea.Right - width);
        var top = Clamp(workArea.Top + HoverDeckPreferredTopInset, workArea.Top, workArea.Bottom - height);

        return new Rect(left, top, width, height);
    }

    private static double Clamp(double value, double min, double max)
    {
        if (max < min)
        {
            return min;
        }

        return Math.Max(min, Math.Min(max, value));
    }

    private void LogHoverDeckWindowState(string eventName)
    {
        UiEventLog.Write(
            $"hoverdeck-window event='{eventName}' mainState='{WindowState}' mainVisible={IsVisible} mainActive={IsActive} showInTaskbar={ShowInTaskbar} keepMinimized={_keepMainWindowMinimizedForHoverDeck} suspend={_suspendHoverDeckMinimizeEnforcement} hoverDeckExists={_hoverDeckCardWindow is not null} hoverDeckState='{_hoverDeckCardWindow?.WindowState.ToString() ?? "none"}' hoverDeckVisible={_hoverDeckCardWindow?.IsVisible.ToString() ?? "none"} hoverDeckActive={_hoverDeckCardWindow?.IsActive.ToString() ?? "none"}");
    }

    private void UpdateArtistPhraseEditorMainHostOpen()
    {
        IsArtistPhraseEditorMainHostOpen =
            !_keepMainWindowMinimizedForHoverDeck &&
            _observedViewModel?.IsArtistPhraseEditorOpen == true;
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(MainWindowViewModel.IsArtistPhraseEditorOpen), StringComparison.Ordinal))
        {
            UpdateArtistPhraseEditorMainHostOpen();
        }

        if (string.Equals(e.PropertyName, nameof(MainWindowViewModel.IntentMode), StringComparison.Ordinal))
        {
            if (sender is MainWindowViewModel intentViewModel)
            {
                UiEventLog.Write($"intent-changed intent='{intentViewModel.IntentMode}'");
            }

            Dispatcher.BeginInvoke(() =>
            {
                CloseAllSliderFlyouts();

                Dispatcher.BeginInvoke(() =>
                {
                    RefreshSliderFlyoutExcludeBindings();
                    InvalidateVisual();
                    UpdateLayout();
                }, DispatcherPriority.ApplicationIdle);
            }, DispatcherPriority.Background);
            return;
        }

        if (!string.Equals(e.PropertyName, nameof(MainWindowViewModel.CopyPromptFeedbackTick), StringComparison.Ordinal))
        {
            return;
        }

        Dispatcher.Invoke(StartCopyPromptFeedbackAnimation);
    }

    private void StartCopyPromptFeedbackAnimation()
    {
        var storyboardKey = SystemParameters.ClientAreaAnimation
            ? "CopyPromptBorderStoryboard"
            : "CopyPromptReducedMotionStoryboard";
        StartNamedStoryboard(storyboardKey);
    }

    private void StartNamedStoryboard(string storyboardKey)
    {
        if (FindResource(storyboardKey) is not Storyboard storyboard)
        {
            return;
        }

        storyboard.Stop(this);
        storyboard.Begin(this, true);
    }

    private void OnActionCardButtonClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        if (ReferenceEquals(button, CopyPromptButton))
        {
            return;
        }

        var borderStoryboardKey = SystemParameters.ClientAreaAnimation
            ? "ActionCardBorderGlowStoryboard"
            : "ActionCardBorderGlowReducedMotionStoryboard";
        StartNamedStoryboard(borderStoryboardKey);
        StartActionCardButtonMorph(button);
    }

    private void StartActionCardButtonMorph(Button button)
    {
        _activeButtonMorphStoryboard?.Stop(this);

        var scale = EnsureButtonScaleTransform(button);
        ResetActionCardButtonMorph(button, scale);

        var storyboard = new Storyboard
        {
            FillBehavior = FillBehavior.Stop,
        };

        var useReducedMotion = !SystemParameters.ClientAreaAnimation;
        var pressScale = GetDouble(useReducedMotion ? "ForgeReducedMotionButtonPressScale" : "ForgeButtonPressScale");
        var collapseScaleX = GetDouble(useReducedMotion ? "ForgeReducedMotionButtonCollapseScaleX" : "ForgeButtonCollapseScaleX");
        var collapseScaleY = GetDouble(useReducedMotion ? "ForgeReducedMotionButtonCollapseScaleY" : "ForgeButtonCollapseScaleY");
        var collapsedOpacity = GetDouble(useReducedMotion ? "ForgeReducedMotionButtonCollapsedOpacity" : "ForgeButtonCollapsedOpacity");
        var pressDuration = GetDuration(useReducedMotion ? "ForgeReducedMotionButtonPressDuration" : "ForgeButtonPressDuration");
        var collapseDuration = GetDuration(useReducedMotion ? "ForgeReducedMotionButtonCollapseDuration" : "ForgeButtonCollapseDuration");
        var returnDuration = GetDuration(useReducedMotion ? "ForgeReducedMotionButtonReturnDuration" : "ForgeButtonReturnDuration");
        var collapseBegin = GetTimeSpan(useReducedMotion ? "ForgeReducedMotionButtonCollapseBegin" : "ForgeButtonCollapseBegin");
        var returnBegin = GetTimeSpan(useReducedMotion ? "ForgeReducedMotionButtonReturnBegin" : "ForgeButtonReturnBegin");

        AddDouble(storyboard, scale, "ScaleX", 1, pressScale, TimeSpan.Zero, pressDuration);
        AddDouble(storyboard, scale, "ScaleY", 1, pressScale, TimeSpan.Zero, pressDuration);
        AddDouble(storyboard, scale, "ScaleX", null, collapseScaleX, collapseBegin, collapseDuration);
        AddDouble(storyboard, scale, "ScaleY", null, collapseScaleY, collapseBegin, collapseDuration);
        AddDouble(storyboard, button, "Opacity", null, collapsedOpacity, collapseBegin, collapseDuration);
        AddDouble(storyboard, scale, "ScaleX", null, 1, returnBegin, returnDuration);
        AddDouble(storyboard, scale, "ScaleY", null, 1, returnBegin, returnDuration);
        AddDouble(storyboard, button, "Opacity", null, 1, returnBegin, returnDuration);

        storyboard.Completed += (_, _) => ResetActionCardButtonMorph(button, scale);
        _activeButtonMorphStoryboard = storyboard;
        storyboard.Begin(this, true);
    }

    private void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
    }

    private void OnIntentModeDropDownOpened(object sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            UiEventLog.Write($"intent-dropdown-opened currentIntent='{viewModel.IntentMode}' selectedItem='{IntentModeComboBox.SelectedItem}' selectedIndex={IntentModeComboBox.SelectedIndex}");
            viewModel.ResetCompressionStateForIntentPicker();
        }

        Dispatcher.BeginInvoke(() =>
        {
            RefreshCompressionCheckboxBindings();
            AttachPhotographyIntentDropdownBehavior();
            InvalidateVisual();
            UpdateLayout();
        }, DispatcherPriority.Loaded);
    }

    private void OnIntentModeDropDownClosed(object sender, EventArgs e)
    {
        UiEventLog.Write($"intent-dropdown-closed selectedItem='{IntentModeComboBox.SelectedItem}' selectedIndex={IntentModeComboBox.SelectedIndex} popupOpen={PhotographyIntentPopup.IsOpen}");
        DetachPhotographyIntentDropdownBehavior();
        if (_keepPhotographyIntentPopupOpen)
        {
            UiEventLog.Write("intent-dropdown-closed preserving-photography-popup");
            _keepPhotographyIntentPopupOpen = false;
            return;
        }

        ClosePhotographyIntentPopup();
    }

    private void OnWindowPreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        UiEventLog.Write($"intent-window-preview-mousedown source='{DescribeDependencyObject(e.OriginalSource as DependencyObject)}' popupOpen={PhotographyIntentPopup.IsOpen} dropdownOpen={IntentModeComboBox.IsDropDownOpen}");

        if (!PhotographyIntentPopup.IsOpen || e.OriginalSource is not DependencyObject originalSource)
        {
            return;
        }

        if (IsDescendantOf(originalSource, PhotographyIntentPopupBorder) ||
            IsDescendantOf(originalSource, _photographyIntentComboBoxItem))
        {
            return;
        }

        ClosePhotographyIntentPopup();
    }

    private void AttachPhotographyIntentDropdownBehavior()
    {
        DetachPhotographyIntentDropdownBehavior();
        BuildPhotographyIntentPopupButtons();
        UpdatePhotographyIntentDropdownVisibility(Visibility.Collapsed);
        UiEventLog.Write($"intent-photo-attach popupButtons={PhotographyIntentPopupPanel.Children.Count} selectedItem='{IntentModeComboBox.SelectedItem}'");

        if (IntentModeComboBox.ItemContainerGenerator.ContainerFromItem(IntentModeCatalog.PhotographyName) is not ComboBoxItem photographyItem)
        {
            UiEventLog.Write("intent-photo-attach-missed-photography-item");
            return;
        }

        _photographyIntentComboBoxItem = photographyItem;
        _photographyIntentComboBoxItem.PreviewMouseLeftButtonDown += OnPhotographyIntentItemPreviewMouseLeftButtonDown;
        _photographyIntentComboBoxItem.PreviewMouseLeftButtonUp += OnPhotographyIntentItemPreviewMouseLeftButtonUp;
    }

    private void DetachPhotographyIntentDropdownBehavior()
    {
        UiEventLog.Write($"intent-photo-detach hasPhotographyItem={_photographyIntentComboBoxItem is not null}");
        UpdatePhotographyIntentDropdownVisibility(Visibility.Visible);

        if (_photographyIntentComboBoxItem is null)
        {
            return;
        }

        _photographyIntentComboBoxItem.PreviewMouseLeftButtonDown -= OnPhotographyIntentItemPreviewMouseLeftButtonDown;
        _photographyIntentComboBoxItem.PreviewMouseLeftButtonUp -= OnPhotographyIntentItemPreviewMouseLeftButtonUp;
        _photographyIntentComboBoxItem = null;
    }

    private void UpdatePhotographyIntentDropdownVisibility(Visibility visibility)
    {
        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var affectedCount = 0;
        foreach (var intentMode in viewModel.IntentModes)
        {
            if (!intentMode.Contains("Photography", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(intentMode, IntentModeCatalog.PhotographyName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (IntentModeComboBox.ItemContainerGenerator.ContainerFromItem(intentMode) is ComboBoxItem item)
            {
                item.Visibility = visibility;
                item.Height = visibility == Visibility.Collapsed ? 0 : double.NaN;
                item.IsHitTestVisible = visibility == Visibility.Visible;
                affectedCount++;
            }
        }

        UiEventLog.Write($"intent-photo-visibility visibility='{visibility}' affectedItems={affectedCount}");
    }

    private void BuildPhotographyIntentPopupButtons()
    {
        PhotographyIntentPopupPanel.Children.Clear();

        foreach (var intent in IntentModeCatalog.PhotographyFamilyNames)
        {
            var button = new Button
            {
                Content = intent,
                Tag = intent,
                Style = (Style)FindResource("EmphasisSecondaryButtonStyle"),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 6),
                Padding = new Thickness(12, 8, 12, 8),
            };

            button.PreviewMouseLeftButtonDown += OnPhotographyIntentPopupButtonPreviewMouseLeftButtonDown;
            PhotographyIntentPopupPanel.Children.Add(button);
        }

        if (PhotographyIntentPopupPanel.Children.Count > 0 &&
            PhotographyIntentPopupPanel.Children[^1] is FrameworkElement lastChild)
        {
            lastChild.Margin = new Thickness(0);
        }

        UiEventLog.Write($"intent-photo-popup-built count={PhotographyIntentPopupPanel.Children.Count} items='{string.Join(", ", IntentModeCatalog.PhotographyFamilyNames)}'");
    }

    private void OnPhotographyIntentItemPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        UiEventLog.Write($"intent-photo-item-mousedown handledBefore={e.Handled} source='{DescribeDependencyObject(sender as DependencyObject)}'");
        e.Handled = true;
        _keepPhotographyIntentPopupOpen = true;
        OpenPhotographyIntentPopup(e.GetPosition(this));
        IntentModeComboBox.IsDropDownOpen = false;
    }

    private void OnPhotographyIntentItemPreviewMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        UiEventLog.Write($"intent-photo-item-mouseup handledBefore={e.Handled} source='{DescribeDependencyObject(sender as DependencyObject)}'");
        e.Handled = true;
    }

    private void OpenPhotographyIntentPopup(Point anchorPoint)
    {
        if (_photographyIntentComboBoxItem is null)
        {
            UiEventLog.Write("intent-photo-popup-open-skipped reason='no photography item'");
            return;
        }

        PhotographyIntentPopupBorder.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var popupHeight = PhotographyIntentPopupBorder.DesiredSize.Height;
        var popupWidth = Math.Max(PhotographyIntentPopupBorder.DesiredSize.Width, IntentModeComboBox.ActualWidth);
        var horizontalOffset = Math.Max(12, Math.Min(anchorPoint.X + 12, ActualWidth - popupWidth - 12));
        var verticalOffset = Math.Max(12, anchorPoint.Y - popupHeight - 12);

        PhotographyIntentPopup.PlacementTarget = this;
        PhotographyIntentPopup.Placement = PlacementMode.RelativePoint;
        PhotographyIntentPopup.HorizontalOffset = horizontalOffset;
        PhotographyIntentPopup.VerticalOffset = verticalOffset;
        PhotographyIntentPopup.IsOpen = true;
        UiEventLog.Write($"intent-photo-popup-opened anchorX={anchorPoint.X:0.##} anchorY={anchorPoint.Y:0.##} popupX={horizontalOffset:0.##} popupY={verticalOffset:0.##} selectedItem='{IntentModeComboBox.SelectedItem}' selectedIndex={IntentModeComboBox.SelectedIndex}");
    }

    private void ClosePhotographyIntentPopup()
    {
        UiEventLog.Write($"intent-photo-popup-closed selectedItem='{IntentModeComboBox.SelectedItem}' selectedIndex={IntentModeComboBox.SelectedIndex}");
        PhotographyIntentPopup.IsOpen = false;
    }

    private void OnPhotographyIntentPopupButtonPreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (sender is not Button button || button.Tag is not string intentMode)
        {
            UiEventLog.Write("intent-photo-popup-preview-click-skipped reason='invalid sender'");
            return;
        }

        UiEventLog.Write($"intent-photo-popup-preview-click intent='{intentMode}' selectedItemBefore='{IntentModeComboBox.SelectedItem}' selectedIndexBefore={IntentModeComboBox.SelectedIndex} dropdownOpen={IntentModeComboBox.IsDropDownOpen}");
        e.Handled = true;

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.IntentMode = intentMode;
            UiEventLog.Write($"intent-photo-popup-preview-applied intent='{intentMode}' vmIntent='{viewModel.IntentMode}'");
        }

        ClosePhotographyIntentPopup();
        IntentModeComboBox.IsDropDownOpen = false;

        Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
        {
            IntentModeComboBox.SelectedItem = intentMode;
            UiEventLog.Write($"intent-photo-popup-preview-complete intent='{intentMode}' selectedItemAfter='{IntentModeComboBox.SelectedItem}' selectedIndexAfter={IntentModeComboBox.SelectedIndex} dropdownOpen={IntentModeComboBox.IsDropDownOpen}");
        }));
    }

    private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
        ClampArtistPhraseEditorPopupToBounds();
    }

    private void OnArtistPhraseEditorPopupOpened(object sender, EventArgs e)
    {
        CenterArtistPhraseEditorPopup();
    }

    private void OnArtistPhraseEditorPopupClosed(object sender, EventArgs e)
    {
        _isDraggingArtistPhraseEditor = false;
        ArtistPhraseEditorDragHandle.ReleaseMouseCapture();
    }

    private void OnArtistPhraseEditorDragHandleMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (!ArtistPhraseEditorPopup.IsOpen)
        {
            return;
        }

        _isDraggingArtistPhraseEditor = true;
        _artistPhraseEditorDragStart = e.GetPosition(RootLayout);
        _artistPhraseEditorStartHorizontalOffset = ArtistPhraseEditorPopup.HorizontalOffset;
        _artistPhraseEditorStartVerticalOffset = ArtistPhraseEditorPopup.VerticalOffset;
        ArtistPhraseEditorDragHandle.CaptureMouse();
        e.Handled = true;
    }

    private void OnArtistPhraseEditorDragHandleMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (!_isDraggingArtistPhraseEditor || !ArtistPhraseEditorDragHandle.IsMouseCaptured)
        {
            return;
        }

        var currentPosition = e.GetPosition(RootLayout);
        var delta = currentPosition - _artistPhraseEditorDragStart;
        ArtistPhraseEditorPopup.HorizontalOffset = _artistPhraseEditorStartHorizontalOffset + delta.X;
        ArtistPhraseEditorPopup.VerticalOffset = _artistPhraseEditorStartVerticalOffset + delta.Y;
    }

    private void OnArtistPhraseEditorDragHandleMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (!_isDraggingArtistPhraseEditor)
        {
            return;
        }

        _isDraggingArtistPhraseEditor = false;
        ArtistPhraseEditorDragHandle.ReleaseMouseCapture();
        e.Handled = true;
    }

    private void CenterArtistPhraseEditorPopup()
    {
        if (RootLayout is null || ArtistPhraseEditorPopupCard is null)
        {
            return;
        }

        ArtistPhraseEditorPopupCard.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var popupSize = ArtistPhraseEditorPopupCard.DesiredSize;
        var availableWidth = Math.Max(0d, RootLayout.ActualWidth);
        var availableHeight = Math.Max(0d, RootLayout.ActualHeight);

        ArtistPhraseEditorPopup.HorizontalOffset = Math.Max(0d, (availableWidth - popupSize.Width) * 0.5);
        ArtistPhraseEditorPopup.VerticalOffset = Math.Max(0d, (availableHeight - popupSize.Height) * 0.5);
        ClampArtistPhraseEditorPopupToBounds();
    }

    private void ClampArtistPhraseEditorPopupToBounds()
    {
        if (RootLayout is null || ArtistPhraseEditorPopupCard is null || !ArtistPhraseEditorPopup.IsOpen)
        {
            return;
        }

        ArtistPhraseEditorPopupCard.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        var popupSize = ArtistPhraseEditorPopupCard.DesiredSize;
        var maxHorizontalOffset = Math.Max(0d, RootLayout.ActualWidth - popupSize.Width);
        var maxVerticalOffset = Math.Max(0d, RootLayout.ActualHeight - popupSize.Height);

        ArtistPhraseEditorPopup.HorizontalOffset = Math.Clamp(ArtistPhraseEditorPopup.HorizontalOffset, 0d, maxHorizontalOffset);
        ArtistPhraseEditorPopup.VerticalOffset = Math.Clamp(ArtistPhraseEditorPopup.VerticalOffset, 0d, maxVerticalOffset);
    }

    private static ScaleTransform EnsureButtonScaleTransform(Button button)
    {
        if (button.RenderTransform is ScaleTransform scale)
        {
            return scale;
        }

        scale = new ScaleTransform(1, 1);
        button.RenderTransform = scale;
        button.RenderTransformOrigin = new Point(0.5, 0.5);
        return scale;
    }

    private static void ResetActionCardButtonMorph(Button button, ScaleTransform scale)
    {
        button.Opacity = 1;
        scale.ScaleX = 1;
        scale.ScaleY = 1;
    }

    private void AddDouble(Storyboard storyboard, DependencyObject target, string propertyPath, double? from, double to, TimeSpan beginTime, Duration duration)
    {
        var animation = new DoubleAnimation
        {
            To = to,
            BeginTime = beginTime,
            Duration = duration,
        };

        if (from.HasValue)
        {
            animation.From = from.Value;
        }

        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, new PropertyPath(propertyPath));
        storyboard.Children.Add(animation);
    }

    private void AddColor(Storyboard storyboard, DependencyObject target, string propertyPath, Color to, TimeSpan beginTime, Duration duration)
    {
        var animation = new ColorAnimation
        {
            To = to,
            BeginTime = beginTime,
            Duration = duration,
        };

        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, new PropertyPath(propertyPath));
        storyboard.Children.Add(animation);
    }

    private double GetDouble(string key) => (double)FindResource(key);

    private Duration GetDuration(string key) => (Duration)FindResource(key);

    private TimeSpan GetTimeSpan(string key) => (TimeSpan)FindResource(key);

    private Color GetColor(string key) => (Color)FindResource(key);

    private void RefreshCompressionCheckboxBindings()
    {
    }

    private void CloseAllSliderFlyouts()
    {
        foreach (var flyout in FindVisualChildren<Controls.SliderFlyout>(this))
        {
            flyout.CloseFlyout();
        }
    }

    private void RefreshSliderFlyoutExcludeBindings()
    {
        foreach (var flyout in FindVisualChildren<Controls.SliderFlyout>(this))
        {
            if (flyout.IsFlyoutSessionActive)
            {
                continue;
            }

            flyout.RefreshExcludeBindingFromSource();
        }
    }

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject root) where T : DependencyObject
    {
        if (root is null)
        {
            yield break;
        }

        var childCount = VisualTreeHelper.GetChildrenCount(root);
        for (var index = 0; index < childCount; index++)
        {
            var child = VisualTreeHelper.GetChild(root, index);
            if (child is T match)
            {
                yield return match;
            }

            foreach (var descendant in FindVisualChildren<T>(child))
            {
                yield return descendant;
            }
        }
    }

    private static bool IsDescendantOf(DependencyObject? descendant, DependencyObject? ancestor)
    {
        if (descendant is null || ancestor is null)
        {
            return false;
        }

        for (var current = descendant; current is not null; current = VisualTreeHelper.GetParent(current))
        {
            if (ReferenceEquals(current, ancestor))
            {
                return true;
            }
        }

        return false;
    }

    private static string DescribeDependencyObject(DependencyObject? dependencyObject)
    {
        if (dependencyObject is null)
        {
            return "null";
        }

        if (dependencyObject is FrameworkElement frameworkElement)
        {
            var name = string.IsNullOrWhiteSpace(frameworkElement.Name) ? string.Empty : $"#{frameworkElement.Name}";
            return $"{frameworkElement.GetType().Name}{name}";
        }

        return dependencyObject.GetType().Name;
    }

}
