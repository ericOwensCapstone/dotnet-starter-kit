using System.ComponentModel.DataAnnotations;

namespace FSH.Framework.Core.Identity.Invitations.Features.ExtendInvitationExpiration;

public class ExtendInvitationExpirationRequest
{
    [Required]
    public Guid InvitationId { get; set; }

    [Required]
    public DateTime NewExpirationDate { get; set; }
}