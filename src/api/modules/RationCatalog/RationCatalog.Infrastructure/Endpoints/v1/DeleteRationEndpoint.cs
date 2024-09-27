using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.RationCatalog.Infrastructure.Endpoints.v1;
public static class DeleteRationEndpoint
{
    internal static RouteHandlerBuilder MapRationDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteRationCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteRationEndpoint))
            .WithSummary("deletes ration by id")
            .WithDescription("deletes ration by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.Rations.Delete")
            .MapToApiVersion(1);
    }
}

