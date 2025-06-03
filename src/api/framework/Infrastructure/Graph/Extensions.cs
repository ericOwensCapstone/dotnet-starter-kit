using FSH.Framework.Infrastructure.Graph.Options;
using FSH.Framework.Infrastructure.Graph.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FSH.Framework.Infrastructure.Graph;

internal static class Extensions
{
    internal static IServiceCollection AddGraphServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<GraphApiOptions>(configuration.GetSection(GraphApiOptions.SectionName));
        services.AddScoped<IGraphService, GraphService>();
        
        return services;
    }
}