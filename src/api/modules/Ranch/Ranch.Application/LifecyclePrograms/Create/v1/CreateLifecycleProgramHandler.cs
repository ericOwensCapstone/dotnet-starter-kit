using Mapster;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Create.v1;
public sealed class CreateLifecycleProgramHandler(
    ILogger<CreateLifecycleProgramHandler> logger,
    [FromKeyedServices("ranch:lifecyclePrograms")] IRepository<LifecycleProgram> repository)
    : IRequestHandler<CreateLifecycleProgramCommand, CreateLifecycleProgramResponse>
{
    public async Task<CreateLifecycleProgramResponse> Handle(CreateLifecycleProgramCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleProgramLifecycleStages = new List<LifecycleProgramLifecycleStage>();
        foreach (var v in request.LifecycleProgramLifecycleStages)
        {
            var r = v.Adapt<LifecycleProgramLifecycleStage>();
            lifecycleProgramLifecycleStages.Add(r);
        }

        var lifecycleProgram = LifecycleProgram.Create(
            request.Name,
            request.Description,
            lifecycleProgramLifecycleStages
        );
        await repository.AddAsync(lifecycleProgram, cancellationToken);
        logger.LogInformation("lifecycleProgram created {LifecycleProgramId}", lifecycleProgram.Id);
        return new CreateLifecycleProgramResponse(lifecycleProgram.Id);
    }
}

