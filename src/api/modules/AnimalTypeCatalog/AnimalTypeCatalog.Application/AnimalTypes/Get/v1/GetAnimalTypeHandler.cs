using Microsoft.Extensions.DependencyInjection;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain.Exceptions;
using FSH.Framework.Core.Persistence;
using FSH.Framework.Core.Caching;
using FSH.Starter.WebApi.AnimalTypeCatalog.Domain;
using MediatR;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Get.v1;
public sealed class GetAnimalTypeHandler(
    [FromKeyedServices("animalTypecatalog:animalTypes")] IReadRepository<AnimalType> repository,
    ICacheService cache)
    : IRequestHandler<GetAnimalTypeRequest, AnimalTypeResponse>
{
    public async Task<AnimalTypeResponse> Handle(GetAnimalTypeRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var item = await cache.GetOrSetAsync(
            $"animalType:{request.Id}",
            async () =>
            {
                var animalTypeItem = await repository.GetByIdAsync(request.Id, cancellationToken);
                if (animalTypeItem == null) throw new AnimalTypeNotFoundException(request.Id);
                return new AnimalTypeResponse
                (
                    animalTypeItem.Id, 
                    animalTypeItem.Name, 
                    animalTypeItem.Description,
                    animalTypeItem.FcrMean,
                    animalTypeItem.FcrStdDev,
                    animalTypeItem.DiseaseIncidenceMean,
                    animalTypeItem.DiseaseIncidenceStdDev,
                    animalTypeItem.CarcassYieldMean,
                    animalTypeItem.CarcassYieldStdDev,
                    animalTypeItem.QualityGradeMean,
                    animalTypeItem.QualityGradeStdDev,
                    animalTypeItem.LowerCriticalTemp,
                    animalTypeItem.UpperCriticalTemp,
                    animalTypeItem.ArrivalHeadCountMean,
                    animalTypeItem.ArrivalHeadCountStdDev,
                    animalTypeItem.ArrivalWeightMean,
                    animalTypeItem.ArrivalWeightStdDev,
                    animalTypeItem.ArrivalCostPerCwtMean,
                    animalTypeItem.ArrivalCostPerCwtStdDev
                );
            },
            cancellationToken: cancellationToken);
        return item!;
    }
}


