using FSH.Framework.Core.Identity.Invitations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class B2CValidateEndpoint
{
    internal static RouteHandlerBuilder MapB2CValidateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/api/b2c/invitations/validate", async (
                B2CValidateRequest request,
                IInvitationService invitationService,
                ILogger<IInvitationService> logger,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    logger.LogInformation("B2C invitation validation request for token: {Token}", request.InvitationToken);
                    
                    var invitation = await invitationService.GetInvitationByTokenAsync(request.InvitationToken, cancellationToken);
                    
                    if (invitation == null)
                    {
                        logger.LogWarning("Invalid invitation token: {Token}", request.InvitationToken);
                        return Results.Ok(new B2CValidateResponse
                        {
                            IsValidInvitation = false,
                            Email = string.Empty,
                            DisplayName = string.Empty,
                            GivenName = string.Empty,
                            Surname = string.Empty,
                            Extension_TenantId = string.Empty
                        });
                    }

                    // Check if invitation is valid for acceptance
                    if (invitation.Status != InvitationStatus.Sent || invitation.IsExpired)
                    {
                        logger.LogWarning("Invitation not valid for acceptance. Status: {Status}, IsExpired: {IsExpired}", 
                            invitation.Status, invitation.IsExpired);
                        return Results.Ok(new B2CValidateResponse
                        {
                            IsValidInvitation = false,
                            Email = string.Empty,
                            DisplayName = string.Empty,
                            GivenName = string.Empty,
                            Surname = string.Empty,
                            Extension_TenantId = string.Empty
                        });
                    }

                    // Return the invitation details for B2C
                    var response = new B2CValidateResponse
                    {
                        IsValidInvitation = true,
                        Email = invitation.Email,
                        DisplayName = invitation.DisplayName,
                        GivenName = invitation.FirstName ?? string.Empty,
                        Surname = invitation.LastName ?? string.Empty,
                        Extension_TenantId = invitation.TargetTenantId
                    };

                    logger.LogInformation("Valid invitation found for email: {Email}, tenant: {TenantId}", 
                        invitation.Email, invitation.TargetTenantId);

                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during B2C invitation validation");
                    // Return invalid to prevent unauthorized access
                    return Results.Ok(new B2CValidateResponse
                    {
                        IsValidInvitation = false,
                        Email = string.Empty,
                        DisplayName = string.Empty,
                        GivenName = string.Empty,
                        Surname = string.Empty,
                        Extension_TenantId = string.Empty
                    });
                }
            })
            .WithName("B2CValidate")
            .WithSummary("Validate invitation token for B2C custom policy")
            .WithDescription("Called by B2C invitation acceptance policy to validate tokens")
            .AllowAnonymous()
            .Produces<B2CValidateResponse>()
            .ProducesValidationProblem()
            .WithTags("B2C Integration");
    }
}

public class B2CValidateRequest
{
    public string InvitationToken { get; set; } = default!;
}

public class B2CValidateResponse
{
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string GivenName { get; set; } = default!;
    public string Surname { get; set; } = default!;
    public string Extension_TenantId { get; set; } = default!;
    public bool IsValidInvitation { get; set; }
}