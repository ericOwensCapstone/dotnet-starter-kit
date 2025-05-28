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
        var entraIdConfig = config.GetSection("EntraExternalId");
        if (entraIdConfig.Exists())
        {
            // Use Entra ID authentication
            services.AddMsalAuthentication(options =>
            {
                // Bind the configuration
                config.Bind("EntraExternalId", options.ProviderOptions.Authentication);
                
                // Configure login mode
                options.ProviderOptions.LoginMode = "redirect";
                
                // Add default scopes
                options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
                options.ProviderOptions.DefaultAccessTokenScopes.Add("profile");
                
                // Add API scopes from configuration
                var apiScopes = config.GetSection("EntraExternalId:ApiScopes").Get<string[]>();
                if (apiScopes != null)
                {
                    foreach (var scope in apiScopes)
                    {
                        options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
                    }
                }
                
                // For Entra External ID, we need to handle the user flow differently
                // The policy/user flow is handled by the service itself, not in the URL
                options.ProviderOptions.Cache.CacheLocation = "localStorage";
                
                // Map additional claims
                options.UserOptions.RoleClaim = "roles";
                options.UserOptions.NameClaim = "name";
            });

            // Register the MSAL authentication service wrapper
            services.AddScoped<IAuthenticationService, MsalAuthenticationService>();
            services.AddScoped<ITenantMappingService, TenantMappingService>();
            services.AddScoped<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>();
            services.AddScoped<JwtAuthenticationHeaderHandler>();
        }
        else
        {
            // Fall back to JWT authentication
            services.AddScoped<AuthenticationStateProvider, JwtAuthenticationService>()
                    .AddScoped(sp => (IAuthenticationService)sp.GetRequiredService<AuthenticationStateProvider>())
                    .AddScoped(sp => (IAccessTokenProvider)sp.GetRequiredService<AuthenticationStateProvider>())
                    .AddScoped<IAccessTokenProviderAccessor, AccessTokenProviderAccessor>()
                    .AddScoped<JwtAuthenticationHeaderHandler>();
        }

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
