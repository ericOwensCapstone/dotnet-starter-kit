using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Search.v1;
public sealed class SearchHarvestContractStatusesHandler(
    [FromKeyedServices("harvest:harvestContractStatuses")] IReadRepository<HarvestContractStatus> repository)
    : IRequestHandler<SearchHarvestContractStatusesCommand, PagedList<HarvestContractStatusResponse>>
{
    public async Task<PagedList<HarvestContractStatusResponse>> Handle(SearchHarvestContractStatusesCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchHarvestContractStatusSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<HarvestContractStatusResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


