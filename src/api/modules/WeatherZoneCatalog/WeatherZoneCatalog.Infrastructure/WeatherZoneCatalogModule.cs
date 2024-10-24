using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using FSH.Starter.WebApi.WeatherZoneCatalog.Domain;
using FSH.Starter.WebApi.WeatherZoneCatalog.Infrastructure.Endpoints.v1;
using FSH.Starter.WebApi.WeatherZoneCatalog.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SharedDbContextProject;

namespace FSH.Starter.WebApi.WeatherZoneCatalog.Infrastructure;
public static class WeatherZoneCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("weatherZonecatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var weatherZoneGroup = app.MapGroup("weatherZones").WithTags("weatherZones");
            weatherZoneGroup.MapWeatherZoneCreationEndpoint();
            weatherZoneGroup.MapGetWeatherZoneEndpoint();
            weatherZoneGroup.MapGetWeatherZoneListEndpoint();
            weatherZoneGroup.MapWeatherZoneUpdateEndpoint();
            weatherZoneGroup.MapWeatherZoneDeleteEndpoint();
        }
    }
    public static WebApplicationBuilder RegisterWeatherZoneCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<SharedDbContext>();
        builder.Services.AddKeyedScoped<IRepository<WeatherZone>, WeatherZoneCatalogRepository<WeatherZone>>("weatherZonecatalog:weatherZones");
        builder.Services.AddKeyedScoped<IReadRepository<WeatherZone>, WeatherZoneCatalogRepository<WeatherZone>>("weatherZonecatalog:weatherZones");
        return builder;
    }
    public static WebApplication UseWeatherZoneCatalogModule(this WebApplication app)
    {
        return app;
    }
}


