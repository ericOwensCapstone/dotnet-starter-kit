using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Get.v1;
public sealed class GetLifecycleStageHandler(
    [FromKeyedServices("lifecycleStagecatalog:lifecycleStages")] IReadRepository<LifecycleStage> repository,
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
                var lifecycleStageItem = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (lifecycleStageItem == null) throw new LifecycleStageNotFoundException(request.Id);
                return new LifecycleStageResponse(
                    lifecycleStageItem.Id, 
                    lifecycleStageItem.Name, 
                    lifecycleStageItem.Description, 
                    lifecycleStageItem.Ration,
                    lifecycleStageItem.GrowthTreatment,
                    lifecycleStageItem.PreventativeTreatment,
                    lifecycleStageItem.TargetWeight,
                    lifecycleStageItem.TargetAdfi,
                    lifecycleStageItem.AdfiStdDev,
                    lifecycleStageItem.TargetWeightRangeForSort,
                    lifecycleStageItem.MergeableDuration,
                    lifecycleStageItem.MergeableWeightRange,
                    lifecycleStageItem.MaxHead
                );
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}

