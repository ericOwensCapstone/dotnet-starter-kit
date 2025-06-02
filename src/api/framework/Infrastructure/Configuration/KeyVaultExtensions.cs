using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace FSH.Framework.Infrastructure.Configuration;

public static class KeyVaultExtensions
{
    public static IConfigurationBuilder AddKeyVaultSecrets(
        this IConfigurationBuilder configuration, 
        IHostEnvironment environment)
    {
        var tempConfig = configuration.Build();
        var keyVaultOptions = tempConfig.GetSection(KeyVaultOptions.SectionName).Get<KeyVaultOptions>();

        if (keyVaultOptions?.Enabled == true && !string.IsNullOrEmpty(keyVaultOptions.VaultUri))
        {
            try
            {
                var keyVaultEndpoint = new Uri(keyVaultOptions.VaultUri);
                var credential = GetCredential(keyVaultOptions, environment);
                
                configuration.AddAzureKeyVault(
                    keyVaultEndpoint,
                    credential,
                    new AzureKeyVaultConfigurationOptions
                    {
                        Manager = new FshKeyVaultSecretManager(),
                        ReloadInterval = TimeSpan.FromMinutes(5)
                    });
            }
            catch (Exception ex)
            {
                // Log error but don't fail startup
                Console.WriteLine($"Failed to configure Azure Key Vault: {ex.Message}");
            }
        }

        // Note: User Secrets should be added in the host application's Program.cs
        // since it requires a reference to the host assembly

        return configuration;
    }

    private static Azure.Core.TokenCredential GetCredential(KeyVaultOptions options, IHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            // For local development, use DefaultAzureCredential which tries multiple auth methods:
            // 1. Environment variables
            // 2. Managed Identity
            // 3. Visual Studio / VS Code
            // 4. Azure CLI
            // 5. Azure PowerShell
            // 6. Interactive browser
            return new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ExcludeManagedIdentityCredential = false,
                ExcludeVisualStudioCredential = false,
                ExcludeAzureCliCredential = false,
                ExcludeInteractiveBrowserCredential = false
            });
        }
        else if (!string.IsNullOrEmpty(options.ClientId) && 
                 !string.IsNullOrEmpty(options.ClientSecret) && 
                 !string.IsNullOrEmpty(options.TenantId))
        {
            // For production with service principal
            return new ClientSecretCredential(
                options.TenantId,
                options.ClientId,
                options.ClientSecret);
        }
        else
        {
            // For production with managed identity
            return new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ExcludeInteractiveBrowserCredential = true,
                ExcludeAzureCliCredential = true,
                ExcludeVisualStudioCredential = true,
                ExcludeVisualStudioCodeCredential = true,
                ExcludeAzurePowerShellCredential = true
            });
        }
    }
}

public class FshKeyVaultSecretManager : Azure.Extensions.AspNetCore.Configuration.Secrets.KeyVaultSecretManager
{
    public override string GetKey(KeyVaultSecret secret)
    {
        // Transform Key Vault secret names to configuration keys
        // Example: "ConnectionStrings--DefaultConnection" becomes "ConnectionStrings:DefaultConnection"
        return secret.Name.Replace("--", ":");
    }
}