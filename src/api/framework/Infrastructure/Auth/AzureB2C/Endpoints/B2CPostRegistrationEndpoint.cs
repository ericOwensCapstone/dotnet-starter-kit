using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Infrastructure.Identity.Invitations;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Identity.Persistence;
using FSH.Framework.Infrastructure.Identity.Roles;
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
                    var authDbContext = serviceProvider.GetRequiredService<AuthenticationDbContext>();

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

                    // Find the user by ObjectId using direct query to avoid multi-tenant issues
                    FshUser? user = null;
                    try
                    {
                        user = await authDbContext.Users
                            .FirstOrDefaultAsync(u => u.ObjectId == request.ObjectId, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error finding user by ObjectId: {ObjectId}", request.ObjectId);
                    }

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
                            // Create user using raw SQL to bypass tenant filtering
                            using var command = authDbContext.Database.GetDbConnection().CreateCommand();
                            command.CommandText = @"
                                INSERT INTO identity.""Users"" (
                                    ""Id"", ""UserName"", ""NormalizedUserName"", ""Email"", ""NormalizedEmail"",
                                    ""EmailConfirmed"", ""PasswordHash"", ""SecurityStamp"", ""ConcurrencyStamp"",
                                    ""PhoneNumber"", ""PhoneNumberConfirmed"", ""TwoFactorEnabled"", ""LockoutEnd"",
                                    ""LockoutEnabled"", ""AccessFailedCount"", ""FirstName"", ""LastName"",
                                    ""ImageUrl"", ""IsActive"", ""RefreshToken"", ""RefreshTokenExpiryTime"",
                                    ""ObjectId"", ""TenantId""
                                ) VALUES (
                                    @id, @userName, @normalizedUserName, @email, @normalizedEmail,
                                    @emailConfirmed, NULL, @securityStamp, @concurrencyStamp,
                                    NULL, false, false, NULL,
                                    true, 0, @firstName, @lastName,
                                    NULL, @isActive, NULL, NULL,
                                    @objectId, @tenantId
                                )";

                            command.Parameters.Add(CreateParameter(command, "@id", user.Id));
                            command.Parameters.Add(CreateParameter(command, "@userName", user.UserName));
                            command.Parameters.Add(CreateParameter(command, "@normalizedUserName", user.NormalizedUserName));
                            command.Parameters.Add(CreateParameter(command, "@email", user.Email));
                            command.Parameters.Add(CreateParameter(command, "@normalizedEmail", user.NormalizedEmail));
                            command.Parameters.Add(CreateParameter(command, "@emailConfirmed", user.EmailConfirmed));
                            command.Parameters.Add(CreateParameter(command, "@securityStamp", user.SecurityStamp));
                            command.Parameters.Add(CreateParameter(command, "@concurrencyStamp", user.ConcurrencyStamp));
                            command.Parameters.Add(CreateParameter(command, "@firstName", user.FirstName));
                            command.Parameters.Add(CreateParameter(command, "@lastName", user.LastName));
                            command.Parameters.Add(CreateParameter(command, "@isActive", user.IsActive));
                            command.Parameters.Add(CreateParameter(command, "@objectId", user.ObjectId));
                            command.Parameters.Add(CreateParameter(command, "@tenantId", invitation.TargetTenantId));

                            await authDbContext.Database.OpenConnectionAsync(cancellationToken);
                            
                            var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
                            
                            await authDbContext.Database.CloseConnectionAsync();
                            
                            if (rowsAffected != 1)
                            {
                                logger.LogError("Failed to create user. Rows affected: {RowsAffected}", rowsAffected);
                                throw new InvalidOperationException("Failed to create user in database.");
                            }
                            
                            logger.LogInformation("Created user {UserId} in database with TenantId {TenantId}", user.Id, invitation.TargetTenantId);

                            // Note: Role assignment will be handled during first sign-in by B2CUserMappingService
                            // This avoids multi-tenant context issues in the anonymous endpoint
                            if (!string.IsNullOrEmpty(invitation.Role))
                            {
                                logger.LogInformation("User will be assigned role {Role} during first sign-in", invitation.Role);
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

    private static async Task AssignUserToTenantAsync(
        AuthenticationDbContext authDbContext, 
        string userId, 
        string tenantId, 
        ILogger logger,
        CancellationToken cancellationToken)
    {
        // Update user's TenantId using raw SQL to avoid multi-tenant context issues
        using var command = authDbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "UPDATE identity.\"Users\" SET \"TenantId\" = @tenantId WHERE \"Id\" = @userId";
        
        var tenantParam = command.CreateParameter();
        tenantParam.ParameterName = "@tenantId";
        tenantParam.Value = tenantId;
        command.Parameters.Add(tenantParam);
        
        var userParam = command.CreateParameter();
        userParam.ParameterName = "@userId";
        userParam.Value = userId;
        command.Parameters.Add(userParam);
        
        await authDbContext.Database.OpenConnectionAsync(cancellationToken);
        
        try
        {
            var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            if (rowsAffected != 1)
            {
                logger.LogError("Failed to assign user {UserId} to tenant {TenantId}. Rows affected: {RowsAffected}", 
                    userId, tenantId, rowsAffected);
                throw new InvalidOperationException("Failed to assign user to tenant.");
            }
            
            logger.LogInformation("Successfully assigned user {UserId} to tenant {TenantId}", userId, tenantId);
        }
        finally
        {
            await authDbContext.Database.CloseConnectionAsync();
        }
    }

    private static System.Data.Common.DbParameter CreateParameter(System.Data.Common.DbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        return parameter;
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