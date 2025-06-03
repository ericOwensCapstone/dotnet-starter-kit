using System.Security.Claims;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Auth;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Graph.Services;
using FSH.Framework.Infrastructure.Identity.Persistence;
using FSH.Framework.Infrastructure.Identity.Roles;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Framework.Infrastructure.Tenant;
using FSH.Starter.Shared.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C;

public class B2CUserMappingService : IB2CUserMappingService
{
    private readonly AuthenticationDbContext _authDbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<B2CUserMappingService> _logger;
    private readonly AzureAdB2COptions _b2cOptions;
    private readonly IInvitationService _invitationService;
    private readonly UserManager<FshUser> _userManager;
    private readonly IGraphService _graphService;

    public B2CUserMappingService(
        AuthenticationDbContext authDbContext,
        IServiceProvider serviceProvider,
        ILogger<B2CUserMappingService> logger,
        IOptions<AuthenticationOptions> authOptions,
        IInvitationService invitationService,
        UserManager<FshUser> userManager,
        IGraphService graphService)
    {
        _authDbContext = authDbContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _b2cOptions = authOptions.Value.AzureAdB2C ?? new AzureAdB2COptions();
        _invitationService = invitationService;
        _userManager = userManager;
        _graphService = graphService;
    }

    public async Task<FshUser> GetOrCreateUserFromB2CClaimsAsync(ClaimsPrincipal principal, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing B2C claims for user authentication");
        
        // Log all available claims for debugging
        foreach (var claim in principal.Claims)
        {
            _logger.LogDebug("Claim: {Type} = {Value}", claim.Type, claim.Value);
        }
        
        var objectId = principal.FindFirst("oid")?.Value ?? 
                      principal.FindFirst("sub")?.Value ?? 
                      principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // B2C emails claim is typically an array, try different approaches
        var email = principal.FindFirst("emails")?.Value ?? 
                   principal.FindFirst("email")?.Value ?? 
                   principal.FindFirst(ClaimTypes.Email)?.Value;
        
        var displayName = principal.FindFirst("name")?.Value ?? principal.FindFirst(ClaimTypes.Name)?.Value;
        var givenName = principal.FindFirst("given_name")?.Value ?? principal.FindFirst(ClaimTypes.GivenName)?.Value;
        var surname = principal.FindFirst("family_name")?.Value ?? principal.FindFirst(ClaimTypes.Surname)?.Value;

        // Extract custom attributes if available
        var tenantId = principal.FindFirst("extension_TenantId")?.Value;
        var userStatus = principal.FindFirst("extension_UserStatus")?.Value;

        _logger.LogInformation("Extracted claims - ObjectId: {ObjectId}, Email: {Email}, DisplayName: {DisplayName}, TenantId: {TenantId}, UserStatus: {UserStatus}", 
            objectId ?? "NULL", email ?? "NULL", displayName ?? "NULL", tenantId ?? "NULL", userStatus ?? "NULL");

        if (string.IsNullOrEmpty(objectId) || string.IsNullOrEmpty(email))
        {
            _logger.LogError("Missing required B2C claims - ObjectId: {ObjectId}, Email: {Email}", 
                objectId ?? "NULL", email ?? "NULL");
            throw new UnauthorizedException("Invalid B2C token - missing required claims");
        }

        // First try to find user by B2C Object ID
        _logger.LogInformation("Searching for user with ObjectId: {ObjectId}", objectId);
        
        FshUser? user = null;
        if (!string.IsNullOrEmpty(objectId))
        {
            try
            {
                user = await _authDbContext.Users
                    .FirstOrDefaultAsync(u => u.ObjectId == objectId, cancellationToken);
                _logger.LogInformation("User found by ObjectId: {Found}", user != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error querying Users table for ObjectId: {ObjectId}", objectId);
                // Continue to email search if ObjectId query fails
            }
        }

        // If user not found by ObjectId, try by email
        if (user == null)
        {
            try
            {
                _logger.LogInformation("Searching for user by email: {Email}", email);
                user = await _authDbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
                
                _logger.LogInformation("User found by email: {Found}", user != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for user by email: {Email}", email);
            }

            if (user != null)
            {
                // Link existing user to B2C by updating ObjectId
                try
                {
                    user.ObjectId = objectId;
                    _authDbContext.Users.Update(user);
                    await _authDbContext.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Linked existing user {Email} to B2C ObjectId {ObjectId}", user.Email, objectId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error linking user to B2C ObjectId");
                    // Continue anyway - user is found, linking can be done later
                }
            }
        }

        // If user still not found, check if they have a valid invitation
        if (user == null)
        {
            _logger.LogInformation("User not found in system. Checking for valid invitations for email: {Email}", email);
            
            // Look for invitations for this email
            var invitations = await _invitationService.GetInvitationsByUserAsync(email, cancellationToken);
            var validInvitation = invitations
                .Where(i => i.Status == InvitationStatus.Sent && !i.IsExpired)
                .OrderByDescending(i => i.Created)
                .FirstOrDefault();

            if (validInvitation != null)
            {
                _logger.LogInformation("Found valid invitation {InvitationId} for {Email} in tenant {TenantId}. Auto-provisioning user.", 
                    validInvitation.Id, email, validInvitation.TenantId);

                try
                {
                    // Create new user from invitation
                    user = new FshUser
                    {
                        UserName = email,
                        Email = email,
                        FirstName = givenName ?? validInvitation.FirstName,
                        LastName = surname ?? validInvitation.LastName,
                        EmailConfirmed = true, // B2C handles email verification
                        ObjectId = objectId,
                        IsActive = true
                    };

                    // Create user with UserManager to handle all the setup
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to create user from invitation: {Errors}", errors);
                        throw new CustomException($"Failed to create user account: {errors}");
                    }

                    // Assign the user to the tenant from the invitation
                    await AssignUserToTenantAsync(user.Id, validInvitation.TenantId, cancellationToken);

                    // Assign role if specified in invitation
                    if (!string.IsNullOrEmpty(validInvitation.Role))
                    {
                        await _userManager.AddToRoleAsync(user, validInvitation.Role);
                        _logger.LogInformation("Assigned role {Role} to user {UserId}", validInvitation.Role, user.Id);
                    }

                    // Mark invitation as accepted
                    await _invitationService.AcceptInvitationAsync(validInvitation.InvitationToken, user.Id, cancellationToken);

                    // Update B2C user status to Active
                    try
                    {
                        await _graphService.UpdateUserCustomAttributesAsync(
                            objectId,
                            new Dictionary<string, object>
                            {
                                ["UserStatus"] = "Active",
                                ["TenantId"] = validInvitation.TenantId
                            },
                            cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to update B2C user attributes for {ObjectId}", objectId);
                    }

                    _logger.LogInformation("Successfully auto-provisioned user {Email} from invitation {InvitationId}", 
                        email, validInvitation.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error auto-provisioning user from invitation");
                    throw new CustomException($"Failed to provision user account from invitation: {ex.Message}");
                }
            }
            else
            {
                // No valid invitation found - check for legacy user mappings
                if (_b2cOptions.UserMappings.TryGetValue(email, out var targetEmail))
                {
                    _logger.LogInformation("Checking legacy mapping: B2C user {B2CEmail} maps to {TargetEmail}", email, targetEmail);
                    user = await _authDbContext.Users
                        .FirstOrDefaultAsync(u => u.Email == targetEmail, cancellationToken);
                    
                    if (user != null)
                    {
                        // Link the user to B2C
                        user.ObjectId = objectId;
                        _authDbContext.Users.Update(user);
                        await _authDbContext.SaveChangesAsync(cancellationToken);
                        _logger.LogInformation("Linked legacy mapped user {Email} to B2C ObjectId {ObjectId}", user.Email, objectId);
                    }
                }
                
                if (user == null)
                {
                    _logger.LogWarning("No valid invitation or existing user found for B2C user {Email}, ObjectId: {ObjectId}", 
                        email, objectId);
                    throw new UnauthorizedException($"No user account found for {email}. Please contact your administrator to request access.");
                }
            }
        }

        // Validate tenant assignment
        var userTenantId = await GetUserTenantIdAsync(user.Id, cancellationToken);
        
        if (string.IsNullOrEmpty(userTenantId))
        {
            _logger.LogError("User {UserId} has no TenantId assigned", user.Id);
            throw new UnauthorizedException("User has no tenant assignment.");
        }

        // Load the tenant info
        var tenantInfo = await _authDbContext.Tenants
            .FirstOrDefaultAsync(t => t.Id == userTenantId, cancellationToken);
            
        if (tenantInfo == null)
        {
            _logger.LogError("Tenant {TenantId} not found for user {UserId}", userTenantId, user.Id);
            throw new UnauthorizedException("User's tenant not found.");
        }

        if (!tenantInfo.IsActive)
        {
            _logger.LogWarning("User {UserId} attempted to access inactive tenant {TenantId}", user.Id, tenantInfo.Id);
            throw new UnauthorizedException("Your organization's account is not active. Please contact support.");
        }

        _logger.LogInformation("Found active tenant {TenantId} for user {UserId}", tenantInfo.Id, user.Id);

        return user;
    }
    
    private async Task AssignUserToTenantAsync(string userId, string tenantId, CancellationToken cancellationToken)
    {
        // Update user's TenantId using raw SQL to avoid multi-tenant context issues
        using var command = _authDbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "UPDATE identity.\"Users\" SET \"TenantId\" = @tenantId WHERE \"Id\" = @userId";
        
        var tenantParam = command.CreateParameter();
        tenantParam.ParameterName = "@tenantId";
        tenantParam.Value = tenantId;
        command.Parameters.Add(tenantParam);
        
        var userParam = command.CreateParameter();
        userParam.ParameterName = "@userId";
        userParam.Value = userId;
        command.Parameters.Add(userParam);
        
        await _authDbContext.Database.OpenConnectionAsync(cancellationToken);
        
        try
        {
            var rowsAffected = await command.ExecuteNonQueryAsync(cancellationToken);
            if (rowsAffected != 1)
            {
                _logger.LogError("Failed to assign user {UserId} to tenant {TenantId}. Rows affected: {RowsAffected}", 
                    userId, tenantId, rowsAffected);
                throw new CustomException("Failed to assign user to tenant.");
            }
            
            _logger.LogInformation("Successfully assigned user {UserId} to tenant {TenantId}", userId, tenantId);
        }
        finally
        {
            await _authDbContext.Database.CloseConnectionAsync();
        }
    }
    
    private async Task<string?> GetUserTenantIdAsync(string userId, CancellationToken cancellationToken)
    {
        // Query the TenantId directly from the Users table using raw SQL
        using var command = _authDbContext.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT \"TenantId\" FROM identity.\"Users\" WHERE \"Id\" = @userId";
        
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@userId";
        parameter.Value = userId;
        command.Parameters.Add(parameter);
        
        await _authDbContext.Database.OpenConnectionAsync(cancellationToken);
        
        try
        {
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result?.ToString();
        }
        finally
        {
            await _authDbContext.Database.CloseConnectionAsync();
        }
    }

    public async Task<List<Claim>> GetUserClaimsAsync(FshUser user, CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
            new(FshClaims.Fullname, $"{user.FirstName} {user.LastName}".Trim()),
            new(FshClaims.ImageUrl, user.ImageUrl?.ToString() ?? string.Empty),
            new("email", user.Email!),
            new("given_name", user.FirstName ?? string.Empty),
            new("family_name", user.LastName ?? string.Empty)
        };

        // Add B2C Object ID if present
        if (!string.IsNullOrEmpty(user.ObjectId))
        {
            claims.Add(new Claim("oid", user.ObjectId));
        }

        // Get user's tenant information
        var userTenantId = await GetUserTenantIdAsync(user.Id, cancellationToken);
        if (string.IsNullOrEmpty(userTenantId))
        {
            _logger.LogError("User {UserId} has no TenantId assigned", user.Id);
            throw new UnauthorizedException("User has no tenant assignment.");
        }

        // Load the tenant info
        var tenantInfo = await _authDbContext.Tenants
            .FirstOrDefaultAsync(t => t.Id == userTenantId, cancellationToken);
            
        if (tenantInfo == null)
        {
            _logger.LogError("Tenant {TenantId} not found for user {UserId}", userTenantId, user.Id);
            throw new UnauthorizedException("User's tenant not found.");
        }

        // Add tenant claim
        claims.Add(new Claim(FshClaims.Tenant, tenantInfo.Id));
        
        _logger.LogInformation("Creating tenant-aware IdentityDbContext for tenant {TenantId}", tenantInfo.Id);
        
        try
        {
            // Create a properly configured IdentityDbContext with tenant connection string
            var dbOptions = new DbContextOptionsBuilder<IdentityDbContext>();
            
            // Use the tenant's connection string (same logic as regular IdentityDbContext)
            var connectionString = !string.IsNullOrWhiteSpace(tenantInfo.ConnectionString) 
                ? tenantInfo.ConnectionString 
                : _serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString;
                
            dbOptions.UseNpgsql(connectionString);
            
            // Create a minimal context accessor for this specific tenant
            var tempContextAccessor = new TempMultiTenantContextAccessor(tenantInfo);
            
            using var identityDbContext = new IdentityDbContext(
                tempContextAccessor, 
                dbOptions.Options, 
                _serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>());
            
            // Query user roles directly from the tenant-scoped database
            var userRoles = await identityDbContext.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Join(identityDbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                .ToListAsync(cancellationToken);
            
            _logger.LogInformation("User roles retrieved: {RoleCount} roles for user {UserId}", userRoles.Count, user.Id);
            
            foreach (var role in userRoles.Where(r => !string.IsNullOrEmpty(r)))
            {
                claims.Add(new Claim(ClaimTypes.Role, role!));
            }
            
            // Query permissions directly from role claims
            var roleIds = await identityDbContext.UserRoles
                .Where(ur => ur.UserId == user.Id)
                .Select(ur => ur.RoleId)
                .ToListAsync(cancellationToken);

            var permissions = await identityDbContext.RoleClaims
                .Where(rc => roleIds.Contains(rc.RoleId) && rc.ClaimType == FshClaims.Permission)
                .Select(rc => rc.ClaimValue!)
                .Distinct()
                .ToListAsync(cancellationToken);

            foreach (var permission in permissions.Where(p => !string.IsNullOrEmpty(p)))
            {
                claims.Add(new Claim(FshClaims.Permission, permission!));
            }
            
            _logger.LogInformation("Successfully retrieved {PermissionCount} permissions for user {UserId}", 
                permissions.Count, user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user claims. Tenant: {TenantId}, UserId: {UserId}", 
                tenantInfo.Id, user.Id);
            throw;
        }

        return claims;
    }

    // Temporary context accessor to provide tenant info for IdentityDbContext
    private sealed class TempMultiTenantContextAccessor : IMultiTenantContextAccessor<FshTenantInfo>
    {
        public IMultiTenantContext<FshTenantInfo> MultiTenantContext { get; }

        IMultiTenantContext IMultiTenantContextAccessor.MultiTenantContext => MultiTenantContext;

        public TempMultiTenantContextAccessor(FshTenantInfo tenantInfo)
        {
            MultiTenantContext = new MultiTenantContext<FshTenantInfo>
            {
                TenantInfo = tenantInfo
            };
        }
    }
}