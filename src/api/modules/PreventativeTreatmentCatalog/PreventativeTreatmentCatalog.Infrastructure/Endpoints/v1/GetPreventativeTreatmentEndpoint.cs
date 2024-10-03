using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Endpoints.v1;
public static class GetPreventativeTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapGetPreventativeTreatmentEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetPreventativeTreatmentRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetPreventativeTreatmentEndpoint))
            .WithSummary("gets preventativeTreatment by id")
            .WithDescription("gets prodct by id")
            .Produces<PreventativeTreatmentResponse>()
            .RequirePermission("Permissions.PreventativeTreatments.View")
            .MapToApiVersion(1);
    }
}

