using Ardalis.Specification;
using FSH.Framework.Core.Identity.Invitations;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

public class InvitationsByEmailSpec : Specification<UserInvitation>
{
    public InvitationsByEmailSpec(string email)
    {
        Query.Where(x => x.Email == email.ToLowerInvariant())
             .OrderByDescending(x => x.Created);
    }
}