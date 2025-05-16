using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.Contracts.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Contracts;
public static class UpdateContractEndpoint
{
    internal static RouteHandlerBuilder MapContractUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateContractCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateContractEndpoint))
            .WithSummary("update a contract")
            .WithDescription("update a contract")
            .Produces<UpdateContractResponse>()
            .RequirePermission("Permissions.Contracts.Update")
            .MapToApiVersion(1);
    }
}

