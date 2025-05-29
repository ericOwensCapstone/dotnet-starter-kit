using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Auth.ApiKey.Endpoints;

internal static class Extensions
{
    public static IEndpointRouteBuilder MapApiKeyEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapCreateApiKeyEndpoint();
        endpoints.MapGetApiKeysEndpoint();
        endpoints.MapDeactivateApiKeyEndpoint();
        return endpoints;
    }
}