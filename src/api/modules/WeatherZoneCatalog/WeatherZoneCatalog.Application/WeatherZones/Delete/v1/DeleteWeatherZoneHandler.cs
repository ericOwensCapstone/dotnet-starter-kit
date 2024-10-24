using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Delete.v1;
public sealed class DeleteWeatherZoneHandler(
    ILogger<DeleteWeatherZoneHandler> logger,
    [FromKeyedServices("weatherZonecatalog:weatherZones")] IRepository<WeatherZone> repository)
    : IRequestHandler<DeleteWeatherZoneCommand>
{
    public async Task Handle(DeleteWeatherZoneCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var weatherZone = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = weatherZone ?? throw new WeatherZoneNotFoundException(request.Id);
        await repository.DeleteAsync(weatherZone, cancellationToken);
        logger.LogInformation("weatherZone with id : {WeatherZoneId} deleted", weatherZone.Id);
    }
}


