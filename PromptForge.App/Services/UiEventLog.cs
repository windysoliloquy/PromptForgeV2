using System;
using System.IO;

namespace PromptForge.App.Services;

internal static class UiEventLog
{
    private static readonly object SyncRoot = new();
    private static readonly string LogPath = Path.Combine(AppContext.BaseDirectory, "ui-event-log.txt");

    public static string PathOnDisk => LogPath;

    public static void Reset()
    {
        lock (SyncRoot)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            File.WriteAllText(LogPath, $"=== UI event log started {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} ==={Environment.NewLine}");
        }
    }

    public static void Write(string message)
    {
        lock (SyncRoot)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            File.AppendAllText(LogPath, $"{DateTime.Now:HH:mm:ss.fff} | {message}{Environment.NewLine}");
        }
    }
}
