using System.Windows;
using System.Windows.Controls;

namespace PromptForge.App.Views.CompactWorkstation;

public partial class HoverDeckCompactConsoleCard : UserControl
{
    public static readonly DependencyProperty IsHoverDeckExperimentalCompressionEnabledProperty =
        DependencyProperty.Register(
            nameof(IsHoverDeckExperimentalCompressionEnabled),
            typeof(bool),
            typeof(HoverDeckCompactConsoleCard),
            new PropertyMetadata(true));

    public HoverDeckCompactConsoleCard()
    {
        InitializeComponent();
    }

    public bool IsHoverDeckExperimentalCompressionEnabled
    {
        get => (bool)GetValue(IsHoverDeckExperimentalCompressionEnabledProperty);
        set => SetValue(IsHoverDeckExperimentalCompressionEnabledProperty, value);
    }

    private void OnVersionInfoClick(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.ShowVersionInfoDialog(Window.GetWindow(this));
        }
    }

    private void OnBrandPromptForgeClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        ImageGalleryVisitPromptWindow.ShowFor(Window.GetWindow(this));
        e.Handled = true;
    }
}
