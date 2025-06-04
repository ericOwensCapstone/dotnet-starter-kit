using Ardalis.Specification;
using FSH.Framework.Core.Identity.Invitations.Features.SearchInvitations;
using FSH.Framework.Core.Specifications;

namespace FSH.Framework.Core.Identity.Invitations.Specifications;

public class InvitationsByPaginationFilterSpec : EntitiesByPaginationFilterSpec<UserInvitation, InvitationDto>
{
    public InvitationsByPaginationFilterSpec(SearchInvitationsQuery request)
        : base(request)
    {
        Query.Where(x => string.IsNullOrEmpty(request.TenantId) || x.TenantId == request.TenantId);
        
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
            Role = x.Role,
            ExpiresAt = x.ExpiresAt,
            Created = x.Created.DateTime
        });
    }
}