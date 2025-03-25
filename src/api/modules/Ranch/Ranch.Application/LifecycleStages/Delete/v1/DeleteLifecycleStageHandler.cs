using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages.Exceptions;
using FSH.Starter.WebApi.Ranch.Domain.LifecycleStages;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.LifecycleStages.Delete.v1;
public sealed class DeleteLifecycleStageHandler(
    ILogger<DeleteLifecycleStageHandler> logger,
    [FromKeyedServices("ranch:lifecycleStages")] IRepository<LifecycleStage> repository)
    : IRequestHandler<DeleteLifecycleStageCommand>
{
    public async Task Handle(DeleteLifecycleStageCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var lifecycleStage = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = lifecycleStage ?? throw new LifecycleStageNotFoundException(request.Id);
        await repository.DeleteAsync(lifecycleStage, cancellationToken);
        logger.LogInformation("lifecycleStage with id : {LifecycleStageId} deleted", lifecycleStage.Id);
    }
}

