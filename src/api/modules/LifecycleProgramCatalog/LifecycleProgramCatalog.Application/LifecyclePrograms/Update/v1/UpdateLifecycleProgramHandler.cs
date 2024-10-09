using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Update.v1;
public sealed class UpdateLifecycleProgramHandler(
    ILogger<UpdateLifecycleProgramHandler> logger,
    [FromKeyedServices("lifecycleProgramcatalog:lifecyclePrograms")] IRepository<LifecycleProgram> repository)
    : IRequestHandler<UpdateLifecycleProgramCommand, UpdateLifecycleProgramResponse>
{
    public async Task<UpdateLifecycleProgramResponse> Handle(UpdateLifecycleProgramCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleProgram = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = lifecycleProgram ?? throw new LifecycleProgramNotFoundException(request.Id);
        var updatedLifecycleProgram = lifecycleProgram.Update(request.Name, request.Description, request.Rating);
        await repository.UpdateAsync(updatedLifecycleProgram, cancellationToken);
        logger.LogInformation("lifecycleProgram with id : {LifecycleProgramId} updated.", lifecycleProgram.Id);
        return new UpdateLifecycleProgramResponse(lifecycleProgram.Id);
    }
}

