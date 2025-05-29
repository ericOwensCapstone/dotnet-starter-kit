using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;

internal static class Extensions
{
    public static IEndpointRouteBuilder MapB2CEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapB2CTokenEndpoint();
        return endpoints;
    }
}