using System.Security.Claims;
using System.Text.Encodings.Web;
using FSH.Framework.Core.Auth;
using FSH.Framework.Core.Auth.ApiKeys;
using FSH.Framework.Core.Tenant.Abstractions;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Infrastructure.Auth.ApiKey;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyService _apiKeyService;
    private readonly ApiKeyOptions _apiKeyOptions;
    private readonly ITenantService _tenantService;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IApiKeyService apiKeyService,
        IOptions<FSH.Framework.Core.Auth.AuthenticationOptions> authOptions,
        ITenantService tenantService)
        : base(options, logger, encoder)
    {
        _apiKeyService = apiKeyService;
        _apiKeyOptions = authOptions.Value.ApiKeys ?? new ApiKeyOptions();
        _tenantService = tenantService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!_apiKeyOptions.Enabled)
        {
            return AuthenticateResult.NoResult();
        }

        if (!Request.Headers.TryGetValue(_apiKeyOptions.HeaderName, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.NoResult();
        }

        var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(providedApiKey))
        {
            return AuthenticateResult.NoResult();
        }

        var validationResult = await _apiKeyService.ValidateKeyAsync(providedApiKey);

        if (!validationResult.IsValid)
        {
            return AuthenticateResult.Fail($"Invalid API Key: {validationResult.FailureReason}");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, $"API Key: {validationResult.KeyName}"),
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new(FshClaims.Tenant, validationResult.TenantId!),
            new(ClaimTypes.AuthenticationMethod, "ApiKey"),
            new("api_key_name", validationResult.KeyName!)
        };

        // Get tenant details to add appropriate claims
        var tenant = await _tenantService.GetByIdAsync(validationResult.TenantId!);

        // Add default permissions for API key authentication (customize as needed)
        claims.Add(new(ClaimTypes.Role, FshRoles.Basic));
        
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}

public static class ApiKeyAuthenticationDefaults
{
    public const string AuthenticationScheme = "ApiKey";
}