using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class CancelInvitationEndpoint
{
    public static void MapCancelInvitationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapDelete("invitations/{id:guid}", CancelInvitation)
            .WithName(nameof(CancelInvitation))
            .WithSummary("Cancel an invitation")
            .WithDescription("Cancels a pending invitation and disables the B2C user if created")
            .Produces<bool>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequirePermission(FshPermission.NameFor(FshActions.Delete, FshResources.UserInvitations))
            .WithTags("Invitations");
    }

    private static async Task<IResult> CancelInvitation(
        Guid id,
        IInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        var result = await invitationService.CancelInvitationAsync(id, cancellationToken);
        return Results.Ok(result);
    }
}