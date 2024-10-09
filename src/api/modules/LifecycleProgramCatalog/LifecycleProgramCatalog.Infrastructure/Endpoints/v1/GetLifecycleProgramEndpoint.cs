using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Endpoints.v1;
public static class GetLifecycleProgramEndpoint
{
    internal static RouteHandlerBuilder MapGetLifecycleProgramEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetLifecycleProgramRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetLifecycleProgramEndpoint))
            .WithSummary("gets lifecycleProgram by id")
            .WithDescription("gets prodct by id")
            .Produces<LifecycleProgramResponse>()
            .RequirePermission("Permissions.LifecyclePrograms.View")
            .MapToApiVersion(1);
    }
}

