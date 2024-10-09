using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Endpoints.v1;
public static class DeleteLifecycleProgramEndpoint
{
    internal static RouteHandlerBuilder MapLifecycleProgramDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteLifecycleProgramCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteLifecycleProgramEndpoint))
            .WithSummary("deletes lifecycleProgram by id")
            .WithDescription("deletes lifecycleProgram by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.LifecyclePrograms.Delete")
            .MapToApiVersion(1);
    }
}

