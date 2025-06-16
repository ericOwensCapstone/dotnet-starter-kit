using System.Text;
using FSH.Framework.Core.Auth;
using FSH.Framework.Core.Auth.ApiKeys;
using FSH.Framework.Core.Auth.Jwt;
using FSH.Framework.Core.Identity.Tokens;
using FSH.Framework.Infrastructure.Auth.ApiKey;
using FSH.Framework.Infrastructure.Auth.AzureB2C;
using FSH.Framework.Infrastructure.Auth.Jwt;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Framework.Infrastructure.Identity.Tokens;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace FSH.Framework.Infrastructure.Auth;

internal static class Extensions
{
    internal static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure authentication options
        services.AddOptions<FSH.Framework.Core.Auth.AuthenticationOptions>()
            .Bind(configuration.GetSection(FSH.Framework.Core.Auth.AuthenticationOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Configure JWT options (needed for B2C token generation)
        services.AddOptions<JwtOptions>()
            .BindConfiguration(nameof(JwtOptions))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register services
        services.AddScoped<IApiKeyService, ApiKeyService>();
        services.AddScoped<IB2CUserMappingService, B2CUserMappingService>();
        services.AddScoped<ITokenService, TokenService>();

        var authOptions = configuration.GetSection(FSH.Framework.Core.Auth.AuthenticationOptions.SectionName).Get<FSH.Framework.Core.Auth.AuthenticationOptions>() ?? new();

        // Configure authentication schemes
        var authBuilder = services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "MultiAuth";
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        });

        // Add local JWT Bearer for tokens generated after B2C authentication
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
        authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!);

        // Add API Key authentication if enabled
        if (authOptions.ApiKeys?.Enabled == true)
        {
            authBuilder.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationDefaults.AuthenticationScheme, null);
        }

        // Add Azure AD B2C
        if (authOptions.AzureAdB2C != null)
        {
            authBuilder.AddJwtBearer("AzureADB2C", options =>
            {
                var b2cOptions = authOptions.AzureAdB2C;
                
                // Use the policy-specific authority for metadata discovery
                options.Authority = b2cOptions.PolicyAuthority;
                options.MetadataAddress = b2cOptions.OpenIdConfigurationEndpoint;
                options.Audience = b2cOptions.ApiClientId;
                options.RequireHttpsMetadata = true;

                Console.WriteLine($"B2C JWT Config - Authority: {options.Authority}");
                Console.WriteLine($"B2C JWT Config - MetadataAddress: {options.MetadataAddress}");
                Console.WriteLine($"B2C JWT Config - Audience: {options.Audience}");

                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuers = new[] { 
                        "https://ohdharvestadb2c.b2clogin.com/c920b378-6387-46ff-8055-ee74352d8593/v2.0/",
                        "https://ohdharvestadb2c.b2clogin.com/c920b378-6387-46ff-8055-ee74352d8593/v2.0",
                        b2cOptions.Authority + "/", 
                        b2cOptions.Authority,
                        b2cOptions.PolicyAuthority + "/",
                        b2cOptions.PolicyAuthority
                    },
                    ValidAudiences = new[] { b2cOptions.ClientId, b2cOptions.ApiClientId }
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"B2C Authentication failed: {context.Exception?.Message}");
                        Console.WriteLine($"Token: {context.Request.Headers["Authorization"]}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"B2C Challenge triggered: {context.Error} - {context.ErrorDescription}");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine($"B2C Token received: {context.Token?.Substring(0, Math.Min(50, context.Token?.Length ?? 0))}...");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = async context =>
                    {
                        Console.WriteLine("B2C Token validated successfully");
                        Console.WriteLine($"Original claims count: {context.Principal?.Claims.Count() ?? 0}");
                        
                        var userMappingService = context.HttpContext.RequestServices.GetRequiredService<IB2CUserMappingService>();
                        var user = await userMappingService.GetOrCreateUserFromB2CClaimsAsync(context.Principal!);
                        var claims = await userMappingService.GetUserClaimsAsync(user);
                        
                        Console.WriteLine($"Claims after user mapping: {claims.Count}");
                        foreach (var claim in claims.Where(c => c.Type == FshClaims.Permission))
                        {
                            Console.WriteLine($"Permission claim: {claim.Value}");
                        }
                        
                        var identity = new System.Security.Claims.ClaimsIdentity(claims, "AzureADB2C");
                        context.Principal = new System.Security.Claims.ClaimsPrincipal(identity);
                        Console.WriteLine($"B2C User mapped successfully: {user.Email}");
                        Console.WriteLine($"Final principal claims count: {context.Principal.Claims.Count()}");
                    }
                };
            });
        }

        // Add policy scheme to handle multiple authentication methods
        authBuilder.AddPolicyScheme("MultiAuth", "Multi-Auth Scheme", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                // Check for API Key first
                if (authOptions.ApiKeys?.Enabled == true && 
                    context.Request.Headers.ContainsKey(authOptions.ApiKeys.HeaderName))
                {
                    return ApiKeyAuthenticationDefaults.AuthenticationScheme;
                }

                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    
                    // Check if this is a B2C token by looking for B2C-specific claims
                    if (IsB2CToken(token))
                    {
                        return "AzureADB2C";
                    }
                }

                // Default to local JWT Bearer (for tokens generated after B2C auth)
                return JwtBearerDefaults.AuthenticationScheme;
            };
        });

        // Configure authorization
        services.AddAuthorizationBuilder().AddRequiredPermissionPolicy();
        services.AddAuthorization(options =>
        {
            // Apply the fallback policy - but it won't apply to endpoints with AllowAnonymous
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("Bearer", "AzureADB2C", "ApiKey")
                .Build();
            
            // Add a policy for public endpoints
            options.AddPolicy("PublicEndpoint", policy => policy.RequireAssertion(context => true));
        });

        return services;
    }

    private static bool IsB2CToken(string token)
    {
        try
        {
            // Simple check: decode the payload to look for B2C-specific claims
            var parts = token.Split('.');
            if (parts.Length != 3) return false;

            var payload = parts[1];
            // Add padding if needed
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = Encoding.UTF8.GetString(payloadBytes);
            
            // Check for B2C-specific claims
            return payloadJson.Contains("b2clogin.com") || 
                   payloadJson.Contains("tfp") || 
                   payloadJson.Contains("B2C_");
        }
        catch
        {
            return false;
        }
    }
}