using MediatR;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Update.v1;
public sealed record UpdateAnimalTypeCommand
(
    Guid Id,
    double FcrMean,
    double FcrStdDev,
    double DiseaseIncidenceMean,
    double DiseaseIncidenceStdDev,
    double CarcassYieldMean,
    double CarcassYieldStdDev,
    double QualityGradeMean,
    double QualityGradeStdDev,
    double LowerCriticalTemp,
    double UpperCriticalTemp,
    double ArrivalHeadCountMean,
    double ArrivalHeadCountStdDev,
    double ArrivalWeightMean,
    double ArrivalWeightStdDev,
    double ArrivalCostPerCwtMean,
    double ArrivalCostPerCwtStdDev,
    string? Name,
    string? Description = null
) : IRequest<UpdateAnimalTypeResponse>;


