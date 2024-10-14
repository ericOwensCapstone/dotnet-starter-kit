using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using FSH.Starter.WebApi.LifecycleStageCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Create.v1;
public sealed class CreateLifecycleProgramHandler(
    ILogger<CreateLifecycleProgramHandler> logger,
    [FromKeyedServices("lifecycleProgramcatalog:lifecyclePrograms")] IComponentRepository<LifecycleProgram> repository)
    : IRequestHandler<CreateLifecycleProgramCommand, CreateLifecycleProgramResponse>
{
    public async Task<CreateLifecycleProgramResponse> Handle(CreateLifecycleProgramCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleProgramStages = new List<LifecycleProgramStage>();
        foreach(var command in request.UpdateLifecycleProgramStageCommands)
        {
            var ls = await repository.GetComponentByIdAsync<LifecycleStage>(command.LifecycleStageId, cancellationToken);
            var lifecycleProgramStage = new LifecycleProgramStage
            {
                LifecycleStage = ls,
                LifecycleStageId = ls.Id,
                Order = command.Order
            };
            lifecycleProgramStages.Add(lifecycleProgramStage);

        }
        var lifecycleProgram = LifecycleProgram.Create(request.Name!, request.Description, request.Rating, lifecycleProgramStages);
        await repository.AddAsync(lifecycleProgram, cancellationToken);
        logger.LogInformation("lifecycleProgram created {LifecycleProgramId}", lifecycleProgram.Id);
        return new CreateLifecycleProgramResponse(lifecycleProgram.Id);
    }
}

