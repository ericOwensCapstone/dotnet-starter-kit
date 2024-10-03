using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Endpoints.v1;
public static class DeletePreventativeTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapPreventativeTreatmentDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeletePreventativeTreatmentCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeletePreventativeTreatmentEndpoint))
            .WithSummary("deletes preventativeTreatment by id")
            .WithDescription("deletes preventativeTreatment by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.PreventativeTreatments.Delete")
            .MapToApiVersion(1);
    }
}

