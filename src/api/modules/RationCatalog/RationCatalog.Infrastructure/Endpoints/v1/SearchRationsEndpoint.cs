using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Get.v1;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.RationCatalog.Infrastructure.Endpoints.v1;

public static class SearchRationsEndpoint
{
    internal static RouteHandlerBuilder MapGetRationListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] PaginationFilter filter) =>
            {
                var response = await mediator.Send(new SearchRationsCommand(filter));
                return Results.Ok(response);
            })
            .WithName(nameof(SearchRationsEndpoint))
            .WithSummary("Gets a list of rations")
            .WithDescription("Gets a list of rations with pagination and filtering support")
            .Produces<PagedList<RationResponse>>()
            .RequirePermission("Permissions.Rations.View")
            .MapToApiVersion(1);
    }
}


