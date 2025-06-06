namespace FSH.Framework.Core.Auth;

public class AuthenticationOptions
{
    public const string SectionName = "AuthenticationOptions";
    
    public AzureAdB2COptions AzureAdB2C { get; set; } = new();
    public ApiKeyOptions? ApiKeys { get; set; }
}

public class AzureAdB2COptions
{
    public string Instance { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string ApiClientId { get; set; } = string.Empty;
    public string ApiScope { get; set; } = string.Empty;
    public string SignUpSignInPolicyId { get; set; } = string.Empty;
    public string ResetPasswordPolicyId { get; set; } = string.Empty;
    public string EditProfilePolicyId { get; set; } = string.Empty;
    public string CallbackPath { get; set; } = "/signin-oidc";
    public Dictionary<string, string> UserMappings { get; set; } = new();
    
    public string Authority => $"{Instance}/{TenantId}/v2.0";
    public string PolicyAuthority => $"{Instance}/{Domain}/{SignUpSignInPolicyId}/v2.0";
    public string OpenIdConfigurationEndpoint => $"{PolicyAuthority}/.well-known/openid-configuration";
}

public class ApiKeyOptions
{
    public bool Enabled { get; set; } = false;
    public string HeaderName { get; set; } = "X-API-Key";
}