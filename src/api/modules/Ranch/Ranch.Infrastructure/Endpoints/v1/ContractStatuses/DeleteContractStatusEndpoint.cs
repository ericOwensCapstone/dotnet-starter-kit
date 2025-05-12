using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.ContractStatuses;
public static class DeleteContractStatusEndpoint
{
    internal static RouteHandlerBuilder MapContractStatusDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteContractStatusCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteContractStatusEndpoint))
            .WithSummary("deletes contractStatus by id")
            .WithDescription("deletes contractStatus by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.ContractStatuses.Delete")
            .MapToApiVersion(1);
    }
}

