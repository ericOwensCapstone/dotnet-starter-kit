using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Identity.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

public class InvitationRepository : RepositoryBase<UserInvitation>, IInvitationRepository, IReadRepository<UserInvitation>, IRepository<UserInvitation>
{
    public InvitationRepository(IdentityDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<UserInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<UserInvitation>()
            .FirstOrDefaultAsync(x => x.InvitationToken == token, cancellationToken);
    }

    public async Task<UserInvitation?> GetByEmailAndTenantAsync(string email, string tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<UserInvitation>()
            .Where(x => x.Email == email.ToLowerInvariant() && x.TenantId == tenantId)
            .OrderByDescending(x => x.Created)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<UserInvitation>> GetByTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<UserInvitation>()
            .Where(x => x.TenantId == tenantId)
            .OrderByDescending(x => x.Created)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserInvitation>> GetByStatusAsync(InvitationStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Set<UserInvitation>()
            .Where(x => x.Status == status)
            .OrderBy(x => x.Created)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserInvitation>> GetExpiredAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Set<UserInvitation>()
            .Where(x => x.ExpiresAt < now && 
                       (x.Status == InvitationStatus.Pending || x.Status == InvitationStatus.Sent))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<UserInvitation>> GetPendingAsync(string? tenantId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Set<UserInvitation>().Where(x => x.Status == InvitationStatus.Pending);
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            query = query.Where(x => x.TenantId == tenantId);
        }

        return await query
            .OrderBy(x => x.Created)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsActiveInvitationAsync(string email, string tenantId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Set<UserInvitation>()
            .AnyAsync(x => x.Email == email.ToLowerInvariant() && 
                          x.TenantId == tenantId &&
                          (x.Status == InvitationStatus.Pending || x.Status == InvitationStatus.Sent) &&
                          x.ExpiresAt > now, 
                     cancellationToken);
    }

    private readonly IdentityDbContext _dbContext;
}