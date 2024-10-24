namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Get.v1;
public sealed record WeatherZoneResponse
(
    Guid? Id, 
    string Name, 
    string? Description,
    double YearlyAverageTemp,
    double TempRange,
    double DeviationPeriod,
    double DeviationAmplitude,
    DateTime PeakTempDate
);


