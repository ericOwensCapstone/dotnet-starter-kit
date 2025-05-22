using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Get.v1;
using FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContracts;

public static class SearchHarvestContractsEndpoint
{
    internal static RouteHandlerBuilder MapGetHarvestContractListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchHarvestContractsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchHarvestContractsEndpoint))
            .WithSummary("Gets a list of harvestContracts")
            .WithDescription("Gets a list of harvestContracts with pagination and filtering support")
            .Produces<PagedList<HarvestContractResponse>>()
            .RequirePermission("Permissions.HarvestContracts.Search")
            .MapToApiVersion(1);
    }
}


