using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Get.v1;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Search.v1;
public sealed class SearchHarvestContractsHandler(
    [FromKeyedServices("harvest:harvestContracts")] IReadRepository<HarvestContract> repository)
    : IRequestHandler<SearchHarvestContractsCommand, PagedList<HarvestContractResponse>>
{
    public async Task<PagedList<HarvestContractResponse>> Handle(SearchHarvestContractsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchHarvestContractSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<HarvestContractResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


