using System.Security.Cryptography;
using System.Text;
using System.Reflection;
using PromptForge.App.Models;

namespace PromptForge.App.Services;

public static class PromptForgeLicenseCodec
{
    public const string ProductName = "Prompt Forge";
    private const string PublicKeyResourceName = "PromptForge.App.Data.promptforge-license-public.pem";
    private static readonly Lazy<string> PublicKeyPem = new(LoadPublicKeyPem);

    public static string SignLicense(PromptForgeLicense license, string privateKeyPem)
    {
        var payload = BuildCanonicalPayload(license);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyPem);
        var signature = rsa.SignData(payloadBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signature);
    }

    public static bool TryValidate(PromptForgeLicense? license, out string message)
    {
        return TryValidate(license, PublicKeyPem.Value, out message);
    }

    public static bool TryValidate(PromptForgeLicense? license, string publicKeyPem, out string message)
    {
        if (license is null)
        {
            message = "The selected unlock file is empty or invalid.";
            return false;
        }

        if (!string.Equals(license.ProductName?.Trim(), ProductName, StringComparison.Ordinal))
        {
            message = "Unlock file validation failed. Please check the file and try again.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(license.PurchaserEmail)
            || string.IsNullOrWhiteSpace(license.LicenseId)
            || license.IssuedUtc == default
            || string.IsNullOrWhiteSpace(license.ValidationToken))
        {
            message = "Unlock file validation failed. Please check the file and try again.";
            return false;
        }

        if (VerifyValidationToken(BuildCanonicalPayload(license), license.ValidationToken, publicKeyPem))
        {
            if (!HasValidCurrentSchema(license))
            {
                message = "Unlock file validation failed. Please check the file and try again.";
                return false;
            }

            message = string.Empty;
            return true;
        }

        if (IsLegacyLicenseShape(license) && VerifyValidationToken(BuildLegacyCanonicalPayload(license), license.ValidationToken, publicKeyPem))
        {
            message = string.Empty;
            return true;
        }

        message = "Unlock file validation failed. Please check the file and try again.";
        return false;
    }

    public static bool IsMachineBound(PromptForgeLicense? license)
    {
        return string.Equals(PromptForgeLicenseModes.Normalize(license?.LicenseMode), PromptForgeLicenseModes.MachineBound, StringComparison.Ordinal);
    }

    public static string GetNormalizedLicenseMode(PromptForgeLicense? license)
    {
        if (license is null)
        {
            return string.Empty;
        }

        var normalizedMode = PromptForgeLicenseModes.Normalize(license.LicenseMode);
        return string.IsNullOrWhiteSpace(normalizedMode) && IsLegacyLicenseShape(license)
            ? PromptForgeLicenseModes.Temporary
            : normalizedMode;
    }

    private static bool HasValidCurrentSchema(PromptForgeLicense license)
    {
        var normalizedMode = PromptForgeLicenseModes.Normalize(license.LicenseMode);
        if (string.IsNullOrWhiteSpace(normalizedMode))
        {
            return IsLegacyLicenseShape(license);
        }

        if (!string.Equals(normalizedMode, PromptForgeLicenseModes.Temporary, StringComparison.Ordinal)
            && !string.Equals(normalizedMode, PromptForgeLicenseModes.MachineBound, StringComparison.Ordinal))
        {
            return false;
        }

        if (string.Equals(normalizedMode, PromptForgeLicenseModes.MachineBound, StringComparison.Ordinal)
            && string.IsNullOrWhiteSpace(PromptForgeMachineBindingService.NormalizeMachineToken(license.MachineToken)))
        {
            return false;
        }

        return true;
    }

    private static string BuildCanonicalPayload(PromptForgeLicense license)
    {
        return string.Join('\n',
            ProductName,
            license.PurchaserEmail.Trim().ToLowerInvariant(),
            license.LicenseId.Trim(),
            license.IssuedUtc.ToUniversalTime().ToString("O"),
            PromptForgeLicenseModes.Normalize(license.LicenseMode),
            PromptForgeMachineBindingService.NormalizeMachineToken(license.MachineToken),
            license.EntitlementProfile.Trim(),
            BuildAllowedLanePayload(license.AllowedLanes));
    }

    private static string BuildLegacyCanonicalPayload(PromptForgeLicense license)
    {
        return string.Join('\n',
            ProductName,
            license.PurchaserEmail.Trim().ToLowerInvariant(),
            license.LicenseId.Trim(),
            license.IssuedUtc.ToUniversalTime().ToString("O"));
    }

    private static string BuildAllowedLanePayload(IReadOnlyCollection<string>? allowedLanes)
    {
        if (allowedLanes is null || allowedLanes.Count == 0)
        {
            return string.Empty;
        }

        return string.Join('|', allowedLanes
            .Where(lane => !string.IsNullOrWhiteSpace(lane))
            .Select(lane => lane.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(lane => lane, StringComparer.OrdinalIgnoreCase));
    }

    private static bool IsLegacyLicenseShape(PromptForgeLicense license)
    {
        return string.IsNullOrWhiteSpace(license.LicenseMode)
            && string.IsNullOrWhiteSpace(license.MachineToken)
            && string.IsNullOrWhiteSpace(license.EntitlementProfile)
            && (license.AllowedLanes?.Count ?? 0) == 0;
    }

    private static bool VerifyValidationToken(string payload, string validationToken, string publicKeyPem)
    {
        try
        {
            var signatureBytes = Convert.FromBase64String(validationToken.Trim());
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            return rsa.VerifyData(payloadBytes, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }

    private static string LoadPublicKeyPem()
    {
        var assembly = typeof(PromptForgeLicenseCodec).Assembly;
        using var stream = assembly.GetManifestResourceStream(PublicKeyResourceName);
        if (stream is null)
        {
            throw new InvalidOperationException($"Embedded license public key resource '{PublicKeyResourceName}' was not found.");
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var pem = reader.ReadToEnd().Trim();
        if (pem.Contains("REPLACE_WITH_GENERATED_PROMPTFORGE_LICENSE_PUBLIC_KEY", StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Prompt Forge license public key has not been generated yet.");
        }

        return pem;
    }
}
