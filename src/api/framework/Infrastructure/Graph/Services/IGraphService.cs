using FSH.Framework.Infrastructure.Graph.Models;

namespace FSH.Framework.Infrastructure.Graph.Services;

public interface IGraphService
{
    // User Management
    Task<GraphUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<GraphUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<GraphUser>> GetUsersAsync(string? filter = null, CancellationToken cancellationToken = default);
    Task<List<GraphUser>> GetUsersByTenantAsync(string tenantId, CancellationToken cancellationToken = default);
    
    // User Creation and Updates
    Task<GraphUser> CreateUserAsync(GraphUser user, string temporaryPassword, CancellationToken cancellationToken = default);
    Task<GraphUser> UpdateUserAsync(string userId, GraphUser user, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserCustomAttributesAsync(string userId, Dictionary<string, object> attributes, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    
    // B2C Deleted User Management
    Task<bool> PermanentlyDeleteUserAsync(string objectId, CancellationToken cancellationToken = default);
    Task<List<GraphUser>> GetDeletedUsersAsync(CancellationToken cancellationToken = default);
    
    // User Status Management
    Task<bool> EnableUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<bool> DisableUserAsync(string userId, CancellationToken cancellationToken = default);
    
    // Invitation Management
    Task<string> CreateInvitationAsync(GraphInvitation invitation, CancellationToken cancellationToken = default);
    
    // Password Management
    Task<bool> ResetPasswordAsync(string userId, string newPassword, CancellationToken cancellationToken = default);
    Task<string> GenerateTemporaryPassword();
}