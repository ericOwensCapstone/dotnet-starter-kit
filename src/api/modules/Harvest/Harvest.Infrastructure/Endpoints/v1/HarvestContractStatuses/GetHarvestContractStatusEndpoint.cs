using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContractStatuses;
public static class GetHarvestContractStatusEndpoint
{
    internal static RouteHandlerBuilder MapGetHarvestContractStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetHarvestContractStatusRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetHarvestContractStatusEndpoint))
            .WithSummary("gets harvestContractStatus by id")
            .WithDescription("gets harvestContractStatus by id")
            .Produces<HarvestContractStatusResponse>()
            .RequirePermission("Permissions.HarvestContractStatuses.Search")
            .MapToApiVersion(1);
    }
}

