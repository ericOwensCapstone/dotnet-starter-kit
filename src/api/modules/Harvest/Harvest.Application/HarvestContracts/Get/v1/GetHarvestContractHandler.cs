using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Get.v1;
public sealed class GetHarvestContractHandler(
    [FromKeyedServices("harvest:harvestContracts")] IReadRepository<HarvestContract> repository,
    ICacheService cache)
    : IRequestHandler<GetHarvestContractRequest, HarvestContractResponse>
{
    public async Task<HarvestContractResponse> Handle(GetHarvestContractRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"harvestContract:{request.Id}",
            async () =>
            {
                var spec = new GetHarvestContractSpecs(request.Id);
                var harvestContractItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (harvestContractItem == null) throw new HarvestContractNotFoundException(request.Id);
                return harvestContractItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

