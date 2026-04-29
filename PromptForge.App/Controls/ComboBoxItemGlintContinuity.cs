using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PromptForge.App.Controls;

public static class ComboBoxItemGlintContinuity
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(ComboBoxItemGlintContinuity),
            new PropertyMetadata(false, OnIsEnabledChanged));

    private const double MinOffset = 0;
    private const double FallbackMaxOffset = 46;
    private const double RightInset = 12;
    private const double PixelsPerSecond = 60;
    private static double _lastOffset;
    private static bool _movingForward = true;
    private static TranslateTransform? _activeTransform;
    private static FrameworkElement? _activeGlint;
    private static DateTime _lastFrameTime;

    public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not FrameworkElement glint)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            glint.Loaded += OnGlintLoaded;
            glint.Unloaded += OnGlintUnloaded;
        }
        else
        {
            glint.Loaded -= OnGlintLoaded;
            glint.Unloaded -= OnGlintUnloaded;
            StopGlint(glint);
        }
    }

    private static void OnGlintLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement glint)
        {
            return;
        }

        var item = FindAncestor<ComboBoxItem>(glint);
        if (item is null)
        {
            return;
        }

        glint.Tag = item;
        DependencyPropertyDescriptor.FromProperty(ComboBoxItem.IsHighlightedProperty, typeof(ComboBoxItem))
            ?.AddValueChanged(item, OnHighlightedChanged);

        if (item.IsHighlighted)
        {
            StartGlint(glint);
        }
    }

    private static void OnGlintUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement glint)
        {
            return;
        }

        if (glint.Tag is ComboBoxItem item)
        {
            DependencyPropertyDescriptor.FromProperty(ComboBoxItem.IsHighlightedProperty, typeof(ComboBoxItem))
                ?.RemoveValueChanged(item, OnHighlightedChanged);
        }

        glint.Tag = null;
        StopGlint(glint);
    }

    private static void OnHighlightedChanged(object? sender, EventArgs e)
    {
        if (sender is not ComboBoxItem item)
        {
            return;
        }

        var glint = FindChildByName<FrameworkElement>(item, "ItemRowGlint");
        if (glint is null)
        {
            return;
        }

        if (item.IsHighlighted)
        {
            StartGlint(glint);
        }
        else
        {
            StopGlint(glint);
        }
    }

    private static void StartGlint(FrameworkElement glint)
    {
        if (glint.RenderTransform is not TranslateTransform transform)
        {
            transform = new TranslateTransform();
            glint.RenderTransform = transform;
        }

        _lastOffset = Clamp(_lastOffset, GetMaxOffset(glint));
        transform.X = _lastOffset;
        _activeGlint = glint;
        _activeTransform = transform;
        _lastFrameTime = DateTime.UtcNow;
        CompositionTarget.Rendering -= OnRendering;
        CompositionTarget.Rendering += OnRendering;
    }

    private static void StopGlint(FrameworkElement glint)
    {
        if (glint.RenderTransform is TranslateTransform transform)
        {
            _lastOffset = Clamp(transform.X, GetMaxOffset(glint));
            transform.X = _lastOffset;
        }

        if (ReferenceEquals(_activeTransform, glint.RenderTransform))
        {
            _activeTransform = null;
            _activeGlint = null;
            CompositionTarget.Rendering -= OnRendering;
        }
    }

    private static void OnRendering(object? sender, EventArgs e)
    {
        if (_activeTransform is null)
        {
            CompositionTarget.Rendering -= OnRendering;
            return;
        }

        var now = DateTime.UtcNow;
        var elapsedSeconds = Math.Max(0, (now - _lastFrameTime).TotalSeconds);
        _lastFrameTime = now;

        var maxOffset = GetMaxOffset(_activeGlint);
        var next = _lastOffset + ((_movingForward ? 1 : -1) * PixelsPerSecond * elapsedSeconds);
        while (next > maxOffset || next < MinOffset)
        {
            if (next > maxOffset)
            {
                next = maxOffset - (next - maxOffset);
                _movingForward = false;
            }
            else
            {
                next = MinOffset + (MinOffset - next);
                _movingForward = true;
            }
        }

        _lastOffset = Clamp(next, maxOffset);
        _activeTransform.X = _lastOffset;
    }

    private static double Clamp(double value, double maxOffset) => Math.Max(MinOffset, Math.Min(maxOffset, value));

    private static double GetMaxOffset(FrameworkElement? glint)
    {
        if (glint?.Parent is FrameworkElement rowSurface && rowSurface.ActualWidth > 0)
        {
            var available = rowSurface.ActualWidth - glint.Margin.Left - glint.ActualWidth - RightInset;
            return Math.Max(MinOffset, available);
        }

        return FallbackMaxOffset;
    }

    private static T? FindAncestor<T>(DependencyObject current)
        where T : DependencyObject
    {
        while (current is not null)
        {
            if (current is T match)
            {
                return match;
            }

            current = VisualTreeHelper.GetParent(current);
        }

        return null;
    }

    private static T? FindChildByName<T>(DependencyObject parent, string name)
        where T : FrameworkElement
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            if (child is T element && element.Name == name)
            {
                return element;
            }

            var nested = FindChildByName<T>(child, name);
            if (nested is not null)
            {
                return nested;
            }
        }

        return null;
    }
}
