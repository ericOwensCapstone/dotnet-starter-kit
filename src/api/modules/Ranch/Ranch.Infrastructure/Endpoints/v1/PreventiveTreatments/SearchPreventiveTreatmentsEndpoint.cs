using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Get.v1;
using FSH.Starter.WebApi.Ranch.Application.PreventiveTreatments.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.PreventiveTreatments;

public static class SearchPreventiveTreatmentsEndpoint
{
    internal static RouteHandlerBuilder MapGetPreventiveTreatmentListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchPreventiveTreatmentsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchPreventiveTreatmentsEndpoint))
            .WithSummary("Gets a list of preventiveTreatments")
            .WithDescription("Gets a list of preventiveTreatments with pagination and filtering support")
            .Produces<PagedList<PreventiveTreatmentResponse>>()
            .RequirePermission("Permissions.PreventiveTreatments.View")
            .MapToApiVersion(1);
    }
}


