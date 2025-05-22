using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContracts;
public static class GetHarvestContractEndpoint
{
    internal static RouteHandlerBuilder MapGetHarvestContractEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetHarvestContractRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetHarvestContractEndpoint))
            .WithSummary("gets harvestContract by id")
            .WithDescription("gets harvestContract by id")
            .Produces<HarvestContractResponse>()
            .RequirePermission("Permissions.HarvestContracts.Search")
            .MapToApiVersion(1);
    }
}

