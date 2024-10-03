using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Get.v1;
using FSH.Starter.WebApi.PreventativeTreatmentCatalog.Application.PreventativeTreatments.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.PreventativeTreatmentCatalog.Infrastructure.Endpoints.v1;

public static class SearchPreventativeTreatmentsEndpoint
{
    internal static RouteHandlerBuilder MapGetPreventativeTreatmentListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] PaginationFilter filter) =>
            {
                var response = await mediator.Send(new SearchPreventativeTreatmentsCommand(filter));
                return Results.Ok(response);
            })
            .WithName(nameof(SearchPreventativeTreatmentsEndpoint))
            .WithSummary("Gets a list of preventativeTreatments")
            .WithDescription("Gets a list of preventativeTreatments with pagination and filtering support")
            .Produces<PagedList<PreventativeTreatmentResponse>>()
            .RequirePermission("Permissions.PreventativeTreatments.View")
            .MapToApiVersion(1);
    }
}


