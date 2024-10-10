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
        var lifecycleStages = new List<LifecycleStage>();
        foreach(var command in request.LifecycleStages)
        {
            var ls = await repository.GetComponentByIdAsync<LifecycleStage>(command.Id, cancellationToken);
            lifecycleStages.Add(ls);
        }
        var lifecycleProgram = LifecycleProgram.Create(request.Name!, request.Description, request.Rating, lifecycleStages);
        await repository.AddAsync(lifecycleProgram, cancellationToken);
        logger.LogInformation("lifecycleProgram created {LifecycleProgramId}", lifecycleProgram.Id);
        return new CreateLifecycleProgramResponse(lifecycleProgram.Id);
    }
}

