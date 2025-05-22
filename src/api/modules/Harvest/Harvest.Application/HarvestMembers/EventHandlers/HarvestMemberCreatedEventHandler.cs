using FSH.Starter.WebApi.Harvest.Domain.HarvestMembers.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Harvest.Application.HarvestMembers.EventHandlers;

public class HarvestMemberCreatedEventHandler(ILogger<HarvestMemberCreatedEventHandler> logger) : INotificationHandler<HarvestMemberCreated>
{
    public async Task Handle(HarvestMemberCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling harvestMember created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling harvestMember created domain event..");
    }
}


