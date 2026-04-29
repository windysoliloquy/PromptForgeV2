using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public sealed class LicenseService : ILicenseService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
    };

    private readonly string _statePath;
    private UnlockState _currentState;

    public LicenseService()
    {
        var appDataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            DemoModeOptions.LicenseStateDirectoryName);

        Directory.CreateDirectory(appDataDirectory);
        _statePath = Path.Combine(appDataDirectory, "license-state.json");
        _currentState = LoadState();
    }

    public bool IsUnlocked => _currentState.IsUnlocked;
    public UnlockState CurrentState => CloneState(_currentState);
    public bool HasAllowedLane(string? intentMode)
    {
        return !string.IsNullOrWhiteSpace(intentMode)
            && _currentState.AllowedLanes.Any(lane => string.Equals(lane, intentMode.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public string PurchasePrice => "$19.99";
    public string PurchaseEmail => "windysoliloquy@gmail.com";
    public string GetActivationRequestCode() => PromptForgeMachineBindingService.GetCurrentMachineToken();

    public string BuildPurchaseMailtoUri()
    {
        var subject = Uri.EscapeDataString("Prompt Forge Full Purchase");
        var body = Uri.EscapeDataString("Name:\r\nEmail:\r\nI would like to purchase Prompt Forge Full.");
        return $"mailto:{PurchaseEmail}?subject={subject}&body={body}";
    }

    public UnlockImportResult ImportUnlockFile(string filePath)
    {
        var parseResult = TryParseUnlockFile(filePath, out var license, out var parseMessage);
        if (!parseResult)
        {
            return new UnlockImportResult
            {
                Success = false,
                Message = parseMessage,
            };
        }

        if (!PromptForgeLicenseCodec.TryValidate(license, out var validationMessage))
        {
            return new UnlockImportResult
            {
                Success = false,
                Message = validationMessage,
            };
        }

        if (!ValidateMachineBinding(license!, out var machineBindingMessage))
        {
            return new UnlockImportResult
            {
                Success = false,
                Message = machineBindingMessage,
            };
        }

        var incomingLicense = NormalizeLicense(license!);
        var unlockState = MergeUnlockStates(_currentState, incomingLicense);

        try
        {
            SaveState(unlockState);
            _currentState = unlockState;
        }
        catch
        {
            return new UnlockImportResult
            {
                Success = false,
                Message = "Activation could not be saved locally. Prompt Forge remains in its current state.",
            };
        }

        var cleanupSucceeded = TryDestroyImportedFile(filePath);
        return new UnlockImportResult
        {
            Success = true,
            CleanupSucceeded = cleanupSucceeded,
            Message = BuildSuccessfulImportMessage(unlockState, cleanupSucceeded),
        };
    }

    public void Refresh()
    {
        _currentState = LoadState();
    }

    private UnlockState LoadState()
    {
        try
        {
            if (!File.Exists(_statePath))
            {
                return CreateLockedState();
            }

            var json = File.ReadAllText(_statePath);
            var state = JsonSerializer.Deserialize<UnlockState>(json, JsonOptions);
            if (state is null || !state.IsUnlocked)
            {
                return CreateLockedState();
            }

            var validLicenses = GetValidSignedLicenses(state);
            return validLicenses.Count > 0
                ? BuildUnlockStateFromLicenses(validLicenses)
                : CreateLockedState();
        }
        catch
        {
            return CreateLockedState();
        }
    }

    private void SaveState(UnlockState state)
    {
        File.WriteAllText(_statePath, JsonSerializer.Serialize(state, JsonOptions));
    }

    private static bool TryParseUnlockFile(string filePath, out PromptForgeLicense? license, out string message)
    {
        license = null;
        message = string.Empty;

        string rawText;
        try
        {
            rawText = File.ReadAllText(filePath);
        }
        catch
        {
            message = "The selected unlock file could not be read.";
            return false;
        }

        if (TryDeserializeLicense(rawText, out license))
        {
            return true;
        }

        if (TryExtractJsonObject(rawText, out var embeddedJson)
            && TryDeserializeLicense(embeddedJson, out license))
        {
            return true;
        }

        message = "The selected unlock file could not be recognized. If it came from an email, import the attached JSON file directly instead of copying the message body.";
        return false;
    }

    private static bool TryDeserializeLicense(string rawText, out PromptForgeLicense? license)
    {
        try
        {
            var normalized = rawText.Trim().Trim('\uFEFF');
            license = JsonSerializer.Deserialize<PromptForgeLicense>(normalized, JsonOptions);
            return license is not null;
        }
        catch
        {
            license = null;
            return false;
        }
    }

    private static bool TryExtractJsonObject(string rawText, out string json)
    {
        json = string.Empty;

        var start = rawText.IndexOf('{');
        var end = rawText.LastIndexOf('}');
        if (start < 0 || end <= start)
        {
            return false;
        }

        json = rawText[start..(end + 1)];
        return true;
    }

    // Best-effort cleanup only: this adds friction but does not guarantee secure erasure.
    private static bool TryDestroyImportedFile(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                return true;
            }

            var fileInfo = new FileInfo(filePath);
            var length = checked((int)fileInfo.Length);
            var randomBytes = new byte[length];
            RandomNumberGenerator.Fill(randomBytes);
            File.WriteAllBytes(filePath, randomBytes);
            File.Delete(filePath);
            return !File.Exists(filePath);
        }
        catch
        {
            return false;
        }
    }

    private static UnlockState CreateLockedState()
    {
        return new UnlockState
        {
            IsUnlocked = false,
            ProductName = PromptForgeLicenseCodec.ProductName,
        };
    }

    private static UnlockState CloneState(UnlockState state)
    {
        return new UnlockState
        {
            IsUnlocked = state.IsUnlocked,
            ProductName = state.ProductName,
            PurchaserEmail = state.PurchaserEmail,
            LicenseId = state.LicenseId,
            IssuedUtc = state.IssuedUtc,
            LicenseMode = state.LicenseMode,
            MachineToken = state.MachineToken,
            EntitlementProfile = state.EntitlementProfile,
            AllowedLanes = state.AllowedLanes is null ? [] : [.. state.AllowedLanes],
            ValidationToken = state.ValidationToken,
            SignedLicenses = (state.SignedLicenses ?? [])
                .Select(CloneLicense)
                .ToList(),
        };
    }

    private static UnlockState MergeUnlockStates(UnlockState currentState, PromptForgeLicense incomingLicense)
    {
        var signedLicenses = GetValidSignedLicenses(currentState);
        signedLicenses.Add(incomingLicense);

        return BuildUnlockStateFromLicenses(DeduplicateSignedLicenses(signedLicenses));
    }

    private static UnlockState BuildUnlockStateFromLicenses(IReadOnlyList<PromptForgeLicense> signedLicenses)
    {
        var validLicenses = DeduplicateSignedLicenses(signedLicenses);
        if (validLicenses.Count == 0)
        {
            return CreateLockedState();
        }

        var primaryLicense = validLicenses[0];
        return new UnlockState
        {
            IsUnlocked = true,
            ProductName = primaryLicense.ProductName,
            PurchaserEmail = primaryLicense.PurchaserEmail,
            LicenseId = primaryLicense.LicenseId,
            IssuedUtc = primaryLicense.IssuedUtc,
            LicenseMode = primaryLicense.LicenseMode,
            MachineToken = primaryLicense.MachineToken,
            EntitlementProfile = ResolveEntitlementProfile(validLicenses),
            AllowedLanes = validLicenses
                .SelectMany(license => license.AllowedLanes)
                .Where(lane => !string.IsNullOrWhiteSpace(lane))
                .Select(lane => lane.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(lane => lane, StringComparer.OrdinalIgnoreCase)
                .ToList(),
            ValidationToken = primaryLicense.ValidationToken,
            SignedLicenses = validLicenses
                .Select(CloneLicense)
                .ToList(),
        };
    }

    private static List<PromptForgeLicense> GetValidSignedLicenses(UnlockState state)
    {
        var signedLicenses = state.SignedLicenses ?? [];
        var candidateLicenses = signedLicenses.Count > 0
            ? signedLicenses
            : [BuildLicenseFromLegacyState(state)];

        return candidateLicenses
            .Select(NormalizeLicense)
            .Where(IsValidSignedLicense)
            .ToList();
    }

    private static PromptForgeLicense BuildLicenseFromLegacyState(UnlockState state)
    {
        return new PromptForgeLicense
        {
            ProductName = state.ProductName,
            PurchaserEmail = state.PurchaserEmail,
            LicenseId = state.LicenseId,
            IssuedUtc = state.IssuedUtc,
            LicenseMode = state.LicenseMode,
            MachineToken = state.MachineToken,
            EntitlementProfile = state.EntitlementProfile,
            AllowedLanes = state.AllowedLanes is null ? [] : [.. state.AllowedLanes],
            ValidationToken = state.ValidationToken,
        };
    }

    private static List<PromptForgeLicense> DeduplicateSignedLicenses(IEnumerable<PromptForgeLicense> licenses)
    {
        return licenses
            .Select(NormalizeLicense)
            .Where(IsValidSignedLicense)
            .GroupBy(BuildSignedLicenseIdentity, StringComparer.Ordinal)
            .Select(group => group.First())
            .ToList();
    }

    private static string BuildSignedLicenseIdentity(PromptForgeLicense license)
    {
        return !string.IsNullOrWhiteSpace(license.ValidationToken)
            ? license.ValidationToken
            : $"{license.LicenseId}|{license.IssuedUtc:O}|{license.PurchaserEmail}";
    }

    private static bool IsValidSignedLicense(PromptForgeLicense license)
    {
        return PromptForgeLicenseCodec.TryValidate(license, out _)
            && ValidateMachineBinding(license, out _);
    }

    private static PromptForgeLicense NormalizeLicense(PromptForgeLicense license)
    {
        return new PromptForgeLicense
        {
            ProductName = license.ProductName.Trim(),
            PurchaserEmail = license.PurchaserEmail.Trim(),
            LicenseId = license.LicenseId.Trim(),
            IssuedUtc = license.IssuedUtc.ToUniversalTime(),
            LicenseMode = PromptForgeLicenseCodec.GetNormalizedLicenseMode(license),
            MachineToken = PromptForgeMachineBindingService.NormalizeMachineToken(license.MachineToken),
            EntitlementProfile = license.EntitlementProfile.Trim(),
            AllowedLanes = (license.AllowedLanes ?? [])
                .Where(lane => !string.IsNullOrWhiteSpace(lane))
                .Select(lane => lane.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(lane => lane, StringComparer.OrdinalIgnoreCase)
                .ToList(),
            ValidationToken = license.ValidationToken.Trim(),
        };
    }

    private static PromptForgeLicense CloneLicense(PromptForgeLicense license)
    {
        return new PromptForgeLicense
        {
            ProductName = license.ProductName,
            PurchaserEmail = license.PurchaserEmail,
            LicenseId = license.LicenseId,
            IssuedUtc = license.IssuedUtc,
            LicenseMode = license.LicenseMode,
            MachineToken = license.MachineToken,
            EntitlementProfile = license.EntitlementProfile,
            AllowedLanes = license.AllowedLanes is null ? [] : [.. license.AllowedLanes],
            ValidationToken = license.ValidationToken,
        };
    }

    private static string ResolveEntitlementProfile(IReadOnlyList<PromptForgeLicense> signedLicenses)
    {
        return signedLicenses
            .Select(license => license.EntitlementProfile)
            .FirstOrDefault(profile => !string.IsNullOrWhiteSpace(profile)) ?? string.Empty;
    }

    private static bool ValidateMachineBinding(PromptForgeLicense license, out string message)
    {
        if (!PromptForgeLicenseCodec.IsMachineBound(license))
        {
            message = string.Empty;
            return true;
        }

        if (PromptForgeMachineBindingService.IsCurrentMachineToken(license.MachineToken))
        {
            message = string.Empty;
            return true;
        }

        message = "This unlock file is valid, but it was issued for a different machine.";
        return false;
    }

    private static string BuildSuccessfulImportMessage(UnlockState unlockState, bool cleanupSucceeded)
    {
        var modeLead = string.Equals(unlockState.LicenseMode, PromptForgeLicenseModes.MachineBound, StringComparison.Ordinal)
            ? "Activation succeeded. Prompt Forge is now unlocked on this machine."
            : "Activation succeeded. Prompt Forge is now unlocked.";

        if (cleanupSucceeded)
        {
            return modeLead;
        }

        return $"{modeLead} The original unlock file could not be removed automatically.";
    }
}
