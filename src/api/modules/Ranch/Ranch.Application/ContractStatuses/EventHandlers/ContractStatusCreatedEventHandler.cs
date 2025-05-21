using FSH.Starter.WebApi.Ranch.Domain.ContractStatuses.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.ContractStatuses.EventHandlers;

public class ContractStatusCreatedEventHandler(ILogger<ContractStatusCreatedEventHandler> logger) : INotificationHandler<ContractStatusCreated>
{
    public async Task Handle(ContractStatusCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling contractStatus created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling contractStatus created domain event..");
    }
}


