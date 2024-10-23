using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Create.v1;
public sealed record CreateAnimalTypeCommand(
    [property: DefaultValue("Sample AnimalType")] string? Name,
    [property: DefaultValue(10)] decimal DollarsPerPound,
    [property: DefaultValue("Descriptive Description")] string? Description = null) : IRequest<CreateAnimalTypeResponse>;


