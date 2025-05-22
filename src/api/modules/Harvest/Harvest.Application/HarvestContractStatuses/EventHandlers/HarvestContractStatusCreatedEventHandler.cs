using FSH.Starter.WebApi.Harvest.Domain.HarvestContractStatuses.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.EventHandlers;

public class HarvestContractStatusCreatedEventHandler(ILogger<HarvestContractStatusCreatedEventHandler> logger) : INotificationHandler<HarvestContractStatusCreated>
{
    public async Task Handle(HarvestContractStatusCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling harvestContractStatus created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling harvestContractStatus created domain event..");
    }
}


