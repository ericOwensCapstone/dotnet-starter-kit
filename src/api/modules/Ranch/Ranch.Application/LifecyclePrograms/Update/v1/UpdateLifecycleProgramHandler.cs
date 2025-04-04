using Mapster;
using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Update.v1;
public sealed class UpdateLifecycleProgramHandler(
    ILogger<UpdateLifecycleProgramHandler> logger,
    [FromKeyedServices("ranch:lifecyclePrograms")] IRepository<LifecycleProgram> repository)
    : IRequestHandler<UpdateLifecycleProgramCommand, UpdateLifecycleProgramResponse>
{
    public async Task<UpdateLifecycleProgramResponse> Handle(UpdateLifecycleProgramCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleProgramLifecycleStages = new List<LifecycleProgramLifecycleStage>();
        foreach (var v in request.LifecycleProgramLifecycleStages)
        {
            var r = v.Adapt<LifecycleProgramLifecycleStage>();
            lifecycleProgramLifecycleStages.Add(r);
        }

        var lifecycleProgram = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = lifecycleProgram ?? throw new LifecycleProgramNotFoundException(request.Id);
        
        var updatedLifecycleProgram = lifecycleProgram.Update(
            request.Name,
            request.Description,
            lifecycleProgramLifecycleStages
        );
        await repository.UpdateAsync(updatedLifecycleProgram, cancellationToken);
        logger.LogInformation("lifecycleProgram with id : {LifecycleProgramId} updated.", lifecycleProgram.Id);
        return new UpdateLifecycleProgramResponse(lifecycleProgram.Id);
    }
}

