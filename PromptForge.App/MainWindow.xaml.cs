using System.Windows;
using PromptForge.App.Services;
using PromptForge.App.ViewModels;

namespace PromptForge.App;

public partial class MainWindow : Window
{
    private readonly ILicenseService _licenseService;

    public MainWindow()
    {
        _licenseService = new LicenseService();
        InitializeComponent();
    }

    public MainWindow(ILicenseService licenseService)
    {
        _licenseService = licenseService;
        InitializeComponent();
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
}
