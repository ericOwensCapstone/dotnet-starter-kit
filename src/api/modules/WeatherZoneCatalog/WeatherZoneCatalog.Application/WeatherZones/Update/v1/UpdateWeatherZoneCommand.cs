using MediatR;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Update.v1;
public sealed record UpdateWeatherZoneCommand
(
    Guid Id,
    double YearlyAverageTemp,
    double TempRange,
    double DeviationPeriod,
    double DeviationAmplitude,
    DateTime PeakTempDate,
    string? Name,
    string? Description = null
) : IRequest<UpdateWeatherZoneResponse>;


