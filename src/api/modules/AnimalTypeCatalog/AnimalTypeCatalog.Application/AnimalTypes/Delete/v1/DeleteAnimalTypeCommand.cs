using MediatR;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Delete.v1;
public sealed record DeleteAnimalTypeCommand(
    Guid Id) : IRequest;


