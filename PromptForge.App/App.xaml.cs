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
        var artistPairGuidanceService = new ArtistPairGuidanceService();
        var promptBuilder = new PromptBuilderService(artistProfileService);
        var savestateFolderSelectionService = new SavestateFolderSelectionService();
        var presetStorage = new PresetStorageService(savestateFolderSelectionService);
        var clipboardService = new ClipboardService();
        var themeService = new ThemeService();
        var demoStateService = new DemoStateService();
        var licenseService = new LicenseService();
        var laneUnlockStateService = new LaneUnlockStateService();
        themeService.ApplyTheme(themeService.CurrentThemeName);

        var viewModel = new MainWindowViewModel(promptBuilder, presetStorage, clipboardService, artistProfileService, artistPairGuidanceService, themeService, demoStateService, licenseService, laneUnlockStateService);
        var mainWindow = new MainWindow(licenseService)
        {
            DataContext = viewModel
        };

        MainWindow = mainWindow;
        mainWindow.OpenHoverDeckCard(minimizeMainWindow: true);
    }
}


