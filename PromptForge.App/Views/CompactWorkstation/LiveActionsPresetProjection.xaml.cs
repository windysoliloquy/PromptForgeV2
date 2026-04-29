using System.Windows;
using System.Windows.Controls;

namespace PromptForge.App.Views.CompactWorkstation;

public partial class LiveActionsPresetProjection : UserControl
{
    public LiveActionsPresetProjection()
    {
        InitializeComponent();
    }

    private void OnOpenHoverDeckClick(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.OpenHoverDeckCard(minimizeMainWindow: true);
        }
    }

    private void OnVersionInfoClick(object sender, RoutedEventArgs e)
    {
        if (Application.Current.MainWindow is MainWindow mainWindow)
        {
            mainWindow.ShowVersionInfoDialog();
        }
    }
}
