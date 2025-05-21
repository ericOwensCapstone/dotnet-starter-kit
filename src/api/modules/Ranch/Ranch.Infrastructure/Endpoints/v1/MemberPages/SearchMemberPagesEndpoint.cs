using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.MemberPages.Get.v1;
using FSH.Starter.WebApi.Ranch.Application.MemberPages.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.MemberPages;

public static class SearchMemberPagesEndpoint
{
    internal static RouteHandlerBuilder MapGetMemberPageListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchMemberPagesCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchMemberPagesEndpoint))
            .WithSummary("Gets a list of memberPages")
            .WithDescription("Gets a list of memberPages with pagination and filtering support")
            .Produces<PagedList<MemberPageResponse>>()
            .RequirePermission("Permissions.MemberPages.Search")
            .MapToApiVersion(1);
    }
}


