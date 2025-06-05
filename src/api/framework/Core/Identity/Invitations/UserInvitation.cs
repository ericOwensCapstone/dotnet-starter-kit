using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;

namespace FSH.Framework.Core.Identity.Invitations;

public class UserInvitation : AuditableEntity, IAggregateRoot
{
    // Ownership properties (for data privacy - who created this invitation)
    public string? TenantId { get; set; }
    public Guid? MemberId { get; set; }
    
    // Invitation specific properties
    public string Email { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string TargetTenantId { get; private set; } = default!;
    public string InvitedBy { get; private set; } = default!;
    public string? Role { get; private set; }
    public InvitationStatus Status { get; private set; }
    public string InvitationToken { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    public string? B2CUserId { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    // Navigation properties
    public string? AcceptedByUserId { get; private set; }

    private UserInvitation() { } // EF Core

    public static UserInvitation Create(
        string email,
        string displayName,
        string targetTenantId,
        string invitedBy,
        string? firstName = null,
        string? lastName = null,
        string? role = null,
        DateTime? expiresAt = null)
    {
        return new UserInvitation
        {
            Id = Guid.NewGuid(),
            Email = email.ToLowerInvariant(),
            DisplayName = displayName,
            FirstName = firstName,
            LastName = lastName,
            TargetTenantId = targetTenantId,
            InvitedBy = invitedBy,
            Role = role,
            Status = InvitationStatus.Pending,
            InvitationToken = GenerateSecureToken(),
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7),
            RetryCount = 0,
            Created = DateTimeOffset.UtcNow
        };
    }

    public void MarkAsSent(string? b2cUserId = null)
    {
        Status = InvitationStatus.Sent;
        SentAt = DateTime.UtcNow;
        B2CUserId = b2cUserId;
        LastModified = DateTimeOffset.UtcNow;
    }

    public void Accept(string acceptedByUserId)
    {
        if (Status != InvitationStatus.Sent)
            throw new InvalidOperationException($"Cannot accept invitation with status {Status}");

        if (DateTime.UtcNow > ExpiresAt)
            throw new InvalidOperationException("Invitation has expired");

        Status = InvitationStatus.Accepted;
        AcceptedAt = DateTime.UtcNow;
        AcceptedByUserId = acceptedByUserId;
        LastModified = DateTimeOffset.UtcNow;
    }

    public void Cancel()
    {
        if (Status == InvitationStatus.Accepted)
            throw new InvalidOperationException("Cannot cancel an accepted invitation");

        Status = InvitationStatus.Cancelled;
        LastModified = DateTimeOffset.UtcNow;
    }

    public void MarkAsExpired()
    {
        if (Status == InvitationStatus.Accepted)
            return; // Already accepted, don't mark as expired

        Status = InvitationStatus.Expired;
        LastModified = DateTimeOffset.UtcNow;
    }

    public void MarkAsFailed(string errorMessage)
    {
        Status = InvitationStatus.Failed;
        ErrorMessage = errorMessage;
        RetryCount++;
        LastModified = DateTimeOffset.UtcNow;
    }

    public void Retry()
    {
        if (Status != InvitationStatus.Failed)
            throw new InvalidOperationException("Can only retry failed invitations");

        Status = InvitationStatus.Pending;
        ErrorMessage = null;
        LastModified = DateTimeOffset.UtcNow;
    }

    public void ExtendExpiration(DateTime newExpirationDate)
    {
        if (Status == InvitationStatus.Accepted)
            throw new InvalidOperationException("Cannot extend expiration of an accepted invitation");

        if (Status == InvitationStatus.Cancelled)
            throw new InvalidOperationException("Cannot extend expiration of a cancelled invitation");

        if (newExpirationDate <= DateTime.UtcNow)
            throw new ArgumentException("New expiration date must be in the future");

        if (newExpirationDate <= ExpiresAt)
            throw new ArgumentException("New expiration date must be later than the current expiration");

        ExpiresAt = newExpirationDate;
        
        // If invitation was expired, change status back to Sent (if it was previously sent)
        if (Status == InvitationStatus.Expired && !string.IsNullOrEmpty(B2CUserId))
        {
            Status = InvitationStatus.Sent;
        }
        
        LastModified = DateTimeOffset.UtcNow;
    }

    public bool IsExpired => DateTime.UtcNow > ExpiresAt;
    public bool CanBeRetried => Status == InvitationStatus.Failed && RetryCount < 3;
    public bool IsActive => Status == InvitationStatus.Sent && !IsExpired;

    private static string GenerateSecureToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}