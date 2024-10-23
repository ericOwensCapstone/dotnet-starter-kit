using FSH.Framework.Core.Paging;
using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Get.v1;
using FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Search.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Infrastructure.Endpoints.v1;

public static class SearchAnimalTypesEndpoint
{
    internal static RouteHandlerBuilder MapGetAnimalTypeListEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/search", async (ISender mediator, [FromBody] PaginationFilter filter) =>
            {
                var response = await mediator.Send(new SearchAnimalTypesCommand(filter));
                return Results.Ok(response);
            })
            .WithName(nameof(SearchAnimalTypesEndpoint))
            .WithSummary("Gets a list of animalTypes")
            .WithDescription("Gets a list of animalTypes with pagination and filtering support")
            .Produces<PagedList<AnimalTypeResponse>>()
            .RequirePermission("Permissions.AnimalTypes.View")
            .MapToApiVersion(1);
    }
}



