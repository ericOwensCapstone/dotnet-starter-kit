using FSH.Framework.Core.Identity.Invitations.Features.SearchInvitations;
using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.Shared.Authorization;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Framework.Infrastructure.Identity.Invitations.Endpoints;

public static class SearchInvitationsEndpoint
{
    internal static RouteHandlerBuilder MapSearchInvitationsEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints.MapPost("/search", async (ISender mediator, SearchInvitationsQuery query) =>
            {
                var response = await mediator.Send(query);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchInvitationsEndpoint))
            .WithSummary("search invitations")
            .WithDescription("search invitations with pagination and filtering support")
            .Produces<PagedList<InvitationDto>>()
            .RequirePermission(FshPermission.NameFor(FshActions.View, FshResources.UserInvitations));
    }
}