using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.ContractStatuses.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.ContractStatuses;
public static class CreateContractStatusEndpoint
{
    internal static RouteHandlerBuilder MapContractStatusCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateContractStatusCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateContractStatusEndpoint))
            .WithSummary("creates a contractStatus")
            .WithDescription("creates a contractStatus")
            .Produces<CreateContractStatusResponse>()
            .RequirePermission("Permissions.ContractStatuses.Create")
            .MapToApiVersion(1);
    }
}

