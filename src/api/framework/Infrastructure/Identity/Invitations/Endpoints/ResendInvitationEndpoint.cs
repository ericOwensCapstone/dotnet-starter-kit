using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class ResendInvitationEndpoint
{
    public static void MapResendInvitationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("invitations/{id:guid}/resend", ResendInvitation)
            .WithName(nameof(ResendInvitation))
            .WithSummary("Resend an invitation email")
            .WithDescription("Resends the invitation email to the user")
            .Produces<bool>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequirePermission(FshPermission.NameFor(FshActions.Update, FshResources.UserInvitations))
            .WithTags("Invitations");
    }

    private static async Task<IResult> ResendInvitation(
        Guid id,
        IInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        var result = await invitationService.ResendInvitationAsync(id, cancellationToken);
        return Results.Ok(result);
    }
}