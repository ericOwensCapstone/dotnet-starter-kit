using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Members.Application.MemberAds.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Members.Infrastructure.Endpoints.v1.MemberAds;
public static class DeleteMemberAdEndpoint
{
    internal static RouteHandlerBuilder MapMemberAdDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteMemberAdCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteMemberAdEndpoint))
            .WithSummary("deletes memberAd by id")
            .WithDescription("deletes memberAd by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.MemberAds.Delete")
            .MapToApiVersion(1);
    }
}

