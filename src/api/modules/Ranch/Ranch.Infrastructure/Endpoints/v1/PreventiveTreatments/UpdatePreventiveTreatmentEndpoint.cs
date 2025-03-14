using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.PreventiveTreatments;
public static class UpdatePreventiveTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapPreventiveTreatmentUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdatePreventiveTreatmentCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdatePreventiveTreatmentEndpoint))
            .WithSummary("update a preventiveTreatment")
            .WithDescription("update a preventiveTreatment")
            .Produces<UpdatePreventiveTreatmentResponse>()
            .RequirePermission("Permissions.PreventiveTreatments.Update")
            .MapToApiVersion(1);
    }
}

