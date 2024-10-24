using Carter;
using FSH.WebApi.Modules.WeatherZoneCatalog.Features.WeatherZones.WeatherZoneCreation.v1;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.WebApi.Modules.WeatherZoneCatalog;

public static class WeatherZoneCatalogModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("weatherZonecatalog") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            var weatherZoneGroup = app.MapGroup("weatherZones").WithTags("weatherZones");
            weatherZoneGroup.MapWeatherZoneCreationEndpoint();

            var testGroup = app.MapGroup("test").WithTags("test");
            testGroup.MapGet("/test", () => "hi");
        }
    }
    public static WebApplicationBuilder RegisterWeatherZoneCatalogServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder;
    }
    public static WebApplication UseWeatherZoneCatalogModule(this WebApplication app)
    {
        return app;
    }
}


