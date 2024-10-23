using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Domain.Exceptions;
public sealed class AnimalTypeNotFoundException : NotFoundException
{
    public AnimalTypeNotFoundException(Guid id)
        : base($"animalType with id {id} not found")
    {
    }
}


