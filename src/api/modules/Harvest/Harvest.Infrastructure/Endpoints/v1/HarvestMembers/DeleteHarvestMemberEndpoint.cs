using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestMembers;
public static class DeleteHarvestMemberEndpoint
{
    internal static RouteHandlerBuilder MapHarvestMemberDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteHarvestMemberCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteHarvestMemberEndpoint))
            .WithSummary("deletes harvestMember by id")
            .WithDescription("deletes harvestMember by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.HarvestMembers.Delete")
            .MapToApiVersion(1);
    }
}

