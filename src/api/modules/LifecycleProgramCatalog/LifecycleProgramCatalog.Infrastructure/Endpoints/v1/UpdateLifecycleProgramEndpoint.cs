using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.LifecycleProgramCatalog.Application.LifecyclePrograms.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.LifecycleProgramCatalog.Infrastructure.Endpoints.v1;
public static class UpdateLifecycleProgramEndpoint
{
    internal static RouteHandlerBuilder MapLifecycleProgramUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateLifecycleProgramCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateLifecycleProgramEndpoint))
            .WithSummary("update a lifecycleProgram")
            .WithDescription("update a lifecycleProgram")
            .Produces<UpdateLifecycleProgramResponse>()
            .RequirePermission("Permissions.LifecyclePrograms.Update")
            .MapToApiVersion(1);
    }
}

