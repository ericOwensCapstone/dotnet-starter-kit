using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using MediatR;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Get.v1;
public sealed class GetLifecycleStageHandler(
    [FromKeyedServices("ranch:lifecycleStages")] IReadRepository<LifecycleStage> repository,
    ICacheService cache)
    : IRequestHandler<GetLifecycleStageRequest, LifecycleStageResponse>
{
    public async Task<LifecycleStageResponse> Handle(GetLifecycleStageRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"lifecycleStage:{request.Id}",
            async () =>
            {
                var spec = new GetLifecycleStageSpecs(request.Id);
                var lifecycleStageItem = await repository.FirstOrDefaultAsync(spec, cancellationToken);
                if (lifecycleStageItem == null) throw new LifecycleStageNotFoundException(request.Id);
                return lifecycleStageItem;
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

