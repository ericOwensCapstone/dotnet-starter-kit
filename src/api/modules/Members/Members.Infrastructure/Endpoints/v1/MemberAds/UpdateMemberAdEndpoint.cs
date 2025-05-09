using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Members.Application.MemberAds.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Members.Infrastructure.Endpoints.v1.MemberAds;
public static class UpdateMemberAdEndpoint
{
    internal static RouteHandlerBuilder MapMemberAdUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateMemberAdCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateMemberAdEndpoint))
            .WithSummary("update a memberAd")
            .WithDescription("update a memberAd")
            .Produces<UpdateMemberAdResponse>()
            .RequirePermission("Permissions.MemberAds.Update")
            .MapToApiVersion(1);
    }
}

