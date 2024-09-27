using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.RationCatalog.Application.Rations.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.RationCatalog.Infrastructure.Endpoints.v1;
public static class UpdateRationEndpoint
{
    internal static RouteHandlerBuilder MapRationUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateRationCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateRationEndpoint))
            .WithSummary("update a ration")
            .WithDescription("update a ration")
            .Produces<UpdateRationResponse>()
            .RequirePermission("Permissions.Rations.Update")
            .MapToApiVersion(1);
    }
}

