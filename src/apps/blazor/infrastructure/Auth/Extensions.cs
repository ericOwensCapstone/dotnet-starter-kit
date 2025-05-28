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
                // Manually configure MSAL authentication options
                var entraConfig = config.GetSection("EntraExternalId");
                options.ProviderOptions.Authentication.Authority = entraConfig["Authority"];
                options.ProviderOptions.Authentication.ClientId = entraConfig["ClientId"];
                options.ProviderOptions.Authentication.ValidateAuthority = false;
                
                // Set redirect URIs
                options.ProviderOptions.Authentication.RedirectUri = entraConfig["RedirectUri"];
                options.ProviderOptions.Authentication.PostLogoutRedirectUri = entraConfig["PostLogoutRedirectUri"];
                
                // Configure for B2C/External ID
                options.ProviderOptions.Authentication.ResponseType = "code";
                options.ProviderOptions.LoginMode = "redirect";
                
                // Add API scopes
                var apiScopes = entraConfig.GetSection("ApiScopes").Get<string[]>();
                if (apiScopes != null)
                {
                    foreach (var scope in apiScopes)
                    {
                        options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
                    }
                }
                
                // Map additional claims
                options.UserOptions.RoleClaim = "roles";
            });

            // Register the MSAL authentication service wrapper
            services.AddScoped<IAuthenticationService, MsalAuthenticationService>();
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
