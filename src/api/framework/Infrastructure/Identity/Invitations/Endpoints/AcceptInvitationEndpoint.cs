using FSH.Framework.Core.Identity.Invitations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class AcceptInvitationEndpoint
{
    public static void MapAcceptInvitationEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapPost("invitations/accept", AcceptInvitation)
            .WithName(nameof(AcceptInvitation))
            .WithSummary("Accept an invitation")
            .WithDescription("Accepts an invitation using the token (public endpoint)")
            .Produces<bool>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .AllowAnonymous()
            .WithTags("Invitations");
    }

    private static Task<IResult> AcceptInvitation()
    {
        // This endpoint is called after B2C authentication, 
        // so the user should already be created via auto-provisioning
        // This is just for completeness if needed
        return Task.FromResult(Results.BadRequest("Invitations are automatically accepted upon first login through B2C."));
    }
}

public record AcceptInvitationRequest(string Token);