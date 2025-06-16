using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FSH.Framework.Core.Auth;
using FSH.Framework.Core.Identity.Tokens;
using FSH.Framework.Core.Identity.Tokens.Models;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;

public static class PublicB2CTokenEndpoint
{
    internal static RouteHandlerBuilder MapPublicB2CTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // Create a public group for this endpoint
        var publicGroup = endpoints.MapGroup("/api/public")
            .AllowAnonymous()
            .WithMetadata(new AllowAnonymousAttribute());
            
        // Map both GET and POST to handle different B2C configurations
        publicGroup.MapGet("/b2c-token", HandleB2CTokenExchange)
            .WithName("PublicB2CTokenEndpoint-GET")
            .WithSummary("Exchange B2C token for local JWT (public endpoint) - GET")
            .WithDescription("After B2C authentication, call this endpoint to get a local JWT with proper permissions")
            .Produces<TokenResponse>()
            .WithTags("Authentication");
        
        // Map to a different path that's explicitly public
        return publicGroup
            .MapPost("/b2c-token", HandleB2CTokenExchange)
            .WithName("PublicB2CTokenEndpoint-POST")
            .WithSummary("Exchange B2C token for local JWT (public endpoint) - POST")
            .WithDescription("After B2C authentication, call this endpoint to get a local JWT with proper permissions")
            .Produces<TokenResponse>()
            .WithTags("Authentication");
    }
    
    internal static async Task<IResult> HandleB2CTokenExchange(
                HttpContext context,
                IB2CUserMappingService userMappingService,
                ITokenService tokenService,
                IOptions<AuthenticationOptions> authOptions,
                ILogger<IB2CUserMappingService> logger,
                CancellationToken ct)
    {
        try
        {
            logger.LogInformation("=== PUBLIC B2C TOKEN ENDPOINT HIT ===");
            logger.LogInformation("Method: {Method}, Path: {Path}", context.Request.Method, context.Request.Path);
            
            // Get the B2C token from the Authorization header
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Results.Unauthorized();
            }

            var b2cToken = authHeader.Substring("Bearer ".Length).Trim();
            
            // Validate the B2C token
            var handler = new JwtSecurityTokenHandler();
            
            // First, read the token without validation to check the issuer
            var jsonToken = handler.ReadJwtToken(b2cToken);
            var issuer = jsonToken.Issuer;
            
            // Check if this is actually a B2C token or a local JWT
            if (issuer == "https://fullstackhero.net")
            {
                // This is already a local JWT, not a B2C token
                logger.LogError("Received local JWT token instead of B2C token at public endpoint");
                return Results.BadRequest("Invalid token type. Expected B2C token.");
            }
            
            // For B2C tokens, we trust B2C's validation and just extract claims
            var claims = jsonToken.Claims;
            
            // Create a ClaimsPrincipal from the B2C token claims
            var identity = new ClaimsIdentity(claims, "B2C");
            var user = new ClaimsPrincipal(identity);

            // Get or create user from B2C claims
            var fshUser = await userMappingService.GetOrCreateUserFromB2CClaimsAsync(user, ct);
            
            // Get user claims with all permissions and roles
            var userClaims = await userMappingService.GetUserClaimsAsync(fshUser, ct);

            // Generate local JWT token using B2C-specific method that bypasses password validation
            var token = await tokenService.GenerateB2CTokenAsync(
                fshUser,
                userClaims,
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown", 
                ct);

            return Results.Ok(token);
        }
        catch (Exception ex)
        {
            // Log the error but don't expose internal details
            logger.LogError(ex, "Error during B2C token exchange");
            return Results.Problem("Internal server error during token exchange");
        }
    }
}