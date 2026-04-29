using PromptForge.App.Models;
using PromptForge.App.Services;

namespace PromptForge.App.ViewModels;

public sealed partial class MainWindowViewModel
{
    public bool IsUnlocked => _licenseService.IsUnlocked;
    public bool IsDemoMode => DemoModeOptions.IsDemoMode && !IsUnlocked;
    public bool IsDemoExpired => IsDemoMode && RemainingDemoCopies <= 0;
    public bool HasLockedLaneAccess => _hasLockedLaneAccess;
    public bool IsLockedLaneActive => RequiresLaneUnlock(IntentMode) && !HasLockedLaneAccess;
    public bool IsVintageBendLocked => IsLockedLaneActive;
    public bool ShowDemoModeBanner => IsDemoMode;
    public bool ShowInteractivePromptPreview => !IsDemoMode && !IsDemoExpired && !IsLockedLaneActive;
    public bool ShowDemoPromptPreview => IsDemoMode && !IsDemoExpired && !IsLockedLaneActive;
    public bool ShowAuthoringWorkspace => !IsDemoExpired;
    public bool ShowDemoExpiredLockScreen => IsDemoExpired;
    public bool ShowLockedLaneAuthoringSections => !IsLockedLaneActive;
    public bool ShowVintageBendAuthoringSections => ShowLockedLaneAuthoringSections;
    public bool ShowLockedLanePane => IsLockedLaneActive && !IsDemoExpired;
    public bool ShowVintageBendLockedPane => ShowLockedLanePane;
    public string VersionButtonText => IsDemoMode ? "How to Unlock" : "Version Info";
    public string DemoModeHeadline => RemainingDemoCopies > 0
        ? $"Demo mode: {RemainingDemoCopies} of {MaxDemoCopies} exports remaining"
        : "Demo mode: export limit reached";
    public string CopyPromptRemainingText => RemainingDemoCopies > 0
        ? $"{RemainingDemoCopies} of {MaxDemoCopies} left"
        : "No exports left";
    public string DemoModeBody => RemainingDemoCopies > 0
        ? "Preview stays readable, but export is limited to the copy buttons."
        : "Demo access has expired. Unlock the full version to restore prompt output and authoring controls.";
    public bool ShowNegativePrompt => UseNegativePrompt && !IsDemoExpired && !IsLockedLaneActive;

    public bool HasLaneAccess(string? intentMode)
    {
        return !RequiresLaneUnlock(intentMode)
            || _laneUnlockStateService.IsUnlocked(intentMode)
            || _licenseService.HasAllowedLane(intentMode);
    }

    private static bool RequiresLaneUnlock(string? intentMode)
    {
        if (string.IsNullOrWhiteSpace(intentMode)
            || string.Equals(intentMode, "Custom", StringComparison.OrdinalIgnoreCase)
            || IntentModeCatalog.IsExperimental(intentMode))
        {
            return false;
        }

        if (BaseUnlockedLaneNames.Contains(intentMode))
        {
            return false;
        }

        return true;
    }

    private static bool TryGetLockedLaneUnlockPresetName(string? intentMode, out string presetName)
    {
        if (!string.IsNullOrWhiteSpace(intentMode)
            && LockedLaneUnlockPresetNames.TryGetValue(intentMode, out var requiredPresetName))
        {
            presetName = requiredPresetName;
            return true;
        }

        presetName = string.Empty;
        return false;
    }

    private static string? GetRequiredUnlockPresetName(string? intentMode)
    {
        return TryGetLockedLaneUnlockPresetName(intentMode, out var presetName)
            ? presetName
            : null;
    }
}
