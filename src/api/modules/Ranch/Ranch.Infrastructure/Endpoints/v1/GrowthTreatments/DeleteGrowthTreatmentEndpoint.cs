using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.GrowthTreatments;
public static class DeleteGrowthTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapGrowthTreatmentDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteGrowthTreatmentCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteGrowthTreatmentEndpoint))
            .WithSummary("deletes growthTreatment by id")
            .WithDescription("deletes growthTreatment by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.GrowthTreatments.Delete")
            .MapToApiVersion(1);
    }
}

