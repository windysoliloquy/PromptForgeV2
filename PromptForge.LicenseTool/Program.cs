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

if (options.ShowRequestCode)
{
    Console.WriteLine(PromptForgeMachineBindingService.GetCurrentMachineToken());
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
    Console.Error.WriteLine("Could not parse --issued-utc. Use an ISO-8601 UTC timestamp like 2026-04-02T18:00:00Z.");
    Environment.ExitCode = 1;
    return;
}

var licenseMode = ResolveLicenseMode(options.ModeText);
if (string.IsNullOrWhiteSpace(licenseMode))
{
    Console.Error.WriteLine("Could not parse --mode. Use Temporary or MachineBound.");
    Environment.ExitCode = 1;
    return;
}

var machineToken = ResolveMachineToken(options, licenseMode);
if (string.Equals(licenseMode, PromptForgeLicenseModes.MachineBound, StringComparison.Ordinal)
    && string.IsNullOrWhiteSpace(machineToken))
{
    Console.Error.WriteLine("MachineBound licenses require --machine-token or --request-code.");
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
    LicenseMode = licenseMode,
    MachineToken = machineToken,
    EntitlementProfile = ResolveEntitlementProfile(options.EntitlementProfile),
    AllowedLanes = ResolveAllowedLanes(options),
};

var privateKeyPem = ResolvePrivateKeyPem(options.PrivateKeyPath);
license.ValidationToken = PromptForgeLicenseCodec.SignLicense(license, privateKeyPem);

var publicKeyPem = ResolvePublicKeyPem(options.PublicKeyPath);
if (!PromptForgeLicenseCodec.TryValidate(license, publicKeyPem, out var validationMessage))
{
    Console.Error.WriteLine("Unlock file generation failed validation.");
    Console.Error.WriteLine(validationMessage);
    Console.Error.WriteLine("Check that the private key and public key belong to the same keypair, then rebuild the app before sending unlock files.");
    Environment.ExitCode = 1;
    return;
}

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
Console.WriteLine($"Mode: {license.LicenseMode}");
Console.WriteLine($"Entitlement: {license.EntitlementProfile}");
if (!string.IsNullOrWhiteSpace(license.MachineToken))
{
    Console.WriteLine($"Machine token: {license.MachineToken}");
}

if (license.AllowedLanes.Count > 0)
{
    Console.WriteLine($"Allowed lanes: {string.Join(", ", license.AllowedLanes)}");
}

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
            case "--show-request-code":
                options.ShowRequestCode = true;
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
            case "--mode":
                options.ModeText = ReadNextValue(args, ref index, argument);
                break;
            case "--machine-token":
            case "--request-code":
                options.MachineToken = ReadNextValue(args, ref index, argument);
                break;
            case "--entitlement-profile":
                options.EntitlementProfile = ReadNextValue(args, ref index, argument);
                break;
            case "--allowed-lane":
                options.AllowedLanes.Add(ReadNextValue(args, ref index, argument));
                break;
            case "--output":
                options.OutputPath = ReadNextValue(args, ref index, argument);
                break;
            case "--private-key":
                options.PrivateKeyPath = ReadNextValue(args, ref index, argument);
                break;
            case "--public-key":
                options.PublicKeyPath = ReadNextValue(args, ref index, argument);
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

static string ResolveLicenseMode(string? modeText)
{
    var normalizedMode = PromptForgeLicenseModes.Normalize(modeText);
    return string.IsNullOrWhiteSpace(normalizedMode)
        ? PromptForgeLicenseModes.Temporary
        : normalizedMode;
}

static string ResolveMachineToken(ToolOptions options, string licenseMode)
{
    if (!string.Equals(licenseMode, PromptForgeLicenseModes.MachineBound, StringComparison.Ordinal))
    {
        return string.Empty;
    }

    return PromptForgeMachineBindingService.NormalizeMachineToken(options.MachineToken);
}

static string ResolveEntitlementProfile(string? entitlementProfile)
{
    return string.IsNullOrWhiteSpace(entitlementProfile)
        ? "Full"
        : entitlementProfile.Trim();
}

static List<string> ResolveAllowedLanes(ToolOptions options)
{
    return options.AllowedLanes
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .Select(value => value.Trim())
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(value => value, StringComparer.OrdinalIgnoreCase)
        .ToList();
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

static string ResolvePublicKeyPem(string? publicKeyPath)
{
    var resolvedPath = !string.IsNullOrWhiteSpace(publicKeyPath)
        ? Path.GetFullPath(publicKeyPath.Trim())
        : Environment.GetEnvironmentVariable("PROMPTFORGE_LICENSE_PUBLIC_KEY_PATH");

    if (string.IsNullOrWhiteSpace(resolvedPath))
    {
        resolvedPath = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "PromptForge.Core",
            "Data",
            "promptforge-license-public.pem"));
    }

    if (!File.Exists(resolvedPath))
    {
        throw new FileNotFoundException(
            $"Prompt Forge public key not found at '{resolvedPath}'. Rebuild Prompt Forge after generating a new keypair, or pass --public-key.");
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
    Console.WriteLine("  dotnet run --project PromptForge.LicenseTool -- --show-request-code");
    Console.WriteLine("  dotnet run --project PromptForge.LicenseTool -- --generate-keypair");
    Console.WriteLine("  dotnet run --project PromptForge.LicenseTool -- --email buyer@example.com --mode MachineBound --request-code PF5-ABCD-EF01-2345-6789 --output C:\\licenses\\PromptForge-License-PF-ORDER-001.json");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --generate-keypair      Generate a new RSA keypair. The private key stays local to the seller machine.");
    Console.WriteLine("  --show-request-code     Print the current machine's activation request code.");
    Console.WriteLine("  --email                 Purchaser email. If omitted, the tool prompts for it.");
    Console.WriteLine("  --license-id            Optional custom license id. If omitted, one is generated.");
    Console.WriteLine("  --issued-utc            Optional UTC issue timestamp. Defaults to current UTC.");
    Console.WriteLine("  --mode                  Optional license mode. Defaults to Temporary. Valid values: Temporary, MachineBound.");
    Console.WriteLine("  --machine-token         Machine token for MachineBound unlocks.");
    Console.WriteLine("  --request-code          Alias for --machine-token.");
    Console.WriteLine("  --entitlement-profile   Optional entitlement label. Defaults to Full.");
    Console.WriteLine("  --allowed-lane          Optional lane id. Repeat to include multiple lanes.");
    Console.WriteLine("  --output                Optional output path. Defaults to PromptForge-License-<LicenseId>.json in the current directory.");
    Console.WriteLine("  --private-key           Optional private key PEM path. Defaults to tools\\licensing\\private\\license-private-key.pem.");
    Console.WriteLine("  --public-key            Optional public key PEM path used for a self-check. Defaults to PromptForge.Core\\Data\\promptforge-license-public.pem.");
    Console.WriteLine("  --private-key-output    Optional output path for --generate-keypair.");
    Console.WriteLine("  --public-key-output     Optional output path for --generate-keypair.");
}

file sealed class ToolOptions
{
    public bool GenerateKeyPair { get; set; }
    public bool ShowRequestCode { get; set; }
    public string? Email { get; set; }
    public string? LicenseId { get; set; }
    public string? IssuedUtcText { get; set; }
    public string? ModeText { get; set; }
    public string? MachineToken { get; set; }
    public string? EntitlementProfile { get; set; }
    public List<string> AllowedLanes { get; } = [];
    public string? OutputPath { get; set; }
    public string? PrivateKeyPath { get; set; }
    public string? PublicKeyPath { get; set; }
    public string? PrivateKeyOutputPath { get; set; }
    public string? PublicKeyOutputPath { get; set; }
    public bool ShowHelp { get; set; }
}
