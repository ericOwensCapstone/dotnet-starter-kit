using FSH.Framework.Core.Paging;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Specifications;
using FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Get.v1;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;


namespace FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Search.v1;
public sealed class SearchWeatherZonesHandler(
    [FromKeyedServices("weatherZonecatalog:weatherZones")] IReadRepository<WeatherZone> repository)
    : IRequestHandler<SearchWeatherZonesCommand, PagedList<WeatherZoneResponse>>
{
    public async Task<PagedList<WeatherZoneResponse>> Handle(SearchWeatherZonesCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spec = new EntitiesByPaginationFilterSpec<WeatherZone, WeatherZoneResponse>(request.filter);

        var items = await repository.ListAsync(spec, cancellationToken).ConfigureAwait(false);
        var totalCount = await repository.CountAsync(spec, cancellationToken).ConfigureAwait(false);

        return new PagedList<WeatherZoneResponse>(items, request.filter.PageNumber, request.filter.PageSize, totalCount);
    }
}



