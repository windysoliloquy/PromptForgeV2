using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PromptForge.App.Controls;

public static class ComboBoxClosedGlintMotion
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(ComboBoxClosedGlintMotion),
            new PropertyMetadata(false, OnIsEnabledChanged));

    private const double MinOffset = 0;
    private const double PixelsPerSecond = 60;

    public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Canvas track)
        {
            return;
        }

        if ((bool)e.NewValue)
        {
            var state = new MotionState(track);
            track.Tag = state;
            track.Loaded += OnLoaded;
            track.Unloaded += OnUnloaded;
        }
        else
        {
            track.Loaded -= OnLoaded;
            track.Unloaded -= OnUnloaded;
            Stop(track);
            track.Tag = null;
        }
    }

    private static void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Canvas track)
        {
            Start(track);
        }
    }

    private static void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (sender is Canvas track)
        {
            Stop(track);
        }
    }

    private static void Start(Canvas track)
    {
        if (track.Tag is not MotionState state)
        {
            state = new MotionState(track);
            track.Tag = state;
        }

        state.LastFrameTime = DateTime.UtcNow;
        CompositionTarget.Rendering -= state.OnRendering;
        CompositionTarget.Rendering += state.OnRendering;
    }

    private static void Stop(Canvas track)
    {
        if (track.Tag is MotionState state)
        {
            CompositionTarget.Rendering -= state.OnRendering;
        }
    }

    private sealed class MotionState(Canvas track)
    {
        private readonly Canvas _track = track;
        private bool _movingForward = true;
        private double _offset;

        public DateTime LastFrameTime { get; set; } = DateTime.UtcNow;

        public void OnRendering(object? sender, EventArgs e)
        {
            var glint = FindChildByName<FrameworkElement>(_track, "ClosedSelectorStaticGlint");
            if (glint is null)
            {
                return;
            }

            if (glint.RenderTransform is not TranslateTransform transform)
            {
                transform = new TranslateTransform();
                glint.RenderTransform = transform;
            }

            var now = DateTime.UtcNow;
            var elapsedSeconds = Math.Max(0, (now - LastFrameTime).TotalSeconds);
            LastFrameTime = now;

            var maxOffset = Math.Max(MinOffset, _track.ActualWidth - glint.ActualWidth);
            var next = _offset + ((_movingForward ? 1 : -1) * PixelsPerSecond * elapsedSeconds);
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

            _offset = Math.Max(MinOffset, Math.Min(maxOffset, next));
            transform.X = _offset;
        }
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
