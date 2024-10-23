using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Get.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Infrastructure.Endpoints.v1;
public static class GetAnimalTypeEndpoint
{
    internal static RouteHandlerBuilder MapGetAnimalTypeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapGet("/{id:guid}", async (Guid id, ISender mediator) =>
            {
                var response = await mediator.Send(new GetAnimalTypeRequest(id));
                return Results.Ok(response);
            })
            .WithName(nameof(GetAnimalTypeEndpoint))
            .WithSummary("gets animalType by id")
            .WithDescription("gets prodct by id")
            .Produces<AnimalTypeResponse>()
            .RequirePermission("Permissions.AnimalTypes.View")
            .MapToApiVersion(1);
    }
}


