namespace FSH.Framework.Infrastructure.Configuration;

public class KeyVaultOptions
{
    public const string SectionName = "KeyVault";
    
    public string? VaultUri { get; set; }
    public bool Enabled { get; set; }
    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public bool UseUserSecrets { get; set; } = true; // For local development
}