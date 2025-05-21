using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.MemberPages.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.MemberPages;
public static class DeleteMemberPageEndpoint
{
    internal static RouteHandlerBuilder MapMemberPageDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteMemberPageCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteMemberPageEndpoint))
            .WithSummary("deletes memberPage by id")
            .WithDescription("deletes memberPage by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.MemberPages.Delete")
            .MapToApiVersion(1);
    }
}

