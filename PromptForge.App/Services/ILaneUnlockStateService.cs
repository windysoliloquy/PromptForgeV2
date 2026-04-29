namespace PromptForge.App.Services;

public interface ILaneUnlockStateService
{
    bool IsUnlocked(string? intentMode);
    void Unlock(string? intentMode);
    void MigrateFromLegacyPresetMarkers(
        IReadOnlyDictionary<string, string> legacyUnlockPresetNames,
        IReadOnlyCollection<string> defaultPresetNames);
}
