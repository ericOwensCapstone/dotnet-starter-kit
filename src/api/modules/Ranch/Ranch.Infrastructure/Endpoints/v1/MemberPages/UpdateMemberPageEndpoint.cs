using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.MemberPages.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.MemberPages;
public static class UpdateMemberPageEndpoint
{
    internal static RouteHandlerBuilder MapMemberPageUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateMemberPageCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateMemberPageEndpoint))
            .WithSummary("update a memberPage")
            .WithDescription("update a memberPage")
            .Produces<UpdateMemberPageResponse>()
            .RequirePermission("Permissions.MemberPages.Update")
            .MapToApiVersion(1);
    }
}

