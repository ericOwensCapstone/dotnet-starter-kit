namespace FSH.Framework.Core.Identity.Invitations.Features.ExtendInvitationExpiration;

public class ExtendInvitationExpirationResponse
{
    public Guid InvitationId { get; set; }
    public DateTime NewExpirationDate { get; set; }
    public DateTime PreviousExpirationDate { get; set; }
    public bool Success { get; set; }
}