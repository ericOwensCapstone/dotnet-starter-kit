using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Get.v1;
using FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.LifecyclePrograms;

public static class SearchLifecycleProgramsEndpoint
{
    internal static RouteHandlerBuilder MapGetLifecycleProgramListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchLifecycleProgramsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchLifecycleProgramsEndpoint))
            .WithSummary("Gets a list of lifecyclePrograms")
            .WithDescription("Gets a list of lifecyclePrograms with pagination and filtering support")
            .Produces<PagedList<LifecycleProgramResponse>>()
            .RequirePermission("Permissions.LifecyclePrograms.View")
            .MapToApiVersion(1);
    }
}


