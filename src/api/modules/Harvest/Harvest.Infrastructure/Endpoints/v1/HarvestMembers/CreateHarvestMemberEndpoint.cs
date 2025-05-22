using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Harvest.Application.HarvestMembers.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Harvest.Infrastructure.Endpoints.v1.HarvestMembers;
public static class CreateHarvestMemberEndpoint
{
    internal static RouteHandlerBuilder MapHarvestMemberCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateHarvestMemberCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateHarvestMemberEndpoint))
            .WithSummary("creates a harvestMember")
            .WithDescription("creates a harvestMember")
            .Produces<CreateHarvestMemberResponse>()
            .RequirePermission("Permissions.HarvestMembers.Create")
            .MapToApiVersion(1);
    }
}

