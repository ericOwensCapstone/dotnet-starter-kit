
using Carter;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Infrastructure.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.Harvest.Infrastructure.Persistence;

namespace FSH.Starter.WebApi.Harvest.Infrastructure;
public static class HarvestModule
{
    public class Endpoints : CarterModule
    {
        public Endpoints() : base("harvest") { }
        public override void AddRoutes(IEndpointRouteBuilder app)
        {

        }
    }
    public static WebApplicationBuilder RegisterHarvestServices(this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.BindDbContext<HarvestDbContext>();
        builder.Services.AddScoped<IDbInitializer, HarvestDbInitializer>();

        return builder;
    }
    public static WebApplication UseHarvestModule(this WebApplication app)
    {
        return app;
    }
}
