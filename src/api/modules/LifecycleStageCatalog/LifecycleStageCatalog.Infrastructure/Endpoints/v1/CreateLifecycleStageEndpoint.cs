using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.LifecycleStageCatalog.Application.LifecycleStages.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.LifecycleStageCatalog.Infrastructure.Endpoints.v1;
public static class CreateLifecycleStageEndpoint
{
    internal static RouteHandlerBuilder MapLifecycleStageCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateLifecycleStageCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateLifecycleStageEndpoint))
            .WithSummary("creates a lifecycleStage")
            .WithDescription("creates a lifecycleStage")
            .Produces<CreateLifecycleStageResponse>()
            .RequirePermission("Permissions.LifecycleStages.Create")
            .MapToApiVersion(1);
    }
}

