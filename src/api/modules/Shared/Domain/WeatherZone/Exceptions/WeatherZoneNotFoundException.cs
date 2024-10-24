using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Domain.Exceptions;
public sealed class WeatherZoneNotFoundException : NotFoundException
{
    public WeatherZoneNotFoundException(Guid id)
        : base($"weatherZone with id {id} not found")
    {
    }
}


