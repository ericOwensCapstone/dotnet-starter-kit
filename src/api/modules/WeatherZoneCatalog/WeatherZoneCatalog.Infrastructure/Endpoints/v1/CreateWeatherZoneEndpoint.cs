using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Infrastructure.Endpoints.v1;
public static class CreateWeatherZoneEndpoint
{
    internal static RouteHandlerBuilder MapWeatherZoneCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateWeatherZoneCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateWeatherZoneEndpoint))
            .WithSummary("creates a weatherZone")
            .WithDescription("creates a weatherZone")
            .Produces<CreateWeatherZoneResponse>()
            .RequirePermission("Permissions.WeatherZones.Create")
            .MapToApiVersion(1);
    }
}


