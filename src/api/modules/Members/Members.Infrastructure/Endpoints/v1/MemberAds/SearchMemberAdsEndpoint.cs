using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Members.Application.MemberAds.Get.v1;
using FSH.Starter.WebApi.Members.Application.MemberAds.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Members.Infrastructure.Endpoints.v1.MemberAds;

public static class SearchMemberAdsEndpoint
{
    internal static RouteHandlerBuilder MapGetMemberAdListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchMemberAdsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchMemberAdsEndpoint))
            .WithSummary("Gets a list of memberAds")
            .WithDescription("Gets a list of memberAds with pagination and filtering support")
            .Produces<PagedList<MemberAdResponse>>()
            .RequirePermission("Permissions.MemberAds.View")
            .MapToApiVersion(1);
    }
}


