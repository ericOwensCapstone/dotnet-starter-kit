using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Infrastructure.Endpoints.v1;
public static class UpdateWeatherZoneEndpoint
{
    internal static RouteHandlerBuilder MapWeatherZoneUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateWeatherZoneCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateWeatherZoneEndpoint))
            .WithSummary("update a weatherZone")
            .WithDescription("update a weatherZone")
            .Produces<UpdateWeatherZoneResponse>()
            .RequirePermission("Permissions.WeatherZones.Update")
            .MapToApiVersion(1);
    }
}


