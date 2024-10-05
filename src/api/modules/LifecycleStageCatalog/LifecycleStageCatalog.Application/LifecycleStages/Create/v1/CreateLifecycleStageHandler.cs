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

        Ration ration = null;
        GrowthTreatment growthTreatment = null;
        PreventativeTreatment preventativeTreatment = null;

        var existingRation = await repository.Get<Ration>(request.UpdateRationCommand.Id, cancellationToken);
        var existingGrowthTreatment = await repository.Get<GrowthTreatment>(request.UpdateGrowthTreatmentCommand.Id, cancellationToken);
        var existingPreventativeTreatment = await repository.Get<PreventativeTreatment>(request.UpdatePreventativeTreatmentCommand.Id, cancellationToken);

        var incomingRation = Ration.Create(request.UpdateRationCommand.Id, request.UpdateRationCommand.Name!, request.UpdateRationCommand.Description, request.UpdateRationCommand.DollarsPerPound);
        var incomingGrowthTreatment = GrowthTreatment.Create(request.UpdateGrowthTreatmentCommand.Id, request.UpdateGrowthTreatmentCommand.Name!, request.UpdateGrowthTreatmentCommand.Description, request.UpdateGrowthTreatmentCommand.DollarsPerHead);
        var incomingPreventativeTreatment = PreventativeTreatment.Create(request.UpdatePreventativeTreatmentCommand.Id, request.UpdatePreventativeTreatmentCommand.Name!, request.UpdatePreventativeTreatmentCommand.Description, request.UpdatePreventativeTreatmentCommand.DollarsPerHead);

        //TODO the only problem below is when the incomingRation is an updated version of a ration that already exists in the database
        //We need to check if the incoming ration is an updated version of an existing ration
        //and if so, we need to update the existing ration with the incoming ration
        //before we can use the existing ration
        // all of this implies we have an implementation of an equals method in the ration class
        // see the commented out code at the bottom of the Ration class

        if (existingRation != null)
        {
            ration = existingRation;
            await repository.Attach<Ration>(request.UpdateRationCommand.Id, cancellationToken);
        }
        else
        {
            ration = incomingRation;
        }

        if(existingGrowthTreatment != null)
        {
            growthTreatment = existingGrowthTreatment;
            await repository.Attach<GrowthTreatment>(request.UpdateGrowthTreatmentCommand.Id, cancellationToken);
        }
        else
        {
            growthTreatment = incomingGrowthTreatment;
        }

        if(existingPreventativeTreatment != null)
        {
            preventativeTreatment = existingPreventativeTreatment;
            await repository.Attach<PreventativeTreatment>(request.UpdatePreventativeTreatmentCommand.Id, cancellationToken);
        }
        else
        {
            preventativeTreatment = incomingPreventativeTreatment;
        }



        var lifecycleStage = LifecycleStage.Create(request.Name!, request.Description, request.Rating, ration, growthTreatment, preventativeTreatment);
        try
        {
            await repository.AddAsync(lifecycleStage, cancellationToken);
        } catch(Exception ex)
        {
            var wd = 40;
        }
        logger.LogInformation("lifecycleStage created {LifecycleStageId}", lifecycleStage.Id);
        return new CreateLifecycleStageResponse(lifecycleStage.Id);
    }
}

