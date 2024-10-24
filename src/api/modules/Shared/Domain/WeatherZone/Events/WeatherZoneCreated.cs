using FSH.Framework.Core.Domain.Events;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Domain.Events;
public sealed record WeatherZoneCreated : DomainEvent
{
    public WeatherZone? WeatherZone { get; set; }
}


