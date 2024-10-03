using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.EventHandlers;

public class PreventativeTreatmentCreatedEventHandler(ILogger<PreventativeTreatmentCreatedEventHandler> logger) : INotificationHandler<PreventativeTreatmentCreated>
{
    public async Task Handle(PreventativeTreatmentCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling preventativeTreatment created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling preventativeTreatment created domain event..");
    }
}


