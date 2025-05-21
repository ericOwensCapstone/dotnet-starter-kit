using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Application.MemberPages.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.MemberPages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.Search.v1;
public sealed class SearchMemberPagesHandler(
    [FromKeyedServices("ranch:memberPages")] IReadRepository<MemberPage> repository)
    : IRequestHandler<SearchMemberPagesCommand, PagedList<MemberPageResponse>>
{
    public async Task<PagedList<MemberPageResponse>> Handle(SearchMemberPagesCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchMemberPageSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<MemberPageResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


