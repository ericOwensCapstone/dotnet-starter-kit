using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Identity.Invitations.Features.ExtendInvitationExpiration;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class ExtendInvitationExpirationEndpoint
{
    public static void MapExtendInvitationExpirationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPatch("invitations/{id:guid}/extend-expiration", ExtendInvitationExpiration)
            .WithName(nameof(ExtendInvitationExpiration))
            .WithSummary("Extend invitation expiration")
            .WithDescription("Extends the expiration date of an invitation and re-enables B2C user if needed")
            .Produces<ExtendInvitationExpirationResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .RequirePermission(FshPermission.NameFor(FshActions.Update, FshResources.UserInvitations))
            .WithTags("Invitations");
    }

    private static async Task<IResult> ExtendInvitationExpiration(
        Guid id,
        ExtendInvitationExpirationRequest request,
        IInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        // Ensure the ID in the route matches the request
        if (id != request.InvitationId)
        {
            return Results.BadRequest("The invitation ID in the route must match the ID in the request body.");
        }

        var result = await invitationService.ExtendInvitationExpirationAsync(request, cancellationToken);
        return Results.Ok(result);
    }
}