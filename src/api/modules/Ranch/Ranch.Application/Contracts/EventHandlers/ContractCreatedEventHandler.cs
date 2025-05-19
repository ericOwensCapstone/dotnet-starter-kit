using FSH.Starter.WebApi.Ranch.Domain.Contracts.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.Contracts.EventHandlers;

public class ContractCreatedEventHandler(ILogger<ContractCreatedEventHandler> logger) : INotificationHandler<ContractCreated>
{
    public async Task Handle(ContractCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling contract created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling contract created domain event..");
    }
}


