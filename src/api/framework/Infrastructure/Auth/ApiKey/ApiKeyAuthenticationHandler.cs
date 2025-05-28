using System.Security.Claims;
using System.Text.Encodings.Web;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Infrastructure.Auth.ApiKey;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly Core.Auth.AuthenticationOptions _authOptions;
    public const string SchemeName = "ApiKey";

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<Core.Auth.AuthenticationOptions> authOptions)
        : base(options, logger, encoder)
    {
        _authOptions = authOptions.Value;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!_authOptions.EnableTestAuth || _authOptions.TestAuth?.ApiKeys == null)
        {
            return AuthenticateResult.NoResult();
        }

        if (!Request.Headers.TryGetValue("X-API-Key", out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var apiKey = apiKeyHeaderValues.FirstOrDefault();
        if (string.IsNullOrEmpty(apiKey))
        {
            return AuthenticateResult.NoResult();
        }

        if (!_authOptions.TestAuth.ApiKeys.TryGetValue(apiKey, out var testUser))
        {
            return AuthenticateResult.Fail("Invalid API key");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, testUser.UserId),
            new(ClaimTypes.Email, testUser.Email),
            new(FshClaims.Tenant, testUser.TenantId),
            new("sub", testUser.UserId),
            new("oid", testUser.UserId), // Object ID claim for compatibility
            new("preferred_username", testUser.Email),
        };

        // Add role claims
        foreach (var role in testUser.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        
        Logger.LogInformation("Test user authenticated via API key: {Email} (Tenant: {TenantId})", 
            testUser.Email, testUser.TenantId);
        
        return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));
    }
}

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
}