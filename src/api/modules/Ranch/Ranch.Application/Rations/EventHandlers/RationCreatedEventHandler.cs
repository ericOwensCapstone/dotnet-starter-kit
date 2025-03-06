using FSH.Starter.WebApi.Ranch.Domain.Rations.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.Rations.EventHandlers;

public class RationCreatedEventHandler(ILogger<RationCreatedEventHandler> logger) : INotificationHandler<RationCreated>
{
    public async Task Handle(RationCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling ration created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling ration created domain event..");
    }
}


