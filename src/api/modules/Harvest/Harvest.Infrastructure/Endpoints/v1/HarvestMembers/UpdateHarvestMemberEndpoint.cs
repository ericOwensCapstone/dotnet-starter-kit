using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestMembers;
public static class UpdateHarvestMemberEndpoint
{
    internal static RouteHandlerBuilder MapHarvestMemberUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateHarvestMemberCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateHarvestMemberEndpoint))
            .WithSummary("update a harvestMember")
            .WithDescription("update a harvestMember")
            .Produces<UpdateHarvestMemberResponse>()
            .RequirePermission("Permissions.HarvestMembers.Update")
            .MapToApiVersion(1);
    }
}

