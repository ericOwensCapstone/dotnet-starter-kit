using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Get.v1;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Endpoints.v1;

public static class SearchLifecycleStagesEndpoint
{
    internal static RouteHandlerBuilder MapGetLifecycleStageListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] PaginationFilter filter) =>
            {
                var response = await mediator.Send(new SearchLifecycleStagesCommand(filter));
                return Results.Ok(response);
            })
            .WithName(nameof(SearchLifecycleStagesEndpoint))
            .WithSummary("Gets a list of lifecycleStages")
            .WithDescription("Gets a list of lifecycleStages with pagination and filtering support")
            .Produces<PagedList<LifecycleStageResponse>>()
            .RequirePermission("Permissions.LifecycleStages.View")
            .MapToApiVersion(1);
    }
}


