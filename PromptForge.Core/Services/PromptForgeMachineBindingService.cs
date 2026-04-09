using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace PromptForge.App.Services;

public static class PromptForgeMachineBindingService
{
    public static string GetCurrentMachineToken()
    {
        var source = string.Join('\n',
            PromptForgeLicenseCodec.ProductName,
            "MachineBindingV1",
            GetStableMachineFingerprintSource(),
            Environment.MachineName.Trim());

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(source));
        return FormatMachineToken(hash.AsSpan(0, 16));
    }

    public static bool IsCurrentMachineToken(string? machineToken)
    {
        var normalizedCandidate = NormalizeMachineToken(machineToken);
        if (string.IsNullOrWhiteSpace(normalizedCandidate))
        {
            return false;
        }

        return string.Equals(
            normalizedCandidate,
            NormalizeMachineToken(GetCurrentMachineToken()),
            StringComparison.Ordinal);
    }

    public static string NormalizeMachineToken(string? machineToken)
    {
        if (string.IsNullOrWhiteSpace(machineToken))
        {
            return string.Empty;
        }

        var builder = new StringBuilder(machineToken.Length);
        foreach (var ch in machineToken)
        {
            if (char.IsLetterOrDigit(ch))
            {
                builder.Append(char.ToUpperInvariant(ch));
            }
        }

        return builder.ToString();
    }

    private static string GetStableMachineFingerprintSource()
    {
        if (OperatingSystem.IsWindows())
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
                if (key?.GetValue("MachineGuid") is string machineGuid && !string.IsNullOrWhiteSpace(machineGuid))
                {
                    return machineGuid.Trim();
                }
            }
            catch
            {
            }
        }

        return Environment.MachineName;
    }

    private static string FormatMachineToken(ReadOnlySpan<byte> bytes)
    {
        var hex = Convert.ToHexString(bytes);
        var groups = new List<string>();
        for (var index = 0; index < hex.Length; index += 4)
        {
            groups.Add(hex.Substring(index, Math.Min(4, hex.Length - index)));
        }

        return $"PF5-{string.Join('-', groups)}";
    }
}
