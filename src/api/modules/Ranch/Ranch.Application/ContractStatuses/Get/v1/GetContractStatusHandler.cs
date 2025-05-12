using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;
public sealed class GetContractStatusHandler(
    [FromKeyedServices("ranch:contractStatuses")] IReadRepository<ContractStatus> repository,
    ICacheService cache)
    : IRequestHandler<GetContractStatusRequest, ContractStatusResponse>
{
    public async Task<ContractStatusResponse> Handle(GetContractStatusRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"contractStatus:{request.Id}",
            async () =>
            {
                var spec = new GetContractStatusSpecs(request.Id);
                var contractStatusItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (contractStatusItem == null) throw new ContractStatusNotFoundException(request.Id);
                return contractStatusItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

