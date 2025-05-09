using FSH.Starter.WebApi.Members.Domain.MemberAds.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Members.Application.MemberAds.EventHandlers;

public class MemberAdCreatedEventHandler(ILogger<MemberAdCreatedEventHandler> logger) : INotificationHandler<MemberAdCreated>
{
    public async Task Handle(MemberAdCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling memberAd created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling memberAd created domain event..");
    }
}


