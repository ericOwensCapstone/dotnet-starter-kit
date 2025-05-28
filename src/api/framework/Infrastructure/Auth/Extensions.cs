using FSH.Framework.Core.Auth;
using FSH.Framework.Infrastructure.Auth.ApiKey;
using FSH.Framework.Infrastructure.Auth.Jwt;
using FSH.Framework.Infrastructure.Auth.Policy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;

namespace FSH.Framework.Infrastructure.Auth;

internal static class Extensions
{
    internal static IServiceCollection ConfigureAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddOptions<AuthenticationOptions>()
            .Bind(config.GetSection("Authentication"))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var authOptions = config.GetSection("Authentication").Get<AuthenticationOptions>() ?? new AuthenticationOptions();

        if (authOptions.Mode == "EntraExternalId")
        {
            services.ConfigureEntraExternalIdAuth(config);
        }
        else
        {
            services.ConfigureJwtAuth();
        }

        // Add API Key authentication for test environments
        if (authOptions.EnableTestAuth)
        {
            services.AddAuthentication()
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    ApiKeyAuthenticationHandler.SchemeName, null);
        }

        services.AddAuthorizationBuilder().AddRequiredPermissionPolicy();
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.GetPolicy(RequiredPermissionDefaults.PolicyName);
        });

        return services;
    }

    private static IServiceCollection ConfigureEntraExternalIdAuth(this IServiceCollection services, IConfiguration config)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddMicrosoftIdentityWebApi(options =>
            {
                config.Bind("Authentication:EntraExternalId", options);
                options.TokenValidationParameters.NameClaimType = "name";
                options.TokenValidationParameters.RoleClaimType = "roles";
            },
            options => 
            { 
                config.Bind("Authentication:EntraExternalId", options); 
            });

        return services;
    }
}