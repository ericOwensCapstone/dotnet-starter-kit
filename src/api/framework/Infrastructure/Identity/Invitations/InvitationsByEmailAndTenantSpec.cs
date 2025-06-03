using Ardalis.Specification;
using FSH.Framework.Core.Identity.Invitations;

namespace FSH.Framework.Infrastructure.Identity.Invitations;

public class InvitationsByEmailAndTenantSpec : Specification<UserInvitation>
{
    public InvitationsByEmailAndTenantSpec(string email, string tenantId)
    {
        Query.Where(x => x.Email == email.ToLowerInvariant() && x.TenantId == tenantId)
             .OrderByDescending(x => x.Created);
    }
}