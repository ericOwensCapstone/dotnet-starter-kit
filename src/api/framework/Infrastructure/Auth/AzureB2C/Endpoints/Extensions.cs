using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Framework.Infrastructure.Auth.AzureB2C.Endpoints;

internal static class Extensions
{
    public static IEndpointRouteBuilder MapB2CEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var loggerFactory = endpoints.ServiceProvider.GetService<ILoggerFactory>();
        var logger = loggerFactory?.CreateLogger("B2CEndpointMapping");
        
        logger?.LogInformation("=== STARTING B2C ENDPOINTS MAPPING ===");
        
        logger?.LogInformation("Mapping B2CTokenEndpoint...");
        endpoints.MapB2CTokenEndpoint();
        
        logger?.LogInformation("Mapping PublicB2CTokenEndpoint...");
        endpoints.MapPublicB2CTokenEndpoint();
        
        logger?.LogInformation("Mapping B2CValidateInvitationEndpoint...");
        endpoints.MapB2CValidateInvitationEndpoint();
        logger?.LogInformation("B2CValidateInvitationEndpoint mapping completed");
        
        logger?.LogInformation("Mapping B2CPreRegistrationValidationEndpoint...");
        endpoints.MapB2CPreRegistrationValidationEndpoint();
        
        logger?.LogInformation("Mapping B2CPostRegistrationEndpoint...");
        endpoints.MapB2CPostRegistrationEndpoint();
        
        logger?.LogInformation("Mapping B2CTestEndpoint...");
        endpoints.MapB2CTestEndpoint();
        
        logger?.LogInformation("=== B2C ENDPOINTS MAPPING COMPLETE ===");
        
        return endpoints;
    }
}