using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContractStatuses.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContractStatuses;
public static class UpdateHarvestContractStatusEndpoint
{
    internal static RouteHandlerBuilder MapHarvestContractStatusUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateHarvestContractStatusCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateHarvestContractStatusEndpoint))
            .WithSummary("update a harvestContractStatus")
            .WithDescription("update a harvestContractStatus")
            .Produces<UpdateHarvestContractStatusResponse>()
            .RequirePermission("Permissions.HarvestContractStatuses.Update")
            .MapToApiVersion(1);
    }
}

