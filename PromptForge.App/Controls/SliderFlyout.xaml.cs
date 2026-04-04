using System;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Animation;
using PromptForge.App.Services;

namespace PromptForge.App.Controls;

public partial class SliderFlyout : UserControl
{
    private bool _suppressExcludeCheckboxEvents;

    public SliderFlyout()
    {
        InitializeComponent();
    }

    private Storyboard? _buttonGlowStoryboard;
    private Storyboard? _buttonMorphStoryboard;
    private Storyboard? _popupGlowStoryboard;
    private DispatcherTimer? _buttonReturnDelayTimer;

    public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
        nameof(Label), typeof(string), typeof(SliderFlyout), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty HelperTextProperty = DependencyProperty.Register(
        nameof(HelperText), typeof(string), typeof(SliderFlyout), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ValueTextProperty = DependencyProperty.Register(
        nameof(ValueText), typeof(string), typeof(SliderFlyout), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty GuideTextProperty = DependencyProperty.Register(
        nameof(GuideText), typeof(string), typeof(SliderFlyout), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
        nameof(Value), typeof(int), typeof(SliderFlyout), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register(
        nameof(Minimum), typeof(double), typeof(SliderFlyout), new PropertyMetadata(0d));

    public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register(
        nameof(Maximum), typeof(double), typeof(SliderFlyout), new PropertyMetadata(100d));

    public static readonly DependencyProperty ButtonWidthProperty = DependencyProperty.Register(
        nameof(ButtonWidth), typeof(double), typeof(SliderFlyout), new PropertyMetadata(130d));

    public static readonly DependencyProperty ShowExcludeFromPromptProperty = DependencyProperty.Register(
        nameof(ShowExcludeFromPrompt), typeof(bool), typeof(SliderFlyout), new PropertyMetadata(false));

    public static readonly DependencyProperty IsExcludedFromPromptProperty = DependencyProperty.Register(
        nameof(IsExcludedFromPrompt), typeof(bool), typeof(SliderFlyout), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsExcludedFromPromptChanged));

    public string Label
    {
        get => (string)GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public string HelperText
    {
        get => (string)GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    public string ValueText
    {
        get => (string)GetValue(ValueTextProperty);
        set => SetValue(ValueTextProperty, value);
    }

    public string GuideText
    {
        get => (string)GetValue(GuideTextProperty);
        set => SetValue(GuideTextProperty, value);
    }

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public double Minimum
    {
        get => (double)GetValue(MinimumProperty);
        set => SetValue(MinimumProperty, value);
    }

    public double Maximum
    {
        get => (double)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public double ButtonWidth
    {
        get => (double)GetValue(ButtonWidthProperty);
        set => SetValue(ButtonWidthProperty, value);
    }

    public bool ShowExcludeFromPrompt
    {
        get => (bool)GetValue(ShowExcludeFromPromptProperty);
        set => SetValue(ShowExcludeFromPromptProperty, value);
    }

    public bool IsExcludedFromPrompt
    {
        get => (bool)GetValue(IsExcludedFromPromptProperty);
        set => SetValue(IsExcludedFromPromptProperty, value);
    }

    public void CloseFlyout()
    {
        FlyoutButton.IsChecked = false;
    }

    public bool IsFlyoutSessionActive => FlyoutPopup.IsOpen || FlyoutButton.IsChecked == true;

    public void RefreshExcludeBindingFromSource()
    {
        _suppressExcludeCheckboxEvents = true;
        try
        {
            GetBindingExpression(IsExcludedFromPromptProperty)?.UpdateTarget();
            GetBindingExpression(ShowExcludeFromPromptProperty)?.UpdateTarget();
        }
        finally
        {
            Dispatcher.BeginInvoke(() => _suppressExcludeCheckboxEvents = false, DispatcherPriority.Background);
        }
    }

    private void FlyoutButton_OnChecked(object sender, RoutedEventArgs e)
    {
        UiEventLog.Write($"slider-flyout checked label='{Label}' value={Value} excluded={IsExcludedFromPrompt}");
        StartButtonMorphAnimation();
        StartButtonGlowAnimation();
    }

    private void FlyoutButton_OnUnchecked(object sender, RoutedEventArgs e)
    {
        UiEventLog.Write($"slider-flyout unchecked label='{Label}' value={Value} excluded={IsExcludedFromPrompt}");
    }

    private void FlyoutPopup_OnOpened(object sender, EventArgs e)
    {
        UiEventLog.Write($"slider-flyout popup-opened label='{Label}' value={Value} excluded={IsExcludedFromPrompt}");
        StartPopupGlowAnimation();
        QueueButtonReturnAnimation();
    }

    private void FlyoutPopup_OnClosed(object sender, EventArgs e)
    {
        UiEventLog.Write($"slider-flyout popup-closed label='{Label}' value={Value} excluded={IsExcludedFromPrompt}");
        _buttonReturnDelayTimer?.Stop();
        _buttonMorphStoryboard?.Stop();
        _popupGlowStoryboard?.Stop();
        ResetButtonMorphVisuals();
        ResetPopupGlowVisuals();
    }

    private void ExcludeFromPromptCheckBox_OnChecked(object sender, RoutedEventArgs e)
    {
        if (_suppressExcludeCheckboxEvents)
        {
            return;
        }

        UiEventLog.Write($"slider-flyout checkbox-checked label='{Label}' value={Value} excluded={IsExcludedFromPrompt} popupOpen={FlyoutPopup.IsOpen} buttonChecked={FlyoutButton.IsChecked}");
    }

    private void ExcludeFromPromptCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
    {
        if (_suppressExcludeCheckboxEvents)
        {
            return;
        }

        UiEventLog.Write($"slider-flyout checkbox-unchecked label='{Label}' value={Value} excluded={IsExcludedFromPrompt} popupOpen={FlyoutPopup.IsOpen} buttonChecked={FlyoutButton.IsChecked}");
    }

    private void StartButtonGlowAnimation()
    {
        _buttonGlowStoryboard?.Stop();
        ResetButtonGlowVisuals();

        var glowPeakOpacity = GetDouble("SliderButtonGlowPeakOpacity");
        var riseDuration = GetDuration("ButtonGlowRiseDuration");
        var fadeDuration = GetDuration("ButtonGlowFadeDuration");
        var dropShadow = (DropShadowEffect)ButtonGlowOverlay.Effect;

        var storyboard = new Storyboard
        {
            FillBehavior = FillBehavior.Stop
        };

        AddDouble(storyboard, ButtonGlowOverlay, UIElement.OpacityProperty, 0d, glowPeakOpacity, riseDuration);
        AddDouble(storyboard, ButtonGlowOverlay, UIElement.OpacityProperty, glowPeakOpacity, 0d, fadeDuration, riseDuration.TimeSpan);
        AddDouble(storyboard, dropShadow, DropShadowEffect.OpacityProperty, 0d, 0.35d, riseDuration);
        AddDouble(storyboard, dropShadow, DropShadowEffect.OpacityProperty, 0.35d, 0d, fadeDuration, riseDuration.TimeSpan);

        storyboard.Completed += (_, _) => ResetButtonGlowVisuals();
        _buttonGlowStoryboard = storyboard;
        storyboard.Begin();
    }

    private void StartButtonMorphAnimation()
    {
        _buttonReturnDelayTimer?.Stop();
        _buttonMorphStoryboard?.Stop();
        ResetButtonMorphVisuals();

        var pressScale = GetDouble("SliderButtonPressScale");
        var collapseScaleX = GetDouble("SliderButtonCollapseScaleX");
        var collapseScaleY = GetDouble("SliderButtonCollapseScaleY");
        var collapsedOpacity = GetDouble("SliderButtonCollapsedOpacity");
        var pressDuration = GetDuration("SliderButtonPressDuration");
        var collapseDuration = GetDuration("SliderButtonCollapseDuration");
        var collapseBegin = pressDuration.TimeSpan;

        var storyboard = new Storyboard
        {
            FillBehavior = FillBehavior.Stop
        };

        AddDouble(storyboard, FlyoutButtonScaleTransform, ScaleTransform.ScaleXProperty, 1d, pressScale, pressDuration);
        AddDouble(storyboard, FlyoutButtonScaleTransform, ScaleTransform.ScaleYProperty, 1d, pressScale, pressDuration);
        AddDouble(storyboard, FlyoutButtonScaleTransform, ScaleTransform.ScaleXProperty, pressScale, collapseScaleX, collapseDuration, collapseBegin);
        AddDouble(storyboard, FlyoutButtonScaleTransform, ScaleTransform.ScaleYProperty, pressScale, collapseScaleY, collapseDuration, collapseBegin);
        AddDouble(storyboard, FlyoutButton, UIElement.OpacityProperty, 1d, collapsedOpacity, collapseDuration, collapseBegin);

        storyboard.Completed += (_, _) =>
        {
            ApplyCollapsedButtonCamouflage();
            FlyoutButtonScaleTransform.ScaleX = collapseScaleX;
            FlyoutButtonScaleTransform.ScaleY = collapseScaleY;
            FlyoutButton.Opacity = collapsedOpacity;
            FlyoutButton.Visibility = Visibility.Hidden;
        };
        _buttonMorphStoryboard = storyboard;
        storyboard.Begin();
    }

    private void QueueButtonReturnAnimation()
    {
        _buttonReturnDelayTimer?.Stop();

        var holdDuration = GetDuration("SliderButtonHoldUntilPopupReadyDuration").TimeSpan;
        _buttonReturnDelayTimer = new DispatcherTimer
        {
            Interval = holdDuration
        };

        _buttonReturnDelayTimer.Tick += OnButtonReturnDelayTimerTick;
        _buttonReturnDelayTimer.Start();
    }

    private void OnButtonReturnDelayTimerTick(object? sender, EventArgs e)
    {
        if (sender is DispatcherTimer timer)
        {
            timer.Stop();
            timer.Tick -= OnButtonReturnDelayTimerTick;
        }

        _buttonReturnDelayTimer = null;

        if (!FlyoutPopup.IsOpen)
        {
            return;
        }

        StartButtonReturnAnimation();
    }

    private void StartButtonReturnAnimation()
    {
        _buttonMorphStoryboard?.Stop();
        RestoreButtonSkin();

        var returnDuration = GetDuration("SliderButtonReturnDuration");
        var collapseScaleX = GetDouble("SliderButtonCollapseScaleX");
        var collapseScaleY = GetDouble("SliderButtonCollapseScaleY");
        var collapsedOpacity = GetDouble("SliderButtonCollapsedOpacity");
        var returnStartOpacity = GetDouble("SliderButtonReturnStartOpacity");

        var storyboard = new Storyboard
        {
            FillBehavior = FillBehavior.Stop
        };

        AddDouble(storyboard, FlyoutButtonScaleTransform, ScaleTransform.ScaleXProperty, collapseScaleX, 1d, returnDuration);
        AddDouble(storyboard, FlyoutButtonScaleTransform, ScaleTransform.ScaleYProperty, collapseScaleY, 1d, returnDuration);
        AddDouble(storyboard, FlyoutButton, UIElement.OpacityProperty, returnStartOpacity, 1d, returnDuration);

        storyboard.Completed += (_, _) => ResetButtonMorphVisuals();
        _buttonMorphStoryboard = storyboard;
        storyboard.Begin();
    }

    private void StartPopupGlowAnimation()
    {
        _popupGlowStoryboard?.Stop();
        ResetPopupGlowVisuals();

        PopupGlowOverlay.UpdateLayout();

        var overlayWidth = Math.Max(0d, PopupGlowOverlay.ActualWidth - 28d);
        var overlayHeight = Math.Max(0d, PopupGlowOverlay.ActualHeight - 28d);
        if (overlayWidth <= 0d || overlayHeight <= 0d)
        {
            overlayWidth = 272d;
            overlayHeight = 180d;
        }

        var edgeDuration = GetDuration("PopupEdgeTraceDuration");
        var peakOpacity = GetDouble("SliderPopupGlowPeakOpacity");
        var dropShadow = (DropShadowEffect)PopupGlowBorder.Effect;

        var topStart = TimeSpan.Zero;
        var rightStart = edgeDuration.TimeSpan;
        var bottomStart = rightStart + edgeDuration.TimeSpan;
        var leftStart = bottomStart + edgeDuration.TimeSpan;

        var storyboard = new Storyboard
        {
            FillBehavior = FillBehavior.Stop
        };

        AddDouble(storyboard, PopupGlowOverlay, UIElement.OpacityProperty, 0d, peakOpacity, edgeDuration);
        AddDouble(storyboard, dropShadow, DropShadowEffect.OpacityProperty, 0d, 0.26d, edgeDuration);

        AddDouble(storyboard, TopEdgeTrace, FrameworkElement.WidthProperty, 0d, overlayWidth, edgeDuration, topStart);
        AddDouble(storyboard, RightEdgeTrace, FrameworkElement.HeightProperty, 0d, overlayHeight, edgeDuration, rightStart);
        AddDouble(storyboard, BottomEdgeTrace, FrameworkElement.WidthProperty, 0d, overlayWidth, edgeDuration, bottomStart);
        AddDouble(storyboard, LeftEdgeTrace, FrameworkElement.HeightProperty, 0d, overlayHeight, edgeDuration, leftStart);

        storyboard.Completed += (_, _) => FreezePopupGlowVisuals(overlayWidth, overlayHeight, peakOpacity);
        _popupGlowStoryboard = storyboard;
        storyboard.Begin();
    }

    private void ResetButtonGlowVisuals()
    {
        ButtonGlowOverlay.Opacity = 0d;
        if (ButtonGlowOverlay.Effect is DropShadowEffect effect)
        {
            effect.Opacity = 0d;
        }
    }

    private void ResetButtonMorphVisuals()
    {
        RestoreButtonSkin();
        FlyoutButton.Visibility = Visibility.Visible;
        FlyoutButton.Opacity = 1d;
        FlyoutButtonScaleTransform.ScaleX = 1d;
        FlyoutButtonScaleTransform.ScaleY = 1d;
    }

    private void ApplyCollapsedButtonCamouflage()
    {
        FlyoutButton.SetResourceReference(Control.BackgroundProperty, "SliderFlyoutCollapseCamouflageBrush");
        FlyoutButton.SetResourceReference(Control.BorderBrushProperty, "SliderFlyoutCollapseCamouflageBrush");
    }

    private void RestoreButtonSkin()
    {
        FlyoutButton.ClearValue(Control.BackgroundProperty);
        FlyoutButton.ClearValue(Control.BorderBrushProperty);
    }

    private void ResetPopupGlowVisuals()
    {
        PopupGlowOverlay.Opacity = 0d;
        TopEdgeTrace.Width = 0d;
        RightEdgeTrace.Height = 0d;
        BottomEdgeTrace.Width = 0d;
        LeftEdgeTrace.Height = 0d;

        if (PopupGlowBorder.Effect is DropShadowEffect effect)
        {
            effect.Opacity = 0d;
        }
    }

    private void FreezePopupGlowVisuals(double overlayWidth, double overlayHeight, double peakOpacity)
    {
        PopupGlowOverlay.Opacity = peakOpacity;
        TopEdgeTrace.Width = overlayWidth;
        RightEdgeTrace.Height = overlayHeight;
        BottomEdgeTrace.Width = overlayWidth;
        LeftEdgeTrace.Height = overlayHeight;

        if (PopupGlowBorder.Effect is DropShadowEffect effect)
        {
            effect.Opacity = 0.26d;
        }
    }

    private static void AddDouble(
        Storyboard storyboard,
        DependencyObject target,
        DependencyProperty property,
        double from,
        double to,
        Duration duration,
        TimeSpan? beginTime = null)
    {
        var animation = new DoubleAnimation
        {
            From = from,
            To = to,
            Duration = duration,
            BeginTime = beginTime ?? TimeSpan.Zero,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };

        Storyboard.SetTarget(animation, target);
        Storyboard.SetTargetProperty(animation, new PropertyPath(property));
        storyboard.Children.Add(animation);
    }

    private Duration GetDuration(string key)
    {
        return TryFindResource(key) is Duration duration
            ? duration
            : new Duration(TimeSpan.FromMilliseconds(120));
    }

    private double GetDouble(string key)
    {
        return TryFindResource(key) is double value ? value : 1d;
    }

    private static void OnIsExcludedFromPromptChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not SliderFlyout flyout)
        {
            return;
        }

        UiEventLog.Write(
            $"slider-flyout exclude-changed label='{flyout.Label}' old={e.OldValue} new={e.NewValue} value={flyout.Value} popupOpen={flyout.FlyoutPopup.IsOpen} buttonChecked={flyout.FlyoutButton.IsChecked}");
    }
}
