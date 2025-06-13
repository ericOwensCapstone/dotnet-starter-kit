using FSH.Framework.Core.Identity.Invitations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class B2CAcceptEndpoint
{
    internal static RouteHandlerBuilder MapB2CAcceptEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/api/b2c/invitations/accept", async (
                B2CAcceptRequest request,
                IInvitationService invitationService,
                ILogger<IInvitationService> logger,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    logger.LogInformation("B2C invitation accept request for token: {Token}, userId: {UserId}", 
                        request.InvitationToken, request.UserId);
                    
                    // Validate the invitation exists
                    var invitation = await invitationService.GetInvitationByTokenAsync(request.InvitationToken, cancellationToken);
                    
                    if (invitation == null)
                    {
                        logger.LogWarning("Invalid invitation token during accept: {Token}", request.InvitationToken);
                        // Return success to not block B2C flow
                        return Results.Ok(new B2CAcceptResponse { Success = true });
                    }

                    // Only accept if it's in the right state
                    if (invitation.Status == InvitationStatus.Sent && !invitation.IsExpired)
                    {
                        try
                        {
                            await invitationService.AcceptInvitationAsync(request.InvitationToken, request.UserId, cancellationToken);
                            logger.LogInformation("Invitation {InvitationId} accepted successfully for user {UserId}", 
                                invitation.Id, request.UserId);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Error accepting invitation {InvitationId} for user {UserId}", 
                                invitation.Id, request.UserId);
                            // Still return success to not block B2C flow
                        }
                    }
                    else
                    {
                        logger.LogWarning("Invitation {InvitationId} not in acceptable state. Status: {Status}, IsExpired: {IsExpired}", 
                            invitation.Id, invitation.Status, invitation.IsExpired);
                    }

                    // Always return success to prevent blocking B2C flow
                    return Results.Ok(new B2CAcceptResponse { Success = true });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during B2C invitation acceptance");
                    // Return success to prevent blocking B2C flow
                    return Results.Ok(new B2CAcceptResponse { Success = true });
                }
            })
            .WithName("B2CAccept")
            .WithSummary("Accept invitation for B2C custom policy")
            .WithDescription("Called by B2C invitation acceptance policy after user setup")
            .AllowAnonymous()
            .Produces<B2CAcceptResponse>()
            .WithTags("B2C Integration");
    }
}

public class B2CAcceptRequest
{
    public string InvitationToken { get; set; } = default!;
    public string UserId { get; set; } = default!;
}

public class B2CAcceptResponse
{
    public bool Success { get; set; }
}