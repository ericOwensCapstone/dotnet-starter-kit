using FSH.Framework.Core.Paging;
using FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Get.v1;
using MediatR;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Search.v1;

public record SearchWeatherZonesCommand(PaginationFilter filter) : IRequest<PagedList<WeatherZoneResponse>>;


