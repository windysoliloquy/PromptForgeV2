using System.Windows;

namespace PromptForge.App.Services;

public sealed class ClipboardService : IClipboardService
{
    public void SetText(string text)
    {
        Clipboard.SetText(text ?? string.Empty);
    }
}
