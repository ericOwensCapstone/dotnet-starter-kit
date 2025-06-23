using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Infrastructure.Identity.Invitations;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Framework.Infrastructure.Identity.Roles;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Linq;

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
                    var tenantFreeDbContext = serviceProvider.GetRequiredService<TenantFreeDbContext>();

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

                    // Find the user by ObjectId using TenantFreeDbContext
                    var user = await tenantFreeDbContext.Users
                        .FirstOrDefaultAsync(u => u.ObjectId == request.ObjectId, cancellationToken);

                    if (user == null)
                    {
                        logger.LogInformation("User not found with ObjectId: {ObjectId}. Creating new user from invitation.", request.ObjectId);
                        
                        // Create new user from invitation directly in database
                        user = new FshUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = request.Email,
                            NormalizedUserName = request.Email.ToUpperInvariant(),
                            Email = request.Email,
                            NormalizedEmail = request.Email.ToUpperInvariant(),
                            FirstName = invitation.FirstName ?? string.Empty,
                            LastName = invitation.LastName ?? string.Empty,
                            EmailConfirmed = true, // B2C handles email verification
                            ObjectId = request.ObjectId,
                            IsActive = true,
                            SecurityStamp = Guid.NewGuid().ToString(),
                            ConcurrencyStamp = Guid.NewGuid().ToString()
                        };

                        try
                        {
                            // Add user to TenantFreeDbContext
                            tenantFreeDbContext.Users.Add(user);
                            
                            // Set TenantId shadow property
                            tenantFreeDbContext.Entry(user).Property("TenantId").CurrentValue = invitation.TargetTenantId;
                            
                            await tenantFreeDbContext.SaveChangesAsync(cancellationToken);
                            
                            logger.LogInformation("Created user {UserId} in database with TenantId {TenantId}", user.Id, invitation.TargetTenantId);

                            // Assign role from invitation if specified
                            if (!string.IsNullOrEmpty(invitation.Role))
                            {
                                logger.LogInformation("Assigning role {Role} to user {UserId}", invitation.Role, user.Id);
                                
                                // Get the role using TenantFreeDbContext with shadow property
                                var role = await tenantFreeDbContext.Roles
                                    .Where(r => r.Name == invitation.Role && EF.Property<string>(r, "TenantId") == invitation.TargetTenantId)
                                    .FirstOrDefaultAsync(cancellationToken);
                                    
                                if (role != null)
                                {
                                    // Create new UserRole
                                    var newUserRole = new IdentityUserRole<string>
                                    {
                                        UserId = user.Id,
                                        RoleId = role.Id
                                    };
                                    
                                    tenantFreeDbContext.UserRoles.Add(newUserRole);
                                    
                                    // Set TenantId shadow property
                                    tenantFreeDbContext.Entry(newUserRole).Property("TenantId").CurrentValue = invitation.TargetTenantId;
                                    
                                    await tenantFreeDbContext.SaveChangesAsync(cancellationToken);
                                    
                                    logger.LogInformation("Successfully assigned role {Role} to user {UserId} in tenant {TenantId}", 
                                        invitation.Role, user.Id, invitation.TargetTenantId);
                                }
                                else
                                {
                                    logger.LogWarning("Role {Role} not found for tenant {TenantId}", 
                                        invitation.Role, invitation.TargetTenantId);
                                }
                            }

                            logger.LogInformation("Successfully created user {UserId} from invitation {InvitationId}", 
                                user.Id, invitation.Id);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex, "Failed to create user in database");
                            return Results.Ok(new B2CPostRegistrationResponse
                            {
                                Success = false,
                                ErrorMessage = "Failed to create user account."
                            });
                        }
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