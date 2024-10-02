using FSH.Starter.WebApi.GrowthTreatmentCatalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.GrowthTreatmentCatalog.Application.GrowthTreatments.EventHandlers;

public class GrowthTreatmentCreatedEventHandler(ILogger<GrowthTreatmentCreatedEventHandler> logger) : INotificationHandler<GrowthTreatmentCreated>
{
    public async Task Handle(GrowthTreatmentCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling growthTreatment created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling growthTreatment created domain event..");
    }
}


