using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Members.Application.MemberAds.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Members.Infrastructure.Endpoints.v1.MemberAds;
public static class CreateMemberAdEndpoint
{
    internal static RouteHandlerBuilder MapMemberAdCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateMemberAdCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateMemberAdEndpoint))
            .WithSummary("creates a memberAd")
            .WithDescription("creates a memberAd")
            .Produces<CreateMemberAdResponse>()
            .RequirePermission("Permissions.MemberAds.Create")
            .MapToApiVersion(1);
    }
}

