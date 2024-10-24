using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Create.v1;
public sealed record CreateWeatherZoneCommand
(
    double YearlyAverageTemp,
    double TempRange,
    double DeviationPeriod,
    double DeviationAmplitude,
    DateTime PeakTempDate,
    [property: DefaultValue("Sample WeatherZone")] string? Name,
    [property: DefaultValue("Descriptive Description")] string? Description = null
) : IRequest<CreateWeatherZoneResponse>;


