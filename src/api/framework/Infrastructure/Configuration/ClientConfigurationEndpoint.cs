using Carter;
using FSH.Framework.Core.Auth;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Infrastructure.Auth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Infrastructure.Configuration;

public class ClientConfigurationEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/configuration")
            .WithTags("Configuration");

        group.MapGet("client", GetClientConfiguration)
            .WithName("GetClientConfiguration")
            .WithSummary("Get client configuration")
            .WithDescription("Returns non-sensitive configuration needed by the Blazor client")
            .AllowAnonymous()
            .Produces<ClientConfiguration>();
    }

    private static IResult GetClientConfiguration(
        IConfiguration configuration,
        IOptions<AuthenticationOptions> authOptions)
    {
        var authConfig = authOptions.Value;
        var azureB2COptions = authConfig.AzureAdB2C;
        
        var clientConfig = new ClientConfiguration
        {
            Authentication = new ClientAuthenticationConfiguration
            {
                Provider = "AzureAdB2C",
                AzureAdB2C = azureB2COptions != null ? new ClientAzureAdB2CConfiguration
                {
                    Enabled = true,
                    TenantId = azureB2COptions.TenantId,
                    ClientId = azureB2COptions.ClientId,
                    Instance = azureB2COptions.Instance,
                    Domain = azureB2COptions.Domain,
                    SignUpSignInPolicyId = azureB2COptions.SignUpSignInPolicyId,
                    ResetPasswordPolicyId = azureB2COptions.ResetPasswordPolicyId,
                    EditProfilePolicyId = azureB2COptions.EditProfilePolicyId,
                    ApiScope = azureB2COptions.ApiScope,
                    // Note: We do NOT include ClientSecret here
                } : null
            },
            ApiBaseUrl = configuration["ApiBaseUrl"] ?? "/",
            ApplicationName = configuration["ApplicationName"] ?? "FSH Starter Kit"
        };

        return Results.Ok(clientConfig);
    }
}

public class ClientConfiguration
{
    public ClientAuthenticationConfiguration Authentication { get; set; } = new();
    public string ApiBaseUrl { get; set; } = "/";
    public string ApplicationName { get; set; } = "FSH Starter Kit";
}

public class ClientAuthenticationConfiguration
{
    public string Provider { get; set; } = "AzureAdB2C";
    public ClientAzureAdB2CConfiguration? AzureAdB2C { get; set; }
}

public class ClientAzureAdB2CConfiguration
{
    public bool Enabled { get; set; }
    public string? TenantId { get; set; }
    public string? ClientId { get; set; }
    public string? Instance { get; set; }
    public string? Domain { get; set; }
    public string? SignUpSignInPolicyId { get; set; }
    public string? ResetPasswordPolicyId { get; set; }
    public string? EditProfilePolicyId { get; set; }
    public string? ApiScope { get; set; }
}