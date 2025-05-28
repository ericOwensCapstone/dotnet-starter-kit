namespace FSH.Framework.Core.Auth;

public class AuthenticationOptions
{
    public string Mode { get; set; } = "JWT"; // JWT, EntraExternalId
    public EntraExternalIdOptions? EntraExternalId { get; set; }
    public bool EnableTestAuth { get; set; }
    public TestAuthOptions? TestAuth { get; set; }
}

public class EntraExternalIdOptions
{
    public string Instance { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SignUpSignInPolicyId { get; set; } = string.Empty;
}

public class TestAuthOptions
{
    public Dictionary<string, TestUserConfiguration> ApiKeys { get; set; } = new();
}

public class TestUserConfiguration
{
    public string UserId { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string[] Roles { get; set; } = Array.Empty<string>();
}