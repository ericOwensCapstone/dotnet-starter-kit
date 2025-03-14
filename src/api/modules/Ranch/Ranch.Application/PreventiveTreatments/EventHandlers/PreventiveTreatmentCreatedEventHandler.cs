using FSH.Starter.WebApi.Ranch.Domain.PreventiveTreatments.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.EventHandlers;

public class PreventiveTreatmentCreatedEventHandler(ILogger<PreventiveTreatmentCreatedEventHandler> logger) : INotificationHandler<PreventiveTreatmentCreated>
{
    public async Task Handle(PreventiveTreatmentCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling preventiveTreatment created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling preventiveTreatment created domain event..");
    }
}


