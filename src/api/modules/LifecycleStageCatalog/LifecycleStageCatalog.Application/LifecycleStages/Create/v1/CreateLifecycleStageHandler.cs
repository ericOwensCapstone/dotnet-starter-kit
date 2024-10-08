using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain;
using FSH.Starter.WebApi.RationCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Create.v1;
public sealed class CreateLifecycleStageHandler(
    ILogger<CreateLifecycleStageHandler> logger,
    [FromKeyedServices("lifecycleStagecatalog:lifecycleStages")] IComponentRepository<LifecycleStage> repository)
    : IRequestHandler<CreateLifecycleStageCommand, CreateLifecycleStageResponse>
{
    public async Task<CreateLifecycleStageResponse> Handle(CreateLifecycleStageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var foundRation = await repository.GetComponentByIdAsync<Ration>(request.UpdateRationCommand!.Id, cancellationToken);
        var foundGrowthTreatment = await repository.GetComponentByIdAsync<GrowthTreatment>(request.UpdateGrowthTreatmentCommand!.Id, cancellationToken);
        var foundPreventativeTreatment = await repository.GetComponentByIdAsync<PreventativeTreatment>(request.UpdatePreventativeTreatmentCommand!.Id, cancellationToken);

        var lifecycleStage = LifecycleStage.Create(request.Name!, request.Description, request.Rating, foundRation, foundGrowthTreatment, foundPreventativeTreatment);
        await repository.AddAsync(lifecycleStage, cancellationToken);
        logger.LogInformation("lifecycleStage created {LifecycleStageId}", lifecycleStage.Id);
        return new CreateLifecycleStageResponse(lifecycleStage.Id);
    }
}

