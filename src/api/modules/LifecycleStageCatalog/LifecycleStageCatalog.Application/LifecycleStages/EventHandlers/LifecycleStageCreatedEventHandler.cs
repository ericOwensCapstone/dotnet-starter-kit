using FSH.Starter.WebApi.LifecycleStageCatalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.EventHandlers;

public class LifecycleStageCreatedEventHandler(ILogger<LifecycleStageCreatedEventHandler> logger) : INotificationHandler<LifecycleStageCreated>
{
    public async Task Handle(LifecycleStageCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling lifecycleStage created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling lifecycleStage created domain event..");
    }
}


