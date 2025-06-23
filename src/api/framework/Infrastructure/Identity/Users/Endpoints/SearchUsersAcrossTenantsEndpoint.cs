using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using FSH.Starter.Shared.Authorization;
using FSH.Framework.Core.Identity.Users.Dtos;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;

public static class SearchUsersAcrossTenantsEndpoint
{
    internal static RouteHandlerBuilder MapSearchUsersAcrossTenantsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapGet("/search-all", async (string? searchTerm, int? pageSize, TenantFreeDbContext dbContext, CancellationToken cancellationToken) =>
        {
            var actualPageSize = pageSize ?? 10;
            if (actualPageSize <= 0 || actualPageSize > 100)
            {
                actualPageSize = 10;
            }

            var query = dbContext.Users.AsQueryable();
            
            if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm.Length >= 2)
            {
                var searchLower = searchTerm.ToLower();
                query = query.Where(u => 
                    u.Email.ToLower().Contains(searchLower) ||
                    u.FirstName.ToLower().Contains(searchLower) ||
                    u.LastName.ToLower().Contains(searchLower) ||
                    u.UserName.ToLower().Contains(searchLower));
            }
            
            var users = await query
                .OrderBy(u => u.Email)
                .Take(actualPageSize)
                .Select(u => new UserWithTenantDetail
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    UserName = u.UserName,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.IsActive,
                    EmailConfirmed = u.EmailConfirmed,
                    TenantId = EF.Property<string>(u, "TenantId")
                })
                .ToListAsync(cancellationToken);

            // For each user, get their tenant name
            foreach (var user in users.Where(u => !string.IsNullOrEmpty(u.TenantId)))
            {
                var tenant = await dbContext.Tenants
                    .Where(t => t.Id == user.TenantId)
                    .Select(t => new { t.Name })
                    .FirstOrDefaultAsync(cancellationToken);
                
                if (tenant != null)
                {
                    user.TenantName = tenant.Name;
                }
            }
                
            return Results.Ok(users);
        })
        .WithName(nameof(SearchUsersAcrossTenantsEndpoint))
        .WithSummary("Search users across all tenants")
        .WithDescription("Searches for users across all tenants. Requires root admin permissions.")
        .RequirePermission(FshActions.ManageAll, FshResources.Users)
        .ProducesProblem(401)
        .ProducesProblem(403)
        .Produces<List<UserWithTenantDetail>>()
        .WithOpenApi(operation =>
        {
            operation.Parameters[0].Description = "Search term (minimum 2 characters)";
            operation.Parameters[1].Description = "Page size (default: 10, max: 100)";
            return operation;
        });
    }
}

public class UserWithTenantDetail
{
    public string Id { get; set; } = default!;
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public string? TenantId { get; set; }
    public string? TenantName { get; set; }
}