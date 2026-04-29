using System.Diagnostics;
using System.Windows;

namespace PromptForge.App;

public partial class ImageGalleryVisitPromptWindow : Window
{
    private const string ImageGalleryUrl = "https://community.openai.com/t/april-2026-chatgpt-api-image-gallery-prompt-tips-and-help-generative-art-theme-spring-new-beginnings/1378298";

    public ImageGalleryVisitPromptWindow()
    {
        InitializeComponent();
    }

    public static void ShowFor(Window? owner)
    {
        var promptWindow = new ImageGalleryVisitPromptWindow();
        if (owner?.IsVisible == true)
        {
            promptWindow.Owner = owner;
        }

        promptWindow.ShowDialog();
    }

    private void OnYesClick(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo(ImageGalleryUrl)
        {
            UseShellExecute = true,
        });

        Close();
    }

    private void OnNoClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
