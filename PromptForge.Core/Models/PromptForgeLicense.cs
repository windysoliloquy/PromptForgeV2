namespace PromptForge.App.Models;

public sealed class PromptForgeLicense
{
    public string ProductName { get; set; } = string.Empty;
    public string PurchaserEmail { get; set; } = string.Empty;
    public string LicenseId { get; set; } = string.Empty;
    public DateTime IssuedUtc { get; set; }
    public string ValidationToken { get; set; } = string.Empty;
}
