namespace FSH.Framework.Core.Identity.Invitations.Features;

public class CreateInvitationResponse
{
    public Guid InvitationId { get; set; }
    public string Email { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string TargetTenantId { get; set; } = default!;
    public InvitationStatus Status { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? B2CUserId { get; set; }
    public bool EmailSent { get; set; }
}