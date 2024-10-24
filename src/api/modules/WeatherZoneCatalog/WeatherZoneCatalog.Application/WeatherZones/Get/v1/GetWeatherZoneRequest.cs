using MediatR;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Get.v1;
public class GetWeatherZoneRequest : IRequest<WeatherZoneResponse>
{
    public Guid Id { get; set; }
    public GetWeatherZoneRequest(Guid id) => Id = id;
}


