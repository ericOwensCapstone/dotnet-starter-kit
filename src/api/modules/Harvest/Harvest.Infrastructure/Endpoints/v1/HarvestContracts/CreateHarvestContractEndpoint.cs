using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContracts;
public static class CreateHarvestContractEndpoint
{
    internal static RouteHandlerBuilder MapHarvestContractCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateHarvestContractCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateHarvestContractEndpoint))
            .WithSummary("creates a harvestContract")
            .WithDescription("creates a harvestContract")
            .Produces<CreateHarvestContractResponse>()
            .RequirePermission("Permissions.HarvestContracts.Create")
            .MapToApiVersion(1);
    }
}

