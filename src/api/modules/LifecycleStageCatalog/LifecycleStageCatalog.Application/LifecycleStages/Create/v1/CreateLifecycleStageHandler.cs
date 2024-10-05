using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Create.v1;
public sealed class CreateLifecycleStageHandler(
    ILogger<CreateLifecycleStageHandler> logger,
    [FromKeyedServices("lifecycleStagecatalog:lifecycleStages")] IRepository<LifecycleStage> repository)
    : IRequestHandler<CreateLifecycleStageCommand, CreateLifecycleStageResponse>
{
    public async Task<CreateLifecycleStageResponse> Handle(CreateLifecycleStageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        //TODO restore
        //var lifecycleStage = LifecycleStage.Create(request.Name!, request.Description, request.Rating, request.Ration, request.GrowthTreatment, request.PreventativeTreatment);
        var lifecycleStage = LifecycleStage.Create(request.Name!, request.Description, request.Rating, null, null, null);
        await repository.AddAsync(lifecycleStage, cancellationToken);
        logger.LogInformation("lifecycleStage created {LifecycleStageId}", lifecycleStage.Id);
        return new CreateLifecycleStageResponse(lifecycleStage.Id);
    }
}

