using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.PreventiveTreatments;
public static class DeletePreventiveTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapPreventiveTreatmentDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeletePreventiveTreatmentCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeletePreventiveTreatmentEndpoint))
            .WithSummary("deletes preventiveTreatment by id")
            .WithDescription("deletes preventiveTreatment by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.PreventiveTreatments.Delete")
            .MapToApiVersion(1);
    }
}

