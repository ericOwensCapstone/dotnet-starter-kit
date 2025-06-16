using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Infrastructure.Identity.Invitations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;

public static class B2CPreRegistrationValidationEndpoint
{
    internal static RouteHandlerBuilder MapB2CPreRegistrationValidationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/api/public/b2c/invitations/pre-validate", async (
                B2CPreRegistrationRequest request,
                IInvitationService invitationService,
                ILogger<IInvitationService> logger,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    logger.LogInformation("B2C pre-registration validation for email: {Email}, token: {Token}", 
                        request.Email, request.InvitationToken);

                    // Validate required fields
                    if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.InvitationToken))
                    {
                        return Results.BadRequest(new B2CPreRegistrationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "Email and invitation token are required."
                        });
                    }

                    // Get the invitation by token
                    var invitation = await invitationService.GetInvitationByTokenAsync(request.InvitationToken, cancellationToken);
                    
                    if (invitation == null)
                    {
                        logger.LogWarning("Invalid invitation token during pre-registration: {Token}", request.InvitationToken);
                        return Results.BadRequest(new B2CPreRegistrationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "Invalid invitation token."
                        });
                    }

                    // Verify the email matches the invitation
                    if (!string.Equals(invitation.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                    {
                        logger.LogWarning("Email mismatch. Invitation email: {InvitationEmail}, Provided email: {ProvidedEmail}", 
                            invitation.Email, request.Email);
                        return Results.BadRequest(new B2CPreRegistrationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "The email address does not match the invitation."
                        });
                    }

                    // Check invitation status
                    if (invitation.Status == InvitationStatus.Accepted)
                    {
                        return Results.BadRequest(new B2CPreRegistrationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "This invitation has already been accepted."
                        });
                    }

                    if (invitation.Status == InvitationStatus.Cancelled)
                    {
                        return Results.BadRequest(new B2CPreRegistrationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "This invitation has been cancelled."
                        });
                    }

                    if (invitation.Status == InvitationStatus.Expired || invitation.IsExpired)
                    {
                        return Results.BadRequest(new B2CPreRegistrationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "This invitation has expired."
                        });
                    }

                    if (invitation.Status != InvitationStatus.Sent)
                    {
                        return Results.BadRequest(new B2CPreRegistrationResponse
                        {
                            IsValid = false,
                            ErrorMessage = "This invitation is not ready to be accepted."
                        });
                    }

                    // Return success with metadata that B2C can use
                    var response = new B2CPreRegistrationResponse
                    {
                        IsValid = true,
                        Email = invitation.Email,
                        FirstName = invitation.FirstName ?? request.FirstName,
                        LastName = invitation.LastName ?? request.LastName,
                        DisplayName = invitation.DisplayName ?? $"{request.FirstName} {request.LastName}".Trim(),
                        TenantId = invitation.TargetTenantId,
                        Role = invitation.Role,
                        ErrorMessage = null,
                        // Additional metadata for B2C custom attributes
                        CustomAttributes = new Dictionary<string, string>
                        {
                            ["TenantId"] = invitation.TargetTenantId,
                            ["UserStatus"] = "Invited",
                            ["InvitationId"] = invitation.Id.ToString()
                        }
                    };

                    logger.LogInformation("Pre-registration validation successful for {Email}, invitation {InvitationId}", 
                        invitation.Email, invitation.Id);

                    return Results.Ok(response);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during B2C pre-registration validation");
                    return Results.Problem("Internal server error during pre-registration validation");
                }
            })
            .WithName("B2CPreRegistrationValidation")
            .WithSummary("Pre-validate user registration for B2C")
            .WithDescription("B2C custom policy calls this endpoint during user registration to validate invitation and get user metadata")
            .AllowAnonymous()
            .WithMetadata(new Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute())
            .Produces<B2CPreRegistrationResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("B2C Integration");
    }
}

public class B2CPreRegistrationRequest
{
    public string Email { get; set; } = default!;
    public string InvitationToken { get; set; } = default!;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ObjectId { get; set; }
}

public class B2CPreRegistrationResponse
{
    public bool IsValid { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? TenantId { get; set; }
    public string? Role { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, string>? CustomAttributes { get; set; }
}