using MediatR;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Delete.v1;
public sealed record DeleteWeatherZoneCommand(
    Guid Id) : IRequest;


