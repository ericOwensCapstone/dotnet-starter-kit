using FSH.Framework.Infrastructure.Identity.Users.Features.DeleteUserCompletely;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.Shared.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Users.Endpoints;

public static class DeleteUserCompletelyEndpoint
{
    internal static RouteHandlerBuilder MapDeleteUserCompletelyEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/delete-completely", async (DeleteUserCompletelyCommand command, ISender mediator) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(DeleteUserCompletelyEndpoint))
            .WithSummary("Completely delete a user from database and optionally from Azure B2C")
            .WithDescription("Permanently removes a user from the system including all related data. Requires Root.ManageAllUsers permission.")
            .RequirePermission(FshActions.ManageAll, FshResources.Users)
            .Produces<DeleteUserCompletelyResponse>();
    }
}