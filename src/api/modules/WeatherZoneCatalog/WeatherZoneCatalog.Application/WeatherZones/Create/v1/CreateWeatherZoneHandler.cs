using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Create.v1;
public sealed class CreateWeatherZoneHandler(
    ILogger<CreateWeatherZoneHandler> logger,
    [FromKeyedServices("weatherZonecatalog:weatherZones")] IRepository<WeatherZone> repository)
    : IRequestHandler<CreateWeatherZoneCommand, CreateWeatherZoneResponse>
{
    public async Task<CreateWeatherZoneResponse> Handle(CreateWeatherZoneCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var weatherZone = WeatherZone.Create
        (
            request.Name!, 
            request.Description,
            request.YearlyAverageTemp,
            request.TempRange,
            request.DeviationPeriod,
            request.DeviationAmplitude,
            request.PeakTempDate
        );
        await repository.AddAsync(weatherZone, cancellationToken);
        logger.LogInformation("weatherZone created {WeatherZoneId}", weatherZone.Id);
        return new CreateWeatherZoneResponse(weatherZone.Id);
    }
}


