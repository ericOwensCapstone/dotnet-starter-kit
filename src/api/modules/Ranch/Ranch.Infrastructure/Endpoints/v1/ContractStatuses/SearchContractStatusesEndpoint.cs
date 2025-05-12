using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.ContractStatuses;

public static class SearchContractStatusesEndpoint
{
    internal static RouteHandlerBuilder MapGetContractStatusListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchContractStatusesCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchContractStatusesEndpoint))
            .WithSummary("Gets a list of contractStatuses")
            .WithDescription("Gets a list of contractStatuses with pagination and filtering support")
            .Produces<PagedList<ContractStatusResponse>>()
            .RequirePermission("Permissions.ContractStatuses.View")
            .MapToApiVersion(1);
    }
}


