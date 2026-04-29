namespace PromptForge.App.Services;

public static class DemoModeOptions
{
    public static bool IsDemoMode { get; } = true;
    public const int MaxDemoCopies = 30;
    public static string LicenseStateDirectoryName => IsDemoMode ? "PromptForgeDemo" : "PromptForgeLocal";
}
