using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PromptForge.App.Controls;

public partial class MacroPad : UserControl
{
    public MacroPad()
    {
        InitializeComponent();
        Loaded += (_, _) => UpdateMarkerPosition();
    }

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(MacroPad), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty HelperTextProperty = DependencyProperty.Register(
        nameof(HelperText), typeof(string), typeof(MacroPad), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty HorizontalStartLabelProperty = DependencyProperty.Register(
        nameof(HorizontalStartLabel), typeof(string), typeof(MacroPad), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty HorizontalEndLabelProperty = DependencyProperty.Register(
        nameof(HorizontalEndLabel), typeof(string), typeof(MacroPad), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty VerticalLowLabelProperty = DependencyProperty.Register(
        nameof(VerticalLowLabel), typeof(string), typeof(MacroPad), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty VerticalHighLabelProperty = DependencyProperty.Register(
        nameof(VerticalHighLabel), typeof(string), typeof(MacroPad), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty XValueProperty = DependencyProperty.Register(
        nameof(XValue), typeof(int), typeof(MacroPad), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPadValueChanged));

    public static readonly DependencyProperty YValueProperty = DependencyProperty.Register(
        nameof(YValue), typeof(int), typeof(MacroPad), new FrameworkPropertyMetadata(50, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnPadValueChanged));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string HelperText
    {
        get => (string)GetValue(HelperTextProperty);
        set => SetValue(HelperTextProperty, value);
    }

    public string HorizontalStartLabel
    {
        get => (string)GetValue(HorizontalStartLabelProperty);
        set => SetValue(HorizontalStartLabelProperty, value);
    }

    public string HorizontalEndLabel
    {
        get => (string)GetValue(HorizontalEndLabelProperty);
        set => SetValue(HorizontalEndLabelProperty, value);
    }

    public string VerticalLowLabel
    {
        get => (string)GetValue(VerticalLowLabelProperty);
        set => SetValue(VerticalLowLabelProperty, value);
    }

    public string VerticalHighLabel
    {
        get => (string)GetValue(VerticalHighLabelProperty);
        set => SetValue(VerticalHighLabelProperty, value);
    }

    public int XValue
    {
        get => (int)GetValue(XValueProperty);
        set => SetValue(XValueProperty, value);
    }

    public int YValue
    {
        get => (int)GetValue(YValueProperty);
        set => SetValue(YValueProperty, value);
    }

    private static void OnPadValueChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        ((MacroPad)dependencyObject).UpdateMarkerPosition();
    }

    private void OnSurfaceSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateMarkerPosition();
    }

    private void OnSurfaceMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        var point = e.GetPosition(SurfaceCanvas);
        var width = Math.Max(1d, SurfaceCanvas.ActualWidth);
        var height = Math.Max(1d, SurfaceCanvas.ActualHeight);

        XValue = Math.Clamp((int)Math.Round((point.X / width) * 100d), 0, 100);
        YValue = Math.Clamp((int)Math.Round(100d - ((point.Y / height) * 100d)), 0, 100);
        e.Handled = true;
    }

    private void UpdateMarkerPosition()
    {
        if (SurfaceCanvas.ActualWidth <= 0 || SurfaceCanvas.ActualHeight <= 0)
        {
            return;
        }

        var width = SurfaceCanvas.ActualWidth - Marker.Width;
        var height = SurfaceCanvas.ActualHeight - Marker.Height;
        var left = (XValue / 100d) * width;
        var top = ((100d - YValue) / 100d) * height;

        Canvas.SetLeft(Marker, left);
        Canvas.SetTop(Marker, top);
    }
}
