using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.Contracts.Get.v1;
using FSH.Starter.WebApi.Ranch.Application.Contracts.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Contracts;

public static class SearchContractsEndpoint
{
    internal static RouteHandlerBuilder MapGetContractListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchContractsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchContractsEndpoint))
            .WithSummary("Gets a list of contracts")
            .WithDescription("Gets a list of contracts with pagination and filtering support")
            .Produces<PagedList<ContractResponse>>()
            .RequirePermission("Permissions.Contracts.Search")
            .MapToApiVersion(1);
    }
}


