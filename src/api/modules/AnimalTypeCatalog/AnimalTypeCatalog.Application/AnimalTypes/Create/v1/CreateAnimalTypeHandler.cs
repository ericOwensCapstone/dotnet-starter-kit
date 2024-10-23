using FSH.Framework.Core.Persistence;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Create.v1;
public sealed class CreateAnimalTypeHandler(
    ILogger<CreateAnimalTypeHandler> logger,
    [FromKeyedServices("animalTypecatalog:animalTypes")] IRepository<AnimalType> repository)
    : IRequestHandler<CreateAnimalTypeCommand, CreateAnimalTypeResponse>
{
    public async Task<CreateAnimalTypeResponse> Handle(CreateAnimalTypeCommand request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var animalType = AnimalType.Create
        (
            request.Name!, 
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
        await repository.AddAsync(animalType, cancellationToken);
        logger.LogInformation("animalType created {AnimalTypeId}", animalType.Id);
        return new CreateAnimalTypeResponse(animalType.Id);
    }
}


