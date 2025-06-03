using FSH.Framework.Core.Persistence;

namespace FSH.Framework.Core.Identity.Invitations;

public interface IInvitationRepository : IRepository<UserInvitation>
{
    Task<UserInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<UserInvitation?> GetByEmailAndTenantAsync(string email, string tenantId, CancellationToken cancellationToken = default);
    Task<List<UserInvitation>> GetByTenantAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<List<UserInvitation>> GetByStatusAsync(InvitationStatus status, CancellationToken cancellationToken = default);
    Task<List<UserInvitation>> GetExpiredAsync(CancellationToken cancellationToken = default);
    Task<List<UserInvitation>> GetPendingAsync(string? tenantId = null, CancellationToken cancellationToken = default);
    Task<bool> ExistsActiveInvitationAsync(string email, string tenantId, CancellationToken cancellationToken = default);
}