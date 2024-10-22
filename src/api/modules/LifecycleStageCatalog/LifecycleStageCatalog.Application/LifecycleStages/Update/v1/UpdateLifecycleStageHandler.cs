using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Update.v1;
public sealed class UpdateLifecycleStageHandler(
    ILogger<UpdateLifecycleStageHandler> logger,
    [FromKeyedServices("lifecycleStagecatalog:lifecycleStages")] IComponentRepository<LifecycleStage> repository)
    : IRequestHandler<UpdateLifecycleStageCommand, UpdateLifecycleStageResponse>
{
    public async Task<UpdateLifecycleStageResponse> Handle(UpdateLifecycleStageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleStage = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = lifecycleStage ?? throw new LifecycleStageNotFoundException(request.Id);

        var foundRation = await repository.GetComponentByIdAsync<Ration>(request.UpdateRationCommand!.Id, cancellationToken);
        var foundGrowthTreatment = await repository.GetComponentByIdAsync<GrowthTreatment>(request.UpdateGrowthTreatmentCommand!.Id, cancellationToken);
        var foundPreventativeTreatment = await repository.GetComponentByIdAsync<PreventativeTreatment>(request.UpdatePreventativeTreatmentCommand!.Id, cancellationToken);
        var updatedLifecycleStage = lifecycleStage.Update(
            request.Name, 
            request.Description, 
            foundRation, 
            foundGrowthTreatment, 
            foundPreventativeTreatment,
            request.TargetWeight,
            request.TargetAdfi,
            request.AdfiStdDev,
            request.TargetWeightRangeForSort,
            request.MergeableDuration,
            request.MergeableWeightRange,
            request.MaxHead
        );
        await repository.UpdateAsync(updatedLifecycleStage, cancellationToken);
        logger.LogInformation("lifecycleStage with id : {LifecycleStageId} updated.", lifecycleStage.Id);
        return new UpdateLifecycleStageResponse(lifecycleStage.Id);
    }
}

