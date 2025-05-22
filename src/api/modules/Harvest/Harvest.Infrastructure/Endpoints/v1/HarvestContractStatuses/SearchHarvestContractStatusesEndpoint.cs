using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContractStatuses;

public static class SearchHarvestContractStatusesEndpoint
{
    internal static RouteHandlerBuilder MapGetHarvestContractStatusListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchHarvestContractStatusesCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchHarvestContractStatusesEndpoint))
            .WithSummary("Gets a list of harvestContractStatuses")
            .WithDescription("Gets a list of harvestContractStatuses with pagination and filtering support")
            .Produces<PagedList<HarvestContractStatusResponse>>()
            .RequirePermission("Permissions.HarvestContractStatuses.Search")
            .MapToApiVersion(1);
    }
}


