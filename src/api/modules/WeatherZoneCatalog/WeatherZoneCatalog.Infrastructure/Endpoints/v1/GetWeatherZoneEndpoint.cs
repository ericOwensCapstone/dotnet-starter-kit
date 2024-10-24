using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Infrastructure.Endpoints.v1;
public static class GetWeatherZoneEndpoint
{
    internal static RouteHandlerBuilder MapGetWeatherZoneEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetWeatherZoneRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetWeatherZoneEndpoint))
            .WithSummary("gets weatherZone by id")
            .WithDescription("gets prodct by id")
            .Produces<WeatherZoneResponse>()
            .RequirePermission("Permissions.WeatherZones.View")
            .MapToApiVersion(1);
    }
}


