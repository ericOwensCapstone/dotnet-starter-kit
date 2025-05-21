using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Domain.Contracts.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Ranch.Domain.Contracts;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.Get.v1;
public sealed class GetContractHandler(
    [FromKeyedServices("ranch:contracts")] IReadRepository<Contract> repository,
    ICacheService cache)
    : IRequestHandler<GetContractRequest, ContractResponse>
{
    public async Task<ContractResponse> Handle(GetContractRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"contract:{request.Id}",
            async () =>
            {
                var spec = new GetContractSpecs(request.Id);
                var contractItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (contractItem == null) throw new ContractNotFoundException(request.Id);
                return contractItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

