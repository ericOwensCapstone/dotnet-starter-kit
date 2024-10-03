using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Endpoints.v1;
public static class GetLifecycleStageEndpoint
{
    internal static RouteHandlerBuilder MapGetLifecycleStageEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetLifecycleStageRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetLifecycleStageEndpoint))
            .WithSummary("gets lifecycleStage by id")
            .WithDescription("gets prodct by id")
            .Produces<LifecycleStageResponse>()
            .RequirePermission("Permissions.LifecycleStages.View")
            .MapToApiVersion(1);
    }
}

