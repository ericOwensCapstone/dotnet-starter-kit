using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.WeatherZoneCatalog.Application.WeatherZones.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Infrastructure.Endpoints.v1;
public static class DeleteWeatherZoneEndpoint
{
    internal static RouteHandlerBuilder MapWeatherZoneDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteWeatherZoneCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteWeatherZoneEndpoint))
            .WithSummary("deletes weatherZone by id")
            .WithDescription("deletes weatherZone by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.WeatherZones.Delete")
            .MapToApiVersion(1);
    }
}


