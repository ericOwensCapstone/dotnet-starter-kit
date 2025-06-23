using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using FSH.Framework.Core.Identity.Invitations;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

/// <summary>
/// Repository for anonymous access to invitations (e.g., validation endpoints)
/// This uses TenantFreeDbContext to bypass multi-tenant filtering for invitation validation without authentication
/// </summary>
public class AnonymousInvitationRepository : IAnonymousInvitationRepository
{
    private readonly TenantFreeDbContext _context;
    private readonly ILogger<AnonymousInvitationRepository>? _logger;

    public AnonymousInvitationRepository(
        TenantFreeDbContext context,
        ILogger<AnonymousInvitationRepository>? logger = null)
    {
        _context = context;
        _logger = logger;
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
            _logger?.LogInformation("AnonymousInvitationRepository.GetByTokenAsync: Looking for invitation with token: {Token}", token);
            
            var invitation = await _context.UserInvitations
                .Where(x => x.InvitationToken == token)
                .FirstOrDefaultAsync(cancellationToken);
                
            if (invitation == null)
            {
                _logger?.LogWarning("AnonymousInvitationRepository.GetByTokenAsync: No invitation found for token: {Token}", token);
            }
            else
            {
                _logger?.LogInformation("AnonymousInvitationRepository.GetByTokenAsync: Found invitation - Id: {Id}, Email: {Email}, Status: {Status}",
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

    public async Task<UserInvitation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInvitations
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving invitation by id: {Id}", id);
            throw;
        }
    }

    public async Task<UserInvitation?> FirstOrDefaultAsync(ISpecification<UserInvitation> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInvitations
                .WithSpecification(specification)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving invitation by specification");
            throw;
        }
    }

    public async Task UpdateAsync(UserInvitation invitation, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.UserInvitations.Update(invitation);
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger?.LogInformation("Updated invitation {Id} with status {Status}", invitation.Id, invitation.Status);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error updating invitation: {Id}", invitation.Id);
            throw;
        }
    }

    public async Task<List<UserInvitation>> ListAsync(ISpecification<UserInvitation> specification, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.UserInvitations
                .WithSpecification(specification)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error retrieving invitations by specification");
            throw;
        }
    }
}