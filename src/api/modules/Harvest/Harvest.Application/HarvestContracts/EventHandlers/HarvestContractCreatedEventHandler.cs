using FSH.Starter.WebApi.Harvest.Domain.HarvestContracts.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestContracts.EventHandlers;

public class HarvestContractCreatedEventHandler(ILogger<HarvestContractCreatedEventHandler> logger) : INotificationHandler<HarvestContractCreated>
{
    public async Task Handle(HarvestContractCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling harvestContract created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling harvestContract created domain event..");
    }
}


