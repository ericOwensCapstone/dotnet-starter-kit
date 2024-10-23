using MediatR;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Update.v1;
public sealed record UpdateAnimalTypeCommand(
    Guid Id,
    string? Name,
    decimal DollarsPerPound,
    string? Description = null) : IRequest<UpdateAnimalTypeResponse>;


