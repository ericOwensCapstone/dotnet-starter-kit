using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.LifecyclePrograms;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.LifecyclePrograms.Delete.v1;
public sealed class DeleteLifecycleProgramHandler(
    ILogger<DeleteLifecycleProgramHandler> logger,
    [FromKeyedServices("ranch:lifecyclePrograms")] IRepository<LifecycleProgram> repository)
    : IRequestHandler<DeleteLifecycleProgramCommand>
{
    public async Task Handle(DeleteLifecycleProgramCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleProgram = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = lifecycleProgram ?? throw new LifecycleProgramNotFoundException(request.Id);
        await repository.DeleteAsync(lifecycleProgram, cancellationToken);
        logger.LogInformation("lifecycleProgram with id : {LifecycleProgramId} deleted", lifecycleProgram.Id);
    }
}

