using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestContracts.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestContracts;
public static class UpdateHarvestContractEndpoint
{
    internal static RouteHandlerBuilder MapHarvestContractUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateHarvestContractCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateHarvestContractEndpoint))
            .WithSummary("update a harvestContract")
            .WithDescription("update a harvestContract")
            .Produces<UpdateHarvestContractResponse>()
            .RequirePermission("Permissions.HarvestContracts.Update")
            .MapToApiVersion(1);
    }
}

