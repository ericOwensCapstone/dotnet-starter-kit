using FSH.Starter.WebApi.LifecycleProgramCatalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.EventHandlers;

public class LifecycleProgramCreatedEventHandler(ILogger<LifecycleProgramCreatedEventHandler> logger) : INotificationHandler<LifecycleProgramCreated>
{
    public async Task Handle(LifecycleProgramCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling lifecycleProgram created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling lifecycleProgram created domain event..");
    }
}


