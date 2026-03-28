using System.Windows;
using PromptForge.App.Services;
using PromptForge.App.ViewModels;

namespace PromptForge.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var artistProfileService = new ArtistProfileService();
        var promptBuilder = new PromptBuilderService(artistProfileService);
        var presetStorage = new PresetStorageService();
        var clipboardService = new ClipboardService();

        var mainWindow = new MainWindow
        {
            DataContext = new MainWindowViewModel(promptBuilder, presetStorage, clipboardService, artistProfileService)
        };

        MainWindow = mainWindow;
        mainWindow.Show();
    }
}
