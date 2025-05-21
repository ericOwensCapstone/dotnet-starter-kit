using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.Contracts.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.Contracts;
public static class CreateContractEndpoint
{
    internal static RouteHandlerBuilder MapContractCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateContractCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateContractEndpoint))
            .WithSummary("creates a contract")
            .WithDescription("creates a contract")
            .Produces<CreateContractResponse>()
            .RequirePermission("Permissions.Contracts.Create")
            .MapToApiVersion(1);
    }
}

