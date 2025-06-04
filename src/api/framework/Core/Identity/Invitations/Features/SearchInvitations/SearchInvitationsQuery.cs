using FSH.Framework.Core.Paging;
using MediatR;

namespace FSH.Framework.Core.Identity.Invitations.Features.SearchInvitations;

public class SearchInvitationsQuery : PaginationFilter, IRequest<PagedList<InvitationDto>>
{
    public string? TenantId { get; set; }
    public InvitationStatus? Status { get; set; }
}