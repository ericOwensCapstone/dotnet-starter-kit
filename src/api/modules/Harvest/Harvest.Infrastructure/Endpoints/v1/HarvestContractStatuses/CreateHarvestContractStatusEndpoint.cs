using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContractStatuses;
public static class CreateHarvestContractStatusEndpoint
{
    internal static RouteHandlerBuilder MapHarvestContractStatusCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateHarvestContractStatusCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateHarvestContractStatusEndpoint))
            .WithSummary("creates a harvestContractStatus")
            .WithDescription("creates a harvestContractStatus")
            .Produces<CreateHarvestContractStatusResponse>()
            .RequirePermission("Permissions.HarvestContractStatuses.Create")
            .MapToApiVersion(1);
    }
}

