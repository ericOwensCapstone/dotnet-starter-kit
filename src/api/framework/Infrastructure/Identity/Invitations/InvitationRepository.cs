using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Identity.Users.Abstractions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Identity.Persistence;
using FSH.Starter.Shared.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

public class InvitationRepository : RepositoryBase<UserInvitation>, IInvitationRepository, IReadRepository<UserInvitation>, IRepository<UserInvitation>
{
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<InvitationRepository>? _logger;
    
    public InvitationRepository(IdentityDbContext context, ICurrentUser currentUser, ILogger<InvitationRepository>? logger = null) : base(context)
    {
        _dbContext = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    private IQueryable<UserInvitation> GetFilteredInvitations()
    {
        var query = _dbContext.Set<UserInvitation>().AsQueryable();
        
        // All users (including root admin) can only see invitations they created (by TenantId ownership)
        var currentTenantId = _currentUser.GetTenant();
        var currentUserId = _currentUser.GetUserId();
        var userEmail = _currentUser.GetUserEmail();
        
        _logger?.LogInformation("GetFilteredInvitations - Current User: {UserId}, Email: {Email}, TenantId: {TenantId}", 
            currentUserId, userEmail, currentTenantId);
        
        var filteredQuery = query.Where(x => x.TenantId == currentTenantId);
        
        // Debug: Log the generated SQL query
        _logger?.LogInformation("Repository filter SQL: {Query}", filteredQuery.ToQueryString());
        
        return filteredQuery;
    }
    
    // Debug method to log all invitations in database (no filtering)
    public async Task LogAllInvitationsAsync()
    {
        try
        {
            var allInvitations = await _dbContext.Set<UserInvitation>()
                .OrderByDescending(x => x.Created)
                .ToListAsync();
            
            _logger?.LogInformation("=== ALL INVITATIONS IN DATABASE (Count: {Count}) ===", allInvitations.Count);
            
            foreach (var invitation in allInvitations)
            {
                _logger?.LogInformation("DB Invitation: Id={Id}, Email={Email}, TenantId={TenantId}, TargetTenantId={TargetTenantId}, Status={Status}, Created={Created}", 
                    invitation.Id, invitation.Email, invitation.TenantId, invitation.TargetTenantId, invitation.Status, invitation.Created);
            }
            
            _logger?.LogInformation("=== END ALL INVITATIONS ===");
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error logging all invitations");
        }
    }

    public async Task<UserInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        // Token lookup doesn't need tenant filtering for invitation acceptance
        return await _dbContext.Set<UserInvitation>()
            .FirstOrDefaultAsync(x => x.InvitationToken == token, cancellationToken);
    }

    public async Task<UserInvitation?> GetByEmailAndTenantAsync(string email, string targetTenantId, CancellationToken cancellationToken = default)
    {
        return await GetFilteredInvitations()
            .Where(x => x.Email == email.ToLowerInvariant() && x.TargetTenantId == targetTenantId)
            .OrderByDescending(x => x.Created)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<UserInvitation>> GetByTenantAsync(string targetTenantId, CancellationToken cancellationToken = default)
    {
        return await GetFilteredInvitations()
            .Where(x => x.TargetTenantId == targetTenantId)
            .OrderByDescending(x => x.Created)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserInvitation>> GetByStatusAsync(InvitationStatus status, CancellationToken cancellationToken = default)
    {
        return await GetFilteredInvitations()
            .Where(x => x.Status == status)
            .OrderBy(x => x.Created)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserInvitation>> GetExpiredAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await GetFilteredInvitations()
            .Where(x => x.ExpiresAt < now && 
                       (x.Status == InvitationStatus.Pending || x.Status == InvitationStatus.Sent))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserInvitation>> GetPendingAsync(string? targetTenantId = null, CancellationToken cancellationToken = default)
    {
        var query = GetFilteredInvitations().Where(x => x.Status == InvitationStatus.Pending);
        
        if (!string.IsNullOrEmpty(targetTenantId))
        {
            query = query.Where(x => x.TargetTenantId == targetTenantId);
        }

        return await query
            .OrderBy(x => x.Created)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsActiveInvitationAsync(string email, string targetTenantId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await GetFilteredInvitations()
            .AnyAsync(x => x.Email == email.ToLowerInvariant() && 
                          x.TargetTenantId == targetTenantId &&
                          (x.Status == InvitationStatus.Pending || x.Status == InvitationStatus.Sent) &&
                          x.ExpiresAt > now, 
                     cancellationToken);
    }

    private readonly IdentityDbContext _dbContext;
    
    public override async Task<UserInvitation?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default)
    {
        return await GetFilteredInvitations()
            .FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
    }
    
    public override async Task<List<UserInvitation>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await GetFilteredInvitations()
            .ToListAsync(cancellationToken);
    }
}