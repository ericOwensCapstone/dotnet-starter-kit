using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.MemberPages.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.MemberPages;
public static class GetMemberPageEndpoint
{
    internal static RouteHandlerBuilder MapGetMemberPageEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetMemberPageRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetMemberPageEndpoint))
            .WithSummary("gets memberPage by id")
            .WithDescription("gets prodct by id")
            .Produces<MemberPageResponse>()
            .RequirePermission("Permissions.MemberPages.View")
            .MapToApiVersion(1);
    }
}

