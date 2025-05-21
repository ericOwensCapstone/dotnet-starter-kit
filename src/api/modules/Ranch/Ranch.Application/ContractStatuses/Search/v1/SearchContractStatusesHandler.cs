using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Search.v1;
public sealed class SearchContractStatusesHandler(
    [FromKeyedServices("ranch:contractStatuses")] IReadRepository<ContractStatus> repository)
    : IRequestHandler<SearchContractStatusesCommand, PagedList<ContractStatusResponse>>
{
    public async Task<PagedList<ContractStatusResponse>> Handle(SearchContractStatusesCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new SearchContractStatusSpecs(request);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<ContractStatusResponse>(items, request!.PageNumber, request!.PageSize, totalCount);
    }
}


