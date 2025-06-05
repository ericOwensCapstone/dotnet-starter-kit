using Ardalis.Specification;
using FSH.Framework.Core.Identity.Invitations;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

public class InvitationsByEmailAndTenantSpec : Specification<UserInvitation>
{
    public InvitationsByEmailAndTenantSpec(string email, string targetTenantId)
    {
        Query.Where(x => x.Email == email.ToLowerInvariant() && x.TargetTenantId == targetTenantId)
             .OrderByDescending(x => x.Created);
    }
}