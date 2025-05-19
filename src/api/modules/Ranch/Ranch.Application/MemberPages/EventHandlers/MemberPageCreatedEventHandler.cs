using FSH.Starter.WebApi.Ranch.Domain.MemberPages.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.MemberPages.EventHandlers;

public class MemberPageCreatedEventHandler(ILogger<MemberPageCreatedEventHandler> logger) : INotificationHandler<MemberPageCreated>
{
    public async Task Handle(MemberPageCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling memberPage created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling memberPage created domain event..");
    }
}


