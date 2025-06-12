namespace FSH.Framework.Core.Identity.Invitations;

/// <summary>
/// Interface for anonymous invitation repository.
/// Used for public endpoints that need to access invitations without authentication context.
/// </summary>
public interface IAnonymousInvitationRepository
{
    /// <summary>
    /// Gets an invitation by its token without any tenant filtering.
    /// </summary>
    /// <param name="token">The invitation token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The invitation if found, null otherwise</returns>
    Task<UserInvitation?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
}