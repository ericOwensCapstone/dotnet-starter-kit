using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Endpoints.v1;
public static class CreatePreventativeTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapPreventativeTreatmentCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreatePreventativeTreatmentCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreatePreventativeTreatmentEndpoint))
            .WithSummary("creates a preventativeTreatment")
            .WithDescription("creates a preventativeTreatment")
            .Produces<CreatePreventativeTreatmentResponse>()
            .RequirePermission("Permissions.PreventativeTreatments.Create")
            .MapToApiVersion(1);
    }
}

