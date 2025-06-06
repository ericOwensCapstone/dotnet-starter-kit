using FSH.Starter.Blazor.Infrastructure.Auth.AzureB2C;
using FSH.Starter.Blazor.Infrastructure.Auth.Jwt;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Starter.Blazor.Infrastructure.Auth;
public static class Extensions
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration config)
    {
        // Register configuration service
        services.AddSingleton<IAuthenticationConfigurationService, AuthenticationConfigurationService>();
        
        // Configure B2C authentication options
        services.Configure<B2CAuthenticationOptions>(options =>
        {
            var b2cConfig = config.GetSection("AuthenticationOptions:AzureAdB2C");
            var instance = b2cConfig["Instance"];
            var domain = b2cConfig["Domain"];
            var policy = b2cConfig["SignUpSignInPolicyId"];
            
            // Construct the B2C authority URL
            options.Authority = $"{instance}/{domain}/{policy}";
            options.ClientId = b2cConfig["ClientId"] ?? string.Empty;
            options.ValidateAuthority = b2cConfig.GetValue<bool>("ValidateAuthority", true);
            
            // Set default scopes
            var apiScope = b2cConfig["ApiScope"];
            options.DefaultScopes = new List<string> { "openid", "offline_access" };
            if (!string.IsNullOrEmpty(apiScope))
            {
                options.DefaultScopes.Add(apiScope);
            }
        });

        // Register B2C authentication services
        services.AddScoped<B2CAuthenticationService>();
        
        // Use B2C as the authentication provider
        services.AddScoped<AuthenticationStateProvider, B2CAuthenticationService>();
        services.AddScoped(sp => (IAuthenticationService)sp.GetRequiredService<AuthenticationStateProvider>());
        services.AddScoped(sp => (IAccessTokenProvider)sp.GetRequiredService<AuthenticationStateProvider>());

        services.AddScoped<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>();
        services.AddScoped<JwtAuthenticationHeaderHandler>();

        services.AddAuthorizationCore(RegisterPermissionClaims);
        services.AddCascadingAuthenticationState();
        return services;
    }


    private static void RegisterPermissionClaims(AuthorizationOptions options)
    {
        foreach (var permission in FshPermissions.All.Select(p => p.Name))
        {
            options.AddPolicy(permission, policy => policy.RequireClaim(FshClaims.Permission, permission));
        }
    }
}
