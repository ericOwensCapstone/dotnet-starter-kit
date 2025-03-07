using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Get.v1;
using FSH.Starter.WebApi.Ranch.Application.GrowthTreatments.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.Ranch.Infrastructure.Endpoints.v1.GrowthTreatments;

public static class SearchGrowthTreatmentsEndpoint
{
    internal static RouteHandlerBuilder MapGetGrowthTreatmentListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] SearchGrowthTreatmentsCommand command) =>
            {
                var response = await mediator.Send(command);
                return Results.Ok(response);
            })
            .WithName(nameof(SearchGrowthTreatmentsEndpoint))
            .WithSummary("Gets a list of growthTreatments")
            .WithDescription("Gets a list of growthTreatments with pagination and filtering support")
            .Produces<PagedList<GrowthTreatmentResponse>>()
            .RequirePermission("Permissions.GrowthTreatments.View")
            .MapToApiVersion(1);
    }
}


