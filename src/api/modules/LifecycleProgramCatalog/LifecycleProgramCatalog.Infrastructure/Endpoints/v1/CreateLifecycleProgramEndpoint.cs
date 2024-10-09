using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Endpoints.v1;
public static class CreateLifecycleProgramEndpoint
{
    internal static RouteHandlerBuilder MapLifecycleProgramCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateLifecycleProgramCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateLifecycleProgramEndpoint))
            .WithSummary("creates a lifecycleProgram")
            .WithDescription("creates a lifecycleProgram")
            .Produces<CreateLifecycleProgramResponse>()
            .RequirePermission("Permissions.LifecyclePrograms.Create")
            .MapToApiVersion(1);
    }
}

