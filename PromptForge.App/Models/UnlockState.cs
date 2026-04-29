namespace PromptForge.App.Models;

public sealed class UnlockState
{
    public bool IsUnlocked { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string PurchaserEmail { get; set; } = string.Empty;
    public string LicenseId { get; set; } = string.Empty;
    public DateTime IssuedUtc { get; set; }
    public string LicenseMode { get; set; } = string.Empty;
    public string MachineToken { get; set; } = string.Empty;
    public string EntitlementProfile { get; set; } = string.Empty;
    public List<string> AllowedLanes { get; set; } = [];
    public string ValidationToken { get; set; } = string.Empty;
    public List<PromptForgeLicense> SignedLicenses { get; set; } = [];
}
