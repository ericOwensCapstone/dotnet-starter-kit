using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Get.v1;
using FSH.Starter.WebApi.RationCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.RationCatalog.Application.Rations.Search.v1;
public sealed class SearchRationsHandler(
    [FromKeyedServices("rationcatalog:rations")] IReadRepository<Ration> repository)
    : IRequestHandler<SearchRationsCommand, PagedList<RationResponse>>
{
    public async Task<PagedList<RationResponse>> Handle(SearchRationsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new EntitiesByPaginationFilterSpec<Ration, RationResponse>(request.filter);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<RationResponse>(items, request.filter.PageNumber, request.filter.PageSize, totalCount);
    }
}


