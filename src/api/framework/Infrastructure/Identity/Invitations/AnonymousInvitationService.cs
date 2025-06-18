using FSH.Framework.Core.Domain.Contracts;
using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Core.Identity.Invitations.Features;
using FSH.Framework.Core.Identity.Invitations.Features.ExtendInvitationExpiration;
using FSH.Framework.Core.Identity.Invitations.Features.SearchInvitations;
using FSH.Framework.Core.Identity.Invitations.Specifications;
using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

/// <summary>
/// Invitation service for anonymous endpoints that bypasses multi-tenant filtering.
/// Used by B2C endpoints where there is no authenticated user context.
/// </summary>
public sealed class AnonymousInvitationService : IInvitationService
{
    private readonly AnonymousInvitationRepository _repository;
    private readonly ILogger<AnonymousInvitationService> _logger;

    public AnonymousInvitationService(
        AnonymousInvitationRepository repository,
        ILogger<AnonymousInvitationService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateInvitationResponse> CreateInvitationAsync(CreateInvitationRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Creating invitations is not supported in anonymous context");
    }

    public async Task<UserInvitation?> GetInvitationByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("AnonymousInvitationService.GetInvitationByTokenAsync: Looking for invitation with token: {Token}", token);
        var invitation = await _repository.GetByTokenAsync(token, cancellationToken);
        
        if (invitation != null)
        {
            _logger.LogInformation("AnonymousInvitationService.GetInvitationByTokenAsync: Found invitation - Id: {Id}, Email: {Email}, Status: {Status}", 
                invitation.Id, invitation.Email, invitation.Status);
        }
        else
        {
            _logger.LogWarning("AnonymousInvitationService.GetInvitationByTokenAsync: No invitation found for token: {Token}", token);
        }
        
        return invitation;
    }

    public async Task<UserInvitation?> GetInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(invitationId, cancellationToken);
    }

    public async Task<UserInvitation?> GetInvitationByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _repository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<UserInvitation?> GetInvitationByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _repository.FirstOrDefaultAsync(new InvitationsByEmailSpec(email), cancellationToken);
    }
    
    public async Task<List<UserInvitation>> GetInvitationsByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _repository.ListAsync(new InvitationsByEmailSpec(email), cancellationToken);
    }

    public async Task<bool> AcceptInvitationAsync(string token, string userId, CancellationToken cancellationToken = default)
    {
        var invitation = await _repository.GetByTokenAsync(token, cancellationToken);
        if (invitation == null)
        {
            return false;
        }

        invitation.Accept(userId);
        await _repository.UpdateAsync(invitation, cancellationToken);
        _logger.LogInformation("Invitation {InvitationId} accepted by user {UserId}", invitation.Id, userId);
        return true;
    }

    public async Task<bool> CancelInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Canceling invitations is not supported in anonymous context");
    }

    public async Task<ExtendInvitationExpirationResponse> ExtendInvitationExpirationAsync(ExtendInvitationExpirationRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Extending invitations is not supported in anonymous context");
    }

    public async Task<List<UserInvitation>> GetPendingInvitationsAsync(string? tenantId = null, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Listing invitations is not supported in anonymous context");
    }

    public Task<PagedList<InvitationDto>> SearchInvitationsAsync(SearchInvitationsQuery request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Searching invitations is not supported in anonymous context");
    }

    public async Task<bool> ResendInvitationAsync(Guid invitationId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Resending invitations is not supported in anonymous context");
    }

    public async Task<List<UserInvitation>> GetInvitationsByTenantAsync(string tenantId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Listing invitations by tenant is not supported in anonymous context");
    }

    public async Task<List<UserInvitation>> GetInvitationsByUserAsync(string userEmail, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Listing invitations by user is not supported in anonymous context");
    }

    public async Task ProcessExpiredInvitationsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Processing expired invitations is not supported in anonymous context");
    }

    public async Task ProcessPendingInvitationsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("Processing pending invitations is not supported in anonymous context");
    }

    public async Task<bool> IsEmailInvitedAsync(string email, string? tenantId = null, CancellationToken cancellationToken = default)
    {
        var invitation = await _repository.FirstOrDefaultAsync(new InvitationsByEmailSpec(email), cancellationToken);
        return invitation != null && invitation.Status == InvitationStatus.Sent;
    }

    public async Task<bool> HasPendingInvitationAsync(string email, string tenantId, CancellationToken cancellationToken = default)
    {
        var invitation = await _repository.FirstOrDefaultAsync(new InvitationsByEmailSpec(email), cancellationToken);
        return invitation != null && invitation.Status == InvitationStatus.Sent && invitation.TargetTenantId == tenantId;
    }
}