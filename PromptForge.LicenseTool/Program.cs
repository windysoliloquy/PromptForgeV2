using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using PromptForge.App.Models;
using PromptForge.App.Services;

var options = ParseArguments(args);
if (options.ShowHelp)
{
    PrintUsage();
    return;
}

if (options.GenerateKeyPair)
{
    GenerateKeyPair(options);
    return;
}

var purchaserEmail = ResolvePurchaserEmail(options.Email);
if (string.IsNullOrWhiteSpace(purchaserEmail))
{
    Console.Error.WriteLine("A purchaser email is required.");
    Environment.ExitCode = 1;
    return;
}

var issuedUtc = ResolveIssuedUtc(options.IssuedUtcText);
if (issuedUtc is null)
{
    Console.Error.WriteLine("Could not parse --issued-utc. Use an ISO-8601 UTC timestamp like 2026-03-30T18:00:00Z.");
    Environment.ExitCode = 1;
    return;
}

var licenseId = string.IsNullOrWhiteSpace(options.LicenseId)
    ? CreateLicenseId()
    : options.LicenseId.Trim();

var license = new PromptForgeLicense
{
    ProductName = PromptForgeLicenseCodec.ProductName,
    PurchaserEmail = purchaserEmail.Trim(),
    LicenseId = licenseId,
    IssuedUtc = issuedUtc.Value,
};

var privateKeyPem = ResolvePrivateKeyPem(options.PrivateKeyPath);
license.ValidationToken = PromptForgeLicenseCodec.SignLicense(license, privateKeyPem);

var outputPath = ResolveOutputPath(options.OutputPath, license.LicenseId);
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

var jsonOptions = new JsonSerializerOptions
{
    WriteIndented = true,
};

File.WriteAllText(outputPath, JsonSerializer.Serialize(license, jsonOptions));

Console.WriteLine("Prompt Forge unlock file generated.");
Console.WriteLine($"Email: {license.PurchaserEmail}");
Console.WriteLine($"License ID: {license.LicenseId}");
Console.WriteLine($"Issued UTC: {license.IssuedUtc:O}");
Console.WriteLine($"Output: {outputPath}");

static ToolOptions ParseArguments(string[] args)
{
    var options = new ToolOptions();

    for (var index = 0; index < args.Length; index++)
    {
        var argument = args[index];
        switch (argument)
        {
            case "--help":
            case "-h":
            case "/?":
                options.ShowHelp = true;
                break;
            case "--generate-keypair":
                options.GenerateKeyPair = true;
                break;
            case "--email":
                options.Email = ReadNextValue(args, ref index, argument);
                break;
            case "--license-id":
                options.LicenseId = ReadNextValue(args, ref index, argument);
                break;
            case "--issued-utc":
                options.IssuedUtcText = ReadNextValue(args, ref index, argument);
                break;
            case "--output":
                options.OutputPath = ReadNextValue(args, ref index, argument);
                break;
            case "--private-key":
                options.PrivateKeyPath = ReadNextValue(args, ref index, argument);
                break;
            case "--private-key-output":
                options.PrivateKeyOutputPath = ReadNextValue(args, ref index, argument);
                break;
            case "--public-key-output":
                options.PublicKeyOutputPath = ReadNextValue(args, ref index, argument);
                break;
            default:
                throw new ArgumentException($"Unknown argument: {argument}");
        }
    }

    return options;
}

static string ResolvePurchaserEmail(string? emailFromArgs)
{
    if (!string.IsNullOrWhiteSpace(emailFromArgs))
    {
        return emailFromArgs.Trim();
    }

    Console.Write("Purchaser email: ");
    return Console.ReadLine()?.Trim() ?? string.Empty;
}

static DateTime? ResolveIssuedUtc(string? issuedUtcText)
{
    if (string.IsNullOrWhiteSpace(issuedUtcText))
    {
        return DateTime.UtcNow;
    }

    return DateTime.TryParse(
        issuedUtcText,
        CultureInfo.InvariantCulture,
        DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
        out var parsed)
        ? parsed.ToUniversalTime()
        : null;
}

static string ResolveOutputPath(string? outputPath, string licenseId)
{
    if (!string.IsNullOrWhiteSpace(outputPath))
    {
        return Path.GetFullPath(outputPath.Trim());
    }

    var fileName = $"PromptForge-License-{licenseId}.json";
    return Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, fileName));
}

static string ResolvePrivateKeyPem(string? privateKeyPath)
{
    var resolvedPath = !string.IsNullOrWhiteSpace(privateKeyPath)
        ? Path.GetFullPath(privateKeyPath.Trim())
        : Environment.GetEnvironmentVariable("PROMPTFORGE_LICENSE_PRIVATE_KEY_PATH");

    if (string.IsNullOrWhiteSpace(resolvedPath))
    {
        resolvedPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "tools",
            "licensing",
            "private",
            "license-private-key.pem"));
    }

    if (!File.Exists(resolvedPath))
    {
        throw new FileNotFoundException(
            $"Prompt Forge private key not found at '{resolvedPath}'. Generate one first with --generate-keypair or set PROMPTFORGE_LICENSE_PRIVATE_KEY_PATH.");
    }

    return File.ReadAllText(resolvedPath);
}

static string CreateLicenseId()
{
    return $"PF-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}"[..29];
}

static string ReadNextValue(string[] args, ref int index, string argumentName)
{
    if (index + 1 >= args.Length)
    {
        throw new ArgumentException($"Missing value for {argumentName}.");
    }

    index++;
    return args[index];
}

static void GenerateKeyPair(ToolOptions options)
{
    var privateKeyOutputPath = ResolvePrivateKeyOutputPath(options.PrivateKeyOutputPath);
    var publicKeyOutputPath = ResolvePublicKeyOutputPath(options.PublicKeyOutputPath);

    Directory.CreateDirectory(Path.GetDirectoryName(privateKeyOutputPath)!);
    Directory.CreateDirectory(Path.GetDirectoryName(publicKeyOutputPath)!);

    using var rsa = RSA.Create(3072);
    var privatePem = rsa.ExportPkcs8PrivateKeyPem();
    var publicPem = rsa.ExportSubjectPublicKeyInfoPem();

    File.WriteAllText(privateKeyOutputPath, privatePem);
    File.WriteAllText(publicKeyOutputPath, publicPem);

    Console.WriteLine("Prompt Forge license keypair generated.");
    Console.WriteLine($"Private key: {privateKeyOutputPath}");
    Console.WriteLine($"Public key: {publicKeyOutputPath}");
    Console.WriteLine("Rebuild Prompt Forge.App after updating the public key file.");
}

static string ResolvePrivateKeyOutputPath(string? privateKeyOutputPath)
{
    if (!string.IsNullOrWhiteSpace(privateKeyOutputPath))
    {
        return Path.GetFullPath(privateKeyOutputPath.Trim());
    }

    return Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory,
        "..",
        "..",
        "..",
        "..",
        "tools",
        "licensing",
        "private",
        "license-private-key.pem"));
}

static string ResolvePublicKeyOutputPath(string? publicKeyOutputPath)
{
    if (!string.IsNullOrWhiteSpace(publicKeyOutputPath))
    {
        return Path.GetFullPath(publicKeyOutputPath.Trim());
    }

    return Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory,
        "..",
        "..",
        "..",
        "..",
        "PromptForge.Core",
        "Data",
        "promptforge-license-public.pem"));
}

static void PrintUsage()
{
    Console.WriteLine("Prompt Forge License Tool");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project PromptForge.LicenseTool -- --email buyer@example.com");
    Console.WriteLine("  dotnet run --project PromptForge.LicenseTool -- --generate-keypair");
    Console.WriteLine("  dotnet run --project PromptForge.LicenseTool -- --email buyer@example.com --license-id PF-ORDER-001 --issued-utc 2026-03-30T18:00:00Z --output C:\\licenses\\PromptForge-License-PF-ORDER-001.json");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --generate-keypair   Generate a new RSA keypair. The private key stays local to the seller machine.");
    Console.WriteLine("  --email        Purchaser email. If omitted, the tool prompts for it.");
    Console.WriteLine("  --license-id   Optional custom license id. If omitted, one is generated.");
    Console.WriteLine("  --issued-utc   Optional UTC issue timestamp. Defaults to current UTC.");
    Console.WriteLine("  --output       Optional output path. Defaults to PromptForge-License-<LicenseId>.json in the current directory.");
    Console.WriteLine("  --private-key  Optional private key PEM path. Defaults to tools\\licensing\\private\\license-private-key.pem.");
    Console.WriteLine("  --private-key-output  Optional output path for --generate-keypair.");
    Console.WriteLine("  --public-key-output   Optional output path for --generate-keypair.");
}

file sealed class ToolOptions
{
    public bool GenerateKeyPair { get; set; }
    public string? Email { get; set; }
    public string? LicenseId { get; set; }
    public string? IssuedUtcText { get; set; }
    public string? OutputPath { get; set; }
    public string? PrivateKeyPath { get; set; }
    public string? PrivateKeyOutputPath { get; set; }
    public string? PublicKeyOutputPath { get; set; }
    public bool ShowHelp { get; set; }
}
