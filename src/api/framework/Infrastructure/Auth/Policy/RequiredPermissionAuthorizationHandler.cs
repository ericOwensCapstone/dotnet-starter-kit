using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Auth.Policy;
public sealed class RequiredPermissionAuthorizationHandler(IUserService userService, ILogger<RequiredPermissionAuthorizationHandler> logger) : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionAuthorizationRequirement requirement)
    {
        var endpoint = context.Resource switch
        {
            HttpContext httpContext => httpContext.GetEndpoint(),
            Endpoint ep => ep,
            _ => null,
        };

        var requiredPermissions = endpoint?.Metadata.GetMetadata<IRequiredPermissionMetadata>()?.RequiredPermissions;
        if (requiredPermissions == null)
        {
            // there are no permission requirements set by the endpoint
            // hence, authorize requests
            context.Succeed(requirement);
            return;
        }

        var requiredPermission = requiredPermissions.First();
        logger.LogDebug("Checking permission {Permission} for user", requiredPermission);

        // First check if the permission is already in the JWT claims
        // This is important for B2C tokens where permissions are embedded in the token
        var hasPermissionClaim = context.User?.Claims
            .Any(c => c.Type == FshClaims.Permission && c.Value == requiredPermission) ?? false;

        if (hasPermissionClaim)
        {
            logger.LogDebug("Permission {Permission} found in JWT claims", requiredPermission);
            context.Succeed(requirement);
            return;
        }

        // If not in JWT claims, check the database (for locally generated tokens)
        // This maintains backward compatibility with existing local authentication
        if (context.User?.GetUserId() is { } userId)
        {
            logger.LogDebug("Permission {Permission} not in JWT claims, checking database for user {UserId}", requiredPermission, userId);
            if (await userService.HasPermissionAsync(userId, requiredPermission))
            {
                logger.LogDebug("Permission {Permission} found in database for user {UserId}", requiredPermission, userId);
                context.Succeed(requirement);
                return;
            }
        }

        logger.LogWarning("Permission {Permission} denied - not found in JWT claims or database", requiredPermission);
    }
}
