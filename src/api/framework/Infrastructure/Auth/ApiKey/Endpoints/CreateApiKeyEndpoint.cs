using FSH.Framework.Core.Auth.ApiKeys;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Framework.Infrastructure.Identity.Users;
using FSH.Starter.Shared.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Auth.ApiKey.Endpoints;

public static class CreateApiKeyEndpoint
{
    internal static RouteHandlerBuilder MapCreateApiKeyEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/api-keys", async (CreateApiKeyRequest request, IApiKeyService service, CancellationToken ct) =>
            {
                var apiKey = await service.CreateApiKeyAsync(request, ct);
                return Results.Ok(new { ApiKey = apiKey, Message = "Store this API key securely. You won't be able to see it again." });
            })
            .WithName(nameof(CreateApiKeyEndpoint))
            .WithSummary("Create a new API key for integration testing")
            .WithDescription("Creates an API key that can be used for authenticating integration tests")
            .RequirePermission(FshActions.Create, FshResources.Tenants)
            .Produces<object>();
    }
}

public static class GetApiKeysEndpoint
{
    internal static RouteHandlerBuilder MapGetApiKeysEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/api-keys", async (string tenantId, IApiKeyService service, CancellationToken ct) =>
            {
                var keys = await service.GetApiKeysAsync(tenantId, ct);
                return Results.Ok(keys);
            })
            .WithName(nameof(GetApiKeysEndpoint))
            .WithSummary("Get API keys for a tenant")
            .RequirePermission(FshActions.View, FshResources.Tenants)
            .Produces<List<ApiKeyDto>>();
    }
}

public static class DeactivateApiKeyEndpoint
{
    internal static RouteHandlerBuilder MapDeactivateApiKeyEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/api-keys/{id:guid}", async (Guid id, IApiKeyService service, CancellationToken ct) =>
            {
                await service.DeactivateApiKeyAsync(id, ct);
                return Results.NoContent();
            })
            .WithName(nameof(DeactivateApiKeyEndpoint))
            .WithSummary("Deactivate an API key")
            .RequirePermission(FshActions.Delete, FshResources.Tenants)
            .Produces(StatusCodes.Status204NoContent);
    }
}