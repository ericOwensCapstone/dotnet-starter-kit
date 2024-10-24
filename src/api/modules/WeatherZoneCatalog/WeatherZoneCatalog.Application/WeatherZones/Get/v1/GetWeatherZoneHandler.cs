using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Get.v1;
public sealed class GetWeatherZoneHandler(
    [FromKeyedServices("weatherZonecatalog:weatherZones")] IReadRepository<WeatherZone> repository,
    ICacheService cache)
    : IRequestHandler<GetWeatherZoneRequest, WeatherZoneResponse>
{
    public async Task<WeatherZoneResponse> Handle(GetWeatherZoneRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"weatherZone:{request.Id}",
            async () =>
            {
                var weatherZoneItem = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (weatherZoneItem == null) throw new WeatherZoneNotFoundException(request.Id);
                return new WeatherZoneResponse
                (
                    weatherZoneItem.Id, 
                    weatherZoneItem.Name, 
                    weatherZoneItem.Description, 
                    weatherZoneItem.YearlyAverageTemp,
                    weatherZoneItem.TempRange,
                    weatherZoneItem.DeviationPeriod,
                    weatherZoneItem.DeviationAmplitude,
                    weatherZoneItem.PeakTempDate
                );
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}


