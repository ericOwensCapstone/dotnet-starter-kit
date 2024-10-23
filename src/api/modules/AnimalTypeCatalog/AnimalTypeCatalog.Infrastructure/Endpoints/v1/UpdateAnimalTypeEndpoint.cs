using FSH.Framework.Infrastructure.Auth.Policy;
using FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Update.v1;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Infrastructure.Endpoints.v1;
public static class UpdateAnimalTypeEndpoint
{
    internal static RouteHandlerBuilder MapAnimalTypeUpdateEndpoint(this IEndpointRouteBuilder endpoints)
    {
        return endpoints
            .MapPut("/{id:guid}", async (Guid id, UpdateAnimalTypeCommand request, ISender mediator) =>
            {
                if (id != request.Id) return Results.BadRequest();
                var response = await mediator.Send(request);
                return Results.Ok(response);
            })
            .WithName(nameof(UpdateAnimalTypeEndpoint))
            .WithSummary("update a animalType")
            .WithDescription("update a animalType")
            .Produces<UpdateAnimalTypeResponse>()
            .RequirePermission("Permissions.AnimalTypes.Update")
            .MapToApiVersion(1);
    }
}


