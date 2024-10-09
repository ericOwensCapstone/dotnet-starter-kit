using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Delete.v1;
public sealed class DeleteLifecycleProgramHandler(
    ILogger<DeleteLifecycleProgramHandler> logger,
    [FromKeyedServices("lifecycleProgramcatalog:lifecyclePrograms")] IRepository<LifecycleProgram> repository)
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

