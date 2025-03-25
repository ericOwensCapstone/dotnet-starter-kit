using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Update.v1;
public sealed class UpdateLifecycleStageHandler(
    ILogger<UpdateLifecycleStageHandler> logger,
    [FromKeyedServices("ranch:lifecycleStages")] IRepository<LifecycleStage> repository)
    : IRequestHandler<UpdateLifecycleStageCommand, UpdateLifecycleStageResponse>
{
    public async Task<UpdateLifecycleStageResponse> Handle(UpdateLifecycleStageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleStage = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = lifecycleStage ?? throw new LifecycleStageNotFoundException(request.Id);
        
        var updatedLifecycleStage = lifecycleStage.Update(
            request.Name,
            request.Description,
            request.RationId,
            request.GrowthTreatmentId,
            request.PreventiveTreatmentId
        );
        await repository.UpdateAsync(updatedLifecycleStage, cancellationToken);
        logger.LogInformation("lifecycleStage with id : {LifecycleStageId} updated.", lifecycleStage.Id);
        return new UpdateLifecycleStageResponse(lifecycleStage.Id);
    }
}

