using System.IO;
using System.Text.Json;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed class PresetStorageService : IPresetStorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _presetDirectory;

    public PresetStorageService()
    {
        _presetDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PromptForge", "Presets");
        Directory.CreateDirectory(_presetDirectory);
    }

    public IReadOnlyList<string> GetPresetNames()
    {
        return Directory.EnumerateFiles(_presetDirectory, "*.json")
            .Select(ReadRecord)
            .Where(record => record is not null)
            .Select(record => record!.Name)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public void Save(string name, PromptConfiguration configuration)
    {
        var record = new PresetRecord
        {
            Name = name.Trim(),
            SavedAtUtc = DateTime.UtcNow,
            Configuration = configuration.Clone(),
        };

        var path = GetPresetPath(record.Name);
        var json = JsonSerializer.Serialize(record, JsonOptions);
        File.WriteAllText(path, json);
    }

    public PromptConfiguration Load(string name)
    {
        var record = ReadRecord(GetPresetPath(name)) ?? throw new FileNotFoundException("Preset not found.", name);
        return record.Configuration.Clone();
    }

    public void Rename(string currentName, string newName)
    {
        var record = ReadRecord(GetPresetPath(currentName)) ?? throw new FileNotFoundException("Preset not found.", currentName);
        Delete(currentName);
        record.Name = newName.Trim();
        record.SavedAtUtc = DateTime.UtcNow;
        File.WriteAllText(GetPresetPath(record.Name), JsonSerializer.Serialize(record, JsonOptions));
    }

    public void Delete(string name)
    {
        var path = GetPresetPath(name);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private PresetRecord? ReadRecord(string path)
    {
        try
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<PresetRecord>(json, JsonOptions);
        }
        catch
        {
            return null;
        }
    }

    private string GetPresetPath(string name)
    {
        var safeName = string.Concat(name.Trim().Select(ch => Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch));
        return Path.Combine(_presetDirectory, $"{safeName}.json");
    }
}
