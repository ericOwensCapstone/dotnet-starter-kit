using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Get.v1;
public sealed class GetHarvestContractStatusHandler(
    [FromKeyedServices("harvest:harvestContractStatuses")] IReadRepository<HarvestContractStatus> repository,
    ICacheService cache)
    : IRequestHandler<GetHarvestContractStatusRequest, HarvestContractStatusResponse>
{
    public async Task<HarvestContractStatusResponse> Handle(GetHarvestContractStatusRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"harvestContractStatus:{request.Id}",
            async () =>
            {
                var spec = new GetHarvestContractStatusSpecs(request.Id);
                var harvestContractStatusItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (harvestContractStatusItem == null) throw new HarvestContractStatusNotFoundException(request.Id);
                return harvestContractStatusItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

