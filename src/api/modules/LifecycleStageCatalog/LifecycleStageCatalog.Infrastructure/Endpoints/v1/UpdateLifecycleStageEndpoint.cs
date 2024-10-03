using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Endpoints.v1;
public static class UpdateLifecycleStageEndpoint
{
    internal static RouteHandlerBuilder MapLifecycleStageUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateLifecycleStageCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateLifecycleStageEndpoint))
            .WithSummary("update a lifecycleStage")
            .WithDescription("update a lifecycleStage")
            .Produces<UpdateLifecycleStageResponse>()
            .RequirePermission("Permissions.LifecycleStages.Update")
            .MapToApiVersion(1);
    }
}

