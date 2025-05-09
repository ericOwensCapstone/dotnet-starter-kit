using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Members.Application.MemberAds.Get.v1;
using FSH.Starter.WebApi.Members.Domain.MemberAds;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Members.Application.MemberAds.Search.v1;
public sealed class SearchMemberAdsHandler(
    [FromKeyedServices("members:memberAds")] IReadRepository<MemberAd> repository)
    : IRequestHandler<SearchMemberAdsCommand, PagedList<MemberAdResponse>>
{
    public async Task<PagedList<MemberAdResponse>> Handle(SearchMemberAdsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchMemberAdSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<MemberAdResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


