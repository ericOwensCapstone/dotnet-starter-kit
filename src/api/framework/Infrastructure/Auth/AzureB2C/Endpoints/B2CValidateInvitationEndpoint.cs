using FSH.Framework.Core.Identity.Invitations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;

public static class B2CValidateInvitationEndpoint
{
    internal static RouteHandlerBuilder MapB2CValidateInvitationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/api/public/b2c/invitations/validate/{token}", async (
                string token,
                IInvitationService invitationService,
                ILogger<IInvitationService> logger,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    logger.LogInformation("B2C invitation validation request for token: {Token}", token);
                    
                    var invitation = await invitationService.GetInvitationByTokenAsync(token, cancellationToken);
                    
                    if (invitation == null)
                    {
                        logger.LogWarning("Invalid invitation token: {Token}", token);
                        return Results.NotFound(new B2CInvitationValidationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "Invalid invitation token."
                        });
                    }

                    if (invitation.Status == InvitationStatus.Accepted)
                    {
                        return Results.BadRequest(new B2CInvitationValidationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "This invitation has already been accepted."
                        });
                    }

                    if (invitation.Status == InvitationStatus.Cancelled)
                    {
                        return Results.BadRequest(new B2CInvitationValidationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "This invitation has been cancelled."
                        });
                    }

                    if (invitation.Status == InvitationStatus.Expired || invitation.IsExpired)
                    {
                        return Results.BadRequest(new B2CInvitationValidationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "This invitation has expired."
                        });
                    }

                    if (invitation.Status != InvitationStatus.Sent)
                    {
                        return Results.BadRequest(new B2CInvitationValidationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "This invitation is not ready to be accepted."
                        });
                    }

                    var response = new B2CInvitationValidationResponse
                    {
                        IsValid = true,
                        Email = invitation.Email,
                        FirstName = invitation.FirstName,
                        LastName = invitation.LastName,
                        DisplayName = invitation.DisplayName,
                        TargetTenantId = invitation.TargetTenantId,
                        ExpiresAt = invitation.ExpiresAt,
                        InvitedBy = invitation.InvitedBy,
                        ErrorMessage = null
                    };

                    logger.LogInformation("Valid invitation found for email: {Email}, tenant: {TenantId}", 
                        invitation.Email, invitation.TargetTenantId);

                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during B2C invitation validation");
                    return Results.Problem("Internal server error during invitation validation");
                }
            })
            .WithName("B2CValidateInvitation")
            .WithSummary("Validate invitation token for B2C integration")
            .WithDescription("B2C custom policy calls this endpoint to validate invitation tokens during signup flow")
            .AllowAnonymous()
            .WithMetadata(new Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute())
            .Produces<B2CInvitationValidationResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("B2C Integration");
    }
}

public class B2CInvitationValidationResponse
{
    public bool IsValid { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? TargetTenantId { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? InvitedBy { get; set; }
    public string? ErrorMessage { get; set; }
}