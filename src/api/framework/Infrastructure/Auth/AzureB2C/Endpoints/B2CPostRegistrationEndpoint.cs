using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Infrastructure.Identity.Invitations;
using FSH.Framework.Infrastructure.Identity.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;

public static class B2CPostRegistrationEndpoint
{
    internal static RouteHandlerBuilder MapB2CPostRegistrationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        // Add a GET endpoint for testing
        endpoints.MapGet("/api/public/b2c/invitations/post-registration", () =>
        {
            var testLogger = endpoints.ServiceProvider.GetService<ILogger<IInvitationService>>();
            testLogger?.LogInformation("=== B2C POST-REGISTRATION TEST ENDPOINT HIT (GET) ===");
            return Results.Ok(new { message = "B2C Post-Registration Endpoint is reachable", timestamp = DateTime.UtcNow });
        })
        .AllowAnonymous()
        .WithMetadata(new Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute());

        return endpoints
            .MapPost("/api/public/b2c/invitations/post-registration", async (
                B2CPostRegistrationRequest request,
                HttpContext context,
                IServiceProvider serviceProvider,
                ILogger<IInvitationService> logger) =>
            {
                try
                {
                    logger.LogInformation("=== B2C POST-REGISTRATION ENDPOINT HIT ===");
                    logger.LogInformation("B2C post-registration processing for ObjectId: {ObjectId}, Email: {Email}, Token: {Token}", 
                        request.ObjectId, request.Email, request.InvitationToken);
                    
                    // Create a timeout for B2C operations (B2C expects quick responses)
                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    var cancellationToken = cts.Token;
                    
                    // Get services manually to avoid DI issues
                    var invitationService = serviceProvider.GetRequiredService<AnonymousInvitationService>();
                    var userManager = serviceProvider.GetRequiredService<UserManager<FshUser>>();

                    // Validate required fields
                    if (string.IsNullOrEmpty(request.ObjectId) || 
                        string.IsNullOrEmpty(request.Email) || 
                        string.IsNullOrEmpty(request.InvitationToken))
                    {
                        return Results.BadRequest(new B2CPostRegistrationResponse
                        {
                            Success = false,
                            ErrorMessage = "ObjectId, email, and invitation token are required."
                        });
                    }

                    // Get the invitation
                    var invitation = await invitationService.GetInvitationByTokenAsync(request.InvitationToken, cancellationToken);
                    
                    if (invitation == null)
                    {
                        logger.LogWarning("Invalid invitation token during post-registration: {Token}", request.InvitationToken);
                        return Results.BadRequest(new B2CPostRegistrationResponse
                        {
                            Success = false,
                            ErrorMessage = "Invalid invitation token."
                        });
                    }

                    // Verify the email matches
                    if (!string.Equals(invitation.Email, request.Email, StringComparison.OrdinalIgnoreCase))
                    {
                        logger.LogWarning("Email mismatch during post-registration. Invitation: {InvitationEmail}, Provided: {ProvidedEmail}", 
                            invitation.Email, request.Email);
                        return Results.BadRequest(new B2CPostRegistrationResponse
                        {
                            Success = false,
                            ErrorMessage = "Email does not match invitation."
                        });
                    }

                    // Find the user by ObjectId
                    FshUser? user = null;
                    try
                    {
                        user = await userManager.Users
                            .FirstOrDefaultAsync(u => u.ObjectId == request.ObjectId, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error finding user by ObjectId: {ObjectId}", request.ObjectId);
                    }

                    if (user == null)
                    {
                        logger.LogWarning("User not found with ObjectId: {ObjectId} after B2C registration", request.ObjectId);
                        
                        // The user hasn't been synced to the app database yet
                        // This is expected since the user was just created in B2C
                        // We'll accept the invitation anyway to not block the B2C flow
                        
                        // We can't mark the invitation as accepted yet because we need a userId
                        // The invitation will be accepted when the user first logs in and gets synced
                        logger.LogInformation("Invitation {InvitationId} will be accepted after user sync", invitation.Id);
                        
                        return Results.Ok(new B2CPostRegistrationResponse
                        {
                            Success = true,
                            Message = "User registration acknowledged. User sync pending.",
                            TenantId = invitation.TargetTenantId
                        });
                    }

                    // Mark invitation as accepted if not already
                    if (invitation.Status != InvitationStatus.Accepted)
                    {
                        await invitationService.AcceptInvitationAsync(invitation.InvitationToken, user.Id, cancellationToken);
                        logger.LogInformation("Invitation {InvitationId} marked as accepted for user {UserId}", 
                            invitation.Id, user.Id);
                    }
                    else
                    {
                        logger.LogInformation("Invitation {InvitationId} already accepted", invitation.Id);
                    }

                    return Results.Ok(new B2CPostRegistrationResponse
                    {
                        Success = true,
                        Message = "User registration completed successfully.",
                        UserId = user.Id,
                        TenantId = invitation.TargetTenantId
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error during B2C post-registration processing");
                    // Return success to avoid blocking B2C flow
                    return Results.Ok(new B2CPostRegistrationResponse
                    {
                        Success = true,
                        Message = "Registration acknowledged with warnings.",
                        ErrorMessage = "Some post-registration tasks may have failed."
                    });
                }
            })
            .WithName("B2CPostRegistration")
            .WithSummary("Complete user registration after B2C signup")
            .WithDescription("B2C custom policy calls this endpoint after successful user creation to finalize invitation acceptance")
            .AllowAnonymous()
            .WithMetadata(new Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute())
            .Produces<B2CPostRegistrationResponse>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithTags("B2C Integration");
    }
}

public class B2CPostRegistrationRequest
{
    public string ObjectId { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string InvitationToken { get; set; } = default!;
    public string? TenantId { get; set; }
}

public class B2CPostRegistrationResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? UserId { get; set; }
    public string? TenantId { get; set; }
    public string? ErrorMessage { get; set; }
}