using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContractStatuses;
public static class DeleteHarvestContractStatusEndpoint
{
    internal static RouteHandlerBuilder MapHarvestContractStatusDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteHarvestContractStatusCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteHarvestContractStatusEndpoint))
            .WithSummary("deletes harvestContractStatus by id")
            .WithDescription("deletes harvestContractStatus by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.HarvestContractStatuses.Delete")
            .MapToApiVersion(1);
    }
}

