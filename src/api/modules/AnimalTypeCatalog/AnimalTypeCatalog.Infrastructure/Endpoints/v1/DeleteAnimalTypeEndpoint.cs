using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Delete.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Infrastructure.Endpoints.v1;
public static class DeleteAnimalTypeEndpoint
{
    internal static RouteHandlerBuilder MapAnimalTypeDeleteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapDelete("/{id:guid}", async (Guid id, ISender mediator) =>
             {
                 await mediator.Send(new DeleteAnimalTypeCommand(id));
                 return Results.NoContent();
             })
            .WithName(nameof(DeleteAnimalTypeEndpoint))
            .WithSummary("deletes animalType by id")
            .WithDescription("deletes animalType by id")
            .Produces(StatusCodes.Status204NoContent)
            .RequirePermission("Permissions.AnimalTypes.Delete")
            .MapToApiVersion(1);
    }
}


