using System.ComponentModel;
using MediatR;

namespace FSH.Starter.WebApi.AnimalTypeCatalog.Application.AnimalTypes.Create.v1;
public sealed record CreateAnimalTypeCommand
(
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
    [property: DefaultValue("Sample AnimalType")] string? Name,
    [property: DefaultValue("Descriptive Description")] string? Description = null

) : IRequest<CreateAnimalTypeResponse>;


