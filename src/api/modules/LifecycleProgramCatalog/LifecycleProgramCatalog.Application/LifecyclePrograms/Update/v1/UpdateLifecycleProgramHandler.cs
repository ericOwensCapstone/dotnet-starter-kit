using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Exceptions;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Update.v1;
public sealed class UpdateLifecycleProgramHandler(
    ILogger<UpdateLifecycleProgramHandler> logger,
    [FromKeyedServices("lifecycleProgramcatalog:lifecyclePrograms")] IComponentRepository<LifecycleProgram> repository)
    : IRequestHandler<UpdateLifecycleProgramCommand, UpdateLifecycleProgramResponse>
{
    public async Task<UpdateLifecycleProgramResponse> Handle(UpdateLifecycleProgramCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleProgram = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = lifecycleProgram ?? throw new LifecycleProgramNotFoundException(request.Id);

        var lifecycleProgramStages = new List<LifecycleProgramStage>();
        foreach (var updateCommand in request.UpdateLifecycleProgramStageCommands)
        {
            var lifecycleStage = await repository.GetComponentByIdAsync<LifecycleStage>(updateCommand.LifecycleStageId, cancellationToken);
            var lifecycleProgramStage = new LifecycleProgramStage
            {
                LifecycleStage = lifecycleStage,
                LifecycleProgram = lifecycleProgram,
                LifecycleStageId = lifecycleStage.Id,
                LifecycleProgramId = lifecycleProgram.Id,
                Order = updateCommand.Order
            };
            lifecycleProgramStages.Add(lifecycleProgramStage);
        }

        var updatedLifecycleProgram = lifecycleProgram.Update(request.Name, request.Description, request.Rating, lifecycleProgramStages);
        await repository.UpdateAsync(updatedLifecycleProgram, cancellationToken);
        logger.LogInformation("lifecycleProgram with id : {LifecycleProgramId} updated.", lifecycleProgram.Id);
        return new UpdateLifecycleProgramResponse(lifecycleProgram.Id);
    }
}

