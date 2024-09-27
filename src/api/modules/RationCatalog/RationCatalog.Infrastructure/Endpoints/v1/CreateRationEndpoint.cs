using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.RationCatalog.Infrastructure.Endpoints.v1;
public static class CreateRationEndpoint
{
    internal static RouteHandlerBuilder MapRationCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateRationCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateRationEndpoint))
            .WithSummary("creates a ration")
            .WithDescription("creates a ration")
            .Produces<CreateRationResponse>()
            .RequirePermission("Permissions.Rations.Create")
            .MapToApiVersion(1);
    }
}

