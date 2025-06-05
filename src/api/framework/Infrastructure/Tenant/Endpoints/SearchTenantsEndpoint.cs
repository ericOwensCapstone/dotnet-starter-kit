using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Tenant.Dtos;
using FSH.Framework.Core.Tenant.Features.SearchTenants;
using FSH.Framework.Infrastructure.Auth.Policy;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Tenant.Endpoints;

public static class SearchTenantsEndpoint
{
    internal static RouteHandlerBuilder MapSearchTenantsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search", async (ISender mediator, SearchTenantsQuery query) =>
        {
            var response = await mediator.Send(query);
            return Results.Ok(response);
        })
        .WithName(nameof(SearchTenantsEndpoint))
        .WithSummary("Search tenants with pagination")
        .RequirePermission("Permissions.Tenants.View")
        .WithDescription("Search and paginate through tenants by name or ID")
        .Produces<PagedList<TenantDetail>>(StatusCodes.Status200OK);
    }
}