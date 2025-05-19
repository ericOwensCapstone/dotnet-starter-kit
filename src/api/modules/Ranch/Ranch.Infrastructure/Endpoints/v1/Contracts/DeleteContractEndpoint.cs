using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.Contracts.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Contracts;
public static class DeleteContractEndpoint
{
    internal static RouteHandlerBuilder MapContractDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteContractCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteContractEndpoint))
            .WithSummary("deletes contract by id")
            .WithDescription("deletes contract by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.Contracts.Delete")
            .MapToApiVersion(1);
    }
}

