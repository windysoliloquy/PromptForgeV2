using System.IO;
using System.Text.Json;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed class PresetStorageService : IPresetStorageService
{
    private const string DefaultSavestateFolderKey = "__default";
    private const string DefaultSavestateFolderDisplayName = "Prompt Forge Default";
    private const string DefaultPresetFolderName = "Presets";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly ISavestateFolderSelectionService _selectionService;
    private readonly string _presetParentDirectory;
    private readonly string _presetDirectory;
    private string _activeSavestateFolderKey = DefaultSavestateFolderKey;

    public PresetStorageService(ISavestateFolderSelectionService selectionService)
    {
        _selectionService = selectionService;
        _presetParentDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "PromptForge");
        _presetDirectory = Path.Combine(_presetParentDirectory, DefaultPresetFolderName);
        Directory.CreateDirectory(_presetDirectory);
        RestorePersistedSelection();
    }

    public IReadOnlyList<PresetSavestateFolder> GetSavestateFolders()
    {
        var folders = new List<PresetSavestateFolder>
        {
            new(DefaultSavestateFolderKey, DefaultSavestateFolderDisplayName, IsDefault: true, CanDelete: false),
        };

        folders.AddRange(
            Directory.EnumerateDirectories(_presetParentDirectory)
                .Where(path => !string.Equals(Path.GetFullPath(path), Path.GetFullPath(_presetDirectory), StringComparison.OrdinalIgnoreCase))
                .Select(path => new DirectoryInfo(path).Name)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .Select(name => new PresetSavestateFolder(name, name, IsDefault: false, CanDelete: true)));

        return folders;
    }

    public PresetSavestateFolder GetActiveSavestateFolder()
    {
        EnsureActiveSavestateFolderIsValid();
        return GetSavestateFolders().First(folder => string.Equals(folder.Key, _activeSavestateFolderKey, StringComparison.OrdinalIgnoreCase));
    }

    public void SelectSavestateFolder(string key)
    {
        var resolvedKey = ResolveSavestateFolderKey(key);
        _activeSavestateFolderKey = resolvedKey;
        _selectionService.SaveSelectedFolderKey(_activeSavestateFolderKey);
    }

    public PresetSavestateFolder CreateSavestateFolder(string name)
    {
        var safeName = SanitizeCustomFolderName(name);
        var path = GetCustomFolderPath(safeName);
        if (Directory.Exists(path) || File.Exists(path))
        {
            throw new InvalidOperationException("A savestate folder with that name already exists.");
        }

        Directory.CreateDirectory(path);
        SelectSavestateFolder(safeName);
        return GetActiveSavestateFolder();
    }

    public void DeleteSavestateFolder(string key)
    {
        var folder = GetSavestateFolders()
            .FirstOrDefault(candidate => string.Equals(candidate.Key, key, StringComparison.OrdinalIgnoreCase));
        if (folder is null)
        {
            return;
        }

        if (folder.IsDefault)
        {
            throw new InvalidOperationException("Prompt Forge Default cannot be deleted.");
        }

        var path = GetCustomFolderPath(folder.Key);
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive: true);
        }

        if (string.Equals(_activeSavestateFolderKey, folder.Key, StringComparison.OrdinalIgnoreCase))
        {
            SelectSavestateFolder(DefaultSavestateFolderKey);
        }
    }

    public IReadOnlyList<string> GetPresetNames()
    {
        return GetPresetNames(GetActivePresetDirectory());
    }

    public IReadOnlyList<string> GetDefaultPresetNames()
    {
        return GetPresetNames(_presetDirectory);
    }

    private IReadOnlyList<string> GetPresetNames(string directory)
    {
        Directory.CreateDirectory(directory);
        return Directory.EnumerateFiles(directory, "*.json")
            .Select(ReadRecord)
            .Where(record => record is not null)
            .Select(record => record!.Name)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public bool PresetNameExists(string name)
    {
        var normalizedCandidate = NormalizePresetName(name);
        if (string.IsNullOrWhiteSpace(normalizedCandidate))
        {
            return false;
        }

        return GetPresetNames().Any(existing => string.Equals(NormalizePresetName(existing), normalizedCandidate, StringComparison.Ordinal));
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
        return Path.Combine(GetActivePresetDirectory(), $"{safeName}.json");
    }

    private static string NormalizePresetName(string? name) => name?.Trim().ToUpperInvariant() ?? string.Empty;

    private void RestorePersistedSelection()
    {
        var persistedKey = _selectionService.LoadSelectedFolderKey();
        _activeSavestateFolderKey = ResolveSavestateFolderKey(persistedKey);
        _selectionService.SaveSelectedFolderKey(_activeSavestateFolderKey);
    }

    private void EnsureActiveSavestateFolderIsValid()
    {
        if (string.Equals(_activeSavestateFolderKey, DefaultSavestateFolderKey, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!Directory.Exists(GetCustomFolderPath(_activeSavestateFolderKey)))
        {
            _activeSavestateFolderKey = DefaultSavestateFolderKey;
            _selectionService.SaveSelectedFolderKey(_activeSavestateFolderKey);
        }
    }

    private string GetActivePresetDirectory()
    {
        EnsureActiveSavestateFolderIsValid();
        return string.Equals(_activeSavestateFolderKey, DefaultSavestateFolderKey, StringComparison.OrdinalIgnoreCase)
            ? _presetDirectory
            : GetCustomFolderPath(_activeSavestateFolderKey);
    }

    private string ResolveSavestateFolderKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key)
            || string.Equals(key, DefaultSavestateFolderKey, StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, DefaultSavestateFolderDisplayName, StringComparison.OrdinalIgnoreCase))
        {
            return DefaultSavestateFolderKey;
        }

        var safeName = SanitizeCustomFolderName(key);
        return Directory.Exists(GetCustomFolderPath(safeName))
            ? safeName
            : DefaultSavestateFolderKey;
    }

    private string GetCustomFolderPath(string key)
    {
        var path = Path.GetFullPath(Path.Combine(_presetParentDirectory, key));
        var parent = Path.GetFullPath(_presetParentDirectory);
        if (!path.StartsWith(parent, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Savestate folder path is outside the preset parent directory.");
        }

        return path;
    }

    private static string SanitizeCustomFolderName(string? name)
    {
        var trimmed = name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(trimmed))
        {
            throw new InvalidOperationException("Enter a savestate folder name first.");
        }

        if (string.Equals(trimmed, DefaultSavestateFolderDisplayName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(trimmed, DefaultPresetFolderName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("That name is reserved for Prompt Forge Default.");
        }

        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = string.Concat(trimmed.Select(ch => invalidChars.Contains(ch) ? '_' : ch)).Trim();
        if (string.IsNullOrWhiteSpace(safeName)
            || string.Equals(safeName, DefaultPresetFolderName, StringComparison.OrdinalIgnoreCase)
            || string.Equals(safeName, DefaultSavestateFolderDisplayName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Enter a valid savestate folder name.");
        }

        return safeName;
    }
}
