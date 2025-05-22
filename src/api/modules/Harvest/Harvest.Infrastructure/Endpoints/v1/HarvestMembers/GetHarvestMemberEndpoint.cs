using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestMembers;
public static class GetHarvestMemberEndpoint
{
    internal static RouteHandlerBuilder MapGetHarvestMemberEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetHarvestMemberRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetHarvestMemberEndpoint))
            .WithSummary("gets harvestMember by id")
            .WithDescription("gets harvestMember by id")
            .Produces<HarvestMemberResponse>()
            .RequirePermission("Permissions.HarvestMembers.Search")
            .MapToApiVersion(1);
    }
}

