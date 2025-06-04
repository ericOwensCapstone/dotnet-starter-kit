using FSH.Framework.Core.Paging;
using MediatR;

namespace FSH.Framework.Core.Identity.Invitations.Features.SearchInvitations;

public class SearchInvitationsHandler(IInvitationService invitationService) : IRequestHandler<SearchInvitationsQuery, PagedList<InvitationDto>>
{
    public async Task<PagedList<InvitationDto>> Handle(SearchInvitationsQuery request, CancellationToken cancellationToken)
    {
        return await invitationService.SearchInvitationsAsync(request, cancellationToken);
    }
}