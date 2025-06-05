using FSH.Framework.Core.Identity.Invitations.Features;
using FSH.Framework.Core.Identity.Invitations.Features.ExtendInvitationExpiration;
using FSH.Framework.Core.Identity.Invitations.Features.SearchInvitations;
using FSH.Framework.Core.Paging;

namespace FSH.Framework.Core.Identity.Invitations;

public interface IInvitationService
{
    // Invitation Management
    Task<CreateInvitationResponse> CreateInvitationAsync(CreateInvitationRequest request, CancellationToken cancellationToken = default);
    Task<bool> ResendInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default);
    Task<bool> CancelInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default);
    Task<ExtendInvitationExpirationResponse> ExtendInvitationExpirationAsync(ExtendInvitationExpirationRequest request, CancellationToken cancellationToken = default);
    Task<UserInvitation?> GetInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default);
    Task<UserInvitation?> GetInvitationByTokenAsync(string token, CancellationToken cancellationToken = default);
    
    // Invitation Queries
    Task<PagedList<InvitationDto>> SearchInvitationsAsync(SearchInvitationsQuery request, CancellationToken cancellationToken = default);
    Task<List<UserInvitation>> GetPendingInvitationsAsync(string? tenantId = null, CancellationToken cancellationToken = default);
    Task<List<UserInvitation>> GetInvitationsByTenantAsync(string tenantId, CancellationToken cancellationToken = default);
    Task<List<UserInvitation>> GetInvitationsByUserAsync(string userEmail, CancellationToken cancellationToken = default);
    
    // Invitation Processing
    Task ProcessPendingInvitationsAsync(CancellationToken cancellationToken = default);
    Task ProcessExpiredInvitationsAsync(CancellationToken cancellationToken = default);
    Task<bool> AcceptInvitationAsync(string token, string userId, CancellationToken cancellationToken = default);
    
    // Validation
    Task<bool> IsEmailInvitedAsync(string email, string? tenantId = null, CancellationToken cancellationToken = default);
    Task<bool> HasPendingInvitationAsync(string email, string tenantId, CancellationToken cancellationToken = default);
}