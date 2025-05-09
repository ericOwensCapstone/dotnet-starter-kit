using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Members.Application.MemberAds.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Members.Infrastructure.Endpoints.v1.MemberAds;
public static class GetMemberAdEndpoint
{
    internal static RouteHandlerBuilder MapGetMemberAdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetMemberAdRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetMemberAdEndpoint))
            .WithSummary("gets memberAd by id")
            .WithDescription("gets prodct by id")
            .Produces<MemberAdResponse>()
            .RequirePermission("Permissions.MemberAds.View")
            .MapToApiVersion(1);
    }
}

