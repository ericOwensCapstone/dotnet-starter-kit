using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.PreventiveTreatments;
public static class CreatePreventiveTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapPreventiveTreatmentCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreatePreventiveTreatmentCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreatePreventiveTreatmentEndpoint))
            .WithSummary("creates a preventiveTreatment")
            .WithDescription("creates a preventiveTreatment")
            .Produces<CreatePreventiveTreatmentResponse>()
            .RequirePermission("Permissions.PreventiveTreatments.Create")
            .MapToApiVersion(1);
    }
}

