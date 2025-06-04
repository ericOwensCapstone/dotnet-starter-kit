namespace FSH.Framework.Core.Identity.Invitations.Features.SearchInvitations;

public class InvitationDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public string? DisplayName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string TenantId { get; set; } = default!;
    public string? InvitedBy { get; set; }
    public string? Role { get; set; }
    public InvitationStatus Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime Created { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public string? B2CUserId { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
}