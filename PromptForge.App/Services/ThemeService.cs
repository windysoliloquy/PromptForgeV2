using System.IO;
using System.Windows;
using System.Windows.Media;

namespace PromptForge.App.Services;

public sealed class ThemeService : IThemeService
{
    private readonly IReadOnlyDictionary<string, string> _themeSources = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["Studio Light"] = "Styles/Skins/StudioLight.xaml",
        ["Gallery Paper"] = "Styles/Skins/GalleryPaper.xaml",
        ["Blueprint"] = "Styles/Skins/Blueprint.xaml",
        ["Atelier"] = "Styles/Skins/Atelier.xaml",
        ["Midnight Workshop"] = "Styles/Skins/MidnightWorkshop.xaml",
    };

    private static readonly string[] ColorKeys =
    {
        "AppBackgroundColor",
        "CardColor",
        "BorderColor",
        "InputBorderColor",
        "InputBorderHoverColor",
        "InputBorderFocusColor",
        "AccentColor",
        "AccentColorHover",
        "AccentSoftColor",
        "TextPrimaryColor",
        "TextMutedColor",
        "InputBackgroundColor",
        "SubtleSurfaceColor",
        "PopupSurfaceColor",
        "PopupBorderColor",
        "SemanticChipSurfaceColor",
        "SemanticChipBorderColor",
        "ShadowColor",
    };

    private static readonly string[] BrushKeys =
    {
        "AppBackgroundBrush",
        "CardBrush",
        "BorderBrush",
        "AccentBrush",
        "AccentHoverBrush",
        "AccentSoftBrush",
        "TextPrimaryBrush",
        "TextMutedBrush",
        "InputBackgroundBrush",
        "SubtleSurfaceBrush",
        "InputBorderBrush",
        "InputBorderHoverBrush",
        "InputBorderFocusBrush",
        "PopupSurfaceBrush",
        "PopupBorderBrush",
        "SemanticChipSurfaceBrush",
        "SemanticChipBorderBrush",
        "ComboBoxBackgroundBrush",
        "ComboBoxForegroundBrush",
        "ComboBoxDropDownBackgroundBrush",
        "ComboBoxDropDownForegroundBrush",
    };

    private readonly string _settingsPath;

    public ThemeService()
    {
        var settingsDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PromptForge");
        Directory.CreateDirectory(settingsDirectory);
        _settingsPath = Path.Combine(settingsDirectory, "theme.txt");
        CurrentThemeName = LoadPersistedThemeName();
    }

    public IReadOnlyList<string> AvailableThemeNames => _themeSources.Keys.ToList();

    public string CurrentThemeName { get; private set; }

    public void ApplyTheme(string themeName)
    {
        if (!_themeSources.TryGetValue(themeName, out var sourcePath))
        {
            return;
        }

        var skinDictionary = new ResourceDictionary
        {
            Source = new Uri(sourcePath, UriKind.Relative)
        };

        var resources = Application.Current.Resources;

        foreach (var key in ColorKeys)
        {
            if (skinDictionary[key] is Color color)
            {
                resources[key] = color;
            }
        }

        foreach (var key in BrushKeys)
        {
            if (skinDictionary[key] is SolidColorBrush incomingBrush)
            {
                var replacement = new SolidColorBrush(incomingBrush.Color);
                if (incomingBrush.CanFreeze)
                {
                    replacement.Freeze();
                }

                resources[key] = replacement;
            }
        }

        CurrentThemeName = themeName;
        File.WriteAllText(_settingsPath, themeName);
    }

    private string LoadPersistedThemeName()
    {
        try
        {
            if (File.Exists(_settingsPath))
            {
                var persisted = File.ReadAllText(_settingsPath).Trim();
                if (!string.IsNullOrWhiteSpace(persisted) && _themeSources.ContainsKey(persisted))
                {
                    return persisted;
                }
            }
        }
        catch
        {
        }

        return "Midnight Workshop";
    }
}
