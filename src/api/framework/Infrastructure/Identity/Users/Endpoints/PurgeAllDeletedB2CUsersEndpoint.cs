using FSH.Framework.Infrastructure.Identity.Users.Features.PurgeDeletedB2CUsers;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.Shared.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;

public static class PurgeAllDeletedB2CUsersEndpoint
{
    internal static RouteHandlerBuilder MapPurgeAllDeletedB2CUsersEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/purge-deleted-b2c-users", async (ISender mediator) =>
            {
                var command = new PurgeAllDeletedB2CUsersCommand();
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(PurgeAllDeletedB2CUsersEndpoint))
            .WithSummary("Permanently purge all soft-deleted users from Azure B2C")
            .WithDescription("Retrieves all users in the B2C deleted items and permanently removes them. Requires Root.ManageAllUsers permission.")
            .RequirePermission(FshActions.ManageAll, FshResources.Users)
            .Produces<PurgeAllDeletedB2CUsersResponse>();
    }
}