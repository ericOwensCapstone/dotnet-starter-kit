using Ardalis.Specification;
using FSH.Framework.Core.Identity.Invitations.Features.SearchInvitations;
using FSH.Framework.Core.Specifications;

namespace FSH.Framework.Core.Identity.Invitations.Specifications;

public class InvitationsByPaginationFilterSpec : EntitiesByPaginationFilterSpec<UserInvitation, InvitationDto>
{
    public InvitationsByPaginationFilterSpec(SearchInvitationsQuery request)
        : base(request)
    {
        // Note: Tenant ownership filtering is handled by InvitationRepository.GetFilteredInvitations()
        // to ensure proper data privacy based on current user context
        
        // Filter by target tenant (where user will be invited) if specified
        if (!string.IsNullOrEmpty(request.TargetTenantId))
        {
            Query.Where(x => x.TargetTenantId == request.TargetTenantId);
        }
        
        if (request.Status.HasValue)
        {
            Query.Where(x => x.Status == request.Status.Value);
        }

        Query.OrderByDescending(x => x.Created);
        
        Query.Select(x => new InvitationDto
        {
            Id = x.Id,
            Email = x.Email,
            DisplayName = x.DisplayName,
            Status = x.Status,
            TenantId = x.TenantId,
            TargetTenantId = x.TargetTenantId,
            Role = x.Role,
            ExpiresAt = x.ExpiresAt,
            Created = x.Created.DateTime
        });
    }
}