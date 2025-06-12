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
    private readonly IdentityDbContext _dbContext;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<InvitationRepository>? _logger;
    
    public InvitationRepository(IdentityDbContext context, ICurrentUser currentUser, ILogger<InvitationRepository>? logger = null) : base(context)
    {
        _dbContext = context ?? throw new ArgumentNullException(nameof(context));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _logger = logger;
    }

    private IQueryable<UserInvitation> GetFilteredInvitations()
    {
        var query = _dbContext.Set<UserInvitation>().AsQueryable();
        
        // Check if user is authenticated
        if (_currentUser.IsAuthenticated())
        {
            // All users (including root admin) can only see invitations they created (by TenantId ownership)
            var currentTenantId = _currentUser.GetTenant();
            var currentUserId = _currentUser.GetUserId();
            var userEmail = _currentUser.GetUserEmail();
            
            _logger?.LogInformation("GetFilteredInvitations - Current User: {UserId}, Email: {Email}, TenantId: {TenantId}", 
                currentUserId, userEmail, currentTenantId);
            
            var filteredQuery = query.Where(x => x.TenantId == currentTenantId);
            
            // Debug: Log the generated SQL query
            try
            {
                _logger?.LogInformation("Repository filter SQL: {Query}", filteredQuery.ToQueryString());
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Unable to get query string for logging");
            }
            
            return filteredQuery;
        }
        else
        {
            _logger?.LogInformation("GetFilteredInvitations - Anonymous access, no filtering applied");
            return query;
        }
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
        if (string.IsNullOrEmpty(token))
        {
            _logger?.LogWarning("GetByTokenAsync called with null or empty token");
            return null;
        }

        try
        {
            _logger?.LogInformation("GetByTokenAsync: Looking for invitation with token: {Token}", token);
            
            // Token lookup doesn't need tenant filtering for invitation acceptance
            // First, let's check if we can access the UserInvitation set
            var invitationSet = _dbContext.Set<UserInvitation>();
            if (invitationSet == null)
            {
                _logger?.LogError("GetByTokenAsync: Unable to get UserInvitation DbSet");
                throw new InvalidOperationException("Unable to access UserInvitation entities");
            }
            
            // Log the SQL query that will be executed
            var query = invitationSet.Where(x => x.InvitationToken != null && x.InvitationToken == token);
            
            // ToQueryString() can fail with anonymous access, so wrap in try-catch
            try
            {
                _logger?.LogInformation("GetByTokenAsync: Executing query: {Query}", query.ToQueryString());
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Unable to get query string for logging");
            }
            
            var invitation = await query.FirstOrDefaultAsync(cancellationToken);
            
            if (invitation == null)
            {
                _logger?.LogWarning("GetByTokenAsync: No invitation found for token: {Token}", token);
                
                // Let's check if there are any invitations at all
                var totalCount = await invitationSet.CountAsync(cancellationToken);
                _logger?.LogInformation("GetByTokenAsync: Total invitations in database: {Count}", totalCount);
                
                // Log a few sample tokens to help debug
                var sampleTokens = await invitationSet
                    .Where(x => x.InvitationToken != null)
                    .Select(x => x.InvitationToken)
                    .Take(5)
                    .ToListAsync(cancellationToken);
                    
                _logger?.LogInformation("GetByTokenAsync: Sample invitation tokens: {Tokens}", string.Join(", ", sampleTokens));
            }
            else
            {
                _logger?.LogInformation("GetByTokenAsync: Found invitation - Id: {Id}, Email: {Email}, Status: {Status}",
                    invitation.Id, invitation.Email, invitation.Status);
            }
            
            return invitation;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving invitation by token: {Token}", token);
            throw;
        }
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