using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Endpoints.v1;
public static class UpdatePreventativeTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapPreventativeTreatmentUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdatePreventativeTreatmentCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdatePreventativeTreatmentEndpoint))
            .WithSummary("update a preventativeTreatment")
            .WithDescription("update a preventativeTreatment")
            .Produces<UpdatePreventativeTreatmentResponse>()
            .RequirePermission("Permissions.PreventativeTreatments.Update")
            .MapToApiVersion(1);
    }
}

