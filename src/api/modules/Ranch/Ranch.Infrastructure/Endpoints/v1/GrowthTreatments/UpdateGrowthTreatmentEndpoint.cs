using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.GrowthTreatments;
public static class UpdateGrowthTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapGrowthTreatmentUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateGrowthTreatmentCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateGrowthTreatmentEndpoint))
            .WithSummary("update a growthTreatment")
            .WithDescription("update a growthTreatment")
            .Produces<UpdateGrowthTreatmentResponse>()
            .RequirePermission("Permissions.GrowthTreatments.Update")
            .MapToApiVersion(1);
    }
}

