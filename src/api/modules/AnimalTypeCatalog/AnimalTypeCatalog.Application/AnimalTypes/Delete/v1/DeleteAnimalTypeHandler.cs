using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Delete.v1;
public sealed class DeleteAnimalTypeHandler(
    ILogger<DeleteAnimalTypeHandler> logger,
    [FromKeyedServices("animalTypecatalog:animalTypes")] IRepository<AnimalType> repository)
    : IRequestHandler<DeleteAnimalTypeCommand>
{
    public async Task Handle(DeleteAnimalTypeCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var animalType = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = animalType ?? throw new AnimalTypeNotFoundException(request.Id);
        await repository.DeleteAsync(animalType, cancellationToken);
        logger.LogInformation("animalType with id : {AnimalTypeId} deleted", animalType.Id);
    }
}


