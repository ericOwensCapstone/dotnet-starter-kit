using FSH.Framework.Core.Identity.Invitations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class ValidateInvitationTokenEndpoint
{
    public static void MapValidateInvitationTokenEndpoint(this IEndpointRouteBuilder builder)
    {
        builder.MapGet("validate/{token}", ValidateInvitationToken)
            .WithName(nameof(ValidateInvitationToken))
            .WithSummary("Validate invitation token")
            .WithDescription("Validates an invitation token and returns invitation details if valid")
            .Produces<InvitationValidationResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .AllowAnonymous() // Public endpoint for invitation validation
            .WithTags("Invitations");
    }

    private static async Task<IResult> ValidateInvitationToken(
        string token,
        IInvitationService invitationService,
        CancellationToken cancellationToken)
    {
        var invitation = await invitationService.GetInvitationByTokenAsync(token, cancellationToken);
        
        if (invitation == null)
        {
            return Results.NotFound("Invalid invitation token.");
        }

        if (invitation.Status == InvitationStatus.Accepted)
        {
            return Results.BadRequest("This invitation has already been accepted.");
        }

        if (invitation.Status == InvitationStatus.Cancelled)
        {
            return Results.BadRequest("This invitation has been cancelled.");
        }

        if (invitation.Status == InvitationStatus.Expired || invitation.IsExpired)
        {
            return Results.BadRequest("This invitation has expired.");
        }

        if (invitation.Status != InvitationStatus.Sent)
        {
            return Results.BadRequest("This invitation is not ready to be accepted.");
        }

        var response = new InvitationValidationResponse
        {
            IsValid = true,
            Email = invitation.Email,
            DisplayName = invitation.DisplayName,
            TargetTenantId = invitation.TargetTenantId,
            ExpiresAt = invitation.ExpiresAt,
            InvitedBy = invitation.InvitedBy
        };

        return Results.Ok(response);
    }
}

public class InvitationValidationResponse
{
    public bool IsValid { get; set; }
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string TargetTenantId { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public string InvitedBy { get; set; } = default!;
}