using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Application.Contracts.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Search.v1;
public sealed class SearchContractsHandler(
    [FromKeyedServices("ranch:contracts")] IReadRepository<Contract> repository)
    : IRequestHandler<SearchContractsCommand, PagedList<ContractResponse>>
{
    public async Task<PagedList<ContractResponse>> Handle(SearchContractsCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchContractSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<ContractResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


