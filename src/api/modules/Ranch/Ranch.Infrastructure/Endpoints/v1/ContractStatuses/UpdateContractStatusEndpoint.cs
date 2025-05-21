using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.ContractStatuses;
public static class UpdateContractStatusEndpoint
{
    internal static RouteHandlerBuilder MapContractStatusUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateContractStatusCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateContractStatusEndpoint))
            .WithSummary("update a contractStatus")
            .WithDescription("update a contractStatus")
            .Produces<UpdateContractStatusResponse>()
            .RequirePermission("Permissions.ContractStatuses.Update")
            .MapToApiVersion(1);
    }
}

