using FSH.Starter.WebApi.WeatherZoneCatalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.EventHandlers;

public class WeatherZoneCreatedEventHandler(ILogger<WeatherZoneCreatedEventHandler> logger) : INotificationHandler<WeatherZoneCreated>
{
    public async Task Handle(WeatherZoneCreated notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("handling weatherZone created domain event..");
        await Task.FromResult(notification);
        logger.LogInformation("finished handling weatherZone created domain event..");
    }
}



