using System.Security.Claims;
using FSH.Framework.Core.Auth;
using FSH.Framework.Core.Identity.Tokens;
using FSH.Framework.Core.Identity.Tokens.Models;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;

public static class B2CTokenEndpoint
{
    internal static RouteHandlerBuilder MapB2CTokenEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/b2c/token", async (
                HttpContext context,
                IB2CUserMappingService userMappingService,
                ITokenService tokenService,
                IOptions<FSH.Framework.Core.Auth.AuthenticationOptions> authOptions,
                CancellationToken ct) =>
            {
                try
                {
                    var user = context.User;
                    
                    if (!user.Identity?.IsAuthenticated ?? true)
                    {
                        return Results.Unauthorized();
                    }

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
                    var logger = context.RequestServices.GetRequiredService<ILogger<IB2CUserMappingService>>();
                    logger.LogError(ex, "Error during B2C token exchange");
                    return Results.Problem("Internal server error during token exchange");
                }
            })
            .WithName(nameof(B2CTokenEndpoint))
            .WithSummary("Exchange B2C token for local JWT")
            .WithDescription("After B2C authentication, call this endpoint to get a local JWT with proper permissions")
            .RequireAuthorization()
            .Produces<TokenResponse>();
    }
}