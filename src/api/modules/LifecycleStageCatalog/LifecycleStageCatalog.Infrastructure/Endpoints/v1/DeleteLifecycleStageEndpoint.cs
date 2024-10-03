using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Endpoints.v1;
public static class DeleteLifecycleStageEndpoint
{
    internal static RouteHandlerBuilder MapLifecycleStageDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteLifecycleStageCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteLifecycleStageEndpoint))
            .WithSummary("deletes lifecycleStage by id")
            .WithDescription("deletes lifecycleStage by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.LifecycleStages.Delete")
            .MapToApiVersion(1);
    }
}

