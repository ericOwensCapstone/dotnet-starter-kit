using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Update.v1;
public sealed class UpdateAnimalTypeHandler(
    ILogger<UpdateAnimalTypeHandler> logger,
    [FromKeyedServices("animalTypecatalog:animalTypes")] IRepository<AnimalType> repository)
    : IRequestHandler<UpdateAnimalTypeCommand, UpdateAnimalTypeResponse>
{
    public async Task<UpdateAnimalTypeResponse> Handle(UpdateAnimalTypeCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var animalType = await repository.GetByIdAsync(request.Id, cancellationToken);
        _ = animalType ?? throw new AnimalTypeNotFoundException(request.Id);
        var updatedAnimalType = animalType.Update
        (
            request.Name, 
            request.Description,
            request.FcrMean,
            request.FcrStdDev,
            request.DiseaseIncidenceMean,
            request.DiseaseIncidenceStdDev,
            request.CarcassYieldMean,
            request.CarcassYieldStdDev,
            request.QualityGradeMean,
            request.QualityGradeStdDev,
            request.LowerCriticalTemp,
            request.UpperCriticalTemp,
            request.ArrivalHeadCountMean,
            request.ArrivalHeadCountStdDev,
            request.ArrivalWeightMean,
            request.ArrivalWeightStdDev,
            request.ArrivalCostPerCwtMean,
            request.ArrivalCostPerCwtStdDev
        );
        await repository.UpdateAsync(updatedAnimalType, cancellationToken);
        logger.LogInformation("animalType with id : {AnimalTypeId} updated.", animalType.Id);
        return new UpdateAnimalTypeResponse(animalType.Id);
    }
}


