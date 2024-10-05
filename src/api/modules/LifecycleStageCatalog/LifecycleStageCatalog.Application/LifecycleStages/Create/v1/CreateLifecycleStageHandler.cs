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
    [FromKeyedServices("lifecycleStagecatalog:lifecycleStages")] IAttachableRepository<LifecycleStage> repository)
    : IRequestHandler<CreateLifecycleStageCommand, CreateLifecycleStageResponse>
{
    public async Task<CreateLifecycleStageResponse> Handle(CreateLifecycleStageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        await repository.GetAndAttach<Ration>(request.UpdateRationCommand.Id, cancellationToken);
        await repository.GetAndAttach<GrowthTreatment>(request.UpdateGrowthTreatmentCommand.Id, cancellationToken);
        await repository.GetAndAttach<PreventativeTreatment>(request.UpdatePreventativeTreatmentCommand.Id, cancellationToken);

        var ration = Ration.CreateWithGuid(request.UpdateRationCommand.Id, request.UpdateRationCommand.Name!, request.UpdateRationCommand.Description, request.UpdateRationCommand.DollarsPerPound);
        var growthTreatment = GrowthTreatment.CreateWithGuid(request.UpdateGrowthTreatmentCommand.Id, request.UpdateGrowthTreatmentCommand.Name!, request.UpdateGrowthTreatmentCommand.Description, request.UpdateGrowthTreatmentCommand.DollarsPerHead);
        var preventativeTreatment = PreventativeTreatment.CreateWithGuid(request.UpdatePreventativeTreatmentCommand.Id, request.UpdatePreventativeTreatmentCommand.Name!, request.UpdatePreventativeTreatmentCommand.Description, request.UpdatePreventativeTreatmentCommand.DollarsPerHead);

        var lifecycleStage = LifecycleStage.Create(request.Name!, request.Description, request.Rating, ration, growthTreatment, preventativeTreatment);
        await repository.AddAsync(lifecycleStage, cancellationToken);
        logger.LogInformation("lifecycleStage created {LifecycleStageId}", lifecycleStage.Id);
        return new CreateLifecycleStageResponse(lifecycleStage.Id);
    }
}

