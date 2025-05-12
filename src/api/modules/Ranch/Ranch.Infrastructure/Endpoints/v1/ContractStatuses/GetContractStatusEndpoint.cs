using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.ContractStatuses;
public static class GetContractStatusEndpoint
{
    internal static RouteHandlerBuilder MapGetContractStatusEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetContractStatusRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetContractStatusEndpoint))
            .WithSummary("gets contractStatus by id")
            .WithDescription("gets prodct by id")
            .Produces<ContractStatusResponse>()
            .RequirePermission("Permissions.ContractStatuses.View")
            .MapToApiVersion(1);
    }
}

