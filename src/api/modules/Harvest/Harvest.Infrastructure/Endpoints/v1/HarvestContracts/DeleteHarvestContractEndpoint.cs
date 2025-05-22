using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContracts;
public static class DeleteHarvestContractEndpoint
{
    internal static RouteHandlerBuilder MapHarvestContractDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteHarvestContractCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteHarvestContractEndpoint))
            .WithSummary("deletes harvestContract by id")
            .WithDescription("deletes harvestContract by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.HarvestContracts.Delete")
            .MapToApiVersion(1);
    }
}

