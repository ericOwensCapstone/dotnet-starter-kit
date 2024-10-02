using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Infrastructure.Endpoints.v1;
public static class CreateGrowthTreatmentEndpoint
{
    internal static RouteHandlerBuilder MapGrowthTreatmentCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateGrowthTreatmentCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateGrowthTreatmentEndpoint))
            .WithSummary("creates a growthTreatment")
            .WithDescription("creates a growthTreatment")
            .Produces<CreateGrowthTreatmentResponse>()
            .RequirePermission("Permissions.GrowthTreatments.Create")
            .MapToApiVersion(1);
    }
}

