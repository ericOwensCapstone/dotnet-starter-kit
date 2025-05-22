using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Get.v1;
using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Search.v1;
public sealed class SearchHarvestMembersHandler(
    [FromKeyedServices("harvest:harvestMembers")] IReadRepository<HarvestMember> repository)
    : IRequestHandler<SearchHarvestMembersCommand, PagedList<HarvestMemberResponse>>
{
    public async Task<PagedList<HarvestMemberResponse>> Handle(SearchHarvestMembersCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchHarvestMemberSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<HarvestMemberResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


