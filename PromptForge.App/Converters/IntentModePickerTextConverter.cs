using System;
using System.Globalization;
using System.Windows.Data;
using PromptForge.App.Services;

namespace PromptForge.App.Converters;

public sealed class IntentModePickerTextConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var intentMode = value as string;
        if (string.IsNullOrWhiteSpace(intentMode))
        {
            return string.Empty;
        }

        if (string.Equals(intentMode, IntentModeCatalog.PhotographyName, StringComparison.OrdinalIgnoreCase))
        {
            return "Photography     >";
        }

        return LaneTaxonomyCatalog.TryGetByIntentName(intentMode, out var metadata)
            ? $"{intentMode}  —  {metadata.Category}"
            : intentMode;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
