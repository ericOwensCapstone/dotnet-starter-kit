using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.PreventiveTreatments;
public static class GetPreventiveTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapGetPreventiveTreatmentEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetPreventiveTreatmentRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetPreventiveTreatmentEndpoint))
            .WithSummary("gets preventiveTreatment by id")
            .WithDescription("gets prodct by id")
            .Produces<PreventiveTreatmentResponse>()
            .RequirePermission("Permissions.PreventiveTreatments.View")
            .MapToApiVersion(1);
    }
}

