using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Create.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Infrastructure.Endpoints.v1;
public static class CreateAnimalTypeEndpoint
{
    internal static RouteHandlerBuilder MapAnimalTypeCreationEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPost("/", async (CreateAnimalTypeCommand request, ISender mediator) =>
            {
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(CreateAnimalTypeEndpoint))
            .WithSummary("creates a animalType")
            .WithDescription("creates a animalType")
            .Produces<CreateAnimalTypeResponse>()
            .RequirePermission("Permissions.AnimalTypes.Create")
            .MapToApiVersion(1);
    }
}


