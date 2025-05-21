using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.MemberPages.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.MemberPages;
public static class CreateMemberPageEndpoint
{
    internal static RouteHandlerBuilder MapMemberPageCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateMemberPageCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateMemberPageEndpoint))
            .WithSummary("creates a memberPage")
            .WithDescription("creates a memberPage")
            .Produces<CreateMemberPageResponse>()
            .RequirePermission("Permissions.MemberPages.Create")
            .MapToApiVersion(1);
    }
}

