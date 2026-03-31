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

        if (!VerifyValidationToken(license))
        {
            message = "Unlock file validation failed. Please check the file and try again.";
            return false;
        }

        message = string.Empty;
        return true;
    }

    private static string BuildCanonicalPayload(PromptForgeLicense license)
    {
        return string.Join('\n',
            ProductName,
            license.PurchaserEmail.Trim().ToLowerInvariant(),
            license.LicenseId.Trim(),
            license.IssuedUtc.ToUniversalTime().ToString("O"));
    }

    private static bool VerifyValidationToken(PromptForgeLicense license)
    {
        try
        {
            var signatureBytes = Convert.FromBase64String(license.ValidationToken.Trim());
            var payloadBytes = Encoding.UTF8.GetBytes(BuildCanonicalPayload(license));

            using var rsa = RSA.Create();
            rsa.ImportFromPem(PublicKeyPem.Value);
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
