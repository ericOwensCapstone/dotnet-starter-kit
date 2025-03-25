using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Create.v1;
public sealed class CreateLifecycleStageHandler(
    ILogger<CreateLifecycleStageHandler> logger,
    [FromKeyedServices("ranch:lifecycleStages")] IRepository<LifecycleStage> repository)
    : IRequestHandler<CreateLifecycleStageCommand, CreateLifecycleStageResponse>
{
    public async Task<CreateLifecycleStageResponse> Handle(CreateLifecycleStageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleStage = LifecycleStage.Create(
            request.Name,
            request.Description,
            request.RationId,
            request.GrowthTreatmentId,
            request.PreventiveTreatmentId
        );
        await repository.AddAsync(lifecycleStage, cancellationToken);
        logger.LogInformation("lifecycleStage created {LifecycleStageId}", lifecycleStage.Id);
        return new CreateLifecycleStageResponse(lifecycleStage.Id);
    }
}

