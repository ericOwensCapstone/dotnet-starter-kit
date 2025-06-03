using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class GetInvitationEndpoint
{
    public static void MapGetInvitationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("invitations/{id:guid}", GetInvitation)
            .WithName(nameof(GetInvitation))
            .WithSummary("Get an invitation by ID")
            .WithDescription("Gets the details of a specific invitation")
            .Produces<UserInvitation>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequirePermission(FshPermission.NameFor(FshActions.View, FshResources.UserInvitations))
            .WithTags("Invitations");
    }

    private static async Task<IResult> GetInvitation(
        Guid id,
        IInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        var invitation = await invitationService.GetInvitationAsync(id, cancellationToken);
        return invitation != null 
            ? Results.Ok(invitation)
            : Results.NotFound();
    }
}