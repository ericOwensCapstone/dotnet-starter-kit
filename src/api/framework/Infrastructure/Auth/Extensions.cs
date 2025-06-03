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

        // Configure JWT options (still needed for local JWT generation)
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

        // Always add JWT Bearer for backward compatibility
        services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();
        authBuilder.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, null!);

        // Add API Key authentication if enabled
        if (authOptions.ApiKeys?.Enabled == true)
        {
            authBuilder.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationDefaults.AuthenticationScheme, null);
        }

        // Add Azure AD B2C if configured for B2C provider
        if (authOptions.Provider == AuthenticationProvider.AzureAdB2C && authOptions.AzureAdB2C != null)
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
                Console.WriteLine($"Policy selector called for path: {context.Request.Path}");
                Console.WriteLine($"Auth provider configured: {authOptions.Provider}");
                
                // Check for API Key first
                if (authOptions.ApiKeys?.Enabled == true && 
                    context.Request.Headers.ContainsKey(authOptions.ApiKeys.HeaderName))
                {
                    Console.WriteLine("Routing to API Key authentication scheme");
                    return ApiKeyAuthenticationDefaults.AuthenticationScheme;
                }

                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"Authorization header: {authHeader?.Substring(0, Math.Min(50, authHeader?.Length ?? 0))}...");
                
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    
                    Console.WriteLine($"Checking if token is B2C. Provider: {authOptions.Provider}");
                    var isB2C = IsPotentialB2CToken(token);
                    Console.WriteLine($"Is B2C token: {isB2C}");
                    
                    // Try to determine if this is a B2C token by checking the issuer
                    if (authOptions.Provider == AuthenticationProvider.AzureAdB2C && isB2C)
                    {
                        Console.WriteLine("Routing to B2C authentication scheme");
                        return "AzureADB2C";
                    }
                }

                Console.WriteLine("Routing to default JWT authentication scheme");
                // Default to JWT Bearer (local authentication)
                return JwtBearerDefaults.AuthenticationScheme;
            };
        });

        // Configure authorization
        services.AddAuthorizationBuilder().AddRequiredPermissionPolicy();
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.GetPolicy(RequiredPermissionDefaults.PolicyName);
        });

        return services;
    }

    private static bool IsPotentialB2CToken(string token)
    {
        try
        {
            Console.WriteLine("Analyzing token to determine if it's B2C...");
            
            // Simple check: B2C tokens typically have 3 parts (header.payload.signature)
            var parts = token.Split('.');
            if (parts.Length != 3) 
            {
                Console.WriteLine($"Token has {parts.Length} parts, not 3");
                return false;
            }

            // Decode the payload to check for B2C-specific claims
            var payload = parts[1];
            
            // Add padding if needed
            switch (payload.Length % 4)
            {
                case 2: payload += "=="; break;
                case 3: payload += "="; break;
            }

            var payloadBytes = Convert.FromBase64String(payload);
            var payloadJson = Encoding.UTF8.GetString(payloadBytes);
            
            Console.WriteLine($"Token payload snippet: {payloadJson.Substring(0, Math.Min(200, payloadJson.Length))}...");
            
            // Check for B2C-specific claims
            var hasB2CLogin = payloadJson.Contains("b2clogin.com");
            var hasTfp = payloadJson.Contains("tfp");
            var hasB2CPolicy = payloadJson.Contains("B2C_");
            
            Console.WriteLine($"B2C indicators - b2clogin.com: {hasB2CLogin}, tfp: {hasTfp}, B2C_: {hasB2CPolicy}");
            
            return hasB2CLogin || hasTfp || hasB2CPolicy;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error analyzing token: {ex.Message}");
            return false;
        }
    }
}