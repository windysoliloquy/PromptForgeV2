using System;
using System.IO;
using System.Diagnostics;

namespace PromptForge.App.Services;

internal static class UiEventLog
{
    private const string EnableLoggingEnvironmentVariable = "PROMPTFORGE_ENABLE_UI_EVENT_LOG";
    private const string LogScopeEnvironmentVariable = "PROMPTFORGE_UI_EVENT_LOG_SCOPE";
    private const string HoverDeckScope = "hoverdeck";
    private const string HoverDeckSizeScope = "hoverdeck-size";
    private static readonly object SyncRoot = new();
    private static readonly string LogPath = Path.Combine(AppContext.BaseDirectory, "ui-event-log.txt");
    private static readonly string? SharedLogPath = ResolveSharedLogPath();

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
            var header = BuildSessionHeader();
            File.WriteAllText(LogPath, header);
            WriteToSharedLog(header, reset: true);
        }
    }

    public static void Write(string message)
    {
        if (!IsEnabled() || !IsInScope(message))
        {
            return;
        }

        lock (SyncRoot)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
            var line = $"{DateTime.Now:HH:mm:ss.fff} | {message}{Environment.NewLine}";
            File.AppendAllText(LogPath, line);
            WriteToSharedLog(line, reset: false);
        }
    }

    private static bool IsEnabled()
    {
        var value = Environment.GetEnvironmentVariable(EnableLoggingEnvironmentVariable);
        return string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
            || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
    }

    private static string BuildSessionHeader()
    {
        var process = Process.GetCurrentProcess();
        return $"=== UI event log started {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} pid={process.Id} scope='{GetScope()}' baseDir='{AppContext.BaseDirectory}' sharedLog='{SharedLogPath ?? "none"}' ==={Environment.NewLine}";
    }

    private static bool IsInScope(string message)
    {
        var scope = GetScope();
        if (string.IsNullOrWhiteSpace(scope))
        {
            return true;
        }

        if (string.Equals(scope, HoverDeckScope, StringComparison.OrdinalIgnoreCase))
        {
            return message.StartsWith("hoverdeck-", StringComparison.OrdinalIgnoreCase);
        }

        if (string.Equals(scope, HoverDeckSizeScope, StringComparison.OrdinalIgnoreCase))
        {
            return message.StartsWith("hoverdeck-size ", StringComparison.OrdinalIgnoreCase);
        }

        return message.StartsWith($"{scope} ", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetScope() =>
        Environment.GetEnvironmentVariable(LogScopeEnvironmentVariable) ?? string.Empty;

    private static void WriteToSharedLog(string content, bool reset)
    {
        if (string.IsNullOrWhiteSpace(SharedLogPath))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(SharedLogPath)!);
        if (reset)
        {
            File.WriteAllText(SharedLogPath, content);
            return;
        }

        File.AppendAllText(SharedLogPath, content);
    }

    private static string? ResolveSharedLogPath()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "PromptForge.sln")))
            {
                return Path.Combine(directory.FullName, "ui-event-log.shared.txt");
            }

            directory = directory.Parent;
        }

        return null;
    }
}
