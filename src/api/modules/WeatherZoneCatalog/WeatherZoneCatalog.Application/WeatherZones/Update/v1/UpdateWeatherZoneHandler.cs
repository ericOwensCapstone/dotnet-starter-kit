using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Update.v1;
public sealed class UpdateWeatherZoneHandler(
    ILogger<UpdateWeatherZoneHandler> logger,
    [FromKeyedServices("weatherZonecatalog:weatherZones")] IRepository<WeatherZone> repository)
    : IRequestHandler<UpdateWeatherZoneCommand, UpdateWeatherZoneResponse>
{
    public async Task<UpdateWeatherZoneResponse> Handle(UpdateWeatherZoneCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var weatherZone = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = weatherZone ?? throw new WeatherZoneNotFoundException(request.Id);
        var updatedWeatherZone = weatherZone.Update
        (
            request.Name, 
            request.Description, 
            request.YearlyAverageTemp,
            request.TempRange,
            request.DeviationPeriod,
            request.DeviationAmplitude,
            request.PeakTempDate
        );
        await repository.UpdateAsync(updatedWeatherZone, cancellationToken);
        logger.LogInformation("weatherZone with id : {WeatherZoneId} updated.", weatherZone.Id);
        return new UpdateWeatherZoneResponse(weatherZone.Id);
    }
}


