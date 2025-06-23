using FSH.Framework.Core.Identity.Users.Dtos;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using FSH.Starter.Shared.Authorization;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;

public static class GetUserRolesAcrossTenantsEndpoint
{
    internal static RouteHandlerBuilder MapGetUserRolesAcrossTenantsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/{userId}/roles-all", async (string userId, TenantFreeDbContext dbContext, CancellationToken cancellationToken) =>
        {
            // First, verify the user exists
            var user = await dbContext.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.Id, TenantId = EF.Property<string>(u, "TenantId") })
                .FirstOrDefaultAsync(cancellationToken);
                
            if (user == null)
            {
                return Results.NotFound(new { message = "User not found" });
            }

            // Get all roles for the user's tenant
            var tenantRoles = await dbContext.Roles
                .Where(r => EF.Property<string>(r, "TenantId") == user.TenantId)
                .ToListAsync(cancellationToken);

            // Get user's role assignments
            var userRoleIds = await dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            var userRoles = tenantRoles.Select(role => new UserRoleDetail
            {
                RoleId = role.Id,
                RoleName = role.Name,
                Description = role.Description,
                Enabled = userRoleIds.Contains(role.Id)
            }).ToList();

            return Results.Ok(userRoles);
        })
        .WithName(nameof(GetUserRolesAcrossTenantsEndpoint))
        .WithSummary("Get user roles across tenants")
        .WithDescription("Gets user roles for any user across all tenants. Requires root admin permissions.")
        .RequirePermission(FshActions.ManageAll, FshResources.Users)
        .ProducesProblem(401)
        .ProducesProblem(403)
        .ProducesProblem(404)
        .Produces<List<UserRoleDetail>>();
    }
}