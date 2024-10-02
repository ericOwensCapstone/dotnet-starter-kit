using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure.Endpoints.v1;
public static class GetGrowthTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapGetGrowthTreatmentEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetGrowthTreatmentRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetGrowthTreatmentEndpoint))
            .WithSummary("gets growthTreatment by id")
            .WithDescription("gets prodct by id")
            .Produces<GrowthTreatmentResponse>()
            .RequirePermission("Permissions.GrowthTreatments.View")
            .MapToApiVersion(1);
    }
}

