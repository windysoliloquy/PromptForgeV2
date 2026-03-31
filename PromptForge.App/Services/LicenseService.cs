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
            "PromptForgeLocal");

        Directory.CreateDirectory(appDataDirectory);
        _statePath = Path.Combine(appDataDirectory, "license-state.json");
        _currentState = LoadState();
    }

    public bool IsUnlocked => _currentState.IsUnlocked;
    public UnlockState CurrentState => CloneState(_currentState);
    public string PurchasePrice => "$19.99";
    public string PurchaseEmail => "windysoliloquy@gmail.com";

    public string BuildPurchaseMailtoUri()
    {
        var subject = Uri.EscapeDataString("Prompt Forge Full Purchase");
        var body = Uri.EscapeDataString("Name:\r\nEmail:\r\nI would like to purchase Prompt Forge Full.");
        return $"mailto:{PurchaseEmail}?subject={subject}&body={body}";
    }

    public UnlockImportResult ImportUnlockFile(string filePath)
    {
        PromptForgeLicense? license;

        try
        {
            var json = File.ReadAllText(filePath);
            license = JsonSerializer.Deserialize<PromptForgeLicense>(json, JsonOptions);
        }
        catch
        {
            return new UnlockImportResult
            {
                Success = false,
                Message = "The selected unlock file could not be read.",
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

        var unlockState = new UnlockState
        {
            IsUnlocked = true,
            ProductName = license!.ProductName.Trim(),
            PurchaserEmail = license.PurchaserEmail.Trim(),
            LicenseId = license.LicenseId.Trim(),
            IssuedUtc = license.IssuedUtc.ToUniversalTime(),
            ValidationToken = license.ValidationToken.Trim(),
        };

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
            Message = cleanupSucceeded
                ? "Activation succeeded. Prompt Forge Full is now unlocked on this machine."
                : "Activation succeeded. Prompt Forge Full is now unlocked, but the original unlock file could not be removed automatically.",
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

            var license = new PromptForgeLicense
            {
                ProductName = state.ProductName,
                PurchaserEmail = state.PurchaserEmail,
                LicenseId = state.LicenseId,
                IssuedUtc = state.IssuedUtc,
                ValidationToken = state.ValidationToken,
            };

            return PromptForgeLicenseCodec.TryValidate(license, out _)
                ? state
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
            ValidationToken = state.ValidationToken,
        };
    }
}
