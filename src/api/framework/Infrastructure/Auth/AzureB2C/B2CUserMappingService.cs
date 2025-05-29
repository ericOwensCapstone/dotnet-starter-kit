using System.Security.Claims;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using FSH.Framework.Core.Auth;
using FSH.Framework.Core.Exceptions;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Persistence;
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

    public B2CUserMappingService(
        AuthenticationDbContext authDbContext,
        IServiceProvider serviceProvider,
        ILogger<B2CUserMappingService> logger,
        IOptions<AuthenticationOptions> authOptions)
    {
        _authDbContext = authDbContext;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _b2cOptions = authOptions.Value.AzureAdB2C ?? new AzureAdB2COptions();
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

        _logger.LogInformation("Extracted claims - ObjectId: {ObjectId}, Email: {Email}, DisplayName: {DisplayName}", 
            objectId ?? "NULL", email ?? "NULL", displayName ?? "NULL");

        if (string.IsNullOrEmpty(objectId) || string.IsNullOrEmpty(email))
        {
            _logger.LogError("Missing required B2C claims - ObjectId: {ObjectId}, Email: {Email}", 
                objectId ?? "NULL", email ?? "NULL");
            throw new UnauthorizedException("Invalid B2C token - missing required claims");
        }

        // Check if this B2C user should be mapped to a specific local user
        string mappedEmail = email;
        if (_b2cOptions.UserMappings.TryGetValue(email, out var targetEmail))
        {
            mappedEmail = targetEmail;
            _logger.LogInformation("Mapping B2C user {B2CEmail} to local user {LocalEmail}", email, targetEmail);
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

        if (user == null)
        {
            try
            {
                // Try to find by mapped email first
                _logger.LogInformation("Searching for user by mapped email: {MappedEmail}", mappedEmail);
                user = await _authDbContext.Users
                    .FirstOrDefaultAsync(u => u.Email == mappedEmail, cancellationToken);
                
                _logger.LogInformation("User found by mapped email: {Found}", user != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching for user by email: {MappedEmail}", mappedEmail);
                // Continue to user creation if email search fails
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
            else
            {
                // No existing user found - fail authentication
                _logger.LogWarning("No existing user found for B2C user. B2C Email: {B2CEmail}, Mapped Email: {MappedEmail}, ObjectId: {ObjectId}", 
                    email, mappedEmail, objectId);
                throw new UnauthorizedException($"No user account found. Please contact your administrator to create an account for {email}.");
            }
        }

        // Store the tenant info on the user object for later use
        var userTenantId = await GetUserTenantIdAsync(user.Id, cancellationToken);
        
        if (string.IsNullOrEmpty(userTenantId))
        {
            _logger.LogError("User {UserId} has no TenantId assigned", user.Id);
            throw new UnauthorizedException("User has no tenant assignment.");
        }

        // Load the tenant info and store it on the user object temporarily
        var tenantInfo = await _authDbContext.Tenants
            .FirstOrDefaultAsync(t => t.Id == userTenantId, cancellationToken);
            
        if (tenantInfo == null)
        {
            _logger.LogError("Tenant {TenantId} not found for user {UserId}", userTenantId, user.Id);
            throw new UnauthorizedException("User's tenant not found.");
        }

        _logger.LogInformation("Found tenant {TenantId} for user {UserId}", tenantInfo.Id, user.Id);

        // Return the user - we'll pass tenant info separately to GetUserClaimsAsync
        return user;
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