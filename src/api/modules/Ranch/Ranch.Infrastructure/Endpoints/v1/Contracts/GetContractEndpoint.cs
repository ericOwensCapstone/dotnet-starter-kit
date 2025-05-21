using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.Contracts.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Contracts;
public static class GetContractEndpoint
{
    internal static RouteHandlerBuilder MapGetContractEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetContractRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetContractEndpoint))
            .WithSummary("gets contract by id")
            .WithDescription("gets contract by id")
            .Produces<ContractResponse>()
            .RequirePermission("Permissions.Contracts.Search")
            .MapToApiVersion(1);
    }
}

