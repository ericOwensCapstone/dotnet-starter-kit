namespace FSH.Framework.Core.Identity.Invitations;

public enum InvitationStatus
{
    Pending = 0,
    Sent = 1,
    Accepted = 2,
    Expired = 3,
    Cancelled = 4,
    Failed = 5
}