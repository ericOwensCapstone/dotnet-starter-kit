using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Get.v1;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestMembers;

public static class SearchHarvestMembersEndpoint
{
    internal static RouteHandlerBuilder MapGetHarvestMemberListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchHarvestMembersCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchHarvestMembersEndpoint))
            .WithSummary("Gets a list of harvestMembers")
            .WithDescription("Gets a list of harvestMembers with pagination and filtering support")
            .Produces<PagedList<HarvestMemberResponse>>()
            .RequirePermission("Permissions.HarvestMembers.Search")
            .MapToApiVersion(1);
    }
}


