using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using PromptForge.App.Services;
using PromptForge.App.ViewModels;

namespace PromptForge.App;

public partial class MainWindow : Window
{
    private readonly ILicenseService _licenseService;
    private MainWindowViewModel? _observedViewModel;
    private Storyboard? _activeButtonMorphStoryboard;
    private bool _isDraggingArtistPhraseEditor;
    private Point _artistPhraseEditorDragStart;
    private double _artistPhraseEditorStartHorizontalOffset;
    private double _artistPhraseEditorStartVerticalOffset;

    public MainWindow()
    {
        _licenseService = new LicenseService();
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnWindowLoaded;
        SizeChanged += OnWindowSizeChanged;
    }

    public MainWindow(ILicenseService licenseService)
    {
        _licenseService = licenseService;
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
        Loaded += OnWindowLoaded;
        SizeChanged += OnWindowSizeChanged;
    }

    private void OnVersionInfoClick(object sender, RoutedEventArgs e)
    {
        var unlockWindow = new UnlockWindow(_licenseService, HandleLicenseStateChanged)
        {
            Owner = this,
        };

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
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
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
        ClampArtistPhraseEditorPopupToBounds();
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

}
