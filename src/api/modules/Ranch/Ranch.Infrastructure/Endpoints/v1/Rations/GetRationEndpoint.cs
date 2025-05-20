using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.Rations.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Rations;
public static class GetRationEndpoint
{
    internal static RouteHandlerBuilder MapGetRationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetRationRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetRationEndpoint))
            .WithSummary("gets ration by id")
            .WithDescription("gets ration by id")
            .Produces<RationResponse>()
            .RequirePermission("Permissions.Rations.Search")
            .MapToApiVersion(1);
    }
}

