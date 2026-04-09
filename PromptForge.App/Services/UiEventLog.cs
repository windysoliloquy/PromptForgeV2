using System;
using System.IO;

namespace PromptForge.App.Services;

internal static class UiEventLog
{
    private const string EnableLoggingEnvironmentVariable = "PROMPTFORGE_ENABLE_UI_EVENT_LOG";
    private static readonly object SyncRoot = new();
    private static readonly string LogPath = Path.Combine(AppContext.BaseDirectory, "ui-event-log.txt");

    public static string PathOnDisk => LogPath;

    public static void Reset()
    {
        if (!IsEnabled())
        {
            return;
        }

        lock (SyncRoot)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            File.WriteAllText(LogPath, $"=== UI event log started {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ==={Environment.NewLine}");
        }
    }

    public static void Write(string message)
    {
        if (!IsEnabled())
        {
            return;
        }

        lock (SyncRoot)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            File.AppendAllText(LogPath, $"{DateTime.Now:HH:mm:ss.fff} | {message}{Environment.NewLine}");
        }
    }

    private static bool IsEnabled()
    {
        var value = Environment.GetEnvironmentVariable(EnableLoggingEnvironmentVariable);
        return string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }
}
