using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Get.v1;
using FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Infrastructure.Endpoints.v1;

public static class SearchWeatherZonesEndpoint
{
    internal static RouteHandlerBuilder MapGetWeatherZoneListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] PaginationFilter filter) =>
            {
                var response = await mediator.Send(new SearchWeatherZonesCommand(filter));
                return Results.Ok(response);
            })
            .WithName(nameof(SearchWeatherZonesEndpoint))
            .WithSummary("Gets a list of weatherZones")
            .WithDescription("Gets a list of weatherZones with pagination and filtering support")
            .Produces<PagedList<WeatherZoneResponse>>()
            .RequirePermission("Permissions.WeatherZones.View")
            .MapToApiVersion(1);
    }
}



