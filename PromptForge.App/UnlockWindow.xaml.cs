using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using PromptForge.App.Services;

namespace PromptForge.App;

public partial class UnlockWindow : Window
{
    private readonly ILicenseService _licenseService;
    private readonly Action _onLicenseStateChanged;

    public UnlockWindow(ILicenseService licenseService, Action onLicenseStateChanged)
    {
        _licenseService = licenseService;
        _onLicenseStateChanged = onLicenseStateChanged;
        InitializeComponent();
        RefreshStatus();
    }

    private void OnEmailToPurchaseClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Process.Start(new ProcessStartInfo(_licenseService.BuildPurchaseMailtoUri())
            {
                UseShellExecute = true,
            });
        }
        catch
        {
            MessageBox.Show(this, "Prompt Forge could not open your default mail client.", "Prompt Forge", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void OnImportUnlockFileClick(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Import Prompt Forge Unlock File",
            Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
            Multiselect = false,
        };

        if (dialog.ShowDialog(this) != true)
        {
            return;
        }

        var result = _licenseService.ImportUnlockFile(dialog.FileName);
        MessageBox.Show(this, result.Message, "Prompt Forge", MessageBoxButton.OK, result.Success ? MessageBoxImage.Information : MessageBoxImage.Warning);

        if (!result.Success)
        {
            return;
        }

        _licenseService.Refresh();
        _onLicenseStateChanged();
        RefreshStatus();
    }

    private void OnCloseClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void RefreshStatus()
    {
        var state = _licenseService.CurrentState;
        var isFullVersion = !DemoModeOptions.IsDemoMode || _licenseService.IsUnlocked;

        StatusHeadlineTextBlock.Text = isFullVersion
            ? "Version: Full"
            : "Version: Demo";

        StatusBodyTextBlock.Text = isFullVersion
            ? "Prompt Forge Full is active on this machine."
            : "Demo mode is active.";

        LicenseSummaryTextBlock.Text = _licenseService.IsUnlocked
            ? $"Unlocked for {state.PurchaserEmail}.{Environment.NewLine}License ID: {state.LicenseId}{Environment.NewLine}Issued: {state.IssuedUtc.ToLocalTime():f}"
            : isFullVersion
                ? "This build is already running as the full version."
                : "No license imported yet.";
    }
}
